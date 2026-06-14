using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardBattler
{
    // Savaş bitince "Kazandın / Kaybettin" panelini gösterir.
    // Responsive olması için bir Canvas (Screen Space) üzerinde kurulması önerilir.
    public class BattleResultView : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] TMP_Text messageText;
        [SerializeField] string winMessage = "Kazandın!";
        [SerializeField] string loseMessage = "Kaybettin!";
        [SerializeField] Button restartButton; // opsiyonel

        void Awake()
        {
            if (restartButton != null)
                restartButton.onClick.AddListener(Restart);
            Hide();
        }

        public void ShowResult(bool won)
        {
            if (panel != null) panel.SetActive(true);
            if (messageText != null) messageText.text = won ? winMessage : loseMessage;
        }

        public void Hide()
        {
            if (panel != null) panel.SetActive(false);
        }

        void Restart()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene.buildIndex);
        }
    }
}
