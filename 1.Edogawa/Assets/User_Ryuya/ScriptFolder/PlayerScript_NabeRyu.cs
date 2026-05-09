using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript_NabeRyu : MonoBehaviour, ICharacter
{
    [Header("移動・ジャンプ設定")]
    [SerializeField] public float movespeed = 5f;
    [SerializeField] public float jumpspeed = 10f;

    private Vector2 _inputDirection;
    private Rigidbody2D rigid;
    private bool isGrounded = true;
    private int jumpCount = 0;
    [SerializeField] private int maxJumps = 2;

    [Header("UI / アイコン")]
    private TextMeshProUGUI scoreText;
    [SerializeField] private Sprite icon;
    public int score = 0;
    public int playerID;

    public string Name => "Player";
    public Sprite Icon => icon;
    public int Score { get => score; set => score = value; }

    [Header("鬼ごっこ状態")]
    [SerializeField] private int downScore = 1;
    public bool isOni = false;
    [SerializeField] private float tagCooldown = 5f;
    private float lastTagTime = -999f;
    private Coroutine oniCoroutine;
    [SerializeField] private GameObject childKingyo;
    [SerializeField] private GameObject youTextObject;
    [SerializeField] private Animator playerAnim;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        scoreText = GameObject.Find("PlayerScoreText").GetComponent<TextMeshProUGUI>();
        UpdateScoreText();
    }

    void FixedUpdate()
    {
        _move();
    }


    private void _move()
    {
        // �ｿｽ�ｿｽ�ｿｽﾍゑｿｽ�ｿｽ�ｿｽ�ｿｽ驍ｩ�ｿｽﾇゑｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽ`�ｿｽF�ｿｽb�ｿｽN
        if (_inputDirection.x != 0)
        {
            playerAnim.SetBool("IsRun", true);
        }
        else
        {
            playerAnim.SetBool("IsRun", false);
        }

        // �ｿｽ�ｿｽ�ｿｽﾛの移難ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽ
        rigid.linearVelocity = new Vector2(_inputDirection.x * movespeed, rigid.linearVelocity.y);

        // �ｿｽ�ｿｽ�ｿｽE�ｿｽ�ｿｽ�ｿｽ]�ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽi�ｿｽL�ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽﾌ鯉ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽﾏゑｿｽ�ｿｽ�ｿｽj
        if (_inputDirection.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            youTextObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (_inputDirection.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            youTextObject.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void _onmove(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>();
    }

    public void onjump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        playerAnim.SetBool("IsJump", true);
        // �ｿｽ�ｿｽ�ｿｽW�ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽv�ｿｽ柏�ｿｽ�ｿｽ�ｿｽ
        if (jumpCount < maxJumps)
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0f); // �ｿｽ繽ｸ�ｿｽ�ｿｽ�ｿｽﾉゑｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽx�ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽﾆゑｿｽ�ｿｽﾌ具ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽ�ｿｽ濶ｻ
            rigid.AddForce(Vector2.up * jumpspeed, ForceMode2D.Impulse);
            jumpCount++;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Platform"))
        {
            jumpCount = 0;
            isGrounded = true;
            playerAnim.SetBool("IsJump", false);
        }

        // ======== プレイヤー/NPC同士のぶつかり処理 ========
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D otherRb = collision.collider.GetComponent<Rigidbody2D>();
            if (otherRb != null)
            {
                Vector2 relativeVelocity = rigid.linearVelocity - otherRb.linearVelocity;
                float mySpeed = rigid.linearVelocity.magnitude;
                float otherSpeed = otherRb.linearVelocity.magnitude;

                // 遅い方を吹き飛ばす（スマブラ風）
                if (mySpeed < otherSpeed)
                {
                    Vector2 dir = (rigid.position - otherRb.position).normalized;
                    rigid.AddForce(dir * 8f, ForceMode2D.Impulse);
                }
                else
                {
                    Vector2 dir = (otherRb.position - rigid.position).normalized;
                    otherRb.AddForce(dir * 8f, ForceMode2D.Impulse);
                }
            }
        }

        // ======== 鬼ごっこ判定 ========
        if (!isOni) return;
        if (Time.time - lastTagTime < tagCooldown) return;

        ICharacter target = collision.collider.GetComponent<ICharacter>();
        if (target != null && !target.IsOni())
        {
            target.SetOni(true);
            this.SetOni(false);
            lastTagTime = Time.time;
            Debug.Log($"{Name} が {target.Name} に鬼を渡した！");
        }
    }

    private IEnumerator OniScoreDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Score -= downScore;
            if (Score < 0) Score = 0;
            scoreText.text = Score.ToString();
        }
    }

    public void SetOni(bool value)
    {
        isOni = value;
        if (value)
        {
            childKingyo.SetActive(true);
            lastTagTime = Time.time;
            if (oniCoroutine == null)
                oniCoroutine = StartCoroutine(OniScoreDown());
        }
        else
        {
            childKingyo.SetActive(false);
            if (oniCoroutine != null)
            {
                StopCoroutine(oniCoroutine);
                oniCoroutine = null;
            }
        }
    }

    public bool IsOni() => isOni;

    public void ApplyScoreEffect(float value)
    {
        score += (int)value;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    public void ApplySpeedEffect(float value, float duration)
    {
        StartCoroutine(SpeedEffectCoroutine(value, duration));
    }

    private IEnumerator SpeedEffectCoroutine(float value, float duration)
    {
        movespeed += value;
        yield return new WaitForSeconds(duration);
        movespeed -= value;
    }
}
