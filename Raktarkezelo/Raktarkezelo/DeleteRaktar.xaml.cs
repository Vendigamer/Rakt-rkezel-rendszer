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
    /// Interaction logic for DeleteRaktar.xaml
    /// </summary>
    public partial class DeleteRaktar : Window, INotifyPropertyChanged
    {
        public RaktarData SelectedRaktar { get; set; }

        public RaktarData DeliveryRaktar { get; set; }

        public string LogedUsername { get; set; }

        public ObservableCollection<ProdData> allProducts;

        public ObservableCollection<RaktarData> Raktarak { get; set; }

        public ObservableCollection<string> raktarakCBX { get; set; }

        public ObservableCollection<ProdStatData> ShippedProducts = new ObservableCollection<ProdStatData>();

        public string celraktar { get; set; }

        public string CelRaktar
        {
            get { return celraktar; }
            set { celraktar = value; OnPropertyChanged(nameof(CelRaktar)); }
        }

        public ObservableCollection<string> RaktarakCBX
        {
            get { return raktarakCBX; }
            set { raktarakCBX = value; OnPropertyChanged(nameof(RaktarakCBX)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }
        public DeleteRaktar(ObservableCollection<RaktarData> raktarak, ObservableCollection<ProdData> allproducts, RaktarData selectedRaktar, string logedusername)
        {
            InitializeComponent();
            this.DataContext = this;
            this.SelectedRaktar = selectedRaktar;
            this.Raktarak = raktarak;
            this.allProducts = allproducts;
            this.raktarakCBX = new(Raktarak.Where(x=> x.nev != SelectedRaktar.nev).Select(x => x.nev).Distinct().Order());
            this.raktarakCBX.Insert(0, "");
            this.LogedUsername = logedusername;
            FileRead();

        }

        private void FileRead()
        {
            string jsonStr = File.ReadAllText("ProductsStatusData.json");
            ShippedProducts = JsonSerializer.Deserialize<ObservableCollection<ProdStatData>>(jsonStr)!;
        }

        private void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void save_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (CelRaktar != null)
            {
                int szam = 0;
                foreach (var product in allProducts)
                {
                    szam += product.darabszam;
                }
                foreach(var raktar in Raktarak)
                {
                    if (raktar.nev == CelRaktar)
                    {
                        DeliveryRaktar = raktar;
                    }
                }
                if (DeliveryRaktar.termek+szam < DeliveryRaktar.kapacitas)
                {
                    MessageBoxResult result = MessageBox.Show($"Biztosan törölni kívánja a raktárt?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Raktarak.Remove(SelectedRaktar);
                        foreach (var raktar in Raktarak)
                        {
                            if (raktar.nev == DeliveryRaktar.nev)
                            {
                                DeliveryRaktar.termek += szam;
                            }
                        }
                        foreach (var product in allProducts)
                        {
                            ProdStatData termek = new ProdStatData()
                            {
                                cikkszam = product.cikkszam,
                                darabszam = product.darabszam,
                                honnan = product.raktar,
                                hova = DeliveryRaktar.nev,
                                user = LogedUsername
                            };
                            ShippedProducts.Add(termek);
                        }
                        string jsonStr = JsonSerializer.Serialize(ShippedProducts);
                        File.WriteAllText("ProductsStatusData.json", jsonStr);
                        this.DialogResult = true;
                    }
                    return;
                }
                else
                {
                    MessageBox.Show($"Nincs elég üres hely a(z) {CelRaktar} raktárban!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            MessageBox.Show("Nincs kiválasztva törlendő elem!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
