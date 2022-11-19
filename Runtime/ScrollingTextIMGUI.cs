using UnityEngine;

namespace SimpleMenu.Runtime
{
    public class ScrollingTextIMGUI : MonoBehaviour
    {
        private string MarqueeMessage = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown prin";
        private readonly float scrollSpeed = 30;
        private Rect RectMarqueeContainer, RectMarquee;
        private bool isVisibleMarquee, isInitMarquee;
        private RectTransform RectTransform_Panel_Menu;
        
        
        private void Start()
        {
            RectTransform_Panel_Menu = GameObject.Find("Panel_Menu").GetComponent<RectTransform>();
            isVisibleMarquee = true;
        }

        private void OnGUI()
        {
            if (!isVisibleMarquee) return;
            var dimensions = GUI.skin.label.CalcSize(new GUIContent(MarqueeMessage));
            var rec = Utility.GetScreenCoordinates(RectTransform_Panel_Menu);
            RectMarqueeContainer = new Rect(rec.x, rec.y - dimensions.y, rec.width, dimensions.y);
            // change RectMarquee.width to the width of the MarqueeMessage
            if (!isInitMarquee)
            {
                RectMarquee = new Rect(-dimensions.x, 0, dimensions.x, RectMarqueeContainer.y);
                isInitMarquee = true;
            }
            
            RectMarqueeContainer = GUI.Window(1, RectMarqueeContainer, MarqueeWindow, "", GUI.skin.box);
        }


        private void MarqueeWindow(int windowID)
        {
            RectMarquee.x += Time.deltaTime * scrollSpeed;
            // place RectMarquee inside the window
            GUI.Label(RectMarquee, MarqueeMessage);
        }
    }
}