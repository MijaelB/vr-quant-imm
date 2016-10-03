using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LetterGenerator : MonoBehaviour {

    public enum Wall { Left, Right, Back, Front, Bottom, Top };

    public GameObject letterPrefab;
    public GameObject doorGO;
    public GameObject leftWallGO;
    public GameObject rightWallGO;
    public GameObject backWallGO;
    public GameObject frontWallGO;
    public GameObject topWallGO;
    public GameObject bottomWallGO;

    public float leftMargin = 0.833f;
    public float topMargin = 0.76f;
    public float lineSpace = 1.273f;
    public float letterSpace = 1.273f;

    public float charWallColumns = 6;
    public float charWallRows = 6;

    private List<GameObject> letters = new List<GameObject>();

    private GameObject letterStartGO;
    private TextMesh letterText;

    private char[] letters_set1 = {'A', 'K', 'M', 'N', 'V', 'W', 'X', 'Y', 'Z' };
    private char[] letters_set2 = {'E', 'F', 'H', 'I', 'L', 'T'};


    // Use this for initialization
    void Start () {

        // Test
        // Character on the Door
        GameObject letter = Instantiate(letterPrefab, doorGO.transform.position, doorGO.transform.rotation) as GameObject;

        letterText = letter.GetComponent<TextMesh>();
        letterText.text = generateRandomLetter(1).ToString();

        // Character on the LeftWall

        generateRandomLettersAtWall(Wall.Left, new int[] { 1, 2 });
        generateRandomLettersAtWall(Wall.Right, new int[] { 1, 2 });
        generateRandomLettersAtWall(Wall.Front, new int[] { 1, 2 });
        generateRandomLettersAtWall(Wall.Back, new int[] { 1, 2 });
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void showHidenLetterAtDoor(char hiden)
    {

    }

    public void generateRandomLetters(char hiden)
    {

    }

    private void generateRandomLettersAtWall(Wall wall, int[] ignore)
    {

        Vector3 startPosition;
        Quaternion rotation;

        switch (wall)
        {
            case Wall.Left:
                startPosition = new Vector3(leftWallGO.transform.position.x,
                                               leftWallGO.transform.position.y + leftWallGO.transform.lossyScale.y * 5 - topMargin,
                                               leftWallGO.transform.position.z - leftWallGO.transform.lossyScale.z * 5 + leftMargin);
                rotation = Quaternion.Euler(0, -90, 0);
                break;
            case Wall.Right:
                startPosition = new Vector3(rightWallGO.transform.position.x,
                                            rightWallGO.transform.position.y + rightWallGO.transform.lossyScale.y * 5 - topMargin,
                                            rightWallGO.transform.position.z + rightWallGO.transform.lossyScale.z * 5 - leftMargin);
                rotation = Quaternion.Euler(0, 90, 0);
                break;
            case Wall.Back:
                startPosition = new Vector3(backWallGO.transform.position.x + backWallGO.transform.lossyScale.z * 5 - leftMargin,
                                            backWallGO.transform.position.y + backWallGO.transform.lossyScale.y * 5 - topMargin,
                                            backWallGO.transform.position.z);
                rotation = Quaternion.Euler(0, 180, 0);
                break;
            case Wall.Front:
                startPosition = new Vector3(frontWallGO.transform.position.x - frontWallGO.transform.lossyScale.z * 5 + leftMargin,
                                            frontWallGO.transform.position.y + frontWallGO.transform.lossyScale.y * 5 - topMargin,
                                            frontWallGO.transform.position.z);
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            default:
                startPosition = new Vector3(leftWallGO.transform.position.x,
                                            leftWallGO.transform.position.y + leftWallGO.transform.lossyScale.y * 5 - topMargin,
                                            leftWallGO.transform.position.z + leftWallGO.transform.lossyScale.z * 5 - leftMargin);
                rotation = Quaternion.Euler(0, 90, 0);
                break;
        }
     
        int letterCount = 0;
        for (int i = 0; i < charWallColumns; i++)
        {
            for (int j = 0; j < charWallRows; j++)
            {
                Vector3 letterPosition;
                switch (wall)
                {
                    case Wall.Left:
                        letterPosition = new Vector3(startPosition.x,
                                                     startPosition.y - j * lineSpace,
                                                     startPosition.z + i * letterSpace);
                        break;
                    case Wall.Right:
                        letterPosition = new Vector3(startPosition.x,
                                                     startPosition.y - j * lineSpace,
                                                     startPosition.z - i * letterSpace);
                        break;
                    case Wall.Back:
                        letterPosition = new Vector3(startPosition.x - i * letterSpace,
                                                     startPosition.y - j * lineSpace,
                                                     startPosition.z);
                        break;
                    case Wall.Front:
                        letterPosition = new Vector3(startPosition.x + i * letterSpace,
                                                     startPosition.y - j * lineSpace,
                                                     startPosition.z);
                        break;
                    default:
                        letterPosition = new Vector3(startPosition.x,
                                                    startPosition.y - j * lineSpace,
                                                    startPosition.z - i * letterSpace);
                        break;
                }

                GameObject letter = Instantiate(letterPrefab, letterPosition, rotation) as GameObject;
                letter.GetComponent<TextMesh>().text = generateRandomLetter(1).ToString();

                letters.Add(letter);

                letterCount++;
            }
        }
    }

    


    private char generateRandomLetter(int set)
    {
        if (set == 1)
            return letters_set1[Random.Range(0, letters_set1.Length - 1)];

        return letters_set2[Random.Range(0, letters_set2.Length - 1)];
    }

}
