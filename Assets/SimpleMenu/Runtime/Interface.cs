using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// Menu items can still be changed if their GO is disabled. 
// Eventlisteners will still be called.

namespace SimpleMenu.Runtime
{
    internal sealed class Interface : MonoBehaviour
    {
        private Text _button_crouch_text, _button_jump_text, _button_drag_text;
        private bool _isPaused;
        private GameObject _object_mainCamera, _object_canvas_menu;
        private TMP_Dropdown _dropdown_resolution, _dropdown_fov, _dropdown_displayMode, _dropdown_fpsCap, _dropdown_graphicsQuality;
        private InputField _inputField_sensitivity;
        private Slider _slider_overallVolume, _slider_ambientVolume, _slider_footstepsVolume, _slider_vehicleVolume, _slider_musicVolume;
        private Toggle _toggle_hideUI, _toggle_autoStrafe, _toggle_autoHop;
        private Button _button_quit, _button_resetSettings, _button_resume, _button_reset;


        private void Awake()
        {
            if (Camera.main != null) _object_mainCamera = Camera.main.gameObject;
            _object_canvas_menu = GameObject.Find("Canvas_Menu");
            _button_resume = GameObject.Find("Button_Resume").GetComponent<Button>();
            _button_reset = GameObject.Find("Button_Reset").GetComponent<Button>();
            _button_quit = GameObject.Find("Button_Quit").GetComponent<Button>();
            _button_resetSettings = GameObject.Find("Button_ResetSettings").GetComponent<Button>();
            _dropdown_resolution = GameObject.Find("Dropdown_Resolution").GetComponent<TMP_Dropdown>();
            _dropdown_fov = GameObject.Find("Dropdown_FOV").GetComponent<TMP_Dropdown>();
            _dropdown_displayMode = GameObject.Find("Dropdown_DisplayMode").GetComponent<TMP_Dropdown>();
            _dropdown_graphicsQuality = GameObject.Find("Dropdown_GraphicsQuality").GetComponent<TMP_Dropdown>();
            _dropdown_fpsCap = GameObject.Find("Dropdown_FPSCap").GetComponent<TMP_Dropdown>();
            _inputField_sensitivity = GameObject.Find("InputField_Sensitivity").GetComponent<InputField>();
            _slider_overallVolume = GameObject.Find("Slider_OverallVolume").GetComponent<Slider>();
            _slider_ambientVolume = GameObject.Find("Slider_AmbientVolume").GetComponent<Slider>();
            _slider_footstepsVolume = GameObject.Find("Slider_FootstepsVolume").GetComponent<Slider>();
            _slider_vehicleVolume = GameObject.Find("Slider_VehicleVolume").GetComponent<Slider>();
            _slider_musicVolume = GameObject.Find("Slider_MusicVolume").GetComponent<Slider>();
            _toggle_autoStrafe = GameObject.Find("Toggle_AutoStrafe").GetComponent<Toggle>();
            _toggle_autoHop = GameObject.Find("Toggle_AutoHop").GetComponent<Toggle>();
            _toggle_hideUI = GameObject.Find("Toggle_HideUI").GetComponent<Toggle>();

            _object_mainCamera.AddComponent<Blur>();
            _object_mainCamera.GetComponent<Blur>().iterations = 1;
            _object_mainCamera.GetComponent<Blur>().blurSpread = 0;
            _object_mainCamera.GetComponent<Blur>().blurShader = Shader.Find("Hidden/FastBlur");
        }

        private void Start()
        {
            AudioListener.pause = false;
            InitSupportedResolutions();
            SettingsInit();
            AddMenuEventListeners();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused) OnClick_Resume();
                else OnClick_Pause();
            }
        }

        // <editor-fold defaultstate="collapsed" desc="">
        
        private void SettingsInit()
        {
            Slider_OverallVolume_Update(PlayerPrefs.HasKey(Slider_OverallVolume_KEY) ? PlayerPrefs.GetFloat(Slider_OverallVolume_KEY) : Slider_OverallVolume_DEFAULT);
            Slider_AmbientVolume_Update(PlayerPrefs.HasKey(Slider_AmbientVolume_KEY) ? PlayerPrefs.GetFloat(Slider_AmbientVolume_KEY) : Slider_AmbientVolume_DEFAULT);
            Slider_FootstepsVolume_Update(PlayerPrefs.HasKey(Slider_FootstepsVolume_KEY) ? PlayerPrefs.GetFloat(Slider_FootstepsVolume_KEY) : Slider_FootstepsVolume_DEFAULT);
            Slider_VehicleVolume_Update(PlayerPrefs.HasKey(Slider_VehicleVolume_KEY) ? PlayerPrefs.GetFloat(Slider_VehicleVolume_KEY) : Slider_VehicleVolume_DEFAULT);
            Slider_MusicVolume_Update(PlayerPrefs.HasKey(Slider_MusicVolume_KEY) ? PlayerPrefs.GetFloat(Slider_MusicVolume_KEY) : Slider_MusicVolume_DEFAULT);
            Dropdown_DisplayMode_Update(PlayerPrefs.HasKey(Dropdown_DisplayMode_KEY) ? PlayerPrefs.GetInt(Dropdown_DisplayMode_KEY) : Dropdown_DisplayMode_DEFAULT);
            Dropdown_Resolution_Update(_dropdown_resolution.options.FindIndex(option => option.text == Screen.currentResolution.width + "x" + Screen.currentResolution.height));
            Dropdown_GraphicsQuality_Update(PlayerPrefs.HasKey(Dropdown_GraphicsQuality_KEY) ? PlayerPrefs.GetInt(Dropdown_GraphicsQuality_KEY) : Dropdown_GraphicsQuality_DEFAULT);
            Dropdown_FPSCap_Update(PlayerPrefs.HasKey(Dropdown_FPSCap_KEY)
                ? _dropdown_fpsCap.options.FindIndex(option => option.text == PlayerPrefs.GetString(Dropdown_FPSCap_KEY))
                : _dropdown_fpsCap.options.FindIndex(option => option.text == Dropdown_FPSCap_DEFAULT));
            InputField_Sensitivity_Update(PlayerPrefs.HasKey(InputField_Sensitivity_KEY) ? PlayerPrefs.GetString(InputField_Sensitivity_KEY) : InputField_Sensitivity_DEFAULT);
            Dropdown_FOV_Update(PlayerPrefs.HasKey(Dropdown_FOV_KEY) ? PlayerPrefs.GetInt(Dropdown_FOV_KEY) : Dropdown_FOV_DEFAULT);
            Toggle_AutoStrafe_Update(PlayerPrefs.HasKey(Toggle_AutoStrafe_KEY) ? PlayerPrefs.GetInt(Toggle_AutoStrafe_KEY) == 1 : Toggle_AutoStrafe_DEFAULT);
            Toggle_AutoHop_Update(PlayerPrefs.HasKey(Toggle_AutoHop_KEY) ? PlayerPrefs.GetInt(Toggle_AutoHop_KEY) == 1 : Toggle_AutoHop_DEFAULT);
            Toggle_HideUI_Update(PlayerPrefs.HasKey(Toggle_HideUI_KEY) && PlayerPrefs.GetInt(Toggle_HideUI_KEY) == 1);
        }
        
        private void AddMenuEventListeners()
        {
            _button_quit.onClick.AddListener(OnClick_Quit);
            _button_resetSettings.onClick.AddListener(ResetSettings);
            _button_resume.onClick.AddListener(OnClick_Resume);
            _button_reset.onClick.AddListener(OnClick_Reset);
            _dropdown_graphicsQuality.onValueChanged.AddListener(Dropdown_GraphicsQuality_Update);
            _dropdown_fpsCap.onValueChanged.AddListener(Dropdown_FPSCap_Update);
            _dropdown_displayMode.onValueChanged.AddListener(Dropdown_DisplayMode_Update);
            _dropdown_fov.onValueChanged.AddListener(Dropdown_FOV_Update);
            _dropdown_resolution.onValueChanged.AddListener(Dropdown_Resolution_Update);
            _slider_overallVolume.onValueChanged.AddListener(Slider_OverallVolume_Update);
            _slider_musicVolume.onValueChanged.AddListener(Slider_MusicVolume_Update);
            _slider_ambientVolume.onValueChanged.AddListener(Slider_AmbientVolume_Update);
            _slider_footstepsVolume.onValueChanged.AddListener(Slider_FootstepsVolume_Update);
            _slider_vehicleVolume.onValueChanged.AddListener(Slider_VehicleVolume_Update);
            _toggle_autoStrafe.onValueChanged.AddListener(Toggle_AutoStrafe_Update);
            _toggle_autoHop.onValueChanged.AddListener(Toggle_AutoHop_Update);
            _toggle_hideUI.onValueChanged.AddListener(Toggle_HideUI_Update);
            _inputField_sensitivity.onValueChanged.AddListener(InputField_Sensitivity_Update);
        }

        private void ResetSettings()
        {
            PlayerPrefs.DeleteAll();
            SettingsInit();
        }

        private void InitSupportedResolutions()
        {
            var resolutions = Screen.resolutions;
            var resStringList = new List<string>();
            for (var i = resolutions.Length - 1; i >= 0; i--) // fill resolution dropdown with user supported resolutions
            {
                var currStr = resolutions[i].width + "x" + resolutions[i].height;
                if (!resStringList.Contains(currStr)) resStringList.Add(currStr);
            }
            _dropdown_resolution.AddOptions(resStringList);
        }
        
        #region RUNTIME SETTINGS

        private const string Slider_OverallVolume_KEY = "volall";
        private const float Slider_OverallVolume_DEFAULT = 1f;
        public static float Slider_OverallVolume_READ;
        public void Slider_OverallVolume_Update(float val)
        {
            PlayerPrefs.SetFloat(Slider_OverallVolume_KEY, val);
            AudioListener.volume = val;
            _slider_overallVolume.value = val;
            Slider_OverallVolume_READ = val;
        }

        private const string Slider_AmbientVolume_KEY = "volamb";
        private const float Slider_AmbientVolume_DEFAULT = 1f;
        public static float Slider_AmbientVolume_READ;
        public void Slider_AmbientVolume_Update(float val)
        {
            PlayerPrefs.SetFloat(Slider_AmbientVolume_KEY, val);
            //AudioMixerGroup.audioMixer.SetFloat("AmbientVolume", val);
            _slider_ambientVolume.value = val;
            Slider_AmbientVolume_READ = val;
        }

        private const string Slider_VehicleVolume_KEY = "volcar";
        private const float Slider_VehicleVolume_DEFAULT = 1f;
        public static float Slider_VehicleVolume_READ;
        public void Slider_VehicleVolume_Update(float val)
        {
            PlayerPrefs.SetFloat(Slider_VehicleVolume_KEY, val);
            //AudioMixerGroup.audioMixer.SetFloat("VehicleVolume", val);
            Slider_VehicleVolume_READ = val;
            _slider_vehicleVolume.value = val;
        }
        private const string Slider_FootstepsVolume_KEY = "volfeet";
        private const float Slider_FootstepsVolume_DEFAULT = 0.5f;
        public static float Slider_FootstepsVolume_READ;
        public void Slider_FootstepsVolume_Update(float val)
        {
            PlayerPrefs.SetFloat(Slider_FootstepsVolume_KEY, val);
            //AudioSource_Footsteps.volume = val;
            _slider_footstepsVolume.value = val;
            Slider_FootstepsVolume_READ = val;
        }

        private const string Slider_MusicVolume_KEY = "volmusic";
        private const float Slider_MusicVolume_DEFAULT = 0.03f;
        public static float Slider_MusicVolume_READ;
        public void Slider_MusicVolume_Update(float val)
        {
            PlayerPrefs.SetFloat(Slider_MusicVolume_KEY, val);
            //AudioSource_Music.volume = val;
            _slider_musicVolume.value = val;
            Slider_MusicVolume_READ = val;
        }

        private const string Dropdown_DisplayMode_KEY = "displaymode";
        private const int Dropdown_DisplayMode_DEFAULT = 0;
        public static int Dropdown_DisplayMode_READ;
        public void Dropdown_DisplayMode_Update(int val)
        {
            PlayerPrefs.SetInt(Dropdown_DisplayMode_KEY, val);
            if (val == 0) Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            else if (val == 1) Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            else if (val == 2) Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
            else if (val == 3) Screen.fullScreenMode = FullScreenMode.Windowed;
            _dropdown_displayMode.value = val;
            Dropdown_DisplayMode_READ = val;
        }
        private const string InputField_Sensitivity_KEY = "sens";
        private const string InputField_Sensitivity_DEFAULT = "1";
        public static float InputField_Sensitivity_READ;
        public void InputField_Sensitivity_Update(string val)
        {
            if (float.TryParse(val, out var sensToUse))
            {
                PlayerPrefs.SetString(InputField_Sensitivity_KEY, val);
                InputField_Sensitivity_READ = sensToUse;
                _inputField_sensitivity.text = val;
            }
            else
            {
                PlayerPrefs.SetString(InputField_Sensitivity_KEY, InputField_Sensitivity_DEFAULT);
                InputField_Sensitivity_READ = float.Parse(InputField_Sensitivity_DEFAULT);
                _inputField_sensitivity.text = InputField_Sensitivity_DEFAULT;
            }
        }
        private const string Dropdown_GraphicsQuality_KEY = "graphics";
        private const int Dropdown_GraphicsQuality_DEFAULT = 3;
        public static int Dropdown_GraphicsQuality_READ;
        public void Dropdown_GraphicsQuality_Update(int val)
        {
            PlayerPrefs.SetInt(Dropdown_GraphicsQuality_KEY, val);
            string selectedQuality = _dropdown_graphicsQuality.options[val].text;

            _object_mainCamera.GetComponent<Camera>().farClipPlane = selectedQuality switch
            {
                "Very Low" => 500,
                "Low" => 550,
                "Medium" => 600,
                "High" => 625,
                _ => _object_mainCamera.GetComponent<Camera>().farClipPlane
            };
            string[] qualityNames = QualitySettings.names;
            for (int t = 0; t < qualityNames.Length; t++)
            {
                if (qualityNames[t].Equals(selectedQuality)) QualitySettings.SetQualityLevel(t, true);
            }
            _dropdown_graphicsQuality.value = val;
            Dropdown_GraphicsQuality_READ = val;
        }
        private const string Dropdown_FOV_KEY = "fov";
        private const int Dropdown_FOV_DEFAULT = 1;
        public static int Dropdown_FOV_READ;
        public void Dropdown_FOV_Update(int val)
        {
            PlayerPrefs.SetInt(Dropdown_FOV_KEY, val);
            int selectedHorzFOV = int.Parse(_dropdown_fov.options[val].text);
            _object_mainCamera.GetComponent<Camera>().fieldOfView = selectedHorzFOV switch
            {
                90 => 59f,
                95 => 63f,
                100 => 68f,
                105 => 73f,
                107 => 75f,
                110 => 78f,
                115 => 83f,
                120 => 88f,
                _ => _object_mainCamera.GetComponent<Camera>().fieldOfView
            };
            _dropdown_fov.value = val;
            Dropdown_FOV_READ = val;
        } 

        private const string Dropdown_Resolution_KEY = "res";
        public void Dropdown_Resolution_Update(int val)
        {
            PlayerPrefs.SetString(Dropdown_Resolution_KEY, _dropdown_resolution.options[val].text);
            string[] splitRes = _dropdown_resolution.options[val].text.Split('x');

            if (!_dropdown_resolution.options[val].text.Equals(Screen.currentResolution.width + "x" + Screen.currentResolution.height))
            {
                if (_dropdown_displayMode.value == 0) Screen.SetResolution(int.Parse(splitRes[0]), int.Parse(splitRes[1]), FullScreenMode.ExclusiveFullScreen);
                else if (_dropdown_displayMode.value == 1) Screen.SetResolution(int.Parse(splitRes[0]), int.Parse(splitRes[1]), FullScreenMode.FullScreenWindow);
                else if (_dropdown_displayMode.value == 2) Screen.SetResolution(int.Parse(splitRes[0]), int.Parse(splitRes[1]), FullScreenMode.MaximizedWindow);
                else if (_dropdown_displayMode.value == 3) Screen.SetResolution(int.Parse(splitRes[0]), int.Parse(splitRes[1]), FullScreenMode.Windowed);
            }
            _dropdown_resolution.value = val;
        }

        private const string Dropdown_FPSCap_KEY = "fpscap";
        private const string Dropdown_FPSCap_DEFAULT = "No Cap";
        public static string Dropdown_FPSCap_READ;
        public void Dropdown_FPSCap_Update(int val)
        {
            var selectedStr = _dropdown_fpsCap.options[val].text;
            PlayerPrefs.SetString(Dropdown_FPSCap_KEY, selectedStr);

            switch (selectedStr)
            {
                case "VSync":
                    Application.targetFrameRate = -1;
                    QualitySettings.vSyncCount = 1;
                    break;
                case "No Cap":
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = -1;
                    break;
                default:
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = int.Parse(selectedStr);
                    break;
            }
            _dropdown_fpsCap.value = val;
            Dropdown_FPSCap_READ = selectedStr;
        }

        private const string Toggle_AutoStrafe_KEY = "autostrafe";
        private const bool Toggle_AutoStrafe_DEFAULT = true;
        public static bool Toggle_AutoStrafe_READ;
        public void Toggle_AutoStrafe_Update(bool val)
        {
            PlayerPrefs.SetInt(Toggle_AutoStrafe_KEY, val ? 1 : 0);
            Toggle_AutoStrafe_READ = val;
            _toggle_autoStrafe.isOn = val;
        }

        private const string Toggle_AutoHop_KEY = "autohop";
        private const bool Toggle_AutoHop_DEFAULT = true;
        public static bool Toggle_AutoHop_READ;
        public void Toggle_AutoHop_Update(bool val)
        {
            PlayerPrefs.SetInt(Toggle_AutoHop_KEY, val ? 1 : 0);
            Toggle_AutoHop_READ = val;
            _toggle_autoHop.isOn = val;
        }

        private const string Toggle_HideUI_KEY = "hideui";
        private const bool Toggle_HideUI_DEFAULT = false;
        public static bool Toggle_HideUI_READ;
        public void Toggle_HideUI_Update(bool val)
        {
            PlayerPrefs.SetInt(Toggle_HideUI_KEY, val ? 1 : 0);
            _toggle_hideUI.isOn = val;
            Toggle_HideUI_READ = val;
        }
        #endregion
        #region SCENE EVENTS

        public void OnClick_Resume()
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _object_mainCamera.GetComponent<Blur>().enabled = false;
            AudioListener.pause = false;
            _object_canvas_menu.SetActive(false);
            _isPaused = false;
        }

        public void OnClick_Pause()
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _object_canvas_menu.SetActive(true);
            _object_mainCamera.GetComponent<Blur>().enabled = true;
            AudioListener.pause = true;
            _isPaused = true;
        }


        public void OnClick_Reset()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        public void OnClick_Quit()
        {
            Application.Quit();
        }

        #endregion
        // </editor-fold>
    }
}