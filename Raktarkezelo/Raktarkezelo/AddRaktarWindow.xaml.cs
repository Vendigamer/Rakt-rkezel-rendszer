using Microsoft.Win32;
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
    /// Interaction logic for AddRaktarWindow.xaml
    /// </summary>
    public partial class AddRaktarWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<ProdData> NewProducts = new ObservableCollection<ProdData>();
        public ObservableCollection<ProdData> Products { get; set; }
        public ObservableCollection<RaktarData> Raktarak { get; set; }

        public ObservableCollection<string> sor = new ObservableCollection<string>();

        public ObservableCollection<string> Sor
        {
            get { return sor; }
            set { sor = value; OnPropertyChanged(nameof(Sor)); }
        }
        public int Meret { get; set; }
        public string RaktarName { get; set; }
        private string fileLocation;
        public string FileLocation
        {
            get { return fileLocation; }
            set { fileLocation = value; OnPropertyChanged(nameof(FileLocation)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }

        public AddRaktarWindow(ObservableCollection<ProdData> products, ObservableCollection<RaktarData> raktarak)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Products = products;
            this.Raktarak = raktarak;
            for (int i = 0; i < 11; i++)
            {
                this.sor.Add(i.ToString());
            }
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Válassz ki egy json fájlt";
            openFileDialog.Filter = "Minden fájl (*.json*)|*.json*";

            if (openFileDialog.ShowDialog() == true)
            {
                FileLocation = openFileDialog.FileName;
            }
        }

        private void add_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (InputCheck())
            {
                this.DialogResult = true;
                int x = 0;
                if (string.IsNullOrEmpty(FileLocation))
                {
                    x = 0;
                }
                else
                {
                    string jsonStr = File.ReadAllText($"{FileLocation}");
                    NewProducts = JsonSerializer.Deserialize<ObservableCollection<ProdData>>(jsonStr)!;
                    foreach (var item in NewProducts)
                    {
                        x += item.darabszam;
                    }
                }
                RaktarData raktar = new RaktarData()
                {
                    nev = RaktarName,
                    kapacitas = Meret * 100,
                    termek = x
                };
                Raktarak.Add(raktar);
            }
        }

        private void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private bool InputCheck()
        {
            if (string.IsNullOrEmpty(RaktarName))
            {
                MessageBox.Show("Kérlek add meg a raktár nevét!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (Products.Any(x => x.raktar.ToLower() == RaktarName.ToLower()))
            {
                MessageBox.Show("Ez a raktár már létezik!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
    }
}
