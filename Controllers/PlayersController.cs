﻿using System;
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
    public class PlayersController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public PlayersController(CompetitionManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ViewPlayers(int? id)
        {
            if (id == null)
            {
                var allPlayers = _context.Players.Include(p => p.Team);
                ViewBag.Teams = new SelectList(_context.Teams, "Id", "Name");
                return View(await allPlayers.ToListAsync());
            }
            else
            {
                var playersInTeam = _context.Players
                    .Where(p => p.TeamId == id)
                    .Include(p => p.Team);
                ViewBag.Teams = new SelectList(_context.Teams, "Id", "Name");
                return View(await playersInTeam.ToListAsync());
            }
        }

        // GET: Players
        public async Task<IActionResult> Index(int? teamId)
        {
            if (teamId == null)
            {
                var allPlayers = _context.Players.Include(p => p.Team);
                ViewBag.Teams = new SelectList(_context.Teams, "Id", "Name");
                return View(await allPlayers.ToListAsync());
            }
            else
            {
                var playersInTeam = _context.Players
                    .Where(p => p.TeamId == teamId)
                    .Include(p => p.Team);
                ViewBag.Teams = new SelectList(_context.Teams, "Id", "Name");
                return View(await playersInTeam.ToListAsync());
            }
        }


        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }
            var player = await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }
            return View(player);
        }

        // GET: Players/Create
        public IActionResult Create()
        {
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Age,TeamId,PictureFile")] Player player)
        {
            if (ModelState.IsValid)
            {
                // Get the uploaded file
                var pictureFile = player.PictureFile;
                if (pictureFile != null && pictureFile.Length > 0)
                {
                    // Read the file data into a byte array
                    using (var stream = new MemoryStream())
                    {
                        await pictureFile.CopyToAsync(stream);
                        player.Picture = stream.ToArray();
                    }
                }

                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = player.TeamId });
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", player.TeamId);
            return View(player);
        }

        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name");
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Age,TeamId,PictureFile")] Player player)
        {
            if (id != player.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the current player from the database
                    var currentPlayer = await _context.Players.FindAsync(id);

                    // Get the uploaded file
                    var pictureFile = player.PictureFile;
                    if (pictureFile != null && pictureFile.Length > 0)
                    {
                        // Read the file data into a byte array
                        using (var stream = new MemoryStream())
                        {
                            await pictureFile.CopyToAsync(stream);
                            currentPlayer.Picture = stream.ToArray();
                        }
                    }

                    // Update the other properties of the current player
                    currentPlayer.FirstName = player.FirstName;
                    currentPlayer.LastName = player.LastName;
                    currentPlayer.Age = player.Age;
                    currentPlayer.TeamId = player.TeamId;

                    _context.Update(currentPlayer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = player.TeamId });
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", player.TeamId);
            return View(player);
        }




        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Players == null)
            {
                return Problem("Entity set 'CompetitionManagementContext.Players'  is null.");
            }
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = player.TeamId });
        }

        private bool PlayerExists(int id)
        {
          return (_context.Players?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
