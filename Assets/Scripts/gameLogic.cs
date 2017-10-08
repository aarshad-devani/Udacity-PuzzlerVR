using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;

public class gameLogic : MonoBehaviour {

    [Header("Game Variables")]
    public float timeToHideStones;
    public float humanHeight = 1.75f;
    public GameObject startPanel;
    public GameObject endPanel;
    public GameObject gameStartPoint;
    public GameObject gameMidPoint;
    public GameObject gameEndPoint;
    public bool fastMode = false;
    public AudioClip sinkingAudio;

    [Header("Hop Points for Realistic Jumps")]
    public GameObject hopPoint1;
    public GameObject hopPoint2;
    public GameObject hopPoint3;

    [Header("Miscellaneous")]
    public GameObject portalPoof;

    [Header("Change Game Level(In Progress)")]
    public int levelSize;
    public GameObject[] stonesArray;
    
    //Private Variables
    private int[] stoneSinkingArray;
    private GameObject[] hollowStones;
    private bool stonesHidden = false;
    private float startTime = 0f;
    private bool MbGameOver = false;
    private bool MbGameStarted = false;
    private GameObject MobjCamera;
    private Vector3 MvecInitialPosition;
    private int currentStoneIndex = 0;
    private GameObject clickedStone;

    private bool Level1Completed = false;
    private bool Level2Completed = false;
    private bool Level3Completed = false;
    private bool Level4Completed = false;
    private int levelCounter = 0;

    private GameObject[] Level1Stones = new GameObject[4];
    private GameObject[] Level2Stones = new GameObject[4];
    private GameObject[] Level3Stones = new GameObject[4];
    private GameObject[] Level4Stones = new GameObject[4];

    void Awake()
    {

    }

    void Start () {
        MobjCamera = Camera.main.transform.parent.transform.gameObject;
        MvecInitialPosition = MobjCamera.transform.position;
        MobjCamera.transform.position = new Vector3(MvecInitialPosition.x, humanHeight, MvecInitialPosition.z);
        Level1Stones = stonesArray.Slice(0, 4);
        Level2Stones = stonesArray.Slice(4, 8);
        Level3Stones = stonesArray.Slice(8, 12);
        Level4Stones = stonesArray.Slice(12, 16);
        portalPoof.SetActive(false);
        gameStartLogic();
    }

    void Update () {
        if (stonesHidden)
        {
            if(Time.time - startTime > timeToHideStones)
            {
                stonesHidden = false;
                ShowAllStones();
            }
        }
	}

    void StoneClicking(bool toggle)
    {
        if (!fastMode)
        {
            foreach (GameObject stone in stonesArray)
            {
                stone.GetComponent<Collider>().enabled = toggle;
            }
        }
    }

    void gameStartLogic()
    {
        stoneSinkingArray = new int[levelSize];
        hollowStones = new GameObject[levelSize * levelSize - 1];
        if (MbGameStarted)
        {
            MvecInitialPosition = MobjCamera.transform.position;
            MobjCamera.transform.position = new Vector3(MvecInitialPosition.x, humanHeight, MvecInitialPosition.z);
        }
        toggleInfoPanels();
    }

    void generateLogic()
    {
        stoneSinkingArray = new int[levelSize];
        List<int> stoneSinkingNumbers = new List<int> { };
        System.Random LrndRandom = new System.Random();
        for(int i = 0; i < levelSize; i++)
        {
            int LintRandomNumber = LrndRandom.Next(1, levelSize+1);
            stoneSinkingNumbers.Add(LintRandomNumber);
        }
        stoneSinkingArray = stoneSinkingNumbers.ToArray();
        string path = "";
        foreach(int value in stoneSinkingArray)
        {
            path = path + value + ",";
        }
        Debug.Log("GameLogic is generated and correct path is : "+path);
        showPath();
    }

    public void RestartGame()
    {
        //MbGameOver = false;
        //MobjCamera.transform.position = new Vector3(MvecInitialPosition.x, humanHeight, MvecInitialPosition.z);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void showPath()
    {
        StoneClicking(false);
        hollowStones = new GameObject[levelSize * levelSize-1];
        List<GameObject> stonesList = new List<GameObject> { };
        for(int i = 0; i < levelSize; i++)
        {
            var levelStoneNumber = stoneSinkingArray[i];
            for (int j = 0; j < levelSize; j++)
            {
                int stoneNumber = i == 0 ? j + 1 : (i * levelSize) + j + 1;
                if(stoneNumber % 4 != (levelStoneNumber == 4 ? 0 : levelStoneNumber) )
                {
                    GameObject stoneObject = stonesArray[stoneNumber-1];
                    this.gameObject.GetComponent<AudioSource>().PlayOneShot(sinkingAudio, 0.5f);
                    stonesList.Add(stoneObject);
                    stoneObject.SetActive(false);
                }
            }
        }
        hollowStones = stonesList.ToArray();
        stonesHidden = true;
        startTime = Time.time;
    }

    void ShowAllStones()
    {
        foreach(GameObject stone in hollowStones)
        {
            stone.SetActive(true);
        }
        StoneClicking(true);
    }

    public static int IndexOfObject(GameObject[] arr, GameObject value)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == value)
            {
                return i;
            }
        }
        return -1;
    }

    public void onStoneClick(GameObject stoneObject)
    {
        clickedStone = stoneObject;
        GameObject cameraObject = Camera.main.transform.parent.gameObject;
        int stoneIndex = IndexOfObject(stonesArray, stoneObject) + 1;
        stoneIndex = stoneIndex > 4 ? stoneIndex % 4 == 0 ? 4 : stoneIndex % 4 : stoneIndex;
        int hopIndex = (currentStoneIndex + stoneIndex) / 2;
        hopIndex = hopIndex == 0 ? 1 : currentStoneIndex==0?stoneIndex:hopIndex;
        currentStoneIndex = stoneIndex;
        if (IndexOfObject(hollowStones, stoneObject) >= 0)
        {
            stoneObject.GetComponent<AudioSource>().Play();
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(sinkingAudio, 0.3f);
            MbGameOver = true;
            GoBackandShowPath();
            StoneClicking(true);
        }
        else
        {
            GameObject hopPoint;
            switch (hopIndex)
            {
                case 1:
                    hopPoint =  hopPoint1;
                    break;
                case 2:
                    hopPoint =  hopPoint1;
                    break;
                case 3:
                    hopPoint =  hopPoint2;
                    break;
                case 4:
                    hopPoint = hopPoint3;
                    break;
                default:
                    hopPoint = hopPoint1;
                    break;

            };
            this.gameObject.GetComponent<AudioSource>().Play();
            //animate Camera to move;
            iTween.MoveTo(MobjCamera,
            iTween.Hash(
                "position", hopPoint.transform.position,
                "time", 0.25F,
                "easetype", iTween.EaseType.easeInSine,
                "oncomplete", "MoveToStone",
                "oncompletetarget", this.gameObject
                )
            );
        }
    }

    public void MoveToStone()
    {
        //Debug.Log("Moving to Stone");
        iTween.MoveTo(MobjCamera,
            iTween.Hash(
                "position", new Vector3(clickedStone.transform.position.x, humanHeight - 0.75f, clickedStone.transform.position.z),
                "time", 0.25F,
                "easetype", iTween.EaseType.easeInSine,
                "oncomplete", "MoveHopPoints",
                "oncompletetarget", this.gameObject
                )
        );
    }

    void MoveHopPoints()
    {
        hopPoint1.transform.position = hopPoint1.transform.position + new Vector3(0f, 0f, 1.5f);
        hopPoint2.transform.position = hopPoint2.transform.position + new Vector3(0f, 0f, 1.5f);
        hopPoint3.transform.position = hopPoint3.transform.position + new Vector3(0f, 0f, 1.5f);
        levelCounter++;
        switch (levelCounter)
        {
            case 1:
                DisableStonesOfThisArray(Level1Stones);
                break;
            case 2:
                DisableStonesOfThisArray(Level2Stones);
                break;
            case 3:
                DisableStonesOfThisArray(Level3Stones);
                break;
            case 4:
                MoveToMid();
                DisableStonesOfThisArray(Level4Stones);
                break;
        }
    }

    void toggleInfoPanels()
    {
        startPanel.SetActive(!MbGameOver);
        endPanel.SetActive(MbGameOver);
    }

    void MoveToMid()
    {
        iTween.MoveTo(MobjCamera,
            iTween.Hash(
                "position", gameMidPoint.transform.position,
                "time", 2F,
                "easetype", "linear",
                "oncomplete", "MoveToEnd",
                "oncompletetarget", this.gameObject

            )
        );
    }

    void MoveToEnd()
    {
        MbGameOver = true;
        iTween.MoveTo(MobjCamera,
            iTween.Hash(
                "position", gameEndPoint.transform.position,
                "time", 2F,
                "easetype", "linear"
                )
        );
        portalPoof.SetActive(true);
        toggleInfoPanels();
        
    }

    
    void DisableStonesOfThisArray(GameObject[] thisstoneArray)
    {
        foreach(GameObject stone in thisstoneArray)
        {
            stone.GetComponent<Collider>().enabled = false;
        }
    }

    void GoBackandShowPath()
    {
        levelCounter = 0;
        MbGameOver = false;
        iTween.MoveTo(MobjCamera,
            iTween.Hash(
                "position", gameStartPoint.transform.position,
                "time", 0.5F,
                "easetype", "linear"
                )
        );
        showPath();
    }


    public void startGame()
    {
        MbGameStarted = true;
        iTween.MoveTo(MobjCamera,
            iTween.Hash(
                "position", new Vector3(-14.85f, humanHeight, -11.88f),
                "time", 2F,
                "easetype", "linear"
            )
        );
        generateLogic();

    }
}

public static class Extensions
{
    public static T[] Slice<T>(this T[] source, int start, int end)
    {
        if (end < 0)
        {
            end = source.Length + end;
        }
        int len = end - start;

        T[] res = new T[len];
        for (int i = 0; i < len; i++)
        {
            res[i] = source[i + start];
        }
        return res;
    }
}
