using UnityEngine;
using System.Collections;

// laozuzong

//这里面有一个bug，就是需要没有检测即将被放置方块的位置上有没有角色。

namespace CubeMaster {

	public class VoxelEvents : MonoBehaviour
	{

		public virtual void OnMouseDown(int mouseButton, VoxelInfo voxelInfo)
		{

		}

		public virtual void OnMouseUp(int mouseButton, VoxelInfo voxelInfo)
		{

		}

		public virtual void OnMouseHold(int mouseButton, VoxelInfo voxelInfo)
		{

		}

		public virtual void OnLook(VoxelInfo voxelInfo)
		{

		}

		public virtual void OnBlockPlace(VoxelInfo voxelInfo)
		{

		}



		public virtual void OnBlockDestroy(VoxelInfo voxelInfo)
		{

		}


		public virtual void OnBlockChange(VoxelInfo voxelInfo)
		{

		}


		public virtual void OnBlockEnter(GameObject enteringObject, VoxelInfo voxelInfo)
		{

		}

		public virtual void OnBlockStay(GameObject stayingObject, VoxelInfo voxelInfo)
		{

		}
	}

}