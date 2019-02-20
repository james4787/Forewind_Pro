using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Forewind
{
    public class FPSCounter : MonoBehaviour
    {
        public float updateInterval = 0.5F;
        private double lastInterval;
        private int frames = 0;
        private float fps;

        void Start()
        {
            lastInterval = Time.realtimeSinceStartup;
            frames = 0;
        }

        void Update()
        {
            ++frames;
            float timeNow = Time.realtimeSinceStartup;
            if (timeNow > lastInterval + updateInterval)
            {
                fps = (float) (frames / (timeNow - lastInterval));
                frames = 0;
                lastInterval = timeNow;
            }
        }

        void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(30, 20, 400, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = (int)(h / 25);
            style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            string text = string.Format(" {0:0.} FPS", fps);
            GUI.Label(rect, text, style);
        }
    }
}


