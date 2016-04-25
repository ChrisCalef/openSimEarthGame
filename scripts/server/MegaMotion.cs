
$MegaMotionScenesFormID = 159;
$addMMProjectID = 241;
$addMMSceneID = 246;
$addMMSceneShapeID = 247;

$MegaMotionFormID = 1;//Testing ground, not a real gui.

//////////////////////////////////////////////////////////////////////////////////////////////////////////////

function exposeMegaMotionScenesForm()
{
   if (isDefined("MegaMotionScenes"))
      MegaMotionScenes.delete();
   
   makeSqlGuiForm($MegaMotionScenesFormID);
   
   setupMegaMotionScenesForm();   
}

function openMegaMotionScenes()
{
   echo("calling openMegaMotionScenes");
}

function setupMegaMotionScenesForm()
{
   echo("calling setupMegaMotionScenesForm");
   
   if (!isDefined("MegaMotionScenes"))
      return;   
   
   //Might be better to forego these and instead findObject one at a time when I need them...
  
   $mmProjectList = MegaMotionScenes.findObjectByInternalName("projectList");
   $mmSceneList = MegaMotionScenes.findObjectByInternalName("sceneList");
   $mmSceneShapeList = MegaMotionScenes.findObjectByInternalName("sceneShapeList"); 
   $mmShapeList = MegaMotionScenes.findObjectByInternalName("shapeList"); 
    
   $mmTabBook = MegaMotionScenes.findObjectByInternalName("mmTabBook");    

   //SCENE TAB
   %tab = $mmTabBook.findObjectByInternalName("sceneShapeTab");
   %panel = %tab.findObjectByInternalName("sceneShapePanel");
   $sceneShapePositionX = %panel.findObjectByInternalName("sceneShapePositionX");
   $sceneShapePositionY = %panel.findObjectByInternalName("sceneShapePositionY");
   $sceneShapePositionZ = %panel.findObjectByInternalName("sceneShapePositionZ");
   $sceneShapeOrientationX = %panel.findObjectByInternalName("sceneShapeOrientationX");//quat
   $sceneShapeOrientationY = %panel.findObjectByInternalName("sceneShapeOrientationY");
   $sceneShapeOrientationZ = %panel.findObjectByInternalName("sceneShapeOrientationZ");
   $sceneShapeOrientationAngle = %panel.findObjectByInternalName("sceneShapeOrientationAngle");
   $sceneShapeScaleX = %panel.findObjectByInternalName("sceneShapeScaleX");
   $sceneShapeScaleY = %panel.findObjectByInternalName("sceneShapeScaleY");
   $sceneShapeScaleZ = %panel.findObjectByInternalName("sceneShapeScaleZ");  
   
   //MOUNT TAB
   //%tab = $mmTabBook.findObjectByInternalName("mountTab");
   //%panel = %tab.findObjectByInternalName("mountPanel");
   
   //SHAPEPART TAB
   %tab = $mmTabBook.findObjectByInternalName("shapePartTab");
   %panel = %tab.findObjectByInternalName("shapePartPanel");
   $mmShapePartList = %panel.findObjectByInternalName("shapePartList");
   $mmShapePartTypeList = %panel.findObjectByInternalName("shapePartTypeList");
   $mmBaseNodeList = %panel.findObjectByInternalName("baseNodeList");
   $mmChildNodeList = %panel.findObjectByInternalName("childNodeList");   
   
   $shapePartDimensionsX = %panel.findObjectByInternalName("shapePartDimensionsX");
   $shapePartDimensionsY = %panel.findObjectByInternalName("shapePartDimensionsY");
   $shapePartDimensionsZ = %panel.findObjectByInternalName("shapePartDimensionsZ");         
   $shapePartOrientationX = %panel.findObjectByInternalName("shapePartOrientationX");//eulers
   $shapePartOrientationY = %panel.findObjectByInternalName("shapePartOrientationY");
   $shapePartOrientationZ = %panel.findObjectByInternalName("shapePartOrientationZ");         
   $shapePartOffsetX = %panel.findObjectByInternalName("shapePartOffsetX");
   $shapePartOffsetY = %panel.findObjectByInternalName("shapePartOffsetY");
   $shapePartOffsetZ = %panel.findObjectByInternalName("shapePartOffsetZ");   
   
   $mmJointList = %panel.findObjectByInternalName("jointList");
   
   $mmShapePartTypeList.add("Box","0");
   $mmShapePartTypeList.add("Capsule","1");
   $mmShapePartTypeList.add("Sphere","2");
   //From here down, currently unsupported.
   //$mmShapeTypeList.add("Convex","3");
   //$mmShapeTypeList.add("Collision","4");
   //$mmShapeTypeList.add("Trimesh","5");
   
   $mmShapePartTypeList.setSelected(0);
   
   //BEHAVIOR TAB
   //%tab = $mmTabBook.findObjectByInternalName("behaviorTab");
   //%panel = %tab.findObjectByInternalName("behaviorPanel");
   
   //$mmPositionList
   //$mmRotationList
   
   %query = "SELECT id,name FROM project ORDER BY name;";  
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {         
         %firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            $mmProjectList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         if (%firstID>0) 
            $mmProjectList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }
   
   %query = "SELECT id,name FROM physicsShape ORDER BY name;";
   %resultSet = sqlite.query(%query, 0);
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {         
         //%firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            $mmShapeList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         //if (%firstID>0) 
            //$mmShapeList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }   
   
   %query = "SELECT id,name FROM px3Joint ORDER BY name;";  
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {         
         //%firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            $mmJointList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         //if (%firstID>0) 
            //$mmShapeList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }   
}

function updateMegaMotionForm()
{
   //This is the big Commit Button at the top of the form. Commit from whichever tab is open.
    
   
}


////////////////////////////////////////////////////////////////////////////////////
//REFACTOR: Still working out the MegaMotion vs openSimEarth division of labor.

//Direct copy of EditorSaveMission from menuHandlers.ed.cs. This version exists
//because mission save is actually just SimObject::save, and that is way too deep 
//into T3D to be making application level changes. Instead, we just call this one,
//but we are still going to have problems with all the plugins until we keep them 
//from calling MissionGroup.save() on their own.

function MegaMotionSaveMission()
{
   echo("Calling MegaMotionSaveMission!!!!!!!!!!!!!!!!!!!!!!!!!!!");
   //if(isFunction("getObjectLimit") && MissionGroup.getFullCount() >= getObjectLimit())
   //{ //(Object limit in trial version, ossible way to nag licensing compliance?)
   //   MessageBoxOKBuy( "Object Limit Reached", "You have exceeded the object limit of " @ getObjectLimit() @ " for this demo. You can remove objects if you would like to add more.", "", "Canvas.showPurchaseScreen(\"objectlimit\");" );
   //   return;
   //}
   
   // first check for dirty and read-only files:
   if((EWorldEditor.isDirty || ETerrainEditor.isMissionDirty) && !isWriteableFileName($Server::MissionFile))
   {
      MessageBox("Error", "Mission file \""@ $Server::MissionFile @ "\" is read-only.  Continue?", "Ok", "Stop");
      return false;
   }
   if(ETerrainEditor.isDirty)
   {
      // Find all of the terrain files
      initContainerTypeSearch($TypeMasks::TerrainObjectType);

      while ((%terrainObject = containerSearchNext()) != 0)
      {
         if (!isWriteableFileName(%terrainObject.terrainFile))
         {
            if (MessageBox("Error", "Terrain file \""@ %terrainObject.terrainFile @ "\" is read-only.  Continue?", "Ok", "Stop") == $MROk)
               continue;
            else
               return false;
         }
      }
   }
  
   // now write the terrain and mission files out:
   
   
   ///////////////////////////   
   //For openSimEarth, we need to save many things to the database instead of to 
   //the mission. Starting with TSStatics.
   $tempStaticGroup = new SimSet();
   $tempRoadGroup = new SimSet();
   $tempForestGroup = new SimSet();
   $tempSceneShapes = new SimSet();
   
   if ($pref::OpenSimEarth::saveSceneShapes)
      osePullSceneShapesAndSave($tempSceneShapes);
   else
      osePullSceneShapes($tempSceneShapes);
   
   if ($pref::OpenSimEarth::saveStatics)
      osePullStaticsAndSave($tempStaticGroup);
   else
      osePullStatics($tempStaticGroup);
   
   
   //TEMP - should actually define this so it's possible to save to mission
   if ($pref::OpenSimEarth::saveRoads)
      osePullRoadsAndSave($tempRoadGroup);
   else
      osePullRoads($tempRoadGroup);
   
   
   //if ($pref::OpenSimEarth::saveForests==false)
   //   osePullForest($tempForestGroup);
   
   
   if(EWorldEditor.isDirty || ETerrainEditor.isMissionDirty)
      MissionGroup.save($Server::MissionFile);
      
   osePushSceneShapes($tempSceneShapes);
   osePushStatics($tempStaticGroup);
   osePushRoads($tempRoadGroup);
   //osePushForest($tempForestGroup);
   ///////////////////////////   
   
   
   if(ETerrainEditor.isDirty)
   {
      // Find all of the terrain files
      initContainerTypeSearch($TypeMasks::TerrainObjectType);

      while ((%terrainObject = containerSearchNext()) != 0)
         %terrainObject.save(%terrainObject.terrainFile);
   }

   ETerrainPersistMan.saveDirty();
      
   // Give EditorPlugins a chance to save.
   for ( %i = 0; %i < EditorPluginSet.getCount(); %i++ )
   {
      %obj = EditorPluginSet.getObject(%i);
      if ( %obj.isDirty() )
         %obj.onSaveMission( $Server::MissionFile );      
   } 
   
   EditorClearDirty();
   
   EditorGui.saveAs = false;
   
   return true;
}

function MegaMotionSaveSceneShapes()
{
   //NOW: find all physicsShapes, and save each of them 
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %obj = SceneShapes.getObject(%i);  
      if ((%obj.sceneShapeID>0)&&(%obj.sceneID>0)&&(%obj.isDirty))
      {
         %query = "SELECT  pos_id,rot_id,scale_id FROM sceneShape " @ 
                  "WHERE id=" @ %obj.sceneShapeID @ ";";
         %resultSet = sqlite.query(%query,0);
         if (sqlite.numRows(%resultSet)==1)
         {
            %pos_id = sqlite.getColumn(%resultSet, "pos_id");
            %rot_id = sqlite.getColumn(%resultSet, "rot_id");
            %scale_id = sqlite.getColumn(%resultSet, "scale_id");
            
            %trans = %obj.getTransform();
            %p_x = getWord(%trans,0);
            %p_y = getWord(%trans,1);
            %p_z = getWord(%trans,2);
            %r_x = getWord(%trans,3);
            %r_y = getWord(%trans,4);
            %r_z = getWord(%trans,5);
            %r_angle = mRadToDeg(getWord(%trans,6));
            %query = "UPDATE vector3 SET x=" @ %p_x @ ",y=" @ %p_y @ ",z=" @ %p_z @
                     " WHERE id=" @ %pos_id @ ";";
            sqlite.query(%query,0);
            %query = "UPDATE rotation SET x=" @ %r_x @ ",y=" @ %r_y @ ",z=" @ %r_z @
                     ",angle=" @ %r_angle @ " WHERE id=" @ %rot_id @ ";";
            sqlite.query(%query,0);
            
            %scale = %obj.getScale();
            //if (%scale $= "1 1 1") //Do what now? we need to check for this and assign id = 1, and maintain that.
            %s_x = getWord(%scale,0);//Or, just ignore it and accept the bloat.
            %s_y = getWord(%scale,1);
            %s_z = getWord(%scale,2);            
            %query = "UPDATE vector3 SET x=" @ %s_x @ ",y=" @ %s_y @ ",z=" @ %s_z @
                     " WHERE id=" @ %scale_id @ ";";
            sqlite.query(%query,0);
            
            echo("updated a dirty scene shape!!");
         }        
      }
   }
}

////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

function selectMMProject()
{   
   echo("calling selectMegaMotionProject " @ $mmProjectList.getSelected());
   
   if ($mmProjectList.getSelected()<=0)
      return;
    
   $mmSceneList.clear();  
   
   %firstID = 0;
   %query = "SELECT id,name FROM scene WHERE project_id=" @ $mmProjectList.getSelected() @ " ORDER BY name;";  
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {
         %firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            
            $mmSceneList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         if (%firstID>0)
            $mmSceneList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }      
}

function addMMProject()
{
   makeSqlGuiForm($addMMProjectID);
}

function reallyAddMMProject()
{
   if (addMMProjectWindow.isVisible())
   {
      %name = addMMProjectWindow.findObjectByInternalName("nameEdit").getText(); 
      if ((strlen(%name)==0)||(substr(%name," ")>0))
      {
         MessageBoxOK("Name Invalid","Project name must be a unique character string with no spaces or special characters.","");
         return;  
      }
      %query = "SELECT id FROM project WHERE name='" @ %name @ "';";
      %resultSet = sqlite.query(%query,0);
      if (sqlite.numRows(%resultSet)>0)
      {
         MessageBoxOK("Name Invalid","Project name must be unique.","");
         return;
      }
      %query = "INSERT INTO project (name) VALUES ('" @ %name @ "');";
      sqlite.query(%query,0);
      
      exposeMegaMotionScenesForm();
   }
}

function deleteMMProject()
{
   
}

function loadMMProject()
{
   //Maybe we will want to load shared static object props used by multiple scenes? 
    
}

function unloadMMProject()
{
   //Remove project environment objects.
   
}

function selectMMScene()
{
   echo("calling selectMegaMotionScene");
   
   if ($mmSceneList.getSelected()<=0)
      return;
      
   $mmSceneShapeList.clear(); 
   
   echo("calling selectMegaMotionScene on scene " @ $mmSceneList.getSelected());
   
   %firstID = 0;
   %query = "SELECT ss.id,ps.id AS ps_id, ps.name FROM sceneShape ss " @
	         "JOIN physicsShape ps ON ps.id=ss.shape_id " @
            "WHERE scene_id=" @ $mmSceneList.getSelected() @ ";";  
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      //echo("adding " @ sqlite.numRows(%resultSet) @ " scene shapes!");
      if (sqlite.numRows(%resultSet)>0)
      {
         %firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name") @ " - " @ %id;
            $mmSceneShapeList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         if (%firstID>0)
            $mmSceneShapeList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }   
}

function addMMScene()
{
   makeSqlGuiForm($addMMSceneID);   
}

function reallyAddMMScene()  //TO DO: Description, position.
{  
   if (addMMSceneWindow.isVisible())
   {
      %name = addMMSceneWindow.findObjectByInternalName("nameEdit").getText(); 
      if ((strlen(%name)==0)||(substr(%name," ")>0))
      {
         MessageBoxOK("Name Invalid","Scene name must be a unique character string with no spaces or special characters.","");
         return;  
      }
      %proj_id = $mmProjectList.getSelected();
      %query = "SELECT id FROM scene WHERE name='" @ %name @ "' AND project_id=" @ %proj_id @ ";";
      %resultSet = sqlite.query(%query,0);
      if (sqlite.numRows(%resultSet)>0)
      {
         MessageBoxOK("Name Invalid","Scene name must be unique for this project.","");
         return;
      }
      %query = "INSERT INTO scene (name,project_id) VALUES ('" @ %name @ "'," @ %proj_id @ ");";
      sqlite.query(%query,0);
      
      exposeMegaMotionScenesForm();
   }
}

function deleteMMScene()
{
   
}

function loadMMScene()
{
   //pdd(1);//physics debug draw
   %scene_id = $mmSceneList.getSelected();
   %dyn = false;
   %grav = true;
   %ambient = true;

	%query = "SELECT ss.id as ss_id,shape_id,shapeGroup_id,behavior_tree," @ 
	         "p.x as pos_x,p.y as pos_y,p.z as pos_z," @ 
	         "r.x as rot_x,r.y as rot_y,r.z as rot_z,r.angle as rot_angle," @ 
	         "sc.x as scale_x,sc.y as scale_y,sc.z as scale_z," @ 
	         "sp.x as scene_pos_x,sp.y as scene_pos_y,sp.z as scene_pos_z," @ 
	         "sh.datablock as datablock " @ 
	         "FROM sceneShape ss " @ 
	         "JOIN scene s ON s.id=scene_id " @
	         "LEFT JOIN vector3 p ON ss.pos_id=p.id " @ 
	         "LEFT JOIN rotation r ON ss.rot_id=r.id " @ 
	         "LEFT JOIN vector3 sc ON ss.scale_id=sc.id " @ 
	         "LEFT JOIN vector3 sp ON s.pos_id=sp.id " @ 
	         "JOIN physicsShape sh ON ss.shape_id=sh.id " @ 
	         "WHERE scene_id=" @ %scene_id @ ";";  
	%resultSet = sqlite.query(%query, 0);
	
	echo("calling loadScene, result " @ %resultSet);
   echo( "Query: " @ %query );	
	
   if (%resultSet)
   {
      while (!sqlite.endOfResult(%resultSet))
      {
         %sceneShape_id = sqlite.getColumn(%resultSet, "ss_id");   
         %shape_id = sqlite.getColumn(%resultSet, "shape_id");
         %shapeGroup_id = sqlite.getColumn(%resultSet, "shapeGroup_id");//not used yet
         %behaviorTree = sqlite.getColumn(%resultSet, "behavior_tree");
         
         %pos_x = sqlite.getColumn(%resultSet, "pos_x");
         %pos_y = sqlite.getColumn(%resultSet, "pos_y");
         %pos_z = sqlite.getColumn(%resultSet, "pos_z");
         
         %rot_x = sqlite.getColumn(%resultSet, "rot_x");
         %rot_y = sqlite.getColumn(%resultSet, "rot_y");
         %rot_z = sqlite.getColumn(%resultSet, "rot_z");
         %rot_angle = sqlite.getColumn(%resultSet, "rot_angle");
         
         %scale_x = sqlite.getColumn(%resultSet, "scale_x");
         %scale_y = sqlite.getColumn(%resultSet, "scale_y");
         %scale_z = sqlite.getColumn(%resultSet, "scale_z");
         
         %scene_pos_x = sqlite.getColumn(%resultSet, "scene_pos_x");
         %scene_pos_y = sqlite.getColumn(%resultSet, "scene_pos_y");
         %scene_pos_z = sqlite.getColumn(%resultSet, "scene_pos_z");
         
         %datablock = sqlite.getColumn(%resultSet, "datablock");
         
         echo("Found a sceneShape: " @ %sceneShape_id @ " " @ %pos_x @ " " @ %pos_y @ " " @ %pos_z @
                " scenePos " @ %scene_pos_x @ " " @ %scene_pos_y @ " " @ %scene_pos_z );
                
         %position = (%pos_x + %scene_pos_x) @ " " @ (%pos_y + %scene_pos_y) @ " " @ (%pos_z + %scene_pos_z);
         %rotation = %rot_x @ " " @ %rot_y @ " " @ %rot_z @ " " @ %rot_angle;
         %scale = %scale_x @ " " @ %scale_y @ " " @ %scale_z;
         
         echo("loading sceneShape id " @ %shape_id @ " position " @ %position @ " rotation " @ 
               %rotation @ " scale " @ %scale);
         
         //TEMP
         %name = "";          
         if (%shape_id==4)
            %name = "ka50";   
         else if (%shape_id==3)
            %name = "bo105";
         else if (%shape_id==2)
            %name = "dragonfly";
            
         %temp =  new PhysicsShape(%name) {
            playAmbient = %ambient;
            dataBlock = %datablock;
            position = %position;
            rotation = %rotation;
            scale = %scale;
            canSave = "1";
            canSaveDynamicFields = "1";
            areaImpulse = "0";
            damageRadius = "0";
            invulnerable = "0";
            minDamageAmount = "0";
            radiusDamage = "0";
            hasGravity = %grav;
            isDynamic = %dyn;
            sceneShapeID = %sceneShape_id;
            sceneID = %scene_id;
            targetType = "Health";//"AmmoClip"
            isDirty = false;
         };
         
         MissionGroup.add(%temp);   
         SceneShapes.add(%temp);   
         echo("Adding a scene shape: " @ %sceneShape_id @ ", position " @ %position );
         
         if (strlen(%behaviorTree)>0)
         {
            %temp.schedule(30,"setBehavior",%behaviorTree);
            //echo(%temp.getId() @ " assigning behavior tree: " @ %behaviorTree );
         }

         sqlite.nextRow(%resultSet);
      }
   }   
   sqlite.clearResult(%resultSet);
   //schedule(40, 0, "startRecording");
} 

//function testSpatialite()
//{
//   //%query = "CREATE TABLE spatialTest ( id INTEGER, name TEXT NOT NULL, geom BLOB NOT NULL);";
//   %query = "INSERT INTO spatialTest ( id , name, geom ) VALUES (1,'Test01',GeomFromText('POINT(1 2)'));";
   
//   %result = sqlite.query(%query, 0);
	
//   if (%result)
//      echo("spatialite inserted into a table with a geom!");
//   else
//      echo("spatialite failed to insert into a table with a geom!  "  );
//}

function unloadMMScene()
{
   %scene_id = $mmSceneList.getSelected();
   //HERE: look up all the sceneShapes from the scene in question, and drop them all from the current mission.
   %shapesCount = SceneShapes.getCount();
   for (%i=0;%i<%shapesCount;%i++)
   {
      %shape = SceneShapes.getObject(%i);  
      //echo("shapesCount " @ %shapesCount @ ", sceneShape id " @ %shape.sceneShapeID @ 
      //         " scene " @ %shape.sceneID ); 
      if (%shape.sceneID==%scene_id)
      {       
         MissionGroup.remove(%shape);
         SceneShapes.remove(%shape);//Wuh oh... removing from SceneShapes shortens the array...
         %shape.delete();//Maybe??
         
         %shapesCount = SceneShapes.getCount();
         if (%shapesCount>0)
            %i=-1;//So start over every time we remove one, until we loop through and remove none.
         else 
            %i=1;//Or else we run out of shapes, and just need to exit the loop.         
      }
   }   
}

function selectMMSceneShape()
{
   echo("selecting mm scene shape!!!!");
   if ($mmSceneShapeList.getSelected()<=0)
      return;
        
   %scene_shape_id = $mmSceneShapeList.getSelected();
   //%query = "SELECT * FROM sceneShape WHERE id=" @ %sceneShapeId @ ";";  
	%query = "SELECT shape_id,ss.name,shapeGroup_id,behavior_tree," @ 
	         "p.x as pos_x,p.y as pos_y,p.z as pos_z," @ 
	         "r.x as rot_x,r.y as rot_y,r.z as rot_z,r.angle as rot_angle," @ 
	         "sc.x as scale_x,sc.y as scale_y,sc.z as scale_z " @ 
	         "FROM sceneShape ss " @ 
	         "LEFT JOIN vector3 p ON ss.pos_id=p.id " @ 
	         "LEFT JOIN rotation r ON ss.rot_id=r.id " @ 
	         "LEFT JOIN vector3 sc ON ss.scale_id=sc.id " @ 
	         "WHERE ss.id=" @ %scene_shape_id @ ";"; 
   
   
   %resultSet = sqlite.query(%query, 0); 
   if (sqlite.numRows(%resultSet)==1)
   {
      %name = sqlite.getColumn(%resultSet, "name");
      %behavior_tree = sqlite.getColumn(%resultSet, "behavior_tree");
      %shape_id = sqlite.getColumn(%resultSet, "shape_id");
      %shapeGroup_id = sqlite.getColumn(%resultSet, "shapeGroup_id");
      %pos_x = sqlite.getColumn(%resultSet, "pos_x");
      %pos_y = sqlite.getColumn(%resultSet, "pos_y");
      %pos_z = sqlite.getColumn(%resultSet, "pos_z");
      %rot_x = sqlite.getColumn(%resultSet, "rot_x");
      %rot_y = sqlite.getColumn(%resultSet, "rot_y");
      %rot_z = sqlite.getColumn(%resultSet, "rot_z");
      %rot_angle = sqlite.getColumn(%resultSet, "rot_angle");
      %scale_x = sqlite.getColumn(%resultSet, "scale_x");
      %scale_y = sqlite.getColumn(%resultSet, "scale_y");
      %scale_z = sqlite.getColumn(%resultSet, "scale_z");
      
      $mmShapeList.setSelected(%shape_id);
      
      $sceneShapePositionX.setText(%pos_x);
      $sceneShapePositionY.setText(%pos_y);
      $sceneShapePositionZ.setText(%pos_z);
      
      $sceneShapeOrientationX.setText(%rot_x);
      $sceneShapeOrientationY.setText(%rot_y);
      $sceneShapeOrientationZ.setText(%rot_z);
      $sceneShapeOrientationAngle.setText(%rot_angle);
      
      $sceneShapeScaleX.setText(%scale_x);
      $sceneShapeScaleY.setText(%scale_y);
      $sceneShapeScaleZ.setText(%scale_z);
      
      sqlite.clearResult(%resultSet);
   }
   
}

function addMMSceneShape()
{
   makeSqlGuiForm($addMMSceneShapeID);   
}

function reallyAddMMSceneShape() //TO DO: pos/rot/scale, shapeGroup, behaviorTree.
{
   if (addMMSceneShapeWindow.isVisible())
   {
      echo("ADDING A SCENE SHAPE");
      %name = addMMSceneShapeWindow.findObjectByInternalName("nameEdit").getText(); 
      //if (substr(%name," ")>0)
      //{
      //   MessageBoxOK("Name Invalid","Scene name must be a unique character string with no spaces or special characters.","");
      //   return;  
      //}
      %scene_id = $mmSceneList.getSelected();
      %shape_id = $mmShapeList.getSelected();
      if (strlen(%name)>0)
      {
         %query = "SELECT id FROM sceneShape WHERE name='" @ %name @ "' AND scene_id=" @ %scene_id @ ";";
         %resultSet = sqlite.query(%query,0);
         if (sqlite.numRows(%resultSet)>0)
         {
            MessageBoxOK("Name Invalid","Scene shape name must be unique for this scene.","");
            return;
         }
      }
      
      %query = "INSERT INTO sceneShape (name,scene_id,shape_id) VALUES ('" @ %name @ "'," @ 
                   %scene_id @ "," @ %shape_id @ ");";
      sqlite.query(%query,0);
      
      %ssID = 0;
      %query = "SELECT id FROM sceneShape WHERE name='" @ %name @ "' AND scene_id=" @ %scene_id @ ";";
      %resultSet = sqlite.query(%query,0);
      if (sqlite.numRows(%resultSet)==1)
         %ss_id = sqlite.getColumn(%resultSet, "id");
      if (%ss_id==0)
         return;
      
      //For first pass at least, just add default values and spawn the character at scene origin.
      %query = "INSERT INTO vector3 (x,y,z) VALUES (0,0,0);";
      sqlite.query(%query,0);
      %query = "UPDATE sceneShape SET pos_id=last_insert_rowid() WHERE id=" @ %ss_id @ ";";
      sqlite.query(%query, 0);  
            
      %query = "INSERT INTO rotation (x,y,z,angle) VALUES (0,0,1,0);";
      sqlite.query(%query,0);
      %query = "UPDATE sceneShape SET rot_id=last_insert_rowid() WHERE id=" @ %ss_id @ ";";
      sqlite.query(%query, 0);  
      
      %query = "INSERT INTO vector3 (x,y,z) VALUES (1,1,1);";
      sqlite.query(%query,0);
      %query = "UPDATE sceneShape SET scale_id=last_insert_rowid() WHERE id=" @ %ss_id @ ";";
      sqlite.query(%query, 0);  
      //%query = "UPDATE sceneShape SET  = , = , WHERE id=" @ %ssID @ ";";
      //sqlite.query(%query,0);
      
      //shapeGroup, behaviorTree ...
      
      exposeMegaMotionScenesForm();
   }
   
}

function deleteMMSceneShape()
{
   
}

function selectMMSceneShapeBehavior()
{
   
}

function selectMMSceneShapeGroup()
{
   
}

function addMMShape()
{
   
}

function reallyAddMMShape()
{
   
}

function deleteMMShape()
{
   
}

///////////////////////////////////////////////////////////////////////////////

function selectMMShape()
{
   echo("calling selectMegaMotionShape");
   
   if ($mmShapeList.getSelected()<=0)
      return;
      
   $mmShapePartList.clear();   
   
   %firstID = 0;
   %query = "SELECT id,name FROM physicsShapePart WHERE physicsShape_id=" @ $mmShapeList.getSelected() @ ";";  
   echo("\n" @ %query @ "\n");   
   %resultSet = sqlite.query(%query, 0); 
   if (%resultSet)
   {
      if (sqlite.numRows(%resultSet)>0)
      {
         %firstID = sqlite.getColumn(%resultSet, "id");
         while (!sqlite.endOfResult(%resultSet))
         {
            %id = sqlite.getColumn(%resultSet, "id");
            %name = sqlite.getColumn(%resultSet, "name");
            $mmShapePartList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         if (%firstID>0)
            $mmShapePartList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }   
   
   //Now, node lists: this is based on querying the actual TSShape object, not the database.
   //But, oh crap, this can only work if we have a live instance of the shape in the scene.
   
}

function selectMMShapePart()
{
   if ($mmShapePartList.getSelected()<=0)
      return;
   
   %part_id = $mmShapePartList.getSelected();
   echo("calling selectMegaMotionShapePart, part_id " @ %part_id);
	%query = "SELECT * FROM physicsShapePart " @ 
	         "WHERE id=" @ %part_id @ ";"; 
   echo("\n" @ %query @ "\n"); 
	%resultSet = sqlite.query(%query, 0);
	
	if (%resultSet)
	{
	   if (sqlite.numRows(%resultSet)==1)
	   {	      
         %tab = $mmTabBook.findObjectByInternalName("shapePartTab");
         %panel = %tab.findObjectByInternalName("shapePartPanel");
         
         $shapePartDimensionsX.setText(sqlite.getColumn(%resultSet, "dimensions_x"));
         $shapePartDimensionsY.setText(sqlite.getColumn(%resultSet, "dimensions_y"));
         $shapePartDimensionsZ.setText(sqlite.getColumn(%resultSet, "dimensions_z"));
         
         $shapePartOrientationX.setText(sqlite.getColumn(%resultSet, "orientation_x"));
         $shapePartOrientationY.setText(sqlite.getColumn(%resultSet, "orientation_y"));
         $shapePartOrientationZ.setText(sqlite.getColumn(%resultSet, "orientation_z"));
         
         $shapePartOffsetX.setText(sqlite.getColumn(%resultSet, "offset_x"));
         $shapePartOffsetY.setText(sqlite.getColumn(%resultSet, "offset_y"));
         $shapePartOffsetZ.setText(sqlite.getColumn(%resultSet, "offset_z"));
         
         //%joint_id = sqlite.getColumn(%resultSet, "joint_id");
         //if (%joint_id > 0)
            //$mmJointList.setSelected(%joint_id);
         
         $mmShapePartTypeList.setSelected(sqlite.getColumn(%resultSet, "shapeType"));
	   } else {
	      echo("shape part num rows: " @ sqlite.numRows(%resultSet));
	   }
	} else { 
	   echo("shape part query failed!  \n " @ %query);
	} 
}

function updateMMShapePart()
{
   if ($mmShapePartList.getSelected()<=0)
      return;
      
   %part_id = $mmShapePartList.getSelected();
   
   %query = "";
   
   %query = %query @ "UPDATE physicsShapePart SET "; 
   %query = %query @ "shapeType=" @ $mmShapePartTypeList.getSelected();
   
   if (strlen($shapePartDimensionsX.getText())>0)//(also check isNumeric)
      %query = %query @ ",dimensions_x=" @ $shapePartDimensionsX.getText();
   if (strlen($shapePartDimensionsY.getText())>0)//(also check isNumeric)
      %query = %query @ ",dimensions_y=" @ $shapePartDimensionsY.getText();
   if (strlen($shapePartDimensionsZ.getText())>0)//(also check isNumeric)
      %query = %query @ ",dimensions_z=" @ $shapePartDimensionsZ.getText();
   if (strlen($shapePartOrientationX.getText())>0)//(also check isNumeric)
      %query = %query @ ",orientation_x=" @ $shapePartOrientationX.getText();
   if (strlen($shapePartOrientationY.getText())>0)//(also check isNumeric)
      %query = %query @ ",orientation_y=" @ $shapePartOrientationY.getText();
   if (strlen($shapePartOrientationZ.getText())>0)//(also check isNumeric)
      %query = %query @ ",orientation_z=" @ $shapePartOrientationZ.getText();
   if (strlen($shapePartOffsetX.getText())>0)//(also check isNumeric)
      %query = %query @ ",offset_x=" @ $shapePartOffsetX.getText();
   if (strlen($shapePartOffsetY.getText())>0)//(also check isNumeric)
      %query = %query @ ",offset_y=" @ $shapePartOffsetY.getText();
   if (strlen($shapePartOffsetZ.getText())>0)//(also check isNumeric)
      %query = %query @ ",offset_z=" @ $shapePartOffsetZ.getText();
      
   %query = %query @ " WHERE id=" @ %part_id @ ";"; 
   echo("\n" @ %query @ "\n"); 
	sqlite.query(%query, 0);
}



//////////////////////////////////////////////////////////////////////



function shapesAct()
{
   //pdd(1);
   for (%i=0;%i<SceneShapes.getCount();%i++)
   {
      %shape = SceneShapes.getObject(%i);        
      //%shape.setHasGravity(false);
      
      %shape.setDynamic(1);
      
      %shape.setPartDynamic(0,0);
      //%shape.setPartDynamic(1,0);
      //%shape.setPartDynamic(2,0);  
      //%shape.setPartDynamic(3,0); 
      //%shape.setPartDynamic(4,0); 
      
      //%shape.setPartDynamic(5,0); 
      //%shape.setPartDynamic(6,0); 
      //%shape.setPartDynamic(7,0);
      //%shape.setPartDynamic(8,0);
      
      //%shape.setPartDynamic(9,0); 
      //%shape.setPartDynamic(10,0); 
      //%shape.setPartDynamic(11,0);
      //%shape.setPartDynamic(12,0);
       
      //%shape.setPartDynamic(13,0);
      //%shape.setPartDynamic(14,1);
      //%shape.setPartDynamic(15,1);
      
      //%shape.setPartDynamic(16,0);
      //%shape.setPartDynamic(17,0);
      //%shape.setPartDynamic(18,0);     
   } 
}









//////////////////////////////////////////////////////////////////////////////////////////////////////////////
//For each form that we hook up to a top menu, give it an "expose" function to load it and call setup for it.
//OBSOLETE, TESTING /////////////////////////////
function exposeMegaMotion()
{
   if (isDefined("MegaMotionWindow"))
      MegaMotionWindow.delete();
   
   makeSqlGuiForm($MegaMotionFormID);
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
}





//OBSOLETE, TESTING /////////////////////////////

//Nice try, but even with modifications to GuiWindowCtrl, there is no way to intercept a mouse
//event that lands on a control. Hence, we're screwed if we want to select controls this way.
//function MegaMotionWindow::onMouseDown(%this,%pos)
//{
   //echo("MegaMotionWindow onMouseDown!!!!  this.pos " @ %this.getPosition() @ " mouse pos " @ %pos);  
//}
//
//function MegaMotionWindow::onMouseUp(%this,%pos)
//{
   //echo("MegaMotionWindow onMouseUp!!!!  this.pos " @ %this.getPosition() @ " mouse pos " @ %pos);  
//}
