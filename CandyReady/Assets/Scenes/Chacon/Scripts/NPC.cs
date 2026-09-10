using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [Header("Animação (opcional — deixe vazio se não usar Animator)")]
    [SerializeField] private Animator animator;

    [Header("NPC Mood")]
    public NPCMood currentMood;

    [Header("Positions")]
    [SerializeField] Transform counterPosition;
    [SerializeField] Transform exit;

    [Header("Seats")]
    [SerializeField] SeatChair[] seats;
    [SerializeField] public float TimeEating = 7f;
    private SeatChair currentSeat;

    [Header("Order Bubble")]
    [SerializeField] GameObject orderBubble;
    [SerializeField] TMPro.TextMeshProUGUI orderText;

    [Header("Time Waiting Order (opcional — deixe vazio pra desativar)")]
    [Tooltip("Slider (UI padrão da Unity) que mostra o tempo restante antes do NPC desistir do pedido.")]
    [SerializeField] Slider sliderTempo;
    public float tempoCalmo = 30f;
    public float tempoNeutro = 22f;
    public float tempoBravo = 15f;

    [Header("Timer")]
    public bool isTimerPaused = false;

    [Header("Queue")]
    [SerializeField] QueueManager queueManager;

    [Header("Food")]
    [SerializeField] Transform foodHoldPoint;

    private List<GameObject> currentFoods = new List<GameObject>();

    [Header("Counter")]
    [SerializeField] Counter counter;

    public int candyQuantity = 0;

    // Só vira true depois que o CreateAnOrder terminou de gerar a quantidade.
    // É essa flag que resolve o bug de "entregar pedido vazio".
    private bool orderReady = false;

    private NavMeshAgent navMeshAgent;

    public NPCState currentState;

    public enum NPCState
    {
        GoingToCounter,
        WaitingOrder,
        GoingToSeat,
        Eating,
        Leaving,
        AwaitingForSeats,
        LookingForSeats
    }

    public enum NPCMood
    {
        Calm,
        Neutral,
        Angry
    }

    #region Animação

    public void SetBoolIsWalking()
    {
        if (animator == null) return;
        animator.SetBool("IsEating", false);
        animator.SetBool("IsLookingAround", false);
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsWalking", true);
    }

    public void SetBoolIsEating()
    {
        if (animator == null) return;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsLookingAround", false);
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsEating", true);
    }

    public void SetBoolIsIdle()
    {
        if (animator == null) return;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsEating", false);
        animator.SetBool("IsLookingAround", false);
        animator.SetBool("IsIdle", true);
    }

    public void SetBoolIsWaitingForSeats()
    {
        if (animator == null) return;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsEating", false);
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsLookingAround", true);
    }

    #endregion

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (sliderTempo != null)
            sliderTempo.interactable = false;
        sliderTempo.enabled = false;

        DesactiveBubble();

        currentMood = (NPCMood)Random.Range(0, System.Enum.GetValues(typeof(NPCMood)).Length);

        queueManager = FindAnyObjectByType<QueueManager>();
        counter = FindAnyObjectByType<Counter>();

        seats = FindObjectsByType<SeatChair>(FindObjectsSortMode.None);

        GameObject counterObj = GameObject.FindGameObjectWithTag("CounterPosition");
        if (counterObj != null) counterPosition = counterObj.transform;

        GameObject exitObj = GameObject.FindGameObjectWithTag("ExitPoint");
        if (exitObj != null) exit = exitObj.transform;

        if (queueManager != null) queueManager.AddToQueue(this);

        if (queueManager == null) print("QUEUE MANAGER NULL");
        if (counter == null) print("COUNTER NULL");
        if (counterPosition == null) print("COUNTER POSITION NULL");
        if (exit == null) print("EXIT NULL");
        if (seats == null || seats.Length <= 0) print("SEATS NULL");
        if (orderBubble == null) print("ORDER BUBBLE NULL");
        if (orderText == null) print("ORDER TEXT NULL");
        if (foodHoldPoint == null) print("FOOD HOLD POINT NULL");
    }

    public void ActiveBubble()
    {
        orderBubble.SetActive(true);
        orderText.enabled = true;
        sliderTempo.enabled = true;
    }

    public void DesactiveBubble()
    {
        orderBubble.SetActive(false);
        orderText.enabled = false;
        sliderTempo.enabled = false;
    }

    public List<SeatChair> GetFreeSeats()
    {
        List<SeatChair> freeSeats = new List<SeatChair>();
        foreach (SeatChair seat in seats)
        {
            if (seat.occupied == false) freeSeats.Add(seat);
        }
        return freeSeats;
    }

    public void MoveToPosition(Vector3 position)
    {
        SetBoolIsWalking();
        navMeshAgent.destination = position;
    }

    void ChangeState(NPCState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case NPCState.GoingToCounter:
                SetBoolIsIdle();
                navMeshAgent.destination = counterPosition.position;
                break;

            case NPCState.WaitingOrder:
                StartCoroutine(CreateAnOrder());
                break;

            case NPCState.GoingToSeat:
                DesactiveBubble();
                StartCoroutine(GoToSeat());
                break;

            case NPCState.Eating:
                StartCoroutine(Eating());
                break;

            case NPCState.Leaving:
                DesactiveBubble();
                StartCoroutine(Leaving());
                break;

            case NPCState.AwaitingForSeats:
                StartCoroutine(WaitingForSeats());
                break;

            case NPCState.LookingForSeats:
                StartCoroutine(LookingForSeats());
                break;
        }
    }

    IEnumerator CreateAnOrder()
    {
        candyQuantity = 0;
        orderReady = false;

        StartCoroutine(WaitingTimeCancelOrder());

        yield return new WaitForSeconds(1f);

        candyQuantity = Random.Range(1, 5);

        ActiveBubble();
        orderText.text = candyQuantity.ToString();

        // Só a partir daqui existe de fato um pedido válido pra ser entregue.
        orderReady = true;

        print(gameObject.name + " quer: " + candyQuantity + " candies");
    }

    IEnumerator WaitingTimeCancelOrder()
    {
        // Se nenhum slider for atribuído no Inspector, esse recurso fica desativado
        // e o NPC nunca desiste sozinho do pedido (comportamento antigo do Candy Ready).
        if (sliderTempo == null) yield break;

        yield return new WaitForSeconds(0.1f);

        float tempoBase = currentMood switch
        {
            NPCMood.Calm => tempoCalmo,
            NPCMood.Neutral => tempoNeutro,
            NPCMood.Angry => tempoBravo,
            _ => tempoNeutro
        };

        sliderTempo.maxValue = tempoBase;
        sliderTempo.value = tempoBase;

        while (sliderTempo.value > 0)
        {
            if (!isTimerPaused)
                sliderTempo.value -= Time.deltaTime;
            yield return null;
        }

        if (currentState == NPCState.WaitingOrder)
        {
            print(gameObject.name + " desistiu do pedido e foi embora.");

            orderReady = false;
            DesactiveBubble();

            if (queueManager != null)
                queueManager.RemoveFromQueue(this);

            ChangeState(NPCState.Leaving);
        }
    }

    [ContextMenu("Pedido Recebido")]
    public void ReceiveOrder()
    {
        // *** CORREÇÃO DO BUG ***
        // Antes, se o player encostasse no trigger de entrega antes do pedido
        // ser realmente gerado (candyQuantity == 0), o NPC era "atendido" sem
        // nenhum candy ter sido de fato retirado do balcão. Agora só aceitamos
        // a entrega se o pedido já estiver pronto e com quantidade > 0.
        if (!orderReady || candyQuantity <= 0)
        {
            print("Pedido ainda não está pronto, não é possível entregar.");
            return;
        }

        StartCoroutine(PickupOrder());
    }

    IEnumerator PickupOrder()
    {
        yield return new WaitForSeconds(0.5f);

        if (!orderReady || candyQuantity <= 0)
        {
            print("Pedido inválido no momento da entrega.");
            yield break;
        }

        if (counter.GetCandyCount() < candyQuantity)
        {
            print("NÃO TEM CANDY SUFICIENTE");
            yield break;
        }

        orderReady = false;

        queueManager.RemoveFromQueue(this);

        DesactiveBubble();
        currentFoods.Clear();

        for (int i = 0; i < candyQuantity; i++)
        {
            GameObject candy = counter.TakeItem();

            if (candy != null)
            {
                currentFoods.Add(candy);
                candy.transform.SetParent(foodHoldPoint);
                candy.transform.localPosition = new Vector3(0, i * 0.25f, 0);
                candy.transform.localRotation = Quaternion.identity;
            }
        }

        ChangeState(NPCState.LookingForSeats);
    }

    IEnumerator LookingForSeats()
    {
        DesactiveBubble();
        SetBoolIsWaitingForSeats();

        yield return new WaitForSeconds(0.1f);
        print("PROCURANDO POR CADEIRA");
        yield return new WaitForSeconds(1f);

        List<SeatChair> freeSeats = GetFreeSeats();

        if (freeSeats.Count <= 0)
        {
            print("TODAS AS CADEIRAS OCUPADAS");
            ChangeState(NPCState.AwaitingForSeats);
        }
        else
        {
            ChangeState(NPCState.GoingToSeat);
        }
    }

    IEnumerator GoToSeat()
    {
        DesactiveBubble();
        SetBoolIsWalking();

        yield return new WaitForSeconds(0.1f);

        List<SeatChair> freeSeats = GetFreeSeats();

        if (freeSeats.Count <= 0)
        {
            ChangeState(NPCState.AwaitingForSeats);
            yield break;
        }

        int aleatoryValue = Random.Range(0, freeSeats.Count);
        SeatChair aleatoryChair = freeSeats[aleatoryValue];

        currentSeat = aleatoryChair;
        aleatoryChair.occupied = true;

        print("Escolheu assento " + aleatoryChair);

        navMeshAgent.destination = aleatoryChair.transform.position;
    }

    IEnumerator Eating()
    {
        SetBoolIsEating();

        yield return new WaitForSeconds(TimeEating);

        foreach (GameObject food in currentFoods)
        {
            if (food != null) Destroy(food);
        }

        currentFoods.Clear();

        ChangeState(NPCState.Leaving);
    }

    IEnumerator Leaving()
    {
        if (currentSeat != null)
            currentSeat.occupied = false;

        if (exit == null)
        {
            Debug.LogError("Exit NULL no NPC");
            yield break;
        }

        navMeshAgent.destination = exit.position;

        SetBoolIsWalking();

        print(gameObject.name + " indo embora");

        Player.moneyScore += 75;

        yield return null;
    }

    IEnumerator WaitingForSeats()
    {
        DesactiveBubble();

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            List<SeatChair> freeSeats = GetFreeSeats();

            if (freeSeats.Count > 0)
            {
                ChangeState(NPCState.GoingToSeat);
                yield break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CounterPosition"))
        {
            ChangeState(NPCState.WaitingOrder);
        }

        if (other.gameObject.CompareTag("TriggerIdle"))
        {
            SetBoolIsIdle();
        }

        if (other.gameObject.CompareTag("ExitPoint"))
        {
            if (queueManager != null)
                queueManager.RemoveFromQueue(this);

            Destroy(gameObject);
        }

        SeatChair seat = other.GetComponent<SeatChair>();

        if (seat != null)
        {
            print("Sentou em " + seat.name);
            ChangeState(NPCState.Eating);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("TriggerIdle"))
        {
            SetBoolIsIdle();
        }
    }
}