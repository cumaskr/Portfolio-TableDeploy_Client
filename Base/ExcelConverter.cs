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
        public void Start() 
        {
            Console.WriteLine("Excel 변환 시작");
            if (Directory.Exists(Const.Path.BuildExcelPath))
            {
                var directoryInfo = new DirectoryInfo(Const.Path.BuildExcelPath);
                var fileInfos = directoryInfo.GetFiles();
                foreach (var fileInfo in fileInfos)
                {
                    var wb = new Workbook($"{Const.Path.BuildExcelPath}\\{fileInfo.Name}");
                    wb.Save($"{Const.Path.ProjectJsonPath}" +
                        $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}" +
                        $".{Const.FileExtension.Json}", SaveFormat.Json);
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
