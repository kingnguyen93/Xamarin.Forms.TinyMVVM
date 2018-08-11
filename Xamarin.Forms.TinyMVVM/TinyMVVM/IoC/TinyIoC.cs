namespace Xamarin.Forms.TinyMVVM
{
    public class TinyIoC
    {
        static TinyIoC()
        {
        }

        private static ITinyIoC _formsIoCContainer;

        public static ITinyIoC Container
        {
            get
            {
                if (_formsIoCContainer == null)
                    _formsIoCContainer = new TinyIoCBuiltIn();

                return _formsIoCContainer;
            }
        }

        public static void OverrideContainer(ITinyIoC overrideContainer)
        {
            _formsIoCContainer = overrideContainer;
        }
    }
}