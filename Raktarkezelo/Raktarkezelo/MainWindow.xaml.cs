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
        public bool IsNew { get; set; }
        private ObservableCollection<ProdData> filteredProducts;
        public ObservableCollection<string> Raktarak { get; set; }
        public SearchModel SearchInput { get; set; } = new SearchModel();
        public ObservableCollection<ProdData> FilteredProducts
        {
            get { return filteredProducts; }
            set { filteredProducts = value; OnPropertyChanged(nameof(FilteredProducts)); }
        }
        public string RaktarName { get; set; }

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
        }

        private void FileRead()
        {
            string jsonStr = File.ReadAllText("ProductData.json");
            Products = JsonSerializer.Deserialize<ObservableCollection<ProdData>>(jsonStr)!;
            FilteredProducts = JsonSerializer.Deserialize<ObservableCollection<ProdData>>(jsonStr)!;
        }

        private void mainLogin_BTN_Click(object sender, RoutedEventArgs e)
        {
            IsNew = false;
            if (RaktarName != null && RaktarName != "")
            {
                LoginWindow loginWindow = new LoginWindow(RaktarName, IsNew);
                loginWindow.ShowDialog();
                if (loginWindow.DialogResult == true)
                {
                    bool IsUser = loginWindow.IsUser;
                    MessageBox.Show("Sikeres bejelentkezés!", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
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
            IsNew = true;
            LoginWindow loginWindow = new LoginWindow(RaktarName, IsNew);
            loginWindow.ShowDialog();
            if (loginWindow.DialogResult == true)
            {
                bool IsUser = loginWindow.IsUser;
                if (!IsUser)
                {
                    AddRaktarWindow addRaktarWindow = new AddRaktarWindow();
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
                else
                {
                    MessageBox.Show("Nincs jogosultságod új raktár hozzáadásához", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
            IsNew = true;
            LoginWindow loginWindow = new LoginWindow(RaktarName, IsNew);
            loginWindow.ShowDialog();
            if (loginWindow.DialogResult == true)
            {
                bool IsUser = loginWindow.IsUser;
                if (!IsUser)
                {
                    NewProfileWindow newProfileWindow = new NewProfileWindow(Raktarak);
                    newProfileWindow.ShowDialog();
                    if (newProfileWindow.DialogResult == true)
                    {
                        string jsonStr = File.ReadAllText("LoginData.json");
                        ObservableCollection<LogData> users = JsonSerializer.Deserialize<ObservableCollection<LogData>>(jsonStr)!;
                        users.Add(newProfileWindow.NewUser);
                        string jsonWriteStr = JsonSerializer.Serialize(users);
                        File.WriteAllText("LoginData.json", jsonWriteStr);
                        MessageBox.Show("Sikeres új profil létrehozás!", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Nincs jogosultságod új profilt létrehozni", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}