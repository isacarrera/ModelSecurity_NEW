using Business;
using Business.Interfaces;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Rols en el sistema
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Produces("application/json")]

    public class RolController : ControllerBase
    {
    
        private readonly IBusiness<RolDTO,RolDTO> _rolBusiness;
        private readonly ILogger<RolController> _logger;

        /// <summary>
        /// Constructor del controlador de Rol
        /// </summary>
        /// <param name="RolBusiness">Capa de negocio de Rol</param>
        /// <param name="logger">Logger para registro de Rol</param>
        public RolController(RolBusiness rolBusiness, ILogger<RolController> logger)
        {
            _rolBusiness = rolBusiness;
            _logger = logger;
        }


        /// <summary>
        /// Obtiene todos los Rols del sistema
        /// </summary>
        /// <returns>Lista de Roles</returns>
        /// <response code="200">Retorna la lista de Rols</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<RolDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRols()
        {
            try
            {
                var Rols = await _rolBusiness.GetAllAsync();
                return Ok(Rols);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener Rols");
                return StatusCode(500, new {message = ex.Message});
            }
        }


        /// <summary>
        /// Obtiene un Rol específico por su ID
        /// </summary>
        /// <param name="id">ID del Rol</param>
        /// <returns>Rol solicitado</returns>
        /// <response code="200">Retorna el Rol solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Rol no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetById{id}/")]
        [ProducesResponseType(typeof(RolDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolById(int id)
        {

            try
            {
                var Rol = await _rolBusiness.GetByIdAsync(id);   
                return Ok(Rol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida para el Rol con ID: {RolId}", id);
                return BadRequest(new {message = ex.Message});
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el Rol con ID: {RolId}", id);
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo Rol en el sistema
        /// </summary>
        /// <param name="RolDTO">Datos del Rol a crear</param>
        /// <returns>rol creado</returns>
        /// <response code="201">Retorna el Rol creado</response>
        /// <response code="400">Datos del ROl no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(RolDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolAsync([FromBody] RolDTO rolDto)
        {
            try
            {
                var createdRol = await _rolBusiness.CreateAsync(rolDto);
                return CreatedAtAction(nameof(GetRolById), new { id = createdRol.Id }, createdRol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida al creal el Rol");
                return BadRequest(new {message = ex.Message});
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el Rol");
                return StatusCode(500, new { message = ex.Message});
            }
        }

        
        /// <summary>
        /// Actualiza un Rol existente en el sistema
        /// </summary>
        /// <param name="id">ID del Rol a actualizar</param>
        /// <param name="rolDto">Datos actualizados del Rol</param>
        /// <returns>Rol actualizado</returns>
        /// <response code="200">Retorna el Rol actualizado</response>
        /// <response code="400">Datos inválidos o ID incorrecto</response>
        /// <response code="404">Rol no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(RolDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolAsync([FromBody] RolDTO rolDto)
        {
            try
            {
                var updatedRol = await _rolBusiness.UpdateAsync(rolDto);

                return Ok(new { message = "Rol actualizado exitosamente", result = updatedRol });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el Rol con ID: {RolId}", rolDto.Id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el Rol con ID: {RolId}", rolDto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el Rol con ID: {RolId}", rolDto.Id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina un Rol del sistema
        /// </summary>
        /// <param name="id">ID del Rol a eliminar</param>
        /// <returns>Mensaje de confirmación</returns>
        /// <response code="200">El Rol fue eliminado exitosamente</response>
        /// <response code="400">Parametro Incorrecto</response>
        /// <response code="404">Rol no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("Delete/{id}/")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRolAsync(int id)
        {
            try
            {
                await _rolBusiness.DeletePersistenceAsync(id);
                return Ok(new { message = "Rol eliminado exitosamente" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ID inválido para eliminación de Rol: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el Ril con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina un Rol de manera logcica del sistema
        /// </summary>
        /// <param name="id">ID del Rol a eliminar de manera logica</param>
        /// <returns>Mensaje de confirmación</returns>
        /// <response code="200">El Rol fue eliminado de manera logica exitosamente</response>
        /// <response code="400">Parametro Incorrecto</response>
        /// <response code="404">Rol no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("DeleteLogical/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteLogicalFormAsync(int id)
        {
            try
            {
                await _rolBusiness.DeleteLogicAsync(id);
                return Ok(new { message = "Eliminación lógica exitosa." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ID inválido para eliminación logica de Rol: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el Rol con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el Rol de manera lógica con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
