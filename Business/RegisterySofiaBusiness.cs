using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los registros de Sofia en el sistema.
    /// </summary>
    public class RegisterySofiaBusiness
    {
        private readonly RegisterySofiaData _registerySofiaData;
        private readonly ILogger _logger;

        public RegisterySofiaBusiness(RegisterySofiaData registerySofiaData, ILogger logger)
        {
            _registerySofiaData = registerySofiaData;
            _logger = logger;
        }

        // Método para obtener todos los registros de Sofia como DTOs
        public async Task<IEnumerable<RegisterySofiaDto>> GetAllRegisterySofiasAsync()
        {
            try
            {
                var registerySofias = await _registerySofiaData.GetAllAsync();
                var registerySofiasDTO = new List<RegisterySofiaDto>();

                foreach (var registerySofia in registerySofias)
                {
                    registerySofiasDTO.Add(new RegisterySofiaDto
                    {
                        Id = registerySofia.Id,
                        Name = registerySofia.Name,
                        Description = registerySofia.Description,
                        Document = registerySofia.Document,
                        Active = registerySofia.Active // si existe la entidad
                    });
                }

                return registerySofiasDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de Sofia");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de registros de Sofia", ex);
            }
        }

        // Método para obtener un registro de Sofia por ID como DTO
        public async Task<RegisterySofiaDto> GetRegisterySofiaByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un registro de Sofia con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del registro de Sofia debe ser mayor que cero");
            }

            try
            {
                var registerySofia = await _registerySofiaData.GetByIdAsync(id);
                if (registerySofia == null)
                {
                    _logger.LogInformation("No se encontró ningún registro de Sofia con ID: {Id}", id);
                    throw new EntityNotFoundException("registerySofia", id);
                }

                return new RegisterySofiaDto
                {
                    Id = registerySofia.Id,
                    Name = registerySofia.Name,
                    Description = registerySofia.Description,
                    Document = registerySofia.Document,
                    Active = registerySofia.Active // si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de Sofia con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el registro de Sofia con ID {id}", ex);
            }
        }

        // Método para crear un registro de Sofia desde un DTO
        public async Task<RegisterySofiaDto> CreateRegisterySofiaAsync(RegisterySofiaDto registerySofiaDto)
        {
            try
            {
                ValidateRegisterySofia(registerySofiaDto);

                var registerySofia = new RegisterySofia
                {
                    Name = registerySofiaDto.Name,
                    Description = registerySofiaDto.Description,
                    Document = registerySofiaDto.Document,
                    Active = registerySofiaDto.Active // si existe la entidad
                };

                var registerySofiaCreado = await _registerySofiaData.CreateAsync(registerySofia);

                return new RegisterySofiaDto
                {
                    Id = registerySofia.Id,
                    Name = registerySofia.Name,
                    Description = registerySofia.Description,
                    Document = registerySofia.Document,
                    Active = registerySofia.Active // si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo registro de Sofia: {Name}", registerySofiaDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el registro de Sofia", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRegisterySofia(RegisterySofiaDto registerySofiaDto)
        {
            if (registerySofiaDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto RegisterySofia no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(registerySofiaDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un registro de Sofia con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del registro de Sofia es obligatorio");
            }
        }
    }
}
