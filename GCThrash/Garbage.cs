using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GCThrash
{
    public class Dummy
    {
        private int value;
        private Dummy objRef;

        public Dummy(int val)
        {
            value = val;
            objRef = null;
        }

        public Dummy GetChildObject()
        {
            return objRef;
        }
    }
    
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class Garbage : MonoBehaviour
    {
        private const float MaxBlocks = 100000f;
        private const int NumLists = 10;

        private Int32 WindowID;
        private String WindowTitle;
        private Rect WindowRect;
        private GUIStyle windowStyle;
        private GUIStyle areaStyle;
        private GUIStyle labelStyle;
        private GUIStyle dataStyle;
        private GUIStyle buttonStyle;
        private List<Dummy>[] lists;
        private Stopwatch timer;

        private bool enableThrash = false;
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

            WindowTitle = "GCThrash (0.2.0.0)";
            WindowRect = new Rect(300, 200, 250, 50);
            WindowID = Guid.NewGuid().GetHashCode();

            // Create an array of lists to hold our garbage
            lists = new List<Dummy>[NumLists];
            for (int i = 0; i < NumLists; i++)
                lists[i] = new List<Dummy>();

            timer = new Stopwatch();
            timer.Start();
        }

        public void Start()
        {
            Visible = true;
        }

        public void Update()
        {
            if (timer.ElapsedMilliseconds > 1000)
            {
                // Timer has run for over 1 second

                // Restart the timer
                timer.Reset();
                timer.Start();

                // Move the lists in the array down one element
                // lists[0] will be collected
                for (int i = 0; i < (NumLists - 1); i++)
                    lists[i] = lists[i + 1];

                // Create a new list at the end of the array
                lists[NumLists - 1] = new List<Dummy>();
            }

            if (enableThrash)
            {
                // Thrash is enabled so allocate new objects and stick each one in a random list in the array
                for (Int32 i = 0; i < numBlocks; i++)
                {
                    lists[UnityEngine.Random.Range(0, NumLists)].Add(new Dummy(i));
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
            numBlocks = (Int32)GUILayout.HorizontalSlider(numBlocks, 1f, MaxBlocks);
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
