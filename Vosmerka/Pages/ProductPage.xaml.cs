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
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        public ProductPage()
        {
            InitializeComponent();
            UpdateProduct();
            CBoxSortBy.SelectedIndex = 0;
            var typeProduct = App.Context.ProductType.ToList();
            typeProduct.Insert(0, new Entities.ProductType
            {
                Title = "Все типы"
            });
            CBoxTypeProduct.ItemsSource = typeProduct;
            CBoxTypeProduct.SelectedIndex = 0;
        }
        private void UpdateProduct()
        {
            var product = App.Context.Product.ToList();

            if (CBoxSortBy.SelectedIndex == 0)
                product = product.OrderBy(p => p.Title).ToList();
            if (CBoxSortBy.SelectedIndex == 1)
                product = product.OrderByDescending(p => p.Title).ToList();
            if (CBoxSortBy.SelectedIndex == 2)
                product = product.OrderBy(p => p.ProductionWorkshopNumber).ToList();
            if (CBoxSortBy.SelectedIndex == 3)
                product = product.OrderByDescending(p => p.ProductionWorkshopNumber).ToList();
            if (CBoxSortBy.SelectedIndex == 4)
                product = product.OrderBy(p => p.MinCostForAgent).ToList();
            if (CBoxSortBy.SelectedIndex == 5)
                product = product.OrderByDescending(p => p.MinCostForAgent).ToList();

            if (CBoxTypeProduct.SelectedIndex == 1)
                product = product.Where(p => p.ProductTypeID == 1).ToList();
            if (CBoxTypeProduct.SelectedIndex == 2)
                product = product.Where(p => p.ProductTypeID == 2).ToList();
            if (CBoxTypeProduct.SelectedIndex == 3)
                product = product.Where(p => p.ProductTypeID == 3).ToList();
            if (CBoxTypeProduct.SelectedIndex == 4)
                product = product.Where(p => p.ProductTypeID == 4).ToList();

            product = product.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            LBoxProduct.ItemsSource = product.Skip(currentPage*sizePage).Take(sizePage);
        }
        private void CBoxSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void CBoxTypeProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).DataContext as Entities.Product;
            NavigationService.Navigate(new AddEditProductPage(product));
            UpdateProduct();
        }
        public List<Entities.Product> products = App.Context.Product.ToList();
        public int totalPage=0, currentPage=0, sizePage = 20;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var countProduct = App.Context.Product.Count();
            totalPage = countProduct / sizePage;
            if (countProduct % sizePage != 0)
                totalPage++;
            ShowCurrentPage();
            TextTotalPage.Text = totalPage.ToString();
            UpdateProduct();
        }
        void ShowCurrentPage()
        {
            TextCurrentPage.Text = (currentPage + 1).ToString();
        }
        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditProductPage());
            UpdateProduct();
        }

        private void BtnEditCost_Click(object sender, RoutedEventArgs e)
        {
            decimal sum = 0;
            List<int> IdList = new List<int>();

            foreach(var product in LBoxProduct.SelectedItems.Cast<Entities.Product>())
            {
                sum += product.MinCostForAgent;
                IdList.Add(product.ID);
            }
            var editCostWindow = new Windows.EditCostWindow(sum / LBoxProduct.SelectedItems.Count);

            editCostWindow.ShowDialog();

            if ((bool)editCostWindow.DialogResult)
            {
                SetEditCost(IdList,editCostWindow.result);
            }
        }
        void SetEditCost(List<int> ProductIds,decimal newCost)
        {
            foreach(int ProductId in ProductIds)
            {
                foreach(Entities.Product product in LBoxProduct.SelectedItems.Cast<Entities.Product>())
                {
                    if (ProductId == product.ID)
                    {
                        product.MinCostForAgent = newCost;
                        App.Context.SaveChanges();
                        NavigationService.Refresh();
                    }
                }
                
            }MessageBox.Show("Стоимость изменен;", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPage - 1)
            {
                currentPage++;
                NavigationService.Refresh();
            }
            ShowCurrentPage();
        }

        private void BtnLastPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPage - 1)
            {
                currentPage=totalPage-1;
                NavigationService.Refresh();
            }
            ShowCurrentPage();
        }

        private void BtnBackPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                NavigationService.Refresh();
            }
            ShowCurrentPage();
        }

        private void BtnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > totalPage - 1)
            {
                currentPage=0;
                NavigationService.Refresh();
            }
            ShowCurrentPage();
        }
        public int countSelectedProduct = 0;
        private void LBoxProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            countSelectedProduct = LBoxProduct.SelectedItems.Count;
            if (countSelectedProduct > 0)
                BtnEditCost.Visibility = Visibility.Visible;
            else
                BtnEditCost.Visibility = Visibility.Collapsed;
        }
    }
}
