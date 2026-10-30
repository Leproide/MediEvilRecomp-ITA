using RecompOne.Runtime.Memory;
using Recompiled;
using RecompOne.Runtime.Dispatch;

if (AutoUpdater.HandleRelaunch(args)) return 0;

var title = "MediEvilRecomp";

var asm = System.Reflection.Assembly.GetExecutingAssembly();

if (Array.Find(asm.GetManifestResourceNames(), n => n.EndsWith(".languages.json", StringComparison.OrdinalIgnoreCase)) is { } languagesRes)
    RecompOne.Runtime.Runtime.AddLanguages(asm, languagesRes);

if (Array.Find(asm.GetManifestResourceNames(), n => n.EndsWith(".MediEvilRecomp.ico", StringComparison.OrdinalIgnoreCase)) is { } iconRes)
{
    using var iconStream = asm.GetManifestResourceStream(iconRes)!;
    using var iconMem = new MemoryStream();
    iconStream.CopyTo(iconMem);
    RecompOne.Runtime.Runtime.SetIcon(iconMem.ToArray());
}


RecompOne.Runtime.Hle.NativeGeometry.Enabled = false;

Dispatcher.Tolerant = true;

TerrainPatch.Register();
WaterMaterialPatch.Register();
TitleShaderPatch.Register();
WidescreenPatch.Register();
QualityOfLifeMenu.Register();
AutoUpdater.Register();
// Mod Hub della versione PAL (MediEvilRecomp ITA, Author: https://github.com/Leproide).
// Non usare l'hub originale BlackLabelHQ/MediEvilRecompModHub: le sue mod usano indirizzi
// della versione USA e su questa build romperebbero il gioco.
// L'indirizzo e' un repository git https (GitHub, GitLab, Gitea/Forgejo/Codeberg o altro
// server git). Si puo' cambiare senza ricompilare con la chiave "modhub.repository" in
// settings.json. Stringa vuota = hub disattivato (menu "Explore Mod Hub" grigio).
const string DefaultModHub = "https://github.com/Leproide/MediEvilRecomp-ITA-ModHub";
var modHub = RecompOne.Runtime.Config.Settings.Get("modhub.repository", DefaultModHub);
if (!string.IsNullOrWhiteSpace(modHub))
    RecompOne.Runtime.Hub.ModHub.Configure(modHub, RecompOne.Runtime.Config.Settings.Get("modhub.branch", "master"));

// Shader: di default l'immagine passata allo shader e' quella alla risoluzione scelta,
// non quella nativa della PSX (con "Native resolution input" attivo Dan's Curse e gli
// altri shader non CRT diventano pixelosi). Resta attivabile da Shaders.
if (!RecompOne.Runtime.Config.Settings.Has("mods.shaders.native"))
    RecompOne.Runtime.Config.Settings.Set("mods.shaders.native", false);

// Predefiniti del fork: colore a 24 bit e "Better rendering" attivi. Valgono solo se
// l'utente non ha mai cambiato l'opzione (settings.json senza quella chiave).
if (!RecompOne.Runtime.Config.Settings.Has("display.true_color"))
    RecompOne.Runtime.Config.Settings.Set("display.true_color", true);
if (!RecompOne.Runtime.Config.Settings.Has("render.better_rendering"))
    RecompOne.Runtime.Config.Settings.Set("render.better_rendering", true);

RecompOne.Runtime.Runtime.Run(() => Entry.Run(new PSMemory(0x00400000), args.Length > 0 ? args[0] : null, title));
return 0;
