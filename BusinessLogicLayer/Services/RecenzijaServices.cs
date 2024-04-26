using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public class RecenzijaServices
    {
        public List<Recenzija> GetAllRecenzije()
        {
            using(var repo = new RecenzijaRepository()) 
            {
                List<Recenzija> recenzije = repo.GetAll().ToList();
                return recenzije;
            }
        }

        public bool AddRecenziju(Recenzija recenzija)
        {
            bool isSuccessful = false;

            using(var repo = new RecenzijaRepository()) 
            {
                int affectedRows = repo.Add(recenzija);
                isSuccessful = affectedRows > 0;
            }
            return isSuccessful;
        }

        public bool UpdateRecenziju(Recenzija recenzija)
        {
            bool isSuccessful = false;

            using (var repo = new RecenzijaRepository())
            {
                int affectedRows = repo.Update(recenzija);
                isSuccessful = affectedRows > 0;
            }
            return isSuccessful;
        }

        public bool RemoveRecenziju(Recenzija recenzija)
        {
            bool isSuccessful = false;

            using (var repo = new RecenzijaRepository())
            {
                int affectedRows = repo.Remove(recenzija);
                isSuccessful = affectedRows > 0;
            }
            return isSuccessful;
        }
    }
}
