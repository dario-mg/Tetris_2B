using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.Collections;


public class Piece : MonoBehaviour
{

    private float lastFall;

    // Start se llama antes de la primera actualización del frame
    void Start()
    {

        // ¿Posición predeterminada no válida? Entonces es game over
        if (!IsValidBoard())
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }

        lastFall = Time.time;
    }

    // Update se llama una vez por frame.
    // Implementa todos los movimientos de la pieza: derecha, izquierda, rotar y bajar.
    void Update()
    {
        // Mover a la izquierda
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Modificar posición
            transform.position += new Vector3(-1, 0, 0);
            Debug.Log("Moved piece left");

            // Ver si es válido
            if (IsValidBoard())
                // Es válido. Actualizar la cuadrícula.
                UpdateBoard();
            else
                // No es válido. Revertir.
                transform.position += new Vector3(1, 0, 0);
        }

        // Implementar mover a la derecha (tecla RightArrow)
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            transform.position += new Vector3(1, 0, 0);
            Debug.Log("Moved piece right");

            if (IsValidBoard())
                // Es válido. Actualizar la cuadrícula.
                UpdateBoard();
            else
                // No es válido. Revertir.
                transform.position += new Vector3(-1, 0, 0);



        }

        // Implementar rotar, rota la pieza 90 grados (tecla UpArrow)

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, 90);
            Debug.Log("Rotated piece");

            if (IsValidBoard())
                UpdateBoard();
            else
            {
                transform.Rotate(0, 0, -90);
            }
        }

        // Implementar mover hacia abajo y caer (cada segundo)
        if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= 1)
        {
            MovePiece(new Vector3(0, -1, 0));
            lastFall = Time.time;
        }

    }

    // Actualiza la cuadrícula con la posición actual de la pieza.
    void UpdateBoard()
    {
        // Primero tienes que recorrer la cuadrícula y hacer que las posiciones actuales de la pieza sean nulas.
        for (int y = 0; y < Board.h; ++y)
        {
            for (int x = 0; x < Board.w; ++x)
            {
                if (Board.grid[x, y] != null && Board.grid[x, y].transform.parent == transform)
                {
                    Board.grid[x, y].SetActive(false);
                }
            }
        }
        Debug.Log("Updated board");
    }

    // Devuelve si la posición actual de la pieza hace que la cuadrícula sea válida o no
    bool IsValidBoard()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = Board.RoundVector2(child.position);
            if (!Board.InsideBorder(v))
            {
                return false;
            }

            if (Board.grid[(int)v.x, (int)v.y] != null &&
            Board.grid[(int)v.x, (int)v.y].activeInHierarchy &&
            Board.grid[(int)v.x, (int)v.y].transform.parent != transform)
            {
                return false;
            }
        }
        return true;
    }

    void MovePiece(Vector3 direction)
    {
        transform.position += direction;
        Debug.Log("Moved piece down");

        if (IsValidBoard())
        {
            UpdateBoard();
        }
        else
        {
            transform.position -= direction;
            if (direction == new Vector3(0, -1, 0))
            {
                // La pieza se ha detenido, ACTIVAR los bloques en la cuadrícula
                foreach (Transform child in transform)
                {
                    Vector2 v = Board.RoundVector2(child.position);
                    Board.ActivateBlock((int)v.x, (int)v.y);
                }
                Object.FindFirstObjectByType<Spawner>().ActivateNextPiece();
                transform.position = new Vector3(-100, -100, 0);
                // Mover a una ubicación no visible
                gameObject.SetActive(false);
                Board.DeleteFullRows();
                enabled = false;
                Debug.Log("Piece stopped and deactivated");
            }
        }
    }
}
