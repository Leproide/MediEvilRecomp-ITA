// PAL port: addresses/constants rewritten for the PAL release by
// medievil-pal-port (GPL-3.0-or-later), Author: https://github.com/Leproide
// Original code: MediEvilRecomp by BlackLabelHQ - their license terms apply to it.

using RecompOne.Runtime.Events;
using RecompOne.Runtime.Host.Window;

namespace Recompiled;

public static class QualityOfLifeMenu
{
    public static void Register()
    {
        QualityOfLife.Load();
        Event.AddListener<RuntimeReadyEvent>(_ => QualityOfLife.Load());
        
        foreach (var option in QualityOfLifeOptions.All)
            option.Changed += _ => QualityOfLife.Load();

        PanelManager.Register(new QualityOfLifePanel());

        MenuRegistry.Menu("menu.misc").After("menu.mods").Panel<QualityOfLifePanel>("panel.qol");
    }
}
