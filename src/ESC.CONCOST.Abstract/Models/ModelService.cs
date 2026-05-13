using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

public class ModelService
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    [Required]
    public string Code { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; }
    public decimal Price { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
