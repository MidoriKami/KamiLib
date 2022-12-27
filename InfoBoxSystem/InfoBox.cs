using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace KamiLib.InfoBoxSystem;

public class InfoBox : DrawList<InfoBox>, IDrawable
{
    private static float CurveRadius => 13.0f * ImGuiHelpers.GlobalScale;
    private static float BorderThickness => 2.0f;
    private static int SegmentResolution => 10;
    private static ImDrawListPtr DrawList => ImGui.GetWindowDrawList();
    private static Vector2 RegionAvailable => ImGui.GetContentRegionAvail();
    private static Vector2 StartPosition { get; set; }
    private static Vector2 Size { get; set; }
    private static Vector4 BorderColor { get; } = Colors.White with { W = 0.50f };
    private static Vector4 TitleColor { get; } = Colors.White;
    private static float TotalWidth { get; set; }
    private string Label { get; set; } = "Label Not Set";
    private float WidthPercentage { get; set; }

    public float InnerWidth { get; set; }

    private InfoBox()
    {
        DrawListOwner = this;
    }

    public static readonly InfoBox Instance = new();

    public void Draw()
    {
        ImGuiHelpers.ScaledDummy(5.0f);

        TotalWidth = RegionAvailable.X * WidthPercentage;
        InnerWidth = TotalWidth - CurveRadius * 3.0f;

        var startX = ImGui.GetCursorPos().X + RegionAvailable.X * ( 0.5f - WidthPercentage / 2.0f ) + CurveRadius / 2.0f;
        ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = startX });
        StartPosition = ImGui.GetCursorScreenPos();
        Size = new Vector2(InnerWidth + CurveRadius * 2.0f, 0);
        
        DrawContents();
        
        var calculatedHeight = ImGui.GetItemRectMax().Y - ImGui.GetItemRectMin().Y + CurveRadius * 2.0f;
        Size = new Vector2(InnerWidth + CurveRadius * 2.0f, calculatedHeight);
        
        DrawCorners();
        DrawBorders();
        
        ImGuiHelpers.ScaledDummy(10.0f);
    }

    private void DrawContents()
    {
        var topLeftCurveCenter = new Vector2(StartPosition.X + CurveRadius, StartPosition.Y + CurveRadius);

        ImGui.SetCursorScreenPos(topLeftCurveCenter);
        ImGui.PushTextWrapPos(ImGui.GetCursorPos().X + Size.X - CurveRadius * 2.0f);

        ImGui.BeginGroup();
        ImGui.PushID(Label);

        DrawListContents();

        ImGui.PopID();
        ImGui.EndGroup();

        ImGui.PopTextWrapPos();

        DrawActions.Clear();
    }

    private void DrawCorners()
    {
        var topLeftCurveCenter = new Vector2(StartPosition.X + CurveRadius, StartPosition.Y + CurveRadius);
        var topRightCurveCenter = new Vector2(StartPosition.X + Size.X - CurveRadius, StartPosition.Y + CurveRadius);
        var bottomLeftCurveCenter = new Vector2(StartPosition.X + CurveRadius, StartPosition.Y + Size.Y - CurveRadius);
        var bottomRightCurveCenter = new Vector2(StartPosition.X + Size.X - CurveRadius, StartPosition.Y + Size.Y - CurveRadius);

        DrawList.PathArcTo(topLeftCurveCenter, CurveRadius, MathF.PI, 1.5f * MathF.PI, SegmentResolution);
        DrawList.PathStroke(BorderColor.ToU32(), ImDrawFlags.None, BorderThickness);

        DrawList.PathArcTo(topRightCurveCenter, CurveRadius, 2.0f * MathF.PI, 1.5f * MathF.PI, SegmentResolution);
        DrawList.PathStroke(BorderColor.ToU32(), ImDrawFlags.None, BorderThickness);

        DrawList.PathArcTo(bottomLeftCurveCenter, CurveRadius, 0.5f * MathF.PI, MathF.PI, SegmentResolution);
        DrawList.PathStroke(BorderColor.ToU32(), ImDrawFlags.None, BorderThickness);

        DrawList.PathArcTo(bottomRightCurveCenter, CurveRadius, 0.0f, 0.5f * MathF.PI, SegmentResolution);
        DrawList.PathStroke(BorderColor.ToU32(), ImDrawFlags.None, BorderThickness);
    }

    private void DrawBorders()
    {
        var color = BorderColor.ToU32();

        DrawList.AddLine(new Vector2(StartPosition.X - 0.5f, StartPosition.Y + CurveRadius - 0.5f), new Vector2(StartPosition.X - 0.5f, StartPosition.Y + Size.Y - CurveRadius + 0.5f), color, BorderThickness);
        DrawList.AddLine(new Vector2(StartPosition.X + Size.X - 0.5f, StartPosition.Y + CurveRadius - 0.5f), new Vector2(StartPosition.X + Size.X - 0.5f, StartPosition.Y + Size.Y - CurveRadius + 0.5f), color, BorderThickness);
        DrawList.AddLine(new Vector2(StartPosition.X + CurveRadius - 0.5f, StartPosition.Y + Size.Y - 0.5f), new Vector2(StartPosition.X + Size.X - CurveRadius + 0.5f, StartPosition.Y + Size.Y - 0.5f), color, BorderThickness);

        var textSize = ImGui.CalcTextSize(Label);
        var textStartPadding = 7.0f * ImGuiHelpers.GlobalScale;
        var textEndPadding = 7.0f * ImGuiHelpers.GlobalScale;
        var textVerticalOffset = textSize.Y / 2.0f;

        DrawList.AddText(new Vector2(StartPosition.X + CurveRadius + textStartPadding, StartPosition.Y - textVerticalOffset), TitleColor.ToU32(), Label);
        DrawList.AddLine(new Vector2(StartPosition.X + CurveRadius + textStartPadding + textSize.X + textEndPadding, StartPosition.Y - 0.5f), new Vector2(StartPosition.X + Size.X - CurveRadius - 0.5f, StartPosition.Y - 0.5f), color, BorderThickness);
    }
    
    public InfoBox AddTitle(string title, float percentFill = 0.80f)
    {
        Label = title;
        WidthPercentage = percentFill;

        return DrawListOwner;
    }

    public InfoBox AddTitle(string title, out float innerWidth, float percentFill = 0.80f)
    {
        Label = title;
        WidthPercentage = percentFill;

        TotalWidth = RegionAvailable.X * WidthPercentage;
        InnerWidth = TotalWidth - CurveRadius * 3.0f;
        
        innerWidth = InnerWidth;
        return DrawListOwner;
    }

    public InfoBoxTable BeginTable(float weight = 0.50f) => new InfoBoxTable(this, weight);

    public InfoBoxList BeginList() => new InfoBoxList(this);

    public InfoBox AddList(IEnumerable<IInfoBoxListConfigurationRow> rows)
    {
        return BeginList()
            .AddRows(rows)
            .EndList();
    }
}