using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations;

public class SectionRepository : ISectionRepository
{   
    private readonly PizzaShop2Context _context;

    public SectionRepository(PizzaShop2Context context)
    {
        _context = context;       
    }

    public async Task<List<Section>?> GetAllSections()
    {
        return await _context.Sections
                     .Where(x => x.Isdeleted == false)
                     .OrderBy(x => x.Sectionid)
                     .ToListAsync();
    }

    public async Task<List<Section>?> CheckSectionByName(string name)
    {
        return await _context.Sections.Where(s => s.Sectionname.Trim().ToLower().Equals(name) && s.Isdeleted == false).ToListAsync();
    }

    public async Task<Section?> GetSectionById(int sectionid)
    {
        return await _context.Sections
                     .Where(x => x.Sectionid == sectionid)
                     .FirstOrDefaultAsync();
    }

    public async Task<string> UpdateSection(Section section)
    {
        try{
            _context.Update(section);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<string> AddSection(Section section)
    {
        try{
            _context.Add(section);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }
}
