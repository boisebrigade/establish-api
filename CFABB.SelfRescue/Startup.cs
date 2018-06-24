using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFABB.SelfRescue.Data;
using CFABB.SelfRescue.Data.Extensions;
using CFABB.SelfRescue.Infrastructure;
using CFABB.SelfRescue.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CFABB.SelfRescue {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            ConfigureDBContext(services);
            services.ConfigureCors();
            ConfigureIdentity(services);
            ConfigureMVC(services);
            services.AddSwaggerDocumentation();
            ConfigureInjectables(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseGeneralCorsPolicy();
            app.UseSwaggerDocumentation();
            app.UseAuthentication();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMvc();

            RunMigrations(app);
        }

        private void ConfigureDBContext(IServiceCollection services) {
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
        }

        private void ConfigureIdentity(IServiceCollection services) {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => {
                options.Events.OnRedirectToAccessDenied = context => {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToLogin = context => {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
        }

        private void ConfigureMVC(IServiceCollection services) {
            services.AddMvc(options => {
                options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            });
        }

        private void ConfigureInjectables(IServiceCollection services) {

        }

        private void RunMigrations(IApplicationBuilder app) {
            using (IServiceScope serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope()) {
                using (var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>()) {
                    if (!dbContext.AllMigrationsApplied()) {
                        dbContext.Database.Migrate();
                    }
                }
            }
        }
    }
}
