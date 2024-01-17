using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    
    private bool isSelected;
    private bool isChecked;
    private bool isTouchInProgress;

    private Graphic cardGraphic;

    private const string selectedColor = "#D0DDFF";
    private const string unselectedColor = "#000000";

    private void Start() 
    {
        isSelected = false;
        isChecked = false;
        isTouchInProgress = false;

        cardGraphic = GetComponent<Graphic>();
    }

    // private void Update()
    // {
    //     isTouchInProgress = Input.touchCount > 0;

    //     if(!isTouchInProgress)
    //         return;

    //     // Check if the mouse button is pressed anywhere else on the screen
    //     //if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
    //     if ((/*isTouchInProgress && */!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) ||
    //         (/*!isTouchInProgress && */Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()))
    //     {
    //         isSelected = false;

    //         Color color;
    //         ColorUtility.TryParseHtmlString(unselectedColor, out color);
    //         Debug.Log(color);
    //         cardGraphic.color = color;

    //         if (isTouchInProgress && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
    //             Debug.Log(isTouchInProgress);
    //             Debug.Log("Mouse Clicked outside the UI element.");
    //         }

    //         if (!isTouchInProgress && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
    //             Debug.Log("No touch is happening");
    //         }
            
    //     }
    // }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Mouse Button Released: " + gameObject.name);
        isChecked = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Mouse Button Pressed: " + gameObject.name);

        Transform parentTransform = transform.parent;
        Transform[] siblings = GetSiblings(parentTransform);
        
        foreach (Transform sibling in siblings)
        {
            Debug.Log("Sibling: " + sibling.name);
            sibling.GetComponent<CardHandler>().SetIsSelected(false);
            sibling.GetComponent<CardHandler>().SetColor(unselectedColor);
        }
        
        isSelected = !isSelected;
        isChecked = true;

        if(isSelected) {
            SetColor(selectedColor);
        } else {
            SetColor(unselectedColor);
        }
    }

    public bool GetIsSelected()
    {
        return isSelected;
    }

    public bool GetIsChecked()
    {
        return isChecked;
    }

    public void SetIsSelected(bool isSelected)
    {
        this.isSelected = isSelected;
    }

    public void SetIsChecked(bool isChecked)
    {
        this.isChecked = isChecked;
    }

    public void SetColor(string color) 
    {
        Color newColor;
        ColorUtility.TryParseHtmlString(color, out newColor);
        cardGraphic.color = newColor;
    }

    private Transform[] GetSiblings(Transform parent)
    {
        int childCount = parent.childCount - 1; // Exclude the calling object
        Transform[] siblings = new Transform[childCount];
        int index = 0;

        foreach (Transform child in parent)
        {
            if (child != transform)
            {
                siblings[index] = child;
                index++;
            }
        }

        return siblings;
    }
}
