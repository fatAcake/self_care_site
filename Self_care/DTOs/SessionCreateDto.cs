using System;
using System.ComponentModel.DataAnnotations;

namespace Self_care.DTOs
{
    public class SessionCreateDto
    {
        [Required, StringLength(100)]
        public string Type { get; set; } = string.Empty;

        [Required, Range(1, long.MaxValue)]
        public long Duration { get; set; }

        public bool? Completed { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }
    }
}

