using CoreGameplay.Controllers;
using TMPro;
using UI.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class MenuScreenWithModelView : BaseScreenWithModelView<MenuScreenModel>
    {
        [SerializeField] private TextMeshProUGUI LevelLabel;
        [SerializeField] private Button PlayButton;

        private void OnEnable()
        {
            Model = new MenuScreenModel();

            LevelLabel.text = $"{Model.Level + 1}";

            PlayButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnDisable()
        {
            PlayButton.onClick.RemoveListener(OnPlayButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            SoundsController.PlaySound(Sfx.Click);

            LevelController.Instance.LoadLevel();
            ScreensManager.ChangeScreen(ScreensType.CoreGameplayScreen);
        }
    }
}