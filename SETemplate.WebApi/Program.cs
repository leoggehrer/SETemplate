namespace SETemplate.WebApi
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                            .AddNewtonsoftJson();       // Add this to the controllers for PATCH-operation.

            // Add ContextAccessor to the services.
            builder.Services.AddScoped<Contracts.IContextAccessor, Controllers.ContextAccessor>();

            // Added GeGe
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            // Added GeGe

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            // Added GeGe
            app.UseCors();
            // Added GeGe

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
