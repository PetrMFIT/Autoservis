using Autoservis.Enums;
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
using Autoservis.Models;
using System.Collections.ObjectModel;

namespace Autoservis.Views
{
    /// <summary>
    /// Interaction logic for AddMaterialView.xaml
    /// </summary>
    public partial class AddMaterialWindow : Window
    {
        private ObservableCollection<Material> _materials;

        public Material material { get; private set; }
        public AddMaterialWindow(ObservableCollection<Material> tempMaterials)
        {
            InitializeComponent();
            SetupSupplierComboBox();

            _materials = tempMaterials;

            LoadUI();
        }

        private void LoadUI()
        {
            SetupUnitComboBox();
        }

        private void SetupUnitComboBox()
        {
            var items = new List<object>();
            items.AddRange(Enum.GetValues(typeof(MeasureUnit)).Cast<object>());
            items.Add("Ostatní");

            MaterialUnitComboBox.ItemsSource = items;
            MaterialUnitComboBox.SelectedIndex = 0;
        }

        private void SetupSupplierComboBox()
        {
            var items = new List<object>();
            items.AddRange(Enum.GetValues(typeof(MaterialSupplier)).Cast<object>());
            items.Add("Ostatní");

            MaterialSupplierComboBox.ItemsSource = items;
            MaterialSupplierComboBox.SelectedIndex = 0;
        }

        private void AddAMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            material = new Material
            {
                Name = MaterialNameBox.Text,
                Code = MaterialCodeBox.Text,
                Quantity = int.TryParse(MaterialQuantityBox.Text, out int qty) ? qty : 0,
                Supplier = (MaterialSupplier)MaterialSupplierComboBox.SelectedItem,
                Price = int.TryParse(MaterialPriceBox.Text, out int price) ? price : 0,
                Unit = (MeasureUnit)MaterialUnitComboBox.SelectedItem
            };

            MessageBox.Show("Material pridan.");
            this.DialogResult = true;
            this.Close();

        }
    }
}
