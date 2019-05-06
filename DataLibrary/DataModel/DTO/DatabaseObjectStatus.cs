namespace DataLibrary.DataModel.DTO
{
    public class DatabaseObjectStatus
    {
        public DatabaseObjectBasicInformation ObjectInformation { get; set; }
        public bool HasUnclaimedChanges { get; set; }
        public bool HasPendingCheckin { get; set; }
        public bool NeedsCodeReview { get; set; }
    }
}
