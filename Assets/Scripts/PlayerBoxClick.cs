using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoxClick : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask CheckLayer;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerTurn();
    }

    void PlayerTurn()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100, CheckLayer))
            {
                CheckBox _box = hit.collider.GetComponent<CheckBox>();

                if (_box != null)
                {

                    _box.OnClicked();
                }
            }
        }
    }
}
