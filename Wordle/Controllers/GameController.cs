using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.Json;

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
        private static readonly List<string> WordList = new() { "apple", "grape", "peach", "lemon", "mango" };
        private const string TargetWordSessionKey = "TargetWord";
        private const string GuessesSessionKey = "Guesses";
        private const string WordIndexSessionKey = "WordIndex";

        private string GetTargetWord()
        {
            var word = HttpContext.Session.GetString(TargetWordSessionKey);
            if (string.IsNullOrEmpty(word))
            {
                HttpContext.Session.SetInt32(WordIndexSessionKey, 0);
                word = WordList[0];
                HttpContext.Session.SetString(TargetWordSessionKey, word);
            }
            return word;
        }

        private void SetNextWord()
        {
            int index = HttpContext.Session.GetInt32(WordIndexSessionKey) ?? 0;
            index = (index + 1) % WordList.Count;
            HttpContext.Session.SetInt32(WordIndexSessionKey, index);
            HttpContext.Session.SetString(TargetWordSessionKey, WordList[index]);
            HttpContext.Session.Remove(GuessesSessionKey);
        }

        private List<List<(char Letter, string Color)>> GetGuesses()
        {
            return HttpContext.Session.GetObject<List<List<(char, string)>>>(GuessesSessionKey) ?? new List<List<(char, string)>>();
        }

        private void SaveGuesses(List<List<(char, string)>> guesses)
        {
            HttpContext.Session.SetObject(GuessesSessionKey, guesses);
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
            var targetWord = GetTargetWord();
            var guesses = GetGuesses();

            if (!string.IsNullOrEmpty(guess) && guess.Length == targetWord.Length)
            {
                var result = new List<(char, string)>();
                var targetChars = targetWord.ToCharArray();
                var guessChars = guess.ToLower().ToCharArray();
                var used = new bool[targetWord.Length];

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

                // If the guess is correct, move to the next word
                if (guess.ToLower() == targetWord)
                {
                    SetNextWord();
                    guesses = new List<List<(char, string)>>();
                    TempData["Message"] = "Correct! Moving to the next word.";
                }

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