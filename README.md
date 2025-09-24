JacoBank 網路銀行專案
專案概述
JacoBank 是一個模擬整合網路銀行的範例系統，主要功能包含：

使用者註冊 / 登入：使用者可以註冊、設定密碼並登入系統。密碼採用 BCrypt 雜湊。
帳戶管理：查看個人帳戶列表、轉帳交易明細等等。
交易查詢：根據日期、帳號、交易類型（存款/提款/跨行）進行篩選查詢。
資金轉帳：使用者可以從下拉選單選擇轉出帳戶，輸入收款銀行與帳號並確認轉帳後程式會呼叫 Web API 完成轉帳。
跨專案 API：JacoBankAPI 專案提供轉帳 RESTful API，並在 JacoDemo 前端透過 HttpClient 串接。
專案架構
依據目前程式檔與模組功能，建議將解耦後的專案分成三個主要專案與一個解決方案：

JacoDemo.sln
├── JacoDemo.Web (ASP.NET Core MVC 前端)
│   ├─ Controllers           # MVC 控制器
│   ├─ Views                 # Razor 視圖
│   ├─ wwwroot               # 靜態資源 (css/js/images)
│   └─ Program.cs / Startup.cs
├── JacoBankAPI (ASP.NET Core Web API)
│   ├─ Controllers           # API 控制器
│   ├─ Services              # 業務邏輯層 (轉帳、帳戶)
│   ├─ Models                # 請求/回應 Model
│   └─ Program.cs / Startup.cs
├── JacoDemo.Core (Class Library)
│   ├─ Entities              # EF Core 實體類別 (CustomerInfo, CustomerBankInfo, CustomerTransInfo, PasswordHistory)
│   ├─ Interfaces            # Repository / Service 介面定義
│   └─ ViewModels            # 前端專案共用的 ViewModel 定義
└── JacoDemo.Infrastructure (Class Library)
    ├─ Data                  # DbContext 與 OnModelCreating
    ├─ Repositories          # EF Core Repository 實作
    └─ Migrations            # EF Core Migrations 檔案
層次優勢（以 MVC 思維）
Model（核心資料與業務邏輯）

JacoDemo.Core／Infrastructure-Data：集中定義資料實體（Entity）與商業規則（Service／Repository），不受 UI 或 API 影響
好處：任何改變（新增欄位、調整驗證、改寫查詢）只要在這層完成，下層與上層不必修改
View（使用者介面展示）

JacoDemo.Web：負責呈現畫面（Razor Views、靜態資源），與使用者互動，完全不需要知道資料怎麼取得
好處：設計師／前端工程師可專注 CSS/JS／視覺改版，不會碰到業務或資料存取程式碼
Controller（流程協調與請求路由）

JacoDemo.Web Controller／JacoBankAPI Controller：
前端 Controller 處理使用者請求、表單驗證、呼叫 Core 或 API
API Controller 處理 RESTful 請求、回傳 JSON，無 UI 樣板
好處：
一眼就能看出哪裡接收使用者輸入、哪裡扮演“橋樑”把資料從 Model 送到 View
跟 UI、跟資料存取的程式碼都分離，便於測試與維護
獨立部署與彈性擴充

Web 與 API 各自成為可獨立運行的模組，可分別水平擴充（多台 Web 伺服器、API 集群）
好處：在流量暴增或維護中，只需針對某一層做容器或服務的滾動重啟，其他層不受影響
測試友善

Core 定義清楚的介面（Interface），上層用依賴注入（DI）引用
可輕易對 Controllers／Services 做單元測試與模擬（Mock）
好處：測試時不需啟動整個資料庫或 UI，縮短測試時間、提升覆蓋率
責任分離（Separation of Concerns）

每一層只專注自己的職責：
Model → 資料結構與商業邏輯
View → 呈現與互動
Controller → 協調流程
好處：當你新增一個功能（例如新報表或第三方 API 串接），只要在對應層新增，減少全域修改範圍
Infrastructure 層 (JacoDemo.Infrastructure) 封裝資料存取實作
高可維護性：功能新增或修改只需在對應層次中進行，不會交叉影響
易於測試：Core 定義介面後，可對 Infrastructure 與 API/Web 層進行單元測試與模擬測試
