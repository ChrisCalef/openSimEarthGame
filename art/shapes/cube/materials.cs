//-----------------------------------------------------------------------------
// Copyright (c) 2012 GarageGames, LLC
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

//--- cube.dae MATERIALS BEGIN ---
singleton Material(cube_GridMaterial)
{
	mapTo = "unmapped_mat";

	diffuseMap[0] = "art/shapes/cube/grid";

	diffuseColor[0] = "1 1 1 1";
	specular[0] = "0.9 0.9 0.9 1";
	specularPower[0] = 0.415939;
	pixelSpecular[0] = false;
	emissive[0] = false;

	doubleSided = false;
	translucent = false;
	translucentBlendOp = "None";
};

//--- cube.dae MATERIALS END ---


singleton Material(_3td_Crate01_Crate02)
{
   mapTo = "unmapped_mat";
   diffuseMap[0] = "art/shapes/3TD/Industrial/3td_Crate_01/3td_Crate_02.png";
   normalMap[0] = "art/shapes/3TD/Industrial/3td_Crate_01/3td_Crate_02_NRM.png";
   specular[0] = "0.9 0.9 0.9 1";
   specularPower[0] = "10";
   useAnisotropic[0] = "1";
   doubleSided = "1";
   translucentBlendOp = "None";
};

singleton Material(Warf_01_WarfWood)
{
   mapTo = "unmapped_mat";
   diffuseMap[0] = "art/shapes/FreeHarborProps/Warf_01/3td_MossWood_04";
   normalMap[0] = "art/shapes/FreeHarborProps/Warf_01/3td_MossWood_04_NRM.png";
   specular[0] = "0.9 0.9 0.9 1";
   specularPower[0] = "50";
   pixelSpecular[0] = "1";
   useAnisotropic[0] = "1";
   translucentBlendOp = "None";
};

singleton Material(skyscraper_Concrete_Block_8x8_Gray)
{
   mapTo = "GridMaterial";
   diffuseMap[0] = "art/shapes/sketchup/skyscraper/Concrete_Block_8x8_Gray";
   translucentBlendOp = "None";
};
