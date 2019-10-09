using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TinyMVVM.Extensions;
using TinyMVVM.IoC;

using Xamarin.Forms;

namespace TinyMVVM
{
    public class MasterDetailNavigationContainer : MasterDetailPage, INavigationService
    {
        public string NavigationServiceName { get; private set; }

        private ContentPage menuPage;
        private readonly ListView listView = new ListView();
        private readonly List<Page> pagesInner = new List<Page>();

        public Dictionary<string, Page> Pages { get; } = new Dictionary<string, Page>();

        protected ObservableCollection<string> PageNames { get; } = new ObservableCollection<string>();

        public MasterDetailNavigationContainer() : this(Constants.DefaultNavigationServiceName)
        {
        }

        public MasterDetailNavigationContainer(string navigationServiceName)
        {
            listView.SelectionMode = ListViewSelectionMode.None;

            NavigationServiceName = navigationServiceName;
            RegisterNavigation();
        }

        public void Init(string menuTitle, string menuIcon = null)
        {
            CreateMenuPage(menuTitle, menuIcon);
        }

        protected virtual void RegisterNavigation()
        {
            _ = TinyIoCLocator.Container.Register<INavigationService>(this, NavigationServiceName);
        }

        protected virtual void CreateMenuPage(string menuPageTitle, string menuIcon = null)
        {
            listView.ItemsSource = PageNames;

            listView.ItemTapped += (sender, args) =>
            {
                if (Pages.ContainsKey((string)args.Item))
                {
                    Detail = Pages[(string)args.Item];

                    if (Detail.GetModel() is TinyViewModel viewModel)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            viewModel.OnNavigatedToAsync(new NavigationParameters());
                            viewModel.OnNavigatedTo(new NavigationParameters());
                        });
                    }
                    if (Detail is INavigatedAware aware)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            aware.OnNavigatedToAsync(new NavigationParameters());
                            aware.OnNavigatedTo(new NavigationParameters());
                        });
                    }
                }

                IsPresented = false;
            };

            menuPage = new ContentPage
            {
                Title = menuPageTitle,
                Content = listView
            };

            var navPage = new NavigationPage(menuPage) { Title = "Menu" };

            if (!string.IsNullOrEmpty(menuIcon))
                navPage.IconImageSource = menuIcon;

            Master = navPage;
        }

        public virtual void AddPage<T>(string title, string icon = null, NavigationParameters parameters = default) where T : TinyViewModel
        {
            AddPage(typeof(T), title, icon, parameters);
        }

        public virtual void AddPage(Type pageType, string title, string icon = null, NavigationParameters parameters = default)
        {
            var page = ViewModelResolver.ResolveViewModel(pageType, parameters);
            AddPage(page, title, icon, parameters);
        }

        private void AddPage(Page page, string title, string icon = null, NavigationParameters parameters = default)
        {
            pagesInner.Add(page);

            if (page.GetModel() is TinyViewModel viewModel)
            {
                viewModel.CurrentNavigationServiceName = NavigationServiceName;
            }

            var container = CreateContainerPage(page);
            container.Title = title;
            if (icon != null)
                container.IconImageSource = icon;

            PageNames.Add(title);
            Pages.Add(title, container);

            if (Pages.Count == 1)
            {
                Detail = container;
            }
        }

        private Page CreateContainerPageSafe(Page root)
        {
            if (root is NavigationPage || root is MasterDetailPage || root is TabbedPage)
                return root;

            return CreateContainerPage(root);
        }

        protected virtual Page CreateContainerPage(Page root)
        {
            return new NavigationPage(root);
        }

        public Task PushPage(Page page, bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PushModalAsync(CreateContainerPageSafe(page));

            return Detail.Navigation.PushAsync(page, animate); //TODO: make this better
        }

        public Task PopPage(bool modal = false, bool animate = true)
        {
            if (modal)
                return Navigation.PopModalAsync(animate);

            return Detail.Navigation.PopAsync(animate); //TODO: make this better
        }

        public Task PopToRoot(bool animate = true)
        {
            return (Detail as NavigationPage).PopToRootAsync(animate);
        }

        public void NotifyChildrenPageWasPopped()
        {
            if (Master is INavigationService)
                ((INavigationService)Master).NotifyChildrenPageWasPopped();

            foreach (var page in Pages.Values)
            {
                if (page is INavigationService)
                {
                    ((INavigationService)page).NotifyChildrenPageWasPopped();
                }
                else if (page is ContentPage)
                {
                    if (page.GetModel() is TinyViewModel viewModel)
                        viewModel.RaisePageWasPopped();
                }
            }

            if (Pages != null && !Pages.ContainsValue(Detail) && Detail is INavigationService)
            {
                ((INavigationService)Detail).NotifyChildrenPageWasPopped();
            }
        }

        public Task<TinyViewModel> SwitchSelectedRootViewModel<T>() where T : TinyViewModel
        {
            var tabIndex = pagesInner.FindIndex(o => o.GetModel().GetType().FullName == typeof(T).FullName);

            listView.SelectedItem = PageNames[tabIndex];

            return Task.FromResult((Detail as NavigationPage).CurrentPage.GetModel());
        }
    }
}