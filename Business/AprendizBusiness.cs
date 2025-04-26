using Data;
using Entity.DTOs.Aprendiz;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los aprendices del sistema.
    /// </summary>
    public class AprendizBusiness
    {
        private readonly AprendizData _aprendizData;
        private readonly ILogger<AprendizData> _logger;

        public AprendizBusiness(AprendizData aprendizData, ILogger<AprendizData> logger)
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
                _logger.LogError(ex, "Error al crear nuevo aprendiz: {Name}", aprendizDto?.PreviousProgram ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el aprendiz", ex);
            }
        }


        /// <summary>
        /// Método para actualizar datos parcialmente (patch).
        /// </summary>
        public async Task<bool> UpdateParcialAprendizAsync(AprendizUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización de aprendiz inválido");
                throw new ValidationException("id", "Datos inválidos para actualizar aprendiz");
            }

            try
            {
                var exists = await _aprendizData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró el aprendiz con ID {AprendizId} para actualizar", dto.Id);
                    throw new EntityNotFoundException("Aprendiz", dto.Id);
                }

                return await _aprendizData.PatchAprendizAsync(dto.Id, dto.PreviousProgram);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar parcialmente el aprendiz con ID {dto.Id}");
                throw new ExternalServiceException("Base de datos", "Error al actualizar el aprendiz", ex);
            }
        }

        /// <summary>
        /// Método para actualizar los datos en su totalidad con (put).
        /// </summary>
        public async Task<bool> UpdateAprendizAsync(AprendizUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de reemplazo de aprendiz inválido");
                throw new ValidationException("id", "Datos inválidos para reemplazar aprendiz");
            }

            try
            {
                var entity = await _aprendizData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("No se encontró el aprendiz con ID {AprendizId} para reemplazar", dto.Id);
                    throw new EntityNotFoundException("Aprendiz", dto.Id);
                }

                // Modifica sus campos directamente
                entity.PreviousProgram = dto.PreviousProgram;

                return await _aprendizData.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al reemplazar el aprendiz con ID {dto.Id}");
                throw new ExternalServiceException("Base de datos", "Error al reemplazar el aprendiz", ex);
            }
        }

        /// <summary>
        /// Método para delete lógico para activar y desactivar el aprendiz (patch).
        /// </summary>
        public async Task<bool> SetAprendizActiveAsync(AprendizStatusDto dto)
        {
            if (dto == null)
            {
                throw new ValidationException("El dto de estado de aprendiz no puede ser nulo");
            }
            if (dto.Id <= 0)
            {
                _logger.LogWarning("Se intentó activar/desactivar un aprendiz con ID inválido: {AprendizId}", dto.Id);
                throw new ValidationException("id", "El ID del aprendiz debe ser mayor a cero");
            }

            try
            {
                var exists = await _aprendizData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró el aprendiz con ID {AprendizId} para cambiar su estado activo", dto.Id);
                    throw new EntityNotFoundException("Aprendiz", dto.Id);
                }

                return await _aprendizData.SetActiveAsync(dto.Id, dto.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado activo del aprendiz con ID {AprendizId}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el estado activo del aprendiz con ID {dto.Id}", ex);
            }
        }

        /// <summary>
        /// Método para eliminar permanentemente un aprendiz (delete físico).
        /// </summary>
        public async Task<bool> DeleteAprendizAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un aprendiz con ID inválido: {AprendizId}", id);
                throw new ValidationException("id", "El ID del aprendiz debe ser mayor a cero");
            }

            try
            {
                var exists = await _aprendizData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró el aprendiz con ID {AprendizId} para eliminar", id);
                    throw new EntityNotFoundException("Aprendiz", id);
                }

                return await _aprendizData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el aprendiz con ID {AprendizId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el aprendiz con ID {id}", ex);
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
                PreviousProgram = aprendiz.PreviousProgram,
                Active = aprendiz.Active,
                UserId = aprendiz.UserId,
             
            };
        }

        // Método para mapear de AprendizDto a Aprendiz
        private Aprendiz MapToEntity(AprendizDto aprendizDto)
        {
            return new Aprendiz
            {
                Id = aprendizDto.Id,
                PreviousProgram = aprendizDto.PreviousProgram,
                Active = aprendizDto.Active,
                UserId= aprendizDto.UserId,
               
            
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
