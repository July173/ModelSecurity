using System;
using Data;
using Entity.DTOs.Enterprise;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las empresas en el sistema.
    /// </summary>
    public class EnterpriseBusiness
    {
        private readonly EnterpriseData _enterpriseData;
        private readonly ILogger<EnterpriseData> _logger;

        public EnterpriseBusiness(EnterpriseData enterpriseData, ILogger<EnterpriseData> logger)
        {
            _enterpriseData = enterpriseData;
            _logger = logger;
        }

        // Método para obtener todas las empresas como DTOs
        public async Task<IEnumerable<EnterpriseDto>> GetAllEnterprisesAsync()
        {
            try
            {
                var enterprises = await _enterpriseData.GetAllAsync();
                
                return MapToDTOList(enterprises);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las empresas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de empresas", ex);
            }
        }

        // Método para obtener una empresa por ID como DTO
        public async Task<EnterpriseDto> GetEnterpriseByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una empresa con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la empresa debe ser mayor que cero");
            }

            try
            {
                var enterprise = await _enterpriseData.GetByIdAsync(id);
                if (enterprise == null)
                {
                    _logger.LogInformation("No se encontró ninguna empresa con ID: {Id}", id);
                    throw new EntityNotFoundException("enterprise", id);
                }

                return MapToDTO(enterprise);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la empresa con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la empresa con ID {id}", ex);
            }
        }

        // Método para crear una empresa desde un DTO
        public async Task<EnterpriseDto> CreateEnterpriseAsync(EnterpriseDto enterpriseDto)
        {
            try
            {
                ValidateEnterprise(enterpriseDto);

                var enterprise = MapToEntity(enterpriseDto);
                enterprise.CreateDate = DateTime.Now;

                var enterpriseCreado = await _enterpriseData.CreateAsync(enterprise);

                return MapToDTO(enterpriseCreado);   
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva empresa: {Name}", enterpriseDto?.NameEnterprise ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la empresa", ex);
            }
        }

        public async Task<bool> SetEnterpriseActiveAsync(EnterpriseStatusDto dto)
        {
            if (dto == null)
                throw new ValidationException("El DTO de estado de empresa no puede ser nulo");

            if (dto.Id <= 0)
            {
                _logger.LogWarning("ID inválido para cambiar estado activo de empresa: {Id}", dto.Id);
                throw new ValidationException("Id", "El ID de la empresa debe ser mayor a 0");
            }

            try
            {
                var entity = await _enterpriseData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("Empresa no encontrada con ID {Id} para cambiar estado", dto.Id);
                    throw new EntityNotFoundException("Enterprise", dto.Id);
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

                return await _enterpriseData.SetActiveAsync(dto.Id, dto.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de empresa con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar estado activo de empresa con ID {dto.Id}", ex);
            }
        }
        public async Task<bool> DeleteEnterpriseAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una empresa con ID inválido: {Id}", id);
                throw new ValidationException("Id", "El ID debe ser mayor a 0");
            }

            try
            {
                var exists = await _enterpriseData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("Empresa no encontrada con ID {Id} para eliminar", id);
                    throw new EntityNotFoundException("Enterprise", id);
                }

                return await _enterpriseData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar empresa con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar empresa con ID {id}", ex);
            }
        }
        public async Task<bool> UpdateParcialEnterpriseAsync(EnterpriseUpdateDTO dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización parcial inválido");
                throw new ValidationException("Id", "Datos inválidos para actualizar empresa");
            }

            try
            {
                var exists = await _enterpriseData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("Empresa no encontrada con ID: {Id}", dto.Id);
                    throw new EntityNotFoundException("Enterprise", dto.Id);
                }

                return await _enterpriseData.PatchAsync(
                    dto.Id,
                    dto.NameEnterprise,
                    dto.PhoneEnterprise,
                    dto.Locate,
                    dto.EmailEnterprise,
                    dto.Observation
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la empresa con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar empresa con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> UpdateEnterpriseAsync(EnterpriseUpdateDTO dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización inválido");
                throw new Utilities.Exceptions.ValidationException("id", "Datos inválidos para actualizar empresa");
            }

            try
            {
                var entity = await _enterpriseData.GetByIdAsync(dto.Id);
                if (entity == null)
                    throw new EntityNotFoundException("Enterprise", dto.Id);

                // Modifica sus campos directamente
                entity.NameEnterprise = dto.NameEnterprise;
                entity.PhoneEnterprise = dto.PhoneEnterprise;
                entity.Locate = dto.Locate;
                entity.EmailEnterprise = dto.EmailEnterprise;
                entity.Observation = dto.Observation;
                entity.UpdateDate = DateTime.Now;

                return await _enterpriseData.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la empresa con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar empresa con ID {dto.Id}", ex);
            }
        }



        // Método para validar el DTO
        private void ValidateEnterprise(EnterpriseDto enterpriseDto)
        {
            if (enterpriseDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Enterprise no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(enterpriseDto.NameEnterprise))
            {
                _logger.LogWarning("Se intentó crear/actualizar una empresa con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la empresa es obligatorio");
            }
        }

        //Metodo para mapear de Enterprise a EnterpriseDto
        private EnterpriseDto MapToDTO(Enterprise enterprise)
        {
            return new EnterpriseDto
            {
                Id = enterprise.Id,
                NameEnterprise = enterprise.NameEnterprise,
                NitEnterprise = enterprise.NitEnterprise,
                Locate = enterprise.Locate,
                Observation = enterprise.Observation,
                PhoneEnterprise = enterprise.PhoneEnterprise,
                EmailEnterprise = enterprise.EmailEnterprise,
                Active = enterprise.Active,

            };
        }
        //Metodo para mapear de EnterpriseDto a Enterprise 
        private Enterprise MapToEntity(EnterpriseDto enterpriseDto)
        {
            return new Enterprise
            {
                Id = enterpriseDto.Id,
                NameEnterprise = enterpriseDto.NameEnterprise,
                NitEnterprise = enterpriseDto.NitEnterprise,
                Locate = enterpriseDto.Locate,
                Observation = enterpriseDto.Observation,
                PhoneEnterprise = enterpriseDto.PhoneEnterprise,
                EmailEnterprise = enterpriseDto.EmailEnterprise,
                Active = enterpriseDto.Active,
            };
        }
        //Metodo para mapear una lista de Enterprise a una lista de EnterpriseDto
        private IEnumerable<EnterpriseDto> MapToDTOList(IEnumerable<Enterprise> enterprises)
        {
            var enterprisesDto = new List<EnterpriseDto>();
            foreach (var enterprise in enterprises)
            {
                enterprisesDto.Add(MapToDTO(enterprise));
            }
            return enterprisesDto;
        }
    }
}
