//!!!!betauser
//#include <hacdHACD.h>
//
//#include "hacdHACD_wrap.h"
//
//#ifndef BULLETC_DISABLE_HACD
//
//HACD_HACD* HACD_HACD_new()
//{
//	return new HACD::HACD();
//}
//
//bool HACD_HACD_Compute(HACD_HACD* obj)
//{
//	return obj->Compute();
//}
//
//bool HACD_HACD_Compute2(HACD_HACD* obj, bool fullCH)
//{
//	return obj->Compute(fullCH);
//}
//
//bool HACD_HACD_Compute3(HACD_HACD* obj, bool fullCH, bool exportDistPoints)
//{
//	return obj->Compute(fullCH, exportDistPoints);
//}
//
//void HACD_HACD_DenormalizeData(HACD_HACD* obj)
//{
//	obj->DenormalizeData();
//}
//
//bool HACD_HACD_GetAddExtraDistPoints(HACD_HACD* obj)
//{
//	return obj->GetAddExtraDistPoints();
//}
//
//bool HACD_HACD_GetAddFacesPoints(HACD_HACD* obj)
//{
//	return obj->GetAddFacesPoints();
//}
//
//bool HACD_HACD_GetAddNeighboursDistPoints(HACD_HACD* obj)
//{
//	return obj->GetAddNeighboursDistPoints();
//}
//
//const HACD_CallBackFunction HACD_HACD_GetCallBack(HACD_HACD* obj)
//{
//	return obj->GetCallBack();
//}
//
//bool HACD_HACD_GetCH(HACD_HACD* obj, int numCH, HACD_Vec3_Real* points, HACD_Vec3_long* triangles)
//{
//	return obj->GetCH(numCH, points, triangles);
//}
//
//double HACD_HACD_GetCompacityWeight(HACD_HACD* obj)
//{
//	return obj->GetCompacityWeight();
//}
//
//double HACD_HACD_GetConcavity(HACD_HACD* obj)
//{
//	return obj->GetConcavity();
//}
//
//double HACD_HACD_GetConnectDist(HACD_HACD* obj)
//{
//	return obj->GetConnectDist();
//}
//
//size_t HACD_HACD_GetNClusters(HACD_HACD* obj)
//{
//	return obj->GetNClusters();
//}
//
//size_t HACD_HACD_GetNPoints(HACD_HACD* obj)
//{
//	return obj->GetNPoints();
//}
//
//size_t HACD_HACD_GetNPointsCH(HACD_HACD* obj, int numCH)
//{
//	return obj->GetNPointsCH(numCH);
//}
//
//size_t HACD_HACD_GetNTriangles(HACD_HACD* obj)
//{
//	return obj->GetNTriangles();
//}
//
//size_t HACD_HACD_GetNTrianglesCH(HACD_HACD* obj, int numCH)
//{
//	return obj->GetNTrianglesCH(numCH);
//}
//
//size_t HACD_HACD_GetNVerticesPerCH(HACD_HACD* obj)
//{
//	return obj->GetNVerticesPerCH();
//}
//
//const long* HACD_HACD_GetPartition(HACD_HACD* obj)
//{
//	return obj->GetPartition();
//}
//
//const HACD_Vec3_Real* HACD_HACD_GetPoints(HACD_HACD* obj)
//{
//	return obj->GetPoints();
//}
//
//double HACD_HACD_GetScaleFactor(HACD_HACD* obj)
//{
//	return obj->GetScaleFactor();
//}
//
//const HACD_Vec3_long* HACD_HACD_GetTriangles(HACD_HACD* obj)
//{
//	return obj->GetTriangles();
//}
//
//double HACD_HACD_GetVolumeWeight(HACD_HACD* obj)
//{
//	return obj->GetVolumeWeight();
//}
//
//void HACD_HACD_NormalizeData(HACD_HACD* obj)
//{
//	obj->NormalizeData();
//}
//
//bool HACD_HACD_Save(HACD_HACD* obj, const char* fileName, bool uniColor)
//{
//	return obj->Save(fileName, uniColor);
//}
//
//bool HACD_HACD_Save2(HACD_HACD* obj, const char* fileName, bool uniColor, long numCluster)
//{
//	return obj->Save(fileName, uniColor, numCluster);
//}
//
//void HACD_HACD_SetAddExtraDistPoints(HACD_HACD* obj, bool addExtraDistPoints)
//{
//	obj->SetAddExtraDistPoints(addExtraDistPoints);
//}
//
//void HACD_HACD_SetAddFacesPoints(HACD_HACD* obj, bool addFacesPoints)
//{
//	obj->SetAddFacesPoints(addFacesPoints);
//}
//
//void HACD_HACD_SetAddNeighboursDistPoints(HACD_HACD* obj, bool addNeighboursDistPoints)
//{
//	obj->SetAddNeighboursDistPoints(addNeighboursDistPoints);
//}
//
//void HACD_HACD_SetCallBack(HACD_HACD* obj, HACD_CallBackFunction callBack)
//{
//	obj->SetCallBack(callBack);
//}
//
//void HACD_HACD_SetCompacityWeight(HACD_HACD* obj, double alpha)
//{
//	obj->SetCompacityWeight(alpha);
//}
//
//void HACD_HACD_SetConcavity(HACD_HACD* obj, double concavity)
//{
//	obj->SetConcavity(concavity);
//}
//
//void HACD_HACD_SetConnectDist(HACD_HACD* obj, double ccConnectDist)
//{
//	obj->SetConnectDist(ccConnectDist);
//}
//
//void HACD_HACD_SetNClusters(HACD_HACD* obj, int nClusters)
//{
//	obj->SetNClusters(nClusters);
//}
//
//void HACD_HACD_SetNPoints(HACD_HACD* obj, int nPoints)
//{
//	obj->SetNPoints(nPoints);
//}
//
//void HACD_HACD_SetNTriangles(HACD_HACD* obj, int nTriangles)
//{
//	obj->SetNTriangles(nTriangles);
//}
//
//void HACD_HACD_SetNVerticesPerCH(HACD_HACD* obj, int nVerticesPerCH)
//{
//	obj->SetNVerticesPerCH(nVerticesPerCH);
//}
//
//void HACD_HACD_SetPoints(HACD_HACD* obj, HACD_Vec3_Real* points)
//{
//	obj->SetPoints(points);
//}
//
//void HACD_HACD_SetScaleFactor(HACD_HACD* obj, double scale)
//{
//	obj->SetScaleFactor(scale);
//}
//
//void HACD_HACD_SetTriangles(HACD_HACD* obj, HACD_Vec3_long* triangles)
//{
//	obj->SetTriangles(triangles);
//}
//
//void HACD_HACD_SetVolumeWeight(HACD_HACD* obj, double beta)
//{
//	obj->SetVolumeWeight(beta);
//}
//
//void HACD_HACD_delete(HACD_HACD* obj)
//{
//	delete obj;
//}
//
//#endif
