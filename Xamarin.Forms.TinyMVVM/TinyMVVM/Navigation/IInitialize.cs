using System.Threading.Tasks;

namespace TinyMVVM
{
    public interface IInitialize
    {
        void Initialize(NavigationParameters parameters);

        Task InitializeAsync(NavigationParameters parameters);
    }
}