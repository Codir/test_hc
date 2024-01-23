namespace UI.Models
{
    public class FailScreenModel : IBaseScreenModel
    {
        public int Level;

        public FailScreenModel()
        {
            Level = AppController.Instance.GameStateModel.LevelNumber;
        }
    }
}