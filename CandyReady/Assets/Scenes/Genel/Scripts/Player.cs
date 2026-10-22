using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Config Player")]
    [SerializeField] public static float moveSpeed = -6f;

    public static int moneyScore = 0;

    // Último valor salvo, pra só salvar quando o dinheiro mudar
    private int lastSavedMoney;

    [SerializeField] TextMeshProUGUI textMoney;

    [Header("Reference Inputs")]
    [SerializeField] InputActionReference moveAction;

    Vector2 movement;

    [Header("Other References")]
    Rigidbody rb;
    [SerializeField] QueueManager queueManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Carrega o dinheiro salvo (New Game já zerou no menu)
        moneyScore = DataManager.LoadMoney();
        lastSavedMoney = moneyScore;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    void Update()
    {
        ReadInput();
        Movement();
        textMoney.text = "MONEY: " + moneyScore;
        SaveMoneyIfChanged();
    }

    void SaveMoneyIfChanged()
    {
        if (moneyScore != lastSavedMoney)
        {
            DataManager.SaveMoney(moneyScore);
            lastSavedMoney = moneyScore;
        }
    }

    // Garante o save no mobile quando o app vai pro segundo plano
    private void OnApplicationPause(bool paused)
    {
        if (paused) DataManager.SaveMoney(moneyScore);
    }

    private void OnApplicationQuit()
    {
        DataManager.SaveMoney(moneyScore);
    }

    void ReadInput()
    {
        movement = moveAction.action.ReadValue<Vector2>().normalized;
    }

    void Movement()
    {
        Vector3 move = transform.right * movement.x + transform.forward * movement.y;
        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ServeCustomer"))
        {
            TryDeliverOrder();
        }
    }

    private void TryDeliverOrder()
    {
        if (queueManager == null || queueManager.npcQueue.Count == 0)
        {
            print("NINGUÉM NA FILA");
            return;
        }

        NPC firstNPC = queueManager.npcQueue[0];

        if (firstNPC.currentState != NPC.NPCState.WaitingOrder)
        {
            print("NPC AINDA NÃO ESTÁ PRONTO PARA RECEBER");
            return;
        }

        firstNPC.ReceiveOrder();
    }
}