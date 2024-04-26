using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
