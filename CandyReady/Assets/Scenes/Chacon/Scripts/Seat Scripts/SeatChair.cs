using UnityEngine;

public class SeatChair : MonoBehaviour
{
    public bool occupied;
    void Start()
    {
        occupied = false;
    }

    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            occupied = true;
            print("Player está em " + gameObject.name);
        }
    }


}
