using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LetterGenerator : MonoBehaviour {

    public enum Wall { Left, Right, Back, Front};

    public GameObject letterPrefab;
    public GameObject doorGO;
    public GameObject leftWallGO;
    public GameObject rightWallGO;
    public GameObject backWallGO;
    public GameObject frontWallGO;
    public GameObject ceilGO;
    public GameObject groundGO;

    public float leftMargin = 0.833f;
    public float topMargin = 0.76f;

    public float lineSpace = 1.273f;
    public float letterSpace = 1.273f;
    public float letterSpaceCircle = 1.25f;

    public float charWallColumns = 6;
    public float charWallRows = 6;

    public int letterCount = 0;

    public int task1Repetitions = 5;
    public int task2Repetitions = 10;
    public float task2RepetitionPercentage = 50;

    private List<GameObject> letters = new List<GameObject>();

    private GameObject letterStartGO;
    private TextMesh letterDoor;

    private char[] letters_set1 = {'A', 'K', 'M', 'N', 'V', 'W', 'X', 'Y', 'Z' };
    private char[] letters_set2 = {'E', 'F', 'H', 'I', 'L', 'T'};

    private bool isLettersCreated = false;
    private bool isLetterShownAtDoor = false;
    private bool isCamouflaged = true;
    private bool isTaskMode = false;

    private int currentTask = 0;
    private int taskCount = 0;

    private char[] selectedSet;

    private char letter;

    private bool[] appear;
    private bool isAppearing;

    // Use this for initialization
    void Start () {

        selectedSet = letters_set1;
        isAppearing = true;
        // Fill the values for appear the experiment 2 
        fillAppearValues(task2Repetitions, task2RepetitionPercentage);

        for (int j = 0; j < appear.Length; j++)
            Debug.Log(appear[j]);

    }

    // Update is called once per frame
    void Update () {

        // If T key pressed, activate or desactivate Task Mode!
        // Task Mode is where the experiment starts 
        if (Input.GetKeyDown(KeyCode.T))
        {
            removeLetterAtDoor();
            removeAllLetters();
            isTaskMode = !isTaskMode;

            if (!isTaskMode)
            {
                currentTask = 0;
                isAppearing = true;
                isTaskMode = false;
                Debug.Log("Task Disabled");
            } else
            {
                Debug.Log("Task Enabled");
            }
        }
        
        // If Key 1 pressed and Task Mode on then start Pilot Experiment
        if(Input.GetKeyDown(KeyCode.Alpha1) && isTaskMode)
        {
            removeLetterAtDoor();
            removeAllLetters();

            currentTask = 1;
            taskCount = task1Repetitions;
            isCamouflaged = false;

            Debug.Log("Task 1 Selected: Not Camouflaged Letters");
        }

        // If Key 2 pressed and Task Mode On then start Experiment 2 
        if (Input.GetKeyDown(KeyCode.Alpha2) && isTaskMode)
        {
            removeLetterAtDoor();
            removeAllLetters();

            currentTask = 2;
            taskCount = task2Repetitions;
            isCamouflaged = true;

            // Fill the values for appear the experiment 2 
            fillAppearValues(task2Repetitions, task2RepetitionPercentage);

            isAppearing = appear[taskCount - 1];

            Debug.Log("Task 2 Selected: Camouflaged Letters " + task2RepetitionPercentage + "%");
        }

        // If Key S pressed then change the current set of letters
        if(Input.GetKeyDown(KeyCode.S))
        {
            // Switch letters set
            if (selectedSet.Equals(letters_set1))
                selectedSet = letters_set2;
            else
                selectedSet = letters_set1;

            Debug.Log("Selected Set: " + selectedSet.ToString());
        }

        // If Key space is pressed then do the different actions for task a non-task
        if (Input.GetKeyDown("space") && (!isTaskMode || (taskCount != 0 && isTaskMode)))
        {
            // The letter is already created?
            if (isLettersCreated)
            {
                // If it is a task mode
                if (isTaskMode)
                {
                    if (taskCount > 0)
                    {
                        // Then decrease task count
                        Debug.Log("Task Iteration " + taskCount + " Finished");
                        taskCount--;
                    }else
                    {
                        // Task finished!
                        currentTask = 0;
                        isAppearing = true;
                        isTaskMode = false;
                        Debug.Log("Task Finished!!");
                    }
                }

                // Remove letters to start again or stop
                removeAllLetters();
            }
            else
            {
                // Is Letter Shown At door?
                if (!isLetterShownAtDoor)
                {
                    // If not then generate a new letter for the task from the selectedSet
                    letter = generateRandomLetter(selectedSet);

                    // Appear the letter at the door
                    showLetterAtDoor(letter, isCamouflaged);
                }
                else
                {

                    // Remove letter at the door
                    removeLetterAtDoor();

                    if (isTaskMode)
                    {
                        if (currentTask == 2)
                            isAppearing = appear[taskCount - 1];
                        else
                            isAppearing = true;

                        Debug.Log("Task: " + taskCount);
                        Debug.Log("Is Letter Appearing: " + isAppearing);
                    }

                    // Generate all letters at the room
                    generateRandomLetters(letter, isCamouflaged, isAppearing, selectedSet);
                }
            }
        }

        // Camouflaged freely if it is not a task mode
        if (Input.GetKeyDown(KeyCode.C) && !isTaskMode)
        {
            isCamouflaged = !isCamouflaged;
            Debug.Log("Letter Camouflaged:" + isCamouflaged);
        } 
       
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="camouflaged"></param>
    public void showLetterAtDoor(char letter, bool camouflaged)
    {
        // Character on the Door
        letterStartGO = Instantiate(letterPrefab, doorGO.transform.position, doorGO.transform.rotation) as GameObject;
        letterDoor = letterStartGO.GetComponent<TextMesh>();

        if (!camouflaged)
            letterDoor.color = Color.red;

        letterDoor.text = letter.ToString();
        isLetterShownAtDoor = true;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="camouflaged"></param>
    public void generateRandomLetters(char letter, bool camouflaged, bool appear, char[] set)
    {
        int position = generateRandomNumber(0, 171);

        generateRandomLettersAtWall(Wall.Left, letter, position, camouflaged, appear, set, new int[] { 11, 10 });
        generateRandomLettersAtWall(Wall.Right, letter, position, camouflaged, appear, set, new int[] { 28, 29 });
        generateRandomLettersAtWall(Wall.Front, letter, position, camouflaged, appear, set, new int[] { 22, 23, 24, 28, 29, 30 });
        generateRandomLettersAtWall(Wall.Back, letter, position, camouflaged, appear, set, new int[] { -1 });
        generateRandomLettersAtCeil(letter, position, camouflaged, appear, set);
        generateRandomLettersAtGround(letter, position, camouflaged, appear, set);

        isLettersCreated = true;
        Debug.Log(letterCount + " letters Created!");
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="camouflaged"></param>
    private void generateRandomLettersAtWall(Wall wall, char letter, int atPosition, bool camouflaged, bool appear, char[] set, int[] ignore)
    {
        // Get the corner top-left start position for the letters
        Vector3 startPosition = getLetterStartPositionAtWall(wall);

        // Get the rotation for current wall
        Quaternion rotation = getLetterRotationAtWall(wall);

        int letterCountPerWall = 0;
        for (int i = 0; i < charWallColumns; i++)
        {
            for (int j = 0; j < charWallRows; j++)
            {
                // Check if the current character should be ignored because probably there is a window or a door over it
                if (!UnityEditor.ArrayUtility.Contains<int>(ignore, letterCountPerWall + 1))
                {
                    // Calculate the new letter position
                    Vector3 letterPosition = getNewLetterPosition(wall, startPosition, i, j);

                    // Create and Save letter in the list
                    letters.Add(createNewLetter(letterPosition, rotation, letter, atPosition, camouflaged, appear, set));
                }

                // Increment count of letters for this specific wall
                letterCountPerWall++;
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="camouflaged"></param>
    private void generateRandomLettersAtGround(char letter, int atPosition, bool camouflaged, bool appear, char[] set)
    {
        generateRandomLettersInACircle(11, groundGO.transform.position, Vector3.down, letter, atPosition, camouflaged, appear, set, new int[] { -1 });
        generateRandomLettersInACircle(17, groundGO.transform.position, Vector3.down, letter, atPosition, camouflaged, appear, set, new int[] {1, 2, 4, 5, 6, 9, 10, 11, 13, 14, 15});
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="camouflaged"></param>
    private void generateRandomLettersAtCeil(char letter, int atPosition, bool camouflaged, bool appear, char[] set)
    {
        generateRandomLettersInACircle(11, ceilGO.transform.position, Vector3.up, letter, atPosition, camouflaged, appear, set, new int[] { -1 });
        generateRandomLettersInACircle(17, ceilGO.transform.position, Vector3.up, letter, atPosition, camouflaged, appear, set, new int[] { 1, 2, 5, 6, 9, 10, 13, 14});
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="camouflaged"></param>
    private void generateRandomLettersInACircle(int numberOfLetters, Vector3 center, Vector3 upwards, char letter, int atPosition, bool camouflaged, bool appear, char[] set, int[] ignore)
    {
        float circunference = letterSpaceCircle * numberOfLetters;
        float radius = circunference / (2 * Mathf.PI);
        float angleSpace = 2 * Mathf.PI * letterSpaceCircle / circunference;

        for (int i = 0; i < numberOfLetters; i++)
        {
            if (!UnityEditor.ArrayUtility.Contains<int>(ignore, i + 1))
            {
                float x = radius * Mathf.Cos(angleSpace * i);
                float y = radius * Mathf.Sin(angleSpace * i);

                Vector3 position = new Vector3(x, 0.0f, y);
                Vector3 lookPos = Vector3.zero;

                if (upwards == Vector3.down)
                    lookPos = new Vector3(-x, -100, -y);
                else
                    lookPos = new Vector3(x, 100, y);

                Quaternion rotation = Quaternion.LookRotation(lookPos, upwards);


                // Create and Save letter in the list
                letters.Add(createNewLetter(center + position, rotation, letter, atPosition, camouflaged, appear, set));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wall"></param>
    /// <param name="startPosition"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    private Vector3 getNewLetterPosition(Wall wall, Vector3 startPosition, int i, int j)
    {
        switch (wall)
        {
            case Wall.Left:
                return new Vector3(startPosition.x,
                                   startPosition.y - j * lineSpace,
                                   startPosition.z + i * letterSpace);
            case Wall.Right:
                return new Vector3(startPosition.x,
                                   startPosition.y - j * lineSpace,
                                   startPosition.z - i * letterSpace);
            case Wall.Back:
                return new Vector3(startPosition.x - i * letterSpace,
                                   startPosition.y - j * lineSpace,
                                   startPosition.z);
            case Wall.Front:
                return new Vector3(startPosition.x + i * letterSpace,
                                   startPosition.y - j * lineSpace,
                                   startPosition.z);
            default:
                return new Vector3(startPosition.x,
                                   startPosition.y - j * lineSpace,
                                   startPosition.z - i * letterSpace);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wall"></param>
    /// <returns></returns>
    private Quaternion getLetterRotationAtWall(Wall wall)
    {
        switch (wall)
        {
            case Wall.Left:
                return Quaternion.Euler(0, -90, 0);
            case Wall.Right:
                return Quaternion.Euler(0, 90, 0);
            case Wall.Back:
                return Quaternion.Euler(0, 180, 0);
            case Wall.Front:
                return Quaternion.Euler(0, 0, 0);
            default:
                return Quaternion.Euler(0, 0, 0);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wall"></param>
    /// <returns></returns>
    private Vector3 getLetterStartPositionAtWall(Wall wall)
    {
        switch (wall)
        {
            case Wall.Left:
                return new Vector3(leftWallGO.transform.position.x,
                                   leftWallGO.transform.position.y + leftWallGO.transform.lossyScale.y * 5 - topMargin,
                                   leftWallGO.transform.position.z - leftWallGO.transform.lossyScale.z * 5 + leftMargin);
            case Wall.Right:
                return new Vector3(rightWallGO.transform.position.x,
                                   rightWallGO.transform.position.y + rightWallGO.transform.lossyScale.y * 5 - topMargin,
                                   rightWallGO.transform.position.z + rightWallGO.transform.lossyScale.z * 5 - leftMargin);
            case Wall.Back:
                return new Vector3(backWallGO.transform.position.x + backWallGO.transform.lossyScale.z * 5 - leftMargin,
                                   backWallGO.transform.position.y + backWallGO.transform.lossyScale.y * 5 - topMargin,
                                   backWallGO.transform.position.z);
            case Wall.Front:
                return new Vector3(frontWallGO.transform.position.x - frontWallGO.transform.lossyScale.z * 5 + leftMargin,
                                   frontWallGO.transform.position.y + frontWallGO.transform.lossyScale.y * 5 - topMargin,
                                   frontWallGO.transform.position.z);
            default:
                return new Vector3(leftWallGO.transform.position.x,
                                   leftWallGO.transform.position.y + leftWallGO.transform.lossyScale.y * 5 - topMargin,
                                   leftWallGO.transform.position.z + leftWallGO.transform.lossyScale.z * 5 - leftMargin);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="letter"></param>
    /// <param name="atPosition"></param>
    /// <param name="camouflaged"></param>
    /// <param name="appear"></param>
    /// <param name="set"></param>
    /// <returns></returns>
    private GameObject createNewLetter(Vector3 position, Quaternion rotation, char letter, int atPosition, bool camouflaged, bool appear, char[] set)
    {
        // Instantiate the new letter in the desired position
        GameObject letterGO = Instantiate(letterPrefab, position, rotation) as GameObject;

        if (letterCount == atPosition && appear)
        {
            letterGO.GetComponent<TextMesh>().text = letter.ToString();

            if (!camouflaged)
                letterGO.GetComponent<TextMesh>().color = Color.red;
        }
        else
            letterGO.GetComponent<TextMesh>().text = generateRandomLetter(set, letter).ToString();

        // Count the total number of letters
        letterCount++;

        return letterGO;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    private char generateRandomLetter(char[] set)
    {
        return set[(int)Mathf.Round(Random.Range(0.0f, (float)(set.Length - 1)))];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <param name="omit"></param>
    /// <returns></returns>
    private char generateRandomLetter(char[] set, char omit)
    {
        char gen;
        do
        {
            gen = generateRandomLetter(set);

        } while (omit == gen);

        return gen;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private int generateRandomNumber(int min, int max)
    {
        return Random.Range(min, max);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="task_repetitions"></param>
    /// <param name="percent"></param>
    private void fillAppearValues(int task_repetitions, float percent)
    {
        appear = new bool[task_repetitions];

        // Calculate the number of time appear is true based on the specified percentage
        int appear_repetitions = (int)Mathf.Round((float)task_repetitions * ((float)percent / 100.0f));
        Debug.Log(appear_repetitions);

        // Populate the appear array with true for appearing and false for not appearing
        do
        {
            int i = 0;
            // It should appear the number of times of appear_repetitions.
            while(i < task_repetitions && appear_repetitions > 0)
            {
                // If the appear has a false value there
                if (!appear[i])
                {
                    // fill in the appear value false or true randomly
                    int b = Mathf.RoundToInt(Random.Range(0.0f, 1.0f));
                    appear[i] = (b == 0 ? false : true);

                    // If appear is true then substract 1 for the number of appear repetitions left
                    if (appear[i])
                        appear_repetitions--;
                }

                i++;
            }

        } while (appear_repetitions > 0);
    }

    /// <summary>
    /// 
    /// </summary>
    private void removeLetterAtDoor()
    {
        if (letterDoor != null)
        {
            Destroy(letterDoor);
        }

        isLetterShownAtDoor = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void removeAllLetters()
    {
        if (letters.Count != 0)
        {
            foreach (GameObject go in letters)
            {
                Destroy(go);
            }
            letters.Clear();
        }

        letterCount = 0;
        isLettersCreated = false;
    }

}
