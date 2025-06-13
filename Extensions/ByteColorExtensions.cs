using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics;

namespace KamiLib.Extensions;

public static class ByteColorExtensions {
	public static Vector4 ToVector4(this ByteColor color)
		=> new Vector4(color.R, color.G, color.B, color.A) / 255.0f;
}