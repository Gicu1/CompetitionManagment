using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagment.Models;

[Table("GAME")]
public partial class Game
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("Team1ID")]
    public int? Team1Id { get; set; }

    [Column("Team2ID")]
    public int? Team2Id { get; set; }

    public int? Team1Score { get; set; }

    public int? Team2Score { get; set; }

    [Column("CompetitionID")]
    public int? CompetitionId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Date { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Stadium { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Team1Name { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Team2Name { get; set; }

    [ForeignKey("CompetitionId")]
    [InverseProperty("Games")]
    public virtual Competition? Competition { get; set; }

    [ForeignKey("Team1Id")]
    [InverseProperty("GameTeam1s")]
    public virtual Team? Team1 { get; set; }

    [ForeignKey("Team2Id")]
    [InverseProperty("GameTeam2s")]
    public virtual Team? Team2 { get; set; }

    [NotMapped]
    public int? WinnerId => Team1Score > Team2Score ? Team1Id : Team2Score > Team1Score ? Team2Id : null;

}
