using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapstoneISki.Data;
using CpastroneSkiRental.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using CpastroneSkiRental.PayPal;
using Microsoft.Extensions.Configuration;

namespace CpastroneSkiRental.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;
        public IConfiguration configuration { get; }
        public ItemsController(ApplicationDbContext context, IHostingEnvironment environment, IConfiguration _configuration)
        {
            _context = context;
            _environment = environment;
            configuration = _configuration;
        }
        /// <summary>
        /// Method that list all available items
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="searchString"></param>
        /// <param name="currentFilter"></param>
        /// <param name="pageNumber"></param>
        /// <param name="itemId"></param>
        /// <param name="data"></param>
        /// <returns>List all items</returns>
        [AllowAnonymous]
        // GET: Items
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, string itemId)
        {
            try
            {
                var categoryList = new SelectList(new[]
                    {

                    new { ID = 1, Name = "Skis" },
                    new { ID = 2, Name = "Poles" },
                    new { ID = 3, Name = "Boots" },
                    new { ID = 4, Name = "Helmet" },
                    new { ID = 5, Name = "Gloves" },
                },
                        "Name", "Name", "Skis");

                ViewData["ItemId"] = categoryList;

                ViewData["CurrentSort"] = "sortOrder";
                ViewData["TypeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "maker_desc" : "";
                ViewData["PriceSortParm"] = sortOrder == "price" ? "price_desc" : "price";
                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;
               
                var items = (from i in _context.Item
                             select i).Where(i => i.ItemStatus != "Rented").Where(i => i.ItemStatus != "Reparing");

                if (!String.IsNullOrEmpty(searchString))
                {
                    items = items.Where(i => (i.Maker.Contains(searchString) && i.ItemType.Contains(itemId)));
                }
                else if (!String.IsNullOrEmpty(itemId))
                {
                    items = items.Where(i => i.ItemType.Contains(itemId));
                }
                ViewData["Item"] = itemId;
                switch (sortOrder)
                {
                    case "maker_desc":
                        items = items.OrderByDescending(i => i.Maker);
                        break;
                    case "price":
                        items = items.OrderBy(i => i.Price);
                        break;
                    case "price_desc":
                        items = items.OrderByDescending(i => i.Price);
                        break;
                    default:
                        items = items.OrderBy(i => i.ItemType);
                        break;
                }

                int pageSize = 15;

                return View(await PaginatedList<Item>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// View details about the  items
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Details about items</returns>
        [Authorize(Roles = "Admin")]
        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var item = await _context.Item
                    .FirstOrDefaultAsync(m => m.ItemId == id);
                if (item == null)
                {
                    return NotFound();
                }

                return View(item);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// Get method for creating items
        /// </summary>
        /// <returns></returns>
        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Image uplodad property
        /// </summary>
        [BindProperty]
        public IFormFile Upload { get; set; }
        /// <summary>
        /// Create method for creating new products
        /// </summary>
        /// <param name="item"></param>
        /// <returns>New product db entry</returns>
        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Item item)
        {
            try
            {
                if (!ModelState.IsValid)
                {

                    return View();
                }
                _context.Add(item);
                await _context.SaveChangesAsync();
                string webRoot = _environment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var itemDb = _context.Item.Find(item.ItemId);


                if (files.Count != 0)
                {
                    var uploads = Path.Combine(webRoot, @"images");
                    var ext = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(uploads, item.ItemId + ext), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    itemDb.ItemImage = item.ItemId + ext;
                }
                else
                {
                    var uploads = Path.Combine(webRoot, @"images" + @"/" + "default_img.png");
                    System.IO.File.Copy(uploads, webRoot + @"/" + @"images" + @"/" + item.ItemId + ".png");
                    itemDb.ItemImage = @"/" + @"images" + @"/" + item.ItemId + ".png";
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }

        }

        /// <summary>
        /// Get method for editing items if needed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var item = await _context.Item.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                return View(item);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Update stasus of an item when it goes on rent 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //Edit status of an item when rent order has been placed.
        public async Task<IActionResult> EditItem( int id)
        {
            try
            {


            var itemUpdate = _context.Item.Find(id);
            if (itemUpdate == null)
            {
                return NotFound();
            }
            else
            {
                var status = "Rented";
                itemUpdate.ItemStatus = status;
                _context.Attach(itemUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            }
            catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// Updates status of item when it returns back from rental
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EndRent(int id)
        {
            try { 

            var itemUpdate = _context.Item.Find(id);
            if (itemUpdate == null)
            {
                return NotFound();
            }
            else
            {
                var status = "Available";
                itemUpdate.ItemStatus = status;
                _context.Attach(itemUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            }
            catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }

        }

        /// <summary>
        /// Sends to repair if returned product was found broken and changes the status to reparing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendToRepair(int id)
        {
            try
            {

            

            var itemUpdate = _context.Item.Find(id);
            if (itemUpdate == null)
            {
                return NotFound();
            }
            else
            {
                var status = "Reparing";
                itemUpdate.ItemStatus = status;
                _context.Attach(itemUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            }
            catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }

        }
        /// <summary>
        /// Post method for editing items if needed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,Maker,ItemType,ItemGender,ItemSize,ItemAltSize,Price,Level,ItemStatus,ItemImage")] Item item)
        {
            try
            {

                if (id != item.ItemId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    _context.Attach(item).State = EntityState.Modified;
                    try
                    {

                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ItemExists(item.ItemId))
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
                return View(item);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// Delete an item if no loger carried
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var item = await _context.Item
                    .FirstOrDefaultAsync(m => m.ItemId == id);
                if (item == null)
                {
                    return NotFound();
                }

                return View(item);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Confirm item deletion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var item = await _context.Item.FindAsync(id);
                _context.Item.Remove(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// Checks if item exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.ItemId == id);
        }

        /// <summary>
        /// Display listed items if not rented or repaired
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public async Task<IActionResult> ProductsDisplay(int? pageNumber)
        {
            try
            {
                var items = (from i in _context.Item
                             select i).Where(i => i.ItemStatus != "Rented").Where(i => i.ItemStatus != "Reparing");
                int pageSize = 15;
                return View((await PaginatedList<Item>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize)));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// Send item data and price to paypal API
        /// </summary>
        /// <param name="total"></param>
        /// <returns>Redirect to PayPal website</returns>
        public async Task<IActionResult> Checkout(double total)
        {
            try
            {
                var payPalApi = new PayPalAPI(configuration);
                string url = await payPalApi.GetRedirectURLToPayPal(total, "CAD");

                return Redirect(url);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

       

    }
}
