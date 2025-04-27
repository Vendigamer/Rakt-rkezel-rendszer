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

        public int szam { get; set; }

        public RaktarData SelectedRaktar { get; set; }


        public RaktarDataWindow(ProdData product, ObservableCollection<ProdData> allproducts, string mode, RaktarData selectedraktar)
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
            this.SelectedRaktar = selectedraktar;
            this.szam = Product.darabszam;
        }

        private void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void save_BTN_Click(object sender, RoutedEventArgs e)
        {
            if(Product.nev != "" && Product.darabszam > 0 && Product.darabszam < 801 && Product.cikkszam != "")
            {
                int x = 0;
                if (Product.darabszam + SelectedRaktar.termek - szam < SelectedRaktar.kapacitas)
                {
                    foreach (ProdData prod in allProducts)
                    {
                        if (Product.cikkszam == prod.cikkszam && Mode == "new")
                        {
                            MessageBox.Show($"Már megtalálható egy ilyen cikkszámű termék az adatok között!({prod.nev})", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                            x++;
                        }
                    }
                    if (x == 0)
                    {
                        this.DialogResult = true;
                        this.Close();
                        SelectedRaktar.termek += Product.darabszam - szam;
                    }
                }
                else
                {
                    MessageBox.Show($"Nincs elég hely a raktárban!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Nem hagyhat üresen mezőt!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
