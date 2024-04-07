using System;
namespace Pustok.Areas.Admin.Dtos;

public class ServiceUpdateDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Icon { get; set; }
    public IFormFile File { get; set; } = null!;

}

