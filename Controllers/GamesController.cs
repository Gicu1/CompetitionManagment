using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CompetitionManagment.Data;
using CompetitionManagment.Models;

namespace CompetitionManagment.Controllers
{
    public class GamesController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public GamesController(CompetitionManagementContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
			
			var competitionManagementContext = _context.Games.Include(g => g.Competition).Include(g => g.Team1).Include(g => g.Team2);
			ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name");
			ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Name");
			ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Name");
			return View(await competitionManagementContext.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Competition)
                .Include(g => g.Team1)
                .Include(g => g.Team2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }
			ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name", game.CompetitionId);
			ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Name", game.Team1Id);
			ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Name", game.Team2Id);
			return View(game);
        }

        // GET: Games/Create
        public IActionResult LeagueCreate(int competitionId, int team1Id, int team2Id)
        {
            var game = new Game
            {
                CompetitionId = competitionId,
                Team1Id = team1Id,
                Team2Id = team2Id,
                Team1Name = _context.Teams.Find(team1Id)?.Name,
                Team2Name = _context.Teams.Find(team2Id)?.Name
            };

            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name", competitionId);
            ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Name", team1Id);
            ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Name", team2Id);
            return View(game);
        }



        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeagueCreate([Bind("Id,Team1Id,Team2Id,Team1Score,Team2Score,CompetitionId,Date,Stadium,Team1Name,Team2Name")] Game game)
        {
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name", game.CompetitionId);
            ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Name", game.Team1Id);
            ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Name", game.Team2Id);
            if (ModelState.IsValid)
            {
                game.Team1Name = _context.Teams.Find(game.Team1Id)?.Name;
                game.Team2Name = _context.Teams.Find(game.Team2Id)?.Name;
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction("LeagueGames", "Home", new { competitionId = game.CompetitionId });
            }

            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> LeagueEdit(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name", game.CompetitionId);
            ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Name", game.Team1Id);
            ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Name", game.Team2Id);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeagueEdit(int id, [Bind("Id,Team1Id,Team2Id,Team1Score,Team2Score,CompetitionId,Date,Stadium,Team1Name,Team2Name")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    game.Team1Name = _context.Teams.Find(game.Team1Id)?.Name;
                    game.Team2Name = _context.Teams.Find(game.Team2Id)?.Name;
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("LeagueGames", "Home", new { competitionId = game.CompetitionId });
            }
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name", game.CompetitionId);
            ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Id", game.Team1Id);
            ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Id", game.Team2Id);
            return View(game);
        }


        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Competition)
                .Include(g => g.Team1)
                .Include(g => g.Team2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }
			ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name", game.CompetitionId);
			ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Name", game.Team1Id);
			ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Name", game.Team2Id);
			return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Games == null)
            {
                return Problem("Entity set 'CompetitionManagementContext.Games'  is null.");
            }
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: Games/KnockoutCreate
        public IActionResult KnockoutCreate(int competitionId)
        {
            var competition = _context.Competitions.Include(c => c.Teams).FirstOrDefault(c => c.Id == competitionId);
            if (competition == null)
            {
                return NotFound();
            }

            ViewData["CompetitionId"] = competitionId;
            ViewData["Team1Id"] = new SelectList(competition.Teams, "Id", "Name");
            ViewData["Team2Id"] = new SelectList(competition.Teams, "Id", "Name");
            return View();
        }

        // POST: Games/KnockoutCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KnockoutCreate([Bind("Id,Team1Id,Team2Id,Team1Score,Team2Score,CompetitionId,Date,Stadium")] Game game)
        {
            if (game.Team1Id == game.Team2Id)
            {
                ModelState.AddModelError(string.Empty, "A team cannot play against itself.");
            }

            var existingGame = _context.Games.FirstOrDefault(g =>
                g.CompetitionId == game.CompetitionId &&
                ((g.Team1Id == game.Team1Id && g.Team2Id == game.Team2Id) ||
                 (g.Team1Id == game.Team2Id && g.Team2Id == game.Team1Id)));
            if (existingGame != null)
            {
                ModelState.AddModelError(string.Empty, "A game between these two teams already exists.");
            }

            if (ModelState.IsValid)
            {
                game.Team1Name = _context.Teams.Find(game.Team1Id)?.Name;
                game.Team2Name = _context.Teams.Find(game.Team2Id)?.Name;
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction("KnockoutGames", "Home", new { competitionId = game.CompetitionId });
            }

            var competition = _context.Competitions.Include(c => c.Teams).FirstOrDefault(c => c.Id == game.CompetitionId);
            if (competition == null)
            {
                return NotFound();
            }

            ViewData["CompetitionId"] = game.CompetitionId;
            ViewData["Team1Id"] = new SelectList(competition.Teams, "Id", "Name", game.Team1Id);
            ViewData["Team2Id"] = new SelectList(competition.Teams, "Id", "Name", game.Team2Id);
            return View(game);
        }


        // GET: Games/KnockoutEdit/5
        public async Task<IActionResult> KnockoutEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            var competition = _context.Competitions.Include(c => c.Teams).FirstOrDefault(c => c.Id == game.CompetitionId);
            if (competition == null)
            {
                return NotFound();
            }

            ViewData["CompetitionId"] = game.CompetitionId;
            ViewData["Team1Id"] = new SelectList(competition.Teams, "Id", "Name", game.Team1Id);
            ViewData["Team2Id"] = new SelectList(competition.Teams, "Id", "Name", game.Team2Id);
            return View(game);
        }

        // POST: Games/KnockoutEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KnockoutEdit(int id, [Bind("Id,Team1Id,Team2Id,Team1Score,Team2Score,CompetitionId,Date,Stadium")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (game.Team1Id == game.Team2Id)
            {
                ModelState.AddModelError(string.Empty, "A team cannot play against itself.");
            }

            var existingGame = _context.Games.FirstOrDefault(g =>
                g.Id != game.Id &&
                g.CompetitionId == game.CompetitionId &&
                ((g.Team1Id == game.Team1Id && g.Team2Id == game.Team2Id) ||
                 (g.Team1Id == game.Team2Id && g.Team2Id == game.Team1Id)));
            if (existingGame != null)
            {
                ModelState.AddModelError(string.Empty, "A game between these two teams already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    game.Team1Name = _context.Teams.Find(game.Team1Id)?.Name;
                    game.Team2Name = _context.Teams.Find(game.Team2Id)?.Name;
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("KnockoutGames", "Home", new { competitionId = game.CompetitionId });
            }

            var competition = _context.Competitions.Include(c => c.Teams).FirstOrDefault(c => c.Id == game.CompetitionId);
            if (competition == null)
            {
                return NotFound();
            }

            ViewData["CompetitionId"] = game.CompetitionId;
            ViewData["Team1Id"] = new SelectList(competition.Teams, "Id", "Name", game.Team1Id);
            ViewData["Team2Id"] = new SelectList(competition.Teams, "Id", "Name", game.Team2Id);
            return View(game);
        }

        // GET: Games/KnockoutDelete/5
        public async Task<IActionResult> KnockoutDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Competition)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/KnockoutDelete/5
        [HttpPost, ActionName("KnockoutDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KnockoutDeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return RedirectToAction("KnockoutGames", "Home", new { competitionId = game.CompetitionId });
        }

        public async Task<IActionResult> LeagueDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Competition)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/KnockoutDelete/5
        [HttpPost, ActionName("LeagueDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeagueDeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return RedirectToAction("LeagueGames", "Home", new { competitionId = game.CompetitionId });
        }




        private bool GameExists(int id)
        {
          return (_context.Games?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
