using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newtileatlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    public TileClass log;
    public TileClass leaf;
    public TileClass lunarCrust;
    public TileClass lunarSoil;
    public TileClass lunarRock;
    public TileClass moonRock;

    public TileClass coal;
    public TileClass iron;
    public TileClass gold;
    public TileClass diamond;
}
