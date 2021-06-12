using System.Text;
using FatCars.Repository;
using FatCars.Repository.Dapper;
using FatCars.Repository.Dapper.Interfaces;
using FatCars.Repository.Dapper.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft;
using System;
using Microsoft.AspNetCore.Identity;
using FatCars.Domain;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace FatCars.Application
{
	public class Startup
	{
		private readonly string _allowEverything = "AllowEverything";
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration["Database:ConnectionString"])
			services.AddDbContext<DataContext>(x => x.UseSqlServer(Configuration["Database:ConnectionString"])
			);

			//Configuração das obrigatoriedades nas criações das senhas
			IdentityBuilder builder = services.AddIdentityCore<Users>(
				options => {
					options.Password.RequireDigit = false;
					options.Password.RequireLowercase = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
					options.Password.RequiredLength = 8;
				});

			builder = new IdentityBuilder(builder.UserType, typeof(Role),
				builder.Services);

			builder.AddUserStore<DataContext>();
			builder.AddRoleValidator<RoleValidator<Role>>();
			builder.AddRoleManager<RoleManager<Role>>();
			builder.AddSignInManager<SignInManager<Users>>();

			//Configuração para requerir que o usuario esteja autenticado
			services.AddControllers(options => {
				var policy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.Build();
					options.Filters.Add(new AuthorizeFilter(policy));
					})
					.AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling=
						Newtonsoft.Json.ReferenceLoopHandling.Ignore
					);
			//AddNewtonsoftJson, tira as redundâncias dos json's

			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			services.AddSingleton<IConfiguration>(Configuration);
			services.AddSingleton<IDatabaseConfig>(new DatabaseConfig(Configuration));

			services.AddTransient<IUserRepository, UserRepository>();

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "FatCars.WebApi", Version = "v1" });

				//add JWT Authentication
				var securityScheme = new OpenApiSecurityScheme
				{
					Name = "JWT Authentication",
					Description = "Enter JWT Bearer token **_only_**",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.Http,
					Scheme = "bearer",
					BearerFormat = "JWT",
					Reference = new OpenApiReference
					{
						Id = JwtBearerDefaults.AuthenticationScheme,
						Type = ReferenceType.SecurityScheme
					}
				};
				c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{securityScheme, new string[] { }}
				});

			});
			services.AddCors(opt => opt.AddPolicy(_allowEverything, builder => builder
							 .AllowAnyHeader()
							 .AllowAnyMethod()
							 .AllowAnyOrigin()));

			//config auth
			services.AddAuthentication(auth =>
			{
				auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidAudience = Configuration["JWT:Audience"],
					ValidIssuer = Configuration["JWT:Issuer"],
					RequireExpirationTime = true,
					IssuerSigningKey =
						new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
					ValidateIssuerSigningKey = true,

				};
			});
			

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(
					c =>
					{
						c.SwaggerEndpoint("/swagger/v1/swagger.json", "FatCars.WebApi v1");
						c.DisplayRequestDuration();
					});
			}

			//CREATE DATABASE
			using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
			{
				var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
				context.Database.Migrate();
			}


			app.UseCors(_allowEverything);

			app.UseHttpsRedirection();

			app.UseRouting();
			app.UseAuthentication();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
