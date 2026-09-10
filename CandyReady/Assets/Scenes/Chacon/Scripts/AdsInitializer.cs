using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsShowListener, IUnityAdsLoadListener
{
    [Header("ADS Settings")]
    [SerializeField] private string androidGameID = "6188821";
    [SerializeField] private string iOSGameID = "6188820";
    [SerializeField] bool testMode = true;

    private string gameID; //ID da plataforma do jogo (Android ou IOS)

    private string interstitialAdUnitId = "Interstitial_Android";
    private string rewardedAdUnitId = "Rewarded_Android";

    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
#if UNITY_IOS
       gameID = iOSGameID;
#else
        gameID = androidGameID;
#endif
        // caso o sistema esteja sem inicializar e tambem e suportado (inicializa)
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameID, testMode, this);
        }
    }

    //Carrega o Anuncio
    public void LoadInterstitialAd()

    {
        // USA A INTERFACE (API) PARA CARREGAR O ANUNCIO (CALLBACK)
        Advertisement.Load(interstitialAdUnitId, this);
    }

    // MOSTRAR O ANUNCIO CARREGADO
    public void ShowInterstitialAd()
    {
        Advertisement.Show(interstitialAdUnitId, this); // Usa a interface IUnityAdsShowListener para call-backs
    }

    // Carrega um an ncio recompensado
    public void LoadRewardedAd()
    {
        Advertisement.Load(rewardedAdUnitId, this);
    }

    // Mostra o an ncio recompensado carregado
    public void ShowRewardedAd()
    {
        Advertisement.Show(rewardedAdUnitId, this);
    }

    // -- IMPLEMENTA  O DAS INTERFACES DA API DE ADS

    // Chamado quando a inicializa  o dos an ncios   bem-sucedida
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        LoadInterstitialAd(); // Carrega an ncio intersticial
        LoadRewardedAd();     // Carrega an ncio recompensado
    }

    // Chamado quando a inicializa  o dos an ncios falha
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    // Chamado quando um an ncio   carregado com sucesso
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
    }

    // Chamado quando o carregamento do an ncio falha
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    // Chamado quando ocorre erro ao mostrar um an ncio
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    // Chamado quando o an ncio come a a ser exibido
    public void OnUnityAdsShowStart(string adUnitId) { }

    // Chamado quando o usu rio clica no an ncio
    public void OnUnityAdsShowClick(string adUnitId) { }

    // Chamado quando o an ncio termina de ser exibido
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        // Se o an ncio exibido foi o recompensado e foi completado
        if (adUnitId.Equals(rewardedAdUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Rewarded ad completed! Give reward to player.");
            // Aqui voc  deve adicionar a l gica para dar a recompensa ao jogador
        }
    }
}
