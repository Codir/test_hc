namespace UI.Models
{
    public class MenuScreenModel : IBaseScreenModel
    {
        public readonly int Level;

        public MenuScreenModel()
        {
            Level = AppController.Instance.GameStateModel.LevelNumber;
        }
    }
}