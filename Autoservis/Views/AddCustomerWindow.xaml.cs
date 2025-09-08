using Autoservis.Enums;
using Autoservis.Models;
using Autoservis.Repositories;
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
using Autoservis.Views;

namespace Autoservis.Views
{
    /// <summary>
    /// Interaction logic for AddCustomerWindow.xaml
    /// </summary>
    public partial class AddCustomerWindow : Window
    {
        private readonly CustomerRepository customer_repo;
        private readonly CarRepository car_repo;

        public AddCustomerWindow()
        {
            InitializeComponent();

            customer_repo = new CustomerRepository(App.DbContext);
            car_repo = new CarRepository(App.DbContext);

            LoadUI();
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

            CarFuelComboBox.SelectedItem = -1;
        }

        private void SetupCarTypeComboBox()
        {
            var items = new List<object>();
            items.AddRange(Enum.GetValues(typeof(CarType)).Cast<object>());
            items.Add("Ostatní");

            CarTypeComboBox.ItemsSource = items;

            CarTypeComboBox.SelectedItem = -1;
        }

        private void AddCustomerToDbButton_Click(object sender, RoutedEventArgs e)
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

            var newCustomer = new Customer
            {
                Name = CustomerNameBox.Text,
                Phone = CustomerPhoneBox.Text,
                Email = CustomerEmailBox.Text,
                Address = CustomerAddressBox.Text,
                ZIP = CustomerZIPBox.Text,
                Notes = CustomerNotesBox.Text
            };

            customer_repo.Add(newCustomer);

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

            MessageBox.Show("Zákazník uložen.");
            this.DialogResult = true;
            this.Close();
        }
    }
}
