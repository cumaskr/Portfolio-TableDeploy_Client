using Aspose.Cells.Charts;
using Google.Cloud.Firestore;
using Google.Protobuf.Compiler;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;

namespace LocalTableBuilder
{    
    public class TableBuild
    {
        //최신버전 목록- 캐릭터 테이블에 저장된 버전별 파일이 1.json, 2.json이 있다면 2가 담김
        private Dictionary<string, int>? RecentVersions = null;
        //최신버전 목록 경로
        private const string VersionFilePath = $"{Const.Path.TablePath}Versions.{Const.FileExtension.Json}";

        #region Util

        //두 파일이 다른지?
        private bool IsDifferentFile(FileInfo beforeFileInfo, FileInfo nowFileInfo)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            var beforeHash = BitConverter.ToString(md5.ComputeHash(beforeFileInfo.OpenRead()));
            var nowHash = BitConverter.ToString(md5.ComputeHash(nowFileInfo.OpenRead()));
            md5.Dispose();

            return (beforeHash != nowHash);
        }

        //테이블 폴더 밑의 Json 폴더명 경로 - ex)Character/1.json 에서 Character
        private string GetTableFolderPath(string folderName)
        {
            //Table폴더 하위 파일이라는 뜻으로 table t 축약형으로 네이밍
             return $"{Const.Path.TablePath}{folderName}";            
        }

        //테이블 폴더 밑의 Json 경로 - ex)Character/1.json 에서 1.json
        private string GetTableJsonPath(string folderName, int version) 
        {
            //Table폴더 하위 파일이라는 뜻으로 table t 축약형으로 네이밍
            return $"{GetTableFolderPath(folderName)}\\{version}.{Const.FileExtension.Json}";
        }

        #endregion

        #region Text

        //테이블별 최신버전 목록 저장
        private void SaveVersions(Dictionary<string, int> versions)
        {
            File.WriteAllText(VersionFilePath, JsonConvert.SerializeObject(versions));
        }

        //테이블별 최신버전 목록 가져오기
        private Dictionary<string, int>? LoadVersions()
        {
            var str = File.ReadAllText(VersionFilePath);
            if (null != str)
            {
                try
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, int>>(str);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return null;
        }
        
        //테이블 빌드 시, 에러나 버그 상황 Txt 저장
        private void SaveError(string errorTitle, string fileName, int version)
        {
            //폴더생성
            var errorPath = $"{Const.Path.TablePath}Error";
            //에러 저장
            Directory.CreateDirectory(errorPath);
            var errorMsg = $"ERROR|{errorTitle} - FileName({fileName}), Version({version}), {DateTime.UtcNow}";
            Console.WriteLine(errorMsg);
            File.AppendAllText($"{errorPath}\\Log.txt", $"{errorMsg}\n");
        }

        #endregion

        public void Start() 
        {
            Console.WriteLine("테이블 빌드 시작");

            //--------------------------------------------------------------------------------0.외부 저장소 셋팅
            //AWS 객체 생성
            using var aws = new AWSS3();
            if (false == aws.IsConnect())
                return;
            //FireBase 객체 생성
            using var fb = new FireBase();
            if (false == fb.IsConnect())
                return;

            //--------------------------------------------------------------------------------1.버전관리 파일이 있다면 로드 or 초기화
            Console.WriteLine("테이블 빌드 중");

            if (File.Exists(VersionFilePath))
            {
                RecentVersions = LoadVersions();
            }
            if (null == RecentVersions)
                RecentVersions = new Dictionary<string, int>();

            //--------------------------------------------------------------------------------2.프로젝트 Json 파일들 순회
            var directoryInfo = new DirectoryInfo(Const.Path.ProjectJsonPath);
            var fileInfos = directoryInfo.GetFiles();
            foreach (var projectJsonFile in fileInfos)
            {
                var projectJsonFolderName = Path.GetFileNameWithoutExtension(projectJsonFile.Name);
                //--------------------------------------------------------------------------------3.해당 파일의 최신버전 조회
                var version = 0;
                if (RecentVersions.TryGetValue(projectJsonFolderName, out var value))
                {
                    version = value;
                }
                //--------------------------------------------------------------------------------4.해당 파일 검증
                //버전 기록 없는경우
                if (0 == version)
                {
                    //폴더생성
                    Directory.CreateDirectory(GetTableFolderPath(projectJsonFolderName));
                    //검증
                    var firstVersion = version + 1;
                    var tJsonPath = GetTableJsonPath(projectJsonFolderName, firstVersion);
                    //예외처리)버전과 파일상태가 맞지않은 경우(버그)
                    if (File.Exists(tJsonPath))
                    {
                        SaveError("AlreadyFileExist", projectJsonFolderName, firstVersion);
                        continue;
                    }
                }
                //버전 기록 있는경우
                else
                {
                    //검증
                    var tJsonPath = GetTableJsonPath(projectJsonFolderName, version);
                    if (File.Exists(tJsonPath))
                    {
                        //기존(테이블 Json)과 비교할(프로젝트 Json)이 같다면
                        if (false == IsDifferentFile(new FileInfo(tJsonPath), projectJsonFile))
                        {
                            //아무처리 하지않고 패스
                            continue;
                        }
                    }
                    else
                    {
                        //예외처리)버전과 파일상태가 맞지않은 경우(버그)
                        SaveError("FileNotExist", projectJsonFolderName, version);
                        continue;
                    }
                }
                //--------------------------------------------------------------------------------5.버전증가 및 최종경로 재갱신 + 내부/외부 테이블 저장
                //내부 경로 저장(버전별 파일 저장하여, 테이블 빌드 진행 상황을 컨트롤 하기 위함)
                var tableJsonPath = GetTableJsonPath(projectJsonFolderName, ++version);
                projectJsonFile.CopyTo(tableJsonPath);
                //AWS S3 - 새로 생성된 버전의 파일 업로드
                aws.UploadTableFile(tableJsonPath, projectJsonFolderName, version.ToString());
                //--------------------------------------------------------------------------------6.최신버전 목록 갱신
                if (0 < version)
                {
                    if (RecentVersions.TryGetValue(projectJsonFolderName, out var pre))
                    {
                        RecentVersions[projectJsonFolderName] = version;
                    }
                    else
                    {
                        RecentVersions.Add(projectJsonFolderName, version);
                    }
                }
            }
            //-------------------------------------------------------------------------------7.최신버전 목록 저장 및 파이어베이스 갱신
            if (0 < RecentVersions.Keys.Count)
            {
                //최신버전 목록 저장
                SaveVersions(RecentVersions);

                //파이어베이스 최신버전 목록 갱신
                fb.UploadTableVersions(RecentVersions);
            }

            Console.WriteLine("테이블 빌드 완료");
        }
    }
}