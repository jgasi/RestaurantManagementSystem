﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public class PiceServices
    {
        public List<Pice> GetAllPica()
        {
            using (var repo = new PiceRepository())
            {
                List<Pice> pica = repo.GetAll().ToList();
                return pica;
            }
        }

        public async Task<List<Pice>> GetAllPicaAsync()
        {
            using (var repo = new PiceRepository())
            {
                List<Pice> pica = await repo.GetAll().ToListAsync();
                return pica;
            }
        }

        public async Task<List<Pice>> GetFirstThreePicaAsync()
        {
            using (var repo = new PiceRepository())
            {
                List<Pice> pica = await repo.GetFirstThreePica().ToListAsync();

                return pica;
            }
        }

        public async Task<List<Pice>> GetPiceByIdAsync(int id)
        {
            using (var repo = new PiceRepository())
            {
                List<Pice> pice = await repo.GetPiceById(id).ToListAsync();
                return pice;
            }
        }

        public async Task<List<Pice>> GetPiceByIdInventaraAsync(int id)
        {
            using (var repo = new PiceRepository())
            {
                List<Pice> pice = await repo.GetPiceByIdInventara(id).ToListAsync();
                return pice;
            }
        }

        public async Task<List<Pice>> GetAllPicaByNameAsync(string name)
        {
            using (var repo = new PiceRepository())
            {
                List<Pice> pica = await repo.GetAllByName(name).ToListAsync();
                return pica;
            }
        }

        public async Task<int> GetPiceCountAsync()
        {
            using (var repo = new PiceRepository())
            {
                int brojPica = await repo.GetBrojPica();

                return brojPica;
            }
        }

        public bool AddPice(Pice pice)
        {
            bool isSuccessful = false;

            using(var repo = new PiceRepository())
            {
                int affectedRows = repo.Add(pice);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool UpdatePice(Pice pice)
        {
            bool isSuccessful = false;

            using (var repo = new PiceRepository())
            {
                int affectedRows = repo.Update(pice);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool RemovePice(Pice pice)
        {
            bool isSuccessful = false;

            using (var repo = new PiceRepository())
            {
                int affectedRows = repo.Remove(pice);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }
    }
}
