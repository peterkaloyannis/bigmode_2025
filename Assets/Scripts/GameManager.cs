/*
 * GameManager is a Singleton class, meaning there is guaranteed to only ever exist a single
 * instance of it. It is globally accessible and thus can be used to persist state across
 * scenes.
 */

using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    int[] achievementsUnlocked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // for now set this manually.
            // TODO: update this based on endings achieved
            achievementsUnlocked = new int[] { 0, 1 };

            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }

    private void Start()
    {
        Debug.Log("GameManager Start() called");
    }

    private void AchievementsCheck()
    {
        if (!PlayerPrefs.HasKey("Achievements")){
            PlayerPrefs.SetInt("Achievements", 0);
        }
    }

    // Given a list of integers (treated as a set), check if that
    // list is a subset of the list of unlocked achievements.
    public bool branchIsValid(int[] branchAchievements)
    {
        return branchAchievements.All(item => achievementsUnlocked.Contains(item));
    }
}
