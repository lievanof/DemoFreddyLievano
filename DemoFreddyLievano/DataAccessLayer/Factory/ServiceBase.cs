using Demo.DataAccessLayer.Context;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Demo.DataAccessLayer.Factory
{
    public abstract class ServiceBase<T> where T : class
    {
        private DemoContext demoContext;
        private readonly IDbSet<T> dbset;

        protected ServiceBase(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            dbset = DemoContext.Set<T>();
        }

        protected ServiceBase(IDatabaseFactory databaseFactory, bool noTracking, bool lazyloading = true)
        {
            DatabaseFactory = databaseFactory;
            dbset = DemoContext.Set<T>();
            if (noTracking)
            {
                dbset.AsNoTracking<T>();
            }
            demoContext.Configuration.LazyLoadingEnabled = lazyloading;
        }

        protected IDatabaseFactory DatabaseFactory
        {
            get;
            private set;
        }

        protected DemoContext DemoContext
        {
            get { return demoContext ?? (demoContext = DatabaseFactory.Get()); }
        }

        public async Task<T> Get(int id)
        {
            return dbset.Find(id);
        }

        public async Task<T> Get(Expression<Func<T, bool>> @where)
        {
            return await dbset.Where(where).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> @where)
        {
            return await dbset.Where(where).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbset.ToListAsync();
        }


        public async Task<int> Update(T entity)
        {
            try
            {
                dbset.Attach(entity);
                DemoContext.Entry(entity).State = EntityState.Modified;

                return await Save();
            }
            catch
            {
                DemoContext.Entry(entity).State = EntityState.Detached;
                throw;
            }
        }

        public async Task<int> Save()
        {
            return await DemoContext.SaveChangesAsync();
        }

        public async Task<int> Create(T entity)
        {
            try
            {
                dbset.Add(entity);
                return await Save();
            }
            catch
            {
                DemoContext.Entry(entity).State = EntityState.Detached;
                throw;
            }
        }

        public async Task<int> Delete(int id)
        {
            var entity = await Get(id);
            dbset.Remove(entity);
            return await Save();
        }

        public async Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> where, string order, bool descending, int take)
        {
            if (take <= 0)
            {
                if (descending)
                {
                    return await dbset.OrderByDescending(order).Where(where).ToListAsync();
                }
                else
                {
                    return await dbset.OrderBy(order).Where(where).ToListAsync();
                }
            }
            else
            {
                if (descending)
                {
                    return await dbset.OrderByDescending(order).Where(where).Take(take).ToListAsync();
                }
                else
                {
                    return await dbset.OrderBy(order).Where(where).Take(take).ToListAsync();
                }
            }
        }

        public virtual IPagedList<T> GetPage<TOrder>(Page page, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order)
        {
            var results = dbset.OrderBy(order).Where(where).GetPage(page).ToList();
            var total = dbset.Count(where);
            return new StaticPagedList<T>(results, page.PageNumber, page.PageSize, total);
        }

        public virtual IPagedList<T> GetPage(Page page, Expression<Func<T, bool>> where, string order, bool descending)
        {
            List<T> results;
            if (descending)
            {
                results = dbset.OrderByDescending(order).Where(where).GetPage(page).ToList();
            }
            else
            {
                results = dbset.OrderBy(order).Where(where).GetPage(page).ToList();
            }
            var total = dbset.Count(where);
            return new StaticPagedList<T>(results, page.PageNumber, page.PageSize, total);
        }

        public virtual void LoadCollection(T model, string collectionName)
        {
            demoContext.Entry<T>(model).Collection(collectionName).Load();

        }

        public async Task<int> Count(Expression<Func<T, bool>> where)
        {
            return await dbset.Where(where).CountAsync();
        }
    }

    static class IOrderedQueryable
    {
        #region Order By String Column Name
        private static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), string.Empty); // I don't care about some naming
            MemberExpression property;

            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields. 
                String[] childProperties = propertyName.Split('.');
                var childProperty = typeof(T).GetProperty(childProperties[0]);
                property = Expression.MakeMemberAccess(param, childProperty);

                for (int i = 1; i < childProperties.Length; i++)
                {
                    Type t = childProperty.PropertyType;
                    if (!t.IsGenericType)
                    {
                        childProperty = t.GetProperty(childProperties[i]);
                    }
                    else
                    {
                        childProperty = t.GetGenericArguments().First().GetProperty(childProperties[i]);
                    }

                    property = Expression.MakeMemberAccess(property, childProperty);
                }
            }
            else
            {
                property = Expression.PropertyOrField(param, propertyName);
            }

            LambdaExpression sort = Expression.Lambda(property, param);

            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, false);
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, false);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, true);
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, true);
        }
        #endregion
    }
}
