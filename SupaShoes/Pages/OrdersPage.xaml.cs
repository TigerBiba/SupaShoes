using SupaShoes.Models;
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

namespace SupaShoes.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private long _role;
        private Users _user;
        public OrdersPage(Users user)
        {
            InitializeComponent();
            _user = user;
            _role = user.role_id;
            LoadProduct();
            GetDopFunction(_user.role_id);
        }

        private void LoadProduct()
        {
            var context = SuperShoesEntities.GetContext();

            txtName.Text = _user.firstname + " " + _user.lastname + "\n" + _user.middlename;

            var orders = context.Orders.ToList();
            LViewOrders.ItemsSource = orders;
        }

        private void GetDopFunction(long role)
        {
            // доп функционал для администратора
            if (role == 1)
            {
                btnAdd.Visibility = Visibility.Visible;
                return;
            }

            return;
        }

        private void LViewOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_role != 1)
                return;

            NavigationService.Navigate(new UpdateOrderPage(LViewOrders.SelectedItem as Orders, _user));
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddOrder(_user));
        }
    }
}
