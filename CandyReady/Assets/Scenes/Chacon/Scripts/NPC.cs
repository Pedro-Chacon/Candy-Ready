using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("Order Bubble")]
    [SerializeField] GameObject orderBubble;
    [SerializeField] TMPro.TextMeshProUGUI orderText;


    public int candyQuantity = 0;

    private NavMeshAgent navMeshAgent;
    SeatChair seatChair;

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
        Leaving
    }
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        DesactiveBubble();
        ChangeState(NPCState.GoingToCounter);
    }


    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            ChangeState(NPCState.GoingToSeat);
            DesactiveBubble();
        }
    }

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

            case NPCState.Leaving:
                StartCoroutine(EatAndLeave());
                break;
        }
    }

 

    IEnumerator EatAndLeave()
    {
        yield return new WaitForSeconds(TimeEating);
        print("terminou de comer");
        //AQUI PRECISAVA DEIXAR SEAT.OCCUPIED = FALSE
        navMeshAgent.destination = exit.transform.position;
    }

    IEnumerator GoToSeat()
    {
        yield return new WaitForSeconds(1);
        foreach(SeatChair seat in seats)
        {
            if (seat.occupied == false)
            {
                seat.occupied = true;
                print("escolheu assento " + seat);
                navMeshAgent.destination = seat.transform.position;
                break;
            }
            else
            {
                print("TODAS AS CADEIRAS OCUPADAS!");
            }
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

        #region SeatsTags
        if (other.gameObject.CompareTag("Seat0"))
        {
            print("Sentou assento 0");
            ChangeState(NPCState.Leaving);


        }
        if (other.gameObject.CompareTag("Seat1"))
        {
            print("Sentou assento 1");
            ChangeState(NPCState.Leaving);

        }
        if (other.gameObject.CompareTag("Seat2"))
        {
            print("Sentou assento 2");
            ChangeState(NPCState.Leaving);

        }
        if (other.gameObject.CompareTag("Seat3"))
        {
            print("Sentou assento 3");
            ChangeState(NPCState.Leaving);

        }
        if (other.gameObject.CompareTag("Seat4"))
        {
            print("Sentou assento 4");
            ChangeState(NPCState.Leaving);

        }
        if (other.gameObject.CompareTag("Seat5"))
        {
            print("Sentou assento 5");
            ChangeState(NPCState.Leaving);

        }
        if (other.gameObject.CompareTag("Seat6"))
        {
            print("Sentou assento 6");
            ChangeState(NPCState.Leaving);

        }
        if (other.gameObject.CompareTag("Seat7"))
        {
            print("Sentou assento 7");
            ChangeState(NPCState.Leaving);

        }

        #endregion

    }





}
