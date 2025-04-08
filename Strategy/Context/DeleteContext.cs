using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.Interfaces;

namespace Strategy.Context
{
    public class DeleteContext
    {
        private IDeleteStrategy _strategy;

        public DeleteContext(IDeleteStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IDeleteStrategy strategy)
        {
            _strategy = strategy;
        }

        public async Task<bool> ExecuteStrategyAsync(int id)
        {
            return await _strategy.DeleteAsync(id);
        }
    }
}
