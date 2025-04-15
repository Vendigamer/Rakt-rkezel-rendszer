using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LogData InputText { get; set; } = new LogData();
        public ObservableCollection<LogData> Users { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            FileRead();
        }

        private void FileRead()
        {
            string jsonStr = File.ReadAllText("LoginData.json");
            Users = JsonSerializer.Deserialize<ObservableCollection<LogData>>(jsonStr)!;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (InputText != null)
            {
                InputText.jelszo = ((PasswordBox)sender).Password;
            }
        }

        private void login_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (InputCheck(InputText))
            {
                LogData user = new LogData();
                user = Users.FirstOrDefault(x => x.felhasznalonev == InputText.felhasznalonev && x.jelszo == InputText.jelszo);
                if (user == null)
                {
                    MessageBox.Show("Hibás felhasználónév vagy jelszó!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
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
            return true;
        }
    }
}
