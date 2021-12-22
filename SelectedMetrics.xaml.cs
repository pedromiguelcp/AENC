using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AENC
{
    /// <summary>
    /// Interaction logic for SelectedMetrics.xaml
    /// </summary>
    public partial class SelectedMetrics : Window
    {
        public SelectedMetrics()
        {
            InitializeComponent();
        }

        private void btn_nometrics_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
