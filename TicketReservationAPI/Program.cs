
using Microsoft.EntityFrameworkCore;
using TicketReservationAPI.Data;
using TicketReservationAPI.Repository;
using TicketReservationAPI.Repository.IRepository;

namespace TicketReservationAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


            builder.Services.AddDbContext<TicketDbContext>(option =>
            option.UseSqlServer(builder.Configuration.GetConnectionString("Ticketconnection")));
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddScoped<IBookingRepository,BookingRepository>();
            builder.Services.AddScoped<IEventRepository,EventRepository>();

            builder.Services.AddCors(options=>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAnyOrigin");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
