using CompetitionManagment.Models;

public class TeamScore
{
    public Team Team { get; set; }
    public int Score { get; set; }
    public int GoalsScored { get; set; }
    public int GoalsConceded { get; set; }
}