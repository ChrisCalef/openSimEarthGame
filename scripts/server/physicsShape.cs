// PhysicsShape functions - MegaMotion

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
      
      //The reason for ActionSequences is so you don't have to use the same walk or run anim for every actor, 
         //but you can treat them as if they were all the same in practice.
      //if (getRandom()>0.4)
         %this.setActionSeq("run","run");
      //else 
      //   %this.setActionSeq("run","runscerd");
         
      //%this.setActionSeq("runscerd","runscerd");
      %this.setActionSeq("fall","runscerd");//Also doesn't matter if you use the same anim for multiple actionSeqs.
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
   %clientShape.openSteerID = %this.openSteerID;
   
   //HERE: where do we store these in the DB???   
   //OKAY, question answered: openSteerProfile table, with openSteer_id in sceneShape.
   %query = "SELECT * FROM openSteerProfile WHERE id=" @ %this.openSteerID @ ";";
   %resultSet = sqlite.query(%query,0);
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
