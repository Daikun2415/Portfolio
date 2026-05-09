using System.Collections;
using System.Collections.Generic;
using System.Linq; // Linqを使う
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.TextCore.Text;

public class NPCScript_Sho : MonoBehaviour, ICharacter
{
    [Header("移動・ジャンプ設定")]
    public float moveSpeed = 3f;
    public float jumpForce = 5f;

    private Rigidbody2D rb;
    private Vector2 inputDir;
    private bool isGrounded;
    public int npcID;
    [SerializeField] private TextMeshProUGUI scoreText;
    public Sprite icon;
    public Animator myAnim;
    [SerializeField] private GameObject childKingyo;

    [Header("アイテム探索")]
    public float detectRange = 10f;  // 探索範囲
    public LayerMask itemLayer;

    [Header("ジャンプ設定")]
    [SerializeField] private int maxJumps = 2;
    private int jumpCount = 0;

    private Transform targetItem;

    // ICharacter インターフェース
    public string Name => $"NPC{npcID}";
    public Sprite Icon => icon;
    public int Score { get; set; }

    [Header("鬼ごっこ状態")]
    [SerializeField] private int downScore;
    public bool isOni = false;
    [SerializeField] private float tagCooldown = 5f;
    private float lastTagTime = -999f;
    private Coroutine oniCoroutine;

    [Header("スタック判定")]
    [SerializeField] private float stuckCheckInterval = 0.5f; // チェック間隔
    [SerializeField] private float stuckThreshold = 0.1f;   // 動いたと判定する最小距離
    private Vector2 lastPosition;
    private float lastMoveCheckTime;
    [Header("スタック回避設定")]
    [SerializeField] private float stuckActionDuration = 1f; // 回避アクション維持時間
    private float stuckActionEndTime = 0f;
    private Vector2 overrideInput = Vector2.zero;

    [Header("鬼ターゲット切り替え設定")]
    [SerializeField] private float targetSwitchTime = 5f; // 同じターゲットを追い続ける最大時間
    private float lastTargetSwitchTime = -999f;

    [Header("アイテムターゲット切り替え設定")]
    [SerializeField] private float itemTargetSwitchTime = 5f; // 同じアイテムを追い続ける最大時間
    private float lastItemTargetSwitchTime = -999f;

    [Header("NPC行動反応設定")]
    [SerializeField] private float inputUpdateInterval = 0.3f; // 入力を更新できる最小間隔
    private float lastInputUpdateTime = 0f;

    private Vector3 initialScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        switch (npcID)
        {
            case 1:
                scoreText = GameObject.Find("NPC1ScoreText").GetComponent<TextMeshProUGUI>();
                break;
            case 2:
                scoreText = GameObject.Find("NPC2ScoreText").GetComponent<TextMeshProUGUI>();
                break;
            case 3:
                scoreText = GameObject.Find("NPC3ScoreText").GetComponent<TextMeshProUGUI>();
                break;
        }
        Score = 0;
        scoreText.text = Score.ToString();
        lastPosition = transform.position;
        lastMoveCheckTime = Time.time;
        myAnim = GetComponent<Animator>();
        Debug.Log("MyAnimator=" + myAnim.name);
        initialScale = transform.localScale; // 現在のスケールを保存
    }

    void Update()
    {
        if (Time.time < stuckActionEndTime)
        {
            // スタック回避中は強制的に overrideInput を使う
            inputDir = overrideInput;
        }
        else
        {
            // 一定間隔でのみ入力を更新
            if (Time.time - lastInputUpdateTime > inputUpdateInterval)
            {
                if (isOni)
                    MoveTowardsRandomCharacter();
                else
                    MoveTowardsItemOrEscape();

                lastInputUpdateTime = Time.time; // 更新時刻を記録
            }
        }
        if (isOni)
        {

        }
        CheckStuck();
        UpdateAnimator();
        UpdateScoreText();
    }

    void FixedUpdate()
    {
        // 水平方向移動
        float targetVelX = inputDir.x * moveSpeed;
        float velChange = targetVelX - rb.linearVelocity.x;
        rb.AddForce(new Vector2(velChange, 0), ForceMode2D.Impulse);

        // ジャンプ
        if (inputDir.y > 0 && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    // -------------------------------
    // 鬼の場合：ランダムキャラを追いかける
    // -------------------------------
    void MoveTowardsRandomCharacter()
    {
        if (targetItem == null || !targetItem.TryGetComponent<ICharacter>(out _) || Time.time - lastTargetSwitchTime > targetSwitchTime)
        {
            var characters = FindObjectsOfType<MonoBehaviour>().OfType<ICharacter>().Where(c => c != this).ToList();
            if (characters.Count > 0)
            {
                var newTarget = characters[Random.Range(0, characters.Count)];
                targetItem = (newTarget as MonoBehaviour).transform;
                lastTargetSwitchTime = Time.time;
                Debug.Log($"{Name} がターゲットを {newTarget.Name} に切り替えた！");
            }
        }

        if (targetItem != null)
        {
            Vector2 dir = (targetItem.position - transform.position).normalized;
            inputDir.x = Mathf.Sign(dir.x);
            inputDir.y = (dir.y > 0.1f && isGrounded) ? 1 : 0;
        }
    }

    // -------------------------------
    // 非鬼の場合：アイテムへ移動 or 鬼から逃げる
    // -------------------------------
    void MoveTowardsItemOrEscape()
    {
        FindClosestItem();
        Vector2 dir = Vector2.zero;

        if (targetItem != null)
            dir = (targetItem.position - transform.position).normalized;

        var oniList = FindObjectsOfType<MonoBehaviour>().OfType<ICharacter>().Where(c => c.IsOni() && c != this).ToList();
        float escapeRange = 3f;
        Transform nearestOni = null;
        float minDist = float.MaxValue;

        foreach (var oni in oniList)
        {
            float dist = Vector2.Distance(transform.position, (oni as MonoBehaviour).transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestOni = (oni as MonoBehaviour).transform;
            }
        }

        if (nearestOni != null && minDist < escapeRange)
        {
            Vector2 escapeDir = (transform.position - nearestOni.position).normalized;
            dir = escapeDir;
        }

        inputDir.x = Mathf.Sign(dir.x);
        inputDir.y = (dir.y > 0.1f && isGrounded) ? 1 : 0;
    }

    void FindClosestItem()
    {
        if (targetItem != null && targetItem.CompareTag("point") && Time.time - lastItemTargetSwitchTime > itemTargetSwitchTime)
        {
            Debug.Log($"{Name} がアイテムターゲットをリセット（長時間取れない）");
            targetItem = null;
        }

        if (targetItem == null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange, itemLayer);
            float minDist = float.MaxValue;
            Transform newTarget = null;

            foreach (var hit in hits)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    newTarget = hit.transform;
                }
            }

            if (newTarget != null)
            {
                targetItem = newTarget;
                lastItemTargetSwitchTime = Time.time;
                Debug.Log($"{Name} がアイテムターゲットを {newTarget.name} に切り替えた！");
            }
        }
    }

    private void UpdateAnimator()
    {
        if (myAnim == null) return;

        bool isJumping = Mathf.Abs(rb.linearVelocity.y) > 0.01f;
        myAnim.SetBool("IsJump", isJumping);

        bool isRunning = !isJumping && Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        myAnim.SetBool("IsRun", isRunning);

        if (rb.linearVelocity.x > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        else if (rb.linearVelocity.x < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
    }

    // -------------------------------
    // スタック判定
    // -------------------------------
    void CheckStuck()
    {
        if (Time.time - lastMoveCheckTime > stuckCheckInterval)
        {
            float movedDist = Vector2.Distance(transform.position, lastPosition);
            if (movedDist < stuckThreshold)
            {
                Debug.Log($"{Name} がスタックしたので回避アクションを実行！");
                DoStuckAction();
            }
            lastMoveCheckTime = Time.time;
            lastPosition = transform.position;
        }
    }

    void DoStuckAction()
    {
        Vector2 newInput = Vector2.zero;
        int action = Random.Range(0, 3); // 0=左右ランダム移動, 1=ジャンプ, 2=ターゲット切替
        switch (action)
        {
            case 0:
                newInput.x = Random.Range(0, 2) == 0 ? -1 : 1;
                break;
            case 1:
                if (isGrounded) newInput.y = 1;
                break;
            case 2:
                targetItem = null;
                newInput.x = Random.Range(0, 2) == 0 ? -1 : 1;
                break;
        }

        overrideInput = newInput;
        stuckActionEndTime = Time.time + stuckActionDuration;
        Debug.Log($"{Name} がスタック回避アクション実行中: {overrideInput}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Platform"))
        {
            isGrounded = true;
            jumpCount = 0;
        }

        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D otherRb = collision.collider.GetComponent<Rigidbody2D>();
            if (otherRb != null)
            {
                Vector2 dir = (otherRb.position - rb.position).normalized;
                float force = 1f;
                rb.AddForce(-dir * force, ForceMode2D.Impulse);
                otherRb.AddForce(dir * force, ForceMode2D.Impulse);
            }
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isOni) return;
        if (Time.time - lastTagTime < tagCooldown) return;

        ICharacter target = collision.collider.GetComponent<ICharacter>();
        if (target != null && !target.IsOni())
        {
            target.SetOni(true);
            this.SetOni(false);
            lastTagTime = Time.time;
            Debug.Log($"{Name} → {target.Name} に鬼が交代（クールタイム開始）");
        }
    }

    private IEnumerator OniScoreDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Score -= downScore;
            if (Score < 0)
            {
                Score = 0;
            }
            UpdateScoreText();
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
            //GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            childKingyo.SetActive(false);
            if (oniCoroutine != null)
            {
                StopCoroutine(oniCoroutine);
                oniCoroutine = null;
            }
            //GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public bool IsOni() => isOni;

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Platform"))
            isGrounded = false;
    }

    public void ApplyScoreEffect(float value)
    {
        Score += (int)value;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        //十秒前から見えなくなるように
        if ((float)TimerManager.RemainingTime <= 10f)
        {
            scoreText.text = "???";
        }
        else
        {
            scoreText.text = Score.ToString();
        }
    }

    public void ApplySpeedEffect(float value, float duration)
    {
        StartCoroutine(SpeedCoroutine(value, duration));
    }

    private IEnumerator SpeedCoroutine(float value, float duration)
    {
        moveSpeed += value;
        yield return new WaitForSeconds(duration);
        moveSpeed -= value;
    }
}

public interface ICharacter
{
    string Name { get; }
    int Score { get; set; }
    Sprite Icon { get; }

    bool IsOni();
    void SetOni(bool value);
    void ApplyScoreEffect(float value);
    void ApplySpeedEffect(float value, float duration);
}
