using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Interfaces;
using Entity.Context;
using Entity.DTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class PersonData : IData<Person>
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger<PersonData> _looger;

        public PersonData(ApplicationDbContext context, ILogger<PersonData> logger)
        {
            _context = context;
            _looger = logger;
        }


        /// <summary>
        /// Obtiene todos los Person almacenados en la base de datos LINQ
        /// </summary>
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.Set<Person>()
             .Where(p => p.Active)
             .ToListAsync();
        }


        /// <summary>
        /// Obtiene un Person especifico por su identificacion LINQ
        /// </summary
        public async Task<Person?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Person>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _looger.LogError(ex, $"Error al obtener Person con id {id}");
                throw;
            }
        }

        // <summary>
        /// Crear un nuevo Person en la base de datos LINQ
        /// </summary>
        public async Task<Person> CreateAsync(Person person)
        {
            try
            {
                await _context.Set<Person>().AddAsync(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception ex)
            {
                _looger.LogError($"Error al crear Person {ex.Message}");
                throw;
            }

        }




        /// <summary>
        /// Actualiza un Person existente en la base de datos LINQ
        /// </summary>
        public async Task<bool> UpdateAsync(Person person)
        {
            try
            {
                _context.Set<Person>().Update(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _looger.LogError($"Error al actualizar Person {ex.Message}");
                throw;
            }

        }




        /// <summary>
        /// Elimina un Person de la base de datos LINQ
        /// </summary>
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                var person = await _context.Set<Person>().FindAsync(id);
                if (person == null)
                    return false;

                _context.Set<Person>().Remove(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _looger.LogError($"Error al eliminar Person {ex.Message}");
                return false;
            }
        }




        /// <summary>
        /// Elimina un Person de manera logica de la base  de datos LINQ
        /// </summary>
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                var person = await GetByIdAsync(id);
                if (person == null)
                {
                    return false;
                }

                person.Active = false;
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                _looger.LogError($"Error al eliminar de manera logica Person: {ex.Message}");
                return false;
            }
        }

    }
}