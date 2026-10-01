using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TriggerADS : MonoBehaviour
{
    [SerializeField] GameObject PainelAds;
    [SerializeField] Button buttonInitializerAds;
    [SerializeField] Button buttonCloseAds;

    private void Start()
    {
        buttonInitializerAds.onClick.AddListener(ShowAds);
        buttonCloseAds.onClick.AddListener(FecharAds);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AbrirAds();
        }
    }


    void AbrirAds()
    {
        PainelAds.SetActive(true);
        Time.timeScale = 0f;
    }

    void FecharAds()
    {
        PainelAds.SetActive(false);
        Time.timeScale = 1f;
    }


    void ShowAds()
    {
        GameObject.Find("AdsInitializer").GetComponent<AdsInitializer>().LoadInterstitialAd();
        GameObject.Find("AdsInitializer").GetComponent<AdsInitializer>().ShowInterstitialAd();
        Player.moneyScore += 300;
    }

}
