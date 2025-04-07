using Data;
using Entity.DTOautogestion;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los instructores en el sistema.
    /// </summary>
    public class InstructorBusiness
    {
        private readonly InstructorData _instructorData;
        private readonly ILogger _logger;

        public InstructorBusiness(InstructorData instructorData, ILogger logger)
        {
            _instructorData = instructorData;
            _logger = logger;
        }

        // Método para obtener todos los instructores como DTOs
        public async Task<IEnumerable<InstructorDto>> GetAllInstructorsAsync()
        {
            try
            {
                var instructors = await _instructorData.GetAllAsync();
              
                return MapToDTOList(instructors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los instructores");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de instructores", ex);
            }
        }

        // Método para obtener un instructor por ID como DTO
        public async Task<InstructorDto> GetInstructorByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un instructor con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del instructor debe ser mayor que cero");
            }

            try
            {
                var instructor = await _instructorData.GetByIdAsync(id);
                if (instructor == null)
                {
                    _logger.LogInformation("No se encontró ningún instructor con ID: {Id}", id);
                    throw new EntityNotFoundException("instructor", id);
                }

                return MapToDTO(instructor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el instructor con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el instructor con ID {id}", ex);
            }
        }

        // Método para crear un instructor desde un DTO
        public async Task<InstructorDto> CreateInstructorAsync(InstructorDto instructorDto)
        {
            try
            {
                ValidateInstructor(instructorDto);

                var instructor = MapToEntity(instructorDto);

                var instructorCreado = await _instructorData.CreateAsync(instructor);

                return MapToDTO(instructorCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo instructor");
                throw new ExternalServiceException("Base de datos", "Error al crear el instructor", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateInstructor(InstructorDto instructorDto)
        {
            if (instructorDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Instructor no puede ser nulo");
            }

            if (instructorDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un instructor con UserId inválido");
                throw new Utilities.Exceptions.ValidationException("UserId", "El UserId del instructor es obligatorio y debe ser mayor que cero");
            }
        }

        //Metodo para mapear de Instructor a InstructorDto
        private InstructorDto MapToDTO(Instructor instructor)
        {
            return new InstructorDto
            {
                Id = instructor.Id,
                Active = instructor.Active,
                UserId = instructor.UserId, // Relación con la entidad User
                InstructorProgramId = instructor.InstructorProgramid
            };
        }
        //Metodo para mapear de InstructorDto a Instructor
        private Instructor MapToEntity(InstructorDto instructorDto)
        {
            return new Instructor
            {
                Id = instructorDto.Id,
                Active = instructorDto.Active,
                UserId = instructorDto.UserId, // Relación con la entidad User
                InstructorProgramid = instructorDto.AprendizProcessInstructorId,
            };
        }
        //Metodo para mapear una lista de Instructor a una lista de InstructorDto
        private IEnumerable<InstructorDto> MapToDTOList(IEnumerable<Instructor> instructors)
        {
            var instructorsDto = new List<InstructorDto>();
            foreach (var instructor in instructors)
            {
                instructorsDto.Add(MapToDTO(instructor));
            }
            return instructorsDto;
        }
    }
}
