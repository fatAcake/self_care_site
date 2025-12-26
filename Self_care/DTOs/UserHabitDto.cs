namespace Self_care.DTOs
{
    public class UserHabitDto
    {
        public int? HabitId { get; set; }
        public bool IsMarked { get; set; } = false;
        public int? Goal { get; set; }
        public string Status { get; set; } = "active"; 
        public string Frequency { get; set; } = "daily";
        public DateOnly ExecuteDate { get; set; } 
    }
}
