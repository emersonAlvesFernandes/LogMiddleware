using CustomMiddleware.Model;
using CustomMiddleware.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace CustomMiddleware.Ioc
{
    public class NativeBootstrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ILogRepository, LogRepository>();
        }
    }
}
