using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public class InventarServices
    {
        public List<Inventar> GetAllInventare()
        {
            using (var repo = new InventarRepository())
            {
                List<Inventar> inventar = repo.GetAll().ToList();
                return inventar;
            }
        }

        public async Task<List<Inventar>> GetAllInventareAsync()
        {
            using (var repo = new InventarRepository())
            {
                List<Inventar> inventar = await repo.GetAll().ToListAsync();
                return inventar;
            }
        }

        public bool AddInventar(Inventar inventar)
        {
            bool isSuccessful = false;

            using(var repo = new InventarRepository())
            {
                int affectedRows = repo.Add(inventar);
                isSuccessful = affectedRows > 0;
            }
            return isSuccessful;
        }

        public bool UpdateInventar(Inventar inventar)
        {
            bool isSuccessful = false;

            using (var repo = new InventarRepository())
            {
                int affectedRows = repo.Update(inventar);
                isSuccessful = affectedRows > 0;
            }
            return isSuccessful;
        }

        public bool RemoveInventar(Inventar inventar)
        {
            bool isSuccessful = false;

            using (var repo = new InventarRepository())
            {
                int affectedRows = repo.Remove(inventar);
                isSuccessful = affectedRows > 0;
            }
            return isSuccessful;
        }
    }
}
