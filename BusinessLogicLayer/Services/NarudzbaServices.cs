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
    public class NarudzbaServices
    {
        public List<Narudzba> GetAllNarudzbe()
        {
            using (var repo = new NarudzbaRepository())
            {
                List<Narudzba> narudzbe = repo.GetAll().ToList();
                return narudzbe;
            }
        }

        public async Task<List<Narudzba>> GetAllNarudzbeByKorisnikAsync(int korisnikId)
        {
            using (var repo = new NarudzbaRepository())
            {
                List<Narudzba> narudzbe = await repo.GetAllById(korisnikId).ToListAsync();
                return narudzbe;
            }
        }

        public Narudzba GetLastNarudzbaByKorisnik(int korisnikId)
        {
            using (var repo = new NarudzbaRepository())
            {
                Narudzba narudzba = repo.GetLastNarudzbaByKorisnik(korisnikId);
                return narudzba;
            }
        }

        public bool AddNarudzbu(Narudzba narudzba)
        {
            bool isSuccessful = true;

            using(var repo = new NarudzbaRepository())
            {
                int affectedRows = repo.Add(narudzba);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public async Task<Narudzba> AddNarudzbuAsync(Narudzba narudzba)
        {
            using (var repo = new NarudzbaRepository())
            {
                await repo.AddAsync(narudzba);
                await repo.SaveChangesAsync();

                return narudzba;
            }
        }




        public bool UpdateNarudzbu(Narudzba narudzba)
        {
            bool isSuccessful = true;

            using (var repo = new NarudzbaRepository())
            {
                int affectedRows = repo.Update(narudzba);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool RemoveNarudzbu(Narudzba narudzba)
        {
            bool isSuccessful = true;

            using (var repo = new NarudzbaRepository())
            {
                int affectedRows = repo.Remove(narudzba);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public void SpremiPromjene()
        {
            using (var repo = new NarudzbaRepository())
            {
                SpremiPromjene();
            }
        }
    }
}
