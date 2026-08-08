using UnityEngine;
using CoracaoAdormecido.Core;
 
namespace CoracaoAdormecido.Content.Interactables
{
    /// <summary>
    /// Exemplo mínimo de NPC interagível: mostra falas em sequência via Debug.Log.
    /// Placeholder até o DialogueService (GDD 13.2) existir de verdade — troque o corpo de
    /// Interact() por uma chamada ao serviço de diálogo quando ele for implementado, sem
    /// precisar mexer no PlayerInteractor.
    /// </summary>
    public class SimpleNpcDialogue : MonoBehaviour, IInteractable
    {
        [SerializeField] private string npcName;
        [TextArea]
        [SerializeField] private string[] lines;
 
        private int currentLine;
 
        public string GetPrompt() => $"Falar com {npcName}";
 
        // Sempre pode conversar de novo; troque para uma condição (ex.: já resgatado,
        // missão concluída) quando o NPC tiver estado próprio.
        public bool CanInteract(GameObject instigator) => lines.Length > 0;
 
        public void Interact(GameObject instigator)
        {
            if (lines.Length == 0) return;
 
            Debug.Log($"{npcName}: {lines[currentLine]}");
            currentLine = (currentLine + 1) % lines.Length;
        }
    }
}