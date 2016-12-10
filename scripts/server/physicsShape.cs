
function PhysicsShape::shapeSpecifics(%this)
{
   //Misc section for things that haven't found a better place yet.
   if (%this.dataBlock $= "M4Physics") 
   {            
      %this.setActionSeq("ambient","ambient");//This might not always be idle, could be just breathing
      %this.setActionSeq("idle","ambient");// and idle could be that plus fidgeting, etc.
      %this.setActionSeq("walk","walk");      
      %this.setActionSeq("run","run");
      %this.setActionSeq("rightGetup","rSideGetup");
      %this.setActionSeq("leftGetup","lSideGetup");
      %this.setActionSeq("frontGetup","frontGetup");
      %this.setActionSeq("backGetup","backGetup");      
   } 
   else if (%this.dataBlock $= "bo105Physics") 
   {            
   } 
   else if (%this.dataBlock $= "dragonflyPhysics") 
   {            
   } 
   else 
   if (%this.dataBlock $= "ka50Physics") 
   {
      echo("Calling shapeSpecifics for ka50!!!!!!!!!!!!!!!!!!!!!");
      %this.schedule(500,"showRotorBlades");
      //%this.schedule(500,"setUseDataSource",true);
      //%this.setIsRecording(true);
   }  
}

///////////////////////////////////////////////////////////////////////////////////////
//HERE: all this needs to be moved farther down into specific behavior trees. 
function PhysicsShape::onStartup(%this)
{
   echo(%this @ " calling onStartup! position " @ %this.getPosition() @ " tree " @ %this.behaviorTree);
   /*
   if (%this.dataBlock $= "M4Physics")
   {      
      %this.setActionSeq("ambient","ambient");//This might not always be idle, could be just breathing
      %this.setActionSeq("idle","ambient");// and idle could be that plus fidgeting, etc.
      %this.setActionSeq("walk","walk");      
      %this.setActionSeq("run","run");   
         
      %this.setActionSeq("fall","runscerd");//Also doesn't matter if you use the same anim for multiple actionSeqs.
      %this.setActionSeq("getup","rSideGetup");   
      %this.setActionSeq("attack","power_punch_down");
      %this.setActionSeq("block","punch_uppercut");//TEMP, don't have any blocking anims atm
      
      //%this.setActionSeq("runscerd","runscerd");
      //%this.setActionSeq("crouch","crouch");
      
      //%this.setIsRecording(true);
      
      %this.groundMove();
      %this.currentAction = "walk";//LATER, dependent on which behaviorTree.
      
      //%this.setIsRecording(true);
      //echo("starting up a M4 physics shape!");      
   } 
   else if (%this.dataBlock $= "bo105Physics") //useDataSource: holding off on this for now.
   {
      //%this.setName("bo105");
      //%this.schedule(500,"setUseDataSource",true);
      //%this.setIsRecording(true);
      //%this.showNodes();     
   } 
   else if (%this.dataBlock $= "dragonflyPhysics") 
   {
      //%this.setName("dragonfly");
      //%this.schedule(500,"setUseDataSource",true);
   }
   else if (%this.dataBlock $= "ka50Physics") 
   { 
      //%this.setName("ka50");
      //%this.schedule(1900,"setUseDataSource",true);
      %this.schedule(2000,"showRotorBlades");
      //%this.showNodes();   
   }
   */
}

function PhysicsShape::openSteerSimpleVehicle(%this)
{   
   if (%this.getVehicleID()>0)
      return;//We've already done all this, so don't do it again.
      
   %this.currentAction = "walk";
   
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;   
      
   %clientShape.createVehicle(%clientShape.getPosition(),0);
   %clientShape.openSteerID = %this.openSteerID;
   
   if (%clientShape.openSteerID <= 0)
      return;
   
   //openSteerProfile table, with openSteer_id in sceneShape. Maybe move this to engine? Probably slow, esp. for blocks.
   %query = "SELECT * FROM openSteerProfile WHERE id=" @ %this.openSteerID @ ";";
   %resultSet = sqlite.query(%query,0);
   if (sqlite.numRows(%resultSet)==1)
   {
      %clientShape.setOpenSteerMass(sqlite.getColumn(%resultSet,"mass"));
      %clientShape.setOpenSteerRadius(sqlite.getColumn(%resultSet,"radius"));
      %clientShape.setOpenSteerMaxForce(sqlite.getColumn(%resultSet,"maxForce"));
      %clientShape.setOpenSteerMaxSpeed(sqlite.getColumn(%resultSet,"maxSpeed"));
      %clientShape.setOpenSteerWanderChance(sqlite.getColumn(%resultSet,"wanderChance"));
      %clientShape.setOpenSteerWanderWeight(sqlite.getColumn(%resultSet,"wanderWeight"));
      %clientShape.setOpenSteerSeekTargetWeight(sqlite.getColumn(%resultSet,"seekTargetWeight"));
      %clientShape.setOpenSteerAvoidTargetWeight(sqlite.getColumn(%resultSet,"avoidTargetWeight"));
      %clientShape.setOpenSteerSeekNeighborWeight(sqlite.getColumn(%resultSet,"seekNeighborWeight"));
      %clientShape.setOpenSteerAvoidNeighborWeight(sqlite.getColumn(%resultSet,"avoidNeighborWeight"));
      %clientShape.setOpenSteerAvoidNavMeshEdgeWeight(sqlite.getColumn(%resultSet,"avoidNavMeshEdgeWeight"));
      %clientShape.setOpenSteerDetectNavMeshEdgeRange(sqlite.getColumn(%resultSet,"detectNavMeshEdgeRange"));
   }
}

function PhysicsShape::openSteerNavVehicle(%this)
{   
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;   
      
   %this.currentPathNode = 0;
   %clientShape.currentPathNode = 0;
   
   if (%clientShape.getVehicleID()>0)
      return;//We've already done all this, so don't do it again.
      
   if (!isObject(Nav))//TEMP! Search MissionGroup for all NavMesh objects, pick best one.
      return;
      
   %this.setNavMesh("Nav");
   
   %this.currentAction = "walk";
   
      
   %clientShape.createVehicle(%clientShape.getPosition(),0);
   %clientShape.openSteerID = %this.openSteerID;
   
   if (%clientShape.openSteerID <= 0)
      return;
   
   %id = %clientShape.openSteerID;
   %query = "SELECT * FROM openSteerProfile WHERE id=" @ %this.openSteerID @ ";";
   %resultSet = sqlite.query(%query,0);
   //echo("trying to setup an opensteer vehicle! opensteer id " @ %id @ " maxForce " @ $openSteerProfile[%id].maxForce);
   
   if (%resultSet)
   {
      %clientShape.setOpenSteerMass(sqlite.getColumn(%resultSet,"mass"));
      %clientShape.setOpenSteerRadius(sqlite.getColumn(%resultSet,"radius"));
      %clientShape.setOpenSteerMaxForce(sqlite.getColumn(%resultSet,"maxForce"));
      %clientShape.setOpenSteerMaxSpeed(sqlite.getColumn(%resultSet,"maxSpeed"));
      %clientShape.setOpenSteerWanderChance(sqlite.getColumn(%resultSet,"wanderChance"));
      %clientShape.setOpenSteerWanderWeight(sqlite.getColumn(%resultSet,"wanderWeight"));
      %clientShape.setOpenSteerSeekTargetWeight(sqlite.getColumn(%resultSet,"seekTargetWeight"));
      %clientShape.setOpenSteerAvoidTargetWeight(sqlite.getColumn(%resultSet,"avoidTargetWeight"));
      %clientShape.setOpenSteerSeekNeighborWeight(sqlite.getColumn(%resultSet,"seekNeighborWeight"));
      %clientShape.setOpenSteerAvoidNeighborWeight(sqlite.getColumn(%resultSet,"avoidNeighborWeight"));
      %clientShape.setOpenSteerAvoidNavMeshEdgeWeight(sqlite.getColumn(%resultSet,"avoidNavMeshEdgeWeight"));
      %clientShape.setOpenSteerDetectNavMeshEdgeRange(sqlite.getColumn(%resultSet,"detectNavMeshEdgeRange"));
   }   
}

/*
   //AND... NOPE! Not yet.This will be trivial but no time right now for it. Array didn't work. Need to create
   // a new engine level object and communicate properties to script, so I can say new OpenSteerProfile() and
   // set up each of them once at load time.
   %clientShape.setOpenSteerMass($openSteerProfile[%id].mass);
   %clientShape.setOpenSteerRadius($openSteerProfile[%id].radius);
   %clientShape.setOpenSteerMaxForce($openSteerProfile[%id].maxForce);
   %clientShape.setOpenSteerMaxSpeed($openSteerProfile[%id].maxSpeed);
   %clientShape.setOpenSteerWanderChance($openSteerProfile[%id].wanderChance);
   %clientShape.setOpenSteerWanderWeight($openSteerProfile[%id].wanderWeight);
   %clientShape.setOpenSteerSeekTargetWeight($openSteerProfile[%id].seekTargetWeight);
   %clientShape.setOpenSteerAvoidTargetWeight($openSteerProfile[%id].avoidTargetWeight);
   %clientShape.setOpenSteerSeekNeighborWeight($openSteerProfile[%id].seekNeighborWeight);
   %clientShape.setOpenSteerAvoidNeighborWeight($openSteerProfile[%id].avoidNeighborWeight);
   %clientShape.setOpenSteerAvoidNavMeshEdgeWeight($openSteerProfile[%id].avoidNavMeshEdgeWeight);
   %clientShape.setOpenSteerDetectNavMeshEdgeRange($openSteerProfile[%id].detectNavMeshEdgeRange);
   */
   
function PhysicsShape::orientTo(%this, %dest)
{
   %pos = isObject(%dest) ? %dest.getPosition() : %dest;
   
   %this.orientToPos(%pos);
}

function PhysicsShape::moveTo(%this, %dest, %slowDown)
{
   %pos = isObject(%dest) ? %dest.getPosition() : %dest;
   
   %this.orientToPos(%pos);
   
   %this.actionSeq(%this.currentAction);
}

function PhysicsShape::say(%this, %message)//Testing, does this only work for AIPlayers?
{
   chatMessageAll(%this, '\c3%1: %2', %this.getid(), %message);  
}

function PhysicsShape::findTargetShapePos(%this)
{ 
   echo(%this.sceneShapeID @ " is seeking target shape: " @ %this.targetShapeID @ " isServer " @ %this.isServerObject());
   if (%this.targetShapeID>0)
   {
      for (%i = 0; %i < SceneShapes.getCount();%i++)
      {
         %targ = SceneShapes.getObject(%i);  
         if (%targ.sceneShapeID==%this.targetShapeID)
         {
            %this.goalPos = %targ.getClientObject().getPosition();
            %this.targetItem = %targ.getClientObject();
            echo(%this.sceneShapeID @ " found target shape: " @ %this.targetShapeID @ " pos " @ %this.goalPos);
            return;
         }
      }
   }   
   return;
}

function PhysicsShape::checkTeamProximity(%this,%detectDist)
{  //HERE: we need to loop through all members of enemy team, and find the closest one, and return
   //TRUE if the closest one is within our minimum threshold value. (?) Or just return the distance?
   
   echo("defender checking team proximity! detectDist " @ %detectDist);
    
   %diff = 0;
   //%minDiff = 9999999;//I guess we don't need this, if we're not returning the closest one.
   %clientShape = 0;
   
   if (%this.shapeGroupID==1)
      %enemyGroupID = 2;
   else if (%this.shapeGroupID==2)
      %enemyGroupID = 1;
   
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else  
      %clientShape = %this;
      
   for (%i = 0; %i < SceneShapes.getCount();%i++)
   {
      %targ = SceneShapes.getObject(%i);  
      if (%targ.shapeGroupID==%enemyGroupID)
      {         
         %diff = VectorLen(VectorSub(%clientShape.position,%targ.getClientObject().position));
         echo(%this @ " checking an enemy shape! dist: " @ %diff);
         //if (%diff < %minDiff)
         //   %minDiff = %diff;
         if (%diff < %detectDist)
            return true;
      }
   }   
   return false;
}

function PhysicsShape::findPlayerPos(%this)
{ 
   %db = %this.dataBlock;//Consider getting findItemRange out of the datablock and into something else.
   initContainerRadiusSearch( %this.position, %db.findItemRange, $TypeMasks::PlayerObjectType );
      
   while ( (%item = containerSearchNext()) != 0 )
   {
      if ( %item.getClassName() $= "Player" )
      {
         %this.goalPos = %item.getPosition();  
         echo("player position: " @ %this.goalPos );          
         return;
      }
   }
   
   %this.goalPos = "0 0 0";
   return;
}

////////////////////////////////////////////////////

function PhysicsShape::relaxNeck(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("neck"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("head"),true);   
}

function PhysicsShape::relaxTorso(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("abdomen"),true); 
   %clientShape.setPartDynamic(%clientShape.getBodyNum("chest"),true); 
   %clientShape.setPartDynamic(%clientShape.getBodyNum("neck"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("head"),true);   
}

function PhysicsShape::relaxAll(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("abdomen"),true); 
   %clientShape.setPartDynamic(%clientShape.getBodyNum("chest"),true); 
   %clientShape.setPartDynamic(%clientShape.getBodyNum("neck"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("head"),true);  
    
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rHand"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rForeArm"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rShldr"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rCollar"),true); 
   
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lHand"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lForeArm"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lShldr"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lCollar"),true); 
   
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rFoot"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rShin"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rThigh"),true); 
   
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lFoot"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lShin"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lThigh"),true);  
}

function PhysicsShape::relaxRightArm(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rHand"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rForeArm"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rShldr"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rCollar"),true);  
}

function PhysicsShape::relaxLeftArm(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lHand"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lForeArm"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lShldr"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lCollar"),true);  
}

function PhysicsShape::relaxRightLeg(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rFoot"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rShin"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("rThigh"),true);   
}

function PhysicsShape::relaxLeftLeg(%this)
{
   if (%this.isServerObject())
      %clientShape = %this.getClientObject();
   else 
      %clientShape = %this;
      
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lFoot"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lShin"),true);  
   %clientShape.setPartDynamic(%clientShape.getBodyNum("lThigh"),true);  
}