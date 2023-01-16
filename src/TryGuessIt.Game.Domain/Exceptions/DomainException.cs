﻿namespace TryGuessIt.Game.Domain.Exceptions;

public abstract class DomainException : Exception
{
	public DomainException(string message) : base(message)
	{
	}
}
