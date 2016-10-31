//--- OBJECT WRITE BEGIN ---
new Root(openSteerGoToTargetTree) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";      
      
      new ScriptEval() {
         behaviorScript = "%obj.openSteerVehicle(); %obj.findTargetShapePos(); %obj.groundMove();";
         defaultReturnStatus = "SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
          
      new Loop() {
         numLoops = "0";
         terminationPolicy = "ON_FAILURE"; 
         canSave = "1";
         canSaveDynamicFields = "1";            
         
         new Sequence() {
            canSave = "1";
            canSaveDynamicFields = "1";      
            
            new ScriptedBehavior() {
               preconditionMode = "TICK";
               class = "openSteerGoToTarget";
               canSave = "1";
               canSaveDynamicFields = "1";
            };  
         };
      };
   };
};
//--- OBJECT WRITE END ---
