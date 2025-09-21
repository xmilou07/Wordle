using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Add this using directive
using System.Collections.Generic;
using System.Text.Json; // Add this using directive

namespace Wordle.Controllers
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }

    public class GameController : Controller
    {
        private const string TargetWord = "apple"; // For demo, use a static word

        // Store guesses in session for demo purposes
        private List<List<(char Letter, string Color)>> GetGuesses()
        {
            return HttpContext.Session.GetObject<List<List<(char, string)>>>("Guesses") ?? new List<List<(char, string)>>();
        }

        private void SaveGuesses(List<List<(char, string)>> guesses)
        {
            HttpContext.Session.SetObject("Guesses", guesses);
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Guesses"] = GetGuesses();
            return View();
        }

        [HttpPost]
        public IActionResult Index(string guess)
        {
            var guesses = GetGuesses();

            if (!string.IsNullOrEmpty(guess) && guess.Length == TargetWord.Length)
            {
                var result = new List<(char, string)>();
                var targetChars = TargetWord.ToCharArray();
                var guessChars = guess.ToLower().ToCharArray();
                var used = new bool[TargetWord.Length];

                // First pass: green
                for (int i = 0; i < guessChars.Length; i++)
                {
                    if (guessChars[i] == targetChars[i])
                    {
                        result.Add((guessChars[i], "green"));
                        used[i] = true;
                    }
                    else
                    {
                        result.Add((guessChars[i], null));
                    }
                }
                // Second pass: orange/red
                for (int i = 0; i < guessChars.Length; i++)
                {
                    if (result[i].Item2 == "green") continue;
                    bool found = false;
                    for (int j = 0; j < targetChars.Length; j++)
                    {
                        if (!used[j] && guessChars[i] == targetChars[j])
                        {
                            found = true;
                            used[j] = true;
                            break;
                        }
                    }
                    result[i] = (guessChars[i], found ? "orange" : "red");
                }
                guesses.Add(result);
                SaveGuesses(guesses);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid guess. Please enter a word of the correct length.");
            }

            ViewData["Guesses"] = guesses;
            return View();
        }
    }
}