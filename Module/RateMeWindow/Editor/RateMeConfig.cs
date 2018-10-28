using System;
using System.IO;
using SKTools.Base.Editor;
using UnityEditor;
using UnityEngine;

namespace SKTools.Module.RateMeWindow
{
    [System.Serializable]
    public class RateMeConfig
    {
        public byte MinStar = 3;
        public byte MaxStar = 5;
        
        public string RequestLabel = "How do you like it?";
        public string RateButtonText = "Rate";
        
        public string RateMainUrl = "https://assetstore.unity.com/packages/tools/utilities/monkey-editor-commands-productivity-booster-119938";
        public string RateMainButtonText = "Rate me on AssetStore";
        
        public string RateOptionalUrl = "";
        public string RateOptionalButtonText = "";
        public string RateOptionalMessage = "";

        public string FeedbackMessage = "Please enter your message here";
        public string FeedbackTitle = "FEEDBACK/SUGGESTION";
        public string FeedbackBackButtonText = "Back";
        public string FeedbackSendEmailButtonText = "Send Email";

        public string EmailAddress = "";
        public string EmailSubject = "[Support] FEEDBACK/SUGGESTION";
        public string EmailAdditionalInfoFormat = "\n\n\n\n________\n\nPlease Do Not Modify This\n\n{0}\n\n________\n";

        public RateMeConfig(string json)
        {
            EditorJsonUtility.FromJsonOverwrite(json, this);
        }
        
        public RateMeConfig()
        {
        }
        
        public RateMeConfig Load()
        {
            try
            {
                var filePath = Utility.GetPathRelativeToExecutableCurrentFile("Editor Resources", "RateMeConfig.json");
                if (File.Exists(filePath))
                {
                    EditorJsonUtility.FromJsonOverwrite(File.ReadAllText(filePath), this);
                    return this;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return new RateMeConfig();
        }

        public void Save(string relativeFolder = "Editor Resources" , string fileName = "RateMeConfig.json")
        {
            try
            {
                var filePath = Utility.GetPathRelativeToExecutableCurrentFile(relativeFolder, fileName);
                File.WriteAllText(filePath, EditorJsonUtility.ToJson(this, true));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}