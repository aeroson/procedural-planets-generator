﻿using MyEngine;
using MyEngine.Components;
using Neitri;
using OpenTK;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyGame.PlanetaryBody
{
	public partial class Segment : SingletonsPropertyAccesor
	{
		/// <summary>
		/// Planet local position range
		/// </summary>
		public TriangleD NoElevationRange { get; private set; }

		public TriangleD NoElevationRangeModifiedForSkirts
		{
			get
			{
				var range = NoElevationRange;

				var z = range.CenterPos;

				double e = (double)planetInfo.ChunkNumberOfVerticesOnEdge;
				double ratio = 1 / (e - 3);
				double twoRatios = ratio * 2;
				double rangeMultiplier = 1 + Math.Sqrt(twoRatios * twoRatios - ratio * ratio) * 1.85;

				range.a = (range.a - z) * rangeMultiplier + z;
				range.b = (range.b - z) * rangeMultiplier + z;
				range.c = (range.c - z) * rangeMultiplier + z;

				return range;
			}
		}

		/// <summary>
		/// Planet local position range
		/// </summary>
		TriangleD realVisibleRange;
		/// <summary>
		/// Planet local position range
		/// </summary>
		TriangleD rangeToCalculateScreenSizeOn;

		List<Vector3> occluderTringles = new List<Vector3>();


		public Segment parent;
		public List<Segment> Children { get; } = new List<Segment>();
		public CustomChunkMeshRenderer RendererSurface { get; private set; }
		public MeshRenderer RendererSea { get; private set; }

		public class CustomChunkMeshRenderer : MeshRenderer
		{
			public Segment segment;

			/*
			public override bool ShouldRenderInContext(Camera camera, RenderContext renderContext)
			{
				if (base.ShouldRenderInContext(camera, renderContext))
				{
					// 1 looking at it from top, 0 looking from side, -1 looking from bottom
					var dotToCamera = chunk.rangeToCalculateScreenSizeOn.Normal.Dot(
						-camera.ViewPointPosition.Towards(chunk.rangeToCalculateScreenSizeOn.CenterPos + chunk.planetaryBody.Transform.Position).ToVector3d().Normalized()
					);
					if (dotToCamera > -0.2f) return true;
					return false;
				}
				return false;
			}
			*/

			public override IEnumerable<Vector3> GetCameraSpaceOccluderTriangles(CameraData camera)
			{
				//if (chunk.occluderTringles.Count < 9 && chunk.IsGenerationDone) throw new Exception("this should not happen");
				if (segment?.IsGenerationDone == true && segment?.occluderTringles.Count == 9)
				{
					var mvp = GetModelViewProjectionMatrix(camera);
					return segment.occluderTringles.Select(v3 => v3.Multiply(ref mvp));
				}
				else
				{
					return null;
				}
			}

			public override MyRenderingMode RenderingMode
			{
				get
				{
					return base.RenderingMode;
				}
				set
				{
					if (value.HasFlag(MyRenderingMode.RenderGeometry) && segment?.IsGenerationDone == false)
					{
						Log.Warn("trying to render segment " + segment + " that did not finish generation");
					}

					if (segment != null && segment.parent != null && segment.parent.Children.Count == 0)
					{
						Log.Error("parent segment had to children, should not happen, " + segment);
					}

					base.RenderingMode = value;
				}
			}

		}




		int subdivisionDepth;
		Planet planetInfo;


		ChildPosition childPosition;

		public enum ChildPosition
		{
			Top = 0,
			Left = 1,
			Middle = 2,
			Right = 3,
			NoneNoParent = -1,
		}

		public readonly ulong ID;
		public Segment(Planet planetInfo, TriangleD noElevationRange, Segment parentChunk, ulong id, ChildPosition childPosition = ChildPosition.NoneNoParent)
		{
			ID = id;
			this.planetInfo = planetInfo;
			this.parent = parentChunk;
			this.childPosition = childPosition;
			this.NoElevationRange = noElevationRange;
			this.rangeToCalculateScreenSizeOn = noElevationRange;
		}


		TriangleD[] meshTriangles;

		TriangleD[] GetMeshTriangles()
		{
			if (meshTriangles == null)
				meshTriangles = RendererSurface?.Mesh?.GetMeshTrianglesD();
			return meshTriangles;
		}

		Vector3d CenterPosVec3 => NoElevationRange.CenterPos;

		public double GetHeight(Vector3d chunkLocalPosition)
		{
			//var barycentricOnChunk = noElevationRange.CalculateBarycentric(planetLocalPosition);
			//var u = barycentricOnChunk.X;
			//var v = barycentricOnChunk.Y;

			var triangles = GetMeshTriangles();
			if (triangles != null)
			{
				var ray = new RayD(-CenterPosVec3.Normalized(), chunkLocalPosition);
				foreach (var t in triangles)
				{
					var hit = ray.CastRay(t);
					if (hit.DidHit)
					{
						return (ray.GetPoint(hit.HitDistance) + CenterPosVec3).Length;
					}
				}
			}

			return -1;
		}



		public double GetSizeOnScreen(Camera cam)
		{
			var myPos = rangeToCalculateScreenSizeOn.CenterPos + planetInfo.Transform.Position;
			var distanceToCamera = myPos.Distance(cam.ViewPointPosition);

			// this is world space, doesnt take into consideration rotation, not good
			var sphere = rangeToCalculateScreenSizeOn.ToBoundingSphere();
			var radiusWorldSpace = sphere.radius;
			var fov = cam.FieldOfView;
			var radiusCameraSpace = radiusWorldSpace * MyMath.Cot(fov / 2) / distanceToCamera;

			return radiusCameraSpace;
		}

		public double GetGenerationWeight(Camera cam)
		{
			bool isVisible = true;

			var myPos = rangeToCalculateScreenSizeOn.CenterPos + planetInfo.Transform.Position;
			var dirToCamera = myPos.Towards(cam.ViewPointPosition).ToVector3d();
			dirToCamera.NormalizeFast();

			// 0 looking at it from side, 1 looking at it from top, -1 looking at it from behind
			var dotToCamera = rangeToCalculateScreenSizeOn.NormalFast.Dot(dirToCamera);

			if (RendererSurface != null && RendererSurface.Mesh != null)
			{
				//var localCamPos = planetaryBody.Transform.Position.Towards(cam.ViewPointPosition).ToVector3();
				//distanceToCamera = renderer.Mesh.Vertices.FindClosest((v) => v.DistanceSqr(localCamPos)).Distance(localCamPos);
				//isVisible = cam.GetFrustum().VsBounds(renderer.GetCameraSpaceBounds(cam.ViewPointPosition));
				isVisible = RendererSurface.GetCameraRenderStatusFeedback(cam).HasFlag(RenderStatus.Rendered);
			}

			var weight = GetSizeOnScreen(cam);

			//weight *= (1 + MyMath.Clamp01(dotToCamera));

			if (isVisible == false) weight *= 0.3f;
			return weight;
		}

		public void CalculateRealVisibleRange()
		{
			if (occluderTringles.Count != 0) return;

			var a = RendererSurface.Mesh.Vertices[planetInfo.AIndexReal];
			var b = RendererSurface.Mesh.Vertices[planetInfo.BIndexReal];
			var c = RendererSurface.Mesh.Vertices[planetInfo.CIndexReal];

			var o = RendererSurface.Offset.ToVector3d();
			realVisibleRange.a = a.ToVector3d() + o;
			realVisibleRange.b = b.ToVector3d() + o;
			realVisibleRange.c = c.ToVector3d() + o;

			rangeToCalculateScreenSizeOn = realVisibleRange;

			var z = a.Distance(b) / 10.0f * -realVisibleRange.Normal.ToVector3();

			var sinkDir = a.Distance(b) / 20.0f * -realVisibleRange.Normal.ToVector3();

			a += sinkDir;
			b += sinkDir;
			c += sinkDir;

			occluderTringles.Add(a);
			occluderTringles.Add(z);
			occluderTringles.Add(b);

			occluderTringles.Add(b);
			occluderTringles.Add(z);
			occluderTringles.Add(c);

			occluderTringles.Add(c);
			occluderTringles.Add(z);
			occluderTringles.Add(a);
		}

		void AddChild(Vector3d a, Vector3d b, Vector3d c, ChildPosition cp, ulong id)
		{
			var range = new TriangleD()
			{
				a = a,
				b = b,
				c = c
			};
			var child = new Segment(planetInfo, range, this, this.ID << 2 | id);
			Children.Add(child);
			child.subdivisionDepth = subdivisionDepth + 1;
			child.rangeToCalculateScreenSizeOn = range;
		}

		public void EnsureChildrenAreCreated()
		{
			if (Children.Count <= 0)
			{
				var a = NoElevationRange.a;
				var b = NoElevationRange.b;
				var c = NoElevationRange.c;
				var ab = (a + b).Divide(2.0f).Normalized();
				var ac = (a + c).Divide(2.0f).Normalized();
				var bc = (b + c).Divide(2.0f).Normalized();

				ab *= planetInfo.RadiusMin;
				ac *= planetInfo.RadiusMin;
				bc *= planetInfo.RadiusMin;

				AddChild(a, ab, ac, ChildPosition.Top, 0);
				AddChild(ab, b, bc, ChildPosition.Left, 1);
				AddChild(ac, bc, c, ChildPosition.Right, 2);
				AddChild(ab, bc, ac, ChildPosition.Middle, 3);
			}
		}


		bool lastVisible = false;
		public void SetVisible(bool visible) // TODO: DestroyRenderer if visible == false for over CVar 60 seconds ?
		{
			if (this.GenerationBegan && this.IsGenerationDone)
			{
				if (visible == lastVisible) return;
				lastVisible = visible;
			}

			if (visible)
			{
				if (this.GenerationBegan == false)
				{
					//Log.Warn("trying to show segment " + this + " that did not begin generation");
				}
				else if (this.IsGenerationDone == false)
				{
					//Log.Warn("trying to show segment " + this + " that did not finish generation");
				}
				else DoRender(true);
			}
			else
			{
				DoRender(false);
			}
		}

		public void HideAllChildren()
		{
			foreach (var child in Children)
			{
				child.SetVisible(false);
				child.HideAllChildren();
			}
		}

		void DoRender(bool yes)
		{
			if (yes)
			{
				if (RendererSurface != null)
					RendererSurface.RenderingMode = MyRenderingMode.RenderGeometryAndCastShadows;
				else
					Log.Warn("trying to show segment " + this + " that does not have surface renderer");
				if (RendererSea != null)
					RendererSea.RenderingMode = MyRenderingMode.RenderGeometryAndCastShadows;
				else
					Log.Warn("trying to show segment " + this + " that does not have sea renderer");
			}
			else
			{
				if (RendererSurface != null)
					RendererSurface.RenderingMode = MyRenderingMode.DontRender;
				if (RendererSea != null)
					RendererSea.RenderingMode = MyRenderingMode.DontRender;
			}
		}


		public void NotifyGenerationDone()
		{
			lock (this)
			{
				IsGenerationDone = true;
			}
		}



		public void DestroyAll()
		{
			DestroyRenderer();
			foreach (var c in Children)
				c.DestroyAll();
			Children.Clear();
		}


		public void MarkForRegeneration()
		{
			DestroyRenderer();
			foreach (var c in Children)
				c.MarkForRegeneration();
			Children.Clear();
		}

		public override string ToString()
		{
			return typeof(Segment) + " planet:#" + planetInfo.ID + " id:#" + ID + " generation:" + subdivisionDepth;
		}


	}
}