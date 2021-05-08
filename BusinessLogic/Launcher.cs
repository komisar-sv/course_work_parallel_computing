namespace CourseWorkVS.BusinessLogic
{
    class Launcher
    {
        public static void Main(string[] args)
        {
            var indexCalculateStrategy = new IndexCalculateStrategy();
            indexCalculateStrategy.Exec();
        }
    }
}