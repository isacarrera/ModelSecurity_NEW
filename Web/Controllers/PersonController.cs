using Business.Interfaces;
using Entity.DTOs;
using Entity.Enums;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Persons en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PersonController : ControllerBase
    {
        private readonly IBusiness<PersonDTO, PersonDTO> _personBusiness;
        private readonly ILogger<PersonController> _logger;

        /// <summary>
        /// Constructor del controlador de Person
        /// </summary>
        /// <param name="PersonBusiness">Capa de negocio de Person</param>
        /// <param name="logger">Logger para registro de Person</param>
        public PersonController(IBusiness<PersonDTO, PersonDTO> personBusiness, ILogger<PersonController> logger)
        {
            _personBusiness = personBusiness;
            _logger = logger;
        }


        /// <summary>
        /// Obtiene todos los Persons del sistema
        /// </summary>
        /// <returns>Lista de Persones</returns>
        /// <response code="200">Retorna la lista de Persons</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<PersonDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPersons()
        {
            try
            {
                var Persons = await _personBusiness.GetAllAsync();
                return Ok(Persons);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error al obtener los Persons");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Obtiene un Person específico por su ID
        /// </summary>
        /// <param name="id">ID del Person</param>
        /// <returns>Person solicitado</returns>
        /// <response code="200">Retorna el Person solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Person no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetByiId/{id}/")]
        [ProducesResponseType(typeof(PersonDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPersonById(int id)
        {
            try
            {
                var Person = await _personBusiness.GetByIdAsync(id);
                return Ok(Person);
            }
            catch (ValidationException ex)
            {
                _logger.LogInformation(ex, "Validacion fallida para Person con ID: {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "Person no encontrado con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el Person con ID: {PersonId}", id);
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo Person en el sistema
        /// </summary>
        /// <param name="PersonDTO">Datos del Person a crear</param>
        /// <returns>permission creado</returns>
        /// <response code="201">Retorna el Person creado</response>
        /// <response code="400">Datos del Person no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(RolDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePersonAsync([FromBody] PersonDTO personDto)
        {
            try
            {
                var createdPerson = await _personBusiness.CreateAsync(personDto);
                return CreatedAtAction(nameof(GetPersonById), new { id = createdPerson.Id }, createdPerson);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida al crear Person");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear Person");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// Actualiza un Person existente en el sistema
        /// </summary>
        /// <param name="id">ID del Person a actualizar</param>
        /// <param name="permissionDto">Datos actualizados del Person</param>
        /// <returns>Person actualizado</returns>
        /// <response code="200">Retorna el Person actualizado</response>
        /// <response code="400">Datos inválidos o ID incorrecto</response>
        /// <response code="404">Person no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(RolDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePersonAsync([FromBody] PersonDTO personDto)
        {
            try
            {
                var updatedPerson = await _personBusiness.UpdateAsync(personDto);

                return Ok(updatedPerson);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar Person con ID: {PersonId}", personDto.Id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró Person con ID: {PersonId}", personDto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar Person con ID: {PersonId}", personDto.Id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina un Person del sistema. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del Person a eliminar</param>
        /// <returns>Mensaje de confirmación</returns>
        /// <response code="200">El Person fue eliminado exitosamente</response>
        /// <response code="400">Parametro Incorrecto</response>
        /// <response code="404">Person no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("Delete/{id}/")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePerson(int id, [FromQuery] DeleteType strategy = DeleteType.Logical)
        {
            try
            {
                await _personBusiness.DeleteAsync(id, strategy);
                return Ok(new { message = $"Eliminación con estrategy {strategy} exitosa." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ID inválido para eliminación de Person: {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el Person con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el Person con ID: {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
