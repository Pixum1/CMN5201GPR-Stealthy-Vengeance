using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuMouse : MonoBehaviour
{
    [SerializeField] private GameObject CursorObject;
    private Vector3 mousePos;

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);

        CursorObject.transform.position = mousePos;
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }
}
