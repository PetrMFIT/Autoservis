using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autoservis.Models;

namespace Autoservis.Repositories
{
    public interface IPhotoRepository
    {
        void Add(Photo photo);
        Photo? GetById(int id);
        List<Photo> GetAll();
        void Update(Photo photo);
        void Delete(int id);
    }
}
