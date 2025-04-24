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

        public ProdData SelectedItem { get; set; }
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
        public RaktarWindow(ObservableCollection<ProdData> products)
        {
            InitializeComponent();
            this.DataContext = this;
            this.allProducts = products;
        }

        private void Edit_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null) 
            {
                ProdData EditData = new ProdData()
                {
                    nev = SelectedItem.nev,
                    cikkszam = SelectedItem.cikkszam,
                    raktar = SelectedItem.raktar,
                    darabszam = SelectedItem.darabszam,
                    sor = SelectedItem.sor,
                    polc = SelectedItem.polc,
                };
                RaktarDataWindow raktarDataWindow = new RaktarDataWindow(EditData);
                raktarDataWindow.ShowDialog();
            }
        }

        private void Trans_BTN_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
