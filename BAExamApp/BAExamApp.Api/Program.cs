using BAExamApp.Business.Extensions;
using BAExamApp.DataAccess.EFCore.Extensions;
using BAExamApp.DataAccess.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddDefaultPolicy(builder => builder
                                                    .AllowAnyHeader()
                                                    .AllowAnyMethod()
                                                    .AllowAnyOrigin()));

builder.Services
    .AddDataAccessServices(builder.Configuration)
    .AddEFCoreServices(builder.Configuration)
    .AddBusinessServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseCors();

app.UseRouting();

app.MapControllers();

app.Run();