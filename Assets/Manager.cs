using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Serializable]
    private class SceneEncounter
    {
        [Tooltip("O GameObject na cena que tem o componente SceneTrigger.")]
        public GameObject trigger;

        [Tooltip("Nome exato da cena a carregar quando o jogador entrar nesse gatilho (bate com Build Settings).")]
        public string sceneToLoad;
    }

    [SerializeField] private List<SceneEncounter> encounters = new List<SceneEncounter>();

    private void Awake()
    {
        foreach (var encounter in encounters)
        {
            if (encounter.trigger == null)
            {
                Debug.LogWarning("[GameManager] Um item da lista de encontros está sem GameObject de gatilho atribuído.");
                continue;
            }

            if (!encounter.trigger.TryGetComponent(out SceneTrigger sceneTrigger))
            {
                Debug.LogWarning($"[GameManager] '{encounter.trigger.name}' não tem o componente SceneTrigger.");
                continue;
            }

            sceneTrigger.Configure(encounter.sceneToLoad);
        }
    }
}
