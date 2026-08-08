using UnityEngine;

namespace CoracaoAdormecido.Core
{
    /// <summary>
    /// Traduz o estado do PlayerController para os parâmetros do Animator Controller:
    /// MoveX/MoveY (input do frame atual), Speed (magnitude do input), LasX/LastY
    /// (última direção não nula, para o Blend Tree 2D Simple Directional das idles
    /// direcionais) e IsRunning.
    ///
    /// Os nomes dos parâmetros abaixo precisam bater exatamente com os que aparecem na aba
    /// Parameters do Animator — inclusive "LasX" sem o T, como já está configurado no
    /// projeto. Se renomear o parâmetro no Animator, atualize a string correspondente aqui.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAnimatorDriver : MonoBehaviour
    {
        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveYHash = Animator.StringToHash("MoveY");
        private static readonly int LasXHash = Animator.StringToHash("LasX");
        private static readonly int LastYHash = Animator.StringToHash("LastY");
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

        private Animator animator;
        private PlayerController controller;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            controller = GetComponent<PlayerController>();
        }

        private void Update()
        {
            Vector2 moveInput = controller.CurrentMoveInput;
            Vector2 facing = controller.FacingDirection; // já congela a última direção não nula

            float speed = moveInput.magnitude;

            // Move/Run e Idle usam a mesma direção congelada. Usar moveInput bruto aqui fazia o
            // Blend Tree de Run amostrar (0,0) no frame em que o jogador solta o movimento — antes
            // da transição pra Idle acontecer — caindo na motion central e "flashando" pra
            // qualquer direção que estivesse na origem do Blend Tree.
            animator.SetFloat(MoveXHash, facing.x);
            animator.SetFloat(MoveYHash, facing.y);
            animator.SetFloat(LasXHash, facing.x);
            animator.SetFloat(LastYHash, facing.y);
            animator.SetFloat(SpeedHash, speed);
            animator.SetBool(IsRunningHash, controller.IsSprinting);
        }
    }
}

