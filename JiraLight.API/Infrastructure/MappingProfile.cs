using AutoMapper;
using JiraLight.API.Domain.Base;
using System.Reflection;

namespace JiraLight.API.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod("Mapping") ??
                                 type.GetInterface("IMapFrom`1")?.GetMethod("Mapping");
                methodInfo?.Invoke(instance, new object[] { this });
            }
        }

        public static void Configure(IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Сканируем сборку API на Domain модели (наследники BaseEntity)
                var domainTypes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.BaseType == typeof(BaseEntity));

                foreach (var domainType in domainTypes)
                {
                    // По конвенции ищем DTO с именем DomainName + "Dto" в пространстве имен JiraLight.Dto
                    var dtoName = $"{domainType.Name}Dto";
                    var dtoType = Type.GetType($"JiraLight.Dto.{dtoName}");

                    if (dtoType != null)
                    {
                        // Создаем маппинг Domain -> DTO и обратно
                        cfg.CreateMap(domainType, dtoType).ReverseMap();
                    }
                }
            });

            services.AddSingleton(config.CreateMapper());
        }
    }

    public interface IMapFrom<T>
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}
