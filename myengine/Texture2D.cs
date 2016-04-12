﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace MyEngine
{
    public class Texture2D : Texture, IUnloadable
    {


        static public Texture2D whiteTexture { private set; get; }
        static public Texture2D greyTexture { private set; get; }
        static public Texture2D blackTexture { private set; get; }


        int textureHandle;


        internal Texture2D(int textureHandle)
        {
            this.textureHandle = textureHandle;
        }
        internal Texture2D(Asset resource)
        {
            this.Load(resource);
        }

        static Texture2D() {
            whiteTexture = new Texture2D(AssetSystem.Instance.FindAsset("internal/white.png"));
            greyTexture = new Texture2D(AssetSystem.Instance.FindAsset("internal/grey.png"));
            blackTexture = new Texture2D(AssetSystem.Instance.FindAsset("internal/black.png"));
        }


        public void Unload()
        {
            GL.DeleteTexture(textureHandle);
        }

        void Load(Asset asset)
        {

            UsingMipMaps = true;

            textureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            Bitmap bmp;
            using(var s = asset.GetDataStream())
                bmp = new Bitmap(s);

            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);
            bmp.UnlockBits(bmp_data);

            // We will not upload mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // We can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            if (UsingMipMaps) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);                       

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GetTextureMagFilter());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GetTextureMinFilter());

            // https://www.opengl.org/wiki/Sampler_Object
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GetTextureWrapMode());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GetTextureWrapMode());

            // ???
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, this.anisoLevel);

            var extensions = GL.GetString(StringName.Extensions);
            if (extensions.Contains("GL_EXT_texture_filter_anisotropic"))
            {
                int max_aniso = GL.GetInteger((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt);
                GL.TexParameter(
                   TextureTarget.Texture2D,
                   (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt,
                   max_aniso);
            }            

        }
        public bool IsOnGpu()
        {
            var yes = new bool[1];
            GL.AreTexturesResident(1, new int[] { textureHandle }, yes);
            return yes[0];
        }
        public override int GetNativeTextureID()
        {
            return textureHandle;
        }
        
    }
}
