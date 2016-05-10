
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
   
   $mmShapeId = 0;
   $mmPosId = 0;
   $mmRotId = 0;
   $mmScaleId = 0;
  
   $mmProjectList = MegaMotionScenes.findObjectByInternalName("projectList");
   $mmSceneList = MegaMotionScenes.findObjectByInternalName("sceneList");
   $mmSceneShapeList = MegaMotionScenes.findObjectByInternalName("sceneShapeList"); 
   $mmShapeList = MegaMotionScenes.findObjectByInternalName("shapeList"); 
    
   $mmTabBook = MegaMotionScenes.findObjectByInternalName("mmTabBook");    

   //SCENE TAB
   %tab = $mmTabBook.findObjectByInternalName("sceneShapeTab");
   %panel = %tab.findObjectByInternalName("sceneShapePanel");
   $mmSceneShapeBehaviorTree = %panel.findObjectByInternalName("sceneShapeBehaviorTree");
   $mmSceneShapeGroupList = %panel.findObjectByInternalName("sceneShapeGroupList");
   $mmSceneShapePositionX = %panel.findObjectByInternalName("sceneShapePositionX");
   $mmSceneShapePositionY = %panel.findObjectByInternalName("sceneShapePositionY");
   $mmSceneShapePositionZ = %panel.findObjectByInternalName("sceneShapePositionZ");
   $mmSceneShapeOrientationX = %panel.findObjectByInternalName("sceneShapeOrientationX");//quat
   $mmSceneShapeOrientationY = %panel.findObjectByInternalName("sceneShapeOrientationY");
   $mmSceneShapeOrientationZ = %panel.findObjectByInternalName("sceneShapeOrientationZ");
   $mmSceneShapeOrientationAngle = %panel.findObjectByInternalName("sceneShapeOrientationAngle");
   $mmSceneShapeScaleX = %panel.findObjectByInternalName("sceneShapeScaleX");
   $mmSceneShapeScaleY = %panel.findObjectByInternalName("sceneShapeScaleY");
   $mmSceneShapeScaleZ = %panel.findObjectByInternalName("sceneShapeScaleZ");  
   
   //SHAPEPART TAB
   %tab = $mmTabBook.findObjectByInternalName("shapePartTab");
   %panel = %tab.findObjectByInternalName("shapePartPanel");
   $mmShapePartList = %panel.findObjectByInternalName("shapePartList");
   
   $mmShapePartTypeList = %panel.findObjectByInternalName("shapePartTypeList");
   $mmShapePartTypeList.add("Box","0");
   $mmShapePartTypeList.add("Capsule","1");
   $mmShapePartTypeList.add("Sphere","2");
   //From here down, currently unsupported.
   //$mmShapeTypeList.add("Convex","3");
   //$mmShapeTypeList.add("Collision","4");
   //$mmShapeTypeList.add("Trimesh","5");
   $mmShapePartTypeList.setSelected(0);
   
   $mmJointList = %panel.findObjectByInternalName("jointList");
   
   $mmJointTypeList = %panel.findObjectByInternalName("jointTypeList");
   $mmJointTypeList.add("Spherical","0");
   $mmJointTypeList.add("Revolute","1");
   $mmJointTypeList.add("Prismatic","2");
   $mmJointTypeList.add("Fixed","3");
   $mmJointTypeList.add("Distance","4");
   $mmJointTypeList.add("D6","5");
   $mmJointTypeList.setSelected(5);
   
   //BEHAVIOR TAB
   //%tab = $mmTabBook.findObjectByInternalName("behaviorTab");
   //%panel = %tab.findObjectByInternalName("behaviorPanel");
   
   
   //MOUNT TAB
   //%tab = $mmTabBook.findObjectByInternalName("mountTab");
   //%panel = %tab.findObjectByInternalName("mountPanel");
   
   
   
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
   
   //behaviorList
   //Whoops, this one is not a query - we need 
   
   //groupList
   %query = "SELECT id,name FROM shapeGroup ORDER BY name;";
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
            
            $mmSceneShapeGroupList.add(%name,%id);
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
   //This is the big Commit Button at the top of the form. 
   //Making it commit whichever tab is on top, plus the 
   
   //NOTE: these need to be fixed anytime we add another tab.
   %sceneShapeTab = 2;      
   %partsTab = 1;
   %behaviorsTab = 0;
   
   %selectedTab = $mmTabBook.getSelectedPage();
   
   echo("Tab book selected page: " @ %selectedTab);
   if (%selectedTab == %sceneShapeTab)
   {
      //save scene shape data
      %sceneShapeId = $mmSceneShapeList.getSelected();
      if (%sceneShapeId<=0)
         return;
      //Maybe, first check for each item to be worth saving, ie is valid at least.
      
      updateMMSceneShapeTab();

   }
   else if (%selectedTab == %partsTab)
   {
      //save part data
      %partId = $mmShapePartList.getSelected();
      if (%partId<=0)
         return;
      
      updateMMShapePartTab();      
      
   }
   else if (%selectedTab == %behaviorsTab)
   {
      //save behavior data
      
   }
   
   
   unloadMMScene();
   loadMMScene();
}

function updateMMSceneShapeTab()
{
   
   %pos_x = $mmSceneShapePositionX.getText();
   %pos_y = $mmSceneShapePositionY.getText();
   %pos_z = $mmSceneShapePositionZ.getText();
   %query = "UPDATE vector3 SET x=" @ %pos_x @ ",y=" @ %pos_y @ ",z=" @ %pos_z @ 
            " WHERE id=" @ $mmPosId @ ";";
   sqlite.query(%query, 0); 
   
   %rot_x = $mmSceneShapeOrientationX.getText();
   %rot_y = $mmSceneShapeOrientationY.getText();
   %rot_z = $mmSceneShapeOrientationZ.getText();
   %rot_a = $mmSceneShapeOrientationAngle.getText();
   %query = "UPDATE rotation SET x=" @ %rot_x @ ",y=" @ %rot_y @ ",z=" @ %rot_z @ 
             ",angle=" @ %rot_a @ " WHERE id=" @ $mmRotId @ ";";
   sqlite.query(%query, 0); 
   
   %scale_x = $mmSceneShapeScaleX.getText();
   %scale_y = $mmSceneShapeScaleY.getText();
   %scale_z = $mmSceneShapeScaleZ.getText();
   %query = "UPDATE vector3 SET x=" @ %scale_x @ ",y=" @ %scale_y @ ",z=" @ %scale_z @ 
            " WHERE id=" @ $mmScaleId @ ";";
   sqlite.query(%query, 0); 
   
   %group_id = $mmSceneShapeGroupList.getSelected();
   if ((%group_id>0)&&(%group_id!=$mmShapeGroupId))
   {
      %query = "UPDATE sceneShape SET shapeGroup_id=" @ %group_id @ " WHERE id=" @ %sceneShapeId @ ";";
      sqlite.query(%query, 0); 
   }
   
   %behavior_tree = $mmSceneShapeBehaviorTree.getText();
   echo("trying to change behavior tree from " @ $mmSceneShapeBehaviorTreeOrig @ " to " @ %behavior_tree @ "!!!!!!!!!!!!!!!!");
   if ((strlen(%behavior_tree)>0) && (%behavior_tree!$="NULL") &&
            (%behavior_tree!$=$mmSceneShapeBehaviorTreeOrig))
   {
      %query = "UPDATE sceneShape SET behavior_tree='" @ %behavior_tree @ "' WHERE id=" @ %sceneShapeId @ ";";
      sqlite.query(%query, 0); 
   }  
}

function updateMMShapePartTab()
{//First shape part properties, then joint properties...
//FIX: All of these fields need to check for blank spaces, will crash query.

   %tab = $mmTabBook.findObjectByInternalName("shapePartTab");
   %panel = %tab.findObjectByInternalName("shapePartPanel");
   
   %partId = $mmShapePartList.getSelected();
   %dimensionsX = %panel.findObjectByInternalName("shapePartDimensionsX").getText();
   %dimensionsY = %panel.findObjectByInternalName("shapePartDimensionsY").getText();
   %dimensionsZ = %panel.findObjectByInternalName("shapePartDimensionsZ").getText();   
   %orientationX = %panel.findObjectByInternalName("shapePartOrientationX").getText();
   %orientationY = %panel.findObjectByInternalName("shapePartOrientationY").getText();
   %orientationZ = %panel.findObjectByInternalName("shapePartOrientationZ").getText();   
   %offsetX = %panel.findObjectByInternalName("shapePartOffsetX").getText();
   %offsetY = %panel.findObjectByInternalName("shapePartOffsetY").getText();
   %offsetZ = %panel.findObjectByInternalName("shapePartOffsetZ").getText();
   
   %query = "UPDATE physicsShapePart SET ";
   %query = %query @ "dimensions_x=" @ %dimensionsX;
   %query = %query @ ",dimensions_y=" @ %dimensionsY;
   %query = %query @ ",dimensions_z=" @ %dimensionsZ;
   %query = %query @ ",orientation_x=" @ %orientationX;
   %query = %query @ ",orientation_y=" @ %orientationY;
   %query = %query @ ",orientation_z=" @ %orientationZ;   
   %query = %query @ ",offset_x=" @ %offsetX;
   %query = %query @ ",offset_y=" @ %offsetY;
   %query = %query @ ",offset_z=" @ %offsetZ;
   %query = %query @ " WHERE id=" @ %partId @ ";";
   sqlite.query(%query,0);
   
   %jointId = $mmJointList.getSelected();
   if (%jointId<=0)
      return;
      
   %twistLimit = %panel.findObjectByInternalName("jointTwistLimit").getText();
   %swingLimit = %panel.findObjectByInternalName("jointSwingLimit").getText();
   %swingLimit2 = %panel.findObjectByInternalName("jointSwingLimit2").getText();
   %xLimit = %panel.findObjectByInternalName("jointXLimit").getText();
   %yLimit = %panel.findObjectByInternalName("jointYLimit").getText();
   %zLimit = %panel.findObjectByInternalName("jointZLimit").getText();
   %axisX = %panel.findObjectByInternalName("jointAxisX").getText();
   %axisY = %panel.findObjectByInternalName("jointAxisY").getText();
   %axisZ = %panel.findObjectByInternalName("jointAxisZ").getText();
   %normalX = %panel.findObjectByInternalName("jointNormalX").getText();
   %normalY = %panel.findObjectByInternalName("jointNormalY").getText();
   %normalZ = %panel.findObjectByInternalName("jointNormalZ").getText();
   %twistSpring = %panel.findObjectByInternalName("jointTwistSpring").getText();
   %swingSpring = %panel.findObjectByInternalName("jointSwingSpring").getText();
   %springDamper = %panel.findObjectByInternalName("jointSpringDamper").getText();
   %motorSpring = %panel.findObjectByInternalName("jointMotorSpring").getText();
   %motorDamper = %panel.findObjectByInternalName("jointMotorDamper").getText();
   %maxForce = %panel.findObjectByInternalName("jointMaxForce").getText();
   %maxTorque = %panel.findObjectByInternalName("jointMaxTorque").getText();
   
   %query = "UPDATE px3Joint SET ";
   %query = %query @ "twistLimit=" @ %twistLimit;
   %query = %query @ ",swingLimit=" @ %swingLimit;
   %query = %query @ ",swingLimit2=" @ %swingLimit2;
   %query = %query @ ",xLimit=" @ %xLimit;
   %query = %query @ ",yLimit=" @ %yLimit;
   %query = %query @ ",zLimit=" @ %zLimit;
   %query = %query @ ",localAxis_x=" @ %axisX;
   %query = %query @ ",localAxis_y=" @ %axisY;
   %query = %query @ ",localAxis_z=" @ %axisZ;
   %query = %query @ ",localNormal_x=" @ %normalX;
   %query = %query @ ",localNormal_y=" @ %normalY;
   %query = %query @ ",localNormal_z=" @ %normalZ;
   %query = %query @ ",twistSpring=" @ %twistSpring;
   %query = %query @ ",swingSpring=" @ %swingSpring;
   %query = %query @ ",springDamper=" @ %springDamper;
   %query = %query @ ",motorSpring=" @ %motorSpring;
   %query = %query @ ",motorDamper=" @ %motorDamper;
   %query = %query @ ",maxForce=" @ %maxForce;
   %query = %query @ ",maxTorque=" @ %maxTorque;
   %query = %query @ " WHERE id=" @ %jointId @ ";";
   sqlite.query(%query,0);  
   
   
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
         //if (%firstID>0) //Don't do this after all, because it fills up the sceneShape tab before the
         //   $mmSceneShapeList.setSelected(%firstID);// user has actually selected a shape.
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
         
         if ((strlen(%behaviorTree)>0)&&(%behaviorTree!$="NULL"))
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
	         "ss.pos_id AS pos_id,p.x AS pos_x,p.y AS pos_y,p.z AS pos_z," @ 
	         "ss.rot_id AS rot_id,r.x AS rot_x,r.y AS rot_y,r.z AS rot_z,r.angle AS rot_angle," @ 
	         "ss.scale_id AS scale_id,sc.x AS scale_x,sc.y AS scale_y,sc.z AS scale_z " @ 
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
      $mmSceneShapeBehaviorTreeOrig = %behavior_tree;
      %shape_id = sqlite.getColumn(%resultSet, "shape_id");
      $mmShapeId = %shape_id;
      %group_id = sqlite.getColumn(%resultSet, "shapeGroup_id");
      $mmShapeGroupId = %group_id;
      %pos_x = sqlite.getColumn(%resultSet, "pos_x");
      %pos_y = sqlite.getColumn(%resultSet, "pos_y");
      %pos_z = sqlite.getColumn(%resultSet, "pos_z");
      $mmPosId = sqlite.getColumn(%resultSet, "pos_id");
      %rot_x = sqlite.getColumn(%resultSet, "rot_x");
      %rot_y = sqlite.getColumn(%resultSet, "rot_y");
      %rot_z = sqlite.getColumn(%resultSet, "rot_z");
      %rot_a = sqlite.getColumn(%resultSet, "rot_angle");
      $mmRotId = sqlite.getColumn(%resultSet, "rot_id");
      %scale_x = sqlite.getColumn(%resultSet, "scale_x");
      %scale_y = sqlite.getColumn(%resultSet, "scale_y");
      %scale_z = sqlite.getColumn(%resultSet, "scale_z");
      $mmScaleId = sqlite.getColumn(%resultSet, "scale_id");
      
      $mmShapeList.setSelected(%shape_id);
      
      $mmSceneShapeBehaviorTree.setText(%behavior_tree); 
      $mmSceneShapeGroupList.setSelected(%group_id);
      
      $mmSceneShapePositionX.setText(%pos_x);
      $mmSceneShapePositionY.setText(%pos_y);
      $mmSceneShapePositionZ.setText(%pos_z);
      
      $mmSceneShapeOrientationX.setText(%rot_x);
      $mmSceneShapeOrientationY.setText(%rot_y);
      $mmSceneShapeOrientationZ.setText(%rot_z);
      $mmSceneShapeOrientationAngle.setText(%rot_a);
      
      $mmSceneShapeScaleX.setText(%scale_x);
      $mmSceneShapeScaleY.setText(%scale_y);
      $mmSceneShapeScaleZ.setText(%scale_z);
      
      sqlite.clearResult(%resultSet);
   }
}

function addMMSceneShape()
{
   makeSqlGuiForm($addMMSceneShapeID);   
   setupAddMMSceneShapeForm();
}

function deleteMMSceneShape()
{   
   if ($mmSceneShapeList.getSelected()<=0)
      return;
        
   %scene_shape_id = $mmSceneShapeList.getSelected();
   %query = "DELETE FROM sceneShape WHERE id=" @ %scene_shape_id @ ";";
   sqlite.query(%query,0);
   
   echo("EXPOSE MEGAMOTION");
   exposeMegaMotionScenesForm();
   
   unloadMMScene();
   loadMMScene();
}

//May return to behaviorTree being a dropdown, but put it back to textEdit for now.
//function selectMMSceneShapeBehavior()
//{
//}

function selectMMSceneShapeGroup()
{
   
}

function addMMShape()
{
   //HERE: file browser window
}

function reallyAddMMShape()
{
   
}

function deleteMMShape()
{   //First, delete all the shapeParts, then the shape.
   if ($mmShapeList.getSelected()<=0)
      return;
      
   MessageBoxOKCancel( "Warning", 
      "This will permanently delete this shape and all shapeParts referencing it. Are you completely sure?", 
      "reallyDeleteMMShape();",
      "" );     
}

function reallyDeleteMMShape()
{
   if ($mmShapeList.getSelected()<=0)
      return;
      
   %shape_id = $mmShapeList.getSelected();
   
   %query = "DELETE FROM physicsShapePart WHERE physicsShape_id=" @ %shape_id @ ";";
   sqlite.query(%query);
   
   %query = "DELETE FROM shapeMount WHERE parent_shape_id=" @ %shape_id @ " OR child_shape_id=" @
                %shape_id @ ";";
   sqlite.query(%query);
   
   %query = "DELETE FROM physicsShape WHERE id=" @ %shape_id @ ";";
   sqlite.query(%query);
   
   exposeMegaMotionScenesForm();   
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
         //if (%firstID>0)
          //  $mmShapePartList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }   
   
   //Now, node lists: this is based on querying the actual TSShape object, not the database.
   //But, oh crap, this can only work if we have a live instance of the shape in the scene.
   
   //Whoops, gotta be much more careful about not doing this on selecting scene shape, only on changing shape.
   //Finally, see if we want to change the shape of the currently selected sceneShape.
   %sceneShapeId = $mmSceneShapeList.getSelected();
   %shape_id = $mmShapeList.getSelected();
   if ((%sceneShapeId>0)&&(%shape_id!=$mmShapeId))
   {
      MessageBoxYesNo("","Really assign sceneShape " @ %sceneShapeId @ " to shape " @ 
         $mmShapeList.getText() @ "?","reassignShape();","");
   }
}

//Not currently hooked in, can't associate it with selectShape() above until we remove all the times
//we select the shapelist automatically, or really any time we select the shape we're already using.
function reassignShape()
{
   echo("REASSIGNING SHAPE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
   if (($mmSceneShapeList.getSelected()<=0)||($mmShapeList.getSelected()<=0))
      return;
      
   %sceneShapeId = $mmSceneShapeList.getSelected();
   %shape_id = $mmShapeList.getSelected();
   %query = "UPDATE sceneShape SET shape_id=" @ %shape_id @ " WHERE id=" @ %sceneShapeId @ ";";
   echo("reassigning shape: " @ %query );
   sqlite.query(%query,0);
   
   unloadMMScene();
   loadMMScene();
   
   $mmSceneShapeList.setSelected(%sceneShapeId);
}

function selectMMShapePart()
{
   if ($mmShapePartList.getSelected()<=0)
      return;
   
   %partId = $mmShapePartList.getSelected();
   if (%partId<=0)
      return;
	
   %tab = $mmTabBook.findObjectByInternalName("shapePartTab");
   %panel = %tab.findObjectByInternalName("shapePartPanel");
	
   %baseNodeList = %panel.findObjectByInternalName("baseNodeList");
   %childNodeList = %panel.findObjectByInternalName("childNodeList");   
   
   //Actually, let's not make these ones globals... going down to selectShapePart
   %dimensionsX = %panel.findObjectByInternalName("shapePartDimensionsX");
   %dimensionsY = %panel.findObjectByInternalName("shapePartDimensionsY");
   %dimensionsZ = %panel.findObjectByInternalName("shapePartDimensionsZ");         
   %orientationX = %panel.findObjectByInternalName("shapePartOrientationX");//eulers
   %orientationY = %panel.findObjectByInternalName("shapePartOrientationY");
   %orientationZ = %panel.findObjectByInternalName("shapePartOrientationZ");         
   %offsetX = %panel.findObjectByInternalName("shapePartOffsetX");
   %offsetY = %panel.findObjectByInternalName("shapePartOffsetY");
   %offsetZ = %panel.findObjectByInternalName("shapePartOffsetZ");   
	      
	%query = "SELECT * FROM physicsShapePart " @ 
	         "WHERE id=" @ %partId @ ";"; 
	%resultSet = sqlite.query(%query, 0);
	if (%resultSet)
	{
	   if (sqlite.numRows(%resultSet)==1)
	   {	               
         %dimensionsX.setText(sqlite.getColumn(%resultSet, "dimensions_x"));
         %dimensionsY.setText(sqlite.getColumn(%resultSet, "dimensions_y"));
         %dimensionsZ.setText(sqlite.getColumn(%resultSet, "dimensions_z"));         
         %orientationX.setText(sqlite.getColumn(%resultSet, "orientation_x"));
         %orientationY.setText(sqlite.getColumn(%resultSet, "orientation_y"));
         %orientationZ.setText(sqlite.getColumn(%resultSet, "orientation_z"));         
         %offsetX.setText(sqlite.getColumn(%resultSet, "offset_x"));
         %offsetY.setText(sqlite.getColumn(%resultSet, "offset_y"));
         %offsetZ.setText(sqlite.getColumn(%resultSet, "offset_z"));
         
         %jointId = sqlite.getColumn(%resultSet, "px3Joint_id");
         if (%jointId > 0)
            $mmJointList.setSelected(%jointId);
         
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
   
   if (strlen($mmShapePartDimensionsX.getText())>0)//(also check isNumeric)
      %query = %query @ ",dimensions_x=" @ $mmShapePartDimensionsX.getText();
   if (strlen($mmShapePartDimensionsY.getText())>0)//(also check isNumeric)
      %query = %query @ ",dimensions_y=" @ $mmShapePartDimensionsY.getText();
   if (strlen($mmShapePartDimensionsZ.getText())>0)//(also check isNumeric)
      %query = %query @ ",dimensions_z=" @ $mmShapePartDimensionsZ.getText();
   if (strlen($mmShapePartOrientationX.getText())>0)//(also check isNumeric)
      %query = %query @ ",orientation_x=" @ $mmShapePartOrientationX.getText();
   if (strlen($mmShapePartOrientationY.getText())>0)//(also check isNumeric)
      %query = %query @ ",orientation_y=" @ $mmShapePartOrientationY.getText();
   if (strlen($mmShapePartOrientationZ.getText())>0)//(also check isNumeric)
      %query = %query @ ",orientation_z=" @ $mmShapePartOrientationZ.getText();
   if (strlen($mmShapePartOffsetX.getText())>0)//(also check isNumeric)
      %query = %query @ ",offset_x=" @ $mmShapePartOffsetX.getText();
   if (strlen($mmShapePartOffsetY.getText())>0)//(also check isNumeric)
      %query = %query @ ",offset_y=" @ $mmShapePartOffsetY.getText();
   if (strlen($mmShapePartOffsetZ.getText())>0)//(also check isNumeric)
      %query = %query @ ",offset_z=" @ $mmShapePartOffsetZ.getText();
      
   %query = %query @ " WHERE id=" @ %part_id @ ";"; 
   echo("\n" @ %query @ "\n"); 
	sqlite.query(%query, 0);
}


function selectMMJoint()
{
   %jointId = $mmJointList.getSelected();
   if (%jointId<=0)
      return;
      
   %tab = $mmTabBook.findObjectByInternalName("shapePartTab");
   %panel = %tab.findObjectByInternalName("shapePartPanel");
   
   %twistLimit = %panel.findObjectByInternalName("jointTwistLimit");
   %swingLimit = %panel.findObjectByInternalName("jointSwingLimit");
   %swingLimit2 = %panel.findObjectByInternalName("jointSwingLimit2");
   %xLimit = %panel.findObjectByInternalName("jointXLimit");
   %yLimit = %panel.findObjectByInternalName("jointYLimit");
   %zLimit = %panel.findObjectByInternalName("jointZLimit");
   %axisX = %panel.findObjectByInternalName("jointAxisX");
   %axisY = %panel.findObjectByInternalName("jointAxisY");
   %axisZ = %panel.findObjectByInternalName("jointAxisZ");
   %normalX = %panel.findObjectByInternalName("jointNormalX");
   %normalY = %panel.findObjectByInternalName("jointNormalY");
   %normalZ = %panel.findObjectByInternalName("jointNormalZ");
   %twistSpring = %panel.findObjectByInternalName("jointTwistSpring");
   %swingSpring = %panel.findObjectByInternalName("jointSwingSpring");
   %springDamper = %panel.findObjectByInternalName("jointSpringDamper");
   %motorSpring = %panel.findObjectByInternalName("jointMotorSpring");
   %motorDamper = %panel.findObjectByInternalName("jointMotorDamper");
   %maxForce = %panel.findObjectByInternalName("jointMaxForce");
   %maxTorque = %panel.findObjectByInternalName("jointMaxTorque");
   
   %query = "SELECT * FROM px3Joint WHERE id=" @ %jointId @ ";";
   %resultSet = sqlite.query(%query,0);
   if (%resultSet)
	{
	   if (sqlite.numRows(%resultSet)==1)
	   {	               
         %twistLimit.setText(sqlite.getColumn(%resultSet, "twistLimit"));
         %swingLimit.setText(sqlite.getColumn(%resultSet, "swingLimit"));
         %swingLimit2.setText(sqlite.getColumn(%resultSet, "swingLimit2"));
         %xLimit.setText(sqlite.getColumn(%resultSet, "xLimit"));
         %yLimit.setText(sqlite.getColumn(%resultSet, "yLimit"));
         %zLimit.setText(sqlite.getColumn(%resultSet, "zLimit"));
         %axisX.setText(sqlite.getColumn(%resultSet, "localAxis_x"));
         %axisY.setText(sqlite.getColumn(%resultSet, "localAxis_y"));
         %axisZ.setText(sqlite.getColumn(%resultSet, "localAxis_z"));
         %normalX.setText(sqlite.getColumn(%resultSet, "localNormal_x"));
         %normalY.setText(sqlite.getColumn(%resultSet, "localNormal_y"));
         %normalZ.setText(sqlite.getColumn(%resultSet, "localNormal_z"));
         %twistSpring.setText(sqlite.getColumn(%resultSet, "twistSpring"));
         %swingSpring.setText(sqlite.getColumn(%resultSet, "swingSpring"));
         %springDamper.setText(sqlite.getColumn(%resultSet, "springDamper"));
         %motorSpring.setText(sqlite.getColumn(%resultSet, "motorSpring"));
         %motorDamper.setText(sqlite.getColumn(%resultSet, "motorDamper"));
         %maxForce.setText(sqlite.getColumn(%resultSet, "maxForce"));
         %maxTorque.setText(sqlite.getColumn(%resultSet, "maxTorque"));
         $mmJointTypeList.setSelected(sqlite.getColumn(%resultSet, "jointType"));
	   }
	}
}


//////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////

function setupAddMMSceneShapeForm()
{
   
   %shapeList = addMMSceneShapeWindow.findObjectByInternalName("shapeList"); 
   %groupList = addMMSceneShapeWindow.findObjectByInternalName("groupList"); 
   if ((!isDefined(%shapeList))||(!isDefined(%groupList)))
      return;
   
   //%shapeList
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
            
            %shapeList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         //if (%firstID>0) 
            //$mmShapeList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }
   
   //groupList
   %query = "SELECT id,name FROM shapeGroup ORDER BY name;";
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
            
            %groupList.add(%name,%id);
            sqlite.nextRow(%resultSet);         
         }
         //if (%firstID>0) 
            //$mmShapeList.setSelected(%firstID);
      }
      sqlite.clearResult(%resultSet);
   }    
   
   
   %posX = addMMSceneShapeWindow.findObjectByInternalName("shapePositionX"); 
   %posY = addMMSceneShapeWindow.findObjectByInternalName("shapePositionY"); 
   %posZ = addMMSceneShapeWindow.findObjectByInternalName("shapePositionZ"); 
   
   %oriX = addMMSceneShapeWindow.findObjectByInternalName("shapeOrientationX"); 
   %oriY = addMMSceneShapeWindow.findObjectByInternalName("shapeOrientationY"); 
   %oriZ = addMMSceneShapeWindow.findObjectByInternalName("shapeOrientationZ"); 
   %oriAngle = addMMSceneShapeWindow.findObjectByInternalName("shapeOrientationAngle"); 
   
   %scaleX = addMMSceneShapeWindow.findObjectByInternalName("shapeScaleX"); 
   %scaleY = addMMSceneShapeWindow.findObjectByInternalName("shapeScaleY"); 
   %scaleZ = addMMSceneShapeWindow.findObjectByInternalName("shapeScaleZ"); 
   
   %posX.setText(0);
   %posY.setText(0);
   %posZ.setText(0);
   
   %oriX.setText(0);
   %oriY.setText(0);
   %oriZ.setText(1);
   %oriAngle.setText(0);
   
   %scaleX.setText(1);
   %scaleY.setText(1);
   %scaleZ.setText(1);
     
   %blockX = addMMSceneShapeWindow.findObjectByInternalName("blockCountX"); 
   %blockY = addMMSceneShapeWindow.findObjectByInternalName("blockCountY"); 
   %blockPaddingX = addMMSceneShapeWindow.findObjectByInternalName("blockPaddingX"); 
   %blockPaddingY = addMMSceneShapeWindow.findObjectByInternalName("blockPaddingY"); 
   %blockVariationX = addMMSceneShapeWindow.findObjectByInternalName("blockVariationX"); 
   %blockVariationY = addMMSceneShapeWindow.findObjectByInternalName("blockVariationY"); 
   
   %blockX.setText(2);
   %blockY.setText(2);
   %blockPaddingX.setText(0);
   %blockPaddingY.setText(0);
   %blockVariationX.setText(0);
   %blockVariationY.setText(0);
   
   %posX = addMMSceneShapeWindow.findObjectByInternalName("shapePositionX"); 
   %posY = addMMSceneShapeWindow.findObjectByInternalName("shapePositionY"); 
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
      %shape_id = addMMSceneShapeWindow.findObjectByInternalName("shapeList").getSelected();
      %group_id = addMMSceneShapeWindow.findObjectByInternalName("groupList").getSelected();
      %behaviorTree = addMMSceneShapeWindow.findObjectByInternalName("behaviorTree").getText();
      
      %pos_x = addMMSceneShapeWindow.findObjectByInternalName("shapePositionX").getText();
      %pos_y = addMMSceneShapeWindow.findObjectByInternalName("shapePositionY").getText();
      %pos_z = addMMSceneShapeWindow.findObjectByInternalName("shapePositionZ").getText(); 
      
      %ori_x = addMMSceneShapeWindow.findObjectByInternalName("shapeOrientationX").getText();
      %ori_y = addMMSceneShapeWindow.findObjectByInternalName("shapeOrientationY").getText();
      %ori_z = addMMSceneShapeWindow.findObjectByInternalName("shapeOrientationZ").getText(); 
      %ori_a = addMMSceneShapeWindow.findObjectByInternalName("shapeOrientationAngle").getText();
      
      %scale_x = addMMSceneShapeWindow.findObjectByInternalName("shapeScaleX").getText();
      %scale_y = addMMSceneShapeWindow.findObjectByInternalName("shapeScaleY").getText();
      %scale_z = addMMSceneShapeWindow.findObjectByInternalName("shapeScaleZ").getText(); 
      
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
      
      //
      //HERE: do some sanity testing before we commit!
      //
      
      //And, insert pos, rot & scale, and get the ids back. Q: what is the most efficient way to do this?
      //For now, I'm inserting the other stuff, grabbing an id, and then inserting the pos/rot/scale and
      //using last_insert_rowid() in update statements.
      %query = "INSERT INTO sceneShape (name,scene_id,shape_id) " @
               " VALUES ('" @ %name @ "'," @ %scene_id @ "," @ %shape_id @ ");";
      sqlite.query(%query,0);

      //These are optional, check for values first.      
      //,shapeGroup_id,behaviorTree
      // "," @ %group_id @ ",'" @ %behaviorTree @ "'"

      %ssID = 0;
      %query = "SELECT id FROM sceneShape WHERE name='" @ %name @ "' AND scene_id=" @ %scene_id @ ";";
      %resultSet = sqlite.query(%query,0);
      if (sqlite.numRows(%resultSet)==1)
         %ss_id = sqlite.getColumn(%resultSet, "id");
      if (%ss_id==0)
         return;
      
      //For first pass at least, just add default values and spawn the character at scene origin.
      %query = "INSERT INTO vector3 (x,y,z) VALUES (" @ %pos_x @ "," @ %pos_y @ "," @ %pos_z @ ");";
      sqlite.query(%query,0);
      %query = "UPDATE sceneShape SET pos_id=last_insert_rowid() WHERE id=" @ %ss_id @ ";";
      sqlite.query(%query, 0);  
            
      %query = "INSERT INTO rotation (x,y,z,angle) VALUES (" @ %ori_x @ "," @ %ori_y @ "," @ 
                     %ori_z @  "," @ %ori_a @ ");";      
      sqlite.query(%query,0);
      %query = "UPDATE sceneShape SET rot_id=last_insert_rowid() WHERE id=" @ %ss_id @ ";";
      sqlite.query(%query, 0);  
      
      %query = "INSERT INTO vector3 (x,y,z) VALUES (" @ %scale_x @ "," @ %scale_y @ "," @ %scale_z @ ");";      
      sqlite.query(%query,0);
      %query = "UPDATE sceneShape SET scale_id=last_insert_rowid() WHERE id=" @ %ss_id @ ";";
      sqlite.query(%query, 0);  
      
      exposeMegaMotionScenesForm();
      unloadMMScene();
      loadMMScene();
   }   
}



//////////////////////////////////////////////////////////////////////



function addMMSceneShapeBlock() 
{
   
   
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
