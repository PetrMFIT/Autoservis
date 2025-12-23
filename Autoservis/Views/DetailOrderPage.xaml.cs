using Autoservis.Data;
using Autoservis.Enums;
using Autoservis.Models;
using Autoservis.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Autoservis.Views
{
    public partial class DetailOrderPage : Page
    {
        private readonly AppDbContext _context;
        private Order _order;
        private int _orderId;

        private Page _previousPage;

        private readonly OrderRepository order_repo;
        private readonly MaterialRepository material_repo;
        private readonly WorkRepository work_repo;
        private readonly PhotoRepository photo_repo;

        public DetailOrderPage(Order order, Page previousPage = null)
        {
            InitializeComponent();
            _orderId = order.Id;

            // 1. Vlastní kontext pro tuto stránku
            _context = new AppDbContext();

            // 2. Repozitáře
            order_repo = new OrderRepository(_context);
            material_repo = new MaterialRepository(_context);
            work_repo = new WorkRepository(_context);
            photo_repo = new PhotoRepository(_context);

            // 3. Uklízení
            this.Unloaded += (s, e) => _context.Dispose();

            LoadData();
        }

        private void LoadData()
        {
            // Načteme čerstvá data z DB včetně vazeb
            _order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Car)
                .Include(o => o.Materials)
                .Include(o => o.Works)
                .Include(o => o.Photos)
                .FirstOrDefault(o => o.Id == _orderId);

            if (_order == null) return;

            // Vyplnění UI
            OrderNameBox.Text = _order.Name;
            OrderDatePicker.SelectedDate = _order.Date;
            CustomerNameText.Text = _order.Customer?.Name ?? "Neznámý";
            CarNameText.Text = $"{_order.Car?.BrandModel} ({_order.Car?.SPZ})";

            // ComboBox pro Stavy
            OrderStateComboBox.ItemsSource = Enum.GetValues(typeof(State));
            OrderStateComboBox.SelectedItem = _order.State;

            // Tabulky
            RefreshGrids();
            /*
            if (_previousPage != null)
            {
                // Přišli jsme z detailu zákazníka -> Zobrazit ZPĚT, skrýt ZAVŘÍT
                BtnBack.Visibility = Visibility.Visible;
                BtnClose.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Přišli jsme z hlavního menu -> Zobrazit ZAVŘÍT, skrýt ZPĚT
                BtnBack.Visibility = Visibility.Collapsed;
                BtnClose.Visibility = Visibility.Visible;
            }*/
        }

        private void RefreshGrids()
        {
            MaterialsGrid.ItemsSource = null;
            MaterialsGrid.ItemsSource = _order.Materials;

            WorksGrid.ItemsSource = null;
            WorksGrid.ItemsSource = _order.Works;

            PhotosList.ItemsSource = null;
            PhotosList.ItemsSource = _order.Photos;

            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            int total = _order.Materials.Sum(m => m.TotalPrice) + _order.Works.Sum(w => w.TotalPrice);
            TotalPriceText.Text = $"{total} Kč";
        }

        // --- MATERIÁLY ---
        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddMaterialWindow();
            if (window.ShowDialog() == true && window.material != null)
            {
                // Přidáme rovnou k zakázce
                window.material.OrderId = _order.Id;
                _order.Materials.Add(window.material);
                material_repo.Add(window.material); // Uloží se rovnou do DB
                RefreshGrids();
            }
        }

        private void DeleteMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Material mat)
            {
                if (MessageBox.Show("Smazat materiál?", "Potvrdit", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    material_repo.Delete(mat.Id);
                    _order.Materials.Remove(mat);
                    RefreshGrids();
                }
            }
        }

        // --- PRÁCE ---
        private void AddWork_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddWorkWindow();
            if (window.ShowDialog() == true && window.work != null)
            {
                window.work.OrderId = _order.Id;
                _order.Works.Add(window.work);
                work_repo.Add(window.work);
                RefreshGrids();
            }
        }

        private void DeleteWork_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Work work)
            {
                if (MessageBox.Show("Smazat práci?", "Potvrdit", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    work_repo.Delete(work.Id);
                    _order.Works.Remove(work);
                    RefreshGrids();
                }
            }
        }

        // --- FOTKY (Kopírování do složky aplikace) ---
        private void AddPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Obrázky|*.jpg;*.jpeg;*.png;*.bmp";

            if (dlg.ShowDialog() == true)
            {
                string sourceFile = dlg.FileName;
                string fileName = Path.GetFileName(sourceFile);

                // Cesta kam ukládáme: %AppData%/AutoservisApp/Images
                string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AutoservisApp", "Images");
                if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);

                // Aby se soubory nejmenovaly stejně, přidáme unikátní ID
                string uniqueName = $"{Guid.NewGuid()}_{fileName}";
                string destFile = Path.Combine(appDataPath, uniqueName);

                try
                {
                    File.Copy(sourceFile, destFile);

                    var photo = new Photo
                    {
                        Name = fileName,
                        FilePath = destFile,
                        OrderId = _order.Id
                    };

                    _order.Photos.Add(photo);
                    photo_repo.Add(photo);
                    RefreshGrids();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chyba při nahrávání fotky: {ex.Message}");
                }
            }
        }

        private void DeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Photo photo)
            {
                if (MessageBox.Show("Smazat fotku?", "Potvrdit", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Smazat z DB
                    photo_repo.Delete(photo.Id);
                    _order.Photos.Remove(photo);

                    // Smazat fyzický soubor (volitelné, aby se neplnil disk)
                    if (File.Exists(photo.FilePath))
                    {
                        try { File.Delete(photo.FilePath); } catch { }
                    }

                    RefreshGrids();
                }
            }
        }

        // --- ULOŽENÍ A ZAVŘENÍ ---
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            _order.Name = OrderNameBox.Text;
            if (OrderDatePicker.SelectedDate.HasValue)
                _order.Date = OrderDatePicker.SelectedDate.Value;

            if (OrderStateComboBox.SelectedItem is State state)
                _order.State = state;

            order_repo.Update(_order);
            MessageBox.Show("Změny uloženy.");
            ClosePage();
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Opravdu smazat celou zakázku?", "Varování", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                order_repo.Delete(_order.Id);
                ClosePage();
            }
        }

        private void ClosePage()
        {
            if (Application.Current.MainWindow is MainWindow mw)
            {
                mw.MainFrame.Content = null;
                mw.MainFrame.Visibility = Visibility.Collapsed;
                mw.LoadOrders(); // Obnovit seznam v hlavním okně
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Navigace zpět na uloženou stránku
            if (Application.Current.MainWindow is MainWindow mainWindow && _previousPage != null)
            {
                mainWindow.MainFrame.Navigate(_previousPage);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            // Klasické zavření (jako doteď)
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Content = null;
                mainWindow.MainFrame.Visibility = Visibility.Collapsed;
                mainWindow.LoadCars(); // Nebo refresh aut
            }
        }
    }
}