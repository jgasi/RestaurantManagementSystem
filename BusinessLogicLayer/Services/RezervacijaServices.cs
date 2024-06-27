using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public class RezervacijaServices
    {
        public List<Rezervacija> GetAllRezervacije()
        {
            using(var repo = new RezervacijaRepository()) 
            {
                List<Rezervacija> rezervacije = repo.GetAll().ToList();
                return rezervacije;
            }
        }

        public async Task<List<Rezervacija>> GetAllRezervacijeAsync()
        {
            using (var repo = new RezervacijaRepository())
            {
                List<Rezervacija> rezervacije = await repo.GetAll().ToListAsync();
                return rezervacije;
            }
        }

        public bool AddRezervaciju(Rezervacija rezervacija)
        {
            bool isSuccessful = false;

            using(var repo = new RezervacijaRepository())
            {
                int affectedRows = repo.Add(rezervacija);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool UpdateRezervaciju(Rezervacija rezervacija)
        {
            bool isSuccessful = false;

            using (var repo = new RezervacijaRepository())
            {
                int affectedRows = repo.Update(rezervacija);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool RemoveRezervaciju(Rezervacija rezervacija)
        {
            bool isSuccessful = false;

            using (var repo = new RezervacijaRepository())
            {
                int affectedRows = repo.Remove(rezervacija);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }
    }
}
