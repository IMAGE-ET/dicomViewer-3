using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HoloPlaySDK;

namespace HoloPlaySDK_UI
{
    public class HoloPlaySaveOnPointerUp : MonoBehaviour, IPointerUpHandler
    {
        public void OnPointerUp(PointerEventData data)
        {
            if (HoloPlay.Main != null)
            {
                HoloPlay.SaveConfigToFile();
            }
        }
    }
}