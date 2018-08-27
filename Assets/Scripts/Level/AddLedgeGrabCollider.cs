using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AddLedgeGrabCollider : MonoBehaviour {

    public List<Sprite> cornerTilesList;
    public List<Sprite> seperateTileList;

    public GameObject ledgeGrabCollider;

    private Tilemap tilemap;

    private void Start () {

        if (cornerTilesList.Count == 0 && seperateTileList.Count == 0)
            return;

        AddLedgeGrabToTiles();
	}

    private void AddLedgeGrabToTiles()
    {
        tilemap = GetComponent<Tilemap>();
        GameObject holderObject = new GameObject();
        holderObject.name = "Ledge Collider Container";

        BoundsInt bounds = tilemap.cellBounds;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tilemap.CellToWorld(localPlace);

            if (CheckIfContainsSprite(localPlace))
            {
                Instantiate(ledgeGrabCollider, place, Quaternion.identity, holderObject.transform);
            }
        }
    }

    private bool CheckIfContainsSprite(Vector3Int cellPos)
    {
        Sprite spriteAtLoc = tilemap.GetSprite(cellPos);

        if (cornerTilesList.Count > 0)
            if (cornerTilesList.Contains(spriteAtLoc))
                return true;

        if (seperateTileList.Count > 0)
            if (seperateTileList.Contains(spriteAtLoc))
                return true;

        return false;
    }
}
