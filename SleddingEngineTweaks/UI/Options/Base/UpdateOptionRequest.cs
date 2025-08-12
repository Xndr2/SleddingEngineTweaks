namespace SleddingEngineTweaks.UI.Options.Base
{
    // Simple DTO used to update options via API
    public class UpdateOptionRequest
    {
        public string OptionId { get; set; }
        public string NewName { get; set; }
        public OptionType OptionType { get; set; }
        public bool? Visible { get; set; }
        public bool? Enabled { get; set; }
        // Add more fields as needed (e.g., new value payload)
    }
}