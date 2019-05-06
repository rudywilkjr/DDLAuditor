using DataLayer.DTO;
using DataLayer.Repository;
using PresentationLayer.Model;
using System.Collections.Generic;

namespace PresentationLayer.ViewModel
{
    //ToDo: I feel like this class should be a Model not a Viewmodel?
    public class DatabaseItemViewModel : TreeViewItemViewModel
    {
        readonly DatabaseItem _databaseItem;

        private readonly UtilityRepository _repository = new UtilityRepository();

        public DatabaseItemViewModel(DatabaseItem databaseItem) : 
            base(null, true)
        {
            _databaseItem = databaseItem;
        }

        public string Name => _databaseItem.Name;

        protected override void LoadChildren()
        {
            foreach (var pair in _repository.GetTypesOfObjectsLookup())
            {
                base.Children.Add(new DatabaseCategoryItemViewModel(new DatabaseCategoryItem(pair.Value + "s", Name, pair.Key)));
            }
        }
    }
}
