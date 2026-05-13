# ESC CON-COST — PHÂN TÍCH & LỘ TRÌNH TRIỂN KHAI TOÀN DIỆN
**Vai trò:** Software Architect & Project Manager Senior
**Phiên bản:** 2.0 | **Ngày cập nhật:** 11/05/2026 | **Trạng thái:** Active

---

## 1. HIỂU & TÓM TẮT HỆ THỐNG

### 1.1. Mục tiêu chính
ESC CON-COST là **hệ thống SaaS tính toán trượt giá hợp đồng xây dựng** theo pháp luật Hàn Quốc. Hệ thống tự động hóa công thức `Kd = Σ(ai × Ki)`, giúp nhà thầu xây dựng tính toán số tiền điều chỉnh giá hợp lệ, xuất báo cáo "종합의견서" (Comprehensive Opinion Statement) và gửi yêu cầu tư vấn tới ESC Claims Manager.

### 1.2. Người dùng mục tiêu
| Loại | Mô tả | Nhu cầu chính |
|------|--------|----------------|
| **Member (Khách hàng)** | Cán bộ công trình xây dựng | Nhập hợp đồng, tính Kd, xem/tải báo cáo PDF |
| **Admin (ESC Staff)** | Nhân viên ESC / Claims Manager | Duyệt hội viên, nhập chỉ số giá, ẩn/hiện vùng dữ liệu, cấu hình email |

### 1.3. Các module chính
- **Auth & Membership:** Đăng ký, duyệt thành viên, phân quyền (Free/Paid)
- **Contract Wizard:** Nhập thông tin hợp đồng 3–4 bước
- **Calculation Engine:** Tính Kd, lưu snapshot index
- **Index Management:** Admin quản lý PPI/CPI theo tháng
- **Report & Request:** Xuất PDF, nút "Request" gửi email
- **Admin Panel:** Cấu hình Blind Data, Email Template, giới hạn sử dụng

---

## 2. LÀM RÕ YÊU CẦU

### 2.1. Điểm chưa rõ — cần xác nhận với Stakeholder

| # | Vấn đề | Mức độ ưu tiên |
|---|--------|----------------|
| 1 | **Số thập phân:** Quy định làm tròn từng bước tính (Ki, Kd) chưa được xác nhận → cần văn bản chính thức | 🔴 Critical |
| 2 | **Thanh toán:** Hình thức (1 lần / tháng / dự án) chưa xác định → ảnh hưởng schema membership | 🔴 Critical |
| 3 | **1 khách hàng = 1 dự án:** Confirm lại — có nghĩa là dữ liệu cũ bị xóa khi tạo mới? Hay chỉ lưu 1 snapshot? | 🟠 High |
| 4 | **Giới hạn Free:** Số lần dùng tối đa chưa xác định — Admin cần cấu hình được trong Settings | 🟠 High |
| 5 | **Auto-link chỉ số:** Phần nào sẽ tự động kết nối (API bên ngoài)? Source code sẽ được cung cấp khi nào? | 🟠 High |
| 6 | **OCR PDF:** Là tính năng thử nghiệm (proof-of-concept) hay production? | 🟡 Medium |
| 7 | **Revised Data:** Khi Admin cập nhật chỉ số tháng cũ → dự án cũ có tính lại không? (Hiện tại: YES - link ngay lập tức) | 🟡 Medium |
| 8 | **Provisional Sums:** Hạng mục không có đơn giá được xử lý thế nào trong công thức? | 🟡 Medium |

### 2.2. Điểm mâu thuẫn phát hiện
- **ERD vs. Development Plan:** ERD ghi 13 bảng, Development Plan ghi 14 entities (có thêm `SETTING_EMAIL_TEMPLATE`). → Cần đồng nhất: **14 entities** là con số đúng.
- **Lo_trinh_ESC vs. ESC_Development_Plan:** Lộ trình gốc (Lo_trinh) ghép Phase 2 (Wizard + Admin) thành 1 giai đoạn 8 ngày; Development Plan tách thành Phase 3 (Wizard) và Phase 6 (Admin). → Ưu tiên theo **Development Plan** vì chi tiết hơn.
- **Snapshot update logic:** Validation Q&A xác nhận index mới sẽ link ngay lập tức, nhưng không rõ liệu các `ADJUST_RECORDS` cũ có recalculate hay chỉ hiển thị warning.

---

## 3. PHÂN TÍCH HỆ THỐNG

### 3.1. Phân chia module theo cấu trúc source thực tế

#### Cấu trúc project hiện có trong `/src`

```
ESC.CONCOST (Solution)
│
├── [INFRASTRUCTURE — Hiện có]
│   ├── ESC.CONCOST.Abstract/          ← Entities, Interfaces, Services gốc
│   │   └── Entities/                  ← 18 entity đã tạo (✅ xong Task 1.1)
│   │       ├── Contract.cs, ContractItem.cs
│   │       ├── AdjustRecord.cs, AdjustItemDetail.cs
│   │       ├── IndexType.cs, IndexTimeSeries.cs
│   │       ├── InsuranceRate.cs, AdminSetting.cs
│   │       ├── Customer.cs, EscServiceRequest.cs
│   │       ├── AuditLog.cs, EntityBase.cs
│   │       ├── SettingEmailTemplate.cs
│   │       ├── SettingNotification.cs, SettingPermission.cs
│   │       └── EntityEmployee.cs, EntityNotification.cs, EntityPermission.cs
│   │
│   ├── ESC.CONCOST.Base/              ← Core framework: SA_USER, Auth, Logic
│   │   └── Entities/SA_USER.cs        ← Account system (đã có)
│   │
│   ├── ESC.CONCOST.Db/                ← EF DbContext, Migrations
│   │   └── Migrations/                ← 2 migration đã apply
│   │
│   └── ESC.CONCOST.SharedComponents/  ← Blazor shared UI components
│
├── [MODULE QUẢN TRỊ — Hiện có, mapping sang Admin Panel]
│   ├── ESC.CONCOST.ModuleManagement/
│   │   └── Services/
│   │       ├── ManagementAccountService.cs   ← Quản lý tài khoản SA_USER
│   │       ├── ManagementPermissionService.cs← Phân quyền
│   │       └── AuditLogService.cs            ← Ghi log hành động
│   ├── ESC.CONCOST.ModuleManagementCore/     ← DTOs, Interfaces
│   └── ESC.CONCOST.ModuleManagementBlazor/
│       └── Pages/
│           ├── PageAccounts.razor            ← UI quản lý tài khoản
│           ├── PagePermissions.razor         ← UI phân quyền
│           └── PageAuditLog.razor            ← UI xem Audit Log
│
├── [MODULE CÀI ĐẶT — Hiện có, mapping sang Index/Email/Blind Data]
│   ├── ESC.CONCOST.ModuleSetting/
│   │   ├── Controllers/, Queries/
│   │   └── Services/                         ← (chưa có service, cần thêm)
│   ├── ESC.CONCOST.ModuleSettingCore/        ← DTOs
│   └── ESC.CONCOST.ModuleSettingBlazor/
│       └── Pages/                            ← (hiện trống, cần xây dựng)
│
├── [MODULE NHÂN SỰ — Hiện có, không liên quan ESC]
│   ├── ESC.CONCOST.ModuleEmployee/
│   │   └── Services/EmployeeService.cs
│   ├── ESC.CONCOST.ModuleEmployeeCore/
│   └── ESC.CONCOST.ModuleEmployeeBlazor/
│       └── Pages/
│           ├── PageEmployee.razor
│           └── PageListEmployee.razor
│
├── [MODULE ESC — CẦN TẠO MỚI ⭐]
│   ├── ESC.CONCOST.ModuleESC/             ← Business Logic
│   │   ├── Classes/
│   │   │   └── ModuleAspNetRegister.cs    ← IModuleAspNet → tự đăng ký DI
│   │   ├── Controllers/
│   │   │   └── EscCustomerController.cs   ← [/api/esc-customer/*]
│   │   └── Services/
│   │       ├── EscCustomerService.cs      ← Đăng ký & duyệt hội viên
│   │       ├── ContractService.cs         ← CRUD Contracts, Items
│   │       ├── CalculationService.cs      ← Kd = Σ(ai × Ki)
│   │       ├── IndexService.cs            ← PPI/CPI time series CRUD
│   │       ├── ReportService.cs           ← Tạo PDF 종합의견서
│   │       └── ServiceRequestService.cs   ← Gửi email Request
│   │
│   ├── ESC.CONCOST.ModuleESCCore/         ← DTOs, Interfaces
│   │   ├── Interfaces/IEscCustomerService.cs
│   │   └── Models/CustomerDtos.cs
│   │
│   └── ESC.CONCOST.ModuleESCBlazor/       ← Blazor UI
│       └── Pages/
│           ├── [KHÁCH HÀNG — cần đăng nhập]
│           │   ├── PageWizardStep1.razor  [/esc/wizard/step1]        ← Bước 1: Thông tin HĐ
│           │   ├── PageWizardStep2.razor  [/esc/wizard/step2]        ← Bước 2: Biểu giá & Tỷ trọng
│           │   ├── PageWizardStep3.razor  [/esc/wizard/step3]        ← Bước 3: Kiểm tra điều kiện
│           │   ├── PageCalculation.razor  [/esc/calculation/{id}]    ← Bảng tính Kd
│           │   └── PageReport.razor       [/esc/report/{id}]         ← Báo cáo PDF
│           └── [ADMIN — Role=Admin]
│               ├── PageAdminCustomers.razor[/esc/admin/customers]    ← Duyệt hội viên
│               └── PageAdminIndex.razor    [/esc/admin/index]        ← Cập nhật chỉ số giá
│
└── [HOST]
    ├── ESC.CONCOST.WebHost/               ← ASP.NET Core API Host + Auth Pages
    │   ├── Program.cs                     ← DI tự động qua IModuleAspNet
    │   ├── Controllers/AuthController.cs  ← JWT Auth endpoint
    │   └── Pages/                         ← Razor Pages (cshtml) — Auth flow
    │       ├── Login.cshtml               [/login]           ✅ Hiện có
    │       ├── ForgotPassword.cshtml      [/forgot-password] ✅ Hiện có
    │       ├── ResetPassword.cshtml       [/reset-password]  ✅ Hiện có
    │       ├── _Host.cshtml               ← Blazor host entry ✅ Hiện có
    │       └── EscRegister.cshtml         [/esc-register]    ⭐ Cần tạo
    └── ESC.CONCOST.WebApp/                ← Blazor Host (Server + WASM)
        ├── Pages/Home.razor               [/]  ← ⭐ DASHBOARD: Danh sách hồ sơ + nút "Tạo hồ sơ"
        └── Shared/                        ← Layouts, Header, Notification
```

---

#### Luồng người dùng (User Flow)

```
[PUBLIC]
  │
  ├── /esc-register  (EscRegister.cshtml)      ← Điền form đăng ký
  │       │
  │       └── POST /api/esc-customer/register   ← Tạo CUSTOMER + SA_USER (Active=false)
  │               │
  │               └── Chờ Admin duyệt (Email notification)
  │
  └── /login  (Login.cshtml)                   ← Đăng nhập
          │
          └── JWT Cookie → Redirect về /        ← Home.razor

[MEMBER — Sau đăng nhập]
  /  (WebApp/Pages/Home.razor)
  ├── Hiển thị: Danh sách hồ sơ (CONTRACTS của user)
  │   ├── Card mỗi hồ sơ: Tên dự án, Trạng thái, Kd mới nhất
  │   └── Nút "Xem chi tiết" → /esc/report/{recordId}
  │
  └── Nút "+ Tạo hồ sơ mới" → /esc/wizard/step1
          │
          ├── Step 1 /esc/wizard/step1          ← Thông tin hợp đồng cơ bản
          │   (project_name, contractor, client, contract_method, bid_rate,
          │    contract_date, start_date, completion_date)
          │
          ├── Step 2 /esc/wizard/step2          ← Cấu hình biểu giá & tỷ trọng
          │   (Bảng CONTRACT_ITEMS: group_name, item_name, index_key, amount)
          │   (Kiểm tra Σ ai = 100%)
          │
          ├── Step 3 /esc/wizard/step3          ← Kiểm tra điều kiện tự động
          │   (elapsed_days ≥ 90 ngày?)
          │   (Δindex ≥ 3% so với thời điểm ký HĐ?)
          │   └── Nút "Tính toán" (chỉ active khi đủ điều kiện)
          │
          ├── /esc/calculation/{contractId}     ← Bảng tính Kd chi tiết
          │   (A0, A1, Ki từng hạng mục → Kd tổng, advance_deduct, net_adjust)
          │   └── Nút "Lưu kết quả" → tạo ADJUST_RECORDS snapshot
          │
          └── /esc/report/{recordId}            ← Báo cáo 종합의견서
              ├── Xem báo cáo (ẩn field theo Blind Data config)
              ├── Nút "Tải PDF"
              └── Nút "Request" (gửi email Claims Manager)
                  (chỉ active: threshold_met=true AND days_met=true)

[ADMIN — Role=Admin]
  /esc/admin/customers   ← Danh sách CUSTOMER, nút Approve/Reject
  /esc/admin/index       ← CRUD INDEX_TIMESERIES tháng/năm
  /management/account    ← SA_USER management ✅
  /management/permissions← Phân quyền Role ✅
  /setting/blind-data    ← Cấu hình Blind Data fields
  /setting/email-template← Cấu hình nội dung email
```

---

#### Màn hình chi tiết — File path, Route & Dữ liệu

| # | Màn hình | File | Route | Người dùng | Dữ liệu chính |
|---|----------|------|-------|------------|----------------|
| 1 | **Đăng ký hội viên** | `WebHost/Pages/EscRegister.cshtml` | `/esc-register` | Public | CompanyName, CeoName, Email, Password → POST API → CUSTOMER |
| 2 | **Đăng nhập** | `WebHost/Pages/Login.cshtml` | `/login` | Public | Email + Password → JWT Cookie ✅ Hiện có |
| 3 | **Quên mật khẩu** | `WebHost/Pages/ForgotPassword.cshtml` | `/forgot-password` | Public | ✅ Hiện có |
| 4 | **Dashboard — Danh sách hồ sơ** | `WebApp/Pages/Home.razor` | `/` | Member | Danh sách CONTRACTS của user, card trạng thái, nút **"+ Tạo hồ sơ mới"** ✅ Tích hợp vào Home |
| 5 | **Wizard Step 1 — Thông tin HĐ** | `ModuleESCBlazor/Pages/PageWizardStep1.razor` | `/esc/wizard/step1` | Member | project_name, contractor, client, contract_method, bid_rate, dates → CONTRACTS |
| 6 | **Wizard Step 2 — Biểu giá** | `ModuleESCBlazor/Pages/PageWizardStep2.razor` | `/esc/wizard/step2` | Member | Bảng items: group_name, item_name, index_key, amount (Σai=100%) → CONTRACT_ITEMS |
| 7 | **Wizard Step 3 — Điều kiện** | `ModuleESCBlazor/Pages/PageWizardStep3.razor` | `/esc/wizard/step3` | Member | elapsed_days ≥ 90? AND Δindex ≥ 3%? → nút Tính toán |
| 8 | **Bảng tính Kd** | `ModuleESCBlazor/Pages/PageCalculation.razor` | `/esc/calculation/{id}` | Member | A0, A1, Ki từng hạng mục → Kd, advance_deduct → ADJUST_RECORDS |
| 9 | **Báo cáo 종합의견서** | `ModuleESCBlazor/Pages/PageReport.razor` | `/esc/report/{id}` | Member | Report PDF, nút Tải PDF, nút Request (active khi threshold_met=true) |
| 10 | **Admin: Duyệt hội viên** | `ModuleESCBlazor/Pages/PageAdminCustomers.razor` | `/esc/admin/customers` | Admin | Bảng CUSTOMER pending, Approve/Reject + SettingNotification |
| 11 | **Admin: Chỉ số giá** | `ModuleESCBlazor/Pages/PageAdminIndex.razor` | `/esc/admin/index` | Admin | Grid INDEX_TIMESERIES: period_key, index_key, index_value, data_verified |
| 12 | **Blind Data Config** | `ModuleSettingBlazor/Pages/PageBlindData.razor` | `/setting/blind-data` | Admin | Toggle ẩn/hiện từng field → ADMIN_SETTINGS |
| 13 | **Email Template** | `ModuleSettingBlazor/Pages/PageEmailTemplate.razor` | `/setting/email-template` | Admin | HTML editor, placeholder → SETTING_EMAIL_TEMPLATE |
| 14 | **Quản lý tài khoản** | `ModuleManagementBlazor/Pages/PageAccounts.razor` | `/management/account` | Admin | SA_USER list ✅ Hiện có |
| 15 | **Phân quyền** | `ModuleManagementBlazor/Pages/PagePermissions.razor` | `/management/permissions` | Admin | Role-based permissions ✅ Hiện có |
| 16 | **Audit Log** | `ModuleManagementBlazor/Pages/PageAuditLog.razor` | `/management/audit-log` | Admin | Log hành động ✅ Hiện có |

---

#### DI Registration — Pattern `IModuleAspNet` (tự động, không cần sửa Program.cs)

```csharp
// Program.cs (WebHost) — ĐÃ CÓ SẴN, không cần chỉnh:
var aspnetModules = AssembliesUtil.GetAspNetAssemblies()
                      .GetInstances<IModuleAspNet>();
foreach (var module in aspnetModules)
    module.ConfigureServices(services, configs);  // scan toàn bộ DLL

// ESC cần tạo: ModuleESC/Classes/ModuleAspNetRegister.cs
public class ModuleAspNetRegister : IModuleAspNet
{
    public void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IEscCustomerService, EscCustomerService>();
        services.AddScoped<IContractService,    ContractService>();
        services.AddScoped<ICalculationService, CalculationService>();
        services.AddScoped<IIndexService,       IndexService>();
        services.AddScoped<IReportService,      ReportService>();
    }
    public void BuildModule(WebApplication app) { }
}

### 3.2. ERD cốt lõi (mức quan hệ)

```
CUSTOMER (1) ──── (N) SA_USER
CUSTOMER (1) ──── (N) CONTRACTS
CUSTOMER (1) ──── (N) ESC_SERVICE_REQUEST
CONTRACTS (1) ──── (N) CONTRACT_ITEMS
CONTRACTS (1) ──── (N) ADJUST_RECORDS
CONTRACT_ITEMS (N) ──── (1) INDEX_TYPES
INDEX_TYPES (1) ──── (N) INDEX_TIMESERIES
ADJUST_RECORDS (1) ──── (N) ADJUST_ITEM_DETAILS
ADJUST_ITEM_DETAILS (N) ──── (1) CONTRACT_ITEMS
SA_USER (1) ──── (N) SETTING_EMAIL_TEMPLATE
```

### 3.3. API / Service chính

| Service | Endpoint / Method | Chức năng |
|---------|-------------------|-----------|
| `AuthService` | POST /register, POST /approve | Đăng ký, duyệt hội viên |
| `ContractService` | POST /contracts, GET /contracts/{id} | CRUD hợp đồng |
| `CalculationService` | POST /calculate | Tính Kd, lưu snapshot |
| `IndexService` | GET/POST /index-timeseries | Quản lý chuỗi chỉ số |
| `ReportService` | GET /report/pdf/{recordId} | Xuất PDF |
| `RequestService` | POST /service-request | Gửi yêu cầu + email |
| `AdminService` | PUT /settings/blind-data, /email-templates | Cấu hình hệ thống |

---

## 4. ĐỀ XUẤT KỸ THUẬT

### 4.1. Kiến trúc đề xuất
**Modular Monolith** (kiến trúc hiện tại của codebase) — giữ nguyên, không refactor sang Microservices trong giai đoạn này.

```
ESC.CONCOST.Abstract        ← Entities, Interfaces
ESC.CONCOST.Base            ← SA_USER, AuditLog (shared)
ESC.CONCOST.ModuleESC       ← Business Logic (Services)
ESC.CONCOST.ModuleESCCore   ← DTOs, Shared Interfaces
ESC.CONCOST.ModuleESCBlazor ← UI Components (Blazor WASM)
ESC.CONCOST.WebHost         ← API Host
ESC.CONCOST.WebApp          ← Blazor Host
```

### 4.2. Tech Stack
| Layer | Technology | Ghi chú |
|-------|------------|---------|
| Frontend | Blazor WASM (.NET 8) | Đang dùng |
| Backend | ASP.NET Core Web API (.NET 8) | Đang dùng |
| Database | SQL Server | Đang dùng |
| ORM | Entity Framework Core 8 | Fluent API |
| PDF | iTextSharp / QuestPDF | Cần quyết định |
| Email | SMTP / SendGrid | Admin config |
| Auth | ASP.NET Identity + JWT | Tích hợp SA_USER |

### 4.3. Rủi ro tiềm ẩn

| Rủi ro | Mức độ | Giải pháp |
|--------|--------|-----------|
| **Công thức tính không khớp HTML gốc** | 🔴 Critical | Unit test so sánh kết quả với file HTML mẫu |
| **Revised Data recalculate cũ** | 🟠 High | Quyết định: lock snapshot hay recalculate khi index thay đổi |
| **Bảo mật công thức (Ctrl+U)** | 🟠 High | Minify + Obfuscate JS, server-side calculation |
| **Performance chuỗi thời gian** | 🟡 Medium | Index DB theo `(index_key, period_key)` |
| **OCR PDF auto-fill** | 🟡 Medium | Chỉ làm proof-of-concept, không block release |
| **1 project/customer** | 🟡 Medium | Logic business cần làm rõ trước khi code |

---

## 5. LỘ TRÌNH PHÁT TRIỂN (ROADMAP)

### Phase 0 — Phân tích & Chuẩn bị (7/5 – 9/5) ✅ Done
- Phân tích nghiệp vụ, xác nhận ERD, kiến trúc hệ thống
- Kết quả: `ESC_ERD_Schema.md`, `ESC_Development_Plan.md`, Tài liệu Q&A

---

### Phase 1 — Nền tảng dữ liệu (11/5 – 12/5)
**Mục tiêu:** 14 Entity + Migration + DB khởi tạo

- Tạo 13 Entity mới trong `ESC.CONCOST.Abstract/Entities/`
- Cập nhật `SA_USER`, `AuditLog`
- Cấu hình Fluent API: FK, kiểu dữ liệu, constraints
- Chạy Migration: `AddESCModuleTables`
- **Kết quả:** Database schema hoàn chỉnh, chạy được trên SQL Server

---

### Phase 2 — Khung Module & Authentication (13/5 – 15/5)
**Mục tiêu:** Cấu trúc Project + Luồng đăng ký/duyệt hội viên

- Tạo 3 project mới: `ModuleESC`, `ModuleESCCore`, `ModuleESCBlazor`
- Đăng ký DI vào `Program.cs`
- Trang đăng ký khách hàng → lưu `CUSTOMER`
- Admin Dashboard: danh sách chờ duyệt, nút Approve/Reject
- Gửi `SETTING_NOTIFICATION` khi duyệt
- **Kết quả:** Hội viên đăng ký được, Admin duyệt được

---

### Phase 3 — Wizard Nhập liệu (18/5 – 22/5)
**Mục tiêu:** Giao diện nhập hợp đồng hoàn chỉnh

- Wizard 3 bước: Thông tin chung → Biểu giá & Tỷ trọng → Điều kiện
- Lưu vào `CONTRACTS`, `CONTRACT_ITEMS`
- Import/Export dữ liệu đã nhập
- Admin: cấu hình ẩn/hiện vùng màu vàng qua `ADMIN_SETTINGS`
- **Kết quả:** Khách hàng nhập & lưu được hợp đồng

---

### Phase 4 — Bộ máy tính toán (28/5 – 5/6)
**Mục tiêu:** Tính Kd chính xác, lưu Snapshot

- `IndexService`: CRUD `INDEX_TIMESERIES`, `INSURANCE_RATES`
- `CalculationService`: tính `Ki`, `Kd = Σ(ai × Ki)`
- Xử lý tạm ứng, khấu trừ, ngưỡng 3% và 90 ngày
- Lưu `ADJUST_RECORDS` + `ADJUST_ITEM_DETAILS` (snapshot)
- Unit Test toàn bộ công thức
- **Kết quả:** Kết quả tính trùng khớp file HTML gốc

---

### Phase 5 — Báo cáo & Service Request (10/6 – 16/6)
**Mục tiêu:** Output sản phẩm cuối

- Xuất PDF báo cáo "종합의견서" từ snapshot
- Nút "Request": chỉ active khi `threshold_met = true && days_met = true`
- Email tự động đến Claims Manager (file PDF đính kèm)
- Cấu hình `SETTING_EMAIL_TEMPLATE`, địa chỉ email Admin
- **Kết quả:** Khách hàng yêu cầu tư vấn được qua 1 click

---

### Phase 6 — Kiểm thử & Bàn giao (18/6 – 22/6)
**Mục tiêu:** Sản phẩm sẵn sàng production

- Integration Test toàn luồng: Đăng ký → Nhập → Tính → PDF → Request
- Tối ưu DB Indexing cho chuỗi thời gian
- Gắn link vào Website `con-cost.com`
- Bàn giao tài liệu vận hành
- **Kết quả:** Go-live

---

## 6. PHÂN RÃ TASK (MỨC DEVELOPER)

### Phase 1 — Database Foundation

| Task | Mô tả | Ưu tiên | Dependency |
|------|--------|---------|-----------|
| T1.1 | Tạo 13 Entity class | 🔴 High | — |
| T1.2 | Fluent API config (FK, Constraints) | 🔴 High | T1.1 |
| T1.3 | Update `SA_USER` → add `customer_id` FK | 🔴 High | T1.1 |
| T1.4 | EF Migration + DB Update | 🔴 High | T1.2, T1.3 |

### Phase 2 — Auth & Module Setup

| Task | Mô tả | Ưu tiên | Dependency |
|------|--------|---------|-----------|
| T2.1 | Tạo 3 project Module | 🔴 High | T1.4 |
| T2.2 | DI Registration trong Program.cs | 🔴 High | T2.1 |
| T2.3 | Trang Register khách hàng | 🔴 High | T2.2 |
| T2.4 | Admin Dashboard duyệt hội viên | 🔴 High | T2.3 |
| T2.5 | Notification khi approve/reject | 🟡 Medium | T2.4 |

### Phase 3 — Contract Wizard

| Task | Mô tả | Ưu tiên | Dependency |
|------|--------|---------|-----------|
| T3.1 | Wizard Step 1: Thông tin hợp đồng | 🔴 High | T2.4 |
| T3.2 | Wizard Step 2: Biểu giá & Tỷ trọng | 🔴 High | T3.1 |
| T3.3 | Wizard Step 3: Kiểm tra điều kiện | 🔴 High | T3.2 |
| T3.4 | Import/Export CSV/Excel | 🟡 Medium | T3.3 |
| T3.5 | Admin: cấu hình Blind Data | 🟡 Medium | T2.4 |

### Phase 4 — Calculation Engine

| Task | Mô tả | Ưu tiên | Dependency |
|------|--------|---------|-----------|
| T4.1 | IndexService: CRUD INDEX_TIMESERIES | 🔴 High | T1.4 |
| T4.2 | CalculationService: Ki, Kd logic | 🔴 High | T4.1 |
| T4.3 | Xử lý advance_deduct, net_adjust | 🔴 High | T4.2 |
| T4.4 | Lưu snapshot vào ADJUST_RECORDS | 🔴 High | T4.3 |
| T4.5 | Unit Test công thức | 🔴 High | T4.4 |
| T4.6 | InsuranceRatesService | 🟡 Medium | T1.4 |

### Phase 5 — Report & Request

| Task | Mô tả | Ưu tiên | Dependency |
|------|--------|---------|-----------|
| T5.1 | PDF Export (종합의견서) | 🔴 High | T4.5 |
| T5.2 | Nút "Request" + logic threshold check | 🔴 High | T4.5 |
| T5.3 | Email Service gửi PDF tới Claims Manager | 🔴 High | T5.1, T5.2 |
| T5.4 | Admin: quản lý Email Template | 🟡 Medium | T5.3 |
| T5.5 | Audit Log cho các hành động quan trọng | 🟡 Medium | T5.3 |

---

## 7. KẾ HOẠCH REVIEW GIỮA KỲ

### Mốc 1 — Demo nội bộ (27/5/2026)
**Sau Phase 1, 2, 3**

- **Demo gì:** Luồng đăng ký → Admin duyệt → Nhập hợp đồng (Wizard 3 bước) → Xem màn hình sau khi lưu
- **Gửi ai:** Dev Cường + Phương + Park Yong-jin
- **Feedback cần thu thập:**
  - Wizard UI có đúng yêu cầu business không?
  - Vùng dữ liệu màu vàng (Blind Data) hiển thị/ẩn đúng chưa?
  - Luồng duyệt hội viên có đủ thông tin không?
  - Xác nhận lại: 1 customer = 1 project?

---

### Mốc 2 — Demo kỹ thuật tính toán (9/6/2026)
**Sau Phase 4**

- **Demo gì:** Nhập dữ liệu thực → Chạy calculation → So sánh kết quả với file HTML gốc
- **Gửi ai:** Dev Cường + Phương + Park Yong-jin
- **Feedback cần thu thập:**
  - Kết quả Kd có đúng với mong đợi không? (Cần test case thực)
  - Độ chính xác thập phân có đúng không?
  - Xác nhận: Revised Data có update retrospective không?

---

### Mốc 3 — Demo sản phẩm (17/6/2026)
**Sau Phase 5**

- **Demo gì:** Toàn bộ luồng end-to-end + xuất PDF + gửi email
- **Link:** Open link staging (cần URL cụ thể)
- **Gửi ai:** Full team + Stakeholder
- **Feedback cần thu thập:**
  - PDF đúng format "종합의견서" không?
  - Email nhận đúng địa chỉ Claims Manager không?
  - Cần thêm/bớt thông tin gì trong báo cáo?
  - Trải nghiệm tổng thể?

---

### Mốc 4 — UAT & Go-live (22/6/2026)
**Sau Phase 6**

- **Demo gì:** Stakeholder tự thao tác toàn luồng (user acceptance)
- **Feedback cần thu thập:**
  - Bug list cuối cùng
  - Xác nhận sign-off chính thức

---

## 8. GHI CHÚ QUAN TRỌNG

### Hành động cần làm ngay (Before Code)
1. ✅ **Xác nhận quy tắc làm tròn số thập phân** với Park Yong-jin
2. ✅ **Xác nhận hình thức thanh toán** (1 lần / tháng / dự án)
3. ✅ **Cung cấp email công ty** để cấu hình SMTP
4. ✅ **Cung cấp source code** tự động liên kết chỉ số
5. ✅ **Xác nhận URL website** `con-cost.com` để chuẩn bị deployment

### Phân công
| Người | Phụ trách |
|-------|-----------|
| **Dev Cường** | Backend (Services, API, EF Migration), Blazor UI |
| **Phương** | Review nghiệp vụ, Kiểm thử, Tài liệu |
| **Park Yong-jin** | Xác nhận nghiệp vụ Hàn Quốc, UAT |

---
**Tài liệu tham chiếu:**
- [ESC_ERD_Schema.md](file:///d:/DEV_CUONG/PM/esc_concost/BA/ESC_ERD_Schema.md)
- [ESC_Development_Plan.md](file:///d:/DEV_CUONG/PM/esc_concost/BA/ESC_Development_Plan.md)
- [Lo_trinh_ESC.md](file:///d:/DEV_CUONG/PM/esc_concost/BA/Lo_trinh_ESC.md)
- [SYSTEM PROFESSIONAL VALIDATION QUESTION 1](file:///d:/DEV_CUONG/PM/esc_concost/BA/SYSTEM%20PROFESSIONAL%20VALIDATION%20QUESTION%201%20%28Translated%29.md)
- [ESC_Database_Architecture_Report.md](file:///d:/DEV_CUONG/PM/esc_concost/BA/ESC_Database_Architecture_Report.md)
