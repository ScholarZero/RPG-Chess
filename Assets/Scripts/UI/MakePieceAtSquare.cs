﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakePieceAtSquare : MonoBehaviour {
    [SerializeField]
    GameObject WhitePiece;
    [SerializeField]
    GameObject BlackPiece;

    private int Location = -1;
    private bool IsWhite;

    public void MakePiece()
    {
        if (MakePiece(GameManager.Instance.Board, IsWhite ? WhitePiece : BlackPiece))
        {
            GameManager.Instance.PieceAddedEvent.Invoke(Location, IsWhite ? "White" : "Black");
        }
        GameManager.Instance.PromotionEvent.Invoke(-1, "Off");
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PromotionEvent.AddListener(promotedLocation);
        }
    }

    private void promotedLocation(int atLocation, string type)
    {
        Location = atLocation;
        IsWhite = type.Contains("White");
    }

    private bool MakePiece(List<GameObject> board, GameObject prefab)
    {
        if(Location < 0)
        {
            Debug.LogError("Location is less than zero.  MakePiece() does not know where to place created piece.  Are you sure the UI is displaying correctly?");
            return false;
        }

        if (board[Location].GetComponent<Square>().Piece != null)
        {
            Destroy(board[Location].GetComponent<Square>().Piece.gameObject);
        }

        GameObject piece = Instantiate(prefab);
        Square square = board[Location].GetComponent<Square>();
        piece.transform.position = square.transform.position;
        piece.transform.SetParent(square.transform.parent);
        square.Piece = piece.GetComponent<IChessPiece>();

        if(square.Piece == null)
        {
            Debug.LogError("Prefab created does not implement interface IChessPiece");
            return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PromotionEvent.RemoveListener(promotedLocation);  // A good habit to get into
        }
    }
}
