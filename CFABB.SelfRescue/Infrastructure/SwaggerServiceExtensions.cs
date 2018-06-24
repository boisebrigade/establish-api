using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CFABB.SelfRescue.Infrastructure {
    public static class SwaggerServiceExtensions {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services) {
            services.AddSwaggerGen(g => {
                g.OperationFilter<ExamplesOperationFilter>();
                g.SwaggerDoc("v1", new Info {
                    Title = "Self Rescue Manual API",
                    Version = "v1",
                    Description = "",
                    Contact = new Contact() {
                        Email = "boisebrigade@gmail.com",
                        Name = "Us"
                    }
                });
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "CFABB.SelfRescue.xml");
                g.IncludeXmlComments(xmlPath);
            });
            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app) {
            app.UseSwagger();
            app.UseSwaggerUI(s => {
                s.DefaultModelsExpandDepth(0);
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Self Rescue Manual API");
            });
            return app;
        }
    }
}
