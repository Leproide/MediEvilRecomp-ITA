# MediEvilRecomp ITA - port PAL italiano (SCES-01494)

Prima versione del fork per MediEvil PAL italiano.

## Novità
- Configurazione e funcmap tradotte per la versione PAL italiana: boot `SCES_014.94`, `MEDIEVIL.EXE` e i 26 overlay.
- Patch di rendering e widescreen (`CullPatch`, `EntityFloatFunctions`, `TerrainPatch`, `TerrainCapture`, `LandMapPatch`) portate sugli indirizzi PAL, con i valori a 50 Hz e la mappa a 256 righe.
- Conservati i nomi delle funzioni SDK, così RecompOne usa le sue implementazioni HLE di VSync, CdRead e simili.
- Nuovo `PalDispatch.cs`: le chiamate agli overlay passano per indirizzo e non dipendono dai suffissi dei nomi.
- Nuovo `PalDebug.cs`: log di debug attivabili con `MEDIEVIL_DEBUG`.
- Correzione di RecompOne per la modalità settore intero del lettore CD (`recompone-fixes/`), applicata in automatico dagli script.
- Script di compilazione in italiano, con i nomi dei file `(Italy)`.
- Aggiornamento automatico collegato alle release di questo fork.
- Strumenti di porting e di verifica in `tools/pal-port/`.

Autore del port: https://github.com/Leproide

---

# 

v0.2b* is not a massive update, just something that i wanted to fix way before the first release but havent had time! the rendering now should be fairly decent looking!

thanks for the support on the initial release! i hope you guys enjoy it!

# Changes
- ditched pgxp renderer in favor of a ""native"" rendering patch to use float, depth buffer and correct textures (the power of recomp patches!)
- fixed overworld map not rendering properly on 16:9
- fixed an issue with the clear buffer not covering 16:9
- added a "better rendering" option to improve the terrain/entity rendering and camera, interpolation is also improved
- added automatic updater
- included shadermod

# Dependencies

You're required to install [dotnet 10 runtime](https://dotnet.microsoft.com/pt-br/download/dotnet/10.0) to run the recomp, otherwise the game will not open, ideally also make sure to install [OpenAL](https://www.openal.org/downloads/), as of the current version no further dependencies are required

you can find instructions downloading dotnet 10 for linux systems that dont have it on their packages [here](https://wiki.archlinux.org/title/.NET), or you can run "run.sh" wich will install dotent if not available and execute sotn for you