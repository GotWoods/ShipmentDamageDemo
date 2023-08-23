namespace ShipmentDamageDemo.Domain
{
    public record DamagedShipmentReported(Guid Id, string ShipmentId, Guid CarrierId);
    public record NoteAdded(string Note);
    public record Approved();
    public record DamageEstimated(Guid CarrierId, string ShipmentId, double Amount);

    public class DamagedShipment
    {
        public Guid Id { get; set; }
        public string ShipmentId { get; set; } = string.Empty;
        public Guid CarrierId { get; set; }
        public List<string> Notes { get; set; } = new();

        public bool IsApproved { get; set; }
        public double DamageEstimate { get; set; }
        public void Apply(DamagedShipmentReported e)
        {
            this.Id = e.Id;
            this.ShipmentId = e.ShipmentId;
            this.CarrierId = e.CarrierId;
        }

        public void Apply(NoteAdded e)
        {
            this.Notes.Add(e.Note);
        }

        public void Apply(Approved e)
        {
            this.IsApproved = true;
        }

        public void Apply(DamageEstimated e)
        {
            this.DamageEstimate = e.Amount;
        }
    }
}
