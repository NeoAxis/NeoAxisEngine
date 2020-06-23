%module "FbxWrapperNative"
%{
#include <fbxsdk.h>
#include <fbxsdk\fileio\collada\fbxcolladatokens.h>
#include <fbxsdk\fileio\fbxbase64coder.h>
#include <fbxsdk\scene\shading\fbxlayerentryview.h>
//#include <fbxsdk\utils\fbxembeddedfilesaccumulator.h> //%include "fbxsdk/utils/fbxembeddedfilesaccumulator.h" //здесь, дл€ одного из типов генерировалось слишком длинное им€
#include <fbxsdk\scene\fbxobjectscontainer.h>
#include <fbxsdk\utils\fbxrenamingstrategyfbx7.h>

/*     //Needed #include <components/libxml2-2.7.8/include/libxml/globals.h>
#include <fbxsdk\fbxsdk_nsbegin.h>
#include <fbxsdk\fileio\collada\fbxcolladanamespace.h>
#include <fbxsdk\fileio\collada\fbxcolladaelement.h>  
#include <fbxsdk\fileio\collada\fbxcolladaiostream.h> 
#include <fbxsdk\fileio\collada\fbxreadercollada14.h>
#include <fbxsdk\fileio\collada\fbxwritercollada14.h>
#include <fbxsdk\fileio\collada\fbxcolladautils.h>
#include <fbxsdk\fileio\collada\fbxcolladaanimationelement.h>
*/

/* added new, ... unresolved external symbol */
/*
#include <fbxsdk\fileio\fbx\fbxreaderfbx5.h>
#include <fbxsdk\fileio\fbx\fbxreaderfbx6.h>
#include <fbxsdk\fileio\fbx\fbxreaderfbx7.h>
#include <fbxsdk\fileio\fbx\fbxwriterfbx5.h>
#include <fbxsdk\fileio\fbx\fbxwriterfbx6.h>
#include <fbxsdk\fileio\fbx\fbxwriterfbx7.h>
*/
%}


#define _WIN32
#define _WIN64
#define _MSC_VER 1800
#define _M_X64


#pragma SWIG nowarn=451,516

%include <windows.i>
%include "arrays_csharp.i"
%include "carrays.i"
%include "cpointer.i"

%array_functions(double, DoubleArrayFunc);  //≈сли с тем же именем что в %array_class(double, DoubleArray); “о выдает Warning
%pointer_functions(double, DoubleArrayPtrFunc);


%array_class(double, DoubleArray);
%array_class(float, FloatArray);
%array_class(int, IntArray);
%array_class(FbxVector4, FbxVector4Array);


// Wrap a class interface around an "int *" 
%pointer_class(int, Int_p);
%pointer_class(float, Float_p); 


%ignore FbxProcessorXRefCopy;
%rename(add)             operator+;
%rename(pos)             operator+();
%rename(pos)             operator+() const;

%rename(sub)             operator-;
%rename(neg)             operator-() const;
%rename(neg)             operator-();

%rename(mul)             operator*;
%rename(deref)           operator*();
%rename(deref)           operator*() const;

%rename(div)             operator/;
%rename(mod)             operator%;
%rename(logxor)          operator^;
%rename(logand)          operator&;
%rename(logior)          operator|;
%rename(lognot)          operator~();
%rename(lognot)          operator~() const;


%rename(assign)          operator=;

%rename(add_assign)      operator+=;
%rename(sub_assign)      operator-=;
%rename(mul_assign)      operator*=;
%rename(div_assign)      operator/=;
%rename(mod_assign)      operator%=;
%rename(logxor_assign)   operator^=;
%rename(logand_assign)   operator&=;
%rename(logior_assign)   operator|=;

%rename(eq)              operator==;
%rename(ne)              operator!=;
%rename(lt)              operator<;
%rename(gt)              operator>;
%rename(lte)             operator<=;
%rename(gte)             operator>=;

%rename(at)              operator[];
%rename(call)            operator();

/* added new */
%rename(And)  		 operator &&;
%rename(Or)              operator ||;
%rename(Not)             operator !;

%rename(ToBool)          operator bool;
%rename(ToDoublePtr)     operator double*;
%rename(ToConstDoublePtr) operator const double*;
%rename(ToConstCharPtr)  operator const char*;
%rename(ToDouble2)       operator FbxVectorTemplate2<double>&;
%rename(ToDouble3)       operator FbxVectorTemplate3<double>&;
%rename(ToFbxVectorTemplate2) operator FbxVectorTemplate2<T>&;
%rename(ToFbxVectorTemplate3) operator FbxVectorTemplate3<T>&;                     	
%rename(ToInt)           operator int*;                //ƒл€ FbxArray<..
%rename(ToFbxVector2)    operator FbxVector2*;  //ƒл€ FbxArray<..
%rename(ToType)   operator T; //this operator is for templates. It does not cover "operator bool"
 
                  

%ignore FbxSelectionToTimeFormat;
%ignore FbxSelectionToTimeMode;
%ignore FbxTimeToSelection;
%ignore PropertyNotify;
%ignore FbxLayeredTexture::mInputData;
%ignore FbxImporter::SetPassword;
%ignore FbxSubDiv::SetLevelCount;
%ignore FbxImplementation::sDefaultType;


%warnfilter(302) FbxGetDirectArray;   
%warnfilter(302) FbxDelete;
%warnfilter(302) MissingUrlHandler;
//Supress the wrong warning about redefined functions:
//fbxlayer.h - Function definition without body followed by definition with body.
//fbxnew.h,fbxalloc.h - FbxDelete(T* p), FbxDelete(const T* p) - definition without body, followed by definition with body.
//fbxprocessorxref.h - Nested class MissingUrlHandler, and member MissingUrlHandler


%include "fbxsdk/core/arch/fbxarch.h"
%include "fbxsdk/core/arch/fbxtypes.h"
%include "fbxsdk/fbxsdk_nsend.h"
%include "fbxsdk/fbxsdk_version.h"


%include "fbxsdk/fileio/collada/fbxcolladatokens.h"
%include "fbxsdk/core/arch/fbxdebug.h"


%template(FbxDouble2) FbxVectorTemplate2<double>;
%template(FbxDouble3) FbxVectorTemplate3<FbxDouble>;
%template(FbxDouble4) FbxVectorTemplate4<FbxDouble>;
%template(FbxDouble4x4) FbxVectorTemplate4<FbxDouble4>;

%template(FbxDouble3x4) FbxVectorTemplate3<FbxDouble4>;
%template(FbxDouble2x4) FbxVectorTemplate2<FbxDouble4>;


%include "fbxsdk/fbxsdk_def.h"
%include "fbxsdk/core/fbxclassid.h"
%include "fbxsdk/core/fbxmodule.h"
%include "fbxsdk/core/fbxperipheral.h"
%include "fbxsdk/core/arch/fbxalloc.h"
%include "fbxsdk/core/arch/fbxnew.h"
%include "fbxsdk/core/arch/fbxstdcompliant.h"

%include "fbxsdk/core/base/fbxarray.h"
%template(FbxArrayFbxVector2) FbxArray<FbxVector2>;
%template(FbxArrayInt) FbxArray<int>;


%include "fbxsdk/core/base/fbxbitset.h"
%include "fbxsdk/core/base/fbxcharptrset.h"
%include "fbxsdk/core/base/fbxcontainerallocators.h"
%include "fbxsdk/core/base/fbxdynamicarray.h"
%include "fbxsdk/core/base/fbxintrusivelist.h"
%include "fbxsdk/core/base/fbxmultimap.h"
%include "fbxsdk/core/base/fbxpair.h"
%include "fbxsdk/core/base/fbxredblacktree.h"
%include "fbxsdk/core/base/fbxstring.h"
%include "fbxsdk/core/base/fbxstringlist.h"
%include "fbxsdk/core/base/fbxtimecode.h"
%include "fbxsdk/core/math/fbxvector2.h"
%include "fbxsdk/core/math/fbxvector4.h"
%include "fbxsdk/core/sync/fbxatomic.h"
%include "fbxsdk/core/sync/fbxclock.h"
%include "fbxsdk/core/sync/fbxthread.h"
%include "fbxsdk/fileio/fbxbase64coder.h"
%include "fbxsdk/fileio/fbxfiletokens.h"
%include "fbxsdk/fileio/fbxglobalcamerasettings.h"
%include "fbxsdk/fileio/fbxgobo.h"
%include "fbxsdk/fileio/fbxiosettingspath.h"
%include "fbxsdk/fileio/fbxprogress.h"
%include "fbxsdk/fileio/fbxstatistics.h"
%include "fbxsdk/fileio/fbxstatisticsfbx.h"
%include "fbxsdk/scene/fbxaxissystem.h"
%include "fbxsdk/scene/fbxgroupname.h"
%include "fbxsdk/scene/constraint/fbxcharacternodename.h"
%include "fbxsdk/scene/constraint/fbxconstraintutils.h"
%include "fbxsdk/scene/geometry/fbxweightedmapping.h"
%include "fbxsdk/scene/shading/fbxbindingtableentry.h"
%include "fbxsdk/scene/shading/fbxentryview.h"
%include "fbxsdk/scene/shading/fbxoperatorentryview.h"
%include "fbxsdk/scene/shading/fbxpropertyentryview.h"
%include "fbxsdk/scene/shading/fbxsemanticentryview.h"
%include "fbxsdk/scene/shading/fbxshadingconventions.h"
%include "fbxsdk/utils/fbxdeformationsevaluator.h"
%include "fbxsdk/utils/fbxgeometryconverter.h"
%include "fbxsdk/utils/fbxnamehandler.h"
%include "fbxsdk/utils/fbxrenamingstrategy.h"
%include "fbxsdk/utils/fbxrenamingstrategyutilities.h"
%include "fbxsdk/utils/fbxusernotification.h"
%include "fbxsdk/core/fbxconnectionpoint.h"
%include "fbxsdk/core/fbxsystemunit.h"
%include "fbxsdk/core/fbxxref.h"
%include "fbxsdk/core/base/fbxfile.h"
%include "fbxsdk/core/base/fbxfolder.h"
%include "fbxsdk/core/base/fbxmap.h"
%include "fbxsdk/core/base/fbxmemorypool.h"
%include "fbxsdk/core/base/fbxset.h"
%include "fbxsdk/core/base/fbxstatus.h"
%include "fbxsdk/core/base/fbxtime.h"
%include "fbxsdk/core/base/fbxutils.h"
%include "fbxsdk/core/math/fbxaffinematrix.h"
%include "fbxsdk/core/math/fbxmatrix.h"
%include "fbxsdk/core/math/fbxquaternion.h"
%include "fbxsdk/core/sync/fbxsync.h"
%include "fbxsdk/fileio/fbxwriter.h"
%include "fbxsdk/fileio/fbx/fbxio.h"
%include "fbxsdk/scene/fbxtakeinfo.h"
%include "fbxsdk/scene/shading/fbxbindingsentryview.h"
%include "fbxsdk/scene/shading/fbxconstantentryview.h"
%include "fbxsdk/utils/fbxrootnodeutility.h"
%include "fbxsdk/core/fbxpropertytypes.h"
%include "fbxsdk/core/fbxquery.h"
%include "fbxsdk/core/fbxstream.h"
%include "fbxsdk/core/fbxsymbol.h"
%include "fbxsdk/core/base/fbxhashmap.h"
%include "fbxsdk/core/math/fbxdualquaternion.h"
%include "fbxsdk/core/math/fbxmath.h"
%include "fbxsdk/core/math/fbxtransforms.h"
%include "fbxsdk/fileio/fbxgloballightsettings.h"
%include "fbxsdk/fileio/fbxreader.h"

%include "fbxsdk/core/fbxevent.h"
%template(FbxEventFbxEventPreExport) FbxEvent<FbxEventPreExport>;
%template(FbxEventFbxEventPostExport) FbxEvent<FbxEventPostExport>;
%template(FbxEventFbxEventPreImport) FbxEvent<FbxEventPreImport>;
%template(FbxEventFbxEventPostImport) FbxEvent<FbxEventPostImport>;
%template(FbxEventFbxEventReferencedDocument) FbxEvent<FbxEventReferencedDocument>;
%template(FbxEventFbxObjectPropertyChanged) FbxEvent<FbxObjectPropertyChanged>;
%template(FbxEventFbxEventPopulateSystemLibrary) FbxEvent<FbxEventPopulateSystemLibrary>;
%template(FbxEventFbxEventUpdateSystemLibrary) FbxEvent<FbxEventUpdateSystemLibrary>;
%template(FbxEventFbxEventWriteLocalization) FbxEvent<FbxEventWriteLocalization>;
%template(FbxEventFbxEventMapAssetFileToAssetObject) FbxEvent<FbxEventMapAssetFileToAssetObject>;

%include "fbxsdk/core/fbxeventhandler.h"
%include "fbxsdk/core/fbxpropertydef.h"
%include "fbxsdk/core/fbxpropertyhandle.h"
%include "fbxsdk/core/fbxqueryevent.h"
%include "fbxsdk/fileio/fbxiopluginregistry.h"
%include "fbxsdk/core/fbxdatatypes.h"
%include "fbxsdk/core/fbxemitter.h"
%include "fbxsdk/core/fbxlistener.h"

%include "fbxsdk/core/fbxproperty.h"
/* realization of FbxPropertyT<> defined in fbxproperty.h */
/* ToDo : add another types FbxPropertyT<>*/
%template(FbxPropertyTFbxDouble3) FbxPropertyT<FbxDouble3>;
%template(FbxPropertyTFbxDouble)  FbxPropertyT<FbxDouble>;
%template(FbxPropertyTEInheritType)  FbxPropertyT<FbxTransform::EInheritType>;


%include "fbxsdk/fileio/fbxexternaldocreflistener.h" //Multiple inheritance warning
%include "fbxsdk/core/fbxobject.h"
%include "fbxsdk/core/fbxplugin.h"
%include "fbxsdk/core/fbxplugincontainer.h"
%include "fbxsdk/core/fbxpropertypage.h"
%include "fbxsdk/fileio/fbxglobalsettings.h"
%include "fbxsdk/fileio/fbxiobase.h"
%include "fbxsdk/fileio/fbxiosettings.h"
%include "fbxsdk/scene/fbxcollection.h"
%include "fbxsdk/scene/fbxcollectionexclusive.h"
%include "fbxsdk/scene/fbxcontainertemplate.h"
%include "fbxsdk/scene/fbxdisplaylayer.h"
%include "fbxsdk/scene/fbxdocument.h"
%include "fbxsdk/scene/fbxdocumentinfo.h"
%include "fbxsdk/scene/fbxenvironment.h"
%include "fbxsdk/scene/fbxobjectfilter.h"
%include "fbxsdk/scene/fbxobjectmetadata.h"
%include "fbxsdk/scene/fbxpose.h"
%include "fbxsdk/scene/fbxreference.h"
%include "fbxsdk/scene/fbxselectionnode.h"
%include "fbxsdk/scene/fbxselectionset.h"
%include "fbxsdk/scene/fbxthumbnail.h"

%include "fbxsdk/scene/fbxmediaclip.h"
%include "fbxsdk/scene/fbxaudio.h"
%include "fbxsdk/scene/fbxaudiolayer.h"
%include "fbxsdk/utils/fbxscenecheckutility.h"

%include "fbxsdk/scene/fbxvideo.h"

%include "fbxsdk/scene/animation/fbxanimcurvebase.h"
%include "fbxsdk/scene/animation/fbxanimcurvenode.h"
%include "fbxsdk/scene/animation/fbxanimlayer.h"
%include "fbxsdk/scene/animation/fbxanimstack.h"
%include "fbxsdk/scene/constraint/fbxconstraint.h"
%include "fbxsdk/scene/constraint/fbxconstraintaim.h"
%include "fbxsdk/scene/constraint/fbxconstraintcustom.h"
%include "fbxsdk/scene/constraint/fbxconstraintparent.h"
%include "fbxsdk/scene/constraint/fbxconstraintposition.h"
%include "fbxsdk/scene/constraint/fbxconstraintrotation.h"
%include "fbxsdk/scene/constraint/fbxconstraintscale.h"
%include "fbxsdk/scene/constraint/fbxconstraintsinglechainik.h"
%include "fbxsdk/scene/geometry/fbxcache.h"
%include "fbxsdk/scene/geometry/fbxdeformer.h"
%include "fbxsdk/scene/geometry/fbxgenericnode.h"
%include "fbxsdk/scene/geometry/fbxgeometryweightedmap.h"
%include "fbxsdk/scene/geometry/fbxnode.h"
%include "fbxsdk/scene/geometry/fbxnodeattribute.h"
%include "fbxsdk/scene/geometry/fbxnull.h"
%include "fbxsdk/scene/geometry/fbxopticalreference.h"
%include "fbxsdk/scene/geometry/fbxskeleton.h"
%include "fbxsdk/scene/geometry/fbxskin.h"
%include "fbxsdk/scene/geometry/fbxsubdeformer.h"
%include "fbxsdk/scene/geometry/fbxvertexcachedeformer.h"
%include "fbxsdk/scene/shading/fbxbindingtablebase.h"
%include "fbxsdk/scene/shading/fbximplementation.h"
%include "fbxsdk/scene/shading/fbximplementationfilter.h"
%include "fbxsdk/scene/shading/fbxsurfacematerial.h"
%include "fbxsdk/scene/shading/fbxtexture.h"
%include "fbxsdk/utils/fbxclonemanager.h"
%include "fbxsdk/utils/fbxprocessor.h"
%include "fbxsdk/utils/fbxprocessorxref.h"
%include "fbxsdk/core/fbxloadingstrategy.h"
%include "fbxsdk/core/fbxmanager.h"
%include "fbxsdk/core/fbxscopedloadingdirectory.h"
%include "fbxsdk/core/fbxscopedloadingfilename.h"
%include "fbxsdk/fileio/fbxexporter.h"
%include "fbxsdk/fileio/fbximporter.h"
%include "fbxsdk/scene/fbxcontainer.h"
%include "fbxsdk/scene/fbxlibrary.h"
%include "fbxsdk/scene/animation/fbxanimcurve.h"
%include "fbxsdk/scene/animation/fbxanimcurvefilters.h"
%include "fbxsdk/scene/animation/fbxanimevalstate.h"
%include "fbxsdk/scene/animation/fbxanimevaluator.h"
%include "fbxsdk/scene/animation/fbxanimutilities.h"
%include "fbxsdk/scene/constraint/fbxcharacter.h"
%include "fbxsdk/scene/constraint/fbxcharacterpose.h"
%include "fbxsdk/scene/constraint/fbxcontrolset.h"
%include "fbxsdk/scene/constraint/fbxhik2fbxcharacter.h"
%include "fbxsdk/scene/geometry/fbxblendshape.h"
%include "fbxsdk/scene/geometry/fbxblendshapechannel.h"
%include "fbxsdk/scene/geometry/fbxcachedeffect.h"
%include "fbxsdk/scene/geometry/fbxcamera.h"
%include "fbxsdk/scene/geometry/fbxcamerastereo.h"
%include "fbxsdk/scene/geometry/fbxcameraswitcher.h"
%include "fbxsdk/scene/geometry/fbxcluster.h"
%include "fbxsdk/scene/geometry/fbxlayer.h"


%template(FbxLayerElementArrayTemplateInt)FbxLayerElementArrayTemplate<int>;
%pointer_functions(FbxLayerElementArrayTemplate<int>*, FbxLayerElementArrayTemplateIntPtrFunc);
%pointer_functions(FbxLayerElementArrayTemplate<int>**, FbxLayerElementArrayTemplateIntPtrPtrFunc);

%include "fbxsdk/scene/geometry/fbxlayercontainer.h"
%include "fbxsdk/scene/geometry/fbxlight.h"
%include "fbxsdk/scene/geometry/fbxlimitsutilities.h"
%include "fbxsdk/scene/geometry/fbxlodgroup.h"
%include "fbxsdk/scene/geometry/fbxmarker.h"
%include "fbxsdk/scene/shading/fbxbindingoperator.h"
%include "fbxsdk/scene/shading/fbxbindingtable.h"
%include "fbxsdk/scene/shading/fbxfiletexture.h"
%include "fbxsdk/scene/shading/fbximplementationutils.h"
%include "fbxsdk/scene/shading/fbxlayeredtexture.h"
%include "fbxsdk/scene/shading/fbxlayerentryview.h"
%include "fbxsdk/scene/shading/fbxproceduraltexture.h"
%include "fbxsdk/scene/shading/fbxsurfacelambert.h"
%include "fbxsdk/scene/shading/fbxsurfacephong.h"

//%include "fbxsdk/utils/fbxembeddedfilesaccumulator.h" //здесь, дл€ одного из типов генерировалось слишком длинное им€

%include "fbxsdk/utils/fbxmanipulators.h"
%include "fbxsdk/utils/fbxmaterialconverter.h"
%include "fbxsdk/utils/fbxprocessorshaderdependency.h"
%include "fbxsdk/scene/fbxscene.h"
%include "fbxsdk/scene/animation/fbxanimevalclassic.h"
%include "fbxsdk/scene/geometry/fbxgeometrybase.h"
%include "fbxsdk/scene/geometry/fbxshape.h"
%include "fbxsdk/utils/fbxrenamingstrategybase.h"
%include "fbxsdk/utils/fbxrenamingstrategyfbx5.h"
%include "fbxsdk/utils/fbxrenamingstrategyfbx6.h"
%include "fbxsdk/utils/fbxrenamingstrategyfbx7.h"
%include "fbxsdk/scene/fbxobjectscontainer.h"

%include "fbxsdk/scene/geometry/fbxgeometry.h"
%include "fbxsdk/scene/geometry/fbxline.h"
%include "fbxsdk/scene/geometry/fbxmesh.h"
%include "fbxsdk/scene/geometry/fbxnurbs.h"
%include "fbxsdk/scene/geometry/fbxnurbscurve.h"
%include "fbxsdk/scene/geometry/fbxnurbssurface.h"
%include "fbxsdk/scene/geometry/fbxpatch.h"
%include "fbxsdk/scene/geometry/fbxproceduralgeometry.h"
%include "fbxsdk/scene/geometry/fbxsubdiv.h"
%include "fbxsdk/scene/geometry/fbxtrimnurbssurface.h"


/*
//Needed #include <components/libxml2-2.7.8/include/libxml/globals.h>
%include "fbxsdk/fileio/collada/fbxcolladaelement.h"
%warnfilter(302) FromString; //double definition ofFromString;
%include "fbxsdk/fileio/collada/fbxcolladaiostream.h"
%warnfilter(+302) FromString;
%include "fbxsdk/fileio/collada/fbxcolladanamespace.h"
%include "fbxsdk/fileio/collada/fbxreadercollada14.h"
%include "fbxsdk/fileio/collada/fbxwritercollada14.h"
%include "fbxsdk/fileio/collada/fbxcolladautils.h"
%include "fbxsdk/fileio/collada/fbxcolladaanimationelement.h"
*/

/*
%include "fbxsdk/fileio/fbx/fbxreaderfbx5.h"
%include "fbxsdk/fileio/fbx/fbxreaderfbx6.h"
%include "fbxsdk/fileio/fbx/fbxreaderfbx7.h"
%include "fbxsdk/fileio/fbx/fbxwriterfbx5.h"
%include "fbxsdk/fileio/fbx/fbxwriterfbx6.h"
%include "fbxsdk/fileio/fbx/fbxwriterfbx7.h"
*/

//-----------------------------------------------------------------------
//Ѕлок операций конвертации от базового к производному типу. ≈сли FBX wrapper возвращает базовый тип, но известно какой у него производный тип,
//иногда нужно конвертировать к производному. Ѕез этих функций Cast это не получитс€, они добавл€ютс€ в wrapped классы.
//Ќапример: converter.Triangulate( mesh, false ), возвращает FbxNodeAttribute, его надо привести к FbxMesh через FbxMesh.Cast()
//-----------------------------------------------------------------------

//ToDo : ƒобавить Cast дл€ других наследников FbxNodeAttribute.

  // ƒобавлено из-за того что SWIG правильно не распознает FbxLongLong хот€ он определен через typedef как __int64
  // »наче не получаетс€ передавать __int64, например, в FbxTime
%extend FbxTime {
  static __int64 ToInt64(FbxLongLong value) {
    return __int64(value);
  }
   static FbxLongLong ToFbxLongLong(__int64 value) {
    return FbxLongLong(value);
   }
};



%extend FbxSurfacePhong {
  static FbxSurfacePhong *Cast(FbxObject *base) {
    return FbxCast<FbxSurfacePhong>(base);
  }
};


%extend FbxSurfaceLambert {
  static FbxSurfaceLambert *Cast(FbxObject *base) {
    return FbxCast<FbxSurfaceLambert>(base);
  }
};


%extend FbxMesh {
  static FbxMesh *Cast(FbxObject *base) {
    return FbxCast<FbxMesh>(base);
  }
};


%extend FbxAnimCurve {
  static FbxAnimCurve *Cast(FbxObject *base) {
    return FbxCast<FbxAnimCurve>(base);
  }
};

%extend FbxAnimCurveNode {
  static FbxAnimCurveNode *Cast(FbxObject *base) {
    return FbxCast<FbxAnimCurveNode>(base);
  }
};

%extend FbxAnimLayer {
  static FbxAnimLayer *Cast(FbxObject *base) {
    return FbxCast<FbxAnimLayer>(base);
  }
};

%extend FbxAnimStack {
  static FbxAnimStack *Cast(FbxObject *base) {
    return FbxCast<FbxAnimStack>(base);
  }
};

%extend FbxCharacter {
  static FbxCharacter *Cast(FbxObject *base) {
    return FbxCast<FbxCharacter>(base);
  }
};

%extend FbxBlendShape {
  static FbxBlendShape *Cast(FbxObject *base) {
    return FbxCast<FbxBlendShape>(base);
  }
};

%extend FbxSkin {
  static FbxSkin *Cast(FbxObject *base) {
    return FbxCast<FbxSkin>(base);
  }
};

%extend FbxVertexCacheDeformer {
  static FbxVertexCacheDeformer *Cast(FbxObject *base) {
    return FbxCast<FbxVertexCacheDeformer>(base);
  }
};

//-----------------------------------------------------------------------