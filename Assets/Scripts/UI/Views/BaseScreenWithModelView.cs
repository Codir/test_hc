using UI.Models;

namespace UI.Views
{
    public class BaseScreenWithModelView<T> : BaseScreenView
        where T : IBaseScreenModel, new()
    {
        protected T Model;
    }
}