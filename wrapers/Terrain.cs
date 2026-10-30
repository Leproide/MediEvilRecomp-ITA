// PAL port: addresses/constants rewritten for the PAL release by
// medievil-pal-port (GPL-3.0-or-later), Author: https://github.com/Leproide
// Original code: MediEvilRecomp by BlackLabelHQ - their license terms apply to it.

using RecompOne.Runtime.Memory;

namespace MediEvil;

public static class FreeTerrain
{
    public const uint DataAddr = 0x800EF024u;
    public const uint DataPool = DataAddr + 0x00u;
    public const uint PrimBuffer0 = DataAddr + 0x04u;
    public const uint PrimBuffer1 = DataAddr + 0x08u;
    public const uint CaptureList = DataAddr + 0x0Cu;
    public const uint RenderPolyPtr = DataAddr + 0x10u;
    public const uint MaxCapture = DataAddr + 0x14u;
    public const uint BufferLimit0 = DataAddr + 0x18u;
    public const uint BufferLimit1 = DataAddr + 0x1Cu;
    public const uint ViewDistance = DataAddr + 0x24u;
    public const uint OldViewDistance = DataAddr + 0x26u;

    static IMemory M => Game.M;

    public static short CurrentViewDistance => (short)M.ReadU16(ViewDistance);
}

public static class TerrainFog
{
    public const uint DataAddr = 0x800F1A1Cu;
    public const uint Table = DataAddr + 0x00u;
    public const uint Entries = DataAddr + 0x04u;

    static IMemory M => Game.M;

    public static uint TableAddr => M.ReadU32(Table);
    public static ushort EntryCount => M.ReadU16(Entries);
}
