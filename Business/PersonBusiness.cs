﻿using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las personas en el sistema.
    /// </summary>
    public class PersonBusiness
    {
        private readonly PersonData _personData;
        private readonly ILogger _logger;

        public PersonBusiness(PersonData personData, ILogger logger)
        {
            _personData = personData;
            _logger = logger;
        }

        // Método para obtener todas las personas como DTOs
        public async Task<IEnumerable<PersonDto>> GetAllPersonsAsync()
        {
            try
            {
                var persons = await _personData.GetAllAsync();
                var personsDTO = new List<PersonDto>();

                foreach (var person in persons)
                {
                    personsDTO.Add(new PersonDto
                    {
                        id = person.id,
                        name = person.name,
                    });
                }

                return personsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las personas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de personas", ex);
            }
        }

        // Método para obtener una persona por ID como DTO
        public async Task<PersonDto> GetPersonByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una persona con ID inválido: {PersonId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la persona debe ser mayor que cero");
            }

            try
            {
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ninguna persona con ID: {PersonId}", id);
                    throw new EntityNotFoundException("Person", id);
                }

                return new PersonDto
                {
                    id = person.id,
                    name = person.name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la persona con ID {id}", ex);
            }
        }

        // Método para crear una persona desde un DTO
        public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
        {
            try
            {
                ValidatePerson(personDto);

                var person = new Person
                {
                   name = personDto.name,
                };

                var personCreada = await _personData.CreateAsync(person);

                return new PersonDto
                {
                    id = person.id,
                    name = person.name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva persona: {Name}", personDto?.name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la persona", ex);
            }
        }

        // Método para validar el DTO
        private void ValidatePerson(PersonDto personDto)
        {
            if (personDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto persona no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(personDto.name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la persona es obligatorio");
            }
        }
    }
}
