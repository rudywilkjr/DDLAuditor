using System;
using Core;

namespace DataTracker.Model
{
    public class TrackerParameters
    {
        private DateTime? _minimumReleaseDate;

        public User SelectedDeveloper { get; set; }

        public bool IsDeveloperSelected => SelectedDeveloper != null;

        public DateTime? MinimumDateTime
        {
            get
            {
                return _minimumReleaseDate;
            }
            set
            {
                _minimumReleaseDate = value.GetValueOrDefault(DateTime.Today.AddDays(-30));
            }
        }
        public string EffectiveDeveloperName
        {
            get
            {
                if(IsDeveloperSelected && SelectedDeveloper?.DomainUserName != null)
                {
                    return SelectedDeveloper.DomainUserName;
                }
                return string.Empty;
            }
        }
    }
}
