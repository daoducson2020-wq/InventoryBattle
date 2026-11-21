using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] private TMP_Text enemyNameText; // Kéo object EnemyName (TMP) trong scene vào đây
    // Danh sách quái (child trong prefab hoặc prefab asset), mỗi quái có MobController với mobType đã set
    [SerializeField] private GameObject[] mobs;
    [SerializeField] private Transform spawnPoint; // Dùng khi mob là prefab asset
    public TMP_Text EnemyName => enemyNameText;
    
    
    private void Start()
    {
        int floor = GameSession.Instance != null ? GameSession.Instance.CurrentFloor : 0;
        SpawnForFloor(floor);
    }

    // Tầng 0-4: begin, 5-8: middle, 9-12: high
    public static MobType GetMobTypeForFloor(int floor)
    {
        if (floor <= 4) return MobType.begin;
        if (floor <= 8) return MobType.middle;
        return MobType.high;
    }

    public void SpawnForFloor(int floor)
    {
        MobType type = GetMobTypeForFloor(floor);

        // Tắt toàn bộ quái nằm sẵn trong prefab
        foreach (var m in mobs)
            if (m != null && m.scene.IsValid()) m.SetActive(false);

        List<GameObject> candidates = new();
        foreach (var m in mobs)
        {
            if (m == null) continue;
            MobController mc = m.GetComponentInChildren<MobController>(true);
            if (mc != null && mc.mobType == type) candidates.Add(m);
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"MobSpawner: không có quái MobType.{type} cho tầng {floor}");
            return;
        }

        GameObject chosen = candidates[Random.Range(0, candidates.Count)];
        if (chosen.scene.IsValid())
            chosen.SetActive(true);
        else
            Instantiate(chosen, spawnPoint != null ? spawnPoint.position : transform.position,
                        Quaternion.identity, transform);

        MobController chosenMob = chosen.GetComponentInChildren<MobController>(true);
        if (enemyNameText != null && chosenMob != null)
            enemyNameText.text = chosenMob.mobName;
    }
}
