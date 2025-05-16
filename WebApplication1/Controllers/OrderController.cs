using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            return View(await _context.Order.ToListAsync());
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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

            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }

        [HttpPost]
[ValidateAntiForgeryToken]
[Authorize]
public async Task<IActionResult> PlaceOrder()
{
    // 1) Get current user ID
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId))
        return Challenge();

    // 2) Load (or create) the cart with items and products
    var cart = await _context.Cart
        .Include(c => c.CartItems)
        .ThenInclude(ci => ci.Product)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    if (cart == null)
    {
        cart = new Cart { UserId = userId };
        _context.Cart.Add(cart);
        await _context.SaveChangesAsync();
    }

    // 3) Empty cart?
    if (!cart.CartItems.Any())
    {
        TempData["ErrorMessage"] = "Your cart is empty.";
        return RedirectToAction(nameof(CartController.MyCart));
    }

    // 4) Stock check
    foreach (var item in cart.CartItems)
    {
        if (item.Quantity > item.Product.Stock)
        {
            TempData["ErrorMessage"] =
                $"Not enough stock for {item.Product.Name}. Only {item.Product.Stock} left.";
            return RedirectToAction(nameof(CartController.MyCart));
        }
    }

    var now = DateTime.UtcNow;

    // 5) Build the Order (with UpdatedAt as well)
    var order = new Order
    {
        UserId    = userId,
        CreatedAt = now,
        UpdatedAt = now,
        Status    = "Paid"
    };

    foreach (var item in cart.CartItems)
    {
        // Snapshot product details into OrderItem
        var oi = new OrderItem
        {
            Name        = item.Product.Name,
            Price       = item.Product.Price,
            Vat         = item.Product.Vat,
            Description = item.Product.Description,
            Image       = item.Product.Image,
            Quantity    = item.Quantity
        };
        order.OrderItems.Add(oi);

        // decrement stock
        item.Product.Stock -= item.Quantity;
    }

    // 6) Build the Invoice and link to the in-memory Order
    var invoice = new Invoice
    {
        CreatedAt = now,
        UpdatedAt = now
    };
    foreach (var oi in order.OrderItems)
    {
        invoice.InvoiceItems.Add(new InvoiceItem
        {
            Name        = oi.Name,
            Price       = oi.Price,
            Vat         = oi.Vat,
            Description = oi.Description,
            Image       = oi.Image,
            Quantity    = oi.Quantity
        });
    }

    // Link invoice to order—EF will handle keys for you
    order.Invoice = invoice;

    // 7) Add the root; EF sees the graph: Order → Invoice → InvoiceItems
    _context.Order.Add(order);

    // 8) Clear the cart
    _context.CartItem.RemoveRange(cart.CartItems);

    // 9) Single SaveChanges: inserts Order, then Invoice, then all items, then deletes cart items
    await _context.SaveChangesAsync();

    // 10) Redirect to confirmation
    return RedirectToAction(nameof(MyOrderDetails), new { id = order.Id });
}

        public async Task<IActionResult> MyOrders()
        {
            // get current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Forbid();

            // fetch all orders for that user, including items
            var orders = await _context.Order
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)      // optional: most recent first
                .ToListAsync();

            return View(orders);
        }
        public async Task<IActionResult> MyOrderDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.Include(o => o.OrderItems).Include(o=>o.Invoice)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    

}
}
