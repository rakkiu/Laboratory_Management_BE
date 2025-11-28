using System.Text.Json.Serialization;

public class ReviewTestOrderCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid TestOrderId { get; set; }

    [JsonIgnore]
    public Guid ReviewedBy { get; set; }
}
