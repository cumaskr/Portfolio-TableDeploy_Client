using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalTableBuilder
{

    /// <summary>
    /// FireBase(저장소) 클래스[whjeon 24.02.12]
    /// 사용용도 : 테이블 버전별 Json 저장, 서버에 파일 저장 시 알림 날림
    /// 사용방법
    /// 1)path 경로에 저장소에서 받은 프로젝트 키(Json)파일 추가
    /// 2)해당파일 빌드 시, 포함되도록 변경
    /// 3)해당 클래스 사용    
    /// </summary>
    public class FireBase
    {
        //저장소 접근하기위한 Json파일명(해당 파일은 깃에 올라가지 않음, 특정 컴퓨터 내부에서 관리)
        private string AdminSdkJson = "tablebuild-e6f20-firebase-adminsdk-xx0ap-d795853497.json";
        private string ProjectId = "tablebuild-e6f20";
        //저장소 객체
        public FirestoreDb Db { get; set; }
        //생성자(초기화)
        public FireBase()
        {
            string path = $"{Const.Path.TablePath}\\FireBase\\{AdminSdkJson}";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            Db = FirestoreDb.Create(ProjectId);
        }
    }
}
