using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScrVersusLogic : MonoBehaviour
{

    private static Vector2Int boardSize = new Vector2Int(240, 135);
    private HashSet<Vector3Int> aliveCells;
    public bool active;
    public int countdown;
    public int player = 1;
    public float period;
    public Vector2Int localScore;
    public Vector2Int scoreSnapshot;
    public Vector2Int score;
    private float timer = 0;
    public GameObject tile_1;
    public GameObject tile_2;
    private int periodSize = 10;
    [SerializeField] private float period_step = 0.1f;
    [SerializeField] private float start_period = 0.5f;
    [SerializeField] private int countdown_total = 100;
    [SerializeField] public int max_placed = 10;
    void Start()
    {
        active = false;
        period = start_period;
        aliveCells = new HashSet<Vector3Int>();
        localScore = new Vector2Int(0, 0);
        scoreSnapshot = new Vector2Int(-1, -1);
        score = new Vector2Int(0, 0);
        countdown = countdown_total;

        #region Randomise board
        //if (PlayerPrefs.GetInt("Randomise") == 1)
        //{
        //    for (int x = 0; x < boardSize.x; x++)
        //    {
        //        for (int y = 0; y < boardSize.y; y++)
        //        {
        //            if (Random.Range(0, 100) >= fill_chance)
        //                aliveCells.Add(new Vector2Int(x, y));
        //        }
        //    }
        //    MakeTiles();
        //}
        #endregion
    }

    void Update()
    {
        #region Input system
        //Return to main menu
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu");

        // Placing cells
        if (!active && countdown > 0 && !Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown (0))
            {

            //Debug.Log("Running, mouse at" + Input.mousePosition.x + " " + Input.mousePosition.y);
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cell = new Vector3Int ((int)mousePos.x, (int)mousePos.y, player);
                if (0 <= cell.x && cell.x < boardSize.x && 0 <= cell.y && cell.y <boardSize.y)
                {
                    if (aliveCells.Contains(cell))
                    {
                        Debug.Log("Removed cell at " + cell.x + " " + cell.y);
                        aliveCells.Remove(cell);
                    }
                    else if (!aliveCells.Contains(new Vector3Int(cell.x, cell.y, 3 - player)))
                    {
                        Debug.Log("Added pattern at " + cell.x + " " + cell.y);
                        aliveCells.Add(cell);
                    }
                    CleanTiles();
                    MakeTiles();
                }
            }
        //// Pause/Go
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    active = !active;
        //    if (active)
        //    {
        //        timer = 0;
        //    }
        //}
        // period control
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Equals))
        {
            periodSize++;
            Debug.Log ("period++: " + period);
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Minus))
        {
            periodSize--;
            Debug.Log("period--: " + period);
        }
        period = period_step * Mathf.Clamp (periodSize, 1, 100);
        #endregion

        #region Match script
        if (countdown == countdown_total)
        {
            if (localScore.x >= max_placed)
                player = 2;
            if (localScore.y >= max_placed)
                active = true;        
        }
        if (countdown == countdown_total / 2 && scoreSnapshot.x == -1)
        {
            active = false;
            player = 1;
            scoreSnapshot = localScore;
        }
        if (countdown == countdown_total / 2)
        {
            if ((localScore.x - scoreSnapshot.x) >= max_placed)
                player = 2;
            if ((localScore.y - scoreSnapshot.y) >= max_placed)
                active = true;
        }
        if (countdown == 0)
            active = false;
        #endregion

        #region Simulation
        if (active)
        {
            timer += Time.deltaTime;
            if (timer >= period)
            {
                CleanTiles();
                UpdateTiles();
                MakeTiles();
                timer = 0;
                countdown--;
                score.x += localScore.x;
                score.y += localScore.y;
                Debug.Log("Score:" + score.x + "/" + score.y + " | Local:" + localScore.x + "/" + localScore.y);
            }
        }
        #endregion
    }

    private void UpdateTiles()
    {
        HashSet<Vector3Int> aliveUpdate = new HashSet<Vector3Int>();
        foreach (Vector3Int i in aliveCells)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (i.x + x < 0 || i.x + x >= boardSize.x || i.y + y < 0 || i.y + y >= boardSize.y)
                        continue;
                    //Vector2Int cnt = Lives(new Vector2Int(i.x + x, i.y + y));
                    //if (aliveCells.Contains(new Vector3Int(i.x + x, i.y + y, 1)) && cnt.x + cnt.y == 2 || aliveCells.Contains(new Vector3Int(i.x + x, i.y + y, 1)) && cnt.x + cnt.y == 3 || !(aliveCells.Contains(new Vector3Int(i.x + x, i.y + y, 1))) && cnt.x + cnt.y == 3)
                    //    aliveUpdate.Add(new Vector3Int(i.x + x, i.y + y, 1));
                    
                    Vector2Int cnt = Lives(new Vector2Int(i.x + x, i.y + y));
                    if ((cnt.x + cnt.y == 2 || cnt.x + cnt.y == 3) && (aliveCells.Contains(new Vector3Int(i.x + x, i.y + y, 1)) || aliveCells.Contains(new Vector3Int(i.x + x, i.y + y, 2))))
                    {
                        int winner = 1;
                        if (cnt.x > cnt.y)
                            winner = 1;
                        else if (cnt.x < cnt.y)
                            winner = 2;
                        else if (aliveCells.Contains(new Vector3Int(i.x + x, i.y + y, 1)))
                            winner = 1;
                        else
                            winner = 2;
                        aliveUpdate.Add(new Vector3Int(i.x + x, i.y + y, winner));
                    }
                    else if (cnt.x + cnt.y == 3 && !aliveCells.Contains(new Vector3Int(i.x + x, i.y + y, 1)) && !aliveCells.Contains(new Vector3Int(i.x + x, i.y + y, 2)))
                    {
                        int winner = 1;
                        if (cnt.x > cnt.y)
                            winner = 1;
                        else if (cnt.x < cnt.y)
                            winner = 2;
                        if (winner != 0)
                            aliveUpdate.Add(new Vector3Int(i.x + x, i.y + y, winner));
                    }
                }
            }
        }
        aliveCells = new HashSet<Vector3Int>(aliveUpdate);
    }

    private Vector2Int Lives (Vector2Int cell)
    {
        Vector2Int cnt = new Vector2Int (0, 0);
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (aliveCells.Contains(new Vector3Int(cell.x + x, cell.y + y, 1)))
                    cnt.x++;
                else if (aliveCells.Contains(new Vector3Int(cell.x + x, cell.y + y, 2)))
                    cnt.y++;
            }
        }
        return cnt;
    }

    private void MakeTiles ()
    {
        localScore.x = 0;
        localScore.y = 0;
        foreach (Vector3Int i in aliveCells)
        {
            if (i.z == 1)
            {
                Instantiate(tile_1, new Vector3(i.x, i.y), Quaternion.identity);
                localScore.x++;
            }
            else
            {
                Instantiate(tile_2, new Vector3(i.x, i.y), Quaternion.identity);
                localScore.y++;
            }
        }
    }

    private void CleanTiles ()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Tiles"))
            Destroy(obj);
    }

}
