using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.Interfaces
{
    public interface IDeleteStrategy
    {
        Task<bool> DeleteAsync(int id);
    }
}
