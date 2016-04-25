singleton TSShapeConstructor(M4Dts)
{
   baseShape = "./M4.dts";
};

function M4Dts::onLoad(%this)
{
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/TPose.dsq", "ambient", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/Root4.dsq", "idle", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/CMU_16_22.dsq", "walk", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/MedRun6.dsq", "run", "0", "-1");

   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/runscerd1.dsq", "runscerd", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/backGetup.dsq", "backGetup", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/frontGetup.dsq", "frontGetup", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/rSideGetup02.dsq", "rSideGetup", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/lSideGetup02.dsq", "lSideGetup", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/zombiePunt2.dsq", "zombiePunt", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/Mime.dsq", "mime", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/Sneak.dsq", "sneak", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/power_punch_down.dsq", "power_punch_down", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/punch_uppercut.dsq", "punch_uppercut", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/RoundHouse.dsq", "roundhouse", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/Strut.dsq", "strut", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/soldier_march.dsq", "march", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/sequences/01_13_swing_under_grcap.dsq", "swingUnder", "0", "-1");
   %this.addSequence("art/shapes/Daz3D/Michael4/scene_2/shape_1.dsq", "seq_1", "0", "-1");

   %this.setSequenceCyclic("ambient", "1");
   %this.setSequenceCyclic("walk", "1");
   %this.setSequenceCyclic("strut", "1");   
   %this.setSequenceCyclic("march", "1");
   %this.setSequenceCyclic("run", "1");
   %this.setSequenceCyclic("runscerd", "1");
   %this.setSequenceCyclic("tpose", "1");
   %this.setSequenceCyclic("swingUnder", "1");
      
   %this.addNode("Col-1","root");
   %this.addCollisionDetail(-1,"box","bounds");   
} 
