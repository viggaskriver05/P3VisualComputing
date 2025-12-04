using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FearMetricsLogger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("EmotionOscListener der modtager OSC-data fra DeepFace")]
    public EmotionOscListener emotionListener;

    [Header("Logging")]
    [Tooltip("Filnavn for metrics-log (en pr. session/testperson er anbefalet).")]
    public string logFileName = "fear_metrics.json";

    [Tooltip("Tilføj automatisk tidsstempel til filnavn (så du får en ny fil pr. session).")]
    public bool appendTimestampToFileName = true;

    [Serializable]
    public class FearEpisode
    {
        public float startTime;   // sekunder siden session start
        public float endTime;     // sekunder siden session start
        public float duration;    // endTime - startTime
    }

    [Serializable]
    public class FearMetrics
    {
        public int fearEventCount;        // hvor mange gange vi går ind i fear-state
        public float totalFearTime;       // samlet tid i sekunder i fear-state
        public float sessionDuration;     // samlet spilletid
        public List<FearEpisode> episodes = new List<FearEpisode>();
    }

    FearMetrics _metrics;
    float _sessionStartTime;
    bool _inFear = false;
    float _fearStartTime;

    void Awake()
    {
        // Find EmotionOscListener automatisk hvis ikke sat i Inspector
        if (emotionListener == null)
            emotionListener = FindObjectOfType<EmotionOscListener>();

        _metrics = new FearMetrics();
    }

    void Start()
    {
        _sessionStartTime = Time.time;
    }

    void Update()
    {
        if (emotionListener == null) return;

        // Vælg hvad du vil tracke på: CurrentEmotion eller DominantEmotion
        string emotion = emotionListener.DominantEmotion;   // eller emotionListener.CurrentEmotion;

        bool isFear = !string.IsNullOrEmpty(emotion) &&
                      emotion.Equals("fear", StringComparison.OrdinalIgnoreCase);

        // Transition: ikke-fear → fear
        if (isFear && !_inFear)
        {
            _inFear = true;
            _fearStartTime = Time.time;
            _metrics.fearEventCount++;
        }
        // Transition: fear → noget andet
        else if (!isFear && _inFear)
        {
            EndFearEpisode();
        }
    }

    void EndFearEpisode()
    {
        _inFear = false;
        float end = Time.time;
        float duration = end - _fearStartTime;

        _metrics.totalFearTime += duration;

        _metrics.episodes.Add(new FearEpisode
        {
            startTime = _fearStartTime - _sessionStartTime,
            endTime = end - _sessionStartTime,
            duration = duration
        });
    }

    void OnApplicationQuit()
    {
        FinalizeAndSave();
    }

    void OnDisable()
    {
        // Hvis scenen skifter eller objektet disables, vil vi også gerne gemme
        FinalizeAndSave();
    }

    void FinalizeAndSave()
    {
        if (_metrics == null) return;

        // Hvis vi stadig er i fear når spillet lukker, afslut episode
        if (_inFear)
        {
            EndFearEpisode();
        }

        _metrics.sessionDuration = Time.time - _sessionStartTime;

        // Byg filnavn
        string fileName = logFileName;
        if (appendTimestampToFileName)
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            // f.eks. fear_metrics_20251125_193000.json
            string baseName = Path.GetFileNameWithoutExtension(logFileName);
            string ext = Path.GetExtension(logFileName);
            if (string.IsNullOrEmpty(ext)) ext = ".json";
            fileName = $"{baseName}_{timeStamp}{ext}";
        }

        // Gem i en undermappe i persistentDataPath (nemt at finde per build)
        string folder = Path.Combine(Application.persistentDataPath, "EmotionMetrics");
        Directory.CreateDirectory(folder);

        string fullPath = Path.Combine(folder, fileName);
        string json = JsonUtility.ToJson(_metrics, true);

        File.WriteAllText(fullPath, json);

        Debug.Log($"[FearMetricsLogger] Saved fear metrics to: {fullPath}");
    }
}
