namespace UI.Models
{
    public class FailScreenModel : IBaseScreenModel
    {
        public readonly int Level;

        public FailScreenModel()
        {
            Level = AppController.Instance.GameStateModel.LevelNumber;
        }
    }
}