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
    public class CompetitionsController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public CompetitionsController(CompetitionManagementContext context)
        {
            _context = context;
        }

        // GET: Competitions
        public async Task<IActionResult> Index()
        {
            var competitionManagementContext = _context.Competitions.Include(c => c.CompetitionTypeNavigation);
            return View(await competitionManagementContext.ToListAsync());
        }

        // GET: Competitions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Competitions == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions
                .Include(c => c.CompetitionTypeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }

        // GET: Competitions/Create
        public IActionResult Create()
        {
            ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate,Location,CompetitionType,WinnerName")] Competition competition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(competition);
                if (competition.StartDate > competition.EndDate)
                {
                    ModelState.AddModelError("EndDate", "End Date must be after Start Date.");
                    ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name", competition.CompetitionType);
                    return View(competition);
                }
                var competitionInDb = _context.Competitions.FirstOrDefault(c => c.Name == competition.Name);
                if (competitionInDb != null)
                {
                    ModelState.AddModelError("Name", "The competition name already exists.");
                    ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name", competition.CompetitionType);
                    return View(competition);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                var modelStates = ModelState.Values.ToList();
                foreach (var modelState in modelStates)
                {
                    foreach (var error in modelState.Errors)
                    {
                        // Log or display the error message
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }



            ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name", competition.CompetitionType);
            return View(competition);
        }


        // GET: Competitions/Edit/5
        public async Task<IActionResult> Edit(int? competitionId)
        {
            Competition? competition = await _context.Competitions
                .Include(c => c.Teams)
                .FirstOrDefaultAsync(c => c.Id == competitionId);
            if (competitionId == null || _context.Competitions == null || competition == null)
            {
                return NotFound();
            }

            ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name");
            return View(competition);
        }

        // POST: Competitions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate,Location,CompetitionType,WinnerName")] Competition competition)
        {
            if (id != competition.Id)
            {
                return NotFound();
            }
            if (competition.StartDate > competition.EndDate)
            {
                ModelState.AddModelError("EndDate", "End Date must be after Start Date.");
                ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name", competition.CompetitionType);
                return View(competition);
            }
            var competitionInDb = _context.Competitions.AsNoTracking().FirstOrDefault(c => c.Name == competition.Name);
            if (competitionInDb != null && competitionInDb.Id != competition.Id)
            {
                ModelState.AddModelError("Name", "The competition name already exists.");
                ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name", competition.CompetitionType);
                return View(competition);
            }
            if (ModelState.IsValid)
            {
                _context.Update(competition);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name", competition.CompetitionType);
            return View(competition);
        }

        // GET: Competitions/Delete/5
        public async Task<IActionResult> Delete(int? competitionId)
        {
            Competition? competition = await _context.Competitions
                .Include(c => c.CompetitionTypeNavigation)
                .FirstOrDefaultAsync(c => c.Id == competitionId);
            if (competitionId == null || _context.Competitions == null)
            {
                return NotFound();
            }

//            var competition = await _context.Competitions
//                .Include(c => c.CompetitionTypeNavigation)
//                .FirstOrDefaultAsync(m => m.Id == id);
            if (competition == null)
            {
                return NotFound();
            }
            return View(competition);
        }

        // POST: Competitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Competitions == null)
            {
                return Problem("Entity set 'CompetitionManagementContext.Competitions'  is null.");
            }
            var competition = await _context.Competitions.FindAsync(id);
            if (competition != null)
            {
                var games = _context.Games.Where(g => g.CompetitionId == id);
                _context.Games.RemoveRange(games);
                _context.Competitions.Remove(competition);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }


        private bool CompetitionExists(int id)
        {
          return (_context.Competitions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
