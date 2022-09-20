public class BuildingFabric
{
    private BuildingConfig[] _platforms;

    public BuildingFabric(BuildingConfig[] platforms) => _platforms = platforms;

    public BuildingConfig GetBuilding(string name)
    {
        for (var i = 0; i < _platforms.Length; i++)
        {
            if (_platforms[i].Name == name)
                return _platforms[i];
        }

        return null;
    }
}

public class TileFabric
{
    private TileConfig[] _tile;

    public TileFabric(TileConfig[] tile) => _tile = tile;

    public TileConfig GetTile(string name)
    {
        for (var i = 0; i < _tile.Length; i++)
        {
            if (_tile[i].Name == name)
                return _tile[i];
        }

        return null;
    }
}