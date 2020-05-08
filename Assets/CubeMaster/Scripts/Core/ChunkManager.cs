

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

// 控制chunk的销毁与延申创建

namespace CubeMaster {

	public class ChunkManager : MonoBehaviour
	{

		public GameObject ChunkObject; // Chunk prefab

		// chunk们
		public static Dictionary<string, Chunk> Chunks;
		private static List<Chunk> ChunkUpdateQueue; //按照更新的优先级存储chunk们，用于ProcessChunkQueue
		private static List<Chunk> ChunksToDestroy; // 延申后需要被销绘的chunks

		public static int SavesThisFrame;


		// global flags
		public static bool SpawningChunks; // 正在spawning
		public static bool StopSpawning;
		public static bool Initialized;

		// local flags	
		private bool Done;
		private Index LastRequest;
		private float targetFrameDuration;
		private Stopwatch frameStopwatch;
		private int SpawnQueue;

		void Start()
		{
			targetFrameDuration = 1f / Engine.TargetFPS;

			ChunkManager.Chunks = new Dictionary<string, Chunk>();
			ChunkManager.ChunkUpdateQueue = new List<Chunk>();
			frameStopwatch = new Stopwatch();

			Engine.ChunkScale = ChunkObject.transform.localScale;
			ChunkObject.GetComponent<Chunk>().MeshContainer.transform.localScale = ChunkObject.transform.localScale;
			ChunkObject.GetComponent<Chunk>().ChunkCollider.transform.localScale = ChunkObject.transform.localScale;

			Done = true;
			ChunkManager.SpawningChunks = false;

			ChunkManager.Initialized = true;
		}

		private void ResetFrameStopwatch()
		{
			frameStopwatch.Stop();
			frameStopwatch.Reset();
			frameStopwatch.Start();
		}

		public static void AddChunkToUpdateQueue(Chunk chunk)
		{
			if (ChunkUpdateQueue.Contains(chunk) == false)
			{
				ChunkUpdateQueue.Add(chunk);
			}
		}

		private void ProcessChunkQueue()
		{ // Spawnchunks调用，更新chunk mesh

			// 更新第一个chunk，然后从队列中删除
			Chunk currentChunk = ChunkUpdateQueue[0];

			if (!currentChunk.Empty && !currentChunk.DisableMesh)
			{
				currentChunk.RebuildMesh();
			}
			currentChunk.Fresh = false;
			ChunkUpdateQueue.RemoveAt(0);
		}

		private bool ProcessChunkQueueLoopActive;
		private IEnumerator ProcessChunkQueueLoop()
		{ // 当SpawnChunks没有运行时，在Update中调用
			ProcessChunkQueueLoopActive = true;
			while (ChunkUpdateQueue.Count > 0 && !SpawningChunks && !StopSpawning)
			{
				ProcessChunkQueue();
				if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
				{
					yield return new WaitForEndOfFrame();
				}
			}
			ProcessChunkQueueLoopActive = false;
		}

		public static void RegisterChunk(Chunk chunk)
		{ //加入到全局的chunk列表中
			ChunkManager.Chunks.Add(chunk.ChunkIndex.ToString(), chunk);
		}

		public static void UnregisterChunk(Chunk chunk)
		{
			ChunkManager.Chunks.Remove(chunk.ChunkIndex.ToString());
		}

		public static GameObject GetChunk(int x, int y, int z)
		{ //返回这个坐标下的chunkIndex或者null；

			return GetChunk(new Index(x, y, z));
		}

		public static GameObject GetChunk(Index index)
		{//咱也不知道写这个后面要不要用，先写了吧，不嫌代码多

			Chunk chunk = ChunkManager.GetChunkComponent(index);
			if (chunk == null)
			{
				return null;
			}
			else
			{
				return chunk.gameObject;
			}

		}

		public static Chunk GetChunkComponent(int x, int y, int z)
		{

			return GetChunkComponent(new Index(x, y, z));
		}

		public static Chunk GetChunkComponent(Index index)
		{

			string indexString = index.ToString();
			if (ChunkManager.Chunks.ContainsKey(indexString))
			{
				return ChunkManager.Chunks[indexString];
			}
			else
			{
				return null;
			}
		}


		// ==== 延申chunk们 ====

		public static GameObject SpawnChunk(int x, int y, int z)
		{ // spawn一个单独的chunk

			GameObject chunk = ChunkManager.GetChunk(x, y, z);
			if (chunk == null)
			{
				return Engine.ChunkManagerInstance.DoSpawnChunk(new Index(x, y, z));
			}
			else return chunk;
		}

		public static GameObject SpawnChunk(Index index)
		{

			GameObject chunk = ChunkManager.GetChunk(index);
			if (chunk == null)
			{
				return Engine.ChunkManagerInstance.DoSpawnChunk(index);
			}
			else return chunk;
		}


		GameObject DoSpawnChunk(Index index)
		{
			GameObject chunkObject = Instantiate(ChunkObject, index.ToVector3(), transform.rotation) as GameObject;
			Chunk chunk = chunkObject.GetComponent<Chunk>();
			AddChunkToUpdateQueue(chunk);
			return chunkObject;
		}

		public static void SpawnChunks(float x, float y, float z)
		{ // 通过世界坐标
			Index index = Engine.PositionToChunkIndex(new Vector3(x, y, z));
			Engine.ChunkManagerInstance.TrySpawnChunks(index);
		}
		public static void SpawnChunks(Vector3 position)
		{//不嫌代码多
			Index index = Engine.PositionToChunkIndex(position);
			Engine.ChunkManagerInstance.TrySpawnChunks(index);
		}

		public static void SpawnChunks(int x, int y, int z)
		{ //不嫌代码多2，扩展的时候可能用到反正
			Engine.ChunkManagerInstance.TrySpawnChunks(x, y, z);
		}
		public static void SpawnChunks(Index index)
		{
			Engine.ChunkManagerInstance.TrySpawnChunks(index.x, index.y, index.z);
		}

		private void TrySpawnChunks(Index index)
		{
			TrySpawnChunks(index.x, index.y, index.z);
		}
		private void TrySpawnChunks(int x, int y, int z)
		{//这个是参考的

			if (Done == true)
			{ // 正常的spawn
				StartSpawnChunks(x, y, z);
			}
			else
			{ // if we are spawning chunks already, flag to spawn again once the previous round is finished using the last requested position as origin.
				LastRequest = new Index(x, y, z);
				SpawnQueue = 1;
				StopSpawning = true;
				ChunkUpdateQueue.Clear();
			}
		}






		public void Update()
		{

			//如果已经有一个chunk spawn queue准备好了，并且前一个也ok了
			if (SpawnQueue == 1 && Done == true)
			{
				SpawnQueue = 0;
				StartSpawnChunks(LastRequest.x, LastRequest.y, LastRequest.z);
			}

			// 如果现在没有spawn的，就处理一个已经排好队的chunk们
			if (!SpawningChunks && !ProcessChunkQueueLoopActive && ChunkUpdateQueue != null && ChunkUpdateQueue.Count > 0)
			{
				StartCoroutine(ProcessChunkQueueLoop());
			}

			ResetFrameStopwatch();
		}


		private void StartSpawnChunks(int originX, int originY, int originZ)
		{

			ChunkManager.SpawningChunks = true;
			Done = false;

			int range = Engine.ChunkSpawnDistance;

			StartCoroutine(SpawnMissingChunks(originX, originY, originZ, range));

		}

		//有借鉴参考
		private IEnumerator SpawnMissingChunks(int originX, int originY, int originZ, int range)
		{

			int heightRange = Engine.HeightRange;

			//清空更新队列updatequeue
			ChunkUpdateQueue = new List<Chunk>();

			//flag chunk不在范围内
			ChunksToDestroy = new List<Chunk>();
			foreach (Chunk chunk in Chunks.Values)
			{
				if (Vector2.Distance(new Vector2(chunk.ChunkIndex.x, chunk.ChunkIndex.z), new Vector2(originX, originZ)) > range + Engine.ChunkDespawnDistance)
				{
					ChunksToDestroy.Add(chunk);
				}
				else if (Mathf.Abs(chunk.ChunkIndex.y - originY) > range + Engine.ChunkDespawnDistance)
				{
					ChunksToDestroy.Add(chunk);
				}
			}


			// main loop==看不懂了，原注释保留
			for (int currentLoop = 0; currentLoop <= range; currentLoop++)
			{
				for (var x = originX - currentLoop; x <= originX + currentLoop; x++)
				{ // iterate through all potential chunk indexes within range
					for (var y = originY - currentLoop; y <= originY + currentLoop; y++)
					{
						for (var z = originZ - currentLoop; z <= originZ + currentLoop; z++)
						{

							if (Mathf.Abs(y) <= heightRange)
							{ // skip chunks outside of height range
								if (Mathf.Abs(originX - x) + Mathf.Abs(originZ - z) < range * 1.3f)
								{ // skip corners

									// pause loop while the queue is not empty
									while (ChunkUpdateQueue.Count > 0)
									{
										ProcessChunkQueue();
										if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
										{
											yield return new WaitForEndOfFrame();
										}
									}

									Chunk currentChunk = ChunkManager.GetChunkComponent(x, y, z);


									// chunks that already exist but haven't had their mesh built yet should be added to the update queue
									if (currentChunk != null)
									{

										// chunks without meshes spawned by server should be changed to regular chunks
										if (currentChunk.DisableMesh || currentChunk.EnableTimeout)
										{
											currentChunk.DisableMesh = false;
											currentChunk.EnableTimeout = false;
											currentChunk.Fresh = true;
										}

										if (currentChunk.Fresh)
										{

											// spawn neighbor chunks
											for (int d = 0; d < 6; d++)
											{
												Index neighborIndex = currentChunk.ChunkIndex.GetAdjacentIndex((Direction)d);
												GameObject neighborChunk = GetChunk(neighborIndex);
												if (neighborChunk == null)
												{
													neighborChunk = Instantiate(ChunkObject, neighborIndex.ToVector3(), transform.rotation) as GameObject;
												}
												currentChunk.NeighborChunks[d] = neighborChunk.GetComponent<Chunk>(); // always add the neighbor to NeighborChunks, in case it's not there already

												// continue loop in next frame if the current frame time is exceeded
												if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
												{
													yield return new WaitForEndOfFrame();
												}
												if (StopSpawning)
												{
													EndSequence();
													yield break;
												}
											}

											if (currentChunk != null)
												currentChunk.AddToQueueWhenReady();
										}


									}

									else
									{ // if chunk doesn't exist, create new chunk (it adds itself to the update queue when its data is ready)

										// spawn chunk
										GameObject newChunk = Instantiate(ChunkObject, new Vector3(x, y, z), transform.rotation) as GameObject; // Spawn a new chunk.
										currentChunk = newChunk.GetComponent<Chunk>();

										// spawn neighbor chunks if they're not spawned yet
										for (int d = 0; d < 6; d++)
										{
											Index neighborIndex = currentChunk.ChunkIndex.GetAdjacentIndex((Direction)d);
											GameObject neighborChunk = GetChunk(neighborIndex);
											if (neighborChunk == null)
											{
												neighborChunk = Instantiate(ChunkObject, neighborIndex.ToVector3(), transform.rotation) as GameObject;
											}
											currentChunk.NeighborChunks[d] = neighborChunk.GetComponent<Chunk>(); // always add the neighbor to NeighborChunks, in case it's not there already

											// continue loop in next frame if the current frame time is exceeded
											if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
											{
												yield return new WaitForEndOfFrame();
											}
											if (StopSpawning)
											{
												EndSequence();
												yield break;
											}
										}

										if (currentChunk != null)
											currentChunk.AddToQueueWhenReady();



									}

								}
							}



							// continue loop in next frame if the current frame time is exceeded
							if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
							{
								yield return new WaitForEndOfFrame();
							}
							if (StopSpawning)
							{
								EndSequence();
								yield break;
							}


						}
					}
				}
			}

			yield return new WaitForEndOfFrame();
			EndSequence();
		}





		private void EndSequence()
		{

			ChunkManager.SpawningChunks = false;
			Resources.UnloadUnusedAssets();
			Done = true;
			StopSpawning = false;

			foreach (Chunk chunk in ChunksToDestroy)
			{
				chunk.FlagToRemove();
			}

		}

	}

}

