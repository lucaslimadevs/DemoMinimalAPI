using DemoMinimalAPI.Data;
using DemoMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MinimalContextDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/fornecedor", async (
    MinimalContextDb context) =>
    await context.Fornecedores.ToListAsync())
    .WithName("GetFonecedores")
    .WithTags("Fornecedor"); //tags name (similar controller name)

app.MapGet("/fornecedor/{id}", async (
    Guid id, 
    MinimalContextDb context) =>
    await context.Fornecedores.FindAsync(id)
        is Fornecedor fornecedor ? //if exists fornecedor
        Results.Ok(fornecedor) : 
        Results.NotFound()
    )
    .Produces<Fornecedor>(StatusCodes.Status200OK) //documentation for swagger
    .Produces<Fornecedor>(StatusCodes.Status404NotFound)
    .WithName("GetFonecedoresById")
    .WithTags("Fornecedor");

app.MapPost("/fornecedor", async (
    MinimalContextDb context,
    Fornecedor fornecedor) =>
{
    if (!MiniValidator.TryValidate(fornecedor, out var errors)) //package MiniValidation, minimalistic validation library
        return Results.ValidationProblem(errors);

    await context.Fornecedores.AddAsync(fornecedor);
    var result = await context.SaveChangesAsync();

    return result > 0
        ? Results.Created($"/fornecedor/{fornecedor.Id}", fornecedor)
        : Results.BadRequest("Houve um problema ao salvar registro");
})
    .Produces<Fornecedor>(StatusCodes.Status201Created) //documentation for swagger
    .Produces<Fornecedor>(StatusCodes.Status400BadRequest)
    .WithName("PostFonecedor")
    .WithTags("Fornecedor");

app.Run();
