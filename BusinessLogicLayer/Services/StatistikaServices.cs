using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public class StatistikaServices
    {
        public List<Statistika> GetAllStatistike()
        {
            using(var repo = new StatistikaRepository())
            {
                List<Statistika> statistike = repo.GetAll().ToList();
                return statistike;
            }
        }

        public bool AddStatistiku(Statistika statistika)
        {
            bool isSuccessful = false;

            using(var repo = new StatistikaRepository()) 
            {
                int affectedRows = repo.Add(statistika);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool UpdateStatistiku(Statistika statistika)
        {
            bool isSuccessful = false;

            using (var repo = new StatistikaRepository())
            {
                int affectedRows = repo.Update(statistika);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool RemoveStatistiku(Statistika statistika)
        {
            bool isSuccessful = false;

            using (var repo = new StatistikaRepository())
            {
                int affectedRows = repo.Remove(statistika);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }
    }
}
