﻿using Neitri;
using Neitri.Base;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MyEngine.Components
{
	[Flags]
	public enum RenderStatus
	{
		NotRendered = 0,
		Rendered = (1 << 1),
		RenderedForced = (1 << 1) | (1 << 2),
		Visible = (1 << 3),
		RenderedAndVisible = (1 << 1) | (1 << 3),
		Unknown = (1 << 9),
	}

	public class RenderContext
	{
		public static readonly RenderContext Geometry = new RenderContext("geometry");
		public static readonly RenderContext Shadows = new RenderContext("shadow");
		public static readonly RenderContext Depth = new RenderContext("depth");
		public string Name { get; set; }

		public RenderContext(string name)
		{
			this.Name = name;
		}

		public override string ToString()
		{
			return this.Name;
		}
	}

	public interface IRenderable
	{
		Material Material { get; }
		bool ForcePassCulling { get; }

		bool ShouldRenderInContext(Camera camera, RenderContext renderContext);

		Bounds GetFloatingOriginSpaceBounds(WorldPos viewPointPos);

		void UploadUBOandDraw(Camera camera, UniformBlock ubo);

		void SetCameraRenderStatusFeedback(Camera camera, RenderStatus renderStatus);

		IEnumerable<Vector3> GetCameraSpaceOccluderTriangles(Camera camera);

		CameraSpaceBounds GetCameraSpaceBounds(Camera camera);
	}

	public abstract class Renderer : ComponentWithShortcuts, IRenderable, IDisposable
	{
		public virtual MyRenderingMode RenderingMode { get; set; }
		public virtual Material Material { get; set; }

		Dictionary<Camera, RenderStatus> cameraToRenderStatus = new Dictionary<Camera, RenderStatus>();

		public virtual bool ForcePassCulling { get; set; }

		RenderableData DataToRender => Entity.Scene.DataToRender;

		public override void OnAddedToEntity(Entity entity)
		{
			base.OnAddedToEntity(entity);

			DataToRender.Add(this);
		}

		public void SetRenderingMode(MyRenderingMode renderingMode) => RenderingMode = renderingMode;
		public abstract Bounds GetFloatingOriginSpaceBounds(WorldPos viewPointPos);

		public virtual void UploadUBOandDraw(Camera camera, UniformBlock ubo)
		{
		}

		public virtual void SetCameraRenderStatusFeedback(Camera camera, RenderStatus renderStatus)
		{
			cameraToRenderStatus[camera] = renderStatus;
		}

		public virtual RenderStatus GetCameraRenderStatusFeedback(Camera camera)
		{
			return cameraToRenderStatus.GetValue(camera, RenderStatus.Unknown);
		}

		public virtual bool ShouldRenderInContext(Camera camera, RenderContext renderContext)
		{
			if (renderContext == RenderContext.Geometry && RenderingMode.HasFlag(MyRenderingMode.RenderGeometry)) return true;
			if (renderContext == RenderContext.Shadows && RenderingMode.HasFlag(MyRenderingMode.CastShadows)) return true;
			return false;
		}

		public void Dispose()
		{
			DataToRender.Remove(this);
		}

		public override string ToString()
		{
			return Entity.Name;
		}

		public abstract IEnumerable<Vector3> GetCameraSpaceOccluderTriangles(Camera camera);
		public abstract CameraSpaceBounds GetCameraSpaceBounds(Camera camera);
	}
}