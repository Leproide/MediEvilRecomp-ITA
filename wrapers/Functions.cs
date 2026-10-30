// PAL port: addresses/constants rewritten for the PAL release by
// medievil-pal-port (GPL-3.0-or-later), Author: https://github.com/Leproide
// Original code: MediEvilRecomp by BlackLabelHQ - their license terms apply to it.

namespace MediEvil;

public static class Functions
{
    //stupid library i hate you
    public const uint MRAllocMem = 0x800A40B8u;
    public const uint MRFreeMem = 0x800A4330u;
    public const uint MRInitialiseResources = 0x800A7EC8u;
    public const uint MRLoadResource = 0x800A80C8u;
    public const uint MRProcessResource = 0x800A845Cu;
    public const uint MRUnloadResource = 0x800A85E0u;
    public const uint PowerPackerDecrunch = 0x800BA550u;
    public const uint MRAllocateDynamicPolys = 0x800A483Cu;
    public const uint MRFreeDynamicPolys = 0x800A48ACu;
    public const uint MRRenderViewport = 0x8009FDF4u;
    public const uint MRKillViewport = 0x8009F370u;
    public const uint MRKillAllViewports = 0x8009BC78u;
    public const uint MRUpdateViewportRenderMatrices = 0x800A04FCu;

    //game stuffs
    public const uint CamUpdate = 0x8002D274u;
    public const uint FadeUpdate = 0x8004EB60u;
    public const uint EntLibPointOccluded = 0x800494C8u;
    public const uint LevelStartUp = 0x80065618u;
    public const uint LevelShutDown = 0x800658BCu;
    public const uint FreeTerrShutDown = 0x8005153Cu;
    public const uint TerrainFogShutdown = 0x8007A140u;
}
