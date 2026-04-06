using AutoMapper;
using BusinessLogic.Mapper;
using BusinessLogic.Services;
using BusinessLogic.Services.Abstract;
using Microsoft.EntityFrameworkCore;
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
    });
builder.Services.AddSwaggerGen(c =>
    {
        c.DescribeAllParametersInCamelCase();
    });
builder.Services.AddAutoMapper(confing =>
    confing.AddProfile<MappingProfile>())
                .AddDbContext<DatabaseContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DevelopmentConnection")!));

// Services
builder.Services.AddScoped<IUserService, UserService>()
                .AddScoped<IBookService, BookService>()
                .AddScoped<IBranchService, BranchService>()
                .AddScoped<ILoanService, LoanService>()
                .AddScoped<IFineService, FineService>();

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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
