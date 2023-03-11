namespace Guexit.Game.Domain.Model.ImageAggregate;

public sealed class ImageId : ValueObject
{
    public Guid Value { get; }

    public ImageId(Guid value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}