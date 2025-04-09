using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Data;
using Data.Interfaces;
using Entity.DTOs;
using Entity.Enums;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class PermissionBusiness : IBusiness<PermissionDTO, PermissionDTO>
    {
        private readonly IData<Permission> _permissionData;
        private readonly ILogger<PermissionBusiness> _logger;

        public PermissionBusiness(IData<Permission> permissionData, ILogger<PermissionBusiness> logger)
        {
            _permissionData = permissionData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos como DTOs.
        /// </summary>
        public async Task<IEnumerable<PermissionDTO>> GetAllAsync()
        {
            try
            {
                var permissions = await _permissionData.GetAllAsync();
                return MapToDTOList(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Permissions.");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de Permissions.", ex);
            }
        }


        /// <summary>
        /// Obtiene un permiso por ID como DTO.
        /// </summary>
        public async Task<PermissionDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de obtener un Permission con ID inválido: {PermissionId}", id);
                throw new ValidationException("id", "El ID del Permission debe ser mayor que cero.");
            }
            var permission = await _permissionData.GetByIdAsync(id);
            if (permission == null)
            {
                _logger.LogInformation("No se encontró ningún Permission con ID: {PermissionId}", id);
                throw new EntityNotFoundException("Permission", id);
            }
            try
            {
                return MapToDTO(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Permission con ID: {PermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el Permission con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Crea un nuevo permiso.
        /// </summary>
        public async Task<PermissionDTO> CreateAsync(PermissionDTO permissionDTO)
        {
            ValidatePermission(permissionDTO);

            try
            {
                var permission = MapToEntity(permissionDTO);
                var createdPermission = await _permissionData.CreateAsync(permission);

                return MapToDTO(createdPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo Permission.");
                throw new ExternalServiceException("Base de datos", "Error al crear el Permission.", ex);
            }
        }


        /// <summary>
        /// Actualiza un permiso existente.
        /// </summary>
        public async Task<bool> UpdateAsync(PermissionDTO permissionDTO)
        {
            ValidatePermission(permissionDTO);

            var existingPermission = await _permissionData.GetByIdAsync(permissionDTO.Id);
            if (existingPermission == null)
            {
                throw new EntityNotFoundException("Permission", permissionDTO.Id);
            }
            try
            {

                existingPermission.Active = permissionDTO.Status;
                existingPermission.Name = permissionDTO.Name;
                existingPermission.Description = permissionDTO.Description;

                return await _permissionData.UpdateAsync(existingPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el Permission con ID: {PermissionId}", permissionDTO.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el Permission.", ex);
            }
        }


        /// <summary>
        /// Elimina un permision por ID.
        /// </summary>
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            var existingPermission = await _permissionData.GetByIdAsync(id);
            if (existingPermission == null)
            {
                throw new EntityNotFoundException("Permission", id);
            }

            try
            {
                return await _permissionData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el Permission con ID: {PermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el Permission.", ex);
            }
        }


        /// <summary>
        /// Elimina un formulario de manera logica por ID
        /// </summary>
        public async Task<bool> DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException("ID", "El ID del Permission debe ser mayor que cero.");
            }

            var existingPermission = await _permissionData.GetByIdAsync(id);
            if (existingPermission == null)
            {
                throw new EntityNotFoundException("Form", id);
            }

            try
            {
                return await _permissionData.DeleteLogicAsync(id);

            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error en servicio externo al eliminar el Permission con ID: {FormId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el Permission de manera logica con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el Permission de manera logica.", ex);
            }
        }


        /// <summary>
        /// Valida los datos del permiso.
        /// </summary>
        private void ValidatePermission(PermissionDTO permissionDTO)
        {
            if (permissionDTO == null)
            {
                throw new ValidationException("El objeto Permission no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(permissionDTO.Name))
            {
                _logger.LogWarning("Intento de crear/actualizar un Permission con Name vacío.");
                throw new ValidationException("Name", "El nombre del Permission es obligatorio.");
            }
        }


        /// <summary>
        /// Mapea un objeto Permission a PermissionDTO.
        /// </summary>
        private PermissionDTO MapToDTO(Permission permission)
        {
            return new PermissionDTO
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                Status = permission.Active
            };
        }


        /// <summary>
        /// Mapea un objeto PermissionDTO a Permission.
        /// </summary>
        private Permission MapToEntity(PermissionDTO permissionDTO)
        {
            return new Permission
            {
                Id = permissionDTO.Id,
                Name = permissionDTO.Name,
                Description = permissionDTO.Description,
                Active = permissionDTO.Status,
            };
        }


        /// <summary>
        /// Metodo para mapear una lista de Permission a una lista de PermissionDTO 
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        private IEnumerable<PermissionDTO> MapToDTOList(IEnumerable<Permission> permissions)
        {
            var permissionsDTO = new List<PermissionDTO>();
            foreach (var permission in permissions)
            {
                permissionsDTO.Add(MapToDTO(permission));
            }
            return permissionsDTO;
        }

        public Task<bool> DeleteAsync(int id, DeleteType deleteType)
        {
            throw new NotImplementedException();
        }
    }
}