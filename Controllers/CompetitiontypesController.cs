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
    public class CompetitiontypesController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public CompetitiontypesController(CompetitionManagementContext context)
        {
            _context = context;
        }

        // GET: Competitiontypes
        public async Task<IActionResult> Index()
        {
              return _context.Competitiontypes != null ? 
                          View(await _context.Competitiontypes.ToListAsync()) :
                          Problem("Entity set 'CompetitionManagementContext.Competitiontypes'  is null.");
        }

        // GET: Competitiontypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Competitiontypes == null)
            {
                return NotFound();
            }

            var competitiontype = await _context.Competitiontypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (competitiontype == null)
            {
                return NotFound();
            }

            return View(competitiontype);
        }

        // GET: Competitiontypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Competitiontypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Competitiontype competitiontype)
        {
            if (ModelState.IsValid)
            {
                _context.Add(competitiontype);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(competitiontype);
        }

        // GET: Competitiontypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Competitiontypes == null)
            {
                return NotFound();
            }

            var competitiontype = await _context.Competitiontypes.FindAsync(id);
            if (competitiontype == null)
            {
                return NotFound();
            }
            return View(competitiontype);
        }

        // POST: Competitiontypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Competitiontype competitiontype)
        {
            if (id != competitiontype.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(competitiontype);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompetitiontypeExists(competitiontype.Id))
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
            return View(competitiontype);
        }

        // GET: Competitiontypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Competitiontypes == null)
            {
                return NotFound();
            }

            var competitiontype = await _context.Competitiontypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (competitiontype == null)
            {
                return NotFound();
            }

            return View(competitiontype);
        }

        // POST: Competitiontypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Competitiontypes == null)
            {
                return Problem("Entity set 'CompetitionManagementContext.Competitiontypes'  is null.");
            }
            var competitiontype = await _context.Competitiontypes.FindAsync(id);
            if (competitiontype != null)
            {
                _context.Competitiontypes.Remove(competitiontype);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompetitiontypeExists(int id)
        {
          return (_context.Competitiontypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
