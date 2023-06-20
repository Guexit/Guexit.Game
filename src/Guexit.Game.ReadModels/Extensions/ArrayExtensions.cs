namespace Guexit.Game.ReadModels.Extensions;

internal static class ArrayExtensions
{
    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            var k = Random.Shared.Next(n + 1);
            T? value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }
}