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
               
                return MapToDTOList(aprendices);
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

                return MapToDTO(aprendiz);
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

                var aprendiz = MapToEntity(aprendizDto);

                var aprendizCreado = await _aprendizData.CreateAsync(aprendiz);

                return MapToDTO(aprendizCreado);
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
        // Método para mapear de Aprendiz a AprendizDto
        private AprendizDto MapToDTO(Aprendiz aprendiz)
        {
            return new AprendizDto
            {
                Id = aprendiz.Id,
                PreviuosProgram = aprendiz.PreviuosProgram,
                UserId = aprendiz.UserId,
                Active = aprendiz.Active,
                AprendizProgramId = aprendiz.AprendizProgramId,
                AprendizProcessInstructorId = aprendiz.AprendizProcessInstructorId,
            };
        }

        // Método para mapear de AprendizDto a Aprendiz
        private Aprendiz MapToEntity(AprendizDto aprendizDto)
        {
            return new Aprendiz
            {
                Id = aprendizDto.Id,
                PreviuosProgram = aprendizDto.PreviuosProgram,
                UserId = aprendizDto.UserId,
                Active = aprendizDto.Active,
                AprendizProgramId = aprendizDto.AprendizProgramId,
                AprendizProcessInstructorId = aprendizDto.AprendizProcessInstructorId,
            
            };
        }

        // Método para mapear una lista de Aprendiz a lista de AprendizDto
        private IEnumerable<AprendizDto> MapToDTOList(IEnumerable<Aprendiz> aprendices)
        {
            var aprendicesDto = new List<AprendizDto>();
            foreach (var aprendiz in aprendices)
            {
                aprendicesDto.Add(MapToDTO(aprendiz));
            }
            return aprendicesDto;
        }
    }
}
