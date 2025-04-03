using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las sedes en el sistema.
    /// </summary>
    public class SedeBusiness
    {
        private readonly SedeData _sedeData;
        private readonly ILogger _logger;

        public SedeBusiness(SedeData sedeData, ILogger logger)
        {
            _sedeData = sedeData;
            _logger = logger;
        }

        // Método para obtener todas las sedes como DTOs
        public async Task<IEnumerable<SedeDto>> GetAllSedesAsync()
        {
            try
            {
                var sedes = await _sedeData.GetAllAsync();
                var sedesDTO = new List<SedeDto>();

                foreach (var sede in sedes)
                {
                    sedesDTO.Add(new SedeDto
                    {
                        id = sede.id,
                        name = sede.name,
                        address = sede.addres,
                        active = sede.active // si existe la entidad
                    });
                }

                return sedesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las sedes");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de sedes", ex);
            }
        }

        // Método para obtener una sede por ID como DTO
        public async Task<SedeDto> GetSedeByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una sede con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la sede debe ser mayor que cero");
            }

            try
            {
                var sede = await _sedeData.GetByIdAsync(id);
                if (sede == null)
                {
                    _logger.LogInformation("No se encontró ninguna sede con ID: {Id}", id);
                    throw new EntityNotFoundException("sede", id);
                }

                return new SedeDto
                {
                    id = sede.id,
                    name = sede.name,
                    address = sede.addres,
                    active = sede.active // si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la sede con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la sede con ID {id}", ex);
            }
        }

        // Método para crear una sede desde un DTO
        public async Task<SedeDto> CreateSedeAsync(SedeDto sedeDto)
        {
            try
            {
                ValidateSede(sedeDto);

                var sede = new Sede
                {
                    name = sedeDto.name,
                    addres = sedeDto.address,
                    active = sedeDto.active // si existe en la entidad
                };

                var sedeCreada = await _sedeData.CreateAsync(sede);

                return new SedeDto
                {
                    id = sedeCreada.id,
                    name = sedeCreada.name,
                    address = sedeCreada.addres,
                    active = sedeCreada.active // si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva sede: {Name}", sedeDto?.name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la sede", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateSede(SedeDto sedeDto)
        {
            if (sedeDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Sede no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(sedeDto.name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una sede con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la sede es obligatorio");
            }
        }
    }
}
