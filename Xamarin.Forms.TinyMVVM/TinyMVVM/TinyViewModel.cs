using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.TinyMVVM
{
    public class TinyViewModel : INotifyPropertyChanged
    {
        private NavigationPage _navigationPage;

        /// <summary>
        /// This event is raise when a page is Popped, this might not be raise everytime a page is Popped.
        /// Note* this might be raised multiple times.
        /// </summary>
        public event EventHandler PageWasPopped;

        /// <summary>
        /// This property is used by the TinyContentPage and allows you to set the toolbar items on the page.
        /// </summary>
        public ObservableCollection<ToolbarItem> ToolbarItems { get; set; }

        /// <summary>
        /// The previous view model, that's automatically filled, on push
        /// </summary>
        public TinyViewModel PreviousViewModel { get; set; }

        /// <summary>
        /// A reference to the current page, that's automatically filled, on push
        /// </summary>
        public Page CurrentPage { get; set; }

        /// <summary>
        /// Core methods are basic built in methods for the App including Pushing, Pop and Alert
        /// </summary>
        public IViewModelCoreMethods CoreMethods { get; set; }

        /// <summary>
        /// A reference to the current page, that's automatically filled, on push
        /// </summary>
        public NavigationParameters Parameters { get; set; } = new NavigationParameters();

        /// <summary>
        /// Is true when this model is the first of a new navigation stack
        /// </summary>
        internal bool IsModalFirstChild;

        /// <summary>
        /// Used when a page is shown modal and wants a new Navigation Stack
        /// </summary>
        public string CurrentNavigationServiceName = Constants.DefaultNavigationServiceName;

        /// <summary>
        /// Used when a page is shown modal and wants a new Navigation Stack
        /// </summary>
        public string PreviousNavigationServiceName;

        /// <summary>
        /// This means the current ViewModel is shown modally and can be pop'd modally
        /// </summary>
        public bool IsModalAndHasPreviousNavigationStack()
        {
            return !string.IsNullOrWhiteSpace(PreviousNavigationServiceName) && PreviousNavigationServiceName != CurrentNavigationServiceName;
        }

        private bool isBusy = false;

        /// <summary>
        /// This means the current page is busy
        /// </summary>
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        private string title = string.Empty;

        /// <summary>
        /// Page title
        /// </summary>
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public TinyViewModel()
        {
        }

        /// <summary>
        /// This method is called when the ViewModel is loaded, the initData is the data that's sent from pagemodel before
        /// </summary>
        /// <param name="data">Data that's sent to this PageModel from the pusher</param>
        public virtual void Init(object data)
        {
        }

        /// <summary>
        /// This method is called when a page is Pop'd, it also allows for data to be returned.
        /// </summary>
        /// <param name="returnedData">This data that's returned from </param>
        public virtual void ReverseInit(object returnedData)
        {
        }

        /// <summary>
        /// This method is called when a page is Push'd or is set as page root in navigation stack.
        /// </summary>
        public virtual void OnCreated()
        {
        }

        /// <summary>
        /// This method is called when a page is Pop'd.
        /// </summary>
        public virtual void OnDisposed()
        {
        }

        internal void WireEvents()
        {
            CurrentPage.Appearing += new WeakEventHandler<EventArgs>(ViewIsAppearing).Handler;
            CurrentPage.Disappearing += new WeakEventHandler<EventArgs>(ViewIsDisappearing).Handler;
        }

        protected virtual void ViewIsAppearing(object sender, EventArgs e)
        {
            if (!_alreadyAttached)
                AttachPageWasPoppedEvent();
        }

        private bool _alreadyAttached = false;

        /// <summary>
        /// This is used to attach the page was popped method to a NavigationPage if available
        /// </summary>
        private void AttachPageWasPoppedEvent()
        {
            var navPage = (CurrentPage.Parent as NavigationPage);
            if (navPage != null)
            {
                _navigationPage = navPage;
                _alreadyAttached = true;
                navPage.Popped += new WeakEventHandler<NavigationEventArgs>(HandleNavPagePopped).Handler;
            }
        }

        private void HandleNavPagePopped(object sender, NavigationEventArgs e)
        {
            if (e.Page == CurrentPage)
            {
                RaisePageWasPopped();
                OnDisposed();
            }
        }

        public void RaisePageWasPopped()
        {
            PageWasPopped?.Invoke(this, EventArgs.Empty);

            var navPage = (CurrentPage.Parent as NavigationPage);
            if (navPage != null)
                navPage.Popped -= HandleNavPagePopped;

            if (_navigationPage != null)
                _navigationPage.Popped -= HandleNavPagePopped;

            _navigationPage = null;

            CurrentPage.Appearing -= ViewIsAppearing;
            CurrentPage.Disappearing -= ViewIsDisappearing;
            CurrentPage.BindingContext = null;
        }

        protected virtual void ViewIsDisappearing(object sender, EventArgs e)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}