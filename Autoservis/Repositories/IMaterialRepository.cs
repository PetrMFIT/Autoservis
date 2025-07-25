using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autoservis.Models;

namespace Autoservis.Repositories
{
    public interface IMaterialRepository
    {
        void Add(Material material);
        Material? GetById(int id);
        List<Material> GetAll();
        void Update (Material material);
        void Delete(int id);
    }
}
