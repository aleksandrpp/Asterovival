using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AK.Asterovival.Parts
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Lifetime
    {
        public float Duration;
        public float Timer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TickAlive(float deltaTime)
        {
            Timer += deltaTime;
            return IsAlive();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive()
        {
            return Timer < Duration;
        }
    }
}
