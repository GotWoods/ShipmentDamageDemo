using Marten.Linq.SoftDeletes;

namespace ShipmentDamageDemo.Domain;

public record CarrierCreated(Guid Id, string Name);

public record CarrierRenamed(string NewName);

public class Carrier
{
    public string Name { get; set; } = string.Empty;
    public Guid CarrierId { get; set; }

    public void Apply(CarrierCreated e)
    {
        this.CarrierId = e.Id;
        this.Name = e.Name;
    }
    public void Apply(CarrierRenamed e)
    {
        this.Name = e.NewName;
    }

    
}