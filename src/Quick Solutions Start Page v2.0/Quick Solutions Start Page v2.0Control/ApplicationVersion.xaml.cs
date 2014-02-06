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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;

namespace Quick_Solutions_Start_Page_v2_0Control
{
    /// <summary>
    /// Interaction logic for ApplicationVersion.xaml
    /// </summary>
    public partial class ApplicationVersion : UserControl
    {
        public ApplicationVersion()
        {
            InitializeComponent();

            this.versionNumber.Content = string.Concat("v", Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new Settings().ShowDialog();
        }
    }
}
