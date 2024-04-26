﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public class StolServices
    {
        public List<Stol> GetAllStolove()
        {
            using (var repo = new StolRepository())
            {
                List<Stol> stolovi = repo.GetAll().ToList();
                return stolovi;
            }
        }

        public bool AddStol(Stol stol)
        {
            bool isSuccessful = false;

            using(var repo = new StolRepository()) 
            { 
                int affectedRows = repo.Add(stol);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool UpdateStol(Stol stol)
        {
            bool isSuccessful = false;

            using (var repo = new StolRepository())
            {
                int affectedRows = repo.Update(stol);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool RemoveStol(Stol stol)
        {
            bool isSuccessful = false;

            using (var repo = new StolRepository())
            {
                int affectedRows = repo.Remove(stol);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }
    }
}
