using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CourseWorkVS.BusinessLogic
{
	/// <summary>
	/// Расчет инвертированного индекса для массива файлов
	/// </summary>
	public interface IindexCalculation
    {
		Dictionary<string, List<string>> Calculate(IEnumerable<string> files, int startIndex, int endIndex);
	}

    internal class IndexCalculation : IindexCalculation
    {
        public Dictionary<string, List<string>> Calculate(IEnumerable<string> files, int startIndex, int endIndex)
        {
            Console.WriteLine("Thread with start index {0} started!", startIndex);

            var resultIndex = new Dictionary<string, List<string>>();

            for(int i = startIndex; i < endIndex; i++)
            {
                var filePath = files.ElementAt(i);
                var words = new List<string>();

                using (var sr = new StreamReader(filePath))
                {
                    while (!sr.EndOfStream)
                    {
                        var stringBuffer = sr.ReadLine();
                        words.AddRange(Regex
                            .Replace(stringBuffer, @"[^\w\s]", " ")
                            .Replace("<br />", " ")
                            .ToLower()
                            .Trim()
                            .Split(' ')
                            .Distinct());
                    }
                }

                foreach (var word in words)
                {
                    if (!resultIndex.TryGetValue(word, out var positions))
                    {
                        positions = new List<string> { filePath };
                        resultIndex.Add(word, positions);
                        continue;
                    }
                    
                    resultIndex[word].Add(filePath);
                }
            }

            Console.WriteLine("Thread with start index {0} finished!", startIndex);
            Console.WriteLine("Count of origin words in one thread: {0}", resultIndex.Count);
            return resultIndex;
        }
    }
}
