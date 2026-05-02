using BusinessLogic.Mapper;
using BusinessLogic.Services;
using BusinessLogic.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Repository.Contexts;
using Repository.Repositories;
using Repository.Repositories.Abstract;

var builder = WebApplication.CreateBuilder(args);

// Controllers and miscellaneous dependencies
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        var converter = new StringEnumConverter
        {
            NamingStrategy = new KebabCaseNamingStrategy()
        };
        options.SerializerSettings.Converters.Add(converter);
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });
        c.CustomSchemaIds(type => type.FullName);
    });
builder.Services.AddAutoMapper(confing =>
    confing.AddProfile<MappingProfile>())
                .AddDbContext<DatabaseContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DevelopmentConnection")!));
builder.Services.AddCors(options =>
{
    options.AddPolicy("TestingCORSPolicy", policy =>
    {
        policy.WithOrigins(
                    "https://localhost:4200",
                    "http://localhost:4200"
                )
                  .AllowAnyHeader()
                  .AllowAnyMethod();
    });
});

// Services
builder.Services.AddScoped<IUserService, UserService>()
                .AddScoped<IBookService, BookService>()
                .AddScoped<IBranchService, BranchService>()
                .AddScoped<ILoanService, LoanService>()
                .AddScoped<IBorrowRequestService, BorrowRequestService>()
                .AddScoped<IFineService, FineService>()
                .AddScoped<IInventoryService, InventoryService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IBookRepository, BookRepository>()
                .AddScoped<IBranchRepository, BranchRepository>()
                .AddScoped<ILoanRepository, LoanRepository>()
                .AddScoped<IFineRepository, FineRepository>()
                .AddScoped<IBookGenreRepository, BookGenreRepository>()
                .AddScoped<IBranchBookRelationRepository, BranchBookRelationRepository>()
                .AddScoped<ILoanBookRelationRepository, LoanBookRelationRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty; // This makes Swagger the home page
    });
}

app.UseCors("TestingCORSPolicy");

// Note: UseHttpsRedirection removed for development — HTTPS redirects break
// cross-origin requests from the Angular dev server (browsers won't follow
// cross-origin 301/307 redirects in CORS context). Use HTTP port 5266 directly.

app.UseAuthorization();

app.MapControllers();

app.Run();
