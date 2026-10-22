using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public const string MONEY_KEY = "money";
    public const string VOLUME_KEY = "volume";
    public const string HAS_SAVE_KEY = "hasSave";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---------- DINHEIRO ----------
    public static void SaveMoney(int value)
    {
        PlayerPrefs.SetInt(MONEY_KEY, value);
        PlayerPrefs.SetInt(HAS_SAVE_KEY, 1);
        PlayerPrefs.Save();
    }

    public static int LoadMoney()
    {
        return PlayerPrefs.GetInt(MONEY_KEY, 0);
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(HAS_SAVE_KEY);
    }

    // ---------- ÁUDIO ----------
    public static void SaveVolume(float value)
    {
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    public static float LoadVolume()
    {
        return PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
    }

    // ---------- NEW GAME ----------
    // Reseta o progresso (dinheiro), mas mantém a configuração de áudio.
    public static void NewGame()
    {
        Player.moneyScore = 0;
        Player.moveSpeed = -6f;
        SaveMoney(0);
    }
}