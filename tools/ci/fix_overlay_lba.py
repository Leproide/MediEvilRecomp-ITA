#!/usr/bin/env python3
# SPDX-License-Identifier: GPL-3.0-or-later
#
# MediEvilRecomp ITA - posizioni reali degli overlay per la compilazione automatica (CI)
# Copyright (C) 2026
# Author: https://github.com/Leproide
#
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <https://www.gnu.org/licenses/>.

"""Il runtime riconosce quale overlay il gioco sta caricando dal settore del disco
letto (LbaStart nel codice generato). L'immagine minima della CI (make_disc.py) ha
i file in posizioni diverse dal disco vero, quindi il recompiler scriverebbe settori
sbagliati e gli overlay non verrebbero mai attivati (crash all'avvio di un livello).
Questo script rimette in generated/*.cs i settori del disco PAL italiano
(SCES-01494, "MediEvil (Italy) (Track 1).bin"), presi da una compilazione locale.

Uso: fix_overlay_lba.py <cartella generated>
"""

import re
import sys
from pathlib import Path

# overlay -> primo settore sul disco reale (MEDIEVIL.EXE: settore del file + 1 per "skip": 2048)
REAL_LBA = {
    "game": 156163,
    "ac": 205212, "ag": 205238, "cc": 205257, "ch": 205277, "cr": 205290,
    "credits": 205294, "dc": 205300, "ee": 205328, "eh": 205358, "gg": 205365,
    "gs": 205372, "gy1": 205389, "gy2": 205399, "hh": 205420, "hr": 205445,
    "ia": 205470, "la": 205481, "landmap": 205497, "pd": 205507, "pg": 205519,
    "ps": 205538, "sf": 205564, "sv": 205591, "td": 205618, "tl": 205645,
    "zl": 205663,
}

PATTERN = re.compile(r"(public int LbaStart => )(-?\d+)(;)")


def main() -> int:
    if len(sys.argv) != 2:
        print(__doc__)
        return 2
    gen = Path(sys.argv[1])
    errors = 0
    for name, lba in REAL_LBA.items():
        path = gen / f"{name}.cs"
        if not path.is_file():
            print(f"ERRORE: manca {path}")
            errors += 1
            continue
        text = path.read_text(encoding="utf-8")
        found = PATTERN.findall(text)
        if len(found) != 1:
            print(f"ERRORE: {path}: trovate {len(found)} righe LbaStart invece di 1")
            errors += 1
            continue
        old = int(found[0][1])
        path.write_text(PATTERN.sub(lambda m: f"{m.group(1)}{lba}{m.group(3)}", text), encoding="utf-8")
        print(f"{name}: {old} -> {lba}")
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main())
