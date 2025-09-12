using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Autoservis.Views
{
    /// <summary>
    /// Interaction logic for AddWorkWindow.xaml
    /// </summary>
    public partial class AddWorkWindow : Window
    {
        private ObservableCollection<Work> _works;
        public Work work { get; private set; }
        public AddWorkWindow(ObservableCollection<Work> tempWorks)
        {
            InitializeComponent();

            _works = tempWorks;
        }

        private void AddWorkButton_Click(object sender, RoutedEventArgs e)
        {
            work = new Work
            {
                Hours = int.TryParse(WorkHoursBox.Text, out int hours) ? hours : 0,
                Price = int.TryParse(WorkPriceBox.Text, out int price) ? price : 0
            };

            MessageBox.Show("Prace pridana.");
            this.DialogResult = true;
            this.Close();
        }
    }
}
