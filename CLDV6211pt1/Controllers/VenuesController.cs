using Microsoft.AspNetCore.Mvc;
using CLDV6211pt1.Models;
using EventEase.Data;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;

namespace CLDV6211pt1.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VenuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Azure Blob Upload Helper
        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=cldvmason;AccountKey=BBLSqqFu1GynHwGAR5uU9ALXoxfCD25pW+gav31halX9GFv9bLCczKiYb3Awao3wgOoO4EbfpFql+AStN8MtGQ==;EndpointSuffix=core.windows.net";
            var containerName = "cldvmason";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }

        // GET: Venues
        public IActionResult Index()
        {
            var venues = _context.Venues.ToList();
            return View(venues);
        }

        // GET: Venues/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venues venue, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    venue.ImageURL = await UploadImageToBlobAsync(imageFile);
                }

                _context.Venues.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Venues/Details/5
        public IActionResult Details(int id)
        {
            var venue = _context.Venues.Find(id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Delete/5
        public IActionResult Delete(int id)
        {
            var venue = _context.Venues
                .Include(v => v.Events) // assuming the relationship exists
                .FirstOrDefault(v => v.VenueID == id);

            if (venue == null)
            {
                return NotFound();
            }

            if (_context.Events.Any(e => e.VenueID == id))
            {
                TempData["Error"] = "Cannot delete this venue because it has related events.";
                return RedirectToAction(nameof(Index));
            }

            _context.Venues.Remove(venue);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Venues/Edit/5
        public IActionResult Edit(int id)
        {
            var venue = _context.Venues.Find(id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueID, VenueName, Address, City, Country, Capacity, Description, ImageURL")] Venues venue, IFormFile imageFile)
        {
            if (id != venue.VenueID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        venue.ImageURL = await UploadImageToBlobAsync(imageFile);
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueID))
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

            return View(venue);
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(v => v.VenueID == id);
        }
    }
}
