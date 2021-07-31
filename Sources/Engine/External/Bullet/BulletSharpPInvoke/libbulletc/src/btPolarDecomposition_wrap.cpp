#include <LinearMath/btPolarDecomposition.h>

#include "conversion.h"
#include "btPolarDecomposition_wrap.h"

btPolarDecomposition* btPolarDecomposition_new(btScalar tolerance, unsigned int maxIterations)
{
	return new btPolarDecomposition(tolerance, maxIterations);
}

unsigned int btPolarDecomposition_decompose(btPolarDecomposition* obj, const btMatrix3x3* a,
	btMatrix3x3* u, btMatrix3x3* h)
{
	BTMATRIX3X3_IN(a);
	BTMATRIX3X3_DEF(u);
	BTMATRIX3X3_DEF(h);
	unsigned int ret = obj->decompose(BTMATRIX3X3_USE(a), BTMATRIX3X3_USE(u), BTMATRIX3X3_USE(h));
	BTMATRIX3X3_DEF_OUT(u);
	BTMATRIX3X3_DEF_OUT(h);
	return ret;
}

unsigned int btPolarDecomposition_maxIterations(btPolarDecomposition* obj)
{
	return obj->maxIterations();
}

void btPolarDecomposition_delete(btPolarDecomposition* obj)
{
	delete obj;
}
