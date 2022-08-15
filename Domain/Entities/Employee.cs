namespace Domain.Entities;

public class Employee
{
    public int ID { get; set; }
    public string Name { get; set; }
    public DateTime JoiningDate { get; set; }
    public string ImageFileName { get; set; }
    public int DepartmentId { get; set; }
}
