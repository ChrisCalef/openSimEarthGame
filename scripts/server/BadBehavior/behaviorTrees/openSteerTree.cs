//--- OBJECT WRITE BEGIN ---
new Root(openSteerTree) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";      
      
      new ScriptEval() {//Ultimately this should be removed and we should have one base tree that 
         behaviorScript = "%obj.onStartup();";//calls this tree or other trees, after doing startup.
         defaultReturnStatus = "SUCCESS";
         canSave = "1";
         canSaveDynamicFields = "1";
      };
      new ScriptEval() {
         behaviorScript = "%obj.openSteerVehicle();";
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
            
            new ScriptEval() {
               behaviorScript = "%obj.targetType=\"Player\";";
               defaultReturnStatus = "SUCCESS";
               canSave = "1";
               canSaveDynamicFields = "1";
            }; 
            new SubTree() {
               subTreeName = "openSteerTargetTree";
               internalName = "go to target";
               canSave = "1";
               canSaveDynamicFields = "1";
            };   
            new ScriptEval() {
               behaviorScript = "%obj.targetType=\"Health\";";
               defaultReturnStatus = "SUCCESS";
               canSave = "1";
               canSaveDynamicFields = "1";
            }; 
            new SubTree() {
               subTreeName = "openSteerTargetTree";
               internalName = "go to target";
               canSave = "1";
               canSaveDynamicFields = "1";
            };      
         };
      };
   };
};
//--- OBJECT WRITE END ---
