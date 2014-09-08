using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GCThrash
{
    public class Dummy
    {
        private Int32 num;
        public Dummy(Int32 i)
        {
            num = i;
        }
    }
    
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class Garbage : MonoBehaviour
    {
        private Int32 WindowID;
        private String WindowTitle;
        private Rect WindowRect;
        private GUIStyle windowStyle;
        private GUIStyle areaStyle;
        private GUIStyle labelStyle;
        private GUIStyle dataStyle;
        private GUIStyle buttonStyle;
        private List<Dummy> list;

        private bool enableThrash = false;
        private float numBlocksExp = 0f;
        private Int32 numBlocks = 1;

        private Boolean _Visible = false;
        private Boolean Visible
        {
            get { return _Visible; }
            set
            {
                if (_Visible != value)
                {
                    if (value)
                        RenderingManager.AddToPostDrawQueue(5, DoPostDraw);
                    else
                        RenderingManager.RemoveFromPostDrawQueue(5, DoPostDraw);
                }
                _Visible = value;
            }
        }

        public void Awake()
        {
            InitStyles();

            WindowTitle = "GCThrash (0.1.0.0)";
            WindowRect = new Rect(300, 200, 200, 50);
            WindowID = Guid.NewGuid().GetHashCode();

            list = new List<Dummy>();
        }

        public void Start()
        {
            Visible = true;
        }

        public void Update()
        {
            list.Clear();

            if (enableThrash)
            {
                // Do the garbage thrash
                for (Int32 i = 0; i < numBlocks; i++)
                {
                    list.Add(new Dummy(i));
                }
            }
        }

        private void DoPostDraw()
        {
            if (Visible)
            {
                WindowRect = GUILayout.Window(WindowID, WindowRect, Window, WindowTitle, windowStyle);
            }
        }
        private void Window(int windowID)
        {
            GUILayout.BeginVertical(areaStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Enable Thrash", labelStyle);
            enableThrash = GUILayout.Toggle(enableThrash, "", buttonStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            numBlocksExp = GUILayout.HorizontalSlider(numBlocksExp, 0, 20.0f);
            numBlocks = (Int32)Math.Pow(2d, numBlocksExp);
            GUILayout.Label(numBlocks.ToString("0"), dataStyle, GUILayout.Width(60));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        private void InitStyles()
        {
            windowStyle = new GUIStyle(HighLogic.Skin.window);

            areaStyle = new GUIStyle(HighLogic.Skin.textArea);

            labelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                stretchWidth = true,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 1, 1)
            };

            dataStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleRight,
                stretchWidth = true,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 1, 1)
            };

            buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                fixedWidth = 20,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
                border = new RectOffset(1, 0, 0, 0)
            };
        }
    }
}
