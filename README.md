<!--
  MediEvilRecomp ITA - README
  Author: https://github.com/Leproide
  License: GPL-3.0-or-later
-->

# MediEvilRecomp ITA

Ricompilazione statica di **MediEvil per PlayStation, versione PAL italiana (SCES-01494)**, per giocarlo in modo nativo sui computer moderni, con widescreen e rendering migliorato.

Questo repository è un **fork** di [MediEvilRecomp](https://github.com/BlackLabelHQ/MediEvilRecomp) di **BlackLabelHQ**, che ricompila la versione USA. Tutto il lavoro di base (ricompilazione, patch di rendering e widescreen, runtime RecompOne) è loro. Questo fork lo porta sulla versione italiana.

> **Progetto in beta.** Possono esserci bug, anche gravi. Segnalali nelle [issue di questo fork](https://github.com/Leproide/MediEvilRecomp-ITA/issues).

<img width="1280" height="672" alt="immagine" src="https://github.com/user-attachments/assets/9018cac3-7458-487d-97f5-2d59d2a436f0" />

---

## Contributi: qui l'AI è benvenuta

**A differenza del progetto originale, questo fork accetta contributi scritti o assistiti dall'intelligenza artificiale.** Issue e pull request generate in tutto o in parte con strumenti AI sono accettate e valutate come tutte le altre: conta che il contributo sia corretto, testato e spiegato bene.

Lo stesso port PAL è stato realizzato con l'aiuto di uno strumento AI. Per questo i contributi vanno inviati **qui**, non al repository originale: BlackLabelHQ chiude i contributi AI e banna chi li invia. Per lo stesso motivo, per i problemi della versione italiana non chiedere aiuto sul loro Discord.

Vi segnalo anche che gli sviluppatori, soprattutto DerpPrincess, hanno un serio ritardo mentale visto il loro comportamento di base.
Molto probabilmente i tool a cui si appoggiano hanno contribuiti AI, però se provi a contribuire con un porting di un altra lingua ti bloccano a caso.
Per tanto, SCONSIGLIO a chiunque di aiutarli, con o senza AI.
Medievil gira tranquillamente emulato, un recomp era superfluo, ma a quanto pare si credono divinità.

---

## Cosa cambia rispetto all'originale

| | Originale | Questo fork |
|---|---|---|
| Versione del gioco | USA (SCUS-94227) | PAL italiana (SCES-01494) |
| `config/MediEvil.json` e `config/funcmaps/*.json` | indirizzi USA | tradotti per la PAL (boot, EXE e 26 overlay) |
| Patch C# (`patches/`) | indirizzi USA | tradotte: indirizzi, offset `$gp`, costanti a 50 Hz, 256 righe |
| `patches/PalDispatch.cs` | — | chiamate agli overlay per indirizzo |
| `patches/PalDebug.cs` | — | log di debug attivabili con `MEDIEVIL_DEBUG` |
| `recompone-fixes/` | — | correzione del lettore CD di RecompOne (modalità settore intero) |
| Script Windows | nomi file USA, in inglese | nomi file `(Italy)`, in italiano, correzione applicata in automatico |
| Aggiornamento automatico | release di BlackLabelHQ | release di questo fork |
| `tools/pal-port/` | — | strumenti che hanno generato il port, con verifica |

Il dettaglio di funzioni, abbinamenti e controlli è in [`tools/pal-port/PORT_REPORT.md`](tools/pal-port/PORT_REPORT.md).

---

## Requisiti

- GPU con supporto ad almeno OpenGL 2.1
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [OpenAL](https://www.openal.org/downloads/)
- [Git](https://git-scm.com/install/)
- Una copia **originale** di MediEvil PAL italiano, da cui ricavare un dump **pulito** in formato bin/cue. Leggi [`disc/README.md`](disc/README.md).

Questo repository **non contiene i file del gioco** e non ne distribuisce codice: tutto viene generato sul tuo computer a partire dal tuo disco.

---

## Compilazione

### Windows (consigliato)

1. Clona il repository insieme ai sottomoduli:
   ```
   git clone --recursive https://github.com/Leproide/MediEvilRecomp-ITA
   cd MediEvilRecomp-ITA
   ```
2. Copia in `disc\` i file del tuo disco con questi nomi:
   - `MediEvil (Italy) (Track 1).bin`
   - `MediEvil (Italy) (Track 2).bin`
   - `MediEvil (Italy).cue`
3. Avvia **`windows_initial_build.bat`**. Lo script:
   - aggiorna i sottomoduli;
   - applica a RecompOne la correzione in `recompone-fixes\`;
   - compila RecompOne;
   - genera il codice del gioco dal tuo disco;
   - compila il gioco.

### A mano (Windows, Linux, macOS)

Dalla cartella del repository:

```
git submodule update --init --recursive
git -C RecompOne apply ../recompone-fixes/cd-whole-sector-mode.patch
dotnet build RecompOne/RecompOne.sln
dotnet run --project RecompOne/RecompOne.Recompiler config/MediEvil.json
dotnet build RecompOne/RecompOne.sln
```

Il comando `git apply` serve solo la prima volta. Se la correzione è già applicata, segnala un errore che puoi ignorare.

---

## Avvio

- **Windows:** `windows_run.bat`. Se il gioco non è ancora compilato, la prima compilazione parte da sola.
- **Avvio rapido senza ricompilare:** `windows_run_no_build.bat`.
- **Da terminale:** `dotnet run`.

Se il gioco chiede il disco, seleziona `disc\MediEvil (Italy).cue`.

Dopo un aggiornamento di `config/` va rigenerato il codice. Cancella la cartella `generated` e rilancia `windows_initial_build.bat`.

---

## Debug

`patches/PalDebug.cs` attiva i log interni di RecompOne quando è impostata la variabile `MEDIEVIL_DEBUG`. I canali disponibili sono `cd`, `irq`, `bios`, `sdk`, `vsync`, `gpu`, `dma`, `spu`, `mdec`, oppure `all`. Se la variabile non è impostata, non cambia nulla.

Prompt dei comandi (cmd):
```
set MEDIEVIL_DEBUG=cd,irq
dotnet run > debug.txt 2>&1
```

PowerShell:
```
$env:MEDIEVIL_DEBUG = "cd,irq"
dotnet run > debug.txt 2>&1
```

Allega `debug.txt` alle issue quando segnali un blocco o un crash.

---

## Trucchi

Il gioco ha due menu trucchi integrati, e nel recomp funzionano come sulla console. In pausa tieni premuto **L2** e premi:

- **Modalità Trucchi**: Giù, Su, Quadrato, Triangolo, Triangolo, Cerchio, Giù, Su, Quadrato, Triangolo
- **Modalità Trucchi Estesa**: Triangolo, Cerchio, Triangolo, Cerchio, Cerchio, Triangolo, Sinistra, Cerchio, Su, Giù, Destra, Cerchio, Sinistra, Sinistra, Triangolo, Destra, Cerchio, Sinistra, Sinistra, Triangolo, Cerchio, Giù, Cerchio, Cerchio, Destra

In alternativa, la mod inclusa **Trucchi (MediEvil ITA)** li attiva dal pannello delle mod, senza combinazioni. I dettagli sono nel [README della mod](mods/Trucchi/README.md).

---

## Strumenti di porting

In [`tools/pal-port/`](tools/pal-port/) ci sono gli script Python che hanno generato questo port a partire dal progetto USA:
- abbinano funzione per funzione i binari USA e PAL;
- traducono configurazione, funcmap e patch C#;
- verificano il risultato.

Servono per aggiornare il fork quando l'originale riceve nuove patch. Le istruzioni sono nel [README degli strumenti](tools/pal-port/README.md).

---

## Struttura

```
config/            configurazione del recompiler e funcmap (PAL)
patches/           patch C# di rendering, widescreen e supporto PAL
recompone-fixes/   correzioni da applicare al sottomodulo RecompOne
mods/Trucchi/      mod per attivare i menu trucchi del gioco
disc/              qui vanno i file del tuo disco (non inclusi)
tools/pal-port/    strumenti di porting e report
misc/              aggiornamento automatico
assets/            icona e lingue dell'interfaccia
```

---

## Crediti

- **BlackLabelHQ** (Derp Princess, flaffymg): [MediEvilRecomp](https://github.com/BlackLabelHQ/MediEvilRecomp) e [RecompOne](https://github.com/BlackLabelHQ/RecompOne), cioè tutto il lavoro di ricompilazione su cui si basa questo fork.
- Il team della decompilazione di MediEvil, i cui riferimenti sono stati usati dal progetto originale.
- Port PAL italiano: https://github.com/Leproide

---

## Licenza

Le modifiche introdotte da questo fork (port PAL, strumenti in `tools/pal-port/`, script, `PalDispatch.cs`, `PalDebug.cs`, `recompone-fixes/`, mod `mods/Trucchi/`) sono rilasciate sotto **GPL-3.0-or-later**. Il testo completo è in [`tools/pal-port/LICENSE`](tools/pal-port/LICENSE).

Il codice originale di MediEvilRecomp e RecompOne appartiene a BlackLabelHQ e resta soggetto ai loro termini. MediEvil è un marchio dei rispettivi proprietari: questo progetto non è affiliato né approvato da Sony Interactive Entertainment.

---

## Autore

https://github.com/Leproide
