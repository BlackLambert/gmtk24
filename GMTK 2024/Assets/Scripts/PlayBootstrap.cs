using UnityEditor;
using UnityEditor.SceneManagement;

namespace Game
{
    [InitializeOnLoad]
    public class PlayFromSpecificScene
    {
        private static string sceneToPlay = "Assets/Scenes/Bootstrap.unity";

        static PlayFromSpecificScene()
        {
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneToPlay);
        }
    }
}
