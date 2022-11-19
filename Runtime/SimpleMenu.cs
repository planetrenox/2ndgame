using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// Menu items can still be changed if their GO is disabled. 
// Eventlisteners will still be called

namespace SimpleMenu.Runtime
{
    internal sealed class SimpleMenu : MonoBehaviour
    {
        private Text Button_Crouch_Text, Button_Jump_Text, Button_Drag_Text;
        private bool isPaused, isRebinding, isRebindingCrouch, isRebindingJump, isRebindingDrag;
        private GameObject GO_MainCamera, GO_Canvas_Menu, GO_Panel_Binds;
        private TMP_Dropdown Dropdown_Resolution, Dropdown_FOV, Dropdown_DisplayMode, Dropdown_FPSCap, Dropdown_GraphicsQuality;
        private InputField InputField_Sensitivity;
        private Slider Slider_OverallVolume, Slider_AmbientVolume, Slider_FootstepsVolume, Slider_VehicleVolume, Slider_MusicVolume;
        private Toggle Toggle_HideUI, Toggle_AutoStrafe, Toggle_AutoHop;
        private Button Button_Resume, Button_Reset, Button_Invite, Button_Crouch, Button_Jump, Button_Drag, Button_Quit, Button_ResetSettings, Button_KeyBindings;
        private AudioSource AudioSource_Ambient, AudioSource_Water, AudioSource_Music;
        private AudioMixerGroup AudioMixerGroup;
        public static bool isScrollingUp, isScrollingDown;


        private void Awake()
        {
            if (Camera.main != null) GO_MainCamera = Camera.main.gameObject;
            GO_Canvas_Menu = GameObject.Find("Canvas_Menu");
            GO_Panel_Binds = GameObject.Find("Panel_Binds");
            Button_Resume = GameObject.Find("Button_Resume").GetComponent<Button>();
            Button_Invite = GameObject.Find("Button_Invite").GetComponent<Button>();
            Button_Reset = GameObject.Find("Button_Reset").GetComponent<Button>();
            Button_Crouch = GameObject.Find("Button_Crouch").GetComponent<Button>();
            Button_Jump = GameObject.Find("Button_Jump").GetComponent<Button>();
            Button_Drag = GameObject.Find("Button_Drag").GetComponent<Button>();
            Button_Quit = GameObject.Find("Button_Quit").GetComponent<Button>();
            Button_ResetSettings = GameObject.Find("Button_ResetSettings").GetComponent<Button>();
            Button_KeyBindings = GameObject.Find("Button_KeyBindings").GetComponent<Button>();
            Button_Crouch_Text = GameObject.Find("Button_Crouch_Text").GetComponent<Text>();
            Button_Jump_Text = GameObject.Find("Button_Jump_Text").GetComponent<Text>();
            Button_Drag_Text = GameObject.Find("Button_Drag_Text").GetComponent<Text>();
            Dropdown_Resolution = GameObject.Find("Dropdown_Resolution").GetComponent<TMP_Dropdown>();
            Dropdown_FOV = GameObject.Find("Dropdown_FOV").GetComponent<TMP_Dropdown>();
            Dropdown_DisplayMode = GameObject.Find("Dropdown_DisplayMode").GetComponent<TMP_Dropdown>();
            Dropdown_GraphicsQuality = GameObject.Find("Dropdown_GraphicsQuality").GetComponent<TMP_Dropdown>();
            Dropdown_FPSCap = GameObject.Find("Dropdown_FPSCap").GetComponent<TMP_Dropdown>();
            InputField_Sensitivity = GameObject.Find("InputField_Sensitivity").GetComponent<InputField>();
            Slider_OverallVolume = GameObject.Find("Slider_OverallVolume").GetComponent<Slider>();
            Slider_AmbientVolume = GameObject.Find("Slider_AmbientVolume").GetComponent<Slider>();
            Slider_FootstepsVolume = GameObject.Find("Slider_FootstepsVolume").GetComponent<Slider>();
            Slider_VehicleVolume = GameObject.Find("Slider_VehicleVolume").GetComponent<Slider>();
            Slider_MusicVolume = GameObject.Find("Slider_MusicVolume").GetComponent<Slider>();
            Toggle_AutoStrafe = GameObject.Find("Toggle_AutoStrafe").GetComponent<Toggle>();
            Toggle_AutoHop = GameObject.Find("Toggle_AutoHop").GetComponent<Toggle>();
            Toggle_HideUI = GameObject.Find("Toggle_HideUI").GetComponent<Toggle>();
            AudioSource_Ambient = GameObject.Find("AudioSource_Ambient").GetComponent<AudioSource>();
            AudioSource_Music = GameObject.Find("AudioSource_Music").GetComponent<AudioSource>();
            AudioMixerGroup = Resources.Load<AudioMixer>("Audio/AudioMixerCZ").FindMatchingGroups("Master")[0];

            isRebinding = false;
            isRebindingCrouch = false;
            isRebindingDrag = false;
            isRebindingJump = false;

            GO_MainCamera.AddComponent<Blur>();
            GO_MainCamera.GetComponent<Blur>().iterations = 1;
            GO_MainCamera.GetComponent<Blur>().blurSpread = 0;
            GO_MainCamera.GetComponent<Blur>().blurShader = Shader.Find("Hidden/FastBlur");
        }

        private void Start()
        {
            AudioListener.pause = false;
            Resolution[] resolutions = Screen.resolutions;
            List<string> resStringList = new List<string>();
            for (int i = resolutions.Length - 1; i >= 0; i--) // fill resolution dropdown with user supported resolutions
            {
                var currStr = resolutions[i].width + "x" + resolutions[i].height;
                if (!resStringList.Contains(currStr)) resStringList.Add(currStr);
            }

            Dropdown_Resolution.AddOptions(resStringList);


            SettingsInit();
            AddMenuEventListeners();
            GO_Panel_Binds.SetActive(false);
        }


        private void Update()
        {
            isScrollingUp = Input.GetAxis("Mouse ScrollWheel") > 0;
            isScrollingDown = Input.GetAxis("Mouse ScrollWheel") < 0;

            if (isRebinding)
            {
                foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(vKey))
                    {
                        UpdateKeybind(vKey);
                        isRebinding = false;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused) OnClick_Resume();
                else OnClick_Pause();
            }
        }

        private void SettingsInitKeybinds()
        {
            Button_Crouch_READ = PlayerPrefs.HasKey(Button_Crouch_KEY) ? (KeyCode) PlayerPrefs.GetInt(Button_Crouch_KEY) : Button_Crouch_DEFAULT;
            Button_Jump_READ = PlayerPrefs.HasKey(Button_Jump_KEY) ? (KeyCode) PlayerPrefs.GetInt(Button_Jump_KEY) : Button_Jump_DEFAULT;
            Button_Drag_READ = PlayerPrefs.HasKey(Button_Drag_KEY) ? (KeyCode) PlayerPrefs.GetInt(Button_Drag_KEY) : Button_Drag_DEFAULT;
            Button_Console_READ = Button_Console_DEFAULT;
            Button_Crouch_Text.text = Button_Crouch_READ.ToString();
            Button_Jump_Text.text = Button_Jump_READ.ToString();
            Button_Drag_Text.text = Button_Drag_READ.ToString();
        }

        private void AddMenuEventListeners()
        {
            Button_Quit.onClick.AddListener(OnClick_Quit);
            Button_ResetSettings.onClick.AddListener(ResetSettings);
            Button_KeyBindings.onClick.AddListener(SetKeyBindsPanelVisible);
        }


        private void ResetSettings()
        {
            PlayerPrefs.DeleteAll();
            SettingsInit();
        }


        private void SettingsInit()
        {
            SettingsInitKeybinds();
            Slider_OverallVolume_Update(PlayerPrefs.HasKey(Slider_OverallVolume_KEY) ? PlayerPrefs.GetFloat(Slider_OverallVolume_KEY) : Slider_OverallVolume_DEFAULT);
            Slider_AmbientVolume_Update(PlayerPrefs.HasKey(Slider_AmbientVolume_KEY) ? PlayerPrefs.GetFloat(Slider_AmbientVolume_KEY) : Slider_AmbientVolume_DEFAULT);
            Slider_FootstepsVolume_Update(PlayerPrefs.HasKey(Slider_FootstepsVolume_KEY) ? PlayerPrefs.GetFloat(Slider_FootstepsVolume_KEY) : Slider_FootstepsVolume_DEFAULT);
            Slider_VehicleVolume_Update(PlayerPrefs.HasKey(Slider_VehicleVolume_KEY) ? PlayerPrefs.GetFloat(Slider_VehicleVolume_KEY) : Slider_VehicleVolume_DEFAULT);
            Slider_MusicVolume_Update(PlayerPrefs.HasKey(Slider_MusicVolume_KEY) ? PlayerPrefs.GetFloat(Slider_MusicVolume_KEY) : Slider_MusicVolume_DEFAULT);
            Dropdown_DisplayMode_Update(PlayerPrefs.HasKey(Dropdown_DisplayMode_KEY) ? PlayerPrefs.GetInt(Dropdown_DisplayMode_KEY) : Dropdown_DisplayMode_DEFAULT);
            Dropdown_Resolution_Update(Dropdown_Resolution.options.FindIndex(option => option.text == Screen.currentResolution.width + "x" + Screen.currentResolution.height));
            Dropdown_GraphicsQuality_Update(PlayerPrefs.HasKey(Dropdown_GraphicsQuality_KEY) ? PlayerPrefs.GetInt(Dropdown_GraphicsQuality_KEY) : Dropdown_GraphicsQuality_DEFAULT);
            Dropdown_FPSCap_Update(PlayerPrefs.HasKey(Dropdown_FPSCap_KEY)
                ? Dropdown_FPSCap.options.FindIndex(option => option.text == PlayerPrefs.GetString(Dropdown_FPSCap_KEY))
                : Dropdown_FPSCap.options.FindIndex(option => option.text == Dropdown_FPSCap_DEFAULT));
            InputField_Sensitivity_Update(PlayerPrefs.HasKey(InputField_Sensitivity_KEY) ? PlayerPrefs.GetString(InputField_Sensitivity_KEY) : InputField_Sensitivity_DEFAULT);
            Dropdown_FOV_Update(PlayerPrefs.HasKey(Dropdown_FOV_KEY) ? PlayerPrefs.GetInt(Dropdown_FOV_KEY) : Dropdown_FOV_DEFAULT);
            Toggle_AutoStrafe_Update(PlayerPrefs.HasKey(Toggle_AutoStrafe_KEY) ? PlayerPrefs.GetInt(Toggle_AutoStrafe_KEY) == 1 : Toggle_AutoStrafe_DEFAULT);
            Toggle_AutoHop_Update(PlayerPrefs.HasKey(Toggle_AutoHop_KEY) ? PlayerPrefs.GetInt(Toggle_AutoHop_KEY) == 1 : Toggle_AutoHop_DEFAULT);
            Toggle_HideUI_Update(PlayerPrefs.HasKey(Toggle_HideUI_KEY) && PlayerPrefs.GetInt(Toggle_HideUI_KEY) == 1);
        }

        #region RUNTIME SETTINGS

        private const string Slider_OverallVolume_KEY = "volall";
        private const float Slider_OverallVolume_DEFAULT = 1f;
        public static float Slider_OverallVolume_READ;
        public void Slider_OverallVolume_Update(float val)
        {
            PlayerPrefs.SetFloat(Slider_OverallVolume_KEY, val);
            AudioListener.volume = val;
            Slider_OverallVolume.value = val;
            Slider_OverallVolume_READ = val;
        }

        private const string Slider_AmbientVolume_KEY = "volamb";
        private const float Slider_AmbientVolume_DEFAULT = 1f;
        public static float Slider_AmbientVolume_READ;
        public void Slider_AmbientVolume_Update(float val)
        {
            PlayerPrefs.SetFloat(Slider_AmbientVolume_KEY, val);
            AudioMixerGroup.audioMixer.SetFloat("AmbientVolume", val);
            Slider_AmbientVolume.value = val;
            Slider_AmbientVolume_READ = val;
        }

        private const string Slider_VehicleVolume_KEY = "volcar";
        private const float Slider_VehicleVolume_DEFAULT = 1f;
        public static float Slider_VehicleVolume_READ;
        public void Slider_VehicleVolume_Update(float val)
        {
            PlayerPrefs.SetFloat(Slider_VehicleVolume_KEY, val);
            AudioMixerGroup.audioMixer.SetFloat("VehicleVolume", val);
            Slider_VehicleVolume_READ = val;
            Slider_VehicleVolume.value = val;
        }
        private const string Slider_FootstepsVolume_KEY = "volfeet";
        private const float Slider_FootstepsVolume_DEFAULT = 0.5f;
        public static float Slider_FootstepsVolume_READ;
        public void Slider_FootstepsVolume_Update(float val)
        {
            PlayerPrefs.SetFloat(Slider_FootstepsVolume_KEY, val);
            //AudioSource_Footsteps.volume = val;
            Slider_FootstepsVolume.value = val;
            Slider_FootstepsVolume_READ = val;
        }

        private const string Slider_MusicVolume_KEY = "volmusic";
        private const float Slider_MusicVolume_DEFAULT = 0.03f;
        public static float Slider_MusicVolume_READ;
        public void Slider_MusicVolume_Update(float val)
        {
            PlayerPrefs.SetFloat(Slider_MusicVolume_KEY, val);
            AudioSource_Music.volume = val;
            Slider_MusicVolume.value = val;
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
            Dropdown_DisplayMode.value = val;
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
                InputField_Sensitivity.text = val;
            }
            else
            {
                PlayerPrefs.SetString(InputField_Sensitivity_KEY, InputField_Sensitivity_DEFAULT);
                InputField_Sensitivity_READ = float.Parse(InputField_Sensitivity_DEFAULT);
                InputField_Sensitivity.text = InputField_Sensitivity_DEFAULT;
            }
        }
        private const string Dropdown_GraphicsQuality_KEY = "graphics";
        private const int Dropdown_GraphicsQuality_DEFAULT = 3;
        public static int Dropdown_GraphicsQuality_READ;
        public void Dropdown_GraphicsQuality_Update(int val)
        {
            PlayerPrefs.SetInt(Dropdown_GraphicsQuality_KEY, val);
            string selectedQuality = Dropdown_GraphicsQuality.options[val].text;

            GO_MainCamera.GetComponent<Camera>().farClipPlane = selectedQuality switch
            {
                "Very Low" => 500,
                "Low" => 550,
                "Medium" => 600,
                "High" => 625,
                _ => GO_MainCamera.GetComponent<Camera>().farClipPlane
            };
            string[] qualityNames = QualitySettings.names;
            for (int t = 0; t < qualityNames.Length; t++)
            {
                if (qualityNames[t].Equals(selectedQuality)) QualitySettings.SetQualityLevel(t, true);
            }
            Dropdown_GraphicsQuality.value = val;
            Dropdown_GraphicsQuality_READ = val;
        }
        private const string Dropdown_FOV_KEY = "fov";
        private const int Dropdown_FOV_DEFAULT = 1;
        public static int Dropdown_FOV_READ;
        public void Dropdown_FOV_Update(int val)
        {
            PlayerPrefs.SetInt(Dropdown_FOV_KEY, val);
            int selectedHorzFOV = int.Parse(Dropdown_FOV.options[val].text);
            GO_MainCamera.GetComponent<Camera>().fieldOfView = selectedHorzFOV switch
            {
                90 => 59f,
                95 => 63f,
                100 => 68f,
                105 => 73f,
                107 => 75f,
                110 => 78f,
                115 => 83f,
                120 => 88f,
                _ => GO_MainCamera.GetComponent<Camera>().fieldOfView
            };
            Dropdown_FOV.value = val;
            Dropdown_FOV_READ = val;
        } // not PERFECT

        private const string Dropdown_Resolution_KEY = "res";
        public void Dropdown_Resolution_Update(int val)
        {
            PlayerPrefs.SetString(Dropdown_Resolution_KEY, Dropdown_Resolution.options[val].text);
            string[] splitRes = Dropdown_Resolution.options[val].text.Split('x');

            if (!Dropdown_Resolution.options[val].text.Equals(Screen.currentResolution.width + "x" + Screen.currentResolution.height))
            {
                if (Dropdown_DisplayMode.value == 0) Screen.SetResolution(int.Parse(splitRes[0]), int.Parse(splitRes[1]), FullScreenMode.ExclusiveFullScreen);
                else if (Dropdown_DisplayMode.value == 1) Screen.SetResolution(int.Parse(splitRes[0]), int.Parse(splitRes[1]), FullScreenMode.FullScreenWindow);
                else if (Dropdown_DisplayMode.value == 2) Screen.SetResolution(int.Parse(splitRes[0]), int.Parse(splitRes[1]), FullScreenMode.MaximizedWindow);
                else if (Dropdown_DisplayMode.value == 3) Screen.SetResolution(int.Parse(splitRes[0]), int.Parse(splitRes[1]), FullScreenMode.Windowed);
            }
            Dropdown_Resolution.value = val;
        }

        private const string Dropdown_FPSCap_KEY = "fpscap";
        private const string Dropdown_FPSCap_DEFAULT = "No Cap";
        public static string Dropdown_FPSCap_READ;
        public void Dropdown_FPSCap_Update(int val)
        {
            var selectedStr = Dropdown_FPSCap.options[val].text;
            PlayerPrefs.SetString(Dropdown_FPSCap_KEY, selectedStr);

            if (selectedStr == "VSync")
            {
                Application.targetFrameRate = -1;
                QualitySettings.vSyncCount = 1;
            }
            else if (selectedStr == "No Cap")
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = -1;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = int.Parse(selectedStr);
            }
            Dropdown_FPSCap.value = val;
            Dropdown_FPSCap_READ = selectedStr;
        }

        private const string Toggle_AutoStrafe_KEY = "autostrafe";
        private const bool Toggle_AutoStrafe_DEFAULT = true;
        public static bool Toggle_AutoStrafe_READ;
        public void Toggle_AutoStrafe_Update(bool val)
        {
            PlayerPrefs.SetInt(Toggle_AutoStrafe_KEY, val ? 1 : 0);
            Toggle_AutoStrafe_READ = val;
            Toggle_AutoStrafe.isOn = val;
        }

        private const string Toggle_AutoHop_KEY = "autohop";
        private const bool Toggle_AutoHop_DEFAULT = true;
        public static bool Toggle_AutoHop_READ;
        public void Toggle_AutoHop_Update(bool val)
        {
            PlayerPrefs.SetInt(Toggle_AutoHop_KEY, val ? 1 : 0);
            Toggle_AutoHop_READ = val;
            Toggle_AutoHop.isOn = val;
        }

        private const string Toggle_HideUI_KEY = "hideui";
        private const bool Toggle_HideUI_DEFAULT = false;
        public static bool Toggle_HideUI_READ;
        public void Toggle_HideUI_Update(bool val)
        {
            PlayerPrefs.SetInt(Toggle_HideUI_KEY, val ? 1 : 0);
            Toggle_HideUI.isOn = val;
            Toggle_HideUI_READ = val;
        }
        private const string Button_Crouch_KEY = "keycrouch";
        private const KeyCode Button_Crouch_DEFAULT = KeyCode.LeftControl;
        public static KeyCode Button_Crouch_READ;
        private void Button_Crouch_Update(KeyCode val)
        {
        }

        private const string Button_Jump_KEY = "keyjump";
        private const KeyCode Button_Jump_DEFAULT = KeyCode.Space;
        public static KeyCode Button_Jump_READ;
        private void Button_Jump_Update(KeyCode val)
        {
        }
        private const string Button_Drag_KEY = "keydrag";
        private const KeyCode Button_Drag_DEFAULT = KeyCode.Mouse1;
        public static KeyCode Button_Drag_READ;
        private void Button_Drag_Update(KeyCode val)
        {
        }
        private const string Button_Console_KEY = "keyconsole";
        private const KeyCode Button_Console_DEFAULT = KeyCode.BackQuote;
        public static KeyCode Button_Console_READ;
        private void Button_Console_Update(KeyCode val)
        {
        }

        #endregion

        #region REBINDING

        private void SetKeyBindsPanelVisible()
        {
            GO_Panel_Binds.SetActive(true);
        }
        public void BeginRebindingCrouch()
        {
            Button_Crouch_Text.text = "";
            isRebindingCrouch = true;
            isRebinding = true;
        }

        public void BeginRebindingJump()
        {
            Button_Jump_Text.text = "";
            isRebindingJump = true;
            isRebinding = true;
        }

        public void BeginRebindingDrag()
        {
            Button_Drag_Text.text = "";
            isRebindingDrag = true;
            isRebinding = true;
        }

        private void UpdateKeybind(KeyCode keycode)
        {
            if (isRebindingCrouch)
            {
                Button_Crouch_READ = keycode;
                PlayerPrefs.SetInt(Button_Crouch_KEY, (int) keycode);
                Button_Crouch_Text.text = keycode.ToString();
                isRebindingCrouch = false;
            }
            else if (isRebindingJump)
            {
                Button_Jump_READ = keycode;
                PlayerPrefs.SetInt(Button_Jump_KEY, (int) keycode);
                Button_Jump_Text.text = keycode.ToString();
                isRebindingJump = false;
            }
            else if (isRebindingDrag)
            {
                Button_Drag_READ = keycode;
                PlayerPrefs.SetInt(Button_Drag_KEY, (int) keycode);
                Button_Drag_Text.text = keycode.ToString();
                isRebindingDrag = false;
            }
        }

        #endregion

        #region SCENE EVENTS

        public void OnClick_Resume()
        {
            Time.timeScale = 1;
            GO_Panel_Binds.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GO_MainCamera.GetComponent<Blur>().enabled = false;
            AudioListener.pause = false;
            GO_Canvas_Menu.SetActive(false);
            isPaused = false;
        }

        public void OnClick_Pause()
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GO_Canvas_Menu.SetActive(true);
            GO_MainCamera.GetComponent<Blur>().enabled = true;
            AudioListener.pause = true;
            isPaused = true;
        }


        public static void OnClick_Reset()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        public void OnClick_Quit()
        {
            Application.Quit();
        }

        #endregion
    }
}