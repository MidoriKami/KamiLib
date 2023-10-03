using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class BoolDescriptionConfigAttribute : BoolConfigAttribute
{
    private readonly string? descriptionLocKey;
    public string Description => TryGetLocalizedString(descriptionLocKey);

    public BoolDescriptionConfigAttribute(string label, string description) : base(label, null)
    {
        descriptionLocKey = description;
    }

    protected override void Draw(object obj, MemberInfo field, Action? saveAction = null)
    {
        ImGui.Text(Description);

        base.Draw(obj, field, saveAction);
    }
}