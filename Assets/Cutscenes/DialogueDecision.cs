using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueDecision : Dialogue
	{
	public GameObject decision;

		public override IEnumerator StartDialogue()
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

		while (true) {
			if (Input.GetKeyDown(KeyCode.F))
			{
				break;
			}

			yield return 0;
		}

		HideIcons();
		_isEndOfDialogue = false;
		_isDialoguePlaying = false;
		decision.SetActive (true);
		gameObject.SetActive(false);
	}
}


