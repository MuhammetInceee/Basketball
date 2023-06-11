using UnityEngine;
using System.Collections;

public class GoalArea : MonoBehaviour {

	public ParticleSystem psStar;


	public int Score {get; private set;}


	// Use this for initialization
	private void Start () 
	{
		Score = 0;
	}
	
	private void OnTriggerEnter (Collider other) 
	{
		if(GameManager.Instance.gameState != GameState.Game) return;
		
		var sb = other.GetComponent<ShotBall>();
		if (sb == null) return;
		Score++;
		GameManager.Instance.ScoreUpdate(Score);
		psStar.Play();
	}

}
