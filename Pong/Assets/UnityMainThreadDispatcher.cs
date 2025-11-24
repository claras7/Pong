using System;
using System.Collections.Concurrent;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher instance;

    private static readonly ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

    void Awake()
    {
        // Garante que só exista um Dispatcher
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Enfileira uma ação para rodar na thread principal do Unity.
    /// </summary>
    public static void Enqueue(Action action)
    {
        if (action == null) return;
        actions.Enqueue(action);
    }

    void Update()
    {
        // Executa todas ações pendentes na thread principal
        while (actions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }
}
