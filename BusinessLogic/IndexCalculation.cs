using System;
using System.Collections.Generic;

namespace CourseWorkVS.BusinessLogic
{
	/// <summary>
	/// Расчет инвертированного индекса для массива файлов
	/// </summary>
	public interface IindexCalculation
    {
		Dictionary<string, IEnumerable<string>> Calculate(IEnumerable<string> files, int startIndex, int endIndex);
	}

    internal class IndexCalculation : IindexCalculation
    {
        public Dictionary<string, IEnumerable<string>> Calculate(IEnumerable<string> files, int startIndex, int endIndex)
        {
            throw new NotImplementedException();
        }
    }
}
