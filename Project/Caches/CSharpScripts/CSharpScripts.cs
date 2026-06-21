#if DEPLOY
namespace Scripts {
// Auto-generated file. This source file is used to compile for Android, UWP, it is included into a Project csproj.
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NeoAxis;
using NeoAxis.Editor;
using Project;

[CSharpScriptGeneratedAttribute("aW50IE1ldGhvZCggaW50IGEsIGludCBiICkKewoJcmV0dXJuIGEgKyBiOwp9Cg==")]
public class DynamicClassEFE66A74484991C50F6D2BF75AD19B08A7F2F3AB36497CEFF9B0405E15C4EB2C
{
    public NeoAxis.CSharpScript Owner;
    int Method(int a, int b)
    {
        return a + b;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uM0Qgc2VuZGVyLCBOZW9BeGlzLkNvbXBvbmVudCBpbml0aWF0b3IpCnsKCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsKCgkvLyBHZXQgb2JqZWN0IHR5cGUuCgl2YXIgcmVzb3VyY2VOYW1lID0gQCJTYW1wbGVzXFN0YXJ0ZXIgQ29udGVudFxNb2RlbHNcU2NpLWZpIEJveFxTY2ktZmkgQm94Lm9iamVjdGluc3BhY2UiOwoJdmFyIGJveFR5cGUgPSBNZXRhZGF0YU1hbmFnZXIuR2V0VHlwZShyZXNvdXJjZU5hbWUpOwoJaWYoYm94VHlwZSA9PSBudWxsKQoJewoJCUxvZy5XYXJuaW5nKCJPYmplY3QgdHlwZSBpcyBudWxsLiIpOwoJCXJldHVybjsKCX0KCQoJLy8gQ3JlYXRlIHRoZSBvYmplY3Qgd2l0aG91dCBlbmFibGluZy4KCXZhciBib3ggPSAoTWVzaEluU3BhY2Upc2NlbmUuQ3JlYXRlQ29tcG9uZW50KGJveFR5cGUsIGVuYWJsZWQ6IGZhbHNlKTsKCS8vdmFyIG9iaiA9IHNjZW5lLkNyZWF0ZUNvbXBvbmVudDxNZXNoSW5TcGFjZT4oZW5hYmxlZDogZmFsc2UpOwoKCS8vIFNldCBpbml0aWFsIHBvc2l0aW9uLgoJdmFyIHJhbmRvbSA9IG5ldyBGYXN0UmFuZG9tKCk7Cglib3guVHJhbnNmb3JtID0gbmV3IFRyYW5zZm9ybSgKCQluZXcgVmVjdG9yMygyICsgcmFuZG9tLk5leHQoMC4wLCA0LjApLCA4ICsgcmFuZG9tLk5leHQoMC4wLCA0LjApLCAxMCArIHJhbmRvbS5OZXh0KDAuMCwgNC4wKSksIAoJCW5ldyBBbmdsZXMocmFuZG9tLk5leHQoMzYwLjApLCByYW5kb20uTmV4dCgzNjAuMCksIHJhbmRvbS5OZXh0KDM2MC4wKSkpOwoJCgkvLyBFbmFibGUgdGhlIG9iamVjdCBpbiB0aGUgc2NlbmUuCglib3guRW5hYmxlZCA9IHRydWU7CgoJLy92YXIgbGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRpcmVjdGlvbmFsIExpZ2h0IikgYXMgTGlnaHQ7CgkvL2lmIChsaWdodCAhPSBudWxsKQoJLy8JbGlnaHQuRW5hYmxlZCA9IHNlbmRlci5BY3RpdmF0ZWQ7Cn0K")]
public class DynamicClass7E33F622903D7B6673A02988874575736A1393B82DFCB063ED638A8999B4E5BA
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button3D sender, NeoAxis.Component initiator)
    {
        var scene = sender.ParentScene;
        // Get object type.
        var resourceName = @"Samples\Starter Content\Models\Sci-fi Box\Sci-fi Box.objectinspace";
        var boxType = MetadataManager.GetType(resourceName);
        if (boxType == null)
        {
            Log.Warning("Object type is null.");
            return;
        }

        // Create the object without enabling.
        var box = (MeshInSpace)scene.CreateComponent(boxType, enabled: false);
        //var obj = scene.CreateComponent<MeshInSpace>(enabled: false);
        // Set initial position.
        var random = new FastRandom();
        box.Transform = new Transform(new Vector3(2 + random.Next(0.0, 4.0), 8 + random.Next(0.0, 4.0), 10 + random.Next(0.0, 4.0)), new Angles(random.Next(360.0), random.Next(360.0), random.Next(360.0)));
        // Enable the object in the scene.
        box.Enabled = true;
    //var light = scene.GetComponent("Directional Light") as Light;
    //if (light != null)
    //	light.Enabled = sender.Activated;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLlJlZ3VsYXRvciBvYmopCnsKCXZhciBzY2VuZSA9IG9iai5QYXJlbnRTY2VuZTsKCgkvL2NoYW5nZSB0aGUgY29sb3Igb2YgdGhlIGxpZ2h0Cgl2YXIgbGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRpcmVjdGlvbmFsIExpZ2h0IikgYXMgTGlnaHQ7CglpZiAobGlnaHQgIT0gbnVsbCkKCQlsaWdodC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEuMCwgMS4wLCAxLjAgLSBvYmouVmFsdWUpOwp9Cg==")]
public class DynamicClass97B85F942DA77D0B1363D6B41E7EEFC5763C86E0AE9D7210D5E97BD857E23A53
{
    public NeoAxis.CSharpScript Owner;
    public void Regulator_ValueChanged(NeoAxis.Regulator obj)
    {
        var scene = obj.ParentScene;
        //change the color of the light
        var light = scene.GetComponent("Directional Light") as Light;
        if (light != null)
            light.Color = new ColorValue(1.0, 1.0, 1.0 - obj.Value);
    }
}
}
#endif