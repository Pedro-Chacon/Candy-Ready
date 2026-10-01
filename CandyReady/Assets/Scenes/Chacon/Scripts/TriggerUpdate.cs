using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TriggerUpdate : MonoBehaviour
{
    [SerializeField] GameObject painelUpgrade;
    [SerializeField] Button buttonUpgradeVelocity;
    [SerializeField] Button buttonClose;
    [SerializeField] Text textStage;

    [Header("Economia e Status")]
    public int moedas;
    public float velocidadeAtual;

    [Header("Sistema de Upgrade")]
    public int estagioAtual = 0; 
    private int estagioMaximo = 4;

    [Tooltip("Custo para alcançar o estágio 1, 2, 3 e 4")]
    public int[] custoUpgrade = { 250, 500, 1000, 1500 };

    [Tooltip("Velocidade nos estágios 0, 1, 2, 3 e 4")]
    public float[] velocidadePorEstagio = { -6f, -8f, -9f, -12f, -15f };

    private void Start()
    {
        buttonUpgradeVelocity.onClick.AddListener(ComprarUpgrade);
        buttonClose.onClick.AddListener(FecharLoja);
        velocidadeAtual = velocidadePorEstagio[estagioAtual];
    }

    private void Update()
    {
      moedas = Player.moneyScore;
      velocidadeAtual = Player.moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AbrirLoja();
        }
    }

    void AbrirLoja()
    {
        painelUpgrade.SetActive(true);
        Time.timeScale = 0f;
    }

    void FecharLoja()
    {
        painelUpgrade.SetActive(false);
        Time.timeScale = 1f;
    }


    public void ComprarUpgrade()
    {
        // Verifica se já está no nível máximo
        if (estagioAtual >= estagioMaximo)
        {
            Debug.Log("O player já atingiu o estágio máximo de velocidade!");
            return;
        }

        // Pega o custo do próximo estágio
        int custo = custoUpgrade[estagioAtual];

        // Verifica se o player tem moedas suficientes
        if (Player.moneyScore >= custo)
        {
            Player.moneyScore -= custo;
            estagioAtual++;
            Player.moveSpeed = velocidadePorEstagio[estagioAtual];

            textStage.text = estagioAtual + "/4";
            Debug.Log($"Upgrade Sucesso! Estágio {estagioAtual}/4. Nova Velocidade: {velocidadeAtual}. Moedas restantes: {moedas}");
        } 
        else
        {
            Debug.Log($"Moedas insuficientes! Você precisa de {custo} moedas, mas tem apenas {moedas}.");
        }
    }


}
