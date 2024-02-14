using Amazon;
using Amazon.S3;
using dotenv.net;

namespace LocalTableBuilder
{
    /// <summary>
    /// AWS S3(저장소) 클래스[whjeon 24.02.14]
    /// 사용용도 : 테이블 Json 저장, 서버에서 해당 테이블을 읽어 데이터 사용
    /// 사용방법
    /// 1).env 파일에 AWS 관련 인증정보 수동 입력(GitHub에는 빈파일 올라감)
    /// 2)해당 클래스 사용
    public class AWSS3 : IDisposable, IExternalStore
    {
        //AWS 서비스 사용하기위한 클라이언트 객체
        public AmazonS3Client? Client { get; set; } = null;

        public AWSS3()
        {
            try
            {
                Console.WriteLine("[AWS] Start Setting");

                //환경변수 불러오기
                DotEnv.Load();
                //클라이언트 생성
                Client = new AmazonS3Client(
                    Environment.GetEnvironmentVariable("AWS_ID"),
                    Environment.GetEnvironmentVariable("AWS_Key"),
                    new AmazonS3Config
                    {
                        RegionEndpoint = RegionEndpoint.APNortheast2,
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Client = null;
                return;
            }

            Console.WriteLine("[AWS] Ok");
        }

        //리소스 정리 - using or 사용후 꼭 수동 호출 해주세요!
        public void Dispose()
        {
            if (null != Client)
            {
                Client.Dispose();
            }
        }

        //연결 확인
        public bool IsConnect()
        {
            Console.WriteLine("[AWSS3] Check Connect");

            if (null == Client)
            {
                return false;
            }
            else
            {
                //API 호출한 후 연결상태 확인
                var response = Client.ListBucketsAsync().GetAwaiter().GetResult();
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("[AWS] Connect Fail");
                    return false;
                }

                Console.WriteLine("[AWSS3] Ok");

                return true;
            }
        }
    }
}