using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapFunctions
{
    /// <summary>
    /// Generates an int array of the supplied width and height
    /// </summary>
    /// <param name="width">How wide you want the array</param>
    /// <param name="height">How high you want the array</param>
    /// <returns>The map array initialised</returns>
    public static int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (empty)
                {
                    map[x, y] = 0;
                }
                else
                {
                    map[x, y] = 1;
                }
            }
        }
        return map;
    }

    public static int[,] ClearEnter(int[,] map, Vector2Int bound)
    {
	    var lengthDeletion = 5;
	    for (int x = 0; x < map.GetUpperBound(0); x++) //Loop through the width of the map
	    {
		    for (int y = 0; y < map.GetUpperBound(1); y++) //Loop through the height of the map
		    {
			    if (y < bound.x && y > bound.y && x < lengthDeletion)
			    {
				    if (map[x + 1, bound.x] == 1 && map[x + 1, bound.y] == 1 && map[5, y] == 1)
					    lengthDeletion = Mathf.Clamp(lengthDeletion + 1, 5, 20);

				    map[x, y] = 0;
			    }
		    }
	    }

	    return map;
    }
    
    public static IEnumerator RenderMapCoroutine(int[,] map, Tilemap tilemap, TileBase tile)
    {
	    var lengthDeletion = 5;
	    for (int x = 0; x < map.GetUpperBound(0); x++) //Loop through the width of the map
	    {
		    for (int y = 0; y < map.GetUpperBound(1); y++) //Loop through the height of the map
		    {
			    if (map[x, y] == 1) // 1 = tile, 0 = no tile
				    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
			    else
				    tilemap.SetTile(new Vector3Int(x, y, 0), null);

			    //yield return null;
		    } 
		    yield return null;
	    }
    }
    
    public static IEnumerator RenderMapWithOffset(int[,] map, Tilemap tilemap, TileBase tile, Vector2Int offset, int[] inputs)
    {
	    for (int y = 0; y < map.GetUpperBound(1); y++)
	    {
		    if(inputs[y] != 1) continue;
		    
		    var zeroTile = true;
		    var count = 15;
		    for (int x = 1; x < count; x++)
		    {
			    if (map[x, y] == 1)
			    {
				    if (map[x + 1, y] == 0 || map[x + 1, y + 1] == 0 || map[x + 1, y - 1] == 0 || map[x, y + 1] == 0 || map[x, y - 1] == 0)
				    {
					    zeroTile = true;
					    break;
				    }

				    count++;
			    }
			    zeroTile = false;
		    }
	    
		    if (!zeroTile) inputs[y] = 0;
	    }
	    
	    for (int x = 0; x < map.GetUpperBound(0); x++)
	    {
		    for (int y = 0; y < map.GetUpperBound(1); y++)
		    {
			    if (inputs[y] == 1 && x < 15)
				    map[x, y] = 0;
			    
			    if(map[x,y] == 1)
				    tilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y ,0), tile);
			    //yield return null;
		    }
		    yield return null;
	    }
    }
    
	public static int[,] PerlinNoiseCave(int[,] map, int intervalExit, float modifier, bool edgesAreWalls, ref int[] outputs)
    {
        int newPoint;
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
	        for (int y = 0; y < map.GetUpperBound(1); y++)
	        {
		        if (edgesAreWalls && (x == 0 || y == 0 || x == map.GetUpperBound(0) - 1 || y == map.GetUpperBound(1) - 1))
			        map[x, y] = 1;
		        else
		        {
			        //Generate a new point using perlin noise, then round it to a value of either 0 or 1
			        newPoint = Mathf.RoundToInt(Mathf.PerlinNoise(x * modifier, y * modifier));
			        map[x, y] = newPoint;
		        }
	        }
        }
        
        if (edgesAreWalls) map = CalculateEdges(map, intervalExit, ref outputs);

        return map;
    }
	
    public static int[,] DirectionalTunnel(int[,] map, int minPathWidth, int maxPathWidth, int maxPathChange, int roughness, int windyness)
    {
		//This value goes from its minus counterpart to its positive value, in this case with a width value of 1, the width of the tunnel is 3
		int tunnelWidth = 1; 
		
		//Set the start X position to the center of the tunnel
		int x = map.GetUpperBound(1) / 2; 

		//Set up our seed for the random.
		System.Random rand = new System.Random(Time.time.GetHashCode()); 

		//Create the first part of the tunnel
		for (int i = -tunnelWidth; i <= tunnelWidth; i++) 
        {
            map[0, x + i] = 0;
        }

		//Cycle through the array
		for (int y = 1; y < map.GetUpperBound(0); y++) 
        {
			//Check if we can change the roughness
			if (rand.Next(0, 100) > roughness) 
            {

				//Get the amount we will change for the width
				int widthChange = Random.Range(-maxPathWidth, maxPathWidth); 
                tunnelWidth += widthChange;

				//Check to see we arent making the path too small
				if (tunnelWidth < minPathWidth) 
                {
                    tunnelWidth = minPathWidth;
                }

				//Check that the path width isnt over our maximum
				if (tunnelWidth > maxPathWidth) 
                {
                    tunnelWidth = maxPathWidth;
                }
            }

			//Check if we can change the windyness
			if (rand.Next(0, 100) > windyness) 
            {
				//Get the amount we will change for the x position
				int xChange = Random.Range(-maxPathChange, maxPathChange);
                x += xChange;

				//Check we arent too close to the left side of the map
				if (x < maxPathWidth) 
                {
                    x = maxPathWidth;
                }
				//Check we arent too close to the right side of the map
				if (x > (map.GetUpperBound(1) - maxPathWidth)) 
                {
                    x = map.GetUpperBound(1) - maxPathWidth;
                }

            }

			//Work through the width of the tunnel
			for (int i = -tunnelWidth; i <= tunnelWidth; i++)
			{ 
                map[y, x + i] = 0;
            }
        }
        return map;
    }
    
    public static int[,] GenerateCellularAutomata(int width, int height, float seed, int fillPercent, bool edgesAreWalls)
    {
		//Seed our random number generator
		System.Random rand = new System.Random(seed.GetHashCode()); 

		//Set up the size of our array
        int[,] map = new int[width, height];

		//Start looping through setting the cells.
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (edgesAreWalls && (x == 0 || x == map.GetUpperBound(0) - 1 || y == map.GetUpperBound(1) - 1))
	                map[x, y] = 1;
                else
                    map[x, y] = (rand.Next(0, 100) < fillPercent) ? 1 : 0;
            }
        }
        return map;
    }
    
    public static int[,] SmoothVNCellularAutomata(int[,] map, int intervalExit, bool edgesAreWalls, int smoothCount)
    {
		for (int i = 0; i < smoothCount; i++)
		{
			for (int x = 0; x < map.GetUpperBound(0); x++)
			{
				for (int y = 0; y < map.GetUpperBound(1); y++)
				{
					//Get the surrounding tiles
					int surroundingTiles = GetVNSurroundingTiles(map, x, y, edgesAreWalls);

					if (edgesAreWalls && (x == 0 || x == map.GetUpperBound(0) - 1 || y == map.GetUpperBound(1)-1))
					{
						map[x, y] = 1; //Keep our edges as walls
					}
					//von Neuemann Neighbourhood requires only 3 or more surrounding tiles to be changed to a tile
					else if (surroundingTiles > 2) 
					{
						map[x, y] = 1;
					}
					//If we have less than 2 neighbours, set the tile to be inactive
					else if (surroundingTiles < 2)
					{
						map[x, y] = 0;
					}
					//Do nothing if we have 2 neighbours
				}
			}
		}
		var outputs = new int[map.GetUpperBound(1)];
		if (edgesAreWalls) map = CalculateEdges(map, intervalExit, ref outputs);
		
        return map;
    }
    
    private static int GetVNSurroundingTiles(int[,] map, int x, int y, bool edgesAreWalls)
	{
		/* von Neumann Neighbourhood looks like this ('T' is our Tile, 'N' is our Neighbour)
		* 
		*   N 
		* N T N
		*   N
		*   
		*/
         
		int tileCount = 0;

		//If we are not touching the left side of the map
		if (x - 1 > 0) 
		{
			tileCount += map[x - 1, y];
		}
		else if(edgesAreWalls)
		{
			tileCount++;
		}

		//If we are not touching the bottom of the map
		if (y - 1 > 0) 
		{
			tileCount += map[x, y - 1];
		}
		else if(edgesAreWalls)
		{
			tileCount++;
		}

		//If we are not touching the right side of the map
		if (x + 1 < map.GetUpperBound(0)) 
		{
			tileCount += map[x + 1, y];
		}
		else if (edgesAreWalls)
		{
			tileCount++;
		}

		//If we are not touching the top of the map
		if (y + 1 < map.GetUpperBound(1)) 
		{
			tileCount += map[x, y + 1];
		}
		else if (edgesAreWalls)
		{
			tileCount++;
		}

		return tileCount;
	}
    
    public static int[,] SmoothMooreCellularAutomata(int[,] map, int intervalExit, bool edgesAreWalls, int smoothCount, ref int[] outputs)
	{
		for (int i = 0; i < smoothCount; i++)
		{
			for (int x = 0; x < map.GetUpperBound(0); x++)
			{
				for (int y = 0; y < map.GetUpperBound(1); y++)
				{
					int surroundingTiles = GetMooreSurroundingTiles(map, x, y);

					//Set the edge to be a wall if we have edgesAreWalls to be true
					if (edgesAreWalls && (x == 0 || x == (map.GetUpperBound(0) - 1) || y == map.GetUpperBound(1) - 1))
					{
						map[x, y] = 1; 
					}
					//If we have more than 4 neighbours, change to an active cell
					else if (surroundingTiles > 4)
					{
						map[x, y] = 1;
					}
					//If we have less than 4 neighbours, change to be an inactive cell
					else if (surroundingTiles < 4)
					{
						map[x, y] = 0;
					}
				}
			}
		}

		if (edgesAreWalls) map = CalculateEdges(map, intervalExit, ref outputs);
		
        return map;
    }
    
	private static int GetMooreSurroundingTiles(int[,] map, int x, int y)
    {
        /* Moore Neighbourhood looks like this ('T' is our tile, 'N' is our neighbours)
         * 
         * N N N
         * N T N
         * N N N
         * 
         */

        int tileCount = 0;       
        
		//Cycle through the x values
        for(int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
			//Cycle through the y values
            for(int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < map.GetUpperBound(0) && neighbourY >= 0 && neighbourY < map.GetUpperBound(1))
				{
					//We don't want to count the tile we are checking the surroundings of
					if (neighbourX != x || neighbourY != y) 
                    {
                        tileCount += map[neighbourX, neighbourY];
                    }
                }
            }
        }
        return tileCount;
    }

	private static int[,] CalculateEdges(int[,] map, int intervalExit, ref int[] outputs)
	{
		for (int y = 0; y < map.GetUpperBound(1); y++)
		{
			var dontHaveCave = true;
			for (int x = map.GetUpperBound(0) - intervalExit; x < map.GetUpperBound(0) - 1; x++)
			{
				dontHaveCave = map[x, y] == 0;
				if(!dontHaveCave) break;
			}

			if (!dontHaveCave) continue;
			map[map.GetUpperBound(0) - 1, y] = 0;
			outputs[y] = 1;
		}
		return map;
	} 
}