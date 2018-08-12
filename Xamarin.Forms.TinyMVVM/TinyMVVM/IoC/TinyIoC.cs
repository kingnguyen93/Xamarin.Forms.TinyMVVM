namespace Xamarin.Forms.TinyMVVM
{
    public class TinyIOC
    {
        static TinyIOC()
        {
        }

        private static ITinyIOC _formsIoCContainer;

        public static ITinyIOC Container
        {
            get
            {
                if (_formsIoCContainer == null)
                    _formsIoCContainer = new TinyIOCBuiltIn();

                return _formsIoCContainer;
            }
        }

        public static void OverrideContainer(ITinyIOC overrideContainer)
        {
            _formsIoCContainer = overrideContainer;
        }
    }
}