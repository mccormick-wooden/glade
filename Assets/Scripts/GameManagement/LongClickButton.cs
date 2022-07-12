using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private float requiredHoldTime;

    [SerializeField]
    private Image fillImage;

    [SerializeField]
    private bool debugOutput;

    private bool pointerDown;
    private float pointerDownTimer;

    public Action LongClickComplete;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
        if (debugOutput)
            Debug.Log("OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();
        if (debugOutput)
            Debug.Log("OnPointerUp");
    }

    private void Update()
    {

        if (pointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            if (pointerDownTimer >= requiredHoldTime)
            {
                LongClickComplete?.Invoke();

                if (debugOutput)
                    Debug.Log("LongClickComplete Invoked");

                Reset();
            }

            AdjustImageFill();
        }
    }

    private void Reset()
    {
        pointerDown = false;
        pointerDownTimer = 0;
        AdjustImageFill();
    }

    private void AdjustImageFill()
    {
        if (fillImage != null)
            fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    }
}
