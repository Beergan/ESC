using System;

namespace ESC.CONCOST.Abstract;

public class FeatureAttribute : Attribute
{
    public string Name { get; set; }
    
    public string TextEn { get; set; }

    public string TextVi { get; set; }

    public string TextKo { get; set; }
}

public class FeatureModel
{
    public string Name { get; set; }
    
    public string TextEn { get; set; }

    public string TextVi { get; set; }

    public string TextKo { get; set; }
}

public class Function : Attribute
{
    public string TextEn { get; set; }

    public string TextVi { get; set; }

    public string TextKo { get; set; }
}