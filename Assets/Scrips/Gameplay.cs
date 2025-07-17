using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using System.Diagnostics;
using System.Collections;
public class Gameplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI  attempsLeft;
    public TextMeshProUGUI currentPlayer;
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI gameLog;

    [Header("Input")]
    public TMP_InputField guessInputField;
    public Button summitButton;
    public Button newgameButton;


    [Header("Game Settings")]
    public int minNumber = 1;
    public int maxNumber = 100;
    public int maxAttempts = 12;

    private int currentAttempts;
    private int targetNumber;
    private bool gameActive;
    private bool isPlayerTurn;

    void InitializeUI()
    { 
        summitButton.onClick.AddListener(SubmitGuess);
        newgameButton.onClick.AddListener(StartNewGame);
        guessInputField.onSubmit.AddListener(delegate { SubmitGuess(); });
    }

    void SubmitGuess()
    {
        if (!gameActive || !isPlayerTurn) return;
        string input = guessInputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        int guess;
        if (! int.TryParse(input, out guess))
        {
            gameState.text = "Please enter a valid number.";
            return;
        }
        if (guess < minNumber || guess > maxNumber)
        {
            gameState.text = $"Please enter a number between {minNumber} - {maxNumber}.";
            return;
        }
        ProcessGuess(guess, true);
        guessInputField.text = "";
    }
    void ProcessGuess(int guess, bool isPlayerTurn)
    {
        currentAttempts++;
        string playerName = isPlayerTurn ? "Player" : "Computer";

        gameLog.text += $"{playerName} guessed: {guess}\n";

        if (guess == targetNumber)
        {
            //Win
            gameLog.text += $"{playerName} got it right!\n";
            EndGame();
        }
        else if (currentAttempts >= maxAttempts)
        { 
            gameLog.text += $"Game Over! The correct number was{targetNumber}\n";
            EndGame();
        }
        else
        {
            //Wrong guess = give hint
            string hint = guess < targetNumber ? "Too Low" : "Too High";
            gameLog.text += $"{hint}!\n";

            //Switch players
            isPlayerTurn = !isPlayerTurn;
            currentPlayer.text = isPlayerTurn ? "Player" : "Computer";
            attempsLeft.text = $"Attempts Left: {maxAttempts - currentAttempts}";

            if (!isPlayerTurn)
            {
                guessInputField.interactable = false;
                summitButton.interactable = false;
                StartCoroutine(ComputerTurn(guess < targetNumber));
            }
            else
            {
                guessInputField.interactable = true;
                summitButton.interactable = true;
                guessInputField.Select();
                guessInputField.ActivateInputField();
            }
        }
    }


    IEnumerator ComputerTurn(bool targetISHigher)
    {
        yield return new WaitForSeconds(2f); // Simulate thinking time
        if (!gameActive) yield break;
        int computerGuess = Random.Range(minNumber, maxNumber + 1);
        ProcessGuess(computerGuess, false);
    }

    void EndGame()
    {
        gameActive = false;
        guessInputField.interactable = false;
        summitButton.interactable = false;
        currentPlayer.text = "";
        gameState.text = "Game Over Click New Game to start again.";
        Canvas.ForceUpdateCanvases(); 
    }

    void StartNewGame()
    {
        targetNumber = Random.Range(minNumber, maxNumber + 1);
        currentAttempts = 0;
        isPlayerTurn = true;
        gameActive = true;

        currentPlayer.text = "Player Turn";
        attempsLeft.text = $"Attempts Left:  { maxAttempts}";
        gameLog.text = "=== Game Log ===\n";
        gameState.text = "New game started! Player goes frist";

        guessInputField.interactable = true;    
        summitButton.interactable = true;
        guessInputField.text = "";
        guessInputField.Select();
        guessInputField.ActivateInputField();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeUI();
        StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
