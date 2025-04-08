using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Data;
using Data.Interfaces;
using Entity.DTOs.RolFormPermissionDTOs;
using Entity.DTOs.UserDTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class RolFormPermissionBusiness : IBusiness<RolFormPermissionDTO, RolFormPermissionOptionsDTO>
    {
        private readonly IData<RolFormPermission> _rolFormPermissionData;
        private readonly ILogger<RolFormPermissionBusiness> _logger;

        public RolFormPermissionBusiness(IData<RolFormPermission> rolFormPermissionData, ILogger<RolFormPermissionBusiness> logger)
        {
            _rolFormPermissionData = rolFormPermissionData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de RolFormPermission como DTOs.
        /// </summary>
        public async Task<IEnumerable<RolFormPermissionDTO>> GetAllAsync()
        {
            try
            {
                var rolFormPermissions = await _rolFormPermissionData.GetAllAsync();
                return MapToDTOList(rolFormPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de RolFormPermission.");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de RolFormPermission.", ex);
            }
        }


        /// <summary>
        /// Obtiene un RolFormPermission por ID como DTO.
        /// </summary>
        public async Task<RolFormPermissionDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de obtener un RolFormPermission con ID inválido: {RolFormPermissionId}", id);
                throw new ValidationException("id", "El ID de RolFormPermission debe ser mayor que cero.");
            }
            var rolFormPermission = await _rolFormPermissionData.GetByIdAsync(id);
            if (rolFormPermission == null)
            {
                _logger.LogInformation("No se encontró ningún RolFormPermission con ID: {RolFormPermissionId}", id);
                throw new EntityNotFoundException("RolFormPermission", id);
            }
            try
            {
                return MapToDTO(rolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el RolFormPermission con ID: {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el RolFormPermission con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Crea un nuevo registro de RolFormPermission.
        /// </summary>
        public async Task<RolFormPermissionOptionsDTO> CreateAsync(RolFormPermissionOptionsDTO rolFormPermissionDTO)
        {
            ValidateRolFormPermission(rolFormPermissionDTO);

            try
            {
                var rolFormPermission = MapOptionsToEntity(rolFormPermissionDTO);
                var createdRolFormPermission = await _rolFormPermissionData.CreateAsync(rolFormPermission);

                return MapToOptionsDTO(createdRolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo RolFormPermission.");
                throw new ExternalServiceException("Base de datos", "Error al crear el RolFormPermission.", ex);
            }
        }


        /// <summary>
        /// Actualiza un registro existente de RolFormPermission.
        /// </summary>
        public async Task<bool> UpdateAsync(RolFormPermissionOptionsDTO rolFormPermissionDTO)
        {
            ValidateRolFormPermission(rolFormPermissionDTO);

            var existingRolFormPermission = await _rolFormPermissionData.GetByIdAsync(rolFormPermissionDTO.Id);
            if (existingRolFormPermission == null)
            {
                throw new EntityNotFoundException("RolFormPermission", rolFormPermissionDTO.Id);
            }
            try
            {
                existingRolFormPermission.Active = rolFormPermissionDTO.Status;
                existingRolFormPermission.RolId = rolFormPermissionDTO.RolId;
                existingRolFormPermission.PermissionId = rolFormPermissionDTO.PermissionId;
                existingRolFormPermission.FormId = rolFormPermissionDTO.FormId;

                return await _rolFormPermissionData.UpdateAsync(existingRolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el RolFormPermission con ID: {RolFormPermissionId}", rolFormPermissionDTO.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el RolFormPermission.", ex);
            }
        }


        /// <summary>
        /// Elimina un RolFormPermission por ID.
        /// </summary>
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            var existingRolFormPermission = await _rolFormPermissionData.GetByIdAsync(id);
            if (existingRolFormPermission == null)
            {
                throw new EntityNotFoundException("RolFormPermission", id);
            }
            try
            {
                return await _rolFormPermissionData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el RolFormPermission con ID: {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolFormPermission.", ex);
            }
        }


        /// <summary>
        /// Elimina un FormModule de manera logica por ID
        /// </summary>
        public async Task<bool> DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException("ID", "El ID del rolFormPermission debe ser mayor que cero.");
            }

            var existingUser = await _rolFormPermissionData.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new EntityNotFoundException("RolFormPermission", id);
            }
            try
            {

                return await _rolFormPermissionData.DeleteLogicAsync(id);

            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error en servicio externo al eliminar el rolFormPermission con ID: {RolFormPermissionId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rolFormPermission de manera logica con ID: {rolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el rolFormPermission de manera logica.", ex);
            }
        }


        /// <summary>
        /// Valida los datos de RolFormPermission.
        /// </summary>
        private void ValidateRolFormPermission(RolFormPermissionOptionsDTO rolFormPermissionDTO)
        {
            if (rolFormPermissionDTO == null)
            {
                throw new ValidationException("El objeto RolFormPermission no puede ser nulo.");
            }

            if (rolFormPermissionDTO.RolId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un RolFormPermission con RoleId inválido.");
                throw new ValidationException("RoleId", "El ID del rol debe ser mayor que cero.");
            }

            if (rolFormPermissionDTO.PermissionId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un RolFormPermission con PermissionId inválido.");
                throw new ValidationException("PermissionId", "El ID del permiso debe ser mayor que cero.");
            }

            if (rolFormPermissionDTO.FormId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un RolFormPermission con FormId inválido.");
                throw new ValidationException("FormId", "El ID del formulario debe ser mayor que cero.");
            }
        }


        /// <summary>
        /// Mapea un objeto RolFormPermission a RolFormPermissionDTO.
        /// </summary>
        private RolFormPermissionDTO MapToDTO(RolFormPermission rolFormPermission)
        {
            return new RolFormPermissionDTO
            {
                Id = rolFormPermission.Id,
                Status = rolFormPermission.Active,

                RolId = rolFormPermission.RolId,
                RolName = rolFormPermission.Rol.Name,

                PermissionId = rolFormPermission.PermissionId,
                PermissionName = rolFormPermission.Permission.Name,


                FormId = rolFormPermission.FormId,
                FormName = rolFormPermission.Form.Name,

            };
        }

        /// <summary>
        /// Mapea un objeto RolFormPermission a RolFormPermissionOptionsDTO.
        /// </summary>
        private RolFormPermissionOptionsDTO MapToOptionsDTO(RolFormPermission rolFormPermission)
        {
            return new RolFormPermissionOptionsDTO
            {
                Id = rolFormPermission.Id,
                Status = rolFormPermission.Active,

                RolId = rolFormPermission.RolId,

                PermissionId = rolFormPermission.PermissionId,

                FormId = rolFormPermission.FormId,
            };
        }


        /// <summary>
        /// Mapea un objeto RolFormPermissionDTO a RolFormPermission.
        /// </summary>
        private RolFormPermission MapToEntity(RolFormPermissionDTO rolFormPermissionDTO)
        {
            return new RolFormPermission
            {
                Id = rolFormPermissionDTO.Id,
                Active = rolFormPermissionDTO.Status,
                RolId = rolFormPermissionDTO.RolId,
                PermissionId = rolFormPermissionDTO.PermissionId,
                FormId = rolFormPermissionDTO.FormId
            };
        }

        /// <summary>
        /// Mapea un objeto RolFormPermissionOptionsDTO a RolFormPermission.
        /// </summary>
        private RolFormPermission MapOptionsToEntity(RolFormPermissionOptionsDTO rolFormPermissionDTO)
        {
            return new RolFormPermission
            {
                Id = rolFormPermissionDTO.Id,
                Active = rolFormPermissionDTO.Status,

                RolId = rolFormPermissionDTO.RolId,

                PermissionId = rolFormPermissionDTO.PermissionId,

                FormId = rolFormPermissionDTO.FormId
            };
        }


        /// <summary>
        /// Metodo para mapear una lista de RolFormPermission a una lista de RolFormPermissionDTO 
        /// </summary>
        /// <param name="rolFormPermissions"></param>
        /// <returns></returns>
        private IEnumerable<RolFormPermissionDTO> MapToDTOList(IEnumerable<RolFormPermission> rolFormPermissions)
        {
            var rolFormPermissionsDTO = new List<RolFormPermissionDTO>();
            foreach (var rolFormPermission in rolFormPermissions)
            {
                rolFormPermissionsDTO.Add(MapToDTO(rolFormPermission));
            }
            return rolFormPermissionsDTO;
        }
    }
}