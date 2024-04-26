using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
