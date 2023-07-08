using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagment.Models;

[Table("COMPETITION")]
public partial class Competition
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column(TypeName = "date")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EndDate { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Location { get; set; }

    public int? CompetitionType { get; set; }

    [ForeignKey("CompetitionType")]
    [InverseProperty("Competitions")]
    public virtual Competitiontype? CompetitionTypeNavigation { get; set; }

    [InverseProperty("Competition")]
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    [ForeignKey("CompetitionId")]
    [InverseProperty("Competitions")]
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    [NotMapped] // This property will not be mapped to the database
    public List<Team> AllTeams { get; set; } = new List<Team>();
}
