namespace UI.Models
{
    public class MenuScreenModel : IBaseScreenModel
    {
        public int Level;

        public MenuScreenModel()
        {
            Level = AppController.Instance.GameStateModel.LevelNumber;
        }
    }
}