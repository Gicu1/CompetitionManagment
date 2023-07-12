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

            Team teamToDelete = competition.Teams.FirstOrDefault(t => t.Id == teamId);

            if (teamToDelete == null)
            {
                return RedirectToAction("Teams", new { competitionId });
            }

            // Delete all games where the team is either Team1 or Team2
            foreach (var game in teamToDelete.GameTeam1s.Concat(teamToDelete.GameTeam2s).ToList())
            {
                _context.Games.Remove(game);
            }

            competition.Teams.Remove(teamToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction("Teams", new { competitionId });
        }


        public async Task<IActionResult> Index()
        {
            List<Competition> competitions = await _context.Competitions
                .Include(c => c.Teams)
                .Include(d => d.CompetitionTypeNavigation)
                .ToListAsync();


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
                teamScores.Add(new TeamScore { Team = team, Score = score });
            }

            teamScores = teamScores.OrderByDescending(ts => ts.Score).ToList();
            ViewData["CompetitionId"] = competitionId;
            return View(teamScores);
        }

        private int CalculateTeamScore(Team team, int competitionId)
        {
            var games = team.GameTeam1s.Concat(team.GameTeam2s)
                .Where(g => g.CompetitionId == competitionId)
                .ToList();

            int score = 0;

            foreach (var game in games)
            {
                if (game.Team1Score.HasValue && game.Team2Score.HasValue)
                {
                    if (game.Team1Id == team.Id && game.Team1Score > game.Team2Score)
                    {
                        score += 3; // Team 1 won the game
                    }
                    else if (game.Team2Id == team.Id && game.Team2Score > game.Team1Score)
                    {
                        score += 3; // Team 2 won the game
                    }
                    else if (game.Team1Score == game.Team2Score)
                    {
                        score += 1; // The game ended in a draw
                    }
                }
            }

            return score;
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
