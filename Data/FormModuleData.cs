using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class FormModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormModuleData> _logger;

        public FormModuleData(ApplicationDbContext context, ILogger<FormModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los formModule almacenados en la base de datos
        /// </summary>
        /// <returns> Lista de formModule </returns>
        public async Task<IEnumerable<FormModule>> GetAllAsync()
        {
            return await _context.Set<FormModule>().ToListAsync();
        }

        public async Task<FormModule?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<FormModule>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError( $"Error al obtener Formulario-Módulo con ID {id}");
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo formModule en la base de datos 
        /// </summary>
        /// <param name="formModule">instancia del formModule a crear.</param>
        /// <returns>el formModule creado</returns>
        public async Task<FormModule> CreateAsync(FormModule formModule)
        {
            try
            {
                await _context.Set<FormModule>().AddAsync(formModule);
                await _context.SaveChangesAsync();
                return formModule;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la relación Formulario-Módulo {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Elimina un FormModule permanente en la base de datos 
        /// </summary>
        /// <param name="id">Identificador unico del FormModule a eliminar</param>
        /// <returns>True si la eliminacion fue exitosa, False en caso contrario.</returns>/*

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var formModule = await _context.Set<FormModule>().FindAsync(id);
                if (formModule == null)
                    return false;

                _context.Set<FormModule>().Remove(formModule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la relación Formulario-Módulo {ex.Message}");
                return false;
            }
        }
    }
}
