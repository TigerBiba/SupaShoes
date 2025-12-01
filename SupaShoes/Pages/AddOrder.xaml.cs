using SupaShoes.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Xml.Linq;

namespace SupaShoes.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddOrder.xaml
    /// </summary>
    public partial class AddOrder : Page
    {
        private Users _user;
        public AddOrder(Users user)
        {
            InitializeComponent();
            _user = user;
            GetComboBoxItems();
        }

        private void GetComboBoxItems()
        {
            var context = SuperShoesEntities.GetContext();

            cmbUser.ItemsSource = context.Users.Select(x => x.email).ToList();

            cmbArticle.ItemsSource = context.Products.Select(x => x.article).ToList();

            cmbPickap.ItemsSource = context.PickapPoints.Select(x => x.adress).ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!TrySave())
                return;

            var context = SuperShoesEntities.GetContext();

            var newOrder = new Orders();

            newOrder.pickup_point_id = cmbPickap.SelectedIndex + 1;
            newOrder.user_id = cmbUser.SelectedIndex + 1;
            newOrder.delivery_date = DateTime.Parse(dpDelivery.Text);
            newOrder.order_date = DateTime.Parse(dpOrder.Text);
            newOrder.order_status_id = cmbStatus.SelectedIndex + 1;
            newOrder.receiver_code = context.Orders.Select(x => x.receiver_code).Max() + 1;

            try
            {
                context.Orders.Add(newOrder);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошабка: {ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            

            var contents = new OrderContents();

            contents.product_article = cmbArticle.Text;
            contents.count = 1;
            contents.order_id = newOrder.id;

            try
            {
                context.OrderContents.Add(contents);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошабка: {ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Успешное сохранение", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.Navigate(new OrdersPage(_user));
        }

        private bool TrySave()
        {
            var context = SuperShoesEntities.GetContext();

            StringBuilder sb = new StringBuilder();
            
            // проверка на валидность даты с её созданием для дальшейшей прверки
            DateTime.TryParse(dpOrder.Text, out DateTime orderDate);
            DateTime.TryParse(dpDelivery.Text, out DateTime deliveryDate);

            if (cmbUser.SelectedIndex == -1)
                sb.AppendLine("Пользователь должен быть указан");
            if (cmbArticle.SelectedIndex == -1)
                sb.AppendLine("Продукт должен быть указан");
            if (cmbPickap.SelectedIndex == -1)
                sb.AppendLine("Пункт выдачи должен быть указан");
            if(string.IsNullOrEmpty(dpOrder.Text))
                sb.AppendLine("Дата заказа должна быть указана");
            if (string.IsNullOrEmpty(dpDelivery.Text))
                sb.AppendLine("Дата выдачи должна быть указана");
            if (orderDate > deliveryDate)
                sb.AppendLine("Дата выдачи не может быть раньше даты заказа");
            if (cmbStatus.SelectedIndex == -1)
                sb.AppendLine("Статус заказа должен быть указан");

            if (sb.Length != 0)
            {
                MessageBox.Show($"Исправьте все ошибки для сохранения: \n {sb.ToString()}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
