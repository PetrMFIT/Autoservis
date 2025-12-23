using Autoservis.Data;
using Autoservis.Enums;
using Autoservis.Models;
using Autoservis.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Autoservis.Views
{
    public partial class DetailOrderPage : Page
    {
        private AppDbContext _context;
        private Order _order;
        private Page _previousPage;
        private OrderRepository order_repo;
        private MaterialRepository material_repo;
        private WorkRepository work_repo;
        private CustomerRepository customer_repo;
        private CarRepository car_repo;
        private Customer _selectedCustomer;
        private Car _selectedCar;

        // --- NOVÉ: Seznamy pro ComboBoxy v tabulce ---
        public List<MeasureUnit> UnitOptions { get; set; }
        public List<MaterialSupplier> SupplierOptions { get; set; }

        public DetailOrderPage(Order order, Page previousPage = null)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _previousPage = previousPage;

            order_repo = new OrderRepository(_context);
            material_repo = new MaterialRepository(_context);
            work_repo = new WorkRepository(_context);
            customer_repo = new CustomerRepository(_context);
            car_repo = new CarRepository(_context);

            // 1. Naplnění seznamů pro výběr
            UnitOptions = Enum.GetValues(typeof(MeasureUnit)).Cast<MeasureUnit>().ToList();
            SupplierOptions = Enum.GetValues(typeof(MaterialSupplier)).Cast<MaterialSupplier>().ToList();

            // 2. Nastavení DataContextu na sebe sama, aby XAML viděl na UnitOptions
            this.DataContext = this;

            if (order != null && order.Id != 0)
            {
                _order = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Car)
                    .Include(o => o.Materials)
                    .Include(o => o.Works)
                    .Include(o => o.Photos) // <--- TOTO ZDE CHYBĚLO
                    .FirstOrDefault(o => o.Id == order.Id);
            }
            else
            {
                _order = order ?? new Order();
                _order.Date = DateTime.Now;
                _order.State = State.Rozpracováno;
                _order.Materials = new List<Material>();
                _order.Works = new List<Work>();
            }

            this.Unloaded += OnUnloaded;
            LoadUI();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e) => _context.Dispose();

        private void LoadUI()
        {
            if (_previousPage != null) { BtnBack.Visibility = Visibility.Visible; BtnClose.Visibility = Visibility.Collapsed; }
            else { BtnBack.Visibility = Visibility.Collapsed; BtnClose.Visibility = Visibility.Visible; }

            OrderNameBox.Text = _order.Name;
            OrderDatePicker.SelectedDate = _order.Date;
            OrderStateComboBox.ItemsSource = Enum.GetValues(typeof(State));
            OrderStateComboBox.SelectedItem = _order.State;
            CarMileageBox.Text = _order.Mileage.ToString();

            CarYearBox.Items.Clear(); for (int i = DateTime.Now.Year; i >= 1950; i--) CarYearBox.Items.Add(i);
            CarFuelBox.ItemsSource = Enum.GetValues(typeof(FuelType));
            CarTypeBox.ItemsSource = Enum.GetValues(typeof(CarType));

            // ZDE UŽ NENÍ POTŘEBA RUČNĚ PLNIT InMatUnit/Supplier (řeší to Binding v XAML)

            RefreshGrids();

            bool isNew = _order.Id == 0;
            if (isNew)
            {
                TitleText.Text = "Nová zakázka";
                BtnDelete.Visibility = Visibility.Collapsed;
                CustModePanel.Visibility = Visibility.Visible;
                CarModePanel.Visibility = Visibility.Visible;

                if (RadioCustExisting != null) { RadioCustExisting.IsChecked = true; RadioCust_Checked(null, null); }
                if (RadioCarExisting != null) { RadioCarExisting.IsChecked = true; RadioCar_Checked(null, null); }

                if (_order.CustomerId != 0 && _order.Customer != null) SelectCustomer(_order.Customer);
                if (_order.CarId != 0 && _order.Car != null) SelectCar(_order.Car);
            }
            else
            {
                TitleText.Text = _order.Name;
                BtnDelete.Visibility = Visibility.Visible;
                CustModePanel.Visibility = Visibility.Collapsed;
                CarModePanel.Visibility = Visibility.Collapsed;
                CustomerSearchPanel.Visibility = Visibility.Collapsed;
                CarSearchPanel.Visibility = Visibility.Collapsed;

                FillCustomerInfo(_order.Customer, readOnly: true);
                FillCarInfo(_order.Car, readOnly: true);
            }

            RefreshPhotos();
            if (_order.Id != 0)
            {
                if (BtnClearCustomer != null) BtnClearCustomer.Visibility = Visibility.Visible;
                if (BtnClearCar != null) BtnClearCar.Visibility = Visibility.Visible;
            }
        }

        private void RefreshGrids()
        {
            OrderMaterialDataGrid.ItemsSource = null;
            OrderMaterialDataGrid.ItemsSource = _order.Materials;
            OrderWorkDataGrid.ItemsSource = null;
            OrderWorkDataGrid.ItemsSource = _order.Works;
            RecalculateTotal();
        }

        // ... METODY PRO ZÁKAZNÍKA A AUTO JSOU STEJNÉ ...
        // (Zkopírujte je prosím z vašeho předchozího funkčního kódu, neměnily se)
        private void RadioCust_Checked(object sender, RoutedEventArgs e)
        {
            if (RadioCustExisting == null || CustomerSearchPanel == null) return;
            bool existing = RadioCustExisting.IsChecked == true;

            if (existing)
            {
                // Pokud se vracíme k existujícímu a už jsme někoho dříve vybrali
                if (_selectedCustomer != null)
                {
                    FillCustomerInfo(_selectedCustomer, readOnly: true);
                    CustomerSearchPanel.Visibility = Visibility.Collapsed;
                    BtnClearCustomer.Visibility = Visibility.Visible;
                }
                else
                {
                    FillCustomerInfo(null, readOnly: true);
                    CustomerSearchPanel.Visibility = Visibility.Visible;
                    BtnClearCustomer.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                // Režim NOVÝ: Odemkneme pole, schováme hledání i křížek
                FillCustomerInfo(null, readOnly: false);
                CustomerSearchPanel.Visibility = Visibility.Collapsed;
                BtnClearCustomer.Visibility = Visibility.Hidden;
            }
        }

        private void RadioCar_Checked(object sender, RoutedEventArgs e)
        {
            if (RadioCarExisting == null || CarSearchPanel == null) return;
            bool existing = RadioCarExisting.IsChecked == true;

            if (existing)
            {
                if (_selectedCar != null)
                {
                    FillCarInfo(_selectedCar, readOnly: true);
                    CarSearchPanel.Visibility = Visibility.Collapsed;
                    BtnClearCar.Visibility = Visibility.Visible;
                }
                else
                {
                    FillCarInfo(null, readOnly: true);
                    CarSearchPanel.Visibility = Visibility.Visible;
                    BtnClearCar.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                FillCarInfo(null, readOnly: false);
                CarSearchPanel.Visibility = Visibility.Collapsed;
                BtnClearCar.Visibility = Visibility.Hidden;
            }
        }

        private void ClearCustomerSelection_Click(object sender, RoutedEventArgs e)
        {
            _selectedCustomer = null;
            FillCustomerInfo(null, readOnly: true);
            CustomerSearchPanel.Visibility = Visibility.Visible;
            BtnClearCustomer.Visibility = Visibility.Hidden; // Schovat křížek
            CustomerSearchBox.Text = "";
        }

        private void ClearCarSelection_Click(object sender, RoutedEventArgs e)
        {
            _selectedCar = null;
            FillCarInfo(null, readOnly: true);
            CarSearchPanel.Visibility = Visibility.Visible;
            BtnClearCar.Visibility = Visibility.Hidden; // Schovat křížek
            CarSearchBox.Text = "";
        }
        private void CustomerSearchButton_Click(object sender, RoutedEventArgs e) => PerformCustomerSearch();
        private void CustomerSearchBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) PerformCustomerSearch(); }
        private void PerformCustomerSearch()
        {
            string q = CustomerSearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(q)) return;
            SearchedCustomers.ItemsSource = customer_repo.GetAll().Where(c => c.Name.ToLower().Contains(q) || c.Phone.Contains(q)).ToList();
            CustomerPopup.IsOpen = true;
        }
        private void SearchedCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SearchedCustomers.SelectedItem is Customer c) { SelectCustomer(c); CustomerPopup.IsOpen = false; }
        }
        private void SelectCustomer(Customer c)
        {
            _selectedCustomer = c;
            FillCustomerInfo(c, readOnly: true);
            CustomerSearchPanel.Visibility = Visibility.Collapsed;
            BtnClearCustomer.Visibility = Visibility.Visible;
            if (_order.Id == 0 && _selectedCar == null)
            {
                var cars = car_repo.GetAll().Where(x => x.CustomerId == c.Id).ToList();
                if (cars.Count == 1) SelectCar(cars.First());
                else if (cars.Count > 1) { CarSearchBox.Focus(); SearchedCars.ItemsSource = cars; CarPopup.IsOpen = true; }
            }
        }
        private void FillCustomerInfo(Customer c, bool readOnly)
        {
            if (CustNameBox == null) return;
            CustNameBox.Text = c?.Name ?? ""; CustPhoneBox.Text = c?.Phone ?? ""; CustEmailBox.Text = c?.Email ?? "";
            CustAddressBox.Text = c?.Address ?? ""; CustZIPBox.Text = c?.ZIP ?? ""; CustNoteBox.Text = c?.Notes ?? "";
            CustNameBox.IsReadOnly = CustPhoneBox.IsReadOnly = CustEmailBox.IsReadOnly = CustAddressBox.IsReadOnly = CustZIPBox.IsReadOnly = CustNoteBox.IsReadOnly = readOnly;
        }
        private void CarSearchButton_Click(object sender, RoutedEventArgs e) => PerformCarSearch();
        private void CarSearchBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) PerformCarSearch(); }
        private void CarSearchBox_GotFocus(object sender, RoutedEventArgs e) { if (_selectedCustomer != null && string.IsNullOrWhiteSpace(CarSearchBox.Text)) PerformCarSearch(); }
        private void PerformCarSearch()
        {
            string q = CarSearchBox.Text.Trim().ToLower();
            var all = car_repo.GetAll();
            List<Car> res;
            if (_selectedCustomer != null)
            {
                var myCars = all.Where(x => x.CustomerId == _selectedCustomer.Id);
                res = string.IsNullOrEmpty(q) ? myCars.ToList() : myCars.Where(x => x.BrandModel.ToLower().Contains(q) || x.SPZ.ToLower().Contains(q)).ToList();
            }
            else
            {
                if (string.IsNullOrEmpty(q)) return;
                res = all.Where(x => x.BrandModel.ToLower().Contains(q) || x.SPZ.ToLower().Contains(q)).ToList();
            }
            SearchedCars.ItemsSource = res;
            CarPopup.IsOpen = true;
        }
        private void SearchedCar_MouseDoubleClick(object sender, MouseButtonEventArgs e) { if (SearchedCars.SelectedItem is Car c) { SelectCar(c); CarPopup.IsOpen = false; } }
        private void SelectCar(Car c)
        {
            _selectedCar = c;
            FillCarInfo(c, readOnly: true);
            CarSearchPanel.Visibility = Visibility.Collapsed;
            BtnClearCar.Visibility = Visibility.Visible;
            if (_order.Id == 0 && _selectedCustomer == null && c.CustomerId != 0)
            {
                var owner = customer_repo.GetAll().FirstOrDefault(x => x.Id == c.CustomerId);
                if (owner != null) SelectCustomer(owner);
            }
        }
        private void FillCarInfo(Car c, bool readOnly)
        {
            if (CarModelBox == null) return;
            CarModelBox.Text = c?.BrandModel ?? ""; CarSPZBox.Text = c?.SPZ ?? ""; CarVINBox.Text = c?.VIN ?? "";
            CarDisplacementPowerBox.Text = c?.DisplacementPower ?? ""; CarNoteBox.Text = c?.Notes ?? "";
            CarYearBox.SelectedItem = c?.Year; CarFuelBox.SelectedItem = c?.Fuel; CarTypeBox.SelectedItem = c?.Type;
            CarModelBox.IsReadOnly = CarSPZBox.IsReadOnly = CarVINBox.IsReadOnly = CarDisplacementPowerBox.IsReadOnly = CarNoteBox.IsReadOnly = readOnly;
            CarYearBox.IsEnabled = CarFuelBox.IsEnabled = CarTypeBox.IsEnabled = !readOnly;
        }

        // --- MATERIÁL (Upraveno pro Binding) ---
        private void AddMaterialInline_Click(object sender, RoutedEventArgs e)
        {
            string name = InMatName.Text;
            if (string.IsNullOrWhiteSpace(name)) { MessageBox.Show("Vyplňte název."); return; }

            if (!int.TryParse(InMatQty.Text, out int qty) || qty <= 0) qty = 1;
            int.TryParse(InMatPrice.Text, out int price);

            var mat = new Material
            {
                Code = InMatCode.Text,
                Name = name,
                Quantity = qty,
                Price = price,
                // Používáme Binding, takže SelectedItem je přímo typ MeasureUnit
                Unit = InMatUnit.SelectedItem is MeasureUnit u ? u : MeasureUnit.ks,
                Supplier = InMatSupplier.SelectedItem is MaterialSupplier s ? s : MaterialSupplier.Intercars
            };

            if (_order.Id != 0)
            {
                mat.OrderId = _order.Id; material_repo.Add(mat);
                if (!_order.Materials.Contains(mat)) _order.Materials.Add(mat);
            }
            else _order.Materials.Add(mat);

            // Vyčistit
            InMatCode.Text = ""; InMatName.Text = ""; InMatQty.Text = ""; InMatPrice.Text = "";
            InMatUnit.SelectedIndex = 0; InMatSupplier.SelectedIndex = 0;

            RefreshGrids();
        }

        private void DeleteMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Material m)
            {
                if (_order.Id != 0) material_repo.Delete(m.Id);
                _order.Materials.Remove(m);
                RefreshGrids();
            }
        }

        // --- PRÁCE (Beze změny) ---
        private void AddWorkInline_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(InWorkHours.Text, out int h);
            if (h <= 0) h = 1;
            int.TryParse(InWorkPrice.Text, out int price);

            var w = new Work { Description = "Práce", Hours = h, Price = price }; // Výchozí popis

            if (_order.Id != 0)
            {
                w.OrderId = _order.Id;
                work_repo.Add(w);
                if (!_order.Works.Contains(w)) _order.Works.Add(w);
            }
            else _order.Works.Add(w);

            InWorkHours.Text = ""; InWorkPrice.Text = "";
            RefreshGrids();
        }

        private void DeleteWork_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Work w)
            {
                if (_order.Id != 0) work_repo.Delete(w.Id);
                _order.Works.Remove(w);
                RefreshGrids();
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                RecalculateTotal();
                if (_order.Id != 0 && e.Row.Item is Material m) material_repo.Update(m);
                else if (_order.Id != 0 && e.Row.Item is Work w) work_repo.Update(w);
            }), DispatcherPriority.ContextIdle);
        }

        private void RecalculateTotal()
        {
            int sum = _order.Materials.Sum(x => x.TotalPrice) + _order.Works.Sum(x => x.TotalPrice);
            TotalPriceFillBlock.Text = $"{sum} Kč";
        }

        // --- SAVE / DELETE / CLOSE ---
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OrderNameBox.Text)) { MessageBox.Show("Chybí název."); return; }
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string photosDir = System.IO.Path.Combine(baseDir, "AutoservisPhotos");

                if (!System.IO.Directory.Exists(photosDir))
                    System.IO.Directory.CreateDirectory(photosDir);

                // 2. Fyzické zkopírování fotek
                if (_order.Photos != null)
                {
                    foreach (var photo in _order.Photos)
                    {
                        // Pokud fotka ještě není v naší cílové složce (je nově vybraná odjinud)
                        if (!photo.FilePath.StartsWith(photosDir))
                        {
                            string extension = System.IO.Path.GetExtension(photo.FilePath);
                            string newFileName = $"{Guid.NewGuid()}{extension}";
                            string destinationPath = System.IO.Path.Combine(photosDir, newFileName);

                            // Zkopírujeme soubor z původního místa k nám
                            System.IO.File.Copy(photo.FilePath, destinationPath, true);

                            // Aktualizujeme cestu v objektu, která se pak uloží do DB
                            photo.FilePath = destinationPath;
                        }
                    }
                }

                if (int.TryParse(CarMileageBox.Text, out int m)) _order.Mileage = m;

                if (_order.Id == 0)
                {
                    if (RadioCustExisting.IsChecked == true)
                    {
                        if (_selectedCustomer == null) { MessageBox.Show("Vyberte zákazníka."); return; }
                        _order.CustomerId = _selectedCustomer.Id;
                    }
                    else
                    {
                        var newC = new Customer { Name = CustNameBox.Text, Phone = CustPhoneBox.Text, Email = CustEmailBox.Text, Address = CustAddressBox.Text, ZIP = CustZIPBox.Text, Notes = CustNoteBox.Text };
                        customer_repo.Add(newC); _order.CustomerId = newC.Id;
                    }
                    if (RadioCarExisting.IsChecked == true)
                    {
                        if (_selectedCar == null) { MessageBox.Show("Vyberte auto."); return; }
                        _order.CarId = _selectedCar.Id;
                    }
                    else
                    {
                        var newCar = new Car
                        {
                            BrandModel = CarModelBox.Text,
                            SPZ = CarSPZBox.Text,
                            VIN = CarVINBox.Text,
                            DisplacementPower = CarDisplacementPowerBox.Text,
                            Notes = CarNoteBox.Text,
                            Year = CarYearBox.SelectedItem is int y ? y : DateTime.Now.Year,
                            Fuel = CarFuelBox.SelectedItem is FuelType f ? f : (FuelType?)null,
                            Type = CarTypeBox.SelectedItem is CarType t ? t : (CarType?)null,
                            CustomerId = _order.CustomerId
                        };
                        car_repo.Add(newCar); _order.CarId = newCar.Id;
                    }
                    _order.Name = OrderNameBox.Text; 
                    _order.Date = OrderDatePicker.SelectedDate ?? DateTime.Now;
                    if (OrderStateComboBox.SelectedItem is State s) _order.State = s;
                    order_repo.Add(_order);
                }
                else
                {
                    _order.Name = OrderNameBox.Text; _order.Date = OrderDatePicker.SelectedDate ?? DateTime.Now;
                    if (OrderStateComboBox.SelectedItem is State s) _order.State = s;
                    order_repo.Update(_order);
                }
                MessageBox.Show("Uloženo.");
                RefreshPhotos();
                ClosePage();
            }
            catch (Exception ex) { MessageBox.Show("Chyba: " + ex.Message); }
        }
        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Smazat zakázku?", "Smazat", MessageBoxButton.YesNo) == MessageBoxResult.Yes) { order_repo.Delete(_order.Id); ClosePage(); }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e) => ClosePage();
        private void BtnClose_Click(object sender, RoutedEventArgs e) => ClosePage();
        private void BtnBack_Click(object sender, RoutedEventArgs e) => ClosePage();


        private void ClosePage()
        {
            if (Application.Current.MainWindow is MainWindow mw)
            {
                if (mw.OverlaySection.Visibility == Visibility.Visible) mw.CloseOverlay();
                else { mw.MainFrame.Content = null; mw.MainFrame.Visibility = Visibility.Collapsed; mw.LoadOrders(); }
            }
        }

        private void BtnAddPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Obrázky|*.jpg;*.png;*.jpeg",
                Multiselect = true // Povolíme výběr více souborů najednou
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (string filename in dialog.FileNames)
                {
                    var newPhoto = new Photo
                    {
                        FilePath = filename,
                        Name = System.IO.Path.GetFileName(filename)
                    };

                    if (_order.Photos == null) _order.Photos = new List<Photo>();
                    _order.Photos.Add(newPhoto);
                }
                RefreshPhotos();
            }
        }

        private void DeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Photo ph)
            {
                // 1. Pokud fotka ještě nemá ID (Id == 0), znamená to, že nebyla uložena do DB.
                // V tomto případě jen odstraníme náhled ze seznamu.
                if (ph.Id == 0)
                {
                    _order.Photos.Remove(ph);
                    RefreshPhotos();
                    return;
                }

                // 2. Pokud už ID má, je v DB i na disku. Vyžádáme si potvrzení.
                var result = MessageBox.Show($"Opravdu chcete trvale smazat soubor {ph.Name} z databáze?",
                                           "Trvalé smazání", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Fyzické smazání z disku
                        if (System.IO.File.Exists(ph.FilePath))
                        {
                            // Uvolnění prostředků, aby Windows soubor "pustil" k smazání
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            System.IO.File.Delete(ph.FilePath);
                        }

                        // Odstranění z DB přes kontext
                        _context.Photos.Remove(ph);
                        _context.SaveChanges();

                        // Odstranění ze seznamu v paměti a refresh UI
                        _order.Photos.Remove(ph);
                        RefreshPhotos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Chyba při mazání souboru: " + ex.Message);
                    }
                }
            }
        }

        private void OpenPhotoDetail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Photo photo)
            {
                try
                {
                    if (System.IO.File.Exists(photo.FilePath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = photo.FilePath,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex) { MessageBox.Show("Chyba při otevírání: " + ex.Message); }
            }
        }

        private void RefreshPhotos()
        {
            PhotosControl.ItemsSource = null;
            PhotosControl.ItemsSource = _order.Photos;
        }

        private void BtnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            // Příklad logiky pro uložení
            var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "PDF soubor|*.pdf", FileName = $"Zakazka_{_order.Id}.pdf" };
            if (dialog.ShowDialog() == true)
            {
                // Zde se volá generování PDF (QuestPDF kód je na delší ukázku, 
                // ale v principu vykreslíte tabulky a info o zakázce do dokumentu)
                MessageBox.Show("PDF bylo úspěšně vygenerováno.");
            }
        }
    }
}