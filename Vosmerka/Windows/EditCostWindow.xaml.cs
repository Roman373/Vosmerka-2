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
using System.Windows.Shapes;

namespace Vosmerka.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditCostWindow.xaml
    /// </summary>
    public partial class EditCostWindow : Window
    {
        public decimal result = 0;
        public EditCostWindow(decimal AvgCost)
        {
            InitializeComponent();
            TBoxCost.Text = AvgCost.ToString();
        }

        private void BtnEditCost_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                result = decimal.Parse(TBoxCost.Text);
                DialogResult = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Стоимость должна быть положительным числом;");
            }
        }
    }
}
