using Autoservis.Data;
using Autoservis.Enums;
using Autoservis.Migrations;
using Autoservis.Models;
using Autoservis.Repositories;
using Autoservis.Views;
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

namespace Autoservis.Views
{
    public partial class CreateOrderPage : Page
    {
        private readonly MaterialRepository material_repo;
        private readonly WorkRepository work_repo;
        private readonly CustomerRepository customer_repo;
        private readonly CarRepository car_repo;

        private Customer? _selectedCustomer;

        public CreateOrderPage()
        {
            InitializeComponent();

            material_repo = new MaterialRepository(App.DbContext);
            work_repo = new WorkRepository(App.DbContext);
            customer_repo = new CustomerRepository(App.DbContext);
            car_repo = new CarRepository(App.DbContext);

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

        // Search Customer
        private void CustomerSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = CustomerSearchBox.Text.Trim();

            var allCustomers = customer_repo.GetAll();
            SearchedCustomers.ItemsSource = FilterCustomers(allCustomers, query);
            SearchedCustomers.Visibility = Visibility.Visible;
        }

        private IEnumerable<Customer> FilterCustomers(IEnumerable<Customer> allCustomers, string query)
        {
            query = query.ToLower();
            return allCustomers.Where(c =>
                c.Name.ToLower().Contains(query) ||
                c.Phone.ToLower().Contains(query) ||
                c.Email.ToLower().Contains(query) ||
                c.Address.ToLower().Contains(query) ||
                c.ZIP.ToLower().Contains(query)
            );
        }

        // Search Cars
        private void CarSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = CarSearchBox.Text.Trim();

            var allCars = car_repo.GetAll();
            SearchedCars.ItemsSource = FilterCars(allCars, query);
            SearchedCars.Visibility = Visibility.Visible;
        }

        private IEnumerable<Car> FilterCars(IEnumerable<Car> allCars, string query)
        {
            query = query.ToLower();
            return allCars.Where(c =>
                c.BrandModel.ToLower().Contains(query) ||
                c.SPZ.ToLower().Contains(query) ||
                c.VIN.ToLower().Contains(query) ||
                c.Year.ToString().Contains(query)
            );
        }


        private void SearchedCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SearchedCustomers.Visibility = Visibility.Collapsed;
            ClearSelectedCustomerButton.Visibility = Visibility.Visible;

            if (SearchedCustomers.SelectedItem is Customer customer)
            {
                _selectedCustomer = customer;
                CustomerNameBlock.Text = customer.Name;
                CustomerPhoneBlock.Text = customer.Phone;
                CustomerEmailBlock.Text = customer.Email;
                CustomerAddressBlock.Text = customer.Address;
                CustomerZipBlock.Text = customer.ZIP;
            }

        }

        /*private void SetReadOnly(bool readOnly)
        {
            var boxes = new[]
            {
                CustomerNameBox,
                CustomerPhoneBox,
                CustomerEmailBox,
                CustomerAddressBox,
                CustomerZIPBox,
                CustomerNotesBox
            };

            foreach (var box in boxes)
            {
                box.IsReadOnly = readOnly;
                if (readOnly)
                {
                    box.BorderThickness = new Thickness(0);
                    box.Background = Brushes.Transparent;
                }
                else
                {
                    box.BorderThickness = new Thickness(1);
                    box.Background = Brushes.White;
                    box.Text = "";
                }
            }
        }*/

        private void ClearSelectedCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedCustomer = null;

            ClearSelectedCustomerButton.Visibility = Visibility.Hidden;
            CustomerNameBlock.Text = "";
            CustomerPhoneBlock.Text = "";
            CustomerEmailBlock.Text = "";
            CustomerAddressBlock.Text = "";
            CustomerZipBlock.Text = "";
        }
    }
}
