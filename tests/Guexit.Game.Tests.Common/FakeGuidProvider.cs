using Guexit.Game.Domain;

namespace Guexit.Game.Tests.Common;

public sealed class FakeGuidProvider : IGuidProvider
{
    private Guid? _value;

    public FakeGuidProvider()
    { }
    
    public FakeGuidProvider(Guid value)
    {
        _value = value;
    }

    public void Returns(Guid id) => _value = id;
    
    public Guid NewGuid() => _value ?? Guid.NewGuid();
}