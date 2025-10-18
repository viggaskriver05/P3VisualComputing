
using UnityEngine;
using UnityEngine.Events;

public class FootstepSystem : MonoBehaviour
{
    [Range(0, 20f)]
    public float frequency = 10.0f;

    public UnityEvent onFootStep;

    float Sin;
    bool isTriggered = false;

    void Update()
    {
        float inputMagnitude = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        ).magnitude;

        if (inputMagnitude > 0)
        {
            StartFootsteps();
        }
    }

    private void StartFootsteps()
    {
        Sin = Mathf.Sin(Time.time* 1.2f * frequency);

        if (Sin > 0 && isTriggered == false)
        {
            isTriggered = true;
            Debug.Log("Step");
            onFootStep.Invoke();
        }
        else if (isTriggered == true && Sin < -0)
        {
            isTriggered = false;
        }
    }
}
