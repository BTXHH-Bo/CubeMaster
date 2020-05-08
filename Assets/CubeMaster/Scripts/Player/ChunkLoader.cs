using UnityEngine;
using System.Collections;

// 加载玩家周围的chunk

namespace CubeMaster
{

	public class ChunkLoader : MonoBehaviour
	{

		private Index LastPos;
		private Index currentPos;

		void Awake()
		{
		}

		public void Update()
		{

			// 未初始化则不创建（Engine要最先初始化）
			if (!Engine.Initialized || !ChunkManager.Initialized)
			{
				return;
			}

			//追踪player当前所处的chunk
			currentPos = Engine.PositionToChunkIndex(transform.position);

			if (currentPos.IsEqual(LastPos) == false)
			{
				ChunkManager.SpawnChunks(currentPos.x, currentPos.y, currentPos.z);
			}

			LastPos = currentPos;
		}

		IEnumerator InitialPositionAndRangeUpdate()
		{
			yield return new WaitForEndOfFrame();

		}
	}

}