using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

using WebApplication1.Controllers;
using WebApplication1.Data;
using WebApplication1.Models;

using Xunit;



    public class CartControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _db;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            // a) Configure EF Core to use an in-memory DB
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _db = new ApplicationDbContext(opts);

            // b) Seed a sample product
            _db.Product.Add(new Product {
                Id              = 1,
                Name            = "Test Product",
                Price           = 1000,
                Vat             = 18,
                DiscountPercent = 0,
                Stock           = 5
            });
            _db.SaveChanges();

            // c) Instantiate the controller
            _controller = new CartController(_db);

            // d) Fake a logged-in user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "user-123")
            }, "test"));
            _controller.ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // e) Provide TempData so your controller can write errors
            _controller.TempData = new TempDataDictionary(
                _controller.ControllerContext.HttpContext,
                new TestTempDataProvider()
            );
        }

        public void Dispose() => _db.Dispose();

        [Fact]
        public async Task AddToCart_WhenCartEmpty_CreatesCartAndItem()
        {
            // Act
            var result = await _controller.AddToCart(1, 2);

            // Assert it redirects to MyCart
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.MyCart), redirect.ActionName);

            // Assert DB state: one cart, one item
            var cart = _db.Cart.Include(c => c.CartItems)
                               .Single(c => c.UserId == "user-123");
            Assert.Single(cart.CartItems);
            var ci = cart.CartItems.Single();
            Assert.Equal(1, ci.ProductId);
            Assert.Equal(2, ci.Quantity);
        }

        [Fact]
        public async Task ChangeQuantity_DecrementToZero_RemovesItem()
        {
            // Arrange: seed a cart + 1 item
            var cart = new Cart { UserId = "user-123" };
            _db.Cart.Add(cart);
            await _db.SaveChangesAsync();
            var ci = new CartItem {
                CartId    = cart.Id,
                ProductId = 1,
                Quantity  = 1
            };
            _db.CartItem.Add(ci);
            await _db.SaveChangesAsync();

            // Act: decrement by 1 → zero
            var result = await _controller.ChangeQuantity(ci.Id, -1);

            // Assert redirect & removal
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.MyCart), redirect.ActionName);
            Assert.False(_db.CartItem.Any(i => i.Id == ci.Id));
        }

        // TempData stub, inside the test class:
        private class TestTempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context)
                => new Dictionary<string, object>();
            public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
        }
    }

