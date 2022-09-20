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
    
    /// <summary>
    /// Draws the map to the screen
    /// </summary>
    /// <param name="map">Map that we want to draw</param>
    /// <param name="tilemap">Tilemap we will draw onto</param>
    /// <param name="tile">Tile we will draw with</param>
    public static void RenderMap(int[,] map, Tilemap tilemap, TileBase tile, Vector2Int bound)
    {
	    var lengthDeletion = 5;
	    for (int x = 0; x < map.GetUpperBound(0) ; x++) //Loop through the width of the map
        {
            for (int y = 0; y < map.GetUpperBound(1); y++) //Loop through the height of the map
            {
	            if (map[x, y] == 1) // 1 = tile, 0 = no tile
	                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                else
	                tilemap.SetTile(new Vector3Int(x, y, 0), null);

	            if (y < bound.x && y > bound.y && x < lengthDeletion)
	            {
		            if(map[x + 1, bound.x] == 1 && map[x + 1, bound.y] == 1 && map[5, y] == 1)
		            {
			            lengthDeletion++;
		            }
		            tilemap.SetTile(new Vector3Int(x, y, 0), null);
	            }
            }
        }
    }
    
    /// <summary>
    /// Renders a map using an offset provided, Useful for having multiple maps on one tilemap
    /// </summary>
    /// <param name="map">The map to draw</param>
    /// <param name="tilemap">The tilemap to draw on</param>
    /// <param name="tile">The tile to draw with</param>
    /// <param name="offset">The offset to apply</param>
    public static void RenderMapWithOffset(int[,] map, Tilemap tilemap, TileBase tile, Vector2Int offset, int[] inputs)
    {
	    for (int y = 0; y < map.GetUpperBound(1); y++)
	    {
		    if(inputs[y] != 1) continue;
		    
		    var zeroTile = true;
		    for (int x = 1; x < 15; x++)
		    {
			    if (map[x, y] == 0)
			    {
				    if (map[x + 1, y] == 0 || map[x + 1, y + 1] == 0 || map[x + 1, y - 1] == 0 || map[x, y + 1] == 0 || map[x, y - 1] == 0)
				    {
					    zeroTile = true;
					    break;
				    }
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
		    }
	    }
    }

    /// <summary>
	/// Creates a cave using perlin noise for the generation process
	/// </summary>
	/// <param name="map">the map to be modified</param>
	/// <param name="modifier">the value we times the position by to get our perlin gen</param>
	/// <param name="edgesAreWalls">If set to <c>true</c> edges are walls.</param>
	/// <returns>The noise cave.</returns>
	public static int[,] PerlinNoiseCave(int[,] map, int intervalExit, float modifier, bool edgesAreWalls)
    {
        int newPoint;
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
	        for (int y = 0; y < map.GetUpperBound(1); y++)
	        {
		        //Generate a new point using perlin noise, then round it to a value of either 0 or 1
		        newPoint = Mathf.RoundToInt(Mathf.PerlinNoise(x * modifier, y * modifier));
		        map[x, y] = newPoint;
	        }
        }

        for (int y = 0; y < map.GetUpperBound(1); y++)
        {
	        var dontHaveCave = true;
	        for (int x = map.GetUpperBound(0) - intervalExit; x < map.GetUpperBound(0)-1; x++)
	        {
		        dontHaveCave = map[x, y] == 0;
		        if(!dontHaveCave) break;
	        }
	        if (edgesAreWalls && !dontHaveCave)
	        {
		        map[map.GetUpperBound(0) - 1, y] = 1;
	        }
        }

        return map;
    }

    /// <summary>
	/// Used to create a new cave using the Random Walk Algorithm. Doesn't exit out of bounds.
	/// </summary>
	/// <param name="map">The array that holds the map information</param>
	/// <param name="seed">The seed for the random</param>
	/// <param name="requiredFloorPercent">The amount of floor we want</param>
	/// <returns>The modified map array</returns>
	public static int[,] RandomWalkCave(int[,] map, float seed,  int requiredFloorPercent)
    {
        //Seed our random
        System.Random rand = new System.Random(seed.GetHashCode());

        //Define our start x position
        int floorX = rand.Next(1, map.GetUpperBound(0) - 1);
        //Define our start y position
        int floorY = rand.Next(1, map.GetUpperBound(1) - 1);
        //Determine our required floorAmount
        int reqFloorAmount = ((map.GetUpperBound(1) * map.GetUpperBound(0)) * requiredFloorPercent) / 100; 
        //Used for our while loop, when this reaches our reqFloorAmount we will stop tunneling
        int floorCount = 0;

        //Set our start position to not be a tile (0 = no tile, 1 = tile)
        map[floorX, floorY] = 0;
        //Increase our floor count
        floorCount++; 
        
        while (floorCount < reqFloorAmount)
        { 
            //Determine our next direction
            int randDir = rand.Next(4); 

            switch (randDir)
            {
                case 0: //Up
                    //Ensure that the edges are still tiles
                    if ((floorY + 1) < map.GetUpperBound(1) - 1) 
                    {
                        //Move the y up one
                        floorY++;

                        //Check if that piece is currently still a tile
                        if (map[floorX, floorY] == 1) 
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase floor count
                            floorCount++; 
                        }
                    }
                    break;
                case 1: //Down
                    //Ensure that the edges are still tiles
                    if ((floorY - 1) > 1)
                    { 
                        //Move the y down one
                        floorY--;
                        //Check if that piece is currently still a tile
                        if (map[floorX, floorY] == 1) 
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++; 
                        }
                    }
                    break;
                case 2: //Right
                    //Ensure that the edges are still tiles
                    if ((floorX + 1) < map.GetUpperBound(0) - 1)
                    {
                        //Move the x to the right
                        floorX++;
                        //Check if that piece is currently still a tile
                        if (map[floorX, floorY] == 1) 
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++; 
                        }
                    }
                    break;
                case 3: //Left
                    //Ensure that the edges are still tiles
                    if ((floorX - 1) > 1)
                    {
                        //Move the x to the left
                        floorX--;
                        //Check if that piece is currently still a tile
                        if (map[floorX, floorY] == 1) 
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++; 
                        }
                    }
                    break;
            }
        }
        //Return the updated map
        return map; 
    }

	/// <summary>
	/// EXPERIMENTAL 
	/// Generates a random walk cave but with the option to move in any of the 8 directions
	/// </summary>
	/// <param name="map">The map array to change</param>
	/// <param name="seed">The seed for the random</param>
	/// <param name="requiredFloorPercent">Required amouount of floor to remove</param>
	/// <returns>The modified map array</returns>
	public static int[,] RandomWalkCaveCustom(int[,] map, float seed,  int requiredFloorPercent)
    {
        //Seed our random
        System.Random rand = new System.Random(seed.GetHashCode());

        //Define our start x position
        int floorX = Random.Range(1, map.GetUpperBound(0) - 1);
        //Define our start y position
        int floorY = Random.Range(1, map.GetUpperBound(1) - 1);
        //Determine our required floorAmount
        int reqFloorAmount = ((map.GetUpperBound(1) * map.GetUpperBound(0)) * requiredFloorPercent) / 100;
        //Used for our while loop, when this reaches our reqFloorAmount we will stop tunneling
        int floorCount = 0;

        //Set our start position to not be a tile (0 = no tile, 1 = tile)
        map[floorX, floorY] = 0;
        //Increase our floor count
        floorCount++;

        while (floorCount < reqFloorAmount)
        {
            //Determine our next direction
            int randDir = rand.Next(8);

            switch (randDir)
            {
                case 0: //North-West
                    //Ensure we don't go off the map
                    if ((floorY + 1) < map.GetUpperBound(1) && (floorX -1) > 0)
                    {
                        //Move the y up 
                        floorY++;
                        //Move the x left
                        floorX--;

                        //Check if the position is a tile
                        if (map[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase floor count
                            floorCount++;
                        }
                    }
                    break;
                case 1: //North
                    //Ensure we don't go off the map
                    if ((floorY + 1) < map.GetUpperBound(1))
                    {
                        //Move the y up
                        floorY++;

                        //Check if the position is a tile
                        if (map[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;
                case 2: //North-East
                    //Ensure we don't go off the map
                    if ((floorY + 1) < map.GetUpperBound(1) && (floorX + 1) < map.GetUpperBound(0))
                    {
                        //Move the y up
                        floorY++;
                        //Move the x right
                        floorX++;

                        //Check if the position is a tile
                        if (map[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;
                case 3: //East
                    //Ensure we don't go off the map
                    if ((floorX + 1) < map.GetUpperBound(0))
                    {
                        //Move the x right
                        floorX++;

                        //Check if the position is a tile
                        if (map[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++; 
                        }
                    }
                    break;
                case 4: //South-East
                    //Ensure we don't go off the map
                    if((floorY -1) > 0 && (floorX + 1) < map.GetUpperBound(0))
                    {
                        //Move the y down
                        floorY--;
                        //Move the x right
                        floorX++;

                        //Check if the position is a tile
                        if(map[floorX,floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;
                case 5: //South
                    //Ensure we don't go off the map
                    if((floorY - 1) > 0)
                    {
                        //Move the y down
                        floorY--;

                        //Check if the position is a tile
                        if(map[floorX,floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;
                case 6: //South-West
                    //Ensure we don't go off the map
                    if((floorY - 1) > 0 && (floorX - 1) > 0)
                    {
                        //Move the y down
                        floorY--;
                        //move the x left
                        floorX--;

                        //Check if the position is a tile
                        if(map[floorX,floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;
                case 7: //West
                    //Ensure we don't go off the map
                    if((floorX - 1) > 0)
                    {
                        //Move the x left
                        floorX--;
                        
                        //Check if the position is a tile
                        if(map[floorX,floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;
            }
        }

        return map; 
    }
    
    /// <summary>
    /// Creates a tunnel of length height. Takes into account roughness and windyness
    /// </summary>
    /// <param name="map">The array that holds the map information</param>
    /// <param name="width">The width of the map</param>
    /// <param name="height">The height of the map</param>
    /// <param name="minPathWidth">The min width of the path</param>
    /// <param name="maxPathWidth">The max width of the path, ensure it is smaller than then width of the map</param>
    /// <param name="maxPathChange">The max amount we can change the center point of the path by</param>
    /// <param name="roughness">How much the edges of the tunnel vary</param>
    /// <param name="windyness">how much the direction of the tunnel varies</param>
    /// <returns>The map after being tunneled</returns>
    public static int[,] DirectionalTunnel(int[,] map, int minPathWidth, int maxPathWidth, int maxPathChange, int roughness, int windyness)
    {
		//This value goes from its minus counterpart to its positive value, in this case with a width value of 1, the width of the tunnel is 3
		int tunnelWidth = 1; 
		
		//Set the start X position to the center of the tunnel
		int x = map.GetUpperBound(0) / 2; 

		//Set up our seed for the random.
		System.Random rand = new System.Random(Time.time.GetHashCode()); 

		//Create the first part of the tunnel
		for (int i = -tunnelWidth; i <= tunnelWidth; i++) 
        {
            map[x + i, 0] = 0;
        }

		//Cycle through the array
		for (int y = 1; y < map.GetUpperBound(1); y++) 
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
				if (x > (map.GetUpperBound(0) - maxPathWidth)) 
                {
                    x = map.GetUpperBound(0) - maxPathWidth;
                }

            }

			//Work through the width of the tunnel
			for (int i = -tunnelWidth; i <= tunnelWidth; i++)
			{ 
                map[x + i, y] = 0;
            }
        }
        return map;
    }
    
    /// <summary>
    /// Creates the basis for our Advanced Cellular Automata functions.
    /// We can then input this map into different functions depending on
    /// what type of neighbourhood we want
    /// </summary>
    /// <param name="map">The array to be modified</param>
    /// <param name="seed">The seed we will use</param>
    /// <param name="fillPercent">The amount we want the map filled</param>
    /// <param name="edgesAreWalls">Whether we want the edges to be walls</param>
    /// <returns>The modified map array</returns>
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
                {
					//Set the cell to be active if edges are walls
                    map[x, y] = 1;
                }
                else
                {
					//Set the cell to be active if the result of rand.Next() is less than the fill percentage
                    map[x, y] = (rand.Next(0, 100) < fillPercent) ? 1 : 0; 
                }
            }
        }
        return map;
    }

    /// <summary>
    /// Smooths the map using the von Neumann Neighbourhood rules
    /// </summary>
    /// <param name="map">The map we will Smooth</param>
	/// <param name="edgesAreWalls">Whether the edges are walls or not</param>
	/// <param name="smoothCount">The amount we will loop through to smooth the array</param>
    /// <returns>The modified map array</returns>
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

    /// <summary>
    /// Gets the surrounding tiles using the von Neumann Neighbourhood rules. This neighbourhood only checks the direct neighbours, i.e. Up, Left, Down Right
    /// </summary>
    /// <param name="map">The map we are checking</param>
    /// <param name="x">The x position we are checking</param>
    /// <param name="y">The y position we are checking</param>
    /// <returns>The amount of neighbours the tile map[x,y] has</returns>
	static int GetVNSurroundingTiles(int[,] map, int x, int y, bool edgesAreWalls)
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

    /// <summary>
    /// Smoothes a map using Moore's Neighbourhood Rules. Moores Neighbourhood consists of all neighbours of the tile, including diagonal neighbours
    /// </summary>
    /// <param name="map">The map to modify</param>
    /// <param name="edgesAreWalls">Whether our edges should be walls</param>
    /// <param name="smoothCount">The amount we will loop through to smooth the array</param>
    /// <param name="outputs">For delete tile where input</param>
    /// <returns>The modified map</returns>
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

	/// <summary>
    /// Gets the surrounding amount of tiles using the Moore Neighbourhood
    /// </summary>
    /// <param name="map">The map to check</param>
    /// <param name="x">The x position we are checking</param>
    /// <param name="y">The y position we are checking</param>
    /// <param name="edgesAreWalls">Whether the edges are walls</param>
    /// <returns>An int with the amount of surrounding tiles</returns>
    static int GetMooreSurroundingTiles(int[,] map, int x, int y)
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
			map[map.GetUpperBound(0) - 1, y] = 2;
			outputs[y] = 1;
		}
		return map;
	} 
}