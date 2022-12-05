using EM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EM.Repository
{
    public abstract class  RepositorioAbstrato<T> where T : IEntidade
    {

        public abstract void Add(T obj);
        public abstract void Remove(T obj);
        public abstract void Update(T obj);
        public abstract IEnumerable<T> GetAll();
        public abstract IEnumerable<T> Get(Expression<Func<T, bool>> predicate);
    }

}
