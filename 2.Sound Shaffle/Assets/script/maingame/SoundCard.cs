using UnityEngine;
using System.Collections;

public class SoundCard : MonoBehaviour
{
    public int categoryID;
    public string categoryName;
    public AudioClip clip;

    public Sprite normalSprite;
    public Sprite selectSprite;

    private SpriteRenderer sr;
    private AudioSource audioSource;
    private GameManager gameManager;

    private bool isOpened = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        gameManager = FindObjectOfType<GameManager>();

        sr.sprite = normalSprite;
    }

    void OnMouseDown()
    {
        if (gameManager == null) return;

        // 設定画面中は何もしない
        if (gameManager.isUIOpen) return;

        // ペア判定中
        if (gameManager.isChecking) return;

        if (isOpened) return;

        Debug.Log("カードクリック ID:" + categoryID);

        audioSource.PlayOneShot(clip);

        StartCoroutine(StopSoundAfterSeconds(2f));

        gameManager.OnCardClicked(this);
    }

    public void Open()
    {
        isOpened = true;
        sr.sprite = selectSprite;
    }

    public void Close()
    {
        isOpened = false;
        sr.sprite = normalSprite;
    }
    IEnumerator StopSoundAfterSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);

        audioSource.Stop();
    }
}