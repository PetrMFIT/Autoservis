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
        public MainWindow()
        {
            InitializeComponent();

            customer_repo = new CustomerRepository(App.DbContext);

            //LoadCustomers();
        }

        /*private void LoadCustomers()
        {
            var customers = customer_repo.GetAll();

            CustomerGrid.ItemsSource = customers;
        }*/

        private void Button_Click()
        {

        }
    }
}