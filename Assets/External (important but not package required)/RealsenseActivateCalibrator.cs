using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RealsenseActivateCalibrator : MonoBehaviour
{
    public RealsenseCalibrator rs;

    void Awake()
    {
        rs.activelyCalibrating = true;
    }
}