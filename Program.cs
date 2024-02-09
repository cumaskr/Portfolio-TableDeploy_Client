using Aspose.Cells;

namespace Whjeon
{
    public class Program
    {
        private const string ExcelExtension = "xlsx";
        private const string JsonExtension = "json";
        private const string CopyExcelPath = "./Design";
        private const string OriginExcelPath = "..\\..\\..\\Design\\";

        /// <summary>        
        /// 1.Design 폴더내 엑셀파일들을 읽어온다
        /// 2.Json으로 변환한다.
        /// 3.이미 있는경우, 해싱값 비교하여 다른경우 덮어쓴다.
        /// </summary>        
        public static void Main(string[] args)
        {
            Console.WriteLine("Excel 변환 시작");
            if (Directory.Exists(CopyExcelPath)) 
            {
                var directoryInfo = new DirectoryInfo(CopyExcelPath);
                var fileInfos = directoryInfo.GetFiles();
                foreach ( var fileInfo in fileInfos ) 
                {
                    var splits = fileInfo.Name.Split('.');
                    if (splits[1] != ExcelExtension)
                        continue;
                    var wb = new Workbook($"{CopyExcelPath}\\{fileInfo.Name}");
                    //TODO 3번기능
                    wb.Save($"{OriginExcelPath}{splits[0]}.{JsonExtension}", SaveFormat.Json);
                    wb.Dispose();
                }
            }
            else
            {
                Console.WriteLine("Design폴더가 존재하지 않습니다.");
                return;
            }
            Console.WriteLine("Excel 변환 종료");
        }
    }
}