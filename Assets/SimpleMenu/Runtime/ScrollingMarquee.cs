using UnityEngine;

namespace SimpleMenu.Runtime
{
    internal sealed class ScrollingMarquee : MonoBehaviour
    {
        public string content = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text.";
        public float speed = 30;
        private Rect _rect_marqueeContainer, _rect_marquee;
        private bool _isVisible_marquee, _isInitilized_marquee;
        private RectTransform _rectTransform_panel_menu;

        private void Start()
        {
            _rectTransform_panel_menu = GameObject.Find("Panel_Menu").GetComponent<RectTransform>();
            _isVisible_marquee = true;
        }

        private void OnGUI()
        {
            if (!_isVisible_marquee) return;
            var dimensions = GUI.skin.label.CalcSize(new GUIContent(content));
            var rec = GetScreenCoordinates(_rectTransform_panel_menu);
            _rect_marqueeContainer = new Rect(rec.x, rec.y - dimensions.y, rec.width, dimensions.y);
            if (!_isInitilized_marquee) // change _rect_marquee.width to the width of the MarqueeMessage
            {
                _rect_marquee = new Rect(-dimensions.x, 0, dimensions.x, _rect_marqueeContainer.y);
                _isInitilized_marquee = true;
            }

            _rect_marqueeContainer = GUI.Window(1, _rect_marqueeContainer, MarqueeWindow, "", GUI.skin.box);
        }

        private void MarqueeWindow(int windowID)
        {
            _rect_marquee.x += Time.deltaTime * speed;
            GUI.Label(_rect_marquee, content); // place _rect_marquee inside the window
        }
        
        public static Rect GetScreenCoordinates(RectTransform uiElement)
        {
            var worldCorners = new Vector3[4];
            uiElement.GetWorldCorners(worldCorners);
            var result = new Rect(worldCorners[0].x,
                worldCorners[0].y,
                worldCorners[2].x - worldCorners[0].x,
                worldCorners[2].y - worldCorners[0].y);
            return result;
        }
    }
}