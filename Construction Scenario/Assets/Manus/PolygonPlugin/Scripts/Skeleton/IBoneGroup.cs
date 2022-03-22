using System.Collections.Generic;
using Hermes.Protocol.Polygon;

namespace Manus.Polygon
{
	public interface IBoneGroup
	{
		Dictionary<BoneType, Bone> GatherBones(GatherType p_GatherType);
	}
}

