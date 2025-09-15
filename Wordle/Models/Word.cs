using System;

namespace Wordle.Models
{
    public class Word
    {
        public int Id { get; set; }
        public string Letters { get; set; } = string.Empty;
        public int Length { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
}