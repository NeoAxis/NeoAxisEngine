#pragma once

#include <LinearMath/btAlignedAllocator.h>
#include <LinearMath/btVector3.h>
#include <LinearMath/btMatrix3x3.h>
#include <LinearMath/btQuickprof.h>
#include "LinearMath/btTransform.h"

#define BTTRANSFORM_TRANSPOSE
#define BTTRANSFORM_TO4X4

inline void btVector3ToVector3(const btVector3* v, btScalar* s)
{
	s[0] = v->getX();
	s[1] = v->getY();
	s[2] = v->getZ();
}

inline void btVector3ToVector3(const btVector3& v, btScalar* s)
{
	s[0] = v.getX();
	s[1] = v.getY();
	s[2] = v.getZ();
}

inline void btVector3_copy(btVector3* destination, const btVector3* source)
{
	destination->m_floats[0] = source->m_floats[0];
	destination->m_floats[1] = source->m_floats[1];
	destination->m_floats[2] = source->m_floats[2];
}

inline void btVector3_copy(btVector3* destination, const btVector3& source)
{
	destination->m_floats[0] = source.m_floats[0];
	destination->m_floats[1] = source.m_floats[1];
	destination->m_floats[2] = source.m_floats[2];
}

inline void Vector3TobtVector3(const btScalar* s, btVector3* v)
{
	v->setX(s[0]);
	v->setY(s[1]);
	v->setZ(s[2]);
}

inline btVector3* Vector3ArrayIn(const btScalar* va, int n)
{
	btVector3* vertices = new btVector3[n];
	for (int i = 0; i < n; i++) {
		Vector3TobtVector3(&va[i * 3], &vertices[i]);
	}
	return vertices;
}

inline void btVector4ToVector4(const btVector4* v, btScalar* s)
{
	s[0] = v->getX();
	s[1] = v->getY();
	s[2] = v->getZ();
	s[3] = v->getW();
}

inline void btVector4ToVector4(const btVector4& v, btScalar* s)
{
	s[0] = v.getX();
	s[1] = v.getY();
	s[2] = v.getZ();
	s[3] = v.getW();
}

inline void Vector4TobtVector4(const btScalar* s, btVector4* v)
{
	v->setX(s[0]);
	v->setY(s[1]);
	v->setZ(s[2]);
	v->setW(s[3]);
}

inline void btVector4_copy(btVector4* destination, const btVector4* source)
{
	destination->m_floats[0] = source->m_floats[0];
	destination->m_floats[1] = source->m_floats[1];
	destination->m_floats[2] = source->m_floats[2];
	destination->m_floats[3] = source->m_floats[3];
}

inline void btVector4_copy(btVector4* destination, const btVector4& source)
{
	destination->m_floats[0] = source.m_floats[0];
	destination->m_floats[1] = source.m_floats[1];
	destination->m_floats[2] = source.m_floats[2];
	destination->m_floats[3] = source.m_floats[3];
}

inline void btQuaternionToQuaternion(const btQuaternion* q, btScalar* s)
{
	s[0] = q->getX();
	s[1] = q->getY();
	s[2] = q->getZ();
	s[3] = q->getW();
}

inline void btQuaternionToQuaternion(const btQuaternion& q, btScalar* s)
{
	s[0] = q.getX();
	s[1] = q.getY();
	s[2] = q.getZ();
	s[3] = q.getW();
}

inline void QuaternionTobtQuaternion(const btScalar* s, btQuaternion* v)
{
	v->setX(s[0]);
	v->setY(s[1]);
	v->setZ(s[2]);
	v->setW(s[3]);
}

inline void btQuaternion_copy(btQuaternion* destination, const btQuaternion* source)
{
	(*destination)[0] = (*source)[0];
	(*destination)[1] = (*source)[1];
	(*destination)[2] = (*source)[2];
	(*destination)[3] = (*source)[3];
}

inline void btQuaternion_copy(btQuaternion* destination, const btQuaternion& source)
{
	(*destination)[0] = source[0];
	(*destination)[1] = source[1];
	(*destination)[2] = source[2];
	(*destination)[3] = source[3];
}


inline void btTransformToMatrix(const btTransform* t, btScalar* m)
{
#ifdef BTTRANSFORM_TO4X4
#ifdef BTTRANSFORM_TRANSPOSE
	m[0] = t->getBasis().getRow(0).getX();
	m[4] = t->getBasis().getRow(0).getY();
	m[8] = t->getBasis().getRow(0).getZ();
	m[1] = t->getBasis().getRow(1).getX();
	m[5] = t->getBasis().getRow(1).getY();
	m[9] = t->getBasis().getRow(1).getZ();
	m[2] = t->getBasis().getRow(2).getX();
	m[6] = t->getBasis().getRow(2).getY();
	m[10] = t->getBasis().getRow(2).getZ();
#else
	m[0] = t->getBasis().getRow(0).getX();
	m[1] = t->getBasis().getRow(0).getY();
	m[2] = t->getBasis().getRow(0).getZ();
	m[4] = t->getBasis().getRow(1).getX();
	m[5] = t->getBasis().getRow(1).getY();
	m[6] = t->getBasis().getRow(1).getZ();
	m[8] = t->getBasis().getRow(2).getX();
	m[9] = t->getBasis().getRow(2).getY();
	m[10] = t->getBasis().getRow(2).getZ();
#endif
	m[3] = 0;
	m[7] = 0;
	m[11] = 0;
	m[12] = t->getOrigin().getX();
	m[13] = t->getOrigin().getY();
	m[14] = t->getOrigin().getZ();
	m[15] = 1;
#else
#ifdef BTTRANSFORM_TRANSPOSE
	m[0] = t->getBasis().getRow(0).getX();
	m[3] = t->getBasis().getRow(0).getY();
	m[6] = t->getBasis().getRow(0).getZ();
	m[1] = t->getBasis().getRow(1).getX();
	m[4] = t->getBasis().getRow(1).getY();
	m[7] = t->getBasis().getRow(1).getZ();
	m[2] = t->getBasis().getRow(2).getX();
	m[5] = t->getBasis().getRow(2).getY();
	m[8] = t->getBasis().getRow(2).getZ();
#else
	m[0] = t->getBasis().getRow(0).getX();
	m[1] = t->getBasis().getRow(0).getY();
	m[2] = t->getBasis().getRow(0).getZ();
	m[3] = t->getBasis().getRow(1).getX();
	m[4] = t->getBasis().getRow(1).getY();
	m[5] = t->getBasis().getRow(1).getZ();
	m[6] = t->getBasis().getRow(2).getX();
	m[7] = t->getBasis().getRow(2).getY();
	m[8] = t->getBasis().getRow(2).getZ();
#endif
	m[9] = t->getOrigin().getX();
	m[10] = t->getOrigin().getY();
	m[11] = t->getOrigin().getZ();
#endif
}

inline void btTransformToMatrix(const btTransform& t, btScalar* m)
{
	btTransformToMatrix(&t, m);
}

inline void MatrixTobtTransform(const btScalar* m, btTransform* t)
{
#ifdef BTTRANSFORM_TO4X4
#ifdef BTTRANSFORM_TRANSPOSE
	t->getBasis().setValue(m[0],m[4],m[8],m[1],m[5],m[9],m[2],m[6],m[10]);
#else
	t->getBasis().setValue(m[0],m[1],m[2],m[4],m[5],m[6],m[8],m[9],m[10]);
#endif
	t->getOrigin().setX(m[12]);
	t->getOrigin().setY(m[13]);
	t->getOrigin().setZ(m[14]);
#else
#ifdef BTTRANSFORM_TRANSPOSE
	t->getBasis().setValue(m[0],m[3],m[6],m[1],m[4],m[7],m[2],m[5],m[8]);
#else
	t->getBasis().setValue(m[0],m[1],m[2],m[3],m[4],m[5],m[6],m[7],m[8]);
#endif
	t->getOrigin().setX(m[9]);
	t->getOrigin().setY(m[10]);
	t->getOrigin().setZ(m[11]);
#endif
	t->getOrigin().setW(1);
}

inline void btTransform_copy(btTransform* destination, const btTransform* source)
{
	MatrixTobtTransform(reinterpret_cast<const btScalar*>(source), destination);
}


inline void btMatrix3x3ToMatrix(const btMatrix3x3* t, btScalar* m)
{
#ifdef BTTRANSFORM_TO4X4
#ifdef BTTRANSFORM_TRANSPOSE
	m[0] = t->getRow(0).getX();
	m[4] = t->getRow(0).getY();
	m[8] = t->getRow(0).getZ();
	m[1] = t->getRow(1).getX();
	m[5] = t->getRow(1).getY();
	m[9] = t->getRow(1).getZ();
	m[2] = t->getRow(2).getX();
	m[6] = t->getRow(2).getY();
	m[10] = t->getRow(2).getZ();
#else
	m[0] = t->getRow(0).getX();
	m[1] = t->getRow(0).getY();
	m[2] = t->getRow(0).getZ();
	m[4] = t->getRow(1).getX();
	m[5] = t->getRow(1).getY();
	m[6] = t->getRow(1).getZ();
	m[8] = t->getRow(2).getX();
	m[9] = t->getRow(2).getY();
	m[10] = t->getRow(2).getZ();
#endif
	m[12] = 0;
	m[13] = 0;
	m[14] = 0;
	m[15] = 1;
#else
#ifdef BTTRANSFORM_TRANSPOSE
	m[0] = t->getRow(0).getX();
	m[3] = t->getRow(0).getY();
	m[6] = t->getRow(0).getZ();
	m[1] = t->getRow(1).getX();
	m[4] = t->getRow(1).getY();
	m[7] = t->getRow(1).getZ();
	m[2] = t->getRow(2).getX();
	m[5] = t->getRow(2).getY();
	m[8] = t->getRow(2).getZ();
#else
	m[0] = t->getRow(0).getX();
	m[1] = t->getRow(0).getY();
	m[2] = t->getRow(0).getZ();
	m[3] = t->getRow(1).getX();
	m[4] = t->getRow(1).getY();
	m[5] = t->getRow(1).getZ();
	m[6] = t->getRow(2).getX();
	m[7] = t->getRow(2).getY();
	m[8] = t->getRow(2).getZ();
#endif
	m[9] = 0;
	m[10] = 0;
	m[11] = 0;
#endif
}

inline void btMatrix3x3ToMatrix(const btMatrix3x3& t, btScalar* m)
{
	btMatrix3x3ToMatrix(&t, m);
}

inline void MatrixTobtMatrix3x3(const btScalar* m, btMatrix3x3* t)
{
#ifdef BTTRANSFORM_TO4X4
#ifdef BTTRANSFORM_TRANSPOSE
	t->setValue(m[0],m[4],m[8],m[1],m[5],m[9],m[2],m[6],m[10]);
#else
	t->setValue(m[0],m[1],m[2],m[4],m[5],m[6],m[8],m[9],m[10]);
#endif
#else
#ifdef BTTRANSFORM_TRANSPOSE
	t->.setValue(m[0],m[3],m[6],m[1],m[4],m[7],m[2],m[5],m[8]);
#else
	t->setValue(m[0],m[1],m[2],m[3],m[4],m[5],m[6],m[7],m[8]);
#endif
#endif
}


// SSE requires math structs to be aligned to 16-byte boundaries.
// Alignment cannot be guaranteed in .NET, so aligned temporary intermediate variables
// must be used to exchange vectors and transforms with Bullet (if SSE is enabled).
#define TEMP(var) var ## Temp
#if defined(BT_USE_SSE) //&& defined(BT_USE_SSE_IN_API) && defined(BT_USE_SIMD_VECTOR3)
#define BTVECTOR3_DEF(v) ATTRIBUTE_ALIGNED16(btVector3) TEMP(v)
#define BTVECTOR3_USE(v) TEMP(v)
#define BTVECTOR3_SET(to, from) btVector3_copy(to, &from)
#define BTVECTOR3_COPY(to, from) btVector3_copy(to, from)
#define BTVECTOR3_IN(v) BTVECTOR3_DEF(v); BTVECTOR3_COPY(&BTVECTOR3_USE(v), v)
#define BTVECTOR3_DEF_OUT(v) BTVECTOR3_SET(v, BTVECTOR3_USE(v))

#define BTVECTOR4_DEF(v) ATTRIBUTE_ALIGNED16(btVector4) TEMP(v)
#define BTVECTOR4_USE(v) TEMP(v)
#define BTVECTOR4_SET(to, from) btVector4_copy(to, &from)
#define BTVECTOR4_COPY(to, from) btVector4_copy(to, from)
#define BTVECTOR4_IN(v) BTVECTOR4_DEF(v); BTVECTOR4_COPY(&BTVECTOR3_USE(v), v)
#define BTVECTOR4_DEF_OUT(v) BTVECTOR4_SET(v, BTVECTOR4_USE(v))

#define BTQUATERNION_DEF(v) ATTRIBUTE_ALIGNED16(btQuaternion) TEMP(v)
#define BTQUATERNION_USE(v) TEMP(v)
#define BTQUATERNION_SET(to, from) btQuaternion_copy(to, &from)
#define BTQUATERNION_COPY(to, from) btQuaternion_copy(to, from)
#define BTQUATERNION_IN(v) BTQUATERNION_DEF(v); BTQUATERNION_COPY(&BTQUATERNION_USE(v), v)
#define BTQUATERNION_DEF_OUT(v) BTQUATERNION_SET(v, BTQUATERNION_USE(v))

#define BTTRANSFORM_DEF(v) ATTRIBUTE_ALIGNED16(btTransform) TEMP(v)
#define BTTRANSFORM_USE(v) TEMP(v)
#define BTTRANSFORM_SET(to, from) btTransform_copy(to, &from)
#define BTTRANSFORM_COPY(to, from) btTransform_copy(to, from)
#define BTTRANSFORM_IN(v) BTTRANSFORM_DEF(v); BTTRANSFORM_COPY(&BTTRANSFORM_USE(v), v)
#define BTTRANSFORM_DEF_OUT(v) BTTRANSFORM_SET(v, BTTRANSFORM_USE(v))
#define BTTRANSFORM_IN_REF(v) BTTRANSFORM_DEF(v); BTTRANSFORM_SET(&BTTRANSFORM_USE(v), v)
#define BTTRANSFORM_USE_REF(v) TEMP(v)
#define BTTRANSFORM_DEF_OUT_REF(v) BTTRANSFORM_SET(&v, BTTRANSFORM_USE_REF(v))

#define BTMATRIX3X3_DEF(tr) ATTRIBUTE_ALIGNED16(btMatrix3x3) TEMP(tr)
#else
// Cant use a pinned pointer to a Vector3 in case sizeof(Vector3) != sizeof(btVector3)
#if VECTOR3_16B
#define BTVECTOR3_DEF(v)
#define BTVECTOR3_USE(v) *v
#define BTVECTOR3_SET(to, from) *to = from
#define BTVECTOR3_COPY(to, from) BTVECTOR3_SET(to, *from)
#define BTVECTOR3_IN(v)
#define BTVECTOR3_DEF_OUT(v)
#else
#define BTVECTOR3_DEF(v) btVector3 TEMP(v)
#define BTVECTOR3_USE(v) TEMP(v)
#define BTVECTOR3_SET(to, from) btVector3_copy(to, &from)
#define BTVECTOR3_COPY(to, from) btVector3_copy(to, from)
#define BTVECTOR3_IN(v) BTVECTOR3_DEF(v); BTVECTOR3_COPY(&BTVECTOR3_USE(v), v)
#define BTVECTOR3_DEF_OUT(v) BTVECTOR3_SET(v, BTVECTOR3_USE(v))
#endif

#define BTVECTOR4_DEF(v)
#define BTVECTOR4_USE(v) *v
#define BTVECTOR4_SET(to, from) *to = from
#define BTVECTOR4_COPY(to, from) BTVECTOR4_SET(to, *from)
#define BTVECTOR4_IN(v)
#define BTVECTOR4_DEF_OUT(v)

#define BTQUATERNION_DEF(v)
#define BTQUATERNION_USE(v) *v
#define BTQUATERNION_SET(to, from) *to = from
#define BTQUATERNION_COPY(to, from) BTQUATERNION_SET(to, *from)
#define BTQUATERNION_IN(v)
#define BTQUATERNION_DEF_OUT(v)

#ifdef BTTRANSFORM_TRANSPOSE
#define BTTRANSFORM_DEF(v) btTransform TEMP(v)
#define BTTRANSFORM_USE(v) TEMP(v)
#define BTTRANSFORM_SET(to, from) btTransform_copy(to, &from)
#define BTTRANSFORM_COPY(to, from) btTransform_copy(to, from)
#define BTTRANSFORM_IN(v) BTTRANSFORM_DEF(v); BTTRANSFORM_COPY(&BTTRANSFORM_USE(v), v)
#define BTTRANSFORM_DEF_OUT(v) BTTRANSFORM_SET(v, BTTRANSFORM_USE(v))
#define BTTRANSFORM_IN_REF(v) BTTRANSFORM_DEF(v); BTTRANSFORM_SET(&BTTRANSFORM_USE(v), v)
#define BTTRANSFORM_USE_REF(v) TEMP(v)
#define BTTRANSFORM_DEF_OUT_REF(v) BTTRANSFORM_SET(&v, BTTRANSFORM_USE_REF(v))
#else
#define BTTRANSFORM_DEF(v)
#define BTTRANSFORM_USE(v) *v
#define BTTRANSFORM_SET(to, from) *to = from
#define BTTRANSFORM_COPY(to, from) BTTRANSFORM_SET(to, *from)
#define BTTRANSFORM_IN(v)
#define BTTRANSFORM_DEF_OUT(v)
#define BTTRANSFORM_IN_REF(v)
#define BTTRANSFORM_USE_REF(v) v
#define BTTRANSFORM_DEF_OUT_REF(v)
#endif

#define BTMATRIX3X3_DEF(tr) btMatrix3x3 TEMP(tr)
#endif

#define BTMATRIX3X3_USE(tr) TEMP(tr)
#define BTMATRIX3X3_SET(to, from) MatrixTobtMatrix3x3(from, to)
#define BTMATRIX3X3_IN(v) BTMATRIX3X3_DEF(v); MatrixTobtMatrix3x3((btScalar*)v, &BTMATRIX3X3_USE(v))
#define BTMATRIX3X3_OUT(to, from) btMatrix3x3ToMatrix(from, (btScalar*)to)
#define BTMATRIX3X3_DEF_OUT(tr) BTMATRIX3X3_OUT(tr, &TEMP(tr))
