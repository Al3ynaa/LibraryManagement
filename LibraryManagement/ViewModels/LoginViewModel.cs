using LibraryManagement.Services;
using LibraryManagement.Views;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace LibraryManagement.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username = "";
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        // Artık command şart değil (istersen sonra tekrar ekleriz)

        public void Login(string password)
        {
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Username and password required.";
                return;
            }

            // DİKKAT: Tablo sütunun gerçekten "Password" ise böyle kalacak
            string sql = "SELECT COUNT(*) FROM dbo.Users WHERE Username=@u AND Password=@p";

            int count = (int)(Db.Scalar(sql,
                new SqlParameter("@u", Username),
                new SqlParameter("@p", password)) ?? 0);

            if (count == 1)
            {
                var main = new MainWindow();
                main.Show();

                Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w is LoginWindow)
                    ?.Close();
            }
            else
            {
                ErrorMessage = "Wrong username or password.";
            }
        }
    }

}
