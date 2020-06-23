
Для изменений в FbxWrapper:
1) В папке SWIG изменить SWIG\fbxwapper.i, иногда может потребоваться изменить SWIG\FBXSDK_ChangedHeaders.
2) Запустить SWIG\generate.cmd
При этом автоматически записываются файлы:
  - FbxWrapperSln\FbxWrapperNative\FbxWrapperNative.cpp
  - Сначала автоматически удаляются FbxWrapperSln\FbxWrapper\*.cs потом в эту папку записываются новые.
3) Затем скомпилировать FbxWrapperSln. В проекте FbxWrapperSln\FbxWrapper\FbxWrapper.csproj  вручную вставлена строка:
   <ItemGroup>
    <Compile Include="*.cs" />
  </ItemGroup>
В результате вместо явного включения файлов, подключаются все файлы, из этой папки. Поэтому через VisualStudio не стоит в этом проекте ничего добавлят/удалять вручную.
=================================
==================================
Не включенные headers:
-------------

1) //%include "fbxsdk/utils/fbxembeddedfilesaccumulator.h" //здесь, для одного из типов генерировалось слишком длинное имя
Если его добавить, то надо добавить и #include, чтобы в *.cpp файле появился)
//#include <fbxsdk\utils\fbxembeddedfilesaccumulator.h> 

2) Здесь нет в *lib файле символа.
%include "fbxsdk/fileio/fbx/fbxreaderfbx5.h"
%include "fbxsdk/fileio/fbx/fbxreaderfbx6.h"
%include "fbxsdk/fileio/fbx/fbxreaderfbx7.h"
%include "fbxsdk/fileio/fbx/fbxwriterfbx5.h"
%include "fbxsdk/fileio/fbx/fbxwriterfbx6.h"
%include "fbxsdk/fileio/fbx/fbxwriterfbx7.h"

3) Не нашлось такого header: //Нужен: #include <components/libxml2-2.7.8/include/libxml/globals.h>
%include "fbxsdk/fileio/collada/fbxcolladaelement.h" 
%include "fbxsdk/fileio/collada/fbxcolladaiostream.h"
%include "fbxsdk/fileio/collada/fbxcolladanamespace.h" //Нужен: #include <components/libxml2-2.7.8/include/libxml/globals.h>
%include "fbxsdk/fileio/collada/fbxreadercollada14.h"
%include "fbxsdk/fileio/collada/fbxwritercollada14.h"
%include "fbxsdk/fileio/collada/fbxcolladautils.h"
%include "fbxsdk/fileio/collada/fbxcolladaanimationelement.h"

4) %include "fbxsdk/utils/fbxprocessorxrefuserlib.h"  // C# файле остаются некорректные override.

5) %include "fbxsdk/fbxsdk_nsbegin.h"
6) Этот включен, но есть warning: %include "fbxsdk/fileio/fbxexternaldocreflistener.h" //Multiple inheritance warning 
================================
================================

Изменения которые были произведены в SWIG\FBXSDK_ChangedHeaders:

Исправлены несколько небольших ошибок:
 - убрать незначащие скобки вокруг имени 
 - переставить местами модификаторы static,const,...
 - удалить одну функцию, сложный указатель с const, есть еще такая же функция но без const(ее достаточно)

------

(это изменение нужно чтобы FbxLayerElementTemplate.GetDirectArray() возвращал правильный тип, иначе SWIG даже warning не выдает, а возвращаемый тип будет с урезанным функционалом)
В файле: C:\Program Files\Autodesk\FBX\FBX SDK\2019.0\include\fbxsdk\scene\geometry\fbxlayer.h 
Строка: 1022
Было: template <class Type> class FbxLayerElementTemplate : public FbxLayerElement
Вставить перед этой строкой текст:
%template(FbxLayerElementArrayTemplateFbxVector4)FbxLayerElementArrayTemplate<FbxVector4>;
%template(FbxLayerElementArrayTemplateFbxSurfaceMaterialPtr)FbxLayerElementArrayTemplate<FbxSurfaceMaterial*>;
%template(FbxLayerElementArrayTemplateFbxVector2)FbxLayerElementArrayTemplate<FbxVector2>;
%template(FbxLayerElementArrayTemplateFbxInt)FbxLayerElementArrayTemplate<int>;
%template(FbxLayerElementArrayTemplateFbxColor)FbxLayerElementArrayTemplate<FbxColor>;
%template(FbxLayerElementArrayTemplateFbxDouble)FbxLayerElementArrayTemplate<double>;
%template(FbxLayerElementArrayTemplateFbxBool)FbxLayerElementArrayTemplate<bool>;
%template(FbxLayerElementArrayTemplateFbxVoidPtr)FbxLayerElementArrayTemplate<void*>;
%template(FbxLayerElementArrayTemplateFbxTexturePtr)FbxLayerElementArrayTemplate<FbxTexture*>;


--------------
В файле: C:\Program Files\Autodesk\FBX\FBX SDK\2019.0\include\fbxsdk\scene\geometry\fbxlayer.h 
Строка: 1303
Было:  class FBXSDK_DLL FbxLayerElementNormal : public FbxLayerElementTemplate<FbxVector4>
Вставить перед этой строкой текст:
%template(FbxLayerElementTemplateFbxVector4)FbxLayerElementTemplate<FbxVector4>;
%template(FbxLayerElementTemplateFbxSurfaceMaterialPtr)FbxLayerElementTemplate<FbxSurfaceMaterial*>;
%template(FbxLayerElementTemplateFbxVector2)FbxLayerElementTemplate<FbxVector2>;
%template(FbxLayerElementTemplateFbxInt)FbxLayerElementTemplate<int>;
%template(FbxLayerElementTemplateFbxColor)FbxLayerElementTemplate<FbxColor>;
%template(FbxLayerElementTemplateFbxDouble)FbxLayerElementTemplate<double>;
%template(FbxLayerElementTemplateFbxBool)FbxLayerElementTemplate<bool>;
%template(FbxLayerElementTemplateFbxVoidPtr)FbxLayerElementTemplate<void*>;
%template(FbxLayerElementTemplateFbxTexturePtr)FbxLayerElementTemplate<FbxTexture*>;

--------------
В файле: C:\Program Files\Autodesk\FBX\FBX SDK\2019.0\include\fbxsdk\core\base\fbxstringlist.h 
Строка: 1251
Вставить: 
%template(FbxStringListTFbxStringListItem) FbxStringListT<FbxStringListItem>;
--------------
В файле : C:\Program Files\Autodesk\FBX\FBX SDK\2019.0\include\fbxsdk\core\base\fbxmap.h 
строка: 433
Закомментировать эти 2 строчки (только чтобы не было wirning что они ignored):
template class FbxSimpleMap<FbxString, FbxObject*, FbxStringCompare>;
template class FbxObjectMap<FbxString, FbxStringCompare>;

строка: 366
было: class FBXSDK_DLL FbxObjectStringMap : public FbxObjectMap<FbxString, FbxStringCompare>
Вставить перед этой строкой:
%template(FbxSimpleMapFbxStringFbxObjectPFbxStringCompare) FbxSimpleMap<FbxString, FbxObject*, FbxStringCompare>;
%template(FbxObjectMapFbxStringFbxStringCompare) FbxObjectMap<FbxString, FbxStringCompare>;

