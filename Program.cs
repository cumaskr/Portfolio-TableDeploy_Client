using Aspose.Cells;
using LocalTableBuilder;

namespace Whjeon
{
    public class Program
    {
        public enum BuildOption
        {
            Excel = 1,  //엑셀 > Json 변경하는 빌드
            Table = 2,  //Json > 
        }

        public static void Main(string[] args)
        {
            //옵션 없이 실행할 경우 - 원하는 로직 실행
            if (args.Length <= 0)
            {
                //자유 로직 
                Console.Write($"빌드 옵션 Excel - {(int)BuildOption.Excel}, Table - {(int)BuildOption.Table}");
            }
            else
            {
                try
                {
                    if ((int)BuildOption.Excel == Convert.ToInt32(args[0]))
                    {
                        var ec = new ExcelConverter();
                        ec.Start();
                    }
                    else if ((int)BuildOption.Table == Convert.ToInt32(args[0]))
                    {
                        var tb = new TableBuild();
                        tb.Start();
                    }
                    else
                    {
                        Console.Write($"지원하지 않는 빌드 옵션입니다. Excel - {(int)BuildOption.Excel}, Table - {(int)BuildOption.Table}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }            
        }
    }
}