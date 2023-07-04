using SKTools.Editor;

namespace SKTools.Editor.Windows.RateMe
{
    [System.Serializable]
    public class RateMeState : EditorJsonAsset
    {
        public string Source;
        public bool UserChoiceNeverShowAgain = false;
        public long SchedulingTimeUtc = 0;
        public bool Cleared = false;
        protected override string FileName => Source + nameof(RateMeState);
    }
    
    [System.Serializable]
    public sealed class RateMeConfig : EditorJsonAsset
    {
        public bool SchedulingEnabled = true;
        public uint DisplayInSeconds = 10;
        public uint TryAgainInSeconds = 10;

        public bool ShowOnEditorStartUp = false;
        //??
        public int DisplayInUsageCount = -1;
        
        public byte MinStar = 3;
        public byte MaxStar = 5;

        public string RequestLabel = "How do you like it?";
        public string RateButtonText = "Rate";

        public string RateMainUrl = "google.com";

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