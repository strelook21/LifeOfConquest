using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum Pattern
{
    DOT, BLOCK, BLINKER, GLIDER, TUB
}

public class ScrSoloLogic : MonoBehaviour
{

    private static Vector2Int boardSize = new Vector2Int(240, 135);
    private HashSet<Vector2Int> aliveCells;
    public bool active;
    public float period;
    private float timer;
    public GameObject tile_obj;
    public Pattern pattern = Pattern.DOT;
    private int periodSize = 10;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private float period_step = 0.1f;
    [SerializeField] private float start_period = 1f;
    [SerializeField] private float fill_chance = 50;
    void Start()
    {
        active = false;
        period = start_period;
        aliveCells = new HashSet<Vector2Int>();

        #region Randomise board
        if (PlayerPrefs.GetInt("Randomise") == 1)
        {
            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = 0; y < boardSize.y; y++)
                {
                    if (Random.Range(0, 100) >= fill_chance)
                        aliveCells.Add(new Vector2Int(x, y));
                }
            }
            MakeTiles();
        }
        #endregion
    }
    
    void Update()
    {

        pattern = (Pattern)dropdown.value;
        #region Input system
        //Return to main menu
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu");

        // Placing cells
        if (!active && !Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown (0))
            {

            //Debug.Log("Running, mouse at" + Input.mousePosition.x + " " + Input.mousePosition.y);
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int cell = new Vector2Int ((int)mousePos.x, (int)mousePos.y);
                if (0 <= cell.x && cell.x < boardSize.x && 0 <= cell.y && cell.y <boardSize.y)
                {
                    if (aliveCells.Contains(cell))
                    {
                        Debug.Log("Removed cell at " + cell.x + " " + cell.y);
                        aliveCells.Remove(cell);
                    }
                    else
                    {
                        Debug.Log("Added pattern at " + cell.x + " " + cell.y);
                        AddTiles(cell);
                    }
                    CleanTiles();
                    MakeTiles();
                }
            }
        // Pause/Go
        if (Input.GetKeyDown(KeyCode.Space))
        {
            active = !active;
            if (active)
            {
                timer = 0;
            }
        }
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
            }
        }
        #endregion
    }

    private void AddTiles (Vector2Int cell)
    {
        switch (pattern)
        {
            case Pattern.DOT:
                aliveCells.Add(cell);
                break;
            case Pattern.BLOCK:
                aliveCells.Add(cell);
                aliveCells.Add(new Vector2Int(cell.x + 1, cell.y));
                aliveCells.Add(new Vector2Int(cell.x, cell.y + 1));
                aliveCells.Add(new Vector2Int(cell.x + 1, cell.y + 1));
                break;
            case Pattern.BLINKER:
                aliveCells.Add(cell);
                aliveCells.Add(new Vector2Int(cell.x + 1, cell.y));
                aliveCells.Add(new Vector2Int(cell.x - 1, cell.y));
                break;
            case Pattern.GLIDER:
                aliveCells.Add(cell);
                aliveCells.Add(new Vector2Int(cell.x + 1, cell.y - 1));
                aliveCells.Add(new Vector2Int(cell.x - 1, cell.y - 2));
                aliveCells.Add(new Vector2Int(cell.x, cell.y - 2));
                aliveCells.Add(new Vector2Int(cell.x + 1, cell.y - 2));
                break;
            case Pattern.TUB:
                aliveCells.Add(new Vector2Int(cell.x + 1, cell.y));
                aliveCells.Add(new Vector2Int(cell.x - 1, cell.y));
                aliveCells.Add(new Vector2Int(cell.x, cell.y + 1));
                aliveCells.Add(new Vector2Int(cell.x, cell.y - 1));
                break;
        }
    }

    private void UpdateTiles()
    {
        HashSet<Vector2Int> aliveUpdate = new HashSet<Vector2Int>();
        foreach (Vector2Int i in aliveCells)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (i.x + x < 0 || i.x + x >= boardSize.x || i.y + y < 0 || i.y + y >= boardSize.y)
                        continue;
                    if (aliveCells.Contains(new Vector2Int(i.x + x, i.y + y)) && Lives(new Vector2Int(i.x + x, i.y + y)) == 2 || aliveCells.Contains(new Vector2Int(i.x + x, i.y + y)) && Lives(new Vector2Int(i.x + x, i.y + y)) == 3 || !(aliveCells.Contains(new Vector2Int(i.x + x, i.y + y))) && Lives(new Vector2Int(i.x + x, i.y + y)) == 3)
                        aliveUpdate.Add(new Vector2Int(i.x + x, i.y + y));
                }
            }
        }
        aliveCells = new HashSet<Vector2Int>(aliveUpdate);
    }

    private int Lives (Vector2Int cell)
    {
        int cnt = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                cnt += (aliveCells.Contains(new Vector2Int(cell.x + x, cell.y + y))) ? 1 : 0;
            }
        }
        return cnt;
    }

    private void MakeTiles ()
    {
        foreach (Vector2Int i in aliveCells)
            Instantiate(tile_obj, new Vector3(i.x, i.y), Quaternion.identity);
    }

    private void CleanTiles ()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Tiles"))
            Destroy(obj);
    }

}
