using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using HoloPlaySDK;

namespace HoloPlaySDK_UI
{
    public class HoloPlayCalibrationCard : MonoBehaviour
    {
        public RectTransform hideTail;
        public Color[] colors;
        string paramGoName = "Param Slider (HoloPlay Calibration)";

        void OnEnable()
        {
            HoloPlay.onLoadConfig += ResetParamSliders;
        }

        void OnDisable()
        {
            HoloPlay.onLoadConfig -= ResetParamSliders;
        }

        void Start()
        {
            ResetParamSliders();
        }

        public void ResetParamSliders()
        {
            foreach (Transform t in transform)
            {
                if (t.gameObject.name == paramGoName) Destroy(t.gameObject);
            }

            FieldInfo[] configFields = typeof(HoloPlayConfig).GetFields();
            List<FieldInfo> configFieldsList = new List<FieldInfo>();
            for (int i = 0; i < configFields.Length; i++)
            {
                if (configFields[i].FieldType == typeof(HoloPlayConfig.ConfigValue))
                {
                    configFieldsList.Add(configFields[i]);
                }
            }
            configFields = configFieldsList.ToArray();
            float paramYPos = 0f;
            int colorIndex = 0;
            GameObject paramSliderGO = (GameObject)Resources.Load("Param Slider (HoloPlay Calibration)");
            for (int i = 0; i < configFields.Length; i++)
            {
                var go = Instantiate(paramSliderGO);
                go.name = "Param Slider (HoloPlay Calibration)";
                go.transform.SetParent(this.transform, false);

                var goRect = go.GetComponent<RectTransform>();
                goRect.anchoredPosition += Vector2.down * paramYPos;
                paramYPos += goRect.rect.height;

                var ps = go.GetComponent<HoloPlayParamSlider>();
                ps.SetConfigValue((HoloPlayConfig.ConfigValue)configFields[i].GetValue(HoloPlay.Config));
                ps.SetColor(colors[colorIndex]);

                colorIndex++;
                colorIndex = colorIndex % 2;
            }

            hideTail.anchoredPosition = Vector2.down * paramYPos;
        }
    }
}