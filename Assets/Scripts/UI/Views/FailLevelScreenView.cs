using Configs;
using CoreGameplay;
using TMPro;
using UI.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class FailScreenWithModelView : BaseScreenWithModelView<FailScreenModel>
    {
        [SerializeField] private TextMeshProUGUI LevelLabel;
        [SerializeField] private Button MenuButton;
        [SerializeField] private Button ReplayButton;

        private void OnEnable()
        {
            Model = new FailScreenModel();

            LevelLabel.text = $"{(Model.Level + 1)}";

            MenuButton.onClick.AddListener(OnMenuButtonClicked);
            ReplayButton.onClick.AddListener(OnReplayButtonClicked);
        }

        private void OnDisable()
        {
            MenuButton.onClick.RemoveListener(OnMenuButtonClicked);
            ReplayButton.onClick.RemoveListener(OnReplayButtonClicked);
        }

        private void OnMenuButtonClicked()
        {
            LevelController.Instance.UnloadLevel();
            ScreensManager.ChangeScreen(ScreensType.MainMenuScreen);
        }

        private void OnReplayButtonClicked()
        {
            LevelController.Instance.UnloadLevel();
            LevelController.Instance.LoadLevel();
            ScreensManager.ChangeScreen(ScreensType.CoreGameplayScreen);
        }
    }
}