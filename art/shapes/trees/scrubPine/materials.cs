//--- scrubPine.DAE MATERIALS BEGIN ---
singleton Material(scrubPine_SoftPine2)
{
	mapTo = "SoftPine2";

	diffuseMap[0] = "LushPine.dds";
	normalMap[0] = "LushPine_NRM.dds";

	diffuseColor[0] = "0.996078 0.992157 0.992157 1";
	specular[0] = "0.9 0.9 0.9 1";
	specularPower[0] = "1";

	doubleSided = 1;
	translucent = 0;
	translucentBlendOp = "LerpAlpha";
   useAnisotropic[0] = "1";
   subSurface[0] = "1";
   alphaTest = "1";
   alphaRef = "50";
   pixelSpecular[0] = "0";
   specularMap[0] = "LushPine_SPEC.dds";
   showFootprints = "0";
   subSurfaceColor[0] = "0.537255 0.619608 0.121569 1";
   subSurfaceRolloff[0] = "1";
};

//--- scrubPine.DAE MATERIALS END ---


singleton Material(scrubPine_SoftPineBark)
{
   mapTo = "SoftPineBark";
   diffuseMap[0] = "Piney.dds";
   specular[0] = "0.9 0.9 0.9 1";
   specularPower[0] = "10";
   translucentBlendOp = "None";
   normalMap[0] = "Piney_NRM.dds";
   doubleSided = "1";
};

singleton Material(scrubPine_ColorEffectR225G88B199_material)
{
   mapTo = "ColorEffectR225G88B199-material";
   diffuseColor[0] = "0.882353 0.345098 0.780392 1";
   specularPower[0] = "10";
   translucentBlendOp = "None";
};

singleton Material(scrubPine_ColorEffectR135G6B6_material)
{
   mapTo = "ColorEffectR135G6B6-material";
   diffuseColor[0] = "0.529412 0.0235294 0.0235294 1";
   specularPower[0] = "10";
   translucentBlendOp = "None";
};

singleton Material(scrubPine_ColorEffectR6G135B113_material)
{
   mapTo = "ColorEffectR6G135B113-material";
   diffuseColor[0] = "0.0235294 0.529412 0.443137 1";
   specularPower[0] = "10";
   translucentBlendOp = "None";
};
