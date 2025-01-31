using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject[] piecePrefabs;
    public GameObject blockPrefab;
    private List<GameObject> piecesPool = new List<GameObject>();
    public static float fallSpeed = 1.0f;

    void Start()
    {
        // Crear y desactivar todas las piezas al inicio
        foreach (GameObject prefab in piecePrefabs)
        {
            GameObject piece = Instantiate(prefab, transform.position, Quaternion.identity);
            piece.SetActive(false);
            piecesPool.Add(piece);
        }
        Board.InitializeGrid(blockPrefab);
        ActivateNextPiece();
    }

    // Activa la siguiente pieza

    public void ActivateNextPiece()
    {
        // Seleccionar una pieza aleatoria del pool
        int randomIndex = Random.Range(0, piecesPool.Count);
        GameObject piece = piecesPool[randomIndex];
        while (piece.activeInHierarchy)
        {
            randomIndex = Random.Range(0, piecesPool.Count);
            piece = piecesPool[randomIndex];
        }
        // Activar la pieza seleccionada
        piece.transform.position = new Vector3(5, 16, 0);
        piece.SetActive(true);
        piece.GetComponent<Piece>().enabled = true;
    }
}