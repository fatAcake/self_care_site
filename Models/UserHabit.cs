using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models;

[Table("user_habits")]
[Index("HabitId", Name = "idx_user_habits_habit_id")]
[Index("Status", Name = "idx_user_habits_status")]
[Index("UserId", Name = "idx_user_habits_user_id")]
public partial class UserHabit
{
    [Key]
    [Column("us_habit_id")]
    public int UsHabitId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("habit_id")]
    public int HabitId { get; set; }

    [Column("is_marked")]
    public bool IsMarked { get; set; }

    [Column("goal")]
    public int? Goal { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Column("frequency")]
    [StringLength(100)]
    public string Frequency { get; set; } = null!;

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp without time zone")]
    public DateTime? DeletedAt { get; set; }

    [Column("deleted")]
    public bool Deleted { get; set; }

    [ForeignKey("HabitId")]
    [InverseProperty("UserHabits")]
    public virtual Habit Habit { get; set; } = null!;

    [InverseProperty("UserHabit")]
    public virtual ICollection<HabitLog> HabitLogs { get; set; } = new List<HabitLog>();

    [ForeignKey("UserId")]
    [InverseProperty("UserHabits")]
    public virtual User User { get; set; } = null!;
}
