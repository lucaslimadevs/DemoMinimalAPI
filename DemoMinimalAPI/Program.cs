using DemoMinimalAPI.Data;
using DemoMinimalAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniValidation;
using NetDevPack.Identity;
using NetDevPack.Identity.Jwt;
using NetDevPack.Identity.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MinimalContextDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityEntityFrameworkContextConfiguration(options =>  //NetDevPack.Identity package with all configuration for identity context 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("DemoMinimalAPI")));

builder.Services.AddIdentityConfiguration();
builder.Services.AddJwtConfiguration(builder.Configuration, "AppJwtSettings");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthConfiguration();
app.UseHttpsRedirection();

app.MapPost("/registro", 
    async (
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IOptions<AppJwtSettings> appJwtSettings,
        RegisterUser registerUser) => 
    {
        if (registerUser == null)
            return Results.BadRequest("Usuário não foi informado");

        if (!MiniValidator.TryValidate(registerUser, out var errors))
            return Results.ValidationProblem(errors);

        var user = new IdentityUser
        {
            UserName = registerUser.Email,
            Email = registerUser.Email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, registerUser.Password);

        if (!result.Succeeded)
            return Results.BadRequest(result.Errors);

        var jwt = new JwtBuilder()
                    .WithUserManager(userManager)
                    .WithJwtSettings(appJwtSettings.Value)
                    .WithEmail(user.Email)
                    .WithJwtClaims()
                    .WithUserRoles()
                    .BuildUserResponse();

        return Results.Ok(jwt);
    }).ProducesValidationProblem()
    .Produces<Fornecedor>(StatusCodes.Status200OK)
    .Produces<Fornecedor>(StatusCodes.Status400BadRequest)
    .WithName("RegistroUsuario")
    .WithTags("Usuario");

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

app.MapPost("/fornecedor", 
    async (MinimalContextDb context, 
           Fornecedor fornecedor) =>
    {
        if (!MiniValidator.TryValidate(fornecedor, out var errors)) //package MiniValidation, minimalistic validation library
            return Results.ValidationProblem(errors);

        await context.Fornecedores.AddAsync(fornecedor);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.Created($"/fornecedor/{fornecedor.Id}", fornecedor)
            : Results.BadRequest("Houve um problema ao salvar registro");

    }).ProducesValidationProblem()
    .Produces<Fornecedor>(StatusCodes.Status201Created) //documentation for swagger
    .Produces<Fornecedor>(StatusCodes.Status400BadRequest)
    .WithName("PostFornecedor")
    .WithTags("Fornecedor");

app.MapPut("/fornecedor/{id}", 
    async (Guid id,
           MinimalContextDb context,
           Fornecedor fornecedor) =>
    { 
        var fornecedorBanco = await context.Fornecedores.AsNoTracking()
                                                        .FirstOrDefaultAsync(e => e.Id == id);

        if (fornecedorBanco == null) return Results.NotFound();

        if (!MiniValidator.TryValidate(fornecedor, out var errors)) 
            return Results.ValidationProblem(errors);

        context.Fornecedores.Update(fornecedor);
        var result = await context.SaveChangesAsync();

        return result > 0 ?
            Results.NoContent() : 
            Results.BadRequest("Houve um problema ao salvar o registro");

    }).ProducesValidationProblem()
    .Produces<Fornecedor>(StatusCodes.Status204NoContent) //documentation for swagger
    .Produces<Fornecedor>(StatusCodes.Status400BadRequest)
    .Produces<Fornecedor>(StatusCodes.Status404NotFound)
    .WithName("PutFornecedor")
    .WithTags("Fornecedor");

app.MapDelete("/fornecedor/{id}",
    async (Guid id,
           MinimalContextDb context) =>
    {
        var fornecedor = await context.Fornecedores.FindAsync(id);
        if (fornecedor == null) return Results.NotFound();

        context.Fornecedores.Remove(fornecedor);
        var result = await context.SaveChangesAsync();

        return result > 0 ?
            Results.NoContent() :
            Results.BadRequest("Houve um problema ao remover o registro");

    }).ProducesValidationProblem()
    .Produces<Fornecedor>(StatusCodes.Status204NoContent) //documentation for swagger
    .Produces<Fornecedor>(StatusCodes.Status400BadRequest)
    .Produces<Fornecedor>(StatusCodes.Status404NotFound)
    .WithName("DeleteFornecedor")
    .WithTags("Fornecedor");

app.Run();
