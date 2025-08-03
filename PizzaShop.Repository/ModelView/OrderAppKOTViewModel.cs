
using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.ModelView;

public class OrderAppKOTViewModel
{
    public List<Category>? categories { get; set; }
    public int? StateForPartial { get; set; }
    public List<OrderKOTViewModel>? orderKOT { get; set; }
}


public class OrderKOTViewModel
{
    public int ModalStatus {get;set;}
    public int orderId { get; set; }
    public string? Ordermessage { get; set; }

    public List<Table>? table { get; set; }
    public string? sectionName { get; set; }
    public List<ItemsKOTViewModel>? itemsKOT { get; set; }
}

public class ItemsKOTViewModel
{
    public int OrderItemMappingId { get; set; }
    public int itemId { get; set; }
    public string? itemName { get; set; }
    public decimal? totalQuantity { get; set; }
    public string? specialMessage { get; set; }
    public DateTime dateTime { get; set; }
    public string? timeSpend { get; set; }
    public int? Readyquantity { get; set; }
    public int? status { get; set; } = 0;
    public List<ModifiersKOTViewModel>? ModifierKOT { get; set; }
}

public class ModifiersKOTViewModel
{
    public int modifierId { get; set; }
    public string? modifierName { get; set; }
}

public class UpdateReadyQuantityModel
{
    public int OrderItemMappingId { get; set; }
    public int ReadyQuantity { get; set; }
}


public class OrderItemModifierJoinModelView
{

    public int OrderItemMappingId { get; set; }
    public int orderId { get; set; }

    public int itemId { get; set; }

    public int modifierId { get; set; }


}


public class OrderItemModifiersMappingViewModel
{
    public int Mappingid { get; set; }
    public int orderId { get; set; }
    public int itemId { get; set; }
    public int Modifierid { get; set; }
    public int Orderitemmappingid { get; set; }
    public int status {get; set;}
    public decimal? totalQuantity {get; set;}
    public decimal? ReadyQuantity {get; set;}

}