using Microsoft.AspNetCore.Mvc;
using PetHostelApi.Entities;

namespace PetHostelApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CommerceController : ControllerBase
{
    private readonly ILogger<CommerceController> _logger;
    private readonly AppDbContext context;

    public CommerceController(ILogger<CommerceController> logger,AppDbContext context)
    {
        _logger = logger;
        this.context = context;
    }

    [HttpGet("{id}")]
    public List<Commerce> Get(string id)
    {
        return context.Commerce.ToList();
    }
}

