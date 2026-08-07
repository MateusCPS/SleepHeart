using UnityEngine;

namespace CoracaoAdormecido.Core
{
    /// <summary>
    /// Contrato mínimo de interação. Objetos de mundo (alavancas, portões, NPCs, pontos de
    /// interesse) implementam esta interface em vez de o PlayerInteractor conhecer cada tipo
    /// concreto — mantém o desacoplamento pedido em GDD 13.3 ("Eventos de gameplay desacoplam
    /// HUD, áudio, missões e combate").
    /// </summary>
    public interface IInteractable
    {
        /// <summary>Texto mostrado no HUD contextual (GDD 11.1 - "Centro contextual").</summary>
        string GetPrompt();

        /// <summary>Executado quando o jogador confirma a interação.</summary>
        void Interact(GameObject instigator);

        /// <summary>Permite ao objeto recusar interação (ex.: alavanca já usada, porta trancada).</summary>
        bool CanInteract(GameObject instigator);
    }
}
