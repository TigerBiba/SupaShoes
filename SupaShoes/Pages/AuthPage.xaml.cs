using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SupaShoes.Models;

namespace SupaShoes.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void tbAuth_Click(object sender, RoutedEventArgs e)
        {
            var context = SuperShoesEntities.GetContext();

            string password = tbPassword.Text;
            string login = tbLogin.Text;

            // поиск пользователя по email и password
            var user = context.Users.FirstOrDefault(x => x.email == login && x.password == password);

            if(user != null)
            {
                NavigationService.Navigate(new ProductsPage(user));

                MessageBox.Show("Успешная регистрация", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }

            MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void tbGuest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductsPage());
        }
    }
}
