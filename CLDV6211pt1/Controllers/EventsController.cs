using Microsoft.AspNetCore.Mvc;
using CLDV6211pt1.Models;
using EventEase.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CLDV6211pt1.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string eventType, string searchName, DateTime? eventDate)
        {
            var query = _context.Events
                .Include(e => e.Venue)
                .AsQueryable();

            // Filter by Event Type
            if (!string.IsNullOrEmpty(eventType))
            {
                query = query.Where(e => e.EventType == eventType);
            }

            // Filter by Event Name
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(e => e.EventName.Contains(searchName));
            }

            // Filter by Specific Date
            if (eventDate.HasValue)
            {
                query = query.Where(e => e.EventDate.Date == eventDate.Value.Date);
            }

            var eventTypes = await _context.Events
                .Select(e => e.EventType)
                .Distinct()
                .ToListAsync();

            ViewBag.EventTypes = new SelectList(eventTypes);
            ViewBag.CurrentSearch = searchName;
            ViewBag.CurrentDate = eventDate?.ToString("yyyy-MM-dd"); // Format for date input value

            var events = await query.ToListAsync();
            return View(events);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Events evnt)
        {
            if (ModelState.IsValid)
            {
                _context.Events.Add(evnt);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(evnt);
        }

        public IActionResult Details(int id)
        {
            var evnt = _context.Events.Find(id);
            if (evnt == null) return NotFound();
            return View(evnt);
        }

        public IActionResult Delete(int id)
        {
            var evnt = _context.Events.Find(id);
            if (evnt != null)
            {
                _context.Events.Remove(evnt);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Events/Edit/5
        public IActionResult Edit(int id)
        {
            var evnt = _context.Events.Find(id);
            if (evnt == null)
            {
                return NotFound();
            }
            return View(evnt);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("EventID,EventName,EventDate,VenueID,EventType,Description,TicketPrice")] Events evnt)
        {
            if (id != evnt.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evnt);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(evnt.EventID))
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
            return View(evnt);
        }

        // Check if the event exists
        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
    }
}
