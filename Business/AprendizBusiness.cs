using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los aprendices del sistema.
    /// </summary>
    public class AprendizBusiness
    {
        private readonly AprendizData _aprendizData;
        private readonly ILogger _logger;

        public AprendizBusiness(AprendizData aprendizData, ILogger logger)
        {
            _aprendizData = aprendizData;
            _logger = logger;
        }

        // Método para obtener todos los aprendices como DTOs
        public async Task<IEnumerable<AprendizDto>> GetAllAprendizAsync()
        {
            try
            {
                var aprendices = await _aprendizData.GetAllAsync();
                var aprendicesDTO = new List<AprendizDto>();

                foreach (var aprendiz in aprendices)
                {
                    aprendicesDTO.Add(new AprendizDto
                    {
                        Id = aprendiz.Id,
                        PreviuosProgram = aprendiz.PreviuosProgram,
                        UserId = aprendiz.UserId,
                        Active = aprendiz.Active //si existe la entidad
                    });
                }

                return aprendicesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los aprendices");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de aprendices", ex);
            }
        }

        // Método para obtener un aprendiz por ID como DTO
        public async Task<AprendizDto> GetAprendizByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un aprendiz con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del aprendiz debe ser mayor que cero");
            }

            try
            {
                var aprendiz = await _aprendizData.GetByIdAsync(id);
                if (aprendiz == null)
                {
                    _logger.LogInformation("No se encontró ningún aprendiz con ID: {Id}", id);
                    throw new EntityNotFoundException("Aprendiz", id);
                }

                return new AprendizDto
                {
                    Id = aprendiz.Id,
                    PreviuosProgram = aprendiz.PreviuosProgram,
                    UserId = aprendiz.UserId,
                    Active = aprendiz.Active //si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el aprendiz con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el aprendiz con ID {id}", ex);
            }
        }

        // Método para crear un usuario desde un DTO
        public async Task<AprendizDto> CreateAprendizAsync(AprendizDto aprendizDto)
        {
            try
            {
                ValidateAprendiz(aprendizDto);

                var aprendiz = new Aprendiz
                {
                    PreviuosProgram = aprendizDto.PreviuosProgram,
                    UserId = aprendizDto.UserId,
                    Active = aprendizDto.Active //si existe la entidad
                };

                var aprendizCreado = await _aprendizData.CreateAsync(aprendiz);

                return new AprendizDto
                {
                    Id = aprendiz.Id,
                    PreviuosProgram = aprendiz.PreviuosProgram,
                    UserId = aprendiz.UserId,
                    Active = aprendiz.Active //si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo aprendiz: {Name}", aprendizDto?.PreviuosProgram ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el aprendiz", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateAprendiz(AprendizDto aprendizDto)
        {
            if (aprendizDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto aprendiz no puede ser nulo");
            }


            
        }
    }
}
