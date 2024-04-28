using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.Window;
using NetStone;

namespace KamiLib.Configuration;

public class ConfigurationManagerWindow : Window.Window, IDisposable {
    private readonly DalamudPluginInterface pluginInterface;
    private readonly ITextureProvider textureProvider;
    private readonly WindowManager windowManager;
    private readonly INotificationManager notificationManager;

    private readonly Dictionary<ulong, IDalamudTextureWrap?> characterProfiles = [];

    private readonly List<CharacterConfiguration> characters;

    private CharacterConfiguration? selectedSourceCharacter;
    private List<CharacterConfiguration>? destinationCharacters;

    private readonly CancellationTokenSource cancellationTokenSource = new();

    public ConfigurationManagerWindow(DalamudPluginInterface pluginInterface, ITextureProvider textureProvider, INotificationManager notificationManager, WindowManager windowManager) : base("Configuration Manager", new Vector2(750.0f, 500.0f)) {
        this.pluginInterface = pluginInterface;
        this.textureProvider = textureProvider;
        this.windowManager = windowManager;
        this.notificationManager = notificationManager;

        AdditionalInfoTooltip = "Allows you to easily copy plugin configuration from one character to multiple characters";
        
        TitleBarButtons.Add(new TitleBarButton {
            Icon = FontAwesomeIcon.Recycle,
            ShowTooltip = () => ImGui.SetTooltip("Reload Profile Pictures"),
            IconOffset = new Vector2(1.5f, 2.0f),
            Click = _ => {
                if (characters != null) {
                    foreach (var character in characters) {
                        character.PurgeProfilePicture = true;
                    }
                    characterProfiles.Clear();
                    Task.Run(LoadCharacterPortraits, cancellationTokenSource.Token);
                }
            },
            Priority = 2,
        });

        characters = pluginInterface.GetAllCharacterConfigurations().ToList();

        Task.Run(LoadCharacterPortraits, cancellationTokenSource.Token);
    }
    
    public void Dispose() {
        cancellationTokenSource.Cancel();
    }

    public override void Draw() {
        base.Draw();

        var leftSize = new Vector2(ImGui.GetContentRegionMax().X * 0.35f - ImGui.GetStyle().ItemInnerSpacing.X, ImGui.GetContentRegionAvail().Y);
        var centerSize = new Vector2(ImGui.GetContentRegionAvail().X * 0.30f - ImGui.GetStyle().ItemInnerSpacing.X * 2.0f, ImGui.GetContentRegionAvail().Y);
        var rightSize = new Vector2(ImGui.GetContentRegionAvail().X * 0.35f - ImGui.GetStyle().ItemInnerSpacing.X, ImGui.GetContentRegionAvail().Y);

        DrawSourceSelect(leftSize);
        ImGui.SameLine();
        DrawCenterPane(centerSize);
        ImGui.SameLine();
        DrawTargetSelect(rightSize);
    }

    
    
    private void DrawSourceSelect(Vector2 leftSize) {
        using var leftPane = ImRaii.Child("leftPane", leftSize);
        if (!leftPane) return;
        
        if (selectedSourceCharacter is null) {
            using var iconFont = ImRaii.PushFont(UiBuilder.IconFont);
            if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##sourceCharacter", ImGui.GetContentRegionAvail())) {
                ShowCharacterSelectWindow();
            }
        }
        else {
            DrawSelectedCharacter(selectedSourceCharacter);
        }
    }
    
    private void DrawCenterPane(Vector2 centerSize) {
        using var centerPane = ImRaii.Child("centerPane", centerSize);
        if (!centerPane) return;
        
        ImGuiHelpers.ScaledDummy(150.0f);
        using var indent = ImRaii.PushIndent(10.0f);
        
        using (var _ = ImRaii.PushColor(ImGuiCol.Text, selectedSourceCharacter is null ? KnownColor.Orange.Vector() : KnownColor.LimeGreen.Vector())) {
            ImGui.BulletText("Select Source Character");
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);

        using (var _ = ImRaii.PushColor(ImGuiCol.Text, destinationCharacters is null || destinationCharacters.Count == 0 ? KnownColor.Orange.Vector() : KnownColor.LimeGreen.Vector())) {
            ImGui.BulletText("Select Target Characters");
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        
        using (var _ = ImRaii.PushColor(ImGuiCol.Text, !(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl) ? KnownColor.Orange.Vector() : KnownColor.LimeGreen.Vector())) {
            ImGui.BulletText("Hold Shift + Control");
        }

        if (selectedSourceCharacter is not null && destinationCharacters is not null && destinationCharacters.Count > 0) {
            ImGui.SetCursorPos(new Vector2(0.0f, ImGui.GetContentRegionMax().Y - 30.0f * ImGuiHelpers.GlobalScale));
            var keyComboHeld = ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl;

            using (var _ = ImRaii.PushStyle(ImGuiStyleVar.Alpha, 0.5f, !keyComboHeld)) {
                if (ImGui.Button("Copy Configurations", new Vector2(ImGui.GetContentRegionAvail().X, 30.0f * ImGuiHelpers.GlobalScale)) && keyComboHeld) {
                    CopySelectedConfigurations();
                }
            }
            
            if (ImGui.IsItemHovered()) {
                ImGui.SetTooltip("Press to copy configuration from source character to all target characters\nThis can not be un-done");
            }
        }
    }

    private void DrawTargetSelect(Vector2 rightSize) {
        using var rightPane = ImRaii.Child("rightPane", rightSize);
        if (!rightPane) return;
        
        if (destinationCharacters is null || destinationCharacters.Count == 0) {
            using var iconFont = ImRaii.PushFont(UiBuilder.IconFont);
            if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##destinationCharacter", ImGui.GetContentRegionAvail())) {
                ShowCharacterMultiSelectWindow();
            }
        }
        else {
            using (var charactersFrame = ImRaii.Child("charactersFrame", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 35.0f * ImGuiHelpers.GlobalScale))) {
                if (charactersFrame) {
                    foreach (var selectedTarget in destinationCharacters) {
                        DrawCharacter(selectedTarget);
                    }
                }
            }
                    
            ImGui.SetCursorPosY(ImGui.GetContentRegionMax().Y - 30.0f * ImGuiHelpers.GlobalScale);
            if (ImGui.Button("Change##target", new Vector2(ImGui.GetContentRegionAvail().X, 30.0f * ImGuiHelpers.GlobalScale))) {
                ShowCharacterMultiSelectWindow();
            }
        }
    }

    private void ShowCharacterSelectWindow() {
        windowManager.AddWindow(new SelectionWindow<CharacterConfiguration>(windowManager) {
            DrawSelection = DrawCharacter,
            SelectionCallback = selectedCharacter => {
                selectedSourceCharacter = selectedCharacter;
                if (selectedSourceCharacter != null && destinationCharacters is not null && destinationCharacters.Contains(selectedSourceCharacter)) {
                    destinationCharacters.Remove(selectedSourceCharacter);
                }
            },
            SelectionHeight = 75.0f,
            SelectionOptions = characters.ToList(),
            FilterResults = (character, searchTerm) => character.CharacterName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase),
        });
    }

    private void ShowCharacterMultiSelectWindow() {
        windowManager.AddWindow(new MultiSelectionWindow<CharacterConfiguration>(windowManager) {
            DrawSelection = DrawCharacter,
            SelectionCallback = selectedCharacters => {
                destinationCharacters = selectedCharacters;

                if (selectedSourceCharacter != null && destinationCharacters is not null && destinationCharacters.Contains(selectedSourceCharacter)) {
                    destinationCharacters.Remove(selectedSourceCharacter);
                }
            },
            SelectionHeight = 75.0f,
            SelectionOptions = characters.ToList(),
        });
    }
    
    private async void LoadCharacterPortraits() {
        try {
            var lodestoneClient = await LodestoneClient.GetClientAsync();
            var httpClient = new HttpClient();

            foreach (var character in characters) {
                _ = Task.Run(() => TryLoadCharacterProfilePicture(lodestoneClient, httpClient, character));
            }
        }
        catch (Exception e) {
            throw new Exception("Exception retrieving character portraits.", e);
        }
    }

    private async void TryLoadCharacterProfilePicture(LodestoneClient lodestoneClient, HttpClient httpClient, CharacterConfiguration characterConfiguration) {
        if (characterConfiguration.ContentId is 0) return;

        var texture = await NetStoneHelpers.TryGetProfilePicture(httpClient, lodestoneClient, pluginInterface, textureProvider, characterConfiguration);
        characterProfiles.Add(characterConfiguration.ContentId, texture);
    }

    private void DrawSelectedCharacter(CharacterConfiguration character) {
        if (characterProfiles.TryGetValue(character.ContentId, out var texture) && texture is not null) {
            var sizeRatio = ImGui.GetContentRegionAvail().X / texture.Width;
            
            ImGui.Image(texture.ImGuiHandle, texture.Size * sizeRatio);
        }
        else {
            if (textureProvider.GetIcon(60042) is { ImGuiHandle: var handle } unknownTexture) {
                var sizeRatio = ImGui.GetContentRegionAvail().X / unknownTexture.Width;
                ImGui.Image(handle, ImGuiHelpers.ScaledVector2(75.0f, 75.0f) * sizeRatio);
            }
        }
        
        ImGuiHelpers.ScaledDummy(5.0f);
        PrintCharacterInfo(character);
            
        ImGui.SetCursorPosY(ImGui.GetContentRegionMax().Y - 30.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Change##source", new Vector2(ImGui.GetContentRegionAvail().X, 30.0f * ImGuiHelpers.GlobalScale))) {
            ShowCharacterSelectWindow();
        }
    }

    private void DrawCharacter(CharacterConfiguration character) {
        using var id = ImRaii.PushId(character.ContentId.ToString());

        using (var portrait = ImRaii.Child("portrait", ImGuiHelpers.ScaledVector2(75.0f, 75.0f), false, ImGuiWindowFlags.NoInputs)) {
            if (portrait) {
                if (characterProfiles.TryGetValue(character.ContentId, out var texture) && texture is not null) {
                    ImGui.Image(texture.ImGuiHandle, new Vector2(75.0f, 75.0f), new Vector2(0.25f, 0.10f), new Vector2(0.75f, 0.47f));
                }
                else {
                    ImGui.Image(textureProvider.GetIcon(60042)?.ImGuiHandle ?? IntPtr.Zero, ImGuiHelpers.ScaledVector2(75.0f, 75.0f));
                }
            }
        }

        ImGui.SameLine();

        using (var info = ImRaii.Child("info", new Vector2(ImGui.GetContentRegionAvail().X, 75.0f * ImGuiHelpers.GlobalScale), false, ImGuiWindowFlags.NoInputs)) {
            if (info) {
                ImGuiHelpers.ScaledDummy(5.0f);
                PrintCharacterInfo(character);
            }
        }
    }
    
    private static void PrintCharacterInfo(CharacterConfiguration character) {
        ImGui.TextUnformatted(character.CharacterName);
        ImGui.TextUnformatted(character.CharacterWorld);

        using (ImRaii.PushColor(ImGuiCol.Text, Vector4.One * 0.75f)) {
            ImGui.TextUnformatted(character.ContentId.ToString());
        }
    }
    
    private void CopySelectedConfigurations() {
        if (selectedSourceCharacter is null) {
            notificationManager.AddNotification(new Notification {
                Type = NotificationType.Error,
                Content = "No Source Character is Selected",
            });
            return;
        }

        if (destinationCharacters is null || destinationCharacters.Count == 0) {
            notificationManager.AddNotification(new Notification {
                Type = NotificationType.Error,
                Content = "No Destination Characters are Selected",
            });
            return;
        }
        
        foreach (var file in pluginInterface.GetCharacterDirectoryInfo(selectedSourceCharacter.ContentId).GetFiles()) {
            if (file is { Exists: true, Name: var fileName } && fileName.Contains("config.json", StringComparison.OrdinalIgnoreCase) && !fileName.Contains("System", StringComparison.OrdinalIgnoreCase)) {
                foreach (var targetCharacter in destinationCharacters) {
                    file.CopyTo(pluginInterface.GetCharacterFileInfo(targetCharacter.ContentId, fileName).FullName, true);
                }
            }
        }

        notificationManager.AddNotification(new Notification {
            Type = NotificationType.Success,
            Content = "Configurations successfully copied"
        });
        
        Close();
    }
}