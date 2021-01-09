using Tenlastic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetScene : MonoBehaviour {

    private void Start() {
        string scene = EnvironmentVariables.singleton.gameServerModel.metadata.scene;

        if (SceneManager.GetActiveScene().name == scene)
            return;

        LoadingScreenUI loadingScreenUI = LoadingScreenUI.singleton;
        if (loadingScreenUI == null)
            return;

        loadingScreenUI.OnShow += () => {
            SceneManager.LoadScene(scene);
            loadingScreenUI.Hide();
        };
        loadingScreenUI.Show("Connecting to Game Server");
    }

}
