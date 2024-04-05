namespace Pustok.Areas.Admin.Dtos;

public class CategoryUpdateDto
{
    public string Name { get; set; } = null!;
    public int? ParentId { get; set; }

}