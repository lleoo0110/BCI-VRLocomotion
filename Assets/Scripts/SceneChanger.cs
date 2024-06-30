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
            // �I�u�W�F�N�g���\���ɂ���
            objectToKeep.SetActive(false);
        }
        SceneManager.LoadScene(sceneToLoad);
    }
}