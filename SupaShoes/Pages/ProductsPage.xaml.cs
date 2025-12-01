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
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private long _role;
        private Users _user;
        public ProductsPage(Users user = null)
        {
            InitializeComponent();
            _user = user;
            if(user != null )
                _role = _user.role_id;
            LoadProduct();
            GetDopFunction();

            cmbFilter.SelectedIndex = 0;
            cmbSort.SelectedIndex = 0;
        }

        private void LoadProduct()
        {
            var context = SuperShoesEntities.GetContext();

            if(_user != null)
                txtUser.Text = _user.firstname + " " + _user.lastname + "\n" + _user.middlename;

            var products = context.Products.ToList();
            LViewProduct.ItemsSource = products;
        }

        private void GetDopFunction()
        {
            // доп функционал для ролей
            if(_role == 1)
            {
                cmbFilter.Visibility = Visibility.Visible;
                cmbSort.Visibility = Visibility.Visible;
                tbSearchg.Visibility = Visibility.Visible;
                btnAdd.Visibility = Visibility.Visible;
                btnOrders.Visibility = Visibility.Visible;
                return;
            }
            if(_role == 2)
            {
                cmbFilter.Visibility = Visibility.Visible;
                cmbSort.Visibility = Visibility.Visible;
                tbSearchg.Visibility = Visibility.Visible;
                btnOrders.Visibility = Visibility.Visible;
                return;
            }

            return;
        }

        private void UpdateProduct()
        {
            var context = SuperShoesEntities.GetContext();

            var result = SuperShoesEntities.GetContext().Products.ToList();

            if(cmbSort.SelectedIndex == 1)
                result = result.OrderBy(p => p.count).ToList();
            if(cmbSort.SelectedIndex == 2)
                result = result.OrderByDescending(p => p.count).ToList();

            if(cmbFilter.SelectedIndex == 1)
                result = result.Where(p => p.manufacturer_id == 1).ToList();
            if (cmbFilter.SelectedIndex == 2)
                result = result.Where(p => p.manufacturer_id == 2).ToList();
            if (cmbFilter.SelectedIndex == 3)
                result = result.Where(p => p.manufacturer_id == 3).ToList();
            if (cmbFilter.SelectedIndex == 4)
                result = result.Where(p => p.manufacturer_id == 4).ToList();
            if (cmbFilter.SelectedIndex == 5)
                result = result.Where(p => p.manufacturer_id == 5).ToList();
            if (cmbFilter.SelectedIndex == 6)
                result = result.Where(p => p.manufacturer_id == 6).ToList();
            if (cmbFilter.SelectedIndex == 7)
                result = result.Where(p => p.manufacturer_id == 7).ToList();

            result = result.Where(p => p.name.ToLower().Contains(tbSearchg.Text.ToLower())
                || p.description.ToLower().Contains(tbSearchg.Text.ToLower())
                || p.Partners1.title.Contains(tbSearchg.Text.ToLower())
                || p.Partners.title.Contains(tbSearchg.Text.ToLower()))
                .ToList();

            LViewProduct.ItemsSource = result;
        }

        private void LViewProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
      
        }

        private void LViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_role != 1)
                return;

            NavigationService.Navigate(new UpdateProductPage(LViewProduct.SelectedItem as Products, _user));
        }

        private void tbSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void cmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddProduct(_user));
        }

        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersPage(_user));
        }
    }
}
