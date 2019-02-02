using SKTools.Core.Editor;

namespace SKTools.Module.RateMeWindow
{
    [System.Serializable]
    public sealed class RateMeConfig : EditorJsonAsset
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
        
        public RateMeConfig() : base()
        {
        }

        public RateMeConfig(string json) : base(json)
        {
          
        }

        protected override string FileName
        {
            get { return GetType().Name + ".json"; }
        }
    }
}