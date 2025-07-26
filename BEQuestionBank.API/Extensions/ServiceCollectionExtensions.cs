
using BEQuestionBank.Core.Repositories;
using BEQuestionBank.Domain.Interfaces.Repo;

namespace BEQuestionBank.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Generic
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        // Khoa
        services.AddScoped<IKhoaRepository, KhoaRepository>();

        // Nếu dùng KhoaService<Khoa>: (không khuyến khích unless cần)
        // services.AddScoped<IKhoaService<Khoa>, KhoaService<Khoa>>();

        return services;
    }
}