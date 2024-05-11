using Microsoft.AspNetCore.Mvc;
using Resource.Api.Entities;
using Resource.Api.Repositories;

namespace Resource.Api;

[Route("api/[controller]")]
[ApiController]
public class ProgrammingLanguageController(
    IProgrammingLanguagesRepository programmingLanguagesRepository
    ) : ControllerBase
{
    private readonly IProgrammingLanguagesRepository 
        _ProgrammingLanguagesRepository = programmingLanguagesRepository;

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_ProgrammingLanguagesRepository.Get());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var result = _ProgrammingLanguagesRepository.Get(id);

        if(result is null)
        {
            return NotFound(id);
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] ProgrammingLanguage data)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(_ProgrammingLanguagesRepository.Exists(data.Id))
        {
            return Ok($"An object with Id = {data.Id} already exists in the list.");
        }

        await _ProgrammingLanguagesRepository.AddAsync(data);

        return CreatedAtAction(nameof(Get), new { data.Id } , data);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] ProgrammingLanguage data)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(!_ProgrammingLanguagesRepository.Exists(data.Id))
        {
            return NotFound($"There is no object with Id = {data.Id}");
        }

        await _ProgrammingLanguagesRepository.UpdateAsync(data);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        var data = _ProgrammingLanguagesRepository.Get(id);

        if(data is null)
        {
            return NotFound($"There is no object with Id = {id}");
        }

        await _ProgrammingLanguagesRepository.DeleteAsync(data); 

        return NoContent(); 
    }
}
