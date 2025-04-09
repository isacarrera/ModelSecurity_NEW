using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Interfaces;
using Entity.Model;
using Strategy.Interfaces;

namespace Strategy.Implementations
{
    public class LogicalDeleteStrategy<TEntity> : IDeleteStrategy<TEntity>
    {
        public async Task<bool> DeleteAsync(int id, IData<TEntity> data)
        {
            var entity = await data.GetByIdAsync(id);
            if (entity == null) return false;
            return await data.DeleteLogicAsync(id);
        }
    }
}
