using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    private Rigidbody rb;
    private Canvas canvas;
    private GameObject canvasObjectHolder;
    private GameObject 
    [SerializeField] private Vector3 canvasSpawnPosition;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void CreateWorldSpaceCanvas()
    {
        //create object
        canvasObjectHolder = new GameObject("WorldSpaceCanvasHold");
        canvasObjectHolder.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;
      //  canvas = canvasObjectHolder.GetComponent<Canvas>();
        
        canvasObjectHolder.AddComponent<CanvasScaler>();
        canvasObjectHolder.AddComponent<GraphicRaycaster>();
        
        //Spawn object
        Instantiate(canvasObjectHolder, canvasSpawnPosition, Quaternion.identity);

        // set text
        
        
    }
    
    
}
