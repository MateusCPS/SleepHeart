using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Faz o jogador assumir a mesma Layer da Unity do grid (Tilemap) em que ele
/// está posicionado no momento.
///
/// Cada Tilemap da lista deve estar em um GameObject já configurado com a
/// Layer que ele representa (ex: um Tilemap "GridA" na layer "GridA", outro
/// "GridB" na layer "GridB" etc). Quando o jogador não está sobre nenhum tile
/// de nenhum Tilemap da lista, a layer original dele (guardada no Awake) é
/// restaurada automaticamente.
///
/// COMO USAR
/// 1. Coloque este componente no jogador.
/// 2. Arraste, em ordem de prioridade, os Tilemaps que representam cada grid
///    para a lista "Grid Tilemaps" (se houver sobreposição de tiles em mais
///    de um Tilemap na mesma posição, o primeiro da lista "vence").
/// 3. Garanta que cada Tilemap esteja em um GameObject cuja Layer é a que o
///    jogador deve assumir ao pisar nele.
///
/// Dica: combine com o LayerCollision.cs para, além de mudar a layer do
/// jogador, também ignorar colisão com outras layers enquanto ele estiver
/// num grid específico.
/// </summary>
public class PlayerGridLayerFollower : MonoBehaviour
{
    [Header("Grids (em ordem de prioridade)")]
    [Tooltip("Tilemaps que representam cada grid. Cada um deve estar num GameObject com a Layer correspondente.")]
    [SerializeField] private Tilemap[] gridTilemaps;

    [Header("Opções")]
    [Tooltip("Se marcado, aplica a mesma layer também em todos os filhos do jogador (útil se o collider/sprite estiver num GameObject filho).")]
    [SerializeField] private bool applyToChildren = false;

    [Tooltip("Ponto usado para checar em qual tile o jogador está. Se não definido, usa a posição deste próprio objeto.")]
    [SerializeField] private Transform referencePoint;

    /// <summary>Layer original do jogador antes de entrar em qualquer grid.</summary>
    public int OriginalLayer { get; private set; }

    /// <summary>Layer atual aplicada (igual a OriginalLayer se fora de qualquer grid).</summary>
    public int CurrentLayer { get; private set; }

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        OriginalLayer = gameObject.layer;
        CurrentLayer = OriginalLayer;

        if (referencePoint == null)
            referencePoint = transform;
    }

    private void Update()
    {
        int targetLayer = FindCurrentGridLayer() ?? OriginalLayer;

        if (targetLayer != CurrentLayer)
            ApplyLayer(targetLayer);
    }

    private int? FindCurrentGridLayer()
    {
        Vector3 pos = referencePoint.position;

        foreach (var tilemap in gridTilemaps)
        {
            if (tilemap == null) continue;

            Vector3Int cell = tilemap.WorldToCell(pos);
            if (tilemap.HasTile(cell))
                return tilemap.gameObject.layer;
        }

        return null;
    }

    private void ApplyLayer(int layer)
    {
        CurrentLayer = layer;
        gameObject.layer = layer;

        if (applyToChildren)
        {
            foreach (Transform child in GetComponentsInChildren<Transform>(true))
                child.gameObject.layer = layer;
        }
    }
    private void ApplySortInLayer(int orderInLayer)
    {
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = orderInLayer;

        if (applyToChildren)
        {
            foreach (var child in GetComponentsInChildren<SpriteRenderer>(true))
                child.sortingOrder = orderInLayer;
        }
    }
}
