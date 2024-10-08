﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public abstract class Repository<T> : IDisposable where T : class
    {
        protected RestaurantDatabaseModel Context { get; set; }
        public DbSet<T> Entities { get; set; }

        public Repository(RestaurantDatabaseModel context)
        {
            Context = context;
            Entities = Context.Set<T>();
        }

        public virtual IQueryable<T> GetAll()
        {
            var query = from e in Entities
                        select e;
            return query;
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public virtual int Add(T entity, bool saveChanges = true)
        {
            Entities.Add(entity);
            if(saveChanges) 
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public virtual async Task<int> AddAsync(T entity, bool saveChanges = true)
        {
            Entities.Add(entity);

            if (saveChanges)
            {
                return await Context.SaveChangesAsync();
            }
            else
            {
                return 0;
            }
        }


        public abstract int Update(T entity, bool saveChanges = true);

        public virtual int Remove(T entity, bool saveChanges = true)
        {
            Entities.Attach(entity);
            Entities.Remove(entity);
            if (saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    } 
}
