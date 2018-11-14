using UnityEditor;
using UnityEngine;

namespace SKTools.Base.Editor
{
    public abstract class CustomEditorWindow<T> : EditorWindow where T : EditorWindow, IGUIContainer
    {
        public GUIDelegate<IGUIContainer> DrawGuiCallback { get; set; }
        public GUIDelegate<IGUIContainer> LostFocusCallback { get; set; }
        public GUIDelegate<IGUIContainer> FocusCallback { get; set; }
        public GUIDelegate<IGUIContainer> CloseCallback { get; set; }

        private static bool IsCreated
        {
            get { return EditorPrefs.GetBool(typeof(T).FullName, false); }
            set { EditorPrefs.SetBool(typeof(T).FullName, value); }
        }

        /// <summary>
        /// I added this method because want to preconfigurate the window and fast detect existing editor window common state with isCreated
        /// <param name="createIfNotExist">In some cases I need to check of exisiting already an opened window</param>
        /// <typeparam name="T">Some type of editor window</typeparam>
        /// <returns>Return a gui container of type T</returns>
        public static IGUIContainer GetWindow(bool createIfNotExist = false)
        {
            if (!createIfNotExist && !IsCreated) return null;

            T window;
            var objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof(T));
            if (objectsOfTypeAll.Length < 1)
            {
                if (!createIfNotExist)
                {
                    IsCreated = false;
                    return null;
                }

                window = ScriptableObject.CreateInstance<T>();
            }
            else
            {
                window = (T) objectsOfTypeAll[0];
            }

            window.Init();

            return window;
        }

        public virtual void Init()
        {
            titleContent = GetTitleContent;
            if (GetMinSize.HasValue)
            {
                minSize = GetMinSize.Value;
            }

            if (GetMaxSize.HasValue)
            {
                maxSize = GetMaxSize.Value;
            }

            autoRepaintOnSceneChange = GetAutoRepaintOnSceneChange;
        }

        protected virtual Vector2? GetMinSize
        {
            get { return null; }
        }

        protected virtual Vector2? GetMaxSize
        {
            get { return null; }
        }

        protected virtual Rect? GetDefaultPosition
        {
            get { return null; }
        }

        protected virtual GUIContent GetTitleContent
        {
            get { return null; }
        }

        protected virtual bool GetAutoRepaintOnSceneChange
        {
            get { return false; }
        }

        protected virtual bool GetAutoRepaintOnSelectionChange
        {
            get { return false; }
        }

        private void Awake()
        {
            IsCreated = true;
        }

        private void OnFocus()
        {
            if (FocusCallback != null) FocusCallback((IGUIContainer) this);
            if (GetDefaultPosition.HasValue)
            {
                //position = GetDefaultPosition.Value;
            }
        }

        /// <summary>
        /// we can easily switch gui content of this window
        /// </summary>
        private void OnGUI()
        {
            if (DrawGuiCallback != null) DrawGuiCallback((IGUIContainer) this);
        }

        /// <summary>
        /// Some menuitems requires a validating  method, and it need to update visual state of items
        /// </summary>
        private void OnSelectionChange()
        {
            if (GetAutoRepaintOnSelectionChange)
            {
                Repaint();
            }
        }

        /// <summary>
        /// This callbacks are used for saving state
        /// </summary>
        private void OnLostFocus()
        {
            if (LostFocusCallback != null) LostFocusCallback((IGUIContainer) this);
        }

        private void OnDestroy()
        {
            IsCreated = false;

            if (CloseCallback != null) CloseCallback((IGUIContainer) this);

            DrawGuiCallback = null;
            LostFocusCallback = null;
            CloseCallback = null;
        }
    }
}