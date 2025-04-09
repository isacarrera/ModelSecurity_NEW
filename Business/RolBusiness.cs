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
    ///<summary>
    ///Clase de negocio encargada de la logica relacionada con Rol en el sistema;
    ///</summary>
    public class RolBusiness : IBusiness<RolDTO, RolDTO>
    {
        private readonly IData<Rol> _rolData;
        private readonly ILogger<RolBusiness> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RolBusiness"/>.
        /// </summary>
        /// <param name="rolData">Capa de acceso a datos para Rol.</param>
        /// <param name="logger">Logger para registro de Rol</param>
        public RolBusiness(IData<Rol> rolData, ILogger<RolBusiness> logger)
        {
            _rolData = rolData;
            _logger = logger;
        }


        /// <summary>
        /// Obtiene todos los Rols y los mapea a objetos <see cref="RolDTO"/>.
        /// </summary>
        /// <returns>Una coleccion de objetos <see cref="RolDTO"/> que representan todos los Rols existentes.</returns>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar recuperar los datos desde la base de datos.
        /// </exception>
        public async Task<IEnumerable<RolDTO>> GetAllAsync()
        {
            try
            {
                var roles = await _rolData.GetAllAsync();
                return MapToDTOList(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Rols");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de Rols", ex);
            }
        }


        /// <summary>
        /// Obtiene un Rol especifico por su identificador y lo mapea a un objeto <see cref="RolDTO"/>.
        /// </summary>
        /// <param name="id">Identificador único del rol a buscar. Debe ser mayor que cero.</param>
        /// <returns>Un objeto <see cref="RolDTO"/> que representa el rol solicitado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza cuando el parámetro <paramref name="id"/> es menor o igual a cero.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza cuando no se encuentra ningún rol con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al mapear o recuperar el rol desde la base de datos.
        /// </exception>
        public async Task<RolDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un Rol con ID inválido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del Rol debe ser mayor que cero");
            }

            var rol = await _rolData.GetByIdAsync(id);

            if (rol == null)
            {
                _logger.LogInformation("No se encontró ningún Rol con ID: {RolId}", id);
                throw new EntityNotFoundException("Rol", id);
            }

            try
            {
                return MapToDTO(rol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el Rol con ID {id}", ex);
            }
        }


        /// <summary>
        /// Crea un nuevo Rol en la base de datos a partir de un objeto <see cref="RolDTO"/>.
        /// </summary>
        /// <param name="RolDto">Objeto <see cref="RolDTO"/> que contiene la información del rol a crear.</param>
        /// <returns>El objeto <see cref="RolDTO"/> que representa el Rol recién creado, incluyendo su identificador asignado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del rol no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar crear el rol en la base de datos.
        /// </exception>
        public async Task<RolDTO> CreateAsync(RolDTO RolDto)
        {
            ValidateRol(RolDto);

            try
            {
                var rol = MapToEntity(RolDto);

                var rolCreado = await _rolData.CreateAsync(rol);

                return MapToDTO(rolCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Rol: {RolNombre}", RolDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el Rol", ex);
            }
        }


        /// <summary>
        /// Actualiza un Rol existente en la base de datos con los datos proporcionados en el <see cref="RolDTO"/>.
        /// </summary>
        /// <param name="rolDTO">Objeto <see cref="RolDTO"/> con la información actualizada del Rol. Debe contener un ID válido.</param>
        /// <returns>Un valor booleano que indica si la operación de actualización fue exitosa.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del rol no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún rol con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el rol en la base de datos.
        /// </exception>
        public async Task<bool> UpdateAsync(RolDTO rolDTO)
        {
            if (rolDTO.Id <= 0)
            {
                throw new ValidationException("El ID del Rol debe ser mayor a cero.");
            }

            ValidateRol(rolDTO);

            var existingRol = await _rolData.GetByIdAsync(rolDTO.Id);
            if (existingRol == null)
            {
                throw new EntityNotFoundException("Rol", rolDTO.Id);
            }

            try
            {
                existingRol.Id = rolDTO.Id;
                existingRol.Name = rolDTO.Name;
                existingRol.Description = rolDTO.Description;
                existingRol.Active = rolDTO.Status;

                return await _rolData.UpdateAsync(existingRol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el Rol con ID: {UserId}", rolDTO.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el Rol.", ex);
            }
        }


        /// <summary>
        /// Elimina un Rol existente por su identificador.
        /// </summary>
        /// <param name="id">Identificador único del Rol a eliminar.</param>
        /// <returns>Un valor booleano que indica si la eliminación fue exitosa.</returns>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún Rol con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar eliminar el Rol desde la base de datos.
        /// </exception>
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del Rol debe ser un número mayor a cero.", nameof(id));
            }

            var existingRol = await _rolData.GetByIdAsync(id);
            if (existingRol == null)
            {
                throw new EntityNotFoundException("Rol", id);
            }

            try
            {
                return await _rolData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el Rol con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el Rol.", ex);
            }
        }


        /// <summary>
        /// Elimina un Rol existente de manera logica por su identificador.
        /// </summary>
        /// <param name="id">Identificador único del Rol a eliminar de manera logica.</param>
        /// <returns>Un valor booleano que indica si la eliminación logica fue exitosa.</returns>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún Rol con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar eliminar de manera logica el Rol desde la base de datos.
        /// </exception>
        public async Task<bool> DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del Rol debe ser un número mayor a cero.", nameof(id));
            }

            var existingRol = await _rolData.GetByIdAsync(id);
            if (existingRol == null)
            {
                throw new EntityNotFoundException("Rol", id);
            }

            try
            {
                return await _rolData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el Rol con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el Rol.", ex);
            }
        }


        /// <summary>
        /// Metodo para Validar el Rol
        /// </summary>
        /// <param name="RolDto"></param>
        /// <exception cref="Utilities.Exceptions.ValidationException"></exception>
        private void ValidateRol(RolDTO RolDto)
        {
            if (RolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(RolDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Rol con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del Rol es obligatorio");
            }
        }


        /// <summary>
        /// Método para mapear de Rol a RolDTO
        /// </summary>
        /// <param name="rol"></param>
        /// <returns></returns>
        private RolDTO MapToDTO(Rol rol)
        {
            return new RolDTO
            {
                Id = rol.Id,
                Name = rol.Name,
                Description = rol.Description,
                Status = rol.Active
            };
        }


        /// <summary>
        /// Metodo para mapear de RolDTO a Rol 
        /// </summary>
        /// <param name="rolDTO"></param>
        /// <returns></returns>
        private Rol MapToEntity(RolDTO rolDTO)
        {
            return new Rol
            {
                Id = rolDTO.Id,
                Name = rolDTO.Name,
                Description = rolDTO.Description,
                Active = rolDTO.Status
            };
        }


        /// <summary>
        /// Metodo para mapear una lista de Rol a una lista de RolDTO 
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        private IEnumerable<RolDTO> MapToDTOList(IEnumerable<Rol> roles)
        {
            var rolesDTO = new List<RolDTO>();
            foreach (var rol in roles)
            {
                rolesDTO.Add(MapToDTO(rol));
            }
            return rolesDTO;
        }

        public Task<bool> DeleteAsync(int id, DeleteType deleteType)
        {
            throw new NotImplementedException();
        }
    }
}
