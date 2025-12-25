namespace Self_care.DTOs
{
    public class UserHabitResponseDto
    {
        public int UsHabitId { get; set; }
        public int HabitId { get; set; }
        public string HabitName { get; set; } = null!;
        public bool IsMarked { get; set; }
        public DateTime ExecuteDate { get; set; }
        public string Status { get; set; } = null!;
    }
}

