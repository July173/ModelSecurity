﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repository encargado de la gestión de la entidad Stade en la base de datos.
    /// </summary>
    public class StateData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StateData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public StateData(ApplicationDbContext context, ILogger<StateData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de Stade almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de registros de Stade.</returns>
        public async Task<IEnumerable<State>> GetAllAsync()
        {
            return await _context.Set<State>()
                            .Where(s => s.Active)//Trae solo los activos
                            .ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro de Stade por su ID.
        /// </summary>
        /// <param name="id">Identificador único del registro de Stade.</param>
        /// <returns>El registro de Stade con el ID especificado.</returns>
        public async Task<State?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<State>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener registro de Stade con ID {id}: {ex.Message}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo registro de Stade en la base de datos.
        /// </summary>
        /// <param name="stade">Instancia del registro de Stade a crear.</param>
        /// <returns>El registro de Stade creado.</returns>
        public async Task<State> CreateAsync(State stade)
        {
            try
            {
                await _context.Set<State>().AddAsync(stade);
                await _context.SaveChangesAsync();
                return stade;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el registro de Stade: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro de Stade existente en la base de datos.
        /// </summary>
        /// <param name="stade">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(State stade)
        {
            try
            {
                _context.Set<State>().Update(stade);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el registro de Stade: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un registro de Stade en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del registro de Stade a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var stade = await _context.Set<State>().FindAsync(id);
                if (stade == null)
                    return false;

                _context.Set<State>().Remove(stade);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el registro de Stade: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        /// Elimina logicamente un state (desactiva o activia el State)
        /// </summary>
        /// <param name="id">Id del State</param>
        /// <returns>True si la operacion fue exitosa</returns>
        public async Task<bool> SetActiveAsync(int id, bool active)
        {
            try
            {
                var state = await _context.Set<State>().FindAsync(id);
                if (state == null)
                    return false;

                state.Active = active; //Desactiva el state
                _context.Entry(state).Property(s => s.Active).IsModified = true;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al realizar eliminacion logica del state con ID {id}");
                return false;
            }
        }


        ///<summary>
        ///Modifica datos especificos de state
        ///</summary>
        ///<param name="id">Id del state</param>
        ///<returns> True si la actualizacion es verdadera</returns>
        public async Task<bool> PatchAsync(int id, string NewTypeState, string newDescription)
        {
            try
            {
                var state = await _context.Set<State>().FindAsync(id);
                if (state == null)
                    return false;

                state.TypeState = NewTypeState;
                state.Description = newDescription;

                _context.Entry(state).Property(r => r.TypeState).IsModified = true;
                _context.Entry(state).Property(r => r.Description).IsModified = true;



                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al modificar datos del state con su Id");
                return false;
            }

        }
    }
}
