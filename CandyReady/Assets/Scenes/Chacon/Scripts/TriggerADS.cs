using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TriggerADS : MonoBehaviour
{
    [SerializeField] GameObject CanvaAds;
    [SerializeField] Button buttonInitializerAds;

    private void Start()
    {
        buttonInitializerAds.onClick.AddListener(ShowAds);   
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            CanvaAds.SetActive(true);
        }
    }

    void ShowAds()
    {
        GameObject.Find("AdsInitializer").GetComponent<AdsInitializer>().LoadInterstitialAd();
        GameObject.Find("AdsInitializer").GetComponent<AdsInitializer>().ShowInterstitialAd();
    }

}
