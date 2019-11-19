using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThAmCo.Events.Data;
using ThAmCo.Events.Models;

namespace ThAmCo.Events.Controllers
{
    public class EventController : Controller
    {
        private readonly EventsDbContext _context;

        public EventController(EventsDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(s => s.Staffing)
                .ThenInclude(s => s.Staff)
                .Include(g => g.Bookings)
                .ThenInclude(c => c.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            var venues = new List<EventVenueDto>().AsEnumerable();
            String eventType = @event.TypeId;
            DateTime beginDate = @event.Date;
            DateTime endDate = @event.Date.Add(@event.Duration.Value);

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:23652/");

            HttpResponseMessage response = await client.GetAsync("api/Availability?eventType=" + eventType
                + "&beginDate=" + beginDate.ToString("yyyy-MM-dd") +
                "&endDate=" + endDate.ToString("yyyy-MM-dd"));

            EventVenueDto venue = null;

            if (response.IsSuccessStatusCode)
            {
                venues = await response.Content.ReadAsAsync<IEnumerable<EventVenueDto>>();
                if (venues.Count() == 0)
                {
                    ViewBag.NullVenues = true;
                    Debug.WriteLine("No items");
                }
                else
                {
                    bool found = false;
                    foreach(var ven in venues)
                    {
                        if(ven.Code == @event.VenueCode)
                        {
                            venue = ven;
                            found = true;
                            break;
                        }
                    }
                    if(!found)
                    {
                        ViewBag.NullVenues = true;
                        Debug.WriteLine("No items");
                    }
                }
            }
            else
            {
                return BadRequest();
            }

            if (@event == null)
            {
                return NotFound();
            }

            var model = new VenueAndEventModel();
            model.Event = @event;
            model.Venue = venue;
            return View(model);
        }

        public async Task<IActionResult> StaffingDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @staffing = await _context.Staffing
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (@staffing == null)
            {
                ViewBag.NoStaff = true;
                staffing = new Staffing();
                staffing.EventId = id.Value;
                staffing.Event = await _context.
                    Events.FirstOrDefaultAsync(e => e.Id == id);
                Debug.WriteLine("No staff");
            }

            ViewBag.Staffing = true;

            return View("_StaffList", staffing);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Date,Duration,TypeId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Date,Duration,TypeId")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var model = await _context.Events.FindAsync(id);
                    model.Title = @event.Title;
                    model.Duration = @event.Duration;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
        public async Task<IActionResult> ChooseVenue(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curEvent = await _context.Events.FirstOrDefaultAsync(m => m.Id == id);
                                         //.Where(e => e.isActive)
            if (curEvent == null)
            {
                return BadRequest();
            }

            String eventType = curEvent.TypeId;
            DateTime beginDate = curEvent.Date;
            DateTime endDate = curEvent.Date.Add(curEvent.Duration.Value);

            var availableVenues = new List<ChooseVenueDto>().AsEnumerable();

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:23652/");

            HttpResponseMessage response = await client.GetAsync("api/Availability?eventType=" + eventType
                + "&beginDate=" + beginDate.ToString("yyyy-MM-dd") +
                "&endDate=" + endDate.ToString("yyyy-MM-dd"));

            if (response.IsSuccessStatusCode)
            {
                availableVenues = await response.Content.ReadAsAsync<IEnumerable<ChooseVenueDto>>();

                if (availableVenues.Count() == 0)
                {
                    ViewBag.NullVenues = true;
                    Debug.WriteLine("No items");
                }
            }
            else
            {
                Debug.WriteLine("Received a bad response from the web service.");
                return BadRequest();
            }

            ViewData["EventTitle"] = curEvent.Title;
            ViewData["EventId"] = curEvent.Id;
            ViewData["EventDate"] = curEvent.Date;

            return View(availableVenues);
        }

        public async Task<IActionResult> ReserveVenue(int EventId, DateTime EventDate, string VenueCode)
        {
            if (EventDate == null || VenueCode == null)
            {
                return NotFound();
            }

            var curEvent = await _context.Events
                                         .FirstOrDefaultAsync(m => m.Id == EventId);

            if (curEvent == null)
            {
                return BadRequest();
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:23652/");

            var reservation = new ReservationPostDto
            {
                EventDate = EventDate,
                VenueCode = VenueCode,
            };

            if (!String.IsNullOrEmpty(curEvent.VenueCode))
            {
                var reference = curEvent.VenueCode + EventDate.ToString("yyyy-MM-dd");
                await client.DeleteAsync("api/reservations/" + reference);
            }

            curEvent.VenueCode = reservation.VenueCode;
            _context.Update(curEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));      
        }
    }
}
