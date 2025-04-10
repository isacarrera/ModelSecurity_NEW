using Business.Interfaces;
using Entity.DTOs.FormModuleDTOs;
using Entity.Enums;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de FormModules en el sistema
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Produces("application/json")]

    public class FormModuleController : ControllerBase
    {
        private readonly IBusiness<FormModuleDTO,FormModuleOptionsDTO> _formModuleBusiness;
        private readonly ILogger<FormModuleController> _logger;

        /// <summary>
        /// Constructor del controlador de FormModule
        /// </summary>
        /// <param name="FormModuleBusiness">Capa de negocio de FormModule</param>
        /// <param name="logger">Logger para registro de FormModule</param>
        public FormModuleController(IBusiness<FormModuleDTO, FormModuleOptionsDTO> formModuleBusiness, ILogger<FormModuleController> logger)
        {
            _formModuleBusiness = formModuleBusiness;
            _logger = logger;
        }


        /// <summary>
        /// Obtiene todos los FormModules del sistema
        /// </summary>
        /// <returns>Lista de FormModulees</returns>
        /// <response code="200">Retorna la lista de FormModules</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<FormModuleDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllFormModules()
        {
            try
            {
                var FormModules = await _formModuleBusiness.GetAllAsync();
                return Ok(FormModules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los formModules");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Obtiene un FormModule específico por su ID
        /// </summary>
        /// <param name="id">ID del FormModule</param>
        /// <returns>FormModule solicitado</returns>
        /// <response code="200">Retorna el FormModule solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">FormModule no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetByiId/{id}/")]
        [ProducesResponseType(typeof(FormModuleDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFormModuleById(int id)
        {
            try
            {
                var Form = await _formModuleBusiness.GetByIdAsync(id);
                return Ok(Form);
            }
            catch (ValidationException ex)
            {
                _logger.LogInformation(ex, "Validacion fallida para formModule con ID: {FormModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "FormModule no encontrado con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el formModule con ID: {FormMOduleId}", id);
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo FormModule en el sistema
        /// </summary>
        /// <param name="FormModuleDTO">Datos del FormModule a crear</param>
        /// <returns>formModule creado</returns>
        /// <response code="201">Retorna el FormModule creado</response>
        /// <response code="400">Datos del FormModule no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(FormModuleOptionsDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateFormModule([FromBody] FormModuleOptionsDTO formModuleOptionsDTO)
        {
            try
            {
                var createdFormModule = await _formModuleBusiness.CreateAsync(formModuleOptionsDTO);
                return CreatedAtAction(nameof(GetFormModuleById), new { id = createdFormModule.Id }, createdFormModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida al crear el formModule");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el formModule");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Actualiza un FormModule existente en el sistema
        /// </summary>
        /// <param name="id">ID del FormModule a actualizar</param>
        /// <param name="formModuleDto">Datos actualizados del FormModule</param>
        /// <returns>FormModule actualizado</returns>
        /// <response code="200">Retorna el FormModule actualizado</response>
        /// <response code="400">Datos inválidos o ID incorrecto</response>
        /// <response code="404">FormModule no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(FormModuleDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateFormModule([FromBody] FormModuleOptionsDTO formModuleDTO)
        {
            try
            {
                var updatedFormModule = await _formModuleBusiness.UpdateAsync(formModuleDTO);
                return Ok(updatedFormModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el formModule con ID: {FormModuleId}",formModuleDTO.Id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el formModule con ID: {FormModuleId}",formModuleDTO.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el formModule con ID: {FormModuleId}",formModuleDTO.Id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina un FormModule del sistema. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del FormModule a eliminar</param>
        /// <returns>Mensaje de confirmación</returns>
        /// <response code="200">El FormModule fue eliminado exitosamente</response>
        /// <response code="400">Parametro Incorrecto</response>
        /// <response code="404">FormModule no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("Delete/{id}/")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteFormModule(int id, [FromQuery] DeleteType strategy = DeleteType.Logical)
        {
            try
            {
                await _formModuleBusiness.DeleteAsync(id, strategy);
                return Ok(new { message = $"Eliminación con estrategy {strategy} exitosa." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ID inválido para eliminación de FormModule: {FormModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el FormModule con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el FormModule con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
