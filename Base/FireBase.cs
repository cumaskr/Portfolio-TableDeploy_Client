using Google.Cloud.Firestore;

namespace LocalTableBuilder
{
    /// <summary>
    /// FireBase(저장소) 클래스[whjeon 24.02.12]
    /// 사용용도 : 테이블 버전별 Json 저장, 서버에 파일 저장 시 알림 날림
    /// 사용방법
    /// 1)FireBase.json 파일에 인증값 수동으로 입력(GitHub에는 빈파일 올라감)
    /// 2)해당 클래스 사용
    /// </summary>
    public class FireBase : IDisposable, IExternalStore
    {
        //저장소 접근하기위한 Json파일명(해당 파일은 깃에 올라가지 않음, 특정 컴퓨터 내부에서 관리)
        private string AdminSdkJson = "FireBase.json";
        private string ProjectId = "tablebuild-e6f20";
        private string Server = "Local";
        private string DocumentName = "Table";

        //저장소 객체
        public FirestoreDb? Db { get; set; }

        //생성자(초기화)
        public FireBase()
        {
            Console.WriteLine("[FireBase] Start Setting");

            Db = null;
            string path = $"./{AdminSdkJson}";

            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                Db = FirestoreDb.Create(ProjectId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Db = null;
                return;
            }
        }

        //리소스 정리
        public void Dispose()
        {
            Db = null;
        }

        //연결 확인
        public bool IsConnect()
        {
            Console.WriteLine("[FireBase] Check Connect");

            if (null == Db)
            {
                return false;
            }
            else
            {
                try
                {
                    var collection = Db.Collection(Server).Document("Test");
                    collection.SetAsync(new Dictionary<string, int>()).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            return true;
        }

        #region TableBuild

        //테이블 최신 버전 업로드
        public void UploadTableVersions(Dictionary<string, int> recentVersions)
        {
            if (null != Db)
            {
                var collection = Db.Collection(Server).Document(DocumentName);
                collection.SetAsync(recentVersions).GetAwaiter().GetResult();

                Console.WriteLine("[FireBase] Upload Recent Versions");

            }
        }

        #endregion
    }
}