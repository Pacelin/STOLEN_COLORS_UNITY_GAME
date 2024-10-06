using System.Runtime.CompilerServices;

namespace UniOwl
{
    public static class MathUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextLoop(in int value, in int count)
        {
            return (value + 1) % count;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PreviousLoop(in int value, in int count)
        {
            return (value - 1 + count) % count;
        }
    }
}
