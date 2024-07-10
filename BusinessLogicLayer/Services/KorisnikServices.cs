using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public class KorisnikServices
    {
        public List<Korisnik> GetAllKorisnike()
        {
            using(var repo = new KorisnikRepository()) 
            {
                List<Korisnik> korisnici = repo.GetAll().ToList();
                return korisnici;
            }
        }

        public async Task<List<Korisnik>> GetAllKorisnikeAsync()
        {
            using (var repo = new KorisnikRepository())
            {
                List<Korisnik> korisnici = await repo.GetAll().ToListAsync();
                return korisnici;
            }
        }

        public List<Korisnik> GetKorisnikByKorime(string phrase)
        {
            using (var repo = new KorisnikRepository())
            {
                List<Korisnik> korisnik = repo.GetKorisnikByKorime(phrase).ToList();

                return korisnik;
            }
        }

        public Korisnik GetKorisnikById(int id)
        {
            using (var repo = new KorisnikRepository())
            {
                Korisnik korisnik = repo.GetKorisnikById(id);

                return korisnik;
            }
        }

        public async Task<int> GetKorisnikCountAsync() 
        {
            using (var repo = new KorisnikRepository())
            {
                int brojKorisnika = await repo.GetBrojKorisnika();

                return brojKorisnika;
            }
        }

        public bool AddKorisnik(Korisnik korisnik)
        {
            bool isSuccesful = false;

            using (var repo = new KorisnikRepository())
            {
                int affectedRows = repo.Add(korisnik);
                isSuccesful = affectedRows > 0;

            }

            return isSuccesful;
        }

        public bool UpdateKorisnik(Korisnik korisnik)
        {
            bool isSuccesful = false;

            using (var repo = new KorisnikRepository())
            {
                int affectedRows = repo.Update(korisnik);
                isSuccesful = affectedRows > 0;

            }

            return isSuccesful;
        }

        public bool RemoveKorisnik(Korisnik korisnik)
        {
            bool isSuccesful = false;

            using (var repo = new KorisnikRepository())
            {
                int affectedRows = repo.Remove(korisnik);
                isSuccesful = affectedRows > 0;

            }

            return isSuccesful;
        }
    }
}
