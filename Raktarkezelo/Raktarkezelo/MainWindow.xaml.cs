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

        public ObservableCollection<RaktarData> RaktarakList = new ObservableCollection<RaktarData>();

        public ObservableCollection<ProdStatData> ShippedProducts = new ObservableCollection<ProdStatData>();

        public ObservableCollection<ProdStatData> AlreadyShippedProducts = new ObservableCollection<ProdStatData>();

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
            ShippedProductHandler();
            Raktarak = new(RaktarakList.Select(x => x.nev).Distinct().Order());
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
            string jsonStr2 = File.ReadAllText("RaktarData.json");
            RaktarakList = JsonSerializer.Deserialize<ObservableCollection<RaktarData>>(jsonStr2)!;
            string jsonStr3 = File.ReadAllText("AlreadyShippedProducts.json");
            AlreadyShippedProducts = JsonSerializer.Deserialize<ObservableCollection<ProdStatData>>(jsonStr3)!;
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
                    RaktarWindow raktarwindow = new RaktarWindow(raktarProducts, RaktarakList, Products, LogedUsername, IsOwner, RaktarName);
                    raktarwindow.ShowDialog();
                    if (raktarwindow.CustomDialogResult == true)
                    {
                        Products = raktarwindow.ProductsList;
                        foreach (var product in raktarwindow.AllProducts)
                        {
                            foreach (var Product in Products)
                            {
                                if (Product.cikkszam == product.cikkszam)
                                {
                                    Product.nev = product.nev;
                                    Product.cikkszam = product.cikkszam;
                                    Product.raktar = product.raktar;
                                    Product.darabszam = product.darabszam;
                                }
                            }
                        }
                        RaktarakList = raktarwindow.Raktarak;
                        filter_BTN_Click(sender, e);
                    }
                    if (raktarwindow.DialogResult == true)
                    {
                        Products = raktarwindow.ProductsList;
                        RaktarData raktardata = new RaktarData();
                        foreach(var raktar in RaktarakList) 
                        {
                            if (raktar.nev == RaktarName)
                            {
                                raktardata = raktar;
                            }
                        }
                        RaktarakList.Remove(raktardata);
                        Raktarak.Remove(RaktarName);
                    }
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
            AddRaktarWindow addRaktarWindow = new AddRaktarWindow(Products, RaktarakList);
            addRaktarWindow.ShowDialog();
            if (addRaktarWindow.DialogResult == true)
            {
                if (addRaktarWindow.NewProducts.Count() > 0)
                {
                    foreach (var product in addRaktarWindow.NewProducts)
                    {
                        product.raktar = addRaktarWindow.RaktarName;
                        Products.Add(product);
                    }
                }
                newRaktarAddToList(addRaktarWindow.RaktarName);
                string jsonWriteStr = JsonSerializer.Serialize(Products);
                File.WriteAllText("ProductData.json", jsonWriteStr);
                MessageBox.Show("Sikeres raktár hozzáadás!", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                filter_BTN_Click(sender, e);
                RaktarakList = addRaktarWindow.Raktarak;
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

        private void ShippedProductHandler()
        {
            string jsonStr = File.ReadAllText("ProductsStatusData.json");
            ShippedProducts = JsonSerializer.Deserialize<ObservableCollection<ProdStatData>>(jsonStr);
            foreach (var item in ShippedProducts)
            {
                if (!AlreadyShippedProducts.Contains(item) && item.statusz == "Kiszállítva")
                {
                    ProdData prodData = new ProdData()
                    {
                        nev = item.nev,
                        raktar = item.hova,
                        cikkszam = item.cikkszam,
                        darabszam = item.darabszam,

                    };
                    Products.Add(prodData);
                    AlreadyShippedProducts.Add(item);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string jsonStr = JsonSerializer.Serialize(RaktarakList);
            File.WriteAllText("RaktarData.json", jsonStr);
            string jsonStr2 = JsonSerializer.Serialize(AlreadyShippedProducts);
            File.WriteAllText("AlreadyShippedProducts.json", jsonStr2);
            string jsonStr3 = JsonSerializer.Serialize(Products);
            File.WriteAllText("ProductData.json", jsonStr3);
        }
    }
}