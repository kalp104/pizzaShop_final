using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces;

public interface ISectionRepository
{
    Task<List<Section>?> GetAllSections();
    Task<Section?> GetSectionById(int sectionid);
    Task<List<Section>?> CheckSectionByName(string name);
    Task<string> UpdateSection(Section section);
    Task<string> AddSection(Section section);
}
