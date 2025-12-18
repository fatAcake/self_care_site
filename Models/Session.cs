using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models;

[Table("sessions")]
[Index("StartedAt", Name = "idx_sessions_started_at")]
[Index("Type", Name = "idx_sessions_type")]
[Index("UserId", Name = "idx_sessions_user_id")]
public partial class Session
{
    [Key]
    [Column("session_id")]
    public int SessionId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("type")]
    [StringLength(100)]
    public string Type { get; set; } = null!;

    [Column("duration")]
    public long Duration { get; set; }

    [Column("completed")]
    public bool? Completed { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("started_at", TypeName = "timestamp without time zone")]
    public DateTime StartedAt { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp without time zone")]
    public DateTime? DeletedAt { get; set; }

    [Column("deleted")]
    public bool Deleted { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Sessions")]
    public virtual User User { get; set; } = null!;
}
