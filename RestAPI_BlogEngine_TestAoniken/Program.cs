using RestAPi_BlogEngine_TestAoniken.Repositories;
using RestAPi_BlogEngine_TestAoniken.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using RestAPi_BlogEngine_TestAoniken.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<PostRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


//Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>

{
    options.EnableAnnotations(); // Enable annotations fot SwaggerOperation and SwaggerResponse
});

//Authentication and authorization configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireWriterRole", policy =>
        policy.RequireRole("Writer"));

    options.AddPolicy("RequireEditorRole", policy =>
        policy.RequireRole("Editor"));
});

var app = builder.Build();

//HTTP request pipeline configuration
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 && !context.User.Identity.IsAuthenticated)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();