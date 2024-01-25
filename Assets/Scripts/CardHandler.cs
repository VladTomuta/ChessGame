using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHandler : NetworkBehaviour, IPointerUpHandler, IPointerDownHandler
{
    
    private bool isSelected;
    private bool isChecked;

    private Graphic cardGraphic;
    [SerializeField] private GameObject spawnPlatePrefab;
    [SerializeField] private GameObject lastMove;

    private string player;

    private const string selectedColor = "#D0DDFF";
    private const string unselectedColor = "#000000";

    private void Start() 
    {
        isSelected = false;
        isChecked = false;

        cardGraphic = GetComponent<Graphic>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isChecked = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        Debug.Log("OnPointerDown: IsHost = " + IsHost + "; player = " + player);

        if(IsHost && player == "black")
            return;

        if(!IsHost && player == "white")
            return;

        DestroyMovePlates("MovePlate");

        Transform parentTransform = transform.parent;
        Transform[] siblings = GetSiblings(parentTransform);
        
        foreach (Transform sibling in siblings)
        {
            sibling.GetComponent<CardHandler>().SetIsSelected(false);
            sibling.GetComponent<CardHandler>().SetColor(unselectedColor);
            // CardHandler siblingCardHandler = sibling.GetComponent<CardHandler>();
            // if (siblingCardHandler.GetNewChessPieceReference()) {
            //     siblingCardHandler.GetNewChessPieceReference().GetComponent<Chessman>().DestroyMovePlates("MovePlate");
            //     gameScript.DestroyPieceServerRpc(siblingCardHandler.GetNewChessPieceReference());
            //     siblingCardHandler.SetNewChessPieceReference(null);
            // }
        }
        
        isSelected = !isSelected;
        isChecked = true;

        // CreateNewChessPieceReference(gameObject.name);

        // Debug.Log("Se presupune ca am creat obiectul :D");
        // Debug.Log(newChessPieceReference);
        // newChessPieceReference.GetComponent<Chessman>().SpawnLocationMovePlate();

        if(isSelected) {
            SetColor(selectedColor);
            for(int i = 0; i < 8; i++) {
                for (int j = 0; j < 4; j++) {
                    if (IsHost) {
                        PointMovePlate(i,j);
                    } else {
                        PointMovePlate(i,7-j);
                    }
                }
            }
        } else {
            SetColor(unselectedColor);
            // newChessPieceReference.GetComponent<Chessman>().DestroyMovePlates("MovePlate");
            // Debug.Log("newChessPieceReference before destroying the piece: " + newChessPieceReference);
            // gameScript.DestroyPieceServerRpc(newChessPieceReference);
            // newChessPieceReference = null;
        }
    }

    public bool GetIsSelected()
    {
        return isSelected;
    }

    public bool GetIsChecked()
    {
        return isChecked;
    }

    // public GameObject GetNewChessPieceReference()
    // {
    //     return newChessPieceReference;
    // }

    public void SetIsSelected(bool isSelected)
    {
        this.isSelected = isSelected;
    }

    public void SetIsChecked(bool isChecked)
    {
        this.isChecked = isChecked;
    }

    public void SetColor(string color) 
    {
        Color newColor;
        ColorUtility.TryParseHtmlString(color, out newColor);
        cardGraphic.color = newColor;
    }

    public void SetPlayer(string player)
    {
        this.player = player;
    }

    // public void SetNewChessPieceReference(GameObject newChessPieceReference) {
    //     this.newChessPieceReference = newChessPieceReference;
    // }

    private Transform[] GetSiblings(Transform parent)
    {
        int childCount = parent.childCount - 1; // Exclude the calling object
        Transform[] siblings = new Transform[childCount];
        int index = 0;

        foreach (Transform child in parent)
        {
            if (child != transform)
            {
                siblings[index] = child;
                index++;
            }
        }

        return siblings;
    }

    public string GetPlayer()
    {
        return player;
    }

    public void PointMovePlate(int x, int y) {
        Game gameScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        // Debug.Log("Position: x: " + x + " y: " + y);
        if (gameScript.PositionOnBoard(x, y)) {
            GameObject chessPiece = gameScript.GetPosition(x, y);

            if (chessPiece == null) {
                SpawnPlateSpawn(x, y);
            } 
        }
    }

    public void SpawnPlateSpawn(int matrixX, int matrixY) {
        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.31f;
        y += -2.31f;

        GameObject spawnPlate = Instantiate(spawnPlatePrefab, new Vector3(x, y, 97f), Quaternion.identity);

        SpawnPlate spawnPlateScript = spawnPlate.GetComponent<SpawnPlate>();
        spawnPlateScript.SetReference(gameObject);
        spawnPlateScript.SetCoords(matrixX, matrixY);
    }

    // private async void CreateNewChessPieceReference(string chessPieceName) 
    // {
    //     switch (chessPieceName) {
    //         case "WhitePawnCard":
    //             await Task.Run(() =>
    //             {
    //                 gameScript.CreatePieceServerRpc("white_pawn", 10, 10);
    //             });
    //             break;
    //         case "WhiteRookCard":
    //             gameScript.CreatePieceServerRpc("white_rook", 10, 10);
    //             break;
    //         case "WhiteKnightCard":
    //             gameScript.CreatePieceServerRpc("white_knight", 10, 10);
    //             break;
    //         case "WhiteBishopCard":
    //             gameScript.CreatePieceServerRpc("white_bishop", 10, 10);
    //             break;
    //         case "WhiteQueenCard":
    //             gameScript.CreatePieceServerRpc("white_queen", 10, 10);
    //             break;
    //         case "BlackPawnCard":
    //             gameScript.CreatePieceServerRpc("black_pawn", 10, 10);
    //             break;
    //         case "BlackRookCard":
    //             gameScript.CreatePieceServerRpc("black_rook", 10, 10);
    //             break;
    //         case "BlackKnightCard":
    //             gameScript.CreatePieceServerRpc("black_knight", 10, 10);
    //             break;
    //         case "BlackBishopCard":
    //             gameScript.CreatePieceServerRpc("black_bishop", 10, 10);
    //             break;
    //         case "BlackQueenCard":
    //             await Task.Run(() =>
    //             {
    //                 gameScript.CreatePieceServerRpc("black_queen", 10, 10);
    //             });
    //             break;
    //     }

        
    //     newChessPieceReference = gameScript.GetRefToLastChessPieceCreated();
    // }

    [ServerRpc (RequireOwnership = false)]
    public void SpawnLastMoveServerRpc(int matrixX, int matrixY) {
        SpawnLastMoveClientRpc(matrixX, matrixY);
    }

    [ClientRpc]
    public void SpawnLastMoveClientRpc(int matrixX, int matrixY) {
        Debug.Log("Am spawnat si local last move");
        LastMoveSpawn(matrixX, matrixY);
        //LastMoveSpawn(newX, newY);
    }

    [ServerRpc (RequireOwnership = false)]
    public void DestroyAllLastMovesServerRpc(string tag) {
        DestroyAllLastMovesClientRpc(tag);
    }

    [ClientRpc]
    public void DestroyAllLastMovesClientRpc(string tag) {
        DestroyMovePlates(tag);
    }

    public void DestroyMovePlates(string tag) {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < movePlates.Length; i++) {
            Destroy(movePlates[i]);
        }
    }

    public void LastMoveSpawn(int matrixX, int matrixY) {
        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.31f;
        y += -2.31f;

        GameObject MovePlate = Instantiate(lastMove, new UnityEngine.Vector3(x, y, 97f), UnityEngine.Quaternion.identity);

        MovePlate movePlateScript = MovePlate.GetComponent<MovePlate>();
        movePlateScript.SetReference(gameObject);
        movePlateScript.SetCoords(matrixX, matrixY);
    }
}
