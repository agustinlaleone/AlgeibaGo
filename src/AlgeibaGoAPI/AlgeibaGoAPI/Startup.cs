using System.Linq;
using AlgeibaGoAPI.Data;
using AlgeibaGoAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace AlgeibaGoAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
           
            services.AddApiVersioning(c =>
            {
                c.DefaultApiVersion = new ApiVersion(1, 0);
                c.AssumeDefaultVersionWhenUnspecified = true;
                c.ApiVersionReader = new MediaTypeApiVersionReader();
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new Info { Title = "AlgeibaGo API 1.0", Version = "v1.0" });
                c.SwaggerDoc("v2.0", new Info
                {
                    Version = "v2.0",
                    Title = "AlgeibaGo API v2.0"
                });



                c.DocInclusionPredicate((docName, apiDesc) =>
                {



                    var actionVersions = apiDesc.ActionDescriptor.GetProperty<ApiVersionModel>();



                    return actionVersions.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);



                });
            });
            services.AddRouting();

            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();
            services.AddAuthentication("Bearer")
                         .AddJwtBearer("Bearer", options =>
                         {
                             options.Authority = Configuration.GetSection("IdentityURL").Value;
                             options.RequireHttpsMetadata = false;
                             options.Audience = "AlgeibaGoManagerDBApi";
                         });

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("MyPolicy", policy =>
                {
                    policy.WithOrigins("*")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AlgeibaGo")));
            services.AddDbContext<AlgeibaGoContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("AlgeibaGo"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");
            app.UseMvc();
            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "AlgeibaGo API v1.0");
                c.SwaggerEndpoint("/swagger/v2.0/swagger.json", "AlgeibaGo API v2.0");
            });
        }
    }
}