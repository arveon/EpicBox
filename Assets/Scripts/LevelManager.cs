using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEditor;
using System.Collections;

public class LevelManager : MonoBehaviour
{

	public void ChangeLevel(string levelName)
	{
		SceneManager.LoadScene(levelName);
	}
	
}
