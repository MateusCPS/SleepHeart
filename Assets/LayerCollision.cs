using UnityEngine;

/// <summary>
/// Liga/desliga em tempo real a colisão entre a layer de um objeto e a layer
/// do jogador (2D). Baseado no seu script original, mas com métodos para
/// ativar e desativar a ignorância de colisão durante o jogo, em vez de só
/// aplicar uma vez no Start.
///
/// COMO USAR
/// 1. Coloque este componente em qualquer GameObject (não precisa ser o jogador).
/// 2. No Inspector, defina "Object Layer" (a layer que deve parar de colidir
///    com o jogador) e "Player Layer" (a layer do jogador).
/// 3. Chame EnableIgnore() para fazer as duas layers pararem de colidir,
///    DisableIgnore() para voltar ao normal, ou ToggleIgnore() para alternar.
///    Marque "Ignore On Start" se quiser que já comece ignorando.
/// </summary>
public class LayerCollision : MonoBehaviour
{
    [Header("Object Layer")]
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private LayerMask playerLayer;

    [Header("Comportamento")]
    [Tooltip("Se marcado, já começa ignorando a colisão assim que a cena inicia.")]
    [SerializeField] private bool ignoreOnStart = false;

    private int objectID;
    private int playerID;
    private bool isIgnoring;

    void Awake()
    {
        objectID = ConvertToLayerIndex(objectLayer);
        playerID = ConvertToLayerIndex(playerLayer);
    }

    void Start()
    {
        if (ignoreOnStart)
            EnableIgnore();
    }

    /// <summary>Faz as duas layers pararem de colidir/interagir.</summary>
    public void EnableIgnore() => SetIgnore(true);

    /// <summary>Restaura a colisão normal entre as duas layers.</summary>
    public void DisableIgnore() => SetIgnore(false);

    /// <summary>Alterna entre ignorar e não ignorar.</summary>
    public void ToggleIgnore() => SetIgnore(!isIgnoring);

    /// <summary>Estado atual: true se as layers estão ignorando colisão entre si.</summary>
    public bool IsIgnoring => isIgnoring;

    private void SetIgnore(bool ignore)
    {
        Physics2D.IgnoreLayerCollision(objectID, playerID, ignore);
        isIgnoring = ignore;
    }

    int ConvertToLayerIndex(LayerMask layerMask)
    {
        return layerMask.value > 0 ? Mathf.RoundToInt(Mathf.Log(layerMask.value, 2)) : 0;
    }
    
    private void OnDestroy()
    {
        // Evita deixar a matriz de colisão "suja" para outros objetos depois
        // que este componente for destruído.
        if (isIgnoring)
            SetIgnore(false);
    }
}