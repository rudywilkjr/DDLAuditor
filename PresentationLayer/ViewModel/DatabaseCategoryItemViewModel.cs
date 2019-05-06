using DataLayer.DTO;
using DataLayer.Repository;
using PresentationLayer.Model;
using PresentationLayer.Model.Enum;
using PresentationLayer.Utility;
using System;
using System.Linq;

namespace PresentationLayer.ViewModel
{
    public class DatabaseCategoryItemViewModel : TreeViewItemViewModel
    {
        private readonly DatabaseCategoryItem _categoryItem;
        private readonly UtilityRepository _repository = new UtilityRepository();
        private DatabaseCategoryItemFilter _filter;

        public DatabaseCategoryItemViewModel(DatabaseCategoryItem categoryItem) :
            base(null, true)
        {
            _categoryItem = categoryItem;
            _filter = DatabaseCategoryItemFilter.All;
        }

        public string Name => _categoryItem.Name;

        public DatabaseObjectTypeCode Type => _categoryItem.Type;

        public string DatabaseName => _categoryItem.DatabaseName;

        public void ReloadChildren(DatabaseCategoryItemFilter filter)
        {
            _filter = filter;
            ClearChildren();
            LoadChildren();
        }

        protected override void LoadChildren()
        {
            var auditRepository = new AuditLogRepository();
            var unclaimedChanges = 
                auditRepository.GetObjectsWithUnclimainedChangesByDatabaseAndType(_categoryItem.DatabaseName, _categoryItem.Type, DateTime.Today.AddMonths(-3), null);
            //ToDo: Not consistent with other viewmodel patterns. Store this in private variable field.
            //ToDo: Not consistent with other viewmodel patterns. Store this in private variable field.
            var utility = new UtilityRepository();

            foreach (var child in _repository.GetObjectBasicInformationFromDatabaseAndType(_categoryItem.DatabaseName, _categoryItem.Type))
            {
                var item = new DatabaseObjectItem(child.ObjectName, child.ObjectSchema, child.DatabaseName, _categoryItem.Type);
                var dbItem = new DatabaseObject
                {
                    DatabaseName = child.DatabaseName,
                    ObjectSchema = child.ObjectSchema,
                    ObjectName = child.ObjectName,
                    TypeCode = _categoryItem.Type
                };

                item.HasPendingCheckin = !dbItem.IsUpToDate;
                
                if(unclaimedChanges.Any(u => u.ObjectInformation.DatabaseName == _categoryItem.DatabaseName && 
                    u.ObjectInformation.ObjectSchema == child.ObjectSchema && u.ObjectInformation.ObjectName == child.ObjectName))
                {
                    item.HasUnclaimedChanges = true;
                }
                if(_filter == DatabaseCategoryItemFilter.All ||
                    (_filter == DatabaseCategoryItemFilter.OnlyPendingCheckins && item.HasPendingCheckin) ||
                    (_filter == DatabaseCategoryItemFilter.OnlyUnclaimedChanges && item.HasUnclaimedChanges) ||
                    (_filter == DatabaseCategoryItemFilter.OnlyPendingOrUnclaimedChanges && (item.HasPendingCheckin || item.HasUnclaimedChanges))
                    )
                {
                    Children.Add(new DatabaseObjectItemViewModel(item, this));
                }
            }
        }
    }
}
