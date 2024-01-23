using System;
using System.Linq;
using UI.Views;
using UnityEngine;

namespace UI
{
    public enum ScreensType
    {
        MainMenuScreen,
        CoreGameplayScreen,
        WinLevelScreen,
        FailLevelScreen
    }

    [Serializable]
    public class ScreenConfig
    {
        public ScreensType Type;
        public BaseScreenView Screen;
    }

    public class ScreensManager : MonoBehaviour
    {
        private static ScreensManager _instance;

        [SerializeField] private ScreensType StartScreenType;
        [SerializeField] private ScreenConfig[] ScreensList;

        private static BaseScreenView _currentScreen;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Debug.LogError("ScreensManager _instance already exist");
            }
        }

        private void Start()
        {
            ChangeScreen(StartScreenType);
        }

        public static void ChangeScreen(ScreensType type)
        {
            HideCurrentScreen();

            _currentScreen = GetScreenByType(type);

            if (_currentScreen == null) return;
            _currentScreen.Show();
        }

        private static void HideCurrentScreen()
        {
            if (_currentScreen == null) return;

            _currentScreen.Hide();
            _currentScreen = null;
        }

        private static BaseScreenView GetScreenByType(ScreensType type)
        {
            return (from screen in _instance.ScreensList where screen.Type == type select screen.Screen)
                .FirstOrDefault();
        }
    }
}