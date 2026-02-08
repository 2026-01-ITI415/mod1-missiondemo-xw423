using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode{ idle, playing, levelEnd }

public class MissionDemolition : MonoBehaviour
{
	static private MissionDemolition S; // private singleton
	
	static public bool gameover = false;

	[Header("Inscribed")]
	public Text uitLevel; // UIText_Level component
	public Text uitShots; // UIText_Shots component
	public Text GameOver; // game over componont
	public Vector3 castlePos; // place to put castles
	public GameObject[] castles;

	[Header("Dynamic")]
	public int level; // current level
	public int levelMax; // total number of levels
	public int shotsTaken; 
	public GameObject castle; // current castle
	public GameMode mode = GameMode.idle;
	public string showing = "Show Sliongshot"; // FollowCam mode

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		S = this; // define singleton
		level = 0;
		shotsTaken = 0;
		levelMax = castles.Length;
		StartLevel();
	}
	void StartLevel()
	{
		// get rid of the old castle if one exists
		if( castle != null )
			Destroy(castle);

		// destroy existing projectiles
		Projectile.DESTROY_PROJECTILES();

		// instantiate new castle
		castle = Instantiate<GameObject>( castles[level] );
		castle.transform.position = castlePos;

		shotsTaken = 0;

		// reset goal
		Goal.goalMet = false;

		UpdateGUI();
		
		mode = GameMode.playing;

		// zoom out to show both
		FollowCam.SWITCH_VIEW( FollowCam.eView.both );
	}
	void UpdateGUI()
	{
		// show data in the GUITexts
		uitLevel.text = "Level: "+(level + 1)+" of "+levelMax;
		uitShots.text = "Shots Taken: "+shotsTaken;
	}


	// Update is called once per frame
	void Update()
	{
		UpdateGUI();

		// check for level end
		if( (mode == GameMode.playing) && Goal.goalMet )
		{
			// change mode to levelEnd
			mode = GameMode.levelEnd;
			// zoom out to show both
			FollowCam.SWITCH_VIEW(FollowCam.eView.both);

			// start next level in 2 sec
			Invoke("NextLevel", 2f);
		}
		if( !Goal.goalMet && shotsTaken == 3 )
		{
			GameOver.text = "Game Over!";
			gameover = true;
		}
	}
	void NextLevel()
	{
		level++;
		if( level == levelMax )
		{
			level = 0;
			shotsTaken = 0;
		}
		StartLevel();
	}

	// static method to allow shotsTaken to increment from anywhere
	static public void SHOT_FIRED()
	{
		S.shotsTaken++;
	}
	// static method to allow S.castle to be referenced
	static public GameObject GET_CASTLE()
	{
		return S.castle;
	}
}
