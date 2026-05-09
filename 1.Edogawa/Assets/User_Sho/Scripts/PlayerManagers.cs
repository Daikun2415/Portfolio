using System.Collections.Generic;
using UnityEngine;

public class PlayerManagers : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject npcPrefab;

    public Vector2 playerSpawnPos;
    public Vector2[] npcSpawnPositions = new Vector2[3];

    public static PlayerManagers Instance;

    [SerializeField] private RuntimeAnimatorController[] npcAnimControllers;
    [SerializeField] private Sprite[] iconSprites;

    private PlayerScript_Sho player;
    private List<NPCScript_Sho> npcs = new List<NPCScript_Sho>();
    public ICharacter CurrentOni { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnPlayer();
        SpawnNPCs();
        DecideOni(); // �� �����ŋS������
    }

    void SpawnPlayer()
    {
        GameObject playerObj = Instantiate(playerPrefab, playerSpawnPos, playerPrefab.transform.rotation);
        player = playerObj.GetComponent<PlayerScript_Sho>();
        player.playerID = 1;
        Debug.Log($"�v���C���[����: {player.name}");
    }

    void SpawnNPCs()
    {
        for (int i = 0; i < npcSpawnPositions.Length; i++)
        {
            GameObject prefab = npcPrefab;
            prefab.GetComponent<Animator>().runtimeAnimatorController = npcAnimControllers[i];
            GameObject npcObj = Instantiate(prefab, npcSpawnPositions[i], prefab.transform.rotation);
            NPCScript_Sho npc = npcObj.GetComponent<NPCScript_Sho>();
            npc.npcID = i + 1; // NPC ID��1�`3
            npcs.Add(npc);
            npc.icon = iconSprites[i];
            Debug.Log($"NPC����: {npc.name}");
        }
    }

    // ===== �S�����߂鏈����ǉ� =====
    void DecideOni()
    {
        // �v���C���[��NPC�̂����ꂩ�������_���ŋS�ɂ���
        int total = 1 + npcs.Count; // �v���C���[1�l + NPC
        int randomIndex = Random.Range(0, total);

        if (randomIndex == 0)
        {
            player.SetOni(true);
            Debug.Log("�v���C���[���S�ɂȂ����I");
        }
        else
        {
            NPCScript_Sho npc = npcs[randomIndex - 1];
            npc.SetOni(true);
            Debug.Log($"{npc.name} ���S�ɂȂ����I");
        }
    }

    

    public PlayerScript_Sho GetPlayer()
    {
        return player;
    }

    public List<NPCScript_Sho> GetAllNPCs()
    {
        return new List<NPCScript_Sho>(npcs);
    }

    public NPCScript_Sho GetRandomNPC()
    {
        if (npcs.Count == 0) return null;
        return npcs[Random.Range(0, npcs.Count)];
    }
}
