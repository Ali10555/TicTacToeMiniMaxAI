using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TicTacToeController : MonoBehaviour {

    public enum AI_DIFFICULTY
    {
        easy,
        hard,
        Invinsible
    }

    public AI_DIFFICULTY difficulty;

    [Tooltip("Player 1 is played by the AI")]

    [HideInInspector]public bool p1Ai = false;

    [Tooltip("Player 2 is played by the AI")]

    public bool p2Ai = true;

    [Range(0,1)]public float HardnessReduction = 0.4f;


    [Tooltip("Using hard coded instructions for the first two moves to improve the speed")]
    [HideInInspector]public bool useShortcuts = true;


    [Tooltip("Duration of each AI algorithm step in seconds")]
    [HideInInspector]
    public float algorithmStepDuration = 1;

    [SerializeField]
    private List<CheckBox> CheckBoxes = new List<CheckBox>();

    public delegate void OnGameOverDelegate(int win);
    public OnGameOverDelegate onGameOverDelegate;

    [ReadOnly] public bool turn; // true: Player, false: AI
    [ReadOnly] public int fieldsLeft;
    [ReadOnly] public bool isGameOver = true;

    // Will hold the current values of the MinMax algorithm
    private int recursionScore;
    private int optimalScoreButtonIndex = -1;

    public GameObject MarkXPrefab;
    public GameObject MarkOPrefab;

    [Header("UI")]
    public GameObject GameplayUI;
    public TextMeshProUGUI gameModeText;

    [Header("GameOver UI")]
    public GameObject GameOverScreen;
    public TextMeshProUGUI winText;
    private void Start()
    {
        
    }
    public void StartGame() {
        turn = Mathf.Round(UnityEngine.Random.Range(0, 1)) == 1;
        Reset();
        GameplayUI.SetActive(true);

        if (p2Ai)
        {
            switch (difficulty)
            {
                case AI_DIFFICULTY.easy:
                    gameModeText.text = "AI (Easy)";
                    break;
                case AI_DIFFICULTY.hard:
                    gameModeText.text = "AI (Hard)";
                    break;
                case AI_DIFFICULTY.Invinsible:
                    gameModeText.text = "AI (Invinsible)";
                    break;
            }
        }
        else
        {
            gameModeText.text = "Player VS Player";
        }
    }

    private void EnableButtons(bool enabled, bool ignoreEmpty = false) {
        
        foreach (CheckBox button in CheckBoxes) {
            // Do not reanable buttons that already were used
            if (!enabled || ignoreEmpty || button.Text == "") {
                button.collider.enabled = enabled;
            }
        }
    }

    private bool SetMarkAndCheckForWin(CheckBox button, bool colorate = false) {

        if (button.Text != "") {
            return false;
        }
        button.Text = turn ? "X" : "O";
        fieldsLeft--;

        return CheckForWin(button.Text, colorate);
    }

    public void OnButtonClick(CheckBox button) {
        if (isGameOver) {
            //Reset();
            return;
        }
        if (fieldsLeft <= 0) {
            return;
        }

        if (SetMarkAndCheckForWin(button, true)) {
            Win(); // Display the game results
        }
        button.collider.enabled = false;

        if(turn)
        {
            Instantiate(MarkXPrefab, button.transform.position, Quaternion.identity, button.transform);
        }
        else
        {
            Instantiate(MarkOPrefab, button.transform.position, Quaternion.identity, button.transform);
        }
        // Game Over - Draw
        if (fieldsLeft <= 0) {
            GameOverDraw();
        }


        // Switch turns
        if (!isGameOver)
            turn = !turn;

        // Let the AI play
        if (!isGameOver && fieldsLeft > 0 && IsAiTurn()) {
            StartCoroutine(AiTurnCoroutine());
        }
    }

    public bool IsAiTurn() {
        return (turn && p1Ai) || (!turn && p2Ai);
    }

    private IEnumerator AiTurnCoroutine()
    {

        EnableButtons(false);

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.2f));

        if (difficulty == AI_DIFFICULTY.easy)
        {
            CheckBox random = RandomSelector();

            OnButtonClick(random);
        }
        else
        {

            if (UnityEngine.Random.Range(0, 1.1f) > (1- HardnessReduction) && difficulty == AI_DIFFICULTY.hard)
            {

                CheckBox random = RandomSelector();

                OnButtonClick(random);
            }
            else
            {
                // Call the MinMax algorithm. It will store the (for the player) worst move in optimalScoreButtonIndex.
                // What is worst for the player, is the best for the AI.
                IEnumerator minMaxEnumerator = MinMaxCoroutine(1);

                // Force the coroutine to do everything in one frame
                while (minMaxEnumerator.MoveNext()) { }
                //HideDepthAndScoreForAllButtons();

                // Debug.Log("buttonIndex: " + optimalScoreButtonIndex);
                CheckBox button = CheckBoxes[optimalScoreButtonIndex]; // Could be random by using: (int)Mathf.Round(Random.Range(0, 9));

                OnButtonClick(button);
            }
        }


        EnableButtons(true);



    }

    CheckBox RandomSelector()
    {
        List<int> randoms = new List<int>();
        for (int i = 0; i < CheckBoxes.Count; i++)
        {
            if (CheckBoxes[i].Text == "")
                randoms.Add(i);
        }

        int index = randoms[UnityEngine.Random.Range(0, randoms.Count)];
        return CheckBoxes[index];
    }

    /// <summary>
    /// Min Max algorithm to find the best and worse moves.
    /// This Method stores the current best and worst moves in
    /// highestCurrentScoreIndex and lowestCurrentScoreIndex as a side effect.
    /// </summary>
    /// <param name="depth">Depth - the number of recursion step for weighting the scores</param>
    /// <returns>The sum of scores of all possible steps from the current recursion level downwards (stored in recursionScore)</returns>
    private IEnumerator MinMaxCoroutine(int depth) {
        // Base case and shortcuts (hard coded moves) to stop recursion
        if (CheckBaseCaseAndShortcuts()) {
            yield break;
        }

        // We want to store which field gives us the best (player) or the worst (CPU) score
        int currentBestScore = turn ? Int32.MinValue : Int32.MaxValue;
        int currentOptimalScoreButtonIndex = -1;

        // Find next free field
        int fieldIndex = 0;
        while (fieldIndex < CheckBoxes.Count) {
            if (IsFieldFree(fieldIndex)) {
                CheckBox button = CheckBoxes[fieldIndex];
                int currentScore = 0;

                bool endRecursion = false;

                SetDepth(button, depth);

                // End iteration and recursion level when we win, because we don't need to go deeper
                if (SetMarkAndCheckForWin(button)) {
                    // Debug.Log("Found a winner: " + GetText(button).text);
                    currentScore = (turn ? 1 : -1) * (10 - depth);
                    endRecursion = true;
                } else if (fieldsLeft > 0) {
                    // If there are fields left after the SetMarkAndCheckForWin we can go deeper in the recursion
                    turn = !turn; // Switch turns - in the next step we want to simulate the other player

                    IEnumerator minMaxEnumerator = MinMaxCoroutine(depth + 1);

                        // Force the coroutine to do everything in one frame
                        while (minMaxEnumerator.MoveNext()) { }
                    
                    currentScore = recursionScore;
                    turn = !turn; // Switch turns back
                }

                if ((turn && currentScore > currentBestScore) || (!turn && currentScore < currentBestScore)) {
                    currentBestScore = currentScore;
                    currentOptimalScoreButtonIndex = fieldIndex;
                }




                // Undo this step and go to the next field
                button.Text = "";

                fieldsLeft++;

                if (endRecursion) {
                    // No need to check further fields if there already is a win
                    break;
                }
            }
            fieldIndex++;
            // Stop if we checked all buttons
        }

        recursionScore = currentBestScore;
        optimalScoreButtonIndex = currentOptimalScoreButtonIndex;
        // Debug.Log("score: " + recursionScore);
    }

    private void SetScore(CheckBox button, int score) {
        button.Points = score + " Points";
    }

    private void SetDepth(CheckBox button, int depth) {
        button.Depth = "Depth: " + depth;
    }


    /*
    private void HideDepthAndScore(Button button) {
        button.transform.GetChild(1).gameObject.SetActive(false);
        button.transform.GetChild(2).gameObject.SetActive(false);
        GetText(button).color = Color.white;
    }

    private void HideDepthAndScoreForAllButtons() {
        foreach (CheckBox button in CheckBoxes) {
            HideDepthAndScore(button);
        }
    }

    */

    // This will return true if we can stop the recursion immediately and handle shortcuts (hard coded moves for faster progress)
    private bool CheckBaseCaseAndShortcuts() {
        if (fieldsLeft <= 0) {
            recursionScore = 0;
            return true;
        }

        if (!useShortcuts) {
            return false;
        }

        // No need to calculate anything if all fields are free - any corner is the best.
        // But let's use that chance for some variety and use random. Index will be 0, 2, 6 or 8
        if (fieldsLeft == 9) {
            RandomCorner();
            return true;
        }
        // Shortcut for the optimal second move after an opening
        if (fieldsLeft == 8) {
            // If the other player used the middle. go for any corner
            if (!CheckBoxes[4].Text.Equals("")) {
                RandomCorner();
            } else { // Else the middle is always the best
                optimalScoreButtonIndex = 4;
            }
            return true;
        }
        return false;
    }

    // Returns true if the given mark if present with three in a row
    private bool CheckForWin(string mark, bool colorate = false) {
        if (fieldsLeft > 6) {
            return false;
        }
        // Horizontal
        if (CompareButtons(0, 1, 2, mark, colorate)
         || CompareButtons(3, 4, 5, mark, colorate)
         || CompareButtons(6, 7, 8, mark, colorate)
        // Vertical
         || CompareButtons(0, 3, 6, mark, colorate)
         || CompareButtons(1, 4, 7, mark, colorate)
         || CompareButtons(2, 5, 8, mark, colorate)
        // Diagonal
         || CompareButtons(0, 4, 8, mark, colorate)
         || CompareButtons(6, 4, 2, mark, colorate)) {
            return true;
        }
        return false;
    }

    private bool CompareButtons(int ind1, int ind2, int ind3, string mark, bool colorate = false) {
        string text1 = CheckBoxes[ind1].Text;
        string text2 = CheckBoxes[ind2].Text;
        string text3 = CheckBoxes[ind3].Text;
        bool equal = text1 == mark
                  && text2 == mark
                  && text3 == mark;
        /*
        if (colorate && equal) {
            Color color = turn ? Color.green : Color.red;
            text1.color = color;
            text2.color = color;
            text3.color = color;
        }
        */
        return equal;
    }

    // Checks if a field is still free
    private bool IsFieldFree(int index) => CheckBoxes[index].Text.Length == 0;

    // Displays the game results
    private void Win() {
        Debug.Log(turn ? "Player 1 won!" : "Game Over!");
        isGameOver = true;
        EnableButtons(false);
        onGameOverDelegate?.Invoke(turn ? 0 : 1);
        StartCoroutine(GameOverUI());
    }

    IEnumerator GameOverUI()
    {
        GameplayUI.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        GameOverScreen.SetActive(true);
        if (turn)
        {
            winText.text = "Player 1 Won";
            winText.color = Color.blue;
        }
        else
        {
            if (IsAiTurn())
            {
                winText.text = "AI Won";
                winText.color = Color.red;
            }
            else
            {
                winText.text = "Player 2 Won";
                winText.color = Color.blue;
            }
        }
    }

    private void GameOverDraw() {
        if (isGameOver)
            return;

        GameOverScreen.SetActive(true);
        winText.text = "Draw";
        winText.color = Color.yellow;
        // Debug.Log("Game Over - Draw");
        isGameOver = true;
        EnableButtons(false);
        onGameOverDelegate?.Invoke(-1);
    }

    // Use some variety and use random to determine an optimal start field. Index will be 0, 2, 6 or 8
    private int RandomCorner() {
        optimalScoreButtonIndex = (int)Mathf.Floor(UnityEngine.Random.Range(0, 4));
        if (optimalScoreButtonIndex == 1) {
            optimalScoreButtonIndex = 6;
        } else if (optimalScoreButtonIndex == 3) {
            optimalScoreButtonIndex = 8;
        }
        return optimalScoreButtonIndex;
    }


    // Right click on the script in the inspector to use these methods
    
    [ContextMenu("Reset")]
    private void Reset() {
        foreach (CheckBox button in CheckBoxes) {
            button.Text = "";
            button.collider.enabled = true;
        }
        fieldsLeft = 9;
        isGameOver = false;
        if (IsAiTurn()) {
            StartCoroutine(AiTurnCoroutine());
        }
    }
    /*
    // Right click on the script in the inspector to use this
    [ContextMenu("Set Depth Test Example")]
    private void SetDepthTestExample() {
        turn = true;
        Reset();
        GetText(CheckBoxes[1]).text = "X"; CheckBoxes[1].interactable = false;
        GetText(CheckBoxes[5]).text = "X"; CheckBoxes[5].interactable = false;
        GetText(CheckBoxes[6]).text = "O"; CheckBoxes[6].interactable = false;
        GetText(CheckBoxes[7]).text = "O"; CheckBoxes[7].interactable = false;
        GetText(CheckBoxes[8]).text = "X"; CheckBoxes[8].interactable = false;

        turn = false;
        fieldsLeft = 4;
        StartCoroutine(AiTurnCoroutine());
    }
    */
}
