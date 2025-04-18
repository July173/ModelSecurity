﻿using Data;
using Entity.DTOautogestion;
using Entity.DTOautogestion.pivote;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los módulos de formulario en el sistema.
    /// </summary>
    public class FormModuleBusiness
    {
        private readonly FormModuleData _formModuleData;
        private readonly ILogger<FormModuleData> _logger;

        public FormModuleBusiness(FormModuleData formModuleData, ILogger<FormModuleData> logger)
        {
            _formModuleData = formModuleData;
            _logger = logger;
        }

        // Método para obtener todos los módulos de formulario como DTOs
        public async Task<IEnumerable<FormModuleDto>> GetAllFormModulesAsync()
        {
            try
            {
                var formModules = await _formModuleData.GetAllAsync();
             
                return MapToDTOList(formModules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos de formulario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de módulos de formulario", ex);
            }
        }

        // Método para obtener un módulo de formulario por ID como DTO
        public async Task<FormModuleDto> GetFormModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un módulo de formulario con ID inválido: {FormModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del módulo de formulario debe ser mayor que cero");
            }

            try
            {
                var formModule = await _formModuleData.GetByIdAsync(id);
                if (formModule == null)
                {
                    _logger.LogInformation("No se encontró ningún módulo de formulario con ID: {FormModuleId}", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                return  MapToDTO(formModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo de formulario con ID: {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el módulo de formulario con ID {id}", ex);
            }
        }

        // Método para crear un módulo de formulario desde un DTO
        public async Task<FormModuleDto> CreateFormModuleAsync(FormModuleDto formModuleDto)
        {
            try
            {
                ValidateFormModule(formModuleDto);

                var formModule =    MapToEntity(formModuleDto);

                var formModuleCreado = await _formModuleData.CreateAsync(formModule);

                return MapToDTO(formModuleCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo módulo de formulario");
                throw new ExternalServiceException("Base de datos", "Error al crear el módulo de formulario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateFormModule(FormModuleDto formModuleDto)
        {
            if (formModuleDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto módulo de formulario no puede ser nulo");
            }

            if (formModuleDto.FormId <= 0 || formModuleDto.ModuleId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un módulo de formulario con FormId o ModuleId inválidos");
                throw new Utilities.Exceptions.ValidationException("FormId/ModuleId", "El FormId y el ModuleId del módulo de formulario son obligatorios y deben ser mayores que cero");
            }
        }


        //Metodo para mapear de FormModule a  FormModuleDto
        private FormModuleDto MapToDTO(FormModule formModule)
        {
            return new FormModuleDto
            {
                Id = formModule.Id,
                StatusProcedure = formModule.StatusProcedure,
                FormId = formModule.FormId,
                ModuleId = formModule.ModuleId
            };
        }

        //Metodo para mapear de FormModuleDto a FormModule 
        private FormModule MapToEntity(FormModuleDto formModuleDto)
        {
            return new FormModule
            {
                Id = formModuleDto.Id,
                StatusProcedure = formModuleDto.StatusProcedure,
                FormId = formModuleDto.FormId,
                ModuleId = formModuleDto.ModuleId
            };
        }
        //Metodo para mapear una lista de FormModuleDto a una lista de FormModule
        private IEnumerable<FormModuleDto> MapToDTOList(IEnumerable<FormModule> formModules)
        {
            var formModulesDto = new List<FormModuleDto>();
            foreach (var formModule in formModules)
            {
                formModulesDto.Add(MapToDTO(formModule));
            }
            return formModulesDto;
        }
    }
}
