using Autoservis.Data;
using Autoservis.Enums;
using Autoservis.Models;
using Autoservis.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Autoservis.Views
{
    public partial class DetailCustomerPage : Page
    {
        private AppDbContext _context;
        private Customer _customer;

        private Page _previousPage;

        private CustomerRepository customer_repo;
        private OrderRepository order_repo;
        private CarRepository car_repo;
        private MaterialRepository material_repo;
        private WorkRepository work_repo;

        public DetailCustomerPage(Customer customer, Page prevoiusPage = null)
        {
            InitializeComponent();

            _context = new AppDbContext();

            _previousPage = prevoiusPage;

            if (customer != null && customer.Id != 0)
            {
                _customer = _context.Customers.FirstOrDefault(c => c.Id == customer.Id);
                if (_customer == null) _customer = new Customer();
            }
            else
            {
                _customer = new Customer();
            }

            customer_repo = new CustomerRepository(_context);
            order_repo = new OrderRepository(_context);
            car_repo = new CarRepository(_context);
            material_repo = new MaterialRepository(_context);
            work_repo = new WorkRepository(_context);

            LoadUI();
        }

        // Tuto metodu volá DetailCarPage, když se vrátí zpět
        public void RefreshData()
        {
            LoadCars();
            LoadOrders();
        }

        private void LoadUI()
        {
            // 1. TLAČÍTKA ZPĚT / ZAVŘÍT
            if (_previousPage != null)
            {
                BtnBack.Visibility = Visibility.Visible;
                BtnClose.Visibility = Visibility.Collapsed;
            }
            else
            {
                BtnBack.Visibility = Visibility.Collapsed;
                BtnClose.Visibility = Visibility.Visible;
            }

            // 2. DATA
            if (_customer.Id == 0)
            {
                TitleNameText.Text = "Nový zákazník";
                BtnDelete.Visibility = Visibility.Collapsed;

                if (HistoryTablesSection != null) HistoryTablesSection.Visibility = Visibility.Collapsed;

                if (NewCarSection != null)
                {
                    NewCarSection.Visibility = Visibility.Visible;
                    SetupNewCarInputs();
                }
            }
            else
            {
                TitleNameText.Text = _customer.Name;
                BtnDelete.Visibility = Visibility.Visible;

                if (NewCarSection != null) NewCarSection.Visibility = Visibility.Collapsed;
                if (HistoryTablesSection != null) HistoryTablesSection.Visibility = Visibility.Visible;

                if (BtnAddCar != null) BtnAddCar.Visibility = Visibility.Visible;
                if (BtnAddOrder != null) BtnAddOrder.Visibility = Visibility.Visible;

                CustomerNameBox.Text = _customer.Name;
                CustomerPhoneBox.Text = _customer.Phone;
                CustomerEmailBox.Text = _customer.Email;
                CustomerAddressBox.Text = _customer.Address;
                CustomerZIPBox.Text = _customer.ZIP;
                CustomerNotesBox.Text = _customer.Notes;

                LoadCars();
                LoadOrders();
            }
        }

        private void SetupNewCarInputs()
        {
            NewCarYearComboBox.Items.Clear();
            for (int year = DateTime.Now.Year; year >= 1900; year--)
            {
                NewCarYearComboBox.Items.Add(year);
            }
            NewCarYearComboBox.SelectedIndex = 0;

            var fuelItems = new List<object>();
            fuelItems.AddRange(Enum.GetValues(typeof(FuelType)).Cast<object>());
            fuelItems.Add("Ostatní");
            NewCarFuelComboBox.ItemsSource = fuelItems;
            NewCarFuelComboBox.SelectedIndex = 0;

            var typeItems = new List<object>();
            typeItems.AddRange(Enum.GetValues(typeof(CarType)).Cast<object>());
            typeItems.Add("Ostatní");
            NewCarTypeComboBox.ItemsSource = typeItems;
            NewCarTypeComboBox.SelectedIndex = 0;
        }

        private void LoadCars()
        {
            var cars = car_repo.GetAll().Where(c => c.CustomerId == _customer.Id).ToList();
            CarList.ItemsSource = cars;
        }

        private void LoadOrders()
        {
            var orders = order_repo.GetAll().Where(o => o.CustomerId == _customer.Id).ToList();

            foreach (var order in orders)
            {
                order.Materials = material_repo.GetAll().Where(m => m.OrderId == order.Id).ToList();
                order.Works = work_repo.GetAll().Where(w => w.OrderId == order.Id).ToList();
            }
            OrderList.ItemsSource = orders;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CustomerNameBox.Text))
            {
                MessageBox.Show("Jméno zákazníka musí být vyplněno.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isNew = _customer.Id == 0;

            if (isNew)
            {
                if (string.IsNullOrWhiteSpace(NewCarModelBox.Text) || string.IsNullOrWhiteSpace(NewCarSPZBox.Text))
                {
                    MessageBox.Show("Při zakládání zákazníka musíte vyplnit Značku/Model a SPZ vozidla.",
                                    "Chybí údaje vozidla", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            _customer.Name = CustomerNameBox.Text;
            _customer.Phone = CustomerPhoneBox.Text;
            _customer.Email = CustomerEmailBox.Text;
            _customer.Address = CustomerAddressBox.Text;
            _customer.ZIP = CustomerZIPBox.Text;
            _customer.Notes = CustomerNotesBox.Text;

            try
            {
                if (isNew)
                {
                    customer_repo.Add(_customer);

                    var newCar = new Car
                    {
                        BrandModel = NewCarModelBox.Text,
                        SPZ = NewCarSPZBox.Text,
                        VIN = NewCarVINBox.Text ?? "",
                        Year = NewCarYearComboBox.SelectedItem is int y ? y : DateTime.Now.Year,
                        Fuel = NewCarFuelComboBox.SelectedItem is FuelType fuel ? fuel : (FuelType?)null,
                        Type = NewCarTypeComboBox.SelectedItem is CarType type ? type : (CarType?)null,
                        DisplacementPower = NewCarPowerBox.Text ?? "",
                        Notes = NewCarNotesBox.Text ?? "",
                        CustomerId = _customer.Id
                    };

                    car_repo.Add(newCar);
                }
                else
                {
                    customer_repo.Update(_customer);
                }

                MessageBox.Show("Zákazník a data uložena.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                if (NewCarSection != null) NewCarSection.Visibility = Visibility.Collapsed;
                if (HistoryTablesSection != null) HistoryTablesSection.Visibility = Visibility.Visible;

                TitleNameText.Text = _customer.Name;
                BtnDelete.Visibility = Visibility.Visible;
                if (BtnAddCar != null) BtnAddCar.Visibility = Visibility.Visible;
                if (BtnAddOrder != null) BtnAddOrder.Visibility = Visibility.Visible;

                LoadCars();
                LoadOrders();

                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.LoadCustomers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddCar_Click(object sender, RoutedEventArgs e)
        {
            var newCar = new Car
            {
                CustomerId = _customer.Id,
                Customer = _customer
            };

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Navigate(new DetailCarPage(newCar, this));
            }
        }

        private void BtnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            var newOrder = new Order
            {
                CustomerId = _customer.Id,
                Customer = _customer,
                Date = DateTime.Now,
                Name = "Nová zakázka"
            };

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Navigate(new DetailOrderPage(newOrder, this));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Opravdu chcete smazat zákazníka {_customer.Name}?\nSmažou se i všechna jeho auta a zakázky!",
                            "Potvrzení smazání",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                customer_repo.Delete(_customer.Id);
                CloseDetail();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow && _previousPage != null)
            {
                mainWindow.MainFrame.Navigate(_previousPage);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDetail();
        }

        private void CloseDetail()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Content = null;
                mainWindow.MainFrame.Visibility = Visibility.Collapsed;
                mainWindow.LoadCustomers();
            }
        }

        private void CarList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CarList.SelectedItem is Car car)
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.MainFrame.Navigate(new DetailCarPage(car, this));
                }
            }
        }

        private void OrderList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrderList.SelectedItem is Order order)
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.OverlaySection.Visibility = Visibility.Visible;
                    mainWindow.OverlayFrame.Navigate(new DetailOrderPage(order, this));
                }
            }
        }

        private void CustomerPhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                // Odpojíme event, abychom nezpůsobili nekonečnou smyčku
                tb.TextChanged -= CustomerPhoneBox_TextChanged;

                // 1. Získáme jen číslice a zapamatujeme si pozici kurzoru
                int cursorPosition = tb.SelectionStart;
                string raw = new string(tb.Text.Where(char.IsDigit).ToArray());

                // 2. Sestavíme text s mezerami po 3 znacích
                StringBuilder formatted = new StringBuilder();
                for (int i = 0; i < raw.Length; i++)
                {
                    if (i > 0 && i % 3 == 0) formatted.Append(" ");
                    formatted.Append(raw[i]);
                }

                string result = formatted.ToString();
                int oldLength = tb.Text.Length;
                tb.Text = result;

                // 3. Korekce kurzoru (aby neodskakoval)
                int newLength = result.Length;
                tb.SelectionStart = Math.Max(0, cursorPosition + (newLength - oldLength));

                tb.TextChanged += CustomerPhoneBox_TextChanged;
            }
        }
    }
}