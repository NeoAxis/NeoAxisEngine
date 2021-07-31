#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btBulletXmlWorldImporter* btBulletXmlWorldImporter_new(btDynamicsWorld* world);
	EXPORT bool btBulletXmlWorldImporter_loadFile(btBulletXmlWorldImporter* obj, const char* fileName);
#ifdef __cplusplus
}
#endif
