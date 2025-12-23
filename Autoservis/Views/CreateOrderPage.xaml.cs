using Autoservis.Data;
using Autoservis.Enums;
using Autoservis.Models;
using Autoservis.Repositories;
using Autoservis.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Autoservis.Views
{
    public partial class CreateOrderPage : Page
    {
        private readonly AppDbContext _context;
        private readonly MaterialRepository material_repo;
        private readonly WorkRepository work_repo;
        private readonly CustomerRepository customer_repo;
        private readonly CarRepository car_repo;
        private readonly OrderRepository order_repo;

        private Customer? _selectedCustomer;
        private Car? _selectedCar;

        private ObservableCollection<Material> tempMaterials = new ObservableCollection<Material>();
        private ObservableCollection<Work> tempWorks = new ObservableCollection<Work>();

        public CreateOrderPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            material_repo = new MaterialRepository(_context);
            work_repo = new WorkRepository(_context);
            customer_repo = new CustomerRepository(_context);
            car_repo = new CarRepository(_context);
            order_repo = new OrderRepository(_context);

            this.Unloaded += (s, e) => _context.Dispose();
            LoadUI();
        }

        private void LoadUI()
        {
            OrderDatePicker.SelectedDate = DateTime.Now;
            var items = new List<object>();
            items.AddRange(Enum.GetValues(typeof(State)).Cast<object>());
            OrderStateComboBox.ItemsSource = items;
            OrderStateComboBox.SelectedIndex = 0;

            // Naplnit rok výroby pro nové auto
            for (int i = DateTime.Now.Year; i >= 1950; i--) NewCarYear.Items.Add(i);
            NewCarYear.SelectedIndex = 0;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mw) mw.CloseOverlay();
        }

        // --- PŘEPÍNÁNÍ REŽIMŮ ---
        private void RadioCust_Checked(object sender, RoutedEventArgs e)
        {
            if (RadioCustExisting == null) return;
            bool isExisting = RadioCustExisting.IsChecked == true;

            CustomerSearchPanel.Visibility = isExisting && _selectedCustomer == null ? Visibility.Visible : Visibility.Collapsed;
            SelectedCustomerInfo.Visibility = isExisting && _selectedCustomer != null ? Visibility.Visible : Visibility.Collapsed;
            NewCustomerForm.Visibility = !isExisting ? Visibility.Visible : Visibility.Collapsed;

            if (!isExisting) _selectedCustomer = null; // Reset při přepnutí na nový
        }

        private void RadioCar_Checked(object sender, RoutedEventArgs e)
        {
            if (RadioCarExisting == null) return;
            bool isExisting = RadioCarExisting.IsChecked == true;

            CarSearchPanel.Visibility = isExisting && _selectedCar == null ? Visibility.Visible : Visibility.Collapsed;
            SelectedCarInfo.Visibility = isExisting && _selectedCar != null ? Visibility.Visible : Visibility.Collapsed;
            NewCarForm.Visibility = !isExisting ? Visibility.Visible : Visibility.Collapsed;

            if (!isExisting) _selectedCar = null;
        }

        // --- ZÁKAZNÍK ---
        private void CustomerSearchButton_Click(object sender, RoutedEventArgs e) => PerformCustomerSearch();
        private void CustomerSearchBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) PerformCustomerSearch(); }

        private void PerformCustomerSearch()
        {
            string query = CustomerSearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(query)) return;
            SearchedCustomers.ItemsSource = customer_repo.GetAll().Where(c => c.Name.ToLower().Contains(query) || c.Phone.Contains(query)).ToList();
            CustomerPopup.IsOpen = true;
        }

        private void SearchedCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SearchedCustomers.SelectedItem is Customer customer)
            {
                SelectCustomer(customer);
                CustomerPopup.IsOpen = false;
            }
        }

        private void SelectCustomer(Customer customer)
        {
            _selectedCustomer = customer;
            CustNameTxt.Text = customer.Name;
            CustPhoneTxt.Text = customer.Phone;
            CustEmailTxt.Text = customer.Email;

            // UI Update
            RadioCustExisting.IsChecked = true;
            CustomerSearchPanel.Visibility = Visibility.Collapsed;
            SelectedCustomerInfo.Visibility = Visibility.Visible;

            // OBOUSMĚRNÁ LOGIKA: Pokud nemám auto, nabídni jeho auta
            if (_selectedCar == null)
            {
                var myCars = car_repo.GetAll().Where(c => c.CustomerId == customer.Id).ToList();
                if (myCars.Count == 1) SelectCar(myCars.First());
                else if (myCars.Count > 1)
                {
                    CarSearchBox.Focus();
                    SearchedCars.ItemsSource = myCars;
                    CarPopup.IsOpen = true;
                }
            }
        }

        private void ClearSelectedCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedCustomer = null;
            RadioCust_Checked(null, null); // Refresh visibility
            CustomerSearchBox.Text = "";
        }

        // --- AUTO ---
        private void CarSearchButton_Click(object sender, RoutedEventArgs e) => PerformCarSearch();
        private void CarSearchBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) PerformCarSearch(); }
        private void CarSearchBox_GotFocus(object sender, RoutedEventArgs e) { if (_selectedCustomer != null && string.IsNullOrWhiteSpace(CarSearchBox.Text)) PerformCarSearch(); }

        private void PerformCarSearch()
        {
            string query = CarSearchBox.Text.Trim().ToLower();
            var allCars = car_repo.GetAll();
            List<Car> results;

            if (_selectedCustomer != null)
            {
                var myCars = allCars.Where(c => c.CustomerId == _selectedCustomer.Id);
                results = string.IsNullOrEmpty(query) ? myCars.ToList() : myCars.Where(c => c.BrandModel.ToLower().Contains(query) || c.SPZ.ToLower().Contains(query)).ToList();
            }
            else
            {
                if (string.IsNullOrEmpty(query)) return;
                results = allCars.Where(c => c.BrandModel.ToLower().Contains(query) || c.SPZ.ToLower().Contains(query)).ToList();
            }
            SearchedCars.ItemsSource = results;
            CarPopup.IsOpen = true;
        }

        private void SearchedCar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SearchedCars.SelectedItem is Car car)
            {
                SelectCar(car);
                CarPopup.IsOpen = false;
            }
        }

        private void SelectCar(Car car)
        {
            _selectedCar = car;
            CarModelTxt.Text = car.BrandModel;
            CarSPZTxt.Text = car.SPZ;
            CarVINTxt.Text = car.VIN;

            // UI Update
            RadioCarExisting.IsChecked = true;
            CarSearchPanel.Visibility = Visibility.Collapsed;
            SelectedCarInfo.Visibility = Visibility.Visible;

            // OBOUSMĚRNÁ LOGIKA: Pokud auto má majitele a my nemáme vybráno, vyber ho
            if (_selectedCustomer == null && car.CustomerId != 0)
            {
                var owner = customer_repo.GetAll().FirstOrDefault(c => c.Id == car.CustomerId);
                if (owner != null) SelectCustomer(owner);
            }
        }

        private void ClearSelectedCarButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedCar = null;
            RadioCar_Checked(null, null);
            CarSearchBox.Text = "";
        }

        // --- ZBYTEK ---
        private void AddMaterialButton_Click(object sender, RoutedEventArgs e) { /* ... beze změny ... */ }
        private void AddWorkButton_Click(object sender, RoutedEventArgs e) { /* ... beze změny ... */ }
        private void RecalculateTotal() { /* ... beze změny ... */ }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OrderNameBox.Text)) { MessageBox.Show("Chybí název."); return; }

            // 1. Řešení zákazníka
            int finalCustId = 0;
            if (RadioCustExisting.IsChecked == true)
            {
                if (_selectedCustomer == null) { MessageBox.Show("Vyberte zákazníka."); return; }
                finalCustId = _selectedCustomer.Id;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(NewCustName.Text)) { MessageBox.Show("Vyplňte jméno nového zákazníka."); return; }
                var newC = new Customer { Name = NewCustName.Text, Phone = NewCustPhone.Text, Email = NewCustEmail.Text };
                customer_repo.Add(newC);
                finalCustId = newC.Id;
            }

            // 2. Řešení auta
            int finalCarId = 0;
            if (RadioCarExisting.IsChecked == true)
            {
                if (_selectedCar == null) { MessageBox.Show("Vyberte auto."); return; }
                finalCarId = _selectedCar.Id;

                // Pokud vybrané auto nemá majitele (bylo "volné"), přiřadíme mu tohoto zákazníka?
                // To je otázka business logiky. Pro jistotu to necháme být, nebo bychom museli updatovat auto.
            }
            else
            {
                if (string.IsNullOrWhiteSpace(NewCarModel.Text) || string.IsNullOrWhiteSpace(NewCarSPZ.Text)) { MessageBox.Show("Vyplňte model a SPZ."); return; }
                var newCar = new Car
                {
                    BrandModel = NewCarModel.Text,
                    SPZ = NewCarSPZ.Text,
                    Year = NewCarYear.SelectedItem is int y ? y : DateTime.Now.Year,
                    CustomerId = finalCustId // Nové auto patří tomuto zákazníkovi
                };
                car_repo.Add(newCar);
                finalCarId = newCar.Id;
            }

            // 3. Vytvoření zakázky
            var order = new Order
            {
                Date = OrderDatePicker.SelectedDate ?? DateTime.Now,
                Name = OrderNameBox.Text,
                State = OrderStateComboBox.SelectedItem is State s ? s : State.Rozpracováno,
                CustomerId = finalCustId,
                CarId = finalCarId
            };
            order_repo.Add(order);

            // Uložení položek
            foreach (var m in tempMaterials) { m.OrderId = order.Id; material_repo.Add(m); }
            foreach (var w in tempWorks) { w.OrderId = order.Id; work_repo.Add(w); }

            MessageBox.Show("Zakázka vytvořena.");
            CloseButton_Click(null, null);
        }
    }
}