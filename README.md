# TinyMVVM for Xamarin.Forms
TinyMVVM is a super light MVVM Framework designed specifically for Xamarin.Forms. It's designed to be Easy, Simple and Flexible.

### How does it compare to other options?
- It's super light and super simple
- It's specifically designed for Xamarin.Forms
- Designed to be easy to learn and develop (great when you are not ready for RxUI)
- Uses a Convention over Configuration

### Features
- ViewModel to ViewModel Navigation
- Automatic wiring of BindingContext
- Automatic wiring of Page events (eg. appearing)
- Basic methods (with values) on ViewModel (init, reverseinit, oncreated, ondisposed)
- Built-in IoC Container
- ViewModel Constructor Injection
- Basic methods available in Model, like Alert
- Built-in Navigation types for SimpleNavigation, Tabbed and MasterDetail

### Conventions
This Framework, while simple, is also powerful and uses a Convention over Configuration style.

- A Page must have a corresponding ViewModel, with naming important so a QuoteViewModel must have a QuotePage. The BindingContext on the page will be automatically set with the Model
- A ViewModel can have a Init method that takes a object
- A ViewModel can have a ReverseInit method that also take a object and is called when a model is poped with a object
- A ViewModel can get multi object from NavigationParameters
- ViewModel can have dependancies automatically injected into the Constructor

### Navigation
The Primary form of Navigation in TinyMVVM is ViewModel to ViewModel, this essentially means our views have no idea of Navigation.

So to Navigate between ViewModel use:

```csharp
    await CoreMethods.PushViewModel<QuoteViewModel>(); // Pushes navigation stack
    await CoreMethods.PushViewModel<QuoteViewModel>(null, true); // Pushes a Modal
```

The engine for Navigation in TinyMVVM is done via a simple interface, with methods for Push and Pop. Essentially these methods can control the Navigation of the application in any way they like.

```csharp
    public interface INavigationService
    {
    	Task PushViewModel(Page page, TinyViewModel model, bool modal = false);
    	Task PushViewModel(bool modal = false);
    }
```

Within the PushPage and PopPage you can do any type of navigation that you like, this can be anything from a simple navigation to a advanced nested navigation.

The Framework contains some built in Navigation containers for the different types of Navigation.

###### Basic Navigation - Built In

```csharp
    var page = ViewModelResolver.ResolveViewModel<MainMenuViewModel>();
    var basicNavContainer = new NavigationContainer(page);
    MainPage = basicNavContainer;
```

###### Master Detail - Built In

```csharp
    var masterDetailNav = new MasterDetailNavigationContainer();
    masterDetailNav.Init("Menu");
    masterDetailNav.AddPage<ContactListViewModel>("Contacts", null);
    masterDetailNav.AddPage<QuoteListViewModel>("Pages", null);
    MainPage = masterDetailNav;
```

###### Tabbed Navigation - Built In

```csharp
    var tabbedNavigation = new TabbedNavigationContainer();
    tabbedNavigation.AddTab<ContactListViewModel>("Contacts", null);
    tabbedNavigation.AddTab<QuoteListViewModel>("Pages", null);
    MainPage = tabbedNavigation;
```

###### Implementing Custom Navigation
It's possible to setup any type of Navigation by implementing INavigationService.There's a sample of this in Sample Application named CustomImplementedNav.cs.

### Sample Apps
- Basic Navigation Sample
- Tabbed Navigation Sample
- MasterDetail Navigation Sample
- Tabbed Navigation with MasterDetail Popover Sample (This is called the CustomImplementedNav in the Sample App)

### Inversion of Control (IOC)
So that you don't need to include your own IoC container, TinyMVVM comes with a IoC Container built-in. It's using TinyIoC underneith.

#### To Register services in the container use Register:

```csharp
    TinyIOC.Container.Register<IDatabaseService, DatabaseService>();
```

#### To obtain a service use Resolve:

```csharp
    TinyIOC.Container.Resolve<IDatabaseService>();
```

*This is also what drives constructor injection.

### IOC Container Lifetime Registration Options
We now support a fluent API for setting the object lifetime of object inside the IoC Container.

```csharp
    // By default we register concrete types as 
    // multi-instance, and interfaces as singletons
    TinyIOC.Container.Register<MyConcreteType>(); // Multi-instance
    TinyIOC.Container.Register<IMyInterface, MyConcreteType>(); // Singleton 
    
    // Fluent API allows us to change that behaviour
    TinyIOC.Container.Register<MyConcreteType>().AsSingleton(); // Singleton
    TinyIOC.Container.Register<IMyInterface, MyConcreteType>().AsMultiInstance(); // Multi-instance
```

As you can see below the ITinyIOC interface methods return the IRegisterOptions interface.

```csharp
    public interface ITinyIOC
    {
        object Resolve(Type resolveType);
        IRegisterOptions Register<RegisterType>(RegisterType instance) where RegisterType : class;
        IRegisterOptions Register<RegisterType>(RegisterType instance, string name) where RegisterType : class;
        ResolveType Resolve<ResolveType>() where ResolveType : class;
        ResolveType Resolve<ResolveType>(string name) where ResolveType : class;
        IRegisterOptions Register<RegisterType, RegisterImplementation> ()
            where RegisterType : class
            where RegisterImplementation : class, RegisterType;
    }

The interface that's returned from the register methods is IRegisterOptions.

    public interface IRegisterOptions
    {
        IRegisterOptions AsSingleton();
        IRegisterOptions AsMultiInstance();
        IRegisterOptions WithWeakReference();
        IRegisterOptions WithStrongReference();
        IRegisterOptions UsingConstructor<RegisterType>(Expression<Func<RegisterType>> constructor);
    }
```

### ViewModel - Constructor Injection
When ViewModels are pushed services that are in the IOC container can be pushed into the Constructor.

```csharp
    TinyIOC.Container.Register<IDatabaseService, DatabaseService>();
```

### ViewModel Important Methods

```csharp
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
        /// This method is called when the ViewModel is loaded, the initData is the data that's sent from ViewModel before
        /// </summary>
        /// <param name="initData">Data that's sent to this ViewModel from the pusher</param>
        public virtual void Init(object initData)
        {
        }
    
        /// <summary>
        /// This method is called when a page is Pop'd, it also allows for data to be returned.
        /// </summary>
        /// <param name="returndData">This data that's returned from </param>
        public virtual void ReverseInit(object returndData)
        {
        }
    
        /// <summary>
        /// This method is called when the page is Push'd.
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
    
        /// <summary>
        /// This methods is called when the View is appearing
        /// </summary>
        protected virtual void ViewIsAppearing (object sender, EventArgs e)
        {
        }
    
        /// <summary>
        /// This method is called when the view is disappearing. 
        /// </summary>
        protected virtual void ViewIsDisappearing (object sender, EventArgs e)
        {
        }
```

#### The CoreMethods
Each ViewModel has a property called 'CoreMethods' which is automatically filled when a ViewModel is pushed, it's the basic functions that most apps need like Alerts, Pushing, Poping etc.

```csharp
    public interface IViewModelCoreMethods
    {
    	Task DisplayAlert (string title, string message, string cancel);
    	Task<string> DisplayActionSheet (string title, string cancel, string destruction, params string[] buttons);
    	Task<bool> DisplayAlert (string title, string message, string accept, string cancel);
        Task PushPage<T>(bool modal = false, bool animate = true) where T : Page;
        Task PushPage(Type pageType, bool modal = false, bool animate = true);
        Task PushPage(Page page, bool modal = false, bool animate = true);
        Task PushViewModel<T>(object data = null, bool modal = false, bool animate = true) where T : TinyViewModel;
        Task PushViewModel<T>(NavigationParameters parameters, bool modal = false, bool animate = true) where T : TinyViewModel;
        Task PushViewModel<T, TPage>(object data = null, bool modal = false, bool animate = true) where T : TinyViewModel where TPage : Page;
        Task PushViewModel<T, TPage>(NavigationParameters parameters, bool modal = false, bool animate = true) where T : TinyViewModel where TPage : Page;
        Task PushViewModel(Type viewModelType, object data = null, bool modal = false, bool animate = true);
        Task PushViewModel(Type viewModelType, NavigationParameters parameters, bool modal = false, bool animate = true);
        Task PushViewModel(Type viewModelType, Type pageType, object data = null, bool modal = false, bool animate = true);
        Task PushViewModel(Type viewModelType, Type pageType, NavigationParameters parameters, bool modal = false, bool animate = true);
        Task PopViewModel(bool modal = false, bool animate = true);
    }
```

#### Multiple Navigation Services
It’s always been possible to do any type of navigation in TinyMVVM, with custom or advanced scenarios were done by implementing a custom navigation service. Even with this ability people found it a little hard to do advanced navigation scenarios in TinyMVVM. After I reviewed all the support questions that came in for TinyMVVM I found that the basic issue people had was they wanted to be able to use our built in navigation containers multiple times, two primary examples are 1) within a master detail having a navigation stack in a master and another in the detail 2) The ability to push modally with a new navigation container. In order to support both these scenarios I concluded that the TinyMVVM required the ability to have named NavigationServices so that we could support multiple NavigationService’s.

#### Using multiple navigation containers
Below we’re running two navigation stacks, in a single MasterDetail.

```csharp
    var masterDetailsMultiple = new MasterDetailPage(); //generic master detail page
    
    //we setup the first navigation container with ContactList
    var contactListPage = ViewModelResolver.ResolveViewModel<ContactListViewModel>();
    contactListPage.Title = "Contact List";
    //we setup the first navigation container with name MasterPageArea
    var masterPageArea = new NavigationContainer(contactListPage, "MasterPageArea");
    masterPageArea.Title = "Menu";
    
    masterDetailsMultiple.Master = masterPageArea; //set the first navigation container to the Master
    
    //we setup the second navigation container with the QuoteList 
    var quoteListPage = ViewModelResolver.ResolveViewModel<QuoteListViewModel>();
    quoteListPage.Title = "Quote List";
    //we setup the second navigation container with name DetailPageArea
    var detailPageArea = new FreshNavigationContainer(quoteListPage, "DetailPageArea");
    
    masterDetailsMultiple.Detail = detailPageArea; //set the second navigation container to the Detail
    
    MainPage = masterDetailsMultiple;
```

#### PushModally with new navigation stack

```csharp
    //push a basic page Modally
    var page = ViewModelResolver.ResolveViewModel<MainMenuViewModel>();
    var basicNavContainer = new NavigationContainer(page, "secondNavPage");
    await CoreMethods.PushNewNavigationServiceModal(basicNavContainer, new TinyViewModel[] { page.GetModel() }); 
    
    //push a tabbed page Modally
    var tabbedNavigation = new TabbedNavigationContainer ("secondNavPage");
    tabbedNavigation.AddTab<ContactListViewModel>("Contacts", "contacts.png", null);
    tabbedNavigation.AddTab<QuoteListViewModel> ("Quotes", "document.png", null);
    await CoreMethods.PushNewNavigationServiceModal(tabbedNavigation);
    
    //push a master detail page Modally
    var masterDetailNav = new MasterDetailNavigationContainer("secondNavPage");
    masterDetailNav.Init("Menu", "Menu.png");
    masterDetailNav.AddPage<ContactListViewModel>("Contacts", null);
    masterDetailNav.AddPage<QuoteListViewModel>("Quotes", null);
    await CoreMethods.PushNewNavigationServiceModal(masterDetailNav);
```

#### Switching out NavigationStacks on the Xamarin.Forms MainPage
There's some cases in Xamarin.Forms you might want to run multiple navigation stacks. A good example of this is when you have a navigation stack for the authentication and a stack for the primary area of your application.

To begin with we can setup some names for our navigation containers.

```csharp
    public class NavigationContainerNames
    {
        public const string AuthenticationContainer = "AuthenticationContainer";
        public const string MainContainer = "MainContainer";
    }
```

Then we can create our two navigation containers and assign to the MainPage.

```csharp
    var loginPage = ViewModelResolver.ResolveViewModel<LoginViewModel>();
    var loginContainer = new NavigationContainer(loginPage, NavigationContainerNames.AuthenticationContainer);
    
    var myPitchListViewContainer = new TabbedNavigationContainer(NavigationContainerNames.MainContainer);
    
    MainPage = loginContainer;
```

The Navigation Container will use the name passed as argument to register in this method

```csharp
    public TabbedNavigationContainer(string navigationServiceName)
    {
        NavigationServiceName = navigationServiceName;
        RegisterNavigation();
    }
    
    protected void RegisterNavigation()
    {
        TinyIOC.Container.Register<INavigationService>(this, NavigationServiceName);
    }
```

Once we've set this up we can now switch out our navigation containers.

```csharp
    CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainContainer);
```
That name will be resolved in this method to find the correct Navigation Container

```csharp
    public void SwitchOutRootNavigation(string navigationServiceName)
    {
        INavigationService rootNavigation = TinyIOC.Container.Resolve<INavigationService>(navigationServiceName);
    }
```

#### Custom IOC Containers
The second major request for TinyMVVM was to allow custom IoC containers. In the case that your application already has a container that you want to leverage.

Using a custom IoC container is very simple in that you only need to implement a single interface.

```csharp
    public interface ITinyIOC
    {
        object Resolve(Type resolveType);
        void Register<RegisterType>(RegisterType instance) where RegisterType : class;
        void Register<RegisterType>(RegisterType instance, string name) where RegisterType : class;
        ResolveType Resolve<ResolveType>() where ResolveType : class;
        ResolveType Resolve<ResolveType>(string name) where ResolveType : class;
        void Register<RegisterType, RegisterImplementation> ()
            where RegisterType : class
            where RegisterImplementation : class, RegisterType;
    }
```

And then set the IoC container in the System.

```csharp
    TinyIOC.OverrideContainer(myContainer);
```
#### Other Features
##### WhenAny
WhenAny is an extension method on INotifyPropertyChanged, it's a shorthand way to subscribe to a property changed event.

In the example below, we use any to link up

```csharp
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class ContactViewModel : TinyViewModel
    {
        public ContactViewModel()
        {
            this.WhenAny(HandleContactChanged, o => o.Contact);
        }
    
        void HandleContactChanged(string propertyName)
        {
            //handle the property changed, nice
        }
    
        public Contact Contact { get; set; }
    }
```
