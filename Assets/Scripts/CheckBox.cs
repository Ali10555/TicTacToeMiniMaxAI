using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheckBox : MonoBehaviour
{
    public int Index;

    [HideInInspector]public Collider collider;

    public string Text;
    public string Depth;
    public string Points;

    [SerializeField]TicTacToeController controller;
    private void Awake()
    {
        collider = GetComponent<Collider>();
    }
    
    public void OnClicked()
    {
        if (!controller.IsAiTurn())
        {
            controller.OnButtonClick(this);
        }
    }
}
