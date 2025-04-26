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

        public ObservableCollection<ProdData> allProducts;

        public string Mode { get; set; }


        public RaktarDataWindow(ProdData product, ObservableCollection<ProdData> allproducts, string mode)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Product = product;
            this.allProducts = allproducts;
            if (product.darabszam == 0)
            {
                cikkszam_TXB.IsEnabled = true;
            }

            this.Mode = mode;
        }

        private void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void save_BTN_Click(object sender, RoutedEventArgs e)
        {
            if(this.Product.nev != "" && this.Product.darabszam > 0 && this.Product.darabszam < 801 && this.Product.cikkszam != "")
            {
                int x = 0;
                int y = 0;
                foreach(ProdData prod in this.allProducts)
                {
                    if (this.Product.cikkszam == prod.cikkszam && this.Product.nev != prod.nev && Mode == "new") 
                    {
                        MessageBox.Show($"Már megtalálható egy ilyen cikkszámű termék az adatok között!({prod.nev})", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        x++;
                    }
                    else if (this.Product.cikkszam == prod.cikkszam && this.Product.nev == prod.nev && Mode == "new")
                    {

                        prod.darabszam += this.Product.darabszam;
                        y++;
                        this.Close();
                    }
                }
                if (x == 0 && y == 0)
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Nem hagyhat üresen mezőt!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
