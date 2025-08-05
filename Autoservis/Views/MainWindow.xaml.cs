using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autoservis.Models;
using Autoservis.Repositories;

namespace Autoservis
{
    public partial class MainWindow : Window
    {
        private readonly CustomerRepository customer_repo;
        private readonly CarRepository car_repo;
        private readonly OrderRepository order_repo;

        public enum ViewType
        {
            Customers,
            Cars,
            Orders
        }

        private ViewType currentView;

        public MainWindow()
        {
            InitializeComponent();

            customer_repo = new CustomerRepository(App.DbContext);
            car_repo = new CarRepository(App.DbContext);
            order_repo = new OrderRepository(App.DbContext);

            LoadCustomers();
        }

        // Load customer list
        private void CustomersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            currentView = ViewType.Customers;
            SetupCustomerColumns();
            DataGrid.ItemsSource = customer_repo.GetAll(); ;
        }
        private void SetupCustomerColumns()
        {
            DataGrid.Columns.Clear();
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Jméno", Binding = new Binding("Name") });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Telefon", Binding = new Binding("Phone") });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "E-mail", Binding = new Binding("Email") });
        }

        // Load car list
        private void CarsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCars();
        }

        private void LoadCars()
        {
            currentView = ViewType.Cars;
            SetupCarColumns();
            DataGrid.ItemsSource = car_repo.GetAll();
        }

        private void SetupCarColumns()
        {
            DataGrid.Columns.Clear();
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Model", Binding = new Binding("BrandModel") });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "SPZ", Binding = new Binding("SPZ") });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Rok", Binding = new Binding("Year") });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Majitel", Binding = new Binding("Customer.Name") });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Typ", Binding = new Binding("Type") });
        }

        // SearchBar
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim();

            switch (currentView)
            {
                case ViewType.Customers:
                    var allCustomers = customer_repo.GetAll();
                    DataGrid.ItemsSource = FilterCustomers(allCustomers, query);
                    break;
                case ViewType.Cars:
                    var allCars = car_repo.GetAll();
                    DataGrid.ItemsSource = FilterCars(allCars, query);
                    break;
                case ViewType.Orders:
                    var allOrders = order_repo.GetAll();
                    DataGrid.ItemsSource = FilterOrders(allOrders, query);
                    break;
            }
        }

        private IEnumerable<Customer> FilterCustomers(IEnumerable<Customer> allCustomers, string query)
        {
            query = query.ToLower();
            return allCustomers.Where(c =>
                c.Name.ToLower().Contains(query) ||
                c.Phone.ToLower().Contains(query) ||
                c.Email.ToLower().Contains(query) ||
                c.Address.ToLower().Contains(query) ||
                c.ZIP.ToLower().Contains(query) ||
                c.Cars.Any(car =>
                    car.BrandModel.ToLower().Contains(query) ||
                    car.SPZ.ToLower().Contains(query) ||
                    car.VIN.ToLower().Contains(query)
                )
            );
        }

        private IEnumerable<Car> FilterCars(IEnumerable<Car> allCars, string query)
        {
            query = query.ToLower();
            return allCars.Where(c =>
                c.BrandModel.ToLower().Contains(query) ||
                c.SPZ.ToLower().Contains(query) ||
                c.VIN.ToLower().Contains(query) ||
                c.Year.ToString().Contains(query) ||
                (c.Customer != null && c.Customer.Name.ToLower().Contains(query))
            );
        }

        private IEnumerable<Order> FilterOrders(IEnumerable<Order> allOrders, string query)
        {
            query = query.ToLower();
            return allOrders.Where(o =>
                o.Date.ToString().Contains(query) ||
                o.Name.ToLower().Contains(query) ||
                o.State.ToLower().Contains(query) ||
                (o.Customer != null && o.Customer.Name.ToLower().Contains(query))
            );
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton_Click(sender, new RoutedEventArgs());

                e.Handled = true;
            }
        }
    }
}