using SimpleSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSystem.DataAccess.Repositories.Interfaces
{
    internal interface IGenaricRepository<T>
    {
        List<T> GetAll();
        T? GetById(int entityId);
        int Add(T entity);
        bool Update(T entity);
        bool Delete(int entityId);

    }
}
