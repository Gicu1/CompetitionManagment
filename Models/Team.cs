using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagment.Models;

[Table("TEAM")]
public partial class Team
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Name { get; set; }

    public int? AwardsNr { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Motto { get; set; }

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? CreatedOn { get; set; }

    [InverseProperty("Team1")]
    public virtual ICollection<Game> GameTeam1s { get; set; } = new List<Game>();

    [InverseProperty("Team2")]
    public virtual ICollection<Game> GameTeam2s { get; set; } = new List<Game>();

    [InverseProperty("Team")]
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    [ForeignKey("TeamId")]
    [InverseProperty("Teams")]
    public virtual ICollection<Competition> Competitions { get; set; } = new List<Competition>();
}
