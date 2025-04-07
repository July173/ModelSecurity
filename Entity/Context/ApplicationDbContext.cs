using Dapper;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using Module = Entity.Model.Module;
using Process = Entity.Model.Process;


namespace Entity.Contexts
{
    /// <summary>
    /// Representa el contexto de la base de datos de la aplicación, proporcionando configuraciones y métodos
    /// para la gestión de entidades y consultas personalizadas con Dapper.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        protected readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del contexto de la base de datos.
        /// </summary>
        /// <param name="options">Opciones de configuración para el contexto de base de datos.</param>
        /// <param name="configuration">Instancia de IConfiguration para acceder a la configuración de la aplicación.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
        {
            _configuration = configuration;
        }

        ///DB SETS
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Verification> Verification { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<UserRol> UserRol { get; set; }
        public DbSet<Sede> Sede { get; set; }
        public DbSet<UserSede> UserSede { get; set; }
        public DbSet<Aprendiz> Aprendiz { get; set; }
        public DbSet<Instructor> Instructor { get; set; }
        public DbSet<Process> Process { get; set; }
        public DbSet<Program> Program { get; set; }
        public DbSet<InstructorProgram> InstructorProgram { get; set; }
        public DbSet<AprendizProgram> AprendizProgram { get; set; }
        public DbSet<AprendizProcessInstructor> AprendizProcessInstructor { get; set; }
        public DbSet<Form> Form { get; set; }
        public DbSet<Module> Module { get; set; }
        public DbSet<FormModule> FormModule { get; set; }
        public DbSet<RolForm> RolForm { get; set; }
        public DbSet<TypeModality> TypeModality { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<RegisterySofia> RegisterySofia { get; set; }
        public DbSet<Regional> Regional { get; set; }
        public DbSet<Center> Center { get; set; }
        public DbSet<Enterprise> Enterprise { get; set; }
        public DbSet<ChangeLog> ChangeLog { get; set; }
        public DbSet<Concept> Concept { get; set; }



        /// <summary>
        /// Configura los modelos de la base de datos aplicando configuraciones desde ensamblados.
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo de base de datos.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //relaciones de las entidades, se colocan las que tienen llaves foraneas 

            //  Relacion de 1 a 1 entre user y person
            modelBuilder.Entity<User>()
                .HasOne(u => u.Person)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.PersonId);

            //Relacion de muchos a muchos tabla pivote de UserRol
            modelBuilder.Entity<UserRol>()
                .HasOne(ur => ur.User)
                .WithMany( u => u.UserRol)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRol>()
                .HasOne(ur => ur.Rol)
                .WithMany(r => r.UserRol)
                .HasForeignKey(ur => ur.RolId);

            //Relacion de muchos a muchos tabla pivote UserSede
            modelBuilder.Entity<UserSede>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSede)
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<UserSede>()
                .HasOne(us => us.Sede)
                .WithMany(s => s.UserSede)
                .HasForeignKey(us => us.SedeId);

            //Relacion de 1 a 1 entre Aprendiz y User
            modelBuilder.Entity<Aprendiz>()
                .HasOne(a => a.User)
                .WithOne(u => u.Aprendiz)
                .HasForeignKey<Aprendiz>(a => a.UserId);

            //Relacion de 1 a 1 entre instructor y user
            modelBuilder.Entity<Instructor>()
                .HasOne(i => i.User)
                .WithOne(u => u.Instructor)
                .HasForeignKey<Instructor>(i => i.UserId);

            //Relacion muchos a muchos tabla pivote AprendizProgram
            modelBuilder.Entity<AprendizProgram>()
                .HasOne(ap => ap.Aprendiz)
                .WithMany(a => a.AprendizProgram)
                .HasForeignKey(ap => ap.AprendizId);

            modelBuilder.Entity<AprendizProgram>()
            .HasOne(ap => ap.Program)
            .WithMany(p => p.AprendizProgram)
            .HasForeignKey(ap => ap.ProgramId);

            //Relacion de muchos a muchos tabla pivote InstructorProgram
            modelBuilder.Entity<InstructorProgram>()
            .HasOne(ip => ip.Instructor)
            .WithMany(i => i.InstructorProgram)
            .HasForeignKey(ip => ip.InstructorId);

            modelBuilder.Entity<InstructorProgram>()
            .HasOne(ip => ip.Program)
            .WithMany(p => p.InstructorProgram)
            .HasForeignKey(ip => ip.ProgramId);

            //Relacion de muchos a muchos de la tabla pivote AprendizProcessInstructor
            modelBuilder.Entity<AprendizProcessInstructor>()
            .HasOne(api => api.TypeModality)
            .WithMany(tm => tm.AprendizProcessInstructor)
            .HasForeignKey(api => api.TypeModalityId);

            modelBuilder.Entity<AprendizProcessInstructor>()
            .HasOne(api => api.RegisterySofia)
            .WithMany(rs => rs.AprendizProcessInstructor)
            .HasForeignKey(api => api.RegisterySofiaId);

            modelBuilder.Entity<AprendizProcessInstructor>()
            .HasOne(api => api.Concept)
            .WithMany(c => c.AprendizProcessInstructor)
            .HasForeignKey(api => api.ConceptId);

            modelBuilder.Entity<AprendizProcessInstructor>()
            .HasOne(api => api.Enterprise)
            .WithMany(e => e.AprendizProcessInstructor)
            .HasForeignKey(api => api.EnterpriseId);

            modelBuilder.Entity<AprendizProcessInstructor>()
            .HasOne(api => api.Process)
            .WithMany(p => p.AprendizProcessInstructor)
            .HasForeignKey(api => api.ProcessId);

            modelBuilder.Entity<AprendizProcessInstructor>()
            .HasOne(api => api.Aprendiz)
            .WithMany(a => a.AprendizProcessInstructor)
            .HasForeignKey(api => api.AprendizId);

            modelBuilder.Entity<AprendizProcessInstructor>()
            .HasOne(api => api.Instructor)
            .WithMany(i => i.AprendizProcessInstructor)
            .HasForeignKey(api => api.InstructorId);

            modelBuilder.Entity<AprendizProcessInstructor>()
            .HasOne(api => api.State)
            .WithMany(s => s.AprendizProcessInstructor)
            .HasForeignKey(api => api.StateId);

            modelBuilder.Entity<AprendizProcessInstructor>()
            .HasOne(api => api.Verification)
            .WithMany(v => v.AprendizProcessInstructor)
            .HasForeignKey(api => api.VerificationId);

            //Relacion de muchos a muchos tabla pivote de FormModule
            modelBuilder.Entity<FormModule>()
            .HasOne(fm => fm.Form)
            .WithMany(f => f.FormModule)
            .HasForeignKey(fm => fm.FormId);

            modelBuilder.Entity<FormModule>()
            .HasOne(fm => fm.Module)
            .WithMany(m => m.FormModule)
            .HasForeignKey(fm => fm.ModuleId);

            //Relacion de muchos a muchos tabla pivote RolForm
            modelBuilder.Entity<RolForm>()
           .HasOne(rf => rf.Rol)
           .WithMany(r => r.RolForm)
           .HasForeignKey(rf => rf.RolId);

            modelBuilder.Entity<RolForm>()
            .HasOne(rf => rf.Form)
            .WithMany(f => f.RolForm)
            .HasForeignKey(rf => rf.FormId);

            //Relacion de 1 a muchos entre Center y Regional
            modelBuilder.Entity<Center>()
           .HasOne(c => c.Regional)
           .WithMany(r => r.Center)
           .HasForeignKey(c => c.RegionalId);

            //Relacion de 1 a muchos entre Sede y center
            modelBuilder.Entity<Sede>()
            .HasOne(s => s.Center)
            .WithMany(c => c.Sede)
            .HasForeignKey(s =>  s.CenterId);


            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
        }

        /// <summary>
        /// Configura opciones adicionales del contexto, como el registro de datos sensibles.
        /// </summary>
        /// <param name="optionsBuilder">Constructor de opciones de configuración del contexto.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            // Otras configuraciones adicionales pueden ir aquí
        }

        /// <summary>
        /// Configura convenciones de tipos de datos, estableciendo la precisión por defecto de los valores decimales.
        /// </summary>
        /// <param name="configurationBuilder">Constructor de configuración de modelos.</param>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos, asegurando la auditoría antes de persistir los datos.
        /// </summary>
        /// <returns>Número de filas afectadas.</returns>
        public override int SaveChanges()
        {
            EnsureAudit();
            return base.SaveChanges();
        }

        /// <summary>
        /// Guarda los cambios en la base de datos de manera asíncrona, asegurando la auditoría antes de la persistencia.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Indica si se deben aceptar todos los cambios en caso de éxito.</param>
        /// <param name="cancellationToken">Token de cancelación para abortar la operación.</param>
        /// <returns>Número de filas afectadas de forma asíncrona.</returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            EnsureAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Ejecuta una consulta SQL utilizando Dapper y devuelve una colección de resultados de tipo genérico.
        /// </summary>
        /// <typeparam name="T">Tipo de los datos de retorno.</typeparam>
        /// <param name="text">Consulta SQL a ejecutar.</param>
        /// <param name="parameters">Parámetros opcionales de la consulta.</param>
        /// <param name="timeout">Tiempo de espera opcional para la consulta.</param>
        /// <param name="type">Tipo opcional de comando SQL.</param>
        /// <returns>Una colección de objetos del tipo especificado.</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.QueryAsync<T>(command.Definition);
        }

        /// <summary>
        /// Ejecuta una consulta SQL utilizando Dapper y devuelve un solo resultado o el valor predeterminado si no hay resultados.
        /// </summary>
        /// <typeparam name="T">Tipo de los datos de retorno.</typeparam>
        /// <param name="text">Consulta SQL a ejecutar.</param>
        /// <param name="parameters">Parámetros opcionales de la consulta.</param>
        /// <param name="timeout">Tiempo de espera opcional para la consulta.</param>
        /// <param name="type">Tipo opcional de comando SQL.</param>
        /// <returns>Un objeto del tipo especificado o su valor predeterminado.</returns>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(command.Definition);
        }

        /// <summary>
        /// Método interno para garantizar la auditoría de los cambios en las entidades.
        /// </summary>
        private void EnsureAudit()
        {
            ChangeTracker.DetectChanges();
        }

        /// <summary>
        /// Estructura para ejecutar comandos SQL con Dapper en Entity Framework Core.
        /// </summary>
        public readonly struct DapperEFCoreCommand : IDisposable
        {
            /// <summary>
            /// Constructor del comando Dapper.
            /// </summary>
            /// <param name="context">Contexto de la base de datos.</param>
            /// <param name="text">Consulta SQL.</param>
            /// <param name="parameters">Parámetros opcionales.</param>
            /// <param name="timeout">Tiempo de espera opcional.</param>
            /// <param name="type">Tipo de comando SQL opcional.</param>
            /// <param name="ct">Token de cancelación.</param>
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

            /// <summary>
            /// Define los parámetros del comando SQL.
            /// </summary>
            public CommandDefinition Definition { get; }

            /// <summary>
            /// Método para liberar los recursos.
            /// </summary>
            public void Dispose()
            {
            }
        }
    }
}