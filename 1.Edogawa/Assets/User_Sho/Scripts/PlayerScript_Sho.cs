using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript_Sho : MonoBehaviour, ICharacter
{
    [SerializeField] public float movespeed;
    [SerializeField] public float jumpspeed;
    private TextMeshProUGUI scoreText;
    [SerializeField] private Sprite icon;
    private Vector2 _inputDirection;
    private Rigidbody2D rigid;
    public int score = 0;
    public int playerID;
    private bool isGrounded = true;
    // ���W�����v�񐔊Ǘ�
    private int jumpCount = 0;
    [SerializeField] private int maxJumps = 2;  // 2��W�����v�܂�


    public string Name => "Player";
    public Sprite Icon => icon;


    public int Score { get => score; set => score = value; }


    [Header("�S���������")]
    [SerializeField] private int downScore;
    public bool isOni = false; // �� �S���ǂ���
    [SerializeField] private float tagCooldown = 5f; // �S���N�[���^�C���i�b�j
    private float lastTagTime = -999f;
    private Coroutine oniCoroutine; // �R���[�`���Ǘ��p
    [SerializeField] private GameObject childKingyo;
    [SerializeField] private GameObject youTextObject;
    [SerializeField] private Animator playerAnim;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        scoreText = GameObject.Find("PlayerScoreText").GetComponent<TextMeshProUGUI>();
        scoreText.text = "0";  
    }

    void Update()
    {
        _move();
        UpdateScoreText();
    }

    private void _move()
    {
        // ���͂����邩�ǂ������`�F�b�N
        if (_inputDirection.x != 0)
        {
            playerAnim.SetBool("IsRun", true);
        }
        else
        {
            playerAnim.SetBool("IsRun", false);
        }

        // ���ۂ̈ړ�����
        rigid.linearVelocity = new Vector2(_inputDirection.x * movespeed, rigid.linearVelocity.y);

        // ���E���]�����i�L�����̌�����ς���j
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
        // ���W�����v�񐔐���
        if (jumpCount < maxJumps)
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0f); // �㏸���ɂ�����x�������Ƃ��̋��������艻
            rigid.AddForce(Vector2.up * jumpspeed, ForceMode2D.Impulse);
            jumpCount++;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // "Ground" �^�O�̃I�u�W�F�N�g�ɐG�ꂽ��n�ʔ���
        if (collision.gameObject.CompareTag("Platform"))
        {
            jumpCount = 0;
            playerAnim.SetBool("IsJump", false);
        }

        //��������
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D otherRb = collision.collider.GetComponent<Rigidbody2D>();
            if (otherRb != null)
            {
                Vector2 dir = (otherRb.position - rigid.position).normalized;
                float force = 10f; // �����̋���
                rigid.AddForce(-dir * force, ForceMode2D.Impulse);
                otherRb.AddForce(dir * force, ForceMode2D.Impulse);
            }
        }

        if (!isOni) return;

        // �N�[���^�C�����Ȃ���s��
        if (Time.time - lastTagTime < tagCooldown)
            return;

        ICharacter target = collision.collider.GetComponent<ICharacter>();

        if (target != null && !target.IsOni())
        {
            // �S��㏈��
            target.SetOni(true);
            this.SetOni(false);

            lastTagTime = Time.time; // �N�[���^�C���J�n
            Debug.Log($"{Name} �� {target.Name} �ɋS�����i�N�[���^�C���J�n�j");
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
    // �S�ɐݒ肷�鏈��
    public void SetOni(bool value)
    {
        isOni = value;
        if (value)
        {
            childKingyo.SetActive(true);
            // �S�ɂȂ����u�Ԃɂ��N�[���^�C����ݒ�
            lastTagTime = Time.time;

            // �X�R�A�����炷�R���[�`���J�n�i���d�J�n��h�~�j
            if (oniCoroutine == null)
                oniCoroutine = StartCoroutine(OniScoreDown());

            // �S�ɂȂ������̉��o�〈���ڕύX
            Debug.Log($"�v���C���[ {playerID} �͋S�ɂȂ����I");
            //GetComponent<SpriteRenderer>().color = Color.red; // ���F�S�͐Ԃ�����
        }
        else
        {
            childKingyo.SetActive(false);
            // �R���[�`����~�i���S�ɊǗ��j
            if (oniCoroutine != null)
            {
                StopCoroutine(oniCoroutine);
                oniCoroutine = null;
            }

            //GetComponent<SpriteRenderer>().color = Color.white; // �ʏ���
            Debug.Log($"�v���C���[ {playerID} �͌��ɖ߂���");
        }
    }

    public bool IsOni()
    {
        return isOni;
    }

    public void ApplyScoreEffect(float value)
    {
        score += (int)value;
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
            scoreText.text = score.ToString();
        }
    }

    public void ApplySpeedEffect(float value, float duration)
    {
        StartCoroutine(SpeedEffectCoroutine(value, duration));
    }

    private IEnumerator SpeedEffectCoroutine(float value, float duration)
    {
        movespeed += value;
        Debug.Log($"{Name} ??X?s?[?h {value} ?��??I ????:{movespeed}");
        yield return new WaitForSeconds(duration);
        movespeed -= value;
        Debug.Log($"{Name} ??X?s?[?h???????? ????:{movespeed}");
    }
}
