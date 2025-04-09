using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Interfaces;
using Strategy.Interfaces;
using static Dapper.SqlMapper;

namespace Strategy.Implementations
{
    public class PermanentDeleteStrategy<TEntity> : IDeleteStrategy<TEntity>
    {
        public async Task<bool> DeleteAsync(int id, IData<TEntity> data)
        {
            return await data.DeletePersistenceAsync(id);
        }
    }
}
