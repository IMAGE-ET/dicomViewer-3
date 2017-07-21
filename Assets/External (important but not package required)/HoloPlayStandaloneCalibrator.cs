using System;
using System.Collections;
using System.Collections.Generic;

using HoloPlaySDK;

using UnityEngine;
using UnityEngine.UI;

namespace HoloPlaySDK_UI
{
    public class HoloPlayStandaloneCalibrator : MonoBehaviour
    {
        class ModelOption
        {
            public string optionText;
            public float screenW;
            public float screenH;
            public float DPI;
            public float pitch;
            public float numViews;

            public ModelOption(
                string optionText,
                float screenW,
                float screenH,
                float DPI,
                float pitch,
                float numViews
            )
            {
                this.optionText = optionText;
                this.screenW = screenW;
                this.screenH = screenH;
                this.DPI = DPI;
                this.pitch = pitch;
                this.numViews = numViews;
            }
        }
        ModelOption[] modelOptions = new []
        {
            new ModelOption(
            "Alice ( 2048 x 1560, 75.1 LPI )",
            2048, 1536, 265, 75.1f, 22
            ),

            new ModelOption(
            "Caroll ( 2560 x 1600, 101.36 LPI )",
            2560, 1600, 338, 101.36f, 22
            ),

            new ModelOption(
            "Dormouse ( 2560 x 1600, 49.91 LPI )",
            2560, 1600, 338, 49.91f, 22
            )
        };

        private string card = "Select Drive";
        public KeyCode nextKey = KeyCode.Return;
        public KeyCode goBackKey = KeyCode.Backspace;
        public Dropdown driveDropdown;
        public Dropdown modelDropdown;
        public Slider pitchSlider;
        public float pitchWindow;
        public Slider centerSlider;
        public float centerWindow;
        public GameObject lenticular3Scene;
        public GameObject advancedScene;
        public static string dontSaveUSBString = "Don't save to USB";
        public string modelUseCurrentStr = "Edit Current Calibration";
        public GameObject realsenseHoloPlayGO;
        public RealsenseCalibrator realsenseCalibrator;
        public Text realsenseBottomText;
        public Text realsenseTopText;
        public Text realsenseFinalText;

        void OnEnable()
        {
            RealsenseCalibrator.onAdvanceCalibration += (x) => RealsenseTextSwitch(x);
        }

        void OnDisable()
        {
            RealsenseCalibrator.onAdvanceCalibration -= (x) => RealsenseTextSwitch(x);
        }

        // Use this for initialization
        void Start()
        {
            SetActiveCard(card);
            InitializeDriveDropdown();
            InitializeModelDropdown();
            InitializeCenterSlider();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(nextKey))
                GoNext();

            if (Input.GetKeyDown(goBackKey))
                GoBack();

            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }

        //next is used when moving forward is linear
        //i.e. when there are no multiselects for the next card
        public void GoNext()
        {
            switch (card)
            {
                case "Select Drive":
                    SetActiveCard("Select Calibrator");
                    break;
                    //no other cards use the next button
                case "Lenticular":
                    //load the selected prefab config
                    LoadSelectedModelConfig();
                    SetActiveCard("Lenticular 2");
                    break;
                case "Lenticular 2":
                    SetActiveCard("Lenticular 3");
                    break;
                case "Lenticular 3":
                    SetActiveCard("Lenticular 4");
                    break;
                case "Lenticular 4":
                    SetActiveCard("Select Calibrator");
                    break;
                case "Realsense":
                    HoloPlay.SaveConfigToFile();
                    SetActiveCard("Select Calibrator");
                    break;
            }
        }

        public void GoBack()
        {
            switch (card)
            {
                case "Select Calibrator":
                    SetActiveCard("Select Drive");
                    break;
                case "Lenticular":
                case "Realsense":
                case "Advanced":
                    SetActiveCard("Select Calibrator");
                    break;
                case "Lenticular 2":
                    SetActiveCard("Lenticular");
                    break;
                case "Lenticular 3":
                    SetActiveCard("Lenticular 2");
                    break;
            }
        }

        void SetActiveCard(string c)
        {
            //set the card as active
            var cards = GameObject.Find("Cards");
            foreach(Transform cg in cards.transform)
            {
                cg.gameObject.SetActive(false);
                if (cg.gameObject.name == c) cg.gameObject.SetActive(true);
            }

            //setup some defaults
            bool n = false;
            bool b = true;
            HoloPlay.Config.test.Value = 0;
            lenticular3Scene.SetActive(false);
            advancedScene.SetActive(false);
            bool rs = false;
            switch (c)
            {
                case "Select Drive":
                    n = true;
                    b = false;
                    break;
                case "Select Calibrator":
                    n = false;
                    b = true;
                    break;
                case "Lenticular":
                    n = true;
                    b = true;
                    break;
                case "Lenticular 2":
                    n = true;
                    b = true;
                    HoloPlay.Config.test.Value = 1;
                    InitializePitchSlider();
                    break;
                case "Lenticular 3":
                    n = true;
                    b = true;
                    InitializeCenterSlider();
                    lenticular3Scene.SetActive(true);
                    break;
                case "Lenticular 4":
                    n = true;
                    b = false;
                    HoloPlay.SaveConfigToFile();
                    break;
                case "Advanced":
                    n = false;
                    b = true;
                    advancedScene.SetActive(true);
                    foreach (var card in FindObjectsOfType<HoloPlayCalibrationCard>())
                    {
                        card.ResetParamSliders();
                    }
                    break;
                case "Realsense":
                    realsenseCalibrator.AdvanceCalibration();
                    n = false;
                    b = false;
                    rs = true;
                    break;
            }
            transform.Find("Next").gameObject.SetActive(n);
            transform.Find("Back").gameObject.SetActive(b);
            realsenseHoloPlayGO.SetActive(rs);

            //update state of fsm
            card = c;
        }

        public void MultiChoiceSelection(string c)
        {
            SetActiveCard(c);
        }

        void LoadSelectedModelConfig()
        {
            if (modelDropdown.options[modelDropdown.value].text == modelUseCurrentStr)
                return;

            var model = modelOptions[modelDropdown.value - 1];
            var currentRealsense = HoloPlay.Config.realsense;

            HoloPlay.Config = new HoloPlayConfig();
            HoloPlay.Config.screenW.Value = model.screenW;
            HoloPlay.Config.screenH.Value = model.screenH;
            HoloPlay.Config.DPI.Value = model.DPI;
            HoloPlay.Config.pitch.Value = model.pitch;
            HoloPlay.Config.numViews.Value = model.numViews;
            HoloPlay.Config.realsense = currentRealsense;

            HoloPlay.SaveConfigToFile();
        }

        public void RealsenseTextSwitch(int i)
        {
            string topText = "";
            bool rt = true;
            bool n = false;
            switch (i)
            {
                case -1:
                    topText = "1. Center Front";
                    break;
                case 0:
                    topText = "2. Center Back";
                    break;
                case 1:
                    topText = "3. Bottom Left";
                    break;
                case 2:
                    topText = "4. Top Right";
                    break;
                case 3:
                    rt = false;
                    n = true;
                    break;
            }
            realsenseBottomText.gameObject.SetActive(rt);
            realsenseTopText.gameObject.SetActive(rt);
            realsenseFinalText.gameObject.SetActive(!rt);
            transform.Find("Next").gameObject.SetActive(n);
            realsenseTopText.text = topText;
        }

        void InitializeDriveDropdown()
        {
            driveDropdown.onValueChanged.RemoveAllListeners();
            driveDropdown.onValueChanged.AddListener(
                (x) => SetConfigDrivePath(driveDropdown.captionText.text)
            );

            driveDropdown.ClearOptions();
            driveDropdown.options.Add(new Dropdown.OptionData(dontSaveUSBString));
            foreach(var d in HoloPlay.GetAllDrives())
            {
                driveDropdown.options.Add(new Dropdown.OptionData(d));
            }

            //todo: search on drives for a calibration folder
            //todo: make the default value here the drive where one is found
            driveDropdown.RefreshShownValue();
            SetConfigDrivePath(driveDropdown.captionText.text);
        }

        void SetConfigDrivePath(string path)
        {
            if (path == dontSaveUSBString) path = "";
            HoloPlay.SetConfigDrivePath(path);
        }

        void InitializeModelDropdown()
        {
            //fill up the options
            modelDropdown.ClearOptions();
            modelDropdown.options.Add(new Dropdown.OptionData(modelUseCurrentStr));
            foreach(var m in modelOptions)
            {
                modelDropdown.options.Add(new Dropdown.OptionData(m.optionText));
            }
            modelDropdown.RefreshShownValue();
        }

        void InitializePitchSlider()
        {
            pitchSlider.onValueChanged.RemoveAllListeners();
            pitchSlider.maxValue = HoloPlay.Config.pitch + pitchWindow;
            pitchSlider.minValue = HoloPlay.Config.pitch - pitchWindow;
            pitchSlider.value = HoloPlay.Config.pitch;
            pitchSlider.onValueChanged.AddListener(
                (x) => HoloPlay.Config.pitch.Value = x
            );
        }

        void InitializeCenterSlider()
        {
            centerSlider.onValueChanged.RemoveAllListeners();
            centerSlider.maxValue = HoloPlay.Config.center + centerWindow;
            centerSlider.minValue = HoloPlay.Config.center - centerWindow;
            centerSlider.value = HoloPlay.Config.center;
            centerSlider.onValueChanged.AddListener(
                (x) => HoloPlay.Config.center.Value = x
            );
        }
    }
}