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
    /// Логика взаимодействия для UpdateOrderPage.xaml
    /// </summary>
    public partial class UpdateOrderPage : Page
    {
        private Orders _order;
        private Users _user;
        public UpdateOrderPage(Orders order, Users user)
        {
            InitializeComponent();
            _order = order;
            _user = user;
            GetComboBoxItems();
            Load();
        }

        private void Load()
        {
            cmbUser.SelectedIndex = (int)_order.user_id - 1;
            cmbStatus.SelectedIndex = (int)_order.order_status_id - 1;
            cmbPickap.SelectedIndex = (int)_order.pickup_point_id - 1;
            dpOrder.Text = _order.order_date.ToString();
            dpDelivery.Text = _order.delivery_date.ToString();
        }

        private void GetComboBoxItems()
        {
            var context = SuperShoesEntities.GetContext();

            // заполнение комбобоксов
            cmbUser.ItemsSource = context.Users.Select(x => x.email).ToList();

            cmbPickap.ItemsSource = context.PickapPoints.Select(x => x.adress).ToList();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Вы уверены что хотите удалить заказ", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var context = SuperShoesEntities.GetContext();

                var contents = context.OrderContents.Where(x => x.order_id == _order.id).ToList();

                foreach(var item in contents)
                {
                    try
                    {
                        context.OrderContents.Remove(item);
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошабка: {ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                try
                {
                    context.Orders.Remove(_order);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошабка: {ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Успешное удаление заказа", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new OrdersPage(_user));
            }

            return;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!TrySave())
                return;

            var context = SuperShoesEntities.GetContext();

            _order.pickup_point_id = cmbPickap.SelectedIndex + 1;
            _order.user_id = cmbUser.SelectedIndex + 1;
            _order.delivery_date = DateTime.Parse(dpDelivery.Text);
            _order.order_date = DateTime.Parse(dpOrder.Text);
            _order.order_status_id = cmbStatus.SelectedIndex + 1;

            try
            {
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

            DateTime.TryParse(dpOrder.Text, out DateTime orderDate);
            DateTime.TryParse(dpDelivery.Text, out DateTime deliveryDate);

            if (cmbUser.SelectedIndex == -1)
                sb.AppendLine("Пользователь должен быть указан");
            if (cmbPickap.SelectedIndex == -1)
                sb.AppendLine("Пункт выдачи должен быть указан");
            if (string.IsNullOrEmpty(dpOrder.Text))
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
