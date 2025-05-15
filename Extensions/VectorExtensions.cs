using System.Numerics;
using Dalamud.Interface;

namespace KamiLib.Extensions;

public static class VectorExtensions {
	public static Vector3 AsNormalizedUnitVector3(this Vector3 vector)
		=> vector.AsVector4().NormalizeToUnitRange().AsVector3();
}