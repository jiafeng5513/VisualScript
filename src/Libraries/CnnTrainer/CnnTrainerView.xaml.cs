using System.Windows;
using System.Windows.Controls;
using Dynamo.Controls;

using ModelAnalyzerUI;


namespace Dynamo.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for ExportAsSATControl.xaml
    /// </summary>
    public partial class CnnTrainerView : UserControl
    {
        public CnnTrainerView(CnnTrainerModel Model, NodeView nodeView)
        {
            InitializeComponent();
        }
    }
}
