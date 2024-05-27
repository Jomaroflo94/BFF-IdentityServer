
using Resource.Api.DbContexts;
using Resource.Api.Entities;

namespace Resource.Api.Repositories;

internal class ProgrammingLanguagesRepository(ApiContext _context) 
    : IProgrammingLanguagesRepository
{
    IEnumerable<ProgrammingLanguage> IProgrammingLanguagesRepository.Get()
    {
        return [.. _context.ProgrammingLanguages];
    }

    ProgrammingLanguage? IProgrammingLanguagesRepository.Get(int id)
    {
        return _context.ProgrammingLanguages
            .FirstOrDefault(f => f.Id.Equals(id));
    }

    bool IProgrammingLanguagesRepository.Exists(int id)
    {
        return _context.ProgrammingLanguages
            .Any(f => f.Id.Equals(id));
    }

    Task IProgrammingLanguagesRepository.AddAsync(ProgrammingLanguage data)
    {
        _context.ProgrammingLanguages.Add(data);

        return _context.SaveChangesAsync();
    }

    Task IProgrammingLanguagesRepository.UpdateAsync(ProgrammingLanguage data)
    {
        _context.ProgrammingLanguages.Update(data);

        return _context.SaveChangesAsync();
    }

    Task IProgrammingLanguagesRepository.DeleteAsync(ProgrammingLanguage data)
    {
        _context.ProgrammingLanguages.Remove(data);

        return _context.SaveChangesAsync();
    }
}
