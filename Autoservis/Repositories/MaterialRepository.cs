using Autoservis.Data;
using Autoservis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservis.Repositories
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly AppDbContext _context;
        public MaterialRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Material material)
        {
            _context.Materials.Add(material);
            _context.SaveChanges();
        }

        public Material? GetById(int id)
        {
            return _context.Materials.Find(id);
        }

        public List<Material> GetAll()
        {
            return _context.Materials.ToList();
        }

        public void Update(Material material)
        {
            _context.Materials.Update(material);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var material = _context.Materials.Find(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
                _context.SaveChanges();
            }
        }
    }
}
