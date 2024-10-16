using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrSoloLogic : MonoBehaviour
{
    private HashSet<Vector2Int> aliveCells;
    public bool active;
    public float period;
    private float timer;
    public GameObject tile_obj;
    [SerializeField] private float period_step = 1.1f;
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
            for (int x = 0; x < 240; x++)
            {
                for (int y = 0; y < 135; y++)
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

        #region Input system
        //Return to main menu
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu");

        // Placing cells
        if (!active && Input.GetMouseButtonDown (0))
            {
                //Debug.Log("Running, mouse at" + Input.mousePosition.x + " " + Input.mousePosition.y);
                Vector2Int cell = new Vector2Int ((int)Input.mousePosition.x, (int)Input.mousePosition.y);
                if (0 <= cell.x && cell.x <= Screen.width && 0 <= cell.y && cell.y <= Screen.height)
                {
                    cell.x /= 8;
                    cell.y /= 8;
                    if (aliveCells.Contains(cell))
                    {
                        Debug.Log("Removed cell at " + cell.x + " " + cell.y);
                        aliveCells.Remove(cell);
                    }
                    else
                    {
                        Debug.Log("Added cell at " + cell.x + " " + cell.y);
                        aliveCells.Add(cell);
                        Instantiate(tile_obj, new Vector3(cell.x, cell.y), Quaternion.identity);
                    }
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
            //if (active)
            //{
            //    foreach (Vector2Int i in aliveCells)
            //        Instantiate(tile_obj, new Vector3(i.x, i.y), Quaternion.identity);
            //}
        }
        // period control
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Equals))
        {
            period *= period_step;
            Debug.Log ("period++: " + period);
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Minus))
        {
            period /= period_step;
            Debug.Log("period--: " + period);
        }
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

    private void UpdateTiles()
    {
        HashSet<Vector2Int> aliveUpdate = new HashSet<Vector2Int>();
        foreach (Vector2Int i in aliveCells)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
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
