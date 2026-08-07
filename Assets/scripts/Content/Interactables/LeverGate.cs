using UnityEngine;
using CoracaoAdormecido.Core;

namespace CoracaoAdormecido.Content.Interactables
{
    /// <summary>
    /// Exemplo concreto de IInteractable: uma alavanca que abre um portão.
    ///
    /// Referência de fluxo para o obstáculo "Portão militar" da Cidadela de Ferro (GDD 6.7):
    /// a rota geral pode usar exatamente este script sem alterações; a rota de classe do
    /// Guerreiro ("aciona o contrapeso e identifica o protocolo") pode reaproveitar o mesmo
    /// contrato IInteractable com uma implementação diferente de Interact(), sem exigir que o
    /// PlayerInteractor saiba qual classe está jogando.
    /// </summary>
    public class LeverGate : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject gate;
        [SerializeField] private string promptText = "Puxar alavanca";

        private bool activated;

        public string GetPrompt() => activated ? "Alavanca já ativada" : promptText;

        public bool CanInteract(GameObject instigator) => !activated;

        public void Interact(GameObject instigator)
        {
            activated = true;

            // Placeholder: trocar por animação/abertura gradual quando a arte entrar (GDD 12.2).
            if (gate != null)
                gate.SetActive(false);
        }
    }
}
