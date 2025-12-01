using Microsoft.Win32;
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

namespace SupaShoes.Pages
{
    /// <summary>
    /// Логика взаимодействия для UpdateProductPage.xaml
    /// </summary>
    public partial class UpdateProductPage : Page
    {
        private Products _product;
        private Users _user;
        public UpdateProductPage(Products product, Users user)
        {
            InitializeComponent();
            _product = product;
            _user = user;
            LoadPage(product);
        }
        private void LoadPage(Products product)
        {
            img.Source = new BitmapImage(new Uri(product.ImgPath)); 
            tbName.Text = product.name;
            tbPrice.Text = product.price.ToString();
            cmbSupplier.SelectedIndex = (int)product.supplier_id - 1;
            cmbManufacturer.SelectedIndex = (int)product.manufacturer_id - 1;
            cmbCategory.SelectedIndex = (int)product.category_id - 1;
            tbDiscount.Text = product.discount.ToString();
            tbCount.Text = product.count.ToString();
            tbDescription.Text = product.description;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // вопрос перед невозвратным действием
            if (MessageBox.Show("Вы уверены что хотите удалить продукт", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var context = SuperShoesEntities.GetContext();

                try
                {
                    context.Products.Remove(_product);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошабка: {ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Успешное удаление", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new ProductsPage(_user));
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!TrySave())
                return;

            var context = SuperShoesEntities.GetContext();

            _product.name = tbName.Text;
            _product.price = decimal.Parse(tbPrice.Text);
            _product.supplier_id = cmbSupplier.SelectedIndex + 1;
            _product.manufacturer_id = cmbManufacturer.SelectedIndex + 1;
            _product.category_id = cmbCategory.SelectedIndex + 1;
            _product.discount = int.Parse(tbDiscount.Text);
            _product.count = int.Parse(tbCount.Text);
            _product.description = tbDescription.Text;

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
            NavigationService.Navigate(new ProductsPage(_user));
        }

        private bool TrySave()
        {
            StringBuilder sb = new StringBuilder();

            if (tbName.Text.Length > 50 || tbName.Text.Length < 3)
                sb.AppendLine("Наименование продукта должно быть длиннее 3 символов и короче 3-х");
            if(!decimal.TryParse(tbPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var price) || price < 0)
                sb.AppendLine("Цена должна быть числом, и не может быть меньше 0");
            if(cmbSupplier.SelectedIndex == -1)
                sb.AppendLine("Поставщик должен быть указан");
            if (cmbManufacturer.SelectedIndex == -1)
                sb.AppendLine("Производитель должен быть указан");
            if (cmbCategory.SelectedIndex == -1)
                sb.AppendLine("Категория должна быть выбрана");
            if (!int.TryParse(tbDiscount.Text, out int  disc) || disc > 99 || disc < 0)
                sb.AppendLine("Скидка должна быть числом, не меньше 0 и не больше 99");
            if (!int.TryParse(tbCount.Text, out var count) || count < 0)
                sb.AppendLine("Количество на складе должно быть числом, и не может быть меньше 0");
            if(string.IsNullOrWhiteSpace(tbDescription.Text))
                sb.AppendLine("Описание должна быть указано");

            if (sb.Length != 0)
            {
                MessageBox.Show($"Исправьте все ошибки для сохранения: \n {sb.ToString()}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void btnEnterImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog getImageDialog = new OpenFileDialog();

            getImageDialog.Filter = "Файлы изображений: (*.png, *.jpg, *.jpeg)| *.png; *.jpg; *.jpeg";
            getImageDialog.InitialDirectory = "C:\\Users\\Mickey\\source\\repos\\SupaShoes\\SupaShoes\\Resources\\";

            if (getImageDialog.ShowDialog() == true)
            {
                _product.image = getImageDialog.SafeFileName;
                img.Source = new BitmapImage(new Uri(getImageDialog.FileName));
            }
        }
    }
}
