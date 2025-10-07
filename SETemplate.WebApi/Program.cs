using Newtonsoft.Json.Serialization;

namespace SETemplate.WebApi
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                            .AddNewtonsoftJson(options =>
                            {
                                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                                options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
                                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                                options.SerializerSettings.ContractResolver = new DefaultContractResolver()
                                {
                                    NamingStrategy = new CamelCaseNamingStrategy()
                                };
                            });       // Add this to the controllers for PATCH-operation.

            // Add ContextAccessor to the services.
            builder.Services.AddScoped<Contracts.IContextAccessor, Controllers.ContextAccessor>();

            // Added GeGe
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(policy =>
                    {
                        policy.WithOrigins("http://localhost:54091"
                                          ,"http://127.0.0.1:54091")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
                });
            }
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
