using System.Linq;

namespace ESC.CONCOST.Abstract;

public static class BasicCodes
{
    public static OptionDuals<string> GenderOptions = new(
        new("M", "Male", "Nam"),
        new("F", "Female", "Nữ"),
        new("U", "Other", "Không xác định")
    );

    //-Dev-Bee-CN: Danh sách loại công việc dự án
    public static OptionTriples<string> WorkTypeOptions = new(
        new("Structure", "Structure", "Kết cấu", "구조"),
        new("Finishing", "Finishing", "Hoàn thiện", "마감"),
        new("StructureFinishing", "Structure + Finishing", "Kết cấu + Hoàn thiện", "구조 + 마감"),
        new("Infrastructure", "Infrastructure", "Hạ tầng kỹ thuật", "기반 시설"),
        new("SurveyDesign", "Survey & Design", "Khảo sát & Thiết kế", "조사 및 설계")
    );

    //-Dev-Cuong-ESC: Trạng thái duyệt khách hàng
    public static class CustomerApprovalStatus
    {
        public const int Pending = 0;
        public const int Approved = 1;
        public const int Rejected = 2;

        public static string GetText(int status)
        {
            return status switch
            {
                Pending => "Pending",
                Approved => "Approved",
                Rejected => "Rejected",
                _ => "Unknown"
            };
        }

        public static string GetTextVi(int status)
        {
            return status switch
            {
                Pending => "Chờ duyệt",
                Approved => "Đã duyệt",
                Rejected => "Từ chối",
                _ => "Không xác định"
            };
        }

        public static string GetTextKr(int status)
        {
            return status switch
            {
                Pending => "승인 대기",
                Approved => "승인됨",
                Rejected => "거절됨",
                _ => "알 수 없음"
            };
        }

        public static string GetBadgeClass(int status)
        {
            return status switch
            {
                Pending => "badge bg-warning text-dark",
                Approved => "badge bg-success",
                Rejected => "badge bg-danger",
                _ => "badge bg-secondary"
            };
        }
    }

    //-Dev-Cuong-ESC: Loại membership khách hàng
    public static class MembershipType
    {
        public const string Free = "Free";
        public const string Paid = "Paid";
        public const string Enterprise = "Enterprise";

        public static readonly string[] All =
        {
            Free,
            Paid,
            Enterprise
        };

        public static bool IsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                && All.Contains(value);
        }

        public static string GetText(string value)
        {
            return value switch
            {
                Free => "Free",
                Paid => "Paid",
                Enterprise => "Enterprise",
                _ => "Unknown"
            };
        }

        public static string GetTextVi(string value)
        {
            return value switch
            {
                Free => "Miễn phí",
                Paid => "Trả phí",
                Enterprise => "Doanh nghiệp",
                _ => "Không xác định"
            };
        }

        public static string GetTextKr(string value)
        {
            return value switch
            {
                Free => "무료",
                Paid => "유료",
                Enterprise => "기업",
                _ => "알 수 없음"
            };
        }

        public static string GetBadgeClass(string value)
        {
            return value switch
            {
                Free => "badge bg-secondary",
                Paid => "badge bg-primary",
                Enterprise => "badge bg-dark",
                _ => "badge bg-secondary"
            };
        }
    }

    //-Dev-Cuong-ESC: Trạng thái yêu cầu báo cáo chi tiết ESC
    public static class EscServiceRequestStatus
    {
        public const int New = 0;
        public const int InProgress = 1;
        public const int Completed = 2;
        public const int Rejected = 3;

        public static string GetText(int status)
        {
            return status switch
            {
                New => "New",
                InProgress => "In Progress",
                Completed => "Completed",
                Rejected => "Rejected",
                _ => "Unknown"
            };
        }

        public static string GetTextVi(int status)
        {
            return status switch
            {
                New => "Mới",
                InProgress => "Đang xử lý",
                Completed => "Hoàn thành",
                Rejected => "Từ chối",
                _ => "Không xác định"
            };
        }

        public static string GetTextKr(int status)
        {
            return status switch
            {
                New => "신규",
                InProgress => "진행중",
                Completed => "완료",
                Rejected => "거절됨",
                _ => "알 수 없음"
            };
        }

        public static string GetBadgeClass(int status)
        {
            return status switch
            {
                New => "badge bg-primary",
                InProgress => "badge bg-warning text-dark",
                Completed => "badge bg-success",
                Rejected => "badge bg-danger",
                _ => "badge bg-secondary"
            };
        }
    }

    //-Dev-Cuong-ESC: Trạng thái hồ sơ ESC
    public static class EscContractStatus
    {
        public const int Draft = 0;
        public const int Created = 1;
        public const int Calculated = 2;
        public const int Requested = 3;
        public const int Completed = 4;
        public const int Cancelled = 5;

        public static string GetText(int status)
        {
            return status switch
            {
                Draft => "Draft",
                Created => "Created",
                Calculated => "Calculated",
                Requested => "Requested",
                Completed => "Completed",
                Cancelled => "Cancelled",
                _ => "Unknown"
            };
        }

        public static string GetTextVi(int status)
        {
            return status switch
            {
                Draft => "Nháp",
                Created => "Đã tạo",
                Calculated => "Đã tính toán",
                Requested => "Đã yêu cầu báo cáo",
                Completed => "Hoàn thành",
                Cancelled => "Đã hủy",
                _ => "Không xác định"
            };
        }

        public static string GetTextKr(int status)
        {
            return status switch
            {
                Draft => "임시 저장",
                Created => "생성됨",
                Calculated => "계산 완료",
                Requested => "상세 보고서 요청됨",
                Completed => "완료",
                Cancelled => "취소됨",
                _ => "알 수 없음"
            };
        }

        public static string GetBadgeClass(int status)
        {
            return status switch
            {
                Draft => "badge bg-secondary",
                Created => "badge bg-info text-dark",
                Calculated => "badge bg-primary",
                Requested => "badge bg-warning text-dark",
                Completed => "badge bg-success",
                Cancelled => "badge bg-danger",
                _ => "badge bg-secondary"
            };
        }
    }

    //-Dev-Cuong-ESC: Trạng thái dữ liệu chỉ số theo tháng
    public static class IndexDataStatus
    {
        public const int Draft = 0;
        public const int Verified = 1;
        public const int Locked = 2;

        public static string GetText(int status)
        {
            return status switch
            {
                Draft => "Draft",
                Verified => "Verified",
                Locked => "Locked",
                _ => "Unknown"
            };
        }

        public static string GetTextVi(int status)
        {
            return status switch
            {
                Draft => "Nháp",
                Verified => "Đã xác nhận",
                Locked => "Đã khóa",
                _ => "Không xác định"
            };
        }

        public static string GetTextKr(int status)
        {
            return status switch
            {
                Draft => "임시",
                Verified => "검증됨",
                Locked => "잠김",
                _ => "알 수 없음"
            };
        }

        public static string GetBadgeClass(int status)
        {
            return status switch
            {
                Draft => "badge bg-secondary",
                Verified => "badge bg-success",
                Locked => "badge bg-dark",
                _ => "badge bg-secondary"
            };
        }
    }
    public static class AccountType
    {
        public const string Employee = "Employee";
        public const string Customer = "Customer";
        public const string Admin = "Admin";

        public static string GetTextKr(string type)
        {
            return type switch
            {
                Employee => "임직원",
                Customer => "고객",
                Admin => "관리자",
                _ => "기타"
            };
        }

        public static string GetTextVi(string type)
        {
            return type switch
            {
                Employee => "Nhân sự",
                Customer => "Khách hàng",
                Admin => "Quản trị viên",
                _ => "Khác"
            };
        }

        public static string GetBadgeClass(string type)
        {
            return type switch
            {
                Employee => "badge bg-primary",
                Customer => "badge bg-success",
                Admin => "badge bg-dark",
                _ => "badge bg-secondary"
            };
        }
    }
}