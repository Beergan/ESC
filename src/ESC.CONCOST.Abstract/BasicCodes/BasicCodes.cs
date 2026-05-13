using System.Collections.Generic;
using System.Linq;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.Abstract;

public static class BasicCodes
{
    public static OptionDuals<string> GenderOptions = new(
        new("M", "Male", "Nam"),
        new("F", "Female", "Nữ"),
        new("U", "Other", "Không xác định")
    );

    //-Dev-Bee-CN: Danh sách loại công việc dự án (đã xong)
    public static OptionTriples<string> WorkTypeOptions = new(
      new("Structure", "Structure", "Kết cấu", "구조"),
      new("Finishing", "Finishing", "Hoàn thiện", "마감"),
      new("StructureFinishing", "Structure + Finishing", "Kết cấu + Hoàn thiện", "구조 + 마감"),
      new("Infrastructure", "Infrastructure", "Hạ tầng kỹ thuật", "기반 시설"),
      new("SurveyDesign", "Survey & Design", "Khảo sát & Thiết kế", "조사 및 설계")
  );

}