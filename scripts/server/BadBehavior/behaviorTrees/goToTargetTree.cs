//--- OBJECT WRITE BEGIN ---
new Root(goToTargetTree) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";

      new ScriptedBehavior() {
         preconditionMode = "ONCE";
         internalName = "look for target";
         class = "findTarget";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      
      //new Loop() {
         //numLoops = "0";
         //terminationPolicy = "ON_SUCCESS";
         //canSave = "1";
         //canSaveDynamicFields = "1";
         
         new ScriptedBehavior() {
            preconditionMode = "TICK";
            internalName = "go to target";
            class = "goToTarget";
            canSave = "1";
            canSaveDynamicFields = "1";
         };
      //};
   };
};
//--- OBJECT WRITE END ---

//=============================================================================
// findTarget task
//=============================================================================
function findTarget::behavior(%this, %obj)
{
   // get the objects datablock
   %db = %obj.dataBlock;
   %category = %obj.targetType;
   echo(%this.getId() @ " trying to find target: " @ %obj.targetType @ "pos " @ %obj.position);
   // do a container search for items
   initContainerRadiusSearch( %obj.position, %db.findItemRange, %db.itemObjectTypes );
   while ( (%item = containerSearchNext()) != 0 )
   {
      if (%item.dataBlock.category $= %category && %item.isEnabled() && !%item.isHidden())
      {      
         %diff = VectorSub(%obj.position,%item.position);
      
         // check that the item is within the bots view cone
         //if(%obj.checkInFov(%item, %db.visionFov))
         if (true)// (We don't have a checkInFov for physicsShapes yet)
         {
            // set the targetItem field on the bot
            %obj.targetItem = %item;
            break;
         }
      }
   }
   
   return isObject(%obj.targetItem) ? SUCCESS : FAILURE;
}


//=============================================================================
// goToTarget task
//=============================================================================
function goToTarget::precondition(%this, %obj)
{
   // check that we have a valid health item to go for
   return (isObject(%obj.targetItem) && %obj.targetItem.isEnabled() && !%obj.targetItem.isHidden());  
}

function goToTarget::onEnter(%this, %obj)
{
   %obj.moveTo(%obj.targetItem);  
   %obj.actionSeq("run");
   //%obj.orientToPos(%obj.targetItem.position);
}

function goToTarget::behavior(%this, %obj)
{
   // succeed when we reach the item
   //HERE: we need targetitem position to be on the ground, not at the actual position, 
   //or else we can never be closer than the height of the object.
   %groundPos = %obj.findGroundPosition(%obj.targetItem.position);
   %diff = VectorSub(%groundPos,%obj.getClientPosition());
   //if(!%obj.atDestination)
   
   //echo(%obj.getId() @ " is looking for target, my position " @ %obj.getClientPosition() @ 
   //" target position " @ %groundPos @  " distance = " @ VectorLen(%diff) );
   
   if ( VectorLen(%diff) > %obj.dataBlock.foundItemDistance )
      return RUNNING;
   
   return SUCCESS;
}