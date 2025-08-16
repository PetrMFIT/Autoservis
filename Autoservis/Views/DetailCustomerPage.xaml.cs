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

namespace Autoservis.Views
{
    /// <summary>
    /// Interaction logic for DetailCustomerPage.xaml
    /// </summary>
    public partial class DetailCustomerPage : Page
    {
        private Customer _customer;
        public DetailCustomerPage(Customer customer)
        {
            InitializeComponent();
            _customer = customer;

            LoadUI();

        }

        private void LoadUI()
        {
            LoadCustomers();
            LoadCars();
            LoadOrders();

            SetReadOnly(true);
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Content = null;
                mainWindow.MainFrame.Visibility = Visibility.Collapsed;
            }
        }
    }
}
