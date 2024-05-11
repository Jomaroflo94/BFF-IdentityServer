using System.ComponentModel.DataAnnotations;

namespace Resource.Api.Entities;

public class ProgrammingLanguage
{   
    [Key]
    public required int Id { get; set; }
    public required string Name { get; set; }
}
