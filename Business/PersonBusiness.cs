using Data;
using Entity.DTOs.Person;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las personas en el sistema.
    /// </summary>
    public class PersonBusiness
    {
        private readonly PersonData _personData;
        private readonly ILogger<PersonData> _logger;

        public PersonBusiness(PersonData personData, ILogger<PersonData> logger)
        {
            _personData = personData;
            _logger = logger;
        }

        public async Task<IEnumerable<PersonDto>> GetAllPeopleAsync()
        {
            try
            {
                var people = await _personData.GetAllAsync();
                return MapToDTOList(people);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las personas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de personas", ex);
            }
        }

        public async Task<PersonDto> GetPersonByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una persona con ID inválido: {PersonId}", id);
                throw new ValidationException("id", "El ID de la persona debe ser mayor que cero");
            }

            try
            {
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ninguna persona con ID: {PersonId}", id);
                    throw new EntityNotFoundException("Person", id);
                }

                return MapToDTO(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la persona con ID {id}", ex);
            }
        }

        public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
        {
            try
            {
                ValidatePerson(personDto);

                var person = MapToEntity(personDto);
                var created = await _personData.CreateAsync(person);

                return MapToDTO(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva persona: {Name}", personDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la persona", ex);
            }
        }

        public async Task<bool> UpdateParcialPersonAsync(PersonUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización inválido");
                throw new ValidationException("Id", "Datos inválidos para actualizar persona");
            }

            try
            {
                var entity = await _personData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("Persona no encontrada con ID: {Id}", dto.Id);
                    throw new EntityNotFoundException("Person", dto.Id);
                }

                // Actualizar solo los campos parciales
               
           
                entity.FirstName = dto.FirstName;
                entity.SecondName = dto.SecondName;
                entity.FirstLastName = dto.FirstLastName;
                entity.SecondLastName = dto.SecondLastName;
                entity.PhoneNumber = dto.PhoneNumber;
                entity.Email = dto.Email;
                entity.NumberIdentification = dto.NumberIdentification;

                return await _personData.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la persona con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar persona con ID {dto.Id}", ex);
            }
        }


        public async Task<bool> SetPersonActiveAsync(PersonStatusDto dto)
        {
            if (dto == null)
                throw new ValidationException("El DTO de estado de persona no puede ser nulo");

            if (dto.Id <= 0)
            {
                _logger.LogWarning("ID inválido para cambiar estado activo de persona: {Id}", dto.Id);
                throw new ValidationException("Id", "El ID de la persona debe ser mayor a 0");
            }

            try
            {
                var entity = await _personData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("Persona no encontrada con ID {Id} para cambiar estado", dto.Id);
                    throw new EntityNotFoundException("Person", dto.Id);
                }

              
                return await _personData.SetActiveAsync(dto.Id, dto.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de persona con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar estado activo de persona con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> DeletePersonAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una persona con ID inválido: {Id}", id);
                throw new ValidationException("Id", "El ID debe ser mayor a 0");
            }

            try
            {
                var exists = await _personData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("Persona no encontrada con ID {Id} para eliminar", id);
                    throw new EntityNotFoundException("Person", id);
                }

                return await _personData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar persona con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar persona con ID {id}", ex);
            }
        }

        private void ValidatePerson(PersonDto personDto)
        {
            if (personDto == null)
                throw new ValidationException("El objeto persona no puede ser nulo");

            if (string.IsNullOrWhiteSpace(personDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con Name vacío");
                throw new ValidationException("Name", "El Name de la persona es obligatorio");
            }
        }

        private PersonDto MapToDTO(Person person)
        {
            return new PersonDto
            {
                Id = person.Id,
                Name = person.Name,
                FirstName = person.FirstName,
                SecondName = person.SecondName,
                FirstLastName = person.FirstLastName,
                SecondLastName = person.SecondLastName,
                PhoneNumber = person.PhoneNumber,
                Email = person.Email,
                TypeIdentification = person.TypeIdentification,
                NumberIdentification = person.NumberIdentification,
                Signing = person.Signing,
                Active = person.Active,
            };
        }

        private Person MapToEntity(PersonDto personDto)
        {
            return new Person
            {
                Id = personDto.Id,
                Name = personDto.Name,
                FirstName = personDto.FirstName,
                SecondName = personDto.SecondName,
                FirstLastName = personDto.FirstLastName,
                SecondLastName = personDto.SecondLastName,
                PhoneNumber = personDto.PhoneNumber,
                Email = personDto.Email,
                TypeIdentification = personDto.TypeIdentification,
                NumberIdentification = personDto.NumberIdentification,
                Signing = personDto.Signing,
                Active = personDto.Active,
            };
        }

        private IEnumerable<PersonDto> MapToDTOList(IEnumerable<Person> people)
        {
            return people.Select(MapToDTO).ToList();
        }
    }
}
