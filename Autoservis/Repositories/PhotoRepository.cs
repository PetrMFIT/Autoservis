using Autoservis.Data;
using Autoservis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservis.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        public readonly AppDbContext _context;

        public PhotoRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Photo photo)
        {
            _context.Photos.Add(photo);
            _context.SaveChanges();
        }

        public Photo? GetById(int id)
        {
            return _context.Photos.Find(id);
        }

        public List<Photo> GetAll()
        {
            return _context.Photos.ToList();
        }

        public void Update(Photo photo)
        {
            _context.Photos.Update(photo);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var photo = _context.Photos.Find(id);

            if (photo != null)
            {
                _context.Photos.Remove(photo);
                _context.SaveChanges();
            }
        }
    }
}
