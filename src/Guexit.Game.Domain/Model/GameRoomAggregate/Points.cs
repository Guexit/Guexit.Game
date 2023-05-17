﻿using System.Diagnostics;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

[DebuggerDisplay("Points: {Value}")]
public sealed class Points : ValueObject, IComparable<Points>
{
    public static readonly Points Zero = new(0);

    public int Value { get; }

    public Points(int points)
    {
        if (points < 0)
            throw new ArgumentOutOfRangeException(nameof(points));

        Value = points;
    }

    public Points Sum(Points other) => new(Value + other.Value);

    public Points Substract(Points other) => new(Value - other.Value);

    public static Points operator +(Points points1, Points points2) => points1.Sum(points2);
    public static Points operator -(Points points1, Points points2) => points1.Substract(points2);
    public static bool operator >(Points points1, Points points2) => points1.CompareTo(points2) > 0;
    public static bool operator <(Points points1, Points points2) => points1.CompareTo(points2) < 0;
    public static bool operator >=(Points points1, Points points2) => points1.CompareTo(points2) >= 0;
    public static bool operator <=(Points points1, Points points2) => points1.CompareTo(points2) <= 0;

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public int CompareTo(Points? other)
    {
        return other is null ? 1 : Value.CompareTo(other.Value);
    }
}
