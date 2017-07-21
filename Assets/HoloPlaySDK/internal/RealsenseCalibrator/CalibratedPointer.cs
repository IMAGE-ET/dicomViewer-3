using System.Collections;
using System.Collections.Generic;

using HoloPlaySDK;

using UnityEngine;

namespace HoloPlaySDK_UI
{
    public class CalibratedPointer : MonoBehaviour
    { 
        void Update()
        {
            transform.position = RealsenseCalibrator.Instance.GetWorldPos(0);
        }
    }
}