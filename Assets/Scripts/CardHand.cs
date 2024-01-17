using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHand : MonoBehaviour
{
    public float cardSpacing = 150; // Adjust the spacing between cards as needed
    public float maxSpread = 850.0f; // Adjust the maximum spread of cards as needed
    public float defaultYPosition = -1250f;
    public float cardSelectedScale = 2.5f;


    void Start()
    {
        ArrangeCards();
    }

    void Update()
    {
        // You can call ArrangeCards() whenever you want to update the card arrangement
        // For example, you might want to call it when the number of cards changes.
        // For simplicity, we'll call it in Update here.
        ArrangeCards();
    }

    void ArrangeCards()
    {
        int cardCount = transform.childCount;

        if (cardCount == 0)
        {
            return; // No cards to arrange
        }

        //float totalSpread = Mathf.Min(cardCount * cardSpacing, maxSpread);
        float totalSpread = Mathf.Min(cardCount * cardSpacing, maxSpread);
        float halfSpread = totalSpread / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            Transform card = transform.GetChild(i);
            Vector3 cardPosition;
            Vector3 cardScale;
            Quaternion cardRotation;

            if (card.GetComponent<CardHandler>().GetIsChecked()) {
                cardPosition = new Vector3(0f, 0f, 0f);
                cardRotation = Quaternion.Euler(0f, 0f, 0f);
                cardScale = new Vector3(cardSelectedScale, cardSelectedScale, 0f);
            } else {
                float t = cardCount > 1 ? i / (float)(cardCount - 1) : 0f; // Avoid division by zero
                float xPos = Mathf.Lerp(-halfSpread, halfSpread, t);

                if (cardCount == 1)
                {
                    xPos = 0f;
                }
                
                float zRot = xPos / 100f;

                cardPosition = new Vector3(xPos, defaultYPosition - Math.Abs(zRot) * 5, card.localPosition.z); // You can modify the Y and Z positions as needed
                cardRotation = Quaternion.Euler(0f, 0f, -zRot);
                cardScale = Vector3.one;
            }

            card.localPosition = cardPosition;
            card.localRotation = cardRotation;
            card.localScale = cardScale;
            
        }
    }
}
