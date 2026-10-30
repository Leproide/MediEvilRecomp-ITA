// SPDX-License-Identifier: GPL-3.0-or-later
//
// MediEvilRecomp ITA - fine traccia dei dialoghi XA
// Copyright (C) 2026
// Author: https://github.com/Leproide
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.


// Il primo discorso del gargoyle nella cripta si chiudeva subito.
//
// Le tracce audio XA del gioco (musica e voci) finiscono con un settore dati
// "marcatore" (intestazione 0x0160) che indica il canale terminato. Il callback dati
// del gioco (func_8006D7A4) lo confronta con il canale della traccia corrente
// ($gp+0x6C4) e, se coincide, segnala "traccia finita" e il discorso viene chiuso.
//
// Quando un discorso parte mentre suona la musica del livello, il gioco prima sceglie
// il nuovo canale (func_8006D0D8) e solo dopo sposta la lettura sul file della voce
// (ReadS). In quell'intervallo RecompOne continua a consegnare i settori della musica:
// un marcatore di fine con lo stesso numero di canale veniva preso per la fine del
// discorso. Tra la scelta di una nuova traccia e il comando di lettura che la avvia,
// il controllo del marcatore viene saltato. Verificato in gioco: il discorso arriva
// fino in fondo e la musica riparte normalmente.
//
// Saltando molto in fretta il filmato iniziale il marcatore della traccia precedente
// puo' arrivare anche subito DOPO il comando di lettura (settori gia' in coda).
// Per questo, dopo l'avvio di una nuova traccia, i marcatori vengono ignorati per
// GuardMs millisecondi: nessun discorso del gioco dura cosi' poco.
// Con MEDIEVIL_DEBUG impostata, le righe "[PalXaSpeech]" del log mostrano ogni
// scelta di traccia, comando di lettura e marcatore ricevuto.

using System.Diagnostics;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;

namespace Recompiled;

public static class PalXaSpeech
{
    const byte CdlSetloc = 0x02, CdlReadN = 0x06, CdlReadS = 0x1B;
    const uint Header = 0x800F0CCCu;   // copia dell'intestazione del settore dati
    const uint GpChannel = 0x6C4u;     // canale della traccia corrente
    const uint GpNewChannel = 0x6E5u;  // canale appena scelto (byte)
    const uint GpFlags = 0x6BCu;       // bit 2: traccia in loop (musica)
    const long GuardMs = 400;

    static volatile bool _pending;     // nuova traccia scelta, lettura non ancora avviata
    static readonly Stopwatch _sinceStart = Stopwatch.StartNew();
    static long _startMs = -GuardMs;

    static readonly bool DiagOn = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MEDIEVIL_DEBUG"));
    static int _lines;
    static void Diag(string s)
    {
        if (DiagOn && _lines++ < 500) Console.WriteLine($"[PalXaSpeech] {_sinceStart.ElapsedMilliseconds,7} ms  {s}");
    }

    // post su func_8006D0D8: scelta di file/canale della nuova traccia XA
    public static void TrackSelected(CpuContext c, IMemory m)
    {
        _pending = true;
        Diag($"nuova traccia: canale {m.ReadU8(c.GP + GpNewChannel)}");
    }

    // pre su CdControl / CdControlF / CdControlB: un comando di lettura avvia la traccia
    public static void OnCdCommand(CpuContext c, IMemory m)
    {
        byte com = (byte)c.A0;
        if (com == CdlSetloc && c.A1 != 0)
            Diag($"Setloc {m.ReadU8(c.A1):X2}:{m.ReadU8(c.A1 + 1):X2}:{m.ReadU8(c.A1 + 2):X2}");
        if (com != CdlReadS && com != CdlReadN) return;
        if (_pending) _startMs = _sinceStart.ElapsedMilliseconds;
        Diag($"lettura {(com == CdlReadS ? "ReadS" : "ReadN")}{(_pending ? " (avvio nuova traccia)" : "")}");
        _pending = false;
    }

    // pre su func_8006D7A4 (callback dati del lettore XA): false = salta la funzione
    public static bool DataCallback(CpuContext c, IMemory m)
    {
        bool marker = m.ReadU16(Header) == 0x0160;
        if (!marker) return true;

        int channel = (m.ReadU16(Header + 2) >> 10) & 0x1F;
        uint current = m.ReadU32(c.GP + GpChannel);
        long age = _sinceStart.ElapsedMilliseconds - _startMs;
        bool run = !_pending && age >= GuardMs;
        Diag($"marcatore canale {channel}, traccia corrente {current}, loop {(m.ReadU32(c.GP + GpFlags) & 4) != 0}, " +
             $"{age} ms dall'avvio -> {(run ? "accettato" : "ignorato")}");
        return run;
    }
}
