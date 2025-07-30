using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autoservis.Models;

namespace Autoservis.Repositories
{
    public interface IWorkRepository
    {
        void Add(Work work);
        Work? GetById(int id);
        List<Work> GetAll();
        void Update(Work work);
        void Delete(int id);
    }
}
