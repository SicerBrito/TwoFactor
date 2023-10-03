using System.Linq.Expressions;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistencia.Data;

namespace Aplicacion.Repository;
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity{

        private readonly DbAppContext _Context;
        protected readonly DbSet<T> _Entity;

        public GenericRepository(DbAppContext context)
        {
            _Context = context;
            _Entity = context.Set<T>();
        }

        public virtual void Add(T entity)
        {
            _Context.Set<T>().Add(entity);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            _Context.Set<T>().AddRange(entities);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _Context.Set<T>().Where(expression);
        }

        public async virtual Task<T> FindFirst(Expression<Func<T, bool>> expression){
            if (expression != null){
                var result = await _Entity.Where(expression).ToListAsync();
                return result.First();
            }
            return await _Entity.FirstAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _Context.Set<T>().ToListAsync();
            
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return (await _Context.Set<T>().FindAsync(id))!;
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
        return (await _Context.Set<T>().FindAsync(id))!;
        }

        public virtual void Remove(T entity)
        {
            _Context.Set<T>().Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _Context.Set<T>().RemoveRange(entities);
        }

        public virtual void Update(T entity)
        {
            _Context.Set<T>()
                .Update(entity);
        }
        public virtual async Task<(int totalRegistros, IEnumerable<T> registros)> GetAllAsync(int pageIndex, int pageSize, string _search)
        {
            var totalRegistros = await _Context.Set<T>().CountAsync();
            var registros = await _Context.Set<T>()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (totalRegistros, registros);
        }


}
