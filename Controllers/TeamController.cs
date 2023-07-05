using CompetitionManagment.Data;
using CompetitionManagment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagment.Controllers
{
    public class TeamController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public TeamController(CompetitionManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Teams.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Team team)
        {
            _context.Teams.Add(team);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
