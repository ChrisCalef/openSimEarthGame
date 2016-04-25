//--- OBJECT WRITE BEGIN ---
new Root(testTree) {
   canSave = "1";
   canSaveDynamicFields = "1";

   new ScriptedBehavior() {
      preconditionMode = "ONCE";
      internalName = "findGroundBehavior";
      class = "findGround";
      canSave = "1";
      canSaveDynamicFields = "1";
   };
};
//--- OBJECT WRITE END ---
