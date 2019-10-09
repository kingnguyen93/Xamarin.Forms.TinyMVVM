namespace TinyMVVM
{
    public class TinyModel : ExtendedBindableObject
    {
        private bool isBusy = false;

        /// <summary>
        /// This means the current model is busy
        /// </summary>
        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
    }
}