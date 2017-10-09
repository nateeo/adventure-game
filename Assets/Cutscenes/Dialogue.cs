using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Text))]
public class Dialogue : MonoBehaviour
{
	private Text _textComponent;

	[TextArea(3, 10)]
	public string[] DialogueStrings;

	public float SecondsBetweenCharacters = 0.05f;
	public float CharacterRateMultiplier = 0.01f;

	public KeyCode DialogueInput = KeyCode.F;

	private bool _isStringBeingRevealed = false;
	private bool _isDialoguePlaying = false;
	private bool _isEndOfDialogue = false;

	public GameObject blackOutScreen;

	public GameObject ContinueIcon;

	// Use this for initialization
	void Start ()
	{
		_textComponent = GetComponent<Text>();
		_textComponent.text = "";
		blackOutScreen.SetActive (true);
		ContinueIcon.SetActive (true);
	}

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.F)) {
			if (!_isDialoguePlaying) {
				_isDialoguePlaying = true;
				StartCoroutine (StartDialogue ());
			}

		} else if (Input.GetKeyDown (KeyCode.Space)) {
			SceneManager.LoadScene (1);
		}
	}

	private IEnumerator StartDialogue()
	{
		int dialogueLength = DialogueStrings.Length;
		int currentDialogueIndex = 0;

		while (currentDialogueIndex < dialogueLength || !_isStringBeingRevealed)
		{
			if (!_isStringBeingRevealed)
			{
				_isStringBeingRevealed = true;
				StartCoroutine(DisplayString(DialogueStrings[currentDialogueIndex++]));

				if (currentDialogueIndex >= dialogueLength)
				{
					_isEndOfDialogue = true;
				}
			}

			yield return 0;
		}

		while (true)
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				break;
			}

			yield return 0;
		}

		HideIcons();
		_isEndOfDialogue = false;
		_isDialoguePlaying = false;
		Debug.Log ("Is this the end of the script? fucking ded");
		SceneManager.LoadScene (1);
	}

	private IEnumerator DisplayString(string stringToDisplay)
	{
		int stringLength = stringToDisplay.Length;
		int currentCharacterIndex = 0;

		HideIcons();

		_textComponent.text = "";

		while (currentCharacterIndex < stringLength)
		{
			_textComponent.text += stringToDisplay[currentCharacterIndex];
			currentCharacterIndex++;

			if (currentCharacterIndex < stringLength)
			{
				if (Input.GetKey(KeyCode.F))
				{
					yield return new WaitForSeconds(SecondsBetweenCharacters*CharacterRateMultiplier);
				}
				else
				{
					yield return new WaitForSeconds(SecondsBetweenCharacters);
				}
			}
			else
			{
				break;
			}
		}

		ShowIcon();

		while (true)
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				break;
			}

			yield return 0;
		}

		HideIcons();

		_isStringBeingRevealed = false;
		_textComponent.text = "";
		blackOutScreen.SetActive (false);

	}

	private void HideIcons()
	{
		ContinueIcon.SetActive(false);
	}

	private void ShowIcon()
	{
		if (_isEndOfDialogue)
		{
			return;
		}

		ContinueIcon.SetActive(true);
	}
}