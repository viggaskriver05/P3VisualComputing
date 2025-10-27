using UnityEngine;
using OscJack;
using System.Collections.Generic;

public class EmotionOscListener : MonoBehaviour
{
    [SerializeField] int listenPort = 9000; 
    [Tooltip("How many frames/messages to consider when computing the dominant emotion.")]
    [SerializeField] int sampleWindowFrames = 10;
    OscServer _server;

    public string CurrentEmotion { get; private set; } = "unknown";
    public string DominantEmotion { get; private set; } = "unknown";

    // Circular buffer for recent emotions
    string[] _buffer;
    int _index;
    int _count;

    void Awake()
    {
        if (sampleWindowFrames < 1) sampleWindowFrames = 24;
        _buffer = new string[sampleWindowFrames];
        _index = 0;
        _count = 0;
    }

    void OnEnable()
    {
        _server = new OscServer(listenPort);
        Debug.Log($"EmotionOscListener listening on UDP {listenPort} for /emotion");

        // Listen for: /emotion "<label>"
        _server.MessageDispatcher.AddCallback("/emotion", (address, data) =>
        {
            string label = data.GetElementAsString(0);
            if (string.IsNullOrEmpty(label)) label = "unknown";

            CurrentEmotion = label;

            // Push into buffer and update dominant
            Push(label);
            DominantEmotion = ComputeDominant();

            Debug.Log($"/emotion → {label} | Dominant: {DominantEmotion}");
        });
    }

    void OnDisable()
    {
        _server?.Dispose();
        _server = null;
    }

    void Push(string label)
    {
        if (_buffer == null || _buffer.Length != sampleWindowFrames)
        {
            // Re-init if user changed the window size in Inspector at runtime
            _buffer = new string[sampleWindowFrames > 0 ? sampleWindowFrames : 24];
            _index = 0;
            _count = 0;
        }

        _buffer[_index] = label;
        _index = (_index + 1) % _buffer.Length;
        if (_count < _buffer.Length) _count++;
    }

    string ComputeDominant()
    {
        if (_count == 0) return "unknown";

        Dictionary<string, int> counts = new Dictionary<string, int>();
        int length = _buffer.Length;
        int start = _count == length ? _index : 0;

        for (int i = 0; i < _count; i++)
        {
            string e = _buffer[(start + i) % length];
            if (string.IsNullOrEmpty(e)) continue;
            if (counts.TryGetValue(e, out var c)) counts[e] = c + 1; else counts[e] = 1;
        }

        string dominant = "unknown";
        int max = 0;
        foreach (var kvp in counts)
        {
            if (kvp.Value > max)
            {
                max = kvp.Value;
                dominant = kvp.Key;
            }
        }
        return dominant;
    }
}