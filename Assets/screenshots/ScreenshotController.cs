#if UNITY_EDITOR

using System.IO;
using UnityEngine;
using System.Collections;
using UnityEditor;

[HelpURLAttribute("https://docs.google.com/document/d/1DelU_13Cl8z1yElxtay4CzMhFAeNzNbBrFZbE3e9KXM/edit?usp=sharing")]
public class ScreenshotController : MonoBehaviour
{
	
	

	[Header("Set corresponding name in Game window resolution drop list.")]
	[Header("[Use S key or right click during execution to take screenshots.]")]
	public string[] resolutionNames;

	[Header("Find screenshots in root folder")]
	public string ScreenshotFolderName = "Screenshots";


	private int count = 1;

	private bool locked = false;

	private Vector2 currentResolution;
	private float currentTimeScale;

	void Start ()
	{
		if (!Directory.Exists(ScreenshotFolderName))
		{
			Debug.Log("Created directory for screenshots: " + ScreenshotFolderName);
			Directory.CreateDirectory(ScreenshotFolderName);
		}
		count = Directory.GetFiles(ScreenshotFolderName).Length / resolutionNames.Length;

		currentResolution = GameViewUtils.GetMainGameViewSize();
		currentTimeScale = Time.timeScale;
	}

	void Update ()
	{
		if ((Input.GetKeyDown(KeyCode.S) || Input.GetMouseButtonDown(1)) && !locked)
		{
			StartCoroutine(CaptureMultipleResolutions());
		}
	}

	public IEnumerator CaptureMultipleResolutions ()
	{
		locked = true;
		Time.timeScale = 0;
		count++;
		yield return new WaitForEndOfFrame();

		for (int i = 0; i < resolutionNames.Length; i++)
		{
			if (GameViewUtils.SizeExists(resolutionNames[i]))
			{
				GameViewUtils.SetSize(resolutionNames[i]);
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				Capture(resolutionNames[i]);
			}
		}
		GameViewUtils.SetSize(currentResolution);
		Time.timeScale = currentTimeScale;
		locked = false;
	}

	void Capture ( string name )
	{
		string screenshotName = ScreenshotFolderName + "/" + Application.productName + "_" + name + "_" + count + ".png";
		ScreenCapture.CaptureScreenshot(screenshotName);
		Debug.Log("Saved screenshot : \"" + screenshotName + "\"");
	}
}
#endif