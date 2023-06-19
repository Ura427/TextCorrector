using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.Diagnostics;
using TextCorrector.Models;

namespace TextCorrector.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }







        [HttpPost]
        public IActionResult Correct(TextFieldValue textFieldValue)
        {
            HashSet<string> wordSet = new HashSet<string>
            {
                "I",
                "he",
                "she",
                "it",
                "want",
                "to",
                "work",
                "properly"
            };


            string[] words = textFieldValue.Text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            List<string> invalidWords = new List<string>();
            //Dictionary<int, string> invalidWords2 = new Dictionary<int, string>();
            OrderedDictionary orderedDictionary = new OrderedDictionary();


            //Get the list of words, that are absent in dictionary 
            foreach (string word in words) { 
                if (!wordSet.Contains(word))
                {
                    invalidWords.Add(word);
                }
            }

            for (int i = 0; i < words.Length; i++)
            {
                if (!wordSet.Contains(words[i]))
                {
                    //invalidWords2.Add(i, words[i]);
                    orderedDictionary.Add(i, words[i]);
                }
            }

            List<string> correctedWords = new List<string>();

            //Iterating through every irregular word 
            foreach(string word in orderedDictionary.Values)
            {
                int mostMatches = 0;
                string correctedWord = default;
                var charactersInWord = word.ToCharArray(); 



                //Iterating through every word in dictionary
                foreach (string word2 in wordSet)
                {

                    //Doesn't match the words with high character number difference
                    if (Math.Abs(word2.Length - word.Length) > 2)
                    {
                        continue;
                    }

                    int matches = 0;
                    var charactersInWord2 = word2.ToCharArray();
                    //Iterating through every symbol in word from wordSet
                    for (int i = 0; i < charactersInWord2.Length; i++)
                    {
                        //Handle the Index out of range exception
                        if(i >= charactersInWord.Length)
                        {
                            break;
                        }

                        if (charactersInWord[i] == charactersInWord2[i])
                        {
                            matches++;
                            //Changing the most suitable word from dictionary instead of invalid one
                            if(matches > mostMatches)
                            {
                                mostMatches = matches;
                                correctedWord = word2;
                            }
                        }
                    }
                }

                correctedWords.Add(correctedWord);
            }

            //Changing irregular words in dictionary into right ones
            for (int i = 0; i < orderedDictionary.Count; i++)
            {
                orderedDictionary[i] = correctedWords[i];
            }


            //Replacing irregular words in original sentence
            for (int i = 0; i < orderedDictionary.Count; i++)
            {
                words[(int)orderedDictionary.Cast<System.Collections.DictionaryEntry>().ElementAt(i).Key] = orderedDictionary[i].ToString();
            }

            return View("Index");
        }
       
    }
}