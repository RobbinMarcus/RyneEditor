namespace Ryne.Scene
{
    // Material information that can be serialized from C#
    // Layout of parameters is explicit: must match Material layout in Engine for NodeEditor to work.
    public struct Material
    {
        // Metallic: whether the object is made out of metal (fully specular with complex Fresnel)
        // For realistic materials, this should either be 0 or 1
        [Range(0, 1)]
        public float Metallic;

        // Roughness: microfacet roughness of the surface
        [Range(0, 1)]
        public float Roughness;

        // Clearcoat: clear layer on top of material
        [Range(0, 1)]

        public float Clearcoat;

        // Anisotropic: degree of anisotropic reflection
        [Range(0, 1)]
        public float Anisotropic;

        // AnisotropicRotation: direction of rotation for anisotropy with 1.0 = 360 degrees?
        [Range(0, 1)]
        public float AnisotropicRotation;

        // IOR: Index Of Refraction
        [Range(1, 2)]
        public float IOR;

        // Transmission: opacity of the material. Glass like at 1, opaque at 0
        [Range(0, 1)]
        public float Transmission;

        // Subsurface: 
        [Range(0, 1)]
        public float Subsurface;

        public float ScatterDistance;

        public float AbsorptionDistance;

        // Default color for the material
        public Float3 Albedo;

        public Float3 AbsorptionColor;

        public string Name;

        // Create a material from given RyneMaterial
        public static Material Create(RyneMaterial material)
        {
            Material result = new Material
            {
                Albedo = material.Albedo,
                Metallic = material.Metallic,
                Roughness = material.Roughness,
                Clearcoat = material.Clearcoat,
                Anisotropic = material.Anisotropic,
                AnisotropicRotation = material.AnisotropicRotation,
                IOR = material.IOR,
                Transmission = material.Transmission,
                Subsurface = material.Subsurface,
                ScatterDistance = material.ScatterDistance,
                AbsorptionColor = material.AbsorptionColor,
                AbsorptionDistance = material.AbsorptionDistance,
                Name = material.Name
            };
            
            return result;
        }

        public RyneMaterial ToRenderMaterial()
        {
            RyneMaterial result = new RyneMaterial
            {
                Albedo = Albedo,
                Metallic = Metallic,
                Roughness = Roughness,
                Clearcoat = Clearcoat,
                Anisotropic = Anisotropic,
                AnisotropicRotation = AnisotropicRotation,
                IOR = IOR,
                Transmission = Transmission,
                Subsurface = Subsurface,
                ScatterDistance = ScatterDistance,
                AbsorptionColor = AbsorptionColor,
                AbsorptionDistance = AbsorptionDistance,
                Name = string.IsNullOrEmpty(Name) ? "" : Name
            };

            return result;
        }
    }
}
