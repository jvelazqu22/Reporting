using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;
using Domain.Interfaces.Database;

namespace iBank.Services.Orm.Databases
{
    public abstract class AbstractCommandDb : ICommandDb
    {
        public DbContext Context { get; set; }
        
        public void Insert<TEntity>(IList<TEntity> recsToAdd) where TEntity : class
        {
            using (Context)
            {
                foreach (var rec in recsToAdd)
                {
                    Context.Entry(rec).State = EntityState.Added;
                }

                Context.SaveChanges();
            }
        }

        public void Update<TEntity>(IList<TEntity> recsToUpdate) where TEntity : class
        {
            using (Context)
            {
                foreach (var rec in recsToUpdate)
                {
                    Context.Entry(rec).State = EntityState.Modified;
                }

                Context.SaveChanges();
            }
        }

        public void Remove<TEntity>(IList<TEntity> recsToRemove) where TEntity : class
        {
            using (Context)
            {
                foreach (var rec in recsToRemove)
                {
                    Context.Entry(rec).State = EntityState.Deleted;
                }

                Context.SaveChanges();
            }
        }

        public void RemoveLetClientWin<TEntity>(IList<TEntity> recsToRemove) where TEntity : class
        {
            using (Context)
            {
                try
                {
                    foreach (var rec in recsToRemove)
                    {
                        Context.Entry(rec).State = EntityState.Deleted;
                    }

                    Context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (e.InnerException is OptimisticConcurrencyException)
                    {
                        var refreshContext = ((IObjectContextAdapter)Context).ObjectContext;

                        if (recsToRemove.Any())
                        {
                            var entity = recsToRemove[0];
                            refreshContext.Refresh(System.Data.Entity.Core.Objects.RefreshMode.ClientWins, Context.Entry(entity));
                            RemoveLetClientWin(recsToRemove);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public abstract object Clone();
    }
}
