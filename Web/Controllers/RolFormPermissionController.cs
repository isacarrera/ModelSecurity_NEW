using Business.Interfaces;
using Entity.DTOs.RolFormPermissionDTOs;
using Entity.Enums;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de RolFormPermissions en el sistema
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Produces("application/json")]

    public class RolFormPermissionController : ControllerBase
    {
        private readonly IBusiness<RolFormPermissionDTO, RolFormPermissionOptionsDTO> _rolFormPermissionBusiness;
        private readonly ILogger<RolFormPermissionController> _logger;

        /// <summary>
        /// Constructor del controlador de RolFormPermissions
        /// </summary>
        /// <param name="PermissionBusiness">Capa de negocio de RolFormPermissions</param>
        /// <param name="logger">Logger para registro de RolFormPermissions</param>

        public RolFormPermissionController(IBusiness<RolFormPermissionDTO, RolFormPermissionOptionsDTO> rolFormPermissionBusiness, ILogger<RolFormPermissionController> logger)
        {
            _rolFormPermissionBusiness = rolFormPermissionBusiness;
            _logger = logger;
        }


        /// <summary>
        /// Obtiene todos los RolFormPermissions del sistema
        /// </summary>
        /// <returns>Lista de RolFormPermissiones</returns>
        /// <response code="200">Retorna la lista de RolFormPermissions</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<RolFormPermissionDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolFormPermissions()
        {
            try
            {
                var RolFormPermissions = await _rolFormPermissionBusiness.GetAllAsync();
                return Ok(RolFormPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los rolFormPermissions");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Obtiene un RolFormPermission específico por su ID
        /// </summary>
        /// <param name="id">ID del RolFormPermission</param>
        /// <returns>RolFormPermission solicitado</returns>
        /// <response code="200">Retorna el RolFormPermission solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">RolFormPermission no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetByiId/{id}/")]
        [ProducesResponseType(typeof(RolFormPermissionDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolFormPermissionById(int id)
        {
            try
            {
                var RolFormPermission = await _rolFormPermissionBusiness.GetByIdAsync(id);
                return Ok(RolFormPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida para rolFormPermissionBusiness con ID: {RolFormPermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "RolFormPermission no encontrado con ID: {RolFormPermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el rolFormPermissionBusiness con ID: {RolFormPermissionId}", id);
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo RolFormPermission en el sistema
        /// </summary>
        /// <param name="RolFormPermissionOptionsDTO">Datos del RolFormPermission a crear</param>
        /// <returns>permission creado</returns>
        /// <response code="201">Retorna el RolFormPermission creado</response>
        /// <response code="400">Datos del RolFormPermission no válidos</response>
        /// <response code="500">Error interno del servidor</response>        
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(RolFormPermissionOptionsDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolFormPermission([FromBody] RolFormPermissionOptionsDTO rolFormPermissionDTO)
        {
            try
            {
                var createdRolFormPermission = await _rolFormPermissionBusiness.CreateAsync(rolFormPermissionDTO);
                return CreatedAtAction(nameof(GetRolFormPermissionById), new { id = createdRolFormPermission.Id }, createdRolFormPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida al crear el rolFormPermissionBusiness");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el rolFormPermissionBusiness");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Actualiza un RolFormPermission existente en el sistema
        /// </summary>
        /// <param name="id">ID del RolFormPermission a actualizar</param>
        /// <param name="permissionDto">Datos actualizados del RolFormPermission</param>
        /// <returns>RolFormPermission actualizado</returns>
        /// <response code="200">Retorna el RolFormPermission actualizado</response>
        /// <response code="400">Datos inválidos o ID incorrecto</response>
        /// <response code="404">RolFormPermission no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(RolFormPermissionOptionsDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolFormPermission([FromBody] RolFormPermissionOptionsDTO rolFormPermissionDTO)
        {
            try
            {
                var updatedRolFormPermission = await _rolFormPermissionBusiness.UpdateAsync(rolFormPermissionDTO);

                return Ok(updatedRolFormPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el rolFormPermissionBusiness con ID: {RolFormPermissionId}", rolFormPermissionDTO.Id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el rolFormPermissionBusiness con ID: {RolFormPermissionId}", rolFormPermissionDTO.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el rolFormPermissionBusiness con ID: {RolFormPermissionId}", rolFormPermissionDTO.Id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina un RolFormPermission del sistema. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del RolFormPermission a eliminar</param>
        /// <returns>Mensaje de confirmación</returns>
        /// <response code="200">El RolFormPermission fue eliminado exitosamente</response>
        /// <response code="400">Parametro Incorrecto</response>
        /// <response code="404">RolFormPermission no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("Delete/{id}/")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRolFormPermission(int id, [FromQuery] DeleteType strategy = DeleteType.Logical)
        {
            try
            {
                await _rolFormPermissionBusiness.DeleteAsync(id, strategy);
                return Ok(new { message = $"Eliminación con estrategy {strategy} exitosa." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ID inválido para eliminación de RolFormPermission: {RolFormPermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el RolFormPermission con ID: {RolFormPermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el RolFormPermission con ID: {RolFormPermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}