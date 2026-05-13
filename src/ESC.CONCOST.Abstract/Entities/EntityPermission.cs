using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

[Table("SETTING_PERMISSION")]
public class EntityPermission : EntityBase
{
    [Display(Name = "Tên nhóm|그룹명")]
    public string GroupName { get; set; }

}