using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFABB.SelfRescue.Infrastructure {
    public static class CorsServiceExtensions {
        private static readonly string CORSPOLICYNAME = "GeneralCors";
        public static IServiceCollection ConfigureCors(this IServiceCollection services) {
            services.AddCors(options => {
                options.AddPolicy(CORSPOLICYNAME, policy => {
                    policy.WithOrigins(
                        "http://localhost:3000",
                        "localhost:3000",
                        "http://localhost:4200",
                        "localhost:4200"
                        ).AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            return services;
        }

        public static IApplicationBuilder UseGeneralCorsPolicy(this IApplicationBuilder app) {
            app.UseCors(CORSPOLICYNAME);
            return app;
        }
    }
}
