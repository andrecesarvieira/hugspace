using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.Features.Departments.DTOs;

public class CreateDepartmentRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public Guid? ParentId { get; set; }
}
