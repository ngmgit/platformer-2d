using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public Text scoreHolder;
	public Text HealthCount;
	public Image HealthUI;
    public GameObject OptionsMenu;

	private int MAX_HEALTHCOUNT = 3;
	private int livesRemain;

	int score = 0;
	// Use this for initialization
	void Awake () {
		GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickups");
		foreach(GameObject pickup in pickups) {
			pickup.AddComponent<Pickup> ();
		}
		livesRemain = MAX_HEALTHCOUNT;
		SetHealthUI ();
	}

	public void SetScore (int value) {
		score = score + value;
		scoreHolder.text = "Score: " + score.ToString();
	}

	public void SetPlayerHealth (float healthRatio) {
		HealthUI.rectTransform.localScale = new Vector3 (healthRatio, 1, 0);
		if (healthRatio <= 0) {
			PlayerDied();
			HealthUI.rectTransform.localScale = new Vector3 (1, 1, 0);
		}
	}

	void PlayerDied () {
		livesRemain -= 1;
		SetHealthUI ();
		if (livesRemain <= 0) {
			SceneManager.LoadScene("SampleScene");
		}
	}

	void SetHealthUI () {
		HealthCount.text = "X " + livesRemain.ToString ();
	}

    public void ToggleOptionsMenu ()
    {
        OptionsMenu.SetActive(!OptionsMenu.activeSelf);
        if (OptionsMenu.activeSelf)
        {
            Time.timeScale = 0;
        } else
        {
            SetTimeScale();
        }
    }

    public void Restartlevel ()
    {
        SetTimeScale();
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadMainMenu ()
    {
        SetTimeScale();
        SceneManager.LoadScene("Menu");
    }

    void SetTimeScale ()
    {
        Time.timeScale = 1;
    }
}
