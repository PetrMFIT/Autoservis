using Autoservis.Data;
using Autoservis.Enums;
using Autoservis.Models;
using Autoservis.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Autoservis.Views
{
    public partial class DetailCarPage : Page
    {
        private AppDbContext _context;
        private Car _car;
        private Page _previousPage;

        private CarRepository car_repo;
        private OrderRepository order_repo;
        private CustomerRepository customer_repo;

        private Customer _selectedOwner;

        public DetailCarPage(Car car, Page previousPage = null)
        {
            InitializeComponent();

            _context = new AppDbContext();

            if (car != null && car.Id != 0)
            {
                _car = _context.Cars.Include(c => c.Customer).FirstOrDefault(c => c.Id == car.Id);
            }
            else
            {
                _car = car ?? new Car();
                if (_car.CustomerId != 0)
                {
                    _car.Customer = _context.Customers.FirstOrDefault(c => c.Id == _car.CustomerId);
                }
            }

            _previousPage = previousPage;

            car_repo = new CarRepository(_context);
            order_repo = new OrderRepository(_context);
            customer_repo = new CustomerRepository(_context);

            LoadUI();
        }

        private void LoadUI()
        {
            SetupCombos();

            if (_previousPage != null)
            {
                BtnBack.Visibility = Visibility.Visible;
                BtnClose.Visibility = Visibility.Collapsed;
            }
            else
            {
                BtnBack.Visibility = Visibility.Collapsed;
                BtnClose.Visibility = Visibility.Visible;
            }

            if (_car.Id == 0)
            {
                TitleSPZText.Text = "Nové vozidlo";
                TitleModelText.Text = "";
                BtnDelete.Visibility = Visibility.Collapsed;
                if (OrderHistorySection != null) OrderHistorySection.Visibility = Visibility.Collapsed;

                if (_car.CustomerId == 0)
                {
                    if (CustomerSelectionSection != null) CustomerSelectionSection.Visibility = Visibility.Visible;
                    if (CarOwnerPanel != null) CarOwnerPanel.Visibility = Visibility.Collapsed;

                    SetCustomerFormState(isNewMode: false);
                }
                else
                {
                    if (CustomerSelectionSection != null) CustomerSelectionSection.Visibility = Visibility.Collapsed;
                    if (CarOwnerPanel != null) CarOwnerPanel.Visibility = Visibility.Visible;
                    CarOwnerBox.Text = _car.Customer?.Name ?? "Neznámý";
                }
            }
            else
            {
                TitleSPZText.Text = _car.SPZ;
                TitleModelText.Text = _car.BrandModel;
                BtnDelete.Visibility = Visibility.Visible;
                if (OrderHistorySection != null) OrderHistorySection.Visibility = Visibility.Visible;
                if (CustomerSelectionSection != null) CustomerSelectionSection.Visibility = Visibility.Collapsed;

                if (CarOwnerPanel != null) CarOwnerPanel.Visibility = Visibility.Visible;
                CarOwnerBox.Text = _car.Customer?.Name ?? "Neznámý";

                CarBrandModelBox.Text = _car.BrandModel;
                CarSPZBox.Text = _car.SPZ;
                CarVINBox.Text = _car.VIN;
                CarYearComboBox.SelectedItem = _car.Year > 0 ? _car.Year : DateTime.Now.Year;
                CarFuelComboBox.SelectedItem = _car.Fuel;
                CarTypeComboBox.SelectedItem = _car.Type;
                CarDisplacementPowerBox.Text = _car.DisplacementPower;
                CarNotesBox.Text = _car.Notes;

                LoadOrders();
            }
        }

        private void SetupCombos()
        {
            CarYearComboBox.Items.Clear();
            for (int year = DateTime.Now.Year; year >= 1900; year--)
            {
                CarYearComboBox.Items.Add(year);
            }
            var fuelItems = new List<object>();
            fuelItems.AddRange(Enum.GetValues(typeof(FuelType)).Cast<object>());
            fuelItems.Add("Ostatní");
            CarFuelComboBox.ItemsSource = fuelItems;

            var typeItems = new List<object>();
            typeItems.AddRange(Enum.GetValues(typeof(CarType)).Cast<object>());
            typeItems.Add("Ostatní");
            CarTypeComboBox.ItemsSource = typeItems;
        }

        private void RadioMode_Checked(object sender, RoutedEventArgs e)
        {
            if (RadioExisting == null || RadioNew == null) return;
            SetCustomerFormState(isNewMode: RadioNew.IsChecked == true);
        }

        private void SetCustomerFormState(bool isNewMode)
        {
            if (PanelSearch == null) return;

            PanelSearch.Visibility = isNewMode ? Visibility.Collapsed : Visibility.Visible;

            bool isReadOnly = !isNewMode;

            CustNameBox.IsReadOnly = isReadOnly;
            CustPhoneBox.IsReadOnly = isReadOnly;
            CustEmailBox.IsReadOnly = isReadOnly;
            CustAddressBox.IsReadOnly = isReadOnly;
            CustZIPBox.IsReadOnly = isReadOnly;
            CustNotesBox.IsReadOnly = isReadOnly;

            if (isNewMode)
            {
                ClearCustomerForm();
                _selectedOwner = null;
            }
        }

        private void ClearCustomerForm()
        {
            CustNameBox.Text = "";
            CustPhoneBox.Text = "";
            CustEmailBox.Text = "";
            CustAddressBox.Text = "";
            CustZIPBox.Text = "";
            CustNotesBox.Text = "";
        }

        private void SearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private void SearchCustomerBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) PerformSearch();
        }

        private void PerformSearch()
        {
            string query = SearchCustomerBox.Text.ToLower().Trim();
            if (string.IsNullOrEmpty(query)) return;

            var results = customer_repo.GetAll()
                .Where(c => c.Name.ToLower().Contains(query) || c.Phone.Contains(query))
                .ToList();

            CustomerSearchResults.ItemsSource = results;
            PopupSearchResults.IsOpen = true;
        }

        private void CustomerSearchResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CustomerSearchResults.SelectedItem is Customer cust)
            {
                _selectedOwner = cust;

                CustNameBox.Text = cust.Name;
                CustPhoneBox.Text = cust.Phone;
                CustEmailBox.Text = cust.Email;
                CustAddressBox.Text = cust.Address;
                CustZIPBox.Text = cust.ZIP;
                CustNotesBox.Text = cust.Notes;

                PopupSearchResults.IsOpen = false;
                SearchCustomerBox.Text = "";
            }
        }

        private void LoadOrders()
        {
            if (_car.Id == 0) return;
            var orders = order_repo.GetAll().Where(o => o.CarId == _car.Id).ToList();
            OrderList.ItemsSource = orders;
        }

        // --- KLIKNUTÍ NA MAJITELE ---
        private void CarOwnerBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (_car != null && _car.Customer != null)
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    // Přejdeme na detail zákazníka a pošleme 'this' (DetailCarPage) jako předchozí stránku,
                    // aby se dalo vrátit zpět k autu.
                    mainWindow.MainFrame.Navigate(new DetailCustomerPage(_car.Customer, this));
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CarSPZBox.Text) || string.IsNullOrWhiteSpace(CarBrandModelBox.Text))
            {
                MessageBox.Show("SPZ a Model jsou povinné údaje.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_car.Id == 0 && _car.CustomerId == 0)
                {
                    if (RadioExisting.IsChecked == true)
                    {
                        if (_selectedOwner == null)
                        {
                            MessageBox.Show("Vyberte majitele ze seznamu.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        _car.CustomerId = _selectedOwner.Id;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(CustNameBox.Text))
                        {
                            MessageBox.Show("Vyplňte jméno zákazníka.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        var newCustomer = new Customer
                        {
                            Name = CustNameBox.Text,
                            Phone = CustPhoneBox.Text,
                            Email = CustEmailBox.Text,
                            Address = CustAddressBox.Text,
                            ZIP = CustZIPBox.Text,
                            Notes = CustNotesBox.Text
                        };
                        customer_repo.Add(newCustomer);
                        _car.CustomerId = newCustomer.Id;
                    }
                }

                _car.BrandModel = CarBrandModelBox.Text;
                _car.SPZ = CarSPZBox.Text;
                _car.VIN = CarVINBox.Text;
                _car.Year = CarYearComboBox.SelectedItem is int y ? y : DateTime.Now.Year;
                _car.Fuel = CarFuelComboBox.SelectedItem is FuelType f ? f : (FuelType?)null;
                _car.Type = CarTypeComboBox.SelectedItem is CarType t ? t : (CarType?)null;
                _car.DisplacementPower = CarDisplacementPowerBox.Text;
                _car.Notes = CarNotesBox.Text;

                if (_car.Id == 0) car_repo.Add(_car);
                else car_repo.Update(_car);

                MessageBox.Show("Vozidlo uloženo.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                GoBackOrClose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba: " + ex.Message);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Opravdu smazat vozidlo {_car.SPZ}?", "Smazat", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                car_repo.Delete(_car.Id);
                GoBackOrClose();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) { GoBackOrClose(); }
        private void BtnClose_Click(object sender, RoutedEventArgs e) { CloseDetail(); }

        private void GoBackOrClose()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                if (_previousPage != null)
                {
                    if (_previousPage is DetailCustomerPage custPage)
                    {
                        custPage.RefreshData();
                    }
                    mainWindow.MainFrame.Navigate(_previousPage);
                }
                else CloseDetail();
            }
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

        private void OrderList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrderList.SelectedItem is Order order)
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.OverlaySection.Visibility = Visibility.Visible;
                    mainWindow.OverlayFrame.Navigate(new DetailOrderPage(order, this));
                }
            }
        }

        private void CustomerPhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                // Odpojíme event, abychom nezpůsobili nekonečnou smyčku
                tb.TextChanged -= CustomerPhoneBox_TextChanged;

                // 1. Získáme jen číslice a zapamatujeme si pozici kurzoru
                int cursorPosition = tb.SelectionStart;
                string raw = new string(tb.Text.Where(char.IsDigit).ToArray());

                // 2. Sestavíme text s mezerami po 3 znacích
                StringBuilder formatted = new StringBuilder();
                for (int i = 0; i < raw.Length; i++)
                {
                    if (i > 0 && i % 3 == 0) formatted.Append(" ");
                    formatted.Append(raw[i]);
                }

                string result = formatted.ToString();
                int oldLength = tb.Text.Length;
                tb.Text = result;

                // 3. Korekce kurzoru (aby neodskakoval)
                int newLength = result.Length;
                tb.SelectionStart = Math.Max(0, cursorPosition + (newLength - oldLength));

                tb.TextChanged += CustomerPhoneBox_TextChanged;
            }
        }
    }
}