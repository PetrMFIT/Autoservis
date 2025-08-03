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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CustomerRepository customer_repo;
        private readonly CarRepository car_repo;
        public MainWindow()
        {
            InitializeComponent();

            customer_repo = new CustomerRepository(App.DbContext);
            car_repo = new CarRepository(App.DbContext);

            LoadCustomers();
        }

        private void CustomersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            SetupCustomerColumns();
            DataGrid.ItemsSource = customer_repo.GetAll(); ;
        }

        private void CarsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCars();
        }

        private void LoadCars()
        {
            SetupCarColumns();
            DataGrid.ItemsSource = car_repo.GetAll();
        }

        private void SetupCustomerColumns()
        {
            DataGrid.Columns.Clear();
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Jméno", Binding = new Binding("Name") });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Telefon", Binding = new Binding("Phone") });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "E-mail", Binding = new Binding("Email") });
        }

        private void SetupCarColumns()
        {
            DataGrid.Columns.Clear();
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Model", Binding = new Binding("BrandModel") });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "SPZ", Binding = new Binding("SPZ") });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Rok", Binding = new Binding("Year") });
        }

    }
}