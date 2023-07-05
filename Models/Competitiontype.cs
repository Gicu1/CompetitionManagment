using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagment.Models;

[Table("COMPETITIONTYPE")]
public partial class Competitiontype
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("NAME")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Name { get; set; }

    [InverseProperty("CompetitionTypeNavigation")]
    public virtual ICollection<Competition> Competitions { get; set; } = new List<Competition>();
}
