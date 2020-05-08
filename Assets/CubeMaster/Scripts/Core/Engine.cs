using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Globe
{
	public static string worldName;
	public static bool isChooseWorld = false;
	public static string worldPath;
	public static bool isMouseVisible;
	public static float walkSpeed = 4.0f;
	public static float RunSpeed = 6.0f;
	public static float JumpSpeed = 10.0f;
	public static float Height = 1.8f;
	public static float StepInterval = 5f;
	public static float GravityMultiplier = 2f;
	public static bool Audio = true;
	public static bool HeadBob = true;
	public static CharacterController characterController;
	public static AudioSource audioSource;
}

namespace CubeMaster {

	// enums
	public enum Facing {
		up, down, right, left, forward, back
	}
	public enum Direction {
		up, down, right, left, forward, back
	}

	public enum Transparency {
		solid, semiTransparent, transparent
	}
	public enum ColliderType {
		cube, mesh, none
	}
	
	//参考
	public class Engine : MonoBehaviour {

		// file paths
		public static string WorldName = Globe.worldName, WorldPath, BlocksPath;
		public static int WorldSeed;
		public string lWorldName = Globe.worldName;
		public string lBlocksPath;

		// voxels
		public static GameObject[] Blocks;
		public GameObject[] lBlocks;

		// chunk spawn settings
		public static int HeightRange, ChunkSpawnDistance, ChunkSideLength, ChunkDespawnDistance;
		public int lHeightRange, lChunkSpawnDistance, lChunkSideLength, lChunkDespawnDistance;

		// texture settings
		public static float TextureUnit, TexturePadding;
		public float lTextureUnit, lTexturePadding;

		// performance settings
		public static int TargetFPS, MaxChunkSaves, MaxChunkDataRequests;
		public int lTargetFPS, lMaxChunkSaves, lMaxChunkDataRequests;

		// global settings	

		public static bool ShowBorderFaces, GenerateColliders, SendCameraLookEvents,
		SendCursorEvents, SaveVoxelData, GenerateMeshes;

		public bool lShowBorderFaces, lGenerateColliders, lSendCameraLookEvents,
		lSendCursorEvents, lSaveVoxelData, lGenerateMeshes;

		public static float ChunkTimeout;
		public float lChunkTimeout;
		public static bool EnableChunkTimeout;

		// other
		public static int SquaredSideLength;
		public static Engine EngineInstance;
		public static ChunkManager ChunkManagerInstance;

		public static Vector3 ChunkScale;

		public static bool Initialized;

		void Start()
		{
			SetWorldName(Globe.worldName);
			Globe.isMouseVisible = false;
		}
		//以下的check都没有必要
		// ==== initialization ====
		public void Awake() {

			Engine.EngineInstance = this;
			Engine.ChunkManagerInstance = GetComponent<ChunkManager>();

			WorldName = lWorldName;
			UpdateWorldPath();

			BlocksPath = lBlocksPath;
			Engine.Blocks = lBlocks;

			TargetFPS = lTargetFPS;
			MaxChunkSaves = lMaxChunkSaves;
			MaxChunkDataRequests = lMaxChunkDataRequests;

			TextureUnit = lTextureUnit;
			TexturePadding = lTexturePadding;
			GenerateColliders = lGenerateColliders;
			ShowBorderFaces = lShowBorderFaces;
			SaveVoxelData = lSaveVoxelData;
			GenerateMeshes = lGenerateMeshes;

			ChunkSpawnDistance = lChunkSpawnDistance;
			HeightRange = lHeightRange;
			ChunkDespawnDistance = lChunkDespawnDistance;

			SendCameraLookEvents = lSendCameraLookEvents;
			SendCursorEvents = lSendCursorEvents;

			ChunkSideLength = lChunkSideLength;
			SquaredSideLength = lChunkSideLength * lChunkSideLength;

			ChunkDataFiles.LoadedRegions = new Dictionary<string, string[]>();
			ChunkDataFiles.TempChunkData = new Dictionary<string, string>();

			if (lChunkTimeout <= 0.00001f) {
				EnableChunkTimeout = false;
			}
			else {
				EnableChunkTimeout = true;
				ChunkTimeout = lChunkTimeout;
			}

			for (int i = 0; i < 31; i++) {
				Physics.IgnoreLayerCollision(i, 26);
			}


			// check block array
			if (Engine.Blocks.Length < 1) {
				Debug.LogError("CubeMaster: The blocks array is empty! Use the Block Editor to update the blocks array.");
				Debug.Break();
			}

			if (Engine.Blocks[0] == null) {
				Debug.LogError("CubeMaster: Cannot find the empty block prefab (id 0)!");
				Debug.Break();
			}
			else if (Engine.Blocks[0].GetComponent<Voxel>() == null) {
				Debug.LogError("CubeMaster: Voxel id 0 does not have the Voxel component attached!");
				Debug.Break();
			}

			// check settings
			if (Engine.ChunkSideLength < 1) {
				Debug.LogError("CubeMaster: Chunk side length must be greater than 0!");
				Debug.Break();
			}

			if (Engine.ChunkSpawnDistance < 1) {
				Engine.ChunkSpawnDistance = 0;
				Debug.LogWarning("CubeMaster: Chunk spawn distance is 0. No chunks will spawn!");
			}

			if (Engine.HeightRange < 0) {
				Engine.HeightRange = 0;
				Debug.LogWarning("CubeMaster: Chunk height range can't be a negative number! Setting chunk height range to 0.");
			}

			if (Engine.MaxChunkDataRequests < 0) {
				Engine.MaxChunkDataRequests = 0;
				Debug.LogWarning("CubeMaster: Max chunk data requests can't be a negative number! Setting max chunk data requests to 0.");
			}

			// check materials
			GameObject chunkPrefab = GetComponent<ChunkManager>().ChunkObject;
			int materialCount = chunkPrefab.GetComponent<Renderer>().sharedMaterials.Length - 1;

			for (ushort i = 0; i < Engine.Blocks.Length; i++) {

				if (Engine.Blocks[i] != null) {
					Voxel voxel = Engine.Blocks[i].GetComponent<Voxel>();

					if (voxel.VSubmeshIndex < 0) {
						Debug.LogError("CubeMaster: Voxel " + i + " has a material index lower than 0! Material index must be 0 or greater.");
						Debug.Break();
					}

					if (voxel.VSubmeshIndex > materialCount) {
						Debug.LogError("CubeMaster: Voxel " + i + " uses material index " + voxel.VSubmeshIndex + ", but the chunk prefab only has " + (materialCount + 1) + " material(s) attached. Set a lower material index or attach more materials to the chunk prefab.");
						Debug.Break();
					}
				}
			}

			// check anti-aliasing
			if (QualitySettings.antiAliasing > 0) {
				Debug.LogWarning("CubeMaster: Anti-aliasing is enabled. This may cause seam lines to appear between blocks. If you see lines between blocks, try disabling anti-aliasing, switching to deferred rendering path, or adding some texture padding in the engine settings.");
			}


			Engine.Initialized = true;

		}

		// ==== 世界场景数据

		private static void UpdateWorldPath() {
			WorldPath = Application.dataPath + "/../Worlds/" + Engine.WorldName + "/"; // you can set World Path here
																					   //WorldPath = "/mnt/sdcard/UniblocksWorlds/" + Engine.WorldName + "/"; // example mobile path for Android
		}

		public static void SetWorldName(string worldName) {
			Engine.WorldName = worldName;
			WorldSeed = 0;
			UpdateWorldPath();
		}

		//从文件中读取世界种子，若无文件则创建一个新的种子文件并保存
		public static void GetSeed() {
			if (File.Exists(Engine.WorldPath + "seed")) {
				StreamReader reader = new StreamReader(Engine.WorldPath + "seed");
				WorldSeed = int.Parse(reader.ReadToEnd());
				reader.Close();
			}
			else {//创建世界种子
				while (WorldSeed == 0) {
					WorldSeed = Random.Range(ushort.MinValue, ushort.MaxValue);
				}
				System.IO.Directory.CreateDirectory(Engine.WorldPath);
				StreamWriter writer = new StreamWriter(Engine.WorldPath + "seed");
				writer.Write(WorldSeed.ToString());
				writer.Flush();
				writer.Close();
			}
		}

		public static void SaveWorld() {
			Engine.EngineInstance.StartCoroutine(ChunkDataFiles.SaveAllChunks());
		}

		public static void SaveWorldInstant() {
			ChunkDataFiles.SaveAllChunksInstant();
		}

		// ==== 其他 ====	


		public static GameObject GetVoxelGameObject(ushort voxelId) {
			try {
				if (voxelId == ushort.MaxValue) voxelId = 0;
				GameObject voxelObject = Engine.Blocks[voxelId];
				if (voxelObject.GetComponent<Voxel>() == null) {
					Debug.LogError("CubeMaster: Voxel id " + voxelId + " does not have the Voxel component attached!");
					return Engine.Blocks[0];
				}
				else {
					return voxelObject;
				}

			}
			catch (System.Exception) {
				Debug.LogError("CubeMaster: Invalid voxel id: " + voxelId);
				return Engine.Blocks[0];
			}
		}

		public static Voxel GetVoxelType(ushort voxelId) {
			try {
				if (voxelId == ushort.MaxValue) voxelId = 0;
				Voxel voxel = Engine.Blocks[(int)voxelId].GetComponent<Voxel>();
				if (voxel == null) {
					Debug.LogError("CubeMaster: Voxel id " + voxelId + " does not have the Voxel component attached!");
					return null;
				}
				else {
					return voxel;
				}

			}
			catch (System.Exception) {
				Debug.LogError("CubeMaster: Invalid voxel id: " + voxelId);
				return null;
			}
		}



		//返回被射线瞄准的体素信息
		public static VoxelInfo VoxelRaycast(Vector3 origin, Vector3 direction, float range, bool ignoreTransparent) {

			RaycastHit hit = new RaycastHit();

			if (Physics.Raycast(origin, direction, out hit, range)) {

				if (hit.collider.GetComponent<Chunk>() != null
					|| hit.collider.GetComponent<ChunkExtension>() != null) { // 是否瞄准了一个chunk

					GameObject hitObject = hit.collider.gameObject;

					if (hitObject.GetComponent<ChunkExtension>() != null) { // 如果瞄准到mesh容器
						hitObject = hitObject.transform.parent.gameObject; // 交换
					}

					Index hitIndex = hitObject.GetComponent<Chunk>().PositionToVoxelIndex(hit.point, hit.normal, false);

					if (ignoreTransparent) { // 投射到透明的voxel时，穿过
						ushort hitVoxel = hitObject.GetComponent<Chunk>().GetVoxel(hitIndex.x, hitIndex.y, hitIndex.z);
						if (Engine.GetVoxelType(hitVoxel).VTransparency != Transparency.solid) { // 投射到透明体素时
							Vector3 newOrigin = hit.point;
							newOrigin.y -= 0.5f; // 新射线下移
							return Engine.VoxelRaycast(newOrigin, Vector3.down, range - hit.distance, true);
						}
					}


					return new VoxelInfo(
										 hitObject.GetComponent<Chunk>().PositionToVoxelIndex(hit.point, hit.normal, false), // 获取被投射的体素index
										 hitObject.GetComponent<Chunk>().PositionToVoxelIndex(hit.point, hit.normal, true), // 获取前者相邻体素index
										 hitObject.GetComponent<Chunk>()); // 获取被瞄准体素所在chunk
				}
			}

			//啥也没瞄准
			return null;
		}

		public static VoxelInfo VoxelRaycast(Ray ray, float range, bool ignoreTransparent) {
			return Engine.VoxelRaycast(ray.origin, ray.direction, range, ignoreTransparent);
		}



		public static Index PositionToChunkIndex(Vector3 position) {
			Index chunkIndex = new Index(Mathf.RoundToInt(position.x / Engine.ChunkScale.x) / Engine.ChunkSideLength,
										  Mathf.RoundToInt(position.y / Engine.ChunkScale.y) / Engine.ChunkSideLength,
										  Mathf.RoundToInt(position.z / Engine.ChunkScale.z) / Engine.ChunkSideLength);
			return chunkIndex;
		}

		public static GameObject PositionToChunk(Vector3 position) {
			Index chunkIndex = new Index(Mathf.RoundToInt(position.x / Engine.ChunkScale.x) / Engine.ChunkSideLength,
										  Mathf.RoundToInt(position.y / Engine.ChunkScale.y) / Engine.ChunkSideLength,
										  Mathf.RoundToInt(position.z / Engine.ChunkScale.z) / Engine.ChunkSideLength);
			return ChunkManager.GetChunk(chunkIndex);

		}

		public static VoxelInfo PositionToVoxelInfo(Vector3 position) {
			GameObject chunkObject = Engine.PositionToChunk(position);
			if (chunkObject != null) {
				Chunk chunk = chunkObject.GetComponent<Chunk>();
				Index voxelIndex = chunk.PositionToVoxelIndex(position);
				return new VoxelInfo(voxelIndex, chunk);
			}
			else {
				return null;
			}

		}

		public static Vector3 VoxelInfoToPosition(VoxelInfo voxelInfo) {
			return voxelInfo.chunk.GetComponent<Chunk>().VoxelIndexToPosition(voxelInfo.index);
		}




		// ==== 创建mesh ====

		public static Vector2 GetTextureOffset(ushort voxel, Facing facing) {

			Voxel voxelType = Engine.GetVoxelType(voxel);
			Vector2[] textureArray = voxelType.VTexture;

			//若无贴图，则使用默认贴图
			if (textureArray.Length == 0) {
				Debug.LogWarning("CubeMaster: Block " + voxel.ToString() + " has no defined textures! Using default texture.");
				return new Vector2(0, 0);
			}
			else if (voxelType.VCustomSides == false) {
				return textureArray[0];
			}
			else if ((int)facing > textureArray.Length - 1) {
				return textureArray[textureArray.Length - 1];
			}
			else {
				return textureArray[(int)facing];
			}
		}


	}

}