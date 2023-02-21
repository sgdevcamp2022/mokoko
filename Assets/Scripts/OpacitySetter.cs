using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
public class OpacitySetter : MonoBehaviour
{
    Tilemap tilemap;
    Vector3Int position;

    void Awake() {
        tilemap = GetComponent<Tilemap>();
        position = new Vector3Int(0, 0, 0);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        Vector3 center = collision.bounds.center;
        Vector3 min = collision.bounds.min;
        Vector3 max = collision.bounds.max;

        Vector3[] corners = {
            new Vector3(min.x, min.y, 0f), new Vector3(min.x, max.y, 0f), new Vector3(min.x, center.y, 0f),
            new Vector3(max.x, min.y, 0f), new Vector3(max.x, max.y, 0f), new Vector3(max.x, center.y, 0f),
        };

        if(collision.CompareTag("Player")) {
            foreach (Vector3 corner in corners) {
                Vector3Int tilePosition = tilemap.WorldToCell(corner);
                Debug.Log("Wfsfa");
                if(tilemap.HasTile(tilePosition)) {
                    tilemap.SetColor(tilePosition, new Color(1, 1, 1, 0.2f));
                    Debug.Log(tilemap.GetColor(tilePosition));
                }
            }

        }
    }
    
    void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "Player") {
            tilemap.SetColor(position, new Color(1, 1, 1, 1.0f));
        }
    }
}
