using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Self_care.Models;

[Table("habits")]
[Index("UserId", Name = "fki_user_id_fkey")]
public partial class Habit
{
    [Key]
    [Column("habits_id")]
    public int HabitsId { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp without time zone")]
    public DateTime? DeletedAt { get; set; }

    [Column("deleted")]
    public bool Deleted { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Habits")]
    public virtual User? User { get; set; }

    [InverseProperty("Habit")]
    public virtual ICollection<UserHabit> UserHabits { get; set; } = new List<UserHabit>();
}
