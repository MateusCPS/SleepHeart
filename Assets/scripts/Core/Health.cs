using System;
using UnityEngine;

namespace CoracaoAdormecido.Combat
{
    /// <summary>
    /// Vida genérica (GDD 7.3 - "Quantidade de dano suportada antes da queda").
    /// Usado tanto pelo Player quanto por inimigos, para não duplicar a mesma lógica em dois
    /// lugares. Não sabe nada sobre quem causou o dano nem sobre animação — apenas expõe
    /// eventos para que HUD, combate e IA reajam de forma desacoplada (GDD 13.3).
    ///
    /// O tingimento do sprite conforme a vida cai é feedback de teste, assim como desativar o
    /// GameObject ao morrer — trocar por animação/VFX de verdade quando existirem.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        [Header("Feedback visual provisório")]
        [Tooltip("Fração da vida máxima (0-1) abaixo da qual o sprite fica amarelo.")]
        [Range(0f, 1f)] [SerializeField] private float yellowThreshold = 0.75f;
        [Tooltip("Fração da vida máxima (0-1) abaixo da qual o sprite fica vermelho.")]
        [Range(0f, 1f)] [SerializeField] private float redThreshold = 0.5f;

        public int MaxHealth => maxHealth;
        public int CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0;

        /// <summary>Disparado a cada dano recebido: quantidade aplicada e quem causou (pode ser null).</summary>
        public event Action<int, GameObject> Damaged;

        /// <summary>Disparado uma única vez, quando a vida chega a zero.</summary>
        public event Action Died;

        private SpriteRenderer spriteRenderer;
        private Color originalColor;

        private void Awake()
        {
            CurrentHealth = maxHealth;

            // Cacheado uma vez em vez de GetComponent a cada dano. Fica null se este Health
            // estiver num objeto pai sem sprite próprio (ex.: um "Enemy" vazio com o visual
            // como filho separado) — nesse caso o feedback de cor é ignorado, sem erro.
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
        }

        public void TakeDamage(int amount, GameObject source = null)
        {
            if (IsDead || amount <= 0) return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            Debug.Log($"[Health] {name} recebeu {amount} de dano de {(source != null ? source.name : "desconhecido")}. Vida: {CurrentHealth}/{maxHealth}");
            UpdateColorFeedback();
            Damaged?.Invoke(amount, source);

            if (CurrentHealth <= 0)
            {
                gameObject.SetActive(false); // placeholder: trocar por animação de morte quando existir
                Died?.Invoke();
            }
        }

        private void UpdateColorFeedback()
        {
            if (spriteRenderer == null) return;

            float healthFraction = (float)CurrentHealth / maxHealth;

            // Limiar mais severo primeiro: com <=75% checado antes, <=50% nunca seria alcançado,
            // já que todo valor <=50% também é <=75%.
            if (healthFraction <= redThreshold)
                spriteRenderer.color = Color.red;
            else if (healthFraction <= yellowThreshold)
                spriteRenderer.color = Color.yellow;
            else
                spriteRenderer.color = originalColor;
        }

        /// <summary>Para curas, santuários (GDD 7.7) ou reinício de encontro de teste.</summary>
        public void Heal(int amount)
        {
            if (IsDead || amount <= 0) return;
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            UpdateColorFeedback();
        }

        public void ResetHealth()
        {
            CurrentHealth = maxHealth;
            UpdateColorFeedback(); // volta a cor original, apaga o tingimento de uma luta anterior
        }
    }
}