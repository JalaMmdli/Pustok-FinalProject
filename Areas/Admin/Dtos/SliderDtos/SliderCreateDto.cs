namespace Pustok.Areas.Admin.Dtos
{
    public class SliderCreateDto
	{

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Button { get; set; } = null!;
        public IFormFile File { get; set; } = null!;
    }
}

