using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for RaktarLoginWindow.xaml
    /// </summary>
    public partial class RaktarLoginWindow : Window
    {
        public string PassWordInput { get; set; }
        public ObservableCollection<LogData> Users { get; set; }
        public string Username { get; set; }
        public string RaktarName { get; set; }
        public bool IsOwnerLog { get; set; } = false;
        public RaktarLoginWindow(ObservableCollection<LogData> users, string username, string raktarName)
        {
            InitializeComponent();
            this.Users = users;
            this.Username = username;
            this.RaktarName = raktarName;
        }

        private void login_BTN_Click(object sender, RoutedEventArgs e)
        {
            string password = jelszo_INP.Password;
            if (InputCheck(password))
            {
                if (Users.Any(x => x.felhasznalonev == Username && x.jelszo == password && x.raktar == RaktarName))
                {
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Hibás jelszó!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private bool InputCheck(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Kérlek add meg a jelszót!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsOwnerLog)
            {
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
