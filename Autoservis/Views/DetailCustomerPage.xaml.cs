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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autoservis.Models;
using Autoservis.Repositories;

namespace Autoservis.Views
{
    /// <summary>
    /// Interaction logic for DetailCustomerPage.xaml
    /// </summary>
    public partial class DetailCustomerPage : Page
    {
        private Customer _customer;

        private CustomerRepository customer_repo; 
        public DetailCustomerPage(Customer customer)
        {
            InitializeComponent();
            _customer = customer;

            customer_repo = new CustomerRepository(App.DbContext);

            LoadUI();

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
            ConfirmUpdateButton.Visibility = Visibility.Visible;
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
    }
}
