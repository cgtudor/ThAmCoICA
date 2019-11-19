using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThAmCo.Events.Data;

namespace ThAmCo.Events.Controllers
{
    public class StaffingController : Controller
    {
        private readonly EventsDbContext _context;

        public StaffingController(EventsDbContext context)
        {
            _context = context;
        }

        // GET: Staffing
        public async Task<IActionResult> Index()
        {
            var eventsDbContext = _context.Staffing.Include(s => s.Event).Include(s => s.Staff);
            return View(await eventsDbContext.ToListAsync());
        }

        // GET: Staffing
        public async Task<IActionResult> EventIndex(int? id)
        {
            var eventsDbContext = _context.Staffing.Include(s => s.Event).Include(s => s.Staff).Where(e => e.EventId == id);
            return View(await eventsDbContext.ToListAsync());
        }

        // GET: Staffing/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffing = await _context.Staffing
                .Include(s => s.Event)
                .Include(s => s.Staff)
                .FirstOrDefaultAsync(m => m.StaffingId == id);
            if (staffing == null)
            {
                return NotFound();
            }

            return View(staffing);
        }

        // GET: Staffing/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Title");
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "FirstName");
            return View();
        }

        // POST: Staffing/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StaffId,EventId")] Staffing staffing)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staffing);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", nameof(Event));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Title", staffing.EventId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "FirstName", staffing.StaffId);
            return RedirectToAction("Index", nameof(Event));
        }

        // GET: Staffing/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffing = await _context.Staffing.FirstOrDefaultAsync(i => i.StaffingId == id);
            if (staffing == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Title", staffing.EventId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "FirstName", staffing.StaffId);
            return View(staffing);
        }

        // POST: Staffing/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StaffId,EventId")] Staffing staffing)
        {
            if (id != staffing.StaffId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staffing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffingExists(staffing.StaffId))
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
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Title", staffing.EventId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "FirstName", staffing.StaffId);
            return View(staffing);
        }

        // GET: Staffing/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffing = await _context.Staffing
                .Include(s => s.Event)
                .Include(s => s.Staff)
                .FirstOrDefaultAsync(m => m.StaffingId == id);
            if (staffing == null)
            {
                return NotFound();
            }

            return View(staffing);
        }

        // POST: Staffing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staffing = await _context.Staffing.FirstOrDefaultAsync(m => m.StaffingId == id);
            _context.Staffing.Remove(staffing);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", nameof(Event));
        }

        private bool StaffingExists(int id)
        {
            return _context.Staffing.Any(e => e.StaffingId == id);
        }
    }
}
