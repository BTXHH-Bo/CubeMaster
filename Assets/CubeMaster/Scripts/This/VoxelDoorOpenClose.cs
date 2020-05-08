using UnityEngine;
using System.Collections;
namespace CubeMaster {

	//放置门

	public class VoxelDoorOpenClose : DefaultVoxelEvents
	{

		public override void OnMouseDown(int mouseButton, VoxelInfo voxelInfo)
		{

			if (mouseButton == 0)
			{
				Voxel.DestroyBlock(voxelInfo);
			}

			else if (mouseButton == 1)
			{ //右键开关门

				if (voxelInfo.GetVoxel() == 70)
				{ // 门开
					Voxel.ChangeBlock(voxelInfo, 7); // 则关
				}

				else if (voxelInfo.GetVoxel() == 7)
				{ // 门关
					Voxel.ChangeBlock(voxelInfo, 70); // 则开
				}

			}
		}

	}

}