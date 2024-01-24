namespace UI.Models
{
    public class GameScreenModel : IBaseScreenModel
    {
        public readonly int Level;

        public GameScreenModel()
        {
            Level = AppController.Instance.GameStateModel.LevelNumber;
        }
    }
}