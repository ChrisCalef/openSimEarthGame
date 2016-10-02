singleton TSShapeConstructor(SwatFbx)
{
   baseShape = "./swat.fbx";
};

function SwatFbx::onLoad(%this)
{
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/TPoseToRoot.dsq", "tpose", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/root.dsq", "root", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/walk.dsq", "walk", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/run.dsq", "run", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/rsidegetup.dsq", "rsidegetup", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/frontgetup.dsq", "frontgetup", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/lsidegetup.dsq", "lsidegetup", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/backgetup.dsq", "backgetup", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/ShootM16.dsq", "ShootM16", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/checkwatch.dsq", "checkwatch", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/power_punch_down.dsq", "power_punch_down", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/throwAttackFail.swat.dsq", "throwAttackFail.swat", "0", "-1");
   %this.setSequenceCyclic("throwAttackFail.swat", "1");
   
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/animation/work/forestry/start_digging.dsq","start_digging", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/animation/work/forestry/dig.dsq","dig", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/animation/work/forestry/stop_digging.dsq","stop_digging", "0", "-1");
   
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/animation/work/forestry/start_chopping_wood.dsq","start_chopping_wood", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/animation/work/forestry/chop_wood.dsq","chop_wood", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/animation/work/forestry/stop_chopping_wood.dsq","stop_chopping_wood", "0", "-1");
   
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/animation/work/food_service/start_mixing_batter.dsq","start_mixing_batter", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/animation/work/food_service/mix_batter.dsq","mix_batter", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/animation/work/food_service/stop_mixing_batter.dsq","stop_mixing_batter", "0", "-1");
   
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/animation/work/food_service/start_kneading_dough.dsq","start_kneading_dough", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/animation/work/food_service/knead_dough.dsq","knead_dough", "0", "-1");
   %this.addSequence("art/shapes/FBX/SoldierCharacterPack/animation/work/food_service/stop_kneading_dough.dsq","stop_kneading_dough", "0", "-1");
   //%this.addSequence("art/shapes/FBX/SoldierCharacterPack/.dsq", "", "0", "-1");
}
 