using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

// Reads lines from the Arduino over USB and prints them in the Unity Console.
// Put this on an empty GameObject in your scene and press Play.
public class ArduinoSerial : MonoBehaviour
{
    [Tooltip("Leave empty to auto-detect (Mac: /dev/cu.usbmodem...). Must match Arduino IDE > Tools > Port.")]
    [SerializeField] private string portName = "";
    [SerializeField] private int baudRate = 9600; // must match Serial.begin(9600)

    [Header("Heartbeat sound")]
    [SerializeField] private AudioSource audioSource;   // leave empty to use one on this GameObject
    [SerializeField] private AudioClip heartbeatClip;   // a single "lub-dub"
    [SerializeField] private bool logBeats = true;

    // Latest heart rate, readable from other scripts: GetComponent<ArduinoSerial>().CurrentBPM
    public int CurrentBPM { get; private set; }

    // Other scripts can react to every beat: arduino.OnBeat += MyMethod;
    public event Action OnBeat;

    private SerialPort port;
    private Thread readThread;
    private volatile bool running;
    private readonly ConcurrentQueue<string> lines = new ConcurrentQueue<string>();

    private void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (string.IsNullOrEmpty(portName)) portName = FindPort();
        if (string.IsNullOrEmpty(portName))
        {
            Debug.LogError("[Arduino] No port found. Is the Arduino plugged in?");
            return;
        }

        port = new SerialPort(portName, baudRate)
        {
            ReadTimeout = 500,
            NewLine = "\n",
            DtrEnable = true
        };

        try
        {
            port.Open();
            Debug.Log($"[Arduino] Connected on {portName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Arduino] Could not open {portName}: {e.Message}\n" +
                           "Close the Arduino IDE Serial Monitor - only one program can use the port.");
            return;
        }

        running = true;
        readThread = new Thread(ReadLoop) { IsBackground = true };
        readThread.Start();
    }

    // Runs in the background so the game never freezes waiting for data.
    private void ReadLoop()
    {
        while (running)
        {
            try
            {
                string line = port.ReadLine().Trim();
                if (line.Length > 0) lines.Enqueue(line);
            }
            catch (TimeoutException) { }
            catch (Exception e)
            {
                if (running) lines.Enqueue("ERROR: " + e.Message);
                break;
            }
        }
    }

    // Debug.Log has to be called on Unity's main thread, so it happens here.
    private void Update()
    {
        while (lines.TryDequeue(out string line))
        {
            if (line == "BEAT")
            {
                if (heartbeatClip != null) audioSource.PlayOneShot(heartbeatClip);
                OnBeat?.Invoke();
                if (logBeats) Debug.Log("[Arduino] ♥ beat");
                continue;
            }

            Debug.Log($"[Arduino] {line}");

            if (line.StartsWith("BPM:") && int.TryParse(line.Substring(4).Trim(), out int bpm))
                CurrentBPM = bpm;
        }
    }

    private static string FindPort()
    {
        // Mac
        if (Directory.Exists("/dev"))
        {
            string[] mac = Directory.GetFiles("/dev", "cu.usbmodem*");
            if (mac.Length > 0) return mac[0];
            mac = Directory.GetFiles("/dev", "cu.usbserial*");
            if (mac.Length > 0) return mac[0];
        }
        // Windows / other
        string[] ports = SerialPort.GetPortNames();
        return ports.Length > 0 ? ports[ports.Length - 1] : null;
    }

    private void OnDestroy()
    {
        running = false;
        if (readThread != null && readThread.IsAlive) readThread.Join(1000);
        if (port != null && port.IsOpen) port.Close();
    }
}
