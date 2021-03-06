﻿using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public enum ServingSide
    {
        Even = 0,
        Odd = 1,
    }

    public UIManager uiManager;
    
    private readonly List<Vector2Int> _sets = new List<Vector2Int>();
    private int _currentSetIndex = 0;
    private Vector2Int _currentGame = new Vector2Int(0, 0);
    private bool _gameWon = false;

    private Player _player1;
    private AIPlayer _player2;
    [HideInInspector] public int player1Id;
    [HideInInspector] public int player2Id;
    
    private int _setsNeededToWin = 0;
    private int _player1SetsWon = 0;
    private int _player2SetsWon = 0;

    [HideInInspector] public bool currentlyInTiebreak;
    [HideInInspector] public ServingSide currentServingSide = ServingSide.Even;
    private int _servingPlayerId;

    private GameManager _gameManager;
    private SoundManager _soundManager;

    private void Awake()
    {
        _gameManager = GetComponent<GameManager>();
        _player1 = _gameManager.player;
        _player2 = _gameManager.aiPlayer;
        
        player1Id = _player1.playerId;
        player2Id = _player2.playerId;
        
        _servingPlayerId = player1Id;

        var totalSets = PlayerPrefs.GetInt("SetNumber", 1);
        _setsNeededToWin = totalSets / 2 + 1;
        for (var i = 0; i < totalSets; i++)
        {
            _sets.Add(new Vector2Int(0, 0));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _soundManager = _gameManager.soundManager;
    }

    public void WinPoint(int playerId)
    {
        TriggerPlayerReactionAnimations(playerId);
        
        if (currentlyInTiebreak)
        {
            WinTiebreakPoint(playerId);
            uiManager.SetPlayerGameScore(_sets, _currentGame, _currentSetIndex + 1, _servingPlayerId == player1Id,
                MatchFinished());
            return;
        }
        if (player1Id == playerId)
        {
            if (_currentGame[0] < 40)
            {
                _currentGame = new Vector2Int(IncreaseGameScore(_currentGame[0]), _currentGame[1]);
            }
            else switch (_currentGame[0])
            {
                case 40 when _currentGame[1] < 40:
                    _gameWon = true;
                    break;
                case 40 when _currentGame[1] == 40:
                    // 40 40 to AD 40
                    _currentGame = new Vector2Int(45, 40);
                    break;
                case 40 when _currentGame[1] == 45:
                    // 40 ad to 40 40
                    _currentGame = new Vector2Int(40, 40);
                    break;
                case 45:
                    // AD 40 to game
                    _gameWon = true;
                    break;
            }
        }
        else if(player2Id == playerId)
        {
            if (_currentGame[1] < 40)
            {
                _currentGame = new Vector2Int(_currentGame[0], IncreaseGameScore(_currentGame[1]));
            }
            else switch (_currentGame[1])
            {
                case 40 when _currentGame[0] < 40:
                    _gameWon = true;
                    break;
                case 40 when _currentGame[0] == 40:
                    // 40 40 to 40 AD
                    _currentGame = new Vector2Int(40, 45);
                    break;
                case 40 when _currentGame[0] == 45:
                    // AD 40 to 40 40
                    _currentGame = new Vector2Int(40, 40);
                    break;
                case 45:
                    _gameWon = true;
                    break;
            }
        }

        if (_gameWon)
        {
            WinGame(playerId);
            _soundManager.PlayLoudCrowdSounds();
        }
        else
        {
            uiManager.ShowEventMessage(playerId == player1Id ? UIManager.MessageType.Player1Score : UIManager.MessageType.Player2Score);
            SwapServingSide();
            _soundManager.PlayCrowdSounds();
        }
        
        uiManager.SetPlayerGameScore(_sets, _currentGame, _currentSetIndex + 1, _servingPlayerId == player1Id,
            MatchFinished());
        
    }

    private void WinTiebreakPoint(int playerId)
    {
        if (player1Id == playerId)
        {
            _currentGame = new Vector2Int(_currentGame[0] + 1, _currentGame[1]);
        }
        else if (player2Id == playerId)
        {
            _currentGame = new Vector2Int(_currentGame[0], _currentGame[1] + 1);
        }
        
        if (_currentGame[0] >= 7 && _currentGame[1] < _currentGame[0] - 1)
        {
            _sets[_currentSetIndex] = new Vector2Int(_sets[_currentSetIndex][0] + 1, _sets[_currentSetIndex][1]);
            currentlyInTiebreak = false;
            _currentGame = new Vector2Int(0, 0);
            CheckSetWon(_sets[_currentSetIndex]);
        }
        else if (_currentGame[1] >= 7 && _currentGame[0] < _currentGame[1] - 1)
        {
            _sets[_currentSetIndex] = new Vector2Int(_sets[_currentSetIndex][0], _sets[_currentSetIndex][1] + 1);
            currentlyInTiebreak = false;
            _currentGame = new Vector2Int(0, 0);
            CheckSetWon(_sets[_currentSetIndex]);
        }
        else
        {
            uiManager.ShowEventMessage(playerId == player1Id ? UIManager.MessageType.Player1Score : UIManager.MessageType.Player2Score);
        }
        
        SwapServingSide();
        if ((_currentGame[1] + _currentGame[0]) % 2 == 1)
        {
            SwitchServingPlayer();
        }
    }

    private void WinGame(int playerId)
    {
        var currentSet = _sets[_currentSetIndex];
        if (player1Id == playerId)
        {
            _sets[_currentSetIndex] = new Vector2Int(_sets[_currentSetIndex][0] + 1, _sets[_currentSetIndex][1]);
        }
        else if (player2Id == playerId)
        {
            _sets[_currentSetIndex] = new Vector2Int(_sets[_currentSetIndex][0], _sets[_currentSetIndex][1] + 1);
        }

        if (_sets[_currentSetIndex][0] == 6 && _sets[_currentSetIndex][1] == 6)
        {
            currentlyInTiebreak = true;
        }
        else
        {
            CheckSetWon(_sets[_currentSetIndex]);
        }
        
        _currentGame = new Vector2Int(0, 0);
        currentServingSide = ServingSide.Even;
        _gameWon = false;
        SwitchServingPlayer();
    }

    private int IncreaseGameScore(int currentScore)
    {
        switch (currentScore)
        {
            case 0: 
                return 15;
            case 15: 
                return 30;
            case 30:
                return 40;
            default:
                return 0;
        }
    }

    private void CheckSetWon(Vector2Int set)
    {
        var setWon = false;
        if (set[0] == 6 && set[1] < 5 || set[0] == 7)
        {
            _player1SetsWon += 1;
            _currentSetIndex += 1;
            currentServingSide = ServingSide.Even;
            _gameWon = false;
            setWon = true;

        }
        else if (set[1] == 6 && set[0] < 5 || set[1] == 7)
        {
            _player2SetsWon += 1;
            _currentSetIndex += 1;
            currentServingSide = ServingSide.Even;
            _gameWon = false;
            setWon = true;
        }
        
        if (MatchFinished())
        {
            currentServingSide = ServingSide.Even;
            _gameWon = false;
            var player1Won = _player1SetsWon == _setsNeededToWin;
            uiManager.ShowEventMessage(player1Won ? UIManager.MessageType.Victory : UIManager.MessageType.Defeat);
            //IEnumerator coroutine = MatchFinishedCoroutine(player1Won);
            //StartCoroutine(coroutine);
        }
        else
        {
            uiManager.ShowEventMessage(setWon ? UIManager.MessageType.Set : UIManager.MessageType.Game);
        }
    }

    /*private IEnumerator MatchFinishedCoroutine(bool player1Won)
    {
        uiManager.ShowEventMessage(player1Won ? UIManager.MessageType.Victory : UIManager.MessageType.Defeat);
        yield return new WaitForSeconds(5f);
        _gameManager.GameFinished();
    }*/

    public ServingSide GetServingSide()
    {
        if (currentlyInTiebreak)
        {
            return (_currentGame[0] + _currentGame[1]) % 2 == 0 ? ServingSide.Even : ServingSide.Odd;
        }
        switch (_currentGame[0]) 
            {
                case 0:
                case 30:
                    if (_currentGame[1] == 0 || _currentGame[1] == 30)
                    {
                        return ServingSide.Even;
                    }
                    return ServingSide.Odd;
                case 15:
                case 40:
                    if (_currentGame[1] == 15 || _currentGame[1] == 40)
                    {
                        return ServingSide.Even;
                    }
                    return ServingSide.Odd;
                case 45:
                    return ServingSide.Odd;
            }
        return ServingSide.Even;
    }

    public int GetServingPlayerId()
    {
        return _servingPlayerId;
    }

    public int GetReceivingPlayerId()
    {
        return player1Id == _servingPlayerId ? player2Id : player1Id;
    }

    private void SwitchServingPlayer()
    {
        if (player1Id == _servingPlayerId)
        {
            _servingPlayerId = player2Id;
        }
        else
        {
            _servingPlayerId = player1Id;
        }  
    }

    private void SwapServingSide()
    {
        currentServingSide = (currentServingSide == ServingSide.Even)
            ? ServingSide.Odd : ServingSide.Even;
    }
    
    private void TriggerPlayerReactionAnimations(int playerId)
    {
        if (playerId == player1Id)
        {
            _player1.Cheer();
            _player2.Defeated();
        }
        else
        {
            _player1.Defeated();
            _player2.Cheer();
        }
    }

    public bool MatchFinished()
    {
        return _player1SetsWon == _setsNeededToWin || _player2SetsWon == _setsNeededToWin;
    }
}