namespace Xamarin.Forms.IoC
{
    public class FormsIoC
    {
        static FormsIoC()
        {
        }

        private static IFormsIoC _formsIoCContainer;

        public static IFormsIoC Container
        {
            get
            {
                if (_formsIoCContainer == null)
                    _formsIoCContainer = new TinyIoCBuiltIn();

                return _formsIoCContainer;
            }
        }

        public static void OverrideContainer(IFormsIoC overrideContainer)
        {
            _formsIoCContainer = overrideContainer;
        }
    }
}