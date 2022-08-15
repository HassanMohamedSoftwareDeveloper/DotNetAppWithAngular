using API.Contexts;
using API.Models;
using Dapper;

namespace API.Repositories;

public class EmployeeRepository : BaseRepository, IEmployeeRepository
{
    public EmployeeRepository(DapperContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        var query = "SELECT EmployeeId,EmployeeName,Department.DepartmentId,DepartmentName AS Department,DateOfJoining,PhotoFileName FROM Employee JOIN Department ON Employee.DepartmentId=Department.DepartmentId";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Employee>(query);
    }
    public async Task<Employee> GetByIdAsync(int id)
    {
        var query = "SELECT EmployeeId,EmployeeName,Department.DepartmentId,DepartmentName,DateOfJoining,PhotoFileName FROM Employee JOIN Department ON Employee.DepartmentId=Department.DepartmentId WHERE EmployeeID=@Id";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Employee>(query, new { Id = id });
    }
    public async Task<Employee> AddAsync(Employee employee)
    {
        var query = "INSERT INTO Employee (EmployeeName,DepartmentId,DateOfJoining,PhotoFileName) VALUES(@Name,@DeptId,@Date,@PhotoFileName); SELECT @@identity; ";

        using var connection = _context.CreateConnection();
        int id = await connection.ExecuteScalarAsync<int>(query, new
        {
            Name = employee.EmployeeName,
            DeptId = employee.DepartmentId,
            Date = employee.DateOfJoining,
            PhotoFileName = employee.PhotoFileName
        });
        employee.EmployeeId = id;

        return employee;
    }

    public async Task UpdateAsync(Employee employee)
    {
        var query = "UPDATE Employee SET EmployeeName=@Name,DepartmentId=@DeptId,DateOfJoining=@Date,PhotoFileName=@PhotoFileName WHERE EmployeeID=@Id";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new
        {
            Id = employee.EmployeeId,
            Name = employee.EmployeeName,
            DeptId = employee.DepartmentId,
            Date = employee.DateOfJoining,
            PhotoFileName = employee.PhotoFileName
        });
    }

    public async Task DeleteAsync(int id)
    {
        var query = "DELETE FROM Employee WHERE EmployeeID=@Id";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new { Id = id });
    }
}
