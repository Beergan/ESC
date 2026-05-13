# Đánh giá source `src` - ESC.CONCOST

Ngày đánh giá: 2026-05-12  
Phạm vi: thư mục `src`, tập trung vào mã nguồn C#/.NET/Blazor; bỏ qua phần lớn file build `bin/obj` và asset/vendor trong `wwwroot/assets`.

## 1. Tóm tắt nhanh

Source là một solution .NET 8 khá đầy đủ, có kiến trúc module rõ ràng:

- `ESC.CONCOST.WebHost`: host ASP.NET Core, API, Razor Pages, Blazor Server/WebAssembly hybrid.
- `ESC.CONCOST.WebApp`: Blazor WebAssembly/client UI.
- `ESC.CONCOST.Abstract`, `Base`, `Db`: tầng model, interface, repository, database provider.
- Các module nghiệp vụ tách theo cụm `Core / Service / Blazor`: `Management`, `Employee`, `Setting`, `ESC`.
- Có hỗ trợ nhiều database: MSSQL, MySQL, PostgreSQL, InMemory.
- Có Identity, JWT, SignalR, Hangfire, MediatR, MudBlazor, RestEase.

Đánh giá tổng quan: **source có nền tảng kiến trúc tốt và build được, nhưng chưa sẵn sàng production vì có rủi ro bảo mật cấu hình, CORS/upload quá rộng, secret nằm trong repo, thiếu test tự động và còn lẫn build artifact trong `src`.**

Điểm ước lượng theo nhóm:

| Nhóm | Điểm | Nhận xét |
|---|---:|---|
| Kiến trúc | 7/10 | Module hóa tốt, tách Core/Service/UI rõ, nhưng `Program.cs` đang ôm quá nhiều cấu hình. |
| Build/khả năng chạy | 7/10 | `dotnet build --no-restore` pass, còn 7 warning. |
| Bảo mật | 4/10 | Secret hard-code, CORS mở, HTTPS metadata tắt, upload anonymous. |
| Maintainability | 6/10 | Có nhiều abstraction dùng lại, nhưng một số file lớn, static service locator, catch rỗng. |
| Test/QA | 3/10 | Chưa thấy project test, chưa có CI rõ ràng, README còn template mặc định. |
| Production readiness | 4/10 | Cần xử lý secret, config, upload, logging, deploy docs trước khi vận hành thật. |

## 2. Thống kê source

Các file mã nguồn chính sau khi loại trừ `bin/obj/wwwroot/assets/wwwroot/upload`:

| Loại file | Số lượng |
|---|---:|
| `.cs` | 249 |
| `.razor` | 41 |
| `.csproj` | 18 |
| `.cshtml` | 6 |
| `.json` | 5 |
| `.sln` | 1 |

Các file lớn đáng chú ý:

- `ESC.CONCOST.WebHost/Program.cs`: 406 dòng, chứa hầu hết cấu hình DI, auth, Swagger, CORS, upload, route.
- `ESC.CONCOST.WebApp/Components/Layout/AppShellLayout.razor`: 612 dòng.
- `ESC.CONCOST.ModuleESCBlazor/Pages/PageContractWizard.razor`: 492 dòng.
- `ESC.CONCOST.SharedComponents/Components/ModalComponent.razor`: 502 dòng.
- Nhiều migration EF trên 1.000 dòng là bình thường, không xem là vấn đề maintainability chính.

## 3. Điểm mạnh

1. **Kiến trúc module tốt**  
   Cách tách `ModuleXCore`, `ModuleX`, `ModuleXBlazor` giúp phân ranh interface/model, backend service/controller và UI. Đây là nền khá ổn để mở rộng module mới.

2. **Có lớp abstract/base dùng chung**  
   Các interface như `IRepository`, `IMyContext`, `IAuthService`, `INotifyService`, `IBlazorContext` giúp giảm phụ thuộc trực tiếp giữa module.

3. **Hỗ trợ nhiều database provider**  
   `DbMssqlRegister`, `DbMysqlRegister`, `DbPostgresRegister`, `DbMemoryRegister` cho thấy thiết kế có ý thức về tính linh hoạt hạ tầng.

4. **Có cơ chế permission version trong JWT**  
   `ServerAuthService` và JWT validation có kiểm tra `UserVersion`/`RoleVersion_*`, giúp thu hồi quyền khi role/user permission thay đổi.

5. **Build hiện tại thành công**  
   Lệnh kiểm tra:

   ```powershell
   dotnet build src\ESC.CONCOST.sln --no-restore
   ```

   Kết quả: **Build succeeded**, 7 warning, 0 error.

## 4. Vấn đề nghiêm trọng cần xử lý

### 4.1. Secret đang nằm trực tiếp trong source

File `src/ESC.CONCOST.WebHost/appsettings.json` đang chứa:

- SQL Server connection string có user/password.
- JWT signing key.
- SMTP Gmail và app password.
- Sentry DSN.

Rủi ro:

- Nếu repo bị chia sẻ, toàn bộ DB/mail/token signing key bị lộ.
- JWT signing key lộ nghĩa là token có thể bị giả mạo.
- SMTP password lộ có thể bị dùng gửi mail trái phép.

Khuyến nghị:

- Xoay toàn bộ secret đã lộ: DB password, Gmail app password, JWT signing key, Sentry DSN nếu cần.
- Chuyển secret sang environment variables, user-secrets cho dev, hoặc vault/CI secret cho production.
- Commit `appsettings.json` chỉ giữ key mẫu, không giữ giá trị thật.
- Thêm `appsettings.Development.json`, `appsettings.Production.json` theo mẫu và ignore file local chứa secret.

### 4.2. CORS đang mở toàn bộ origin

Trong `Program.cs`, CORS đang cho:

- `AllowAnyOrigin`
- `AllowAnyHeader`
- `AllowAnyMethod`

Rủi ro:

- API có thể bị gọi từ bất kỳ domain nào.
- Khi kết hợp cookie/token và endpoint upload/API, bề mặt tấn công tăng lên.

Khuyến nghị:

- Production chỉ allow danh sách domain thật.
- Tách policy cho dev và prod.
- Nếu dùng cookie auth qua browser, cần xem lại SameSite/Secure/HttpOnly.

### 4.3. Upload file quá rộng và một số endpoint anonymous

`UploadController` có `[Authorize]` ở class, nhưng `SaveLogo` và `SaveAvatar` đang `[AllowAnonymous]`. `Program.cs` cũng cấu hình:

- `MaxRequestBodySize = null`
- `MultipartBodyLengthLimit = 10GB`
- SignalR maximum message size 100MB

Rủi ro:

- Anonymous user có thể upload file.
- Chưa thấy kiểm tra MIME type, extension whitelist, antivirus scan, dung lượng theo role/user.
- Upload quá lớn có thể gây đầy disk hoặc DoS.

Khuyến nghị:

- Chỉ anonymous nếu có business case thật; nếu có, thêm token tạm thời hoặc captcha/rate limit.
- Whitelist extension: ảnh chỉ `.jpg/.jpeg/.png/.webp`; document chỉ các định dạng cần thiết.
- Kiểm tra content type và magic bytes.
- Giới hạn dung lượng theo endpoint, không để global 10GB nếu không thật sự cần.
- Lưu file ngoài webroot hoặc random hóa đường dẫn, không cho thực thi nội dung upload.

### 4.4. HTTPS metadata bị tắt cho JWT bearer

Trong `Program.cs`, `RequireHttpsMetadata = false`.

Rủi ro:

- Chấp nhận cấu hình không HTTPS trong môi trường ngoài dev.

Khuyến nghị:

- Chỉ tắt trong Development.
- Production bắt buộc HTTPS, `Secure` cookie, HSTS đã bật trong non-dev là điểm tốt nhưng chưa đủ nếu token/cookie chưa cấu hình chặt.

## 5. Vấn đề chất lượng code/maintainability

### 5.1. `Program.cs` quá nhiều trách nhiệm

`Program.cs` đang xử lý nhiều nhóm việc: database selection, auth, Swagger, CORS, upload limit, Hangfire, SignalR, module loading, route, seeding.

Khuyến nghị:

- Tách extension methods:
  - `AddDatabaseProvider`
  - `AddJwtAuthentication`
  - `AddCorsPolicies`
  - `AddUploadLimits`
  - `AddAppModules`
  - `UseAppEndpoints`

### 5.2. Có `catch { }` rỗng

Tìm thấy một số catch rỗng ở:

- `Program.cs`
- `NotificationHeader.razor`
- `MyCookieServer.cs`
- `RedirectToLogin.razor`

Rủi ro:

- Che lỗi thật, khó debug production.
- Dễ tạo bug im lặng.

Khuyến nghị:

- Ít nhất log warning/debug.
- Chỉ swallow exception khi có lý do rõ và có comment.

### 5.3. Static service locator

`AppStatic` giữ `IServiceProvider` static và có hàm build service provider thủ công.

Rủi ro:

- Dễ làm lifetime DI sai.
- Khó test.
- Dễ tạo memory leak hoặc resolve service ngoài scope.

Khuyến nghị:

- Hạn chế dùng static service locator.
- Ưu tiên inject service qua constructor/component.
- Nếu bắt buộc dùng static helper, chỉ để constant/config đơn giản, không giữ service provider.

### 5.4. Một số controller/service chưa đồng nhất authorization

Ví dụ:

- `ESCController`, `ConstructionCategoryController`, `UploadController` có `[Authorize]`.
- `ContractCategoryController` chưa thấy `[Authorize]` ở class/action, nhưng có endpoint `POST`/`DELETE`.

Khuyến nghị:

- Rà lại toàn bộ API mutation (`POST`, `PUT`, `DELETE`) và bắt buộc auth/permission.
- Dùng policy hoặc permission attribute thống nhất.

### 5.5. Encoding/comment tiếng Việt bị lỗi

Một số chuỗi/comment hiển thị bị mojibake như `XÃ¡c thá»±c`, `Máº¥t káº¿t ná»‘i`. Điều này làm tài liệu Swagger/UI/log khó đọc.

Khuyến nghị:

- Chuẩn hóa file encoding UTF-8.
- Rà các resource/message tiếng Việt/Hàn đang bị lỗi.

## 6. Build warning hiện tại

Build pass nhưng còn 7 warning:

1. `EmployeeService.cs(53,1)`: XML comment sai cấu trúc, thiếu end tag `summary`.
2. `ConstructionCategoryController.cs(28,43)`: dùng nullable annotation khi nullable context chưa bật.
3. `ConstructionCategoryService.cs(35,43)`: tương tự nullable annotation.
4. `AppShellLayout.razor(467,21)`: field `_position` assigned but never used.
5. `AppShellLayout.razor(468,21)`: field `_department` assigned but never used.
6. `AppShellLayout.razor(648,21)`: field `_mobSection` assigned but never used.
7. `MyContext.cs(169,43)`: event `OnNotify` never used.

Khuyến nghị:

- Xử lý warning nhỏ trước để build sạch.
- Bật `TreatWarningsAsErrors` theo từng project sau khi warning về 0.
- Cân nhắc bật nullable dần cho project mới/core trước.

## 7. Repository hygiene

Trong workspace hiện có nhiều thư mục `bin` và `obj` dưới từng project trong `src`. `.gitignore` có rule ignore `bin/obj`, nhưng không kiểm tra được trạng thái tracked vì máy hiện tại không nhận lệnh `git`.

Khuyến nghị:

- Đảm bảo `bin/obj` không bị commit.
- Không commit file upload mẫu trong `wwwroot/upload` nếu đó là dữ liệu runtime.
- Thêm tài liệu setup DB, migration, chạy dev server.
- README hiện vẫn là template GitLab mặc định, cần viết lại theo dự án.

## 8. Test và QA

Chưa thấy project test trong solution.

Khuyến nghị tối thiểu:

- Thêm test project cho service quan trọng: auth, permission, contract/category, employee.
- Test repository/service với InMemory DB hoặc test container DB.
- Test authorization cho endpoint mutation.
- Thêm smoke test build trong CI:

  ```powershell
  dotnet restore src\ESC.CONCOST.sln
  dotnet build src\ESC.CONCOST.sln --no-restore
  dotnet test src\ESC.CONCOST.sln --no-build
  ```

## 9. Ưu tiên xử lý đề xuất

### P0 - Làm ngay trước khi share/deploy

1. Xoay toàn bộ secret đã nằm trong `appsettings.json`.
2. Đưa secret ra environment variables/user-secrets/vault.
3. Khóa CORS theo domain thật.
4. Rà endpoint upload anonymous, thêm auth/rate limit/size limit/extension whitelist.
5. Đảm bảo JWT signing key production đủ mạnh và không commit.

### P1 - Ổn định source

1. Sửa 7 build warning.
2. Tách `Program.cs` thành các extension method.
3. Chuẩn hóa authorization cho tất cả controller.
4. Thêm logging thay cho catch rỗng.
5. Viết README thật: cách cài DB, chạy migration, chạy app, cấu hình secret.

### P2 - Nâng chất lượng dài hạn

1. Thêm test project và CI.
2. Rà lại static service locator.
3. Chuẩn hóa encoding UTF-8 và resource localization.
4. Tách các `.razor` quá lớn thành component con.
5. Rà dependency/package version định kỳ.

## 10. Kết luận

Source `ESC.CONCOST` có cấu trúc module khá tốt, có nền tảng để phát triển tiếp và hiện tại build thành công. Tuy nhiên, điểm nghẽn lớn nhất không phải là compile mà là **bảo mật cấu hình và production hardening**. Nếu xử lý được nhóm P0, dự án sẽ an toàn hơn nhiều để demo/deploy nội bộ. Sau đó nên dọn warning, thêm test và tài liệu setup để team mới vào có thể chạy dự án mà không phụ thuộc vào tri thức truyền miệng.

