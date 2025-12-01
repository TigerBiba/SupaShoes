using Microsoft.Win32;
using SupaShoes.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    /// Логика взаимодействия для AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Page
    {
        private Users _user;
        private Products _newProduct;
        public AddProduct(Users user)
        {
            InitializeComponent();
            _newProduct = new Products();

            img.Source = new BitmapImage(new Uri(_newProduct.ImgPath));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!TrySave())
                return;

            var context = SuperShoesEntities.GetContext();

            _newProduct.article = tbArticle.Text;
            _newProduct.name = tbName.Text;
            _newProduct.unit_id = 1;
            _newProduct.price = decimal.Parse(tbPrice.Text, CultureInfo.InvariantCulture);
            _newProduct.supplier_id = cmbSupplier.SelectedIndex + 1;
            _newProduct.manufacturer_id = cmbManufacturer.SelectedIndex + 1;
            _newProduct.category_id = cmbCategory.SelectedIndex + 1;
            _newProduct.discount = int.Parse(tbDiscount.Text);
            _newProduct.count = int.Parse(tbCount.Text);
            _newProduct.description = tbDescription.Text;

            try
            {
                context.Products.Add(_newProduct);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошабка: {ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Успешное создание продукта", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.Navigate(new ProductsPage(_user));
        }

        private bool TrySave()
        {
            var context = SuperShoesEntities.GetContext();

            StringBuilder sb = new StringBuilder();

            if(tbArticle.Text.Length > 10 || tbArticle.Text.Length < 3 || context.Products.FirstOrDefault(x => x.article == tbArticle.Text) != null)
                sb.AppendLine("Артикуль должен быть длинной не меньше 3х символов и не больше 10-ти символов, или товар с данным артикулом уже существует");
            if (tbName.Text.Length > 50 || tbName.Text.Length < 3)
                sb.AppendLine("Наименование продукта должно быть длиннее 3 символов и короче 3-х");
            if (!decimal.TryParse(tbPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var price) || price < 0)
                sb.AppendLine("Цена должна быть числом, и не может быть меньше 0");
            if (cmbSupplier.SelectedIndex == -1)
                sb.AppendLine("Поставщик должен быть указан");
            if (cmbManufacturer.SelectedIndex == -1)
                sb.AppendLine("Производитель должен быть указан");
            if (cmbCategory.SelectedIndex == -1)
                sb.AppendLine("Категория должна быть выбрана");
            if (!int.TryParse(tbDiscount.Text, out int disc) || disc > 99 || disc < 0)
                sb.AppendLine("Скидка должна быть числом, не меньше 0 и не больше 99");
            if (!int.TryParse(tbCount.Text, out var count) || count < 0)
                sb.AppendLine("Количествомна складе должно быть числом, и не может быть меньше 0");
            if (string.IsNullOrWhiteSpace(tbDescription.Text))
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
            //Открытие диаалогового кнна выбора фотографии
            OpenFileDialog getImageDialog = new OpenFileDialog();

            getImageDialog.Filter = "Файлы изображений: (*.png, *.jpg, *.jpeg)| *.png; *.jpg; *.jpeg"; // фильтры для расширения файлов
            getImageDialog.InitialDirectory = "C:\\Users\\Mickey\\source\\repos\\SupaShoes\\SupaShoes\\Resources\\"; // путь открытия папки

            if (getImageDialog.ShowDialog() == true)
            {
                _newProduct.image = getImageDialog.SafeFileName;
                img.Source = new BitmapImage(new Uri(getImageDialog.FileName));
            }
        }
    }
}
