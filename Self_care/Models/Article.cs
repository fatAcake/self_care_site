using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Self_care.Models;

[Table("articles")]
[Index("IsPublished", Name = "idx_articles_is_published")]
[Index("PublishedAt", Name = "idx_articles_published_at")]
public partial class Article
{
    [Key]
    [Column("article_id")]
    public int ArticleId { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("author")]
    [StringLength(255)]
    public string Author { get; set; } = null!;

    [Column("tags")]
    public string? Tags { get; set; }

    [Column("published_at", TypeName = "timestamp without time zone")]
    public DateTime? PublishedAt { get; set; }

    [Column("is_published")]
    [StringLength(100)]
    public string IsPublished { get; set; } = null!;

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp without time zone")]
    public DateTime? DeletedAt { get; set; }

    [Column("deleted")]
    public bool Deleted { get; set; }
}
