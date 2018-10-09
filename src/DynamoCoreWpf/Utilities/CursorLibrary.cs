﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Dynamo.Wpf.Utilities
{
    internal enum CursorSet
    {
        Pointer,
        HandPan,
        HandPanActive,
        ArcSelect,
        ArcAdding,
        ArcRemoving,
        LinkSelect,
        Expand,
        Condense,
        RectangularSelection,
        ResizeHorizontal,
        ResizeVertical,
        ResizeDiagonal,
        DragMove
    }

    internal class CursorLibrary
    {
        private static List<Cursor> cursorInstance;

        #region Static Methods
        public static Cursor GetCursor(CursorSet cursorSet)
        {
            if (cursorInstance == null)
                cursorInstance = LoadCursors();

            return CursorLibrary.cursorInstance[(int)cursorSet];
        }

        private static List<Cursor> LoadCursors()
        {
            Dictionary<CursorSet, string> resources = new Dictionary<CursorSet, string>();
            resources.Add(CursorSet.Pointer,                "pointer.cur");
            resources.Add(CursorSet.HandPan,                "hand_pan.cur");
            resources.Add(CursorSet.HandPanActive,          "hand_pan_active.cur");
            resources.Add(CursorSet.ArcSelect,              "arc_select.cur");
            resources.Add(CursorSet.ArcAdding,              "arc_add.cur");
            resources.Add(CursorSet.ArcRemoving,            "arc_remove.cur");
            resources.Add(CursorSet.LinkSelect,             "hand.cur");
            resources.Add(CursorSet.Expand,                 "expand.cur");
            resources.Add(CursorSet.Condense,               "condense.cur");
            resources.Add(CursorSet.RectangularSelection,   "rectangular_selection.cur");
            resources.Add(CursorSet.ResizeHorizontal,       "resize_horizontal.cur");
            resources.Add(CursorSet.ResizeVertical,         "resize_vertical.cur");
            resources.Add(CursorSet.ResizeDiagonal,         "resize_diagonal.cur");
            resources.Add(CursorSet.DragMove,               "drag_move.cur");

            List<Cursor> cursors = new List<Cursor>();

            foreach (KeyValuePair<CursorSet, string> resource in resources)
            {
                const string baseUri = "pack://application:,,,/DynamoCoreWpf;component/UI/Images/";
                var cursorStream = Application.GetResourceStream(new Uri(baseUri + resource.Value));
                cursors.Add(new Cursor(cursorStream.Stream));
            }

            return cursors;
        }
        #endregion

    }
}
