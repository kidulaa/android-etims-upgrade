using EBM2x.Database.Master;
using EBM2x.Models;
using EBM2x.RraSdc;
using EBM2x.UI.i18n;
using System;

namespace EBM2x.UI
{
    public class UIManager
    {
        public static int TIME_OUT = 15000;
        public static bool SwVersionDownloadProcessRunFlag = true;

        public static bool DEBUG_MODE = false;

        //public static string AppVersion = "v20200424.NEW.0107";
        //public static string AppVersion = "v20200520.NEW.0109";
        //public static string AppVersion = "v20200721.NEW.0113";
        //public static string AppVersion = "v20200803.NEW.0115";
        //public static string AppVersion = "v20200907.NEW.0117";

        //public static string AppVersion = "v20201124.NEW.0119";
        //public static string AppVersion = "v20201208.NEW.0119";
        //public static string AppVersion = "v20210108.NEW.0119";
        //public static string AppVersion = "v20210113.NEW.0119";
        //public static string AppVersion = "v20210202.NEW.0121";
        //public static string AppVersion = "v20210222.NEW.0121";
        //public static string AppVersion = "v20210304.NEW.0123";
        //public static string AppVersion = "v20210317.NEW.0123";
        //public static string AppVersion = "v20210322.NEW.0125";
        //public static string AppVersion = "v20210331.NEW.0125";
        //public static string AppVersion = "v20210427.NEW.0127";
        //public static string AppVersion = "v20210511.NEW.0127";
        //public static string AppVersion = "v20210524.NEW.0129";
        //public static string AppVersion = "v20210609.NEW.0129";
        //public static string AppVersion = "v20210710.NEW.0129";
        //public static string AppVersion = "v20211004.NEW.0133";

        //public static string AppVersion = "v20211206.NEW.0135";

        //public static string AppVersion = "v20220908.NEW.0148";
       // public static string AppVersion = "v20221028.NEW.0149";
        public static string AppVersion = "v20230131.NEW.0100";

        //======================================================
        // "v20200318.NEW.0105";
        // "v20200424.NEW.0107";
        // 1. Taxpayer Name Length를 모두 100 byte로 변경, MySQL, SQLite Win, SQLite Tablet, SQLite PDA  

        // =====================================================
        // "v20200520.NEW.0109";
        // "v20200717.NEW.0111";

        // 1. 오프라인 사용일자 변경 1일->7일 (ok)  

        // 2. ZREPORT table create (ok)
        // 2-1. SQLite (ok)
        // 2-2. MySQL (ok)

        // 3. ZREPORT table data insert : (ok) - 0119 (ON)
        // 3-1. send ZREPORT to server (ok)

        // 3-2. ZREPORT Excel eport (tamplet, PDA)
        // 3-3. 사용자별 ZREPORT

        // 4. ReceiptSignature Tax type A,B,C로 변경 (ok)
        // 5. 다운로드 기준일자 변경 : 20150101000000 => 20200218000000 (ok)
        // 6. 납세자 패스워드 암호처리 (Backoffice, POS) (ok)
        // 7. "rcptPbctDt"에 시분초 추가  (ok)

        // 8. 외장메모리 : (ok)

        // 9. 기타 프린터 처리 : (ok)

        // 10. 소숫점 자리 *.00으로 보정 (ok)
        // 11. 고객번호 9자리로 고정 (ok)

        // 12. 서비스 상품 재고가 '-'이면 0으로 변경 (ok)
        // 13. 재고이동 중복처리 체크 (ok)
        // 14. RPT_NO 는 최초 영업일로 부터의 증분값임(2020.07.17 하부장, 데니) (ok)
        // 14-1 증분값 계산 (ok)
        // 14-2 증분값 반영 (ok)
        // 14-3 trnsSaleReceiptRecord.RptNo 반영 (ok)

        // "v20200807.NEW.0117";
        // 공지사항 숫자 표시
        // 점간이동 중복체크 처리
        // 기타 프린터 설정 처리

        // "v20201012.NEW.0119";
        // TR 파일이 잘못된 경우 제외시킴.
        // Z Report ON (ZreportMaster.cs)
        // "v20201105.NEW.0119";
        // Exported to SD Card 'Android/data/com.companyname.ebm2x/EBM2xBackup'.

        // "v20201124.NEW.0119";
        // USB 프린터 sleep 처리

        // "v20210108.NEW.0119";
        // Z Report reportnumber 처리

        // "v20210113.NEW.0119";
        // 거래 내역 백업 정리 (서버 응답오류 보정)

        // "v20210202.NEW.0121";
        // 출력 영수증 SMS 전송
        // - 모바일 프로젝트 추가 : 완료
        // - 영수증적정보 SMS 전송 모듈 : 완료
        // - 출력화면 표시 및 전화번호 입력
        // - 영수증 조회 화면에서 출력화면 표시 및 전화번호 입력
        // NVRT 처리
        // - 환경변수 추가 : 완료
        // - D Type 처리 (판매 등록 시 Read) : 완료
        // - 영수증 문구 추가 (BO 일반(80mm,50mm) : 완료, BO A4 : 완료, POS 일반(80mm,50mm) : 완료
        // - NVRT 화면 처리 (화면에 비과세 대상임을 표시) :
        // - 장비인증 API 추가 : 

        // "v20210222.NEW.0121";
        // 서버버젼이 현재 버젼 보다 낮으면 프로그램 갱신하지 않음 
        // 1.Android 7 처리
        //   - Unzip lip 변경
        // 2. zero after comma issue
        //   - 소숫점 이하 Zero 입력 처리 (소숫점 이하 2자리 입력 가능)
        // 3. A4 출력 Text 줄바꿈
        // 4. A4 출력 NS->CS, NR->CR 처리

        // 5. 사인온 영문 지원
        // 6. "VAT Flag"로 변경
        // 7. 비과세인 경우 재고 확인 막음
        // 8. SMS 전송 내용 변경

        // "v20210226.NEW.0123";
        // 1. INTERNAL_URL => EXTERNAL_URL

        // "v20210304.NEW.0123";
        // 1. 오류 처리 보강.

        // "v20210316.NEW.0123";
        // 1. 환경설정 변경.
        // 1.1 offline 일자, 금액을 고정함(readonly)
        // 2. 오류 수정
        // 3. TIN 으로 로그인
        // "v20210317.NEW.0123";
        // 4. 안드로이드 프린터 58mm 처리

        // "v20210322.NEW.0125";
        // 1. 다국어 내용 수정 및 프랑스어 추가 

        // "v20210331.NEW.0125";
        // 1. 수량을 1 ~ 999999으로 변경
        // 2. 

        // "v20210427.NEW.0127";
        // 1. 장비인증을 외부에서 가능하도록 반영

        // "v20210511.NEW.0127";
        // 1. 오류 코드가 994인 경우 송신성공으로 처리함

        // "v20210524.NEW.0129";
        // 1. TIN 과 같은 User ID를 사용하는 경우, 정상적인 로그인 처리
        // 2. Code_class, Code_dtl에 기본 값을 넣어 놓음

        // "v20210609.NEW.0129";
        // 1. 재 전송 기능 추가 (Invoice From ~ To)
        // 2. 상품 단가 

        // "v20210710.NEW.0129";
        // 1. 901인 경우 사용할 수 없음 

        // "v20210724.NEW.0131"; - (translated)EBM2.1 Client Pending Issues v0.2.docx
        // 1. 매입입고 자료 수신시 상품마스터 UPDATE 하지 않음 
        // 2. 납세자의 이름 변경 : SwVersion.TaxpayerInfoDownloadProcess
        // 3. 20자를 초과하는 USER_ID 입력 제한
        //    UserManagementPage,TabletUserManagementPopupPage,PhoneUserManagementPopupPage 
        // 4. 소수점을 지원하도록 변경되었습니다. 이전에 사용하던 "000"을 "."로 대체하였습니다
        //    AlphaKeypadPanel, KeypadPanel
        // 5. LOGO 출력
        // 6. 설치 프로그램
        // 7. 매입입고 자료 수신시 상품마스터 UPDATE 하지 않고  Acceptt시에 한다.
        //======================================================
        // "v20211206.NEW.0135"; - (translated)EBM2.1 Client Pending Issues v0.5.docx

        public static DateTime Watchdog;

        private static UILocation _UILocation = null;
        private static UIManager _UIManager = null;

        public bool IsWindows { get; set; }
        public bool IsMySQL { get; set; }
        public bool Is58mmPrinter { get; set; }

        // 2021.01 Mobile
        public bool IsMobile { get; set; }

        public PosModel PosModel { get; set; }                   // PosModel
        public InformationModel InformationModel { get; set; }   // InformationModel
        public InputModel InputModel { get; set; }               // InputModel

        public TaxpayerBhfDeviceUserRecord UserModel { get; set; }

        // 20210710 : 901인 경우 사용중지,
        public bool IsValidDevice { get; set; }

        private UIManager()
        {
            Watchdog = DateTime.Now;

            _UILocation = UILocation.Instance();

            IsWindows = false;
            IsMySQL = false;
            Is58mmPrinter = false;
            // 2021.01 Mobile
            IsMobile = false;

            IsValidDevice = true;

            PosModel = new PosModel();
            InformationModel = new InformationModel();
            InputModel = new InputModel();

            UserModel = new TaxpayerBhfDeviceUserRecord();

            if(RraSdcService.APPLICATION_NAME.Equals("RWANDA TEST SERVER") || RraSdcService.APPLICATION_NAME.Equals("KOREA TEST SERVER"))
            {
                DEBUG_MODE = true;
            }
        }

        public static UIManager Instance()
        {
            if (_UIManager == null) _UIManager = new UIManager();

            if(_UIManager.UserModel == null) _UIManager.UserModel = new TaxpayerBhfDeviceUserRecord();

            return _UIManager;
        }        
    }
}
