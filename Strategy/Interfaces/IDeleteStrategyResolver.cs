using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Enums;

namespace Strategy.Interfaces
{
    public interface IDeleteStrategyResolver<TEntity>
    {
        IDeleteStrategy<TEntity> Resolve(DeleteType strategyType);
    }
}
