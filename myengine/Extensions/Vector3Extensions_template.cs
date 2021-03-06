﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MyEngine
{
	public static partial class Vector3Extensions
	{
		public static Vector3d CompomentWiseMult(this Vector3d a, Vector3d b)
		{
			return new Vector3d(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		}

		public static double Distance(this Vector3d a, Vector3d b)
		{
			return (a - b).Length;
		}

		public static double DistanceSqr(this Vector3d a, Vector3d b)
		{
			return (a - b).LengthSquared;
		}

		public static Vector3d Cross(this Vector3d a, Vector3d b)
		{
			return Vector3d.Cross(a, b);
		}

		public static double Angle(this Vector3d a, Vector3d b)
		{
			return Vector3d.CalculateAngle(a, b);
		}

		public static double Dot(this Vector3d a, Vector3d b)
		{
			double result;
			Vector3d.Dot(ref a, ref b, out result);
			return result;
		}

		public static Vector3d Multiply(this Vector3d a, double scale)
		{
			return Vector3d.Multiply(a, scale);
		}

		public static Vector3d Divide(this Vector3d a, double scale)
		{
			Vector3d.Divide(ref a, scale, out a);
			//return Vector3d.Divide(a, scale);
			return a;
		}

		/// <summary>
		/// returns -1.0 if x is less than 0.0, 0.0 if x is equal to 0.0, and +1.0 if x is greater than 0.0.
		/// https://www.opengl.org/sdk/docs/man/html/sign.xhtml
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>

		public static Vector3d Sign(this Vector3d a)
		{
			var ret = new Vector3d(0, 0, 0);

			if (a.X > 0) ret.X = 1;
			else if (a.X < 0) ret.X = -1;

			if (a.Y > 0) ret.Y = 1;
			else if (a.Y < 0) ret.Y = -1;

			if (a.Z > 0) ret.Z = 1;
			else if (a.Z < 0) ret.Z = -1;

			return ret;
		}

		public static Vector3d Abs(this Vector3d a)
		{
			if (a.X < 0) a.X *= -1;
			if (a.Y < 0) a.Y *= -1;
			if (a.Z < 0) a.Z *= -1;
			return a;
		}

		public static Vector3d Towards(this Vector3d from, Vector3d to)
		{
			return to - from;
		}

		public static Quaterniond LookRot(this Vector3d fwd, Vector3d up)
		{
			return Matrix4d.LookAt(Vector3d.Zero, fwd, up).ExtractRotation();
		}

		// http://stackoverflow.com/questions/12435671/quaternion-lookat-function
		// http://gamedev.stackexchange.com/questions/15070/orienting-a-model-to-face-a-target
		public static Quaterniond LookRot(this Vector3d dir)
		{
			//return LookRot(dir, Constants.Vector3dUp);

			var up = Constants.Vector3dUp;
			var fwd = Constants.Vector3dForward;
			dir.Normalize();
			double dot = Vector3d.Dot(fwd, dir);

			if (Math.Abs(dot - (-1.0f)) < 0.000001f)
			{
				return new Quaterniond(up.X, up.Y, up.Z, 3.1415926535897932f);
			}
			if (Math.Abs(dot - (1.0f)) < 0.000001f)
			{
				return Quaterniond.Identity;
			}

			double rotAngle = (double)Math.Acos(dot);
			Vector3d rotAxis = Vector3d.Cross(fwd, dir);
			rotAxis = Vector3d.Normalize(rotAxis);
			return Quaterniond.FromAxisAngle(rotAxis, rotAngle);
		}

		public static Vector3d LerpTo(this Vector3d a, Vector3d b, double t)
		{
			return Vector3d.Lerp(a, b, t);
		}
	}
}
