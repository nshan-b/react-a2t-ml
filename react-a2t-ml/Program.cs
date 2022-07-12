using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using react_a2t_ml.Data;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<react_a2t_mlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("react_a2t_mlContext") ?? throw new InvalidOperationException("Connection string 'react_a2t_mlContext' not found.")));

builder.Services.AddCors(options => {
    options.AddPolicy(name: MyAllowSpecificOrigins, policy => {
        policy.AllowAnyOrigin();
    });
    //options.AddPolicy(name: MyAllowSpecificOrigins, policy => {
    //    policy.WithOrigins("http://localhost:5001").AllowAnyMethod().AllowAnyHeader();
    //});
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
}

app.UseStaticFiles();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.MapControllers();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");



app.MapFallbackToFile("index.html"); ;

app.Run();
