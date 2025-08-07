using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Autoservis.Enums;
using Autoservis.Models;
using Autoservis.Repositories;
using Autoservis.Views;
using Autoservis.Data;

namespace Autoservis.Views
{
    public partial class CreateOrderPage : Page
    {
        private readonly MaterialRepository material_repo;
        private readonly WorkRepository work_repo;

        public CreateOrderPage()
        {
            InitializeComponent();

            material_repo = new MaterialRepository(App.DbContext);
            work_repo = new WorkRepository(App.DbContext);

            LoadUI();
        }

        private void LoadUI()
        {
            // Datepicker
            OrderDatePicker.SelectedDate = DateTime.Now;

            // States
            var items = new List<object>();
            items.AddRange(Enum.GetValues(typeof(State)).Cast<object>());
            items.Add("Ostatní");

            OrderStateComboBox.ItemsSource = items;

            OrderStateComboBox.SelectedIndex = 0;


            OrderMaterialDataGrid.ItemsSource = material_repo.GetAll();

            OrderWorkDataGrid.ItemsSource = work_repo.GetAll();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Content = null;
                mainWindow.MainFrame.Visibility = Visibility.Collapsed;
            }
        }
    }
}
