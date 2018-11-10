using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Geometry;

namespace Dynamo.Manipulation
{

    /// <summary>
    /// Translation Gizmo, that handles translation.
    /// </summary>
    class TranslationGizmo : Gizmo
    {

        #region Private members

        /// <summary>
        /// List of axis available for manipulation
        /// </summary>
        private readonly List<Vector> axes = new List<Vector>();

        /// <summary>
        /// List of planes available for manipulation
        /// </summary>
        private readonly List<Plane> planes = new List<Plane>();

        /// <summary>
        /// Scale to draw the gizmo
        /// </summary>
        private double scale = 1.0;

        /// <summary>
        /// Hit Data
        /// </summary>
        private Vector hitAxis = null;
        private Plane hitPlane = null;

        #endregion

        #region public methods and constructors

        /// <summary>
        /// Constructs a linear gizmo, can be moved in one direction only.
        /// </summary>
        /// <param name="manipulator"></param>
        /// <param name="axis1">Axis of freedom</param>
        /// <param name="size">Visual size of the Gizmo</param>
        public TranslationGizmo(NodeManipulator manipulator, Vector axis1, double size)
            : base(manipulator) 
        {
            ReferenceCoordinateSystem = CoordinateSystem.Identity();
            UpdateGeometry(axis1, null, null, size);
        }

        /// <summary>
        /// Construcs a 3D gizmo, can be manipulated in all three directions.
        /// </summary>
        /// <param name="manipulator"></param>
        /// <param name="axis1">First axis of freedom</param>
        /// <param name="axis2">Second axis of freedom</param>
        /// <param name="axis3">Third axis of freedom</param>
        /// <param name="size">Visual size of the Gizmo</param>
        public TranslationGizmo(NodeManipulator manipulator, Vector axis1, Vector axis2, Vector axis3, double size)
            : base(manipulator)
        {
            ReferenceCoordinateSystem = CoordinateSystem.Identity();
            UpdateGeometry(axis1, axis2, axis3, size);
        }

        /// <summary>
        /// Construcs a 3D gizmo, can be manipulated in all three directions.
        /// </summary>
        /// <param name="axis1">First axis of freedom</param>
        /// <param name="axis2">Second axis of freedom</param>
        /// <param name="axis3">Third axis of freedom</param>
        /// <param name="size">Visual size of the Gizmo</param>
        internal void UpdateGeometry(Vector axis1, Vector axis2, Vector axis3, double size)
        {
            if (axis1 == null) throw new ArgumentNullException("axis1");

            //Reset the dataset, but don't reset the cached hitAxis or hitPlane.
            //hitAxis and hitPlane are used to compute the offset for a move.
            axes.Clear();
            planes.Clear();
            
            scale = size;

            axes.Add(axis1);
            if (axis2 != null)
            {
                axes.Add(axis2);
                planes.Add(Plane.ByOriginXAxisYAxis(Origin, axis1, axis2));
            }
            if (axis3 != null)
            {
                axes.Add(axis3);
                if (axis2 != null)
                {
                    planes.Add(Plane.ByOriginXAxisYAxis(Origin, axis2, axis3));
                }
                planes.Add(Plane.ByOriginXAxisYAxis(Origin, axis3, axis1));
            }

            if (axes.Count == 1 && hitAxis != null)
            {
                hitAxis = axes.First();
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private object HitTest(Point source, Vector direction)
        {
            double tolerance = 0.15; //Hit tolerance
            
            using (var ray = GetRayGeometry(source, direction))
            {
                //First hit test for position
                if (ray.DistanceTo(Origin) < tolerance)
                {
                    if (planes.Any())
                    {
                        return planes.First(); //Xy or first available plane is hit
                    }
                    return axes.First(); //xAxis or first axis is hit
                }

                foreach (var plane in planes)
                {
                    // plane needs to be up-to-date at this time with the current value of Origin
                    using (var pt = plane.Intersect(ray).FirstOrDefault() as Point)
                    {
                        if (pt == null) continue;

                        using (var vec = Vector.ByTwoPoints(Origin, pt))
                        {
                            var dot1 = plane.XAxis.Dot(vec);
                            var dot2 = plane.YAxis.Dot(vec);
                            if (dot1 > 0 && dot2 > 0 && dot1 < scale/2 && dot2 < scale/2)
                            {
                                return plane; //specific plane is hit
                            }
                        }
                    }
                }

                foreach (var axis in axes)
                {
                    using (var line = Line.ByStartPointDirectionLength(Origin, axis, scale))
                    {
                        if (line.DistanceTo(ray) < tolerance)
                        {
                            return axis; //specific axis is hit.
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Line GetRayGeometry(Point source, Vector direction)
        {
            double size = ManipulatorOrigin.DistanceTo(source) * 100;
            return Line.ByStartPointDirectionLength(source, direction, size);
        }

        #endregion

        #region interface methods

        /// <summary>
        /// Reference coordinate system for the Gizmo
        /// </summary>
        public CoordinateSystem ReferenceCoordinateSystem { get; set; }

        /// <summary>
        /// Performs hit test on Gizmo to find out hit object. The returned 
        /// hitObject could be either axis vector or a plane.
        /// </summary>
        /// <param name="source">Mouse click source for hit test</param>
        /// <param name="direction">View projection direction</param>
        /// <param name="hitObject">object hit</param>
        /// <returns>True if Gizmo was hit successfully</returns>
        public override bool HitTest(Point source, Vector direction, out object hitObject)
        {
            hitAxis = null;
            hitPlane = null; //reset hit objects
            hitObject = HitTest(source, direction);
            hitAxis = hitObject as Vector;
            if (hitAxis == null)
            {
                hitPlane = hitObject as Plane;
            }

            return hitObject != null;
        }

        /// <summary>
        /// Computes move vector, based on new position of mouse and view direction.
        /// </summary>
        /// <param name="newPosition">New mouse position</param>
        /// <param name="viewDirection">view direction</param>
        /// <returns>Offset vector wrt manipulator origin</returns>
        public override Vector GetOffset(Point newPosition, Vector viewDirection)
        {
            Point hitPoint = Origin;
            using (var ray = GetRayGeometry(newPosition, viewDirection))
            {
                if (hitPlane != null)
                {
                    using (var testPlane = Plane.ByOriginXAxisYAxis(ManipulatorOrigin, hitPlane.XAxis, hitPlane.YAxis))
                    {
                        hitPoint = testPlane.Intersect(ray).FirstOrDefault() as Point;
                    }
                }
                else if (hitAxis != null)
                {
                    using (var axisLine = RayExtensions.ToOriginCenteredLine(ManipulatorOrigin, hitAxis))
                    {
                        hitPoint = axisLine.ClosestPointTo(ray);
                    }
                }
            }
            if (hitPoint == null)
            {
                return Vector.ByCoordinates(0, 0, 0);
            }

            return Vector.ByTwoPoints(ManipulatorOrigin, hitPoint);
        }

        public override void UpdateGizmoGraphics()
        {
            // Update gizmo geometry wrt to current Origin
            var newPlanes = planes.Select(
                plane => Plane.ByOriginXAxisYAxis(Origin, plane.XAxis, plane.YAxis)).ToList();

            planes.Clear();

            planes.AddRange(newPlanes);
        }

        public override void DeleteTransientGraphics()
        {
            
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            axes.ForEach(x => x.Dispose());
            planes.ForEach(x => x.Dispose());

            if(ReferenceCoordinateSystem != null) ReferenceCoordinateSystem.Dispose();

            base.Dispose(disposing);
        }
    }
}
