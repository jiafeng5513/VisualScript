using System.Windows;
using Dynamo.Graph.Nodes;
using System.Diagnostics;
using Dynamo.Configuration;
using System.Windows.Input;
using Dynamo.ViewModels;

namespace Dynamo.Prompts
{
    /// <summary>
    /// Interaction logic for NodeHelpPrompt.xaml
    /// </summary>
    public partial class NodeHelpPrompt : Window
    {
        public NodeHelpPrompt(NodeModel node)
        {
            DataContext = node;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();
            
            if (node.IsCustomFunction)
            {
                // Hide the dictionary link if the node is a custom node
                DynamoDictionaryHeight.Height = new GridLength(0);
            }
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Focus(); //apply focus on the Dynamo window when the Help window is closed
        }
    }
}
