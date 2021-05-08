﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CourseWorkVS.BusinessLogic
{
    /// <summary>
    /// Класс, инкапсулирующий всю логику расчета инвертированного индекса
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

        public void Exec()
        {
            var path = "C://LAB_3_COURSE_2sem/Paral/CourseWork/datasets/train/";

            string[] files;
            if (Directory.Exists(path))
            {
                files = Directory.GetFiles(path);
            }
            else
            {
                Console.WriteLine("{0} is not a valid directory.", path);
                return;
            }
            
            //foreach(var file in files)
            //{
            //    Console.WriteLine($"{file}");
            //}

            Console.Write("Enter a count of threads (from 1 to 10): ");
            var countOfTheads = Convert.ToInt32(Console.ReadLine());

            if(countOfTheads == 1)
            {
                var indexCalculation = new IndexCalculation();
                updateResultIndex(indexCalculation.Calculate(files, 0, files.Length));
            }

            else
            {
                var tasks = new Task<Dictionary<string, IEnumerable<string>>>[countOfTheads];
                for(var taskId = 0; taskId < countOfTheads; taskId++)
                {
                    var indexCalculation = new IndexCalculation();

                    var startIndex = files.Length / countOfTheads * taskId;
                    var endIndex = taskId == countOfTheads - 1 ? files.Length : files.Length / countOfTheads * (taskId + 1);

                    tasks[taskId] = Task<Dictionary<string, IEnumerable<string>>>.
                        Factory.StartNew(() => { return indexCalculation.Calculate(files, startIndex, endIndex); });
                }

                foreach(var task in tasks)
                {
                    task.Wait();
                }

                foreach(var task in tasks)
                {
                    updateResultIndex(task.Result);
                }
            }

            
        }

        private void updateResultIndex(Dictionary<string, IEnumerable<string>> itemsToAdd)
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
    }
}