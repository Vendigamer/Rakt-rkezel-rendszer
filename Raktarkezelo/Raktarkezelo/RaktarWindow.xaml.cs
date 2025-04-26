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
        ObservableCollection<ProdData> allProducts;

        public ObservableCollection<RaktarData> Raktarak { get; set; }

        public ProdData SelectedItem { get; set; }

        public bool issomethingselected = new bool();

        public bool CustomDialogResult = false;

        public ObservableCollection<ProdData> AllProducts
        {
            get { return allProducts; }
            set { allProducts = value; OnPropertyChanged(nameof(AllProducts)); }
        }

        public bool IsSomethingSelected
        {
            get { return issomethingselected; }
            set { issomethingselected = value; OnPropertyChanged(nameof(IsSomethingSelected)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }
        public RaktarWindow(ObservableCollection<ProdData> products, ObservableCollection<RaktarData> raktarak)
        {
            InitializeComponent();
            this.DataContext = this;
            this.allProducts = products;
            this.Raktarak = raktarak;
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
                RaktarDataWindow raktarDataWindow = new RaktarDataWindow(EditData, allProducts, mode);
                raktarDataWindow.ShowDialog();
                if (raktarDataWindow.DialogResult == true)
                {
                    allProducts[index] = raktarDataWindow.Product;
                    this.CustomDialogResult = true;
                }
            }
        }

        private void New_BTN_Click(object sender, RoutedEventArgs e)
        {
            ProdData NewData = new ProdData();
            string mode = "new";
            RaktarDataWindow newProductWindow = new RaktarDataWindow(NewData, allProducts, mode);
            newProductWindow.ShowDialog();
            if (newProductWindow.DialogResult == true)
            {
                NewData = newProductWindow.Product;
                this.allProducts.Add(NewData);
                this.CustomDialogResult = true;
            }
            else
            {
                allProducts = newProductWindow.allProducts;
                this.CustomDialogResult = true;
            }
        }

        private void Delete_BTN_Click(object sender, RoutedEventArgs e)
        {
            int index = allProducts.IndexOf(allProducts.FirstOrDefault(x => x.cikkszam == SelectedItem.cikkszam));
            MessageBoxResult result = MessageBox.Show($"Biztosan törölni kívánja a(z) {SelectedItem.nev} terméket??", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                allProducts.Remove(SelectedItem);
                this.CustomDialogResult = true;
            }
            return;

        }

        private void Trans_BTN_Click(object sender, RoutedEventArgs e)
        {
            ProductShippingWindow newProductWindow = new ProductShippingWindow(SelectedItem, Raktarak);
            newProductWindow.ShowDialog();
            if (newProductWindow.DialogResult == true)
            {
                this.CustomDialogResult = true;
            }
        }
    }
}
