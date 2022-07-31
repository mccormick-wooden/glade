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

    private bool longClickCompleted = false;

    private CharacterPlayerControls controls;

    private void Awake()
    {
        Reset();
        controls = new CharacterPlayerControls();
        controls.Gameplay.Disable();
        controls.SkipScene.Enable();
    }

    private void Start()
    {
        controls.SkipScene.SkipSceneAction.started += _ => OnPointerDown(null);
        controls.SkipScene.SkipSceneAction.canceled += _ => OnPointerUp(null);
    }

    private void OnDestroy()
    {
        controls.SkipScene.SkipSceneAction.started -= _ => OnPointerDown(null);
        controls.SkipScene.SkipSceneAction.canceled -= _ => OnPointerUp(null);
    }

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
                if (!longClickCompleted)
                {
                    LongClickComplete?.Invoke();

                    longClickCompleted = true;

                    if (debugOutput)
                        Debug.Log("LongClickComplete Invoked");
                }
            }

            AdjustImageFill();
        }
    }

    private void Reset()
    {
        if (!longClickCompleted)
        {
            pointerDown = false;
            pointerDownTimer = 0;
            AdjustImageFill();
        }
    }

    private void AdjustImageFill()
    {
        if (fillImage != null)
            fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    }
}
