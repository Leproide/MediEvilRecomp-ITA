// SPDX-License-Identifier: GPL-3.0-or-later
//
// Trucchi (MediEvil ITA) - mod per MediEvilRecomp ITA
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

// Attiva i menu trucchi integrati nel gioco senza inserire le combinazioni.
//
// Come funziona il gioco (versione PAL italiana, MEDIEVIL.EXE):
//   - func_80022C2C gira a ogni frame di gioco e legge il pad; con L2 premuto
//     confronta i tasti con due tabelle:
//       0x800CE0B4: 10 tasti  "DUST TO DUST"  (Giu Su Quadrato Triangolo Triangolo
//                   Cerchio Giu Su Quadrato Triangolo)              -> stato ^= 1
//       0x800CE0DC: 25 tasti  (Modalita' Trucchi Estesa)            -> stato ^= 3
//   - lo stato e' un halfword a 0x800EF0A2: 0 = nessun trucco,
//     1 = Modalita' Trucchi, 3 = Modalita' Trucchi Estesa.
//     Il menu di pausa mostra le voci in base a questo valore.
// La mod scrive lo stato scelto subito dopo func_80022C2C, quindi resta attivo
// anche dopo cambi di livello. Prima di scrivere controlla che le due tabelle
// siano quelle della versione italiana: su altre versioni non tocca nulla.

using ImGuiNET;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

public class TrucchiMod : IMod
{
    // --- indirizzi della versione PAL italiana (SCES-01494) ---------------------------------
    private const uint CheatState = 0x800EF0A2u;      // u16: 0 / 1 / 3
    private const uint CheatCheckFunc = 0x80022C2Cu;  // routine per-frame che legge le combinazioni
    private const uint TableDust = 0x800CE0B4u;       // primo tasto "DUST TO DUST" (Giu)
    private const uint TableExtended = 0x800CE0DCu;   // primo tasto sequenza estesa (Triangolo)
    private const uint PadDown = 0x00080000u;         // codifica del tasto Giu nelle tabelle
    private const uint PadTriangle = 0x00100000u;     // codifica del tasto Triangolo

    private const string SettingKey = "mods.trucchi-ita.modalita";

    // 0 = disattivati, 1 = Modalita' Trucchi, 3 = Modalita' Trucchi Estesa
    private static volatile int _mode;
    // true quando l'utente ha appena spento i trucchi: azzera lo stato una volta sola
    private static volatile bool _clearPending;
    // esito dell'ultimo controllo di versione (mostrato nel pannello)
    private static volatile int _versionCheck; // 0 = non ancora verificato, 1 = ok, -1 = versione diversa

    private static readonly string[] Labels =
    {
        "Disattivati",
        "Modalita' Trucchi",
        "Modalita' Trucchi Estesa"
    };

    public void OnLoad()
    {
        _mode = ModeFromSetting(RecompOne.Runtime.Runtime.View.GetString(SettingKey, "0"));
    }

    public void OnUnload()
    {
        // allo scaricamento della mod rimette lo stato del gioco a zero
        _mode = 0;
        _clearPending = true;
    }

    public void DrawSettings()
    {
        int index = _mode switch { 1 => 1, 3 => 2, _ => 0 };

        ImGui.TextUnformatted("Trucchi del gioco");
        ImGui.SetNextItemWidth(260f);
        if (ImGui.Combo("##trucchi", ref index, Labels, Labels.Length))
        {
            int newMode = index switch { 1 => 1, 2 => 3, _ => 0 };
            if (newMode == 0 && _mode != 0) _clearPending = true;
            _mode = newMode;
            RecompOne.Runtime.Runtime.View.SetString(SettingKey, newMode.ToString());
        }

        ImGui.TextWrapped("Le voci compaiono nel menu di pausa (Start), come con le combinazioni originali.");

        switch (_versionCheck)
        {
            case -1:
                ImGui.TextColored(new System.Numerics.Vector4(1f, 0.4f, 0.4f, 1f),
                    "Versione del gioco non riconosciuta: la mod funziona solo con MediEvil PAL italiano (SCES-01494).");
                break;
            case 0:
                ImGui.TextDisabled("Si attiva quando entri in un livello.");
                break;
        }
    }

    // Eseguito dopo la routine per-frame che gestisce le combinazioni dei trucchi.
    [PostHook("game", Address = CheatCheckFunc)]
    public static void AfterCheatCheck(CpuContext c, IMemory m)
    {
        if (!IsItalianBuild(m))
        {
            _versionCheck = -1;
            return;
        }
        _versionCheck = 1;

        int mode = _mode;
        if (mode != 0)
        {
            if (m.ReadU16(CheatState) != (ushort)mode)
                m.WriteU16(CheatState, (ushort)mode);
        }
        else if (_clearPending)
        {
            m.WriteU16(CheatState, 0);
            _clearPending = false;
        }
    }

    // Le due tabelle delle combinazioni sono dati costanti dell'eseguibile:
    // se sono al loro posto, gli indirizzi della mod sono quelli giusti.
    private static bool IsItalianBuild(IMemory m)
    {
        return m.ReadU32(TableDust) == PadDown && m.ReadU32(TableExtended) == PadTriangle;
    }

    private static int ModeFromSetting(string value)
    {
        return value switch { "1" => 1, "3" => 3, _ => 0 };
    }
}
