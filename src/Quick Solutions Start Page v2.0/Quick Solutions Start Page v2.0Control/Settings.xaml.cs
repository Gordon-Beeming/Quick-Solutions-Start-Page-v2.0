using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Quick_Solutions_Start_Page_v2_0Control
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Microsoft.VisualStudio.PlatformUI.DialogWindow
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void DialogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtPaths.Text = DataManager.LoadPathSettings();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DataManager.SavePathSettings(txtPaths.Text.Trim());
            MessageBox.Show("Changes will only show after you restart");
            this.Close();
        }
    }
}
