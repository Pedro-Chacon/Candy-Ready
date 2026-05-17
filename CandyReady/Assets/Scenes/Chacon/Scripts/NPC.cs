using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] Transform counterPosition;
    [SerializeField] Transform exit;

    [Header("Seats")]
    [SerializeField] SeatChair[] seats;
    [SerializeField] public float TimeEating = 5f;
    private SeatChair currentSeat;

    [Header("Order Bubble")]
    [SerializeField] GameObject orderBubble;
    [SerializeField] TMPro.TextMeshProUGUI orderText;

    public int candyQuantity = 0;

    private NavMeshAgent navMeshAgent;

    public NPCState currentState;

    private void Awake()
    {

    }

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

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        DesactiveBubble();
        ChangeState(NPCState.GoingToCounter);
    }

    void Update()
    {

    }

    // FUNÇÕES
    public void ActiveBubble()
    {
        orderBubble.SetActive(true);
        orderText.enabled = true;
    }

    public void DesactiveBubble()
    {
        orderBubble.SetActive(false);
        orderText.enabled = false;
    }

    public List<SeatChair> GetFreeSeats()
    {
        List<SeatChair> freeSeats = new List<SeatChair>();

        foreach (SeatChair seat in seats)
        {
            if (seat.occupied == false)
            {
                freeSeats.Add(seat);
            }
        }

        return freeSeats;
    }

    // ESTADOS DO NPC
    void ChangeState(NPCState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case NPCState.GoingToCounter:
                navMeshAgent.destination = counterPosition.position;
                break;

            case NPCState.WaitingOrder:
                StartCoroutine(CreateAnOrder());
                break;

            case NPCState.GoingToSeat:
                StartCoroutine(GoToSeat());
                break;

            case NPCState.Eating:
                StartCoroutine(Eating());
                break;

            case NPCState.Leaving:
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
        yield return new WaitForSeconds(1);

        print("Criou Pedido");

        candyQuantity = Random.Range(1, 5);

        ActiveBubble();

        orderText.text = candyQuantity.ToString();

        print("quantidade do pedido: " + candyQuantity);
    }

    [ContextMenu("Pedido Recebido")]
    public void ReceiveOrder()
    {
        DesactiveBubble();
        ChangeState(NPCState.LookingForSeats);
    }

    IEnumerator LookingForSeats()
    {
        yield return new WaitForSeconds(0.1f);

        print("PROCURANDO POR CADEIRA!");

        yield return new WaitForSeconds(1);

        List<SeatChair> freeSeats = GetFreeSeats();

        if (freeSeats.Count <= 0)
        {
            print("Todas as cadeiras ocupadas!");

            ChangeState(NPCState.AwaitingForSeats);
        }
        else
        {
            ChangeState(NPCState.GoingToSeat);
        }
    }

    IEnumerator GoToSeat()
    {
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

        print("escolheu assento " + aleatoryChair);

        navMeshAgent.destination = aleatoryChair.transform.position;
    }

    IEnumerator Eating()
    {
        yield return new WaitForSeconds(TimeEating);

        ChangeState(NPCState.Leaving);
    }

    IEnumerator Leaving()
    {
        currentSeat.occupied = false;

        navMeshAgent.destination = exit.transform.position;

        print("terminou de comer");

        yield return null;
    }

    IEnumerator WaitingForSeats()
    {
        orderText.text = "!!!";

        ActiveBubble();

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            List<SeatChair> freeSeats = GetFreeSeats();

            if (freeSeats.Count > 0)
            {
                DesactiveBubble();

                ChangeState(NPCState.GoingToSeat);

                yield break;
            }
        }
    }

    // FIM DOS ESTADOS DO NPC
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CounterPosition"))
        {
            ChangeState(NPCState.WaitingOrder);
        }

        if (other.gameObject.CompareTag("ExitPoint"))
        {
            Destroy(gameObject);
        }

        SeatChair seat = other.GetComponent<SeatChair>();

        if (seat != null)
        {
            print("Sentou em " + seat.name);

            ChangeState(NPCState.Eating);
        }
    }
}