using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Engines.Csv;
using Hackathon.Premiersoft.API.Engines.DataProcess;
using Hackathon.Premiersoft.API.Engines.DataProcess;
using Hackathon.Premiersoft.API.Engines.DataProcess;
using Hackathon.Premiersoft.API.Engines.Extensions;
using Hackathon.Premiersoft.API.Engines.Factory;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Engines.Xml;
using Hackathon.Premiersoft.API.Messaging.MassTransit;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Repository;
using Hackathon.Premiersoft.API.Repository.Municipios;
using Hackathon.Premiersoft.API.Repository.MunicipiosRepo;
using Hackathon.Premiersoft.API.Services;
using Hackathon.Premiersoft.API.Services.ImportFiles;
using Hackathon.Premiersoft.API.SharedKernel;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Hackathon.Premiersoft.API
{
    internal static class DependencyInjection
    {
        /// <summary>
        /// Método de extensão responsável por registrar os serviços principais da aplicação.
        /// Adiciona suporte a controllers, configura serviços de banco de dados e regras de negócio da aplicação.
        /// </summary>
        /// <param name="services">A coleção de serviços da aplicação.</param>
        /// <param name="configuration">A configuração da aplicação, usada para inicializar dependências.</param>
        /// <returns>A coleção de serviços com as dependências registradas.</returns>
        internal static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDatabaseServices(configuration)
                    .AddApplicationRulesServices();

            services.AddMessageBrokers(configuration, Assembly.GetExecutingAssembly());

            return services;
        }

        private static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PremiersoftHackathonDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("db_connection"),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            });

            services.AddScoped<IPremiersoftHackathonDbContext, PremiersoftHackathonDbContext>();
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            return services;
        }

        private static IServiceCollection AddApplicationRulesServices(this IServiceCollection services)
        {
            services.AddScoped<IFileReaderEngineFactory, FileReaderEngineFactory>();
            services.AddScoped<IFileReaderEngine, ExcelFileReader>();
            services.AddScoped<IFileReaderEngine, CsvFileReaderEngine>();
            //services.AddScoped<IFileReaderEngine, ExcelFileReaderEngine>();
            services.AddScoped<IEntityFactory, EntityFactory>();
            services.AddScoped<IMunicipiosRepository, MunicipiosRepository>();
            services.AddScoped<IPacientesHandler, PacientesHandler>();
            services.AddScoped<IMedicosHandler, MedicosHandler>();
            services.AddScoped<ICsvFileReaderProcess, CsvFileReaderProcess>();  // Register CsvFileReaderProcess

            // Registrar handlers necessários
            services.AddScoped<IMedicosHandler, MedicosHandler>();
            services.AddScoped<IPacientesHandler, PacientesHandler>();
            
            // Registrar repositórios específicos
            services.AddScoped<IMunicipiosRepository, MunicipiosRepository>();
 

            services.AddScoped<IImportFilesService, ImportFilesService>();
            services.AddScoped<IGetDataService, GetDataService>();
            services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

            services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
                            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
                            .AsImplementedInterfaces()
                            .WithScopedLifetime());



            return services;
        }
    }
}
