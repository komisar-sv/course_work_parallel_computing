using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CourseWorkVS.BusinessLogic
{
    /// <summary>
    /// Класс, инкапсулирующий всю логику работы с инвертированным индексом
    /// </summary>
	public class IndexCalculateStrategy
	{
        /// <summary>
        /// Инвертированный индекс
        /// </summary>
        public ConcurrentDictionary<string, BlockingCollection<string>> Index { get; private set; }

        public IndexCalculateStrategy()
        {
            Index = new ConcurrentDictionary<string, BlockingCollection<string>>();
        }

        public bool Exec()
        {
            var path = "C://LAB_3_COURSE_2sem/Paral/CourseWork/datasets/train/";

            string[] files;
            if (!Directory.Exists(path))
            {
                Console.WriteLine("{0} is not a valid directory.", path);
                return false;
            }
                
            files = Directory.GetFiles(path);

            Console.Write("Enter a count of threads (from 1 to 100): ");
            var countOfTheads = Convert.ToInt32(Console.ReadLine());

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            if(countOfTheads == 1)
            {
                var indexCalculation = new IndexCalculation();
                updateResultIndex(indexCalculation.Calculate(files, 0, files.Length));
            }
            else
            {
                multiThreadCalc(countOfTheads, files);
            }

            stopWatch.Stop();
            var time = stopWatch.Elapsed.Milliseconds;
            Console.WriteLine("Time: {0} ms", time);

            Console.WriteLine("Count of origin words: {0}", Index.Count);
            saveResultFile();


            // использование индекса
            while (true)
            {
                Console.Write("Write words you want to find in files via space: ");
                var searchingWord = Console.ReadLine().Split(' ');
                var filesContains = multiThreadFinding(searchingWord);

                if (filesContains.Count != 0)
                {
                    foreach (var wordToFind in filesContains)
                    {
                        Console.WriteLine(wordToFind.Key);

                        if(wordToFind.Value.Count == 0)
                        {
                            Console.WriteLine("This word is not contained in the index!");
                            continue;
                        }

                        foreach (var file in wordToFind.Value)
                        {
                            Console.WriteLine(file);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No files were found containing these words..");
                }

                Console.Write("For exiting press 0 or another for continuing: ");
                if (Console.ReadLine() == "0")
                {
                    break;
                }

                Console.WriteLine();
            }

            return true;
        }

        private void multiThreadCalc(int countOfTheads, string[] files)
        {
            var tasks = new Task<Dictionary<string, List<string>>>[countOfTheads];
            for (var taskId = 0; taskId < countOfTheads; taskId++)
            {
                var indexCalculation = new IndexCalculation();

                var startIndex = files.Length / countOfTheads * taskId;
                var endIndex = taskId == countOfTheads - 1 ? files.Length : files.Length / countOfTheads * (taskId + 1);

                tasks[taskId] = Task<Dictionary<string, List<string>>>.
                    Factory.StartNew(() => { return indexCalculation.Calculate(files, startIndex, endIndex); });
            }

            foreach (var task in tasks)
            {
                task.Wait();
            }

            foreach (var task in tasks)
            {
                updateResultIndex(task.Result);
            }
        }

        private Dictionary<string, BlockingCollection<string>> multiThreadFinding(string[] wordsToFind)
        {
            var tasks = new Task<BlockingCollection<string>>[wordsToFind.Length];
            for (var taskId = 0; taskId < wordsToFind.Length; taskId++)
            {
                var index = taskId;
                tasks[taskId] = Task<BlockingCollection<string>>.Factory.StartNew(() => { return searchInIndex(wordsToFind[index]); });
            }

            foreach (var task in tasks)
            {
                task.Wait();
            }

            var wordContains = new Dictionary<string, BlockingCollection<string>>();
            for (var taskId = 0; taskId < wordsToFind.Length; taskId++)
            {
                wordContains.Add(wordsToFind[taskId], tasks[taskId].Result);
            }

            return wordContains;
        }



        private void updateResultIndex(Dictionary<string, List<string>> itemsToAdd)
        {
            foreach(var item in itemsToAdd)
            {
                if (!Index.TryGetValue(item.Key, out var positions))
                {
                    positions = new BlockingCollection<string>();
                    foreach(var position in item.Value)
                    {
                        positions.TryAdd(position);
                    }

                    Index.TryAdd(item.Key, positions);
                    continue;
                }

                foreach (var position in item.Value)
                {
                    Index[item.Key].TryAdd(position);
                }
            }
        }

        private void saveResultFile()
        {
            var pathForSavingIndex = "C://LAB_3_COURSE_2sem/Paral/CourseWork/datasets/resultIndex.txt";
            using (var sw = new StreamWriter(pathForSavingIndex, false))
            {
                foreach(var indexPair in Index)
                {
                    sw.WriteLine(indexPair.Key);
                    foreach(var position in indexPair.Value)
                    {
                        sw.WriteLine(position);
                    }
                }
            }
        }

        private BlockingCollection<string> searchInIndex(string searchingWord)
        {
            if (!Index.TryGetValue(searchingWord, out var filesContains))
            {
                return new BlockingCollection<string>();
            }

            return filesContains;
        }
    }
}