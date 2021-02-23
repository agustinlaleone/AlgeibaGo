using IdentityServer4;
using IdentityServer4.Services;
using IdentityServerAspNetIdentity.Migrations;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;

namespace IdentityServerAspNetIdentity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AlgeibaGo")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            services.AddScoped<IProfileService, IdentityProfileService>();
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients(Configuration))
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<IdentityProfileService>();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
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
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            });
            services.AddAuthentication()
                        //.AddOpenIdConnect("oidc", "IDM Algeiba", options =>
                        //{
                        //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        //    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                        //    options.SaveTokens = true;
                        //    options.Authority = Configuration.GetValue<string>("IdentityExternalURL");
                        //    options.ClientId = Configuration.GetValue<string>("ExternalClientId");
                        //    options.Scope.Add("openid");
                        //    options.Scope.Add("profile");
                        //    options.Scope.Add("email");
                        //    options.RequireHttpsMetadata = false;

                        //    options.TokenValidationParameters = new TokenValidationParameters
                        //    {
                        //        NameClaimType = "name",
                        //        RoleClaimType = "role"
                        //    };
                        //})
                         .AddOpenIdConnect("Azure AD", "AZURE AD", options =>
                         {
                             options.ClientId = "f5fa012d-619d-4286-875f-3ce2b08f85c0";
                             options.ClientSecret = "NAb70inuV/AV-qQ/daK_9383YUt74jd]";
                             options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                             options.SignOutScheme = IdentityServerConstants.SignoutScheme; 
                             options.Authority = "https://login.microsoftonline.com/adf33e84-7531-42a9-91c2-8a6881e19cdf";
                             options.ResponseType = OpenIdConnectResponseType.IdToken;
                             options.Scope.Add("profile");
                             options.Scope.Add("email");
                             options.Scope.Add("openid");
                             options.GetClaimsFromUserInfoEndpoint = false;
                             options.TokenValidationParameters = new TokenValidationParameters
                             {
                                 ValidateIssuer = false,
                                 NameClaimType = "email",
                             };
                             options.CallbackPath = "/oidc-signin";
                             options.Prompt = "login"; // login, consent
                         });

        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseCors("MyPolicy");

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }
}