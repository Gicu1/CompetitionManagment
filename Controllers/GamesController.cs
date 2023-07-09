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

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Id");
            ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Id");
            ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Id");
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Team1Id,Team2Id,Team1Score,Team2Score,CompetitionId,Date,Stadium,Team1Name,Team2Name")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Id", game.CompetitionId);
            ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Id", game.Team1Id);
            ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Id", game.Team2Id);
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Id", game.CompetitionId);
            ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Id", game.Team1Id);
            ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Id", game.Team2Id);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Team1Id,Team2Id,Team1Score,Team2Score,CompetitionId,Date,Stadium,Team1Name,Team2Name")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Id", game.CompetitionId);
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

        private bool GameExists(int id)
        {
          return (_context.Games?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
