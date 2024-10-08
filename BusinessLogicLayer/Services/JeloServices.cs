﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public class JeloServices
    {
        public List<Jelo> GetAllJela()
        {
            using (var repo = new JeloRepository())
            {
                List<Jelo> jelo = repo.GetAll().ToList();

                return jelo;
            }
        }

        public async Task<List<Jelo>> GetAllJelaAsync()
        {
            using (var repo = new JeloRepository())
            {
                List<Jelo> jelo = await repo.GetAll().ToListAsync();
                return jelo;
            }
        }

        public async Task<List<Jelo>> GetJeloByIdAsync(int id)
        {
            using (var repo = new JeloRepository())
            {
                List<Jelo> jelo = await repo.GetJeloById(id).ToListAsync();
                return jelo;
            }
        }

        public async Task<List<Jelo>> GetJeloByIdInventaraAsync(int id)
        {
            using (var repo = new JeloRepository())
            {
                List<Jelo> jelo = await repo.GetJeloByIdInventara(id).ToListAsync();
                return jelo;
            }
        }

        public async Task<List<Jelo>> GetJelaByPageAsync(int pageNumber, int itemsPerPage)
        {
            using (var repo = new JeloRepository())
            {
                return await repo.GetJelaByPage(pageNumber, itemsPerPage).ToListAsync();
            }
        }

        public async Task<List<Jelo>> GetFirstThreeJelaAsync(int id)
        {
            using (var repo = new JeloRepository())
            {
                return await repo.GetJeloById(id).ToListAsync();
            }
        }

        public async Task<List<Jelo>> GetFirstThreeJelaByIdAsync()
        {
            using (var repo = new JeloRepository())
            {
                return await repo.GetFirstThree().ToListAsync();
            }
        }

        public async Task<List<Jelo>> GetJelaByNameAsync(string name)
        {
            using (var repo = new JeloRepository())
            {
                return await repo.GetJeloByName(name).ToListAsync();
            }
        }

        public async Task<int> GetJeloCountAsync()
        {
            using (var repo = new JeloRepository())
            {
                int brojJela = await repo.GetBrojJela();

                return brojJela;
            }
        }

        public bool AddJelo(Jelo jelo)
        {
            bool isSuccessful = false;

            using (var repo = new JeloRepository())
            {
                int affectedRows = repo.Add(jelo);
                isSuccessful = affectedRows > 0;
            }
            return isSuccessful;
        }

        public bool UpdateJelo(Jelo jelo)
        {
            bool isSuccessful = false;

            using (var repo = new JeloRepository())
            {
                int affectedRows = repo.Update(jelo);
                isSuccessful = affectedRows > 0;
            }
            return isSuccessful;
        }

        public bool RemoveJelo(Jelo jelo)
        {
            bool isSuccessful = false;

            using (var repo = new JeloRepository())
            {
                int affectedRows = repo.Remove(jelo);
                isSuccessful = affectedRows > 0;
            }
            return isSuccessful;
        }
    }
}
