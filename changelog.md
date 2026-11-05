# MediEvilRecomp ITA v1.1.15

- **Runa sulla mano visibile da lontano corretta**: le mani che reggono le rune (tipo `800D73C0`) da lontano mostravano la runa già messa prima di averla messa. Non vengono più precaricate e da lontano si vedono solo nello stato in cui il gioco le ha mostrate l'ultima volta da vicino (`patches/render/DistantObjectsPatch.cs`).
- **Casse già aperte**: da lontano si vedevano chiuse (tipo `800D528C`); ora restano come il gioco le ha mostrate l'ultima volta da vicino.
- Gli oggetti lontani vengono caricati almeno fin dove si vede il terreno (`TerrainPatch.CurrentViewDistance`): in alcuni livelli le mani vuote comparivano in ritardo.
- **Pannello di diagnostica degli oggetti lontani** (solo con `MEDIEVIL_DEBUG`, in Misc → Quality of life): elenca i tipi di oggetti del livello e spuntandone uno lo nasconde del tutto a qualsiasi distanza, subito e senza riavviare, per riconoscere gli oggetti a schermo. Spiegato nel README.
- **Nuova opzione Map edge fade** (Misc → Quality of life, spenta di default): il terreno vicino al vuoto oltre i bordi della mappa sfuma al nero in base alla distanza dal bordo, invece di finire a scalini (`patches/render/TerrainEdgeFade.cs`). Scurisce anche oggetti di scena vicini al bordo e, in alcuni livelli come il cimitero, zone che non sono bordi: per questo è spenta.
- Shader Dan's Curse V2 aggiornato a parte: nuove opzioni *Disable fog* e *Keep game darkness in fog*, e vuoto oltre i bordi della mappa nero invece che grigio.
- `.gitignore`: le mod di diagnostica (`mods/Diag*/`), la mod locale di conversione delle texture e i log restano fuori dal repo.

---

# MediEvilRecomp ITA v1.1.14

- **RecompOne aggiornato alla versione di MediEvilRecomp 1.0.3** (`b18bec0`, prima `cc18281`). Correzioni incluse:
  - pacchetti di texture: l'area di VRAM disegnata veniva calcolata male e alcune texture sostitutive venivano associate alle texture sbagliate;
  - pacchetti di texture: entrando in un livello che inizia con un filmato le texture sostitutive non venivano caricate bene;
  - audio XA sostituito: riproduzione e ricerca più affidabili, e il lettore CD non riempie il buffer XA mentre è in corso una ricerca;
  - selettore dei file (disco, pacchetti di texture): su Windows si apre in un thread separato e non blocca più la finestra, su Linux con Wayland non crasha più;
  - piccole correzioni all'editor dei pacchetti di texture e alla telecamera degli shader.
- **Nuova opzione Disable camera collision** (Misc → Quality of life, dalla 1.0.3 originale): con la telecamera libera, la telecamera non si avvicina più a Dan quando c'è un muro o una recinzione in mezzo. Spenta di default; le correzioni del fork alla telecamera restano.
- **Barra dei menu nascosta automaticamente: resta quella del fork.** RecompOne ora ha una sua versione (la barra compare portando il mouse in cima allo schermo) sulla stessa impostazione; il fork usa un'impostazione propria e lascia spenta quella di RecompOne. Chi aveva l'opzione accesa la ritrova accesa.
- La correzione del lettore CD del fork (`recompone-fixes/`) si applica senza modifiche al nuovo RecompOne.

---

# MediEvilRecomp ITA v1.1.13

- **Oggetti visibili da lontano di nuovo attivi di default** (Misc → Quality of life → Distant objects), con il precaricamento per tutti gli oggetti come prima della v1.1.12. Il crash al Villaggio Addormentato non dipendeva dalle modifiche del fork (succede anche con gli oggetti lontani spenti): lo causa il salto di un passaggio della trama con i trucchi. Il livello carica i modelli a gruppi man mano che la trama avanza; ottenendo l'oggetto coi trucchi il gruppo successivo non viene mai caricato e, entrando dal camino della casa dopo aver messo la croce in chiesa, il gioco inizializza un oggetto il cui modello (429) non è in memoria. Spiegato nella sezione Trucchi del README. Chi aveva cambiato l'opzione nelle impostazioni mantiene la sua scelta.
- Restano i 2 MB di memoria in più e la protezione per memoria quasi esaurita della v1.1.12.

---

# MediEvilRecomp ITA v1.1.12

- **Oggetti visibili da lontano spenti di default** (Misc → Quality of life → Distant objects): con l'opzione attiva il Villaggio Addormentato può crashare, perché il gioco inizializza un oggetto del livello caricato molto prima del normale. In corso di analisi; i fix restano e l'opzione si può riattivare a proprio rischio. Chi l'aveva già cambiata nelle impostazioni mantiene la sua scelta.
- **Crash per memoria esaurita corretto** (per esempio a Scarecrow Fields con Draw distance alta): gli oggetti caricati da lontano riempivano la memoria del gioco (poco più di 100 KB liberi) e il gioco crashava in punti diversi. Ora il gioco ha 2 MB di memoria in più: la memoria emulata passa da 4 a 8 MB e il blocco viene aggiunto all'allocatore del gioco all'avvio (`patches/HeapExpansion.cs`). I controlli degli indirizzi delle patch usano tutta la memoria.
- Precaricamento degli oggetti lontani limitato agli oggetti di scena (porte, cancelli, decorazioni): i personaggi del livello non vengono più svegliati per un frame da lontano e restano invisibili da lontano come nell'originale.
- Protezione: se la memoria libera scende comunque sotto 96 KB, gli oggetti lontani vengono sospesi finché non torna sopra 256 KB (`patches/render/DistantObjectsPatch.cs`).

---

# MediEvilRecomp ITA v1.1.11

- **Muri distruttibili visibili da lontano**: il gioco li disegna solo quando sono "svegli", quindi da lontano (Draw distance alta, shader o telecamera libera) sparivano in qualsiasi stato, lasciando vedere la stanza dietro. Ora entro la distanza allargata restano svegli e visibili; rottura e colpi funzionano come prima (`patches/render/DistantObjectsPatch.cs`).
- Diagnostica degli oggetti lontani più completa con `MEDIEVIL_DEBUG`; il README spiega come funziona il caricamento degli oggetti lontani e come trovare il tipo di un oggetto che sparisce.

---

# MediEvilRecomp ITA v1.1.10

- **Oggetti visibili da lontano**: ora anche le fiaccole vengono disegnate da lontano invece di sparire; da lontano sono spente e si accendono avvicinandosi, come nell'originale (`patches/render/DistantObjectsPatch.cs`).

---

# MediEvilRecomp ITA v1.1.9

- **Oggetti visibili da lontano**: anche a **Draw distance** 1x (utile con gli shader, dove la nebbia lascia vedere lontano) porte, cancelli e decorazioni vengono caricati e disegnati da lontano invece di comparire solo vicino a Dan (per esempio i battenti del cancello del cimitero). Vale con qualsiasi telecamera. Nemici, trappole ed eventi si attivano alla distanza originale; da lontano gli oggetti restano fermi e le fiaccole compaiono, come nell'originale, solo avvicinandosi. Attivo di default, si spegne da **Misc → Quality of life → Distant objects** (`patches/render/DistantObjectsPatch.cs`).

---

# MediEvilRecomp ITA v1.1.8

- Il cursore del mouse sparisce dopo qualche secondo fermo (anche a schermo intero e giocando col controller) e ricompare muovendolo.
- **System → Autohide Menu Bar** ora funziona (prima era una voce disattivata): la barra dei menu si nasconde dopo qualche secondo senza muovere il mouse e ricompare muovendolo. F1 resta per mostrarla o nasconderla a mano (`misc/InterfaceAutoHide.cs`).

---

# MediEvilRecomp ITA v1.1.7

- Telecamera libera con la levetta destra **spenta di default**: si gioca con la telecamera originale. Si riaccende da **Misc → Quality of life → Right stick camera**. Chi l'aveva già cambiata nelle impostazioni mantiene la sua scelta.
- **R3** (pressione della levetta destra) accende e spegne la telecamera libera in qualsiasi momento, con un avviso a schermo; la scelta viene salvata (`patches/qol/CameraToggle.cs`).

---

# MediEvilRecomp ITA v1.1.6

- **Aggiornamenti del Mod Hub all'avvio**: se una mod, un pacchetto o uno shader installato dal Mod Hub ha una versione nuova, compare un avviso con l'elenco e il pulsante **Update All**. Se lo shader in uso viene aggiornato si ricarica subito, tenendo i parametri cambiati. Si disattiva dal menu **Updates → Mod Hub Updates on Startup** (`misc/ModHubUpdateNotice.cs`).
- La distanza di disegno del gioco viene passata agli shader (`patches/render/ShaderDrawDistance.cs`): Dan's Curse V2 la usa per sfumare il limite di disegno al nero, come nell'originale, senza che la nebbia si avvicini davanti ai muri.
- Shader Dan's Curse V2 aggiornato: limite di disegno e vuoto sotto l'orizzonte scuri invece che grigi, bloom predefinito a 0.70.

---

# MediEvilRecomp ITA v1.1.5

- **Sistema → Esci** a schermo intero: prima di chiudere il gioco torna in finestra. Chiudere con lo schermo intero esclusivo attivo poteva bloccare il processo nel driver video, al punto che nemmeno il Task Manager riusciva a terminarlo.

---

# MediEvilRecomp ITA v1.1.4

- Uscita dal menu **Sistema → Esci** senza blocchi: prima il gioco usciva senza fermare l'audio e su Windows poteva restare bloccato (chiudendo con la X funzionava). Ora Esci chiude il gioco come la X della finestra; se dopo qualche secondo il processo e' ancora attivo, viene terminato (`patches/PalQuitFix.cs`).

---

# MediEvilRecomp ITA v1.1.3

- Shader Dan's Curse V2 aggiornato: fiamme e magie illuminano davvero (riflessi, calore, tremolio del fuoco, ombre delle sorgenti di luce), onde dell'acqua piu' alte, foschia che parte piu' lontano dal personaggio.
- I pacchetti delle release e gli script di compilazione includono sempre l'ultima versione di Dan's Curse V2 (prima era fissata a un commit).

---

# MediEvilRecomp ITA v1.1.2

- Telecamera libera (levetta destra): niente piu' tremolio. Quando il gioco non ricalcolava la telecamera in un frame, la rotazione veniva applicata due volte; vicino a muri e recinzioni la telecamera entrava e usciva a ogni frame. Ora riparte sempre dai valori del gioco e si avvicina e si allontana dagli ostacoli in modo graduale.

---

# MediEvilRecomp ITA v1.1.1

- Shader **Dan's Curse V2** incluso nei pacchetti: basato su Dan's Curse di flaffymg, con nebbia e illuminazione riscritte (nebbia lineare e lontana, niente figure squadrate al limite di disegno, foschia a terra con banchi in movimento, ombre senza rumore). Si attiva accendendo la mod **Shaders** dal pannello **Mods**. Consigliato: **Draw distance** a 3x in Settings → Display.

---

# MediEvilRecomp ITA v1.1 - basato su MediEvilRecomp 1.0

Port PAL della versione 1.0 di MediEvilRecomp (BlackLabelHQ, commit 6383fe8) con il suo RecompOne (cc18281).

## Dalla 1.0 del progetto originale
- Telecamera con la levetta destra (`patches/qol/`). In PAL la velocita' e' calcolata su 25 fotogrammi al secondo.
- Interpolazione migliorata, acqua per gli shader, shader slang (ShaderMod aggiornata), colore a 24 bit, opzione per il dithering, editor di asset pack.
- Funzioni ricompilate in modalita' `"precise"` al posto delle vecchie patch in virgola mobile.
- Crash all'uscita da un livello e con distanza visiva alta, sfarfallio a inizio dissolvenza.
- Dissolvenze in widescreen (`FadeWidenPatch.cs`): in PAL esclude l'iride, che resta a `IrisFadePatch.cs` con il foro rotondo.
- Nuovo sistema di impostazioni: le impostazioni precedenti non vengono riprese.

## Port PAL
- Immagine tagliata in basso dopo lo skip dei filmati: il runtime 1.0 poteva trovarsi in modo video NTSC e mostrare 226 righe invece di 256. `PalDisplay.cs` rimette il modo PAL prima di ogni PutDispEnv.
- Mod Hub: ramo `master` del repository ITA (prima cercava `main`, da qui l'errore 404).
- Colore a 24 bit e "Better rendering" attivi di default.
- Release automatiche con GitHub Actions (`.github/workflows/release.yml`, `tools/ci/make_disc.py`): Windows, Linux e macOS.
- `tools/pal-port` aggiornato: gestisce le voci `"precise"` e la cartella `wrapers/`.
- Offset rispetto a `$gp` in `wrapers/Game.cs` e `wrapers/Mr.cs` tradotti a mano, tutti verificati come corrispondenze esatte.
- `WaterMaterialPatch.EndLevel` agganciata a mano a `0x800658BC`: in PAL la funzione di fine livello e' diversa nella parte finale, quindi lo strumento la scartava.
- Mantenute tutte le correzioni PAL del fork: callback CD, LibCrypt (Mausoleo), filmati, 256 righe, dialoghi XA, iride.
- Mod Trucchi e Filmati a schermo intero passate al nuovo sistema di impostazioni e attive di default.
- Shader Dan's Curse V2 (GPL-3.0, basato su Dan's Curse di flaffymg, con nebbia e illuminazione riscritte) incluso nei pacchetti delle release e scaricato dagli script di prima compilazione (Windows e Linux), alla versione fissata, in `shaders/dans-curse-v2-shader`, gia' registrato come installato dal Mod Hub.
- Shader: "Native resolution input" disattivato di default (con l'opzione attiva Dan's Curse diventa pixeloso). Resta attivabile da Shaders, per esempio per gli shader CRT.
- Dan's Curse: nebbia piu' leggera solo nel menu principale (`patches/render/TitleShaderPatch.cs`), dove la scena e' inquadrata da lontano e finiva quasi tutta nella nebbia.
- Mod **Filmati a schermo intero** 1.2: saltando molto in fretta il filmato iniziale lo schermo restava tagliato alle righe del video (dialogo del gargoyle e gioco). A fine filmato la mod rimanda la DISPENV originale. Log con `MEDIEVIL_DEBUG`.
- Iride con uno shader attivo: l'anello sfumato (sottrattivo) diventa nero opaco, perche' gli shader non sanno rappresentarlo e mostravano luci e nebbia sopra il nero.
- Mod Hub della versione PAL: https://github.com/Leproide/MediEvilRecomp-ITA-ModHub (`DefaultModHub` in `Program.cs`, sostituibile con `modhub.repository` e `modhub.branch` in `settings.json`). L'hub originale non e' usato: le sue mod sono per la versione USA.
- Aggiornate le copie di riserva in `BACKUP_Dipendenze`.

Da provare: filmati (saltati e non), mappa e Mausoleo, gargoyle della cripta, dissolvenze in widescreen, distanza visiva al massimo e uscita dai livelli, telecamera, Asylum, Scarecrow Fields, Pumpkin Gorge e Zarok.

Autore del port: https://github.com/Leproide

---

# MediEvilRecomp ITA - script per Linux

## Novità
- Nuovi `linux_initial_build.sh` e `linux_run.sh`: stessi passaggi degli script Windows (sottomoduli, correzione di RecompOne, controllo di .NET 10 e dei file del disco, generazione e compilazione). `./linux_run.sh --no-build` avvia senza ricompilare.
- `.gitattributes`: gli script `.sh` mantengono la fine riga LF anche se il repository viene clonato su Windows.

Autore: https://github.com/Leproide

---

# MediEvilRecomp ITA - Mausoleo e LibCrypt

## Correzioni
- Il livello **Il Mausoleo in Cima alla Collina** non si apriva e il gioco restava sulla schermata con il CD e il punto esclamativo. Il disco PAL è protetto con LibCrypt: dalla mappa il gioco controlla 16 settori del disco e, da quelli con il sottocodice modificato, ricava la chiave che decifra l'inizio dei dati del Mausoleo in `MEDRES.MWD`. Un dump bin/cue non ha il sottocodice, quindi la chiave risultava 0 e i dati restavano cifrati.
- Nuovo `patches/PalLibCrypt.cs`: sui settori protetti fa riportare a GetlocP la posizione del settore precedente, come fa il lettore reale con un sottocodice non valido. L'elenco dei settori di SCES-01494 è incluso; se in `disc` c'è un file `.sbi` o `.lsd` viene usato quello. Agganciato in `config/MediEvil.json`.
- Corretti i commenti di `PalCdCallbacks.cs`: la macchina a stati a 0x80065D20 è il controllo LibCrypt, non la musica XA.
- Dopo l'aggiornamento serve rigenerare il codice: cancella la cartella `generated` e rilancia `windows_initial_build.bat`, una volta completato lancia `windows_run.bat`

Autore: https://github.com/Leproide

---

# MediEvilRecomp ITA - dialoghi XA e filmati a schermo intero

## Correzioni
- Il primo discorso del gargoyle nella cripta si chiudeva da solo se il filmato iniziale veniva saltato subito: un marcatore di fine traccia della musica veniva preso per la fine del discorso. Nuovo `patches/PalXaSpeech.cs`, agganciato in `config/MediEvil.json`.
- Mod **Filmati a schermo intero** 1.1: saltando un filmato dopo qualche secondo, il gioco poteva restare con l'immagine tagliata. Ora lo stato del lettore di avvio e quello del gioco sono separati.
- Dopo l'aggiornamento serve rigenerare il codice: cancella la cartella `generated` e rilancia `windows_initial_build.bat`, una volta completato lancia `windows_run.bat`

Autore: https://github.com/Leproide

---

# MediEvilRecomp ITA - filmati a schermo intero

## Novità
- Nuova mod opzionale **Filmati a schermo intero** (`mods/FilmatiSchermoIntero/`): durante i filmati mostra solo l'area del video (320×176) e la adatta alla finestra senza deformarla. Si attiva dal pannello delle mod e non richiede di ricompilare.

Autore: https://github.com/Leproide

---

# MediEvilRecomp ITA - filmati e dissolvenze

## Correzioni
- Filmati saltati dopo un secondo o due (issue #5 del progetto originale): il lettore dei filmati considerava finito il filmato dopo circa 4 milioni di tentativi a vuoto, che su PC durano una frazione di secondo. Ora l'attesa è misurata in tempo reale (2 secondi). Nuovo `patches/PalMovies.cs`.
- Dissolvenza a iride che in widescreen non copriva gli angoli (issue #7 del progetto originale): il bordo esterno della dissolvenza viene allargato in proporzione al margine widescreen. Nuovo `patches/widescreen/IrisFadePatch.cs`.
- Filmati spostati in basso: RecompOne usava 240 righe anche in PAL quando il gioco non indica l'altezza dello schermo. Ora sono 256. Nuovo `patches/PalDisplay.cs`.
- Dopo l'aggiornamento serve rigenerare il codice: cancella la cartella `generated` e rilancia `windows_initial_build.bat`.

Autore: https://github.com/Leproide

---

# MediEvilRecomp ITA - correzione mappa dopo la Sala degli Eroi

## Correzioni
- La mappa dopo la Sala degli Eroi andava a circa 1 fotogramma al secondo e i comandi non rispondevano; scegliendo il livello il gioco poi si bloccava.
- Causa 1: lo `StUnSetRing` HLE di RecompOne non azzera i callback del CD come quello originale, quindi dopo un filmato restava attivo `StCdInterrupt`.
- Causa 2: il gestore della musica XA del gioco scrive i callback direttamente nelle variabili di libcd (`0x800EB660`/`0x800EB664`) e RecompOne non le vedeva.
- Nuovo `patches/PalCdCallbacks.cs`, agganciato in `config/MediEvil.json`: azzera i callback dopo `StUnSetRing` e tiene allineate le variabili del gioco con quelle di RecompOne.
- Dopo l'aggiornamento serve rigenerare il codice: cancella la cartella `generated` e rilancia `windows_initial_build.bat`.

Autore: https://github.com/Leproide

---

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
- Nuova mod **Trucchi (MediEvil ITA)** (`mods/Trucchi/`): attiva la Modalità Trucchi o la Modalità Trucchi Estesa dal pannello delle mod.

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
