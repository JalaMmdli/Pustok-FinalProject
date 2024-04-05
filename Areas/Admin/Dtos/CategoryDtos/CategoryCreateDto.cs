using System;
namespace Pustok.Areas.Admin.Dtos;

public class CategoryCreateDto
{
	public string Name { get; set; } = null!;
	public int? ParentId { get; set; }

}
