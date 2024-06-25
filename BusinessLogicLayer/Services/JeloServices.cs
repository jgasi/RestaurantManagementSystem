using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System.Data.Entity;
using System.Diagnostics;

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
