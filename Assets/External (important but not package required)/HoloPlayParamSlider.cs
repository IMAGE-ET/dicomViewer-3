using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HoloPlaySDK;

namespace HoloPlaySDK_UI
{
    public class HoloPlayParamSlider : MonoBehaviour
    {
        public HoloPlayConfig.ConfigValue configValue;
        Slider slider;
        InputField valueInput;
        Button defaultButton;
        Button plusButton;
        Button minusButton;
        InputField intervalInput;
        Text title;

        public void SetColor(Color color)
        {
            transform.Find("bg").GetComponent<Image>().color = color;
        }

        public void SetConfigValue(HoloPlayConfig.ConfigValue configValueIn)
        {
            this.configValue = configValueIn;

            slider = transform.Find("slider").GetComponent<Slider>();
            valueInput = transform.Find("value").GetComponent<InputField>();
            defaultButton = transform.Find("default").GetComponent<Button>();
            plusButton = transform.Find("plus").GetComponent<Button>();
            minusButton = transform.Find("minus").GetComponent<Button>();
            intervalInput = transform.Find("interval").GetComponent<InputField>();
            title = transform.Find("title").GetComponent<Text>();

            valueInput.characterValidation = configValue.isInt
                ? InputField.CharacterValidation.Integer
                : InputField.CharacterValidation.Decimal;
            valueInput.onEndEdit.AddListener(
                arg => { configValue.Value = float.Parse(arg); HoloPlay.SaveConfigToFile(); }
            );

            title.text = configValue.name;

            slider.minValue = configValue.min;
            slider.maxValue = configValue.max;
            slider.wholeNumbers = configValue.isInt;
            slider.value = configValue.Value;
            slider.onValueChanged.AddListener(
                arg => configValue.Value = arg
            );

            defaultButton.onClick.AddListener(
                () => { configValue.Value = configValue.defaultValue; HoloPlay.SaveConfigToFile(); }
            );

            if (configValue.isInt)
                intervalInput.gameObject.SetActive(false);

            plusButton.onClick.AddListener(
                () => { configValue.Value += configValue.isInt ? 1 : float.Parse(intervalInput.text); HoloPlay.SaveConfigToFile(); }
            );

            minusButton.onClick.AddListener(
                () => { configValue.Value -= configValue.isInt ? 1 : float.Parse(intervalInput.text); HoloPlay.SaveConfigToFile(); }
            );
        }

        void Update()
        {
            slider.value = configValue.Value;
            if (!valueInput.isFocused)
                valueInput.text = configValue.Value.ToString(configValue.isInt ? "0" : "0.0000000");
        }
    }
}
