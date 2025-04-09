using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Interfaces;
using Data;
using Data.Interfaces;
using Entity.DTOs.RolUserDTOs;
using Entity.Enums;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Strategy.Interfaces;
using Utilities.Exceptions;

namespace Business
{
    public class RolUserBusiness : IBusiness<RolUserDTO, RolUserOptionsDTO>
    {
        private readonly IData<RolUser> _rolUserData;
        private readonly IDeleteStrategyResolver<RolUser> _strategyResolver;
        private readonly ILogger<RolUserBusiness> _logger;
        private readonly IMapper _mapper;

        public RolUserBusiness(IData<RolUser> rolUserData, IDeleteStrategyResolver<RolUser> strategyResolver, ILogger<RolUserBusiness> logger, IMapper mapper)
        {
            _rolUserData = rolUserData;
            _strategyResolver = strategyResolver;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene todos los registros de RolUser como DTOs.
        /// </summary>
        public async Task<IEnumerable<RolUserDTO>> GetAllAsync()
        {
            try
            {
                var rolUsers = await _rolUserData.GetAllAsync();
                return _mapper.Map<IEnumerable<RolUserDTO>>(rolUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de RolUser.");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de RolUser.", ex);
            }
        }


        /// <summary>
        /// Obtiene un RolUser por ID como DTO.
        /// </summary>
        public async Task<RolUserDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de obtener un RolUser con ID inválido: {RolUserId}", id);
                throw new ValidationException("id", "El ID de RolUser debe ser mayor que cero.");
            }

            var rolUser = await _rolUserData.GetByIdAsync(id);
            if (rolUser == null)
            {
                _logger.LogInformation("No se encontró ningún RolUser con ID: {RolUserId}", id);
                throw new EntityNotFoundException("RolUser", id);
            }

            try
            {
                return _mapper.Map<RolUserDTO>(rolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el RolUser con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el RolUser con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Crea una nueva asignación de RolUser.
        /// </summary>
        public async Task<RolUserOptionsDTO> CreateAsync(RolUserOptionsDTO rolUserDTO)
        {
            ValidateRolUser(rolUserDTO);
            try
            {
                var rolUser = _mapper.Map<RolUser>(rolUserDTO);
                var createdRolUser = await _rolUserData.CreateAsync(rolUser);

                return _mapper.Map<RolUserOptionsDTO>(createdRolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo RolUser.");
                throw new ExternalServiceException("Base de datos", "Error al crear la asignación de RolUser.", ex);
            }
        }


        /// <summary>
        /// Actualiza una asignación existente de RolUser.
        /// </summary>
        public async Task<bool> UpdateAsync(RolUserOptionsDTO rolUserDTO)
        {
            ValidateRolUser(rolUserDTO);

            var existingRolUser = await _rolUserData.GetByIdAsync(rolUserDTO.Id);
            if (existingRolUser == null)
            {
                throw new EntityNotFoundException("RolUser", rolUserDTO.Id);
            }
            try
            {
                existingRolUser.Active = rolUserDTO.Status;
                existingRolUser.UserId = rolUserDTO.UserId;
                existingRolUser.RolId = rolUserDTO.RolId;

                return await _rolUserData.UpdateAsync(existingRolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el RolUser con ID: {RolUserId}", rolUserDTO.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el RolUser.", ex);
            }
        }


        /// <summary>
        /// Elimina un RolUser. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del RolUser</param>
        /// <param name="strategy">Tipo de eliminación (Logical o Permanent)</param>
        public async Task<bool> DeleteAsync(int id, DeleteType strategyType)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del RolUser debe ser un número mayor a cero.", nameof(id));
            }

            var existingRolUser = await _rolUserData.GetByIdAsync(id);
            if (existingRolUser == null)
            {
                throw new EntityNotFoundException("RolUser", id);
            }

            try
            {
                var strategy = _strategyResolver.Resolve(strategyType);
                return await strategy.DeleteAsync(id, _rolUserData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el RolUser con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolUser.", ex);
            }
        }


        /// <summary>
        /// Valida los datos de RolUser.
        /// </summary>
        private void ValidateRolUser(RolUserOptionsDTO rolUserDTO)
        {
            if (rolUserDTO == null)
            {
                throw new ValidationException("El objeto RolUser no puede ser nulo.");
            }

            if (rolUserDTO.UserId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un RolUser con UserId inválido.");
                throw new ValidationException("UserId", "El ID del usuario debe ser mayor que cero.");
            }

            if (rolUserDTO.RolId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un RolUser con RoleId inválido.");
                throw new ValidationException("RoleId", "El ID del rol debe ser mayor que cero.");
            }
        }


        
    }
}