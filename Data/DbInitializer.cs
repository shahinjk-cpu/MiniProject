using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Models;
using PetShopApp.Models.Enums;

namespace PetShopApp.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // 1. Avtomatik miqrasiyanın icrası
        await context.Database.MigrateAsync();

        // Rolların yaradılması
        string[] roles = { "Admin", "Customer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Default Admin İstifadəçisi
        var adminEmail = "admin@petshop.com";
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Super Admin",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(existingAdmin, "Admin"))
            {
                await userManager.AddToRoleAsync(existingAdmin, "Admin");
            }
        }

        // 3. Settings (Sayt tənzimləmələri)
        if (!await context.Settings.AnyAsync())
        {
            var settings = new List<Setting>
            {
                new() { Key = "SiteName", Value = "Waggy - Pet Shop eCommerce", Description = "Veb sayt adı" },
                new() { Key = "LogoUrl", Value = "logo.png", Description = "Loqo fayl adı" },
                new() { Key = "ContactPhone", Value = "+1 800 555 0199", Description = "Əlaqə telefonu" },
                new() { Key = "ContactEmail", Value = "info@waggypetshop.com", Description = "Əlaqə elektron poçtu" },
                new() { Key = "ContactAddress", Value = "123 Pet Street, Animal City, AC 45678", Description = "Ünvan" },
                new() { Key = "WorkingHours", Value = "Mon - Sat: 9:00 AM - 8:00 PM", Description = "İş saatları" },
                new() { Key = "FacebookUrl", Value = "https://facebook.com", Description = "Facebook keçidi" },
                new() { Key = "TwitterUrl", Value = "https://twitter.com", Description = "Twitter keçidi" },
                new() { Key = "InstagramUrl", Value = "https://instagram.com", Description = "Instagram keçidi" },
                new() { Key = "YouTubeUrl", Value = "https://youtube.com", Description = "YouTube keçidi" }
            };
            await context.Settings.AddRangeAsync(settings);
            await context.SaveChangesAsync();
        }

        // 4. SliderItems (Hero Swiper Slayderi)
        if (!await context.SliderItems.AnyAsync())
        {
            var sliders = new List<SliderItem>
            {
                new()
                {
                    Title = "Best destination for your pets",
                    Subtitle = "Save 10 - 20% off",
                    DiscountText = "Save 10 - 20% off",
                    ImageUrl = "banner-img.png",
                    ButtonText = "Shop Now",
                    ButtonUrl = "/Shop",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new()
                {
                    Title = "Healthy & organic pet food for friends",
                    Subtitle = "Natural Ingredients",
                    DiscountText = "Up to 30% off",
                    ImageUrl = "banner-img2.png",
                    ButtonText = "Explore Foods",
                    ButtonUrl = "/Shop?category=foodies",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new()
                {
                    Title = "Clearance sale !!! Warm clothes & toys",
                    Subtitle = "Special Discounts",
                    DiscountText = "UP TO 40% OFF",
                    ImageUrl = "banner-img3.png",
                    ButtonText = "Shop Sale",
                    ButtonUrl = "/Shop?category=clothing",
                    DisplayOrder = 3,
                    IsActive = true
                }
            };
            await context.SliderItems.AddRangeAsync(sliders);
            await context.SaveChangesAsync();
        }

        // 5. Categories (Kateqoriyalar)
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { Name = "Foodies", Slug = "foodies", Description = "Qidalandırıcı və sağlam yemlər", ImageUrl = "item7.jpg", IconClass = "ph:bowl-food", DisplayOrder = 1, IsActive = true },
                new() { Name = "Dog Shop", Slug = "dog-shop", Description = "İtlər üçün hər cür məhsul və ləvazimatlar", ImageUrl = "item3.jpg", IconClass = "ph:dog", DisplayOrder = 2, IsActive = true },
                new() { Name = "Cat Shop", Slug = "cat-shop", Description = "Pişiklər üçün yemlər, yataqlar və oyuncaqlar", ImageUrl = "item4.jpg", IconClass = "ph:cat", DisplayOrder = 3, IsActive = true },
                new() { Name = "Bird Shop", Slug = "bird-shop", Description = "Quşlar üçün yemlər, qəfəslər və aksessuarlar", ImageUrl = "item11.jpg", IconClass = "ph:bird", DisplayOrder = 4, IsActive = true },
                new() { Name = "Clothing", Slug = "clothing", Description = "Rahat və dəbli ev heyvanı geyimləri", ImageUrl = "item1.jpg", IconClass = "ph:t-shirt", DisplayOrder = 5, IsActive = true },
                new() { Name = "Accessories", Slug = "accessories", Description = "Xaltalar, qayışlar, qablar və gigiyena vasitələri", ImageUrl = "item13.jpg", IconClass = "ph:paw-print", DisplayOrder = 6, IsActive = true }
            };
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // 6. Products (16 real məhsul şablondakı item1-item16 şəkilləri ilə)
        if (!await context.Products.AnyAsync())
        {
            var foodiesCat = await context.Categories.FirstAsync(c => c.Slug == "foodies");
            var dogCat = await context.Categories.FirstAsync(c => c.Slug == "dog-shop");
            var catCat = await context.Categories.FirstAsync(c => c.Slug == "cat-shop");
            var birdCat = await context.Categories.FirstAsync(c => c.Slug == "bird-shop");
            var clothingCat = await context.Categories.FirstAsync(c => c.Slug == "clothing");
            var accCat = await context.Categories.FirstAsync(c => c.Slug == "accessories");

            var products = new List<Product>
            {
                new()
                {
                    Name = "Grey Pet Hoodie",
                    Slug = "grey-pet-hoodie",
                    Description = "Soft, warm, and stylish cotton blend hoodie designed for small and medium dogs. Features easy slip-on fit and hood with drawstrings.",
                    AdditionalInfo = "Material: 100% Cotton Blend; Machine washable; Sizes: S, M, L, XL",
                    Price = 35.00m,
                    OldPrice = 45.00m,
                    SKU = "CLO-001",
                    StockQuantity = 45,
                    Rating = 5.0,
                    ReviewCount = 8,
                    MainImageUrl = "item1.jpg",
                    IsClothing = true,
                    IsFeatured = true,
                    IsBestSeller = false,
                    IsActive = true,
                    CategoryId = clothingCat.Id
                },
                new()
                {
                    Name = "Cotton Knitted Dog Sweater",
                    Slug = "cotton-knitted-dog-sweater",
                    Description = "Charming knit sweater providing optimum warmth during autumn and chilly winter walks. Elastic cuffs prevent slipping.",
                    AdditionalInfo = "Material: Acrylic Knit; Hand wash recommended; Weight: 180g",
                    Price = 28.00m,
                    OldPrice = 38.00m,
                    SKU = "CLO-002",
                    StockQuantity = 60,
                    Rating = 4.8,
                    ReviewCount = 12,
                    MainImageUrl = "item2.jpg",
                    IsClothing = true,
                    IsFeatured = false,
                    IsBestSeller = true,
                    IsActive = true,
                    CategoryId = clothingCat.Id
                },
                new()
                {
                    Name = "Winter Warm Puffer Vest",
                    Slug = "winter-warm-puffer-vest",
                    Description = "Water-resistant padded jacket with reflective stripes for evening safety. Zipper closure along the back for swift dressing.",
                    AdditionalInfo = "Waterproof nylon shell, fleece interior padding.",
                    Price = 42.00m,
                    OldPrice = 55.00m,
                    SKU = "CLO-003",
                    StockQuantity = 30,
                    Rating = 4.9,
                    ReviewCount = 15,
                    MainImageUrl = "item3.jpg",
                    IsClothing = true,
                    IsFeatured = true,
                    IsBestSeller = false,
                    IsActive = true,
                    CategoryId = clothingCat.Id
                },
                new()
                {
                    Name = "Waterproof Dog Raincoat",
                    Slug = "waterproof-dog-raincoat",
                    Description = "Bright yellow waterproof poncho with clear visor hood to shield your furry best friend from downpours.",
                    AdditionalInfo = "100% Polyurethane waterproof fabric.",
                    Price = 30.00m,
                    OldPrice = 40.00m,
                    SKU = "CLO-004",
                    StockQuantity = 40,
                    Rating = 4.7,
                    ReviewCount = 6,
                    MainImageUrl = "item4.jpg",
                    IsClothing = true,
                    IsFeatured = false,
                    IsBestSeller = false,
                    IsActive = true,
                    CategoryId = clothingCat.Id
                },
                new()
                {
                    Name = "Striped Puppy T-Shirt",
                    Slug = "striped-puppy-tshirt",
                    Description = "Lightweight and breathable cotton tee perfect for casual everyday indoor wear and sun protection.",
                    AdditionalInfo = "Cotton 95%, Spandex 5%",
                    Price = 22.00m,
                    OldPrice = null,
                    SKU = "CLO-005",
                    StockQuantity = 80,
                    Rating = 4.6,
                    ReviewCount = 4,
                    MainImageUrl = "item5.jpg",
                    IsClothing = true,
                    IsFeatured = false,
                    IsBestSeller = false,
                    IsActive = true,
                    CategoryId = clothingCat.Id
                },
                new()
                {
                    Name = "Fleece Cozy Cat Vest",
                    Slug = "fleece-cozy-cat-vest",
                    Description = "Ultra-plush fleece vest tailored specifically for cats and toy dog breeds who dislike constrictive clothing.",
                    AdditionalInfo = "Ultra-soft polar fleece.",
                    Price = 26.00m,
                    OldPrice = null,
                    SKU = "CLO-006",
                    StockQuantity = 55,
                    Rating = 4.5,
                    ReviewCount = 5,
                    MainImageUrl = "item6.jpg",
                    IsClothing = true,
                    IsFeatured = false,
                    IsBestSeller = false,
                    IsActive = true,
                    CategoryId = clothingCat.Id
                },
                new()
                {
                    Name = "Crunchy Salmon Kibble 3kg",
                    Slug = "crunchy-salmon-kibble-3kg",
                    Description = "Grain-free natural formula packed with Atlantic salmon, sweet potato, and Omega-3 fatty acids for skin and coat vitality.",
                    AdditionalInfo = "Net Weight: 3.0 kg; Crude Protein: 32%; Crude Fat: 16%",
                    Price = 49.00m,
                    OldPrice = 60.00m,
                    SKU = "FOD-001",
                    StockQuantity = 100,
                    Rating = 5.0,
                    ReviewCount = 28,
                    MainImageUrl = "item7.jpg",
                    IsFoodie = true,
                    IsFeatured = true,
                    IsBestSeller = true,
                    IsActive = true,
                    CategoryId = foodiesCat.Id
                },
                new()
                {
                    Name = "Organic Raw Dog Meat Pate",
                    Slug = "organic-raw-dog-meat-pate",
                    Description = "Premium canned wet food with chunks of organic beef, bone broth, and organic carrots for optimal canine digestion.",
                    AdditionalInfo = "400g can; 100% natural, no preservatives.",
                    Price = 36.00m,
                    OldPrice = 45.00m,
                    SKU = "FOD-002",
                    StockQuantity = 120,
                    Rating = 4.9,
                    ReviewCount = 19,
                    MainImageUrl = "item8.jpg",
                    IsFoodie = true,
                    IsFeatured = true,
                    IsBestSeller = false,
                    IsActive = true,
                    CategoryId = foodiesCat.Id
                },
                new()
                {
                    Name = "Dental Chew Sticks for Dogs",
                    Slug = "dental-chew-sticks-dogs",
                    Description = "Veterinarian formulated dental sticks reducing tartar, freshening breath, and supporting gum health with mint and spirulina.",
                    AdditionalInfo = "Pack of 14 sticks; Low calorie.",
                    Price = 18.00m,
                    OldPrice = null,
                    SKU = "FOD-003",
                    StockQuantity = 90,
                    Rating = 4.8,
                    ReviewCount = 14,
                    MainImageUrl = "item9.jpg",
                    IsFoodie = true,
                    IsFeatured = false,
                    IsBestSeller = false,
                    IsActive = true,
                    CategoryId = foodiesCat.Id
                },
                new()
                {
                    Name = "Gourmet Chicken Cat Casserole",
                    Slug = "gourmet-chicken-cat-casserole",
                    Description = "Tender shredded free-range chicken breast in savory gravy designed to satisfy even the most discerning feline palate.",
                    AdditionalInfo = "12 x 85g pouches; High moisture content.",
                    Price = 24.00m,
                    OldPrice = 30.00m,
                    SKU = "FOD-004",
                    StockQuantity = 75,
                    Rating = 4.7,
                    ReviewCount = 11,
                    MainImageUrl = "item10.jpg",
                    IsFoodie = true,
                    IsFeatured = false,
                    IsBestSeller = true,
                    IsActive = true,
                    CategoryId = foodiesCat.Id
                },
                new()
                {
                    Name = "Healthy Seed Mix for Parrots",
                    Slug = "healthy-seed-mix-parrots",
                    Description = "Nutrient-dense bird feed blending millet, sunflower seeds, dried fruits, and calcium pellets for bright plumage and energy.",
                    AdditionalInfo = "Net weight: 1.5 kg; Fortified with vitamins A, D3, E.",
                    Price = 16.00m,
                    OldPrice = null,
                    SKU = "BRD-001",
                    StockQuantity = 50,
                    Rating = 4.8,
                    ReviewCount = 7,
                    MainImageUrl = "item11.jpg",
                    IsFoodie = false,
                    IsFeatured = false,
                    IsBestSeller = true,
                    IsActive = true,
                    CategoryId = birdCat.Id
                },
                new()
                {
                    Name = "Catnip Infused Mouse Toy",
                    Slug = "catnip-infused-mouse-toy",
                    Description = "Interactive plush mice filled with 100% organic Canadian catnip to stimulate hunting instincts and active play.",
                    AdditionalInfo = "Pack of 3 toys; Durable fabric.",
                    Price = 12.00m,
                    OldPrice = null,
                    SKU = "CAT-001",
                    StockQuantity = 110,
                    Rating = 4.6,
                    ReviewCount = 9,
                    MainImageUrl = "item12.jpg",
                    IsFoodie = false,
                    IsFeatured = false,
                    IsBestSeller = true,
                    IsActive = true,
                    CategoryId = catCat.Id
                },
                new()
                {
                    Name = "Heavy Duty Retractable Leash",
                    Slug = "heavy-duty-retractable-leash",
                    Description = "5-meter heavy duty nylon cord leash with 360-degree tangle-free movement and ergonomic anti-slip grip brake.",
                    AdditionalInfo = "Length: 5m (16ft); For pets up to 50kg.",
                    Price = 29.00m,
                    OldPrice = 35.00m,
                    SKU = "ACC-001",
                    StockQuantity = 45,
                    Rating = 4.9,
                    ReviewCount = 16,
                    MainImageUrl = "item13.jpg",
                    IsFoodie = false,
                    IsFeatured = false,
                    IsBestSeller = false,
                    IsActive = true,
                    CategoryId = accCat.Id
                },
                new()
                {
                    Name = "Orthopedic Memory Foam Pet Bed",
                    Slug = "orthopedic-memory-foam-pet-bed",
                    Description = "High-density orthopedic foam mattress relieving pressure on joints. Soft faux-suede bolster rim with removable washable cover.",
                    AdditionalInfo = "Dimensions: 90 x 70 x 18 cm; Non-skid bottom.",
                    Price = 75.00m,
                    OldPrice = 95.00m,
                    SKU = "DOG-001",
                    StockQuantity = 25,
                    Rating = 5.0,
                    ReviewCount = 22,
                    MainImageUrl = "item14.jpg",
                    IsFoodie = false,
                    IsFeatured = true,
                    IsBestSeller = true,
                    IsActive = true,
                    CategoryId = dogCat.Id
                },
                new()
                {
                    Name = "Stainless Steel Non-Slip Bowl",
                    Slug = "stainless-steel-non-slip-bowl",
                    Description = "Rust-resistant double wall stainless steel food and water bowl fitted with a rubber base to prevent floor scratches and spills.",
                    AdditionalInfo = "Capacity: 1200ml; Dishwasher safe.",
                    Price = 19.00m,
                    OldPrice = null,
                    SKU = "ACC-002",
                    StockQuantity = 70,
                    Rating = 4.7,
                    ReviewCount = 8,
                    MainImageUrl = "item15.jpg",
                    IsFoodie = false,
                    IsFeatured = false,
                    IsBestSeller = false,
                    IsActive = true,
                    CategoryId = accCat.Id
                },
                new()
                {
                    Name = "Automatic Water Fountain 2.5L",
                    Slug = "automatic-water-fountain-25l",
                    Description = "Ultra-silent recirculating pet fountain with multi-stage carbon filter encouraging pets to drink more fresh, oxygenated water.",
                    AdditionalInfo = "Capacity: 2.5L; USB powered 5V pump; Includes 2 filters.",
                    Price = 48.00m,
                    OldPrice = 60.00m,
                    SKU = "CAT-002",
                    StockQuantity = 35,
                    Rating = 4.9,
                    ReviewCount = 18,
                    MainImageUrl = "item16.jpg",
                    IsFoodie = false,
                    IsFeatured = true,
                    IsBestSeller = false,
                    IsActive = true,
                    CategoryId = catCat.Id
                }
            };
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            // ProductImages (Qalereya üçün hər məhsula əlavə şəkillər)
            var productImages = new List<ProductImage>();
            foreach (var p in products)
            {
                productImages.Add(new ProductImage { ProductId = p.Id, ImageUrl = p.MainImageUrl, IsMain = true, DisplayOrder = 1 });
            }
            await context.ProductImages.AddRangeAsync(productImages);

            // Məhsul rəyləri (ProductReviews)
            var sampleReviews = new List<ProductReview>
            {
                new() { ProductId = products[0].Id, FullName = "Sarah Jenkins", Email = "sarah@gmail.com", Comment = "Fantastic quality! Fits my French bulldog perfectly and looks adorable.", Rating = 5, IsApproved = true },
                new() { ProductId = products[0].Id, FullName = "David Brown", Email = "david@yahoo.com", Comment = "Soft material and easy to clean. Highly recommended!", Rating = 5, IsApproved = true },
                new() { ProductId = products[6].Id, FullName = "Michael Scott", Email = "michael@dundermifflin.com", Comment = "My lab loves this salmon kibble! His coat has never been shinier.", Rating = 5, IsApproved = true },
                new() { ProductId = products[13].Id, FullName = "Jessica Taylor", Email = "jessica@outlook.com", Comment = "The memory foam bed is worth every penny. My older dog sleeps like a puppy again.", Rating = 5, IsApproved = true }
            };
            await context.ProductReviews.AddRangeAsync(sampleReviews);
            await context.SaveChangesAsync();
        }

        // 7. Testimonials (Müştəri Rəyləri Karuseli)
        if (!await context.Testimonials.AnyAsync())
        {
            var testimonials = new List<Testimonial>
            {
                new()
                {
                    FullName = "Anna Malkova",
                    Role = "Golden Retriever Owner",
                    Comment = "At the core of our practice is the idea that pet care should be high quality and accessible. Waggy gave my dog the best comfort ever!",
                    Rating = 5,
                    AvatarUrl = "reviewer-1.jpg",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new()
                {
                    FullName = "James Smith",
                    Role = "Veterinary Specialist",
                    Comment = "I recommend their food and organic treats to all my clients. The ingredients are top-notch, pure, and scientifically balanced.",
                    Rating = 5,
                    AvatarUrl = "reviewer-2.jpg",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new()
                {
                    FullName = "Rachel Green",
                    Role = "Cat Enthusiast",
                    Comment = "The customer service is unmatched, and delivery arrived the very next day. My cats absolutely love the cozy beds and food!",
                    Rating = 5,
                    AvatarUrl = "reviewer-3.jpg",
                    DisplayOrder = 3,
                    IsActive = true
                }
            };
            await context.Testimonials.AddRangeAsync(testimonials);
            await context.SaveChangesAsync();
        }

        // 8. BlogPosts (Bloq Yazıları)
        if (!await context.BlogPosts.AnyAsync())
        {
            var blogs = new List<BlogPost>
            {
                new()
                {
                    Title = "10 Tips for Healthy and Happy Puppies in 2026",
                    Slug = "10-tips-healthy-happy-puppies",
                    AuthorName = "Dr. Sarah Adams",
                    CategoryName = "Pet Health",
                    ImageUrl = "blog1.jpg",
                    ShortDescription = "Discover essential nutrition, exercise, and veterinary tips to keep your furry friend thriving every day.",
                    Content = "Welcoming a new puppy into your family is an exciting adventure filled with snuggles, playtime, and a few inevitable challenges. To ensure your puppy develops into a healthy and confident adult dog, proper socialization and nutrient-rich feeding during the first six months are paramount. Regular veterinary checkups, vaccination schedules, and daily physical stimulation foster lifelong vitality.",
                    ViewCount = 142
                },
                new()
                {
                    Title = "How to Choose the Right Winter Clothes for Your Pet",
                    Slug = "choose-right-winter-clothes",
                    AuthorName = "Mark Wilson",
                    CategoryName = "Pet Fashion",
                    ImageUrl = "blog2.jpg",
                    ShortDescription = "Keeping your pet warm during cold months without sacrificing mobility, safety, and comfort.",
                    Content = "When winter arrives with crisp chills and snowy pavements, smaller breeds and short-coated dogs need extra warmth. Selecting the right pet coat requires balancing insulation with freedom of movement. Always opt for water-resistant outer layers, breathable fleece linings, and reflective details that keep pets visible during dark afternoon strolls.",
                    ViewCount = 98
                },
                new()
                {
                    Title = "Why Organic Food Makes a Noticeable Difference in Cats",
                    Slug = "organic-food-difference-cats",
                    AuthorName = "Elena Rostova",
                    CategoryName = "Nutrition",
                    ImageUrl = "blog3.jpg",
                    ShortDescription = "Learn how grain-free and organic diets improve feline coat shine, digestion, and energy levels.",
                    Content = "Cats are obligate carnivores with digestive systems primed for animal proteins rather than artificial binders and grain fillers. Switching to organic, whole-meat formulations results in visible improvements in coat luster, lower risk of urinary tract issues, and sustained everyday playfulness.",
                    ViewCount = 215
                }
            };
            await context.BlogPosts.AddRangeAsync(blogs);
            await context.SaveChangesAsync();

            // Bloq şərhləri
            var sampleBlogComments = new List<BlogComment>
            {
                new()
                {
                    BlogPostId = blogs[0].Id,
                    AuthorName = "Sam Smith",
                    Email = "sam@example.com",
                    Content = "These puppy tips are spot on! We started socializing our golden retriever early and the results are incredible.",
                    IsApproved = true
                },
                new()
                {
                    BlogPostId = blogs[0].Id,
                    AuthorName = "Santie Mary",
                    Email = "santie@example.com",
                    Content = "Thank you for emphasizing mental stimulation. Puzzle toys have completely transformed our routine!",
                    IsApproved = true
                }
            };
            await context.BlogComments.AddRangeAsync(sampleBlogComments);
            await context.SaveChangesAsync();
        }

        // 9. InstagramPosts (İnstaqram Qalereyası)
        if (!await context.InstagramPosts.AnyAsync())
        {
            var instaPosts = new List<InstagramPost>
            {
                new() { ImageUrl = "insta1.jpg", PostUrl = "https://instagram.com", DisplayOrder = 1 },
                new() { ImageUrl = "insta2.jpg", PostUrl = "https://instagram.com", DisplayOrder = 2 },
                new() { ImageUrl = "insta3.jpg", PostUrl = "https://instagram.com", DisplayOrder = 3 },
                new() { ImageUrl = "insta4.jpg", PostUrl = "https://instagram.com", DisplayOrder = 4 },
                new() { ImageUrl = "insta5.jpg", PostUrl = "https://instagram.com", DisplayOrder = 5 },
                new() { ImageUrl = "insta6.jpg", PostUrl = "https://instagram.com", DisplayOrder = 6 }
            };
            await context.InstagramPosts.AddRangeAsync(instaPosts);
            await context.SaveChangesAsync();
        }

        // 10. FaqItems (Tez-tez Verilən Suallar)
        if (!await context.FaqItems.AnyAsync())
        {
            var faqs = new List<FaqItem>
            {
                new() { Question = "How long does standard delivery take?", Answer = "Orders are typically processed within 24 hours and delivered in 2 to 4 business days.", Category = "Shipping", DisplayOrder = 1, IsActive = true },
                new() { Question = "What is your return and refund policy?", Answer = "We offer a 30-day money-back guarantee on all unopened food items and unworn accessories.", Category = "Returns", DisplayOrder = 2, IsActive = true },
                new() { Question = "Are all pet foods certified and natural?", Answer = "Yes, every food product in our store complies with the highest international animal nutrition safety standards.", Category = "Products", DisplayOrder = 3, IsActive = true },
                new() { Question = "Can I track my shipment online?", Answer = "Once your order is dispatched, you will receive a tracking code via email to monitor live delivery.", Category = "Orders", DisplayOrder = 4, IsActive = true }
            };
            await context.FaqItems.AddRangeAsync(faqs);
            await context.SaveChangesAsync();
        }
    }
}
