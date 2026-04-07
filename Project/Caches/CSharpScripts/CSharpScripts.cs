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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uM0Qgc2VuZGVyLCBOZW9BeGlzLkNvbXBvbmVudCBpbml0aWF0b3IpCnsKCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsKCgl2YXIgZ3JvdW5kID0gc2NlbmUuR2V0Q29tcG9uZW50KCJHcm91bmQiKSBhcyBNZXNoSW5TcGFjZTsKCWlmIChncm91bmQgIT0gbnVsbCkKCXsKCQlpZiAoIWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwuUmVmZXJlbmNlU3BlY2lmaWVkKQoJCXsKCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IFJlZmVyZW5jZVV0aWxpdHkuTWFrZVJlZmVyZW5jZSgKCQkJCUAiQ29udGVudFxNYXRlcmlhbHNcQmFzaWMgTGlicmFyeVxDb25jcmV0ZVxDb25jcmV0ZSBGbG9vciAwMS5tYXRlcmlhbCIpOwoJCX0KCQllbHNlCgkJCWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwgPSBudWxsOwoJfQp9Cg==")]
public class DynamicClassDBCE9B8F695A6E48BA60A1D066D9A30C03003A9FDD6779AECEF75A1857CB52E4
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button3D sender, NeoAxis.Component initiator)
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uM0Qgc2VuZGVyLCBOZW9BeGlzLkNvbXBvbmVudCBpbml0aWF0b3IpCnsKCS8vcHV0IGNoYXJhY3RlcnMgdG8gZnJlZSBzZWF0cwoKCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsKCXZhciBnYW1lTW9kZSA9IChHYW1lTW9kZSlzY2VuZS5HZXRHYW1lTW9kZSgpOwoJdmFyIHBsYXllckNoYXJhY3RlciA9IGluaXRpYXRvciBhcyBDaGFyYWN0ZXI7CglpZihwbGF5ZXJDaGFyYWN0ZXIgPT0gbnVsbCkKCQlyZXR1cm47CgkKCWZvcmVhY2ggKHZhciB2ZWhpY2xlIGluIHNjZW5lLkdldENvbXBvbmVudHM8VmVoaWNsZT4oKSkKCXsKCQlkbwoJCXsKCQkJLy9maW5kIGEgZnJlZSBzZWF0CgkJCXZhciBzZWF0SW5kZXggPSB2ZWhpY2xlLkdldEZyZWVTZWF0KCk7CgkJCWlmIChzZWF0SW5kZXggPT0gLTEpCgkJCQlicmVhazsKCgkJCS8vY3JlYXRlIGEgbmV3IGNoYXJhY3RlciB3aXRoIHRoZSB0eXBlIG9mIHRoZSBwbGF5ZXIgY2hhcmFjdGVyCgkJCXZhciBjaGFyYWN0ZXIgPSBzY2VuZS5DcmVhdGVDb21wb25lbnQ8Q2hhcmFjdGVyPihlbmFibGVkOiBmYWxzZSk7CgkJCWNoYXJhY3Rlci5DaGFyYWN0ZXJUeXBlID0gcGxheWVyQ2hhcmFjdGVyLkNoYXJhY3RlclR5cGU7CgkJCWNoYXJhY3Rlci5FbmFibGVkID0gdHJ1ZTsKCgkJCS8vcHV0IHRvIHRoZSBzZWF0CgkJCXZlaGljbGUuUHV0T2JqZWN0VG9TZWF0KGdhbWVNb2RlLCBzZWF0SW5kZXgsIGNoYXJhY3Rlcik7CgkJfQoJCXdoaWxlICh0cnVlKTsKCX0KfQo=")]
public class DynamicClassAB9DC80EBE9EBBF02E537B1D06F78366840C54FB4EDD2639BB9D6BA523695D89
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button3D sender, NeoAxis.Component initiator)
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uM0Qgc2VuZGVyLCBOZW9BeGlzLkNvbXBvbmVudCBpbml0aWF0b3IpCnsKCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsKCgl2YXIgbGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkFtYmllbnQgTGlnaHQiKSBhcyBMaWdodDsKCWlmIChsaWdodCAhPSBudWxsKQoJCWxpZ2h0LkVuYWJsZWQgPSBzZW5kZXIuQWN0aXZhdGVkOwkKfQo=")]
public class DynamicClass376662EDDB01C70F333D545EE8F8B99E75F445C61DD42172FC04EA733D063F69
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button3D sender, NeoAxis.Component initiator)
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
}
#endif