using Configs;
using CoreGameplay;
using TMPro;
using UI.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class WinScreenWithModelView : BaseScreenWithModelView<WinScreenModel>
    {
        [SerializeField] private TextMeshProUGUI LevelLabel;
        [SerializeField] private Button MenuButton;
        [SerializeField] private Button NextButton;

        private void OnEnable()
        {
            Model = new WinScreenModel();

            LevelLabel.text = $"{(Model.Level + 1)}";

            MenuButton.onClick.AddListener(OnMenuButtonClicked);
            NextButton.onClick.AddListener(OnNextButtonClicked);
        }

        private void OnDisable()
        {
            MenuButton.onClick.RemoveListener(OnMenuButtonClicked);
            NextButton.onClick.RemoveListener(OnNextButtonClicked);
        }

        private void OnMenuButtonClicked()
        {
            LevelController.Instance.UnloadLevel();
            ScreensManager.ChangeScreen(ScreensType.MainMenuScreen);
        }

        private void OnNextButtonClicked()
        {
            LevelController.Instance.NextLevel();
            LevelController.Instance.UnloadLevel();
            LevelController.Instance.LoadLevel();
            ScreensManager.ChangeScreen(ScreensType.CoreGameplayScreen);

        }
    }
}