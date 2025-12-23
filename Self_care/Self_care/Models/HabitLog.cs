using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Self_care.Models;

[Table("habit_logs")]
[Index("Date", Name = "idx_habit_logs_date")]
[Index("UserHabitId", Name = "idx_habit_logs_user_habit_id")]
public partial class HabitLog
{
    [Key]
    [Column("habit_lg_id")]
    public int HabitLgId { get; set; }

    [Column("user_habit_id")]
    public int UserHabitId { get; set; }

    [Column("date", TypeName = "timestamp without time zone")]
    public DateTime Date { get; set; }

    [Column("completed")]
    public bool Completed { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp without time zone")]
    public DateTime? DeletedAt { get; set; }

    [Column("deleted")]
    public bool Deleted { get; set; }

    [ForeignKey("UserHabitId")]
    [InverseProperty("HabitLogs")]
    public virtual UserHabit UserHabit { get; set; } = null!;
}
