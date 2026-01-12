using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        public Button startButton;

       private void Start()
        {
            if (startButton != null)
            {
                startButton.onClick.AddListener(StartGame);
            }
        }
       
        public static void StartGame()
        {
            SceneManager.LoadScene("Gameplay");
        }
    }
}
