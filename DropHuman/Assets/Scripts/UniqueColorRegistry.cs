using System.Collections.Generic;
using UnityEngine;

public static class UniqueColorRegistry
{
	private static readonly Dictionary<int, Queue<Material>> soIdToMaterialQueue = new Dictionary<int, Queue<Material>>();
	private static readonly Dictionary<int, int> soIdToAssignedCount = new Dictionary<int, int>();
    private static readonly HashSet<int> warnedOverflowSoIds = new HashSet<int>();

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
        // Warn once per SO when overflow happens.
        if (soIdToAssignedCount[soId] >= colorSo.blockColors.Count && !warnedOverflowSoIds.Contains(soId))
		{
            Debug.LogWarning("UniqueColorRegistry: More blocks than available unique materials. Colors will repeat.");
            warnedOverflowSoIds.Add(soId);
		}

        if (queue.Count == 0)
		{
            // Refill with a reshuffled order to avoid repeating patterns
            List<Material> reshuffled = new List<Material>(colorSo.blockColors);
            for (int i = reshuffled.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                Material tmp = reshuffled[i];
                reshuffled[i] = reshuffled[j];
                reshuffled[j] = tmp;
            }
            foreach (Material m in reshuffled)
            {
                queue.Enqueue(m);
            }
		}

		Material selected = queue.Dequeue();
		soIdToAssignedCount[soId]++;
		return selected;
	}

    public static void Reset(ColorMaterialsSO colorSo)
    {
        if (colorSo == null) return;
        int soId = colorSo.GetInstanceID();
        soIdToMaterialQueue.Remove(soId);
        soIdToAssignedCount.Remove(soId);
        warnedOverflowSoIds.Remove(soId);
    }

    public static void ResetAll()
    {
        soIdToMaterialQueue.Clear();
        soIdToAssignedCount.Clear();
        warnedOverflowSoIds.Clear();
    }
}

