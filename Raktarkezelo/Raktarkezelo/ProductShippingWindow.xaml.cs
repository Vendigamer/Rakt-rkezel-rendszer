using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    /// Interaction logic for ProductShippingWindow.xaml
    /// </summary>
    public partial class ProductShippingWindow : Window, INotifyPropertyChanged
    {
        public ProdData Product { get; set; }

        public ObservableCollection<ProdData> ProductList = new ObservableCollection<ProdData>();

        public ObservableCollection<ProdData> allProducts;

        public ObservableCollection<RaktarData> Raktarak { get; set; }

        public string LogedUsername = "";


        public ObservableCollection<string> raktarak = new ObservableCollection<string>();

        public ObservableCollection<string>? raktaraktoshow
        {
            get { return raktarak; }
            set { raktarak = value; OnPropertyChanged(nameof(raktaraktoshow)); }
        }

        RaktarData RaktarData { get; set; }

        RaktarData DestinationRaktarData { get; set; }

        public ObservableCollection<ProdStatData> ShippedProducts = new ObservableCollection<ProdStatData>();

        public int mennyiseg = 0;

        public int Mennyiseg
        {
            get { return mennyiseg; }
            set { mennyiseg = value; OnPropertyChanged(nameof(Mennyiseg)); }
        }

        public string Raktar { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }
        public ProductShippingWindow(ProdData data,ObservableCollection<RaktarData> raktarak, ObservableCollection<ProdData> productList, string logedUsername, ObservableCollection<ProdData> allproducts)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Product = data;
            this.Raktarak = raktarak;
            this.ProductList = productList;
            this.LogedUsername = logedUsername;
            CBXFill();
            FileRead();
            this.allProducts = allproducts;
        }

        private void FileRead()
        {
            string jsonStr = File.ReadAllText("ProductsStatusData.json");
            ShippedProducts = JsonSerializer.Deserialize<ObservableCollection<ProdStatData>>(jsonStr)!;
        }
        public void CBXFill()
        {
            foreach (var raktar in Raktarak)
            {
                if (Product.raktar == raktar.nev)
                {
                    RaktarData = raktar;
                }
                raktarak.Add(raktar.nev);
            }
            raktarak.Remove(RaktarData.nev);
        }

        private void MAX_Button_Click(object sender, RoutedEventArgs e)
        {
            Mennyiseg = Product.darabszam;
        }

        private void save_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (Raktar != null)
            {
                foreach(var raktar in Raktarak)
                {
                    if (raktar.nev == Raktar)
                    {
                        DestinationRaktarData = raktar;
                    }
                }
                if (mennyiseg > 0)
                {
                    if (DestinationRaktarData.termek + mennyiseg < DestinationRaktarData.kapacitas)
                    {
                        if (Product.darabszam - mennyiseg == 0)
                        {
                            ProductList.Remove(Product);
                            allProducts.Remove(Product);
                        }
                        else
                        {
                            foreach (var product in ProductList)
                            {
                                if (product == Product)
                                {
                                    product.darabszam -= mennyiseg;
                                }
                            }
                        }
                        foreach (var raktar in Raktarak)
                        {
                            if (raktar.nev == Product.raktar)
                            {
                                raktar.termek -= mennyiseg;
                            }
                            else if (raktar.nev == Raktar)
                            {
                                raktar.termek += mennyiseg;
                            }
                        }
                        ProdStatData termek = new ProdStatData()
                        {
                            nev = Product.nev,
                            cikkszam = Product.cikkszam,
                            darabszam = mennyiseg,
                            honnan = Product.raktar,
                            hova = Raktar,
                            user = LogedUsername
                        };
                        ShippedProducts.Add(termek);
                        string jsonStr = JsonSerializer.Serialize(ShippedProducts);
                        File.WriteAllText("ProductsStatusData.json", jsonStr);
                        this.DialogResult = true;

                    }
                    else
                    {
                        MessageBox.Show($"Nincs elég üres hely a(z) {DestinationRaktarData.nev} raktárban", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Adja meg mennyi termék legyen átszállítva!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                MessageBox.Show("Nincs kiválasztva raktár!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
