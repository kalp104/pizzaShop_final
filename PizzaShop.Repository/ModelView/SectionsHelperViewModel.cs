using System;
using System.ComponentModel.DataAnnotations;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.ModelView;

public class SectionsHelperViewModel
{
    //ids
    public int Sectionid { get; set; }
    public int Userid { get; set; }
    public int Tableid { get; set; }

    // pagging info
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalTables { get; set; }

    // helper view models
    public List<Section>? Sections { get; set; }
    public List<Table>? Tables { get; set; }


    //helper properties
    [Required(ErrorMessage = "Section name is required")]
    [MaxLength(40, ErrorMessage = "Name limit exceed ")]
    public string Sectionname { get; set; } = null!;

    [Required(ErrorMessage = "Section Description is required")]
    [MaxLength(250, ErrorMessage = "description limit exceed ")]
    public string? Sectiondescription { get; set; }

    [Required(ErrorMessage = "Table name is required")]
    [MaxLength(40, ErrorMessage = "Name limit exceed ")]
    public string? Tablename { get; set; }

    [Required(ErrorMessage = "Capacity is required")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "capacity must be a number")]
    public int? Capacity { get; set; }
    public string SectionnameTableHelper { get; set; } = null!;
    public int? Status { get; set; } 
    
}

public class TableHelper {
    public int Tableid { get; set; }
    public string Tablename { get; set; } = null!;
    public int Capacity { get; set; }
    public string Sectionname { get; set; } = null!;
    public int Sectionid { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
    
}