using System.Collections.Generic;

namespace Ryne.Scene
{
    // Material information that can be serialized from C#
    // Layout of parameters is explicit: must match Material layout in Engine for NodeEditor to work.
    // TODO: most values should be clamped between [0, 1]
    public struct Material
    {
        // Metallic: whether the object is made out of metal (fully specular with complex Fresnel)
        // For realistic materials, this should either be 0 or 1
        public float Metallic;

        // Roughness: microfacet roughness of the surface
        public float Roughness;

        // Clearcoat: clear layer on top of material
        public float Clearcoat;

        // Anisotropic: degree of anisotropic reflection
        public float Anisotropic;

        // AnisotropicRotation: direction of rotation for anisotropy with 1.0 = 360 degrees?
        public float AnisotropicRotation;

        // IOR: Index Of Refraction
        public float IOR;

        // Transmission: opacity of the material. Glass like at 1, opaque at 0
        public float Transmission;

        // Subsurface: 
        public float Subsurface;

        public float ScatterDistance;

        public float AbsorptionDistance;

        // Default color for the material
        public Float3 Albedo;

        public Float3 AbsorptionColor;

        public string Name;

        // List of used textures
        public List<RyneTextureInstance> Textures;

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
                Textures = material.Textures,
                Name = material.Name
            };

            //result.Textures = new List<RyneTextureInstance>();
            //foreach (var texture in material.Textures)
            //{
            //    result.Textures.Add(texture);
            //}

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

            if (Textures != null)
            {
                foreach (var texture in Textures)
                {
                    result.AddTexture(texture);
                }
            }

            return result;
        }

        public int GetTextureListId(RyneTextureInstance texture)
        {
            if (Textures is null)
            {
                return -1;
            }

            for (int i = 0; i < Textures.Count; i++)
            {
                if (Textures[i].Type == texture.Type && Textures[i].Filename == texture.Filename)
                    return i;
            }

            return -1;
        }
    }
}
