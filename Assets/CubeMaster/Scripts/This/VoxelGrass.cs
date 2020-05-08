using UnityEngine;
using System.Collections;

//带草的尼

namespace CubeMaster {

	public class VoxelGrass : DefaultVoxelEvents
	{

		public override void OnBlockPlace(VoxelInfo voxelInfo)
		{

			// 头顶非空则变泥
			Index adjacentIndex = voxelInfo.chunk.GetAdjacentIndex(voxelInfo.index, Direction.up);
			if (voxelInfo.chunk.GetVoxel(adjacentIndex) != 0)
			{
				voxelInfo.chunk.SetVoxel(voxelInfo.index, 1, true);
			}

			Index indexBelow = new Index(voxelInfo.index.x, voxelInfo.index.y - 1, voxelInfo.index.z);

			if (voxelInfo.GetVoxelType().VTransparency == Transparency.solid
			&& voxelInfo.chunk.GetVoxel(indexBelow) == 2)
			{
				voxelInfo.chunk.SetVoxel(indexBelow, 1, true);
			}

		}
	}

}