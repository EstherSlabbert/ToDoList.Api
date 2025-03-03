﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using ToDoList;
using ToDoList.Data;
using ToDoList.Repositories;
using ToDoList.Services;

namespace AcceptanceTests
{
    public class ToDoListWebApplicationFactory : WebApplicationFactory<IAssemblyMarker>
    {
        private void Configure(IConfigurationBuilder builder)
        {
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(Configure);
            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Remove existing database context registration
                services.RemoveAll<DataContext>();
                services.RemoveAll<DbContextOptions<DataContext>>();

                // Get configuration
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var testDbConnectionString = configuration.GetConnectionString("TestConnection");

                // Register DbContext with the shared database connection
                services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(testDbConnectionString);
                });

                // Ensure the database is created and migrated
                var dbContextFactory = services.BuildServiceProvider();
                using var dbScope = dbContextFactory.CreateScope();
                var dbContext = dbScope.ServiceProvider.GetRequiredService<DataContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.Migrate();
                dbContext.Database.EnsureCreated();

                // Register services as needed
                services.AddScoped<IProjectRepository, ProjectRepository>();
                services.AddScoped<IProjectService, ProjectService>();
                services.AddScoped<IToDoRepository, ToDoRepository>();
                services.AddScoped<IToDoService, ToDoService>();
            });
        }
    }
}
