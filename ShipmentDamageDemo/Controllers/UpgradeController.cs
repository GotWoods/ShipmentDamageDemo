using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Mvc;
using ShipmentDamageDemo.Projections;

namespace ShipmentDamageDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpgradeController
    {
        private readonly IDocumentStore _store;

        public UpgradeController(IDocumentStore store)
        {
            this._store = store;
        }

        [HttpGet]
        public async Task UseAsyncDaemon(CancellationToken cancellation)
        {
            using var daemon = await _store.BuildProjectionDaemonAsync();
            await daemon.StopAll();

            // Fire up everything!
            await daemon.StartAllShards();

            // or a single projection by its type
            await daemon.RebuildProjection<CarrierDamageReportProjection>(5.Minutes(), cancellation);

            // Be careful with this. Wait until the async daemon has completely
            // caught up with the currently known high water mark
            await daemon.WaitForNonStaleData(5.Minutes());
        }
    }
}
