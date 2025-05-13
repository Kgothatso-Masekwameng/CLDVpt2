using Microsoft.AspNetCore.Mvc;
using CLDV6211pt1.Models;
using EventEase.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CLDV6211pt1.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var bookingsQuery = _context.Bookings
                .Include(b => b.Events)
               
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookingsQuery = bookingsQuery.Where(b => b.Events.EventName.Contains(searchString));
            }

            var bookings = await bookingsQuery.ToListAsync();
            return View(bookings);
        }

        [HttpGet]  // Optional but clear
        public IActionResult Create()
        {
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName");
            return View();
        }

        // GET: Bookings/Create
        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bookings booking)
        {
            // Check if the selected EventID exists in the Events table
            var eventDetails = await _context.Events
                .Include(e => e.Venue) // Only if your Event model includes navigation property to Venue
                .FirstOrDefaultAsync(e => e.EventID == booking.EventID);

            if (eventDetails == null)
            {
                ModelState.AddModelError("EventID", "The selected Event does not exist.");
            }
            else
            {
                // Get event date and venue ID
                var eventDate = eventDetails.EventDate;
                var venueId = eventDetails.VenueID;

                // Check if another booking already exists for the same venue on the same date
                var doubleBooking = await _context.Bookings
                    .Include(b => b.Events)
                    .AnyAsync(b => b.Events.VenueID == venueId && b.Events.EventDate == eventDate);

                if (doubleBooking)
                {
                    ModelState.AddModelError("", "This venue is already booked on the selected date.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdown in case of error
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            return View(booking);
        }


        public IActionResult Details(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        public IActionResult Delete(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings/Edit/5
        public IActionResult Edit(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }

            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,CustomerID,EventID,BookingDate,SeatsBooked,BookingStatus")] Bookings booking)
        {
            if (id != booking.BookingID)
            {
                return NotFound();
            }

            // Check if the event exists
            var eventDetails = await _context.Events
                .Include(e => e.Venue) // Only if your Event model includes Venue as a navigation property
                .FirstOrDefaultAsync(e => e.EventID == booking.EventID);

            if (eventDetails == null)
            {
                ModelState.AddModelError("EventID", "The selected Event does not exist.");
            }
            else
            {
                var eventDate = eventDetails.EventDate;
                var venueId = eventDetails.VenueID;

                // Check for double booking (excluding the current booking being edited)
                var doubleBooking = await _context.Bookings
                    .Include(b => b.Events)
                    .AnyAsync(b =>
                        b.BookingID != booking.BookingID && // Exclude the current booking
                        b.Events.VenueID == venueId &&
                        b.Events.EventDate == eventDate);

                if (doubleBooking)
                {
                    ModelState.AddModelError("", "This venue is already booked on the selected date.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
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

            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            return View(booking);
        }


        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(b => b.BookingID == id);
        }
    }
}
