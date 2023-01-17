namespace TryGuessIt.Game.Persistence.Outbox;

public class OutboxMessage
{ 
    public Guid Id { get; set; }
    public string FullyQualifiedTypeName { get; set; } = default!;
    public string SerializedData { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public bool IsPublished => PublishedAt is not null;

    public OutboxMessage()
    {

    }

    public OutboxMessage(Guid id, string fullyQualifiedTypeName, string serializedData, DateTimeOffset createdAt)
    {
        Id = id;
        FullyQualifiedTypeName = fullyQualifiedTypeName;
        SerializedData = serializedData;
        CreatedAt = createdAt;
    }

    public void MarkAsPublished(DateTimeOffset publishedAt)
    {
        PublishedAt = publishedAt;
    }
}
