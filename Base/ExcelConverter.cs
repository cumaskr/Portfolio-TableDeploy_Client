using Aspose.Cells;

namespace LocalTableBuilder
{
    /// <summary>        
    /// 1.Design 폴더내 엑셀파일들을 읽어온다
    /// 2.Json으로 변환한다.
    /// 3.이미있는파일이면 덮어쓰게 되고, Git에 의해 변경된파일 커밋
    /// </summary>
    public class ExcelConverter
    {
        private const string ExcelExtension = "xlsx";
        private const string JsonExtension = "json";
        private const string CopyExcelPath = "./Design\\Excel";
        private const string OriginExcelPath = "..\\..\\..\\Design\\Json\\";

        public void Start() 
        {
            Console.WriteLine("Excel 변환 시작");
            if (Directory.Exists(CopyExcelPath))
            {
                var directoryInfo = new DirectoryInfo(CopyExcelPath);
                var fileInfos = directoryInfo.GetFiles();
                foreach (var fileInfo in fileInfos)
                {                    
                    var wb = new Workbook($"{CopyExcelPath}\\{fileInfo.Name}");
                    wb.Save($"{OriginExcelPath}" +
                        $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}" +
                        $".{JsonExtension}", SaveFormat.Json);
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
