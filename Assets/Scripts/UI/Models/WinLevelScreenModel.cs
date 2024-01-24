namespace UI.Models
{
    public class WinScreenModel : IBaseScreenModel
    {
        public readonly int Level;

        public WinScreenModel()
        {
            Level = AppController.Instance.GameStateModel.LevelNumber;
        }
    }
}