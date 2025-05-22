using System;
using System.Collections.Generic;

namespace test1.Models;

public partial class TbBrand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public string? ImgName { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdateDate { get; set; }
}
