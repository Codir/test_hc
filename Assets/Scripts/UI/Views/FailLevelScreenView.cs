using CoreGameplay.Controllers;
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

            SoundsController.PlaySound(Sfx.Fail);
        }

        private void OnDisable()
        {
            MenuButton.onClick.RemoveListener(OnMenuButtonClicked);
            ReplayButton.onClick.RemoveListener(OnReplayButtonClicked);
        }

        private void OnMenuButtonClicked()
        {
            SoundsController.PlaySound(Sfx.Click);

            LevelController.Instance.UnloadLevel();
            ScreensManager.ChangeScreen(ScreensType.MainMenuScreen);
        }

        private void OnReplayButtonClicked()
        {
            SoundsController.PlaySound(Sfx.Click);

            LevelController.Instance.ReloadLevel();
            ScreensManager.ChangeScreen(ScreensType.CoreGameplayScreen);
        }
    }
}