using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using Serilog;
using System.Data;
using TodoWeb.Application.ActionFillter;
using TodoWeb.Application.BackGroundJobs;
using TodoWeb.Application.MapperProfiles;
using TodoWeb.Application.Middleware;
using TodoWeb.Application.Services;
using TodoWeb.Application.Services.CacheServices;
using TodoWeb.Domain.AppsetingsConfigurations;
using TodoWeb.Infrastructures;
using TodoWeb.Infrastructures.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(option =>
{
    option.Filters.Add<TestFillter>();
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDpApp",
        Version = "v1",
        Description = "ToDoApp API"
    });
    var securitySchema = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In =  ParameterLocation.Header,
        Description = "Please enter JWT token",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
     };

    option.AddSecurityDefinition("Bearer", securitySchema);

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securitySchema, new string[] {} }
    });
});
// add dependency
builder.Services.AddServices();
builder.Services.AddMemoryCache();

// session can cai nay
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly = true; // chỉ cho phép truy cập cookie từ server
    // nếu mà ko có cái này thì client có thể truy cập cookie
    options.Cookie.IsEssential = true; // tự động lưu cookie trên browser của client
});
// config file 
var fileInformation = builder.Configuration.GetSection("FileInformation");
builder.Services.Configure<FileInformation>(fileInformation);


// NOTE fix
string? connectString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(
    option => option.UseSqlServer(connectString));


var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // không cho phép gia hạn token
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"]

        };
    });

// config cookie
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//    {
//        options.Events = new CookieAuthenticationEvents // 
//        {
//            OnRedirectToLogin = context =>
//            {
//                context.Response.StatusCode = 401; // Unauthorized
//                return Task.CompletedTask;
//            },
//            OnRedirectToAccessDenied = context =>
//            {
//                context.Response.StatusCode = 403; // Forbidden
//                return Task.CompletedTask;
//            }
//        };
//        options.Cookie.HttpOnly = true;
//        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // chỉ cho phép truy cập cookie từ https
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // thời gian hết hạn của cookie
//        options.SlidingExpiration = true; // tự động gia hạn cookie khi người dùng truy cập
//    });

//builder.Services.AddSingleton<LogFillter>();

Log.Logger =  new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.File("D:\\CSharp\\log.txt", 
        rollingInterval: RollingInterval.Day)
    .CreateLogger();


builder.Host.UseSerilog();

// Theem ExcelPackage
ExcelPackage.License.SetNonCommercialPersonal("Ng-Tuan");


// add automaper
builder.Services.AddAutoMapper(typeof(TodoProfile));

builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectString));

builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// use build in middleware

//app.UseExceptionHandler("/Error"); // redirect to error controller
app.UseSession();

app.UseHttpsRedirection();

//ban middleware
app.UseMiddleware<BanTokenMiddleware>();

app.UseAuthentication();   // authentication truowsc khi authorization

app.UseAuthorization();

app.MapControllers();
// 
app.UseCors(option =>
{
    option.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader();
        //.AllowCredentials(); // cho phép cookie được gửi từ client
});

// custome middleware
app.Use(async (context, next) =>
{
    Console.WriteLine("Request to middleware 1");
    //Console.WriteLine("Request: " + context.Request.Method + " " + context.Request.Path);
    await next(context);
    //Console.WriteLine("Response: " + context.Response.StatusCode);
    Console.WriteLine("Response to middleware 1");

});

app.Use(async (context, next) =>
{
    Console.WriteLine("Request to middleware 2");
    //Console.WriteLine("Request: " + context.Request.Method + " " + context.Request.Path);
    await next(context);
    //Console.WriteLine("Response: " + context.Response.StatusCode);
    Console.WriteLine("Response to middleware 2");

});

app.UseMiddleware<LogMiddleware>();

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<GenerateSchoolReportJob>(
    "test-job",
    job => job.ExcuteAsync(),
    Cron.Minutely); // chạy mỗi phút

app.Run();
