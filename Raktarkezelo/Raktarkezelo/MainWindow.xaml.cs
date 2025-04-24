using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Raktarkezelo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<ProdData> Products { get; set; }
        private ObservableCollection<ProdData> filteredProducts;
        private ObservableCollection<ProdData> raktarProducts;
        public ObservableCollection<ProdData> FilteredProducts
        {
            get { return filteredProducts; }
            set { filteredProducts = value; OnPropertyChanged(nameof(FilteredProducts)); }
        }

        public ObservableCollection<string> Raktarak { get; set; }
        private ObservableCollection<string> usersRaktarak;
        public ObservableCollection<string> UsersRaktarak
        {
            get { return usersRaktarak; }
            set { usersRaktarak = value; OnPropertyChanged(nameof(UsersRaktarak)); }
        }
        public string LogedUsername { get; set; }
        public bool IsOwner { get; set; }
        public SearchModel SearchInput { get; set; } = new SearchModel();
        public string RaktarName { get; set; }
        private bool isEnabledBTN;
        public bool IsEnabledBTN
        {
            get { return isEnabledBTN; }
            set { isEnabledBTN = value; OnPropertyChanged(nameof(IsEnabledBTN)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            FileRead();
            Raktarak = new(Products.Select(x => x.raktar).Distinct().Order());
            Raktarak.Insert(0, "");
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
            if (loginWindow.DialogResult == true)
            {
                LogedUsername = loginWindow.InputText.felhasznalonev;
                IsOwner = loginWindow.Users.Any(x => x.felhasznalonev == LogedUsername && x.isOwner == true);
                if (IsOwner == true)
                {
                    UsersRaktarak = Raktarak;
                    IsEnabledBTN = true;
                }
                else
                {
                    UsersRaktarak = new(loginWindow.Users.Where(x => x.felhasznalonev == LogedUsername && x.isUser == false).Select(x => x.raktar));
                }
            }
            else
            {
                this.Close();
            }
        }

        private void FileRead()
        {
            string jsonStr = File.ReadAllText("ProductData.json");
            Products = JsonSerializer.Deserialize<ObservableCollection<ProdData>>(jsonStr)!;
            FilteredProducts = JsonSerializer.Deserialize<ObservableCollection<ProdData>>(jsonStr)!;
        }

        private void mainLogin_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (RaktarName != null && RaktarName != "")
            {
                LoginWindow loginWindow = new LoginWindow();
                RaktarLoginWindow raktarLoginWindow = new RaktarLoginWindow(loginWindow.Users, LogedUsername, RaktarName);
                if (!IsOwner)
                { 
                    raktarLoginWindow.ShowDialog();
                }
                else
                {
                    raktarLoginWindow.IsOwnerLog = true; 
                    raktarLoginWindow.ShowDialog();
                }
                if (raktarLoginWindow.DialogResult == true)
                {
                    raktarProducts = new(Products.Where(x => x.raktar.ToLower().StartsWith(RaktarName.ToLower())));
                    RaktarWindow raktarwindow = new RaktarWindow(raktarProducts);
                    raktarwindow.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Kérlek válassz egy raktárat!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void filter_BTN_Click(object sender, RoutedEventArgs e)
        {
            FilteredProducts = new(Products.Where(x => x.nev.ToLower().StartsWith(SearchInput.InputName.ToLower()) && x.raktar.Contains(SearchInput.InputRaktar) && x.cikkszam.Contains(SearchInput.InputCikkszam)));
        }

        private void addRaktar_BTN_Click(object sender, RoutedEventArgs e)
        {
            AddRaktarWindow addRaktarWindow = new AddRaktarWindow(Products);
            addRaktarWindow.ShowDialog();
            if (addRaktarWindow.DialogResult == true)
            {
                string jsonStr = File.ReadAllText("ProductData.json");
                Products = JsonSerializer.Deserialize<ObservableCollection<ProdData>>(jsonStr)!;
                foreach (var product in addRaktarWindow.NewProducts)
                {
                    product.raktar = addRaktarWindow.RaktarName;
                    Products.Add(product);
                }
                newRaktarAddToList(addRaktarWindow.RaktarName);
                string jsonWriteStr = JsonSerializer.Serialize(Products);
                File.WriteAllText("ProductData.json", jsonWriteStr);
                MessageBox.Show("Sikeres raktár hozzáadás!", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                filter_BTN_Click(sender, e);
            }
        }

        private void newRaktarAddToList(string raktarName)
        {
            if (!Raktarak.Contains(raktarName))
            {
                Raktarak.Add(raktarName);
            }
        }

        private void newProfile_BTN_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<LogData> Users;
            string jsonStr = File.ReadAllText("LoginData.json");
            Users = JsonSerializer.Deserialize<ObservableCollection<LogData>>(jsonStr)!;
            NewProfileWindow newProfileWindow = new NewProfileWindow(Raktarak, Users);
            newProfileWindow.ShowDialog();
            if (newProfileWindow.DialogResult == true)
            {
                string jsonWriteStr = JsonSerializer.Serialize(Users);
                File.WriteAllText("LoginData.json", jsonWriteStr);
                MessageBox.Show("Sikeres új profil létrehozás!", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void szallitasiNaplo_BTN_Click(object sender, RoutedEventArgs e)
        {
            DeliveryLogWindow deliveryLogWindow = new DeliveryLogWindow();
            deliveryLogWindow.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string jsonWriteStr = JsonSerializer.Serialize(Products);
            File.WriteAllText("ProductData.json", jsonWriteStr);
        }
    }
}