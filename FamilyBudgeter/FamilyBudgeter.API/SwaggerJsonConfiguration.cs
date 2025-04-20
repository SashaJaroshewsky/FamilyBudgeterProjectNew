using Microsoft.OpenApi.Models;

namespace FamilyBudgeter.API
{
    public static class SwaggerJsonConfiguration
    {
        public static IServiceCollection AddSwaggerJsonConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FamilyBudgeter API",
                    Version = "v1",
                    Description = "API для платформи FamilyBudgeter для спільного планування сімейних фінансів",
                });

                // Налаштування JWT
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerJsonConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger(options =>
            {
                // Забезпечує сумісність з OpenAPI 3.0
                options.RouteTemplate = "swagger/{documentName}/swagger.json";
                options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    // Явно встановлюємо версію OpenAPI
                    swaggerDoc.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
                    };
                });
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "FamilyBudgeter API v1");
                options.RoutePrefix = "swagger";
            });

            return app;
        }
    }
}
