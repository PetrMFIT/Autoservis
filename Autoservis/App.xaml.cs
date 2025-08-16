using Autoservis.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Autoservis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DbPath { get; private set; }

        public static AppDbContext DbContext { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AutoservisApp"
            );

            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            DbPath = Path.Combine(appDataPath, "autoservis.db");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data source={DbPath}")
                .Options;

            DbContext = new AppDbContext(options);

            DbContext.Database.Migrate();
        }
    }

}
