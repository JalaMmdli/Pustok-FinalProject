using Pustok.Models.baseModels;

namespace Pustok.Models;

public class Setting:BaseModel
{
	public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}

