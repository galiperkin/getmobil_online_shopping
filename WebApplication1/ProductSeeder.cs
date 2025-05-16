using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

public static class ProductSeeder
{
    public static void Initialize(IServiceProvider services)
    {
        using var scope   = services.CreateScope();
        var context       = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        // if there are any products, we’ve already seeded
        if (context.Product.Any()) 
            return;

        context.Product.AddRange(
               new Product {
            Id          = 1,
            Name        = "Silicone Phone Case",
            Price       = 1599,     
            Stock = 15,
            Vat         = 18,
            DiscountPercent = 10,
            Description = "Shock-absorbing silicone case for 6.1” displays",
            Image       = "https://i5.walmartimages.com/seo/Dteck-iPhone-11-Case-Ultra-Slim-Fit-Case-Liquid-Silicone-Gel-Cover-Full-Body-Protection-Anti-Scratch-Shockproof-Compatible-6-1-inch-lilac-violet_4837a400-8753-4a51-9a44-6166c18ddd1b_1.386d050ab9cd73c9957c8fb9ed3cb21d.jpeg"
        },
        new Product {
            Id          = 2,
            Name        = "Tempered Glass Screen Protector",
            Price       = 899,     
            Stock = 8,
            Vat         = 18,
            DiscountPercent = 5,
            Description = "9H hardness, oleo-phobic coating, edge-to-edge clarity",
            Image       = "https://m.media-amazon.com/images/I/71NqqWOm7FL._AC_SL1500_.jpg"
        },
        new Product {
            Id          = 3,
            Name        = "USB-C Fast Charger (18W)",
            Price       = 2499,     
            Stock = 24,
            Vat         = 18,
            DiscountPercent = 20,
            Description = "Quick-charge wall adapter with USB-C PD output",
            Image       = "https://i5.walmartimages.com/seo/USB-C-Charger-Set-Overtime-20W-Fast-Charger-and-USBC-Lightning-Cable-6ft-White_711a52d9-fb24-4261-9e5b-27890cafaee3.9d58828c18d3f1dc4f5523adb13e29fd.jpeg"
        },
        new Product {
            Id          = 4,
            Name        = "Wireless Charging Pad",
            Price       = 1999,   
            Stock=19,
            Vat         = 18,
            DiscountPercent = 10,
            Description = "Qi-certified 10W pad, slim aluminum design",
            Image       = "https://i5.walmartimages.com/seo/EEEkit-Wireless-Charger-Qi-10W-Max-Fast-Wireless-Charging-Pad-Compatible-iPhone-13-12-11-11-Pro-11-Pro-Max-XR-XS-X-Apple-Watch-Series-Airpods-Pro-2-Q_235fe51f-c23d-4c65-9f98-fe7f9a317d8f.1e362ff2a431031ebe98bc7fab4d3b76.jpeg"
        },
        new Product {
            Id          = 5,
            Name        = "Car Vent Phone Mount",
            Price       = 1199,   
            Stock = 11,
            Vat         = 18,
            DiscountPercent = 0,
            Description = "360° swivel, one-hand operation, universal grip",
            Image       = "https://i5.walmartimages.com/seo/LAX-Pro-Grip-Phone-Holder-Car-Mount-for-AC-Air-Vent-Black_0c558513-7b8d-4bc5-aad7-aceca35aadb3_1.f4caf07ae1f1b0cb6b4f9b0a7bde9c51.jpeg"
        },
        new Product {
            Id          = 6,
            Name        = "PopSocket Grip Stand",
            Price       = 799,    
            Stock = 7,
            Vat         = 18,
            DiscountPercent = 10,
            Description = "Collapsible grip & stand, easy one-handed use",
            Image       = "https://m.media-amazon.com/images/I/615h4QQcc0L._AC_SL1000_.jpg"
        },
        new Product {
            Id          = 7,
            Name        = "10,000 mAh Power Bank",
            Price       = 2999,    
            Stock = 29,
            Vat         = 18,
            DiscountPercent = 40,
            Description = "Dual-output USB ports, LED battery indicator",
            Image       = "https://productimages.hepsiburada.net/s/777/960-1280/110000820620242.jpg"
        },
        new Product {
            Id          = 8,
            Name        = "Lightning to USB-C Cable (1 m)",
            Price       = 499,   
            Stock = 4,
            Vat         = 18,
            DiscountPercent = 20,
            Description = "Durable braided nylon, MFi-certified",
            Image       = "https://i5.walmartimages.com/seo/Onn-3-feet-Lightning-to-USB-C-Charging-and-Data-Cable-for-iPhone-iPad-Black_a76fe0b9-d790-4aef-841a-cd6b06c505ec.69296a42434c801b664348821e2be1c2.jpeg"
        },
        new Product {
            Id          = 9,
            Name        = "Microfiber Cleaning Cloth",
            Price       = 299,  
            Stock = 0,
            Vat         = 18,
            DiscountPercent = 40,
            Description = "Streak-free lens and screen cleaning",
            Image       = "https://www.tlbox.com/wp-content/uploads/2015/03/Microfiber-Cleaning-Cloth-Breeze-through-your-cleaning-tasks-624x624.jpg"
        },
        new Product {
            Id          = 10,
            Name        = "Earphone Cable Organizer",
            Price       = 399,     
            Stock = 0,
            Vat         = 18,
            DiscountPercent = 0,
            Description = "Tangle-free wrap, keychain attachment",
            Image       = "https://m.media-amazon.com/images/I/71Oxf5sXjCL._AC_SL1500_.jpg"
        }
            
        );

        context.SaveChanges();
    }
}