namespace UI.Models
{
    public class WinScreenModel : IBaseScreenModel
    {
        public int Level;

        public WinScreenModel()
        {
            Level = AppController.Instance.GameStateModel.LevelNumber;
        }
    }
}