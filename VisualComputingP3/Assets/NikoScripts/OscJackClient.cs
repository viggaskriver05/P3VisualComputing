using UnityEngine;
using OscJack;

public class EmotionOscListener : MonoBehaviour
{
    [SerializeField] int listenPort = 9000; 
    OscServer _server;

    public string CurrentEmotion { get; private set; } = "unknown";

    void OnEnable()
    {
        _server = new OscServer(listenPort);

        // Listen for: /emotion "<label>"
        _server.MessageDispatcher.AddCallback("/emotion", (address, data) =>
        {
            string label = data.GetElementAsString(0);
            CurrentEmotion = label;
            Debug.Log($"/emotion → {label}");
        });
    }

    void OnDisable()
    {
        _server?.Dispose();
        _server = null;
    }
}