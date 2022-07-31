using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseSelect : MonoBehaviour, IPointerEnterHandler
{
     public void OnPointerEnter(PointerEventData eventData)
     {
         // Mousing over a button "Highlights", selecting via keyboard/gamepad
         // "Selects". That distinction is annoying. Make the mouse Select on
         // mouseover by attaching this script to the button
        GetComponent<Button>().Select();
     }
}
