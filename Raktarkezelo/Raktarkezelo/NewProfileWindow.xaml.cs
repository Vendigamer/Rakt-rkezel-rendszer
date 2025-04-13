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
    /// Interaction logic for NewProfileWindow.xaml
    /// </summary>
    public partial class NewProfileWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<string> Raktarak { get; set; }
        public ObservableCollection<LogData> Users { get; set; }
        public LogData NewUser { get; set; } = new()
        {
            felhasznalonev = "",
            jelszo = "",
            raktar = "",
            isUser = true
        };
        private bool isUser;
        public bool IsUser
        {
            get => isUser;
            set
            {
                if (isUser != value)
                {
                    isUser = value;
                    OnPropertyChanged(nameof(IsUser));
                    OnPropertyChanged(nameof(IsAdmin));
                }
            }
        }
        public bool IsAdmin
        {
            get => !IsUser;
            set { IsUser = !value; }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string tulajdonsagNev)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tulajdonsagNev));
        }
        public NewProfileWindow(ObservableCollection<string> raktarak, ObservableCollection<LogData> users)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Raktarak = raktarak;
            this.Users = users;
        }

        private void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (NewUser != null)
            {
                NewUser.jelszo = ((PasswordBox)sender).Password;
            }
        }

        private void add_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (InputCheck(NewUser))
            {
                NewUser.isUser = this.IsUser;
                this.DialogResult = true;
            }
        }

        private bool InputCheck(LogData user)
        {
            if (string.IsNullOrEmpty(user.felhasznalonev))
            {
                MessageBox.Show("Kérlek add meg a felhasználónevet!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(user.jelszo))
            {
                MessageBox.Show("Kérlek add meg a jelszót!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (user.raktar == null || user.raktar == "")
            {
                MessageBox.Show("Kérlek add meg a raktárat!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (Users.Any(x => x.felhasznalonev == user.felhasznalonev && x.raktar == user.raktar))
            {
                MessageBox.Show("Ez a felhasználónév már létezik ilyen raktárral!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
    }
}
