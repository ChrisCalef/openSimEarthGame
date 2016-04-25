//--- OBJECT WRITE BEGIN ---
new Root(FallingTree) {
   canSave = "1";
   canSaveDynamicFields = "1";


   new Sequence() {
      canSave = "1";
      canSaveDynamicFields = "1";      

      new Sequence() {
         canSave = "1";
         canSaveDynamicFields = "1";      
         
         new ScriptEval() {
            behaviorScript = "%obj.setDynamic(0);";// %obj.actionSeq(\"idle\");";
            defaultReturnStatus = "SUCCESS";
            canSave = "1";
            canSaveDynamicFields = "1";
         };
         new RandomWait() {
            waitMinMs = "0";
            waitMaxMs = "10";
            canSave = "1";
            canSaveDynamicFields = "1";
         };
         new ScriptEval() {
            behaviorScript = "%obj.setDynamic(1);";// %obj.actionSeq(\"idle\");";
            defaultReturnStatus = "SUCCESS";
            canSave = "1";
            canSaveDynamicFields = "1";
         };
         new ScriptEval() {
            behaviorScript = "%obj.applyImpulseToPart(0,\"0 0 0\",\"50000 0 0\");";
            defaultReturnStatus = "SUCCESS";
            canSave = "1";
            canSaveDynamicFields = "1";
         };
         new RandomWait() {
            waitMinMs = "200000";
            waitMaxMs = "250000";
            canSave = "1";
            canSaveDynamicFields = "1";
         };
      };      
   };
};
//--- OBJECT WRITE END ---
