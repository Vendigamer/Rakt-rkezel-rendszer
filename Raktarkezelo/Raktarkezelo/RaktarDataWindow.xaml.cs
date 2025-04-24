using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Raktarkezelo
{
    /// <summary>
    /// Interaction logic for RaktarDataWindow.xaml
    /// </summary>
    public partial class RaktarDataWindow : Window
    {
        public ProdData Product { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public RaktarDataWindow(ProdData product)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Product = product;
        }

        private void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
