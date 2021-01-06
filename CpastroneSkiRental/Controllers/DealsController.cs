using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CpastroneSkiRental.Data;
using CpastroneSkiRental.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace CpastroneSkiRental.Controllers
{
    [Authorize]
    public class DealsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;

        public DealsController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        /// <summary>
        /// Method that list all available deal bundle items
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="searchString"></param>
        /// <param name="currentFilter"></param>
        /// <param name="pageNumber"></param>
        /// <param name="dealId"></param>
        /// <returns>List of deal items</returns>
        [AllowAnonymous]
        // GET: Deals
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, string dealId)
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
            ViewData["DealId"] = categoryList;

            ViewData["CurrentSort"] = "sortOrder";
            ViewData["TypeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "description_desc" : "";
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
            var deals = (from d in _context.Deal
                        select d).Where(d=>d.ItemStatus != "Rented").Where(d => d.ItemStatus != "Reparing");

            if (!String.IsNullOrEmpty(searchString))
            {
                deals = deals.Where(d => (d.ItemDescription.Contains(searchString)));
            }
            else if (!String.IsNullOrEmpty(dealId))
            {
              
                deals = deals.Where(i => i.ItemDescription.Contains(dealId));
            }
                ViewData["Deal"] = dealId;
                switch (sortOrder)
            {
                case "description_desc":
                    deals = deals.OrderByDescending(d => d.ItemDescription);
                    break;
                case "price":
                    deals = deals.OrderBy(d => d.Price);
                    break;
                case "price_desc":
                    deals = deals.OrderByDescending(d => d.Price);
                    break;
                default:
                    deals = deals.OrderBy(d=>d.RentType);
                    break;
            }

            int pageSize = 15;

          
            return View(await PaginatedList<Deal>.CreateAsync(deals.AsNoTracking(), pageNumber ?? 1, pageSize));
            }
            catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// View details about the deal items
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Details of deal items</returns>
        [Authorize(Roles = "Admin")]
        // GET: Deals/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var deal = await _context.Deal
                    .FirstOrDefaultAsync(m => m.DealId == id);
                if (deal == null)
                {
                    return NotFound();
                }

                return View(deal);
            }
            catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// Get method for creating deal items
        /// </summary>
        /// <returns> View</returns>
        // GET: Deals/Create
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
        /// Create method for creating new deal bundle
        /// </summary>
        /// <param name="deal"></param>
        /// <returns>New deal bundle</returns>
        [Authorize(Roles = "Admin")]
        // POST: Deals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Deal deal)
        {
            try
            {

                if (!ModelState.IsValid)
                {

                    return View();
                }

                _context.Add(deal);
                await _context.SaveChangesAsync();
                string webRoot = _environment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var itemDb = _context.Deal.Find(deal.DealId);

                if (files.Count != 0)
                {
                    var uploads = Path.Combine(webRoot, @"img");
                    var ext = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(uploads, deal.DealId + ext), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    itemDb.ItemImage = deal.DealId + ext;
                }
                else
                {
                    var uploads = Path.Combine(webRoot, @"img" + @"/" + "default_img.png");
                    System.IO.File.Copy(uploads, webRoot + @"/" + @"img" + @"/" + deal.DealId + ".png");
                    itemDb.ItemImage = @"/" + @"img" + @"/" + deal.DealId + ".png";
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Method that changes deal item status to reaparing when deal items are on repair
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Updated staus of the deal item</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendToRepair(Guid id)
        {
            var itemUpdate = _context.Deal.Find(id);
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
        /// <summary>
        /// Method that changes deal item status to reaparing when deal items are rented
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Updated staus of the deal item</returns>
        public async Task<IActionResult> EditDeal(Guid id)
        {
            var DealUpdate = _context.Deal.Find(id);
            if (DealUpdate == null)
            {
                return NotFound();
            }
            else
            {
                var status = "Rented";
                DealUpdate.ItemStatus = status;
                _context.Attach(DealUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        /// <summary>
        /// Get method for editing deal
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deal item to edit</returns>
        [Authorize(Roles = "Admin")]
        // GET: Deals/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var deal = await _context.Deal.FindAsync(id);
            if (deal == null)
            {
                return NotFound();
            }
            return View(deal);
        }

        /// <summary>
        /// Post method to Edit deal item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deal"></param>
        /// <returns>Edited item</returns>
        // POST: Deals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, Deal deal)
        {
            try
            {
                if (id != deal.DealId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(deal);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!DealExists(deal.DealId))
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
                return View(deal);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }


        /// <summary>
        /// Method to update deal item status to be available for rent
        /// </summary>
        /// <param name="id"></param>
        /// <returns>if sucessful deal items will be available for rent again</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EndRent(Guid id)
        {


            var itemUpdate = _context.Deal.Find(id);
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

        /// <summary>
        /// Dispplay list of deal items
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns>List of deal items</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DealsDisplay(int? pageNumber)
        {
            try
            {
                var items = (from i in _context.Deal
                             select i).Where(i => i.ItemStatus != "Rented").Where(i => i.ItemStatus != "Reparing");
                int pageSize = 15;

                return View((await PaginatedList<Deal>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize)));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// Get method for deal item to delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns> deal item to delete</returns>
        [Authorize(Roles = "Admin")]
        // GET: Deals/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var deal = await _context.Deal
                    .FirstOrDefaultAsync(m => m.DealId == id);
                if (deal == null)
                {
                    return NotFound();
                }

                return View(deal);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Confirm deletion for deal item
        /// </summary>
        /// <param name="id"></param>
        /// <returns> if sucessful deal item will be deleted</returns>
        [Authorize(Roles = "Admin")]
        // POST: Deals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var deal = await _context.Deal.FindAsync(id);
                _context.Deal.Remove(deal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// Checks if deal item exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns> deal item</returns>
        private bool DealExists(Guid id)
        {
            return _context.Deal.Any(e => e.DealId == id);
        }
    }
}
