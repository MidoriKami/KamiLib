using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.Utilities;
using Action = System.Action;

namespace KamiLib.InfoBoxSystem;

public abstract class DrawList<T>
{
    protected T DrawListOwner { get; init; } = default!;
    protected List<Action> DrawActions { get; } = new();

    protected void DrawListContents()
    {
        foreach (var action in DrawActions)
        {
            action();
        }
    }

    public T AddDummy(float value)
    {
        DrawActions.Add(() => ImGuiHelpers.ScaledDummy(value));

        return DrawListOwner;
    }
    
    public T AddIndent(int tab)
    {
        DrawActions.Add(() => ImGui.Indent(15.0f * tab));

        return DrawListOwner;
    }
    
    public T AddIcon(uint iconID, Vector2 size, Vector4 color)
    {
        var icon = IconCache.Instance.GetIcon(iconID);

        if (icon != null)
        {
            DrawActions.Add(() =>
            {
                ImGui.Image(icon.ImGuiHandle, size, Vector2.Zero, Vector2.One, color);
            });
        }

        return DrawListOwner;
    }
    
    public T AddIcon(uint iconID, Vector2 size, float transparency)
    {
        var icon = IconCache.Instance.GetIcon(iconID);

        if (icon != null)
        {
            DrawActions.Add(() =>
            {
                ImGui.Image(icon.ImGuiHandle, size, Vector2.Zero, Vector2.One, Vector4.One with {W = transparency});
            });
        }

        return DrawListOwner;
    }

    public T AddString(string message, Vector4? color = null)
    {
        if (color == null)
        {
            DrawActions.Add(() => ImGui.Text(message));
        }
        else
        {
            DrawActions.Add(() => ImGui.TextColored(color.Value, message));
        }

        return DrawListOwner;
    }

    public T AddStringCentered(string message, float? availableArea = null, Vector4? color = null)
    {
        if (color == null)
        {
            DrawActions.Add(() =>
            {
                var area = availableArea / 2.0f ?? ImGui.GetContentRegionAvail().X / 2.0f;
                
                ImGui.SetCursorPos(ImGui.GetCursorPos() with {X = ImGui.GetCursorPos().X + area - ImGui.CalcTextSize(message).X / 2.0f});
                ImGui.Text(message);
            });
        }
        else
        {
            DrawActions.Add(() =>
            {
                var area = availableArea / 2.0f ?? ImGui.GetContentRegionAvail().X / 2.0f;
                
                ImGui.SetCursorPos(ImGui.GetCursorPos() with {X = ImGui.GetCursorPos().X + area - ImGui.CalcTextSize(message).X / 2.0f});
                ImGui.TextColored(color.Value, message);
            });
        }

        return DrawListOwner;
    }

    public T Indent(int indent)
    {
        DrawActions.Add( () => ImGui.Indent(indent) );

        return DrawListOwner;
    }

    public T UnIndent(int indent)
    {
        DrawActions.Add( () => ImGui.Unindent(indent) );

        return DrawListOwner;
    }

    public T AddConfigCheckbox(string label, Setting<bool> setting, string? helpText = null, string? additionalID = null)
    {
        DrawActions.Add(() =>
        {
            if (additionalID != null) ImGui.PushID(additionalID);

            var cursorPosition = ImGui.GetCursorPos();

            if (ImGui.Checkbox($"##{label}", ref setting.Value))
            {
                KamiCommon.SaveConfiguration();
            }

            var spacing = ImGui.GetStyle().ItemSpacing;
            cursorPosition += spacing;
            ImGui.SetCursorPos(cursorPosition with { X = cursorPosition.X + 27.0f * ImGuiHelpers.GlobalScale });

            ImGui.TextUnformatted(label);

            if (helpText != null)
            {
                ImGuiComponents.HelpMarker(helpText);
            }

            if (additionalID != null) ImGui.PopID();
        });

        return DrawListOwner;
    }

    public T AddConfigCombo<TU>(IEnumerable<TU> values, Setting<TU> setting, Func<TU, string> localizeFunction, string label = "", float width = 0.0f) where TU : struct
    {
        DrawActions.Add(() =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }
            else
            {
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            }

            if (ImGui.BeginCombo(label, localizeFunction(setting.Value)))
            {
                foreach (var value in values)
                {
                    if (ImGui.Selectable(localizeFunction(value), setting.Value.Equals(value)))
                    {
                        setting.Value = value;
                        KamiCommon.SaveConfiguration();
                    }
                }

                ImGui.EndCombo();
            }
        });

        return DrawListOwner;
    }

    public T AddConfigColor(string label, Setting<Vector4> setting)
    {
        DrawActions.Add(() =>
        {
            if (ImGui.ColorEdit4(label, ref setting.Value, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreviewHalf))
            {
                KamiCommon.SaveConfiguration();
            }
        });

        return DrawListOwner;
    }
    
    public T AddConfigColor(string label, string defaultLabel, Setting<Vector4> setting, Vector4 defaultValue)
    {
        DrawActions.Add(() =>
        {
            ImGui.PushID(label);
            
            if (ImGui.ColorEdit4($"##{label}", ref setting.Value, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreviewHalf))
            {
                KamiCommon.SaveConfiguration();
            }
            
            ImGui.SameLine();
            ImGui.BeginDisabled(setting == defaultValue);
            if (ImGui.Button(defaultLabel))
            {
                setting.Value = defaultValue;
                KamiCommon.SaveConfiguration();
            }
            ImGui.EndDisabled();
            
            ImGui.SameLine();
            ImGui.TextUnformatted(label);
            
            ImGui.PopID();
        });

        return DrawListOwner;
    }

    public T AddDragFloat(string label, Setting<float> setting, float minValue, float maxValue, float width = 0.0f, int precision = 2)
    {
        DrawActions.Add(() =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width);
            }
            else
            {
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            }

            ImGui.DragFloat(label, ref setting.Value, 0.01f * maxValue, minValue, maxValue, $"%.{precision}f");
            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                KamiCommon.SaveConfiguration();
            }
        });

        return DrawListOwner;
    }

    public T AddAction(Action action)
    {
        DrawActions.Add(action);

        return DrawListOwner;
    }
    
    public T AddSliderInt(string label, Setting<int> setting, int minValue, int maxValue, float width = 200.0f)
    {
        DrawActions.Add(() =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }
            else
            {
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            }

            ImGui.SliderInt(label, ref setting.Value, minValue, maxValue);
            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                KamiCommon.SaveConfiguration();
            }
        });

        return DrawListOwner;
    }
    
    public T AddConfigRadio<TU>(string label, Setting<TU> setting, TU buttonValue, string? helpText = null ) where TU : struct
    {
        DrawActions.Add(() =>
        {
            var value = Convert.ToInt32(setting.Value);

            if (ImGui.RadioButton(label, ref value, Convert.ToInt32(buttonValue)))
            {
                setting.Value = (TU)Enum.ToObject(typeof(TU), value);
                KamiCommon.SaveConfiguration();
            }

            if (helpText != null)
            {
                ImGuiComponents.HelpMarker(helpText);
            }
        });

        return DrawListOwner;
    }

    public T AddConfigString(Setting<string> settingsCustomName, float width = 0.0f)
    {
        DrawActions.Add(() =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }

            ImGui.InputText("", ref settingsCustomName.Value, 24);

            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                KamiCommon.SaveConfiguration();
            }
        });

        return DrawListOwner;
    }

    public T AddConfigVector2(Setting<Vector2> setting, float width = 200.0f)
    {
        DrawActions.Add(() =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }
            else
            {
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            }

            ImGui.InputFloat2("", ref setting.Value);
            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                KamiCommon.SaveConfiguration();
            }
        });

        return DrawListOwner;
    }

    public T AddInputInt(string label, Setting<int> settingsPriority, int min, int max, int step = 1, int stepFast = 1, float width = 77.0f)
    {
        DrawActions.Add(() =>
        {
            ImGui.SetNextItemWidth(width);
            ImGui.InputInt(label, ref settingsPriority.Value, step, stepFast);
            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                settingsPriority.Value = Math.Clamp(settingsPriority.Value, min, max);
                KamiCommon.SaveConfiguration();
            }
        });

        return DrawListOwner;
    }

    public T AddHelpMarker(string helpText)
    {
        DrawActions.Add(() =>
        {
            ImGuiComponents.HelpMarker(helpText);
        });

        return DrawListOwner;
    }

    public T BeginDisabled(bool shouldDisable)
    {
        DrawActions.Add(() =>
        {
            ImGui.BeginDisabled(shouldDisable);
        });

        return DrawListOwner;
    }
    
    public T EndDisabled()
    {
        DrawActions.Add(ImGui.EndDisabled);

        return DrawListOwner;
    }

    public T AddSeparator()
    {
        DrawActions.Add(() =>
        {
            var startPosition = ImGui.GetCursorScreenPos();
            var stopPosition = startPosition with { X = startPosition.X + InfoBox.Instance.InnerWidth };
            var color = ImGui.GetColorU32(Colors.White);
            
            ImGui.GetWindowDrawList().AddLine(startPosition, stopPosition, color);
        });

        return DrawListOwner;
    }

    public T SameLine(float width = 0)
    {
        if (width == 0)
        {
            DrawActions.Add(ImGui.SameLine);
        }
        else
        {
            DrawActions.Add(() => ImGui.SameLine(width));
        }

        return DrawListOwner;
    }

    public T AddDisabledButton(string label, Action action, bool disable, string? hoverTooltip = null, float? buttonSize = null)
    {
        if (buttonSize is not null)
        {
            AddDisabledButtonWithSize(label, action, disable, buttonSize.Value, hoverTooltip);
        }
        else
        {
            AddDisabledButtonWithoutSize(label, action, disable, hoverTooltip);
        }

        return DrawListOwner;
    }


    private void AddDisabledButtonWithSize(string label, Action action, bool disable, float buttonSize, string? hoverTooltip = null)
    {
        DrawActions.Add(() =>
        {
            if(disable) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
            if (ImGui.Button(label, new Vector2(buttonSize, 23.0f * ImGuiHelpers.GlobalScale)) && !disable)
            {
                action.Invoke();
            }
            if(disable) ImGui.PopStyleVar();

            if (hoverTooltip is not null && ImGui.IsItemHovered() && disable)
            {
                ImGui.SetTooltip(hoverTooltip);
            }
        });
    }

    private void AddDisabledButtonWithoutSize(string label, Action action, bool disable, string? hoverTooltip = null)
    {
        DrawActions.Add(() =>
        {
            if(disable) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
            if (ImGui.Button(label) && !disable)
            {
                action.Invoke();
            }                
            if(disable) ImGui.PopStyleVar();
                
            if (hoverTooltip is not null && ImGui.IsItemHovered() && disable)
            {
                ImGui.SetTooltip(hoverTooltip);
            }
        });
    }
    
    public T AddButton(string label, Action action, Vector2? buttonSize = null)
    {
        if (buttonSize is not null)
        {
            DrawActions.Add(() =>
            {
                if (ImGui.Button(label, buttonSize.Value))
                {
                    action.Invoke();
                }
            });
        }
        else
        {
            DrawActions.Add(() =>
            {
                if (ImGui.Button(label))
                {
                    action.Invoke();
                }
            });
        }

        return DrawListOwner;
    }
}