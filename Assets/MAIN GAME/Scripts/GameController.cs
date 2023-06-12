using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine.EventSystems;
using System.Linq;
using VisCircle;
using FluffyUnderware.Curvy.Examples;
using FluffyUnderware.Curvy.Controllers;

public class GameController : MonoBehaviour
{
    [Header("Variable")]
    public static GameController instance;
    public int maxLevel;
    public bool isStartGame = false;
    public bool isControl = false;
    int maxPlusEffect = 0;
    bool isVibrate = false;
    Rigidbody rigid;
    public float speed;
    Vector3 dir, firstPos, lastPos;
    bool isDrag = false;
    int currentSpline = 0;
    public int score;

    [Header("UI")]
    public GameObject winPanel;
    public GameObject losePanel;
    public Text currentLevelText;
    public int currentLevel;
    public Canvas canvas;
    public GameObject startGameMenu;
    public Text title;
    public Text timer;
    static int currentBG = 0;
    public Text status;
    public InputField levelInput;
    public GameObject runButton;
    public Image star1, star2, star3;
    public Text winTitle;
    public GameObject tutorial;

    [Header("Objects")]
    public GameObject plusVarPrefab;
    public GameObject conffeti;
    GameObject conffetiSpawn;
    public List<GameObject> listLevel = new List<GameObject>();
    public List<Color> listBGColor = new List<Color>();
    public GameObject BG;
    public GameObject coinPrefab;
    public GameObject blast;    
    public List<GameObject> listBalls = new List<GameObject>();
    public List<PaintSpline> listSpline = new List<PaintSpline>();
    public List<Color> ballColor = new List<Color>();
    PaintSpline spline;
    public GameObject ballPrefab;
    public int total = 20;
    public Transform cursor;
    public LayerMask dragMask;
    public GameObject hole;
    public GameObject worldUI;
    public SphereCollider collider;
    public GameObject pencils;
    public GameObject chooseEffect;
    public GameObject flag;
    public List<Transform> listPos = new List<Transform>();
    public List<GameObject> listObstacle = new List<GameObject>();
    public List<GameObject> ballType = new List<GameObject>();
    public List<GameObject> listCanon = new List<GameObject>();

    private void OnEnable()
    {
        // PlayerPrefs.DeleteAll();
        Application.targetFrameRate = 60;
        instance = this;
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(delayStart());
        maxLevel = listLevel.Count - 1;
        collider = GetComponent<SphereCollider>();
    }

    IEnumerator delayStart()
    {
        Camera.main.transform.DOMoveX(30, 0);
        Camera.main.transform.DOMoveX(0, 1);
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        currentLevelText.text = "LEVEL " + currentLevel.ToString();
        if(currentLevel == 0)
        {
            tutorial.SetActive(true);
        }
        score = PlayerPrefs.GetInt("score");
        status.text = score.ToString();
        var colorID = Random.Range(0, 5);
        // BG.GetComponent<Renderer>().material.color = listBGColor[currentBG];
        currentBG++;
        if (currentBG > listBGColor.Count - 1)
        {
            currentBG = 0;
        }
        if (currentLevel > 35)
        {
            foreach(var item in ballType)
            {
                item.gameObject.SetActive(true);
                var posId = Random.Range(0, listPos.Count - 1);
                item.transform.position = listPos[posId].position;
                listPos.RemoveAt(posId);
                listSpline.Add(item.GetComponent<PaintSpline>());
            }
            var posId2 = Random.Range(0, listPos.Count - 1);
            hole.transform.position = new Vector3(listPos[posId2].position.x, hole.transform.position.y, listPos[posId2].position.z);
            hole.SetActive(true);
            listPos.RemoveAt(posId2);
            var posId3 = Random.Range(0, listPos.Count - 1);
            var idOb = Random.Range(0, listObstacle.Count - 1);
            var obstacle = listObstacle[idOb];
            listObstacle.RemoveAt(idOb);
            obstacle.transform.position = new Vector3(listPos[posId3].position.x, obstacle.transform.position.y, listPos[posId3].position.z);
            obstacle.transform.parent = null;
            obstacle.transform.localScale /= 1.5f;
            listPos.RemoveAt(posId3);
            var posId4 = Random.Range(0, listPos.Count - 1);
            var idOb2 = Random.Range(0, listObstacle.Count - 1);
            var obstacle2 = listObstacle[idOb2];
            listObstacle.RemoveAt(idOb2);
            obstacle2.transform.position = new Vector3(listPos[posId4].position.x, obstacle2.transform.position.y, listPos[posId4].position.z);
            obstacle2.transform.parent = null;
            obstacle2.transform.localScale /= 1.75f;
            listPos.RemoveAt(posId4);
        }
        else
        {
            listLevel[currentLevel].SetActive(true);
            hole = listLevel[currentLevel].transform.GetChild(0).gameObject;
            var getSplines = listLevel[currentLevel].GetComponentsInChildren<PaintSpline>();
            listSpline = getSplines.ToList();
        }

        for(int i = 0; i < listSpline.Count; i++)
        {
            var canon = listCanon[i];
            canon.transform.parent = listSpline[i].transform;
            canon.transform.localPosition = Vector3.zero;
            canon.transform.LookAt(hole.transform, Vector3.up);
        }
        var standard = Instantiate(worldUI);
        standard.transform.parent = hole.transform;
        standard.transform.localPosition = new Vector3(-0.43f, 0.5f, 0);
        hole.GetComponent<Hole>().standardText = standard.transform.GetChild(0).GetComponent<Text>(); 
        startGameMenu.SetActive(true);
        //TinySauce.OnGameStarted(levelNumber: currentLevel.ToString());
        isControl = true;
        var ballCount = listSpline.Count;
        if(ballCount <= 1)
        {
            ballCount = 2;
        }
        hole.GetComponent<Hole>().standard = (ballCount - 1) * total;
        if(currentLevel == 0)
        {
            hole.GetComponent<Hole>().standard = 10;
        }
        hole.GetComponent<Hole>().standardText.text = hole.GetComponent<Hole>().standard.ToString();
        hole.GetComponent<Hole>().standardText.fontSize = 130;
        hole.GetComponent<Hole>().standardText.alignment = TextAnchor.MiddleRight;
        flag.transform.parent = hole.transform;
        flag.transform.localPosition = new Vector3(0, -0.2f, 0);
        flag.SetActive(true);
        int id = 0;
        foreach(var item in listSpline)
        {
            var tempTotal = total;
            Vector3 temp = item.transform.position;
            item.transform.position = Vector3.zero;
            // for(int radius = 1; tempTotal > 0; radius++)
            // {
            //     for (var i = 0; i < radius * 6; i += 1)
            //     {
            //         var angle = i * Mathf.PI * 2 / (radius * 6);
            //         var pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            //         var spawn = Instantiate(ballPrefab, pos, Quaternion.identity);
            //         spawn.GetComponent<Renderer>().material.color = ballColor[id];
            //         spawn.name = id.ToString();
            //         spawn.transform.parent = item.transform;
            //         item.listController.Add(spawn.GetComponent<SplineController>());
            //         tempTotal--;
            //         listBalls.Add(spawn);
            //     }
            // }
            for(int i = 0; i < total; i++)
            {
                var spawn = Instantiate(ballPrefab);
                spawn.GetComponent<Renderer>().material.color = ballColor[id];
                spawn.name = id.ToString();
                spawn.transform.parent = item.transform;
                spawn.transform.position = Vector3.zero;
                item.listController.Add(spawn.GetComponent<SplineController>());
                listBalls.Add(spawn);
            }
            item.transform.position = temp;
            item.Controller = item.listController[0];
            var ui = Instantiate(worldUI);
            ui.transform.parent = item.transform;
            ui.transform.localPosition = Vector3.zero;
            item.countText = ui.transform.GetChild(0).GetComponent<Text>();
            id++;
        }
        // listSpline[currentSpline].enabled = true;
        yield return new WaitForSeconds(1f);
        title.DOColor(new Color32(255, 255, 255, 0), 2);
    }

    private void Update()
    {
        if (isStartGame && isControl)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseDown();
            }

            if (Input.GetMouseButton(0))
            {
                OnMouseDrag();
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnMouseUp();
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, dragMask))
            {
                transform.position = hit.point;
            }
        }
        else if (!isStartGame && isControl)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ButtonStartGame();
                OnMouseDown();
            }
        }
    }

    void OnMouseDown()
    {
        // if (!isDrag)
        // {
            isDrag = true;
        // }
    }

    void OnMouseDrag()
    {
        // if (!isDrag)
        // {
            // isDrag = true;
            // collider.enabled = false;
        // }
    }

    void OnMouseUp()
    {
        isDrag = false;
        collider.enabled = true;
        listSpline[currentSpline].enabled = false;
        pencils.SetActive(false);
        chooseEffect.SetActive(false);
        bool isGoodToGo = true;
        foreach(var item in listSpline)
        {
            if(item.mRenderer.positionCount <= 1)
            {
                isGoodToGo = false;
                break;
            }
        }
        if(isGoodToGo)
        {
            runButton.SetActive(true);
            runButton.GetComponent<Button>().interactable = true;
            runButton.transform.DOKill();
            runButton.transform.localEulerAngles = new Vector3(0, 0, -10);
            runButton.transform.DOPunchRotation(new Vector3(0, 0, 20), 0.5f, 5).SetLoops(-1, LoopType.Yoyo);
            if(currentLevel == 0)
            {
                var tap = tutorial.transform.GetChild(0);
                tap.gameObject.SetActive(true);
                tap.transform.DOScale(Vector3.one * 1.05f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                tutorial.GetComponent<Image>().DOFade(0,0);
                tutorial.transform.GetChild(1).gameObject.SetActive(false);
                tutorial.transform.GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    public void PlusEffectMethod()
    {
        if (maxPlusEffect < 10)
        {
            Vector3 posSpawn = timer.transform.position;
            StartCoroutine(PlusEffect(posSpawn));
        }
    }

    IEnumerator PlusEffect(Vector3 pos)
    {
        maxPlusEffect++;
        if (!UnityEngine.iOS.Device.generation.ToString().Contains("5") && !isVibrate)
        {
            isVibrate = true;
            StartCoroutine(delayVibrate());
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }
        var plusVar = Instantiate(plusVarPrefab);
        plusVar.transform.SetParent(canvas.transform);
        plusVar.transform.localScale = new Vector3(1, 1, 1);
        //plusVar.transform.position = worldToUISpace(canvas, pos);
        plusVar.transform.position = new Vector3(pos.x + Random.Range(-50, 50), pos.y + Random.Range(-100, -75), pos.z);
        plusVar.GetComponent<Text>().DOColor(new Color32(255, 255, 255, 0), 1f);
        plusVar.SetActive(true);
        plusVar.transform.DOMoveY(plusVar.transform.position.y + Random.Range(50, 90), 0.5f);
        plusVar.transform.DOMoveX(timer.transform.position.x, 0.5f);
        Destroy(plusVar, 0.5f);
        yield return new WaitForSeconds(0.01f);
        maxPlusEffect--;
    }

    IEnumerator delayVibrate()
    {
        yield return new WaitForSeconds(0.2f);
        isVibrate = false;
    }

    public void Vibrate()
    {
        if (!UnityEngine.iOS.Device.generation.ToString().Contains("5") && !isVibrate)
        {
            isVibrate = true;
            StartCoroutine(delayVibrate());
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }
    }

    public Vector3 worldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        return parentCanvas.transform.TransformPoint(movePos);
    }

    public void ButtonStartGame()
    {
        startGameMenu.SetActive(false);
        isStartGame = true;
        isControl = true;
    }

    IEnumerator Win()
    {
        // float time = 4f;
        // var timePanel = timer.transform.parent;
        // timePanel.gameObject.SetActive(true);
        // timePanel.transform.GetChild(0).GetComponent<Image>().DOFillAmount(1, 3);
        float countDown = 1.5f;
        // timer.text = countDown.ToString();
        // var cdSeq = DOTween.Sequence();
        // cdSeq.AppendInterval(1).AppendCallback(() => { countDown -= 1; timer.text = countDown.ToString(); }).SetLoops(3).Play();
        // conffetiSpawn = Instantiate(conffeti);
        var blastDup = Instantiate(blast);
        blast.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        blastDup.SetActive(true);
        currentLevel++;
        // if (currentLevel > maxLevel)
        // {
        //     currentLevel = 0;
        // }
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        PlayerPrefs.SetInt("score", score);
        yield return new WaitForSeconds(countDown);
        StartCoroutine(Rating());
        // timePanel.gameObject.SetActive(false);
        if (isStartGame)
        {
            var star = PlayerPrefs.GetInt("currentStar");
            //TinySauce.OnGameFinished(levelNumber:currentLevel.ToString(), score);
            Debug.Log("Win");
            isStartGame = false;
            losePanel.SetActive(false);
            winPanel.SetActive(true);
            // conffetiSpawn.GetComponent<ParticleSystem>().loop = false;
            // blast.SetActive(false);
            // blast.SetActive(true);
            // yield return new WaitForSeconds(1);
            // yield return new WaitForSeconds(1);
            // LoadScene();
        }
    }

    public void Lose()
    {
        if (isStartGame)
        {
            var star = PlayerPrefs.GetInt("currentStar");
            //TinySauce.OnGameFinished(levelNumber:currentLevel.ToString(), score);
            Debug.Log("Lose");
            isStartGame = false;
            StartCoroutine(delayLose());
        }
    }

    IEnumerator delayLose()
    {
        yield return new WaitForSeconds(1);
        // status.text = "OPPS!!";
        losePanel.SetActive(true);
    }

    IEnumerator timeScanner()
    {
        var timePanel = timer.transform.parent;
        float angle = -1200;
        float a = Time.deltaTime * -360;
        while (angle < 0)
        {
            timePanel.transform.GetChild(0).RotateAround(timePanel.transform.position, Vector3.forward, a);
            angle += a;
            yield return null;
        }
        timePanel.transform.GetChild(0).RotateAround(timePanel.transform.position, Vector3.forward, angle);
    }

    public void ButtonContinue()
    {
        StartCoroutine(delayLoadScene());
    }

    IEnumerator delayLoadScene()
    {
        Camera.main.transform.DOMoveX(-60, 1);
        yield return new WaitForSeconds(1);
        LoadScene();
    }

    public void LoadScene()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        var temp = conffetiSpawn;
        Destroy(temp);
        SceneManager.LoadScene(0);
    }

    public void OnChangeMap()
    {
        if (levelInput != null)
        {
            int level = int.Parse(levelInput.text.ToString());
            Debug.Log(level);
            // if (level < maxLevel)
            // {
                PlayerPrefs.SetInt("currentLevel", level);
                SceneManager.LoadScene(0);
            // }
        }
    }

    public void ButtonNextLevel()
    {
        title.DOKill();
        isStartGame = true;
        currentLevel++;
        // if (currentLevel > maxLevel)
        // {
        //     currentLevel = 0;
        // }
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        SceneManager.LoadScene(0);
    }

    public void ButtonRun()
    {
        runButton.transform.DOKill();
        Destroy(runButton);
        for(int i = 0; i < listSpline.Count; i++)
        {
            listSpline[i].Run(listCanon[i].gameObject);
        }
        tutorial.SetActive(false);
    }

    public void RemoveBall(GameObject target)
    {
        Vibrate();
        listBalls.Remove(target);
        if(listBalls.Count == 0)
        {
            if(hole.GetComponent<Hole>().current >= hole.GetComponent<Hole>().standard)
            {
                StartCoroutine(Win());
            }
            else
            {
                Lose();
            }
            // Debug.Log(listBalls.Count + " " + hole.GetComponent<Hole>().standard);
        }
    }

    void OnTriggerStay(Collider other) {
        if(other.CompareTag("Control") && isDrag)
        {
            Vibrate();
            pencils.SetActive(true);
            listSpline[currentSpline].enabled = false;
            var temp1 = other.transform.parent;
            var temp2 = temp1.transform.parent;
            spline = temp2.GetComponent<PaintSpline>();
            spline.enabled = true;
            chooseEffect.transform.position = spline.transform.position;
            chooseEffect.SetActive(true);
            spline.mResetSpline = true;
            currentSpline = int.Parse(spline.name);
            // Debug.Log(currentSpline);
            // runButton.GetComponent<Button>().interactable = true;
            // runButton.transform.DOKill();
            // runButton.transform.localEulerAngles = new Vector3(0, 0, -10);
            // runButton.transform.DOPunchRotation(new Vector3(0, 0, 20), 0.5f, 5).SetLoops(-1, LoopType.Yoyo);
            collider.enabled = false;
        }
    }

    public void AutoHole(GameObject target)
    {
        var des = hole.transform.position;
        var dis = Vector3.Distance(target.transform.position, des);
        var time = dis / 18;
        target.transform.DOMove(new Vector3(des.x, target.transform.position.y, des.z), time).SetEase(Ease.Flash);
        try
        {
            Destroy(target, time + 0.1f);
            RemoveBall(target);
        }
        catch { }
    }

    IEnumerator Rating()
    {
        star1.DOKill();
        star2.DOKill();
        star3.DOKill();
        star1.gameObject.SetActive(true);
        star2.gameObject.SetActive(true);
        star3.gameObject.SetActive(true);
        float time = 0.2f;
        var standard = hole.GetComponent<Hole>().standard;
        var currentScore = hole.GetComponent<Hole>().current;
        Debug.Log(hole.GetComponent<Hole>().current + " " + total);
        if (currentScore == standard + 20 || currentLevel == 0)
        {
            winTitle.text = "PERFECT!";
            Debug.Log(3);
            star1.transform.DOScale(1, time);
            yield return new WaitForSeconds(time);
            star1.transform.DOScale(Vector3.one * 1.4f, time/2).SetLoops(4, LoopType.Yoyo);
            star2.transform.DOScale(1, time);
            yield return new WaitForSeconds(time);
            star2.transform.DOScale(Vector3.one * 1.4f, time/2).SetLoops(4, LoopType.Yoyo);
            star3.transform.DOScale(1, time);
            yield return new WaitForSeconds(time);
            star3.transform.DOScale(Vector3.one * 1.4f, time/2).SetLoops(4, LoopType.Yoyo);
            var star = PlayerPrefs.GetInt("currentStar");
            star += 3;
            PlayerPrefs.SetInt("currentStar", star);
        }
        else if (currentScore > standard &&  currentScore < standard + 20)
        {
            winTitle.text = "AWESOME!";
            Debug.Log(2);
            star1.transform.DOScale(1, time);
            yield return new WaitForSeconds(time);
            star1.transform.DOScale(Vector3.one * 1.4f, time/2).SetLoops(4, LoopType.Yoyo);
            star2.transform.DOScale(1, time);
            yield return new WaitForSeconds(time);
            star2.transform.DOScale(Vector3.one * 1.4f, time/2).SetLoops(4, LoopType.Yoyo);
            var star = PlayerPrefs.GetInt("currentStar");
            star += 2;
            PlayerPrefs.SetInt("currentStar", star);
        }
        else
        {
            winTitle.text = "GOOD JOB!";
            Debug.Log(1);
            star1.transform.DOScale(1, time);
            yield return new WaitForSeconds(time);
            star1.transform.DOScale(Vector3.one * 1.4f, time/2).SetLoops(2, LoopType.Yoyo);
            var star = PlayerPrefs.GetInt("currentStar");
            star += 1;
            PlayerPrefs.SetInt("currentStar", star);
        }
    }
}
