using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Interactable : MonoBehaviour
{
    [SerializeField] private float defaultThrowForce;
    private Rigidbody rb;
    private Canvas canvas;
    private GameObject TextObj;
    private GameObject canvasObjectHolder;
    [SerializeField] private float YOffset = 1.5f;
    private void Awake()
    {
        GetComponent<SphereCollider>().radius = transform.localScale.magnitude;
        rb = GetComponent<Rigidbody>();
        CreateWorldSpaceCanvas();        
    }

    private void CreateWorldSpaceCanvas()
    {
        //create object
        canvasObjectHolder = new GameObject("WorldSpaceCanvasHold");
        canvasObjectHolder.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        canvasObjectHolder.AddComponent<Canvas>();
        canvasObjectHolder.AddComponent<CanvasScaler>();
        canvasObjectHolder.AddComponent<GraphicRaycaster>();

        canvas = canvasObjectHolder.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        
        canvasObjectHolder.transform.SetParent(this.transform);

        var newPosistion = new Vector3(transform.position.x, transform.position.y + YOffset, transform.position.z);

        canvasObjectHolder.transform.position = newPosistion; 

        //resize text obj so it looks nice and isnt huge
       TextObj = new GameObject("Text object", new []{typeof(RectTransform), typeof(TextMeshProUGUI), typeof(CanvasRenderer)});
       TextObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
       TextObj.transform.position = canvasObjectHolder.transform.position;
       TextObj.transform.SetParent(canvasObjectHolder.transform);
       
       //set text info
       var textMesh = TextObj.GetComponent<TextMeshProUGUI>();

        textMesh.text = "E";
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = 13;
        
        //hide
        canvasObjectHolder.SetActive(false);
        TextObj.SetActive(false);
    }

    public void ThrowObject(Vector3 direction, float throwForce = 0)
    {
        if (throwForce == 0) throwForce = defaultThrowForce;
        
        rb.AddForce(direction * throwForce, ForceMode.Impulse);

    }

    public void SetTextActive(bool b)
    {
        canvas.gameObject.SetActive(b);
        TextObj.gameObject.SetActive(b);
    }
}
