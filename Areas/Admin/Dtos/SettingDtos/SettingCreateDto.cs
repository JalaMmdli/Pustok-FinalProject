using System;
namespace Pustok.Areas.Admin.Dtos;

	public class SettingCreateDto
	{

    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}


public class SettingUpdateDto
{

    public string? Key { get; set; } 
    public string Value { get; set; } = null!;
}
