using Marten.Events.Aggregation;
using ShipmentDamageDemo.Domain;

namespace ShipmentDamageDemo.Projections
{
    public class DamageReportsPendingReview
    {
        public Guid Id { get; set; }
        public string ShipmentId { get; set; } = string.Empty;

        public string LastNote { get; set; } = string.Empty;
    }

    public class DamageReportsPendingReviewProjection : SingleStreamProjection<DamageReportsPendingReview>
    {
        public DamageReportsPendingReviewProjection()
        {
            DeleteEvent<Approved>();
        }

        public DamageReportsPendingReview Create(DamagedShipmentReported e)
        {
            var result = new DamageReportsPendingReview();
            result.Id = e.Id;
            result.ShipmentId = e.ShipmentId;
            return result;
        }

        public void Apply(NoteAdded e, DamageReportsPendingReview current)
        {
            current.LastNote = e.Note;
        }
        
    }
}
