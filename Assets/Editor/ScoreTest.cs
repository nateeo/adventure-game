using UnityEngine;
using UnityEditor;
using NUnit.Framework;


// This class tests the correct functionality of the player's scoring mechanic and the persistence
// Time based and bonus based scoring
public class ScoreTest {

	public PlayerScore GenerateMockScore() {
		var mock = new GameObject ();
		var textMock = mock.AddComponent<UnityEngine.UI.Text> ();
		var score = mock.AddComponent<PlayerScore>();
		score.bountyText = textMock;
		score.resetScore ();
		return score;
	}

	// This tests for the correct bonus score calculations and persistence
	[Test]
	public void BonusScore() {
		Text bountyText;
		var score = GenerateMockScore ();
		Assert.That (score.numberOfBonuses == 0);
		score.maxNumberOfBonuses = 5;
		score.incrementBonus ();
		Assert.That (score.numberOfBonuses == 1);
		score.incrementBonus ();
		score.incrementBonus ();
		score.incrementBonus ();
		score.incrementBonus ();
		// check that the text displays correctly
		Assert.That (score.bountyText.text == "Bonus fugitives: 5");


		// check that scoring is correct and persists
		score.calculateScore();
		score.resetScore ();
		Assert.That (PlayerPrefs.GetInt ("BonusScore") == 3);
		Assert.That (PlayerPrefs.GetInt ("Bonus") == 5);

		// check that the score system was reset
		Assert.That(score.numberOfBonuses == 0);

		// check that empty score works
		var emptyScore = GenerateMockScore();
		emptyScore.calculateScore ();
		Assert.That(PlayerPrefs.GetInt("Bonus") == 0);
	}

	// This tests for the correct timing score and persistence
	[Test]
	public void TimeScore() {
		var emptyScore = GenerateMockScore ();
		emptyScore.startTime = 0f;
		emptyScore.maxTime = 0f;
		emptyScore.calculateScore ();
		Assert.That (PlayerPrefs.GetInt ("TimeScore") == 0);

		var score = GenerateMockScore ();
		var startTime = 1f;
		score.startTime = startTime;
		score.maxTime = 3f;
		score.calculateScore ();

		// check that time was reset
		score.resetScore();
		Assert.That(score.startTime != startTime);
	}
}
