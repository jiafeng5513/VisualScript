﻿using Dynamo.Utilities;
using Dynamo.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.ViewModel;
using Dynamo.Wpf.Properties;
using Dynamo.Logging;
using Dynamo.Configuration;

namespace Dynamo.UI.Controls
{
    /// <summary>
    /// This class represents the unified data class that is bound to all the 
    /// list boxes on the StartPageView. The bound data item can be handled in 
    /// different ways depending on their ClickAction and ContextData properties.
    /// See "Action" enumeration below for more details of each item sub-type.
    /// </summary>
    /// 
    public class StartPageListItem : NotificationObject
    {
        private ImageSource icon = null;

        public enum Action
        {
            /// <summary>
            /// Indicates a regular command should be invoked if the list view 
            /// item corresponding to this StartPageListItem is clicked. The 
            /// meaning of ContextData will be interpreted in StartPageViewModel
            /// and corresponding action taken as a result.
            /// </summary>
            RegularCommand,

            /// <summary>
            /// Indicates that the StartPageListItem carries a file path. When 
            /// clicked, StartPageViewModel issues a file open command to open 
            /// the file path indicated by ContextData property.
            /// </summary>
            FilePath,

            /// <summary>
            /// Indicates that the StartPageListItem points to an external URL.
            /// When the list view item corresponding to this StartPageListItem
            /// is clicked, StartPageViewModel brings up the default browser and 
            /// navigate to the URL indicated by ContextData property.
            /// </summary>
            ExternalUrl
        }

        internal StartPageListItem(string caption)
        {
            this.Caption = caption;
        }

        internal StartPageListItem(string caption, string iconPath)
        {
            this.Caption = caption;
            this.icon = LoadBitmapImage(iconPath);
        }

        #region Public Class Properties

        public string Caption { get; private set; }
        public string SubScript { get; set; }
        public string ToolTip { get; set; }
        public string ContextData { get; set; }
        public Action ClickAction { get; set; }

        public ImageSource Icon
        {
            get { return this.icon; }
        }

        // Extended (derived) class properties.
        public Visibility IconVisibility
        {
            get { return ((icon == null) ? Visibility.Collapsed : Visibility.Visible); }
        }

        #endregion

        #region Private Class Helper Methods

        private BitmapImage LoadBitmapImage(string iconPath)
        {
            var format = @"pack://application:,,,/DynamoCoreWpf;component/UI/Images/StartPage/{0}";
            iconPath = string.Format(format, iconPath);
            return new BitmapImage(new Uri(iconPath, UriKind.Absolute));
        }

        #endregion
    }

    public class StartPageViewModel : ViewModelBase
    {
        // Static lists that gets created only during creation.
        List<StartPageListItem> fileOperations = new List<StartPageListItem>();
        List<StartPageListItem> communityLinks = new List<StartPageListItem>();
        List<StartPageListItem> contributeLinks = new List<StartPageListItem>();

        // Dynamic lists that update views on the fly.
        ObservableCollection<StartPageListItem> recentFiles = null;
        internal readonly DynamoViewModel DynamoViewModel;
        private readonly bool isFirstRun;

        internal StartPageViewModel(DynamoViewModel dynamoViewModel, bool isFirstRun)
        {
            this.DynamoViewModel = dynamoViewModel;
            this.isFirstRun = isFirstRun;

            this.recentFiles = new ObservableCollection<StartPageListItem>();


            #region File Operations

            fileOperations.Add(new StartPageListItem(Resources.StartPageNewFile, "icon-new.png")
            {
                ContextData = ButtonNames.NewWorkspace,
                ClickAction = StartPageListItem.Action.RegularCommand
            });

            fileOperations.Add(new StartPageListItem(Resources.StartPageNewCustomNode, "icon-customnode.png")
            {
                ContextData = ButtonNames.NewCustomNodeWorkspace,
                ClickAction = StartPageListItem.Action.RegularCommand
            });

            fileOperations.Add(new StartPageListItem(Resources.StartPageOpenFile, "icon-open.png")
            {
                ContextData = ButtonNames.OpenWorkspace,
                ClickAction = StartPageListItem.Action.RegularCommand
            });

            #endregion

            #region Community Links

            communityLinks.Add(new StartPageListItem(Resources.StartPageDiscussionForum, "icon-discussion.png")
            {
                ContextData = Configurations.DynamoBimForum,
                ClickAction = StartPageListItem.Action.ExternalUrl
            });

            communityLinks.Add(new StartPageListItem(
                string.Format(Resources.StartPageVisitWebsite, dynamoViewModel.BrandingResourceProvider.ProductName),
                "icon-dynamobim.png")
            {
                ContextData = Configurations.DynamoSiteLink,
                ClickAction = StartPageListItem.Action.ExternalUrl
            });

            #endregion

            #region Contribution Links

            contributeLinks.Add(new StartPageListItem(Resources.StartPageGithubRepository, "icon-github.png")
            {
                ContextData = Configurations.GitHubDynamoLink,
                ClickAction = StartPageListItem.Action.ExternalUrl
            });

            contributeLinks.Add(new StartPageListItem(Resources.StartPageSendIssues, "icon-issues.png")
            {
                ContextData = Configurations.GitHubBugReportingLink,
                ClickAction = StartPageListItem.Action.ExternalUrl
            });

            #endregion

            var dvm = this.DynamoViewModel;
            RefreshRecentFileList(dvm.RecentFiles);
            dvm.RecentFiles.CollectionChanged += OnRecentFilesChanged;
        }

        internal void HandleListItemClicked(StartPageListItem clicked)
        {
            if (clicked != null)
            {
                switch (clicked.ClickAction)
                {
                    case StartPageListItem.Action.RegularCommand:
                        HandleRegularCommand(clicked);
                        break;

                    case StartPageListItem.Action.FilePath:
                        HandleFilePath(clicked);
                        break;

                    case StartPageListItem.Action.ExternalUrl:
                        HandleExternalUrl(clicked);
                        break;
                }
            }
        }

        #region Public Class Properties (Static Lists)

        public IEnumerable<StartPageListItem> FileOperations
        {
            get { return this.fileOperations; }
        }

        public IEnumerable<StartPageListItem> ContributeLinks
        {
            get { return this.contributeLinks; }
        }

        #endregion

        #region Public Class Properties (Dynamic Lists)


        public ObservableCollection<StartPageListItem> RecentFiles
        {
            get { return this.recentFiles; }
        }

        #endregion


        #region Private Class Event Handlers

        private void OnRecentFilesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshRecentFileList(sender as IEnumerable<string>);
        }

        #endregion

        #region Private Class Helper Methods

        private void RefreshRecentFileList(IEnumerable<string> filePaths)
        {
            RefreshFileList(recentFiles, filePaths);
        }

        private void RefreshFileList(ObservableCollection<StartPageListItem> files,
            IEnumerable<string> filePaths)
        {
            files.Clear();
            foreach (var filePath in filePaths)
            {
                var extension = Path.GetExtension(filePath).ToUpper();
                // If not extension specified and code reach here, this means this is still a valid file 
                // only without file type. Otherwise, simply take extension substring skipping the 'dot'.
                var subScript = extension.IndexOf(".") == 0 ? extension.Substring(1) : "";
                var caption = Path.GetFileNameWithoutExtension(filePath);

                files.Add(new StartPageListItem(caption)
                {
                    ContextData = filePath,
                    ToolTip = filePath,
                    SubScript = subScript,
                    ClickAction = StartPageListItem.Action.FilePath
                });
            }
        }

        private void HandleRegularCommand(StartPageListItem item)
        {
            var dvm = this.DynamoViewModel;

            switch (item.ContextData)
            {
                case ButtonNames.NewWorkspace:
                    dvm.NewHomeWorkspaceCommand.Execute(null);
                    break;

                case ButtonNames.OpenWorkspace:
                    dvm.ShowOpenDialogAndOpenResultCommand.Execute(null);
                    break;
                    
                case ButtonNames.NewCustomNodeWorkspace:
                    dvm.ShowNewFunctionDialogCommand.Execute(null);
                    break;

                default:
                    throw new ArgumentException(
                        string.Format("Invalid command: {0}", item.ContextData));
            }
        }

        private void HandleFilePath(StartPageListItem item)
        {
            var path = item.ContextData;
            this.DynamoViewModel.OpenCommand.Execute(path);
        }

        private void HandleExternalUrl(StartPageListItem item)
        {
            System.Diagnostics.Process.Start(item.ContextData);
        }

        #endregion

    }

    struct ButtonNames
    {
        public const string NewWorkspace = "NewWorkspace";
        public const string NewCustomNodeWorkspace = "NewCustomNodeWorkspace";
        public const string OpenWorkspace = "OpenWorkspace";
    }

    public partial class StartPageView : UserControl
    {
        private DynamoViewModel dynamoViewModel;

        public StartPageView()
        {
            InitializeComponent();
            this.Loaded += OnStartPageLoaded;
        }

        #region Private Class Event Handlers

        private void OnStartPageLoaded(object sender, RoutedEventArgs e)
        {
            var startPageViewModel = this.DataContext as StartPageViewModel;
            this.dynamoViewModel = startPageViewModel.DynamoViewModel;
            this.filesListBox.ItemsSource = startPageViewModel.FileOperations;
            this.codeListBox.ItemsSource = startPageViewModel.ContributeLinks;
            this.recentListBox.ItemsSource = startPageViewModel.RecentFiles;

            var id = Wpf.Interfaces.ResourceNames.StartPage.Image;
            StartPageLogo.Source = dynamoViewModel.BrandingResourceProvider.GetImageSource(id);

        }

        private void OnItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e == null || (e.AddedItems == null) || (e.AddedItems.Count <= 0))
                return;

            var selected = e.AddedItems[0] as StartPageListItem;
            var startPageViewModel = this.DataContext as StartPageViewModel;
            startPageViewModel.HandleListItemClicked(selected);

            // Clear list box selection so that the same item, when 
            // clicked, still triggers "selection changed" notification.
            var listBox = sender as ListBox;
            listBox.SelectedIndex = -1;
        }

        private void OnCloseStartPageClicked(object sender, MouseButtonEventArgs e)
        {
            this.dynamoViewModel.ShowStartPage = false;
        }

        #endregion






        private void StartPage_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                var homespace = dynamoViewModel.HomeSpace;
                if (homespace.HasUnsavedChanges && 
                    !dynamoViewModel.AskUserToSaveWorkspaceOrCancel(homespace))
                {
                    return;
                }

                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files != null && (files.Length > 0))
                {
                    if (dynamoViewModel.OpenCommand.CanExecute(files[0]))
                    {
                        dynamoViewModel.OpenCommand.Execute(files[0]);
                        e.Handled = true;
                    }
                }

            }
        }
    }
}
