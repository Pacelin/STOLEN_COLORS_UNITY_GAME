namespace Gameplay.Map.Enemies
{
    public static class ArrayExtensions
    {
        public static T GetRandom<T>(this T[] arr) =>
            arr[UnityEngine.Random.Range(0, arr.Length)];
    }
}