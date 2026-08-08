using UnityEngine;
using UnityEngine.InputSystem;

namespace CoracaoAdormecido.Core
{
    /// <summary>
    /// Movimento top-down em oito direções para o protótipo cinza (GDD 14.4, Passo 1:
    /// "Movimento, câmera, colisão e interação").
    ///
    /// Usa o novo Input System criado via código, sem depender de um asset .inputactions,
    /// para simplificar a configuração inicial do slice de uma classe só. Quando o
    /// remapeamento total de controles entrar em produção (GDD 11.3 - Acessibilidade),
    /// migrar para um InputActionAsset compartilhado entre as cinco classes.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movimento (GDD 7.2 - Controles-base)")]
        [Tooltip("Velocidade máxima em unidades/segundo. Ponto de partida para playtest.")]
        [SerializeField] private float moveSpeed = 4.5f;
        [SerializeField] private float acceleration = 40f;
        [SerializeField] private float deceleration = 45f;

        [Header("Sprint")]
        [Tooltip("Multiplicador aplicado sobre moveSpeed enquanto o sprint está pressionado e o jogador se move.")]
        [SerializeField] private float sprintMultiplier = 1.6f;

        private Rigidbody2D rb;
        private InputAction moveAction;
        private InputAction sprintAction;
        private Vector2 moveInput;
        private Vector2 currentVelocity;

        /// <summary>
        /// Última direção não nula. Será usada por combate e interação de classe
        /// (ex.: mirar ataque, direção de esquiva) nos próximos passos da GDD 14.4.
        /// Também alimenta os parâmetros LasX/LastY do Blend Tree de Idle.
        /// </summary>
        public Vector2 FacingDirection { get; private set; } = Vector2.down;

        /// <summary>
        /// Input bruto deste frame (-1..1 em cada eixo, já normalizado na diagonal).
        /// Alimenta os parâmetros MoveX/MoveY/Speed do Animator.
        /// </summary>
        public Vector2 CurrentMoveInput => moveInput;

        /// <summary>
        /// True enquanto o botão de sprint está pressionado E o jogador está se movendo.
        /// Parado, segurar sprint não conta como "correndo". Alimenta o parâmetro IsRunning do Animator.
        /// </summary>
        public bool IsSprinting { get; private set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // jogo top-down: sem gravidade
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate; // suaviza o movimento em FPS variável

            moveAction = new InputAction("Move");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            moveAction.AddBinding("<Gamepad>/leftStick");

            sprintAction = new InputAction("Sprint");
            sprintAction.AddBinding("<Keyboard>/leftShift");
            sprintAction.AddBinding("<Gamepad>/leftStickPress");
        }

        private void OnEnable()
        {
            moveAction.Enable();
            sprintAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
            sprintAction.Disable();
        }

        private void Update()
        {
            moveInput = moveAction.ReadValue<Vector2>();

            // Evita que o movimento diagonal seja mais rápido que o cardinal
            // (GDD 7.2: "Movimento em oito direções").
            if (moveInput.sqrMagnitude > 1f)
                moveInput.Normalize();

            if (moveInput.sqrMagnitude > 0.01f)
                FacingDirection = moveInput;

            // Sprint só conta como "correndo" se o jogador estiver de fato se movendo
            // (parado, segurar Shift não deve acionar a animação de corrida).
            IsSprinting = sprintAction.IsPressed() && moveInput.sqrMagnitude > 0.01f;
        }

        private void FixedUpdate()
        {
            float effectiveSpeed = IsSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
            Vector2 targetVelocity = moveInput * effectiveSpeed;
            float rate = moveInput.sqrMagnitude > 0.01f ? acceleration : deceleration;
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);

            // Unity 2022 LTS usa rb.velocity. Em Unity 6 / 2023.3+ a API foi renomeada para rb.linearVelocity —
            // troque a linha abaixo se o projeto estiver em Unity 6.
            rb.velocity = currentVelocity;
        }
    }
}
