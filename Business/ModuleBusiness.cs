using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Interfaces;
using Data;
using Data.Interfaces;
using Entity.DTOs;
using Entity.DTOs.FormModuleDTOs;
using Entity.Enums;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Strategy.Interfaces;
using Utilities.Exceptions;

namespace Business
{
    ///<summary>
    ///Clase de negocio encargada de la logica relacionada con Module en el sistema;
    ///</summary>
    public class ModuleBusiness : IBusiness<ModuleDTO, ModuleDTO>
    {
        private readonly IData<Module> _moduleData;
        private readonly IDeleteStrategyResolver<Module> _strategyResolver;
        private readonly ILogger<ModuleBusiness> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ModuleBusiness"/>.
        /// </summary>
        /// <param name="moduleData">Capa de acceso a datos para Module.</param>
        /// <param name="logger">Logger para registro de Module</param>
        public ModuleBusiness(IData<Module> moduleData, IDeleteStrategyResolver<Module> strategyResolver, ILogger<ModuleBusiness> logger, IMapper mapper)
        {
            _moduleData = moduleData;
            _strategyResolver = strategyResolver;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        /// Obtiene todos los Modules y los mapea a objetos <see cref="ModuleDTO"/>.
        /// </summary>
        /// <returns>Una colección de objetos <see cref="ModuleDTO"/> que representan todos los Modules existentes.</returns>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar recuperar los datos desde la base de datos.
        /// </exception>
        public async Task<IEnumerable<ModuleDTO>> GetAllAsync()
        {
            try
            {
                var modules = await _moduleData.GetAllAsync();
                return _mapper.Map<IEnumerable<ModuleDTO>>(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Modules.");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de Modules.", ex);
            }
        }


        /// <summary>
        /// Obtiene un Module especifico por su identificador y lo mapea a un objeto <see cref="ModuleDTO"/>.
        /// </summary>
        /// <param name="id">Identificador único del module a buscar. Debe ser mayor que cero.</param>
        /// <returns>Un objeto <see cref="ModuleDTO"/> que representa el module solicitado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza cuando el parámetro <paramref name="id"/> es menor o igual a cero.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza cuando no se encuentra ningún module con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al mapear o recuperar el module desde la base de datos.
        /// </exception>
        public async Task<ModuleDTO> GetByIdAsync(int id)
        {
            var module = await _moduleData.GetByIdAsync(id);

            if (module == null)
            {
                _logger.LogInformation("No se encontró ningún módulo con ID: {ModuleId}", id);
                throw new EntityNotFoundException("Module", id);
            }

            try
            {
                return _mapper.Map<ModuleDTO>(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Module con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el Module con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Crea un nuevo Module en la base de datos a partir de un objeto <see cref="ModuleDTO"/>.
        /// </summary>
        /// <param name="ModuleDto">Objeto <see cref="ModuleDTO"/> que contiene la inmoduleación del module a crear.</param>
        /// <returns>El objeto <see cref="ModuleDTO"/> que representa el Module recién creado, incluyendo su identificador asignado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del module no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar crear el module en la base de datos.
        /// </exception>
        public async Task<ModuleDTO> CreateAsync(ModuleDTO moduleDTO)
        {
            ValidateModule(moduleDTO);
            try
            {
                var module = _mapper.Map<Module>(moduleDTO);
                var createdModule = await _moduleData.CreateAsync(module);

                return _mapper.Map<ModuleDTO>(createdModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo Module.");
                throw new ExternalServiceException("Base de datos", "Error al crear el Module.", ex);
            }
        }


        /// <summary>
        /// Actualiza un Module existente en la base de datos con los datos proporcionados en el <see cref="ModuleDTO"/>.
        /// </summary>
        /// <param name="moduleDTO">Objeto <see cref="ModuleDTO"/> con la inmoduleación actualizada del Module. Debe contener un ID válido.</param>
        /// <returns>Un valor booleano que indica si la operación de actualización fue exitosa.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del module no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún module con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el module en la base de datos.
        /// </exception>
        public async Task<bool> UpdateAsync(ModuleDTO moduleDTO)
        {
            ValidateModule(moduleDTO);

            var existingModule = await _moduleData.GetByIdAsync(moduleDTO.Id);
            if (existingModule == null)
            {
                throw new EntityNotFoundException("Module", moduleDTO.Id);
            }

            try
            {
                existingModule.Active = moduleDTO.Status;
                existingModule.Name = moduleDTO.Name;
                existingModule.Description = moduleDTO.Description;

                return await _moduleData.UpdateAsync(existingModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el Module con ID: {ModuleId}", moduleDTO.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el Module.", ex);
            }
        }


        /// <summary>
        /// Elimina un Module. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del Module</param>
        /// <param name="strategy">Tipo de eliminación (Logical o Permanent)</param>
        public async Task<bool> DeleteAsync(int id, DeleteType strategyType)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del Module debe ser un número mayor a cero.", nameof(id));
            }

            var existingForm = await _moduleData.GetByIdAsync(id);
            if (existingForm == null)
            {
                throw new EntityNotFoundException("Module", id);
            }

            try
            {
                var strategy = _strategyResolver.Resolve(strategyType);
                return await strategy.DeleteAsync(id, _moduleData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el Module con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el Module.", ex);
            }
        }


        /// <summary>
        /// Valida los datos del módulo.
        /// </summary>
        private void ValidateModule(ModuleDTO moduleDTO)
        {
            if (moduleDTO == null)
            {
                throw new ValidationException("El objeto Module no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(moduleDTO.Name))
            {
                _logger.LogWarning("Intento de crear/actualizar un Module con Name vacío.");
                throw new ValidationException("Name", "El Name del Module es obligatorio.");
            }
        }


        
    }
}