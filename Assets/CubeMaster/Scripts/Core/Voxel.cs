using UnityEngine;

namespace CubeMaster {

	public class Voxel : MonoBehaviour
	{

		public string VName;
		public Mesh VMesh;
		public bool VCustomMesh;
		public bool VCustomSides;
		public Vector2[] VTexture; // ÌùÍ¼Ë÷Òý£¬0ÉÏ
		public Transparency VTransparency;
		public ColliderType VColliderType;
		public int VSubmeshIndex;
		public MeshRotation VRotation;


		public static void DestroyBlock(VoxelInfo voxelInfo)
		{
			GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(voxelInfo.GetVoxel())) as GameObject;
			if (voxelObject.GetComponent<VoxelEvents>() != null)
			{
				voxelObject.GetComponent<VoxelEvents>().OnBlockDestroy(voxelInfo);

				voxelInfo.chunk.SetVoxel(voxelInfo.index, 0, true);
				Destroy(voxelObject);
			}
		}

		public static void PlaceBlock(VoxelInfo voxelInfo, ushort data)
		{
			voxelInfo.chunk.SetVoxel(voxelInfo.index, data, true);

			GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(data)) as GameObject;
			if (voxelObject.GetComponent<VoxelEvents>() != null)
			{
				voxelObject.GetComponent<VoxelEvents>().OnBlockPlace(voxelInfo);
			}
			Destroy(voxelObject);

		}

		public static void ChangeBlock(VoxelInfo voxelInfo, ushort data)
		{
			voxelInfo.chunk.SetVoxel(voxelInfo.index, data, true);

			GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(data)) as GameObject;
			if (voxelObject.GetComponent<VoxelEvents>() != null)
			{
				voxelObject.GetComponent<VoxelEvents>().OnBlockChange(voxelInfo);
			}
			Destroy(voxelObject);

		}

	}

}