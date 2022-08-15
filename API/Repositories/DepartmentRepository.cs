using API.Contexts;
using API.Models;
using Dapper;

namespace API.Repositories;

public class DepartmentRepository : BaseRepository, IDepartmentRepository
{
    public DepartmentRepository(DapperContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        var query = "SELECT * FROM Department";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Department>(query);
    }
    public async Task<Department> GetByIdAsync(int id)
    {
        var query = "SELECT * FROM Department WHERE DepartmentID=@Id";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Department>(query, new { Id = id });
    }
    public async Task<Department> AddAsync(Department department)
    {
        var query = "INSERT INTO Department VALUES(@Name); SELECT @@identity; ";

        using var connection = _context.CreateConnection();
        int id = await connection.ExecuteScalarAsync<int>(query, new { Name = department.DepartmentName });
        department.DepartmentId = id;

        return department;
    }

    public async Task UpdateAsync(Department department)
    {
        var query = "UPDATE Department SET DepartmentName=@Name WHERE DepartmentID=@Id";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new { Id = department.DepartmentId, Name = department.DepartmentName });
    }

    public async Task DeleteAsync(int id)
    {
        var query = "DELETE FROM Department WHERE DepartmentID=@Id";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new { Id = id });
    }
}
