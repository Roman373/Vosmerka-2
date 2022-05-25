using Microsoft.Win32;
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

namespace Vosmerka.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditProductPage.xaml
    /// </summary>
    public partial class AddEditProductPage : Page
    {
        public Entities.Product _currentProduct = null;
        public string _imageMain;
        public AddEditProductPage()
        {
            InitializeComponent();
            CBoxProductType.ItemsSource = App.Context.ProductType.ToList();
        }
        public AddEditProductPage(Entities.Product product)
        {
            InitializeComponent();
            Title = "Редактирование продукта";
            _currentProduct = product;
            TBoxTitle.Text = _currentProduct.Title;
            TBoxArticle.Text =_currentProduct.ArticleNumber;
            CBoxProductType.SelectedIndex =(int)_currentProduct.ProductTypeID-1;
            CBoxProductType.ItemsSource =App.Context.ProductType.ToList();
            TBoxDescription.Text =_currentProduct.Description;
            TBoxPersonCount.Text =_currentProduct.ProductionPersonCount.ToString();
            TBoxWorkshopNumber.Text =_currentProduct.ProductionWorkshopNumber.ToString();
            TBoxMinCostForAgent.Text =_currentProduct.MinCostForAgent.ToString();
            if (_currentProduct != null)
            {
                ImageProduct.Source = new BitmapImage(new Uri(_currentProduct.Image, UriKind.RelativeOrAbsolute));
                _imageMain = _currentProduct.Image;
            }
            
        }
        private void BtnSelectedImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                _imageMain = "/products/" + ofd.SafeFileName;
                ImageProduct.Source = new BitmapImage(new Uri(_imageMain, UriKind.RelativeOrAbsolute));
            }
        }
        private string CheckError()
        {
            var errorBuilder = new StringBuilder();

            if (string.IsNullOrWhiteSpace(TBoxTitle.Text))
                errorBuilder.AppendLine("Название обязательно для заполнения;");

            var productForDB = App.Context.Product.FirstOrDefault(p => p.Title.ToLower() == TBoxTitle.Text.ToLower());
            if (productForDB != null && productForDB != _currentProduct)
                errorBuilder.AppendLine("Такой продукта уже есть в базе;");

            var articleForDB = App.Context.Product.FirstOrDefault(p => p.ArticleNumber.ToLower() == TBoxArticle.Text.ToLower());
            if (articleForDB != null && articleForDB != _currentProduct)
                errorBuilder.AppendLine("Артикул занят в базе;");

            int personCount = 0;
            if (int.TryParse(TBoxPersonCount.Text, out personCount) == false || personCount < 0)
                errorBuilder.AppendLine("Количество людей должно быть положительное число;");

            int workshopNumber = 0;
            if (int.TryParse(TBoxWorkshopNumber.Text, out workshopNumber) == false || workshopNumber < 0)
                errorBuilder.AppendLine("Номер цеха должен быть положительным числом;");

            decimal minCostForAgent = 0;
            if (decimal.TryParse(TBoxMinCostForAgent.Text, out minCostForAgent) == false || minCostForAgent < 0)
                errorBuilder.AppendLine("Стоимость должен быть положительным числом;");

            if (errorBuilder.Length > 0)
                errorBuilder.Insert(0, "Устраните следующие ошибки:\n");

            return errorBuilder.ToString();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var errorMessage = CheckError();

            if (errorMessage.Length > 0)
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (_currentProduct == null)
                {
                    var product = new Entities.Product
                    {
                        Title=TBoxTitle.Text,
                        ArticleNumber=TBoxArticle.Text,
                        ProductTypeID=int.Parse(CBoxProductType.SelectedValue.ToString()),
                        Description=TBoxDescription.Text,
                        ProductionPersonCount=int.Parse(TBoxPersonCount.Text),
                        ProductionWorkshopNumber=int.Parse(TBoxWorkshopNumber.Text),
                        MinCostForAgent=decimal.Parse(TBoxMinCostForAgent.Text),
                        Image=_imageMain,
                    };
                    App.Context.Product.Add(product);
                    App.Context.SaveChanges();
                    MessageBox.Show($"Продукт {product.Title} добавлен", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _currentProduct.Title =TBoxTitle.Text;
                    _currentProduct.ArticleNumber =TBoxArticle.Text;
                    _currentProduct.Description =TBoxDescription.Text;
                    _currentProduct.ProductTypeID =int.Parse(CBoxProductType.SelectedValue.ToString());
                    _currentProduct.ProductionPersonCount =int.Parse(TBoxPersonCount.Text);
                    _currentProduct.ProductionWorkshopNumber =int.Parse(TBoxWorkshopNumber.Text);
                    _currentProduct.MinCostForAgent =decimal.Parse(TBoxMinCostForAgent.Text);
                    _currentProduct.Image =_imageMain;
                    App.Context.SaveChanges();
                    MessageBox.Show($"Продукт {_currentProduct.Title} изменен", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                NavigationService.GoBack();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show($"Вы уверены, что хотите удалить продукт: {_currentProduct.Title}?", 
                "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var materialProduct = App.Context.ProductMaterial.Cast<Entities.ProductMaterial>();
                foreach(Entities.ProductMaterial productMaterial in materialProduct)
                {
                    if (_currentProduct.ID == productMaterial.MaterialID)
                    {
                        App.Context.ProductMaterial.Remove(productMaterial);
                        MessageBox.Show($"ПродуктМатериал {productMaterial.MaterialID & productMaterial.ProductID}  удален", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                App.Context.Product.Remove(_currentProduct);
                App.Context.SaveChanges();
                NavigationService.GoBack();
                MessageBox.Show($"Продукт {_currentProduct.Title} удален", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
