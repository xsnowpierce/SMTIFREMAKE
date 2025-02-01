using Intro;
using UnityEngine;

public class NovaDialogueController : MonoBehaviour
{
    private NovaDialogueUI _novaDialogueUI;
    [SerializeField] private TextAsset novaBegin;
    [SerializeField] private TextAsset novaContent;
    [SerializeField] private TextAsset novaExtra;
    [SerializeField] private TextAsset novaEnd;
    private int currentPoints;

    private void Awake()
    {
        _novaDialogueUI = FindObjectOfType<NovaDialogueUI>();
    }
    private void Start()
    {
        
    }

    void OnInteract()
    {
        
    }

    private void NovaEnding()
    {
        /*
        if (currentPoints >= 0 && currentPoints <= 6)
        {
            _playerStats.playerStats.strength = 7;
            _playerStats.playerStats.intelligence = 2;
            _playerStats.playerStats.magic = 2;
            _playerStats.playerStats.stamina = 7;
            _playerStats.playerStats.speed = 4;
            _playerStats.playerStats.luck = 2;
            _playerStats.playerStats.playerType = PlayerStats.PlayerType.Power;
        }
        else if (currentPoints >= 7 && currentPoints <= 14)
        {
            _playerStats.playerStats.strength = 5;
            _playerStats.playerStats.intelligence = 2;
            _playerStats.playerStats.magic = 2;
            _playerStats.playerStats.stamina = 5;
            _playerStats.playerStats.speed = 7;
            _playerStats.playerStats.luck = 3;
            _playerStats.playerStats.playerType = PlayerStats.PlayerType.Speed;
        }
        else if (currentPoints >= 15 && currentPoints <= 26)
        {
            _playerStats.playerStats.strength = 4;
            _playerStats.playerStats.intelligence = 4;
            _playerStats.playerStats.magic = 4;
            _playerStats.playerStats.stamina = 4;
            _playerStats.playerStats.speed = 4;
            _playerStats.playerStats.luck = 4;
            _playerStats.playerStats.playerType = PlayerStats.PlayerType.Balance;
        }
        else if (currentPoints >= 27)
        {
            _playerStats.playerStats.strength = 4;
            _playerStats.playerStats.intelligence = 3;
            _playerStats.playerStats.magic = 3;
            _playerStats.playerStats.stamina = 4;
            _playerStats.playerStats.speed = 6;
            _playerStats.playerStats.luck = 8;
            _playerStats.playerStats.playerType = PlayerStats.PlayerType.Lucky;
            _playerStats.currentMacca += 5000;
        }

        _playerStats.playerStats.playerName = "GamePlayer";
        */
    }
}