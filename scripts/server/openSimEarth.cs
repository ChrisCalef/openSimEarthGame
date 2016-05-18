

//=============================================================================
//
//                 OPEN SIM EARTH
//
//=============================================================================
$numScenes = 0;

function startSQL(%dbname)
{//Create the sqlite object that we will use in all the scripts.
   %sqlite = new SQLiteObject(sqlite);
   
   if (%sqlite.openDatabase(%dbname))
      echo("Successfully opened database: " @ %dbname );
   else {
      echo("We had a problem involving database: " @ %dbname );
      return;
   }
}

   //TESTING - SpatiaLite.  Exciting promise, disappointing failure... so far.
   //if (%sqlite.openDatabase("testDB.db"))
   //{
   //   echo("Successfully opened database: testDB.db" );
      //%query = "INSERT INTO testTable ( name, geom ) VALUES ('Test01',GeomFromText('POINT(1 2)'));";
      //%query = "";
      //%result = sqlite.query(%query, 0);
      //if (%result)
      //   echo("spatialite inserted into a table with a geom!");
      //else
      //   echo("spatialite failed to insert into a table with a geom!  "   );
   //   %sqlite.closeDatabase();
   //}   
   //NOW... apparently all we have to do is this, to gain access to all of SpatiaLite.
   //%query = "SELECT load_extension('libspatialite-2.dll');";
   //%result = sqlite.query(%query, 0);
   //echo( "Loaded SpatiaLite: " @ %result );
   //Except, maybe have to do this in the engine.
   
function stopSQL()
{
   sqlite.closeDatabase();
   sqlite.delete();      
}

function openSimEarthTick()
{
   if (($numScenes==0)&&($pref::opensimearth::autoLoadScenes)) //first time through, unless DB is missing or corrupt.
   {
      %query = "SELECT s.id,p.x AS pos_x,p.y AS pos_y,p.z AS pos_z " @
               "FROM scene s LEFT JOIN vector3 p ON p.id=s.pos_id;";
      %result = sqlite.query(%query, 0);
      echo("query: " @ %query);
      %i=0;
      if (%result)
      {	   
         while (!sqlite.endOfResult(%result))
         {
            %id = sqlite.getColumn(%result, "id");     
            %x = sqlite.getColumn(%result, "pos_x");
            %y = sqlite.getColumn(%result, "pos_y");
            %z = sqlite.getColumn(%result, "pos_z");
            //DatabaseSceneList.add(%name,%id);
            echo("scene " @ %id  @ " " @ %x @ " " @ %y @ " " @ %z);
            
            $scenePos[%i] = %x @ " " @ %y @ " " @ %z;
            $sceneId[%i] = %id;
            $sceneLoaded[%i] = false;
            $sceneDist[%i] = 5.0;//TEMP, add this to scenes table
            
            %i++;
            sqlite.nextRow(%result);
         }
      } 
      $numScenes = %i;
      echo("Num scenes: " @ %numScenes);
   }
   sqlite.clearResult(%result);
   
   if (($myPlayer)&&($pref::opensimearth::autoLoadScenes))
   {
      %pos = $myPlayer.getPosition();
      for (%i=0;%i<$numScenes;%i++)
      {
         %diff = VectorSub(%pos,$scenePos[%i]);
         
         if ((VectorLen(%diff)<$sceneDist[%i])&&($sceneLoaded[%i]==false))
         {
            loadScene($sceneId[%i]);
            $sceneLoaded[%i] = true;
         } 
         else if ((VectorLen(%diff)>$sceneDist[%i]*20)&&($sceneLoaded[%i]==true))//*20 completely arbitrary
         {
            unloadScene($sceneId[%i]);
            $sceneLoaded[%i] = false;              
         }
           
      }
      //echo("player position: " @ %pos );
   }
   
   schedule(60,0,"openSimEarthTick");

}
   
function PhysicsShape::onAdd(%this)
{
   echo("physics shape calling onAdd!!!!!!!!!!!!!!!!!! datablock " @ %this.dataBlock.getName() );
}


///////////////////////////////////////////////////////////////////////////////////////
//MOVE: probably these should go under tools/openSimEarth, if not obsoleted entirely.
function PhysicsShape::onStartup(%this)
{
   echo(%this @ " calling onStartup! position " @ %this.getPosition() @ " datablock " @ %this.dataBlock.getName());
   
   if (%this.dataBlock $= "M4Physics")
   {
      
      %this.setNavMesh("Nav");
      
      %this.setActionSeq("ambient","ambient");//This might not always be idle, could be just breathing
      %this.setActionSeq("idle","ambient");// and idle could be that plus fidgeting, etc.
      %this.setActionSeq("walk","walk");
      if (getRandom()>0.4)
         %this.setActionSeq("run","run");
      else 
         %this.setActionSeq("run","runscerd");
         
      %this.setActionSeq("runscerd","runscerd");
      %this.setActionSeq("fall","runscerd");
      %this.setActionSeq("getup","rSideGetup");   
      %this.setActionSeq("attack","power_punch_down");
      %this.setActionSeq("block","punch_uppercut");//TEMP, don't have any blocking anims atm
      
      //%this.setIsRecording(true);
      
      %this.currentAction = "walk";
      
      %this.groundMove();
      
      
      echo("starting up a M4 physics shape!");      
   } 
   else if (%this.dataBlock $= "bo105Physics") //useDataSource: holding off on this for now.
   {
      %this.setName("bo105");
      //%this.schedule(500,"setUseDataSource",true);
      //%this.setIsRecording(true);
      //%this.showNodes();     
   } 
   else if (%this.dataBlock $= "dragonflyPhysics") 
   {
      %this.setName("dragonfly");
      //%this.schedule(500,"setUseDataSource",true);
   }
   else if (%this.dataBlock $= "ka50Physics") 
   { 
      %this.setName("ka50");
      //%this.schedule(500,"setUseDataSource",true);
      %this.schedule(500,"showRotorBlades");
      //%this.showNodes();   
   }
}

function PhysicsShape::openSteerVehicle(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.createVehicle(%clientShape.getPosition(),0);  
   
   %clientShape.setOpenSteerMaxForce(3.0);
   %clientShape.setOpenSteerMaxSpeed(5.0);
}

function PhysicsShape::orientTo(%this, %dest)
{
   %pos = isObject(%dest) ? %dest.getPosition() : %dest;
   
   %this.orientToPos(%pos);
}

function PhysicsShape::moveTo(%this, %dest, %slowDown)
{
   %pos = isObject(%dest) ? %dest.getPosition() : %dest;
   
   
   //%this.say("moving to " @ %pos);  //say() prints to chat gui instead of console .
   //echo(%this.getId() @ " moving to " @ %pos);//This goes to the console.
   
   %this.orientToPos(%pos);
   
   %this.actionSeq(%this.currentAction);
   //%obj.atDestination = false;
}

function PhysicsShape::say(%this, %message)//Testing, does this only work for AIPlayers?
{
   chatMessageAll(%this, '\c3%1: %2', %this.getid(), %message);  
}

///////////////////////////////////////////////////////////////////////////////////////

//MOVE: these should be in a behaviorTrees folder, or at least a single file.
function onStartup::precondition(%this, %obj)
{
   if (%obj.startedUp != true)
      return true;
   else
      return false;
}

function onStartup::behavior(%this, %obj)
{
   //echo("calling onStartup!");   
   
   //Temp, store these in DB by shape and/or sceneShape
   %obj.setAmbientSeqByName("ambient");
   %obj.setIdleSeqByName("ambient");
   %obj.setWalkSeqByName("ambient");
   %obj.setRunSeqByName("run");
   %obj.setAttackSeqByName("power_punch_down");
   %obj.setBlockSeqByName("tpose");//TEMP, need block seq
   %obj.setFallSeqByName("ambient");
   %obj.setGetupSeqByName("rSideGetup");
   //Possibly these should not be named actions but should all be included in 
   //a sequenceActions table so it can be infinitely expanded.
   
   //Should this be automatic here, 
   %obj.groundMove();
   //or wait until we find out if we're more than just a ragdoll?
   
   %obj.startedUp = true;
   
   return SUCCESS;   
}

////////////// BEHAVIORS ///////////////////////////////

///////////////////////////////////
//[behaviorName]::precondition()
//[behaviorName]::onEnter()
//[behaviorName]::onExit()

//Do a raycast, either torque or physx, and find the ground directly below me.
//if below some threshold, then just move/interpolate us there. If above that, go to
//falling animation and/or ragdoll until we hit the ground and stop, then go to getUp task.

/* // No longer necessary... this is now done during processTick.
function goToGround::behavior(%this, %obj)
{
   %start = VectorAdd(%obj.position,"0 0 1.0");//Add a tiny bit (or, a huge amount)
                // so we don't get an error when we're actually on the ground.
                
   %contact = physx3CastGroundRay(%start);
   
   %obj.setPosition(%contact);
   echo(%this @ " is going to ground!!!!!!");
   %obj.setAmbientSeqByName("ambient");
   %obj.setIdleSeqByName("ambient");
   %obj.setWalkSeqByName("walk");
   %obj.setRunSeqByName("run");
   %obj.setAttackSeqByName("power_punch_down");
   %obj.setBlockSeqByName("tpose");
   %obj.setFallSeqByName("ambient");
   %obj.setGetupSeqByName("rSideGetup");
   
   return SUCCESS;
}
*/

///////////////////////////////////
//getUp::precondition()
//getUp::onEnter()
//getUp::onExit()

function getUp::behavior(%this, %obj)
{
   
   return SUCCESS;
}

///////////////////////////////////
//moveToPosition::precondition()
//moveToPosition::onEnter()
//moveToPosition::onExit()

function moveToPosition::behavior(%this, %obj)
{
   //echo("calling move to position!");
   %obj.groundMove();
   return SUCCESS;   
}

//////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////
/*
function openSimEarthGUIs()
{
   //Eventually load up all OpenSimEarth-related GUI objects from the DB.
   //For now this just fills the DatabaseSceneList dropdown.
   %query = "SELECT id,name,description from scene;";  
	%result = sqlite.query(%query, 0);
   if (%result)
   {	   
      while (!sqlite.endOfResult(%result))
      {
         %id = sqlite.getColumn(%result, "id");        
         %name = sqlite.getColumn(%result, "name");
         %descrip = sqlite.getColumn(%result, "description"); 
         
         //DatabaseSceneList.add(%name,%id);
         
         sqlite.nextRow(%result);
      }
   } 
   sqlite.clearResult(%result);
}*/

///////////////////////////////////////////////////////////////////////////////////
// REFACTOR - Still figuring out what goes with MegaMotion vs openSimEarth.

function osePullSceneShapes(%simGroup)
{//So, here we need to remove objects from the MissionGroup and put them into another simGroup.
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i);  
      %simGroup.add(%obj);
   }
   for (%i = 0; %i < %simGroup.getCount();%i++)
      MissionGroup.remove(%simGroup.getObject(%i));
}

function osePullSceneShapesAndSave(%simGroup)
{
   MegaMotionSaveSceneShapes();
   
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i);  
      %simGroup.add(%obj);
   }
   
   for (%i = 0; %i < %simGroup.getCount();%i++)
   {
      MissionGroup.remove(%simGroup.getObject(%i));
   }
}

function osePushSceneShapes(%simGroup)
{
   for (%i = 0; %i < %simGroup.getCount();%i++)
   {
      MissionGroup.add(%simGroup.getObject(%i));
   }
}

function osePullStatics(%simGroup)
{//So, here we need to remove objects from the MissionGroup and put them into another simGroup.
   for (%i = 0; %i < MissionGroup.getCount();%i++)
   {
      %obj = MissionGroup.getObject(%i);  
      if (%obj.getClassName()$="TSStatic")
         %simGroup.add(%obj);
   }
   for (%i = 0; %i < %simGroup.getCount();%i++)
      MissionGroup.remove(%simGroup.getObject(%i));
}

function osePullStaticsAndSave(%simGroup)
{   
   if (isDefined(theTP))   
      theTP.saveStaticShapes();
   
   for (%i = 0; %i < MissionGroup.getCount();%i++)
   {
      %obj = MissionGroup.getObject(%i);  
      if (%obj.getClassName()$="TSStatic")
      {
         %simGroup.add(%obj);
      }
   }
   
   for (%i = 0; %i < %simGroup.getCount();%i++)
   {
      MissionGroup.remove(%simGroup.getObject(%i));
   }
}

function osePushStatics(%simGroup)
{
   for (%i = 0; %i < %simGroup.getCount();%i++)
   {
      MissionGroup.add(%simGroup.getObject(%i));
   }
}

function osePullRoads(%simGroup)
{//So, here we need to remove objects from the MissionGroup and put them into another simGroup.
   for (%i = 0; %i < MissionGroup.getCount();%i++)
   {
      %obj = MissionGroup.getObject(%i);  
      if (%obj.getClassName()$="DecalRoad")// and/or MeshRoad
         %simGroup.add(%obj);
   }
   for (%i = 0; %i < %simGroup.getCount();%i++)
      MissionGroup.remove(%simGroup.getObject(%i));
}

function osePullRoadsAndSave(%simGroup)
{   
   theTP.saveRoads();
   
   for (%i = 0; %i < MissionGroup.getCount();%i++)
   {
      %obj = MissionGroup.getObject(%i);  
      if (%obj.getClassName()$="DecalRoad")// and/or MeshRoad
         %simGroup.add(%obj);
   }
   for (%i = 0; %i < %simGroup.getCount();%i++)
      MissionGroup.remove(%simGroup.getObject(%i));
}

function osePushRoads(%simGroup)
{
   for (%i = 0; %i < %simGroup.getCount();%i++)
   {
      MissionGroup.add(%simGroup.getObject(%i));
   }
}


function assignBehaviors()
{//This seems arbitrary, store initial behavior tree and dynamic status in sceneShape instead.
      
   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i);  
      
      %shape.setBehavior("baseTree");
      
      //%shape.setDynamic(1);       
   }   
}

function startRecording()
{
   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i);  
      %shape.setIsRecording(true);
   }   
}

function stopRecording()
{
   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i);  
      %shape.setIsRecording(false);
   }   
}

function makeSequences()
{
   //OKAY... here we go. We now need to:
   // a) find our model's home directory   
   // b) in that directory, create a new directory with a naming protocol
   //       "scene_[%scene_id].[timestamp]"?
   // c) fill it with sequences
   
   //For now, just "workSeqs", if name changes we'll have to update M4.cs every time.
   %dirPath = %shape.getPath() @ "/scenes";
   createDirectory(%dirPath);//make shape/scenes folder first, if necessary.
   %dirPath = %shape.getPath() @ "/scenes/" @ %shape.sceneID ;//then make specific scene folder.
   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i);  
      %dirPath = %shape.getPath() @ "/scenes/" @ %shape.sceneID ;
      %shape.makeSequence(%dirPath @ "/" @ %shape.getSceneShapeID());
   }
}

function loadOSM()  // OpenStreetMap XML data
{
   //here, read lat/long for each node as we get to it, convert it to xyz coords,
   //and save it in an array, to be used in the DecalRoad declaration.    
   
   %beforeTime = getRealTime();
   
   theTP.loadOSM($pref::OpenSimEarth::OSM,$pref::OpenSimEarth::MapDB);     
   //theTP.loadOSM("min.osm");     
   //theTP.loadOSM("kincaid_map.osm");  
   //theTP.loadOSM("central_south_eug.osm");  
   //theTP.loadOSM("thirtieth_map.osm");
   //theTP.loadOSM("all_eugene.osm");  
   
   %loadTime = getRealTime() - %beforeTime;
   echo("OpenStreetMap file load time: " @ %loadTime );
}

function makeStreets()
{
   %mapDB = new SQLiteObject();
   %dbname = $pref::OpenSimEarth::MapDB;//HERE: need to find this in prefs or something.
   %result = %mapDB.openDatabase(%dbname);
   //echo("tried to open osmdb: " @ %result);
   
   %query = "SELECT osmId,type,name FROM osmWay;";  
	%result = %mapDB.query(%query, 0);
   if (%result)
   {	   
      while (!%mapDB.endOfResult(%result))
      {
         %wayId = %mapDB.getColumn(%result, "osmId");
         %wayType = %mapDB.getColumn(%result, "type");         
         %wayName = %mapDB.getColumn(%result, "name");
         echo("found a way: " @ %wayName @ " id " @ %wayId);
         if ((%wayType $= "residential")||
               (%wayType $= "tertiary")||
               (%wayType $= "trunk")||
               (%wayType $= "trunk_link")||
               (%wayType $= "motorway")||
               (%wayType $= "motorway_link")||
               (%wayType $= "service")||
               (%wayType $= "footway")||
               (%wayType $= "path")||
               (%wayType $= "track"))
         {   
            
            //Width
            %roadWidth = 10.0;       
            if ((%wayType $= "tertiary")||(%wayType $= "trunk_link"))
               %roadWidth = 18.0; 
            else if ((%wayType $= "trunk")||(%wayType $= "motorway_link"))
               %roadWidth = 32.0; 
            else if (%wayType $= "motorway")
               %roadWidth = 40.0; 
            else if (%wayType $= "footway")
               %roadWidth = 2.5; 
            else if ((%wayType $= "path")||(%wayType $= "track"))
               %roadWidth = 5.0; 
            
            //Material
            %roadMaterial = "DefaultDecalRoadMaterial";
            if (%wayType $= "footway")
               %roadMaterial = "DefaultRoadMaterialPath";
            else if ((%wayType $= "service")||(%wayType $= "path"))
               %roadMaterial = "DefaultRoadMaterialOther";
               
            //now, query the osmWayNode and osmNode tables to get the list of points
            %node_query = "SELECT wn.nodeId,n.latitude,n.longitude,n.type,n.name from " @ 
                           "osmWayNode wn JOIN osmNode n ON wn.nodeId = n.osmId " @
                           "WHERE wn.wayID = " @ %wayId @ ";";
            %result2 = %mapDB.query(%node_query, 0);
            if (%result2)
            {	   
               //echo("query2 results: " @ mapDB.numRows(%result2));
               %nodeString = "";
               while (!%mapDB.endOfResult(%result2))
               {
                  %nodeId = %mapDB.getColumn(%result2, "nodeId");
                  %latitude = %mapDB.getColumn(%result2, "latitude");
                  %longitude = %mapDB.getColumn(%result2, "longitude");
                  %pos = theTP.convertLatLongToXYZ(%longitude @ " " @ %latitude @ " 0.0");
                  %type = %mapDB.getColumn(%result2, "type");         
                  %name = %mapDB.getColumn(%result2, "name");               
                  echo("  Node " @ %nodeId @ " longitude " @ %longitude @ " latitude " @ %latitude @ 
                       " type " @ %type @ " name " @ %name );
                  //%nodeString = %nodeString @ " Node = \"" @ %pos @ " " @ %roadWidth @ " 2 0 0 1\";";//2 = road depth, fix                  
                  %nodeString = %nodeString @ " Node = \"" @ %pos @ " " @ %roadWidth @ "\";";                  
                  %mapDB.nextRow(%result2);
               }            
               %mapDB.clearResult(%result2);
            }
            //Node = "-2263.4 -2753.58 233.796 10 5 0 0 1";
           // " Node = \"0.0 0.0 300.0 30.000000\";" @
            echo( %nodeString );
            //Then, do the new DecalRoad, execed in order to get a loop into the declaration.
            
            %roadString = "      new DecalRoad() {" @
               " InternalName = \"" @ %wayId @ "\";" @
               " Material = \"" @ %roadMaterial @ "\";" @
               " textureLength = \"25\";" @
               " breakAngle = \"3\";" @
               " renderPriority = \"10\";" @
               " position = \"" @ %pos @ "\";" @ //Better position of last node than nothing, I guess.
               " rotation = \"1 0 0 0\";" @
               " scale = \"1 1 1\";" @
               " canSave = \"1\";" @
               " canSaveDynamicFields = \"1\";" @
               %nodeString @
            "};";
            /*
            %roadString = "      new MeshRoad() {" @
            " topMaterial = \"DefaultRoadMaterialTop\";" @
            " bottomMaterial = \"DefaultRoadMaterialOther\";" @
            " sideMaterial = \"DefaultRoadMaterialOther\";" @
            " textureLength = \"5\";" @
            " breakAngle = \"3\";" @
            " widthSubdivisions = \"0\";" @
            " position = \"-2263.4 -2753.58 233.796\";" @
            " rotation = \"1 0 0 0\";" @
            " scale = \"1 1 1\";" @
            " canSave = \"1\";" @
            " canSaveDynamicFields = \"1\";" @
             %nodeString @
            "};";
         */
            eval(%roadString); 
         }
         
         %mapDB.nextRow(%result);
      }
      %mapDB.clearResult(%result);
   } else echo ("no results.");
   
   %mapDB.closeDatabase();
   %mapDB.delete();
}


/*
function streetMap()
 {   
    %xml = new SimXMLDocument() {};
    %xml.loadFile( "only_kincaid_map.osm" );

    // "Get" inside of the root element, "Students".     
    %result = %xml.pushChildElement("osm");  
    %version = %xml.attribute("version");     
    %generator = %xml.attribute("generator");      
    // "Get" into the first child element    
    %xml.pushFirstChildElement("bounds"); 
    %minlat = %xml.attribute("minlat");
    %maxlat = %xml.attribute("maxlat");
    echo("result: " @ %result @ " version: " @ %version @ ", generator " @ %generator @" minlat " @ %minlat @ " maxlat " @ %maxlat );
    while  (%xml.nextSiblingElement("node"))     
    {     
       %id = %xml.attribute("id"); 
       %lat = %xml.attribute("lat");     
       %lon = %xml.attribute("lon");    
       echo("node " @ %id @ " lat " @ %lat @ " long " @ %lon);   
       //HERE: store data in sqlite, and then read it back in the makeStreets function. 
       //Need at least a "way" table and a "node" table, plus other decorators I'm sure.
    } 
    %xml.nextSiblingElement("way");    
    echo("way: " @ %xml.attribute("id"));
    %xml.pushFirstChildElement("nd");
    echo("ref: " @ %xml.attribute("ref"));
    while (%xml.nextSiblingElement("nd")) 
    {
       echo("ref: " @ %xml.attribute("ref"));
    }
    while (%xml.nextSiblingElement("tag"))
    {
       echo("k: " @ %xml.attribute("k") @ "  v: " @ %xml.attribute("v") );
    }
    
 }  */



////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////

//AND... all of the following moved off to uiForms.cs. Delete from here... soon.


/*
function exposeMegaMotion()
{
   if (isDefined("MegaMotionWindow"))
      MegaMotionWindow.delete();
   
   makeSqlGuiForm(1);
   setupMegaMotionForm();   
}

function setupMegaMotionForm()
{
   if (!isDefined("MegaMotionWindow"))
      return;   
      
   %sceneSetList = MegaMotionWindow.findObjectByInternalName("sceneSetList");
   %sceneList = MegaMotionWindow.findObjectByInternalName("sceneList");
   
   %sceneSetList.add("Testing","1");
   %sceneSetList.add("Portlingrad","2");
   %sceneSetList.setSelected(1);
   
   %sceneList.add("PG_001","1");
   %sceneList.add("PG_002","2");
   %sceneList.add("PG_003","3");
   %sceneList.add("PG_004","4");
   %sceneList.setSelected(1);
   
}

function exposeUIFormWindow()
{
   if (isDefined("uiFormWindow"))
      uiFormWindow.delete();
  
   makeSqlGuiForm(37);
   setupUIFormWindow();   
}

function setupUIFormWindow()
{
   if (!isDefined("uiFormWindow"))
      return; 
   
   $formList = uiFormWindow.findObjectByInternalName("formList");
   $elementList = uiFormWindow.findObjectByInternalName("elementList");
   $parentList = uiFormWindow.findObjectByInternalName("parentList");
   $leftAnchorList = uiFormWindow.findObjectByInternalName("leftAnchorList");
   $rightAnchorList = uiFormWindow.findObjectByInternalName("rightAnchorList");
   $topAnchorList = uiFormWindow.findObjectByInternalName("topAnchorList");
   $bottomAnchorList = uiFormWindow.findObjectByInternalName("bottomAnchorList");
   
   %tempControlCount = 0;
   $formList.add("",0);
   $elementList.add("",0);
   $parentList.add("",0);
   $leftAnchorList.add("",0);
   $rightAnchorList.add("",0);
   $topAnchorList.add("",0);
   $bottomAnchorList.add("",0);
   
   //Now, QUERIES!
   echo("setting up uiFormWindow!!!");
   %query = "SELECT e.id,e.name from uiElement e " @
            "WHERE e.id = e.form_id;";   
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      while (!sqlite.endOfResult(%resultSet))
      {
         %id = sqlite.getColumn(%resultSet, "id");
         %name = sqlite.getColumn(%resultSet, "name");
         $formList.add(%name,%id);
         sqlite.nextRow(%resultSet);
      }
   }
   
   $horizAlignList = uiFormWindow.findObjectByInternalName("horizAlignList");
   $vertAlignList = uiFormWindow.findObjectByInternalName("vertAlignList");
   
   $horizAlignList.add("",0);
   $horizAlignList.add("Left",1);
   $horizAlignList.add("Center",2);
   $horizAlignList.add("Right",3);
   
   $vertAlignList.add("",0);
   $vertAlignList.add("Top",1);
   $vertAlignList.add("Center",2);
   $vertAlignList.add("Bottom",3);
   
}

////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////

function saveSqlGuiXML(%form_id,%xml_file)
{   
   
   %count = 0;
   
   %column_names[0] = "id";//This may not be used, but keep it here so we don't bump all the indices by one.
   %column_names[1] = "parent_id";//This will be a name instead of an ID here, but same field works.
   %column_names[2] = "name";
   %column_names[3] = "width";
   %column_names[4] = "height";
   %column_names[5] = "type";
   %column_names[6] = "path";
   %column_names[7] = "left_anchor";
   %column_names[8] = "right_anchor";
   %column_names[9] = "top_anchor";
   %column_names[10] = "bottom_anchor";
   %column_names[11] = "content";
   %column_names[12] = "command";
   %column_names[13] = "tooltip";
   %column_names[14] = "horiz_align";
   %column_names[15] = "vert_align";
   %column_names[16] = "pos_x";
   %column_names[17] = "pos_y";
   %column_names[18] = "horiz_padding";
   %column_names[19] = "vert_padding";
   %column_names[20] = "horiz_edge_padding";
   %column_names[21] = "vert_edge_padding";
   %column_names[22] = "variable";
   %column_names[23] = "button_type";
   %column_names[24] = "group_num";
   %column_names[25] = "profile";
   %column_names[26] = "value";
   	         
   %query = "SELECT e.id,e.parent_id,e.bitmap_id,e.left_anchor,e.right_anchor,e.top_anchor,e.bottom_anchor,e.type," @
            "e.content,e.name,e.width,e.height,e.command,e.tooltip,e.horiz_align,e.vert_align,e.pos_x,e.pos_y,e.horiz_padding," @
            "e.vert_padding,e.horiz_edge_padding,e.vert_edge_padding,e.variable,e.button_type,e.group_num,e.profile," @
            "e.value,la.name as la_name,ra.name as ra_name,ta.name as ta_name,ba.name as ba_name,b.path " @
            "FROM uiElement e " @ 
	         "LEFT JOIN uiBitmap b ON b.id=e.bitmap_id " @ 
	         "LEFT JOIN uiElement la ON la.id=e.left_anchor " @ 
	         "LEFT JOIN uiElement ra ON ra.id=e.right_anchor " @ 
	         "LEFT JOIN uiElement ta ON ta.id=e.top_anchor " @ 
	         "LEFT JOIN uiElement ba ON ba.id=e.bottom_anchor " @ 
	         "WHERE e.form_id=" @ %form_id @ ";"; 
	%resultSet = sqlite.query(%query, 0);	
   
   if (%resultSet)
   {
      while (!sqlite.endOfResult(%resultSet))
      {
         %c = 0;         
         %results[%count,%c] = sqlite.getColumn(%resultSet, "id"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "parent_id"); %c++;
         
         %results[%count,%c] = sqlite.getColumn(%resultSet, "name"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "width"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "height"); %c++;
         
         %results[%count,%c] = sqlite.getColumn(%resultSet, "type"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "path"); %c++;

         %results[%count,%c] = sqlite.getColumn(%resultSet, "left_anchor"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "right_anchor"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "top_anchor"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "bottom_anchor"); %c++;

         %results[%count,%c] = sqlite.getColumn(%resultSet, "content"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "command"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "tooltip"); %c++;

         %results[%count,%c] = sqlite.getColumn(%resultSet, "horiz_align"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "vert_align"); %c++;
         
         %results[%count,%c] = sqlite.getColumn(%resultSet, "pos_x"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "pos_y"); %c++;

         %results[%count,%c] = sqlite.getColumn(%resultSet, "horiz_padding"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "vert_padding"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "horiz_edge_padding"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "vert_edge_padding"); %c++;

         %results[%count,%c] = sqlite.getColumn(%resultSet, "variable"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "button_type"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "group_num"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "profile"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "value"); %c++;
         
         %results[%count,%c] = false;//OR, instead of overriding type, just add one more column at the end.
         
         echo("loading result " @ %count @ " " @ %results[%count,2] @ " width " @ %results[%count,3] @ 
         " height " @ %results[%count,4] @ "  command: " @ %results[%count,12] );
         
         %count++;
         sqlite.nextRow(%resultSet);
      }
   }
   
   if ((strlen(%results[0,2])==0)||(strlen(%results[0,5])==0))
   {
      echo("Gui form is missing either name: " @ %results[0,2] @ ", or type: " @ %results[0,5] @ ".");   
      return;
   }

   //First, do the NULL fix on the entire array.
   for (%k=0;%k<%count;%k++)
      for (%d=0;%d<%c;%d++)
         if (%results[%k,%d] $= "NULL") %results[%k,%d] = "";

   //Next, fix all the anchors, to use names instead of IDs, and maintain the sign-flip convention.
   for (%k=0;%k<%count;%k++)
   {
      for (%d=7;%d<11;%d++)
      {
         if (strlen(%results[%k,%d])>0)
         {
            %flip = "";
            if (%results[%k,%d]<0)
            {
               %flip = "-";
               %results[%k,%d] *= -1;
            }
            for (%j=0;%j<%count;%j++)
               if (%results[%j,0]==%results[%k,%d])
                  %results[%k,%d] = %flip @ %results[%j,2];
         }
      }
   }
   
   //Next, start the form, then start looping through children.
   %xml = new SimXMLDocument() {};
   %xml.addHeader();
	
   %xml.pushNewElement("gui");
   
   //OKAY, now we're getting down to it. Loop through children, same as before, except different.
   
   %finished = false;
   %formname = "";
   %sanityCount = 0;
   %layerCount = 0;//Keeps track of how many layers deep we are in the parent hierarchy.
   %currentElement = %form_id;
   
   while ((%finished==false)&&(%sanityCount++ < 50))//For this application, there are no undefined anchors,
   {               // so we only have to worry about doing the main loop again for subcontainers.
   
      %currentCounter = 0;//First, %currentElement is a DB ID, so change it to an array counter.
      for (%k=0;%k<%count;%k++)
         if (%results[%k,0]==%currentElement)
            %currentCounter = %k;
      
      %currentChildCount = 0;//Now, make sure we have children, and count them.
      for (%k=0;%k<%count;%k++)
      {
         if (%results[%k,1]==%currentElement) // 1 = parent_id
         {
            %currentChildren[%currentChildCount] = %results[%k,0]; // 0 = id
            %currentChildCount++;//TorqueScript: don't put ++ inside brackets, or it will ++ too soon.
         }
      }
      if (%currentChildCount == 0) //Something went wrong, get us out of here.
      { 
         %finished = true;
         continue;
      }
      
      if (!%results[%currentCounter,%c]) //Don't start the form again, if we're back here to
      {  //finish the rest of the children after having grandchildren.
         if (%currentElement == %form_id)
            %xml.pushNewElement("form");
         else 
            %xml.pushNewElement("element");
            
         %xml.setAttribute(%column_names[0],%results[%currentCounter,0]);
         for (%d=2;%d<5;%d++)
            %xml.setAttribute(%column_names[%d],%results[%currentCounter,%d]);
      
         for (%d=5;%d<%c;%d++)
         {
            if (strlen(%results[%currentCounter,%d]) > 0)
            {
               if (%d==5) //This is why we put type in the array, so pushNewElement will always happen first.
                  %xml.pushNewElement(%column_names[%d]);
               else       //Afterward they are all addNewElement.
                  %xml.addNewElement(%column_names[%d]);         
               
               %xml.addData(%results[%currentCounter,%d]);
            }
         }
         %results[%currentCounter,%c] = true;
      }
      
      //Next, run through this container's children.
      for (%k=0;%k<%currentChildCount;%k++)
      {
         %childCounter = 0;//Convert this child's DB ID into an array counter.
         for (%d=0;%d<%count;%d++)
            if (%results[%d,0]==%currentChildren[%k])
               %childCounter = %d;
          
         //Then, check for finished flag. 
         if (%results[%childCounter,%c])  
            continue;
         
         //Now, check this child for its own children. If so, save current parent to %layers and increment %layerCount.
         %subfinished = true;
         %newChildCount = 0;
         for (%j=1;%j<%count;%j++)//Search through whole array of all results. (Except don't start at zero, that's always the form.)
         {
            if (%results[%j,1]==%currentChildren[%k]) // 1 = parent_id
               %newChildCount++;            
         }
         if (%newChildCount>0)
         {
            %layers[%layerCount] = %currentElement; 
            %currentElement = %currentChildren[%k];
            %k = %currentChildCount; //Go to the end, exit loop.
            %subfinished = false;
            %layerCount++;
            %xml.popElement();
            continue;
         } ////////// Full stop if children found. Exit loop and start over, one layer deeper. //////////
            
         ///////////////////////////////
         //Now, if this is a leaf node, go ahead and render it and set finished=true;
         %xml.addNewElement("element");
         
         %xml.setAttribute(%column_names[0],%results[%childCounter,0]);
         for (%d=2;%d<5;%d++)
            %xml.setAttribute(%column_names[%d],%results[%childCounter,%d]);
      
         for (%d=5;%d<%c;%d++)
         {
            if (strlen(%results[%childCounter,%d]) > 0)
            {
               if (%d==5) //This is why we put type in the array, so pushNewElement will always happen first.
                  %xml.pushNewElement(%column_names[%d]);
               else       //Afterward they are all addNewElement.
                  %xml.addNewElement(%column_names[%d]);         
               
               %xml.addData(%results[%childCounter,%d]);
            }
         }
         %results[%childCounter,%c] = true;
         %xml.popElement();
      }
      
      //Now, even though we don't have the "undefined" complication here, due to not needing to obtain
      //final positions from anchors, we still need to keep track of when we stop midway in a list of
      //children and jump down a layer, to a child's children.
      for (%k=0;%k<%currentChildCount;%k++)
      {
         %childCounter = 0;
         for (%d=0;%d<%count;%d++)
            if (%results[%d,0]==%currentChildren[%k])
               %childCounter = %d;
               
          if (!%results[%childCounter,%c])
            %subfinished = false;               
      }
      
      //But now, if subfinished is still true, that means we are done with this container, go back up.
      if (%subfinished) //Only do this if we made it to the end of the children.
      {    
         %layerCount--;
         %currentElement = %layers[%layerCount];//-1, or just %layerCount?
         %xml.popElement();
      }
      
      //Finally: run through the whole list, and if finished is true for every control, we're done.
      for (%k=0;%k<%count;%k++)
      {
         if (!%results[%k,%c])
         {//exit with finished=false the first time we find a valid type.
            %k = %count;
            continue;
         }
         if (%k==(%count-1))
            %finished=true;         
      }      
   }
   
   %xml.popElement();//form
   %xml.popElement();//gui
   
   %xml.saveFile(%xml_file);    
   echo("Saving " @ %xml_file);
}



////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////


function makeXmlGuiForm(%filename)
{
  echo("MAKE XML GUI FORM!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
   //////////////////////////////////////////////////////////////////////
   //HERE: for xml, need to load results with contents of the file, it will contain exactly the children of this 
   //form and nothing else, so no need for queryies.   
   //////////////////////////////////////////////////////////////////////
   %column_names[0] = "id";//This may not be used, but keep it here so we don't bump all the indices by one.
   %column_names[1] = "parent_id";//This will be a name instead of an ID here, but same field works.
   %column_names[2] = "name";
   %column_names[3] = "width";
   %column_names[4] = "height";
   %column_names[5] = "type";
   %column_names[6] = "path";
   %column_names[7] = "left_anchor";
   %column_names[8] = "right_anchor";
   %column_names[9] = "top_anchor";
   %column_names[10] = "bottom_anchor";
   %column_names[11] = "content";
   %column_names[12] = "command";
   %column_names[13] = "tooltip";
   %column_names[14] = "horiz_align";
   %column_names[15] = "vert_align";
   %column_names[16] = "pos_x";
   %column_names[17] = "pos_y";
   %column_names[18] = "horiz_padding";
   %column_names[19] = "vert_padding";
   %column_names[20] = "horiz_edge_padding";
   %column_names[21] = "vert_edge_padding";
   %column_names[22] = "variable";
   %column_names[23] = "button_type";
   %column_names[24] = "group_num";
   %column_names[25] = "profile";
   %column_names[26] = "value";
   %c = 27;
   %xml = new SimXMLDocument() {};
   %xml.loadFile( %filename );
   
   %xml.pushChildElement("gui");  
   
   %xml.pushChildElement("form");  
   
   %count = 0;
   
   for (%k=0;%k<27;%k++) // first clear the array.
      %results[%count,%k] = "";
      
   %results[%count,0] = %xml.attribute(%column_names[0]);//ID
   if (strlen(%results[%count,0])==0) %results[%count,0] = %count;
   
   for (%c=2;%c<5;%c++)
      %results[%count,%c] = %xml.attribute(%column_names[%c]);     
   for (%c=5;%c<27;%c++)
   {
      if (%xml.pushFirstChildElement(%column_names[%c]))
      {
         %results[%count,%c] = %xml.getData();   
         %xml.popElement();
      }
   }
   
   %results[%count,1] = 0;//No parent, for we are the top.
   %results[%count,27] = false;//And always mark "finished" flag false.
   %form_id = %results[%count,0];
   
   %container_type = %results[%count,5];//Save this for later. (?)
   //echo("loading form " @ %count @ " " @ %results[%count,2] @ " width " @ %results[%count,3] @ 
   //   " height " @ %results[%count,4] @ " type " @ %container_type @ " pos_x " @ %results[%count,16] @ 
   //   " pos y " @ %results[%count,17] @ " horiz edge padding " @ %results[%count,20] @ 
   //   " vert edge padding " @ %results[%count,21] @ " horiz align " @ %results[%count,14] @ "!!!\n" );
   %count++; 
   
   /////////////////////////////////////////////// 
   //Now, load all the element children, and their children, and their children...
   %elem = %xml.pushFirstChildElement("element");
   while (%elem)
   {      
      for (%k=0;%k<27;%k++) %results[%count,%k] = "";      
      %results[%count,0] = %xml.attribute(%column_names[0]);   
      if (strlen(%results[%count,0])==0) %results[%count,0] = %count;
      for (%c=2;%c<5;%c++)
         %results[%count,%c] = %xml.attribute(%column_names[%c]);     
      for (%c=5;%c<27;%c++)
      {
         if (%xml.pushFirstChildElement(%column_names[%c]))
         {
            %results[%count,%c] = %xml.getData();   
            %xml.popElement();
         }
      }
      %results[%count,1] = %form_id;
      %results[%count,27] = false;
      %count++; 
      ///////////////////////////////////////////////
      %elem2 =  %xml.pushFirstChildElement("element");
      if (%elem2)
      {
         %parent2 = %results[%count-1,0];
         while (%elem2)
         {            
            for (%k=0;%k<27;%k++) %results[%count,%k] = "";         
            %results[%count,0] = %xml.attribute(%column_names[0]);   
            if (strlen(%results[%count,0])==0) %results[%count,0] = %count;
            for (%c=2;%c<5;%c++)
               %results[%count,%c] = %xml.attribute(%column_names[%c]);     
            for (%c=5;%c<27;%c++)
            {
               if (%xml.pushFirstChildElement(%column_names[%c]))
               {
                  %results[%count,%c] = %xml.getData();   
                  %xml.popElement();
               }
            }
            %results[%count,1] = %parent2;
            %results[%count,27] = false;
            %count++; 
            ///////////////////////////////////////////////            
            %elem3 =  %xml.pushFirstChildElement("element");
            if (%elem3)
            {
               %parent3 = %results[%count-1,0];
               while (%elem3)
               {               
                  for (%k=0;%k<27;%k++) %results[%count,%k] = "";         
                  %results[%count,0] = %xml.attribute(%column_names[0]);  
                  if (strlen(%results[%count,0])==0) %results[%count,0] = %count;    
                  for (%c=2;%c<5;%c++)
                     %results[%count,%c] = %xml.attribute(%column_names[%c]);     
                  for (%c=5;%c<27;%c++)
                  {
                     if (%xml.pushFirstChildElement(%column_names[%c]))
                     {
                        %results[%count,%c] = %xml.getData();   
                        %xml.popElement();
                     }
                  }
                  %results[%count,1] = %parent3;                  
                  %results[%count,27] = false;
                  %count++; 
                  ///////////////////////////////////////////////             
                  %elem4 =  %xml.pushFirstChildElement("element");
                  if (%elem4)
                  {
                     %parent4 = %results[%count-1,0];
                     while (%elem4)
                     {                  
                        for (%k=0;%k<27;%k++) %results[%count,%k] = "";
                        %results[%count,0] = %xml.attribute(%column_names[0]);      
                               
                        for (%c=2;%c<5;%c++)
                           %results[%count,%c] = %xml.attribute(%column_names[%c]);     
                        for (%c=5;%c<27;%c++)
                        {
                           if (%xml.pushFirstChildElement(%column_names[%c]))
                           {
                              %results[%count,%c] = %xml.getData();   
                              %xml.popElement();
                           }
                        }
                        %results[%count,1] = %parent4;  
                        %results[%count,27] = false;
                        %count++; 

                        ///And... that's as many layers as we get for XML, if you want more use SQL version.
                        
                        %elem4 = %xml.nextSiblingElement("element");
                     }
                     %xml.popElement();
                  }/////////////////////////////////////////////// 
                  %elem3 = %xml.nextSiblingElement("element");
               }
               %xml.popElement();               
            }/////////////////////////////////////////////// 
            %elem2 = %xml.nextSiblingElement("element");
         }
         %xml.popElement();
      }/////////////////////////////////////////////// 
      %elem = %xml.nextSiblingElement("element");
   }/////////////////////////////////////////////// 
      
   //Now: we have all the data, loaded up into %results, but we need to fix the parents and anchors, 
   // to use IDs instead of names, but maintain the sign-flip convention.
   for (%k=0;%k<%count;%k++)
   {
      //First, parent. If we have a parent name, then convert it to the parent ID.
      if (strlen(%results[%k,1])>0)
      {
         for (%j=0;%j<%count;%j++)
            if (%results[%j,2]$=%results[%k,1])
               %results[%k,1] = %results[%j,0];
      }
      //Then, do the same thing for the four anchors, along with the sign-flipping convention.
      for (%d=7;%d<11;%d++)
      {
         if (strlen(%results[%k,%d])>0)
         {
            %flip = 1;
            if (strchrpos(%results[%k,%d],"-")>=0) //If leading hyphen is found in name, flip sign on anchor id.
            { 
               %flip = -1;
               %results[%k,%d] = getSubStr(%results[%k,%d],1);//Remove leading hyphen.
            }
            for (%j=0;%j<%count;%j++)
               if (%results[%j,2]$=%results[%k,%d])//If (name = anchor name) then go back to ID,
                  %results[%k,%d] = %flip * %results[%j,0];//  flipped if necessary.
                  
         }
      }
   }

  
   //////////////////////////////////////////////////////////////////////
   
   %script = "";
   %indent = "";
   %finished = false;
   %formname = "";
   %sanityCount = 0;
   %layers[0,0] = %results[0,0];//First parent is always the form itself.
   %layers[0,1] = %indent;
   %layers[0,2] = false;//Maybe? use this to see if we've added our indent yet.
   %layerCount = 1;//Keeps track of how many layers deep we are in the parent hierarchy.
   %currentElement = %results[0,0];
   while ((%finished == false)&&(%sanityCount++ < 100))
   {      
      //This is now a loop down through each set of children, until we hit the bottom and come back.
      %currentCounter = 0;
      for (%k=0;%k<%count;%k++)
         if (%results[%k,0]==%currentElement)
            %currentCounter = %k;
      
      %currentChildCount = 0;
      for (%k=0;%k<%count;%k++)
      {
         if (%results[%k,1]==%currentElement) // 1 = parent_id
         {
            %currentChildren[%currentChildCount] = %results[%k,0]; // 0 = id
            %currentChildCount++;//TorqueScript: don't put ++ inside brackets, or it will ++ too soon.
         }
      }
      if (%currentChildCount == 0) //Something went wrong, get us out of here.
      { 
         %finished = true;
         continue;
      }
      
      //First, clean up our data for this parent, and grab what we need, a subset of all the fields.
      for (%d=0;%d<%c;%d++)
         if (%results[%currentCounter,%d] $= "NULL") 
            %results[%currentCounter,%d] = "";

      //Then grab the subset of data we need, minus things that are only for leaf nodes.
      %id = %results[%currentCounter,0];
      %parent_id = %results[%currentCounter,1];
      %name = %results[%currentCounter,2];
      %width = %results[%currentCounter,3];
      %height = %results[%currentCounter,4];               
      %type = %results[%currentCounter,5]; 
         
      %container_type = %type;//Save this for later.
      
      %bitmap_path = %results[%currentCounter,6];       
      %left_anchor = %results[%currentCounter,7]; 
      %right_anchor = %results[%currentCounter,8];
      %top_anchor = %results[%currentCounter,9];
      %bottom_anchor = %results[%currentCounter,10];

      %horiz_align = %results[%currentCounter,14];
      %vert_align = %results[%currentCounter,15];
      
      %pos_x = %results[%currentCounter,16]; 
      %pos_y = %results[%currentCounter,17]; 

      %horiz_padding = %results[%currentCounter,18];
      %vert_padding = %results[%currentCounter,19];
      %horiz_edge_padding = %results[%currentCounter,20];
      %vert_edge_padding = %results[%currentCounter,21];

      %profile = %results[%currentCounter,25];
      
      if (%currentElement==%form_id)
      {//We have to keep checking finished marker because we could be coming back up, not our first time here.
         if (!%results[%currentCounter,%c])
         {
            %formname = %name;
            echo("starting form, id=" @ %currentElement @ " name = " @ %name @ "\n");
            %script = %script @ "%guiContent = new " @ %type @ "(" @ %name @ ") {\n";

            %editorExtents = EWorldEditor.getExtent();//FIX: I'm assuming all guis are part of the world editor,
            %editorWidth = getWord(%editorExtents,0);//       which is not a good assumption.
            %editorHeight = getWord(%editorExtents,1);
            %pos_x = %pos_x * %editorWidth;
            %pos_y = %pos_y * %editorHeight;
            %results[%currentCounter,16] = %pos_x; //Oh, this is a little awkward... in the DB, I set up pos x/y
            %results[%currentCounter,17] = %pos_y; // to be in percentages, ie float from {0.0,1.0}, but after 
                                                   // we start writing to the buffer these are in pixels.
         
            %script = %script @ "   position = \"" @ mFloor(%pos_x) @ " " @ mFloor(%pos_y) @ "\";\n";
            %script = %script @ "   extent = \"" @ %width @ " " @ %height @ "\";\n";
            %script = %script @ "   text = \"" @ %name @ "\";\n";
            %script = %script @ "   canClose = \"1\";\n";
            %script = %script @ "   closeCommand = \"" @ %name @ ".setVisible(false);\";\n\n";
            
            %results[%currentCounter,%c] = true;//Set finished marker to true, we're done with this one.
         }
      } 
      else if (!%results[%currentCounter,%c])
      {  //We are not the top form, so we need to find out our anchor situation.
         //echo("adding element: " @ %currentElement @ " left " @ %left_anchor @ " right " @ %right_anchor @ 
         //   " top " @ %top_anchor @ " bottom " @ %bottom_anchor @  " parent id " @ %parent_id @ "\n");
            
         //First, find parent container padding numbers.         
         %i = 0;
         %container_width = 0;
         %container_height = 0;
         %container_pos_x = 0;
         %container_pos_y = 0;         
         %container_horiz_padding = 0;
         %container_vert_padding = 0;
         %container_horiz_edge_padding = 0;
         %container_vert_edge_padding = 0;
         while (%i < %count)
         {
            if (%results[%i,0] == %parent_id)
            {
               %container_width = %results[%i,3];
               %container_height = %results[%i,4];
               %container_pos_x = %results[%i,16];
               %container_pos_y = %results[%i,17];
               %container_horiz_padding = %results[%i,18];
               %container_vert_padding = %results[%i,19];
               %container_horiz_edge_padding = %results[%i,20];
               %container_vert_edge_padding = %results[%i,21];              
            }
            %i++;
         }
         
         echo("container " @ %currentCounter @ " parent " @ %parent_id @ " anchors: l " @ %left_anchor @ " r " @ %right_anchor @ " t " @ %top_anchor @ " b " @ %bottom_anchor );
         %undefined = false;
         %horiz_anchor_flip = false;
         if (%left_anchor < 0)
         {
            %horiz_anchor_flip = true;
            %left_anchor *= -1;
         }
         if (%right_anchor < 0)
         {
            %horiz_anchor_flip = true;
            %right_anchor *= -1;
         }
         if ((strlen(%left_anchor)>0)&&(%left_anchor == %parent_id))
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {    
                     %pos_x = %container_pos_x + %container_horiz_edge_padding + %horiz_padding;      
                  } else { 
                     %pos_x = %container_horiz_edge_padding + %horiz_padding;
                  }
               }
               %i++;
            }               
         }
         else if (%left_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %left_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_x = %results[%i,16];
                  %anchor_width = %results[%i,3];
                  echo("container left anchor pos " @ %anchor_pos_x @ " width " @ %anchor_width);
                  if (%horiz_anchor_flip == false)
                     %pos_x = %anchor_pos_x + %anchor_width + %container_horiz_padding + %horiz_padding;
                  else
                     %pos_x = %anchor_pos_x + %horiz_padding;
                  
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;
         } 
         else if (%right_anchor == %parent_id) 
         {
            echo("container right anchor pos " @ %container_pos_x @ " width " @ %container_width);
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  
                  if (%results[%i,5] $= "Virtual")
                  {
                     %pos_x = %container_pos_x + %container_width - %width - %container_horiz_edge_padding - %horiz_padding;                     
                  } else {
                     %pos_x = %container_width - %width - %container_horiz_edge_padding - %horiz_padding;
                  }
               }
               %i++;
            }
         } 
         else if (%right_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %right_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_x = %results[%i,16];
                  %anchor_width = %results[%i,3];
                  if (%horiz_anchor_flip == false)
                     %pos_x = %anchor_pos_x - %width - %container_horiz_padding - %horiz_padding;
                  else
                     %pos_x = %anchor_pos_x + %anchor_width - %width - %horiz_padding;
                  
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;
         }

         ////// top/bottom anchors //////////////
         %vert_anchor_flip = false;
         if (%top_anchor < 0)
         {
            %vert_anchor_flip = true;
            %top_anchor *= -1;
         }
         if (%bottom_anchor < 0)
         {
            %vert_anchor_flip = true;
            %bottom_anchor *= -1;
         }
         if ((strlen(%top_anchor)>0)&&(%top_anchor == %parent_id)) //check for top anchor first, then bottom.
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {
                     %pos_y = %container_pos_y + %container_vert_edge_padding + %vert_padding;  
                  } else {
                     %pos_y = %container_vert_edge_padding + %vert_padding;
                  }
               }
               %i++;
            }      
         }
         else if (%top_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %top_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_y = %results[%i,17];
                  %anchor_height = %results[%i,4];
                  if (%vert_anchor_flip == false)
                     %pos_y = %anchor_pos_y + %anchor_height + %container_vert_padding + %vert_padding;
                  else
                     %pos_y = %anchor_pos_y + %vert_padding;
                  
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;            
         }   
         else if (%bottom_anchor == %parent_id) 
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {
                     %pos_y = %container_pos_y + %container_height - %height - %container_vert_edge_padding - %vert_padding; 
                  } else {
                     %pos_y = %container_height - %height - %container_vert_edge_padding - %vert_padding;
                  }
               }
               %i++;
            }             
         } 
         else if (%bottom_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %bottom_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_y = %results[%i,17];
                  %anchor_height = %results[%i,4];
                  if (%vert_anchor_flip == false)
                     %pos_y = %anchor_pos_y - %height - %container_vert_padding - %vert_padding;
                  else
                     %pos_y = %anchor_pos_y + %anchor_height - %height - %vert_padding;
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;
         }
         
         if (%undefined)
         {
            //This should never happen, we wouldn't be here if we hadn't found our anchors already below.
            echo("OOPS! Gui control has undefined anchors when it is supposed to be a parent! " @ %currentElement);
            return;
         }
         
         //Save these back to the results array so other controls can find them.
         %results[%currentCounter,16] = %pos_x; 
         %results[%currentCounter,17] = %pos_y; 
         //echo("parent control " @ %currentElement @ " found a position: " @ %pos_x @ " " @ %pos_y );
         
         if (%type !$= "Virtual")
         {  
            %script = %script @ %indent @ "new " @ %type @ "() {\n";
            %script = %script @ %indent @ "   position = \"" @ mFloor(%pos_x) @ " " @ mFloor(%pos_y) @ "\";\n";
            %script = %script @ %indent @ "   extent = \"" @ %width @ " " @ %height @ "\";\n";
            if (strlen(%name)>0) %script = %script @ %indent @ "   internalName = \"" @ %name @ "\";\n";            
            if (strlen(%bitmap_path)>0) %script = %script @ %indent @ "   bitmap = \"" @ %bitmap_path @ "\";\n"; 
            if (strlen(%profile)>0) %script = %script @ %indent @ "   profile = \"" @ %profile @ "\";\n"; 
            %script = %script @ "\n";
         }
         %results[%currentCounter,%c] = true;//Whether or not we're virtual, don't come back here.
      }
      
      //////////////////////////////////////////////////////////////////////////////////////////////////////////
      //////////////////////////////////////////////////////////////////////////////////////////////////////////
      //Next, run through the children.      
      if ((%container_type !$= "Virtual")&&(%layers[%layerCount-1,2]==false))
      {//AH, but this needs to *not* add a new indent if we are coming back here after the first time.
         %indent = %indent @ "   ";  
         %layers[%layerCount-1,2] = true;
      }
      for (%k=0;%k<%currentChildCount;%k++)
      {
         %childCounter = 0;
         for (%d=0;%d<%count;%d++)
            if (%results[%d,0]==%currentChildren[%k])
               %childCounter = %d;
               
         //First, change the database's returned "NULL" values into actual null strings.
         for (%d=0;%d<%c;%d++)
            if (%results[%childCounter,%d] $= "NULL") 
               %results[%childCounter,%d] = "";
               
         //echo("checking child " @ %k @ " id=" @ %currentChildren[%k] @ ", finished=" @ %results[%childCounter,%c]);
         
         //Then, check for finished flag.
         if (%results[%childCounter,%c])
            continue;
         
         ///////////////////////////////
         
         //Now, check this child for its own children. If so, save current parent to %layers and increment %layerCount.
         %subfinished = true;
         %newChildCount = 0;
         for (%j=1;%j<%count;%j++)//Search through whole array of all results. (Except don't start at zero, that's always the form.)
         {
            if (%results[%j,1]==%currentChildren[%k]) // 1 = parent_id
            {
               %newChildCount++;
            }
         }
         if (%newChildCount>0)
         {
            %layers[%layerCount,0] = %currentElement; 
            %layers[%layerCount,1] = %indent;       
            %layers[%layerCount,2] = false;      
            %currentElement = %currentChildren[%k];
            %k = %currentChildCount; //Go to the end, exit loop.
            %subfinished = false;
            %layerCount++;
            continue;
         } ////////// Full stop if children found. Exit loop and start over, one layer deeper. //////////
            
         ///////////////////////////////
         //Now, if this is a leaf node, go ahead and render it and set type="";

         //First, give results some variable names.
         %id = %results[%childCounter,0];
         %parent_id = %results[%childCounter,1];
         %name = %results[%childCounter,2];
         %width = %results[%childCounter,3];
         %height = %results[%childCounter,4];               
         %type = %results[%childCounter,5]; 
         
         %bitmap_path = %results[%childCounter,6]; 
         %left_anchor = %results[%childCounter,7]; 
         %right_anchor = %results[%childCounter,8];
         %top_anchor = %results[%childCounter,9];
         %bottom_anchor = %results[%childCounter,10];

         %content = %results[%childCounter,11];
         %command = %results[%childCounter,12];
         %tooltip = %results[%childCounter,13];

         %horiz_align = %results[%childCounter,14];
         %vert_align = %results[%childCounter,15];
         
         %pos_x = %results[%childCounter,16]; 
         %pos_y = %results[%childCounter,17]; 

         %horiz_padding = %results[%childCounter,18];
         %vert_padding = %results[%childCounter,19];
         %horiz_edge_padding = %results[%childCounter,20];
         %vert_edge_padding = %results[%childCounter,21];

         %variable = %results[%childCounter,22];
         %button_type = %results[%childCounter,23];
         %group_num = %results[%childCounter,24];
         %profile = %results[%childCounter,25];
         %value = %results[%childCounter,26];
             
         echo("reading element, name " @ %name @ " type " @ %type @ " id " @ %id );
         //And, UNFORTUNATELY, we (currently) need to repeat the whole block of anchor logic again here. 
         //Edit: REALLY?? Some way to refactor this MUST be possible. Out of time at the moment however.
         //Really not sure how to fix it. If TorqueScript had goto at least... but, nope.
         ///////////////////////////  BEGIN NASTY BLOCK OF REPEATED CODE /////////////////////////
         %i = 0;
         %container_width = 0;
         %container_height = 0;
         %container_pos_x = 0;
         %container_pos_y = 0;         
         %container_horiz_padding = 0;
         %container_vert_padding = 0;
         %container_horiz_edge_padding = 0;
         %container_vert_edge_padding = 0;
         while (%i < %count)
         {
            if (%results[%i,0] == %parent_id)
            {
               %container_width = %results[%i,3];
               %container_height = %results[%i,4];
               %container_pos_x = %results[%i,16];
               %container_pos_y = %results[%i,17];
               %container_horiz_padding = %results[%i,18];
               %container_vert_padding = %results[%i,19];
               %container_horiz_edge_padding = %results[%i,20];
               %container_vert_edge_padding = %results[%i,21];              
            }
            %i++;
         }
         
         %undefined = false;
         %horiz_anchor_flip = false;
         if (%left_anchor < 0)
         {
            %horiz_anchor_flip = true;
            %left_anchor *= -1;
         }
         if (%right_anchor < 0)
         {
            %horiz_anchor_flip = true;
            %right_anchor *= -1;
         }
         if ((strlen(%left_anchor)>0)&&(%left_anchor == %parent_id))
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {    
                     %pos_x = %container_pos_x + %container_horiz_edge_padding + %horiz_padding;      
                  } else { 
                     %pos_x = %container_horiz_edge_padding + %horiz_padding;
                  }
               }
               %i++;
            }               
         }
         else if (%left_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %left_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_x = %results[%i,16];
                  %anchor_width = %results[%i,3];
                  echo("child left anchor pos " @ %anchor_pos_x @ " width " @ %anchor_width);
                  if (%horiz_anchor_flip == false)
                     %pos_x = %anchor_pos_x + %anchor_width + %container_horiz_padding + %horiz_padding;
                  else
                     %pos_x = %anchor_pos_x + %horiz_padding;
                  
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;
         } 
         else if (%right_anchor == %parent_id) 
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {
                     %pos_x = %container_pos_x + %container_width - %width - %container_horiz_edge_padding - %horiz_padding;                     
                  } else {
                     %pos_x = %container_width - %width - %container_horiz_edge_padding - %horiz_padding;
                  }
               }
               %i++;
            }
         } 
         else if (%right_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %right_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_x = %results[%i,16];
                  %anchor_width = %results[%i,3];
                  if (%horiz_anchor_flip == false)
                     %pos_x = %anchor_pos_x - %width - %container_horiz_padding - %horiz_padding;
                  else
                     %pos_x = %anchor_pos_x + %anchor_width - %width - %horiz_padding;
                  
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;
         }

         ////// top/bottom anchors //////////////
         %vert_anchor_flip = false;
         if (%top_anchor < 0)
         {
            %vert_anchor_flip = true;
            %top_anchor *= -1;
         }
         if (%bottom_anchor < 0)
         {
            %vert_anchor_flip = true;
            %bottom_anchor *= -1;
         }
         if ((strlen(%top_anchor)>0)&&(%top_anchor == %parent_id)) //check for top anchor first, then bottom.
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {
                     %pos_y = %container_pos_y + %container_vert_edge_padding + %vert_padding;  
                  } else {
                     %pos_y = %container_vert_edge_padding + %vert_padding;
                  }
               }
               %i++;
            }      
         }
         else if (%top_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %top_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_y = %results[%i,17];
                  %anchor_height = %results[%i,4];
                  if (%vert_anchor_flip == false)
                     %pos_y = %anchor_pos_y + %anchor_height + %container_vert_padding + %vert_padding;
                  else
                     %pos_y = %anchor_pos_y + %vert_padding;
                  
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;            
         }   
         else if (%bottom_anchor == %parent_id) 
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {
                     %pos_y = %container_pos_y + %container_height - %height - %container_vert_edge_padding - %vert_padding; 
                  } else {
                     %pos_y = %container_height - %height - %container_vert_edge_padding - %vert_padding;
                  }
               }
               %i++;
            }             
         } 
         else if (%bottom_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %bottom_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_y = %results[%i,17];
                  %anchor_height = %results[%i,4];
                  if (%vert_anchor_flip == false)
                     %pos_y = %anchor_pos_y - %height - %container_vert_padding - %vert_padding;
                  else
                     %pos_y = %anchor_pos_y + %anchor_height - %height - %vert_padding;
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;
         }
         ////////////////////////////  END NASTY BLOCK OF REPEATED CODE //////////////////////////
         
         if (%undefined) //Bail if we are missing anchors, we will check again later.
         {
            echo("control " @ %currentChildren[%k] @ " is currently undefined. Skipping for now.");
            continue;
         }
         %results[%childCounter,16] = %pos_x; 
         %results[%childCounter,17] = %pos_y; 
         echo("Element " @ %currentChildren[%k] @ " found a position: " @ %pos_x @ " " @ %pos_y);

         if (%type !$= "Virtual") //Should never happen, but if someone accidentally adds a Virtual and doesn't
         {                       //give it any children, we still need to not render it.
            %script = %script @ %indent @ "new " @ %type @ "() {\n";
            %script = %script @ %indent @ "   position = \"" @ mFloor(%pos_x) @ " " @ mFloor(%pos_y) @ "\";\n";
            %script = %script @ %indent @ "   extent = \"" @ %width @ " " @ %height @ "\";\n";
            if (strlen(%content)>0) %script = %script @ %indent @ "   text = \"" @ %content @ "\";\n";
            if (strlen(%name)>0) %script = %script @ %indent @ "   internalName = \"" @ %name @ "\";\n";            
            if (strlen(%command)>0) %script = %script @ %indent @ "   command = \"" @ %command @ "\";\n";
            if (strlen(%tooltip)>0) %script = %script @ %indent @ "   tooltip = \"" @ %tooltip @ "\";\n"; 
            if (strlen(%tooltip)>0) %script = %script @ %indent @ "   tooltipprofile = \"GuiToolTipProfile\";\n";
            if (strlen(%bitmap_path)>0) %script = %script @ %indent @ "   bitmap = \"" @ %bitmap_path @ "\";\n"; 
            if (strlen(%variable)>0) %script = %script @ %indent @ "   variable = \"" @ %variable @ "\";\n"; 
            if (strlen(%button_type)>0) %script = %script @ %indent @ "   buttonType = \"" @ %button_type @ "\";\n"; 
            if (strlen(%group_num)>0) %script = %script @ %indent @ "   groupNum = \"" @ %group_num @ "\";\n"; 
            if (strlen(%profile)>0) %script = %script @ %indent @ "   profile = \"" @ %profile @ "\";\n"; 
            %script = %script @ %indent @ "};\n";
            %results[%childCounter,%c] = true; 
         }
         
         //Clear all, so you don't end up with the last control's tooltip because this one doesn't have one...
         %width = "";
         %height = "";
         %left_anchor = "";
         %right_anchor = "";
         %top_anchor = "";
         %bottom_anchor = "";
         %name = "";
         %type = "";
         %content = "";
         %command = "";
         %tooltip = "";
         %bitmap_path = "";
         %variable = "";
         %button_type = "";
         %group_num = "";
         %value = "";
         %profile = "";
         
      }//end of for (0..%currentChildCount) loop.
      
      //Now, double check, did we finish everybody?
      //Two ways we can not be done: either we were sent out of the loop because we need to go into a deeper
      //child layer, or we have children we passed by earlier because of undefined anchors.
      //This is for checking the second case. 
      for (%k=0;%k<%currentChildCount;%k++)
      {
         %childCounter = 0;
         for (%d=0;%d<%count;%d++)
            if (%results[%d,0]==%currentChildren[%k])
               %childCounter = %d;
               
          if (!%results[%childCounter,%c])
            %subfinished = false;               
      }
      
      //But now, if subfinished is still true, that means we are done with this container, go back up.
      if (%subfinished) //Only do this if we made it to the end of the children.
      {    
         %layerCount--;
         %currentElement = %layers[%layerCount,0];//-1, or just %layerCount?
         %indent = %layers[%layerCount,1];       
         if (%container_type !$= "Virtual")
         {
            %script = %script @ %indent @ "};\n";
         }       
      }
      
      //Finally: run through the whole list, and if type is null for every control, we're DONE done.
      for (%k=0;%k<%count;%k++)
      {
         if (!%results[%k,%c])
         {//exit with finished=false the first time we find a valid type.
            %k = %count;
            continue;
         }
         if (%k==(%count-1))
         {
            echo("finished main loop with all controls defined, sanityCount " @ %sanityCount);
            %finished=true;
         }
      }
   }
   
   //%script = %script @ "};\n";
   
   
   //And then, save it out as a .gui file.
   %filename = %formname @ ".gui";
   %fileObject = new FileObject();
   %fileObject.openForWrite( %filename ); 
   %fileObject.writeLine(%script);
   %fileObject.close();
   %fileObject.delete();
      
   echo("\n\n SCRIPT:\n\n" @ %script);
   echo("finished GUI: sanityCount " @ %sanityCount);
   
   //////////////////////////////
   eval(%script);  //DO IT!
   //////////////////////////////
   
   EWorldEditor.add(%formname);
   
}



////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
function makeSqlGuiForm(%form_id)
{
   %count = 0;
   %query = "SELECT e.id,e.parent_id,e.bitmap_id,e.left_anchor,e.right_anchor,e.top_anchor,e.bottom_anchor,e.type," @
            "e.content,e.name,e.width,e.height,e.command,e.tooltip,e.horiz_align,e.vert_align,e.pos_x,e.pos_y," @
            "e.horiz_padding,e.vert_padding,e.horiz_edge_padding,e.vert_edge_padding,e.variable,e.button_type," @
            "e.group_num,e.profile,e.value,b.path " @
            "FROM uiElement e " @ 
	         "LEFT JOIN uiBitmap b ON b.id=e.bitmap_id " @ 
	         "WHERE e.form_id=" @ %form_id @ ";"; 	         
	%resultSet = sqlite.query(%query, 0);	
   echo( "makeSqlGuiForm2, query: \n" @ %query @ "\n  NUMROWS " @ sqlite.numRows(%resultSet));
   
   if (%resultSet)
   {
      while (!sqlite.endOfResult(%resultSet))
      {
         %c = 0;         
         %results[%count,%c] = sqlite.getColumn(%resultSet, "id"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "parent_id"); %c++;
         
         %results[%count,%c] = sqlite.getColumn(%resultSet, "name"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "width"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "height"); %c++;
         
         echo("loading result " @ %count @ " " @ %results[%count,2] @ " width " @ %results[%count,3] @ 
         " height " @ %results[%count,4]  );
         
         %results[%count,%c] = sqlite.getColumn(%resultSet, "type"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "path"); %c++;

         %results[%count,%c] = sqlite.getColumn(%resultSet, "left_anchor"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "right_anchor"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "top_anchor"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "bottom_anchor"); %c++;

         %results[%count,%c] = sqlite.getColumn(%resultSet, "content"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "command"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "tooltip"); %c++;

         %results[%count,%c] = sqlite.getColumn(%resultSet, "horiz_align"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "vert_align"); %c++;
         
         %results[%count,%c] = sqlite.getColumn(%resultSet, "pos_x"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "pos_y"); %c++;

         %results[%count,%c] = sqlite.getColumn(%resultSet, "horiz_padding"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "vert_padding"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "horiz_edge_padding"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "vert_edge_padding"); %c++;

         %results[%count,%c] = sqlite.getColumn(%resultSet, "variable"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "button_type"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "group_num"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "profile"); %c++;
         %results[%count,%c] = sqlite.getColumn(%resultSet, "value"); %c++;
         
         %results[%count,%c] = false;//OR, instead of overriding type, just add one more column at the end.
         
         %count++;
         sqlite.nextRow(%resultSet);
      }
   }
   
   //////////////////////////////////////////////////////////////////////
   //////////////////////////////////////////////////////////////////////
      
   %script = "";
   %indent = "";
   %finished = false;
   %formname = "";
   %sanityCount = 0;
   %layers[0,0] = %form_id;//First parent is always the form itself.
   %layers[0,1] = %indent;
   %layers[0,2] = false;//Maybe? use this to see if we've added our indent yet.
   %layerCount = 1;//Keeps track of how many layers deep we are in the parent hierarchy.
   %currentElement = %form_id;
   while ((%finished == false)&&(%sanityCount++ < 100))
   {      
      //This is now a loop down through each set of children, until we hit the bottom and come back.
      %currentCounter = 0;
      for (%k=0;%k<%count;%k++)
         if (%results[%k,0]==%currentElement)
            %currentCounter = %k;
      
      %currentChildCount = 0;
      for (%k=0;%k<%count;%k++)
      {
         if (%results[%k,1]==%currentElement) // 1 = parent_id
         {
            %currentChildren[%currentChildCount] = %results[%k,0]; // 0 = id
            %currentChildCount++;//TorqueScript: don't put ++ inside brackets, or it will ++ too soon.
         }
      }
      if (%currentChildCount == 0) //Something went wrong, get us out of here.
      { 
         %finished = true;
         continue;
      }
      
      //First, clean up our data for this parent, and grab what we need, a subset of all the fields.
      for (%d=0;%d<%c;%d++)
         if (%results[%currentCounter,%d] $= "NULL") 
            %results[%currentCounter,%d] = "";

      //Then grab the subset of data we need, minus things that are only for leaf nodes.
      %id = %results[%currentCounter,0];
      %parent_id = %results[%currentCounter,1];
      %name = %results[%currentCounter,2];
      %width = %results[%currentCounter,3];
      %height = %results[%currentCounter,4];               
      %type = %results[%currentCounter,5]; 
         
      %container_type = %type;//Save this for later.
      
      %bitmap_path = %results[%currentCounter,6];       
      %left_anchor = %results[%currentCounter,7]; 
      %right_anchor = %results[%currentCounter,8];
      %top_anchor = %results[%currentCounter,9];
      %bottom_anchor = %results[%currentCounter,10];

      %horiz_align = %results[%currentCounter,14];
      %vert_align = %results[%currentCounter,15];
      
      %pos_x = %results[%currentCounter,16]; 
      %pos_y = %results[%currentCounter,17]; 

      %horiz_padding = %results[%currentCounter,18];
      %vert_padding = %results[%currentCounter,19];
      %horiz_edge_padding = %results[%currentCounter,20];
      %vert_edge_padding = %results[%currentCounter,21];

      %profile = %results[%currentCounter,25];
      
      if (%currentElement==%form_id)
      {//We have to keep checking finished marker because we could be coming back up, not our first time here.
         if (!%results[%currentCounter,%c])
         {
            %formname = %name;
            echo("starting form, id=" @ %currentElement @ " name = " @ %name @ "\n");
            %script = %script @ "%guiContent = new " @ %type @ "(" @ %name @ ") {\n";

            %editorExtents = EWorldEditor.getExtent();//FIX: I'm assuming all guis are part of the world editor,
            %editorWidth = getWord(%editorExtents,0);//       which is not a good assumption.
            %editorHeight = getWord(%editorExtents,1);
            %pos_x = %pos_x * %editorWidth;
            %pos_y = %pos_y * %editorHeight;
            %results[%currentCounter,16] = %pos_x; //Oh, this is a little awkward... in the DB, I set up pos x/y
            %results[%currentCounter,17] = %pos_y; // to be in percentages, ie float from {0.0,1.0}, but after 
                                                   // we start writing to the buffer these are in pixels.
         
            %script = %script @ "   position = \"" @ mFloor(%pos_x) @ " " @ mFloor(%pos_y) @ "\";\n";
            %script = %script @ "   extent = \"" @ %width @ " " @ %height @ "\";\n";
            %script = %script @ "   text = \"" @ %name @ "\";\n";
            %script = %script @ "   canClose = \"1\";\n";
            %script = %script @ "   closeCommand = \"" @ %name @ ".setVisible(false);\";\n\n";
            
            %results[%currentCounter,%c] = true;//Set finished marker to true, we're done with this one.
         }
      } 
      else if (!%results[%currentCounter,%c])
      {  //We are not the top form, so we need to find out our anchor situation.
         echo("adding element: " @ %currentElement @ " left " @ %left_anchor @ " right " @ %right_anchor @ 
            " top " @ %top_anchor @ " bottom " @ %bottom_anchor @  " parent id " @ %parent_id @ "\n");
            
         //First, find parent container padding numbers.         
         %i = 0;
         %container_width = 0;
         %container_height = 0;
         %container_pos_x = 0;
         %container_pos_y = 0;         
         %container_horiz_padding = 0;
         %container_vert_padding = 0;
         %container_horiz_edge_padding = 0;
         %container_vert_edge_padding = 0;
         while (%i < %count)
         {
            if (%results[%i,0] == %parent_id)
            {
               %container_width = %results[%i,3];
               %container_height = %results[%i,4];
               %container_pos_x = %results[%i,16];
               %container_pos_y = %results[%i,17];
               %container_horiz_padding = %results[%i,18];
               %container_vert_padding = %results[%i,19];
               %container_horiz_edge_padding = %results[%i,20];
               %container_vert_edge_padding = %results[%i,21];              
            }
            %i++;
         }
         
         %undefined = false;
         %horiz_anchor_flip = false;
         if (%left_anchor < 0)
         {
            %horiz_anchor_flip = true;
            %left_anchor *= -1;
         }
         if (%right_anchor < 0)
         {
            %horiz_anchor_flip = true;
            %right_anchor *= -1;
         }
         if (%left_anchor == %parent_id)
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {    
                     %pos_x = %container_pos_x + %container_horiz_edge_padding + %horiz_padding;      
                  } else { 
                     %pos_x = %container_horiz_edge_padding + %horiz_padding;
                  }
               }
               %i++;
            }               
         }
         else if (%left_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %left_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_x = %results[%i,16];
                  %anchor_width = %results[%i,3];
                  if (%horiz_anchor_flip == false)
                     %pos_x = %anchor_pos_x + %anchor_width + %container_horiz_padding + %horiz_padding;
                  else
                     %pos_x = %anchor_pos_x + %horiz_padding;
                  
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;
         } 
         else if (%right_anchor == %parent_id) 
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {
                     %pos_x = %container_pos_x + %container_width - %width - %container_horiz_edge_padding - %horiz_padding;                     
                  } else {
                     %pos_x = %container_width - %width - %container_horiz_edge_padding - %horiz_padding;
                  }
               }
               %i++;
            }
         } 
         else if (%right_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %right_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_x = %results[%i,16];
                  %anchor_width = %results[%i,3];
                  if (%horiz_anchor_flip == false)
                     %pos_x = %anchor_pos_x - %width - %container_horiz_padding - %horiz_padding;
                  else
                     %pos_x = %anchor_pos_x + %anchor_width - %width - %horiz_padding;
                  
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;
         }

         ////// top/bottom anchors //////////////
         %vert_anchor_flip = false;
         if (%top_anchor < 0)
         {
            %vert_anchor_flip = true;
            %top_anchor *= -1;
         }
         if (%bottom_anchor < 0)
         {
            %vert_anchor_flip = true;
            %bottom_anchor *= -1;
         }
         if (%top_anchor == %parent_id) //check for top anchor first, then bottom.
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {
                     %pos_y = %container_pos_y + %container_vert_edge_padding + %vert_padding;  
                  } else {
                     %pos_y = %container_vert_edge_padding + %vert_padding;
                  }
               }
               %i++;
            }      
         }
         else if (%top_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %top_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_y = %results[%i,17];
                  %anchor_height = %results[%i,4];
                  if (%vert_anchor_flip == false)
                     %pos_y = %anchor_pos_y + %anchor_height + %container_vert_padding + %vert_padding;
                  else
                     %pos_y = %anchor_pos_y + %vert_padding;
                  
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;            
         }   
         else if (%bottom_anchor == %parent_id) 
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {
                     %pos_y = %container_pos_y + %container_height - %height - %container_vert_edge_padding - %vert_padding; 
                  } else {
                     %pos_y = %container_height - %height - %container_vert_edge_padding - %vert_padding;
                  }
               }
               %i++;
            }             
         } 
         else if (%bottom_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %bottom_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_y = %results[%i,17];
                  %anchor_height = %results[%i,4];
                  if (%vert_anchor_flip == false)
                     %pos_y = %anchor_pos_y - %height - %container_vert_padding - %vert_padding;
                  else
                     %pos_y = %anchor_pos_y + %anchor_height - %height - %vert_padding;
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;
         }
         
         if (%undefined)
         {
            //This should never happen, we wouldn't be here if we hadn't found our anchors already below.
            echo("OOPS! Gui control has undefined anchors when it is supposed to be a parent! " @ %currentElement);
            return;
         }
         
         //Save these back to the results array so other controls can find them.
         %results[%currentCounter,16] = %pos_x; 
         %results[%currentCounter,17] = %pos_y; 
         //echo("parent control " @ %currentElement @ " found a position: " @ %pos_x @ " " @ %pos_y );
         
         if (%type !$= "Virtual")
         {  
            %script = %script @ %indent @ "new " @ %type @ "() {\n";
            %script = %script @ %indent @ "   position = \"" @ mFloor(%pos_x) @ " " @ mFloor(%pos_y) @ "\";\n";
            %script = %script @ %indent @ "   extent = \"" @ %width @ " " @ %height @ "\";\n";
            if (strlen(%name)>0) %script = %script @ %indent @ "   internalName = \"" @ %name @ "\";\n";            
            if (strlen(%bitmap_path)>0) %script = %script @ %indent @ "   bitmap = \"" @ %bitmap_path @ "\";\n"; 
            if (strlen(%profile)>0) %script = %script @ %indent @ "   profile = \"" @ %profile @ "\";\n"; 
            %script = %script @ "\n";
         }
         %results[%currentCounter,%c] = true;//Whether or not we're virtual, don't come back here.
      }
      
      //////////////////////////////////////////////////////////////////////////////////////////////////////////
      //////////////////////////////////////////////////////////////////////////////////////////////////////////
      //Next, run through the children.      
      if ((%container_type !$= "Virtual")&&(%layers[%layerCount-1,2]==false))
      {//AH, but this needs to *not* add a new indent if we are coming back here after the first time.
         %indent = %indent @ "   ";  
         %layers[%layerCount-1,2] = true;
      }
      for (%k=0;%k<%currentChildCount;%k++)
      {
         %childCounter = 0;
         for (%d=0;%d<%count;%d++)
            if (%results[%d,0]==%currentChildren[%k])
               %childCounter = %d;
               
         //First, change the database's returned "NULL" values into actual null strings.
         for (%d=0;%d<%c;%d++)
            if (%results[%childCounter,%d] $= "NULL") 
               %results[%childCounter,%d] = "";
               
         //Then, check for finished flag.
         if (%results[%childCounter,%c])
            continue;
         
         ///////////////////////////////
         
         //Now, check this child for its own children. If so, save current parent to %layers and increment %layerCount.
         %subfinished = true;
         %newChildCount = 0;
         for (%j=1;%j<%count;%j++)//Search through whole array of all results. (Except don't start at zero, that's always the form.)
         {
            if (%results[%j,1]==%currentChildren[%k]) // 1 = parent_id
            {
               %newChildCount++;
            }
         }
         if (%newChildCount>0)
         {
            %layers[%layerCount,0] = %currentElement; 
            %layers[%layerCount,1] = %indent;       
            %layers[%layerCount,2] = false;      
            %currentElement = %currentChildren[%k];
            %k = %currentChildCount; //Go to the end, exit loop.
            %subfinished = false;
            %layerCount++;
            continue;
         } ////////// Full stop if children found. Exit loop and start over, one layer deeper. //////////
            
         ///////////////////////////////
         //Now, if this is a leaf node, go ahead and render it and set finished=true;

         //First, give results some variable names.
         %id = %results[%childCounter,0];
         %parent_id = %results[%childCounter,1];
         %name = %results[%childCounter,2];
         %width = %results[%childCounter,3];
         %height = %results[%childCounter,4];               
         %type = %results[%childCounter,5]; 
         
         %bitmap_path = %results[%childCounter,6]; 
         %left_anchor = %results[%childCounter,7]; 
         %right_anchor = %results[%childCounter,8];
         %top_anchor = %results[%childCounter,9];
         %bottom_anchor = %results[%childCounter,10];

         %content = %results[%childCounter,11];
         %command = %results[%childCounter,12];
         %tooltip = %results[%childCounter,13];

         %horiz_align = %results[%childCounter,14];
         %vert_align = %results[%childCounter,15];
         
         %pos_x = %results[%childCounter,16]; 
         %pos_y = %results[%childCounter,17]; 

         %horiz_padding = %results[%childCounter,18];
         %vert_padding = %results[%childCounter,19];
         %horiz_edge_padding = %results[%childCounter,20];
         %vert_edge_padding = %results[%childCounter,21];

         %variable = %results[%childCounter,22];
         %button_type = %results[%childCounter,23];
         %group_num = %results[%childCounter,24];
         %profile = %results[%childCounter,25];
         %value = %results[%childCounter,26];
             
         echo("reading element, name " @ %name @ " type " @ %type @ " id " @ %id );
         //And, UNFORTUNATELY, we (currently) need to repeat the whole block of anchor logic again here. 
         //Edit: REALLY?? Some way to refactor this MUST be possible. Out of time at the moment however.
         //Really not sure how to fix it. If TorqueScript had goto at least... but, nope.
         ///////////////////////////  BEGIN NASTY BLOCK OF REPEATED CODE /////////////////////////
         %i = 0;
         %container_width = 0;
         %container_height = 0;
         %container_pos_x = 0;
         %container_pos_y = 0;         
         %container_horiz_padding = 0;
         %container_vert_padding = 0;
         %container_horiz_edge_padding = 0;
         %container_vert_edge_padding = 0;
         while (%i < %count)
         {
            if (%results[%i,0] == %parent_id)
            {
               %container_width = %results[%i,3];
               %container_height = %results[%i,4];
               %container_pos_x = %results[%i,16];
               %container_pos_y = %results[%i,17];
               %container_horiz_padding = %results[%i,18];
               %container_vert_padding = %results[%i,19];
               %container_horiz_edge_padding = %results[%i,20];
               %container_vert_edge_padding = %results[%i,21];              
            }
            %i++;
         }
         
         %undefined = false;
         %horiz_anchor_flip = false;
         if (%left_anchor < 0)
         {
            %horiz_anchor_flip = true;
            %left_anchor *= -1;
         }
         if (%right_anchor < 0)
         {
            %horiz_anchor_flip = true;
            %right_anchor *= -1;
         }
         if (%left_anchor == %parent_id)
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {    
                     %pos_x = %container_pos_x + %container_horiz_edge_padding + %horiz_padding;      
                  } else { 
                     %pos_x = %container_horiz_edge_padding + %horiz_padding;
                  }
               }
               %i++;
            }               
         }
         else if (%left_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %left_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_x = %results[%i,16];
                  %anchor_width = %results[%i,3];
                  if (%horiz_anchor_flip == false)
                     %pos_x = %anchor_pos_x + %anchor_width + %container_horiz_padding + %horiz_padding;
                  else
                     %pos_x = %anchor_pos_x + %horiz_padding;
                  
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;
         } 
         else if (%right_anchor == %parent_id) 
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {
                     %pos_x = %container_pos_x + %container_width - %width - %container_horiz_edge_padding - %horiz_padding;                     
                  } else {
                     %pos_x = %container_width - %width - %container_horiz_edge_padding - %horiz_padding;
                  }
               }
               %i++;
            }
         } 
         else if (%right_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %right_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_x = %results[%i,16];
                  %anchor_width = %results[%i,3];
                  if (%horiz_anchor_flip == false)
                     %pos_x = %anchor_pos_x - %width - %container_horiz_padding - %horiz_padding;
                  else
                     %pos_x = %anchor_pos_x + %anchor_width - %width - %horiz_padding;
                  
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;
         }

         ////// top/bottom anchors //////////////
         %vert_anchor_flip = false;
         if (%top_anchor < 0)
         {
            %vert_anchor_flip = true;
            %top_anchor *= -1;
         }
         if (%bottom_anchor < 0)
         {
            %vert_anchor_flip = true;
            %bottom_anchor *= -1;
         }
         if (%top_anchor == %parent_id) //check for top anchor first, then bottom.
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {
                     %pos_y = %container_pos_y + %container_vert_edge_padding + %vert_padding;  
                  } else {
                     %pos_y = %container_vert_edge_padding + %vert_padding;
                  }
               }
               %i++;
            }      
         }
         else if (%top_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %top_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_y = %results[%i,17];
                  %anchor_height = %results[%i,4];
                  if (%vert_anchor_flip == false)
                     %pos_y = %anchor_pos_y + %anchor_height + %container_vert_padding + %vert_padding;
                  else
                     %pos_y = %anchor_pos_y + %vert_padding;
                  
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;            
         }   
         else if (%bottom_anchor == %parent_id) 
         {
            %i = 0;
            while (%i < %count)
            {
               if (%results[%i,0] == %parent_id)
               {
                  if (%results[%i,5] $= "Virtual")
                  {
                     %pos_y = %container_pos_y + %container_height - %height - %container_vert_edge_padding - %vert_padding; 
                  } else {
                     %pos_y = %container_height - %height - %container_vert_edge_padding - %vert_padding;
                  }
               }
               %i++;
            }             
         } 
         else if (%bottom_anchor > %parent_id)
         {
            %i = 0;
            %found = false;
            while (%i < %count)
            {
               if ((%results[%i,0] == %bottom_anchor) && (%results[%i,%c]))
               {
                  %anchor_pos_y = %results[%i,17];
                  %anchor_height = %results[%i,4];
                  if (%vert_anchor_flip == false)
                     %pos_y = %anchor_pos_y - %height - %container_vert_padding - %vert_padding;
                  else
                     %pos_y = %anchor_pos_y + %anchor_height - %height - %vert_padding;
                  %found = true;
               }
               %i++;  
            }
            if ( !%found )
               %undefined = true;
         }
         ////////////////////////////  END NASTY BLOCK OF REPEATED CODE //////////////////////////
         
         if (%undefined) //Bail if we are missing anchors, we will check again later.
         {
            echo("control " @ %currentChildren[%k] @ " is currently undefined. Skipping for now.");
            continue;
         }
         %results[%childCounter,16] = %pos_x; 
         %results[%childCounter,17] = %pos_y; 
         echo("Element " @ %currentChildren[%k] @ " found a position: " @ %pos_x @ " " @ %pos_y);

         if (%type !$= "Virtual") //Should never happen, but if someone accidentally adds a Virtual and doesn't
         {                       //give it any children, we still need to not render it.
            %script = %script @ %indent @ "new " @ %type @ "() {\n";
            %script = %script @ %indent @ "   position = \"" @ mFloor(%pos_x) @ " " @ mFloor(%pos_y) @ "\";\n";
            %script = %script @ %indent @ "   extent = \"" @ %width @ " " @ %height @ "\";\n";
            if (strlen(%content)>0) %script = %script @ %indent @ "   text = \"" @ %content @ "\";\n";
            if (strlen(%name)>0) %script = %script @ %indent @ "   internalName = \"" @ %name @ "\";\n";            
            if (strlen(%command)>0) %script = %script @ %indent @ "   command = \"" @ %command @ "\";\n";
            if (strlen(%tooltip)>0) %script = %script @ %indent @ "   tooltip = \"" @ %tooltip @ "\";\n"; 
            if (strlen(%tooltip)>0) %script = %script @ %indent @ "   tooltipprofile = \"GuiToolTipProfile\";\n";
            if (strlen(%bitmap_path)>0) %script = %script @ %indent @ "   bitmap = \"" @ %bitmap_path @ "\";\n"; 
            if (strlen(%variable)>0) %script = %script @ %indent @ "   variable = \"" @ %variable @ "\";\n"; 
            if (strlen(%button_type)>0) %script = %script @ %indent @ "   buttonType = \"" @ %button_type @ "\";\n"; 
            if (strlen(%group_num)>0) %script = %script @ %indent @ "   groupNum = \"" @ %group_num @ "\";\n"; 
            if (strlen(%profile)>0) %script = %script @ %indent @ "   profile = \"" @ %profile @ "\";\n"; 
            %script = %script @ %indent @ "};\n";
            %results[%childCounter,%c] = true; 
         }
         
         //Clear all, so you don't end up with the last control's tooltip because this one doesn't have one...
         %width = "";
         %height = "";
         %left_anchor = "";
         %right_anchor = "";
         %top_anchor = "";
         %bottom_anchor = "";
         %name = "";
         %type = "";
         %content = "";
         %command = "";
         %tooltip = "";
         %bitmap_path = "";
         %variable = "";
         %button_type = "";
         %group_num = "";
         %value = "";
         %profile = "";
         
      }//end of for (0..%currentChildCount) loop.
      
      //Now, double check, did we finish everybody?
      //Two ways we can not be done: either we were sent out of the loop because we need to go into a deeper
      //child layer, or we have children we passed by earlier because of undefined anchors.
      //This is for checking the second case. 
      for (%k=0;%k<%currentChildCount;%k++)
      {
         %childCounter = 0;
         for (%d=0;%d<%count;%d++)
            if (%results[%d,0]==%currentChildren[%k])
               %childCounter = %d;
               
          if (!%results[%childCounter,%c])
            %subfinished = false;               
      }
      
      //But now, if subfinished is still true, that means we are done with this container, go back up.
      if (%subfinished) //Only do this if we made it to the end of the children.
      {    
         %layerCount--;
         %currentElement = %layers[%layerCount,0];//-1, or just %layerCount?
         %indent = %layers[%layerCount,1];       
         if (%container_type !$= "Virtual")
         {
            %script = %script @ %indent @ "};\n";
         }       
      }
      
      //Finally: run through the whole list, and if type is null for every control, we're DONE done.
      for (%k=0;%k<%count;%k++)
      {
         if (!%results[%k,%c])
         {//exit with finished=false the first time we find a valid type.
            %k = %count;
            continue;
         }
         if (%k==(%count-1))
         {
            echo("finished main loop with all controls defined, sanityCount " @ %sanityCount);
            %finished=true;
         }
      }
   }
   
   //%script = %script @ "};\n";
   
   
   //And then, save it out as a .gui file.
   %filename = %formname @ ".gui";
   %fileObject = new FileObject();
   %fileObject.openForWrite( %filename ); 
   %fileObject.writeLine(%script);
   %fileObject.close();
   %fileObject.delete();
      
   echo("\n\n SCRIPT:\n\n" @ %script);
   echo("finished GUI: sanityCount " @ %sanityCount);
   
   //////////////////////////////
   eval(%script);  //DO IT!
   //////////////////////////////
   
   EWorldEditor.add(%formname); 
}


////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
 
 
//OBSOLETE
function makeXmlGuiChildren(%xml,%parent_id)
{
   //NEXT: get names for anchors, as additional JOINs. Unfortunately, because the sqlite implementation  
   //doesn't support "la.name" syntax, now I have to list all fields and give the joined ones aliases.
   %query = "SELECT e.id,e.parent_id,e.bitmap_id,e.left_anchor,e.right_anchor,e.top_anchor,e.bottom_anchor,e.type," @
            "e.content,e.name,e.width,e.height,e.command,e.tooltip,e.horiz_align,e.vert_align,e.pos_x,e.pos_y,e.horiz_padding," @
            "e.vert_padding,e.horiz_edge_padding,e.vert_edge_padding,e.variable,e.button_type,e.group_num,e.profile," @
            "e.value,la.name as la_name,ra.name as ra_name,ta.name as ta_name,ba.name as ba_name,b.path " @
            "FROM uiElement e " @ 
	         "LEFT JOIN uiBitmap b ON b.id=e.bitmap_id " @ 
	         "LEFT JOIN uiElement la ON la.id=e.left_anchor " @ 
	         "LEFT JOIN uiElement ra ON ra.id=e.right_anchor " @ 
	         "LEFT JOIN uiElement ta ON ta.id=e.top_anchor " @ 
	         "LEFT JOIN uiElement ba ON ba.id=e.bottom_anchor " @ 
	         "WHERE e.parent_id=" @ %parent_id @ ";"; 

   %resultSet = sqlite.query(%query, 0);
   if (%resultSet)
   {
      echo("makeSqlGuiChildren found " @ sqlite.numRows(%resultSet) @ " children for parent " @ %parent_id );
      while (!sqlite.endOfResult(%resultSet))
      {
         %undefined = false;
         
         echo("hoping I have a left anchor name? " @ sqlite.getColumn(%resultSet, "la_name"));
         
         %horiz_anchor_flip = false;//These track whether either of our anchors are "flipped" - e.g. a negative sign 
         %vert_anchor_flip = false;//in the left_anchor means we use the _left_ edge of the anchor control to align
         //the left edge of this control, instead of aligning the left edge of this control to the right edge of the anchor. 
         
         %control_id = sqlite.getColumn(%resultSet, "id");//We need this to call this function recursively.
         %name = sqlite.getColumn(%resultSet, "name");// and they're reasonably short fields.
         %width = sqlite.getColumn(%resultSet, "width");//Width and height may not actually be required, if/when 
         %height = sqlite.getColumn(%resultSet, "height");// we implement stretching controls between anchors.
      
         %c = 0;
         %results[%c,0] = sqlite.getColumn(%resultSet, "type"); %results[%c,1] = "type"; %c++;//We do need a required field for the first one,
         %results[%c,0] = sqlite.getColumn(%resultSet, "path"); %results[%c,1] = "bitmap"; %c++;//because pushNewElement/addNewElement.

         %results[%c,0] = sqlite.getColumn(%resultSet, "la_name"); %results[%c,1] = "left_anchor"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "ra_name"); %results[%c,1] = "right_anchor"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "ta_name"); %results[%c,1] = "top_anchor"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "ba_name"); %results[%c,1] = "bottom_anchor"; %c++;

         %results[%c,0] = sqlite.getColumn(%resultSet, "content"); %results[%c,1] = "content"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "command"); %results[%c,1] = "command"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "tooltip"); %results[%c,1] = "tooltip"; %c++;

         %results[%c,0] = sqlite.getColumn(%resultSet, "horiz_align"); %results[%c,1] = "horiz_align"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "vert_align"); %results[%c,1] = "vert_align"; %c++;
         
         %results[%c,0] = sqlite.getColumn(%resultSet, "pos_x"); %results[%c,1] = "pos_x"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "pos_y"); %results[%c,1] = "pos_y"; %c++;

         %results[%c,0] = sqlite.getColumn(%resultSet, "horiz_padding"); %results[%c,1] = "horiz_padding"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "vert_padding"); %results[%c,1] = "vert_padding"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "horiz_edge_padding"); %results[%c,1] = "horiz_edge_padding"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "vert_edge_padding"); %results[%c,1] = "vert_edge_padding"; %c++;

         %results[%c,0] = sqlite.getColumn(%resultSet, "variable"); %results[%c,1] = "variable"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "button_type"); %results[%c,1] = "button_type"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "group_num"); %results[%c,1] = "group_num"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "profile"); %results[%c,1] = "profile"; %c++;
         %results[%c,0] = sqlite.getColumn(%resultSet, "value"); %results[%c,1] = "value"; %c++;
         
         echo("reading ui element " @ %control_id  @ " command: " @ %results[7,0]);
               
         if ((%name $= "NULL")||(%results[0,0] $= "NULL")||
               (strlen(%name)==0)||(strlen(%results[0,0])==0))
         {
            echo("Gui form is missing either name: " @ %name @ ", or type: " @ %results[0,0] @ ".");   
            return;
         }   
         
         %xml.addNewElement("element");
         %xml.setAttribute("name",%name);
         if (%width $= "NULL") %width = "";
         %xml.setAttribute("width",%width);
         if (%height $= "NULL") %height = "";
         %xml.setAttribute("height",%height);
         
         for (%d=0;%d<%c;%d++)
         {
            if (%results[%d,0] $= "NULL") %results[%d,0] = "";
               
            if (strlen(%results[%d,0]) > 0)
            {
               //strreplace(%results[%d,0],"&quot","\"");//Hm, maybe &quot is how the quotes have to be in XML?
               if (%d==0) //This is why we put type in the array, so pushNewElement will always happen first.
                  %xml.pushNewElement(%results[%d,1]);
               else       //Afterward they are all addNewElement.
                  %xml.addNewElement(%results[%d,1]);
                  
               %xml.addData(%results[%d,0]);
            }
         }
         
         %xml.popElement();
         
         sqlite.nextRow(%resultSet);
      }
   }
}


/////////////////// AND, the old way, soon to be delete forever. /////////////////////////////


function makeSqlGuiFormObsolete(%control_id)
{
   echo("trying to load gui! " @ %control_id);  
   
   $controlCount = 0;
   
   $undefinedCount = 0; //This keeps track of controls that need
   $undefinedList = ""; //anchors which have not been created yet.
   //Note: remember that TorqueScript arrays are just variables, so there is no way to clear
   //the whole array in one line like it looks like I'm doing here. Shouldn't matter because
   //we'll be refilling it with only what we need and accessing only those values every time.
   
   %script = "";
   %indent = "   ";//Three-space indents, increment and decrement this as we go down the tree.

   //SO, first we need to select the parent control, with control_id. Then, we need a 
   //recursive algorithm to select all the children of this parent, and then all the children of 
   //those children, until we come up with no children. Would be snazzy to fit this all into 
   //one query, but reality is the speed gain will probably be totally undetectable, as long as 
   //we're not doing this every frame on resize.
   %query = "SELECT * FROM uiElement e " @ 
	         "LEFT JOIN uiBitmap b ON b.id=e.bitmap_id " @ 
	         "WHERE e.id=" @ %control_id @ ";";  
	%result = sqlite.query(%query, 0);
	
   echo( "Query: " @ %query );
	
   if (%result)
   {
      %bitmap_path = sqlite.getColumn(%result, "path");
      %type = sqlite.getColumn(%result, "type");
      %name = sqlite.getColumn(%result, "name");
      %width = sqlite.getColumn(%result, "width");
      %height = sqlite.getColumn(%result, "height");
      %horiz_align = sqlite.getColumn(%result, "horiz_align");
      %vert_align = sqlite.getColumn(%result, "vert_align");
      %pos_x = sqlite.getColumn(%result, "pos_x");
      %pos_y = sqlite.getColumn(%result, "pos_y");
      %horiz_padding = sqlite.getColumn(%result, "horiz_padding");
      %vert_padding = sqlite.getColumn(%result, "vert_padding");
      %horiz_edge_padding = sqlite.getColumn(%result, "horiz_edge_padding");
      %vert_edge_padding = sqlite.getColumn(%result, "vert_edge_padding");
   }
   
   if (strlen(%type)>0) //New way: to hell with a massive table of types, I'm just going to 
   {//use the actual Torque names of the classes here. 
   //Advantage: I can add new ones just by typing them in. And browsing the DB is informative.
   //Disadvantage:  I am locking myself to Torque names. But, later for other systems I can convert from
   //  them in a table just like I would with ints.
   
      %script = %script @ "%guiContent = new " @ %type @ "(" @ %name @ ") {\n";
   } 
   else 
      return;

   %editorExtents = EWorldEditor.getExtent();
   %editorWidth = getWord(%editorExtents,0);
   %editorHeight = getWord(%editorExtents,1);
   
   %pos_x = %pos_x * %editorWidth;
   %pos_y = %pos_y * %editorHeight;
   
   %script = %script @ "   position = \"" @ mFloor(%pos_x) @ " " @ mFloor(%pos_y) @ "\";\n";
   %script = %script @ "   extent = \"" @ %width @ " " @ %height @ "\";\n";
   %script = %script @ "   text = \"" @ %name @ "\";\n";
   %script = %script @ "   canClose = \"1\";\n";
   %script = %script @ "   closeCommand = \"" @ %name @ ".setVisible(false);\";\n\n";
   
   
   $controls[$controlCount,0] = %control_id;
   $controls[$controlCount,1] = %pos_x; $controls[$controlCount,2] = %pos_y;
   $controls[$controlCount,3] = %width; $controls[$controlCount,4] = %height;
   $controls[$controlCount,5] = false;//Base window can't be virtual.
   $controls[$controlCount,6] = %type;//For filling uiElement list objects.
   $controls[$controlCount,7] = %name;
   $controlCount++;

   /////////////
   %script = %script @ makeSqlGuiChildren(%control_id,%width,%height,%horiz_padding,%vert_padding,%horiz_edge_padding,%vert_edge_padding,%indent);
   /////////////
   if ($undefinedCount > 0)
      %script = %script @ makeSqlUndefChildren();
   /////////////
   
   %script = %script @ "};\n";
   
   echo(%script);
   
   //////////////////////////////
   eval(%script);  //DO IT!
   //////////////////////////////
   
   //And then, save it out as a .gui file.
   %filename = %name @ ".gui";
   %fileObject = new FileObject();
   %fileObject.openForWrite( %filename ); 
   %fileObject.writeLine(%script);
   %fileObject.close();
   %fileObject.delete();
      
   EWorldEditor.add(%name);   
}

function makeSqlGuiChildren(%parent_id,%parent_width,%parent_height,%parent_horiz_padding,
                     %parent_vert_padding,%parent_horiz_edge_padding,%parent_vert_edge_padding,%indent)
{//RECURSIVE - so make sure we don't use any globals.

   
   %script = "";
   
   %query = "SELECT * FROM uiElement e " @ 
	         "LEFT JOIN uiBitmap b ON b.id=e.bitmap_id " @ 
	         "WHERE e.parent_id=" @ %parent_id @ ";"; 

   %result = sqlite.query(%query, 0);
   
   if (%result)
   {
      echo("makeSqlGuiChildren found " @ sqlite.numRows(%result) @ " children for parent " @ %parent_id );
      while (!sqlite.endOfResult(%result))
      {
         %undefined = false;
         
         %horiz_anchor_flip = false;//These track whether either of our anchors are "flipped" - e.g. a negative sign 
         %vert_anchor_flip = false;//in the left_anchor means we use the _left_ edge of the anchor control to align
         //the left edge of this control, instead of aligning the left edge of this control to the right edge of the anchor. 
         
         %control_id = sqlite.getColumn(%result, "id");
         %bitmap_path = sqlite.getColumn(%result, "path");
         %left_anchor = sqlite.getColumn(%result, "left_anchor");
         %right_anchor = sqlite.getColumn(%result, "right_anchor");
         %top_anchor = sqlite.getColumn(%result, "top_anchor");
         %bottom_anchor = sqlite.getColumn(%result, "bottom_anchor");
         %type = sqlite.getColumn(%result, "type");
         %content = sqlite.getColumn(%result, "content");
         %name = sqlite.getColumn(%result, "name");
         %width = sqlite.getColumn(%result, "width");
         %height = sqlite.getColumn(%result, "height");
         %command = sqlite.getColumn(%result, "command");
         %tooltip = sqlite.getColumn(%result, "tooltip");
         %horiz_align = sqlite.getColumn(%result, "horiz_align");
         %vert_align = sqlite.getColumn(%result, "vert_align");
         %pos_x = sqlite.getColumn(%result, "pos_x");
         %pos_y = sqlite.getColumn(%result, "pos_y");
         %horiz_padding = sqlite.getColumn(%result, "horiz_padding");
         %vert_padding = sqlite.getColumn(%result, "vert_padding");
         %horiz_edge_padding = sqlite.getColumn(%result, "horiz_edge_padding");
         %vert_edge_padding = sqlite.getColumn(%result, "vert_edge_padding");
         %variable = sqlite.getColumn(%result, "variable");
         %button_type = sqlite.getColumn(%result, "button_type");
         %group_num = sqlite.getColumn(%result, "group_num");
         %profile = sqlite.getColumn(%result, "profile");
         echo("reading ui element " @ %control_id);
         if (strlen(%type)>0) 
         {
            ////// left/right anchors //////////////
            if (%left_anchor < 0)
            {
               %horiz_anchor_flip = true;
               %left_anchor *= -1;
            }
            if (%right_anchor < 0)
            {
               %horiz_anchor_flip = true;
               %right_anchor *= -1;
            }
            if (%left_anchor == %parent_id) //check for left anchor first, then right.
            {//If parent is Virtual, then we add its position, otherwise restart at zero.
               %i = 0;
               %foundVirtual = false;
               while (%i < $controlCount)
               {
                  if ($controls[%i,0] == %parent_id)
                  {
                     if ($controls[%i,5])
                     {
                        %pos_x = $controls[%i,1] + %parent_horiz_edge_padding + %horiz_padding;  
                        echo("found virtual parent! pos_x " @  %pos_x );                
                        %foundVirtual = true;
                     }
                  }
                  %i++;
               }
               if (!%foundVirtual)
                  %pos_x = %parent_horiz_edge_padding + %horiz_padding;
            }
            else if (%left_anchor > %parent_id)//Parents and previous siblings MUST be ahead in database order.
            {
               %i = 0;
               %found = false;
               while (%i < $controlCount)
               {
                  if ($controls[%i,0] == %left_anchor)
                  {
                     if (%horiz_anchor_flip == false)
                        %pos_x = $controls[%i,1] + $controls[%i,3] + %parent_horiz_padding + %horiz_padding;
                     else
                        %pos_x = $controls[%i,1] + %horiz_padding;
                     
                     %found = true;
                  }
                  %i++;  
               }
               if ( !%found && !%undefined )
               {
                  $undefinedList[$undefinedCount,0] = %control_id;
                  $undefinedList[$undefinedCount,1] = %indent;
                  $undefinedList[$undefinedCount,2] = %parent_horiz_padding;
                  $undefinedList[$undefinedCount,3] = %parent_vert_padding;
                  $undefinedList[$undefinedCount,4] = %parent_horiz_edge_padding;
                  $undefinedList[$undefinedCount,5] = %parent_vert_edge_padding;
                  $undefinedCount++;
                  %undefined = true;                  
               }
            } 
            else if (%right_anchor == %parent_id) 
            {
               %i = 0;
               %foundVirtual = false;
               while (%i < $controlCount)
               {
                  if ($controls[%i,0] == %parent_id)
                  {
                     if ($controls[%i,5])
                     {
                        %pos_x = $controls[%i,1] + $controls[%i,3] - %width - %parent_horiz_edge_padding - %horiz_padding;                     
                        %foundVirtual = true;
                     }
                  }
                  %i++;
               }
               if (!%foundVirtual)
                  %pos_x = %parent_width - %width - %parent_horiz_edge_padding - %horiz_padding;
            } 
            else if (%right_anchor > %parent_id)
            {
               %i = 0;
               %found = false;
               while (%i < $controlCount)
               {
                  if ($controls[%i,0] == %right_anchor)
                  {
                     if (%horiz_anchor_flip == false)
                        %pos_x = $controls[%i,1] - %width - %parent_horiz_padding - %horiz_padding;
                     else
                        %pos_x = $controls[%i,1] + $controls[%i,3] - %width - %horiz_padding;
                     
                     %found = true;
                  }
                  %i++;  
               }
               if ( !%found && !%undefined )
               {
                  $undefinedList[$undefinedCount,0] = %control_id;
                  $undefinedList[$undefinedCount,1] = %indent;
                  $undefinedList[$undefinedCount,2] = %parent_horiz_padding;
                  $undefinedList[$undefinedCount,3] = %parent_vert_padding;
                  $undefinedList[$undefinedCount,4] = %parent_horiz_edge_padding;
                  $undefinedList[$undefinedCount,5] = %parent_vert_edge_padding;
                  $undefinedCount++;
                  %undefined = true;                  
               }
            }

            ////// top/bottom anchors //////////////
            if (%top_anchor < 0)
            {
               %vert_anchor_flip = true;
               %top_anchor *= -1;
            }
            if (%bottom_anchor < 0)
            {
               %vert_anchor_flip = true;
               %bottom_anchor *= -1;
            }
            if (%top_anchor == %parent_id) //check for top anchor first, then bottom.
            {
               %i = 0;
               %foundVirtual = false;
               while (%i < $controlCount)
               {
                  if ($controls[%i,0] == %parent_id)
                  {
                     if ($controls[%i,5])
                     {
                        %pos_y = $controls[%i,2] + %parent_vert_edge_padding + %vert_padding;                     
                        %foundVirtual = true;
                     }
                  }
                  %i++;
               }
               if (!%foundVirtual)               
                  %pos_y = %parent_vert_edge_padding + %vert_padding;
            }
            else if (%top_anchor > %parent_id)
            {
               %i = 0;
               %found = false;
               while (%i < $controlCount)
               {
                  if ($controls[%i,0] == %top_anchor)
                  {
                     if (%vert_anchor_flip == false)
                        %pos_y = $controls[%i,2] + $controls[%i,4] + %parent_vert_padding + %vert_padding;
                     else
                        %pos_y = $controls[%i,2] + %vert_padding;
                     
                     %found = true;
                  }
                  %i++;  
               }
               if ( !%found && !%undefined )
               {
                  $undefinedList[$undefinedCount,0] = %control_id;
                  $undefinedList[$undefinedCount,1] = %indent;
                  $undefinedList[$undefinedCount,2] = %parent_horiz_padding;
                  $undefinedList[$undefinedCount,3] = %parent_vert_padding;
                  $undefinedList[$undefinedCount,4] = %parent_horiz_edge_padding;
                  $undefinedList[$undefinedCount,5] = %parent_vert_edge_padding;
                  $undefinedCount++;
                  %undefined = true;                  
               }             
            }   
            else if (%bottom_anchor == %parent_id) 
            {
               %i = 0;
               %foundVirtual = false;
               while (%i < $controlCount)
               {
                  if ($controls[%i,0] == %parent_id)
                  {
                     if ($controls[%i,5])
                     {
                        %pos_y = $controls[%i,2] + $controls[%i,4] - %height - %parent_vert_edge_padding - %vert_padding;                     
                        %foundVirtual = true;
                     }
                  }
                  %i++;
               }
               if (!%foundVirtual)  
                  %pos_y = %parent_height - %height - %parent_vert_edge_padding - %vert_padding;
            } 
            else if (%bottom_anchor > %parent_id)
            {
               %i = 0;
               %found = false;
               while (%i < $controlCount)
               {
                  if ($controls[%i,0] == %bottom_anchor)
                  {
                     if (%vert_anchor_flip == false)
                        %pos_y = $controls[%i,2] - %height - %parent_vert_padding - %vert_padding;
                     else
                        %pos_y = $controls[%i,2] + $controls[%i,4] - %height - %vert_padding;
                     %found = true;
                  }
                  %i++;  
               }
               if ( !%found && !%undefined )
               {
                  $undefinedList[$undefinedCount,0] = %control_id;
                  $undefinedList[$undefinedCount,1] = %indent;
                  $undefinedList[$undefinedCount,2] = %parent_horiz_padding;
                  $undefinedList[$undefinedCount,3] = %parent_vert_padding;
                  $undefinedList[$undefinedCount,4] = %parent_horiz_edge_padding;
                  $undefinedList[$undefinedCount,5] = %parent_vert_edge_padding;
                  $undefinedCount++;
                  %undefined = true;                  
               }
            }
            
            if (%undefined == false)
            {
               //Now, write it to the script buffer.
               if (%type !$= "Virtual")
               {
                  %script = %script @ %indent @ "new " @ %type @ "() {\n";
                  %script = %script @ %indent @ "   position = \"" @ mFloor(%pos_x) @ " " @ mFloor(%pos_y) @ "\";\n";
                  %script = %script @ %indent @ "   extent = \"" @ %width @ " " @ %height @ "\";\n";
                  if (%content !$= "NULL") %script = %script @ %indent @ "   text = \"" @ %content @ "\";\n";
                  if (%name !$= "NULL") %script = %script @ %indent @ "   internalName = \"" @ %name @ "\";\n";            
                  if (%command !$= "NULL") %script = %script @ %indent @ "   command = \"" @ %command @ "\";\n";
                  if (%tooltip !$= "NULL") %script = %script @ %indent @ "   tooltip = \"" @ %tooltip @ "\";\n"; 
                  if (%tooltip !$= "NULL") %script = %script @ %indent @ "   tooltipprofile = \"GuiToolTipProfile\";\n";
                  if (%bitmap_path !$= "NULL") %script = %script @ %indent @ "   bitmap = \"" @ %bitmap_path @ "\";\n"; 
                  if (%variable !$= "NULL") %script = %script @ %indent @ "   variable = \"" @ %variable @ "\";\n"; 
                  if (%button_type !$= "NULL") %script = %script @ %indent @ "   buttonType = \"" @ %button_type @ "\";\n"; 
                  if (%group_num !$= "NULL") %script = %script @ %indent @ "   groupNum = \"" @ %group_num @ "\";\n"; 
                  if (%profile !$= "NULL") %script = %script @ %indent @ "   profile = \"" @ %profile @ "\";\n"; 
               }
               
               //Next, store position/extent info in a 2d array, for positioning other elements.           
               $controls[$controlCount,0] = %control_id;
               $controls[$controlCount,1] = %pos_x; $controls[$controlCount,2] = %pos_y;
               $controls[$controlCount,3] = %width; $controls[$controlCount,4] = %height;
               if (%type !$= "Virtual")
                  $controls[$controlCount,5] = false;
               else
                  $controls[$controlCount,5] = true;               
               $controls[$controlCount,6] = %type;
               $controls[$controlCount,7] = %name;
               $controlCount++;
               
               ////// Now, take care of the children. ///////
               if (%type !$= "Virtual")
               {
                  %prev_indent = %indent;
                  %indent = %indent @ "   ";//Make it three spaces deeper before we go to the children...
               }
               
               //And... RECURSE!
               %script = %script @ makeSqlGuiChildren(%control_id,%width,%height,%horiz_padding,%vert_padding,%horiz_edge_padding,%vert_edge_padding,%indent);

               if (%type !$= "Virtual")
               {
                  %indent = %prev_indent;//... and then take them away when we come back. 
                  %script = %script @ %indent @ "};\n";
               }
            }
            ///////////////////////////////////////////////
         }
         sqlite.nextRow(%result);
      }
   }
      
   return %script;
}

function makeSqlUndefChildren()
{
   //Now, the interesting part. We have to keep looping through the undefinedList, skipping over anything that
   //still can't be anchored, but instantiating and removing anything that now has valid anchors.
   
   %sanityCount = 0;
   %script = "";
   echo("makeUndefChildren, count " @ $undefinedCount);
   for (%i=0;%i<$undefinedCount;%i++)
   {
      echo("undefined " @ %i @ " " @ $undefinedList[%i,0] @ " [" @ $undefinedList[%i,1] @ "]" );
   }
   while (($undefinedCount > 0)&&(%sanityCount<500))
   {      
      %sanityCount++;//(Just in case something goes haywire and we end up looping here forever.)
      echo("making undef children, undefined count " @ $undefinedCount @ " sanity " @ %sanityCount );
      //This could totally happen, if the user for instance accidentally makes two controls dependent 
      //on each other, or parents of each other. 
      for (%k=0;%k<$undefinedCount;%k++)
      {
         %control_id = $undefinedList[%k,0];
         %indent = $undefinedList[%k,1];                  
         %parent_horiz_padding = $undefinedList[%k,2];
         %parent_vert_padding = $undefinedList[%k,3];
         %parent_horiz_edge_padding = $undefinedList[%k,4];
         %parent_vert_edge_padding = $undefinedList[%k,5];
         %query = "SELECT * FROM uiElement e " @ 
	         "LEFT JOIN uiBitmap b ON b.id=e.bitmap_id " @ 
	         "WHERE e.id=" @ %control_id @ ";";  
	      %result = sqlite.query(%query, 0);
         if (%result)
         {
            %parent_id = sqlite.getColumn(%result, "parent_id"); 
            %bitmap_path = sqlite.getColumn(%result, "path");
            %left_anchor = sqlite.getColumn(%result, "left_anchor");
            %right_anchor = sqlite.getColumn(%result, "right_anchor");
            %top_anchor = sqlite.getColumn(%result, "top_anchor");
            %bottom_anchor = sqlite.getColumn(%result, "bottom_anchor");
            %type = sqlite.getColumn(%result, "type");
            %content = sqlite.getColumn(%result, "content");
            %name = sqlite.getColumn(%result, "name");
            %width = sqlite.getColumn(%result, "width");
            %height = sqlite.getColumn(%result, "height");
            %command = sqlite.getColumn(%result, "command");
            %tooltip = sqlite.getColumn(%result, "tooltip");
            %horiz_align = sqlite.getColumn(%result, "horiz_align");
            %vert_align = sqlite.getColumn(%result, "vert_align");
            %pos_x = sqlite.getColumn(%result, "pos_x");
            %pos_y = sqlite.getColumn(%result, "pos_y");
            %horiz_padding = sqlite.getColumn(%result, "horiz_padding");
            %vert_padding = sqlite.getColumn(%result, "vert_padding");
            %horiz_edge_padding = sqlite.getColumn(%result, "horiz_edge_padding");
            %vert_edge_padding = sqlite.getColumn(%result, "vert_edge_padding");
            
            %undefined = false;
            sqlite.clearResult(%result);
            
            echo("first undefined: id " @ %control_id @ " parent " @ %parent_id @ " left_anchor " @ %left_anchor @ 
                  " right_anchor " @ %right_anchor @ " top_anchor " @ %top_anchor @ " bottom_anchor " @ %bottom_anchor @ " ");
            
            if (strlen(%type)>0)
            {
               ////// left/right anchors //////////////
               %horiz_anchor_flip = false;
               if (%left_anchor < 0)
               {
                  %horiz_anchor_flip = true;
                  %left_anchor *= -1;
               }
               if (%right_anchor < 0)
               {
                  %horiz_anchor_flip = true;
                  %right_anchor *= -1;
               }
               if (%left_anchor == %parent_id) //check for left anchor first, then right.
               {
                  %i = 0;
                  %foundVirtual = false;
                  while (%i < $controlCount)
                  {
                     if ($controls[%i,0] == %parent_id)
                     {
                        if ($controls[%i,5])
                        {
                           %pos_x = $controls[%i,1] + %parent_horiz_edge_padding + %horiz_padding;                     
                           %foundVirtual = true;
                        }
                     }
                     %i++;
                  }
                  if (!%foundVirtual)
                     %pos_x = %parent_horiz_edge_padding + %horiz_padding;
               }
               else if (%left_anchor > %parent_id)//Parents and previous siblings MUST be ahead in database order.
               {
                  %i = 0;
                  %found = false;
                  while (%i < $controlCount)
                  {
                     if ($controls[%i,0] == %left_anchor)
                     {
                        if (%horiz_anchor_flip == false)
                           %pos_x = $controls[%i,1] + $controls[%i,3] + %parent_horiz_padding + %horiz_padding;
                        else
                           %pos_x = $controls[%i,1] + %horiz_padding;
                        %found = true;
                     }
                     %i++;  
                  }
                  if (!%found)
                  {
                     %undefined = true;                  
                  }
               } 
               else if (%right_anchor == %parent_id) 
               {
                  %i = 0;
                  %foundVirtual = false;
                  while (%i < $controlCount)
                  {
                     if ($controls[%i,0] == %parent_id)
                     {
                        if ($controls[%i,5])
                        {
                           %pos_x = $controls[%i,1] + $controls[%i,3] - %width - %parent_horiz_edge_padding - %horiz_padding;                     
                           %foundVirtual = true;
                        }
                     }
                     %i++;
                  }
                  if (!%foundVirtual)
                     %pos_x = %parent_width - %width - %parent_horiz_edge_padding - %horiz_padding;
               } 
               else if (%right_anchor > %parent_id)
               {
                  %i = 0;
                  %found = false;
                  while (%i < $controlCount)
                  {
                     if ($controls[%i,0] == %right_anchor)
                     {
                        if (%horiz_anchor_flip == false)
                           %pos_x = $controls[%i,1] - %width - %parent_horiz_padding - %horiz_padding;
                        else
                           %pos_x = $controls[%i,1] + $controls[%i,3] - %width - %horiz_padding;
                        %found = true;
                     }
                     %i++;  
                  }
                  if (!%found)
                  {
                     %undefined = true;                  
                  }
               }
               
               ////// top/bottom anchors //////////////
               if (%top_anchor < 0)
               {
                  %vert_anchor_flip = true;
                  %top_anchor *= -1;
               }
               if (%bottom_anchor < 0)
               {
                  %vert_anchor_flip = true;
                  %bottom_anchor *= -1;
               }
               if (%top_anchor == %parent_id) //check for top anchor first, then bottom.
               {
                  %i = 0;
                  %foundVirtual = false;
                  while (%i < $controlCount)
                  {
                     if ($controls[%i,0] == %parent_id)
                     {
                        if ($controls[%i,5])
                        {
                           %pos_y = $controls[%i,2] + %parent_vert_edge_padding + %vert_padding;                     
                           %foundVirtual = true;
                        }
                     }
                     %i++;
                  }
                  if (!%foundVirtual)               
                     %pos_y = %parent_vert_edge_padding + %vert_padding;
                
               }
               else if (%top_anchor > %parent_id)
               {
                  %i = 0;
                  %found = false;
                  while (%i < $controlCount)
                  {
                     if ($controls[%i,0] == %top_anchor)
                     {
                        if (%vert_anchor_flip == false)
                           %pos_y = $controls[%i,2] + $controls[%i,4] + %parent_vert_padding + %vert_padding;
                        else
                           %pos_y = $controls[%i,2] + %vert_padding;
                        %found = true;
                     }
                     %i++;  
                  }
                  if (!%found)
                  {
                     %undefined = true;                  
                  }             
               }   
               else if (%bottom_anchor == %parent_id) 
               {
                  %i = 0;
                  %foundVirtual = false;
                  while (%i < $controlCount)
                  {
                     if ($controls[%i,0] == %parent_id)
                     {
                        if ($controls[%i,5])
                        {
                           %pos_y = $controls[%i,2] + $controls[%i,4] - %height - %parent_vert_edge_padding - %vert_padding;                     
                           %foundVirtual = true;
                        }
                     }
                     %i++;
                  }
                  if (!%foundVirtual)  
                     %pos_y = %parent_height - %height - %parent_vert_edge_padding - %vert_padding;
               } 
               else if (%bottom_anchor > %parent_id)
               {
                  %i = 0;
                  %found = false;
                  while (%i < $controlCount)
                  {
                     if ($controls[%i,0] == %bottom_anchor)
                     {
                        if (%vert_anchor_flip == false)
                           %pos_y = $controls[%i,2] - %parent_vert_padding - %height - %vert_padding;
                        else
                           %pos_y = $controls[%i,2] + $controls[%i,4] - %height - %vert_padding;
                        %found = true;
                     }
                     %i++;  
                  }
                  if (!%found)
                  {
                     %undefined = true;                  
                  }
               }
               
               echo("undefined after searching: " @ %undefined);
               
               if (%undefined == false) //Yay, we found our anchors this time!
               {
                  //Now, write it to the script buffer.
                  if (%type !$= "Virtual")
                  {
                     %script = %script @ %indent @ "new " @ %type @ "() {\n";
                     %script = %script @ %indent @ "   position = \"" @ mFloor(%pos_x) @ " " @ mFloor(%pos_y) @ "\";\n";
                     %script = %script @ %indent @ "   extent = \"" @ %width @ " " @ %height @ "\";\n";
                     if (strlen(%content)>0) %script = %script @ %indent @ "   text = \"" @ %content @ "\";\n";
                     if (strlen(%name)>0) %script = %script @ %indent @ "   internalName = \"" @ %name @ "\";\n";            
                     if (strlen(%command)>0) %script = %script @ %indent @ "   command = \"" @ %command @ "\";\n";
                     if (strlen(%tooltip)>0) %script = %script @ %indent @ "   tooltip = \"" @ %tooltip @ "\";\n"; 
                     if (strlen(%tooltip)>0) %script = %script @ %indent @ "   tooltipprofile = \"GuiToolTipProfile\";\n";
                     if (strlen(%bitmap_path)>0) %script = %script @ %indent @ "   bitmap = \"" @ %bitmap_path @ "\";\n"; 
                     if (strlen(%variable)>0) %script = %script @ %indent @ "   variable = \"" @ %variable @ "\";\n"; 
                     if (strlen(%button_type)>0) %script = %script @ %indent @ "   buttonType = \"" @ %button_type @ "\";\n"; 
                     if (strlen(%group_num)>0) %script = %script @ %indent @ "   groupNum = \"" @ %group_num @ "\";\n"; 
                     if (strlen(%profile)>0) %script = %script @ %indent @ "   profile = \"" @ %profile @ "\";\n";
                  }
                  
                  //Next, store position/extent info in a 2d array, for positioning other elements.           
                  $controls[$controlCount,0] = %control_id;
                  $controls[$controlCount,1] = %pos_x; $controls[$controlCount,2] = %pos_y;
                  $controls[$controlCount,3] = %width; $controls[$controlCount,4] = %height;               
                  if (%type !$= "Virtual")
                     $controls[$controlCount,5] = false;
                  else
                     $controls[$controlCount,5] = true;              
                  $controls[$controlCount,6] = %type;
                  $controls[$controlCount,7] = %name;
                  $controlCount++;
                  
                  ////// Now, take care of the children. ///////
                  if (%type !$= "Virtual")
                  {
                     %prev_indent = %indent;
                     %indent = %indent @ "   ";//Make it three spaces deeper before we go to the children...
                  }
                  
                  //And... RECURSE!
                  %script = %script @ makeSqlGuiChildren(%control_id,%width,%height,%horiz_padding,%vert_padding,%horiz_edge_padding,%vert_edge_padding,%indent);

                  if (%type !$= "Virtual")
                  {
                     %indent = %prev_indent;//... and then take them away when we come back. 
                     %script = %script @ %indent @ "};\n";
                  }
                  
                  //Now, since we found one, we need to delete this one from the list and advance the rest of the
                  //list forward one, then break and restart.
                  
                  echo("Found an undefined! " @ %control_id @ " k = " @ %k);
                  for (%j=%k;%j<$undefinedCount-1;%j++)
                  {
                     %n = %j+1;
                     $undefinedList[%j,0] = $undefinedList[%n,0];//Just in case "(%j+1)" gets interpreted
                     $undefinedList[%j,1] = $undefinedList[%n,1];// as a new array entry...
                  }
                  $undefinedCount--;
                 
                  break; //break out of "for (%i=0;%i<undefinedCount;%i++)" loop                 
               } 
            }                       
         } else echo("No result for control id " @ %control_id);
      }
   }
   return %script;
}
*/
