
using Balance.BL.Services.BalanceService;
using Balance.DAL.IRepository;
using Balance.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Balance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add DbContext
            builder.Services.AddDbContext<EliteErp2342024Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BalanceCon")));
            #endregion

            #region Add Cors

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            #endregion


            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<IAccountService, AccountService>();


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            //
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowAll");


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
