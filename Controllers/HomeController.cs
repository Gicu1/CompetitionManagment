using CompetitionManagment.Data;
using CompetitionManagment.Models;
using Microsoft.AspNetCore.Mvc;
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
                .FirstOrDefaultAsync(c => c.Id == competitionId);

			if (competition == null )
			{
				//return NotFound();
				return RedirectToAction("Teams", new { competitionId });
			}

			Team? teamToDelete = competition.Teams.FirstOrDefault(t => t.Id == teamId);

			if (teamToDelete == null)
			{
				//return NotFound();
				return RedirectToAction("Teams", new { competitionId });
			}

			if (competition != null && teamToDelete != null)
            {
                competition.Teams.Remove(teamToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Teams", new { competitionId });
        }

        public async Task<IActionResult> Index()
        {
            List<Competition> competitions = await _context.Competitions
                .Include(c => c.Teams)
                .ToListAsync();

            return View(competitions);
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
