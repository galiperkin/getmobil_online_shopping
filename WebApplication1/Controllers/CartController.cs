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
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cart.ToListAsync());
        }

        // GET: Cart/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Cart/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Cart/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            return View(cart);
        }

        // POST: Cart/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            return View(cart);
        }

        // GET: Cart/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Cart.FindAsync(id);
            if (cart != null)
            {
                _context.Cart.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.Id == id);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            // 1. Get current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Challenge();  // not logged in

            
            // 2. Find or create their cart
            var cart = await _context.Cart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
            }

            // 3. Check product exists & stock
            var product = await _context.Product.FindAsync(productId);
            if (product == null) return NotFound();

            if (product.Stock < quantity)
            {
                ModelState.AddModelError("", "Not enough stock available.");
                return RedirectToAction(nameof(Details), "Product", new { id = productId });
            }

            // 4. Add or update CartItem
            var item = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId);

            if (item != null)
            {
                item.Quantity += quantity;
                _context.CartItem.Update(item);
            }
            else
            {
                item = new CartItem
                {
                    CartId    = cart.Id,
                    ProductId = productId,
                    Quantity  = quantity
                };
                _context.CartItem.Add(item);
            }

            await _context.SaveChangesAsync();

            // 5. Redirect back to cart
            return RedirectToAction(nameof(MyCart));
        }
    
    
        public async Task<IActionResult> MyCart()
        {
            // 1. Get current user ID (as int)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Forbid();  // or Challenge()
            }

            // 2. Try to load their cart (with items and product details)
            var cart = await _context.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // 3. If none, create and persist a new one
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
            }

            // 4. Show the view
            return View(cart);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangeQuantity(int cartItemId, int quantityChange)
        {
            // 1. Get current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            // 2. Load the CartItem with its Cart (to verify ownership) and Product (for stock check)
            var cartItem = await _context.CartItem
                .Include(ci => ci.Cart)
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci =>
                    ci.Id == cartItemId &&
                    ci.Cart.UserId == userId
                );
            if (cartItem == null)
                return NotFound();

            // 3. Compute the new desired quantity
            var newQuantity = cartItem.Quantity + quantityChange;

            // 4. If it drops to zero or below, remove the item entirely
            if (newQuantity <= 0)
            {
                _context.CartItem.Remove(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyCart));
            }

            // 5. If incrementing, ensure we don’t exceed stock
            if (quantityChange > 0 && newQuantity > cartItem.Product.Stock)
            {
                TempData["ErrorMessage"] = "Not enough stock available.";
                return RedirectToAction(nameof(MyCart));
            }

            // 6. Otherwise update to the new quantity
            cartItem.Quantity = newQuantity;
            _context.CartItem.Update(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyCart));
        }
        
    }
}
