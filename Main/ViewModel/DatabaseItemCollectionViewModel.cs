using DataLibrary.DataModel.DTO;
using DataLibrary.Repository;
using DataTracker.Model;
using DataTracker.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DataTracker.ViewModel
{
    public class DatabaseItemCollectionViewModel
    {
        //ToDo: I feel like this class should be a Model not a Viewmodel?
        private IEnumerator<TreeViewItemViewModel> _searchEnumerator;

        #region Commands

        public ICommand CheckinChangesCommand => new DelegateCommand(CheckinChanges);

        public ICommand ClaimChangesCommand => new DelegateCommand(ClaimChanges);

        public ICommand SearchTreeCommand => new DelegateCommand(SearchTree);

        public ICommand FilterByPendingChangesCommand => new DelegateCommand(FilterByPendingChanges);

        #endregion Commands

        public string Comment { get; set; }

        public bool OutstandingOnly { get; set; }

        public string SearchText { get; set; }

        //ToDo: This should be a model not a viewmodel
        public ReadOnlyCollection<DatabaseItemViewModel> DatabaseItems { get; }

        #region Public Functions

        public DatabaseItemCollectionViewModel(DatabaseItem[] databaseItems)
        {
            DatabaseItems = new ReadOnlyCollection<DatabaseItemViewModel>(
                databaseItems.Select(d => new DatabaseItemViewModel(d)).ToList());
        }
        
        public DatabaseItemCollectionViewModel()
        {

        }
        
        public void SearchTree(object o)
        {
            //ToDo: 
            var viewModel = DatabaseItems.FirstOrDefault(d => d.IsExpanded);
            if(viewModel != null)
            {
                PerformSearch(viewModel);
            }
        }

        public void ClaimChanges(object o)
        {
            var items = GetSelectedItems();
            var databaseObjects = new ObservableCollection<AssignableDatabaseObject>();
            items.ForEach(i =>
                databaseObjects.Add(new AssignableDatabaseObject(new DatabaseObject
                {
                    DatabaseName = i.DatabaseName,
                    TypeCode = i.ItemType,
                    ObjectSchema = i.SchemaName,
                    ObjectName = i.Name
                }))
            );
            var linkViewModel = new LinkObjectViewModel {DatabaseObjects = databaseObjects};
            //new window
            var linkWindow = new Views.LinkObject { DataContext = linkViewModel };
            linkWindow.Show();
        }

        public void CheckinChanges(object o)
        {
            if(string.IsNullOrEmpty(Comment))
            {
                MessageBox.Show("No comment was added to this check in.  Please add a comment and save again.");
                return;
            }

            var items = GetSelectedItems();
            if (!items.Any())
            {
                MessageBox.Show("No objects were selected to check in.");
                return;
            }
            
        }

        public void FilterByPendingChanges (object o)
        {
            var model = (DatabaseItemCollectionViewModel)o;
            var db = model.DatabaseItems.Where(d => d.IsExpanded).ToList();
            for (var i = 0; i < db.Count(); i++)
            {
                var cat = db[i].Children.Where(c => c.IsExpanded).Select(c => (DatabaseCategoryItemViewModel)c).ToList();
                foreach (var t in cat)
                {
                    t.ReloadChildren(model.OutstandingOnly
                        ? Model.Enum.DatabaseCategoryItemFilter.OnlyPendingOrUnclaimedChanges
                        : Model.Enum.DatabaseCategoryItemFilter.All);
                }
            }
        }

        #endregion Public Functions

        #region Private Functions

        private void PerformSearch(DatabaseItemViewModel model)
        {
            if (_searchEnumerator != null && _searchEnumerator.MoveNext()) return;
            if (_searchEnumerator == null || !_searchEnumerator.MoveNext())
                VerifySearchEnumerator(model);

            var currentObject = _searchEnumerator?.Current;

            if (currentObject == null)
                return;

            // Ensure that this person is in view.
            if (currentObject.Parent != null)
                currentObject.Parent.IsExpanded = true;

            currentObject.IsHighlighted = true;
        }

        private void VerifySearchEnumerator(DatabaseItemViewModel model)
        {
            TreeViewItemViewModel firstAvailable = model;
            if (firstAvailable != null)
            {
                var matches = FindMatches(SearchText, firstAvailable);
                _searchEnumerator = matches.GetEnumerator();
            }
            
            if (!_searchEnumerator.MoveNext())
            {
                MessageBox.Show(
                    "No matching names were found.",
                    "Try Again",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
        }

        IEnumerable<TreeViewItemViewModel> FindMatches(string searchText, TreeViewItemViewModel subTree)
        {
            if (subTree.GetType() == typeof(DatabaseObjectItemViewModel))
            {
                var dbObject = (DatabaseObjectItemViewModel)subTree;
                if(dbObject.Name.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) > -1 ||
                    dbObject.SchemaName.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    if(!OutstandingOnly || (OutstandingOnly && dbObject.HasUnclaimedChanges))
                    {
                        yield return subTree;
                    }
                }
            }

            if (!subTree.IsExpanded) yield break;
            foreach (var child in subTree.Children)
            {
                foreach (var match in FindMatches(searchText, child))
                {
                    yield return match;
                }
            }
        }

        private List<DatabaseObjectItem> GetSelectedItems()
        {
            var items = new List<DatabaseObjectItem>();
            foreach (var dbItem in DatabaseItems)
            {
                items.AddRange(GetSelectedItems(dbItem));
            }
            return items;
        }

        private IEnumerable<DatabaseObjectItem> GetSelectedItems(TreeViewItemViewModel model)
        {
            var items = new List<DatabaseObjectItem>();
            if (model == null) return items;
            if (typeof(DatabaseCategoryItemViewModel) == model.GetType())
            {
                items.AddRange(from DatabaseObjectItemViewModel dbItem in model.Children
                        .Where(c => c.IsSelected)
                    select new DatabaseObjectItem
                        (dbItem.Name, dbItem.SchemaName, dbItem.DatabaseName, dbItem.Type));
            }
            else
            {
                if (model.Children == null || !model.Children.Any()) return items;
                foreach (var child in model.Children)
                {
                    items.AddRange(GetSelectedItems(child));
                }
            }

            return items;
        }
        #endregion Private Functions
    }
}
