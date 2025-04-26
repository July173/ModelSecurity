using Data;
using Entity.DTOs.Sede;
using Entity.DTOs.Verification;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las sedes en el sistema.
    /// </summary>
    public class SedeBusiness
    {
        private readonly SedeData _sedeData;
        private readonly ILogger<SedeData> _logger;

        public SedeBusiness(SedeData sedeData, ILogger<SedeData> logger)
        {
            _sedeData = sedeData;
            _logger = logger;
        }

        // Método para obtener todas las sedes como DTOs get
        public async Task<IEnumerable<SedeDto>> GetAllSedesAsync()
        {
            try
            {
                var sedes = await _sedeData.GetAllAsync();
                return MapToDTOList(sedes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las sedes");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de sedes", ex);
            }
        }

        // Método para obtener una sede por ID como DTO getById
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

                return MapToDTO(sede);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la sede con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la sede con ID {id}", ex);
            }
        }

        // Método para crear una sede desde un DTO Post
        public async Task<SedeDto> CreateSedeAsync(SedeDto sedeDto)
        {
            try
            {
                ValidateSede(sedeDto);

                var sede = MapToEntity(sedeDto);
                sede.CreateDate = DateTime.Now;

                var sedeCreada = await _sedeData.CreateAsync(sede);

                return MapToDTO(sedeCreada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva sede: {Name}", sedeDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la sede", ex);
            }
        }

        // Método para actualizar una sede desde un DTO put
        public async Task<bool> UpdateSedeAsync(SedeUpdateDto sedeUpdateDto)
        {
            try
            {
                var sede = await _sedeData.GetByIdAsync(sedeUpdateDto.Id);
                if (sede == null)
                {
                    _logger.LogWarning("No se encontró la sede con ID: {Id} para actualizar", sedeUpdateDto.Id);
                    throw new EntityNotFoundException("sede", sedeUpdateDto.Id);
                }

                // Actualizamos la sede
                sede.Name = sedeUpdateDto.Name;
                sede.Address = sedeUpdateDto.Address;
                sede.PhoneSede = sedeUpdateDto.PhoneSede;
                sede.EmailContact = sedeUpdateDto.EmailContact;

                return await _sedeData.UpdateAsync(sede);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la sede con ID: {Id}", sedeUpdateDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar la sede", ex);
            }
        }

        // Método para eliminar una sede de manera permanente delete
        public async Task DeleteSedeAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una sede con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la sede debe ser mayor que cero");
            }

            try
            {
                // Verificar si la sede existe antes de intentar eliminarla
                var sede = await _sedeData.GetByIdAsync(id);
                if (sede == null)
                {
                    _logger.LogInformation("No se encontró ninguna sede con ID: {Id}", id);
                    throw new EntityNotFoundException("sede", id);
                }

                // Llamar al método en SedeData para realizar la eliminación permanente
                await _sedeData.DeleteAsync(id);

                _logger.LogInformation("Sede con ID: {Id} eliminada correctamente.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la sede con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la sede con ID {id}", ex);
            }
        }


        public async Task<bool> UpdateParcialSedeAsync(SedeUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización parcial inválido");
                throw new ValidationException("Id", "Datos inválidos para actualizar sede");
            }

            try
            {
                var exists = await _sedeData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("Sede no encontrada con ID: {Id}", dto.Id);
                    throw new EntityNotFoundException("Sede", dto.Id);
                }

                return await _sedeData.PatchSedeAsync(dto.Id, dto.Name, dto.EmailContact, dto.Address, dto.PhoneSede);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la sede con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar sede con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> SetSedeActiveAsync(SedeStatusDto dto)
        {
            if (dto == null)
                throw new ValidationException("El DTO de estado de sede no puede ser nulo");

            if (dto.Id <= 0)
            {
                _logger.LogWarning("ID inválido para cambiar estado activo de sede: {Id}", dto.Id);
                throw new ValidationException("Id", "El ID de la sede debe ser mayor a 0");
            }

            try
            {
                var entity = await _sedeData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("Sede no encontrada con ID {Id} para cambiar estado", dto.Id);
                    throw new EntityNotFoundException("Sede", dto.Id);
                }

                // Establecer DeleteDate si se va a desactivar (borrado lógico)
                if (!dto.Active)
                {
                    entity.DeleteDate = DateTime.Now;
                }
                else
                {
                    entity.DeleteDate = null; // Reactivación: eliminamos la marca de eliminación
                }

                return await _sedeData.SetActiveAsync(dto.Id, dto.Active); // Usamos UpdateAsync porque modificamos el objeto
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de sede con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar estado activo de sede con ID {dto.Id}", ex);
            }
        }



        // Método para validar el DTO
        private void ValidateSede(SedeDto sedeDto)
        {
            if (sedeDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Sede no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(sedeDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una sede con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la sede es obligatorio");
            }
        }

        
        // Método para mapear de Sede a SedeDto
        private SedeDto MapToDTO(Sede sede)
        {
            return new SedeDto
            {
                Id = sede.Id,
                Name = sede.Name,
                CodeSede = sede.CodeSede,
                Address = sede.Address,
                PhoneSede = sede.PhoneSede,
                EmailContact = sede.EmailContact,
                CenterId = sede.CenterId,
                Active = sede.Active, // si existe la entidad
            };
        }

        // Método para mapear de SedeDto a Sede
        private Sede MapToEntity(SedeDto sedeDto)
        {
            return new Sede
            {
                Id = sedeDto.Id,
                Name = sedeDto.Name,
                CodeSede = sedeDto.CodeSede,
                Address = sedeDto.Address,
                PhoneSede = sedeDto.PhoneSede,
                EmailContact = sedeDto.EmailContact,
                CenterId = sedeDto.CenterId,
                Active = sedeDto.Active, // si existe la entidad 
            };
        }

        // Método para mapear una lista de Sede a una lista de SedeDto
        private IEnumerable<SedeDto> MapToDTOList(IEnumerable<Sede> sedes)
        {
            var sedesDto = new List<SedeDto>();
            foreach (var sede in sedes)
            {
                sedesDto.Add(MapToDTO(sede));
            }
            return sedesDto;
        }
    }
}
