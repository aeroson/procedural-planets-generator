﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using MyEngine;
using MyEngine.Events;
using MyEngine.Components;

namespace MyGame
{
    public class SSAO : ComponentWithShortcuts
    {
        public SSAO(Entity entity) : base(entity)
        {

            //var shader = Factory.GetShader("postProcessEffects/SSAO.shader");

            //shader.Uniform.Set("testColor", new Vector3(0, 1, 0));

            //Camera.main.AddPostProcessEffect(shader);
            Entity.EventSystem.Register<GraphicsUpdate>(e => Update(e.DeltaTime));
        }

        void Update(double deltaTime)
        {
            
        }
    }
}