using UnityEngine;

namespace CoracaoAdormecido.Combat
{
    /// <summary>
    /// Ponte mínima entre a lógica de turno (BattleController) e o Animator de um combatente.
    /// Não sabe se é o Player ou um inimigo — só dispara o trigger de ataque. Funciona pros dois
    /// lados da batalha sem duplicar código.
    ///
    /// Requer um parâmetro Trigger chamado "AttackTrigger" no Animator Controller do combatente,
    /// com uma transição Idle -> Attack usando essa condição.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CombatAnimator : MonoBehaviour
    {
        private static readonly int AttackTriggerHash = Animator.StringToHash("AttackTrigger");

        private Animator animator;

        private void Awake() => animator = GetComponent<Animator>();

        public void TriggerAttack() => animator.SetTrigger(AttackTriggerHash);
    }
}
