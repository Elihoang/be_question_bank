using BEQuestionBank.Application.Services;
using BEQuestionBank.Core.Repositories;
using BEQuestionBank.Core.Services;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Infrastructure.Repositories;
using File = BEQuestionBank.Domain.Models.File;

namespace BEQuestionBank.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Generic
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IService<Khoa>, KhoaService>();
        services.AddScoped<IService<CauHoi>, CauHoiService>();
        services.AddScoped<IService<Phan>, PhanService>();
        services.AddScoped<IService<MonHoc>, MonHocService>();
        services.AddScoped<IService<CauTraLoi>, CauTraLoiService>();
        services.AddScoped<IService<File>, FileService>();
        services.AddScoped<IService<DeThi>, DeThiService>();
        services.AddScoped<IService<YeuCauRutTrich>, YeuCauRutTrichService>();
        

        // Khoa
        services.AddScoped<IKhoaRepository, KhoaRepository>();
        services.AddScoped<IKhoaService, KhoaService>();

        // MonHoc
        services.AddScoped<IMonHocRepository, MonHocRepository>();
        services.AddScoped<IMonHocService, MonHocService>();
        
        // CauHoi
        services.AddScoped<ICauHoiRepository, CauHoiRepository>();
        services.AddScoped<ICauHoiService, CauHoiService>();

        // CauTraLoi
        services.AddScoped<ICauTraLoiRepository, CauTraLoiRepository>();
        services.AddScoped<ICauTraLoiService, CauTraLoiService>();

        //Phan
        services.AddScoped<IPhanRepository, PhanRepository>();
        services.AddScoped<IPhanService, PhanService>();
        // File
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<WordImportService>();
        
        // Nguoi Dung
        services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
        services.AddScoped<JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<WordImportService>();
        
        //De Thi
        services.AddScoped<IDeThiRepository, DeThiRepository>();
        services.AddScoped<IDeThiService, DeThiService>();
        
        // User
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        
        //yeu cau rut trich
        services.AddScoped<IYeuCauRutTrichRepository, YeuCauRutTrichRepository>();
        services.AddScoped<IYeuCauRutTrichService, YeuCauRutTrichService>();
        return services;
    }
}