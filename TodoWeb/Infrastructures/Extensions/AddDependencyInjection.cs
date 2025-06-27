using TodoWeb.Application.Middleware;
using TodoWeb.Application.Services.CacheServices;
using TodoWeb.Application.Services;
using TodoWeb.Application.Services.MiddlwareServices;

namespace TodoWeb.Infrastructures.Extensions
{
    public static class AddDependencyInjection
    {
        public static void AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<IApplicationDbContext, ApplicationDbContext>();
            serviceCollection.AddScoped<IToDoService, ToDoService>();
            serviceCollection.AddTransient<IGuidGenerator, GuidGenerator>();
            serviceCollection.AddSingleton<ISingletronGenerator, SingletronGenerator>();
            serviceCollection.AddScoped<ISchoolService, SchoolService>();
            serviceCollection.AddScoped<IStudentService, StudentService>();
            serviceCollection.AddScoped<ICourseService, CourseService>();
            serviceCollection.AddScoped<IGradeService, GradeService>();
            serviceCollection.AddSingleton<LogMiddleware>();
            serviceCollection.AddSingleton<ICacheService, CacheService>();
            serviceCollection.AddScoped<IUserServices, UserServices>();
            
            // add singleton
            serviceCollection.AddSingleton<IGoogleCredentialService, GoogleCredentialService>();

            // Có một lỗi h mới biết đã làm middlware thì phải sử dụng AddSingleton
            serviceCollection.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();
        }
    }
}
