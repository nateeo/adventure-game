#pragma strict
 
 public var terrain : Terrain;
 
 public var minScale : float = 0.09842519;
 public var maxScale : float = 0.1;
 
 #if UNITY_EDITOR
 @ContextMenu( "Scale Existing Trees" )
 function ScaleExistingTrees() 
 {
     // check if there is no terrain in the inspector, then use the current active terrain
     if ( !terrain )
     {
         terrain = Terrain.activeTerrain;
     }
     
     // check if maxScale is less than minScale
     if ( maxScale < minScale )
         maxScale = minScale;
     
     // variable for storing random calculated scale
     var rndScale : float;
     
     // store reference of all the current trees
     var treeInstances : TreeInstance[] = terrain.terrainData.treeInstances;
     
     // cycle through each tree
     for ( var t : int = 0; t < treeInstances.length; t ++ )
     {
         // calculate random scale
         rndScale = Random.Range( minScale, maxScale );
         
         // apply to curent tree in array
         treeInstances[t].heightScale = rndScale;
         treeInstances[t].widthScale = rndScale;
     }
     
     // apply treeInstances back to terrain data
     terrain.terrainData.treeInstances = treeInstances;
     
     Debug.Log( "Scaling Trees Complete" );
 }
 #endif