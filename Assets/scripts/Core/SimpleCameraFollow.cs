using UnityEngine;

namespace CoracaoAdormecido.Core
{
    /// <summary>
    /// Câmera de seguimento provisória para o protótipo cinza (GDD 14.4, Passo 1).
    ///
    /// Substituir por um CinemachineVirtualCamera assim que o pacote Cinemachine for
    /// configurado (GDD 13.1) — este script existe só para validar movimento e colisão sem
    /// bloquear o trabalho na dependência do pacote. Ele não deve sobreviver até o vertical slice.
    /// </summary>
    public class SimpleCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        private Vector3 velocity;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }
    }
}
