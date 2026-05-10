using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    public GameObject cardPrefab;
    public List<SoundGroup> soundGroups;

    public int pairCount;

    private List<SoundCard> cards = new List<SoundCard>();

    void Start()
    {
        GenerateCards();
    }

    void GenerateCards()
    {
        List<SoundGroup> shuffled = new List<SoundGroup>(soundGroups);

        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand = Random.Range(i, shuffled.Count);
            var temp = shuffled[i];
            shuffled[i] = shuffled[rand];
            shuffled[rand] = temp;
        }

        for (int i = 0; i < pairCount; i++)
        {
            SoundGroup group = shuffled[i];

            int index1 = Random.Range(0, group.clips.Count);
            int index2;

            do
            {
                index2 = Random.Range(0, group.clips.Count);
            }
            while (index1 == index2);

            CreateCard(group.clips[index1], i, group.name);
            CreateCard(group.clips[index2], i, group.name);
        }

        ShuffleCards();
        ArrangeCards();
    }

    void CreateCard(AudioClip clip, int id, string groupName)
    {
        GameObject obj = Instantiate(cardPrefab);

        SoundCard card = obj.GetComponent<SoundCard>();

        card.clip = clip;
        card.categoryID = id;
        card.categoryName = groupName;

        cards.Add(card);
    }

    void ShuffleCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int rand = Random.Range(i, cards.Count);
            var temp = cards[i];
            cards[i] = cards[rand];
            cards[rand] = temp;
        }
    }

    void ArrangeCards()
    {
        int total = cards.Count;

        // 列数（自動）
        int cols = Mathf.CeilToInt(Mathf.Sqrt(total));
        int rows = Mathf.CeilToInt((float)total / cols);

        float spacingX = 2.0f;
        float spacingY = 2.5f;

        // 画面中央（ワールド座標）
        Camera cam = Camera.main;

        Vector3 screenCenter = new Vector3(
            Screen.width / 2f,
            Screen.height / 2f,
            Mathf.Abs(cam.transform.position.z)
        );

        Vector3 centerPos = cam.ScreenToWorldPoint(screenCenter);
        centerPos.z = 0f;

        // 全体サイズ
        float width = (cols - 1) * spacingX;
        float height = (rows - 1) * spacingY;

        for (int i = 0; i < total; i++)
        {
            int x = i % cols;
            int y = i / cols;

            float posX = centerPos.x + (x * spacingX - width / 2f);
            float posY = centerPos.y + (-y * spacingY + height / 2f);

            cards[i].transform.position = new Vector3(posX, posY, 0);
        }
    }
}