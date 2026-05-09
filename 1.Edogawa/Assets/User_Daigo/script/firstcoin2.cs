using UnityEngine;

public class firstCoin2 : MonoBehaviour
{
    public int scoreValue = 2;       // æ‚Á‚½‚É‘‚¦‚é“_”
    public float rotateSpeed = 100f;  // ‰ñ“]‘¬“x
    public float floatAmplitude = 0.1f; // ã‰º‚Ì•
    public float floatFrequency = 2f;   // ã‰º‚Ì‘¬‚³
    public Transform player;

    private Vector2 startPos;

    public AudioClip pickupSE;   // æ“¾‚ÌŒø‰Ê‰¹‚ğInspector‚Åİ’è
    private AudioSource audioSource;

    void Start()
    {
        // ‰ŠúˆÊ’u‚ğ‹L˜^
        startPos = transform.position;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public int GetScore()
    {
        return scoreValue;
    }

    void Update()
    {
        // ‰ñ“]
        //transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);

        // ã‰º‚É‚Ó‚í‚Ó‚íˆÚ“®
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector2(startPos.x, newY);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ICharacter character = other.GetComponent<ICharacter>();
        if (character != null)
        {
            character.ApplyScoreEffect(scoreValue);
            SEManager.Instance.PlaySE(pickupSE);
            Destroy(gameObject);
        }
    }
}

