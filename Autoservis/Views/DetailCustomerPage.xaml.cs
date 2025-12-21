using Autoservis.Data;
using Autoservis.Models;
using Autoservis.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
    /// Interaction logic for DetailCustomerPage.xaml
    /// </summary>
    public partial class DetailCustomerPage : Page
    {
        private AppDbContext _context;

        private Customer _customer;

        private CustomerRepository customer_repo;
        private OrderRepository order_repo;
        private MaterialRepository material_repo;
        private WorkRepository work_repo;
        public DetailCustomerPage(Customer customer)
        {
            InitializeComponent();

            _context = new AppDbContext();

            _customer = customer;

            customer_repo = new CustomerRepository(_context);
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
            LoadCustomers();
            LoadCars();
            LoadOrders();

            SetReadOnly(true);
            CarList.IsReadOnly = true;
            OrderList.IsReadOnly = true;
        }

        private void LoadCustomers()
        {
            CustomerNameBox.Text = _customer.Name;
            CustomerPhoneBox.Text = _customer.Phone;
            CustomerEmailBox.Text = _customer.Email;
            CustomerAddressBox.Text = _customer.Address;
            CustomerZIPBox.Text = _customer.ZIP;
            CustomerNotesBox.Text = _customer.Notes;
        }

        private void LoadCars()
        {
            CarList.ItemsSource = _customer.Cars;
        }

        private void LoadOrders()
        {
            OrderList.ItemsSource = _customer.Orders;
            var orders = order_repo.GetAll().Where(o => o.CustomerId == _customer.Id).ToList();

            foreach (var order in orders)
            {
                order.Materials = material_repo.GetAll().Where(m => m.OrderId == order.Id).ToList();
                order.Works = work_repo.GetAll().Where(w => w.OrderId == order.Id).ToList();
            }

            OrderList.ItemsSource = orders;
        }

        private void SetReadOnly(bool readOnly)
        {
            CustomerNameBox.IsReadOnly = readOnly;
            CustomerPhoneBox.IsReadOnly = readOnly;
            CustomerEmailBox.IsReadOnly = readOnly;
            CustomerAddressBox.IsReadOnly = readOnly;
            CustomerZIPBox.IsReadOnly = readOnly;
            CustomerNotesBox.IsReadOnly = readOnly;
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
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseDetail();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Opravdu chcete smazat zákazníka {_customer.Name}?",
                            "Potvrzení smazání",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                customer_repo.Delete(_customer.Id);
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
            _customer.Name = CustomerNameBox.Text;
            _customer.Phone = CustomerPhoneBox.Text;
            _customer.Email = CustomerEmailBox.Text;
            _customer.Address = CustomerAddressBox.Text;
            _customer.ZIP = CustomerZIPBox.Text;
            _customer.Notes = CustomerNotesBox.Text;

            customer_repo.Update(_customer);
            CloseDetail();
        }
        private void CancelUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            SetReadOnly(true);
            UpdateButtons.Visibility = Visibility.Collapsed;
        }

        private void CarList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CarList.SelectedItem is Car car)
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.MainFrame.Visibility = Visibility.Visible;
                    mainWindow.MainFrame.Navigate(new DetailCarPage(car));
                }
                
            }
            
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
