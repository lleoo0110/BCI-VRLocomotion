using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad;
    public GameObject objectToKeep;

    public void ChangeScene()
    {
        if (objectToKeep != null)
        {
            if (objectToKeep.GetComponent<ObjectPersister>() == null)
            {
                objectToKeep.AddComponent<ObjectPersister>();
            }
            // オブジェクトを非表示にする
            objectToKeep.SetActive(false);
        }
        SceneManager.LoadScene(sceneToLoad);
    }
}