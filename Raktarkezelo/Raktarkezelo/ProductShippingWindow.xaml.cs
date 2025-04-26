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
    /// Interaction logic for ProductShippingWindow.xaml
    /// </summary>
    public partial class ProductShippingWindow : Window
    {
        public ProdData Product { get; set; }

        public ObservableCollection<RaktarData> Raktarak { get; set; }

        public ObservableCollection<string> raktarak = new ObservableCollection<string>();

        public ObservableCollection<string>? raktaraktoshow
        {
            get { return raktarak; }
            set { raktarak = value; OnPropertyChanged(nameof(raktaraktoshow)); }
        }

        RaktarData RaktarData = new RaktarData();

        public string Raktar { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }
        public ProductShippingWindow(ProdData data,ObservableCollection<RaktarData> raktarak)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Product = data;
            this.Raktarak = raktarak;
            CBXFill();
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
            max_TXB.Text = this.Product.darabszam.ToString();
        }

        private void save_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (Raktar != null)
            {

            }
            else
            {
                MessageBox.Show("Nincs kiválasztva raktár!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.DialogResult = true;
        }

        private void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
