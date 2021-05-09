using System;

namespace CourseWorkVS.BusinessLogic
{
    class Launcher
    {
        public static void Main(string[] args)
        {
            var indexCalculateStrategy = new IndexCalculateStrategy();
            if(!indexCalculateStrategy.Exec())
            {
                Console.WriteLine("Error during of the building an index!");
            }
        }
    }
}