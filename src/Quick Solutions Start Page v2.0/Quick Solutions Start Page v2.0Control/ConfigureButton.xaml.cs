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

namespace Quick_Solutions_Start_Page_v2_0Control
{
    /// <summary>
    /// Interaction logic for ConfigureButton.xaml
    /// </summary>
    public partial class ConfigureButton : UserControl
    {
        public ConfigureButton()
        {
            InitializeComponent();
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            new Settings().ShowDialog();
        }
    }
}
