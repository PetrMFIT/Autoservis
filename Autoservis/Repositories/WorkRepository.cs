using Autoservis.Data;
using Autoservis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservis.Repositories
{
    public class WorkRepository : IWorkRepository
    {
        public readonly AppDbContext _context;

        public WorkRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Work work)
        {
            _context.Works.Add(work);
            _context.SaveChanges();
        }

        public Work? GetById(int id)
        {
            return _context.Works.Find(id);
        }

        public List<Work> GetAll()
        {
            return _context.Works.ToList();
        }

        public void Update(Work work)
        {
            _context.Works.Update(work);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var work = _context.Works.Find(id);

            if (work != null)
            {
                _context.Works.Remove(work);
                _context.SaveChanges();
            }
        }
    }
}
