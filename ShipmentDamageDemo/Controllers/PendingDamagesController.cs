using Marten;
using Microsoft.AspNetCore.Mvc;
using ShipmentDamageDemo.Projections;

namespace ShipmentDamageDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class PendingDamagesController : ControllerBase
{
    private readonly IQuerySession _querySession;

    public PendingDamagesController(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    [HttpGet]
    public ActionResult Get()
    {
        return Ok(_querySession.Query<DamageReportsPendingReview>().ToList());
    }
}