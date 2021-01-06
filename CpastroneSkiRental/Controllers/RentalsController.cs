using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapstoneISki.Data;
using CpastroneSkiRental.Data;
using Microsoft.AspNetCore.Identity;
using CpastroneSkiRental.Models;
using Microsoft.AspNetCore.Authorization;

namespace CpastroneSkiRental.Controllers
{
    [Authorize]
    public class RentalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<WebsiteUser> _userManager;
       

        public RentalsController(ApplicationDbContext context, UserManager<WebsiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        /// <summary>
        /// List all rented items for administrative use
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns>Listed rented items</returns>
        [Authorize(Roles = "Admin")]
        // GET: Rentals
        public async Task<IActionResult> Index(int? pageNumber)
        {
            try
            {

                int pageSize = 15;
                var rental = (from i in _context.Rental select i).OrderBy(i => i.RentalReturnDate);

                return View(await PaginatedList<Rental>.CreateAsync(rental.AsNoTracking(), pageNumber ?? 1, pageSize));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Details about rented items
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        // GET: Rentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var rental = await _context.Rental
                    .FirstOrDefaultAsync(m => m.ProductId == id);
                if (rental == null)
                {
                    return NotFound();
                }

                return View(rental);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }

        }

        /// <summary>
        /// Get method for creating rental items rental order
        /// </summary>
        /// <returns></returns>
        // GET: Rentals/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Get method for creating bundle items rental order
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateBundle()
        {
            return View();
        }
        /// <summary>
        /// Post method for created rental order
        /// </summary>
        /// <param name="rental"></param>
        /// <returns></returns>
        // POST: Rentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("ItemId,DealId,UserId,UserName,ItemDescription,RentalStartDate,RentalReturnDate,RentalReturnDate,DayesRented,ItemStatus")]Rental rental)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(rental);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// Create rental order when payment was succesful
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="itemDescription"></param>
        /// <param name="startRent"></param>
        /// <param name="endRent"></param>
        /// <param name="totalDays"></param>
        /// <returns>New rented item</returns>
        public async Task<IActionResult> CreateInBackground(int id, string userId, string userName,
           string itemDescription, DateTime startRent, DateTime endRent, int totalDays)
        {
            try
            {
                var viewModel = new Rental
                {

                    UserId = userId,
                    ItemId = id,
                    ItemDescription = itemDescription,
                    UserName = userName,
                    RentalStartDate = startRent,
                    RentalReturnDate = endRent,
                    DayesRented = totalDays,
                    ItemStatus = "Rented"


                };
                if (totalDays > 0)
                {

                    _context.Add(viewModel);
                }

                await _context.SaveChangesAsync();
                return View(viewModel);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }

        }

        /// <summary>
        /// Create bundle order rental if payment was succesful
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="itemDescription"></param>
        /// <param name="startRent"></param>
        /// <param name="endRent"></param>
        /// <param name="totalDays"></param>
        /// <returns>New rented bundle</returns>
        public async Task<IActionResult> CreateBundleInBackground(Guid id, string userId, string userName,
        string itemDescription, DateTime startRent, DateTime endRent, int totalDays)
        {
            try
            {
                var rental = new Rental();
                if (id != Guid.Empty)
                {
                    var dealFinder = _context.Deal.Find(id);
                    var dealDescription = dealFinder.ItemDescription;

                    rental = new Rental
                    {

                        UserId = userId,
                        DealId = id,
                        ItemDescription = dealDescription,
                        UserName = userName,
                        RentalStartDate = startRent,
                        RentalReturnDate = endRent,
                        DayesRented = totalDays,
                        ItemStatus = "Rented"


                    };

                }
                _context.Add(rental);

                await _context.SaveChangesAsync();
                return View(rental);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }

        }

        /// <summary>
        /// Get method Edit rental order if needed(not used autogenerated)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Rentals/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var rental = await _context.Rental.FindAsync(id);
                if (rental == null)
                {
                    return NotFound();
                }
                return View(rental);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Edit rental order if needed(not used autogenerated)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rental"></param>
        /// <returns></returns>
        // POST: Rentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Rental rental)
        {
            try
            {
                if (id != rental.ProductId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(rental);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RentalExists(rental.ProductId))
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
                return View(rental);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        /// <summary>
        /// Delete rental order (used only if item was returned and checked that it was not broken)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        // GET: Rentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var rental = await _context.Rental
                    .FirstOrDefaultAsync(m => m.ProductId == id);
                if (rental == null)
                {
                    return NotFound();
                }

                return View(rental);
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Confirm deletion of rental order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var rental = await _context.Rental.FindAsync(id);
                _context.Rental.Remove(rental);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
       
        private bool RentalExists(int id)
        {
            return _context.Rental.Any(e => e.ProductId == id);
        }
    }
}
