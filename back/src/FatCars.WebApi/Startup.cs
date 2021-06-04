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

namespace FatCars.WebApi
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

			services.AddControllers();
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
			services.AddSingleton<IConfiguration>(Configuration);
			services.AddSingleton<IDatabaseConfig>(new DatabaseConfig(Configuration));

			services.AddTransient<IUserRepository, UserRepository>();

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
