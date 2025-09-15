using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wordle.Models;

namespace Wordle.Data
{
    public class WordleContext : DbContext
    {
        public WordleContext (DbContextOptions<WordleContext> options)
            : base(options)
        {
        }

        public DbSet<Wordle.Models.Word> Word { get; set; } = default!;
    }
}
