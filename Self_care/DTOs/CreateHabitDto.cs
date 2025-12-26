namespace Self_care.DTOs
{
    public class CreateHabitDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}

