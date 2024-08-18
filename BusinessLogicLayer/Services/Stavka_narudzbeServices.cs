using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public class Stavka_narudzbeServices
    {
        public List<Stavka_narudzbe> GetAllStavkeNarudzbe()
        {
            using(var repo = new Stavka_narudzbeRepository())
            {
                List<Stavka_narudzbe> stavkeNarudzbe = repo.GetAll().ToList();
                return stavkeNarudzbe;
            }
        }

        public async Task<List<Stavka_narudzbe>> GetAllStavkeNarudzbeAsync()
        {
            using (var repo = new Stavka_narudzbeRepository())
            {
                List<Stavka_narudzbe> stavkeNarudzbe = await repo.GetAll().ToListAsync();
                return stavkeNarudzbe;
            }
        }

        public bool AddStavkeNarudzbe(Stavka_narudzbe stavkaNarudzbe)
        {
            bool isSuccessful = false;

            using (var repo = new Stavka_narudzbeRepository())
            {
                int affectedRows = repo.Add(stavkaNarudzbe);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public async Task<bool> AddStavkeNarudzbeAsync(Stavka_narudzbe stavkaNarudzbe)
        {
            bool isSuccessful = false;

            using (var repo = new Stavka_narudzbeRepository())
            {
                int affectedRows = await repo.AddAsync(stavkaNarudzbe);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool UpdateStavkeNarudzbe(Stavka_narudzbe stavkaNarudzbe)
        {
            bool isSuccessful = false;

            using (var repo = new Stavka_narudzbeRepository())
            {
                int affectedRows = repo.Update(stavkaNarudzbe);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool RemoveStavkeNarudzbe(Stavka_narudzbe stavkaNarudzbe)
        {
            bool isSuccessful = false;

            using (var repo = new Stavka_narudzbeRepository())
            {
                int affectedRows = repo.Remove(stavkaNarudzbe);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public async Task<bool> RemoveStavkeNarudzbeByIdAsync(int id)
        {
            bool isSuccessful = false;

            using (var repo = new Stavka_narudzbeRepository())
            {
                int affectedRows = await repo.RemoveByIdAsync(id);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }
    }
}
