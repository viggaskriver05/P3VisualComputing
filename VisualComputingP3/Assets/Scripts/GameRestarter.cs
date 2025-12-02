using UnityEngine;

public class GameRestarter : MonoBehaviour
{
    public GameObject soundAlarm1;   
    public GameObject Speach0;
    public GameObject Speach1;
    public GameObject Speach2;
    public GameObject Speach3;
    public GameObject Speach4;
 

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (soundAlarm1 != null) soundAlarm1.SetActive(false);
        if (Speach0 != null) Speach0.SetActive(false);
        if (Speach1 != null) Speach1.SetActive(false);
        if (Speach2 != null) Speach2.SetActive(false);
        if (Speach3 != null) Speach3.SetActive(false);
        if (Speach4 != null) Speach4.SetActive(false);

        Debug.Log("Sound alarms deactivated");
    }
}
