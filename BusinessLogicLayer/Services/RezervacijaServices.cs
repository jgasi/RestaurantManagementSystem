using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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


        public async Task<List<Rezervacija>> GetAllRezervacijeByKorisnikId(int id)
        {
            using (var repo = new RezervacijaRepository())
            {
                List<Rezervacija> rezervacije = await repo.GetAllByKorId(id).ToListAsync();
                return rezervacije;
            }
        }

        public async Task<List<Rezervacija>> GetAllRezervacijeByKorisnikIdAndDatum(int id, DateTime? vrijeme)
        {
            using (var repo = new RezervacijaRepository())
            {
                var rezervacije = await repo.GetAllByKorIdAndVrijeme(id, vrijeme)
                                            .Include(r => r.Stol)
                                            .ToListAsync();

                return rezervacije;
            }
        }

        public async Task<List<Rezervacija>> GetAllRezervacijeByDatum(DateTime? vrijeme)
        {
            using (var repo = new RezervacijaRepository())
            {
                var rezervacije = await repo.GetAllByDatum(vrijeme)
                                            .Include(r => r.Stol)
                                            .ToListAsync();

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

        public async Task<bool> RemoveRezervacijuPoDatumAndIdAsync(DateTime? vrijeme, int id)
        {
            bool isSuccessful = false;

            using (var repo = new RezervacijaRepository())
            {
                int affectedRows = await repo.RemoveByDateAndIdAsync(vrijeme, id);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }
    }
}
