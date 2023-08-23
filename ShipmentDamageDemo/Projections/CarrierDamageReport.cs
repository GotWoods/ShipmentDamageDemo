using Marten;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using ShipmentDamageDemo.Domain;

namespace ShipmentDamageDemo.Projections
{
    public class CarrierDamageReport
    {
        public Guid Id { get; set; }
        public string CarrierName { get; set;} = String.Empty;

        public Dictionary<string, double> Damages = new();
    }

    public class CarrierDamageReportProjection : MultiStreamProjection<CarrierDamageReport, Guid>
    {
        public CarrierDamageReportProjection()
        {
            Identity<CarrierCreated>(e=>e.Id); //carrier stream
            Identity<DamageEstimated>(e=>e.CarrierId); //damage stream
            Identity<IEvent<CarrierRenamed>>(e => e.StreamId); //carrier stream
            //CustomGrouping(new CarrierEventGrouper());
        }

        public void Apply (CarrierCreated e, CarrierDamageReport current)
        {
            current.Id = e.Id;
            current.CarrierName = e.Name;
        }

        public void Apply(DamageEstimated e, CarrierDamageReport current)
        {
            current.Damages.Add(e.ShipmentId, e.Amount);
        }

        public void Apply(CarrierRenamed e, CarrierDamageReport current)
        {
            current.CarrierName = e.NewName;
        }

        public void Apply(IEvent<CarrierRenamed> e, CarrierDamageReport current)
        {
            current.CarrierName = e.Data.NewName;
        }
    }


}

public class CarrierEventGrouper : IAggregateGrouper<Guid>
{
    private readonly Type[] eventTypes = { typeof(CarrierRenamed) };

    public async Task Group(IQuerySession session, IEnumerable<IEvent> events, ITenantSliceGroup<Guid> grouping)
    {
        var filteredEvents = events.Where(ev => eventTypes.Contains(ev.EventType)).ToList();

        if (!filteredEvents.Any())
            return;

        var carrierIds = filteredEvents.Select(e => e.StreamId).ToList();

        foreach (var carrierId in carrierIds)
        {
            grouping.AddEvents(carrierId, filteredEvents.Where(x=>x.StreamId == carrierId));
        }
    }
}
