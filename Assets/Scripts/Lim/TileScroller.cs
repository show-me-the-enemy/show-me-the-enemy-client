using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileScroller : MonoBehaviour
{
    [field: SerializeField] private float Choke { get; set; } = 16f;

    private Transform _cameraTransform;

    private Vector3 _lastCameraPosition;

    private Tilemap _tilemap;

    [field: SerializeField]
    private int boundWidth = 62;
    [field: SerializeField]
    private int borderLeft;
    private int borderRight;

    [field: SerializeField]
    private int shiftBufferLines = 3;
    private int bufferWidth;
    [field: SerializeField]
    private int bufferLeft;
    private int bufferRight;
    private int bufferTop;
    private int bufferDown;

    public void Init()
    {
        Camera mainCamera = Camera.main;
        _cameraTransform = mainCamera.transform;
        _lastCameraPosition = _cameraTransform.position;

        _tilemap = GetComponent<Tilemap>();
        _tilemap.CompressBounds();

        int cx = _tilemap.WorldToCell(_lastCameraPosition).x;
        borderLeft = cx - boundWidth / 2;
        borderRight = cx + boundWidth / 2;

        BoundsInt tileSize = _tilemap.cellBounds;
        bufferLeft = tileSize.position.x;
        bufferRight = tileSize.position.x + tileSize.size.x;
        bufferTop = tileSize.position.y;
        bufferDown = tileSize.position.y + tileSize.size.y;
    }
    public void AdvanceTime(float dt_sec)
    {
        Vector3 dtPos = _cameraTransform.position - _lastCameraPosition;
        if (dtPos.x == 0)
            return;

        int cx = _tilemap.WorldToCell(_cameraTransform.position).x;
        borderLeft = cx - boundWidth / 2;
        borderRight = cx + boundWidth / 2;

        if (bufferLeft > borderLeft)
            shiftBuffer(-1);
        else if (bufferRight < borderRight)
            shiftBuffer(1);

        _lastCameraPosition = _cameraTransform.position;
    }

    private void shiftBuffer(int shiftDir)
    {
        for (int i = 0; i < shiftBufferLines; i++)
        {
            for (int j = bufferTop; j < bufferDown; j++)
            {
                if (shiftDir > 0)
                {
                    _tilemap.SetTile(new Vector3Int(bufferRight + i, j, 0), _tilemap.GetTile(new Vector3Int(bufferLeft + i + 1, j, 0)));
                    _tilemap.SetTile(new Vector3Int(bufferLeft + i + 1, j, 0), null);
                }
                else
                {
                    _tilemap.SetTile(new Vector3Int(bufferLeft - i, j, 0), _tilemap.GetTile(new Vector3Int(bufferRight - i - 1, j, 0)));
                    _tilemap.SetTile(new Vector3Int(bufferRight - i - 1, j, 0), null);
                }
            }
        }

        if (shiftDir > 0)
        {
            bufferRight += shiftBufferLines;
            bufferLeft += shiftBufferLines;
        }
        if (shiftDir < 0)
        {
            bufferLeft -= shiftBufferLines;
            bufferRight -= shiftBufferLines;
        }

    }
}
