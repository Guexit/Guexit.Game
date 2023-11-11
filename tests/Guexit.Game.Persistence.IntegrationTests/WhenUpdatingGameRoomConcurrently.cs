using System.Data;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence.Repositories;
using Guexit.Game.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Guexit.Game.Persistence.IntegrationTests;

public sealed class WhenUpdatingGameRoomConcurrently : DatabaseMappingIntegrationTest
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ManualResetEventSlim _waiterForSecondUpdate = new(false);
    private bool _firstThreadArrived;
    
    public WhenUpdatingGameRoomConcurrently(IntegrationTestFixture fixture, ITestOutputHelper testOutput) : base(fixture, testOutput)
    { }

    [Fact]
    public async Task TwoPlayersJoiningThrowsDbUpdateConcurrencyException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var repository = new GameRoomRepository(DbContext);
        await repository.Add(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator("creator")
            .Build());
        
        await SaveChanges();

        var twoPlayersJoiningConcurrently = async () => await Task.WhenAll(
            ExecuteEnsuringConcurrency(gameRoomId, g => g.Join("joiningPlayer1")),
            ExecuteEnsuringConcurrency(gameRoomId, g => g.Join("joiningPlayer2"))
        );
        
        await twoPlayersJoiningConcurrently.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }

    [Fact]
    public async Task TwoPlayersVotingThrowsDbUpdateConcurrencyException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var initialStoryTeller = new PlayerId("creatorPlayerId");
        var invitedPlayers = new PlayerId[] { "invitedPlayer1", "invitedPlayer2", "invitedPlayer3" };

        var repository = new GameRoomRepository(DbContext);
        await repository.Add(GameRoomBuilder.CreateStarted(gameRoomId, initialStoryTeller, invitedPlayers)
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard(invitedPlayers)
            .Build());
        await SaveChanges();

        var twoPlayersVotingConcurrently = async () => await Task.WhenAll(
            ExecuteEnsuringConcurrency(gameRoomId, gameRoom =>
            {
                var cardId = gameRoom.SubmittedCards.First(x => x.PlayerId != invitedPlayers[0]).Card.Id;
                gameRoom.VoteCard(invitedPlayers[0], cardId);
            }), 
            ExecuteEnsuringConcurrency(gameRoomId, gameRoom =>
            {
                var cardId = gameRoom.SubmittedCards.Last(x => x.PlayerId != invitedPlayers[1]).Card.Id;
                gameRoom.VoteCard(invitedPlayers[1], cardId);
            })
        );

        await twoPlayersVotingConcurrently.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }

    private async Task ExecuteEnsuringConcurrency(GameRoomId gameRoomId, Action<GameRoom> criticalAction)
    {
        await using var dbContext = new GameDbContext(DbContextOptions);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var repository = new GameRoomRepository(dbContext);
        GameRoom? gameRoom;

        await _semaphore.WaitAsync(new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
        try
        {
            gameRoom = await repository.GetBy(gameRoomId);

            if (!_firstThreadArrived)
            {
                _firstThreadArrived = true;
                
                _semaphore.Release(); // Allow the second thread to enter
                _waiterForSecondUpdate.Wait(TimeSpan.FromSeconds(5)); // Wait for the second thread
            }
            else
            {
                // Signal the first thread that it can continue
                _waiterForSecondUpdate.Set();
            }
        }
        finally
        {
            if (!_waiterForSecondUpdate.IsSet)
            {
                _semaphore.Release();
            }
        }

        gameRoom!.Should().NotBeNull();
        
        criticalAction.Invoke(gameRoom!);

        dbContext.Entry(gameRoom!).State = EntityState.Modified;
        
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}