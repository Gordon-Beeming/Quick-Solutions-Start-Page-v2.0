// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.PlatformUI;
using System.Reflection;

namespace Quick_Solutions_Start_Page_v2_0Control
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        public MyControl()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Load control user settings from previous session.
            // Example: string value = StartPageSettings.RetrieveString("MySetting");
            bool doWork = false;
            DataManager.Load();
            lstProjects.ItemsSource = DataManager.DataSource;
            lstProjects.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            if (!string.IsNullOrEmpty(StartPageSettings.RetrieveString("LastScanDateTime")))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParse(StartPageSettings.RetrieveString("LastScanDateTime"), out dt))
                {
                    if (dt >= DateTime.Now.AddMinutes(10))
                    {
                        doWork = true;
                    }
                }
                else
                    doWork = true;
            }
            else
                doWork = true;

            if (doWork || !lstProjects.HasItems)
            {
                ScanAllPaths();
            }

            DataManager.SettingsSaved += () =>
            {
                ScanAllPaths();
            };
        }
  
        private void ScanAllPaths()
        {
            foreach (string line in DataManager.ScanPaths)
            {
                string[] lineData = line.Split('|');
                if (lineData.Length >= 2)
                {
                    new DirectoryInfo(lineData[1].Trim()).GetFiles("*.sln", (fi) =>
                    {
                        System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " - " + fi.Name);
                    }, lineData[0].Trim());
                }
                else
                {
                    new DirectoryInfo(lineData[0].Trim()).GetFiles("*.sln", (fi) =>
                    {
                        System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " - " + fi.Name);
                    }, Guid.NewGuid().ToString().Remove(5));
                }
            }
            StartPageSettings.StoreString("LastScanDateTime", DateTime.Now.ToString());
        }

        private StartPageSettings _settings;
        /// <summary>
        /// Return a StartPageSettings object.
        ///
        /// Use: StartPageSettings.StoreString("MySettingName", "MySettingValue");
        /// Use: string value = StartPageSettings.RetrieveString("MySettingName");
        ///
        /// Note: As this property is using the Start Page tool window DataContext to retrieve the Visual Studio DTE, 
        /// this property can only be used after the UserControl is loaded and the inherited DataContext is set.
        /// </summary>
        private StartPageSettings StartPageSettings
        {
            get
            {
                if (_settings == null)
                {
                    DTE2 dte = Utilities.GetDTE(DataContext);
                    ServiceProvider serviceProvider = Utilities.GetServiceProvider(dte);
                    _settings = new StartPageSettings(serviceProvider);
                }
                return _settings;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DataManager.Save();
            //StartPageSettings.StoreString("LastScanDateTime", string.Empty);
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            //new DTE().ExecuteCommand("File.OpenProject", ((ImageButton)sender).CommandParameter.ToString());
            DTE2 dte = Utilities.GetDTE(DataContext);
            ServiceProvider serviceProvider = Utilities.GetServiceProvider(dte);
            IVsSolution service = serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
            service.OpenSolutionFile(1, ((ImageButton)sender).CommandParameter.ToString());
        }
    }
}
