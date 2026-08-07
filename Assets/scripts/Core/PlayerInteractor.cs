using UnityEngine;
using UnityEngine.InputSystem;

namespace CoracaoAdormecido.Core
{
    /// <summary>
    /// Detecta o IInteractable mais próximo dentro de um raio e expõe o texto de prompt atual.
    /// A UI (HUD contextual, GDD 11.1) deve ler CurrentPrompt; combate e diálogo não precisam
    /// conhecer este script diretamente — só implementar IInteractable quando fizer sentido.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Detecção")]
        [SerializeField] private float interactionRadius = 1.2f;
        [SerializeField] private LayerMask interactableMask;

        private InputAction interactAction;
        private readonly Collider2D[] overlapBuffer = new Collider2D[8];

        /// <summary>Texto atual para o HUD contextual, ou null se nada estiver ao alcance.</summary>
        public string CurrentPrompt { get; private set; }

        private IInteractable currentTarget;

        private void Awake()
        {
            interactAction = new InputAction("Interact", binding: "<Keyboard>/e");
            // Botão de face (X/Quadrado): no controle, defesa/especial fica no gatilho (GDD 7.2),
            // então interação usa um botão de face livre.
            interactAction.AddBinding("<Gamepad>/buttonWest");
        }

        private void OnEnable() => interactAction.Enable();
        private void OnDisable() => interactAction.Disable();

        private void Update()
        {
            FindClosestInteractable();

            if (currentTarget != null
                && interactAction.WasPerformedThisFrame()
                && currentTarget.CanInteract(gameObject))
            {
                currentTarget.Interact(gameObject);
            }
        }

        private void FindClosestInteractable()
        {
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, interactionRadius, overlapBuffer, interactableMask);

            IInteractable closest = null;
            float closestDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (overlapBuffer[i].TryGetComponent<IInteractable>(out var interactable))
                {
                    float dist = (overlapBuffer[i].transform.position - transform.position).sqrMagnitude;
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = interactable;
                    }
                }
            }

            currentTarget = closest;
            CurrentPrompt = (closest != null && closest.CanInteract(gameObject)) ? closest.GetPrompt() : null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
