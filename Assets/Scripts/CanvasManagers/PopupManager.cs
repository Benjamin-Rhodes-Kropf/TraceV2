using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PopupManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]private Vector2 pointerOffset;
    [SerializeField]private Vector2 pointerDownPosition;
    [SerializeField]private float threshold, panelSpeed;
    [SerializeField]private RectTransform canvasRectTransform;
    [SerializeField]private RectTransform panelRectTransform;
    [SerializeField]private bool onDeploy, onFold = false;
    public bool isDeployed = false;
    public bool isFolded = true;
    public bool startDeployed;
    [SerializeField]public float homePositionOffset = 1f;
    [SerializeField]public bool disableOnFold = true;
    [SerializeField]private RectTransform inactiveParent;

    void Awake()
    {
        // Canvas canvas = GetComponentInParent<Canvas>();
        // if (canvas != null)
        // {
        //     canvasRectTransform = canvas.transform as RectTransform;
        //     panelRectTransform = transform as RectTransform;
        // }      
    }
 
    void OnEnable()
    {
        // panelSpeed = canvasRectTransform.rect.height * 0.025f;
        threshold = canvasRectTransform.rect.height * 0.10f;
        
        //make y value a decimal to show on enabled
        if (!startDeployed)
        {
            panelRectTransform.localPosition = new Vector3(panelRectTransform.localPosition.x,-homePositionOffset * canvasRectTransform.rect.height, panelRectTransform.localPosition.z);
        }
        else
        {
            panelRectTransform.localPosition = new Vector3(panelRectTransform.localPosition.x,0, panelRectTransform.localPosition.z);
        }
        onDeploy = true;
    }
    void Update()
    {      
        if (onDeploy)
        {
            if (panelRectTransform.localPosition.y < 0)
                panelRectTransform.localPosition += (panelSpeed * new Vector3(0, 1, 0));
            else
            {
                panelRectTransform.localPosition = new Vector3(panelRectTransform.localPosition.x,0.0f, panelRectTransform.localPosition.z);
                isDeployed = true;
                isFolded = !isDeployed;
                onDeploy = false;
                onFold = false;
            }
        }
        if (onFold)
        {
            if (panelRectTransform.localPosition.y > (-homePositionOffset * canvasRectTransform.rect.height))
                panelRectTransform.localPosition += (panelSpeed * new Vector3(0, -1, 0));
            else
            {
                panelRectTransform.localPosition = new Vector3(panelRectTransform.localPosition.x,-homePositionOffset * canvasRectTransform.rect.height, panelRectTransform.localPosition.z);
                isFolded = true;
                isDeployed = !isFolded;
                onDeploy = false;
                onFold = false;
                if (disableOnFold)
                { 
                    panelRectTransform.SetParent(inactiveParent);
                }
            }          
        }
    }
 
    public void OnPointerDown(PointerEventData data)
    {
        panelRectTransform.SetAsLastSibling();
        pointerDownPosition = data.position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out pointerOffset);      
    }
 
    public void OnPointerUp(PointerEventData data)
    {
        Vector2 pointerDelta = data.position - pointerDownPosition;
        if (Mathf.Abs(pointerDelta.y) > threshold)
        {
            if (pointerDelta.y > 0)
            {
                if (!isDeployed)
                {
                    onDeploy = true;
                    onFold = !onDeploy;
                }
            }
            else
            {
                if (!isFolded)
                {
                    onFold = true;
                    onDeploy = !onFold;
                }
            }
        }
        else
        {
            if (pointerDelta.y > 0)
            {
                if (!isDeployed)
                {
                    onFold = !onDeploy;
                    onDeploy = !onFold;
                }
            }
            else
            {
                if (!isFolded)
                {
                    onDeploy = !onFold;
                    onFold = !onDeploy;                  
                }
            }
        }      
    }

    public void OnDrag(PointerEventData data)
    {
        if (panelRectTransform == null)
            return;
 
        //Vector2 pointerPostion = ClampToWindow(data);
        isDeployed = false;
        isFolded = false;
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, data.position, data.pressEventCamera, out localPointerPosition
        ))
        {
            Vector3 newPos = new Vector3(panelRectTransform.localPosition.x,localPointerPosition.y - pointerOffset.y, panelRectTransform.localPosition.z);
            //panelRectTransform.localPosition = localPointerPosition - pointerOffset;
            if (newPos.y >= 0)
            {
                panelRectTransform.localPosition = new Vector3(panelRectTransform.localPosition.x,0.0f, panelRectTransform.localPosition.z);
                isDeployed = true;
                //isFolded = !isDeployed;
                onDeploy = false;
                onFold = false;
            }
            else if (newPos.y <= (-homePositionOffset * canvasRectTransform.rect.height))
            {
                panelRectTransform.localPosition = new Vector3(panelRectTransform.localPosition.x,-homePositionOffset * canvasRectTransform.rect.height, panelRectTransform.localPosition.z);
                isFolded = true;
                //isDeployed = !isFolded;
                onDeploy = false;
                onFold = false;
                if (disableOnFold)
                { 
                    panelRectTransform.SetParent(inactiveParent);
                }
                Debug.Log("Popup: closed");
            }
            else
                panelRectTransform.localPosition = newPos;
        }
    }
    
    public void ClosePop()
    {
        onFold = true;
    }
}


