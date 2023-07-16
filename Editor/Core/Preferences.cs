using UnityEditor;

namespace SKTools.Editor
{
    public class Preferences
    {
        private static Config _root;
        private static Config Root => _root ?? (_root = LoadFromEditorPrefs<Config>());

        public static string FullRawJson(string key = null)
        {
            if (key == null)
            {
                var all = EditorPrefs.GetString(typeof(Config).FullName, null);
                return all;
            }

            var index = Root.Keys.FindIndex(i => i == key);
            if (index > -1)
            {
                var json = Root.Values[index]; //exception
                return json;
            }

            return default;
        }

        public static T Load<T>(string key = null) where T : EditorJsonAsset, new()
        {
            var instance = new T();
            var k = key ?? typeof(T).FullName;
            var index = Root.Keys.FindIndex(i => i == k);
            if (index > -1)
            {
                var json = Root.Values[index]; //exception
               
                if (!string.IsNullOrEmpty(json))
                {
                    EditorJsonUtility.FromJsonOverwrite(json, instance);
                    return instance;
                }
            }
            return instance;
        }


        public static string Save<T>(T value, string key = null)
        {
            var json = EditorJsonUtility.ToJson((object) value);
            var k = key ?? typeof(T).FullName;
            var index = Root.Keys.FindIndex(i => i == k);
            if (index > -1)
            {
                Root.Values[index] = json;
            }
            else
            {
                Root.Keys.Add(k);
                Root.Values.Add(json);
            }

            SaveToEditorPrefs<Config>(_root, null);
            return json;
        }

        public static bool Delete<T>(string key = null) where T : EditorJsonAsset
        {
            var k = key ?? typeof(T).FullName;
            var index = Root.Keys.FindIndex(i => i == k);
            if (index > -1)
            {
                Root.Keys.RemoveAt(index);
                Root.Values.RemoveAt(index);
                SaveToEditorPrefs<Config>(_root, null);
                return true;
            }

            return false;
        }

        public static void DeleteRoot()
        {
            UnityEditor.EditorPrefs.DeleteKey(typeof(Config).FullName);
        }
        
        private static T LoadFromEditorPrefs<T>(string key = null) where T : EditorJsonAsset, new()
        {
            var json = EditorPrefs.GetString(key ?? typeof(T).FullName, null);
            var instance = new T();
            if (!string.IsNullOrEmpty(json))
            {
                EditorJsonUtility.FromJsonOverwrite(json, instance);
                return instance;
            }

            return instance;
        }
        
        private static void SaveToEditorPrefs<T>(T value, string key)
        {
            var json = EditorJsonUtility.ToJson((object) value);
            EditorPrefs.SetString(key ?? typeof(T).FullName, json);
        }
    }
}