using Autoservis.Data;
using Autoservis.Enums;
using Autoservis.Models;
using Autoservis.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <summary>
    /// Interaction logic for DetailCarPage.xaml
    /// </summary>
    public partial class DetailCarPage : Page
    {
        private AppDbContext _context;

        private Car _car;

        private CarRepository car_repo;
        private OrderRepository order_repo;
        private MaterialRepository material_repo;
        private WorkRepository work_repo;
        public DetailCarPage(Car car)
        {
            InitializeComponent();

            _context = new AppDbContext();

            _car = car;

            car_repo = new CarRepository(_context);
            order_repo = new OrderRepository(_context);
            material_repo = new MaterialRepository(_context);
            work_repo = new WorkRepository(_context);

            this.Unloaded += OnUnloaded;

            LoadUI();
        }
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _context.Dispose();
        }


        private void LoadUI()
        {
            LoadCars();
            LoadOrders();
            for (int year = DateTime.Now.Year; year >= 1900; year--)
            {
                CarYearComboBox.Items.Add(year);
            }
            CarYearComboBox.SelectedIndex = 0;

            SetupCarFuelComboBox();
            SetupCarTypeComboBox();

            SetReadOnly(true);
            OrderList.IsReadOnly = true;
        }

        private void LoadCars()
        {
            CarBrandModelBox.Text = _car.BrandModel;
            CarSPZBox.Text = _car.SPZ;
            CarVINBox.Text = _car.VIN;
            CarOwnerBox.Text = _car.Customer.Name;
            CarYearBox.Text = _car.Year.ToString();
            CarFuelBox.Text = _car.Fuel.ToString();
            CarTypeBox.Text = _car.Type.ToString();
            CarDisplacementPowerBox.Text = _car.DisplacementPower;
            CarNotesBox.Text = _car.Notes;
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

        private void LoadOrders()
        {
            OrderList.ItemsSource = _car.Orders;
            var orders = order_repo.GetAll().Where(o => o.CarId == _car.Id).ToList();

            foreach (var order in orders)
            {
                order.Materials = material_repo.GetAll().Where(m => m.OrderId == order.Id).ToList();
                order.Works = work_repo.GetAll().Where(w => w.OrderId == order.Id).ToList();
            }

            OrderList.ItemsSource = orders;
        }

        private void SetReadOnly(bool readOnly)
        {
            CarBrandModelBox.IsReadOnly = readOnly;
            CarSPZBox.IsReadOnly = readOnly;
            CarVINBox.IsReadOnly = readOnly;
            CarYearBox.IsReadOnly = readOnly;
            CarFuelBox.IsReadOnly = readOnly;
            CarTypeBox.IsReadOnly = readOnly;

            if (!readOnly)
            {
                CarYearBox.Visibility = Visibility.Collapsed;
                CarYearComboBox.Visibility = Visibility.Visible;

                CarFuelBox.Visibility = Visibility.Collapsed;
                CarFuelComboBox.Visibility = Visibility.Visible;

                CarTypeBox.Visibility = Visibility.Collapsed;
                CarTypeComboBox.Visibility = Visibility.Visible;
            }
            else
            {
                CarYearBox.Visibility = Visibility.Visible;
                CarYearComboBox.Visibility = Visibility.Collapsed;

                CarFuelBox.Visibility = Visibility.Visible;
                CarFuelComboBox.Visibility = Visibility.Collapsed;

                CarTypeBox.Visibility = Visibility.Visible;
                CarTypeComboBox.Visibility = Visibility.Collapsed;
            }

                CarDisplacementPowerBox.IsReadOnly = readOnly;
            CarNotesBox.IsReadOnly = readOnly;
        }

        private void CloseDetail()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Content = null;
                mainWindow.MainFrame.Visibility = Visibility.Collapsed;
                mainWindow.LoadCars();
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseDetail();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Opravdu chcete smazat auto s SPZ: {_car.SPZ}?",
                            "Potvrzení smazání",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                car_repo.Delete(_car.Id);
                CloseDetail();
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            SetReadOnly(false);
            UpdateButtons.Visibility = Visibility.Visible;
        }

        private void ConfirmUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            _car.BrandModel = CarBrandModelBox.Text;
            _car.SPZ = CarSPZBox.Text;
            _car.VIN = CarVINBox.Text;
            //_car.Customer.Name = CarOwnerBox.Text;
            _car.Year = (int)CarYearComboBox.SelectedItem;
            _car.Fuel = CarFuelComboBox.SelectedItem is FuelType fuel ? fuel : null;
            _car.Type = CarTypeComboBox.SelectedItem is CarType type ? type : null;
            _car.DisplacementPower = CarDisplacementPowerBox.Text;
            _car.Notes = CarNotesBox.Text;

            car_repo.Update(_car);
            CloseDetail();
        }

        private void CancelUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            SetReadOnly(true);
            UpdateButtons.Visibility = Visibility.Collapsed;
        }

        private void OrderList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrderList.SelectedItem is Order order)
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.MainFrame.Visibility = Visibility.Visible;
                    mainWindow.MainFrame.Navigate(new DetailOrderPage(order));
                }
            }
        }
    }
}
