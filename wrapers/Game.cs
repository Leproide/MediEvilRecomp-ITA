// PAL port: addresses/constants rewritten for the PAL release by
// medievil-pal-port (GPL-3.0-or-later), Author: https://github.com/Leproide
// Original code: MediEvilRecomp by BlackLabelHQ - their license terms apply to it.

using RecompOne.Runtime.Memory;

namespace MediEvil;

// PAL: offset rispetto a $gp tradotti a mano (tools/pal-port non li riconosce in questo
// file), tutti verificati come corrispondenze esatte USA -> PAL:
// 0x568->0x558, 0x5E0->0x5D0, 0x7F0->0x7E0, 0x470->0x460, 0x474->0x464, 0x484->0x474, 0x7C0->0x7B0

public static class Game
{
    public const uint Gp = 0x800ED848u;

    public const uint CurrentLevelAddr = 0x800EF090u;
    public const uint PlayerSlot = 0x558u;
    public const uint MainViewportSlot = 0x5D0u;
    public const uint ViewportRootSlot = 0x7E0u;
    public const uint CameraSlot = 0x460u;
    public const uint CameraEntitySlot = 0x464u;
    public const uint CameraPluginSlot = 0x474u;
    public const uint TextureListSlot = 0x7B0u;
    public const uint TextureAttributesAddr = 0x800EDA28u;

    internal static IMemory M => RecompOne.Runtime.Runtime.Mem!;

    public static bool Available => RecompOne.Runtime.Runtime.Mem != null;

    public static uint GpWord(uint slot) => M.ReadU32(Gp + slot);

    public static int CurrentLevel => (int)M.ReadU32(CurrentLevelAddr);
    public static uint PlayerAddr => GpWord(PlayerSlot);
    public static uint MainViewportAddr => GpWord(MainViewportSlot);
    public static uint CameraAddr => GpWord(CameraSlot);
    public static uint CameraEntityAddr => GpWord(CameraEntitySlot);
    public static uint TextureListAddr => GpWord(TextureListSlot);

    public static byte TextureAttributes(int index) => M.ReadU8(TextureAttributesAddr + (uint)index);

    public static bool IsRam(uint address) => (address & 0xFF000000u) == 0x80000000u && (address & 0x00FFFFFFu) < RecompOne.Runtime.Runtime.RamSize;
}

[Flags]
public enum TextureAttribute : byte
{
    NotGround = 1 << 0,
    Water = 1 << 1,
    Mud = 1 << 2,
    DeadlyMud = 1 << 3,
    Corn = 1 << 4,
    Special1 = 1 << 5,
    Special2 = 1 << 6,
}
