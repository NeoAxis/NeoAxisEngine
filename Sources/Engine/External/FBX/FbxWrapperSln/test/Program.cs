using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Fbx;

namespace test
{

    class Log
    {
        public static void Info(object s)
        {
            Console.WriteLine(s);
        }

        public static void Error(object s)
        {
            Console.WriteLine(s);
        }
    }


    class Program
    {

        void PrintDoubleArray( DoubleArray array, int count)
        {
            if (count == 4)
            {
                string s = string.Format("({0} {1} {2} {3})",
                    array.getitem(0),
                    array.getitem(1),
                    array.getitem(2),
                    array.getitem(3));
                Log.Info(s);
            }
        }


        void PrintAttribute( FbxManager manager, FbxNodeAttribute attri)
        {
           FbxLayerElementVertexColor pp;
           

            Log.Info("attri " + attri.GetName());
            Log.Info("attri type " + Enum.GetName(typeof(FbxNodeAttribute.EType), attri.GetAttributeType()));
            //attri.is

            if (attri.GetAttributeType() == FbxNodeAttribute.EType.eMesh)
            {

                Type t = attri.GetType();
                Log.Info("type name " + t.Name);

                FbxMesh mesh = attri.GetNode().GetMesh();
	            

                //FbxMesh mesh = attri as FbxMesh;
                if (mesh == null)
                {
                    Log.Error("convert mesh failed!");
                    return;
                }


                Console.WriteLine($"mesh.GetMeshEdgeCount() = {mesh.GetMeshEdgeCount()}; mesh.GetPolygonCount() = {mesh.GetPolygonCount()}; mesh.GetPolygonVertexCount()={mesh.GetPolygonVertexCount()}; " +
                                  $"mesh.GetTextureUVCount() {mesh.GetTextureUVCount()}; mesh.GetControlPointsCount()={mesh.GetControlPointsCount()}; mesh.GetElementTangentCount()={mesh.GetElementTangentCount()};" +
                                  $" mesh.GetElementNormalCount()={mesh.GetElementNormalCount()}; mesh.GetElementVertexColorCount()={mesh.GetElementVertexColorCount()};" +
                                  $"mesh.GetUVLayerCount() = {mesh.GetUVLayerCount()}; mesh.GetLayerCount() = {mesh.GetLayerCount()}");
	            var pts = mesh.GetControlPoints();
                var ar = FbxVector4Array.frompointer(pts);

                for (int i = 0; i < mesh.GetLayerCount(); i++)
                {
                    var layer = mesh.GetLayer(i);
                }

	            try
	            {
		            var v2 = new FbxVector2();

	                IntPtr mem = Marshal.AllocHGlobal(4);
                    
                    FbxStringList lst = new FbxStringList();
	                //int nameCount = lst.GetCount();
                    mesh.GetUVSetNames(lst); //ToDo : что за List, расширяется ли он сам?
	                var name = lst.GetItemAt(0).mString.Buffer();

					//var myBool = new _MyBool(mem);
					//var res = mesh.GetPolygonVertexUV(0, 0, name, v2, myBool);
	    //            var c0 = v2.at(0);
	    //            var c2 = v2.at(1);

					var fbxArV2 = new FbxArrayFbxVector2();
		            var fbxArI = new FbxArrayInt();
					var res = mesh.GetPolygonVertexUVs(name, fbxArV2, fbxArI);
		            var ptr = fbxArV2.GetAt(2).mData;
		            double coord1= FbxWrapperNative.DoubleArrayFunc_getitem(ptr, 0);
	                double coord2 = FbxWrapperNative.DoubleArrayFunc_getitem(ptr, 1);
		            var param = FbxWrapperNative.new_FbxLayerElementArrayTemplateIntPtrFunc();
		            res= mesh.GetMaterialIndices(param);
		            var param2 = FbxWrapperNative.FbxLayerElementArrayTemplateIntPtrFunc_value(param);
		            int count = param2.GetCount();

					List<int> mind = new List<int>();
		            for (int i = 0; i < count; i++)
		            {
			            mind.Add(param2.GetAt(i));
		            }



					//var vec = new FbxVector4Array(5);
					//var res2 = mesh.GetPolygonVertexUVs("", , null);

		            bool res1 = mesh.GenerateTangentsData(0);
					//bool res2 = mesh.GenerateTangentsDataForAllUVSets( );
					var tCount = mesh.GetElementTangentCount();
					var tang = mesh.GetElementTangent(  );
					var tangAr = tang.GetDirectArray();
					int tC = tangAr.GetCount();

					//int binCount = mesh.GetElementBinormalCount();
					//var bin = mesh.GetElementBinormal().GetDirectArray().GetCount();
				}
	            catch (Exception ex)
	            {

	            }


				//var vertices =  mesh.GetPolygonVertices();

				//FbxMesh mesh;
				//FbxLayerElementUV uv = mesh.GetElementUV();

				//uv.GetDirectArray()


				FbxLayerElementNormal normal = mesh.GetElementNormal();

                //ToDo 

                //DirectArrayFbxVector4 array = normal.GetDirectArray();
                var array = normal.GetDirectArray();

                Log.Info("normal count " + array.GetCount());
                //for (int i = 0; i < array.GetCount(); i++)
                //{
                //    FbxVector4 v = array.GetAt(i);
                //    SWIGTYPE_p_double data = v.mData;
                //    DoubleArray d = DoubleArray.frompointer(data);
                //    PrintDoubleArray(d, 4);
                //}
                
            }
            

        }


        void Run()
        {
	        

            FbxManager manager = FbxManager.Create();
            FbxIOSettings setting = FbxIOSettings.Create(manager, "IOSRoot");

            //fbxiosettingspath.h
            //PostProcessSteps.CalculateTangentSpace = #define EXP_TANGENTSPACE				EXP_GEOMETRY "|" IOSN_TANGENTS_BINORMALS
            //PostProcessSteps.JoinIdenticalVertices = #define IOSN_DXF_WELD_VERTICES           "WeldVertices"
            //PostProcessSteps.Triangulate = #define IOSN_TRIANGULATE                "Triangulate"
            //PostProcessSteps.RemoveComponent = 
            //PostProcessSteps.GenerateSmoothNormals = 
            //setting.AddProperty()
			setting.SetBoolProp("Import|AdvOptGrp|Dxf|WeldVertices", true);
	        setting.SetBoolProp("Triangulate", true);

			manager.SetIOSettings(setting);

            FbxImporter impoter = FbxImporter.Create(manager, "");

            bool status = impoter.Initialize(@"1.fbx", -1, setting);

            Log.Info(status);

            if (!status)
            {
                return;
            }

            FbxScene scene = FbxScene.Create(manager, "scene1");
            status = impoter.Import(scene);
            Log.Info(status);


            int numTrack = scene.GetSrcObjectCount(FbxCriteria.ObjectType(FbxAnimStack.ClassId));
            Log.Info("num stack " + numTrack);

            FbxObject obj = scene.GetSrcObject(FbxCriteria.ObjectType(FbxAnimStack.ClassId), 0);
            
            FbxAnimStack stack = FbxAnimStack.Cast(obj);
            if (stack == null)
            {
                Log.Error("can not get anim stack!");
                return;
            }

            FbxCriteria cri = FbxCriteria.ObjectTypeStrict(FbxAnimLayer.ClassId);
            int numLayer = stack.GetMemberCount(cri);
            Log.Info("anim layer count : " + numLayer);

             FbxAnimLayer layer = null;
            if (numLayer > 0)
            {
                FbxObject layerobj = stack.GetMember(cri, 0);
                layer = FbxAnimLayer.Cast(layerobj);
                if (layer == null)
                {
                    Log.Error("anim layer is null!");
                    return;
                }

                Log.Info("anim layer name " + layer.GetName());
            }
            
			
            Log.Info("node count " + scene.GetNodeCount());
            for (int i = 0; i < scene.GetNodeCount(); i++)
            {
                FbxNode node = scene.GetNode(i); 
                Log.Info("node " + i + " " + node.GetName() + " ChildCount:" + node.GetChildCount());

                //----------------
                //node.LclTranslation.IsAnimated
                //----------------
                 //ToDo : 
                
                if (node.LclTranslation.IsAnimated(layer))
                {
                    FbxAnimCurveNode curveNode = node.LclTranslation.GetCurveNode(layer);
                    if (curveNode == null)
                    {
                        Log.Error("curve node is null");                        
                    }
                    else
                    {
                        for (int c = 0; c < curveNode.GetCurveCount(0); c++)
                        {
                            FbxAnimCurve curve = curveNode.GetCurve(0, (uint)c);
                            if (curve != null)
                            {
                                Log.Info("curve " + curve.GetName());
                                Log.Info("key count " + curve.KeyGetCount());
                                FbxAnimCurveKey key = curve.KeyGet(0);
                                FbxTime t = key.GetTime();
                                Log.Info("key " + t.GetTimeString() + " value " + key.GetValue());
                            }
                        }
                    }
                }
                


                if (node.GetNodeAttribute() != null)
                {
                    Log.Info("got attribu");
                    FbxNodeAttribute att = node.GetNodeAttribute();
                    PrintAttribute(manager, att);
                }
                else
                {

                    Log.Info("att count " + node.GetNodeAttributeCount());
                    for (int j = 0; j < node.GetNodeAttributeCount(); j++)
                    {
                        FbxNodeAttribute att = node.GetNodeAttributeByIndex(j);
                        PrintAttribute(manager, att);
                    }
                }

                FbxVector4 rot = node.GetPostRotation(FbxNode.EPivotSet.eSourcePivot);
                FbxQuaternion q;

            }
        }

        static void Main(string[] args)
        {
            
            Program pro = new Program();
            pro.Run();
        }


    }
}
