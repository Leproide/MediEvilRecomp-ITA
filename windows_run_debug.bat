@echo off
REM ==================================================================================================
REM MediEvilRecomp ITA - avvio con log di debug
REM Copyright (C) 2026
REM Author: https://github.com/Leproide
REM SPDX-License-Identifier: GPL-3.0-or-later
REM
REM This program is free software: you can redistribute it and/or modify it under the terms of the
REM GNU General Public License as published by the Free Software Foundation, either version 3 of the
REM License, or (at your option) any later version. This program is distributed WITHOUT ANY WARRANTY.
REM See <https://www.gnu.org/licenses/>.
REM
REM Avvia windows_run.bat con MEDIEVIL_DEBUG=1 (righe di debug di patch e mod nel log) e salva
REM tutto l'output anche in debug.txt, mostrandolo nella console.
REM ==================================================================================================
title MediEvilRecomp ITA (debug)
cd /d "%~dp0"

set MEDIEVIL_DEBUG=1
powershell -NoProfile -ExecutionPolicy Bypass -Command "& cmd /c windows_run.bat 2>&1 | Tee-Object -FilePath debug.txt"
