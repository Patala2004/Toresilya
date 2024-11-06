using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{

    [SerializeField] private Texture2D gameCursor = null;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(gameCursor, new Vector2(0,0),CursorMode.Auto);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = MousePos;
    }
}
