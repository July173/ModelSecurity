using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;

namespace Entity.Contexts
{
    //representa el contexto de la base de datos de la aplicacion,
    //proporcionando configuraciones y metodos para la gestion de entidades y consultas personalizadas con Dapper.

    public class ApplicationDbContext : DbContext
    {
        //configuracion de la applicacion
        protected readonly IConfiguration _configuration;

        //constructor del contexto de la BD

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        //confogura los modelos de la base de datos aplicando configuraciones desde ensambaldos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        //configura opciones adicionales del contexto, como el registro de datos sensibles
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            //otras configuraciones adiconales pueden ir aqui
        }

        //configura convenciones de tipos de datos, estableciendo la precision estableciendo por defecto los valores decimales.
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }
        //Guarda los cambios en la base de datos, aseguranod la auditoria antes de persistir los datos.
        //retorna numero de filas afectadas
        public override int SaveChanges()
        {
            EnsureAudit();
            return base.SaveChanges();
        }

        //guarda los cambisod e la base de datos de manera asincrona asegurando la auditoria antes de la persistencia 
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSucces, CancellationToken cancellationToken = default)
        {
            EnsureAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSucces, cancellationToken);
        }

        //ejecuta una consulta sql utilizando Dapper y devuelve una coleccion de resultados de tipo genreico

        public async Task<IEnumerable<T>> QueryAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.QueryAsync<T>(command.Definition);
        }

        //ejecuta una sola consulta con SQL utilizando Dapper y devuelve un solo resultado o el valor predeterminado si no hay resultados.
        public async Task<T> QueryFirstOrDefaultAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(command.Definition);
        }

        //metodo interno para garantizar la auditoria de los cambios en las entidades.

        private void EnsureAudit()
        {
            ChangeTracker.DetectChanges();
        }

        //estrucutura para ejecutar comando SQL con Dapper en Entity Framework Core.
        public readonly struct DapperEFCoreCommand : IDisposable
        {
            //constructor del comando Dapper

            public DapperEFCoreCommand(DbContext context, string text, object parameters, int? timeout, CommandType? type, CancellationToken ct)
            {
                var transaction = context.Database.CurrentTransaction?.GetDbTransaction();
                var commandType = type ?? CommandType.Text;
                var commandTimeout = timeout ?? context.Database.GetCommandTimeout() ?? 30;

                Definition = new CommandDefinition(
                    text,
                    parameters,
                    transaction,
                    commandTimeout,
                    commandType,
                    cancellationToken: ct
                    );
            }

            //Define los parametros del comando SQL.
            public CommandDefinition Definition { get; }
            
            public void Dispose()
            {
            }
        }
    
    }
}