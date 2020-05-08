using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ����

namespace CubeMaster {

	public enum MeshRotation
	{
		none, back, left, right
	}

	public class ChunkMeshCreator : MonoBehaviour
	{

		private Chunk chunk;
		private int SideLength;
		private GameObject noCollideCollider;

		public Mesh Cube;

		// �洢mesh���ݵ�
		private List<Vector3> Vertices = new List<Vector3>();
		private List<List<int>> Faces = new List<List<int>>();
		private List<Vector2> UVs = new List<Vector2>();
		private int FaceCount;

		// �洢collider���ݵ�
		private List<Vector3> SolidColliderVertices = new List<Vector3>();
		private List<int> SolidColliderFaces = new List<int>();
		private int SolidFaceCount;
		private List<Vector3> NoCollideVertices = new List<Vector3>();
		private List<int> NoCollideFaces = new List<int>();
		private int NoCollideFaceCount;

		private bool initialized;

		public void Initialize()
		{

			chunk = GetComponent<Chunk>();
			SideLength = chunk.SideLength;

			// ��������ʸ�һ���б�
			for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
			{
				Faces.Add(new List<int>());
			}

			initialized = true;
		}

		// ==== ����voxel

		public void RebuildMesh()
		{

			if (!initialized)
			{
				Initialize();
			}

			//���ٶ����mesh����
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}

			int x = 0, y = 0, z = 0;

			// ˢ��һ��
			chunk.GetNeighbors();

			//����ÿһ��voxel���������Ƿ�¶�����ǣ�������뵽������
			while (x < SideLength)
			{
				while (y < SideLength)
				{
					while (z < SideLength)
					{

						ushort voxel = chunk.GetVoxel(x, y, z); // current voxel data
						if (voxel != 0)
						{ // ���ܿյ�

							Voxel voxelType = Engine.GetVoxelType(voxel);
							if (voxelType.VCustomMesh == false)
							{//�Ǹ���

								Transparency transparency = voxelType.VTransparency;
								ColliderType colliderType = voxelType.VColliderType;

								if (CheckAdjacent(x, y, z, Direction.forward, transparency) == true)
									CreateFace(voxel, Facing.forward, colliderType, x, y, z);

								if (CheckAdjacent(x, y, z, Direction.back, transparency) == true)
									CreateFace(voxel, Facing.back, colliderType, x, y, z);

								if (CheckAdjacent(x, y, z, Direction.up, transparency) == true)
									CreateFace(voxel, Facing.up, colliderType, x, y, z);

								if (CheckAdjacent(x, y, z, Direction.down, transparency) == true)
									CreateFace(voxel, Facing.down, colliderType, x, y, z);

								if (CheckAdjacent(x, y, z, Direction.right, transparency) == true)
									CreateFace(voxel, Facing.right, colliderType, x, y, z);

								if (CheckAdjacent(x, y, z, Direction.left, transparency) == true)
									CreateFace(voxel, Facing.left, colliderType, x, y, z);

								if (colliderType == ColliderType.none && Engine.GenerateColliders)
								{
									AddCubeMesh(x, y, z, false);
								}
							}
							else
							{ // ���Ǹ���
								if (CheckAllAdjacent(x, y, z) == false)
								{ //�������voxel���ǲ�͸����
									CreateCustomMesh(voxel, x, y, z, voxelType.VMesh);
								}
							}
						}
						z += 1;
					}
					z = 0;
					y += 1;

				}
				y = 0;
				x += 1;
			}

			// ����mesh
			UpdateMesh(GetComponent<MeshFilter>().mesh);
		}


		private bool CheckAdjacent(int x, int y, int z, Direction direction, Transparency transparency)
		{

			Index index = chunk.GetAdjacentIndex(x, y, z, direction);
			ushort adjacentVoxel = chunk.GetVoxel(index.x, index.y, index.z);

			if (adjacentVoxel == ushort.MaxValue)
			{

				if (Engine.ShowBorderFaces || direction == Direction.up)
				{
					return true;
				}
				else
				{
					return false;
				}

			}

			Transparency result = Engine.GetVoxelType(adjacentVoxel).VTransparency;

			if (transparency == Transparency.transparent)
			{
				if (result == Transparency.transparent)
					return false;
				else
					return true;
			}
			else
			{
				if (result == Transparency.solid)
					return false;
				else
					return true;
			}
		}

		//������voxel͸��
		public bool CheckAllAdjacent(int x, int y, int z)
		{

			for (int direction = 0; direction < 6; direction++)
			{
				if (Engine.GetVoxelType(chunk.GetVoxel(chunk.GetAdjacentIndex(x, y, z, (Direction)direction))).VTransparency != Transparency.solid)
				{
					return false;
				}
			}
			return true;
		}


		// ==== ����mesh

		private void CreateFace(ushort voxel, Facing facing, ColliderType colliderType, int x, int y, int z)
		{

			Voxel voxelComponent = Engine.GetVoxelType(voxel);
			List<int> FacesList = Faces[voxelComponent.VSubmeshIndex];

			// ==== ���� ====

			// ������������������
			if (facing == Facing.forward)
			{
				Vertices.Add(new Vector3(x + 0.5001f, y + 0.5001f, z + 0.5f));
				Vertices.Add(new Vector3(x - 0.5001f, y + 0.5001f, z + 0.5f));
				Vertices.Add(new Vector3(x - 0.5001f, y - 0.5001f, z + 0.5f));
				Vertices.Add(new Vector3(x + 0.5001f, y - 0.5001f, z + 0.5f));
				if (colliderType == ColliderType.cube && Engine.GenerateColliders)
				{
					SolidColliderVertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
					SolidColliderVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
					SolidColliderVertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
					SolidColliderVertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
				}
			}
			else if (facing == Facing.up)
			{
				Vertices.Add(new Vector3(x - 0.5001f, y + 0.5f, z + 0.5001f));
				Vertices.Add(new Vector3(x + 0.5001f, y + 0.5f, z + 0.5001f));
				Vertices.Add(new Vector3(x + 0.5001f, y + 0.5f, z - 0.5001f));
				Vertices.Add(new Vector3(x - 0.5001f, y + 0.5f, z - 0.5001f));
				if (colliderType == ColliderType.cube && Engine.GenerateColliders)
				{
					SolidColliderVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
					SolidColliderVertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
					SolidColliderVertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
					SolidColliderVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
				}
			}
			else if (facing == Facing.right)
			{
				Vertices.Add(new Vector3(x + 0.5f, y + 0.5001f, z - 0.5001f));
				Vertices.Add(new Vector3(x + 0.5f, y + 0.5001f, z + 0.5001f));
				Vertices.Add(new Vector3(x + 0.5f, y - 0.5001f, z + 0.5001f));
				Vertices.Add(new Vector3(x + 0.5f, y - 0.5001f, z - 0.5001f));
				if (colliderType == ColliderType.cube && Engine.GenerateColliders)
				{
					SolidColliderVertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
					SolidColliderVertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
					SolidColliderVertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
					SolidColliderVertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
				}
			}
			else if (facing == Facing.back)
			{
				Vertices.Add(new Vector3(x - 0.5001f, y + 0.5001f, z - 0.5f));
				Vertices.Add(new Vector3(x + 0.5001f, y + 0.5001f, z - 0.5f));
				Vertices.Add(new Vector3(x + 0.5001f, y - 0.5001f, z - 0.5f));
				Vertices.Add(new Vector3(x - 0.5001f, y - 0.5001f, z - 0.5f));
				if (colliderType == ColliderType.cube && Engine.GenerateColliders)
				{
					SolidColliderVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
					SolidColliderVertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
					SolidColliderVertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
					SolidColliderVertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
				}
			}
			else if (facing == Facing.down)
			{
				Vertices.Add(new Vector3(x - 0.5001f, y - 0.5f, z - 0.5001f));
				Vertices.Add(new Vector3(x + 0.5001f, y - 0.5f, z - 0.5001f));
				Vertices.Add(new Vector3(x + 0.5001f, y - 0.5f, z + 0.5001f));
				Vertices.Add(new Vector3(x - 0.5001f, y - 0.5f, z + 0.5001f));
				if (colliderType == ColliderType.cube && Engine.GenerateColliders)
				{
					SolidColliderVertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
					SolidColliderVertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
					SolidColliderVertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
					SolidColliderVertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
				}
			}
			else if (facing == Facing.left)
			{
				Vertices.Add(new Vector3(x - 0.5f, y + 0.5001f, z + 0.5001f));
				Vertices.Add(new Vector3(x - 0.5f, y + 0.5001f, z - 0.5001f));
				Vertices.Add(new Vector3(x - 0.5f, y - 0.5001f, z - 0.5001f));
				Vertices.Add(new Vector3(x - 0.5f, y - 0.5001f, z + 0.5001f));
				if (colliderType == ColliderType.cube && Engine.GenerateColliders)
				{
					SolidColliderVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
					SolidColliderVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
					SolidColliderVertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
					SolidColliderVertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
				}
			}

			// ==== UV =====
			float tUnit = Engine.TextureUnit;
			Vector2 tOffset = Engine.GetTextureOffset(voxel, facing);

			float pad = tUnit * Engine.TexturePadding;
			UVs.Add(new Vector2(tUnit * tOffset.x + pad, tUnit * tOffset.y + tUnit - pad)); // top left
			UVs.Add(new Vector2(tUnit * tOffset.x + tUnit - pad, tUnit * tOffset.y + tUnit - pad)); // top right
			UVs.Add(new Vector2(tUnit * tOffset.x + tUnit - pad, tUnit * tOffset.y + pad)); // bottom right
			UVs.Add(new Vector2(tUnit * tOffset.x + pad, tUnit * tOffset.y + pad)); // bottom left

			// ==== Face ====
			FacesList.Add(FaceCount + 0);
			FacesList.Add(FaceCount + 1);
			FacesList.Add(FaceCount + 3);
			FacesList.Add(FaceCount + 1);
			FacesList.Add(FaceCount + 2);
			FacesList.Add(FaceCount + 3);
			if (colliderType == ColliderType.cube && Engine.GenerateColliders)
			{
				SolidColliderFaces.Add(SolidFaceCount + 0);
				SolidColliderFaces.Add(SolidFaceCount + 1);
				SolidColliderFaces.Add(SolidFaceCount + 3);
				SolidColliderFaces.Add(SolidFaceCount + 1);
				SolidColliderFaces.Add(SolidFaceCount + 2);
				SolidColliderFaces.Add(SolidFaceCount + 3);
			}

			//ÿ����4������
			FaceCount += 4;
			if (colliderType == ColliderType.cube && Engine.GenerateColliders)
			{
				SolidFaceCount += 4;
			}

			// ���
			if (Vertices.Count > 65530)
			{
				CreateNewMeshObject();
			}
		}

		private void CreateCustomMesh(ushort voxel, int x, int y, int z, Mesh mesh)
		{

			Voxel voxelComponent = Engine.GetVoxelType(voxel);
			List<int> FacesList = Faces[voxelComponent.VSubmeshIndex];

			// ���mesh�Ƿ����
			if (mesh == null)
			{
				Debug.LogError("CubeMaster: voxel: " + voxel + " ʹ�����Զ����mesh��û�з���mesh!");
				return;
			}


			// === mesh

			// ����Ƿ����㹻�Ŀռ����洢����Ķ���
			if (Vertices.Count + mesh.vertices.Length > 65534)
			{
				CreateNewMeshObject();
			}

			//��ת����
			List<Vector3> rotatedVertices = new List<Vector3>();
			MeshRotation rotation = voxelComponent.VRotation;

			// x-->z ˮƽ180
			if (rotation == MeshRotation.back)
			{
				foreach (Vector3 vertex in mesh.vertices)
				{
					rotatedVertices.Add(new Vector3(-vertex.x, vertex.y, -vertex.z));
				}
			}

			// ��90
			else if (rotation == MeshRotation.right)
			{
				foreach (Vector3 vertex in mesh.vertices)
				{
					rotatedVertices.Add(new Vector3(vertex.z, vertex.y, -vertex.x));
				}
			}

			// ��90
			else if (rotation == MeshRotation.left)
			{
				foreach (Vector3 vertex in mesh.vertices)
				{
					rotatedVertices.Add(new Vector3(-vertex.z, vertex.y, vertex.x));
				}
			}

			// ��
			else
			{
				foreach (Vector3 vertex in mesh.vertices)
				{
					rotatedVertices.Add(vertex);
				}
			}

			// ����
			foreach (Vector3 vertex in rotatedVertices)
			{
				Vertices.Add(vertex + new Vector3(x, y, z)); // add all vertices from the mesh
			}

			// UV
			foreach (Vector2 uv in mesh.uv)
			{
				UVs.Add(uv);
			}

			// face
			foreach (int face in mesh.triangles)
			{
				FacesList.Add(FaceCount + face);
			}

			FaceCount += mesh.vertexCount;


			// === ��ײ��
			if (Engine.GenerateColliders)
			{
				ColliderType colliderType = Engine.GetVoxelType(voxel).VColliderType;

				// ������ײ��
				if (colliderType == ColliderType.mesh)
				{
					foreach (Vector3 vertex1 in rotatedVertices)
					{
						SolidColliderVertices.Add(vertex1 + new Vector3(x, y, z)); // �����meshcollider���ʹ����meshȡ������ӵ�solidcollider
					}
					foreach (int face1 in mesh.triangles)
					{
						SolidColliderFaces.Add(SolidFaceCount + face1);
					}
					SolidFaceCount += mesh.vertexCount;
				}

				// cube collider
				if (colliderType == ColliderType.cube)
				{
					AddCubeMesh(x, y, z, true);
				}
				else if (voxel != 0)
				{
					AddCubeMesh(x, y, z, false);
				}
			}
		}

		//���cube�ĵ��浽list
		private void AddCubeMesh(int x, int y, int z, bool solid)
		{

			if (solid)
			{
				// vertices
				foreach (Vector3 vertex in Cube.vertices)
				{
					//�����ж���ӵ�mesh
					SolidColliderVertices.Add(vertex + new Vector3(x, y, z));
				}

				// faces
				foreach (int face in Cube.triangles)
				{
					SolidColliderFaces.Add(SolidFaceCount + face);
				}

				SolidFaceCount += Cube.vertexCount;
			}
			else
			{
				// vertices
				foreach (Vector3 vertex1 in Cube.vertices)
				{
					NoCollideVertices.Add(vertex1 + new Vector3(x, y, z));
				}

				// faces
				foreach (int face1 in Cube.triangles)
				{
					NoCollideFaces.Add(NoCollideFaceCount + face1);
				}

				NoCollideFaceCount += Cube.vertexCount;
			}
		}

		private void UpdateMesh(Mesh mesh)
		{

			// ����mesh
			mesh.Clear();
			mesh.vertices = Vertices.ToArray();
			mesh.subMeshCount = GetComponent<Renderer>().materials.Length;

			for (int i = 0; i < Faces.Count; ++i)
			{
				mesh.SetTriangles(Faces[i].ToArray(), i);
			}

			mesh.uv = UVs.ToArray();
			;
			mesh.RecalculateNormals();

			if (Engine.GenerateColliders)
			{

				// ���� solid collider
				Mesh colMesh = new Mesh();

				colMesh.vertices = SolidColliderVertices.ToArray();
				colMesh.triangles = SolidColliderFaces.ToArray();
				;
				colMesh.RecalculateNormals();

				GetComponent<MeshCollider>().sharedMesh = null;
				GetComponent<MeshCollider>().sharedMesh = colMesh;

				// ����nocollide collider
				if (NoCollideVertices.Count > 0)
				{

					// make mesh
					Mesh nocolMesh = new Mesh();
					nocolMesh.vertices = NoCollideVertices.ToArray();
					nocolMesh.triangles = NoCollideFaces.ToArray();
					;
					nocolMesh.RecalculateNormals();

					noCollideCollider = Instantiate(chunk.ChunkCollider, transform.position, transform.rotation) as GameObject;
					noCollideCollider.transform.parent = this.transform;
					noCollideCollider.GetComponent<MeshCollider>().sharedMesh = nocolMesh;

				}
				else if (noCollideCollider != null)
				{
					Destroy(noCollideCollider);
				}
			}


			// ���
			Vertices.Clear();
			UVs.Clear();
			foreach (List<int> faceList in Faces)
			{
				faceList.Clear();
			}

			SolidColliderVertices.Clear();
			SolidColliderFaces.Clear();

			NoCollideVertices.Clear();
			NoCollideFaces.Clear();


			FaceCount = 0;
			SolidFaceCount = 0;
			NoCollideFaceCount = 0;


		}


		//��������������mesh���ֵʱ�������µ�meshobject
		private void CreateNewMeshObject()
		{

			GameObject meshContainer = Instantiate(chunk.MeshContainer, transform.position, transform.rotation) as GameObject;
			meshContainer.transform.parent = this.transform;

			UpdateMesh(meshContainer.GetComponent<MeshFilter>().mesh);
		}


	}

}