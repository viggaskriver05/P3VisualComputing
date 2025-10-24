using UnityEngine;

public class BoxColor : InteractionScript
{
    private Renderer render;
    private Color colorRed = Color.red;
    private Color colorBlue = Color.blue;
    private bool isRed = true;

    void Start()
    {
        render = GetComponent<Renderer>();
        render.material.color = colorRed;
    }

    public override void Interact()
    {
        Debug.Log("Box Interaction.");

        if (isRed)
        {
            render.material.color = colorBlue;
        }
        else
        {
            render.material.color = colorRed;
        }
        isRed = !isRed;
    }
}
