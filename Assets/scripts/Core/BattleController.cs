using UnityEngine;
using UnityEngine.InputSystem;

namespace CoracaoAdormecido.Combat
{
    /// <summary>
    /// Orquestra uma batalha 1x1 por turnos. Turnos valem só para as batalhas — a exploração
    /// no mundo continua em tempo real (PlayerController, PlayerInteractor etc. não mudam).
    ///
    /// Loop mínimo pra provar o ciclo: jogador ataca -> inimigo ataca de volta -> repete até
    /// alguém morrer. Seleção de ação hoje é um placeholder por teclado (Espaço/botão Sul do
    /// controle); trocar por um menu de verdade quando a UI de batalha entrar em produção
    /// (GDD 14.4, item 7 vem depois do framework de gameplay, então isso está na ordem certa).
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        private enum BattleState { PlayerTurn, EnemyTurn, Won, Lost }

        [Header("Participantes")]
        [SerializeField] private Health playerHealth;
        [SerializeField] private Health enemyHealth;

        [Header("Animação (opcional — deixe em branco se ainda não tiver Animator)")]
        [SerializeField] private CombatAnimator playerAnimator;
        [SerializeField] private CombatAnimator enemyAnimator;

        [Header("Balanceamento provisório")]
        [SerializeField] private int playerAttackDamage;
        [SerializeField] private int enemyAttackDamage;
        [Tooltip("Pausa entre o golpe do inimigo e o turno voltar pro jogador, pra dar tempo de ler o que aconteceu.")]
        [SerializeField] private float enemyTurnDelay = 0.6f;

        private InputAction confirmAction;
        private BattleState state;
        private float turnTimer;

        private void Awake()
        {
            confirmAction = new InputAction("ConfirmAttack", binding: "<Keyboard>/space");
            confirmAction.AddBinding("<Gamepad>/buttonSouth");
        }

        private void OnEnable() => confirmAction.Enable();
        private void OnDisable() => confirmAction.Disable();

        private void Start()
        {
            playerHealth.ResetHealth();
            enemyHealth.ResetHealth();
            state = BattleState.PlayerTurn;
        }

        private void Update()
        {
            switch (state)
            {
                case BattleState.PlayerTurn:
                    if (confirmAction.WasPerformedThisFrame())
                        PlayerAttack();
                    break;

                case BattleState.EnemyTurn:
                    turnTimer -= Time.deltaTime;
                    if (turnTimer <= 0f)
                        EnemyAttack();
                    break;

                // Won/Lost: loop parado de propósito. Reiniciar ou sair da cena é o próximo passo.
            }
        }

        private void PlayerAttack()
        {
            playerAnimator?.TriggerAttack();
            enemyHealth.TakeDamage(playerAttackDamage, playerHealth.gameObject);

            if (enemyHealth.IsDead)
            {
                state = BattleState.Won;
                Debug.Log("Vitória!");
                return;
            }

            state = BattleState.EnemyTurn;
            turnTimer = enemyTurnDelay;
        }

        private void EnemyAttack()
        {
            enemyAnimator?.TriggerAttack();
            playerHealth.TakeDamage(enemyAttackDamage, enemyHealth.gameObject);

            if (playerHealth.IsDead)
            {
                state = BattleState.Lost;
                Debug.Log("Derrota.");
                return;
            }

            state = BattleState.PlayerTurn;
        }
    }
}
