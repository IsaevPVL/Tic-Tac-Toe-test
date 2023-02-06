using UnityEngine;

public sealed class InputListener : MonoBehaviour
{
    [SerializeField] PlayArea playArea;
    Camera cam;
    Vector3 mousePosition;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        playArea.SelectCell(mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            playArea.PlaceSymbol();
        }
    }
}