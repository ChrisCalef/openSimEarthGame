//--- OBJECT WRITE BEGIN ---
new Root(openSteerTargetTree) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";

      new ScriptedBehavior() {//Same findTarget as goToTargetTree, so don't forget to include that!
         preconditionMode = "ONCE";
         internalName = "openSteer find target";
         class = "openSteerFindTarget";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      
      new Loop() {
         numLoops = "0";
         terminationPolicy = "ON_SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
         
         new Sequence() {
            canSave = "1";
            canSaveDynamicFields = "1";     
            
             
            new ScriptEval() {
               behaviorScript = "echo(\"openSteer go to target...\");";
               defaultReturnStatus = "SUCCESS";
               canSave = "1";
               canSaveDynamicFields = "1";
            };
            new ScriptedBehavior() {
               preconditionMode = "TICK";
               internalName = "openSteer go to target";
               class = "openSteerGoToTarget";
               canSave = "1";
               canSaveDynamicFields = "1";
            };
         };
      };
   };
};
//--- OBJECT WRITE END ---


//=============================================================================
// findTarget task - same as goToTarget, but calling it didn't work...?
//=============================================================================
function openSteerFindTarget::behavior(%this, %obj)
{
   // get the objects datablock
   %db = %obj.dataBlock;
   %category = %obj.targetType;
   //echo(%this.getId() @ " trying to find target: " @ %obj.targetType @ "pos " @ %obj.position);
   // do a container search for items
   
   //HERE: let's use category to switch between player and item searches. This could get more involved.
   if (%category $= "Player")
      initContainerRadiusSearch( %obj.position, %db.findItemRange, %db.targetObjectTypes );
   else//if not player, assume an item, but more (esp. PhysicsShape) categories could be added.
      initContainerRadiusSearch( %obj.position, %db.findItemRange, %db.itemObjectTypes );
   
   while ( (%item = containerSearchNext()) != 0 )
   {
      if ( (%category$="Player") ||
            (%item.dataBlock.category $= %category && %item.isEnabled() && !%item.isHidden()) )
      {      
         %diff = VectorSub(%obj.position,%item.position);
      
         // check that the item is within the bots view cone
         //if(%obj.checkInFov(%item, %db.visionFov))
         if (true)// (We don't have a checkInFov for physicsShapes yet)
         {
            // set the targetItem field on the bot
            %obj.targetItem = %item;
            //echo("FOUND TARGET: " @ %item  @ "  " @ %item.getClassName() @ "  " @ %item.getPosition() );
            break;
         }
      }
   }
   
   return isObject(%obj.targetItem) ? SUCCESS : FAILURE;
}


//=============================================================================
// goToTarget task
//=============================================================================
function openSteerGoToTarget::precondition(%this, %obj)
{
   // check that we have a valid health item to go for
   //echo("checking precondition: " @ (isObject(%obj.targetItem) && %obj.targetItem.isEnabled() && !%obj.targetItem.isHidden()) );
   return (isObject(%obj.targetItem) && %obj.targetItem.isEnabled() && !%obj.targetItem.isHidden());  
}

function openSteerGoToTarget::onEnter(%this, %obj)
{
   if (%obj.currentPathNode==0)//Well this is weird, this time it's calling onEnter all the time...??
   {
      %obj.setNavPathTo(%obj.targetItem.position);
   
      %obj.currentPathNode = 1;
      %obj.currentPathGoal = %obj.getNavPathNode(%obj.currentPathNode);
      //echo("openSteerGoToTarget, onEnter, first goal: " @ %obj.currentPathGoal @ 
      //         " ultimate target " @ %obj.targetItem.position);
      %clientObj =  %obj.getClientObject();
      %clientObj.setOpenSteerMoveTarget(%obj.currentPathGoal);
   }
}

function openSteerGoToTarget::behavior(%this, %obj)
{
   // succeed when we reach the item
   //HERE: we need targetitem position to be on the ground, not at the actual position, 
   //or else we can never be closer than the height of the object.
  
   
   %clientObj =  %obj.getClientObject();
      
   %groundPos = %obj.findGroundPosition(%obj.targetItem.position);
   %targetMove = VectorLen(%groundPos - %obj.getNavPathNode(%obj.getNavPathSize()-1));
   %clientGroundPos = %obj.findGroundPosition(%obj.getClientPosition());//NOTE: this "client" refers to the player
   %diff = VectorSub(%groundPos,%clientGroundPos);//Where "clientObj" above refers to client ghost of this shape.
   //if(!%obj.atDestination)   
   //echo("my position " @ %obj.getClientPosition() @ " goal " @ %obj.currentPathGoal @ 
   //      " diff " @ VectorLen(%diff)  @  "  target move " @ %targetMove @ " target pos " @ 
   //       %obj.targetItem.position );
   
   if ((%obj.currentPathNode == 0)||(%targetMove>2.0))//2.0=%obj.targetMoveThreshold?
   {    
      //echo("setting NavPathTo, pathNode=0");
      %obj.setNavPathTo(%groundPos);   
      %obj.currentPathNode = 1;
      %obj.currentPathGoal = %obj.getNavPathNode(%obj.currentPathNode);
      //echo("New Target, first goal: " @ %obj.currentPathGoal);
      //%obj.moveTo(%obj.currentPathGoal);   
      %clientObj.setOpenSteerMoveTarget(%obj.currentPathGoal);
   }
   if ( VectorLen(%diff) > %obj.dataBlock.foundItemDistance )
   {      
      %nodeDiff = VectorSub(%obj.currentPathGoal,%clientGroundPos);
      //echo("checking distance to path node: " @ VectorLen(%diff));
      //echo(%obj.getId() @ " is looking for target, my position " @ %clientGroundPos @ 
      //      " target position " @ %obj.currentPathGoal @  " distance = " @ VectorLen(%diff) );

      if (VectorLen(%nodeDiff) < %obj.dataBlock.foundItemDistance)
      {
         %obj.currentPathNode++;
         %obj.currentPathGoal = %obj.getNavPathNode(%obj.currentPathNode);
         
         //echo("setting new path goal: " @ %obj.currentPathGoal @ "  current node " @ %obj.currentPathNode @
         //          " total nodes " @ %obj.getNavPathSize() );
         //%obj.moveTo(%obj.currentPathGoal); 
         %clientObj.setOpenSteerMoveTarget(%obj.currentPathGoal);
      }
      return RUNNING;
   }
   else
   {
      %obj.currentPathNode = 0;
      //%obj.currentPathGoal = %obj.getClientPosition();
      %obj.actionSeq("ambient");
      return SUCCESS;
   }
}