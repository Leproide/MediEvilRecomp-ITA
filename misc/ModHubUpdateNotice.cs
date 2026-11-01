// SPDX-License-Identifier: GPL-3.0-or-later
//
// MediEvilRecomp ITA - avviso all'avvio per gli aggiornamenti del Mod Hub
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

// All'avvio legge l'indice del Mod Hub e confronta il commit installato di ogni mod, pacchetto
// o shader (file .modhub nella sua cartella) con quello dell'indice. Se qualcosa e' rimasto
// indietro mostra un avviso con l'elenco e il pulsante "Aggiorna tutto".
// - Disattivabile: menu Updates -> "Mod Hub updates on startup" (chiave modhub.notify_on_startup).
// - Gli aggiornamenti usano lo stesso installatore del Mod Hub (ModHub.Install).
// - Se lo shader attivo viene aggiornato si ricarica subito, tenendo i parametri cambiati
//   dall'utente; quelli lasciati al valore predefinito prendono il nuovo predefinito.
// Log: righe "[ModHubUpdates]".

using System.Numerics;
using ImGuiNET;
using RecompOne.Runtime.Config;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Hle;
using RecompOne.Runtime.Host.Window;
using RecompOne.Runtime.Hub;

namespace Recompiled;

public static class ModHubUpdateNotice
{
    internal enum Stage { Idle, Checking, Available, Updating, Finished }

    static readonly Option<bool> NotifyOnStartup = Options.Bool("modhub.notify_on_startup", true);

    static volatile Stage _stage = Stage.Idle;
    static List<HubEntry> _outdated = [];
    static bool _dismissed;

    internal static Stage Current => _stage;
    internal static IReadOnlyList<HubEntry> Outdated => _outdated;
    internal static bool ShouldShow => !_dismissed && _stage is Stage.Available or Stage.Updating or Stage.Finished;

    public static void Register()
    {
        PopupManager.Register(new ModHubUpdatePopup());

        MenuRegistry.Menu("menu.updates")
            .Separator()
            .Check("menu.updates.modhub_on_startup",
                () => NotifyOnStartup,
                enabled => NotifyOnStartup.Value = enabled)
            .Item("menu.updates.modhub_check_now", CheckNow)
                .Enabled(() => ModHub.Configured && _stage is Stage.Idle or Stage.Finished);

        Event.AddListener<RuntimeReadyEvent>(_ =>
        {
            if (!NotifyOnStartup)
            {
                Log("startup check disabled in settings");
                return;
            }

            Start();
        });
    }

    static void CheckNow()
    {
        _dismissed = false;
        Start();
    }

    static void Start()
    {
        if (!ModHub.Configured || _stage is Stage.Checking or Stage.Updating) return;
        _stage = Stage.Checking;
        Log("checking the mod hub for updates...");
        ModHub.Refresh(true);
    }

    // chiamato ad ogni frame dal popup (thread dell'interfaccia), anche quando e' chiuso
    internal static void Tick()
    {
        ModHub.Update();

        switch (_stage)
        {
            case Stage.Checking when !ModHub.Loading:
                ModHub.Scan();
                _outdated = ModHub.Entries.Where(e => ModHub.StateOf(e) == HubInstallState.Outdated).ToList();
                if (ModHub.Error != null) Log($"check failed: {ModHub.Error}");
                Log(_outdated.Count == 0
                    ? "everything installed from the mod hub is up to date"
                    : "updates available: " + string.Join(", ", _outdated.Select(e => e.Id)));
                _stage = _outdated.Count > 0 ? Stage.Available : Stage.Idle;
                break;

            case Stage.Updating when _outdated.All(e => ModHub.JobOf(e) is null or { Error: not null }):
                _stage = Stage.Finished;
                foreach (var e in _outdated)
                    Log(ModHub.JobOf(e)?.Error is { } error ? $"{e.Id}: {error}" : $"{e.Id}: updated");
                ReloadShader();
                break;
        }
    }

    internal static void UpdateAll()
    {
        _stage = Stage.Updating;
        foreach (var entry in _outdated)
        {
            // "Riprova": si rifanno solo quelli non ancora aggiornati
            if (ModHub.StateOf(entry) == HubInstallState.Installed && !Failed(entry)) continue;
            ModHub.DismissError(entry);
            ModHub.Install(entry);
        }
    }

    internal static void Dismiss() => _dismissed = true;

    internal static bool Failed(HubEntry entry) => ModHub.JobOf(entry)?.Error != null;

    // ricarica lo shader in uso se e' tra quelli aggiornati
    static void ReloadShader()
    {
        var path = PostFx.Path;
        if (path == null) return;

        foreach (var entry in _outdated)
        {
            if (entry.Type != ModHub.ShaderType || Failed(entry)) continue;
            var folder = Path.GetFullPath(Path.Combine(ModHub.DirectoryFor(entry.Type), entry.Id)) + Path.DirectorySeparatorChar;
            if (!path.StartsWith(folder, StringComparison.OrdinalIgnoreCase)) continue;

            var changed = PostFx.Parameters
                .Where(p => p.Value != p.Default)
                .ToDictionary(p => p.Name, p => p.Value);
            if (!File.Exists(path) || !PostFx.Load(path))
            {
                Log($"{entry.Id}: cannot reload the shader: {PostFx.Error}");
                return;
            }

            PostFx.SetParams(changed);
            Log($"{entry.Id}: shader reloaded, {changed.Count} changed parameters kept");
            return;
        }
    }

    internal static string TypeLabel(HubEntry entry)
    {
        return ModHub.FindType(entry.Type) is { } type ? Localization.T(type.LabelKey) : entry.Type;
    }

    static void Log(string message) => Console.WriteLine($"[ModHubUpdates] {message}");
}

public sealed class ModHubUpdatePopup : Popup
{
    protected override string TitleKey => "modhub_updates.title";
    protected override Vector2 Size => new(520f, 380f);

    protected override void Update()
    {
        ModHubUpdateNotice.Tick();
        if (ModHubUpdateNotice.ShouldShow && !IsOpen) Open();
        else if (!ModHubUpdateNotice.ShouldShow && IsOpen) Close();
    }

    protected override void OnClosed() => ModHubUpdateNotice.Dismiss();

    protected override void DrawContent()
    {
        var stage = ModHubUpdateNotice.Current;
        var entries = ModHubUpdateNotice.Outdated;

        ImGui.TextUnformatted(stage == ModHubUpdateNotice.Stage.Finished
            ? Localization.T("modhub_updates.headline.done")
            : Localization.T("modhub_updates.headline", entries.Count));
        ImGui.Separator();
        ImGui.Spacing();

        float footer = ImGui.GetFrameHeightWithSpacing() * 2f + ImGui.GetStyle().ItemSpacing.Y * 2f;
        ImGui.BeginChild("##hubupdates", new Vector2(0, -footer), ImGuiChildFlags.Borders);
        foreach (var entry in entries) DrawEntry(entry, stage);
        ImGui.EndChild();

        ImGui.Spacing();
        switch (stage)
        {
            case ModHubUpdateNotice.Stage.Available:
                if (Row(0, 2, Localization.T("modhub_updates.update_all"))) ModHubUpdateNotice.UpdateAll();
                if (Row(1, 2, Localization.T("update.not_now"))) Close();
                break;

            case ModHubUpdateNotice.Stage.Updating:
                ImGui.BeginDisabled();
                Row(0, 1, Localization.T("modhub_updates.updating"));
                ImGui.EndDisabled();
                break;

            default:
                if (entries.Any(ModHubUpdateNotice.Failed) && Row(0, 2, Localization.T("update.retry")))
                    ModHubUpdateNotice.UpdateAll();
                if (Row(entries.Any(ModHubUpdateNotice.Failed) ? 1 : 0, entries.Any(ModHubUpdateNotice.Failed) ? 2 : 1,
                        Localization.T("common.close")))
                    Close();
                break;
        }

        // promemoria dell'opzione nel menu
        ImGui.TextDisabled(Localization.T("modhub_updates.hint"));
    }

    static void DrawEntry(HubEntry entry, ModHubUpdateNotice.Stage stage)
    {
        ImGui.TextUnformatted(entry.Name);
        ImGui.SameLine();
        ImGui.TextDisabled($"({ModHubUpdateNotice.TypeLabel(entry)})");

        var job = ModHub.JobOf(entry);
        if (job?.Error is { } error)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0.45f, 0.4f, 1f));
            ImGui.TextWrapped(error);
            ImGui.PopStyleColor();
        }
        else if (job != null)
            ImGui.ProgressBar(job.Progress, new Vector2(-1, 0));
        else if (stage == ModHubUpdateNotice.Stage.Finished)
            ImGui.TextDisabled(Localization.T("modhub_updates.updated"));
        else if (entry.Description.Length > 0)
            ImGui.TextDisabled(entry.Description.Length > 90 ? entry.Description[..90] + "..." : entry.Description);

        ImGui.Spacing();
    }

    static bool Row(int index, int count, string label)
    {
        float spacing = ImGui.GetStyle().ItemSpacing.X;
        float width = (ImGui.GetContentRegionAvail().X - spacing * (count - 1)) / count;
        if (index > 0) ImGui.SameLine();
        return ImGui.Button(label, new Vector2(width, 0));
    }
}
