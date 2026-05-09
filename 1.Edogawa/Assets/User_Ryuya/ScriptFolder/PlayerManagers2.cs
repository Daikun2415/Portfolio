using System.Collections.Generic;
using UnityEngine;

public class PlayerManagers2 : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject npcPrefab;

    public Vector2 playerSpawnPos;
    public Vector2[] npcSpawnPositions = new Vector2[3];

    public static PlayerManagers2 Instance;

    [SerializeField] private RuntimeAnimatorController[] npcAnimControllers;
    [SerializeField] private Sprite[] iconSprites;

    private PlayerScript_NabeRyu player;
    private List<NPCScript_NabeRyu> npcs = new List<NPCScript_NabeRyu>();
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
        player = playerObj.GetComponent<PlayerScript_NabeRyu>();
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
            NPCScript_NabeRyu npc = npcObj.GetComponent<NPCScript_NabeRyu>();
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
            NPCScript_NabeRyu npc = npcs[randomIndex - 1];
            npc.SetOni(true);
            Debug.Log($"{npc.name} ���S�ɂȂ����I");
        }
    }

    

    public PlayerScript_NabeRyu GetPlayer()
    {
        return player;
    }

    public List<NPCScript_NabeRyu> GetAllNPCs()
    {
        return new List<NPCScript_NabeRyu>(npcs);
    }

    public NPCScript_NabeRyu GetRandomNPC()
    {
        if (npcs.Count == 0) return null;
        return npcs[Random.Range(0, npcs.Count)];
    }
}
