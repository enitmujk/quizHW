using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class QuizQuestion
{
    public string Question { get; set; }
    public List<string> Options { get; set; }
    public string Answer { get; set; }
    public int? SelectedOptionIndex { get; set; } //nullable int to represent if an option was selected
}

class Program
{
    static async Task Main()
    {
        string jsonUrl = "https://raw.githubusercontent.com/mujkenit/Small-Quiz-Application/main/pitanja.json";

        try
        {
            string jsonData = await DownloadJsonDataAsync(jsonUrl);

            if (!string.IsNullOrEmpty(jsonData))
            {
                Console.WriteLine("Press enter to start the Quiz!");
                Console.ReadLine();

                PrintRandomizedQuestionsWithAnswers(jsonData);
            }
            else
            {
                Console.WriteLine("Failed to download JSON data.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static async Task<string> DownloadJsonDataAsync(string url)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }

    static void PrintRandomizedQuestionsWithAnswers(string jsonData)
    {
        //parses JSON data
        JObject json = JObject.Parse(jsonData);

        //accesses the "Quiz" property and the "questions" array
        JArray questionsArray = (JArray)json["Quiz"]["questions"];

        if (questionsArray != null)
        {
            //randomizes the order of questions using Fisher-Yates shuffle algorithm
            List<QuizQuestion> randomizedQuestions = new List<QuizQuestion>();

            foreach (var questionToken in questionsArray)
            {
                var question = questionToken.ToObject<QuizQuestion>();
                randomizedQuestions.Add(question);
            }

            Random rand = new Random();
            int n = randomizedQuestions.Count;

            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                var value = randomizedQuestions[k];
                randomizedQuestions[k] = randomizedQuestions[n];
                randomizedQuestions[n] = value;
            }

            //flag to check if it's the first question
            bool firstQuestion = true;

            //variables to track correct and incorrect answers
            int correctAnswers = 0;
            int incorrectAnswers = 0;

            //iterates through each randomized question
            for (int i = 0; i < randomizedQuestions.Count; i++)
            {
                
                if (!firstQuestion)
                {
                    Console.WriteLine("Press enter to move on to the next question!");
                    Console.ReadLine();
                }
                else
                {
                    firstQuestion = false;
                }

                Console.WriteLine($"{i + 1}. {randomizedQuestions[i].Question}");

                //prints answers
                for (int j = 0; j < randomizedQuestions[i].Options.Count; j++)
                {
                    Console.WriteLine($"   {char.ConvertFromUtf32(97 + j)}. {randomizedQuestions[i].Options[j]}");
                }

                //forces user to input
                string userAnswer;
                do
                {
                    Console.Write("Your answer (a, b, c, d): ");
                    userAnswer = Console.ReadLine();

                    if (userAnswer == null || userAnswer.Length != 1 || !"abcd".Contains(userAnswer.ToLower()))
                    {
                        Console.WriteLine("Invalid answer. Please enter a valid option (a, b, c, d).");
                    }
                } while (userAnswer == null || userAnswer.Length != 1 || !"abcd".Contains(userAnswer.ToLower()));

                int selectedOptionIndex = userAnswer.ToLower()[0] - 'a';
                randomizedQuestions[i].SelectedOptionIndex = selectedOptionIndex;

                //checks if the user's answers are correct
                if (selectedOptionIndex == randomizedQuestions[i].Options.FindIndex(opt => opt.ToLower() == randomizedQuestions[i].Answer.ToLower()))
                {
                    Console.WriteLine("Correct!");
                    correctAnswers++;
                }
                else
                {
                    Console.WriteLine($"Incorrect. The correct answer is {randomizedQuestions[i].Answer.ToUpper()}.");
                    incorrectAnswers++;
                }

                
                Console.WriteLine();
            }

            //pritns the results
            Console.WriteLine($"You have finished this quiz :)");
            Console.WriteLine($"Number of Correct Answers: {correctAnswers}");
            Console.WriteLine($"Number of Incorrect Answers: {incorrectAnswers}");
        }
        else
        {
            Console.WriteLine("No questions found in the JSON data.");
        }
    }
}
