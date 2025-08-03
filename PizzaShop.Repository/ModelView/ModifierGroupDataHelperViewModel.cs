using System;

namespace PizzaShop.Repository.ModelView;

public class ModifierGroupDataHelperViewModel
{
    public int ModifierGroupid { get; set; }
    public string? Modifiergroupname { get; set; }
    public int? MinValue { get; set; } // Must match "minValue" in JSON (case-insensitive with options)
    public int? MaxValue { get; set; } // Must match "maxValue" in JSON (case-insensitive with options)
    public List<ModifierData>? Modifiers { get; set; }
}

public class ModifierData
{
    public int ModifierId { get; set; }
    public string? Modifiername { get; set; }
    public decimal? Modifierrate { get; set; }
}