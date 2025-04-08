using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IBusiness<TDto, TCreateDto>
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(int id);
        Task<TCreateDto> CreateAsync(TCreateDto createDto);
        Task<bool> UpdateAsync(TCreateDto createDto);
        Task<bool> DeletePersistenceAsync(int id); 
        Task<bool> DeleteLogicAsync(int id); 
    }
}
