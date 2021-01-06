using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapstoneISki.Data;
using CpastroneSkiRental.Data;
using CpastroneSkiRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CpastroneSkiRental.Controllers
{
    [Authorize]
    public class JoinedController : Controller
    {

        private readonly ApplicationDbContext _context;
        private UserManager<WebsiteUser> _userManager;
       
        public JoinedController(ApplicationDbContext context, UserManager<WebsiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        /// <summary>
        /// List all items and bundle items of the store for administration use
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns>List of all products and deal items that are currenty listed in the store's database</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int? pageNumber)
        {
            try
            {
                var itemReprot = new List<JoinedClass>();

                int pageSize = 15;
                var leftJoin =
                        (from i in _context.Item
                         join r in _context.Rental
                             on i.ItemId equals r.ItemId into temp
                         from r in temp.DefaultIfEmpty()
                         select new
                         {
                             i.ItemId,
                             i.ItemStatus,
                             i.Maker,
                             i.ItemType,
                             r.RentalStartDate,
                             r.RentalReturnDate,
                             r.DayesRented,
                             r.UserName,
                             r.ProductId,
                             r.ItemDescription,
                             r.DealId


                         }
                    );
                var rightJoin =
                                    (from r in _context.Rental
                                     join i in _context.Item
                             on r.ItemId equals i.ItemId into temp
                                     from i in temp.DefaultIfEmpty()
                                     select new
                                     {
                                         r.ItemId,
                                         r.ItemStatus,
                                         i.Maker,
                                         i.ItemType,
                                         r.RentalStartDate,
                                         r.RentalReturnDate,
                                         r.DayesRented,
                                         r.UserName,
                                         r.ProductId,
                                         r.ItemDescription,
                                         r.DealId


                                     }
                                );

                var fullOutterJoin = leftJoin.Union(rightJoin);


                var leftJoinTable = (from d in fullOutterJoin
                                     join f in _context.Deal
                                     on d.DealId equals f.DealId into temp
                                     from f in temp.DefaultIfEmpty()
                                     select new
                                     {
                                         d.ItemId,
                                         d.Maker,
                                         d.ItemType,
                                         d.RentalStartDate,
                                         d.RentalReturnDate,
                                         d.DayesRented,
                                         d.UserName,
                                         d.ProductId,
                                         f.ItemDescription,
                                         d.DealId,
                                         f.ItemStatus

                                     }

                                );

                var rightJoinTable = (from f in _context.Deal
                                      join d in fullOutterJoin
                                  on f.DealId equals d.DealId into temp
                                      from d in temp.DefaultIfEmpty()
                                      select new
                                      {
                                          d.ItemId,
                                          d.Maker,
                                          d.ItemType,
                                          d.RentalStartDate,
                                          d.RentalReturnDate,
                                          d.DayesRented,
                                          d.UserName,
                                          d.ProductId,
                                          f.ItemDescription,
                                          f.DealId,
                                          f.ItemStatus
                                      }

                                 );
                var fullJoinTable = leftJoinTable.Union(rightJoinTable);
              
                var itemView = (from t in fullJoinTable
                                join f in fullOutterJoin
                               on new { t.ItemId, t.ProductId } equals new { f.ItemId, f.ProductId } into temp

                                from f in temp.DefaultIfEmpty()
                                select new JoinedClass
                                {
                                    ProductId = t.ProductId,
                                    ItemId = t.ItemId,
                                    DealId = t.DealId,
                                    ItemStatus = ((t.ItemStatus == null) ? f.ItemStatus : t.ItemStatus),
                                    ItemName = ((t.Maker == null) ? t.ItemDescription : t.Maker + " " + t.ItemType),
                                    RentalStartDate = t.RentalStartDate,
                                    RentalReturnDate = t.RentalReturnDate,
                                    UserName = t.UserName,
                                    DayesRented = t.DayesRented
                                });
    
                return View(await PaginatedList<JoinedClass>.CreateAsync(itemView.AsNoTracking(), pageNumber ?? 1, pageSize));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// List currently available products that are not rented or on reparir
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns>Available items</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AvilableItems(int? pageNumber)
        {
            try
            {

                int pageSize = 15;
                var leftJoin =
                        (from i in _context.Item
                         join r in _context.Rental
                             on i.ItemId equals r.ItemId into temp
                         from r in temp.DefaultIfEmpty()
                         select new
                         {
                             i.ItemId,
                             i.ItemStatus,
                             i.Maker,
                             i.ItemType,
                             r.RentalStartDate,
                             r.RentalReturnDate,
                             r.DayesRented,
                             r.UserName,
                             r.ProductId,
                             r.ItemDescription,
                             r.DealId


                         }
                    );
                var rightJoin =
                                    (from r in _context.Rental
                                     join i in _context.Item
                             on r.ItemId equals i.ItemId into temp
                                     from i in temp.DefaultIfEmpty()
                                     select new
                                     {
                                         r.ItemId,
                                         r.ItemStatus,
                                         i.Maker,
                                         i.ItemType,
                                         r.RentalStartDate,
                                         r.RentalReturnDate,
                                         r.DayesRented,
                                         r.UserName,
                                         r.ProductId,
                                         r.ItemDescription,
                                         r.DealId


                                     }
                                );

                var fullOutterJoin = leftJoin.Union(rightJoin);

                var leftJoinTable = (from d in fullOutterJoin
                                     join f in _context.Deal
                                     on d.DealId equals f.DealId into temp
                                     from f in temp.DefaultIfEmpty()
                                     select new
                                     {
                                         d.ItemId,
                                         d.Maker,
                                         d.ItemType,
                                         d.RentalStartDate,
                                         d.RentalReturnDate,
                                         d.DayesRented,
                                         d.UserName,
                                         d.ProductId,
                                         f.ItemDescription,
                                         d.DealId,
                                         f.ItemStatus

                                     }

                                  );

                var rightJoinTable = (from f in _context.Deal
                                      join d in fullOutterJoin
                                  on f.DealId equals d.DealId into temp
                                      from d in temp.DefaultIfEmpty()
                                      select new
                                      {
                                          d.ItemId,
                                          d.Maker,
                                          d.ItemType,
                                          d.RentalStartDate,
                                          d.RentalReturnDate,
                                          d.DayesRented,
                                          d.UserName,
                                          d.ProductId,
                                          f.ItemDescription,
                                          f.DealId,
                                          f.ItemStatus
                                      }

                                 );
                var fullJoinTable = leftJoinTable.Union(rightJoinTable);

                var itemView = (from t in fullJoinTable
                                join f in fullOutterJoin
                                  on t.ItemId equals f.ItemId into temp
                                from f in temp.DefaultIfEmpty()
                                select new JoinedClass
                                {
                                    ProductId = t.ProductId,
                                    ItemId = t.ItemId,
                                    DealId = t.DealId,
                                    ItemStatus = ((t.ItemStatus == null) ? f.ItemStatus : t.ItemStatus),
                                    ItemName = ((t.Maker == null) ? t.ItemDescription : t.Maker + " " + t.ItemType),
                                    RentalStartDate = t.RentalStartDate,
                                    RentalReturnDate = t.RentalReturnDate,
                                    UserName = t.UserName,
                                    DayesRented = t.DayesRented
                                });

                return View(await PaginatedList<JoinedClass>.CreateAsync(itemView.AsNoTracking(), pageNumber ?? 1, pageSize));
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
      
        /// <summary>
        /// Set up the order of an item
        /// </summary>
        /// <param name="id"></param>
        /// <returns>item order</returns>
        // GET: Rentals/Create rental order for items
      
        public IActionResult CreateOrder(int id)
        {
            try
            {
                ViewBag.Id = id;
                var itemFinder = _context.Item.Find(id);
                var itemDescription = itemFinder.Maker + " " + itemFinder.ItemType;


                ViewBag.Des = itemDescription;

                return View();
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Set up order of bundle items
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bundle items order</returns>
        // GET: Rentals/Create rental order for deal bundle
        public IActionResult CreateBundleOrder(Guid id)
        {
            try
            {
                ViewBag.DealId = id;
                var dealFinder = _context.Deal.Find(id);
                var dealDescription = dealFinder.ItemDescription;


                ViewBag.dealDesc = dealDescription;

                return View();
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }

        public async Task<IActionResult> Create()
        {

            return View();
        }

      /// <summary>
      /// Set up checkout page and order summary prior to order for single product
      /// </summary>
      /// <param name="id"></param>
      /// <param name="userId"></param>
      /// <param name="userName"></param>
      /// <param name="itemDescription"></param>
      /// <param name="startRent"></param>
      /// <param name="endRent"></param>
      /// <param name="totalDays"></param>
      /// <returns>checkout page</returns>
        public IActionResult CheckOutPayment(int id,string userId, string userName,
            string itemDescription,DateTime startRent , DateTime endRent ,int totalDays)
        {
            try
            {
                var itemFinder = _context.Item.Find(id);

                var price = 0.00;

                if (itemFinder != null)
                {
                    HttpContext.Session.SetInt32("id", id);
                    price = itemFinder.Price;
                    HttpContext.Session.SetString("price", price.ToString());
                    HttpContext.Session.SetInt32("total", totalDays);
                    HttpContext.Session.SetString("userName", userName);
                    var des = itemFinder.Maker + " " + itemFinder.ItemType;
                    HttpContext.Session.SetString("des", des);
                    HttpContext.Session.SetString("strart", startRent.ToString());
                    HttpContext.Session.SetString("end", endRent.ToString());

                }

                return Json(new { result = "Redirect", url = Url.Action("CheckOutProduct", "Joined") });
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Set up session variables to contain order data for single items
        /// </summary>
        /// <returns>session variables contining order data</returns>
        public IActionResult CheckOutProduct()
        {
            try
            {

                ViewBag.id = HttpContext.Session.GetInt32("id");
                ViewBag.ItemDes = HttpContext.Session.GetString("des");
                ViewBag.UserName = HttpContext.Session.GetString("userName");
                ViewBag.Price = Convert.ToDouble(HttpContext.Session.GetString("price"));
                ViewBag.StartRent = DateTime.Parse(HttpContext.Session.GetString("strart")).Date;
                ViewBag.EndRent = DateTime.Parse(HttpContext.Session.GetString("end"));
                ViewBag.Total = HttpContext.Session.GetInt32("total");
                return View("CheckOutProduct");
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Set up checkout page and order summary prior to order for bundle products
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="itemDescription"></param>
        /// <param name="startRent"></param>
        /// <param name="endRent"></param>
        /// <param name="totalDays"></param>
        /// <returns>checkout page</returns>
        public IActionResult CheckOutBundle(Guid id, string userId, string userName,
           string itemDescription, DateTime startRent, DateTime endRent, int totalDays)
        {
            try
            {
                var dealFinder = _context.Deal.Find(id);

                var guidDeal = "";
                var price = 0.00;
                if (dealFinder != null)
                {
                    guidDeal = id.ToString();
                    HttpContext.Session.SetString("deal", guidDeal);
                    price = dealFinder.Price;
                    HttpContext.Session.SetString("dealPrice", price.ToString());
                    HttpContext.Session.SetInt32("dealTotal", totalDays);
                    HttpContext.Session.SetString("dealUserName", userName);
                    var des = dealFinder.ItemDescription;
                    HttpContext.Session.SetString("dealDescription", des);
                    HttpContext.Session.SetString("dealStartRent", startRent.ToString());
                    HttpContext.Session.SetString("dealEndRent", endRent.ToString());

                }

                return Json(new { result = "Redirect", url = Url.Action("ConfirmBundlePayment", "Joined") });
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }

        }


        /// <summary>
        /// Set up session variables to contain order data for bundle items
        /// </summary>
        /// <returns>session variables contining order data</returns>
        public IActionResult ConfirmBundlePayment()
        {
            try
            {

                ViewBag.id = HttpContext.Session.GetString("deal");
                ViewBag.ItemDes = HttpContext.Session.GetString("dealDescription");
                ViewBag.UserName = HttpContext.Session.GetString("dealUserName");
                ViewBag.Price = Convert.ToDouble(HttpContext.Session.GetString("dealPrice"));
                ViewBag.StartRent = DateTime.Parse(HttpContext.Session.GetString("dealStartRent")).Date;
                ViewBag.EndRent = DateTime.Parse(HttpContext.Session.GetString("dealEndRent"));
                ViewBag.Total = HttpContext.Session.GetInt32("dealTotal");
                return View("ConfirmBundlePayment");
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        
        /// <summary>
        /// Set up confirmed order items data for single or bundle items
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemDescription"></param>
        /// <param name="startRent"></param>
        /// <param name="endRent"></param>
        /// <param name="totalDays"></param>
        /// <param name="isItem"></param>
        /// <param name="price"></param>
        /// <returns>Confirmed item data</returns>
        public async Task<IActionResult> Confirm(int id,
           string itemDescription, DateTime startRent, DateTime endRent, int totalDays,bool isItem, string price)
        {
            try
            {
                if (isItem == false)
                {
                    HttpContext.Session.SetString("isItem", "false");
                    if (itemDescription != null)
                    {
                        var dealFinder = Guid.Parse(HttpContext.Session.GetString("deal"));

                        if (dealFinder != Guid.Empty)
                        {
                            var dealIte = _context.Deal.Find(dealFinder);
                            HttpContext.Session.SetString("newDeal", dealIte.ToString());
                            HttpContext.Session.SetInt32("totalDays", totalDays);
                            HttpContext.Session.SetString("price", price);

                            HttpContext.Session.SetString("dealDesc", itemDescription);
                            HttpContext.Session.SetString("strartRent", startRent.ToString());
                            HttpContext.Session.SetString("endRent", endRent.ToString());
                        }
                    }


                }
                else if (isItem == true)
                {
                    HttpContext.Session.SetString("isItem", "true");
                    var itemFinder = _context.Item.Find(id);
                    if (itemFinder != null)
                    {
                        HttpContext.Session.SetInt32("idItem", id);
                        HttpContext.Session.SetInt32("totalDays", totalDays);
                        HttpContext.Session.SetString("price", price);
                        HttpContext.Session.SetString("description", itemDescription);
                        HttpContext.Session.SetString("strartRent", startRent.ToString());
                        HttpContext.Session.SetString("endRent", endRent.ToString());


                    }

                }
                return View("Success");
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
        /// <summary>
        /// Setting up all post order summary
        /// </summary>
        /// <returns>Post order summary</returns>
        public async Task<IActionResult> Success()
        {
            try
            {
                if (HttpContext.Session.GetString("isItem").Equals("false"))
                {
                    ViewBag.itemId = Guid.Parse(HttpContext.Session.GetString("deal"));
                    ViewBag.itemDesc = HttpContext.Session.GetString("dealDesc");
                    ViewBag.startDate = DateTime.Parse(HttpContext.Session.GetString("strartRent")).Date;
                    ViewBag.endRent = DateTime.Parse(HttpContext.Session.GetString("endRent")).Date;
                    ViewBag.totalDays = HttpContext.Session.GetInt32("totalDays");
                    ViewBag.price = HttpContext.Session.GetString("price");
                    ViewBag.isEmpty = HttpContext.Session.GetString("isItem");
                }
                else
                {
                    ViewBag.itemId = HttpContext.Session.GetInt32("id");
                    ViewBag.itemDesc = HttpContext.Session.GetString("description");
                    ViewBag.startDate = DateTime.Parse(HttpContext.Session.GetString("strartRent")).Date;
                    ViewBag.endRent = DateTime.Parse(HttpContext.Session.GetString("endRent")).Date;
                    ViewBag.totalDays = HttpContext.Session.GetInt32("total");
                    ViewBag.price = HttpContext.Session.GetString("price");
                    ViewBag.isEmpty = HttpContext.Session.GetString("isItem");
                }
                return View();
            }catch
            {
                return RedirectToAction("ErrorOccured", "Home");
            }
        }
    }
    }
     