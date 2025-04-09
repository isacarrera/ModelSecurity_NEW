using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Enums;
using Microsoft.Extensions.DependencyInjection;
using Strategy.Implementations;
using Strategy.Interfaces;

namespace Strategy.Context
{
    public class DeleteContext<TEntity> : IDeleteStrategyResolver<TEntity>
    {

        private readonly IServiceProvider _serviceProvider;
        public DeleteContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDeleteStrategy<TEntity> Resolve(DeleteType strategyType)
        {
            return strategyType switch
            {
                DeleteType.Logical => _serviceProvider.GetRequiredService<LogicalDeleteStrategy<TEntity>>(),
                DeleteType.Permanent => _serviceProvider.GetRequiredService<PermanentDeleteStrategy<TEntity>>(),
                _ => throw new NotImplementedException()
            };
        }

    }
}
