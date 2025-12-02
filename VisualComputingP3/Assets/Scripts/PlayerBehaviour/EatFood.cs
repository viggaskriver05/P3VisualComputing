using UnityEngine;
using System.Collections;

public class EatFood : MonoBehaviour
{
    public GameObject player;
    public GameObject sitPosObj;
    public GameObject playerCamera;
    public GameObject food;
    public GameObject runFood;
    public PlayerMove playerMove;
    public GameObject introManager;
    private IntroManager introMan;


    //private Vector3 sitPos;
    //private Vector3 startPos;
    //private Quaternion sitRot;
    //private Quaternion startRot;

    private void Start()
    {
        playerMove = player.GetComponent<PlayerMove>();
        introMan = introManager.GetComponent<IntroManager>();
    }

    public IEnumerator EatFoodAnimation()
    {
        playerMove.enabled = false;
        player.GetComponent<CapsuleCollider>().enabled = false;
        
        Vector3 startPos = player.transform.position;
        Vector3 sitPos = sitPosObj.transform.position;
        Quaternion startRot = player.transform.rotation;
        Quaternion sitRot = sitPosObj.transform.rotation;

        float t = 0f;
        
        while (t < 1f)
        {
            t += Time.deltaTime / 2f; // 2 seconds to sit down
            player.transform.position = Vector3.Lerp(startPos, sitPos, t);
            player.transform.rotation = Quaternion.Slerp(startRot, sitRot, t);
            yield return null;
        }
        Quaternion cameraStartRot = playerCamera.transform.rotation;
        playerCamera.transform.localRotation = Quaternion.Slerp(cameraStartRot, Quaternion.Euler(35f, 0f, 0f), t);
        
        StartCoroutine(introMan.EyesFlutter());
        yield return new WaitForSeconds(3f); // Wait for the eyes to flutter

        foreach (Renderer r in food.GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
        runFood.SetActive(true);

        yield return new WaitForSeconds(5); //tilpas i forhold til hvor længe ai snakker
        playerMove.enabled = true;
        player.GetComponent<CapsuleCollider>().enabled = true;
    }
}
