﻿namespace TryGuessIt.Game.Application.Exceptions;

public class AggregateNotFoundException : Exception
{
    public AggregateNotFoundException(string message) : base(message)
    {
    }
}