#include <BulletDynamics/ConstraintSolver/btSolverBody.h> // USE_SIMD
#include <LinearMath/btCpuFeatureUtility.h>

#include "btCpuFeatureUtility_wrap.h"

int btCpuFeatureUtility_getCpuFeatures()
{
	return btCpuFeatureUtility::getCpuFeatures();
}
