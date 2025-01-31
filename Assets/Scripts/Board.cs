using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static int w = 10;
    public static int h = 20;
    public static GameObject[,] grid = new GameObject[w, h];

    private static Spawner spawner;

    void Start()
    {
        spawner = Object.FindFirstObjectByType<Spawner>();
    }

    // Inicializa la cuadrícula con bloques inactivos
    public static void InitializeGrid(GameObject blockPrefab)
    {
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                GameObject block = Instantiate(blockPrefab, new Vector3(x, y, 0),
                Quaternion.identity);
                block.SetActive(false);
                grid[x, y] = block;
            }
        }
    }

    // Activa un bloque en la posición (x, y)
    public static void ActivateBlock(int x, int y)
    {
        if (grid[x, y] != null)
        {
            grid[x, y].SetActive(true);
        }
    }

    // Redondea un Vector2 para que no tenga valores decimales
    public static Vector2 RoundVector2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    // Devuelve true si la posición (x, y) está dentro de la cuadrícula, false en caso contrario
    public static bool InsideBorder(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < w && pos.y >= 0 && pos.y < h;
    }

    // Elimina todos los GameObjects en la fila Y y establece las celdas de la fila en null
    public static void DeleteRow(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            if (grid[x, y] != null)
            {
                grid[x, y].SetActive(false);
            }
        }
    }

    // Mueve todos los GameObjects en la fila Y a la fila Y-1
    public static void DecreaseRow(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            if (grid[x, y].activeSelf)
            {
                grid[x, y].SetActive(false);
                grid[x, y - 1].SetActive(true);
            }
        }
    }

    // Disminuye todas las filas por encima de Y
    public static void DecreaseRowsAbove(int y)
    {
        for (int row = y + 1; row < h; ++row)
        {
            for (int x = 0; x < w; ++x)
            {
                DecreaseRow(row);
            }
        }
    }

    // Devuelve true si todas las celdas en una fila tienen un GameObject (no son null), false en caso contrario
    public static bool IsRowFull(int y)
    {
        for (int x = 0; x < w; x++)
        {
            if (grid[x, y] == null || !grid[x, y].activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }

    // Elimina las filas completas
    public static void DeleteFullRows()
    {
        for (int y = 0; y < h; ++y)
        {
            if (IsRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowsAbove(y);
                --y;
                Spawner.fallSpeed *= 0.9f; // Decrementar la velocidad de caída
            }
        }
    }

    // Devuelve true si la posición actual de la pieza hace que la cuadrícula sea válida o no
    public static bool IsValidBoard(Vector2 pos, Transform pieceTransform)
    {
        if (!InsideBorder(pos))
            return false;

        if (grid[(int)pos.x, (int)pos.y] != null && grid[(int)pos.x, (int)pos.y].activeInHierarchy &&
            grid[(int)pos.x, (int)pos.y].transform.parent != pieceTransform)
            return false;

        return true;
    }

}