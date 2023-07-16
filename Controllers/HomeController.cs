using CompetitionManagment.Data;
using CompetitionManagment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CompetitionManagment.Controllers
{
    public class HomeController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public HomeController(CompetitionManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Teams(int competitionId)
        {
            Competition? competition = await _context.Competitions
                .Include(c => c.Teams)
                .FirstOrDefaultAsync(c => c.Id == competitionId);

            List<Team> allTeams = await _context.Teams.ToListAsync();
            if (competition == null)
            {
                return NotFound();
            }

            List<Team> participatingTeams = competition.Teams.ToList();
            List<Team> availableTeams = allTeams.Except(participatingTeams).ToList();

            competition.AllTeams = availableTeams;
            ViewData["CompetitionId"] = competitionId;
            return View(competition);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddTeam(int competitionId, int teamId)
        {
            Competition? competition = await _context.Competitions
                .Include(c => c.Teams)
                .FirstOrDefaultAsync(c => c.Id == competitionId);

            Team? teamToAdd = await _context.Teams.FindAsync(teamId);

            if (competition == null || teamToAdd == null)
            {
                //return NotFound();
                return RedirectToAction("Teams", new { competitionId });
            }
            
            if (competition.Teams.Contains(teamToAdd))
            {
                return BadRequest("The team is already participating in the competition.");
            }

            competition.Teams.Add(teamToAdd);
            await _context.SaveChangesAsync();

            return RedirectToAction("Teams", new { competitionId });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteTeam(int competitionId, int teamId)
        {
            Competition? competition = await _context.Competitions
                .Include(c => c.Teams)
                    .ThenInclude(t => t.GameTeam1s)
                .Include(c => c.Teams)
                    .ThenInclude(t => t.GameTeam2s)
                .FirstOrDefaultAsync(c => c.Id == competitionId);

            if (competition == null)
            {
                return RedirectToAction("Teams", new { competitionId });
            }

            Team? teamToDelete = competition.Teams.FirstOrDefault(t => t.Id == teamId);

            if (teamToDelete == null)
            {
                return RedirectToAction("Teams", new { competitionId });
            }

            // Delete all games where the team is either Team1 or Team2
            foreach (var game in teamToDelete.GameTeam1s.Concat(teamToDelete.GameTeam2s).ToList())
            {
                if (game.CompetitionId == competitionId)
                {
                    _context.Games.Remove(game);
                }
            }

            competition.Teams.Remove(teamToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction("Teams", new { competitionId });
        }


        public IActionResult Index()
        {
            var competitions = _context.Competitions
                .Include(c => c.CompetitionTypeNavigation)
                .Include(c => c.Teams)
                .ThenInclude(t => t.GameTeam1s)
                .Include(c => c.Teams)
                .ThenInclude(t => t.GameTeam2s)
                .ToList();

            foreach (var competition in competitions)
            {
                var teamScores = new List<TeamScore>();

                foreach (var team in competition.Teams)
                {
                    int score;
                    if (competition.CompetitionType == 1) // League
                    {
                        var result = CalculateTeamScore(team, competition.Id);
                        score = result.Score;
                    }
                    else // Knockout
                    {
                        var result = CalculateKnockoutTeamScore(team, competition.Id);
                        score = result.Score;
                    }

                    teamScores.Add(new TeamScore { Team = team, Score = score });
                }

                teamScores = teamScores.OrderByDescending(ts => ts.Score).ToList();
                var winner = teamScores.FirstOrDefault();
                if (winner != null && (teamScores.Count < 2 || winner.Score > teamScores[1].Score))
                {
                    competition.WinnerId = winner.Team.Id;
                    competition.WinnerName = winner.Team.Name;
                }
                else
                {
                    competition.WinnerId = null;
                    competition.WinnerName = "No winner";
                }
            }

            _context.SaveChanges();

            return View(competitions);
        }


        public IActionResult LeagueGames(int competitionId)
        {
            var competition = _context.Competitions
                .Include(c => c.Games)
                .Include(c => c.Teams)
                .FirstOrDefault(c => c.Id == competitionId);

            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }

        public IActionResult LeagueLeaderboard(int competitionId)
        {
            var teams = _context.Teams
                .Include(t => t.GameTeam1s)
                .Include(t => t.GameTeam2s)
                .Include(t => t.Competitions)
                .Where(t => t.Competitions.Any(c => c.Id == competitionId))
                .ToList();

            var teamScores = new List<TeamScore>();

            foreach (var team in teams)
            {
                var score = CalculateTeamScore(team, competitionId);
                teamScores.Add(new TeamScore { Team = team, Score = score.Score, GoalsScored = score.GoalsScored, GoalsConceded = score.GoalsConceded });
            }

            teamScores = teamScores.OrderByDescending(ts => ts.Score).ToList();
            ViewData["CompetitionId"] = competitionId;
            return View(teamScores);
        }

        private (int Score, int GoalsScored, int GoalsConceded) CalculateTeamScore(Team team, int competitionId)
        {
            var games = team.GameTeam1s.Concat(team.GameTeam2s)
                .Where(g => g.CompetitionId == competitionId)
                .ToList();

            int score = 0;
            int goalsScored = 0;
            int goalsConceded = 0;

            foreach (var game in games)
            {
                if (game.Team1Score.HasValue && game.Team2Score.HasValue)
                {
                    if (game.Team1Id == team.Id && game.Team1Score > game.Team2Score)
                    {
                        score += 3; // Team 1 won the game
                        goalsScored += game.Team1Score.Value;
                        goalsConceded += game.Team2Score.Value;
                    }
                    else if (game.Team2Id == team.Id && game.Team2Score > game.Team1Score)
                    {
                        score += 3; // Team 2 won the game
                        goalsScored += game.Team2Score.Value;
                        goalsConceded += game.Team1Score.Value;
                    }
                    else if (game.Team1Score == game.Team2Score)
                    {
                        score += 1; // The game ended in a draw
                        if (game.Team1Id == team.Id)
                        {
                            goalsScored += game.Team1Score.Value;
                            goalsConceded += game.Team2Score.Value;
                        }
                        else
                        {
                            goalsScored += game.Team2Score.Value;
                            goalsConceded += game.Team1Score.Value;
                        }
                    }
                    else
                    {
                        if (game.Team1Id == team.Id)
                        {
                            goalsScored += game.Team1Score.Value;
                            goalsConceded += game.Team2Score.Value;
                        }
                        else
                        {
                            goalsScored += game.Team2Score.Value;
                            goalsConceded += game.Team1Score.Value;
                        }
                    }
                }
            }

            return (score, goalsScored, goalsConceded);
        }



        public IActionResult KnockoutGames(int competitionId)
        {
            var competition = _context.Competitions
                .Include(c => c.Games)
                .Include(c => c.Teams)
                .FirstOrDefault(c => c.Id == competitionId);

            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }


        public IActionResult KnockoutLeaderboard(int competitionId)
        {
            var teams = _context.Teams
                .Include(t => t.GameTeam1s)
                .Include(t => t.GameTeam2s)
                .Include(t => t.Competitions)
                .Where(t => t.Competitions.Any(c => c.Id == competitionId))
                .ToList();

            var teamScores = new List<TeamScore>();

            foreach (var team in teams)
            {
                var score = CalculateKnockoutTeamScore(team, competitionId);
                teamScores.Add(new TeamScore { Team = team, Score = score.Score, GoalsScored = score.GoalsScored, GoalsConceded = score.GoalsConceded });
            }

            teamScores = teamScores.OrderByDescending(ts => ts.Score).ToList();
            ViewData["CompetitionId"] = competitionId;
            return View(teamScores);
        }


        private (int Score, int GoalsScored, int GoalsConceded) CalculateKnockoutTeamScore(Team team, int competitionId)
        {
            var games = team.GameTeam1s.Concat(team.GameTeam2s)
                .Where(g => g.CompetitionId == competitionId)
                .ToList();

            int score = 0;
            int goalsScored = 0;
            int goalsConceded = 0;

            foreach (var game in games)
            {
                if (game.Team1Score.HasValue && game.Team2Score.HasValue)
                {
                    if (game.Team1Id == team.Id && game.Team1Score > game.Team2Score)
                    {
                        score += 1; // Team 1 won the game
                        goalsScored += game.Team1Score.Value;
                        goalsConceded += game.Team2Score.Value;
                    }
                    else if (game.Team2Id == team.Id && game.Team2Score > game.Team1Score)
                    {
                        score += 1; // Team 2 won the game
                        goalsScored += game.Team2Score.Value;
                        goalsConceded += game.Team1Score.Value;
                    }
                    else
                    {
                        if (game.Team1Id == team.Id)
                        {
                            goalsScored += game.Team1Score.Value;
                            goalsConceded += game.Team2Score.Value;
                        }
                        else
                        {
                            goalsScored += game.Team2Score.Value;
                            goalsConceded += game.Team1Score.Value;
                        }
                    }
                }
            }

            return (score, goalsScored, goalsConceded);
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
