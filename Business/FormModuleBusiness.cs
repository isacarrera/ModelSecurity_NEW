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
    public class FormModuleBusiness : IBusiness<FormModuleDTO, FormModuleOptionsDTO>
    {
        private readonly IData<FormModule> _formModuleData;
        private readonly IDeleteStrategyResolver<FormModule> _strategyResolver;
        private readonly ILogger<FormModuleBusiness> _logger;
        private readonly IMapper _mapper;

        public FormModuleBusiness(IData<FormModule> formModuleData, IDeleteStrategyResolver<FormModule> strategyResolver, ILogger<FormModuleBusiness> logger, IMapper mapper)
        {
            _formModuleData = formModuleData;
            _strategyResolver = strategyResolver;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene todas las relaciones FormModule como DTOs.
        /// </summary>
        public async Task<IEnumerable<FormModuleDTO>> GetAllAsync()
        {
            try
            {
                var formModules = await _formModuleData.GetAllAsync();
                return _mapper.Map<IEnumerable<FormModuleDTO>>(formModules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las relaciones FormModule.");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de relaciones FormModule.", ex);
            }
        }


        /// <summary>
        /// Obtiene una relación FormModule por ID como DTO.
        /// </summary>
        public async Task<FormModuleDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de obtener una relación FormModule con ID inválido: {FormModuleId}", id);
                throw new ValidationException("id", "El ID de la relación FormModule debe ser mayor que cero.");
            }

            var formModule = await _formModuleData.GetByIdAsync(id);
            if (formModule == null)
            {
                _logger.LogInformation("No se encontró ninguna relación FormModule con ID: {FormModuleId}", id);
                throw new EntityNotFoundException("FormModule", id);
            }

            try
            {
                return _mapper.Map<FormModuleDTO>(formModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación FormModule con ID: {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la relación FormModule con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Crea una nueva relación FormModule.
        /// </summary>
        public async Task<FormModuleOptionsDTO> CreateAsync(FormModuleOptionsDTO formModuleDTO)
        {
            ValidateFormModule(formModuleDTO);

            try
            {
                var formModule = _mapper.Map<FormModule>(formModuleDTO);
                var createdFormModule = await _formModuleData.CreateAsync(formModule);

                return _mapper.Map<FormModuleOptionsDTO>(createdFormModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva relación FormModule.");
                throw new ExternalServiceException("Base de datos", "Error al crear la relación FormModule.", ex);
            }
        }


        /// <summary>
        /// Actualiza una relación FormModule existente.
        /// </summary>
        public async Task<bool> UpdateAsync(FormModuleOptionsDTO formModuleDTO)
        {

            var existingFormModule = await _formModuleData.GetByIdAsync(formModuleDTO.Id);
            if (existingFormModule == null)
            {
                throw new EntityNotFoundException("FormModule", formModuleDTO.Id);
            }

            ValidateFormModule(formModuleDTO);


            try
            {
                existingFormModule.Active = formModuleDTO.Status;
                existingFormModule.FormId = formModuleDTO.FormId;
                existingFormModule.ModuleId = formModuleDTO.ModuleId;

                return await _formModuleData.UpdateAsync(existingFormModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación FormModule con ID: {FormModuleId}", formModuleDTO.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar la relación FormModule.", ex);
            }
        }


        /// <summary>
        /// Elimina un FormModule. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del FormModule</param>
        /// <param name="strategy">Tipo de eliminación (Logical o Permanent)</param>
        public async Task<bool> DeleteAsync(int id, DeleteType strategyType)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del FormModule debe ser un número mayor a cero.", nameof(id));
            }

            var existingForm = await _formModuleData.GetByIdAsync(id);
            if (existingForm == null)
            {
                throw new EntityNotFoundException("FormModule", id);
            }

            try
            {
                var strategy = _strategyResolver.Resolve(strategyType);
                return await strategy.DeleteAsync(id, _formModuleData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el FormModule con ID: {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el FormModule.", ex);
            }
        }


        /// <summary>
        /// Valida los datos de la relación FormModule.
        /// </summary>
        private void ValidateFormModule(FormModuleOptionsDTO formModuleDTO)
        {
            if (formModuleDTO == null)
            {
                throw new ValidationException("El objeto FormModule no puede ser nulo.");
            }

            if (formModuleDTO.FormId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un FormModule con FormId inválido.");
                throw new ValidationException("FormId", "El FormId es obligatorio y debe ser mayor que cero.");
            }

            if (formModuleDTO.ModuleId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un FormModule con ModuleId inválido.");
                throw new ValidationException("ModuleId", "El ModuleId es obligatorio y debe ser mayor que cero.");
            }
        }


        
    }
}
