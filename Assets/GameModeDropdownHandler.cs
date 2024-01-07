using System.Collections;
using System.Collections.Generic;
using Nobi.UiRoundedCorners;
using TMPro;
using UnityEngine;

public class GameModeDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private Transform dropdownList;
    private bool isOpen;

    void Start()
    {
        isOpen = false;
        //dropdown.Show()
        //dropdown.Show.AddListener(OnDropdownShow);
        //dropdown.Hide.AddListener(OnDropdownHide);
    }

    void Update()
    {
        CheckDropdownState();
    }

    void CheckDropdownState()
    {
        // Try to find the "Dropdown List" child
        dropdownList = dropdown.transform.Find("Dropdown List");

        if (dropdownList != null && isOpen == false)
        {
            // Dropdown is open
            OnDropdownShow();
            // Additional logic for when the dropdown is opened
            isOpen = true;
        }
        else if (dropdownList == null && isOpen == true)
        {
            // Dropdown is closed
            OnDropdownHide();
            // Additional logic for when the dropdown is closed
            isOpen = false;
        }
    }

    private void OnDropdownShow() {
        dropdown.GetComponent<ImageWithIndependentRoundedCorners>().r = new Vector4(40f, 40f, 0f, 0f);
        dropdown.GetComponent<ImageWithIndependentRoundedCorners>().Refresh();
    }

    private void OnDropdownHide() {
        dropdown.GetComponent<ImageWithIndependentRoundedCorners>().r = new Vector4(40f, 40f, 40f, 40f);
        dropdown.GetComponent<ImageWithIndependentRoundedCorners>().Refresh();
    }
}
