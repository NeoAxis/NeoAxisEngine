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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJLy8gR2V0IG9iamVjdCB0eXBlLgoJdmFyIHJlc291cmNlTmFtZSA9IEAiU2FtcGxlc1xTdGFydGVyIENvbnRlbnRcTW9kZWxzXFNjaS1maSBCb3hcU2NpLWZpIEJveC5vYmplY3RpbnNwYWNlIjsKCXZhciBib3hUeXBlID0gTWV0YWRhdGFNYW5hZ2VyLkdldFR5cGUocmVzb3VyY2VOYW1lKTsKCWlmKGJveFR5cGUgPT0gbnVsbCkKCXsKCQlMb2cuV2FybmluZygiT2JqZWN0IHR5cGUgaXMgbnVsbC4iKTsKCQlyZXR1cm47Cgl9CgkKCS8vIENyZWF0ZSB0aGUgb2JqZWN0IHdpdGhvdXQgZW5hYmxpbmcuCgl2YXIgYm94ID0gKE1lc2hJblNwYWNlKXNjZW5lLkNyZWF0ZUNvbXBvbmVudChib3hUeXBlLCBlbmFibGVkOiBmYWxzZSk7CgkvL3ZhciBvYmogPSBzY2VuZS5DcmVhdGVDb21wb25lbnQ8TWVzaEluU3BhY2U+KGVuYWJsZWQ6IGZhbHNlKTsKCgkvLyBTZXQgaW5pdGlhbCBwb3NpdGlvbi4KCXZhciByYW5kb20gPSBuZXcgRmFzdFJhbmRvbSgpOwoJYm94LlRyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oCgkJbmV3IFZlY3RvcjMoMiArIHJhbmRvbS5OZXh0KDAuMCwgNC4wKSwgOCArIHJhbmRvbS5OZXh0KDAuMCwgNC4wKSwgMTAgKyByYW5kb20uTmV4dCgwLjAsIDQuMCkpLCAKCQluZXcgQW5nbGVzKHJhbmRvbS5OZXh0KDM2MC4wKSwgcmFuZG9tLk5leHQoMzYwLjApLCByYW5kb20uTmV4dCgzNjAuMCkpKTsKCQoJLy8gRW5hYmxlIHRoZSBvYmplY3QgaW4gdGhlIHNjZW5lLgoJYm94LkVuYWJsZWQgPSB0cnVlOwoKCS8vdmFyIGxpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJEaXJlY3Rpb25hbCBMaWdodCIpIGFzIExpZ2h0OwoJLy9pZiAobGlnaHQgIT0gbnVsbCkKCS8vCWxpZ2h0LkVuYWJsZWQgPSBzZW5kZXIuQWN0aXZhdGVkOwp9Cg==")]
public class DynamicClassA6CB3F8F5BB3E3467A75CB38BB8CB7A654F8F0F31F8AC8FC442680131C1B1731
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
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

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpCnsKCXZhciBhbmdsZSA9IEVuZ2luZUFwcC5FbmdpbmVUaW1lICogMC4zOwoJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIDIuNTsKCXZhciBsb29rVG8gPSBuZXcgVmVjdG9yMygxMS43Mzc0ODM5MTI0ODI3LCAtMC4wNTE3NzY3NTAzMjQzOSwgLTE1LjUwOTI3NTU4MjUwOTIpOwoJdmFyIGxvb2tBdCA9IFF1YXRlcm5pb24uTG9va0F0KC1vZmZzZXQsIG5ldyBWZWN0b3IzKDAsMCwxKSk7CgkKCXJldHVybiBuZXcgVHJhbnNmb3JtKCBsb29rVG8gKyBvZmZzZXQsIGxvb2tBdCwgVmVjdG9yMy5PbmUgKTsKfQo=")]
public class DynamicClass38E1E68F4590ADDA8A2E2D87A752F325B659F81F3159DB8B10100F7BB9E01C46
{
    public NeoAxis.CSharpScript Owner;
    Transform Method()
    {
        var angle = EngineApp.EngineTime * 0.3;
        var offset = new Vector3(Math.Cos(angle), Math.Sin(angle), 0) * 2.5;
        var lookTo = new Vector3(11.7374839124827, -0.05177675032439, -15.5092755825092);
        var lookAt = Quaternion.LookAt(-offset, new Vector3(0, 0, 1));
        return new Transform(lookTo + offset, lookAt, Vector3.One);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29uc3RyYWludF9TaW11bGF0aW9uU3RlcChOZW9BeGlzLkNvbXBvbmVudCBvYmopCnsKCS8vZ2V0IG9iamVjdHMKCXZhciBjb25zdHJhaW50ID0gKENvbnN0cmFpbnRfU2l4RE9GKW9iajsKCXZhciBzY2VuZSA9IGNvbnN0cmFpbnQuUGFyZW50U2NlbmU7Cgl2YXIgc3BoZXJlID0gc2NlbmUuR2V0Q29tcG9uZW50KCJTcGhlcmUiKSBhcyBNZXNoSW5TcGFjZTsKCgkvL3VwZGF0ZSBtb3RvcgoJdmFyIGxvb2tUbyA9IHNwaGVyZS5UcmFuc2Zvcm1WLlBvc2l0aW9uOwoJdmFyIGxvb2tGcm9tID0gY29uc3RyYWludC5UcmFuc2Zvcm1WLlBvc2l0aW9uOwoJdmFyIGRpZmYgPSBsb29rVG8gLSBsb29rRnJvbTsKCWNvbnN0cmFpbnQuQW5ndWxhclpBeGlzTW90b3JUYXJnZXQgPSBNYXRoRXguUmFkaWFuVG9EZWdyZWUoTWF0aEV4LkF0YW4yKGRpZmYuWSwgZGlmZi5YKSk7Cn0K")]
public class DynamicClassD77247E301AC6FDD9339B8A807DBD90D022CBC44A3DD75A24F9F3D50A3B31F21
{
    public NeoAxis.CSharpScript Owner;
    public void Constraint_SimulationStep(NeoAxis.Component obj)
    {
        //get objects
        var constraint = (Constraint_SixDOF)obj;
        var scene = constraint.ParentScene;
        var sphere = scene.GetComponent("Sphere") as MeshInSpace;
        //update motor
        var lookTo = sphere.TransformV.Position;
        var lookFrom = constraint.TransformV.Position;
        var diff = lookTo - lookFrom;
        constraint.AngularZAxisMotorTarget = MathEx.RadianToDegree(MathEx.Atan2(diff.Y, diff.X));
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQm94MTBfVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIG9iaiA9IChPYmplY3RJblNwYWNlKXNlbmRlcjsKCgl2YXIgdHIgPSBvYmouVHJhbnNmb3JtVjsKCXZhciBuZXdUciA9IHRyLlVwZGF0ZVJvdGF0aW9uKFF1YXRlcm5pb24uRnJvbVJvdGF0ZUJ5WihFbmdpbmVBcHAuRW5naW5lVGltZSAvIDIpKTsKCW9iai5UcmFuc2Zvcm0gPSBuZXdUcjsKfQo=")]
public class DynamicClass0E8E609371B70770BC55E73FC09A837304ABB7E8A508B15C379AAC9E8EAD4B72
{
    public NeoAxis.CSharpScript Owner;
    public void Box10_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var obj = (ObjectInSpace)sender;
        var tr = obj.TransformV;
        var newTr = tr.UpdateRotation(Quaternion.FromRotateByZ(EngineApp.EngineTime / 2));
        obj.Transform = newTr;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpCnsKCXRyeQoJewoJCXZhciBhbGwgPSBUaW1lLkN1cnJlbnQgJSA4ID4gNC4wOwoJCQoJCWZvcmVhY2godmFyIHNlbnNvciBpbiBzZW5kZXIuR2V0Q29tcG9uZW50czxTZW5zb3I+KCkpCgkJewoJCQlzZW5zb3IuTW9kZSA9IGFsbCA_IFNlbnNvci5Nb2RlRW51bS5BbGxPYmplY3RzIDogU2Vuc29yLk1vZGVFbnVtLk9uZUNsb3Nlc3RPYmplY3Q7CgkJfQoJfQoJY2F0Y2goRXhjZXB0aW9uIGUpCgl7CgkJTG9nLldhcm5pbmcoZS5NZXNzYWdlKTsKCX0JCn0K")]
public class DynamicClass3A57E5338BE6077103B8811D0EB5BCD2DB48A32E3A3C110C368ACD9486FAB7AA
{
    public NeoAxis.CSharpScript Owner;
    public void _UpdateEvent(NeoAxis.Component sender, float delta)
    {
        try
        {
            var all = Time.Current % 8 > 4.0;
            foreach (var sensor in sender.GetComponents<Sensor>())
            {
                sensor.Mode = all ? Sensor.ModeEnum.AllObjects : Sensor.ModeEnum.OneClosestObject;
            }
        }
        catch (Exception e)
        {
            Log.Warning(e.Message);
        }
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpCnsKCXZhciBhbmdsZSA9IEVuZ2luZUFwcC5FbmdpbmVUaW1lICogLTEuMDsKCXZhciBvZmZzZXQgPSBuZXcgVmVjdG9yMyhNYXRoLkNvcyhhbmdsZSksIE1hdGguU2luKGFuZ2xlKSwgMCkgKiAyLjA7Cgl2YXIgbG9va1RvID0gbmV3IFZlY3RvcjMoMTEuNzM3NDgzOTEyNDgyNywgLTAuMDUxNzc2NzUwMzI0MzksIC0xNC44MDkyNzU1ODI1MDkyKTsKCXZhciBsb29rQXQgPSBRdWF0ZXJuaW9uLkxvb2tBdCgtb2Zmc2V0LCBuZXcgVmVjdG9yMygwLDAsMSkpOwoJCglyZXR1cm4gbmV3IFRyYW5zZm9ybSggbG9va1RvICsgb2Zmc2V0LCBsb29rQXQsIFZlY3RvcjMuT25lICk7Cn0K")]
public class DynamicClass18A7B3363998B96C0E6D19CE157D3F0E05EA2CD7494616F26C28412469E21318
{
    public NeoAxis.CSharpScript Owner;
    Transform Method()
    {
        var angle = EngineApp.EngineTime * -1.0;
        var offset = new Vector3(Math.Cos(angle), Math.Sin(angle), 0) * 2.0;
        var lookTo = new Vector3(11.7374839124827, -0.05177675032439, -14.8092755825092);
        var lookAt = Quaternion.LookAt(-offset, new Vector3(0, 0, 1));
        return new Transform(lookTo + offset, lookAt, Vector3.One);
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpCnsKCXZhciBhbmdsZSA9IEVuZ2luZUFwcC5FbmdpbmVUaW1lICogMS4zOwoJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIDIuMDsKCXZhciBsb29rVG8gPSBuZXcgVmVjdG9yMygxMS43Mzc0ODM5MTI0ODI3LCAtMC4wNTE3NzY3NTAzMjQzOSwgLTE1LjUwOTI3NTU4MjUwOTIpOwoJdmFyIGxvb2tBdCA9IFF1YXRlcm5pb24uTG9va0F0KC1vZmZzZXQsIG5ldyBWZWN0b3IzKDAsMCwxKSk7CgkKCXJldHVybiBuZXcgVHJhbnNmb3JtKCBsb29rVG8gKyBvZmZzZXQsIGxvb2tBdCwgbmV3IFZlY3RvcjMoMC41LDAuNSwwLjUpICk7Cn0K")]
public class DynamicClass4E5C224DE2D6FE23DDD4DAFC8B01F3FBD396357061CC49E0F8B9445FCDE75B0F
{
    public NeoAxis.CSharpScript Owner;
    Transform Method()
    {
        var angle = EngineApp.EngineTime * 1.3;
        var offset = new Vector3(Math.Cos(angle), Math.Sin(angle), 0) * 2.0;
        var lookTo = new Vector3(11.7374839124827, -0.05177675032439, -15.5092755825092);
        var lookAt = Quaternion.LookAt(-offset, new Vector3(0, 0, 1));
        return new Transform(lookTo + offset, lookAt, new Vector3(0.5, 0.5, 0.5));
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpCnsKCXZhciBhbmdsZSA9IEVuZ2luZUFwcC5FbmdpbmVUaW1lICogNTsKCXZhciByb3RhdGlvbiA9IFF1YXRlcm5pb24uRnJvbVJvdGF0ZUJ5WihhbmdsZSk7CgoJcmV0dXJuIG5ldyBUcmFuc2Zvcm0obmV3IFZlY3RvcjMoMTEuNzM3NCwgLTAuMDUxNywgLTE2LjMwNzEpLCByb3RhdGlvbiwgbmV3IFZlY3RvcjMoMC40LCAwLjQsIDAuNCkpOwp9Cg==")]
public class DynamicClass59F5160527C220D61121765812535F715E795B3A07FE6A450DEEFCC0746F11D5
{
    public NeoAxis.CSharpScript Owner;
    Transform Method()
    {
        var angle = EngineApp.EngineTime * 5;
        var rotation = Quaternion.FromRotateByZ(angle);
        return new Transform(new Vector3(11.7374, -0.0517, -16.3071), rotation, new Vector3(0.4, 0.4, 0.4));
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJdmFyIGdyb3VuZCA9IHNjZW5lLkdldENvbXBvbmVudCgiR3JvdW5kIikgYXMgTWVzaEluU3BhY2U7CglpZiAoZ3JvdW5kICE9IG51bGwpCgl7CgkJaWYgKCFncm91bmQuUmVwbGFjZU1hdGVyaWFsLlJlZmVyZW5jZVNwZWNpZmllZCkKCQl7CgkJCWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwgPSBSZWZlcmVuY2VVdGlsaXR5Lk1ha2VSZWZlcmVuY2UoCgkJCQlAIkNvbnRlbnRcTWF0ZXJpYWxzXEJhc2ljIExpYnJhcnlcQ29uY3JldGVcQ29uY3JldGUgRmxvb3IgMDEubWF0ZXJpYWwiKTsKCQl9CgkJZWxzZQoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gbnVsbDsKCX0KfQo=")]
public class DynamicClass3F47AACB27EFFC479563EF6BC7B522B17CA3D413549CA5BD70ADA132BAF3E8CA
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
    {
        var scene = sender.ParentScene;
        var ground = scene.GetComponent("Ground") as MeshInSpace;
        if (ground != null)
        {
            if (!ground.ReplaceMaterial.ReferenceSpecified)
            {
                ground.ReplaceMaterial = ReferenceUtility.MakeReference(@"Content\Materials\Basic Library\Concrete\Concrete Floor 01.material");
            }
            else
                ground.ReplaceMaterial = null;
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLlJlZ3VsYXRvciBvYmopCnsKCXZhciBzY2VuZSA9IG9iai5QYXJlbnRTY2VuZTsKCgl2YXIgZ3JvdW5kID0gc2NlbmUuR2V0Q29tcG9uZW50KCJHcm91bmQiKSBhcyBNZXNoSW5TcGFjZTsKCWlmIChncm91bmQgIT0gbnVsbCkKCQlncm91bmQuQ29sb3IgPSBDb2xvclZhbHVlLkxlcnAobmV3IENvbG9yVmFsdWUoMSwgMSwgMSksIG5ldyBDb2xvclZhbHVlKDAuNCwgMC45LCAwLjQpLCAoZmxvYXQpb2JqLlZhbHVlKTsKfQo=")]
public class DynamicClass085FF268F2DD264A4CB763825B8A15600BAAE0084F983C5707B0948290195724
{
    public NeoAxis.CSharpScript Owner;
    public void Regulator_ValueChanged(NeoAxis.Regulator obj)
    {
        var scene = obj.ParentScene;
        var ground = scene.GetComponent("Ground") as MeshInSpace;
        if (ground != null)
            ground.Color = ColorValue.Lerp(new ColorValue(1, 1, 1), new ColorValue(0.4, 0.9, 0.4), (float)obj.Value);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUGFpbnRMYXllcl9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQp7Cgl2YXIgbGF5ZXIgPSAoUGFpbnRMYXllcilzZW5kZXI7CglsYXllci5NYXRlcmlhbENvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMSwgMSwgTWF0aEV4LlNpbihUaW1lLkN1cnJlbnQpICogMC41ICsgMC41KTsKfQo=")]
public class DynamicClass87CB81277738E3204CECEEF6374F3AD27C4171EA1B50DAD28C3F381575BB5F23
{
    public NeoAxis.CSharpScript Owner;
    public void PaintLayer_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var layer = (PaintLayer)sender;
        layer.MaterialColor = new ColorValue(1, 1, 1, MathEx.Sin(Time.Current) * 0.5 + 0.5);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUGFpbnRMYXllcl9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQp7Cgl2YXIgbGF5ZXIgPSAoUGFpbnRMYXllcilzZW5kZXI7CglsYXllci5NYXRlcmlhbENvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMSwgMSwgTWF0aEV4LkNvcyhUaW1lLkN1cnJlbnQpICogMC41ICsgMC41KTsKfQo=")]
public class DynamicClass0CEA82422398667C47366B923DF74C9208964575BB36AE3031D0BF5B96FCC8FE
{
    public NeoAxis.CSharpScript Owner;
    public void PaintLayer_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var layer = (PaintLayer)sender;
        layer.MaterialColor = new ColorValue(1, 1, 1, MathEx.Cos(Time.Current) * 0.5 + 0.5);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgU0ZDcmF0ZTFfVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIG1lc2hJblNwYWNlID0gKE1lc2hJblNwYWNlKXNlbmRlcjsKCQoJLy91c2UgbWVzaCBvZiB0aGUgY29tcG9uZW50Cgl2YXIgbWVzaCA9IG1lc2hJblNwYWNlLk1lc2guVmFsdWU7CgkKCXZhciBkaXN0YW5jZVN0ZXBzID0gNTsKCXZhciBhbmdsZVN0ZXBzID0gODsKCgl2YXIgYWRkaXRpb25hbEl0ZW1zID0gbmV3IE1lc2hJblNwYWNlLkFkZGl0aW9uYWxJdGVtW2Rpc3RhbmNlU3RlcHMgKiBhbmdsZVN0ZXBzXTsKCglpbnQgY3VycmVudEluZGV4ID0gMDsKCglmb3IgKGludCBkaXN0YW5jZVN0ZXAgPSAwOyBkaXN0YW5jZVN0ZXAgPCBkaXN0YW5jZVN0ZXBzOyBkaXN0YW5jZVN0ZXArKykKCXsKCQl2YXIgZGlzdGFuY2UgPSAoZG91YmxlKWRpc3RhbmNlU3RlcCAqIDEuMGY7CgoJCWZvciAoaW50IGFuZ2xlU3RlcCA9IDA7IGFuZ2xlU3RlcCA8IGFuZ2xlU3RlcHM7IGFuZ2xlU3RlcCsrKQoJCXsKCQkJdmFyIGFuZ2xlID0gTWF0aC5QSSAqIDIgKiBhbmdsZVN0ZXAgLyBhbmdsZVN0ZXBzICsgVGltZS5DdXJyZW50ICogMC41OwoKCQkJdmFyIHBvcyA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIGRpc3RhbmNlOwoJCQl2YXIgcm90ID0gUXVhdGVybmlvbi5JZGVudGl0eTsKCQkJdmFyIHNjbCA9IG5ldyBWZWN0b3IzKDAuMyAvIGRpc3RhbmNlLCAwLjMgLyBkaXN0YW5jZSwgMC4zIC8gZGlzdGFuY2UpOwoKCQkJYWRkaXRpb25hbEl0ZW1zW2N1cnJlbnRJbmRleF0gPSBuZXcgTWVzaEluU3BhY2UuQWRkaXRpb25hbEl0ZW0obWVzaCwgcG9zLCByb3QsIHNjbCwgbmV3IENvbG9yVmFsdWUoMSwgMSwgMCkpOwoJCQljdXJyZW50SW5kZXgrKzsKCQl9Cgl9CgoJLyoKCXZhciBhZGRpdGlvbmFsSXRlbXMgPSBuZXcgTWVzaEluU3BhY2UuQWRkaXRpb25hbEl0ZW1bMV07CglyZWYgdmFyIGl0ZW0gPSByZWYgYWRkaXRpb25hbEl0ZW1zWzBdOwoJaXRlbS5NZXNoID0gbWVzaDsKCWl0ZW0uUG9zaXRpb24gPSBuZXcgVmVjdG9yMygxLCAwLCAwKTsKCWl0ZW0uUm90YXRpb24gPSBRdWF0ZXJuaW9uLklkZW50aXR5OzsKCWl0ZW0uU2NhbGUgPSBuZXcgVmVjdG9yMyguMiwgLjIsIC4yKTsKCWl0ZW0uQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAxLCAwKTsKCSovCgoJbWVzaEluU3BhY2UuQWRkaXRpb25hbEl0ZW1zID0gYWRkaXRpb25hbEl0ZW1zOwoKCgkvL3VwZGF0ZSBib3VuZHMgd2hlbiBuZWVkIHVwZGF0ZQoJewoJCXZhciB0ciA9IG1lc2hJblNwYWNlLlRyYW5zZm9ybVY7CgkJdmFyIGJvdW5kcyA9IG5ldyBCb3VuZHMoLTQsIC00LCAtMC41LCA0LCA0LCAwLjUpOwoJCXZhciBib3ggPSBuZXcgQm94KGJvdW5kcywgdHIuUG9zaXRpb24sIHRyLlJvdGF0aW9uLlRvTWF0cml4MygpKTsKCQltZXNoSW5TcGFjZS5TcGFjZUJvdW5kc092ZXJyaWRlID0gbmV3IFNwYWNlQm91bmRzKGJveC5Ub0JvdW5kcygpKTsKCQltZXNoSW5TcGFjZS5TcGFjZUJvdW5kc1VwZGF0ZSgpOwoJfQp9Cg==")]
public class DynamicClass4006E112E81EFCB3154A5D2D5F374B8CBC164255996A9CB6E609B1805712BE55
{
    public NeoAxis.CSharpScript Owner;
    public void SFCrate1_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var meshInSpace = (MeshInSpace)sender;
        //use mesh of the component
        var mesh = meshInSpace.Mesh.Value;
        var distanceSteps = 5;
        var angleSteps = 8;
        var additionalItems = new MeshInSpace.AdditionalItem[distanceSteps * angleSteps];
        int currentIndex = 0;
        for (int distanceStep = 0; distanceStep < distanceSteps; distanceStep++)
        {
            var distance = (double)distanceStep * 1.0f;
            for (int angleStep = 0; angleStep < angleSteps; angleStep++)
            {
                var angle = Math.PI * 2 * angleStep / angleSteps + Time.Current * 0.5;
                var pos = new Vector3(Math.Cos(angle), Math.Sin(angle), 0) * distance;
                var rot = Quaternion.Identity;
                var scl = new Vector3(0.3 / distance, 0.3 / distance, 0.3 / distance);
                additionalItems[currentIndex] = new MeshInSpace.AdditionalItem(mesh, pos, rot, scl, new ColorValue(1, 1, 0));
                currentIndex++;
            }
        }

        /*
	var additionalItems = new MeshInSpace.AdditionalItem[1];
	ref var item = ref additionalItems[0];
	item.Mesh = mesh;
	item.Position = new Vector3(1, 0, 0);
	item.Rotation = Quaternion.Identity;;
	item.Scale = new Vector3(.2, .2, .2);
	item.Color = new ColorValue(1, 1, 0);
	*/
        meshInSpace.AdditionalItems = additionalItems;
        //update bounds when need update
        {
            var tr = meshInSpace.TransformV;
            var bounds = new Bounds(-4, -4, -0.5, 4, 4, 0.5);
            var box = new Box(bounds, tr.Position, tr.Rotation.ToMatrix3());
            meshInSpace.SpaceBoundsOverride = new SpaceBounds(box.ToBounds());
            meshInSpace.SpaceBoundsUpdate();
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgTWVzaEluU3BhY2VBbmltYXRpb25Db250cm9sbGVyX0NhbGN1bGF0ZUJvbmVUcmFuc2Zvcm1zKE5lb0F4aXMuTWVzaEluU3BhY2VBbmltYXRpb25Db250cm9sbGVyIHNlbmRlciwgTmVvQXhpcy5Ta2VsZXRvbkFuaW1hdGlvblRyYWNrLkNhbGN1bGF0ZUJvbmVUcmFuc2Zvcm1zSXRlbVtdIHJlc3VsdCkKewoJLy90byBlbmFibGUgdGhpcyBldmVudCBoYW5kbGVyIGluIHRoZSBlZGl0b3IgY2hhbmdlICJXaGVuIEVuYWJsZSIgcHJvcGVydHkgdG8gIlNpbXVsYXRpb24gfCBJbnN0YW5jZSB8IEVkaXRvciIuCgkvL2NvbXBvbmVudDogQ2hhcmFjdGVyL01lc2ggSW4gU3BhY2UvQyMgU2NyaXB0L0V2ZW50IEhhbmRsZXIgQ2FsY3VsYXRlQm9uZVRyYW5zZm9ybXMuCgkKCXZhciBib25lSW5kZXggPSBzZW5kZXIuR2V0Qm9uZUluZGV4KCJtaXhhbW9yaWc6U3BpbmUxIik7CglpZihib25lSW5kZXggIT0gLTEpCgl7CgkJcmVmIHZhciBpdGVtID0gcmVmIHJlc3VsdFtib25lSW5kZXhdOwoKCQkvL2NhbGN1bGF0ZSBib25lIG9mZnNldAoJCXZhciBhbmdsZSA9IG5ldyBEZWdyZWUoNjApICogTWF0aC5TaW4oVGltZS5DdXJyZW50KTsgCgkJdmFyIG9mZnNldCA9IE1hdHJpeDNGLkZyb21Sb3RhdGVCeVkoKGZsb2F0KWFuZ2xlLkluUmFkaWFucygpKS5Ub1F1YXRlcm5pb24oKTsKCQkKCQkvL3VwZGF0ZSB0aGUgYm9uZQoJCWl0ZW0uUm90YXRpb24gKj0gb2Zmc2V0OwoJfQkKfQo=")]
public class DynamicClassD11D0BDF2CE301BB2E8F6F60961E3230562912D7DE50E1854F960982DD980C0E
{
    public NeoAxis.CSharpScript Owner;
    public void MeshInSpaceAnimationController_CalculateBoneTransforms(NeoAxis.MeshInSpaceAnimationController sender, NeoAxis.SkeletonAnimationTrack.CalculateBoneTransformsItem[] result)
    {
        //to enable this event handler in the editor change "When Enable" property to "Simulation | Instance | Editor".
        //component: Character/Mesh In Space/C# Script/Event Handler CalculateBoneTransforms.
        var boneIndex = sender.GetBoneIndex("mixamorig:Spine1");
        if (boneIndex != -1)
        {
            ref var item = ref result[boneIndex];
            //calculate bone offset
            var angle = new Degree(60) * Math.Sin(Time.Current);
            var offset = Matrix3F.FromRotateByY((float)angle.InRadians()).ToQuaternion();
            //update the bone
            item.Rotation *= offset;
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgTGlnaHQ0X1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpCnsKCXZhciBvYmogPSAoT2JqZWN0SW5TcGFjZSlzZW5kZXI7Cgl2YXIgdHIgPSBvYmouVHJhbnNmb3JtVjsKCW9iai5UcmFuc2Zvcm0gPSBuZXcgVHJhbnNmb3JtKHRyLlBvc2l0aW9uLCBRdWF0ZXJuaW9uLkZyb21Sb3RhdGVCeVooVGltZS5DdXJyZW50ICogMC4yNSksIHRyLlNjYWxlKTsKfQo=")]
public class DynamicClass97F08E2F9EE2F19AC07C149F6993F605552F1C8E62B48E815F37305A31606821
{
    public NeoAxis.CSharpScript Owner;
    public void Light4_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var obj = (ObjectInSpace)sender;
        var tr = obj.TransformV;
        obj.Transform = new Transform(tr.Position, Quaternion.FromRotateByZ(Time.Current * 0.25), tr.Scale);
    }
}

[CSharpScriptGeneratedAttribute("c3RhdGljIGJvb2wgbmVhckNhbWVyYTsKc3RhdGljIGJvb2wgYWRkaXRpb25hbExpZ2h0cyA9IHRydWU7CnN0YXRpYyBib29sIHNoYWRvd3MgPSB0cnVlOwoKcHVibGljIHZvaWQgR2FtZU1vZGVfUmVuZGVyVUkoTmVvQXhpcy5HYW1lTW9kZSBzZW5kZXIsIE5lb0F4aXMuQ2FudmFzUmVuZGVyZXIgcmVuZGVyZXIpCnsKCXZhciBsaW5lcyA9IG5ldyBMaXN0PHN0cmluZz4oKTsKCglsaW5lcy5BZGQoIkMgLSBzd2l0Y2ggY2FtZXJhIik7CglsaW5lcy5BZGQoIkwgLSBhZGRpdGlvbmFsIGxpZ2h0cyIpOwoJbGluZXMuQWRkKCJIIC0gc2hhZG93cyIpOwoJbGluZXMuQWRkKCIiKTsKCWxpbmVzLkFkZCgiRjcgLSBmcmVlIGNhbWVyYSIpOwoJbGluZXMuQWRkKCJXIEEgUyBEIFEgRSAtIGZyZWUgY2FtZXJhIGNvbnRyb2wiKTsKCWxpbmVzLkFkZCgiIik7CglsaW5lcy5BZGQoIllvdSBhbHNvIGNhbiBwbGF5IHdpdGggYW50aWFsaWFzaW5nIGFuZCBvdGhlciBzZXR0aW5ncyBmcm9tIE9wdGlvbnMgKEVzYykiKTsKCgl2YXIgZm9udFNpemUgPSByZW5kZXJlci5EZWZhdWx0Rm9udFNpemU7Cgl2YXIgb2Zmc2V0ID0gbmV3IFZlY3RvcjIoZm9udFNpemUgKiByZW5kZXJlci5Bc3BlY3RSYXRpb0ludiAqIDAuOCwgMC44KTsKCgkvL2RyYXcgYmFja2dyb3VuZAoJewoJCXZhciBtYXhMZW5ndGggPSAwLjA7CgkJZm9yZWFjaCAodmFyIGxpbmUgaW4gbGluZXMpCgkJewoJCQl2YXIgbGVuZ3RoID0gcmVuZGVyZXIuRGVmYXVsdEZvbnQuR2V0VGV4dExlbmd0aChmb250U2l6ZSwgcmVuZGVyZXIsIGxpbmUpOwoJCQlpZiAobGVuZ3RoID4gbWF4TGVuZ3RoKQoJCQkJbWF4TGVuZ3RoID0gbGVuZ3RoOwoJCX0KCQl2YXIgcmVjdCA9IG9mZnNldCArIG5ldyBSZWN0YW5nbGUoMCwgMCwgbWF4TGVuZ3RoLCBmb250U2l6ZSAqIGxpbmVzLkNvdW50KTsKCQlyZWN0LkV4cGFuZChuZXcgVmVjdG9yMihmb250U2l6ZSAqIDAuMiwgZm9udFNpemUgKiAwLjIgKiByZW5kZXJlci5Bc3BlY3RSYXRpbykpOwoJCXJlbmRlcmVyLkFkZFF1YWQocmVjdCwgbmV3IENvbG9yVmFsdWUoMCwgMCwgMCwgMC43NSkpOwoJfQoKCS8vZHJhdyB0ZXh0IAoJQ2FudmFzUmVuZGVyZXJVdGlsaXR5LkFkZFRleHRMaW5lc1dpdGhTaGFkb3cocmVuZGVyZXIuVmlld3BvcnRGb3JTY3JlZW5DYW52YXNSZW5kZXJlciwgcmVuZGVyZXIuRGVmYXVsdEZvbnQsIHJlbmRlcmVyLkRlZmF1bHRGb250U2l6ZSwgbGluZXMsIG5ldyBSZWN0YW5nbGUob2Zmc2V0LlgsIG9mZnNldC5ZLCAxLCAxKSwgRUhvcml6b250YWxBbGlnbm1lbnQuTGVmdCwgRVZlcnRpY2FsQWxpZ25tZW50LlRvcCwgbmV3IENvbG9yVmFsdWUoMSwgMSwgMSkpOwp9CgpwdWJsaWMgdm9pZCBHYW1lTW9kZV9JbnB1dE1lc3NhZ2VFdmVudChOZW9BeGlzLkdhbWVNb2RlIHNlbmRlciwgTmVvQXhpcy5JbnB1dE1lc3NhZ2UgbWVzc2FnZSkKewoJdmFyIGtleURvd24gPSBtZXNzYWdlIGFzIElucHV0TWVzc2FnZUtleURvd247CglpZiAoa2V5RG93biAhPSBudWxsKQoJewoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5DKQoJCXsKCQkJLy91cGRhdGUgY2FtZXJhIHNldHRpbmdzCgkJCW5lYXJDYW1lcmEgPSAhbmVhckNhbWVyYTsKCgkJCS8vdXBkYXRlIGNhbWVyYQoJCQl2YXIgc2NlbmUgPSBzZW5kZXIuRmluZFBhcmVudDxTY2VuZT4oKTsKCQkJc2NlbmUuQ2FtZXJhRGVmYXVsdCA9IHNjZW5lLkdldENvbXBvbmVudDxDYW1lcmE+KG5lYXJDYW1lcmEgPyAiQ2FtZXJhIE5lYXIiIDogIkNhbWVyYSBGYXIiKTsKCQkJU2ltdWxhdGlvbkFwcC5NYWluVmlld3BvcnQuTm90aWZ5SW5zdGFudENhbWVyYU1vdmVtZW50KCk7CgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoKCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuTCkKCQl7CgkJCWFkZGl0aW9uYWxMaWdodHMgPSAhYWRkaXRpb25hbExpZ2h0czsKCgkJCXZhciBzY2VuZSA9IHNlbmRlci5GaW5kUGFyZW50PFNjZW5lPigpOwoKCQkJZm9yZWFjaCAodmFyIGxpZ2h0IGluIHNjZW5lLkdldENvbXBvbmVudHM8TGlnaHQ+KCkpCgkJCXsKCQkJCWlmIChsaWdodC5OYW1lID09ICJMaWdodCIgfHwgbGlnaHQuTmFtZS5Db250YWlucygiTGlnaHQgIikpCgkJCQkJbGlnaHQuRW5hYmxlZCA9IGFkZGl0aW9uYWxMaWdodHM7CgkJCX0KCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5IKQoJCXsKCQkJc2hhZG93cyA9ICFzaGFkb3dzOwoKCQkJdmFyIHNjZW5lID0gc2VuZGVyLkZpbmRQYXJlbnQ8U2NlbmU+KCk7CgkJCXZhciBwaXBlbGluZSA9IHNjZW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdQaXBlbGluZV9CYXNpYz4oKTsKCQkJcGlwZWxpbmUuU2hhZG93cyA9IHNoYWRvd3M7CgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJfQp9Cg==")]
public class DynamicClass690C20DD109F42C98C8FA48795CB9D0235480800766425980530B7CF0486D3C0
{
    public NeoAxis.CSharpScript Owner;
    static bool nearCamera;
    static bool additionalLights = true;
    static bool shadows = true;
    public void GameMode_RenderUI(NeoAxis.GameMode sender, NeoAxis.CanvasRenderer renderer)
    {
        var lines = new List<string>();
        lines.Add("C - switch camera");
        lines.Add("L - additional lights");
        lines.Add("H - shadows");
        lines.Add("");
        lines.Add("F7 - free camera");
        lines.Add("W A S D Q E - free camera control");
        lines.Add("");
        lines.Add("You also can play with antialiasing and other settings from Options (Esc)");
        var fontSize = renderer.DefaultFontSize;
        var offset = new Vector2(fontSize * renderer.AspectRatioInv * 0.8, 0.8);
        //draw background
        {
            var maxLength = 0.0;
            foreach (var line in lines)
            {
                var length = renderer.DefaultFont.GetTextLength(fontSize, renderer, line);
                if (length > maxLength)
                    maxLength = length;
            }

            var rect = offset + new Rectangle(0, 0, maxLength, fontSize * lines.Count);
            rect.Expand(new Vector2(fontSize * 0.2, fontSize * 0.2 * renderer.AspectRatio));
            renderer.AddQuad(rect, new ColorValue(0, 0, 0, 0.75));
        }

        //draw text 
        CanvasRendererUtility.AddTextLinesWithShadow(renderer.ViewportForScreenCanvasRenderer, renderer.DefaultFont, renderer.DefaultFontSize, lines, new Rectangle(offset.X, offset.Y, 1, 1), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue(1, 1, 1));
    }

    public void GameMode_InputMessageEvent(NeoAxis.GameMode sender, NeoAxis.InputMessage message)
    {
        var keyDown = message as InputMessageKeyDown;
        if (keyDown != null)
        {
            if (keyDown.Key == EKeys.C)
            {
                //update camera settings
                nearCamera = !nearCamera;
                //update camera
                var scene = sender.FindParent<Scene>();
                scene.CameraDefault = scene.GetComponent<Camera>(nearCamera ? "Camera Near" : "Camera Far");
                SimulationApp.MainViewport.NotifyInstantCameraMovement();
                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.L)
            {
                additionalLights = !additionalLights;
                var scene = sender.FindParent<Scene>();
                foreach (var light in scene.GetComponents<Light>())
                {
                    if (light.Name == "Light" || light.Name.Contains("Light "))
                        light.Enabled = additionalLights;
                }

                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.H)
            {
                shadows = !shadows;
                var scene = sender.FindParent<Scene>();
                var pipeline = scene.GetComponent<RenderingPipeline_Basic>();
                pipeline.Shadows = shadows;
                message.Handled = true;
                return;
            }
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX1NpbXVsYXRpb25TdGVwKE5lb0F4aXMuQ29tcG9uZW50IG9iaikKewoJZm9yZWFjaCAodmFyIGxpZ2h0IGluIG9iai5HZXRDb21wb25lbnRzPExpZ2h0PigpKQoJewoJCS8vc2tpcCBBbWJpZW50IExpZ2h0LCBEaXJlY3Rpb25hbCBMaWdodAoJCWlmIChsaWdodC5OYW1lID09ICJBbWJpZW50IExpZ2h0IiB8fCBsaWdodC5OYW1lID09ICJEaXJlY3Rpb25hbCBMaWdodCIpCgkJCWNvbnRpbnVlOwoKCQl2YXIgc3BlZWQgPSAwLjM7CgoJCXZhciB1cCA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKFRpbWUuQ3VycmVudCAqIHNwZWVkKSwgTWF0aC5TaW4oVGltZS5DdXJyZW50ICogc3BlZWQpLCAwKS5HZXROb3JtYWxpemUoKTsKCQl2YXIgZGlyID0gbmV3IFZlY3RvcjMoMCwgMCwgLTEpOwoKCQl2YXIgbmV3Um90YXRpb24gPSBRdWF0ZXJuaW9uLkxvb2tBdChkaXIsIHVwKTsKCgkJbGlnaHQuVHJhbnNmb3JtID0gbmV3IFRyYW5zZm9ybShsaWdodC5UcmFuc2Zvcm1WLlBvc2l0aW9uLCBuZXdSb3RhdGlvbik7CgoJCS8vdmFyIG5ld1JvdGF0aW9uID0gUXVhdGVybmlvbi5Gcm9tUm90YXRlQnlaKFRpbWUuQ3VycmVudCk7Cgl9Cn0K")]
public class DynamicClassACC11F227898AFE0EFD620D8835FB0064E872DF18ABD487C5C0C283F134E61FB
{
    public NeoAxis.CSharpScript Owner;
    public void _SimulationStep(NeoAxis.Component obj)
    {
        foreach (var light in obj.GetComponents<Light>())
        {
            //skip Ambient Light, Directional Light
            if (light.Name == "Ambient Light" || light.Name == "Directional Light")
                continue;
            var speed = 0.3;
            var up = new Vector3(Math.Cos(Time.Current * speed), Math.Sin(Time.Current * speed), 0).GetNormalize();
            var dir = new Vector3(0, 0, -1);
            var newRotation = Quaternion.LookAt(dir, up);
            light.Transform = new Transform(light.TransformV.Position, newRotation);
        //var newRotation = Quaternion.FromRotateByZ(Time.Current);
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJLy9vcGVuIG9yIGNsb3NlIHRoZSBnYXRlCgl2YXIgZ2F0ZSA9IHNjZW5lLkdldENvbXBvbmVudDxHYXRlPigiRGVmYXVsdCBHYXRlIDIiKTsKCWlmKGdhdGUgIT0gbnVsbCkKCXsKCQlnYXRlLlRyeVN3aXRjaChpbml0aWF0b3IpOwoKCQkvLyEhISFhZGQgbmV0d29ya2luZyBzdXBwb3J0CgkJLy9nYXRlLlRyeVNldFN0YXRlCgkJLy9nYXRlLkRlc2lyZWRTdGF0ZSA9IHNlbmRlci5BY3RpdmF0ZWQuVmFsdWUgPyAxIDogMDsKCX0JCn0K")]
public class DynamicClass54EE438C1B198F2C4ACEB88E7437455D66CEE3F2D0DF1DED80B06D56D74521E9
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
    {
        var scene = sender.ParentScene;
        //open or close the gate
        var gate = scene.GetComponent<Gate>("Default Gate 2");
        if (gate != null)
        {
            gate.TrySwitch(initiator);
        //!!!!add networking support
        //gate.TrySetState
        //gate.DesiredState = sender.Activated.Value ? 1 : 0;
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RfQ2xpY2soTmVvQXhpcy5JbnRlcmFjdGl2ZU9iamVjdCBzZW5kZXIsIE5lb0F4aXMuQ29tcG9uZW50IGluaXRpYXRvcikKewoJLy9zaG93IHNjcmVlbiBtZXNzYWdlCgl2YXIgdGV4dCA9ICJDbGljayEiOwoJaWYgKHNlbmRlci5OZXR3b3JrSXNTZXJ2ZXIpCgl7CgkJdmFyIG5ldHdvcmtMb2dpYyA9IE5ldHdvcmtMb2dpY1V0aWxpdHkuR2V0TmV0d29ya0xvZ2ljKGluaXRpYXRvcikgYXMgTmV0d29ya0xvZ2ljOwoJCW5ldHdvcmtMb2dpYz8uU2VuZFNjcmVlbk1lc3NhZ2VUb0NsaWVudEJ5Q29udHJvbGxlZE9iamVjdChpbml0aWF0b3IsIHRleHQsIGZhbHNlKTsKCX0KCWVsc2UKCQlTY3JlZW5NZXNzYWdlcy5BZGQodGV4dCk7Cn0K")]
public class DynamicClassC6807387C6C4481F71DA9C37D8CB52E0DAC8BC76C98ACD92F55EAC01F1D5690F
{
    public NeoAxis.CSharpScript Owner;
    public void InteractiveObject_Click(NeoAxis.InteractiveObject sender, NeoAxis.Component initiator)
    {
        //show screen message
        var text = "Click!";
        if (sender.NetworkIsServer)
        {
            var networkLogic = NetworkLogicUtility.GetNetworkLogic(initiator) as NetworkLogic;
            networkLogic?.SendScreenMessageToClientByControlledObject(initiator, text, false);
        }
        else
            ScreenMessages.Add(text);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgRGVmYXVsdEdhdGUzX0NhblN3aXRjaChOZW9BeGlzLkdhdGUgc2VuZGVyLCBOZW9BeGlzLkNvbXBvbmVudCBpbml0aWF0b3IsIHJlZiBib29sIGNhblN3aXRjaCkKewoJLy9nZXQgYSBjaGFyYWN0ZXIgb2YgdGhlIHBsYXllcgoJdmFyIHBsYXllckNoYXJhY3RlciA9IGluaXRpYXRvciBhcyBDaGFyYWN0ZXI7CglpZiAocGxheWVyQ2hhcmFjdGVyICE9IG51bGwpCgl7CgkJLy9jaGVja3MgcGxheWVyJ3MgY2hhcmFjdGVyIGhhcyBhIGtleQoJCXZhciBpdGVtID0gcGxheWVyQ2hhcmFjdGVyLkdldEl0ZW1CeVJlc291cmNlTmFtZShAIkNvbnRlbnRcSXRlbXNcQXV0aG9yc1xOZW9BeGlzXEtleVxLZXkuaXRlbXR5cGUiKTsKCQlpZiAoaXRlbSA9PSBudWxsKQoJCXsKCQkJLy9zaG93IHNjcmVlbiBtZXNzYWdlCgkJCXZhciB0ZXh0ID0gIllvdSBuZWVkIHRvIGhhdmUgYSBrZXkgdG8gb3BlbiB0aGUgZG9vci4iOwkJCQoJCQlpZihzZW5kZXIuTmV0d29ya0lzU2VydmVyKSAvL2lmKFNpbXVsYXRpb25BcHBTZXJ2ZXIuU2VydmVyICE9IG51bGwpCgkJCXsKCQkJCXZhciBuZXR3b3JrTG9naWMgPSBOZXR3b3JrTG9naWNVdGlsaXR5LkdldE5ldHdvcmtMb2dpYyhpbml0aWF0b3IpIGFzIE5ldHdvcmtMb2dpYzsKCQkJCW5ldHdvcmtMb2dpYz8uU2VuZFNjcmVlbk1lc3NhZ2VUb0NsaWVudEJ5Q29udHJvbGxlZE9iamVjdChpbml0aWF0b3IsIHRleHQsIGZhbHNlKTsKCQkJfQoJCQllbHNlCgkJCQlTY3JlZW5NZXNzYWdlcy5BZGQodGV4dCk7CgkJCQkKCQkJLy9zZXQgY2FuJ3QgaW50ZXJhY3QgCgkJCWNhblN3aXRjaCA9IGZhbHNlOwoJCQkKCQkJcmV0dXJuOwoJCX0KCX0JCQkKfQo=")]
public class DynamicClass510F6AF9F5978766C521583AE39C5EF4406F20DD6A82DCF288EB6F928396FA27
{
    public NeoAxis.CSharpScript Owner;
    public void DefaultGate3_CanSwitch(NeoAxis.Gate sender, NeoAxis.Component initiator, ref bool canSwitch)
    {
        //get a character of the player
        var playerCharacter = initiator as Character;
        if (playerCharacter != null)
        {
            //checks player's character has a key
            var item = playerCharacter.GetItemByResourceName(@"Content\Items\Authors\NeoAxis\Key\Key.itemtype");
            if (item == null)
            {
                //show screen message
                var text = "You need to have a key to open the door.";
                if (sender.NetworkIsServer) //if(SimulationAppServer.Server != null)
                {
                    var networkLogic = NetworkLogicUtility.GetNetworkLogic(initiator) as NetworkLogic;
                    networkLogic?.SendScreenMessageToClientByControlledObject(initiator, text, false);
                }
                else
                    ScreenMessages.Add(text);
                //set can't interact 
                canSwitch = false;
                return;
            }
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW5kaXJlY3RMaWdodGluZ19VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQp7Cgl2YXIgZWZmZWN0ID0gc2VuZGVyIGFzIFJlbmRlcmluZ0VmZmVjdF9JbmRpcmVjdExpZ2h0aW5nOwoJaWYoZWZmZWN0ICE9IG51bGwpCgkJZWZmZWN0LkludGVuc2l0eSA9IChUaW1lLkN1cnJlbnQgJSA4LjApID4gNCA_IDEgOiAwOwp9Cg==")]
public class DynamicClass3C0B0632C0376DBDE4309FC857CEA711519D39308970B7C42F61B0DD30E61A98
{
    public NeoAxis.CSharpScript Owner;
    public void IndirectLighting_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var effect = sender as RenderingEffect_IndirectLighting;
        if (effect != null)
            effect.Intensity = (Time.Current % 8.0) > 4 ? 1 : 0;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ2hhcmFjdGVyQUlfT2JqZWN0SW50ZXJhY3Rpb25JbnB1dE1lc3NhZ2VFdmVudChOZW9BeGlzLkFJIHNlbmRlciwgTmVvQXhpcy5HYW1lTW9kZSBnYW1lTW9kZSwgTmVvQXhpcy5JbnB1dE1lc3NhZ2UgbWVzc2FnZSwgcmVmIGJvb2wgaGFuZGxlZCkKewoJLy92YXIga2V5RG93biA9IG1lc3NhZ2UgYXMgSW5wdXRNZXNzYWdlS2V5RG93bjsKCS8vaWYgKGtleURvd24gIT0gbnVsbCAmJiAoa2V5RG93bi5LZXkgPT0gZ2FtZU1vZGUuS2V5SW50ZXJhY3QxIHx8IGtleURvd24uS2V5ID09IGdhbWVNb2RlLktleUludGVyYWN0MikpCgl2YXIgYnV0dG9uRG93biA9IG1lc3NhZ2UgYXMgSW5wdXRNZXNzYWdlTW91c2VCdXR0b25Eb3duOwoJaWYgKGJ1dHRvbkRvd24gIT0gbnVsbCkKCXsKCQkvL25vIG5ldHdvcmsgc3VwcG9ydC4gZm9yIG5ldHdvcmtpbmcgc3VwcG9ydCBpcyBlYXNpZXIgdG8gdXNlIEZsb3cgR3JhcGggYmFzZWQgdmFyaWFudAoKCQkvL2NyZWF0ZSBpbnRlcmFjdGlvbgoJCXZhciBpbnRlcmFjdGlvbiA9IGdhbWVNb2RlLkNyZWF0ZUNvbXBvbmVudDxDb250aW51b3VzSW50ZXJhY3Rpb24+KGVuYWJsZWQ6IGZhbHNlKTsKCQlpbnRlcmFjdGlvbi5DcmVhdG9yID0gc2VuZGVyOwoJCWludGVyYWN0aW9uLlNlY29uZFBhcnRpY2lwYW50ID0gZ2FtZU1vZGUuT2JqZWN0Q29udHJvbGxlZEJ5UGxheWVyLlZhbHVlOwoJCQoJCS8vc2V0IHVwIHRoZSBmaXJzdCBtZXNzYWdlIGZyb20gTlBDIGFuZCBwb3NzaWJsZSBhbnN3ZXJzCgkJdmFyIGJsb2NrID0gbmV3IFRleHRCbG9jaygpOwoJCWJsb2NrLlNldEF0dHJpYnV0ZSgiTWVzc2FnZUlEIiwgIjEiKTsKCQlibG9jay5TZXRBdHRyaWJ1dGUoIk1lc3NhZ2UiLCAiSGkhIEhvdyBjYW4gSSBoZWxwIHlvdT8iKTsKCQlibG9jay5TZXRBdHRyaWJ1dGUoIkFuc3dlciAxIiwgIlNheSBzb21ldGhpbmcgZWxzZSIpOwoJCWJsb2NrLlNldEF0dHJpYnV0ZSgiQW5zd2VyIDIiLCAiQnllIik7CgkJaW50ZXJhY3Rpb24uQ3VycmVudE1lc3NhZ2VGcm9tQ3JlYXRvciA9IGJsb2NrLkR1bXBUb1N0cmluZygpOwoKCQlpbnRlcmFjdGlvbi5NZXNzYWdlRnJvbVBhcnRpY2lwYW50RXZlbnQgKz0gZGVsZWdhdGUgKENvbnRpbnVvdXNJbnRlcmFjdGlvbiBzZW5kZXIyLCBzdHJpbmcgbWVzc2FnZTIpCgkJewoJCQkvL3Byb2Nlc3MgbWVzc2FnZXMgZnJvbSBOUEMKCQkJCgkJCXZhciBibG9jazIgPSBUZXh0QmxvY2suUGFyc2UobWVzc2FnZTIsIG91dCBfKTsKCQkJaWYgKGJsb2NrMiAhPSBudWxsKQoJCQl7CgkJCQl2YXIgbWVzc2FnZUlEID0gYmxvY2syLkdldEF0dHJpYnV0ZSgiTWVzc2FnZUlEIik7CgkJCQl2YXIgYW5zd2VyID0gYmxvY2syLkdldEF0dHJpYnV0ZSgiQW5zd2VyIik7CgkJCQkvL0xvZy5JbmZvKGFuc3dlcik7CgkJCQkKCQkJCS8vZ2VuZXJhdGUgYW5zd2VycyB0byBtZXNzYWdlIDEgCgkJCQlpZihtZXNzYWdlSUQgPT0gIjEiKQoJCQkJewoJCQkJCWlmIChhbnN3ZXIgPT0gIjEiKQoJCQkJCXsKCQkJCQkJdmFyIGJsb2NrMyA9IG5ldyBUZXh0QmxvY2soKTsKCQkJCQkJYmxvY2szLlNldEF0dHJpYnV0ZSgiTWVzc2FnZUlEIiwgIjIiKTsKCQkJCQkJYmxvY2szLlNldEF0dHJpYnV0ZSgiTWVzc2FnZSIsICJObyBwcm9ibGVtIDopIik7CgkJCQkJCWJsb2NrMy5TZXRBdHRyaWJ1dGUoIkFuc3dlciAxIiwgIkJ5ZSIpOwoJCQkJCQlpbnRlcmFjdGlvbi5DdXJyZW50TWVzc2FnZUZyb21DcmVhdG9yID0gYmxvY2szLkR1bXBUb1N0cmluZygpOwoJCQkJCX0KCQkJCQlpZiAoYW5zd2VyID09ICIyIikKCQkJCQl7CgkJCQkJCXNlbmRlcjIuUmVtb3ZlRnJvbVBhcmVudCh0cnVlKTsKCQkJCQl9CgkJCQl9CgoJCQkJLy9nZW5lcmF0ZSBhbnN3ZXJzIHRvIG1lc3NhZ2UgMiAKCQkJCWlmKG1lc3NhZ2VJRCA9PSAiMiIpCgkJCQl7CgkJCQkJaWYgKGFuc3dlciA9PSAiMSIpCgkJCQkJewoJCQkJCQlzZW5kZXIyLlJlbW92ZUZyb21QYXJlbnQodHJ1ZSk7CgkJCQkJfQoJCQkJfQoJCQl9CgkJfTsKCgkJaW50ZXJhY3Rpb24uRW5hYmxlZCA9IHRydWU7CgoJCWhhbmRsZWQgPSB0cnVlOwoJfQp9Cg==")]
public class DynamicClassFB80DFFF8F782855F05190B08D32818D6E087688434F9F52A13CCE9D7D630809
{
    public NeoAxis.CSharpScript Owner;
    public void CharacterAI_ObjectInteractionInputMessageEvent(NeoAxis.AI sender, NeoAxis.GameMode gameMode, NeoAxis.InputMessage message, ref bool handled)
    {
        //var keyDown = message as InputMessageKeyDown;
        //if (keyDown != null && (keyDown.Key == gameMode.KeyInteract1 || keyDown.Key == gameMode.KeyInteract2))
        var buttonDown = message as InputMessageMouseButtonDown;
        if (buttonDown != null)
        {
            //no network support. for networking support is easier to use Flow Graph based variant
            //create interaction
            var interaction = gameMode.CreateComponent<ContinuousInteraction>(enabled: false);
            interaction.Creator = sender;
            interaction.SecondParticipant = gameMode.ObjectControlledByPlayer.Value;
            //set up the first message from NPC and possible answers
            var block = new TextBlock();
            block.SetAttribute("MessageID", "1");
            block.SetAttribute("Message", "Hi! How can I help you?");
            block.SetAttribute("Answer 1", "Say something else");
            block.SetAttribute("Answer 2", "Bye");
            interaction.CurrentMessageFromCreator = block.DumpToString();
            interaction.MessageFromParticipantEvent += delegate (ContinuousInteraction sender2, string message2)
            {
                //process messages from NPC
                var block2 = TextBlock.Parse(message2, out _);
                if (block2 != null)
                {
                    var messageID = block2.GetAttribute("MessageID");
                    var answer = block2.GetAttribute("Answer");
                    //Log.Info(answer);
                    //generate answers to message 1 
                    if (messageID == "1")
                    {
                        if (answer == "1")
                        {
                            var block3 = new TextBlock();
                            block3.SetAttribute("MessageID", "2");
                            block3.SetAttribute("Message", "No problem :)");
                            block3.SetAttribute("Answer 1", "Bye");
                            interaction.CurrentMessageFromCreator = block3.DumpToString();
                        }

                        if (answer == "2")
                        {
                            sender2.RemoveFromParent(true);
                        }
                    }

                    //generate answers to message 2 
                    if (messageID == "2")
                    {
                        if (answer == "1")
                        {
                            sender2.RemoveFromParent(true);
                        }
                    }
                }
            }

            ;
            interaction.Enabled = true;
            handled = true;
        }
    }
}

[CSharpScriptGeneratedAttribute("Ym9vbCBNZXRob2QoKQp7CgkvL3ByZXZlbnQgZXhlY3V0aW9uIHRoZSBzY3JpcHQgaW4gdGhlIGVkaXRvciBhbmQgd2hlbiBsb2FkaW5nCglpZihFbmdpbmVBcHAuSXNTaW11bGF0aW9uICYmICFPd25lci5QYXJlbnRSb290LkhpZXJhcmNoeUNvbnRyb2xsZXIuTG9hZGluZykKCXsKCQkvL2dldCBjdXJyZW50IGludGVyYWN0aW9uCgkJdmFyIGludGVyYWN0aW9uID0gQ29udGludW91c0ludGVyYWN0aW9uLkxhdGVzdDsKCgkJLy9nZXQgYSBjaGFyYWN0ZXIgb2YgdGhlIHBsYXllcgoJCXZhciBwbGF5ZXJDaGFyYWN0ZXIgPSAoQ2hhcmFjdGVyKWludGVyYWN0aW9uLlNlY29uZFBhcnRpY2lwYW50LlZhbHVlOwoKCQkvL2NoZWNrcyBwbGF5ZXIncyBjaGFyYWN0ZXIgaGFzIGEga2V5CgkJdmFyIGl0ZW0gPSBwbGF5ZXJDaGFyYWN0ZXIuR2V0SXRlbUJ5UmVzb3VyY2VOYW1lKEAiQ29udGVudFxJdGVtc1xBdXRob3JzXE5lb0F4aXNcS2V5XEtleS5pdGVtdHlwZSIpOwoJCWlmIChpdGVtICE9IG51bGwpCgkJCXJldHVybiB0cnVlOwoJfQoJCglyZXR1cm4gZmFsc2U7Cn0K")]
public class DynamicClass1CA23B1559D6A76F647B46797121830D2E3602786C521514E7A7FEDA00DF24D2
{
    public NeoAxis.CSharpScript Owner;
    bool Method()
    {
        //prevent execution the script in the editor and when loading
        if (EngineApp.IsSimulation && !Owner.ParentRoot.HierarchyController.Loading)
        {
            //get current interaction
            var interaction = ContinuousInteraction.Latest;
            //get a character of the player
            var playerCharacter = (Character)interaction.SecondParticipant.Value;
            //checks player's character has a key
            var item = playerCharacter.GetItemByResourceName(@"Content\Items\Authors\NeoAxis\Key\Key.itemtype");
            if (item != null)
                return true;
        }

        return false;
    }
}

[CSharpScriptGeneratedAttribute("dm9pZCBNZXRob2QoKQp7CglMb2cuSW5mbygiTWVzc2FnZSBmcm9tIHRoZSBkaWFsb2d1ZSIpOwp9Cg==")]
public class DynamicClassCD77F714BBDC128E3675B41DB2C321D17906634F20B508D44777046026BCAD4D
{
    public NeoAxis.CSharpScript Owner;
    void Method()
    {
        Log.Info("Message from the dialogue");
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ2hhcmFjdGVyX1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpCnsKCXZhciBjaGFyYWN0ZXIgPSBzZW5kZXIgYXMgQ2hhcmFjdGVyOwoJaWYoY2hhcmFjdGVyICE9IG51bGwpCgl7CgkJLy9maW5nZXJzCgoJCXsKCQkJdmFyIHZhbHVlID0gKE1hdGguU2luKFRpbWUuQ3VycmVudCAqIDEuNSkgKyAxKSAvIDI7CgkJCQoJCQkvL2NoYXJhY3Rlci5MZWZ0SGFuZFRodW1iRmluZ2VyRmxleGlvbkZhY3RvciA9IDE7CgkJCS8vY2hhcmFjdGVyLkxlZnRIYW5kVGh1bWJGaW5nZXJGbGV4aW9uVmFsdWUgPSB2YWx1ZTsKCQkJY2hhcmFjdGVyLkxlZnRIYW5kSW5kZXhGaW5nZXJGbGV4aW9uRmFjdG9yID0gMTsKCQkJY2hhcmFjdGVyLkxlZnRIYW5kSW5kZXhGaW5nZXJGbGV4aW9uVmFsdWUgPSB2YWx1ZTsKCQkJY2hhcmFjdGVyLkxlZnRIYW5kTWlkZGxlRmluZ2VyRmxleGlvbkZhY3RvciA9IDE7CgkJCWNoYXJhY3Rlci5MZWZ0SGFuZE1pZGRsZUZpbmdlckZsZXhpb25WYWx1ZSA9IHZhbHVlOwoJCQljaGFyYWN0ZXIuTGVmdEhhbmRSaW5nRmluZ2VyRmxleGlvbkZhY3RvciA9IDE7CgkJCWNoYXJhY3Rlci5MZWZ0SGFuZFJpbmdGaW5nZXJGbGV4aW9uVmFsdWUgPSB2YWx1ZTsKCQkJY2hhcmFjdGVyLkxlZnRIYW5kTGl0dGxlRmluZ2VyRmxleGlvbkZhY3RvciA9IDE7CgkJCWNoYXJhY3Rlci5MZWZ0SGFuZExpdHRsZUZpbmdlckZsZXhpb25WYWx1ZSA9IHZhbHVlOwoJCX0KCgkJewoJCQl2YXIgdmFsdWUgPSAoTWF0aC5TaW4oKFRpbWUuQ3VycmVudCArIDEpICogMS41KSArIDEpIC8gMjsKCgkJCS8vY2hhcmFjdGVyLlJpZ2h0SGFuZFRodW1iRmluZ2VyRmxleGlvbkZhY3RvciA9IDE7CgkJCS8vY2hhcmFjdGVyLlJpZ2h0SGFuZFRodW1iRmluZ2VyRmxleGlvblZhbHVlID0gdmFsdWU7CgkJCWNoYXJhY3Rlci5SaWdodEhhbmRJbmRleEZpbmdlckZsZXhpb25GYWN0b3IgPSAxOwoJCQljaGFyYWN0ZXIuUmlnaHRIYW5kSW5kZXhGaW5nZXJGbGV4aW9uVmFsdWUgPSB2YWx1ZTsKCQkJY2hhcmFjdGVyLlJpZ2h0SGFuZE1pZGRsZUZpbmdlckZsZXhpb25GYWN0b3IgPSAxOwoJCQljaGFyYWN0ZXIuUmlnaHRIYW5kTWlkZGxlRmluZ2VyRmxleGlvblZhbHVlID0gdmFsdWU7CgkJCWNoYXJhY3Rlci5SaWdodEhhbmRSaW5nRmluZ2VyRmxleGlvbkZhY3RvciA9IDE7CgkJCWNoYXJhY3Rlci5SaWdodEhhbmRSaW5nRmluZ2VyRmxleGlvblZhbHVlID0gdmFsdWU7CgkJCWNoYXJhY3Rlci5SaWdodEhhbmRMaXR0bGVGaW5nZXJGbGV4aW9uRmFjdG9yID0gMTsKCQkJY2hhcmFjdGVyLlJpZ2h0SGFuZExpdHRsZUZpbmdlckZsZXhpb25WYWx1ZSA9IHZhbHVlOwoJCX0KCX0KfQo=")]
public class DynamicClassD740203FA7C256A143D4A53FB644115371AAF5C7A7740BEBB16474579E0C90BD
{
    public NeoAxis.CSharpScript Owner;
    public void Character_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var character = sender as Character;
        if (character != null)
        {
            //fingers
            {
                var value = (Math.Sin(Time.Current * 1.5) + 1) / 2;
                //character.LeftHandThumbFingerFlexionFactor = 1;
                //character.LeftHandThumbFingerFlexionValue = value;
                character.LeftHandIndexFingerFlexionFactor = 1;
                character.LeftHandIndexFingerFlexionValue = value;
                character.LeftHandMiddleFingerFlexionFactor = 1;
                character.LeftHandMiddleFingerFlexionValue = value;
                character.LeftHandRingFingerFlexionFactor = 1;
                character.LeftHandRingFingerFlexionValue = value;
                character.LeftHandLittleFingerFlexionFactor = 1;
                character.LeftHandLittleFingerFlexionValue = value;
            }

            {
                var value = (Math.Sin((Time.Current + 1) * 1.5) + 1) / 2;
                //character.RightHandThumbFingerFlexionFactor = 1;
                //character.RightHandThumbFingerFlexionValue = value;
                character.RightHandIndexFingerFlexionFactor = 1;
                character.RightHandIndexFingerFlexionValue = value;
                character.RightHandMiddleFingerFlexionFactor = 1;
                character.RightHandMiddleFingerFlexionValue = value;
                character.RightHandRingFingerFlexionFactor = 1;
                character.RightHandRingFingerFlexionValue = value;
                character.RightHandLittleFingerFlexionFactor = 1;
                character.RightHandLittleFingerFlexionValue = value;
            }
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJdmFyIGdyb3VuZCA9IHNjZW5lLkdldENvbXBvbmVudCgiR3JvdW5kIikgYXMgTWVzaEluU3BhY2U7CglpZiAoZ3JvdW5kICE9IG51bGwpCgl7CgkJaWYgKCFncm91bmQuUmVwbGFjZU1hdGVyaWFsLlJlZmVyZW5jZVNwZWNpZmllZCkKCQl7CgkJCWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwgPSBSZWZlcmVuY2VVdGlsaXR5Lk1ha2VSZWZlcmVuY2UoIEAiQmFzZVxNYXRlcmlhbHNcRGFyayBZZWxsb3cubWF0ZXJpYWwiKTsKCQl9CgkJZWxzZQoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gbnVsbDsKCX0KfQo=")]
public class DynamicClassC531FCB04863AD2145B4BEC2E9B1B9A487095C0E48750102EEC5D3BFD939A9B3
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
    {
        var scene = sender.ParentScene;
        var ground = scene.GetComponent("Ground") as MeshInSpace;
        if (ground != null)
        {
            if (!ground.ReplaceMaterial.ReferenceSpecified)
            {
                ground.ReplaceMaterial = ReferenceUtility.MakeReference(@"Base\Materials\Dark Yellow.material");
            }
            else
                ground.ReplaceMaterial = null;
        }
    }
}

[CSharpScriptGeneratedAttribute("UmVuZGVyaW5nUGlwZWxpbmUgR2V0UGlwZWxpbmUoKQp7CglzdHJpbmcgbmFtZTsKCWlmKEVuZ2luZUFwcC5FbmdpbmVUaW1lICUgNiA+IDMpCgkJbmFtZSA9ICJSZW5kZXJpbmcgUGlwZWxpbmUiOwoJZWxzZQoJCW5hbWUgPSAiUmVuZGVyaW5nIFBpcGVsaW5lIDIiOwoJCQoJcmV0dXJuIE93bmVyLlBhcmVudC5HZXRDb21wb25lbnQobmFtZSkgYXMgUmVuZGVyaW5nUGlwZWxpbmU7Cn0K")]
public class DynamicClassE9187D41516A838882B97D8B60F698D11F337B7737278CBFF9DB427D2BB97E84
{
    public NeoAxis.CSharpScript Owner;
    RenderingPipeline GetPipeline()
    {
        string name;
        if (EngineApp.EngineTime % 6 > 3)
            name = "Rendering Pipeline";
        else
            name = "Rendering Pipeline 2";
        return Owner.Parent.GetComponent(name) as RenderingPipeline;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpCnsKCXZhciBvYmplY3QxID0gc2VuZGVyLkNvbXBvbmVudHNbIlNwaGVyZSJdIGFzIE1lc2hJblNwYWNlOwoJaWYob2JqZWN0MSAhPSBudWxsKQoJCW9iamVjdDEuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgwLjUsIDAuNzUgKyBNYXRoLlNpbihUaW1lLkN1cnJlbnQpICogMC4yNSwgMC41KTsKCgl2YXIgbWF0ZXJpYWwyID0gc2VuZGVyLkNvbXBvbmVudHNbIkJveFxcTWF0ZXJpYWwiXSBhcyBNYXRlcmlhbDsKCWlmKG1hdGVyaWFsMiAhPSBudWxsKQoJCW1hdGVyaWFsMi5FbWlzc2l2ZSA9IG5ldyBDb2xvclZhbHVlUG93ZXJlZCgwLCAoMS4wICsgTWF0aC5TaW4oVGltZS5DdXJyZW50KSkgKiA1LCAwKTsKCQkKCXZhciBtYXRlcmlhbDMgPSBzZW5kZXIuQ29tcG9uZW50c1siQ3lsaW5kZXJcXE1hdGVyaWFsIl0gYXMgTWF0ZXJpYWw7CglpZihtYXRlcmlhbDMgIT0gbnVsbCkKCQltYXRlcmlhbDMuUHJvcGVydHlTZXQoIk11bHRpcGxpZXIiLCBuZXcgQ29sb3JWYWx1ZSgxLCAxLCAxLjAgKyAoMS4wICsgTWF0aC5TaW4oVGltZS5DdXJyZW50KSkgKiA1KSk7Cn0K")]
public class DynamicClass05C48E8299CF8F7A31FE138987B25138DC388677E1788A606B8EC18AFAACBFF3
{
    public NeoAxis.CSharpScript Owner;
    public void _UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var object1 = sender.Components["Sphere"] as MeshInSpace;
        if (object1 != null)
            object1.Color = new ColorValue(0.5, 0.75 + Math.Sin(Time.Current) * 0.25, 0.5);
        var material2 = sender.Components["Box\\Material"] as Material;
        if (material2 != null)
            material2.Emissive = new ColorValuePowered(0, (1.0 + Math.Sin(Time.Current)) * 5, 0);
        var material3 = sender.Components["Cylinder\\Material"] as Material;
        if (material3 != null)
            material3.PropertySet("Multiplier", new ColorValue(1, 1, 1.0 + (1.0 + Math.Sin(Time.Current)) * 5));
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJdmFyIGxpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJBbWJpZW50IExpZ2h0IikgYXMgTGlnaHQ7CglpZiAobGlnaHQgIT0gbnVsbCkKCQlsaWdodC5FbmFibGVkID0gc2VuZGVyLkFjdGl2YXRlZDsJCn0K")]
public class DynamicClass96656C5A6CC0B1EDBE0FDD701410A6AABE2A697632F72821901D9A8841FE2424
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
    {
        var scene = sender.ParentScene;
        var light = scene.GetComponent("Ambient Light") as Light;
        if (light != null)
            light.Enabled = sender.Activated;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLlJlZ3VsYXRvciBvYmopCnsKCXZhciBzY2VuZSA9IG9iai5QYXJlbnRTY2VuZTsKCgl2YXIgbWVzaEluU3BhY2UgPSBzY2VuZS5HZXRDb21wb25lbnQoIkdyb3VuZCIpIGFzIE1lc2hJblNwYWNlOwoJaWYgKG1lc2hJblNwYWNlICE9IG51bGwpCgkJbWVzaEluU3BhY2UuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLjAgLSBvYmouVmFsdWUsIDEuMCwgMS4wIC0gb2JqLlZhbHVlKTsKfQo=")]
public class DynamicClass44C8C9BBF173B5F1D3C3C2DEF0D73724470FF9F2857E0FE7F14F1957170EDBFF
{
    public NeoAxis.CSharpScript Owner;
    public void Regulator_ValueChanged(NeoAxis.Regulator obj)
    {
        var scene = obj.ParentScene;
        var meshInSpace = scene.GetComponent("Ground") as MeshInSpace;
        if (meshInSpace != null)
            meshInSpace.Color = new ColorValue(1.0 - obj.Value, 1.0, 1.0 - obj.Value);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ2hhcmFjdGVyQUlfT2JqZWN0SW50ZXJhY3Rpb25JbnB1dE1lc3NhZ2VFdmVudChOZW9BeGlzLkFJIHNlbmRlciwgTmVvQXhpcy5HYW1lTW9kZSBnYW1lTW9kZSwgQ29tcG9uZW50IGluaXRpYXRvciwgTmVvQXhpcy5JbnB1dE1lc3NhZ2UgbWVzc2FnZSwgcmVmIGJvb2wgaGFuZGxlZCkKewoJLy92YXIga2V5RG93biA9IG1lc3NhZ2UgYXMgSW5wdXRNZXNzYWdlS2V5RG93bjsKCS8vaWYgKGtleURvd24gIT0gbnVsbCAmJiAoa2V5RG93bi5LZXkgPT0gZ2FtZU1vZGUuS2V5SW50ZXJhY3QxIHx8IGtleURvd24uS2V5ID09IGdhbWVNb2RlLktleUludGVyYWN0MikpCgl2YXIgYnV0dG9uRG93biA9IG1lc3NhZ2UgYXMgSW5wdXRNZXNzYWdlTW91c2VCdXR0b25Eb3duOwoJaWYgKGJ1dHRvbkRvd24gIT0gbnVsbCkKCXsKCQkvL25vIG5ldHdvcmsgc3VwcG9ydC4gZm9yIG5ldHdvcmtpbmcgc3VwcG9ydCBpcyBlYXNpZXIgdG8gdXNlIEZsb3cgR3JhcGggYmFzZWQgdmFyaWFudAoKCQkvL2NyZWF0ZSBpbnRlcmFjdGlvbgoJCXZhciBpbnRlcmFjdGlvbiA9IGdhbWVNb2RlLkNyZWF0ZUNvbXBvbmVudDxDb250aW51b3VzSW50ZXJhY3Rpb24+KGVuYWJsZWQ6IGZhbHNlKTsKCQlpbnRlcmFjdGlvbi5DcmVhdG9yID0gc2VuZGVyOwoJCWludGVyYWN0aW9uLlNlY29uZFBhcnRpY2lwYW50ID0gZ2FtZU1vZGUuT2JqZWN0Q29udHJvbGxlZEJ5UGxheWVyLlZhbHVlOwoJCQoJCS8vc2V0IHVwIHRoZSBmaXJzdCBtZXNzYWdlIGZyb20gTlBDIGFuZCBwb3NzaWJsZSBhbnN3ZXJzCgkJdmFyIGJsb2NrID0gbmV3IFRleHRCbG9jaygpOwoJCWJsb2NrLlNldEF0dHJpYnV0ZSgiTWVzc2FnZUlEIiwgIjEiKTsKCQlibG9jay5TZXRBdHRyaWJ1dGUoIk1lc3NhZ2UiLCAiSGkhIEhvdyBjYW4gSSBoZWxwIHlvdT8iKTsKCQlibG9jay5TZXRBdHRyaWJ1dGUoIkFuc3dlciAxIiwgIlNheSBzb21ldGhpbmcgZWxzZSIpOwoJCWJsb2NrLlNldEF0dHJpYnV0ZSgiQW5zd2VyIDIiLCAiQnllIik7CgkJaW50ZXJhY3Rpb24uQ3VycmVudE1lc3NhZ2VGcm9tQ3JlYXRvciA9IGJsb2NrLkR1bXBUb1N0cmluZygpOwoKCQlpbnRlcmFjdGlvbi5NZXNzYWdlRnJvbVBhcnRpY2lwYW50RXZlbnQgKz0gZGVsZWdhdGUgKENvbnRpbnVvdXNJbnRlcmFjdGlvbiBzZW5kZXIyLCBzdHJpbmcgbWVzc2FnZTIpCgkJewoJCQkvL3Byb2Nlc3MgbWVzc2FnZXMgZnJvbSBOUEMKCQkJCgkJCXZhciBibG9jazIgPSBUZXh0QmxvY2suUGFyc2UobWVzc2FnZTIsIG91dCBfKTsKCQkJaWYgKGJsb2NrMiAhPSBudWxsKQoJCQl7CgkJCQl2YXIgbWVzc2FnZUlEID0gYmxvY2syLkdldEF0dHJpYnV0ZSgiTWVzc2FnZUlEIik7CgkJCQl2YXIgYW5zd2VyID0gYmxvY2syLkdldEF0dHJpYnV0ZSgiQW5zd2VyIik7CgkJCQkvL0xvZy5JbmZvKGFuc3dlcik7CgkJCQkKCQkJCS8vZ2VuZXJhdGUgYW5zd2VycyB0byBtZXNzYWdlIDEgCgkJCQlpZihtZXNzYWdlSUQgPT0gIjEiKQoJCQkJewoJCQkJCWlmIChhbnN3ZXIgPT0gIjEiKQoJCQkJCXsKCQkJCQkJdmFyIGJsb2NrMyA9IG5ldyBUZXh0QmxvY2soKTsKCQkJCQkJYmxvY2szLlNldEF0dHJpYnV0ZSgiTWVzc2FnZUlEIiwgIjIiKTsKCQkJCQkJYmxvY2szLlNldEF0dHJpYnV0ZSgiTWVzc2FnZSIsICJObyBwcm9ibGVtIDopIik7CgkJCQkJCWJsb2NrMy5TZXRBdHRyaWJ1dGUoIkFuc3dlciAxIiwgIkJ5ZSIpOwoJCQkJCQlpbnRlcmFjdGlvbi5DdXJyZW50TWVzc2FnZUZyb21DcmVhdG9yID0gYmxvY2szLkR1bXBUb1N0cmluZygpOwoJCQkJCX0KCQkJCQlpZiAoYW5zd2VyID09ICIyIikKCQkJCQl7CgkJCQkJCXNlbmRlcjIuUmVtb3ZlRnJvbVBhcmVudCh0cnVlKTsKCQkJCQl9CgkJCQl9CgoJCQkJLy9nZW5lcmF0ZSBhbnN3ZXJzIHRvIG1lc3NhZ2UgMiAKCQkJCWlmKG1lc3NhZ2VJRCA9PSAiMiIpCgkJCQl7CgkJCQkJaWYgKGFuc3dlciA9PSAiMSIpCgkJCQkJewoJCQkJCQlzZW5kZXIyLlJlbW92ZUZyb21QYXJlbnQodHJ1ZSk7CgkJCQkJfQoJCQkJfQoJCQl9CgkJfTsKCgkJaW50ZXJhY3Rpb24uRW5hYmxlZCA9IHRydWU7CgoJCWhhbmRsZWQgPSB0cnVlOwoJfQp9Cg==")]
public class DynamicClassBF4DB8B384881FB790CCDD4ADF8BED1CBDD60FD6B11D5930F8FC6B3D917EDFDC
{
    public NeoAxis.CSharpScript Owner;
    public void CharacterAI_ObjectInteractionInputMessageEvent(NeoAxis.AI sender, NeoAxis.GameMode gameMode, Component initiator, NeoAxis.InputMessage message, ref bool handled)
    {
        //var keyDown = message as InputMessageKeyDown;
        //if (keyDown != null && (keyDown.Key == gameMode.KeyInteract1 || keyDown.Key == gameMode.KeyInteract2))
        var buttonDown = message as InputMessageMouseButtonDown;
        if (buttonDown != null)
        {
            //no network support. for networking support is easier to use Flow Graph based variant
            //create interaction
            var interaction = gameMode.CreateComponent<ContinuousInteraction>(enabled: false);
            interaction.Creator = sender;
            interaction.SecondParticipant = gameMode.ObjectControlledByPlayer.Value;
            //set up the first message from NPC and possible answers
            var block = new TextBlock();
            block.SetAttribute("MessageID", "1");
            block.SetAttribute("Message", "Hi! How can I help you?");
            block.SetAttribute("Answer 1", "Say something else");
            block.SetAttribute("Answer 2", "Bye");
            interaction.CurrentMessageFromCreator = block.DumpToString();
            interaction.MessageFromParticipantEvent += delegate (ContinuousInteraction sender2, string message2)
            {
                //process messages from NPC
                var block2 = TextBlock.Parse(message2, out _);
                if (block2 != null)
                {
                    var messageID = block2.GetAttribute("MessageID");
                    var answer = block2.GetAttribute("Answer");
                    //Log.Info(answer);
                    //generate answers to message 1 
                    if (messageID == "1")
                    {
                        if (answer == "1")
                        {
                            var block3 = new TextBlock();
                            block3.SetAttribute("MessageID", "2");
                            block3.SetAttribute("Message", "No problem :)");
                            block3.SetAttribute("Answer 1", "Bye");
                            interaction.CurrentMessageFromCreator = block3.DumpToString();
                        }

                        if (answer == "2")
                        {
                            sender2.RemoveFromParent(true);
                        }
                    }

                    //generate answers to message 2 
                    if (messageID == "2")
                    {
                        if (answer == "1")
                        {
                            sender2.RemoveFromParent(true);
                        }
                    }
                }
            }

            ;
            interaction.Enabled = true;
            handled = true;
        }
    }
}

[CSharpScriptGeneratedAttribute("dm9pZCBNZXRob2QoKQp7CgkvL3RoZSBzY3JpcHQgbWFrZXMgY29weSBvZiBjaGlsZCBjb21wb25lbnRzIG9mIHRlbXBsYXRlIGJ1aWxkaW5nIHRvIG90aGVyIGJ1aWxkaW5ncyAKCgl2YXIgcmFuZG9tID0gbmV3IEZhc3RSYW5kb20oMCk7CgkKCXZhciBzY2VuZSA9IE93bmVyLkZpbmRQYXJlbnQ8U2NlbmU+KCk7CglpZihzY2VuZSAhPSBudWxsKQoJewoJCXZhciBzb3VyY2UgPSBzY2VuZS5HZXRDb21wb25lbnQ8QnVpbGRpbmc+KCJCdWlsZGluZyIpOwoJCXZhciBkZXN0QnVpbGRpbmdzID0gc2NlbmUuR2V0Q29tcG9uZW50czxCdWlsZGluZz4oKS5XaGVyZShiID0+IGIuTmFtZSAhPSAiQnVpbGRpbmciKS5Ub0FycmF5KCk7CgkJCgkJaWYoc291cmNlICE9IG51bGwgJiYgZGVzdEJ1aWxkaW5ncy5MZW5ndGggIT0gMCkKCQl7CgkJCWZvcmVhY2godmFyIGRlc3QgaW4gZGVzdEJ1aWxkaW5ncykKCQkJewoJCQkJZGVzdC5FbmFibGVkID0gZmFsc2U7CgkJCQlkZXN0LlJlbW92ZUFsbENvbXBvbmVudHMoZmFsc2UpOwoKCQkJCWRlc3QuQnVpbGRpbmdUeXBlID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKEAiU2FtcGxlc1xDaXR5IERlbW9cQnVpbGRpbmdzXFR5cGUgMS5idWlsZGluZ3R5cGUiKTsKCQkJCWRlc3QuT2NjbHVkZXIgPSBzb3VyY2UuT2NjbHVkZXI7CgkJCQlkZXN0LlNlZWQgPSByYW5kb20uTmV4dEludGVnZXIoKTsKCgoJCQkJLyoKCQkJCWZvcmVhY2godmFyIGMgaW4gc291cmNlLkdldENvbXBvbmVudHMoKSkKCQkJCXsKCQkJCQlpZihjLkVuYWJsZWQpCgkJCQkJewoJCQkJCQl2YXIgbmV3QyA9IChDb21wb25lbnQpYy5DbG9uZSgpOwoJCQkJCQlkZXN0LkFkZENvbXBvbmVudChuZXdDKTsKCQkJCQl9CgkJCQl9Ki8KCQkJfQoJCQkJCQkKCQkJZm9yZWFjaCh2YXIgZGVzdCBpbiBkZXN0QnVpbGRpbmdzKQoJCQkJZGVzdC5FbmFibGVkID0gdHJ1ZTsKCQl9Cgl9Cn0K")]
public class DynamicClass116CBBFE8C60C52489D28A9D3ADA1060EEBBD4CEC00AD997532B918A435CF0FF
{
    public NeoAxis.CSharpScript Owner;
    void Method()
    {
        //the script makes copy of child components of template building to other buildings 
        var random = new FastRandom(0);
        var scene = Owner.FindParent<Scene>();
        if (scene != null)
        {
            var source = scene.GetComponent<Building>("Building");
            var destBuildings = scene.GetComponents<Building>().Where(b => b.Name != "Building").ToArray();
            if (source != null && destBuildings.Length != 0)
            {
                foreach (var dest in destBuildings)
                {
                    dest.Enabled = false;
                    dest.RemoveAllComponents(false);
                    dest.BuildingType = ReferenceUtility.MakeReference(@"Samples\City Demo\Buildings\Type 1.buildingtype");
                    dest.Occluder = source.Occluder;
                    dest.Seed = random.NextInteger();
                /*
				foreach(var c in source.GetComponents())
				{
					if(c.Enabled)
					{
						var newC = (Component)c.Clone();
						dest.AddComponent(newC);
					}
				}*/
                }

                foreach (var dest in destBuildings)
                    dest.Enabled = true;
            }
        }
    }
}

[CSharpScriptGeneratedAttribute("c3RhdGljIGJvb2wgY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nOwpzdGF0aWMgYm9vbCBjdXJyZW50TmlnaHQ7CnN0YXRpYyBpbnQgY3VycmVudFdlYXRoZXI7CnN0YXRpYyBib29sIGN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9yczsKc3RhdGljIGJvb2wgY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXI7Ci8vc3RhdGljIGJvb2wgY3VycmVudFJlZmxlY3Rpb25Qcm9iZSA9IHRydWU7Cgpjb25zdCBpbnQgU3VubnkgPSAwOwpjb25zdCBpbnQgUmFpbkZhbGxpbmcgPSAxOwpjb25zdCBpbnQgUmFpbkZhbGxlbiA9IDI7Cgp2b2lkIFVwZGF0ZUZvZ0FuZEZhckNsaXBQbGFuZShGb2cgZm9nLCBDYW1lcmEgY2FtZXJhKQp7Cglmb2cuRW5hYmxlZCA9ICFjdXJyZW50TmlnaHQ7Ly8gfHwgY3VycmVudFJhaW47Cglmb2cuRGVuc2l0eSA9IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nID8gMC4wMSA6IDAuMDAxOy8vZm9nLkRlbnNpdHkgPSBjdXJyZW50UmFpbiA_IDAuMDEgOiAwLjAwMTsKCglpZiAoY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcpLy9pZiAoY3VycmVudFJhaW4pCgkJZm9nLkFmZmVjdEJhY2tncm91bmQgPSAxOwoJZWxzZQoJCWZvZy5BZmZlY3RCYWNrZ3JvdW5kID0gY3VycmVudE5pZ2h0ID8gMCA6IDAuNTsKCglpZiAoY3VycmVudE5pZ2h0KQoJCWZvZy5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAsIDAsIDApOwoJZWxzZQoJCWZvZy5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNDUzOTYwOCwgMC41MTYwMzkyLCAwLjY1ODgyMzUpOwoKCWlmIChmb2cuRW5hYmxlZCAmJiBmb2cuQWZmZWN0QmFja2dyb3VuZCA9PSAxKQoJCWNhbWVyYS5GYXJDbGlwUGxhbmUgPSAzMDA7CgllbHNlCgkJY2FtZXJhLkZhckNsaXBQbGFuZSA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDIwMDAgOiAxMDAwOwp9Cgp2b2lkIFVwZGF0ZU1pY3JvcGFydGljbGVzSW5BaXIoIENvbXBvbmVudCBzZW5kZXIgKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50Um9vdDsKCXZhciByZW5kZXJpbmdQaXBlbGluZSA9IHNjZW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdQaXBlbGluZV9CYXNpYz4oIlJlbmRlcmluZyBQaXBlbGluZSIpOwoJdmFyIGVmZmVjdCA9IHJlbmRlcmluZ1BpcGVsaW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdFZmZlY3RfTWljcm9wYXJ0aWNsZXNJbkFpcj4oY2hlY2tDaGlsZHJlbjogdHJ1ZSk7CglpZiAoZWZmZWN0ICE9IG51bGwpCgl7CgkJaWYgKGN1cnJlbnRNaWNyb3BhcnRpY2xlc0luQWlyKQoJCXsKCQkJZWZmZWN0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMC43LCAwLjYpOwoJCQkvL2VmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuOCwgMC41KTsKCQkJZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMTU7CgkJfQoJCWVsc2UKCQl7CgkJCWlmIChjdXJyZW50TmlnaHQpCgkJCXsKCQkJCWVmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNzUsIDAuNzUsIDEpOwoJCQkJZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMDE1OwoJCQl9CgkJCWVsc2UKCQkJewoJCQkJLy9zaW11bGF0ZSBpbmRpcmVjdCBsaWdodGluZyBieSBtZWFucyBtaWNyb3BhcnRpY2xlcyBpbiBhaXIKCQkJCWVmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuOCwgMC43KTsKCQkJCS8vZWZmZWN0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMC44LCAwLjUpOwoJCQkJZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMDM7CgkJCX0KCgkJCS8vZWZmZWN0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMC43NSwgMC43NSwgMSk7CgkJCS8vZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMDE1OwoJCX0KCX0KfQoKdm9pZCBFeGl0RnJvbVZlaGljbGUoTmVvQXhpcy5HYW1lTW9kZSBnYW1lTW9kZSkKewoJdmFyIG9iaiA9IGdhbWVNb2RlLk9iamVjdENvbnRyb2xsZWRCeVBsYXllci5WYWx1ZSBhcyBWZWhpY2xlOwoJaWYgKG9iaiAhPSBudWxsKQoJewoJCXZhciBpbnB1dFByb2Nlc3NpbmcgPSBvYmouR2V0Q29tcG9uZW50PFZlaGljbGVJbnB1dFByb2Nlc3Npbmc+KCk7CgkJaWYgKGlucHV0UHJvY2Vzc2luZyAhPSBudWxsKQoJCQlpbnB1dFByb2Nlc3NpbmcuRXhpdEFsbE9iamVjdHNGcm9tVmVoaWNsZShnYW1lTW9kZSk7Cgl9Cn0KCnZvaWQgUHJvY2Vzc0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuR2FtZU1vZGUgc2VuZGVyLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQp7Cgl2YXIga2V5RG93biA9IG1lc3NhZ2UgYXMgSW5wdXRNZXNzYWdlS2V5RG93bjsKCWlmIChrZXlEb3duICE9IG51bGwpLy8mJiAhc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5Db250cm9sKSkKCXsKCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDEpCgkJewoJCQl2YXIgbWFuYWdlciA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxCdWlsZGluZ01hbmFnZXI+KCk7CgkJCWlmIChtYW5hZ2VyICE9IG51bGwpCgkJCXsKCQkJCW1hbmFnZXIuRGlzcGxheSA9ICFtYW5hZ2VyLkRpc3BsYXk7CgkJCQltYW5hZ2VyLkNvbGxpc2lvbiA9IG1hbmFnZXIuRGlzcGxheTsKCQkJfQoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EMikKCQl7CgkJCUV4aXRGcm9tVmVoaWNsZShzZW5kZXIpOwoKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uUGFya2VkVmVoaWNsZXMgPSBzeXN0ZW0uUGFya2VkVmVoaWNsZXMuVmFsdWUgIT0gMCA_IDAgOiA1MDAwOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EMykKCQl7CgkJCUV4aXRGcm9tVmVoaWNsZShzZW5kZXIpOwoKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uRmx5aW5nVmVoaWNsZXMgPSBzeXN0ZW0uRmx5aW5nVmVoaWNsZXMuVmFsdWUgIT0gMCA_IDAgOiA1MDA7CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ5KQoJCXsKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uU2ltdWxhdGVEeW5hbWljT2JqZWN0cyA9ICFzeXN0ZW0uU2ltdWxhdGVEeW5hbWljT2JqZWN0czsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDQpCgkJewoJCQlFeGl0RnJvbVZlaGljbGUoc2VuZGVyKTsKCgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLldhbGtpbmdQZWRlc3RyaWFucyA9IHN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnMuVmFsdWUgIT0gMCA_IDAgOiAxMDA7CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQwKQoJCXsKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uV2Fsa2luZ1BlZGVzdHJpYW5zTWFuYWdlVGFza3MgPSAhc3lzdGVtLldhbGtpbmdQZWRlc3RyaWFuc01hbmFnZVRhc2tzOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5ENSkKCQl7CgkJCXZhciBzY2VuZSA9IChTY2VuZSlzZW5kZXIuUGFyZW50Um9vdDsKCQkJdmFyIHJlbmRlcmluZ1BpcGVsaW5lID0gc2NlbmUuR2V0Q29tcG9uZW50PFJlbmRlcmluZ1BpcGVsaW5lPigiUmVuZGVyaW5nIFBpcGVsaW5lIik7CgkJCXZhciByZWZsZWN0aW9uID0gcmVuZGVyaW5nUGlwZWxpbmU_LkdldENvbXBvbmVudDxSZW5kZXJpbmdFZmZlY3RfUmVmbGVjdGlvbj4oY2hlY2tDaGlsZHJlbjogdHJ1ZSk7CgkJCXZhciBmb2cgPSBzY2VuZS5HZXRDb21wb25lbnQoIkZvZyIpIGFzIEZvZzsKCQkJdmFyIHByZWNpcGl0YXRpb24gPSByZW5kZXJpbmdQaXBlbGluZT8uR2V0Q29tcG9uZW50PFJlbmRlcmluZ0VmZmVjdF9QcmVjaXBpdGF0aW9uPihjaGVja0NoaWxkcmVuOiB0cnVlKTsKCQkJdmFyIHNvdW5kU291cmNlUmFpbiA9IHNjZW5lLkdldENvbXBvbmVudCgiU291bmQgU291cmNlIFJhaW4iKSBhcyBTb3VuZFNvdXJjZTsKCQkJdmFyIGNhbWVyYSA9IHNjZW5lLkdldENvbXBvbmVudDxDYW1lcmE+KCJDYW1lcmEgRGVmYXVsdCIpOwoJCQl2YXIgZGlyZWN0aW9uYWxMaWdodCA9IHNjZW5lLkdldENvbXBvbmVudCgiRGlyZWN0aW9uYWwgTGlnaHQiKSBhcyBMaWdodDsKCgkJCWN1cnJlbnRXZWF0aGVyKys7CgkJCWlmIChjdXJyZW50V2VhdGhlciA+IDIpCgkJCQljdXJyZW50V2VhdGhlciA9IDA7CgkJCS8vY3VycmVudFJhaW4gPSAhY3VycmVudFJhaW47CgoJCQl0cnkKCQkJewoJCQkJVXBkYXRlRm9nQW5kRmFyQ2xpcFBsYW5lKGZvZywgY2FtZXJhKTsKCgkJCQlzb3VuZFNvdXJjZVJhaW4uRW5hYmxlZCA9IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nOwoKCQkJCXNjZW5lLlByZWNpcGl0YXRpb25GYWxsaW5nID0gY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcgPyAxIDogMDsKCQkJCXNjZW5lLlByZWNpcGl0YXRpb25GYWxsZW4gPSAoY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcgfHwgY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxlbikgPyAxIDogMDsKCgkJCQkvL3ByZWNpcGl0YXRpb24uRW5hYmxlZCA9IGN1cnJlbnRSYWluOwoJCQkJLy9zb3VuZFNvdXJjZVJhaW4uRW5hYmxlZCA9IGN1cnJlbnRSYWluOwoJCQkJLy9zY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGluZyA9IGN1cnJlbnRSYWluID8gMSA6IDA7CgkJCQkvL3NjZW5lLlByZWNpcGl0YXRpb25GYWxsZW4gPSBjdXJyZW50UmFpbiA_IDEgOiAwOwoKCQkJCS8qCgkJCQkJCQkJaWYoY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcpLy9pZiAoY3VycmVudFJhaW4pCgkJCQkJCQkJewoJCQkJCQkJCQlkaXJlY3Rpb25hbExpZ2h0Lk1hc2sgPSBuZXcgUmVmZXJlbmNlTm9WYWx1ZShAIlNhbXBsZXNcQ2l0eSBEZW1vXFNraWVzXFJhaW4gY2xvdWRzIG1hc2tcUmFpbiBjbG91ZHMgbWFzay5qcGciKTsKCQkJCQkJCQkJZGlyZWN0aW9uYWxMaWdodC5NYXNrVHJhbnNmb3JtID0gbmV3IFRyYW5zZm9ybShWZWN0b3IzLlplcm8sIFF1YXRlcm5pb24uSWRlbnRpdHksIG5ldyBWZWN0b3IzKDAuMDA1LCAwLjAwNSwgMC4wMDUpKTsKCQkJCQkJCQl9CgkJCQkJCQkJZWxzZQoJCQkJCQkJCXsKCQkJCQkJCQkJZGlyZWN0aW9uYWxMaWdodC5NYXNrID0gbnVsbDsKCQkJCQkJCQl9CgkJCQkqLwoJCQl9CgkJCWNhdGNoIChFeGNlcHRpb24gZSkKCQkJewoJCQkJTG9nLldhcm5pbmcoZS5NZXNzYWdlKTsKCQkJfQoKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDYpCgkJewoJCQl2YXIgc2NlbmUgPSAoU2NlbmUpc2VuZGVyLlBhcmVudFJvb3Q7CgkJCXZhciBhbWJpZW50TGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkFtYmllbnQgTGlnaHQiKSBhcyBMaWdodDsKCQkJdmFyIGRpcmVjdGlvbmFsTGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRpcmVjdGlvbmFsIExpZ2h0IikgYXMgTGlnaHQ7CgkJCXZhciBzdHJlZXRMaWdodExpZ2h0cyA9IHNjZW5lLkdldENvbXBvbmVudCgiU3RyZWV0IGxpZ2h0IGxpZ2h0cyIpOwoJCQl2YXIgc2t5ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJTa3kiKSBhcyBTa3k7CgkJCS8vdmFyIGRheVNreSA9IHNjZW5lLkdldENvbXBvbmVudCgiRGF5IHNreSIpOwoJCQkvL3ZhciBuaWdodFNreSA9IHNjZW5lLkdldENvbXBvbmVudCgiTmlnaHQgc2t5Iik7CgkJCXZhciBmb2cgPSBzY2VuZS5HZXRDb21wb25lbnQoIkZvZyIpIGFzIEZvZzsKCQkJdmFyIGNhbWVyYSA9IHNjZW5lLkdldENvbXBvbmVudDxDYW1lcmE+KCJDYW1lcmEgRGVmYXVsdCIpOwoKCQkJY3VycmVudE5pZ2h0ID0gIWN1cnJlbnROaWdodDsKCgkJCXRyeQoJCQl7CgkJCQlzY2VuZS5UaW1lT2ZEYXkgPSBjdXJyZW50TmlnaHQgPyAwIDogMTI7IAoJCQkJYW1iaWVudExpZ2h0LkJyaWdodG5lc3MgPSBjdXJyZW50TmlnaHQgPyAyNTAwMCA6IDEwMDAwMDsKCQkJCWRpcmVjdGlvbmFsTGlnaHQuRW5hYmxlZCA9ICFjdXJyZW50TmlnaHQ7CgkJCQlzdHJlZXRMaWdodExpZ2h0cy5FbmFibGVkID0gY3VycmVudE5pZ2h0OwoJCQkJc2t5LlByb2NlZHVyYWxBdG1vc3BoZXJlID0gY3VycmVudE5pZ2h0ID8gMCA6IDE7CgkJCQkvL3NreS5Qcm9jZWR1cmFsU3RhcnMgPSBjdXJyZW50TmlnaHQgPyAxIDogMDsKCQkJCS8vc2t5Lk1vZGUgPSBjdXJyZW50TmlnaHQgPyBTa3kuTW9kZUVudW0uUmVzb3VyY2UgOiBTa3kuTW9kZUVudW0uUHJvY2VkdXJhbDsJCQkJCgkJCQkvLy8vc2t5LlByb2NlZHVyYWxJbnRlbnNpdHkgPSBjdXJyZW50TmlnaHQgPyAwIDogMTsKCQkJCS8vLy9kYXlTa3kuRW5hYmxlZCA9ICFjdXJyZW50TmlnaHQ7CgkJCQkvLy8vbmlnaHRTa3kuRW5hYmxlZCA9IGN1cnJlbnROaWdodDsKCQkJCVVwZGF0ZUZvZ0FuZEZhckNsaXBQbGFuZShmb2csIGNhbWVyYSk7CgkJCQlVcGRhdGVNaWNyb3BhcnRpY2xlc0luQWlyKHNlbmRlcik7CgkJCX0KCQkJY2F0Y2ggKEV4Y2VwdGlvbiBlKQoJCQl7CgkJCQlMb2cuV2FybmluZyhlLk1lc3NhZ2UpOwoJCQl9CgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EOCkKCQl7CgkJCUV4aXRGcm9tVmVoaWNsZShzZW5kZXIpOwoKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCXsKCQkJCWlmIChzeXN0ZW0uUGFya2VkVmVoaWNsZXNPYmplY3RNb2RlLlZhbHVlID09IFRyYWZmaWNTeXN0ZW0uT2JqZWN0TW9kZUVudW0uVmVoaWNsZUNvbXBvbmVudCkKCQkJCQlzeXN0ZW0uUGFya2VkVmVoaWNsZXNPYmplY3RNb2RlID0gVHJhZmZpY1N5c3RlbS5PYmplY3RNb2RlRW51bS5TdGF0aWNPYmplY3Q7CgkJCQllbHNlCgkJCQkJc3lzdGVtLlBhcmtlZFZlaGljbGVzT2JqZWN0TW9kZSA9IFRyYWZmaWNTeXN0ZW0uT2JqZWN0TW9kZUVudW0uVmVoaWNsZUNvbXBvbmVudDsKCQkJfQoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5ENykKCQl7CgkJCWN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA9ICFjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmc7CgoJCQl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50Um9vdDsKCQkJdmFyIHJlbmRlcmluZ1BpcGVsaW5lID0gc2NlbmUuR2V0Q29tcG9uZW50PFJlbmRlcmluZ1BpcGVsaW5lX0Jhc2ljPigiUmVuZGVyaW5nIFBpcGVsaW5lIik7CgkJCXZhciBjYW1lcmEgPSBzY2VuZS5HZXRDb21wb25lbnQ8Q2FtZXJhPigiQ2FtZXJhIERlZmF1bHQiKTsKCQkJdmFyIGZvZyA9IHNjZW5lLkdldENvbXBvbmVudCgiRm9nIikgYXMgRm9nOwoKCQkJLy9jYW1lcmEuRmFyQ2xpcFBsYW5lID0gY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID8gMjAwMCA6IDEwMDA7CgkJCXJlbmRlcmluZ1BpcGVsaW5lLk1pbmltdW1WaXNpYmxlU2l6ZU9mT2JqZWN0cyA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDIgOiA0OwoKCQkJcmVuZGVyaW5nUGlwZWxpbmUuU2hhZG93RGlyZWN0aW9uYWxEaXN0YW5jZSA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDYwMCA6IDIwMDsKCQkJcmVuZGVyaW5nUGlwZWxpbmUuU2hhZG93RGlyZWN0aW9uYWxMaWdodENhc2NhZGVzID0gY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID8gMyA6IDI7CgoJCQl0cnkKCQkJewoJCQkJVXBkYXRlRm9nQW5kRmFyQ2xpcFBsYW5lKGZvZywgY2FtZXJhKTsKCQkJfQoJCQljYXRjaCAoRXhjZXB0aW9uIGUpCgkJCXsKCQkJCUxvZy5XYXJuaW5nKGUuTWVzc2FnZSk7CgkJCX0KCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCgkJCS8qCgkJCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRSb290IGFzIFNjZW5lOwoJCQlpZiAoc2NlbmUgIT0gbnVsbCkKCQkJewoJCQkJaWYgKHNjZW5lLk9jdHJlZVRocmVhZGluZ01vZGUuVmFsdWUgPT0gT2N0cmVlQ29udGFpbmVyLlRocmVhZGluZ01vZGVFbnVtLkJhY2tncm91bmRUaHJlYWQpCgkJCQkJc2NlbmUuT2N0cmVlVGhyZWFkaW5nTW9kZSA9IE9jdHJlZUNvbnRhaW5lci5UaHJlYWRpbmdNb2RlRW51bS5TaW5nbGVUaHJlYWRlZDsKCQkJCWVsc2UKCQkJCQlzY2VuZS5PY3RyZWVUaHJlYWRpbmdNb2RlID0gT2N0cmVlQ29udGFpbmVyLlRocmVhZGluZ01vZGVFbnVtLkJhY2tncm91bmRUaHJlYWQ7CgkJCX0KCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCQkqLwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuQykKCQl7CgkJCWN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9ycyA9ICFjdXJyZW50UmFuZG9taXplU3RyZWV0TGlnaHRDb2xvcnM7CgkJCQoJCQl2YXIgbGlnaHRzID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50KCJTdHJlZXQgbGlnaHQgbGlnaHRzIik7CgkJCWlmKGxpZ2h0cyAhPSBudWxsKQoJCQl7CgkJCQl2YXIgcmFuZG9tID0gbmV3IEZhc3RSYW5kb20oKTsKCQkJCQoJCQkJZm9yZWFjaCh2YXIgbGlnaHQgaW4gbGlnaHRzLkdldENvbXBvbmVudHM8TGlnaHQ+KCkpCgkJCQl7CgkJCQkJaWYoY3VycmVudFJhbmRvbWl6ZVN0cmVldExpZ2h0Q29sb3JzKQoJCQkJCXsKCQkJCQkJdmFyIGNvbG9yID0gbGlnaHQuQ29sb3IuVmFsdWU7CgkJCQkJCXZhciBtYXggPSAwLjZmOy8vMC4yZjsKCQkJCQkJY29sb3IuUmVkICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCQkJCWNvbG9yLkdyZWVuICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCQkJCWNvbG9yLkJsdWUgKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJCQkJbGlnaHQuQ29sb3IgPSBjb2xvcjsKCQkJCQl9CgkJCQkJZWxzZQoJCQkJCXsKCQkJCQkJbGlnaHQuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAxLCAwLjcxMTAxOTYpOwoJCQkJCX0KCQkJCX0KCQkJfQkKCgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5NKQoJCXsKCQkJY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXIgPSAhY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXI7CgkJCVVwZGF0ZU1pY3JvcGFydGljbGVzSW5BaXIoc2VuZGVyKTsKCQkJCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJLyppZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuUCkKCQl7CgkJCWN1cnJlbnRSZWZsZWN0aW9uUHJvYmUgPSAhY3VycmVudFJlZmxlY3Rpb25Qcm9iZTsKCgkJCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRSb290OwoJCQlmb3JlYWNoICh2YXIgcHJvYmUgaW4gc2NlbmUuR2V0Q29tcG9uZW50czxSZWZsZWN0aW9uUHJvYmU+KCkpCgkJCQlwcm9iZS5SZWFsVGltZSA9IGN1cnJlbnRSZWZsZWN0aW9uUHJvYmU7CgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfSovCgl9Cn0KCnB1YmxpYyB2b2lkIEdhbWVNb2RlX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuR2FtZU1vZGUgc2VuZGVyLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQp7CglpZiAoIXNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuQ29udHJvbCkpCgkJUHJvY2Vzc0lucHV0TWVzc2FnZUV2ZW50KHNlbmRlciwgbWVzc2FnZSk7Cn0KCnB1YmxpYyB2b2lkIEdhbWVNb2RlX0VuYWJsZWRJblNpbXVsYXRpb24oTmVvQXhpcy5Db21wb25lbnQgb2JqKQp7CgkvLy8vYWN0aXZhdGUgbmlnaHQgbW9kZQoJLy9Qcm9jZXNzSW5wdXRNZXNzYWdlRXZlbnQoKEdhbWVNb2RlKW9iaiwgbmV3IElucHV0TWVzc2FnZUtleURvd24oRUtleXMuRDYpKTsKCQoJLy9yYW5kb21pemUgc3RyZWV0IGxpZ2h0cwoJdmFyIGxpZ2h0cyA9IG9iai5QYXJlbnRSb290LkdldENvbXBvbmVudCgiU3RyZWV0IGxpZ2h0IGxpZ2h0cyIpOwoJaWYobGlnaHRzICE9IG51bGwpCgl7CgkJdmFyIHJhbmRvbSA9IG5ldyBGYXN0UmFuZG9tKCk7CgkJCgkJZm9yZWFjaCh2YXIgbGlnaHQgaW4gbGlnaHRzLkdldENvbXBvbmVudHM8TGlnaHQ+KCkpCgkJewoJCQkvL3JhbmRvbWl6ZSByb3RhdGlvbgoJCQl2YXIgdHIgPSBsaWdodC5UcmFuc2Zvcm1WOwoJCQl0ciA9IHRyLlVwZGF0ZVJvdGF0aW9uKFF1YXRlcm5pb24uRnJvbVJvdGF0ZUJ5WihyYW5kb20uTmV4dChNYXRoLlBJICogMikpKTsKCQkJbGlnaHQuVHJhbnNmb3JtID0gdHI7CgovKgoJCQkvL3JhbmRvbWl6ZSBjb2xvcnMKCQkJdmFyIGNvbG9yID0gbGlnaHQuQ29sb3IuVmFsdWU7CgkJCXZhciBtYXggPSAwLjZmOy8vMC4yZjsKCQkJY29sb3IuUmVkICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCWNvbG9yLkdyZWVuICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCWNvbG9yLkJsdWUgKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJbGlnaHQuQ29sb3IgPSBjb2xvcjsKKi8JCQkKCQl9Cgl9CQp9")]
public class DynamicClassB46EB839772E90566017F591015C545B6B064B1B87042441F9431D587C0A69DB
{
    public NeoAxis.CSharpScript Owner;
    static bool currentFarDistanceRendering;
    static bool currentNight;
    static int currentWeather;
    static bool currentRandomizeStreetLightColors;
    static bool currentMicroparticlesInAir;
    //static bool currentReflectionProbe = true;
    const int Sunny = 0;
    const int RainFalling = 1;
    const int RainFallen = 2;
    void UpdateFogAndFarClipPlane(Fog fog, Camera camera)
    {
        fog.Enabled = !currentNight; // || currentRain;
        fog.Density = currentWeather == RainFalling ? 0.01 : 0.001; //fog.Density = currentRain ? 0.01 : 0.001;
        if (currentWeather == RainFalling) //if (currentRain)
            fog.AffectBackground = 1;
        else
            fog.AffectBackground = currentNight ? 0 : 0.5;
        if (currentNight)
            fog.Color = new ColorValue(0, 0, 0);
        else
            fog.Color = new ColorValue(0.4539608, 0.5160392, 0.6588235);
        if (fog.Enabled && fog.AffectBackground == 1)
            camera.FarClipPlane = 300;
        else
            camera.FarClipPlane = currentFarDistanceRendering ? 2000 : 1000;
    }

    void UpdateMicroparticlesInAir(Component sender)
    {
        var scene = sender.ParentRoot;
        var renderingPipeline = scene.GetComponent<RenderingPipeline_Basic>("Rendering Pipeline");
        var effect = renderingPipeline.GetComponent<RenderingEffect_MicroparticlesInAir>(checkChildren: true);
        if (effect != null)
        {
            if (currentMicroparticlesInAir)
            {
                effect.Color = new ColorValue(1, 0.7, 0.6);
                //effect.Color = new ColorValue(1, 0.8, 0.5);
                effect.Multiplier = 0.0015;
            }
            else
            {
                if (currentNight)
                {
                    effect.Color = new ColorValue(0.75, 0.75, 1);
                    effect.Multiplier = 0.00015;
                }
                else
                {
                    //simulate indirect lighting by means microparticles in air
                    effect.Color = new ColorValue(1, 0.8, 0.7);
                    //effect.Color = new ColorValue(1, 0.8, 0.5);
                    effect.Multiplier = 0.0003;
                }
            //effect.Color = new ColorValue(0.75, 0.75, 1);
            //effect.Multiplier = 0.00015;
            }
        }
    }

    void ExitFromVehicle(NeoAxis.GameMode gameMode)
    {
        var obj = gameMode.ObjectControlledByPlayer.Value as Vehicle;
        if (obj != null)
        {
            var inputProcessing = obj.GetComponent<VehicleInputProcessing>();
            if (inputProcessing != null)
                inputProcessing.ExitAllObjectsFromVehicle(gameMode);
        }
    }

    void ProcessInputMessageEvent(NeoAxis.GameMode sender, NeoAxis.InputMessage message)
    {
        var keyDown = message as InputMessageKeyDown;
        if (keyDown != null) //&& !sender.IsKeyPressed(EKeys.Control))
        {
            if (keyDown.Key == EKeys.D1)
            {
                var manager = sender.ParentRoot.GetComponent<BuildingManager>();
                if (manager != null)
                {
                    manager.Display = !manager.Display;
                    manager.Collision = manager.Display;
                }

                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D2)
            {
                ExitFromVehicle(sender);
                var system = sender.ParentRoot.GetComponent<TrafficSystem>();
                if (system != null)
                    system.ParkedVehicles = system.ParkedVehicles.Value != 0 ? 0 : 5000;
                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D3)
            {
                ExitFromVehicle(sender);
                var system = sender.ParentRoot.GetComponent<TrafficSystem>();
                if (system != null)
                    system.FlyingVehicles = system.FlyingVehicles.Value != 0 ? 0 : 500;
                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D9)
            {
                var system = sender.ParentRoot.GetComponent<TrafficSystem>();
                if (system != null)
                    system.SimulateDynamicObjects = !system.SimulateDynamicObjects;
                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D4)
            {
                ExitFromVehicle(sender);
                var system = sender.ParentRoot.GetComponent<TrafficSystem>();
                if (system != null)
                    system.WalkingPedestrians = system.WalkingPedestrians.Value != 0 ? 0 : 100;
                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D0)
            {
                var system = sender.ParentRoot.GetComponent<TrafficSystem>();
                if (system != null)
                    system.WalkingPedestriansManageTasks = !system.WalkingPedestriansManageTasks;
                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D5)
            {
                var scene = (Scene)sender.ParentRoot;
                var renderingPipeline = scene.GetComponent<RenderingPipeline>("Rendering Pipeline");
                var reflection = renderingPipeline?.GetComponent<RenderingEffect_Reflection>(checkChildren: true);
                var fog = scene.GetComponent("Fog") as Fog;
                var precipitation = renderingPipeline?.GetComponent<RenderingEffect_Precipitation>(checkChildren: true);
                var soundSourceRain = scene.GetComponent("Sound Source Rain") as SoundSource;
                var camera = scene.GetComponent<Camera>("Camera Default");
                var directionalLight = scene.GetComponent("Directional Light") as Light;
                currentWeather++;
                if (currentWeather > 2)
                    currentWeather = 0;
                //currentRain = !currentRain;
                try
                {
                    UpdateFogAndFarClipPlane(fog, camera);
                    soundSourceRain.Enabled = currentWeather == RainFalling;
                    scene.PrecipitationFalling = currentWeather == RainFalling ? 1 : 0;
                    scene.PrecipitationFallen = (currentWeather == RainFalling || currentWeather == RainFallen) ? 1 : 0;
                //precipitation.Enabled = currentRain;
                //soundSourceRain.Enabled = currentRain;
                //scene.PrecipitationFalling = currentRain ? 1 : 0;
                //scene.PrecipitationFallen = currentRain ? 1 : 0;
                /*
								if(currentWeather == RainFalling)//if (currentRain)
								{
									directionalLight.Mask = new ReferenceNoValue(@"Samples\City Demo\Skies\Rain clouds mask\Rain clouds mask.jpg");
									directionalLight.MaskTransform = new Transform(Vector3.Zero, Quaternion.Identity, new Vector3(0.005, 0.005, 0.005));
								}
								else
								{
									directionalLight.Mask = null;
								}
				*/
                }
                catch (Exception e)
                {
                    Log.Warning(e.Message);
                }

                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D6)
            {
                var scene = (Scene)sender.ParentRoot;
                var ambientLight = scene.GetComponent("Ambient Light") as Light;
                var directionalLight = scene.GetComponent("Directional Light") as Light;
                var streetLightLights = scene.GetComponent("Street light lights");
                var sky = scene.GetComponent("Sky") as Sky;
                //var daySky = scene.GetComponent("Day sky");
                //var nightSky = scene.GetComponent("Night sky");
                var fog = scene.GetComponent("Fog") as Fog;
                var camera = scene.GetComponent<Camera>("Camera Default");
                currentNight = !currentNight;
                try
                {
                    scene.TimeOfDay = currentNight ? 0 : 12;
                    ambientLight.Brightness = currentNight ? 25000 : 100000;
                    directionalLight.Enabled = !currentNight;
                    streetLightLights.Enabled = currentNight;
                    sky.ProceduralAtmosphere = currentNight ? 0 : 1;
                    //sky.ProceduralStars = currentNight ? 1 : 0;
                    //sky.Mode = currentNight ? Sky.ModeEnum.Resource : Sky.ModeEnum.Procedural;				
                    ////sky.ProceduralIntensity = currentNight ? 0 : 1;
                    ////daySky.Enabled = !currentNight;
                    ////nightSky.Enabled = currentNight;
                    UpdateFogAndFarClipPlane(fog, camera);
                    UpdateMicroparticlesInAir(sender);
                }
                catch (Exception e)
                {
                    Log.Warning(e.Message);
                }

                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D8)
            {
                ExitFromVehicle(sender);
                var system = sender.ParentRoot.GetComponent<TrafficSystem>();
                if (system != null)
                {
                    if (system.ParkedVehiclesObjectMode.Value == TrafficSystem.ObjectModeEnum.VehicleComponent)
                        system.ParkedVehiclesObjectMode = TrafficSystem.ObjectModeEnum.StaticObject;
                    else
                        system.ParkedVehiclesObjectMode = TrafficSystem.ObjectModeEnum.VehicleComponent;
                }

                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D7)
            {
                currentFarDistanceRendering = !currentFarDistanceRendering;
                var scene = sender.ParentRoot;
                var renderingPipeline = scene.GetComponent<RenderingPipeline_Basic>("Rendering Pipeline");
                var camera = scene.GetComponent<Camera>("Camera Default");
                var fog = scene.GetComponent("Fog") as Fog;
                //camera.FarClipPlane = currentFarDistanceRendering ? 2000 : 1000;
                renderingPipeline.MinimumVisibleSizeOfObjects = currentFarDistanceRendering ? 2 : 4;
                renderingPipeline.ShadowDirectionalDistance = currentFarDistanceRendering ? 600 : 200;
                renderingPipeline.ShadowDirectionalLightCascades = currentFarDistanceRendering ? 3 : 2;
                try
                {
                    UpdateFogAndFarClipPlane(fog, camera);
                }
                catch (Exception e)
                {
                    Log.Warning(e.Message);
                }

                message.Handled = true;
                return;
            /*
			var scene = sender.ParentRoot as Scene;
			if (scene != null)
			{
				if (scene.OctreeThreadingMode.Value == OctreeContainer.ThreadingModeEnum.BackgroundThread)
					scene.OctreeThreadingMode = OctreeContainer.ThreadingModeEnum.SingleThreaded;
				else
					scene.OctreeThreadingMode = OctreeContainer.ThreadingModeEnum.BackgroundThread;
			}
			message.Handled = true;
			return;
			*/
            }

            if (keyDown.Key == EKeys.C)
            {
                currentRandomizeStreetLightColors = !currentRandomizeStreetLightColors;
                var lights = sender.ParentRoot.GetComponent("Street light lights");
                if (lights != null)
                {
                    var random = new FastRandom();
                    foreach (var light in lights.GetComponents<Light>())
                    {
                        if (currentRandomizeStreetLightColors)
                        {
                            var color = light.Color.Value;
                            var max = 0.6f; //0.2f;
                            color.Red += random.Next(-max, max);
                            color.Green += random.Next(-max, max);
                            color.Blue += random.Next(-max, max);
                            light.Color = color;
                        }
                        else
                        {
                            light.Color = new ColorValue(1, 1, 0.7110196);
                        }
                    }
                }

                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.M)
            {
                currentMicroparticlesInAir = !currentMicroparticlesInAir;
                UpdateMicroparticlesInAir(sender);
                message.Handled = true;
                return;
            }
        /*if (keyDown.Key == EKeys.P)
		{
			currentReflectionProbe = !currentReflectionProbe;

			var scene = sender.ParentRoot;
			foreach (var probe in scene.GetComponents<ReflectionProbe>())
				probe.RealTime = currentReflectionProbe;

			message.Handled = true;
			return;
		}*/
        }
    }

    public void GameMode_InputMessageEvent(NeoAxis.GameMode sender, NeoAxis.InputMessage message)
    {
        if (!sender.IsKeyPressed(EKeys.Control))
            ProcessInputMessageEvent(sender, message);
    }

    public void GameMode_EnabledInSimulation(NeoAxis.Component obj)
    {
        ////activate night mode
        //ProcessInputMessageEvent((GameMode)obj, new InputMessageKeyDown(EKeys.D6));
        //randomize street lights
        var lights = obj.ParentRoot.GetComponent("Street light lights");
        if (lights != null)
        {
            var random = new FastRandom();
            foreach (var light in lights.GetComponents<Light>())
            {
                //randomize rotation
                var tr = light.TransformV;
                tr = tr.UpdateRotation(Quaternion.FromRotateByZ(random.Next(Math.PI * 2)));
                light.Transform = tr;
            /*
			//randomize colors
			var color = light.Color.Value;
			var max = 0.6f;//0.2f;
			color.Red += random.Next(-max, max);
			color.Green += random.Next(-max, max);
			color.Blue += random.Next(-max, max);
			light.Color = color;
*/
            }
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgRGVtb01vZGVfU2hvd0tleXNFdmVudChOZW9BeGlzLkRlbW9Nb2RlIHNlbmRlciwgU3lzdGVtLkNvbGxlY3Rpb25zLkdlbmVyaWMuTGlzdDxzdHJpbmc+IGxpbmVzKQp7Cgl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CglpZiAoc3lzdGVtID09IG51bGwpCgkJcmV0dXJuOwoJdmFyIHNjZW5lID0gc3lzdGVtLlBhcmVudFJvb3QgYXMgU2NlbmU7CglpZiAoc2NlbmUgPT0gbnVsbCkKCQlyZXR1cm47Cgl2YXIgcmVuZGVyaW5nUGlwZWxpbmUgPSBzY2VuZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nUGlwZWxpbmU+KCJSZW5kZXJpbmcgUGlwZWxpbmUiKTsKCWlmIChyZW5kZXJpbmdQaXBlbGluZSA9PSBudWxsKQoJCXJldHVybjsKCgl2YXIgZmFyRGlzdGFuY2VSZW5kZXJpbmcgPSByZW5kZXJpbmdQaXBlbGluZS5NaW5pbXVtVmlzaWJsZVNpemVPZk9iamVjdHMgPT0gMjsKCXZhciBmYXJEaXN0YW5jZVJlbmRlcmluZ1N0cmluZyA9IGZhckRpc3RhbmNlUmVuZGVyaW5nID8gIm9uIiA6ICJvZmYiOwoKCXZhciBwYXJrZWRWZWhpY2xlc0FzU3RhdGljID0gc3lzdGVtLlBhcmtlZFZlaGljbGVzT2JqZWN0TW9kZS5WYWx1ZSA9PSBUcmFmZmljU3lzdGVtLk9iamVjdE1vZGVFbnVtLlN0YXRpY09iamVjdDsKCXZhciBwYXJrZWRWZWhpY2xlc0FzU3RhdGljU3RyaW5nID0gcGFya2VkVmVoaWNsZXNBc1N0YXRpYyA_ICJvbiIgOiAib2ZmIjsKCgl2YXIgbXVsdGl0aHJlYWRlZFNjZW5lT2N0cmVlID0gc2NlbmUuT2N0cmVlVGhyZWFkaW5nTW9kZS5WYWx1ZSA9PSBPY3RyZWVDb250YWluZXIuVGhyZWFkaW5nTW9kZUVudW0uQmFja2dyb3VuZFRocmVhZDsKCXZhciBtdWx0aXRocmVhZGVkU2NlbmVPY3RyZWVTdHJpbmcgPSBtdWx0aXRocmVhZGVkU2NlbmVPY3RyZWUgPyAib24iIDogIm9mZiI7CgoJc3RyaW5nIHJhaW5TdGF0ZTsKCWlmIChzY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGluZyA+IDApCgkJcmFpblN0YXRlID0gImZhbGxpbmciOwoJZWxzZSBpZiAoc2NlbmUuUHJlY2lwaXRhdGlvbkZhbGxlbiA+IDApCgkJcmFpblN0YXRlID0gImZhbGxlbiI7CgllbHNlCgkJcmFpblN0YXRlID0gInN1bm55IjsKCgkvL3ZhciB3YWxraW5nUGVkZXN0cmlhbnNNYW5hZ2VUYXNrc1N0cmluZyA9IHN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnNNYW5hZ2VUYXNrcy5WYWx1ZSA_ICJvbiIgOiAib2ZmIjsKCglsaW5lcy5BZGQoIiIpOwoJbGluZXMuQWRkKCIxIC0gYnVpbGRpbmdzIik7CglsaW5lcy5BZGQoJCIyIC0gcGFya2VkIHZlaGljbGVzIC0ge3N5c3RlbS5HZXRQYXJrZWRWZWhpY2xlcygpLkNvdW50fSIpOwoJbGluZXMuQWRkKCQiMyAtIGZseWluZyB2ZWhpY2xlcyAtIHtzeXN0ZW0uR2V0Rmx5aW5nT2JqZWN0cygpLkNvdW50fSIpOwoJbGluZXMuQWRkKCQiNCAtIHdhbGtpbmcgcGVkZXN0cmlhbnMgLSB7c3lzdGVtLkdldFdhbGtpbmdQZWRlc3RyaWFucygpLkNvdW50fSIpOwoJbGluZXMuQWRkKCQiNSAtIHJhaW4gLSB7cmFpblN0YXRlfSIpOy8vIC0ge3dhbGtpbmdQZWRlc3RyaWFuc01hbmFnZVRhc2tzU3RyaW5nfSIpOwoJbGluZXMuQWRkKCQiNiAtIHRpbWUgb2YgZGF5Iik7Ly8gLSB7d2Fsa2luZ1BlZGVzdHJpYW5zTWFuYWdlVGFza3NTdHJpbmd9Iik7CglsaW5lcy5BZGQoJCI3IC0gZmFyIGRpc3RhbmNlIHJlbmRlcmluZyAtIHtmYXJEaXN0YW5jZVJlbmRlcmluZ1N0cmluZ30iKTsKCWxpbmVzLkFkZCgkIkMgLSByYW5kb21pemUgc3RyZWV0IGxpZ2h0IGNvbG9ycyIpOwoJbGluZXMuQWRkKCQiTSAtIG1pY3JvcGFydGljbGVzIGluIGFpciAoZHVzdCkiKTsKCS8vbGluZXMuQWRkKCQiUCAtIHJlYWwtdGltZSByZWZsZWN0aW9uIHByb2JlIik7CglsaW5lcy5BZGQoIiIpOwoJbGluZXMuQWRkKCQiOCAtIHBhcmtlZCB2ZWhpY2xlcyBhcyBzdGF0aWMgb2JqZWN0cyAtIHtwYXJrZWRWZWhpY2xlc0FzU3RhdGljU3RyaW5nfSIpOwoJbGluZXMuQWRkKCI5IC0gc2ltdWxhdGUgZmx5aW5nIHZlaGljbGVzIik7CglsaW5lcy5BZGQoJCIwIC0gYWN0aXZlIHdhbGtpbmcgcGVkZXN0cmlhbnMiKTsvLyAtIHt3YWxraW5nUGVkZXN0cmlhbnNNYW5hZ2VUYXNrc1N0cmluZ30iKTsKCS8vbGluZXMuQWRkKCQiMCAtIG11bHRpdGhyZWFkZWQgc2NlbmUgb2N0cmVlIC0ge211bHRpdGhyZWFkZWRTY2VuZU9jdHJlZVN0cmluZ30iKTsKfQo=")]
public class DynamicClass2DF7B4DD291979A88C71843ABCADCF979DCD0328C02F21A9E2BB60BE5CA88123
{
    public NeoAxis.CSharpScript Owner;
    public void DemoMode_ShowKeysEvent(NeoAxis.DemoMode sender, System.Collections.Generic.List<string> lines)
    {
        var system = sender.ParentRoot.GetComponent<TrafficSystem>();
        if (system == null)
            return;
        var scene = system.ParentRoot as Scene;
        if (scene == null)
            return;
        var renderingPipeline = scene.GetComponent<RenderingPipeline>("Rendering Pipeline");
        if (renderingPipeline == null)
            return;
        var farDistanceRendering = renderingPipeline.MinimumVisibleSizeOfObjects == 2;
        var farDistanceRenderingString = farDistanceRendering ? "on" : "off";
        var parkedVehiclesAsStatic = system.ParkedVehiclesObjectMode.Value == TrafficSystem.ObjectModeEnum.StaticObject;
        var parkedVehiclesAsStaticString = parkedVehiclesAsStatic ? "on" : "off";
        var multithreadedSceneOctree = scene.OctreeThreadingMode.Value == OctreeContainer.ThreadingModeEnum.BackgroundThread;
        var multithreadedSceneOctreeString = multithreadedSceneOctree ? "on" : "off";
        string rainState;
        if (scene.PrecipitationFalling > 0)
            rainState = "falling";
        else if (scene.PrecipitationFallen > 0)
            rainState = "fallen";
        else
            rainState = "sunny";
        //var walkingPedestriansManageTasksString = system.WalkingPedestriansManageTasks.Value ? "on" : "off";
        lines.Add("");
        lines.Add("1 - buildings");
        lines.Add($"2 - parked vehicles - {system.GetParkedVehicles().Count}");
        lines.Add($"3 - flying vehicles - {system.GetFlyingObjects().Count}");
        lines.Add($"4 - walking pedestrians - {system.GetWalkingPedestrians().Count}");
        lines.Add($"5 - rain - {rainState}"); // - {walkingPedestriansManageTasksString}");
        lines.Add($"6 - time of day"); // - {walkingPedestriansManageTasksString}");
        lines.Add($"7 - far distance rendering - {farDistanceRenderingString}");
        lines.Add($"C - randomize street light colors");
        lines.Add($"M - microparticles in air (dust)");
        //lines.Add($"P - real-time reflection probe");
        lines.Add("");
        lines.Add($"8 - parked vehicles as static objects - {parkedVehiclesAsStaticString}");
        lines.Add("9 - simulate flying vehicles");
        lines.Add($"0 - active walking pedestrians"); // - {walkingPedestriansManageTasksString}");
    //lines.Add($"0 - multithreaded scene octree - {multithreadedSceneOctreeString}");
    }
}

[CSharpScriptGeneratedAttribute("Y2xhc3MgX1RlbXB7Cn0=")]
public class DynamicClassDE8FFF4AC5357F29AC8BBDD6822A0D4AB8BF8DDC6BD03130D02DDFE04424217D
{
    public NeoAxis.CSharpScript Owner;
    class _Temp
    {
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7Cn0K")]
public class DynamicClass8F5DCD1D0C9E91FDE6F884D9D379CC22AFAD834109FB3BA2FD191ABCE6E381F6
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
    {
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7CgkvL2NyZWF0ZSBjaGFyYWN0ZXJzLCBwdXQgdGhlbSB0byBzZWF0cwoJCgkvLyEhISEKCQp9Cg==")]
public class DynamicClass62D7A0ABB268E7FF04BAB0C5D27A7AC4536F5621FD7174376458F2E45824E0E4
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
    {
    //create characters, put them to seats
    //!!!!
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7CgkvL2NyZWF0ZSBjaGFyYWN0ZXJzLCBwdXQgdGhlbSB0byBzZWF0cwoKCS8vISEhIQoJTG9nLkluZm8oImRmZGciKTsKCQp9Cg==")]
public class DynamicClassB3503E38BF3F034167FC9E4946E07981B60432520FF1B044DA81DB9B5CD5382C
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
    {
        //create characters, put them to seats
        //!!!!
        Log.Info("dfdg");
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7CgkvL2NyZWF0ZSBjaGFyYWN0ZXJzLCBwdXQgdGhlbSB0byBzZWF0cwoKCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsKCWZvcmVhY2ggKHZhciB2ZWhpY2xlIGluIHNjZW5lLkdldENvbXBvbmVudHM8VmVoaWNsZT4oKSkKCXsKCQlkbwoJCXsKCQkJdmFyIHNlYXRJbmRleCA9IHZlaGljbGUuR2V0RnJlZVNlYXQoKTsKCQkJCgkJCQoKCQl9CgkJd2hpbGUgKHRydWUpOwoJfQp9Cg==")]
public class DynamicClass8BC9664BE9EA4F812D4D45B174F1700108076CEF7241C00C0A66D399DF5FF1AA
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
    {
        //create characters, put them to seats
        var scene = sender.ParentScene;
        foreach (var vehicle in scene.GetComponents<Vehicle>())
        {
            do
            {
                var seatIndex = vehicle.GetFreeSeat();
            }
            while (true);
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7CgkvL3B1dCBjaGFyYWN0ZXJzIHRvIGZyZWUgc2VhdHMKCgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7Cgl2YXIgZ2FtZU1vZGUgPSAoR2FtZU1vZGUpc2NlbmUuR2V0R2FtZU1vZGUoKTsKCXZhciBwbGF5ZXJDaGFyYWN0ZXIgPSBpbml0aWF0b3IgYXMgQ2hhcmFjdGVyOwoJaWYocGxheWVyQ2hhcmFjdGVyID09IG51bGwpCgkJcmV0dXJuOwoJCglmb3JlYWNoICh2YXIgdmVoaWNsZSBpbiBzY2VuZS5HZXRDb21wb25lbnRzPFZlaGljbGU+KCkpCgl7CgkJZG8KCQl7CgkJCXZhciBzZWF0SW5kZXggPSB2ZWhpY2xlLkdldEZyZWVTZWF0KCk7CgkJCWlmIChzZWF0SW5kZXggPT0gLTEpCgkJCQlicmVhazsKCgkJCXZhciBjaGFyYWN0ZXIgPSBzY2VuZS5DcmVhdGVDb21wb25lbnQ8Q2hhcmFjdGVyPihlbmFibGVkOiBmYWxzZSk7CgkJCWNoYXJhY3Rlci5DaGFyYWN0ZXJUeXBlID0gcGxheWVyQ2hhcmFjdGVyLkNoYXJhY3RlclR5cGU7CgkJCWNoYXJhY3Rlci5FbmFibGVkID0gdHJ1ZTsKCgkJCXZlaGljbGUuUHV0T2JqZWN0VG9TZWF0KGdhbWVNb2RlLCBzZWF0SW5kZXgsIGNoYXJhY3Rlcik7CgkJfQoJCXdoaWxlICh0cnVlKTsKCX0KfQo=")]
public class DynamicClass47596BE560AB969DF915B2A3124EF91506D3294A50EAE81B5979350BB8AE2BFF
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
    {
        //put characters to free seats
        var scene = sender.ParentScene;
        var gameMode = (GameMode)scene.GetGameMode();
        var playerCharacter = initiator as Character;
        if (playerCharacter == null)
            return;
        foreach (var vehicle in scene.GetComponents<Vehicle>())
        {
            do
            {
                var seatIndex = vehicle.GetFreeSeat();
                if (seatIndex == -1)
                    break;
                var character = scene.CreateComponent<Character>(enabled: false);
                character.CharacterType = playerCharacter.CharacterType;
                character.Enabled = true;
                vehicle.PutObjectToSeat(gameMode, seatIndex, character);
            }
            while (true);
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7CgkvL3B1dCBjaGFyYWN0ZXJzIHRvIGZyZWUgc2VhdHMKCgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7Cgl2YXIgZ2FtZU1vZGUgPSAoR2FtZU1vZGUpc2NlbmUuR2V0R2FtZU1vZGUoKTsKCXZhciBwbGF5ZXJDaGFyYWN0ZXIgPSBpbml0aWF0b3IgYXMgQ2hhcmFjdGVyOwoJaWYocGxheWVyQ2hhcmFjdGVyID09IG51bGwpCgkJcmV0dXJuOwoJCglmb3JlYWNoICh2YXIgdmVoaWNsZSBpbiBzY2VuZS5HZXRDb21wb25lbnRzPFZlaGljbGU+KCkpCgl7CgkJZG8KCQl7CgkJCS8vZmluZCBhIGZyZWUgc2VhdAoJCQl2YXIgc2VhdEluZGV4ID0gdmVoaWNsZS5HZXRGcmVlU2VhdCgpOwoJCQlpZiAoc2VhdEluZGV4ID09IC0xKQoJCQkJYnJlYWs7CgoJCQl2YXIgY2hhcmFjdGVyID0gc2NlbmUuQ3JlYXRlQ29tcG9uZW50PENoYXJhY3Rlcj4oZW5hYmxlZDogZmFsc2UpOwoJCQljaGFyYWN0ZXIuQ2hhcmFjdGVyVHlwZSA9IHBsYXllckNoYXJhY3Rlci5DaGFyYWN0ZXJUeXBlOwoJCQljaGFyYWN0ZXIuRW5hYmxlZCA9IHRydWU7CgoJCQl2ZWhpY2xlLlB1dE9iamVjdFRvU2VhdChnYW1lTW9kZSwgc2VhdEluZGV4LCBjaGFyYWN0ZXIpOwoJCX0KCQl3aGlsZSAodHJ1ZSk7Cgl9Cn0K")]
public class DynamicClassCB6FE55301A151E14E85E4937CDD682ABBA8676D24B1CBB1EFADAF68FF2D6662
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
    {
        //put characters to free seats
        var scene = sender.ParentScene;
        var gameMode = (GameMode)scene.GetGameMode();
        var playerCharacter = initiator as Character;
        if (playerCharacter == null)
            return;
        foreach (var vehicle in scene.GetComponents<Vehicle>())
        {
            do
            {
                //find a free seat
                var seatIndex = vehicle.GetFreeSeat();
                if (seatIndex == -1)
                    break;
                var character = scene.CreateComponent<Character>(enabled: false);
                character.CharacterType = playerCharacter.CharacterType;
                character.Enabled = true;
                vehicle.PutObjectToSeat(gameMode, seatIndex, character);
            }
            while (true);
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7CgkvL3B1dCBjaGFyYWN0ZXJzIHRvIGZyZWUgc2VhdHMKCgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7Cgl2YXIgZ2FtZU1vZGUgPSAoR2FtZU1vZGUpc2NlbmUuR2V0R2FtZU1vZGUoKTsKCXZhciBwbGF5ZXJDaGFyYWN0ZXIgPSBpbml0aWF0b3IgYXMgQ2hhcmFjdGVyOwoJaWYocGxheWVyQ2hhcmFjdGVyID09IG51bGwpCgkJcmV0dXJuOwoJCglmb3JlYWNoICh2YXIgdmVoaWNsZSBpbiBzY2VuZS5HZXRDb21wb25lbnRzPFZlaGljbGU+KCkpCgl7CgkJZG8KCQl7CgkJCS8vZmluZCBhIGZyZWUgc2VhdAoJCQl2YXIgc2VhdEluZGV4ID0gdmVoaWNsZS5HZXRGcmVlU2VhdCgpOwoJCQlpZiAoc2VhdEluZGV4ID09IC0xKQoJCQkJYnJlYWs7CgoJCQkvL2NyZWF0ZSBhIG5ldyBjaGFyYWN0ZXIgd2l0aCB0aGUgdHlwZSBvZiB0aGUgcGxheWVyIGNoYXJhY3RlcnMKCQkJdmFyIGNoYXJhY3RlciA9IHNjZW5lLkNyZWF0ZUNvbXBvbmVudDxDaGFyYWN0ZXI+KGVuYWJsZWQ6IGZhbHNlKTsKCQkJY2hhcmFjdGVyLkNoYXJhY3RlclR5cGUgPSBwbGF5ZXJDaGFyYWN0ZXIuQ2hhcmFjdGVyVHlwZTsKCQkJY2hhcmFjdGVyLkVuYWJsZWQgPSB0cnVlOwoKCQkJdmVoaWNsZS5QdXRPYmplY3RUb1NlYXQoZ2FtZU1vZGUsIHNlYXRJbmRleCwgY2hhcmFjdGVyKTsKCQl9CgkJd2hpbGUgKHRydWUpOwoJfQp9Cg==")]
public class DynamicClass8B73CACDB1C9D9136484E76A0E2F424CF39E62D079FDFB56D9754700E3FF03A6
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
    {
        //put characters to free seats
        var scene = sender.ParentScene;
        var gameMode = (GameMode)scene.GetGameMode();
        var playerCharacter = initiator as Character;
        if (playerCharacter == null)
            return;
        foreach (var vehicle in scene.GetComponents<Vehicle>())
        {
            do
            {
                //find a free seat
                var seatIndex = vehicle.GetFreeSeat();
                if (seatIndex == -1)
                    break;
                //create a new character with the type of the player characters
                var character = scene.CreateComponent<Character>(enabled: false);
                character.CharacterType = playerCharacter.CharacterType;
                character.Enabled = true;
                vehicle.PutObjectToSeat(gameMode, seatIndex, character);
            }
            while (true);
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7CgkvL3B1dCBjaGFyYWN0ZXJzIHRvIGZyZWUgc2VhdHMKCgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7Cgl2YXIgZ2FtZU1vZGUgPSAoR2FtZU1vZGUpc2NlbmUuR2V0R2FtZU1vZGUoKTsKCXZhciBwbGF5ZXJDaGFyYWN0ZXIgPSBpbml0aWF0b3IgYXMgQ2hhcmFjdGVyOwoJaWYocGxheWVyQ2hhcmFjdGVyID09IG51bGwpCgkJcmV0dXJuOwoJCglmb3JlYWNoICh2YXIgdmVoaWNsZSBpbiBzY2VuZS5HZXRDb21wb25lbnRzPFZlaGljbGU+KCkpCgl7CgkJZG8KCQl7CgkJCS8vZmluZCBhIGZyZWUgc2VhdAoJCQl2YXIgc2VhdEluZGV4ID0gdmVoaWNsZS5HZXRGcmVlU2VhdCgpOwoJCQlpZiAoc2VhdEluZGV4ID09IC0xKQoJCQkJYnJlYWs7CgoJCQkvL2NyZWF0ZSBhIG5ldyBjaGFyYWN0ZXIgd2l0aCB0aGUgdHlwZSBvZiB0aGUgcGxheWVyIGNoYXJhY3RlcgoJCQl2YXIgY2hhcmFjdGVyID0gc2NlbmUuQ3JlYXRlQ29tcG9uZW50PENoYXJhY3Rlcj4oZW5hYmxlZDogZmFsc2UpOwoJCQljaGFyYWN0ZXIuQ2hhcmFjdGVyVHlwZSA9IHBsYXllckNoYXJhY3Rlci5DaGFyYWN0ZXJUeXBlOwoJCQljaGFyYWN0ZXIuRW5hYmxlZCA9IHRydWU7CgoJCQkvL3B1dCB0byB0aGUgc2VhdAoJCQl2ZWhpY2xlLlB1dE9iamVjdFRvU2VhdChnYW1lTW9kZSwgc2VhdEluZGV4LCBjaGFyYWN0ZXIpOwoJCX0KCQl3aGlsZSAodHJ1ZSk7Cgl9Cn0K")]
public class DynamicClassC37C2A4663070CAF2BEDF8B30D213F49F21AC52F99C74699C1C5A3606E3D8D8A
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender, NeoAxis.Component initiator)
    {
        //put characters to free seats
        var scene = sender.ParentScene;
        var gameMode = (GameMode)scene.GetGameMode();
        var playerCharacter = initiator as Character;
        if (playerCharacter == null)
            return;
        foreach (var vehicle in scene.GetComponents<Vehicle>())
        {
            do
            {
                //find a free seat
                var seatIndex = vehicle.GetFreeSeat();
                if (seatIndex == -1)
                    break;
                //create a new character with the type of the player character
                var character = scene.CreateComponent<Character>(enabled: false);
                character.CharacterType = playerCharacter.CharacterType;
                character.Enabled = true;
                //put to the seat
                vehicle.PutObjectToSeat(gameMode, seatIndex, character);
            }
            while (true);
        }
    }
}

[CSharpScriptGeneratedAttribute("Ym9vbCBNZXRob2QoKQp7CgkvL3ByZXZlbnQgZXhlY3V0aW9uIHRoZSBzY3JpcHQgaW4gdGhlIGVkaXRvciBhbmQgd2hlbiBsb2FkaW5nCglpZihFbmdpbmVBcHAuSXNTaW11bGF0aW9uICYmICFPd25lci5QYXJlbnRSb290LkhpZXJhcmNoeUNvbnRyb2xsZXIuTG9hZGluZykKCXsKCQkvL2dldCBjdXJyZW50IGludGVyYWN0aW9uCgkJdmFyIGludGVyYWN0aW9uID0gQ29udGludW91c0ludGVyYWN0aW9uLkxhdGVzdDsKCgkJLy9nZXQgYSBjaGFyYWN0ZXIgb2YgdGhlIHBsYXllcgoJCXZhciBwbGF5ZXJDaGFyYWN0ZXIgPSAoQ2hhcmFjdGVyKWludGVyYWN0aW9uLlNlY29uZFBhcnRpY2lwYW50LlZhbHVlOwoKCQkvL2NoZWNrcyBwbGF5ZXIncyBjaGFyYWN0ZXIgaGFzIGEga2V5CgkJdmFyIGl0ZW0gPSBwbGF5ZXJDaGFyYWN0ZXIuR2V0SXRlbUJ5UmVzb3VyY2VOYW1lKEAiQ29udGVudFxJdGVtc1xOZW9BeGlzXEtleVxLZXkuaXRlbXR5cGUiKTsKCQlpZiAoaXRlbSAhPSBudWxsKQoJCQlyZXR1cm4gdHJ1ZTsKCX0KCQoJcmV0dXJuIGZhbHNlOwp9Cg==")]
public class DynamicClassC8DC1C3DB3B186715E06B968E33628F7B0A030D8AEAC2C7DA78E94CFDA78DAFA
{
    public NeoAxis.CSharpScript Owner;
    bool Method()
    {
        //prevent execution the script in the editor and when loading
        if (EngineApp.IsSimulation && !Owner.ParentRoot.HierarchyController.Loading)
        {
            //get current interaction
            var interaction = ContinuousInteraction.Latest;
            //get a character of the player
            var playerCharacter = (Character)interaction.SecondParticipant.Value;
            //checks player's character has a key
            var item = playerCharacter.GetItemByResourceName(@"Content\Items\NeoAxis\Key\Key.itemtype");
            if (item != null)
                return true;
        }

        return false;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgRGVmYXVsdEdhdGUzX0NhblN3aXRjaChOZW9BeGlzLkdhdGUgc2VuZGVyLCBOZW9BeGlzLkNvbXBvbmVudCBpbml0aWF0b3IsIHJlZiBib29sIGNhblN3aXRjaCkKewoJLy9nZXQgYSBjaGFyYWN0ZXIgb2YgdGhlIHBsYXllcgoJdmFyIHBsYXllckNoYXJhY3RlciA9IGluaXRpYXRvciBhcyBDaGFyYWN0ZXI7CglpZiAocGxheWVyQ2hhcmFjdGVyICE9IG51bGwpCgl7CgkJLy9jaGVja3MgcGxheWVyJ3MgY2hhcmFjdGVyIGhhcyBhIGtleQoJCXZhciBpdGVtID0gcGxheWVyQ2hhcmFjdGVyLkdldEl0ZW1CeVJlc291cmNlTmFtZShAIkNvbnRlbnRcSXRlbXNcTmVvQXhpc1xLZXlcS2V5Lml0ZW10eXBlIik7CgkJaWYgKGl0ZW0gPT0gbnVsbCkKCQl7CgkJCS8vc2hvdyBzY3JlZW4gbWVzc2FnZQoJCQl2YXIgdGV4dCA9ICJZb3UgbmVlZCB0byBoYXZlIGEga2V5IHRvIG9wZW4gdGhlIGRvb3IuIjsJCQkKCQkJaWYoc2VuZGVyLk5ldHdvcmtJc1NlcnZlcikgLy9pZihTaW11bGF0aW9uQXBwU2VydmVyLlNlcnZlciAhPSBudWxsKQoJCQl7CgkJCQl2YXIgbmV0d29ya0xvZ2ljID0gTmV0d29ya0xvZ2ljVXRpbGl0eS5HZXROZXR3b3JrTG9naWMoaW5pdGlhdG9yKSBhcyBOZXR3b3JrTG9naWM7CgkJCQluZXR3b3JrTG9naWM_LlNlbmRTY3JlZW5NZXNzYWdlVG9DbGllbnRCeUNvbnRyb2xsZWRPYmplY3QoaW5pdGlhdG9yLCB0ZXh0LCBmYWxzZSk7CgkJCX0KCQkJZWxzZQoJCQkJU2NyZWVuTWVzc2FnZXMuQWRkKHRleHQpOwoJCQkJCgkJCS8vc2V0IGNhbid0IGludGVyYWN0IAoJCQljYW5Td2l0Y2ggPSBmYWxzZTsKCQkJCgkJCXJldHVybjsKCQl9Cgl9CQkJCn0K")]
public class DynamicClassD000E7A2E49FDEDA062A19347B66C0E8EC6B6ECA22A7F5CDE461C2A26385FAEF
{
    public NeoAxis.CSharpScript Owner;
    public void DefaultGate3_CanSwitch(NeoAxis.Gate sender, NeoAxis.Component initiator, ref bool canSwitch)
    {
        //get a character of the player
        var playerCharacter = initiator as Character;
        if (playerCharacter != null)
        {
            //checks player's character has a key
            var item = playerCharacter.GetItemByResourceName(@"Content\Items\NeoAxis\Key\Key.itemtype");
            if (item == null)
            {
                //show screen message
                var text = "You need to have a key to open the door.";
                if (sender.NetworkIsServer) //if(SimulationAppServer.Server != null)
                {
                    var networkLogic = NetworkLogicUtility.GetNetworkLogic(initiator) as NetworkLogic;
                    networkLogic?.SendScreenMessageToClientByControlledObject(initiator, text, false);
                }
                else
                    ScreenMessages.Add(text);
                //set can't interact 
                canSwitch = false;
                return;
            }
        }
    }
}

[CSharpScriptGeneratedAttribute("UXVhdGVybmlvbiBHZXRSb3RhdGlvbk9mZnNldCgpCnsKICAgIHZhciBzcGVlZCA9IC0wLjE7CiAgICB2YXIgbWF0ID0gTWF0cml4My5Gcm9tUm90YXRlQnlYKEVuZ2luZUFwcC5FbmdpbmVUaW1lICogc3BlZWQpOwogICAgcmV0dXJuIG1hdC5Ub1F1YXRlcm5pb24oKTsKfQ==")]
public class DynamicClass563517FC8ACE2898931CFF4AB12DA7F36758D3DB260769BF24436B5515203DB2
{
    public NeoAxis.CSharpScript Owner;
    Quaternion GetRotationOffset()
    {
        var speed = -0.1;
        var mat = Matrix3.FromRotateByX(EngineApp.EngineTime * speed);
        return mat.ToQuaternion();
    }
}

[CSharpScriptGeneratedAttribute("ZG91YmxlIE1ldGhvZCgpCnsKCXJldHVybiAtRW5naW5lQXBwLkVuZ2luZVRpbWUgLyA1Owp9Cg==")]
public class DynamicClassF465467C4CD278724A48D60460E9E95FFAFC5CD6C0687D6E088EB77C7475950B
{
    public NeoAxis.CSharpScript Owner;
    double Method()
    {
        return -EngineApp.EngineTime / 5;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uRG9fQ2xpY2soTmVvQXhpcy5VSUJ1dHRvbiBzZW5kZXIpCnsKCXZhciBwYXJlbnQgPSBzZW5kZXIuUGFyZW50OwoJdmFyIGxpbmsgPSBwYXJlbnQuUHJvcGVydHlHZXQ8c3RyaW5nPigiTGVhcm4gTGluayIpOwoJU3lzdGVtLkRpYWdub3N0aWNzLlByb2Nlc3MuU3RhcnQoIG5ldyBTeXN0ZW0uRGlhZ25vc3RpY3MuUHJvY2Vzc1N0YXJ0SW5mbyggbGluayApIHsgVXNlU2hlbGxFeGVjdXRlID0gdHJ1ZSB9ICk7Cn0K")]
public class DynamicClassC57B2BE7D7AA808AF5529252706739B4F11469DF256DBAA1D2816C9AC1B74DAD
{
    public NeoAxis.CSharpScript Owner;
    public void ButtonDo_Click(NeoAxis.UIButton sender)
    {
        var parent = sender.Parent;
        var link = parent.PropertyGet<string>("Learn Link");
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(link)
        {UseShellExecute = true});
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpCnsKCXZhciBjb250cm9sID0gKFVJQ29udHJvbClzZW5kZXI7Cgljb250cm9sLkNvbG9yTXVsdGlwbGllciA9IGNvbnRyb2wuUmVhZE9ubHkgPyBuZXcgQ29sb3JWYWx1ZSgwLjUsIDAuNSwgMC41KSA6IG5ldyBDb2xvclZhbHVlKDEsIDEsIDEpOwp9Cg==")]
public class DynamicClass6F764B3774F6B672AF43AF97D69EB9EDA0FAFF352641C6028B634491237B8FA2
{
    public NeoAxis.CSharpScript Owner;
    public void _UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var control = (UIControl)sender;
        control.ColorMultiplier = control.ReadOnly ? new ColorValue(0.5, 0.5, 0.5) : new ColorValue(1, 1, 1);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29udHJvbF9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQp7Cgl2YXIgdGFiQ29udHJvbCA9IHNlbmRlci5Db21wb25lbnRzWyJUYWIgQ29udHJvbCJdIGFzIFVJVGFiQ29udHJvbDsKCWlmKHRhYkNvbnRyb2wgPT0gbnVsbCkKCQlyZXR1cm47CgoJYm9vbCBJc0RvbmUoVUlDb250cm9sIGJsb2NrKQoJewoJCXZhciBjaGVjayA9IGJsb2NrLkdldENvbXBvbmVudDxVSUNoZWNrPigiQ2hlY2sgRG9uZSIpOwoJCXJldHVybiBjaGVjayAhPSBudWxsICYmIGNoZWNrLkNoZWNrZWQuVmFsdWUgPT0gVUlDaGVjay5DaGVja1ZhbHVlLkNoZWNrZWQ7IAoJfQoKCXZhciBwYWdlQmFzaWMgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBCYXNpYyIpIGFzIFVJQ29udHJvbDsKCWlmKHBhZ2VCYXNpYyAhPSBudWxsKQoJewoJCXZhciBibG9jazEgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7CgkJdmFyIGJsb2NrMiA9IHBhZ2VCYXNpYy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgMiIpIGFzIFVJQ29udHJvbDsKCQl2YXIgYmxvY2szID0gcGFnZUJhc2ljLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayAzIikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazQgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7CgkJdmFyIGJsb2NrNSA9IHBhZ2VCYXNpYy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgNSIpIGFzIFVJQ29udHJvbDsKCQl2YXIgYmxvY2s2ID0gcGFnZUJhc2ljLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA2IikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazcgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7CgkJdmFyIGJsb2NrOCA9IHBhZ2VCYXNpYy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgOCIpIGFzIFVJQ29udHJvbDsKCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOwoJCWJsb2NrOC5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2syKSAmJiAhSXNEb25lKGJsb2NrNSk7CgkJYmxvY2s1LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOwoJCWJsb2NrMy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2syKTsKCQlibG9jazQuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMik7CgkJYmxvY2s2LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazMpICYmICFJc0RvbmUoYmxvY2s1KSAmJiAhSXNEb25lKGJsb2NrOCk7CgkJYmxvY2s3LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpIHx8ICFJc0RvbmUoYmxvY2syKSB8fCAhSXNEb25lKGJsb2NrMykgfHwgIUlzRG9uZShibG9jazQpIHx8ICFJc0RvbmUoYmxvY2s1KSB8fCAhSXNEb25lKGJsb2NrNikgfHwgIUlzRG9uZShibG9jazgpOwoJCQoJCXZhciB0YWJCdXR0b25zID0gdGFiQ29udHJvbC5HZXRBbGxCdXR0b25zKCk7CgkJdGFiQnV0dG9uc1sxXS5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2s3KTsKCQl0YWJCdXR0b25zWzJdLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazcpOwoJfQoKCXZhciBwYWdlU2NyaXB0aW5nID0gdGFiQ29udHJvbC5HZXRDb21wb25lbnQoIlBhZ2UgU2NyaXB0aW5nIikgYXMgVUlDb250cm9sOwoJaWYocGFnZVNjcmlwdGluZyAhPSBudWxsKQoJewoJCXZhciBibG9jazEgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayAxIikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazIgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayAyIikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazMgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayAzIikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazQgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA0IikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazUgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA1IikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazYgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA2IikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazcgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA3IikgYXMgVUlDb250cm9sOwoKCQlibG9jazIuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7CgkJYmxvY2szLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazIpOwoJCWJsb2NrNC5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2sxKTsKCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7CgkJYmxvY2s2LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOwoJCWJsb2NrNy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2sxKTsJCQoJfQoKfQo=")]
public class DynamicClassD4F946B6894424BF42A3CDAC8A4AFBD47105E44A6ED33E70D18845F0928BC01C
{
    public NeoAxis.CSharpScript Owner;
    public void Control_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var tabControl = sender.Components["Tab Control"] as UITabControl;
        if (tabControl == null)
            return;
        bool IsDone(UIControl block)
        {
            var check = block.GetComponent<UICheck>("Check Done");
            return check != null && check.Checked.Value == UICheck.CheckValue.Checked;
        }

        var pageBasic = tabControl.GetComponent("Page Basic") as UIControl;
        if (pageBasic != null)
        {
            var block1 = pageBasic.GetComponent("LearningBlock 1") as UIControl;
            var block2 = pageBasic.GetComponent("LearningBlock 2") as UIControl;
            var block3 = pageBasic.GetComponent("LearningBlock 3") as UIControl;
            var block4 = pageBasic.GetComponent("LearningBlock 4") as UIControl;
            var block5 = pageBasic.GetComponent("LearningBlock 5") as UIControl;
            var block6 = pageBasic.GetComponent("LearningBlock 6") as UIControl;
            var block7 = pageBasic.GetComponent("LearningBlock 7") as UIControl;
            var block8 = pageBasic.GetComponent("LearningBlock 8") as UIControl;
            block2.ReadOnly = !IsDone(block1);
            block8.ReadOnly = !IsDone(block2) && !IsDone(block5);
            block5.ReadOnly = !IsDone(block1);
            block3.ReadOnly = !IsDone(block2);
            block4.ReadOnly = !IsDone(block2);
            block6.ReadOnly = !IsDone(block3) && !IsDone(block5) && !IsDone(block8);
            block7.ReadOnly = !IsDone(block1) || !IsDone(block2) || !IsDone(block3) || !IsDone(block4) || !IsDone(block5) || !IsDone(block6) || !IsDone(block8);
            var tabButtons = tabControl.GetAllButtons();
            tabButtons[1].ReadOnly = !IsDone(block7);
            tabButtons[2].ReadOnly = !IsDone(block7);
        }

        var pageScripting = tabControl.GetComponent("Page Scripting") as UIControl;
        if (pageScripting != null)
        {
            var block1 = pageScripting.GetComponent("LearningBlock 1") as UIControl;
            var block2 = pageScripting.GetComponent("LearningBlock 2") as UIControl;
            var block3 = pageScripting.GetComponent("LearningBlock 3") as UIControl;
            var block4 = pageScripting.GetComponent("LearningBlock 4") as UIControl;
            var block5 = pageScripting.GetComponent("LearningBlock 5") as UIControl;
            var block6 = pageScripting.GetComponent("LearningBlock 6") as UIControl;
            var block7 = pageScripting.GetComponent("LearningBlock 7") as UIControl;
            block2.ReadOnly = !IsDone(block1);
            block3.ReadOnly = !IsDone(block2);
            block4.ReadOnly = !IsDone(block1);
            block5.ReadOnly = !IsDone(block1);
            block6.ReadOnly = !IsDone(block1);
            block7.ReadOnly = !IsDone(block1);
        }
    }
}
}
#endif