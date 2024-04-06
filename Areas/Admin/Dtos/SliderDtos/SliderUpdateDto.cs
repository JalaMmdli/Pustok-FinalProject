using System;
namespace Pustok.Areas.Admin.Dtos;

public class SliderUpdateDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Button { get; set; } = null!;
    public string? ImagePath { get; set; }
    public IFormFile? File { get; set; }

}

