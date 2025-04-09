using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Data;
using Data.Interfaces;
using Entity.DTOs.RolUserDTOs;
using Entity.Enums;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class RolUserBusiness : IBusiness<RolUserDTO, RolUserOptionsDTO>
    {
        private readonly IData<RolUser> _rolUserData;
        private readonly ILogger<RolUserBusiness> _logger;

        public RolUserBusiness(IData<RolUser> rolUserData, ILogger<RolUserBusiness> logger)
        {
            _rolUserData = rolUserData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de RolUser como DTOs.
        /// </summary>
        public async Task<IEnumerable<RolUserDTO>> GetAllAsync()
        {
            try
            {
                var rolUsers = await _rolUserData.GetAllAsync();
                return MapToDTOList(rolUsers);
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
                return MapToDTO(rolUser);
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
                var rolUser = MapOptionsToEntity(rolUserDTO);
                var createdRolUser = await _rolUserData.CreateAsync(rolUser);

                return MapToOptionsDTO(createdRolUser);
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
        /// Elimina una asignación de RolUser por ID.
        /// </summary>
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            var existingRolUser = await _rolUserData.GetByIdAsync(id);
            if (existingRolUser == null)
            {
                throw new EntityNotFoundException("RolUser", id);
            }
            try
            {
                return await _rolUserData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el RolUser con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolUser.", ex);
            }
        }



        /// <summary>
        /// Elimina un RolUser de manera logica por ID
        /// </summary>
        public async Task<bool> DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException("ID", "El ID del rolUser debe ser mayor que cero.");
            }

            var existingUser = await _rolUserData.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new EntityNotFoundException("rolUser", id);
            }
            try
            {

                return await _rolUserData.DeleteLogicAsync(id);

            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error en servicio externo al eliminar el rolUser con ID: {RolUserId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rolUser de manera logica con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el rolUser de manera logica.", ex);
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


        /// <summary>
        /// Mapea un objeto RolUser a RolUserDTO.
        /// </summary>
        private RolUserDTO MapToDTO(RolUser rolUser)
        {
            return new RolUserDTO
            {
                Id = rolUser.Id,
                Status = rolUser.Active,

                UserId = rolUser.UserId,
                UserName = rolUser.User.Username,

                RolId = rolUser.RolId,
                RolName = rolUser.Rol.Name
            };
        }

        /// <summary>
        /// Mapea un objeto RolUser a RolUserOptionsDTO.
        /// </summary>
        private RolUserOptionsDTO MapToOptionsDTO(RolUser rolUser)
        {
            return new RolUserOptionsDTO
            {
                Id = rolUser.Id,
                Status = rolUser.Active,

                UserId = rolUser.UserId,

                RolId = rolUser.RolId,
            };
        }


        /// <summary>
        /// Mapea un objeto RolUserDTO a RolUser.
        /// </summary>
        private RolUser MapToEntity(RolUserDTO rolUserDTO)
        {
            return new RolUser
            {
                Id = rolUserDTO.Id,
                Active = rolUserDTO.Status,

                UserId = rolUserDTO.UserId,

                RolId = rolUserDTO.RolId
            };
        }

        /// <summary>
        /// Mapea un objeto RolUserOptionsDTO a RolUser.
        /// </summary>
        private RolUser MapOptionsToEntity(RolUserOptionsDTO rolUserDTO)
        {
            return new RolUser
            {
                Id = rolUserDTO.Id,
                Active = rolUserDTO.Status,

                UserId = rolUserDTO.UserId,

                RolId = rolUserDTO.RolId
            };
        }


        /// <summary>
        /// Metodo para mapear una lista de RolUser a una lista de RolUserDTO 
        /// </summary>
        /// <param name="rolUsers"></param>
        /// <returns></returns>
        private IEnumerable<RolUserDTO> MapToDTOList(IEnumerable<RolUser> rolUsers)
        {
            var rolUsersDTO = new List<RolUserDTO>();
            foreach (var rolUser in rolUsers)
            {
                rolUsersDTO.Add(MapToDTO(rolUser));
            }
            return rolUsersDTO;

        }

        public Task<bool> DeleteAsync(int id, DeleteType deleteType)
        {
            throw new NotImplementedException();
        }
    }
}