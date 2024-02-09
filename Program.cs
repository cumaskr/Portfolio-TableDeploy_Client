using Aspose.Cells;

namespace Whjeon
{
    public class Program
    {

        /// <summary>        
        /// 1.Design 폴더내 엑셀파일들을 읽어온다
        /// 2.Json으로 변환한다.
        /// 3.이미 있는경우, 비교하여 다르다면 덮어쓴다.
        /// </summary>
        public static void Main(string[] args)
        {
            Console.WriteLine(Directory.Exists("Hello"));            
        }
    }
}