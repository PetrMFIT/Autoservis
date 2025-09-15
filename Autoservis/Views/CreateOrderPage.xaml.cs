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
        private readonly OrderRepository order_repo;

        private Customer? _selectedCustomer;
        private Car? _selectedCar;

        private ObservableCollection<Material> tempMaterials = new ObservableCollection<Material>();
        private ObservableCollection<Work> tempWorks = new ObservableCollection<Work>();

        public CreateOrderPage()
        {
            InitializeComponent();

            material_repo = new MaterialRepository(App.DbContext);
            work_repo = new WorkRepository(App.DbContext);
            customer_repo = new CustomerRepository(App.DbContext);
            car_repo = new CarRepository(App.DbContext);
            order_repo = new OrderRepository(App.DbContext);

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
        private void SearchedCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
        private void SearchedCar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SearchedCars.Visibility = Visibility.Collapsed;
            ClearSelectedCarButton.Visibility = Visibility.Visible;

            if (SearchedCars.SelectedItem is Car car)
            {
                _selectedCar = car;
                CarBrandModelBlock.Text = car.BrandModel;
                CarSpzBlock.Text = car.SPZ;
                CarYearBlock.Text = car.Year.ToString();
            }

        }
        private void ClearSelectedCarButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedCar = null;

            ClearSelectedCarButton.Visibility = Visibility.Hidden;
            CarBrandModelBlock.Text = "";
            CarSpzBlock.Text = "";
            CarYearBlock.Text = "";
        }

        // Material
        private void LoadMaterials()
        {
            OrderMaterialDataGrid.ItemsSource = tempMaterials;
            TotalPrice();
        }
        private void AddMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            var addMaterialWindow = new AddMaterialWindow();
            bool? result = addMaterialWindow.ShowDialog();

            if (result == true && addMaterialWindow.material != null)
            {
                tempMaterials.Add(addMaterialWindow.material);
                LoadMaterials();
            }
        }

        private void LoadWorks()
        {
            OrderWorkDataGrid.ItemsSource = tempWorks;
            TotalPrice();
        }
        private void AddWorkButton_Click(object sender, RoutedEventArgs e)
        {
            var addWorkWindow = new AddWorkWindow();
            bool? result = addWorkWindow.ShowDialog();

            if (result == true && addWorkWindow.work != null)
            {
                tempWorks.Add(addWorkWindow.work);
                LoadWorks();
            }
        }

        private void TotalPrice()
        {
            var materialSum = 0;
            foreach (var material in tempMaterials)
            {
                materialSum = materialSum + material.TotalPrice;
            }

            var workSum = 0;
            foreach(var work in tempWorks)
            {
                workSum = workSum + work.TotalPrice;
            }

            TotalPriceFillBlock.Text = $"{materialSum} + {workSum} = {materialSum + workSum}";
        }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var order = new Order
            {
                Date = OrderDatePicker.SelectedDate.Value,
                Name = OrderNameBox.Text,
                State = (State)OrderStateComboBox.SelectedItem,
                CustomerId = _selectedCustomer.Id,
                CarId = _selectedCar.Id
            };
            order_repo.Add(order);

            foreach (var material in tempMaterials)
            {
                material.OrderId = order.Id;
                material_repo.Add(material);
            }

            foreach (var work in tempWorks)
            {
                work.OrderId = order.Id;
                work_repo.Add(work);
            }

            MessageBox.Show("Zakazka ulozena.");
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Content = null;
                mainWindow.MainFrame.Visibility = Visibility.Collapsed;
                mainWindow.LoadOrders();
            }
        }
    }
}
