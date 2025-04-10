using Business.Interfaces;
using Entity.DTOs;
using Entity.Enums;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Permissions en el sistema
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Produces("application/json")]

    public class PermissionController : ControllerBase
    {
        private readonly IBusiness<PermissionDTO, PermissionDTO> _permissionBusiness;
        private readonly ILogger<PermissionController> _logger;

        /// <summary>
        /// Constructor del controlador de Permission
        /// </summary>
        /// <param name="PermissionBusiness">Capa de negocio de Permission</param>
        /// <param name="logger">Logger para registro de Permission</param>
        public PermissionController(IBusiness<PermissionDTO, PermissionDTO> permissionBusiness, ILogger<PermissionController> logger)
        {
            _permissionBusiness = permissionBusiness;
            _logger = logger;
        }


        /// <summary>
        /// Obtiene todos los Permissions del sistema
        /// </summary>
        /// <returns>Lista de Permissiones</returns>
        /// <response code="200">Retorna la lista de Permissions</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<PermissionDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                var Permissions = await _permissionBusiness.GetAllAsync();
                return Ok(Permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los Permissions");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Obtiene un Permission específico por su ID
        /// </summary>
        /// <param name="id">ID del Permission</param>
        /// <returns>Permission solicitado</returns>
        /// <response code="200">Retorna el Permission solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Permission no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetByiId/{id}/")]
        [ProducesResponseType(typeof(PermissionDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            try
            {
                var Permission = await _permissionBusiness.GetByIdAsync(id);
                return Ok(Permission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida para Permission con ID: {PermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "Permission no encontrado con ID: {PermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el Permission con ID: {PermissionId}", id);
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo Permission en el sistema
        /// </summary>
        /// <param name="PermissionDTO">Datos del Permission a crear</param>
        /// <returns>permission creado</returns>
        /// <response code="201">Retorna el Permission creado</response>
        /// <response code="400">Datos del Permission no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(PermissionDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionDTO permissionDTO)
        {
            try
            {
                var createdPermission = await _permissionBusiness.CreateAsync(permissionDTO);
                return CreatedAtAction(nameof(GetPermissionById), new { id = createdPermission.Id }, createdPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida al crear el Permission");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el Permission");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Actualiza un Permission existente en el sistema
        /// </summary>
        /// <param name="id">ID del Permission a actualizar</param>
        /// <param name="permissionDto">Datos actualizados del Permission</param>
        /// <returns>Permission actualizado</returns>
        /// <response code="200">Retorna el Permission actualizado</response>
        /// <response code="400">Datos inválidos o ID incorrecto</response>
        /// <response code="404">Permission no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("Updated/")]
        [ProducesResponseType(typeof(PermissionDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePermission([FromBody] PermissionDTO permissionDTO)
        {
            try
            {
                var updatedPermission = await _permissionBusiness.UpdateAsync(permissionDTO);

                return Ok(updatedPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el Permission con ID: {PermissionId}", permissionDTO.Id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el Permission con ID: {PermissionId}", permissionDTO.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el Permission con ID: {PermissionId}", permissionDTO.Id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina un Permission del sistema. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del Permission a eliminar</param>
        /// <returns>Mensaje de confirmación</returns>
        /// <response code="200">El Permission fue eliminado exitosamente</response>
        /// <response code="400">Parametro Incorrecto</response>
        /// <response code="404">Permission no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("Delete/{id}/")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePermission(int id, [FromQuery] DeleteType strategy = DeleteType.Logical)
        {
            try
            {
                await _permissionBusiness.DeleteAsync(id, strategy);
                return Ok(new { message = $"Eliminación con estrategy {strategy} exitosa." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ID inválido para eliminación de Permission: {PermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el Permission con ID: {PermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el Permission con ID: {PermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}