using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextParent : MonoBehaviour
{
	public static TextParent singleton;
	[SerializeField] private TextMeshProUGUI textObject;
	[SerializeField] private GameObject graphicsObject;
	[SerializeField] private bool textOnObject = true;
	private Camera _cam;
	private IEnumerator textTimer;
	private bool textOnScreen = false;

	private void Awake() { singleton = this;
	}

	private void Start() { _cam = Camera.main; }

	public static void SpawnText( ScriptableInteractionText text, Vector3 position )
	{
		if( text == null ) return;
		if( singleton == null ) return;
		
		singleton.MoveText(text, position);
	}
	public static void AbortTextPrompt()
	{
		if( singleton == null ) return;
		singleton.StopText();
	}

	public void MoveText(ScriptableInteractionText text, Vector3 position)
	{
		if (textOnObject)
		{
			Vector2 pos = _cam.WorldToScreenPoint(position);
			transform.position = pos;
		}
		
		
		if (textOnScreen)
		{
			StopCoroutine(textTimer);
		}
		graphicsObject.gameObject.SetActive(true);
		textObject.text = text.interactionText;
		
		ForceUpdateLayout();
		
		textOnScreen = true;
		textTimer = TextTimer(text.secondsOnScreen);
		StartCoroutine(textTimer);
	}

	public void ForceUpdateLayout()
	{
		//I hate unity UI
		LayoutRebuilder.ForceRebuildLayoutImmediate(textObject.transform.parent as RectTransform);
	}

	public IEnumerator TextTimer(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		graphicsObject.SetActive(false);
		textOnScreen = false;
	}
	public void StopText()
	{
		if (textOnScreen)
		{
			StopCoroutine(textTimer);
			textOnScreen = false;
		}
	}
}
