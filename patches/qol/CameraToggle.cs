// SPDX-License-Identifier: GPL-3.0-or-later
//
// MediEvilRecomp ITA - R3 accende e spegne la telecamera libera
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

// Il gioco non usa R3 (pressione della levetta destra). Qui ogni pressione inverte l'opzione
// "Right stick camera" (Misc -> Quality of life), la salva nelle impostazioni e mostra un
// avviso in sovrimpressione. Spegnendola la telecamera torna subito quella del gioco
// (CameraControlPatch rilascia la rotazione al frame dopo).

using RecompOne.Runtime.Events;
using RecompOne.Runtime.Hardware;
using RecompOne.Runtime.Host.Window;

namespace Recompiled;

public static class CameraToggle
{
    const int DebounceFrames = 10;   // pressioni piu' ravvicinate vengono ignorate

    static bool _held;
    static long _lastToggle = long.MinValue;

    public static void Register()
    {
        Event.AddListener<VSyncEvent>(OnVSync);
    }

    static void OnVSync(VSyncEvent e)
    {
        // State e' attivo basso: bit a 0 = tasto premuto
        bool pressed = (Controller.State & Controller.R3) == 0;
        if (pressed && !_held && e.Frame - _lastToggle >= DebounceFrames)
        {
            _lastToggle = e.Frame;
            bool enabled = !QualityOfLifeOptions.CameraControl.Value;
            QualityOfLifeOptions.CameraControl.Value = enabled;
            ToastNotifications.Show("qol.camera_control",
                enabled ? "qol.camera_toggle.on" : "qol.camera_toggle.off", duration: 2f);
            Console.WriteLine($"[CameraToggle] right stick camera {(enabled ? "on" : "off")}");
        }

        _held = pressed;
    }
}
