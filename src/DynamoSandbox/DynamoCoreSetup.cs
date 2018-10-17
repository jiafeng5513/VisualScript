using System;
using System.Diagnostics;
using System.Windows;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.ViewModels;
using Dynamo.Core;
using Dynamo.DynamoSandbox;

namespace DynamoSandbox
{
    class DynamoCoreSetup
    {
        private DynamoViewModel viewModel = null;


        public DynamoCoreSetup(string[] args){}

        public void RunApplication(Application app)
        {
            try
            {
                var model = Dynamo.Applications.StartupUtils.MakeModel();

                viewModel = DynamoViewModel.Start(
                    new DynamoViewModel.StartConfiguration()
                    {
                        DynamoModel = model,
                        ShowLogin = true
                    });

                var view = new DynamoView(viewModel);
                view.Loaded += OnDynamoViewLoaded;

                app.Run(view);
            }

            catch (Exception e)
            {
                try
                {
#if DEBUG
                    // Display the recorded command XML when the crash happens, 
                    // so that it maybe saved and re-run later
                    if (viewModel != null)
                        viewModel.SaveRecordedCommand.Execute(null);
#endif

                    DynamoModel.IsCrashing = true;
                    //Dynamo.Logging.Analytics.TrackException(e, true);

                    if (viewModel != null)
                    {
                        // Show the unhandled exception dialog so user can copy the 
                        // crash details and report the crash if she chooses to.
                        viewModel.Model.OnRequestsCrashPrompt(null,
                            new CrashPromptArgs(e.Message + "\n\n" + e.StackTrace));

                        // Give user a chance to save (but does not allow cancellation)
                        viewModel.Exit(allowCancel: false);
                    }
                }
                catch
                {
                }

                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }
        /// <summary>
        /// 响应主界面加载完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDynamoViewLoaded(object sender, RoutedEventArgs e)
        {
            //todo:主界面加载完毕回调
        }

    }
}
