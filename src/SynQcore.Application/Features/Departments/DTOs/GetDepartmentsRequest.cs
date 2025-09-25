using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.Features.Departments.DTOs;

public class GetDepartmentsRequest
{
    public int Page { get; set; } = 1;
    
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
    
    [StringLength(100)]
    public string? Name { get; set; }
    
    [StringLength(100)]
    public string? Code { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public bool? IsActive { get; set; }
    
    public bool IncludeEmployees { get; set; }
}