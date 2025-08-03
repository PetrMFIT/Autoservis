using Autoservis.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Autoservis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AppDbContext DbContext { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=Data/autoservis.db")
                .Options;

            DbContext = new AppDbContext(options);
        }
    }

}
