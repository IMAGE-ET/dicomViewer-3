using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HoloPlaySDK_UI
{
    public class HoloPlayCalibrationMovementButton : MonoBehaviour, IDragHandler
    {
        public void OnDrag(PointerEventData data)
        {
            transform.parent.Translate(data.delta);
        }
    }
}
