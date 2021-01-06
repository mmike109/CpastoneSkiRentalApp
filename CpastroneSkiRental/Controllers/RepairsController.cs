using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CpastroneSkiRental.Data;
using CpastroneSkiRental.Models;
using Microsoft.AspNetCore.Authorization;

namespace CpastroneSkiRental.Controllers
{
    [Authorize]
    public class RepairsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RepairsController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Listing all the items currently on repair
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns>List of items on reapair</returns>
        [Authorize(Roles = "Admin")]
        // GET: Repairs
        public async Task<IActionResult> Index(int? pageNumber)
        {
            try
            {
                int pageSize = 15;
                var repair = (from i in _context.Repair select i).OrderBy(i => i.RepairId);
                return View(await PaginatedList<Repair>.CreateAsync(repair.AsNoTracking(), pageNumber ?? 1, pageSize));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Details about items on repair
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Items on repair details</returns>
        [Authorize(Roles = "Admin")]
        // GET: Repairs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var repair = await _context.Repair
                    .FirstOrDefaultAsync(m => m.RepairId == id);
                if (repair == null)
                {
                    return NotFound();
                }

                return View(repair);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// Get create method
        /// </summary>
        /// <returns></returns>
        // GET: Repairs/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Post create method to create new repair db entry
        /// </summary>
        /// <param name="repair"></param>
        /// <returns>repair item listing</returns>
        // POST: Repairs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RepairId,ItemId,DealId,ProductId,ItemName,ItemStatus,Condition")] Repair repair)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(repair);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(repair);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// create method will create a repair item listing in the database works for both bundle and single items
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dealId"></param>
        /// <param name="prodId"></param>
        /// <returns>new repair lisitng</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRepair(int id,Guid dealId,int prodId)
        {
            try
            {
                var repair = new Repair();

                if (dealId != Guid.Empty)
                {
                    var dealFinder = _context.Deal.Find(dealId);
                    var DealDescription = dealFinder.ItemDescription;


                    repair = new Repair
                    {
                        ItemId = id,
                        DealId = dealId,
                        ProductId = prodId,
                        ItemName = DealDescription,
                        Condition = "Repairing"
                    };

                }
                else
                {
                    var itemFinder = _context.Item.Find(id);
                    var itemDescription = itemFinder.Maker + " " + itemFinder.ItemType;
                    repair = new Repair
                    {
                        ItemId = id,
                        DealId = dealId,
                        ProductId = prodId,
                        ItemName = itemDescription,
                        Condition = "Repairing"
                    };
                }

                _context.Add(repair);


                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Get method for editing repair lisitng 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>edit repair</returns>
        [Authorize(Roles = "Admin")]
        // GET: Repairs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repair = await _context.Repair.FindAsync(id);
            if (repair == null)
            {
                return NotFound();
            }
            return View(repair);
        }

        /// <summary>
        /// Edit repair listing if needed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="repair"></param>
        /// <returns>edit repair</returns>
        // POST: Repairs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RepairId,ItemId,DealId,ProductId,ItemName,ItemStatus,Condition")] Repair repair)
        {
            try
            {
                if (id != repair.RepairId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(repair);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RepairExists(repair.RepairId))
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
                return View(repair);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// Remove bundle items from repair list when its been repaird
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBundle(int id)
        {
            try
            {
                var repair = _context.Repair.Find(id);
                _context.Repair.Remove(repair);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// End the repair of bundle items  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EndBundleRepair(int id)
        {
            try
            {
                var repair = _context.Repair.Find(id);
                ViewBag.ItemId = repair.ItemId;
                ViewBag.DealId = repair.DealId;
                ViewBag.ItemName = repair.ItemName;
                ViewBag.Condition = repair.Condition;
                ViewBag.RepairId = id;
                return View(repair);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Delete repair listing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        // GET: Repairs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var repair = await _context.Repair
                    .FirstOrDefaultAsync(m => m.RepairId == id);
                if (repair == null)
                {
                    return NotFound();
                }

                return View(repair);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Confirm repair deletion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        // POST: Repairs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var repair = await _context.Repair.FindAsync(id);
                _context.Repair.Remove(repair);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
       
        private bool RepairExists(int id)
        {
            return _context.Repair.Any(e => e.RepairId == id);
        }
    }
}
