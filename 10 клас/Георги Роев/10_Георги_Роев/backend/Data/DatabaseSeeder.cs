using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // ==========================
            // Admin Account
            // ==========================

           if (!context.Users.Any())
{
    var passwordHasher = new PasswordHasher<User>();

    var admin = new User
    {
        FirstName = "Admin",
        LastName = "Administrator",
        Email = "admin@babastrinka.bg",
        EmailVerified = true,
        IsAdmin = true
    };

    admin.Password = passwordHasher.HashPassword(admin, "Admin123!");

    context.Users.Add(admin);

    context.SaveChanges();
}

            // ==========================
            // Badges
            // ==========================

            if (!context.Badges.Any())
            {
                context.Badges.AddRange(

                    new Badge { Name = "Изборът на дядо" },

                    new Badge { Name = "Баба не го дава!" },

                    new Badge { Name = "Изчезва първи" },

                    new Badge { Name = "Кралете се бият за него" },

                    new Badge { Name = "Тайната рецепта" },

                    new Badge { Name = "Само за ценители" },

                    new Badge { Name = "Последните буркани" },

                    new Badge { Name = "Гордостта на баба Стринка" }

                );

                context.SaveChanges();
            }

           // ==========================
// Products
// ==========================

if (!context.Products.Any())
{
    context.Products.AddRange(

        new Product
        {
            Name = "Компот от праскови",
            Description = "Класически домашен компот с ароматни праскови, приготвен по традиционна рецепта.",
            Ingredients = "Праскови, захар, вода, стръкче индрише",
            Price = 3.10m,
            Quantity = 50,
            ImagePath = "Праскови.png",
            IsAvailable = true
        },

        new Product
        {
            Name = "Компот от череши",
            Description = "Сладък компот от добре узрели череши с лек аромат на индрише.",
            Ingredients = "Череши, вода, захар, лист от индрише",
            Price = 2.20m,
            Quantity = 50,
            ImagePath = "Череши.png",
            IsAvailable = true
        },

        new Product
        {
            Name = "Компот от кайсии",
            Description = "Домашен компот от сочни кайсии с богат плодов вкус.",
            Ingredients = "Кайсии, захар, вода, ядки от кайсиеви костилки",
            Price = 2.30m,
            Quantity = 50,
            ImagePath = "Кайсии.png",
            IsAvailable = true
        },

        new Product
        {
            Name = "Компот от круши",
            Description = "Ароматен компот от круши с фин вкус на карамфил.",
            Ingredients = "Круши, захар, вода, лимонена киселина, карамфил",
            Price = 2.90m,
            Quantity = 50,
            ImagePath = "Круши.png",
            IsAvailable = true
        },

        new Product
        {
            Name = "Компот от ябълки",
            Description = "Домашен ябълков компот с деликатен аромат на джинджифил и лимон.",
            Ingredients = "Райски ябълки, бяло вино, джинджифил, лимон, вода, захар",
            Price = 2.20m,
            Quantity = 50,
            ImagePath = "Ябълки.png",
            IsAvailable = true
        }
        ,
        new Product
        {
            Name = "Компот от сливи",
            Description = "Домашен компот от сочни сини сливи с приятен аромат на канела.",
            Ingredients = "Сини сливи, захар, вода, канела",
            Price = 1.90m,
            Quantity = 50,
            ImagePath = "Сливи.png",
            IsAvailable = true
        },

        new Product
        {
            Name = "Компот от джанки",
            Description = "Освежаващ домашен компот от джанки с лек аромат на индрише.",
            Ingredients = "Джанки, индрише, захар, вода",
            Price = 2.80m,
            Quantity = 50,
            ImagePath = "Джанки.png",
            IsAvailable = true
        },

        new Product
        {
            Name = "Компот от ягоди",
            Description = "Ароматен компот от сладки ягоди, любим вкус за всеки сезон.",
            Ingredients = "Ягоди, вода, захар",
            Price = 2.50m,
            Quantity = 50,
            ImagePath = "Ягоди.png",
            IsAvailable = true
        },

        new Product
        {
            Name = "Компот от вишни",
            Description = "Домашен компот от вишни с приятен аромат на мед и ванилия.",
            Ingredients = "Вишни, лимон, ванилия, мед, вода",
            Price = 2.50m,
            Quantity = 50,
            ImagePath = "Вишни.png",
            IsAvailable = true
        },

        new Product
        {
            Name = "Компот от грозде",
            Description = "Класически домашен компот от сладко грозде с фин аромат на ванилия.",
            Ingredients = "Грозде, захар, вода, ванилия",
            Price = 2.10m,
            Quantity = 50,
            ImagePath = "Грозде.png",
            IsAvailable = true
        },
        new Product
        {
            Name = "Праскови на дядо Христо",
            Description = "Подбрани на ръка праскови по семейна рецепта, пазена с години. Любимият вкус от детството, който носи домашен уют във всеки буркан.",
            Ingredients = "Едри праскови, захар, вода, индрише, ванилия",
            Price = 4.80m,
            Quantity = 40,
            ImagePath = "ПрасковиНаДядоХристо.png",
            IsAvailable = true
        },

        new Product
        {
            Name = "Екзотична мушмула",
            Description = "Необичаен компот с богат плодов аромат и внимателно подбрани екзотични съставки. За хора, които обичат да опитват нещо различно.",
            Ingredients = "Мушмула, мед, кафява захар, ванилия, канела, лимонов сок",
            Price = 6.30m,
            Quantity = 30,
            ImagePath = "Ексотична мушмула.png",
            IsAvailable = true
        },

        new Product
        {
            Name = "Кралски компот",
            Description = "Компот, достоен за царска трапеза. Подбрани плодове с богат аромат и изискан вкус – Кралете се бият за него!",
            Ingredients = "Праскови, кайсии, круши, череши, мед, ванилия, канела, индрише",
            Price = 8.80m,
            Quantity = 20,
            ImagePath = "КралскиКомпот.png",
            IsAvailable = true
        }

    );

    context.SaveChanges();
}
        }
    }
}