using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class Gameplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI attempsLeft;
    public TextMeshProUGUI currentPlayer;
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI gameLog;

    [Header("Input")]
    public TMP_InputField guessInputField;
    public Button submitButton;
    public Button newgameButton;

    [Header("Game Settings")]
    public int minNumber = 1;
    public int maxNumber = 100;
    public int maxAttemps = 12;

    private int targetNumber;
    private int currentAttemps;
    private bool isPlayerTurn;
    private bool gameActive;

    private int computerMinGuess;
    private int computerMaxGuess;
    private List<int> computerGuesses;

    void InitializeUI()
    {
        submitButton.onClick.AddListener(SubmitGuess);
        newgameButton.onClick.AddListener(StartNewGame);
        guessInputField.onSubmit.AddListener(delegate { SubmitGuess(); });
    }

    void SubmitGuess()
    {
        if (!gameActive || !isPlayerTurn) return;

        string input = guessInputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        int guess;
        if (!int.TryParse(input, out guess))
        {
            gameState.text = "<sprite=15> Please enter a valid number";
            return;
        }
        if (guess < minNumber || guess > maxNumber)
        {
            gameState.text = $"<sprite=15> Please enter a number between {minNumber} - {maxNumber}";
            return;
        }
        ProcessGuess(guess, true);
        guessInputField.text = "";
    }

    void ProcessGuess(int guess, bool isPlayerTurn)
    {
        currentAttemps++;
        string playerName = isPlayerTurn ? "Player" : "Computer";

        gameLog.text += $"{playerName} guessd: {guess}\n";

        if (guess == targetNumber)
        {
            //Win
            gameLog.text += $"<sprite=\"Symbols\" index=23> {playerName} got it right!\n";
            EndGame();
        }
        else if (currentAttemps >= maxAttemps)
        {
            //Lose
            gameLog.text += $"<sprite=10> Game Over! The correct number was {targetNumber}\n";
            EndGame();
        }
        else
        {
            //Wrong guess - give hint
            string hint = guess < targetNumber ? "Too Low" : "Too High";
            gameLog.text += $"<sprite=\"Symbols\" index=24> {hint}\n";

            //Switch players
            isPlayerTurn = !isPlayerTurn;
            currentPlayer.text = isPlayerTurn ? "Player" : "Computer";
            attempsLeft.text = $"Attemps Left: {maxAttemps - currentAttemps}";

            if (!isPlayerTurn)
            {
                guessInputField.interactable = false;
                submitButton.interactable = false;
                StartCoroutine(ComputerTurn(guess < targetNumber));
                //StartCoroutine(ComputerTurn(higher));
            }
            else
            {
                guessInputField.interactable = true;
                submitButton.interactable = true;
                guessInputField.Select();
                guessInputField.ActivateInputField();
            }
        }
    }

    IEnumerator ComputerTurn(bool targetIsHigher)
    {
        yield return new WaitForSeconds(2f); // Wait to simulate thinking
        if (!gameActive) yield break;
        if (computerGuesses.Count > 0)
        {
            int lastGuess = computerGuesses[computerGuesses.Count - 1];
            if (targetIsHigher)
            {
                // Too Low
                computerMinGuess = lastGuess + 1;
                //computerMaxGuess = lastGuess - 1;
            }
            else
            {
                // Too High
                computerMaxGuess = lastGuess - 1;
                //computerMinGuess = lastGuess + 1;
            }
        }

        // AI uses Binary Search strategy
        Debug.Log($"{targetIsHigher} {computerMinGuess} {computerMaxGuess}");
        int computerGuess = (computerMinGuess + computerMaxGuess) / 2;

        computerGuesses.Add(computerGuess);

        ProcessGuess(computerGuess, false);
    }

    void EndGame()
    {
        attempsLeft.text = $"Attemps Left: {maxAttemps - currentAttemps}";
        gameActive = false;
        guessInputField.interactable = false;
        submitButton.interactable = false;
        currentPlayer.text = "";
        gameState.text += "Game Over - Click New Game to start again";
        Canvas.ForceUpdateCanvases();
    }

    void StartNewGame()
    {
        targetNumber = Random.Range(minNumber, maxNumber + 1);
        currentAttemps = 0;
        isPlayerTurn = true;
        gameActive = true;

        currentPlayer.text = "Player Turn";
        attempsLeft.text = $"Attemps Left: {maxAttemps}";
        gameLog.text = "=== Game Log ===\n";
        gameState.text = "Game In Progress";

        guessInputField.interactable = true;
        submitButton.interactable = true;
        guessInputField.text = "";
        guessInputField.Select();
        guessInputField.ActivateInputField();

        computerMinGuess = minNumber;
        computerMaxGuess = maxNumber;
        computerGuesses = new List<int>();
    }

    void Start()
    {
        InitializeUI();
        StartNewGame();
    }

    void Update()
    {

    }
}