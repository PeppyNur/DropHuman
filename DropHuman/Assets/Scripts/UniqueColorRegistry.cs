using System.Collections.Generic;
using UnityEngine;

public static class UniqueColorRegistry
{
	private static readonly Dictionary<int, Queue<Material>> soIdToMaterialQueue = new Dictionary<int, Queue<Material>>();
	private static readonly Dictionary<int, int> soIdToAssignedCount = new Dictionary<int, int>();

	public static Material GetUniqueMaterial(ColorMaterialsSO colorSo)
	{
		if (colorSo == null || colorSo.blockColors == null || colorSo.blockColors.Count == 0)
		{
			Debug.LogWarning("UniqueColorRegistry: Color SO or materials list is empty. Returning null material.");
			return null;
		}

		int soId = colorSo.GetInstanceID();

		if (!soIdToMaterialQueue.TryGetValue(soId, out Queue<Material> queue) || queue == null || queue.Count == 0)
		{
			// Initialize or refill a shuffled queue
			List<Material> shuffled = new List<Material>(colorSo.blockColors);
			for (int i = shuffled.Count - 1; i > 0; i--)
			{
				int j = Random.Range(0, i + 1);
				Material tmp = shuffled[i];
				shuffled[i] = shuffled[j];
				shuffled[j] = tmp;
			}

			queue = new Queue<Material>(shuffled);
			soIdToMaterialQueue[soId] = queue;
		}

		if (!soIdToAssignedCount.ContainsKey(soId))
		{
			soIdToAssignedCount[soId] = 0;
		}

		// If we have already assigned more blocks than materials, duplicates are unavoidable.
		// We still cycle deterministically while warning once when overflow happens.
		if (soIdToAssignedCount[soId] >= colorSo.blockColors.Count && soIdToAssignedCount[soId] % colorSo.blockColors.Count == 0)
		{
			Debug.LogWarning("UniqueColorRegistry: More blocks than available unique materials. Colors will repeat.");
		}

		if (queue.Count == 0)
		{
			// Refill to continue cycling when there are more blocks than materials
			foreach (Material m in colorSo.blockColors)
			{
				queue.Enqueue(m);
			}
		}

		Material selected = queue.Dequeue();
		soIdToAssignedCount[soId]++;
		return selected;
	}
}

