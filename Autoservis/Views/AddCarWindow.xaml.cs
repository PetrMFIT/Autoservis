using Autoservis.Enums;
using Autoservis.Models;
using Autoservis.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Autoservis.Data;

namespace Autoservis.Views
{
    /// <summary>
    /// Interaction logic for AddCarWindow.xaml
    /// </summary>
    public partial class AddCarWindow : Window
    {
        private AppDbContext _context;

        private readonly CustomerRepository customer_repo;
        private readonly CarRepository car_repo;

        private Customer? _selectedCustomer;

        public AddCarWindow()
        {
            InitializeComponent();

            _context = new AppDbContext();

            customer_repo = new CustomerRepository(_context);
            car_repo = new CarRepository(_context);

            LoadUI();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }

        private void LoadUI()
        {
            for (int year = DateTime.Now.Year; year >= 1900; year--)
            {
                CarYearComboBox.Items.Add(year);
            }
            CarYearComboBox.SelectedIndex = 0;

            SetupCarFuelComboBox();
            SetupCarTypeComboBox();

        }

        private void SetupCarFuelComboBox()
        {
            var items = new List<object>();
            items.AddRange(Enum.GetValues(typeof(FuelType)).Cast<object>());
            items.Add("Ostatní");

            CarFuelComboBox.ItemsSource = items;

            CarFuelComboBox.SelectedIndex = 2;
        }

        private void SetupCarTypeComboBox()
        {
            var items = new List<object>();
            items.AddRange(Enum.GetValues(typeof(CarType)).Cast<object>());
            items.Add("Ostatní");

            CarTypeComboBox.ItemsSource = items;

            CarTypeComboBox.SelectedIndex = 4;
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            AddCustomerLayout.Visibility = Visibility.Visible;
        }

        private void AddCarToDbButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CustomerNameBox.Text) ||
                string.IsNullOrWhiteSpace(CarBrandModelBox.Text) ||
                string.IsNullOrWhiteSpace(CarSPZBox.Text)
                )
            {
                MessageBox.Show(
                           "Prosím vyplňte všechna povinná pole",
                           "Neúplné údaje",
                           MessageBoxButton.OK,
                           MessageBoxImage.Warning
                       );
                CustomerNameText.Foreground = new SolidColorBrush(Colors.Red);
                CarBrandModelText.Foreground = new SolidColorBrush(Colors.Red);
                CarSPZText.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }

            Customer newCustomer;

            if (_selectedCustomer != null)
            {
                newCustomer = _selectedCustomer;
            }
            else
            {
                newCustomer = new Customer
                {
                    Name = CustomerNameBox.Text,
                    Phone = CustomerPhoneBox.Text,
                    Email = CustomerEmailBox.Text,
                    Address = CustomerAddressBox.Text,
                    ZIP = CustomerZIPBox.Text,
                    Notes = CustomerNotesBox.Text
                };
                customer_repo.Add(newCustomer);
            }

            var newCar = new Car
            {
                BrandModel = CarBrandModelBox.Text,
                SPZ = CarSPZBox.Text,
                VIN = CarVINBox.Text,
                Year = (int)CarYearComboBox.SelectedItem,
                Fuel = CarFuelComboBox.SelectedItem is FuelType fuel ? fuel : null,
                Type = CarTypeComboBox.SelectedItem is CarType type ? type : null,
                DisplacementPower = CarDisplacementPowerBox.Text,
                Notes = CarNotesBox.Text,
                CustomerId = newCustomer.Id
            };

            car_repo.Add(newCar);

            MessageBox.Show("Auto uloženo.");
            this.DialogResult = true;
            this.Close();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim();

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

        private void SearchedCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SearchedCustomers.Visibility = Visibility.Collapsed;
            ClearSelectedButton.Visibility = Visibility.Visible;

            if (SearchedCustomers.SelectedItem is Customer customer)
            {
                _selectedCustomer = customer;
                CustomerNameBox.Text = customer.Name;
                CustomerPhoneBox.Text = customer.Phone;
                CustomerEmailBox.Text = customer.Email;
                CustomerAddressBox.Text = customer.Address;
                CustomerZIPBox.Text = customer.ZIP;
                CustomerNotesBox.Text = customer.Notes;
            }

            SetReadOnly(true);

        }

        private void SetReadOnly(bool readOnly)
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
        }

        private void ClearSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedCustomer = null;

            SetReadOnly(false);
            ClearSelectedButton.Visibility = Visibility.Hidden;
        }
    }
}

