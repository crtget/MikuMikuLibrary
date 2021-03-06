﻿using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Textures;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLMaterial : IDisposable
    {
        public GLTexture Diffuse { get; }
        public GLTexture Ambient { get; }
        public GLTexture Normal { get; }
        public GLTexture Specular { get; }
        public GLTexture Reflection { get; }
        public GLTexture SpecularPower { get; }
        public Vector4 DiffuseColor { get; }
        public Vector4 AmbientColor { get; }
        public Vector4 SpecularColor { get; }
        public Vector4 EmissionColor { get; }
        public float Shininess { get; }

        public void Bind( GLShaderProgram shaderProgram )
        {
            shaderProgram.SetUniform( "diffuseColor", DiffuseColor );
            shaderProgram.SetUniform( "ambientColor", AmbientColor );
            shaderProgram.SetUniform( "specularColor", SpecularColor );
            //shaderProgram.SetUniform( "emissionColor", EmissionColor );
            shaderProgram.SetUniform( "specularPower", Shininess );

            int textureIndex = 0;
            void ActivateTexture( GLTexture texture, string uniformName, TextureTarget target = TextureTarget.Texture2D )
            {
                GL.ActiveTexture( TextureUnit.Texture0 + textureIndex );
                GL.BindTexture( target, texture?.ID ?? 0 );
                shaderProgram.SetUniform( $"has{uniformName}", texture != null );
                shaderProgram.SetUniform( uniformName, textureIndex++ );
            }

            ActivateTexture( Diffuse, "diffuseTexture" );
            ActivateTexture( Ambient, "ambientTexture" );
            ActivateTexture( Normal, "normalTexture" );
            ActivateTexture( Specular, "specularTexture" );
            ActivateTexture( Reflection, "reflectionTexture", TextureTarget.TextureCubeMap );
            ActivateTexture( SpecularPower, "specularPowerTexture" );
        }

        public void Dispose()
        {
            Diffuse?.Dispose();
            Ambient?.Dispose();
            Normal?.Dispose();
            Specular?.Dispose();
            Reflection?.Dispose();
            SpecularPower?.Dispose();
        }

        public GLMaterial( Material material, Dictionary<int, GLTexture> textures, TextureSet textureSet )
        {
            if ( textureSet == null )
                return;

            Diffuse = GetTexture( material.Diffuse );
            Ambient = GetTexture( material.Ambient );
            Normal = GetTexture( material.Normal );
            Specular = GetTexture( material.Specular );
            Reflection = GetTexture( material.Reflection );
            SpecularPower = GetTexture( material.SpecularPower );

            if ( material.Shader.Equals( "EYEBALL", StringComparison.OrdinalIgnoreCase ) )
                DiffuseColor = Vector4.One;
            else
                DiffuseColor = new Vector4( material.DiffuseColor.R, material.DiffuseColor.G, material.DiffuseColor.B, material.DiffuseColor.A );
            AmbientColor = new Vector4( material.AmbientColor.R, material.AmbientColor.G, material.AmbientColor.B, material.AmbientColor.A );
            SpecularColor = new Vector4( material.SpecularColor.R, material.SpecularColor.G, material.SpecularColor.B, material.SpecularColor.A );
            EmissionColor = new Vector4( material.EmissionColor.R, material.EmissionColor.G, material.EmissionColor.B, material.EmissionColor.A );
            Shininess = material.Shininess;

            GLTexture GetTexture( MaterialTexture materialTexture )
            {
                if ( !materialTexture.IsActive )
                    return null;

                if ( textures.TryGetValue( materialTexture.TextureID, out GLTexture texture ) )
                    return texture;
                else
                {
                    var textureToUpload = textureSet.Textures.FirstOrDefault( x => x.ID.Equals( materialTexture.TextureID ) );
                    if ( textureToUpload == null )
                        return null;

                    texture = new GLTexture( textureToUpload );
                    textures.Add( textureToUpload.ID, texture );
                    return texture;
                }
            }
        }
    }
}
