using CCC.Api.Attributes;
using CCC.Api.Interfaces;
using System.Reflection;

namespace CCC.Api.Services
{
    public class EntityManager
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly Dictionary<Type, Type> _entityToConcreteRepositoryMap = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, Type> _concreteRepositoryToEntityMap = new Dictionary<Type, Type>();

        public EntityManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static void RegisterAllRepositories(IServiceCollection services, params Assembly[] scanAssemblies)
        {
            if (scanAssemblies == null || scanAssemblies.Length == 0)
            {
                scanAssemblies = new[] { Assembly.GetEntryAssembly()! };
            }

            foreach (var assembly in scanAssemblies)
            {
                var repositoryTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract &&
                                t.GetCustomAttribute<RepositoryAttribute>() != null)
                    .ToList();

                foreach (var repoType in repositoryTypes)
                {
                    var attribute = repoType.GetCustomAttribute<RepositoryAttribute>()!;
                    var entityType = attribute.EntityType;
                    var concreteRepositoryType = attribute.RepositoryType;

                    if (repoType != concreteRepositoryType)
                    {
                        Console.WriteLine($"Advertencia: El tipo de repositorio '{repoType.Name}' en el atributo no coincide con el tipo de clase. Se ignorará.");
                        continue;
                    }
                    services.AddScoped(concreteRepositoryType);

                    var genericInterfaceType = typeof(IGenericRepository<>).MakeGenericType(entityType);
                    if (genericInterfaceType.IsAssignableFrom(repoType))
                    {
                        services.AddScoped(genericInterfaceType, concreteRepositoryType);
                        Console.WriteLine($"Registrado: {genericInterfaceType.Name} -> {concreteRepositoryType.Name} para entidad {entityType.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"Advertencia: El repositorio '{repoType.Name}' marcado para la entidad '{entityType.Name}' no implementa '{genericInterfaceType.Name}'.");
                    }

                    _entityToConcreteRepositoryMap[entityType] = concreteRepositoryType;
                    _concreteRepositoryToEntityMap[concreteRepositoryType] = entityType;

                    Console.WriteLine($"Registrado: {concreteRepositoryType.Name} para entidad {entityType.Name}");
                }
            }
        }
        public TRepository GetRepository<TRepository>() where TRepository : class
        {
            var repositoryType = typeof(TRepository);

            if (!_concreteRepositoryToEntityMap.ContainsKey(repositoryType))
            {
                throw new InvalidOperationException($"El tipo de repositorio '{repositoryType.Name}' no está registrado como un repositorio válido a través de RepositoryAttribute.");
            }

            var repository = _serviceProvider.GetService(repositoryType) as TRepository;
            if (repository == null)
            {
                throw new InvalidOperationException($"No se pudo resolver el repositorio '{repositoryType.Name}' desde el ServiceProvider. ¿Está correctamente registrado?");
            }
            return repository;
        }
    }
}