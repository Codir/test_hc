namespace UI.Models
{
    public class GameScreenModel : IBaseScreenModel
    {
        public int Level;

        public GameScreenModel()
        {
            Level = AppController.Instance.GameStateModel.LevelNumber;
        }
    }
}