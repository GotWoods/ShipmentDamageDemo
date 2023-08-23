using Marten;
using Microsoft.AspNetCore.Mvc;
using ShipmentDamageDemo.Domain;

namespace ShipmentDamageDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class DamageController : ControllerBase
{
    private readonly ILogger<DamageController> _logger;
    private readonly IDocumentSession _documentSession;
    private readonly IQuerySession _querySession;


    public DamageController(ILogger<DamageController> logger, IDocumentSession documentSession, IQuerySession querySession)
    {
        _logger = logger;
        _documentSession = documentSession;
        _querySession = querySession;
    }

    [HttpPost]
    public async Task<ActionResult> Create(DamagedShipmentReport report)
    {
        var id = Guid.NewGuid();
        _documentSession.Events.StartStream<DamagedShipment>(id, 
            new DamagedShipmentReported(id, report.ShipmentId, report.CarrierId),
            new DamageEstimated(report.CarrierId, report.ShipmentId, report.DamageEstimate)
            );

        await _documentSession.SaveChangesAsync();
        return Ok("Created!");
    }
    
    [HttpPut("{id}/note")]
    public async Task<ActionResult> AddNote(Guid id, string noteData)
    {
        _documentSession.Events.Append(id, new NoteAdded(noteData));
        await _documentSession.SaveChangesAsync();
        return Ok("Note Added");
    }
        
    [HttpPut("{id}/approve")]
    public async Task<ActionResult> Approve(Guid id, double damageAmount)
    {
        var damagedShipment = await _querySession.Events.AggregateStreamAsync<DamagedShipment>(id);
        _documentSession.Events.Append(id, 
            new Approved(), 
            new DamageEstimated(damagedShipment.CarrierId, damagedShipment.ShipmentId, damageAmount));
        await _documentSession.SaveChangesAsync();
        return Ok("Approved");
    }
        
    [HttpGet]
    public async Task<ActionResult> Get(Guid id)
    {
        var damagedShipment = await _querySession.Events.AggregateStreamAsync<DamagedShipment>(id);
        return Ok(damagedShipment);
    }


}


public class DamagedShipmentReport
{
    public string ShipmentId { get; set; }
    public Guid CarrierId { get; set; }
    public double DamageEstimate { get; set; }
}