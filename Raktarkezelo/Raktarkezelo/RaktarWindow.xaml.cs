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
    /// Interaction logic for RaktarWindow.xaml
    /// </summary>
    public partial class RaktarWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<ProdData> allProducts;

        public ObservableCollection<ProdData> ProductsList;

        public ObservableCollection<RaktarData> Raktarak { get; set; }

        public RaktarData SelectedRaktar = new RaktarData();

        public RaktarData SelectedRaktarToShow
        {
            get { return SelectedRaktar; }
            set { SelectedRaktar = value; OnPropertyChanged(nameof(SelectedRaktarToShow)); }
        }

        public ProdData SelectedItem { get; set; }

        public bool issomethingselected = new bool();

        public bool IsOwner { get; set; }

        public bool IsSomethingSelected
        {
            get { return issomethingselected; }
            set { issomethingselected = value; OnPropertyChanged(nameof(IsSomethingSelected)); }
        }

        public bool CustomDialogResult = false;

        public string LogedUsername = "";

        public string RaktarName = "";
        public ObservableCollection<ProdData> AllProducts
        {
            get { return allProducts; }
            set { allProducts = value; OnPropertyChanged(nameof(AllProducts)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }
        public RaktarWindow(ObservableCollection<ProdData> products, ObservableCollection<RaktarData> raktarak, ObservableCollection<ProdData> productsList, string logedUsername, bool isowner, string raktarname)
        {
            InitializeComponent();
            this.DataContext = this;
            this.allProducts = products;
            this.Raktarak = raktarak;
            ProductsList = productsList;
            this.ProductsList = productsList;
            this.LogedUsername = logedUsername;
            this.IsOwner = isowner;
            this.RaktarName = raktarname;
            RaktarSelect();
        }

        private void raktar_DG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItem != null)
            {
                IsSomethingSelected = true;
            }
            if (SelectedItem == null)
            {
                IsSomethingSelected = false;
            }
        }

        private void RaktarSelect() 
        {
            foreach (var raktar in Raktarak)
            {
                if (raktar.nev == RaktarName)
                {
                    SelectedRaktarToShow = raktar;
                }
            }
        }

        private void Edit_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                int index = allProducts.IndexOf(allProducts.FirstOrDefault(x => x.cikkszam == SelectedItem.cikkszam));
                ProdData EditData = new ProdData()
                {
                    nev = SelectedItem.nev,
                    cikkszam = SelectedItem.cikkszam,
                    raktar = SelectedItem.raktar,
                    darabszam = SelectedItem.darabszam,
                };
                string mode = "edit";
                RaktarDataWindow raktarDataWindow = new RaktarDataWindow(EditData, allProducts, mode, SelectedRaktar);
                raktarDataWindow.ShowDialog();
                if (raktarDataWindow.DialogResult == true)
                {
                    AllProducts[index] = raktarDataWindow.Product;
                    this.CustomDialogResult = true;
                    SelectedRaktarToShow = raktarDataWindow.SelectedRaktar;
                    RaktarSelect();
                }
            }
        }

        private void New_BTN_Click(object sender, RoutedEventArgs e)
        {
            ProdData NewData = new ProdData();
            string mode = "new";
            RaktarDataWindow newProductWindow = new RaktarDataWindow(NewData, allProducts, mode, SelectedRaktar);
            newProductWindow.ShowDialog();
            if (newProductWindow.DialogResult == true)
            {
                NewData = newProductWindow.Product;
                NewData.raktar = SelectedRaktar.nev;
                AllProducts.Add(NewData);
                ProductsList.Add(NewData);
                this.CustomDialogResult = true;
                SelectedRaktarToShow = newProductWindow.SelectedRaktar;
                RaktarSelect();
            }
        }

        private void Delete_BTN_Click(object sender, RoutedEventArgs e)
        {
            int index = allProducts.IndexOf(allProducts.FirstOrDefault(x => x.cikkszam == SelectedItem.cikkszam));
            MessageBoxResult result = MessageBox.Show($"Biztosan törölni kívánja a(z) {SelectedItem.nev} terméket??", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                SelectedRaktarToShow.termek -= SelectedItem.darabszam;
                AllProducts.Remove(SelectedItem);
                this.CustomDialogResult = true;
                RaktarSelect();
            }
            return;

        }

        private void Trans_BTN_Click(object sender, RoutedEventArgs e)
        {
            ProductShippingWindow newProductWindow = new ProductShippingWindow(SelectedItem, Raktarak, ProductsList, LogedUsername, allProducts);
            newProductWindow.ShowDialog();
            if (newProductWindow.DialogResult == true)
            {
                this.CustomDialogResult = true;
                AllProducts = newProductWindow.allProducts;
                Raktarak = newProductWindow.Raktarak;
                RaktarSelect();
            }
        }

        private void DeleteRak_BTN_Click(object sender, RoutedEventArgs e)
        {
            DeleteRaktar deleteraktar = new DeleteRaktar(Raktarak, AllProducts, SelectedRaktar, LogedUsername);
            deleteraktar.ShowDialog();
            if (deleteraktar.DialogResult == true)
            {
                ProdData data = new ProdData();
                foreach (var item in deleteraktar.allProducts) 
                {
                    data = item;
                    int x = 0;
                    foreach (var product in ProductsList)
                    {
                        if (data == product)
                        {
                            x += 1;
                        }
                    }
                    if(x == 1)
                    {
                        ProductsList.Remove(data);
                    }
                }
                Raktarak = deleteraktar.Raktarak;
                this.DialogResult = true;
            }
        }
    }
}
