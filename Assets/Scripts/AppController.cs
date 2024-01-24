using Models;

public class AppController
{
    private static AppController _instance;

    public static AppController Instance
    {
        get { return _instance ??= new AppController(); }
    }

    public GameStateModel GameStateModel => _gameStateModel;

    private readonly GameStateModel _gameStateModel;

    private AppController()
    {
        _gameStateModel = new GameStateModel();
        _gameStateModel.Load();
    }
}