using System;
using System.Text.Json.Serialization;
using Dalamud.Configuration;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;

namespace KamiLib.Configuration;

public class CharacterConfiguration  : IPluginConfiguration {
    public int Version { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public string CharacterWorld { get; set; } = string.Empty;
    public ulong ContentId { get; set; }
    public string? LodestoneId { get; set; }

    [JsonIgnore] public bool PurgeProfilePicture { get; set; }
    [JsonIgnore] public ISharedImmediateTexture? ProfilePicture { get; set; }

    public unsafe void UpdateCharacterData() {
        if (AgentLobby.Instance()->IsLoggedIn) {
            CharacterName = PlayerState.Instance()->CharacterNameString;
            CharacterWorld = AgentLobby.Instance()->LobbyData.HomeWorldName.ToString();
        }
    }

    public void Draw(ITextureProvider textureProvider) {
        using var id = ImRaii.PushId(ContentId.ToString());

        using (var portrait = ImRaii.Child("portrait", ImGuiHelpers.ScaledVector2(75.0f, 75.0f), false, ImGuiWindowFlags.NoInputs)) {
            if (portrait) {
                if (ProfilePicture is not null) {
                    ImGui.Image(ProfilePicture.GetWrapOrEmpty().ImGuiHandle, new Vector2(75.0f, 75.0f), new Vector2(0.25f, 0.10f), new Vector2(0.75f, 0.47f));
                }
                else {
                    ImGui.Image(textureProvider.GetFromGameIcon(60042).GetWrapOrDefault()?.ImGuiHandle ?? IntPtr.Zero, ImGuiHelpers.ScaledVector2(75.0f, 75.0f));
                }
            }
        }

        ImGui.SameLine();

        using (var info = ImRaii.Child("info", new Vector2(ImGui.GetContentRegionAvail().X, 75.0f * ImGuiHelpers.GlobalScale), false, ImGuiWindowFlags.NoInputs)) {
            if (info) {
                ImGuiHelpers.ScaledDummy(5.0f);
                ImGui.TextUnformatted(CharacterName);
                ImGui.TextUnformatted(CharacterWorld);

                using (ImRaii.PushColor(ImGuiCol.Text, Vector4.One * 0.75f)) {
                    ImGui.TextUnformatted(ContentId.ToString());
                }
            }
        }
    }
}