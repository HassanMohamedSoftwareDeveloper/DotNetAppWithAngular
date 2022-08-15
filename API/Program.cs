using API.Contexts;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", opt => opt.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

var app = builder.Build();
app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Photos")),
    RequestPath = "/Photos"
});

app.MapGet("api/department", async ([FromServices] IDepartmentRepository repo) =>
{
    var data = await repo.GetAllAsync();
    return Results.Ok(data);
});

app.MapGet("api/department/{id}", async ([FromServices] IDepartmentRepository repo, int id) =>
{
    var department = await repo.GetByIdAsync(id);
    if (department is null) return Results.NotFound();
    return Results.Ok(department);
});

app.MapPost("api/department", async ([FromServices] IDepartmentRepository repo, Department request) =>
{
    var dept = await repo.AddAsync(request);
    return Results.Created($"/api/department/{dept.DepartmentId}", dept);
});

app.MapPut("api/department/{id}", async ([FromServices] IDepartmentRepository repo, int id, Department request) =>
{
    var dept = await repo.GetByIdAsync(id);
    if (dept is null) return Results.NotFound();
    dept.DepartmentName = request.DepartmentName;
    await repo.UpdateAsync(dept);
    return Results.NoContent();
});

app.MapDelete("api/department/{id}", async ([FromServices] IDepartmentRepository repo, int id) =>
{
    var dept = await repo.GetByIdAsync(id);
    if (dept is null) return Results.NotFound();
    await repo.DeleteAsync(id);
    return Results.NoContent();
});


app.MapGet("api/employee", async ([FromServices] IEmployeeRepository repo) =>
{
    var data = await repo.GetAllAsync();
    return Results.Ok(data);
});

app.MapGet("api/employee/{id}", async ([FromServices] IEmployeeRepository repo, int id) =>
{
    var employee = await repo.GetByIdAsync(id);
    if (employee is null) return Results.NotFound();
    return Results.Ok(employee);
});

app.MapPost("api/employee", async ([FromServices] IEmployeeRepository repo, Employee request) =>
{
    var emp = await repo.AddAsync(request);
    return Results.Created($"/api/employee/{emp.EmployeeId}", emp);
});

app.MapPut("api/employee/{id}", async ([FromServices] IEmployeeRepository repo, int id, Employee request) =>
{
    var emp = await repo.GetByIdAsync(id);
    if (emp is null) return Results.NotFound();
    emp.EmployeeName = request.EmployeeName;
    emp.DepartmentId = request.DepartmentId;
    emp.DateOfJoining = request.DateOfJoining;
    emp.PhotoFileName = request.PhotoFileName;

    await repo.UpdateAsync(emp);
    return Results.NoContent();
});

app.MapDelete("api/employee/{id}", async ([FromServices] IEmployeeRepository repo, int id) =>
{
    var dept = await repo.GetByIdAsync(id);
    if (dept is null) return Results.NotFound();
    await repo.DeleteAsync(id);
    return Results.NoContent();
});


app.MapPost("api/file", async (HttpRequest request) =>
{
    try
    {
        var httpRequest = request.Form;
        var postedFile = httpRequest.Files[0];
        var fileName = postedFile.FileName;
        var physicalPath = app.Environment.ContentRootPath + "/Photos/" + fileName;

        using var stream = new FileStream(physicalPath, FileMode.Create);
        await postedFile.CopyToAsync(stream);
        return Results.Ok(fileName);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return Results.Ok("avatar.jpg");
    }
});


app.Run();