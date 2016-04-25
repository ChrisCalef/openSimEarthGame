//-----------------------------------------------------------------------------
// Copyright (c) 2014 Andrew MacIntyre
//               Aldyre Studios - aldyre.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

/*
enum physicsShapeType
{
	PHYS_SHAPE_BOX = 0,
	PHYS_SHAPE_CAPSULE = 1
	PHYS_SHAPE_SPHERE = 2
	PHYS_SHAPE_CONVEX = 3
	PHYS_SHAPE_COLLISION = 4
	PHYS_SHAPE_TRIMESH = 5
};
*/

datablock PhysicsShapeData( M4Physics )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/Daz3D/Michael4/M4.dts";
   emap = 1;
   //simType = 2;
   
   isArticulated = true;        //Tells us to look for an array of bodyparts instead of one body. 
   shapeID = 1;        //ID into the physicsShape table in the database.

   mass = "1";                // This is mass for every bodypart, which is wrong, need physx to calculate mass based on density.
   massCenter = "0 0 0";      // Center of mass for rigid body
   massBox = "1 1 1";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2;                // Drag coefficient
   bodyFriction = 0.8;
   bodyRestitution = 0.1;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance
   
   minRollSpeed = 10;
   
   maxDrag = 0.5;
   minDrag = 0.01;

   triggerDustHeight = 1;
   dustHeight = 10;

   dragForce = 0.05;
   vertFactor = 0.05;

   normalForce = 0.05;
   restorativeForce = 0.05;
   rollForce = 0.05;
   pitchForce = 0.05;
   
   friction = "0.4";
   linearDamping = "0.1";
   angularDamping = "0.2";
   buoyancyDensity = "0.9";
   staticFriction = "0.5";
   
   radiusDamage        = 0;
   damageRadius        = 0;
   areaImpulse         = 0;
   restitution = "0.3";
   invulnerable = "0";
   waterDampingScale = "10";
   
   ///////////////////////////////////////
   //From BadBot...
   
   // max visible distance
   VisionRange = 200;
   
   // vision field of view
   VisionFov = 120;
   
   // max range to look for items
   findItemRange = 80;
   
   // min range to look for items, ie if we're this close we found it.
   foundItemDistance = 1.5;
   
   // the type of object to search for when looking for targets
   targetObjectTypes = $TypeMasks::PlayerObjectType;
   
   // the type of object to search for when looking for items
   itemObjectTypes = $TypeMasks::itemObjectType;
};


datablock PhysicsShapeData( a6m2Physics )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/FlightGear/A6M2/a6m2.dts";
   emap = 1;
   //simType = 2;
   
   isArticulated = false;//true;  //Tells us to look for an array of bodyparts instead of one body. 
   shapeID = 9;               //FIX: look this up by filename.

   mass = "1";                // This is mass for every bodypart, which is wrong, need physx to calculate mass based on density.
   massCenter = "0 0 0";      // Center of mass for rigid body
   massBox = "1 1 1";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2;                // Drag coefficient
   bodyFriction = 0.8;
   bodyRestitution = 0.1;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance
   
   minRollSpeed = 10;
   
   maxDrag = 0.5;
   minDrag = 0.01;

   triggerDustHeight = 1;
   dustHeight = 10;

   dragForce = 0.05;
   vertFactor = 0.05;

   normalForce = 0.05;
   restorativeForce = 0.05;
   rollForce = 0.05;
   pitchForce = 0.05;
   
   friction = "0.4";
   linearDamping = "0.1";
   angularDamping = "0.2";
   buoyancyDensity = "0.9";
   staticFriction = "0.5";
   
   radiusDamage        = 0;
   damageRadius        = 0;
   areaImpulse         = 0;
   restitution = "0.3";
   invulnerable = "0";
   waterDampingScale = "10";
   
};

datablock PhysicsShapeData( bo105Physics )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/FlightGear/bo105/bo105.dts";
   emap = 1;
   //simType = 2;
   
   isArticulated = false;//true;  //Tells us to look for an array of bodyparts instead of one body. 
   shapeID = 3;        //ID into the physicsShape table in the database.

   mass = "1";                // This is mass for every bodypart, which is wrong, need physx to calculate mass based on density.
   massCenter = "0 0 0";      // Center of mass for rigid body
   massBox = "1 1 1";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2;                // Drag coefficient
   bodyFriction = 0.8;
   bodyRestitution = 0.1;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance
   
   minRollSpeed = 10;
   
   maxDrag = 0.5;
   minDrag = 0.01;

   triggerDustHeight = 1;
   dustHeight = 10;

   dragForce = 0.05;
   vertFactor = 0.05;

   normalForce = 0.05;
   restorativeForce = 0.05;
   rollForce = 0.05;
   pitchForce = 0.05;
   
   friction = "0.4";
   linearDamping = "0.1";
   angularDamping = "0.2";
   buoyancyDensity = "0.9";
   staticFriction = "0.5";
   
   radiusDamage        = 0;
   damageRadius        = 0;
   areaImpulse         = 0;
   restitution = "0.3";
   invulnerable = "0";
   waterDampingScale = "10";
   
};

datablock PhysicsShapeData( ka50Physics )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/FlightGear/ka50/Models/ka50.dts";
   emap = 1;
   //simType = 2;
   
   isArticulated = false;//true;  //Tells us to look for an array of bodyparts instead of one body. 
   shapeID = 4;        //ID into the physicsShape table in the database.

   mass = "1";                // This is mass for every bodypart, which is wrong, need physx to calculate mass based on density.
   massCenter = "0 0 0";      // Center of mass for rigid body
   massBox = "1 1 1";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2;                // Drag coefficient
   bodyFriction = 0.8;
   bodyRestitution = 0.1;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance
   
   minRollSpeed = 10;
   
   maxDrag = 0.5;
   minDrag = 0.01;

   triggerDustHeight = 1;
   dustHeight = 10;

   dragForce = 0.05;
   vertFactor = 0.05;

   normalForce = 0.05;
   restorativeForce = 0.05;
   rollForce = 0.05;
   pitchForce = 0.05;
   
   friction = "0.4";
   linearDamping = "0.1";
   angularDamping = "0.2";
   buoyancyDensity = "0.9";
   staticFriction = "0.5";
   
   radiusDamage        = 0;
   damageRadius        = 0;
   areaImpulse         = 0;
   restitution = "0.3";
   invulnerable = "0";
   waterDampingScale = "10";
   
};


datablock PhysicsShapeData( ka50mainRotorPhysics )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/FlightGear/ka50/Models/MainRotor/MainRotor.dts";
   isArticulated = false;
   shapeID = 5;    
};

datablock PhysicsShapeData( ka50bladePhysics )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/FlightGear/ka50/Models/MainRotor/blade.dts";
   isArticulated = false;
   shapeID = 6;    
};

datablock PhysicsShapeData( ka50rocketPhysics )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/FlightGear/ka50/Models/weapons/rocket.dts";
   isArticulated = false;
   shapeID = 7;    
};

datablock PhysicsShapeData( ka50tubeRocketPhysics )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/FlightGear/ka50/Models/weapons/tuberocket.dts";
   isArticulated = false;
   shapeID = 8;    
};

////////////////////////////////////////////////////////////////

datablock PhysicsShapeData( dragonflyPhysics )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/FlightGear/Dragonfly/dragonfly.dts";
   emap = 1;
   //simType = 2;
   
   isArticulated = false;//true;  //Tells us to look for an array of bodyparts instead of one body. 
   shapeID = 2;        //ID into the physicsShape table in the database.

   mass = "1";                // This is mass for every bodypart, which is wrong, need physx to calculate mass based on density.
   massCenter = "0 0 0";      // Center of mass for rigid body
   massBox = "1 1 1";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2;                // Drag coefficient
   bodyFriction = 0.8;
   bodyRestitution = 0.1;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance
   
   minRollSpeed = 10;
   
   maxDrag = 0.5;
   minDrag = 0.01;

   triggerDustHeight = 1;
   dustHeight = 10;

   dragForce = 0.05;
   vertFactor = 0.05;

   normalForce = 0.05;
   restorativeForce = 0.05;
   rollForce = 0.05;
   pitchForce = 0.05;
   
   friction = "0.4";
   linearDamping = "0.1";
   angularDamping = "0.2";
   buoyancyDensity = "0.9";
   staticFriction = "0.5";
   
   radiusDamage        = 0;
   damageRadius        = 0;
   areaImpulse         = 0;
   restitution = "0.3";
   invulnerable = "0";
   waterDampingScale = "10";
   
};


datablock PhysicsShapeData( PSCube_ClientServer )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/physx3/cube.dae";
   emap = 1;
   simType = 2;

   mass = "0.5";
   massCenter = "0 0 0";      // Center of mass for rigid body
   massBox = "0 0 0";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2;                // Drag coefficient
   bodyFriction = 0.2;
   bodyRestitution = 0.1;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance
   
   minRollSpeed = 10;
   
   maxDrag = 0.5;
   minDrag = 0.01;

   triggerDustHeight = 1;
   dustHeight = 10;

   dragForce = 0.05;
   vertFactor = 0.05;

   normalForce = 0.05;
   restorativeForce = 0.05;
   rollForce = 0.05;
   pitchForce = 0.05;
   
   friction = "0.4";
   linearDamping = "0.1";
   angularDamping = "0.2";
   buoyancyDensity = "0.9";
   staticFriction = "0.5";
   
   radiusDamage        = 0;
   damageRadius        = 0;
   areaImpulse         = 0;
   restitution = "0.3";
   invulnerable = "0";
   waterDampingScale = "10";
};

datablock PhysicsShapeData( PSCube_Server )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/physx3/cube.dae";
   emap = 1;
   simType = 1;

   mass = "0.5";
   massCenter = "0 0 0";      // Center of mass for rigid body
   massBox = "0 0 0";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2;                // Drag coefficient
   bodyFriction = 0.2;
   bodyRestitution = 0.1;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance
   
   minRollSpeed = 10;
   
   maxDrag = 0.5;
   minDrag = 0.01;

   triggerDustHeight = 1;
   dustHeight = 10;

   dragForce = 0.05;
   vertFactor = 0.05;

   normalForce = 0.05;
   restorativeForce = 0.05;
   rollForce = 0.05;
   pitchForce = 0.05;
   
   friction = "0.4";
   linearDamping = "0.1";
   angularDamping = "0.2";
   buoyancyDensity = "0.9";
   staticFriction = "0.5";
   
   radiusDamage        = 0;
   damageRadius        = 0;
   areaImpulse         = 0;
   restitution = "0.3";
   invulnerable = "0";
   waterDampingScale = "10";
};


datablock PhysicsShapeData( PSCube_Client )
{	
   category = "PhysicsShape";
   shapeName = "art/shapes/physx3/cube.dae";
   emap = 1;
   simType = 0;

   mass = "0.5";
   massCenter = "0 0 0";      // Center of mass for rigid body
   massBox = "0 0 0";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2;                // Drag coefficient
   bodyFriction = 0.2;
   bodyRestitution = 0.1;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance
   
   minRollSpeed = 10;
   
   maxDrag = 0.5;
   minDrag = 0.01;

   triggerDustHeight = 1;
   dustHeight = 10;

   dragForce = 0.05;
   vertFactor = 0.05;

   normalForce = 0.05;
   restorativeForce = 0.05;
   rollForce = 0.05;
   pitchForce = 0.05;
   
   friction = "0.4";
   linearDamping = "0.1";
   angularDamping = "0.2";
   buoyancyDensity = "0.9";
   staticFriction = "0.5";
   
   radiusDamage        = 0;
   damageRadius        = 0;
   areaImpulse         = 0;
   restitution = "0.3";
   invulnerable = "0";
   waterDampingScale = "10";
};



// Cube that can activate triggers
datablock RigidPhysicsShapeData (PSCubeActivateTriggers)
{
   category = "PhysicsShape";
   shapeName = "art/shapes/physx3/cube.dae";
   emap = 1;

   mass = "0.5";
   massCenter = "0 0 0";      // Center of mass for rigid body
   massBox = "0 0 0";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2;                // Drag coefficient
   bodyFriction = 0.2;
   bodyRestitution = 0.1;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance
   
   minRollSpeed = 10;
   
   maxDrag = 0.5;
   minDrag = 0.01;

   triggerDustHeight = 1;
   dustHeight = 10;

   dragForce = 0.05;
   vertFactor = 0.05;

   normalForce = 0.05;
   restorativeForce = 0.05;
   rollForce = 0.05;
   pitchForce = 0.05;
   
   friction = "0.4";
   linearDamping = "0.1";
   angularDamping = "0.2";
   buoyancyDensity = "0.9";
   staticFriction = "0.5";
   
   radiusDamage        = 0;
   damageRadius        = 0;
   areaImpulse         = 0;
   restitution = "0.3";
   invulnerable = "0";
   waterDampingScale = "10";
};
