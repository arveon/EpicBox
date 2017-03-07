using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FightManager : MonoBehaviour
{
	//elements that display enemy and player health and actions performed on the scene
	//set in the Unreal editor
	public Text Player;
	public Text Enemy;
	public Text InfoScreen;

	public int START_HEALTH = 100;
	
	//types of actions player and enemy can perform
	enum ActionTypes { None, AttackHigh, AttackLow, BlockHigh, BlockLow };

	//damage caused by low and high attacks
	public int LOW_DAMAGE = 10;
	public int HIGH_DAMAGE = 15;

	//player and enemy health
	int playerHealth;
	int enemyHealth;

	//required to avoid keys being held down
	bool keyDown;

	//required to display proper info in info field
	int dmgByEnemy;
	int dmgByPlayer;

	//required to detect and change scene when game is over
	bool gameOver;
	float gameOverElapsedTime;
	int gameOverTotalTime = 2;

	// Use this for initialization
	void Start()
	{
		gameOver = false;
		playerHealth = START_HEALTH;
		enemyHealth = START_HEALTH;
		keyDown = false;
		gameOverElapsedTime = 0;

		UpdateHealth();//need to run because values in the editor are 100 and need to be updated as soon as scene starts
	}

	// Update is called once per frame
	void Update()
	{
		//will only do player actions when game isn't over
		if (!gameOver)
		{
			//need to reset to avoid using last values
			dmgByEnemy = 0;
			dmgByPlayer = 0;
			//actions performed this update
			ActionTypes playerAction = ActionTypes.None, enemyAction = ActionTypes.None;

			//if one of the keys is pressed, but none of them were pressed in previous update
			//set playerAction to an appropriate action
			if (Input.GetKeyDown(KeyCode.Q) && !keyDown)
			{
				playerAction = ActionTypes.AttackHigh;
				keyDown = true;
			}
			else if (Input.GetKeyDown(KeyCode.A) && !keyDown)
			{
				playerAction = ActionTypes.AttackLow;
				keyDown = true;
			}
			else if (Input.GetKeyDown(KeyCode.E) && !keyDown)
			{
				playerAction = ActionTypes.BlockHigh;
				keyDown = true;
			}
			else if (Input.GetKeyDown(KeyCode.D) && !keyDown)
			{
				playerAction = ActionTypes.BlockLow;
				keyDown = true;
			}
			else//if none of the required keys were pressed, go back
				keyDown = false;

			//if player did any action this update
			//process it
			if (playerAction != ActionTypes.None)
			{
				enemyAction = GenerateEnemyAction();//generate a random action enemy would perform
				ResolveActions(playerAction, enemyAction);//resolve actions
				CheckVictory();
				UpdateHealth();
				UpdateInfoScreen(playerAction, enemyAction);
			}
		}
		else//when game is over, wait for 2 seconds and switch to appropriate scene
		{
			gameOverElapsedTime += Time.deltaTime;//keep track of passed time
			if (gameOverElapsedTime >= (int)gameOverTotalTime)
			{
				if (enemyHealth > 0)//if enemy is dead then game won
					SceneManager.LoadScene("Lost");
				else
					SceneManager.LoadScene("Won");
			}
		}
	}

	/// <summary>
	/// Method will resolve performed actions and update the health variables
	/// </summary>
	/// <param name="playerAction">action performed by the player</param>
	/// <param name="enemyAction">action performed by the enemy</param>
	void ResolveActions(ActionTypes playerAction, ActionTypes enemyAction)
	{
		//check for player attacks
		switch (playerAction)
		{
			case ActionTypes.AttackHigh:
				if (enemyAction != ActionTypes.BlockHigh)
				{//if enemy doesn't block the same level, deal damage
					dmgByPlayer = HIGH_DAMAGE;
					enemyHealth -= HIGH_DAMAGE;
				}
				break;
			case ActionTypes.AttackLow:
				if (enemyAction != ActionTypes.BlockLow)
				{
					dmgByPlayer = LOW_DAMAGE;
					enemyHealth -= LOW_DAMAGE;
				}
				break;
		}
		switch(enemyAction)
		{
			case ActionTypes.AttackHigh:
				if (playerAction != ActionTypes.BlockHigh)
				{//if player doesn't block the same level, deal damage
					dmgByEnemy = (int)(HIGH_DAMAGE * 1.1f);//damage the enemy deals is slightly higher than player's
					playerHealth -= dmgByEnemy;
				}
				break;
			case ActionTypes.AttackLow:
				if (playerAction != ActionTypes.BlockLow)
				{
					dmgByEnemy = (int)(LOW_DAMAGE * 1.1f);
					playerHealth -= dmgByEnemy;
				}
				break;
		}
	}

	/// <summary>
	/// Method will randomly generate one of four possible actions
	/// </summary>
	/// <returns></returns>
	ActionTypes GenerateEnemyAction()
	{
		return (ActionTypes)Random.Range(1, 5);
	}

	/// <summary>
	/// Method will update UI text strings to player and enemies health
	/// </summary>
	void UpdateHealth()
	{
		Player.text = playerHealth.ToString();
		Enemy.text = enemyHealth.ToString();
	}

	/// <summary>
	/// Method will update the action information on menu screen
	/// </summary>
	/// <param name="playerAction">action performed by the player</param>
	/// <param name="enemyAction">action performed by the enemy</param>
	void UpdateInfoScreen(ActionTypes playerAction, ActionTypes enemyAction)
	{
		string enm = "", plr = "";
		//add the action to the string
		switch(playerAction)
		{
			case ActionTypes.AttackHigh:
				plr += "You attacked high: ";
				break;
			case ActionTypes.AttackLow:
				plr += "You attacked low: ";
				break;
			case ActionTypes.BlockHigh:
				plr += "You blocked high: ";
				break;
			case ActionTypes.BlockLow:
				plr += "You blocked low: ";
				break;
		}
		plr += dmgByPlayer;//add damage dealt by player to action info

		switch (enemyAction)
		{
			case ActionTypes.AttackHigh:
				enm += "Enemy attacked high: ";
				break;
			case ActionTypes.AttackLow:
				enm += "Enemy attacked low: ";
				break;
			case ActionTypes.BlockHigh:
				enm += "Enemy blocked high: ";
				break;
			case ActionTypes.BlockLow:
				enm += "Enemy blocked low: ";
				break;
		}
		enm += dmgByEnemy;//add damage dealt by the enemy to action info

		InfoScreen.text = plr + "\n" + enm;//update the field
	}

	/// <summary>
	/// 
	/// </summary>
	void CheckVictory()
	{
		if (enemyHealth < 0) { enemyHealth = 0; gameOver = true; }
		if (playerHealth < 0) { playerHealth = 0; gameOver = true; }
	}
}
