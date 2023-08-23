using System.Composition;
using System.Xml.Linq;
using Marten;
using Microsoft.AspNetCore.Mvc;
using ShipmentDamageDemo.Domain;

namespace ShipmentDamageDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class CarrierController : ControllerBase
{
    private readonly IDocumentSession _documentSession;

    public CarrierController(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    [HttpPost]
    public async Task<ActionResult> Create(string name)
    {
        var id = Guid.NewGuid();
        _documentSession.Events.StartStream<Carrier>(id, new CarrierCreated(id, name));
        await _documentSession.SaveChangesAsync();
        return Ok("Created!");
    }

    [HttpPut("/{id}/rename")]
    public async Task<ActionResult> Rename(Guid id, string newName)
    {
        _documentSession.Events.Append(id, new CarrierRenamed(newName));
        await _documentSession.SaveChangesAsync();
        return Ok("Renamed!");
    }
}