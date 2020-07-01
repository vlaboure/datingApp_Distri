using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DattingApp.api.Data;
using DattingApp.api.helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DattingApp.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
 
        public IConfiguration Configuration { get; }
 
        public void ConfigureDeveloppementServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x=>
            {
                x.UseLazyLoadingProxies();
                x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
                ConfigureServices(services);
            });

        }
        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x =>
            {
                x.UseLazyLoadingProxies();
                x.UseSqlServer(Configuration.
                GetConnectionString("DefaultConnection"));//, ServiceLifetime.Scoped
            });
 
            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // les services sont configurés dans void Configure
<<<<<<< HEAD
=======
            services.AddDbContext<DataContext>(X=>
            {
                X.UseLazyLoadingProxies();
                X.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
>>>>>>> 56034acd22d0259637fc99b17463fae12366e83b
            services.AddControllers().AddNewtonsoftJson(opt=>{
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddCors();//ajout service pour le passage d'un port à un autre      
            //le service est configuré dans Data      
            services.AddAutoMapper(typeof(DattingRepository).Assembly);//ou typeof(Startup)
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddScoped<IAuthRepository,AuthRepository>();
            services.AddScoped<IDattingRepository,DattingRepository>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options=>{
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                                    //idem à AuthController Login
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration
                    .GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddScoped<LogUserActivity>();
        }
 
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }else
            {
                // l'utilisation de l'expression lambda permet l'accès à l'erreur avant la réponse du serveur
                app.UseExceptionHandler(builder =>{
                    builder.Run(async context =>{
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        // récup de la requète de builder.features<IFeatureCollection>
                                //IExceptionHandlerFeature --> use Microsoft.AspNetCore.Diagnostics
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                            context.Response.AddApplicationHelper(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);                       
                    });
                }); 
            }

           // app.UseHttpsRedirection();---> pas de https
 
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //utilisation de la règle de routage AddCors et de faire des requêtes inter-domaines
            app.UseCors(x=>x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
 
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // ajout deu controller Fallback MapFallbackController--> mappe le controller
                endpoints.MapFallbackToController("Index","Fallback");
            });
        }
    }
}