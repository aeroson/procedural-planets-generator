﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace MyEngine
{
	public struct Triangle
	{
		public Vector3 a;
		public Vector3 b;
		public Vector3 c;



		public Vector3 CenterPos
		{
			get
			{
				return (a + b + c) / 3.0f;
			}
		}

		public Vector3 Normal
		{
			get
			{
				return Vector3.Normalize(Vector3.Cross(
					b - a,
					c - a
				));
			}
		}

		public Triangle(Vector3 a, Vector3 b, Vector3 c)
		{
			this.a = a;
			this.b = b;
			this.c = c;
		}

		public Sphere ToBoundingSphere()
		{
			var c = CenterPos;
			var radius = (float)Math.Sqrt(
				Math.Max(
					Math.Max(
						a.DistanceSqr(b),
						a.DistanceSqr(c)
					),
					b.DistanceSqr(c)
				)
			);
			return new Sphere(c, radius);
		}

		// http://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates
		public Vector3 CalculateBarycentric(Vector3 p)
		{
			var v0 = b - a;
			var v1 = c - a;
			var v2 = p - a;
			var d00 = v0.Dot(v0);
			var d01 = v0.Dot(v1);
			var d11 = v1.Dot(v1);
			var d20 = v2.Dot(v0);
			var d21 = v2.Dot(v1);
			var denom = d00 * d11 - d01 * d01;
			var result = new Vector3();
			result.Y = (d11 * d20 - d01 * d21) / denom;
			result.Z = (d00 * d21 - d01 * d20) / denom;
			result.X = 1.0f - result.Y - result.Z;
			return result;
		}
	}
}
