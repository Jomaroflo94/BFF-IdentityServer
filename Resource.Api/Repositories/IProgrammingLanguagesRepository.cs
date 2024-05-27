using Resource.Api.Entities;

namespace Resource.Api.Repositories;

public interface IProgrammingLanguagesRepository
{
    internal IEnumerable<ProgrammingLanguage> Get();
    internal ProgrammingLanguage? Get(int id);
    internal bool Exists(int id);
    internal Task AddAsync(ProgrammingLanguage data);
    internal Task UpdateAsync(ProgrammingLanguage data);
    internal Task DeleteAsync(ProgrammingLanguage data);
}
