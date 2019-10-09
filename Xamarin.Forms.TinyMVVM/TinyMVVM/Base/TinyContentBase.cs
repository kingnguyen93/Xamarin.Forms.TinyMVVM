using System.Collections.Specialized;

using Xamarin.Forms;

namespace TinyMVVM
{
    public class TinyContentBase : ContentPage
    {
        public TinyContentBase()
        {
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is TinyViewModel viewModel && viewModel.ToolbarItems != null && viewModel.ToolbarItems.Count > 0)
            {
                viewModel.ToolbarItems.CollectionChanged += ViewModel_ToolbarItems_CollectionChanged;

                foreach (var toolBarItem in viewModel.ToolbarItems)
                {
                    if (!(ToolbarItems.Contains(toolBarItem)))
                    {
                        ToolbarItems.Add(toolBarItem);
                    }
                }
            }
        }

        private void ViewModel_ToolbarItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (ToolbarItem toolBarItem in e.NewItems)
            {
                if (!(ToolbarItems.Contains(toolBarItem)))
                {
                    ToolbarItems.Add(toolBarItem);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (ToolbarItem toolBarItem in e.OldItems)
                {
                    if (!(ToolbarItems.Contains(toolBarItem)))
                    {
                        ToolbarItems.Add(toolBarItem);
                    }
                }
            }
        }
    }
}