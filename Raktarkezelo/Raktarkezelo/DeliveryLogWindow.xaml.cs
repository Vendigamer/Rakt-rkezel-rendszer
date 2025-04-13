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
    /// Interaction logic for DeliveryLogWindow.xaml
    /// </summary>
    public partial class DeliveryLogWindow : Window, INotifyPropertyChanged
    {
        public SearchModel SearchInput { get; set; } = new SearchModel();
        public ObservableCollection<string> Raktarak { get; set; }
        public ObservableCollection<string> Statusz { get; set; }
        public ObservableCollection<ProdStatData> Products { get; set; }
        private ObservableCollection<ProdStatData> filteredProducts;
        public ObservableCollection<ProdStatData> FilteredProducts
        {
            get { return filteredProducts; }
            set { filteredProducts = value; OnPropertyChanged(nameof(FilteredProducts)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }

        public DeliveryLogWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            FileRead();
            Raktarak = new(Products.SelectMany(x => new[] { x.honnan, x.hova }).Distinct().Order());
            Statusz = new(Products.Select(x => x.statusz).Distinct().Order());
            Statusz.Insert(0, "");
            Raktarak.Insert(0, "");
        }

        private void FileRead()
        {
            string jsonStr = File.ReadAllText("ProductsStatusData.json");
            Products = JsonSerializer.Deserialize<ObservableCollection<ProdStatData>>(jsonStr)!;
            FilteredProducts = JsonSerializer.Deserialize<ObservableCollection<ProdStatData>>(jsonStr)!;
        }

        private void filter_BTN_Click(object sender, RoutedEventArgs e)
        {
            FilteredProducts = new(Products.Where(x => x.cikkszam.StartsWith(SearchInput.InputCikkszam) && (x.hova.Contains(SearchInput.InputRaktar) || x.honnan.Contains(SearchInput.InputRaktar)) && x.statusz.Contains(SearchInput.InputStatusz)));
        }

        private void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void download_BTN_Click(object sender, RoutedEventArgs e)
        {
            var data = new StringBuilder();
            data.AppendLine("Cikkszám;Mennyiség;Honnan;Hova;Felhasználó;Indul;Érkezik;Státusz");
            foreach (var product in Products)
            {
                string line = $"{product.cikkszam};{product.darabszam};{product.honnan};{product.hova};{product.user};{product.indul};{product.erkezik};{product.statusz}";
                data.AppendLine(line);
            }
            string fileName = "szállítási_napló.csv";
            string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            try
            {
                File.WriteAllText(filePath, data.ToString(), Encoding.UTF8);
                MessageBox.Show($"A fájl sikeresen el lett mentve ide: {filePath}", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show($"Hiba történt a fájl mentésekor!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
