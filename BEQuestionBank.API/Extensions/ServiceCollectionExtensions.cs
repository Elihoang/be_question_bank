using BEQuestionBank.Core.Repositories;
using BEQuestionBank.Core.Services;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;

namespace BEQuestionBank.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Generic
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(IService<>), typeof(GenericService<>));

        // Khoa
        services.AddScoped<IKhoaRepository, KhoaRepository>();
        services.AddScoped<IKhoaService, KhoaService>();

        // Nếu dùng KhoaService<Khoa>: (không khuyến khích unless cần)
        // services.AddScoped<IKhoaService<Khoa>, KhoaService<Khoa>>();

        return services;
    }
}