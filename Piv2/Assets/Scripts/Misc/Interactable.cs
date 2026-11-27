using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Interactable : MonoBehaviour
{
    [SerializeField] private float defaultThrowForce;
    [SerializeField] private float interactRadius = 1.5f;
    private Rigidbody rb;
    private Canvas canvas;
    private GameObject TextObj;
    private GameObject canvasObjectHolder;
    [SerializeField] private float YOffset = 1.5f;
    private void Awake()
    {
        SetupInteractable();
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        CreateWorldSpaceCanvas();        
    }

    private void SetupInteractable()
    {
        var col = GetComponent<SphereCollider>();
        col.radius = interactRadius;
        col.isTrigger = true;
        transform.tag = "Interactable";
    }

    private void CreateWorldSpaceCanvas()
    {
        //create object
        canvasObjectHolder = new GameObject("WorldSpaceCanvasHold");
        canvasObjectHolder.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        canvasObjectHolder.AddComponent<Canvas>();
        canvasObjectHolder.AddComponent<CanvasScaler>();
        canvasObjectHolder.AddComponent<GraphicRaycaster>();
        canvasObjectHolder.AddComponent<Billboard>();

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
        textMesh.fontSize = 3;
        textMesh.color = Color.red;
        
        //hide
        canvasObjectHolder.SetActive(false);
        TextObj.SetActive(false);
    }

    public void ThrowObject(Vector3 direction, Vector3 playerVelocity, float throwForce = 0)
    {
        if (throwForce == 0) throwForce = defaultThrowForce;

        rb.linearVelocity = playerVelocity;
        rb.AddForce(direction * throwForce, ForceMode.Impulse);
     //   Debug.DrawRay(transform.position, direction * throwForce, Color.red, 5f);
    }

    public void SetTextActive(bool b)
    {
        
        canvas.gameObject.SetActive(b);
        TextObj.gameObject.SetActive(b);
    }
}
