using Autoservis.Data;
using Autoservis.Enums;
using Autoservis.Migrations;
using Autoservis.Models;
using Autoservis.Repositories;
using Autoservis.Views;
using System.Collections.Generic;
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

namespace Autoservis
{
    public partial class MainWindow : Window
    {
        private AppDbContext _context;

        private readonly CustomerRepository customer_repo;
        private readonly CarRepository car_repo;
        private readonly OrderRepository order_repo;
        
        private readonly MaterialRepository material_repo;
        private readonly WorkRepository work_repo;

        public ViewType currentView;

        public MainWindow()
        {
            InitializeComponent();

            _context = new AppDbContext();

            customer_repo = new CustomerRepository(_context);
            car_repo = new CarRepository(_context);
            order_repo = new OrderRepository(_context);

            material_repo = new MaterialRepository(_context);
            work_repo = new WorkRepository(_context);

            MainFrame.Navigated += (s, e) => RefreshCurrentView();

            LoadCustomers();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }

        private void UpdateUI()
        {
            MainFrame.Content = null;
            MainFrame.Visibility = Visibility.Hidden;

            var activeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00AE91"));
            var inactiveColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D8D8D8"));

            double normalHeight = 35;  // Standardní výška
            double activeHeight = 45;

            // Zákazníci
            CustomersButton.Background = inactiveColor;
            CustomersButton.Height = normalHeight;
            CustomersButton.FontWeight = FontWeights.Normal;

            // Auta
            CarsButton.Background = inactiveColor;
            CarsButton.Height = normalHeight;
            CarsButton.FontWeight = FontWeights.Normal;

            // Zakázky
            OrdersButton.Background = inactiveColor;
            OrdersButton.Height = normalHeight;
            OrdersButton.FontWeight = FontWeights.Normal;
            //Filter.Visibility = (currentView == ViewType.Cars) ? Visibility.Visible : Visibility.Collapsed;
            switch (currentView)
            {
                case ViewType.Customers:
                    CarFilters.Visibility = Visibility.Collapsed;
                    DataListLabel.Content = "Seznam zákazníků";
                    ContextAddButton.Visibility = Visibility.Visible;
                    CustomersButton.Background = activeColor;
                    CustomersButton.Height = activeHeight;       // <--- ZMĚNA VELIKOSTI
                    CustomersButton.FontWeight = FontWeights.Bold;
                    break;
                case ViewType.Cars:
                    CarFilters.Visibility = Visibility.Visible;
                    DataListLabel.Content = "Seznam aut";
                    ContextAddButton.Visibility = Visibility.Visible;
                    CarsButton.Background = activeColor;
                    CarsButton.Background = activeColor;
                    CarsButton.Height = activeHeight;
                    CarsButton.FontWeight = FontWeights.Bold;
                    break;
                case ViewType.Orders:
                    CarFilters.Visibility = Visibility.Collapsed;
                    DataListLabel.Content = "Seznam zakázek";
                    ContextAddButton.Visibility = Visibility.Collapsed;
                    OrdersButton.Background = activeColor;
                    OrdersButton.Height = activeHeight;
                    OrdersButton.FontWeight = FontWeights.Bold;
                    break;
            }
        }

        // Load customer list
        private void CustomersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomers();
        }

        public void LoadCustomers()
        {
            currentView = ViewType.Customers;
            UpdateUI();
            SetupCustomerColumns();
            using (var context = new AppDbContext())
            {
                var repository = new CustomerRepository(context);

                // Načteme čerstvý seznam z databáze
                var customers = repository.GetAll();

                // Nastavíme do tabulky
                DataGrid.ItemsSource = customers;
            }
        }
        private void SetupCustomerColumns()
        {
            DataGrid.Columns.Clear();
            DataGrid.Columns.Add(new DataGridTextColumn 
            { 
                Header = "Jméno", 
                Binding = new Binding("Name"), 
                Width = new DataGridLength(1, DataGridLengthUnitType.Star) 
            });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Telefon", Binding = new Binding("Phone") { StringFormat = "{0:N0}"}, Width = 200 });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "E-mail", Binding = new Binding("Email"), Width = 250 });
        }

        // Load car list
        private void CarsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCars();
        }

        public void LoadCars()
        {
            currentView = ViewType.Cars;
            UpdateUI();
            SetupCarColumns();
            SetupFuelTypeComboBox();
            SetupCarTypeComboBox();
            using (var context = new AppDbContext())
            {
                var repository = new CarRepository(context);

                // Načteme čerstvý seznam z databáze
                var cars = repository.GetAll();

                // Nastavíme do tabulky
                DataGrid.ItemsSource = cars;
            }
        }

        private void SetupCarColumns()
        {
            DataGrid.Columns.Clear();
            DataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Model",
                Binding = new Binding("BrandModel"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "SPZ", Binding = new Binding("SPZ"), Width = 120 });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Rok", Binding = new Binding("Year"), Width = 80 });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Majitel", Binding = new Binding("Customer.Name"), Width = 120 });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Typ", Binding = new Binding("Type"), Width = 120 });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Palivo", Binding = new Binding("Fuel"),Width = 90});
        }

        private void SetupFuelTypeComboBox()
        {
            var items = new List<object>();
            items.Add("Všechny");
            items.AddRange(Enum.GetValues(typeof(FuelType)).Cast<object>());
            items.Add("Ostatní");

            FuelTypeComboBox.ItemsSource = items;

            FuelTypeComboBox.SelectedIndex = 0;
        }

        private void SetupCarTypeComboBox()
        {
            var items = new List<object>();
            items.Add("Všechny");
            items.AddRange(Enum.GetValues(typeof(CarType)).Cast<object>());
            items.Add("Ostatní");

            CarTypeComboBox.ItemsSource = items;

            CarTypeComboBox.SelectedIndex = 0;
        }

        // Load order list
        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        public void LoadOrders()
        {
            currentView = ViewType.Orders;
            UpdateUI();
            SetupOrderColumns();
            using (var context = new AppDbContext())
            {
                // Vytvoříme čerstvé repozitáře uvnitř bloku
                var o_repo = new OrderRepository(context);
                var m_repo = new MaterialRepository(context);
                var w_repo = new WorkRepository(context);

                // 1. Načteme zakázky
                var orders = o_repo.GetAll().ToList();

                // 2. Ke každé zakázce načteme její detaily (aby se správně spočítala TotalPrice)
                foreach (var order in orders)
                {
                    order.Materials = m_repo.GetAll().Where(m => m.OrderId == order.Id).ToList();
                    order.Works = w_repo.GetAll().Where(w => w.OrderId == order.Id).ToList();
                }

                // 3. Přiřadíme do tabulky až po kompletním načtení všeho
                DataGrid.ItemsSource = orders;
            }
        }
        private void SetupOrderColumns()
        {
            DataGrid.Columns.Clear();
            DataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Název",
                Binding = new Binding("Name"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Datum", Binding = new Binding("Date") { StringFormat = "dd.MM.yyyy" }, Width = 120 });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Stav", Binding = new Binding("State"), Width = 150 });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Cena", Binding = new Binding("TotalPrice") { StringFormat = "{0:N0} Kč" }, Width = 120 });
        }

        // SearchBar
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text.Length > 0)
            {
                ClearSearchButton.Visibility = Visibility.Visible;
            }
            else
            {
                ClearSearchButton.Visibility = Visibility.Collapsed;
            }

            PerformSearch();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformSearch();
            }
        }

        private void PerformSearch()
        {
            string query = SearchBox.Text.Trim(); // Vezmeme text z boxu

            // Pokud je prázdno, načteme všechna data (reset)
            if (string.IsNullOrWhiteSpace(query))
            {
                switch (currentView)
                {
                    case ViewType.Customers: LoadCustomers(); break;
                    case ViewType.Cars: LoadCars(); break;
                    case ViewType.Orders: LoadOrders(); break;
                }
                return;
            }

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

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            SearchBox.Focus();
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
                (o.Customer != null && o.Customer.Name.ToLower().Contains(query))
            );
        }

        // Car filters
        private void FuelFilterSelection(object sender, SelectionChangedEventArgs e)
        {
            var allCars = car_repo.GetAll();

            var selected = FuelTypeComboBox.SelectedItem;

            if (selected is FuelType fuel)
            {
                DataGrid.ItemsSource = allCars.Where(c => c.Fuel == fuel);
            } else if (selected.ToString() == "Všechny")
            {
                DataGrid.ItemsSource = allCars;
            } else
            {
                DataGrid.ItemsSource = allCars.Where(c => c.Fuel == null);
            }
        }

        private void CarFilterSelection(object sender, SelectionChangedEventArgs e)
        {
            var allCars = car_repo.GetAll();

            var selected = CarTypeComboBox.SelectedItem;

            if (selected is CarType type)
            {
                DataGrid.ItemsSource = allCars.Where(c => c.Type == type);
            }
            else if (selected.ToString() == "Všechny")
            {
                DataGrid.ItemsSource = allCars;
            }
            else
            {
                DataGrid.ItemsSource = allCars.Where(c => c.Type == null);
            }
        }

        // Add customer
        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            OverlaySection.Visibility = Visibility.Visible;
            OverlayFrame.Navigate(new DetailOrderPage(null));
        }

        private void OverlayBackground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseDetailAndRefresh();
        }

        // Veřejná metoda pro zavření (volaná z CreateOrderPage)
        public void CloseOverlayAndRefresh()
        {
            OverlaySection.Visibility = Visibility.Collapsed;
            OverlayFrame.Content = null;

            // Klíčový krok: Zavoláme existující metodu pro načtení zakázek
            LoadOrders();
        }

        public void CloseDetailAndRefresh()
        {
            MainFrame.Visibility = Visibility.Collapsed;
            MainFrame.Content = null;

            // Podle toho, co zrovna prohlížíme, to refreshneme
            switch (currentView)
            {
                case ViewType.Customers: LoadCustomers(); break;
                case ViewType.Cars: LoadCars(); break;
            }
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new DetailCustomerPage(null));
        }
        private void AddCarButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new DetailCarPage(null));
        }

        private void Datagrid_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedItem == null) return;
            MainFrame.Visibility = Visibility.Visible;
            switch(DataGrid.SelectedItem)
            {
                case Customer customer:
                    MainFrame.Visibility = Visibility.Visible;
                    MainFrame.Navigate(new DetailCustomerPage(customer));
                    break;
                case Car car:
                    MainFrame.Visibility = Visibility.Visible;
                    MainFrame.Navigate(new DetailCarPage(car));
                    break;
                case Order order:
                    OverlaySection.Visibility = Visibility.Visible;
                    OverlayFrame.Navigate(new DetailOrderPage(order));
                    break;
            }
        }

        private void ContextAddButton_Click(object sender, RoutedEventArgs e)
        {
            switch (currentView)
            {
                case ViewType.Customers:
                    AddCustomerButton_Click(sender, e); 
                    break;
                case ViewType.Cars:
                    AddCarButton_Click(sender, e);
                    break;
                case ViewType.Orders:
                    CreateOrderButton_Click(sender, e);
                    break;
            }
        }

        private void RefreshCurrentView()
        {
            // Pokud je Frame skrytý, znamená to, že jsme zpět na hlavní tabulce
            if (MainFrame.Content == null && MainFrame.Visibility == Visibility.Hidden)
            {
                switch (currentView)
                {
                    case ViewType.Customers: LoadCustomers(); break;
                    case ViewType.Cars: LoadCars(); break;
                    case ViewType.Orders: LoadOrders(); break;
                }
            }
        }
    }
}