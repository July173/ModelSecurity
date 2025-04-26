using System;
using Data;
using Entity.DTOs.Center;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los centros en el sistema.
    /// </summary>
    public class CenterBusiness
    {
        private readonly CenterData _centerData;
        private readonly ILogger<CenterData> _logger;

        public CenterBusiness(CenterData centerData, ILogger<CenterData> logger)
        {
            _centerData = centerData;
            _logger = logger;
        }

        // Método para obtener todos los centros como DTOs
        public async Task<IEnumerable<CenterDto>> GetAllCentersAsync()
        {
            try
            {
                var centers = await _centerData.GetAllAsync();
               
                return MapToDTOList(centers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los centros");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de centros", ex);
            }
        }

        // Método para obtener un centro por ID como DTO
        public async Task<CenterDto> GetCenterByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un centro con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del centro debe ser mayor que cero");
            }

            try
            {
                var center = await _centerData.GetByIdAsync(id);
                if (center == null)
                {
                    _logger.LogInformation("No se encontró ningún centro con ID: {Id}", id);
                    throw new EntityNotFoundException("center", id);
                }

                return MapToDTO(center);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el centro con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el centro con ID {id}", ex);
            }
        }

        // Método para crear un centro desde un DTO
        public async Task<CenterDto> CreateCenterAsync(CenterDto centerDto)
        {
            try
            {
                ValidateCenter(centerDto);

                var center = MapToEntity(centerDto);
                center.CreateDate = DateTime.Now;


                var centerCreado = await _centerData.CreateAsync(center);

                return MapToDTO(centerCreado);   
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo centro: {Name}", centerDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el centro", ex);
            }
        }

        public async Task<bool> SetCenterActiveAsync(CenterStatusDto dto)
        {
            if (dto == null)
                throw new ValidationException("El DTO de estado de centro no puede ser nulo");

            if (dto.Id <= 0)
            {
                _logger.LogWarning("ID inválido para cambiar estado activo de centro: {Id}", dto.Id);
                throw new ValidationException("Id", "El ID del centro debe ser mayor a 0");
            }

            try
            {
                var entity = await _centerData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("Centro no encontrado con ID {Id} para cambiar estado", dto.Id);
                    throw new EntityNotFoundException("Center", dto.Id);
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

                return await _centerData.SetActiveAsync(dto.Id, dto.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de centro con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar estado activo de centro con ID {dto.Id}", ex);
            }
        }


        public async Task<bool> DeleteCenterAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un centro con ID inválido: {Id}", id);
                throw new ValidationException("Id", "El ID debe ser mayor a 0");
            }

            try
            {
                var exists = await _centerData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("Centro no encontrado con ID {Id} para eliminar", id);
                    throw new EntityNotFoundException("Center", id);
                }

                return await _centerData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar centro con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar centro con ID {id}", ex);
            }
        }

        public async Task<bool> UpdateParcialCenterAsync(CenterUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización parcial inválido");
                throw new ValidationException("Id", "Datos inválidos para actualizar centro");
            }

            try
            {
                var exists = await _centerData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("Centro no encontrado con ID: {Id}", dto.Id);
                    throw new EntityNotFoundException("Center", dto.Id);
                }

                return await _centerData.PatchAsync(dto.Id, dto.Name, dto.Address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el centro con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar centro con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> UpdateCenterAsync(CenterUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización inválido");
                throw new Utilities.Exceptions.ValidationException("id", "Datos inválidos para actualizar centro");
            }

            try
            {
                var entity = await _centerData.GetByIdAsync(dto.Id);
                if (entity == null)
                    throw new EntityNotFoundException("Center", dto.Id);

                // Modifica sus campos directamente
                entity.Name = dto.Name;
                entity.Address = dto.Address;
                entity.UpdateDate = DateTime.Now;

                return await _centerData.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el centro con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar centro con ID {dto.Id}", ex);
            }
        }


        // Método para validar el DTO
        private void ValidateCenter(CenterDto centerDto)
        {
            if (centerDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Center no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(centerDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un centro con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del centro es obligatorio");
            }
        }

        //Metodo para mapear de Center a CenterDto
        private CenterDto MapToDTO(Center center)
        {
            return new CenterDto
            {
                Id = center.Id,
                Name = center.Name,
                CodeCenter = center.CodeCenter,
                Active = center.Active,
                RegionalId = center.RegionalId,
                Address = center.Address,
               

            };
        }
        //Metodo para mapear de CenterDto a Center 
        private Center MapToEntity(CenterDto centerDto)
        {
            return new Center
            {
                Id = centerDto.Id,
                Name = centerDto.Name,
                CodeCenter = centerDto.CodeCenter,
                Active = centerDto.Active,
                RegionalId = centerDto.RegionalId,
                Address = centerDto.Address,


            };
        }
        //Metodo para mapear una lista de Center a una lista de CenterDto
        private IEnumerable<CenterDto> MapToDTOList(IEnumerable<Center> centers)
        {
            var centersDto = new List<CenterDto>();
            foreach (var center in centers)
            {
                centersDto.Add(MapToDTO(center));
            }
            return centersDto;
        }
    }
}
