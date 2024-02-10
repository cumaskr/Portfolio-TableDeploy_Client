using Aspose.Cells;

namespace Whjeon
{
    public class Program
    {
        private const string ExcelExtension = "xlsx";
        private const string JsonExtension = "json";
        private const string CopyExcelPath = "./Design";
        private const string OriginExcelPath = "..\\..\\..\\Design\\";

        //두 파일이 같은 내용인지?
        //private static bool IsDifferentFile(FileInfo beforeFileInfo, FileInfo nowFileInfo) 
        //{
        //    MD5 md5 = System.Security.Cryptography.MD5.Create();
        //    var beforeHash = BitConverter.ToString(md5.ComputeHash(beforeFileInfo.OpenRead()));
        //    var nowHash = BitConverter.ToString(md5.ComputeHash(nowFileInfo.OpenRead()));
        //    md5.Dispose();
            
        //    return (beforeHash != nowHash);
        //}
    
        /// <summary>        
        /// 1.Design 폴더내 엑셀파일들을 읽어온다
        /// 2.Json으로 변환한다.
        /// 3.이미있는파일이면 덮어쓰게 되고, Git에 의해 변경된파일 커밋
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