/******************************************************************************\
* Copyright (C) 2012-2016 Leap Motion, Inc. All rights reserved.               *
* Leap Motion proprietary and confidential. Not for distribution.              *
* Use subject to the terms of the Leap Motion SDK Agreement available at       *
* https://developer.leapmotion.com/sdk_agreement, or another agreement         *
* between Leap Motion and you, your company or other organization.             *
\******************************************************************************/
namespace LeapMotionWebsocket
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	/**
   * The Bone class represents a tracked bone.
   *
   * All fingers contain 4 bones that make up the anatomy of the finger.
   * Get valid Bone objects from a Finger object.
   *
   * Bones are ordered from base to tip, indexed from 0 to 3.  Additionally, the
   * bone's Type enum may be used to index a specific bone anatomically.
   *
   * \include Bone_iteration.txt
   *
   * The thumb does not have a base metacarpal bone and therefore contains a valid,
   * zero length bone at that location.
   * @since 2.0
   */
	public class Bone
	{


		/**
     * Constructs a new Bone object.
     *
     * @param prevJoint The proximal end of the bone (closest to the body)
     * @param nextJoint The distal end of the bone (furthest from the body)
     * @param center The midpoint of the bone
     * @param direction The unit direction vector pointing from prevJoint to nextJoint.
     * @param length The estimated length of the bone.
     * @param width The estimated average width of the bone.
     * @param type The type of finger bone.
     * @param basis The matrix representing the orientation of the bone.
     * @since 3.0
     */
		public Bone (Vector prevJoint,
		             Vector nextJoint,
		             float width,
		             Bone.BoneType type,
		             float[][] basis
		)
		{
			PrevJoint = prevJoint;
			NextJoint = nextJoint;
			Width = width;
			Type = type;

			Length = prevJoint.DistanceTo (nextJoint);
			Direction = new Vector (
				basis [2] [0] * -1,
				basis [2] [1] * -1,
				basis [2] [2] * -1);
			Center = Vector.Lerp (prevJoint, nextJoint, 0.5f);

			Left = determinant (basis) < 0;
			float factor = Left ? -1 : 1;
			Basis = new Matrix (
				basis[0][0]*factor,basis[0][1]*factor,basis[0][2]*factor,
				basis[1][0],basis[1][1],basis[1][2],
				basis[2][0],basis[2][1],basis[2][2],
				Center.x,Center.y,Center.z
			);
		}


		float determinant(float[][] a) {
			return a[0][0] * (a[2][2] * a[1][1] - a[1][2] * a[2][1]) 
				 + a[0][1] * (-a[2][2] * a[1][0] + a[1][2] * a[2][0]) 
				 + a[0][2] * (a[2][1] * a[1][0] - a[1][1] * a[2][0]);
		}

		/**
     * The base of the bone, closest to the wrist.
     *
     * In anatomical terms, this is the proximal end of the bone.
     * \include Bone_prevJoint.txt
     *
     * @returns The Vector containing the coordinates of the previous joint position.
     * @since 2.0
     */
		public Vector PrevJoint { get; private set; }

		/**
     * The end of the bone, closest to the finger tip.
     *
     * In anatomical terms, this is the distal end of the bone.
     *
     * \include Bone_nextJoint.txt
     *
     * @returns The Vector containing the coordinates of the next joint position.
     * @since 2.0
     */
		public Vector NextJoint { get; private set; }

		/**
     * The midpoint of the bone.
     *
     * \include Bone_center.txt
     *
     * @returns The midpoint in the center of the bone.
     * @since 2.0
     */
		public Vector Center { get; private set; }

		/**
     * The normalized direction of the bone from base to tip.
     *
     * \include Bone_direction.txt
     *
     * @returns The normalized direction of the bone from base to tip.
     * @since 2.0
     */
		public Vector Direction { get; private set; }

		/**
     * The estimated length of the bone in millimeters.
     *
     * \include Bone_length.txt
     *
     * @returns The length of the bone in millimeters.
     * @since 2.0
     */
		public float Length { get; private set; }

		/**
     * The average width of the flesh around the bone in millimeters.
     *
     * \include Bone_width.txt
     *
     * @returns The width of the flesh around the bone in millimeters.
     * @since 2.0
     */
		public float Width { get; private set; }

		/**
     * The name of this bone.
     *
     * \include Bone_type.txt
     *
     * @returns The anatomical type of this bone as a member of the Bone::Type
     * enumeration.
     * @since 2.0
     */
		public Bone.BoneType Type { get; private set; }


		/**
     * The orthonormal basis vectors for this Bone as a Matrix.
     * The orientation of this Bone as a Quaternion.
     *
     * Basis vectors specify the orientation of a bone.
     *
     * **xBasis** Perpendicular to the longitudinal axis of the
     *   bone; exits the sides of the finger.
     *
     * **yBasis or up vector** Perpendicular to the longitudinal
     *   axis of the bone; exits the top and bottom of the finger. More positive
     *   in the upward direction.
     *
     * **zBasis** Aligned with the longitudinal axis of the bone.
     *   More positive toward the base of the finger.
     *
     * The bases provided for the right hand use the right-hand rule; those for
     * the left hand use the left-hand rule. Thus, the positive direction of the
     * x-basis is to the right for the right hand and to the left for the left
     * hand. You can change from right-hand to left-hand rule by multiplying the
     * z basis vector by -1.
     *
     * You can use the basis vectors for such purposes as measuring complex
     * finger poses and skeletal animation.
     *
     * Note that converting the basis vectors directly into a quaternion
     * representation is not mathematically valid. If you use quaternions,
     * create them from the derived rotation matrix not directly from the bases.
     *
     * \include Bone_basis.txt
     *
     * @returns The basis of the bone as a matrix.
     * @returns The Quaternion.
     * @since 2.0
     */
		public Matrix Basis { get; private set; }


		public bool Left { get; private set; }

		/**
     * Enumerates the names of the bones.
     *
     * Members of this enumeration are returned by Bone::type() to identify a
     * Bone object.
     * @since 2.0
     */
		public enum BoneType
		{
			TYPE_INVALID = -1,
			TYPE_METACARPAL = 0,
			TYPE_PROXIMAL = 1,
			TYPE_INTERMEDIATE = 2,
			TYPE_DISTAL = 3,
			TYPE_ARM = 4,
		}
	}
}
