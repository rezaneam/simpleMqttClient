
using ControlTool.Models;
using ControlTool.Services;
using ControlTool.SignalR;

namespace ControlTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Allow CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    corsPolicyBuilder =>
                    {
                        corsPolicyBuilder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .SetIsOriginAllowed(_ => true);
                    });
            });

            builder.Services.AddSignalR();
            builder.Services.AddSingleton<MqttService>();
            builder.Services.AddSingleton<SensorService>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowAll");
            app.MapControllers();
            app.MapHub<BaseHub>("/hub");
            app.Services.GetRequiredService<MqttService>().Connect("broker.hivemq.com", 8883, "CodeTestMqttTopic");
            app.Services.GetRequiredService<SensorService>();

            app.Run();
        }
    }
}
