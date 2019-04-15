using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelegateCommand = Dynamo.UI.Commands.DelegateCommand;
namespace Dynamo.Wpf
{
    partial class CnnTrainerViewModel
    {
        private void InitializeDelegateCommands()
        {
            StartCommand = new DelegateCommand(Start, o => true);
            StopCommand = new DelegateCommand(Stop, o => true);
        }
        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }
    }
}
