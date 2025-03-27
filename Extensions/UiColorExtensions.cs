using System.Numerics;
using Lumina.Excel.Sheets;

namespace KamiLib.Extensions;

public static class UiColorExtensions {
	public static Vector4 Foreground(this UIColor color)
		=> ConvertToVector4(color.Dark);

	public static Vector4 Glow(this UIColor color)
		=> ConvertToVector4(color.Light);
	
	private static Vector4 ConvertToVector4(uint color) {
		var r = (byte)(color >> 24);
		var g = (byte)(color >> 16);
		var b = (byte)(color >> 8);
		var a = (byte)color;

		return new Vector4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
	}
}