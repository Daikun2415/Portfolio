using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    /*
    public GameObject playerPrefab;
    public int playerCount = 4;

    // �l�����Ƃ̃X�|�[�����W���X�g�i2D�Ȃ̂� z=0�j
    public Vector2[] spawnPositions2Players = new Vector2[2];
    public Vector2[] spawnPositions3Players = new Vector2[3];
    public Vector2[] spawnPositions4Players = new Vector2[4];

    public static PlayerManager Instance;

    private List<PlayerScript_Sho> players = new List<PlayerScript_Sho>();

    void Awake()
    {
        Instance = this; // Start �ł͂Ȃ� Awake �ŃZ�b�g
    }


    void Start()
    {
        //SpawnPlayers();
        foreach (var p in PlayerManagers.Instance.GetAllPlayers())
            Debug.Log($"�o�^�v���C���[: {p.name}, score={p.score}");

    }

    void SpawnPlayers()
    {
        Vector2[] positions;

        // �l���ɉ����č��W�z���I��
        switch (playerCount)
        {
            case 2:
                positions = spawnPositions2Players;
                break;
            case 3:
                positions = spawnPositions3Players;
                break;
            case 4:
                positions = spawnPositions4Players;
                break;
            default:
                Debug.LogError("�Ή����Ă��Ȃ��l���ł�");
                return;
        }

        for (int i = 0; i < playerCount; i++)
        {
            Vector2 spawnPos = positions[i];
            GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

            PlayerScript_Sho status = playerObj.GetComponent<PlayerScript_Sho>();
            status.playerID = i + 1;

            players.Add(status);
            Debug.Log($"Player {status.playerID} ���o��: {spawnPos}");
        }
    }

    public void Register(PlayerScript_Sho player)
    {
        if (!players.Contains(player)) players.Add(player);
    }

    public void Unregister(PlayerScript_Sho player)
    {
        if (players.Contains(player)) players.Remove(player);
    }

    public List<PlayerScript_Sho> GetAllPlayers()
    {
        return new List<PlayerScript_Sho>(players); // �R�s�[���ĕԂ�
    }

    public PlayerScript_Sho GetRandomOtherPlayer(PlayerScript_Sho exclude)
    {
        var others = players.FindAll(p => p != exclude);
        if (others.Count == 0) return null;
        return others[Random.Range(0, others.Count)];
    }
    */
}
