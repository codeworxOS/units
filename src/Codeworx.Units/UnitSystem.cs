using System;

namespace Codeworx.Units
{
    [Flags]
    public enum UnitSystem : byte
    {
        Metric =   0x01,
        Imperial = 0x02,

        Both = Metric | Imperial,
    }
}
