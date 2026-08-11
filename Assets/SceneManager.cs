using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Tooltip("Nome exato da cena a carregar (bate com o nome em Build Settings). Pode ser sobrescrito pelo GameManager.")]
    [SerializeField] private string sceneToLoad = "CombatScene";
    public void Configure(string newSceneToLoad)
    {
        sceneToLoad = newSceneToLoad;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            SceneManager.LoadScene(sceneToLoad);
    }
}