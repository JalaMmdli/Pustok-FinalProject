using System;
namespace Pustok.Areas.Admin.Dtos;

public class ServiceCreateDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public IFormFile File { get; set; } = null!;
    
}

