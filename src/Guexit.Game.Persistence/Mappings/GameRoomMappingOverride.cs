using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence.Mappings.ValueComparers;
using Guexit.Game.Persistence.Mappings.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guexit.Game.Persistence.Mappings;

internal sealed class GameRoomMappingOverride : IEntityTypeConfiguration<GameRoom>
{
    public void Configure(EntityTypeBuilder<GameRoom> builder)
    {
        builder.Property<uint>("Version").IsRowVersion();
        
        builder.Property(x => x.Id).HasConversion(to => to.Value, from => new GameRoomId(from));
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedBy).HasConversion(to => to.Value, from => new PlayerId(from)).IsRequired();
        
        builder.Property(x => x.PlayerIds)
            .HasConversion<PlayerIdsToCommaSeparatedTextCollectionValueConverter>()
            .Metadata
            .SetValueComparer(new PlayerIdsCommaSeparatedTextCollectionValueComparer());

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.RequiredMinPlayers)
            .HasConversion(to => to.Count, from => new RequiredMinPlayers(from))
            .IsRequired();

        builder.ComplexProperty(x => x.CurrentStoryTeller, st =>
        {
            st.Property(x => x.PlayerId).HasConversion(to => to.Value, from => new PlayerId(from));
            st.Property(x => x.Story);
        });

        builder.Property(x => x.Status)
            .HasConversion(to => to.Value, from => GameStatus.From(from))
            .IsRequired();

        builder.Property(x => x.NextGameRoomId)
            .HasConversion(to => to.Value, from => new GameRoomId(from))
            .HasDefaultValue(GameRoomId.Empty);
        
        builder.HasMany(x => x.PlayerHands).WithOne().HasForeignKey(x => x.GameRoomId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Deck).WithOne().HasForeignKey("GameRoomId").OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.SubmittedCards).WithOne().HasForeignKey(x => x.GameRoomId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.FinishedRounds).WithOne().HasForeignKey(x => x.GameRoomId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.CurrentCardReRolls).WithOne().HasForeignKey("GameRoomId").OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.PlayerHands).AutoInclude();
        builder.Navigation(x => x.Deck).AutoInclude();
        builder.Navigation(x => x.SubmittedCards).AutoInclude();
        builder.Navigation(x => x.FinishedRounds).AutoInclude();
        builder.Navigation(x => x.CurrentCardReRolls).AutoInclude();

        builder.HasIndex(x => x.NextGameRoomId);
    }
}
