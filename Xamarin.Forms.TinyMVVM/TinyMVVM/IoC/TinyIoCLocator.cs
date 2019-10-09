namespace TinyMVVM.IoC
{
    public static class TinyIoCLocator
    {
        static TinyIoCLocator()
        {
        }

        private static ITinyIoCBuiltIn _formsIoCContainer;

        public static ITinyIoCBuiltIn Container
        {
            get
            {
                if (_formsIoCContainer == null)
                    _formsIoCContainer = new TinyIoCBuiltIn();

                return _formsIoCContainer;
            }
        }

        public static void OverrideContainer(ITinyIoCBuiltIn overrideContainer)
        {
            _formsIoCContainer = overrideContainer;
        }
    }
}