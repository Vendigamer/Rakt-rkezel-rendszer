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
        public ObservableCollection<ProdData> NewProducts { get; set; }
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

        public AddRaktarWindow()
        {
            InitializeComponent();
            this.DataContext = this;
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
                string jsonStr = File.ReadAllText($"{FileLocation}");
                NewProducts = JsonSerializer.Deserialize<ObservableCollection<ProdData>>(jsonStr)!;
                this.DialogResult = true;
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
            if (string.IsNullOrEmpty(FileLocation))
            {
                MessageBox.Show("Kérlek add meg a termék(ek) fájljának a helyét!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
    }
}
