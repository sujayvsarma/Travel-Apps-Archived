using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
        options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
            {
                Title = "Sujay Sarma's Airport Finder",
                Description = "REST API to interact with OurAirports dataset: https://ourairports.com/data/",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                {
                    Name = "Sujay Sarma",
                    Email = "sujay@sarma.in",
                    Url = new System.Uri("https://twitter.com/sujaysarma")
                },
                License = new Microsoft.OpenApi.Models.OpenApiLicense()
                {
                    Name = "The Unlicense", 
                    Url = new System.Uri("https://github.com/davidmegginson/ourairports-data/blob/main/LICENSE")
                },
                Version = "6.0.0"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        }
    );
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

var app = builder.Build();

app.UseExceptionHandler(
        (error) =>
        {
            error.Run(
                    context =>
                    {
                        IExceptionHandlerFeature? exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        Exception? exception = exceptionFeature?.Error;
                        if (exception == default)
                        {
                            return Task.CompletedTask;
                        }

                        if (exception is AggregateException ae)
                        {
                            exception = ae.InnerException!;
                        }

                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/plain";
                        context.Response.WriteAsync(exception.Message);

                        return Task.FromException(exception);
                    }
                );  
        }
    );

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(
        (options) =>
        {
            options.DisplayRequestDuration();
        }
    );

app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();

app.MapControllers();
app.MapRazorPages();

app.Run();
