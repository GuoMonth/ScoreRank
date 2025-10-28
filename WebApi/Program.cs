using ScoreRank.Models;
using ScoreRank.Services;

namespace ScoreRank
{
    /// <summary>
    /// The main program class for the ScoreRank application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            RegisterServices(builder.Services);

            builder.Services.AddControllers();
            // Swagger/OpenAPI at http://localhost:5235/swagger/index.html
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        /// <summary>
        /// Registers application services.
        /// </summary>
        /// <param name="services"></param>
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<Leaderboard>();
            services.AddScoped<LeaderboardService>();
            services.AddScoped<CustomerService>();
        }
    }
}
