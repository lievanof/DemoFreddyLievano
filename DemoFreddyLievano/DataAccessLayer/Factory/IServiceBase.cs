using PagedList;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Demo.DataAccessLayer.Factory
{
    public interface IServiceBase<T> where T : class
    {
        Task<T> Get(int id);
        Task<T> Get(Expression<Func<T, bool>> where);

        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> where);
        Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> where, string order, bool descending, int take);

        Task<int> Update(T entity);
        Task<int> Save();
        Task<int> Create(T entity);
        Task<int> Delete(int id);
        IPagedList<T> GetPage<TOrder>(Page page, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order);
        IPagedList<T> GetPage(Page page, Expression<Func<T, bool>> where, string order, bool descending);
        Task<int> Count(Expression<Func<T, bool>> where);
    }
}
