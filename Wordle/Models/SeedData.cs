using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Wordle.Data;

namespace Wordle.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new WordleContext(
                serviceProvider.GetRequiredService<DbContextOptions<WordleContext>>());

            // Look for any words already in database.
            if (context.Word.Any())
            {
                return;   // DB has been seeded
            }

            context.Word.AddRange(
                new Word { Letters = "apple", Length = 5, Created = DateTime.UtcNow },
                new Word { Letters = "grape", Length = 5, Created = DateTime.UtcNow },
                new Word { Letters = "peach", Length = 5, Created = DateTime.UtcNow },
                new Word { Letters = "lemon", Length = 5, Created = DateTime.UtcNow },
                new Word { Letters = "mango", Length = 5, Created = DateTime.UtcNow },
                new Word { Letters = "berry", Length = 5, Created = DateTime.UtcNow },
                new Word { Letters = "melon", Length = 5, Created = DateTime.UtcNow },
                new Word { Letters = "plumb", Length = 5, Created = DateTime.UtcNow },
                new Word { Letters = "olive", Length = 5, Created = DateTime.UtcNow },
                new Word { Letters = "guava", Length = 5, Created = DateTime.UtcNow }
            );

            context.SaveChanges();
        }
    }
}
