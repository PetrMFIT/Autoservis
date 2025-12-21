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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Při startu aplikace vytvoříme dočasný kontext, abychom aplikovali migrace (vytvořili DB)
            using (var context = new AppDbContext())
            {
                // Tento příkaz zajistí, že se databáze vytvoří, pokud neexistuje,
                // a aplikují se všechny čekající změny.
                context.Database.Migrate();
            }
        }
    }

}
