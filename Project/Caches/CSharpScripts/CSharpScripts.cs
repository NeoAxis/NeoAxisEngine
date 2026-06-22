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

[CSharpScriptGeneratedAttribute("c3RhdGljIGJvb2wgY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nOwpzdGF0aWMgYm9vbCBjdXJyZW50TmlnaHQ7CnN0YXRpYyBpbnQgY3VycmVudFdlYXRoZXI7CnN0YXRpYyBib29sIGN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9yczsKc3RhdGljIGJvb2wgY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXI7Ci8vc3RhdGljIGJvb2wgY3VycmVudFJlZmxlY3Rpb25Qcm9iZSA9IHRydWU7Cgpjb25zdCBpbnQgU3VubnkgPSAwOwpjb25zdCBpbnQgUmFpbkZhbGxpbmcgPSAxOwpjb25zdCBpbnQgUmFpbkZhbGxlbiA9IDI7Cgp2b2lkIFVwZGF0ZUZvZ0FuZEZhckNsaXBQbGFuZShGb2cgZm9nLCBDYW1lcmEgY2FtZXJhKQp7Cglmb2cuRW5hYmxlZCA9ICFjdXJyZW50TmlnaHQ7Ly8gfHwgY3VycmVudFJhaW47Cglmb2cuRGVuc2l0eSA9IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nID8gMC4wMSA6IDAuMDAxOy8vZm9nLkRlbnNpdHkgPSBjdXJyZW50UmFpbiA_IDAuMDEgOiAwLjAwMTsKCglpZiAoY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcpLy9pZiAoY3VycmVudFJhaW4pCgkJZm9nLkFmZmVjdEJhY2tncm91bmQgPSAxOwoJZWxzZQoJCWZvZy5BZmZlY3RCYWNrZ3JvdW5kID0gY3VycmVudE5pZ2h0ID8gMCA6IDAuNTsKCglpZiAoY3VycmVudE5pZ2h0KQoJCWZvZy5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAsIDAsIDApOwoJZWxzZQoJCWZvZy5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNDUzOTYwOCwgMC41MTYwMzkyLCAwLjY1ODgyMzUpOwoKCWlmIChmb2cuRW5hYmxlZCAmJiBmb2cuQWZmZWN0QmFja2dyb3VuZCA9PSAxKQoJCWNhbWVyYS5GYXJDbGlwUGxhbmUgPSAzMDA7CgllbHNlCgkJY2FtZXJhLkZhckNsaXBQbGFuZSA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDIwMDAgOiAxMDAwOwp9Cgp2b2lkIFVwZGF0ZU1pY3JvcGFydGljbGVzSW5BaXIoIENvbXBvbmVudCBzZW5kZXIgKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50Um9vdDsKCXZhciByZW5kZXJpbmdQaXBlbGluZSA9IHNjZW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdQaXBlbGluZV9CYXNpYz4oIlJlbmRlcmluZyBQaXBlbGluZSIpOwoJdmFyIGVmZmVjdCA9IHJlbmRlcmluZ1BpcGVsaW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdFZmZlY3RfTWljcm9wYXJ0aWNsZXNJbkFpcj4oY2hlY2tDaGlsZHJlbjogdHJ1ZSk7CglpZiAoZWZmZWN0ICE9IG51bGwpCgl7CgkJaWYgKGN1cnJlbnRNaWNyb3BhcnRpY2xlc0luQWlyKQoJCXsKCQkJZWZmZWN0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMC43LCAwLjYpOwoJCQkvL2VmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuOCwgMC41KTsKCQkJZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMTU7CgkJfQoJCWVsc2UKCQl7CgkJCWlmIChjdXJyZW50TmlnaHQpCgkJCXsKCQkJCWVmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNzUsIDAuNzUsIDEpOwoJCQkJZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMDE1OwoJCQl9CgkJCWVsc2UKCQkJewoJCQkJLy9zaW11bGF0ZSBpbmRpcmVjdCBsaWdodGluZyBieSBtZWFucyBtaWNyb3BhcnRpY2xlcyBpbiBhaXIKCQkJCWVmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuOCwgMC43KTsKCQkJCS8vZWZmZWN0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMC44LCAwLjUpOwoJCQkJZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMDM7CgkJCX0KCgkJCS8vZWZmZWN0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMC43NSwgMC43NSwgMSk7CgkJCS8vZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMDE1OwoJCX0KCX0KfQoKdm9pZCBFeGl0RnJvbVZlaGljbGUoTmVvQXhpcy5HYW1lTW9kZSBnYW1lTW9kZSkKewoJdmFyIG9iaiA9IGdhbWVNb2RlLk9iamVjdENvbnRyb2xsZWRCeVBsYXllci5WYWx1ZSBhcyBWZWhpY2xlOwoJaWYgKG9iaiAhPSBudWxsKQoJewoJCXZhciBpbnB1dFByb2Nlc3NpbmcgPSBvYmouR2V0Q29tcG9uZW50PFZlaGljbGVJbnB1dFByb2Nlc3Npbmc+KCk7CgkJaWYgKGlucHV0UHJvY2Vzc2luZyAhPSBudWxsKQoJCQlpbnB1dFByb2Nlc3NpbmcuRXhpdEFsbE9iamVjdHNGcm9tVmVoaWNsZShnYW1lTW9kZSk7Cgl9Cn0KCnZvaWQgUHJvY2Vzc0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuR2FtZU1vZGUgc2VuZGVyLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQp7Cgl2YXIga2V5RG93biA9IG1lc3NhZ2UgYXMgSW5wdXRNZXNzYWdlS2V5RG93bjsKCWlmIChrZXlEb3duICE9IG51bGwpLy8mJiAhc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5Db250cm9sKSkKCXsKCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDEpCgkJewoJCQl2YXIgbWFuYWdlciA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxCdWlsZGluZ01hbmFnZXI+KCk7CgkJCWlmIChtYW5hZ2VyICE9IG51bGwpCgkJCXsKCQkJCW1hbmFnZXIuRGlzcGxheSA9ICFtYW5hZ2VyLkRpc3BsYXk7CgkJCQltYW5hZ2VyLkNvbGxpc2lvbiA9IG1hbmFnZXIuRGlzcGxheTsKCQkJfQoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EMikKCQl7CgkJCUV4aXRGcm9tVmVoaWNsZShzZW5kZXIpOwoKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uUGFya2VkVmVoaWNsZXMgPSBzeXN0ZW0uUGFya2VkVmVoaWNsZXMuVmFsdWUgIT0gMCA_IDAgOiA1MDAwOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EMykKCQl7CgkJCUV4aXRGcm9tVmVoaWNsZShzZW5kZXIpOwoKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uRmx5aW5nVmVoaWNsZXMgPSBzeXN0ZW0uRmx5aW5nVmVoaWNsZXMuVmFsdWUgIT0gMCA_IDAgOiA1MDA7CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ5KQoJCXsKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uU2ltdWxhdGVEeW5hbWljT2JqZWN0cyA9ICFzeXN0ZW0uU2ltdWxhdGVEeW5hbWljT2JqZWN0czsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDQpCgkJewoJCQlFeGl0RnJvbVZlaGljbGUoc2VuZGVyKTsKCgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLldhbGtpbmdQZWRlc3RyaWFucyA9IHN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnMuVmFsdWUgIT0gMCA_IDAgOiAxMDA7CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQwKQoJCXsKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uV2Fsa2luZ1BlZGVzdHJpYW5zTWFuYWdlVGFza3MgPSAhc3lzdGVtLldhbGtpbmdQZWRlc3RyaWFuc01hbmFnZVRhc2tzOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5ENSkKCQl7CgkJCXZhciBzY2VuZSA9IChTY2VuZSlzZW5kZXIuUGFyZW50Um9vdDsKCQkJdmFyIHJlbmRlcmluZ1BpcGVsaW5lID0gc2NlbmUuR2V0Q29tcG9uZW50PFJlbmRlcmluZ1BpcGVsaW5lPigiUmVuZGVyaW5nIFBpcGVsaW5lIik7CgkJCXZhciByZWZsZWN0aW9uID0gcmVuZGVyaW5nUGlwZWxpbmU_LkdldENvbXBvbmVudDxSZW5kZXJpbmdFZmZlY3RfUmVmbGVjdGlvbj4oY2hlY2tDaGlsZHJlbjogdHJ1ZSk7CgkJCXZhciBmb2cgPSBzY2VuZS5HZXRDb21wb25lbnQoIkZvZyIpIGFzIEZvZzsKCQkJdmFyIHByZWNpcGl0YXRpb24gPSByZW5kZXJpbmdQaXBlbGluZT8uR2V0Q29tcG9uZW50PFJlbmRlcmluZ0VmZmVjdF9QcmVjaXBpdGF0aW9uPihjaGVja0NoaWxkcmVuOiB0cnVlKTsKCQkJdmFyIHNvdW5kU291cmNlUmFpbiA9IHNjZW5lLkdldENvbXBvbmVudCgiU291bmQgU291cmNlIFJhaW4iKSBhcyBTb3VuZFNvdXJjZTsKCQkJdmFyIGNhbWVyYSA9IHNjZW5lLkdldENvbXBvbmVudDxDYW1lcmE+KCJDYW1lcmEgRGVmYXVsdCIpOwoJCQl2YXIgZGlyZWN0aW9uYWxMaWdodCA9IHNjZW5lLkdldENvbXBvbmVudCgiRGlyZWN0aW9uYWwgTGlnaHQiKSBhcyBMaWdodDsKCgkJCWN1cnJlbnRXZWF0aGVyKys7CgkJCWlmIChjdXJyZW50V2VhdGhlciA+IDIpCgkJCQljdXJyZW50V2VhdGhlciA9IDA7CgkJCS8vY3VycmVudFJhaW4gPSAhY3VycmVudFJhaW47CgoJCQl0cnkKCQkJewoJCQkJVXBkYXRlRm9nQW5kRmFyQ2xpcFBsYW5lKGZvZywgY2FtZXJhKTsKCgkJCQlzb3VuZFNvdXJjZVJhaW4uRW5hYmxlZCA9IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nOwoKCQkJCXNjZW5lLlByZWNpcGl0YXRpb25GYWxsaW5nID0gY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcgPyAxIDogMDsKCQkJCXNjZW5lLlByZWNpcGl0YXRpb25GYWxsZW4gPSAoY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcgfHwgY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxlbikgPyAxIDogMDsKCgkJCQkvL3ByZWNpcGl0YXRpb24uRW5hYmxlZCA9IGN1cnJlbnRSYWluOwoJCQkJLy9zb3VuZFNvdXJjZVJhaW4uRW5hYmxlZCA9IGN1cnJlbnRSYWluOwoJCQkJLy9zY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGluZyA9IGN1cnJlbnRSYWluID8gMSA6IDA7CgkJCQkvL3NjZW5lLlByZWNpcGl0YXRpb25GYWxsZW4gPSBjdXJyZW50UmFpbiA_IDEgOiAwOwoKCQkJCS8qCgkJCQkJCQkJaWYoY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcpLy9pZiAoY3VycmVudFJhaW4pCgkJCQkJCQkJewoJCQkJCQkJCQlkaXJlY3Rpb25hbExpZ2h0Lk1hc2sgPSBuZXcgUmVmZXJlbmNlTm9WYWx1ZShAIlNhbXBsZXNcQ2l0eSBEZW1vXFNraWVzXFJhaW4gY2xvdWRzIG1hc2tcUmFpbiBjbG91ZHMgbWFzay5qcGciKTsKCQkJCQkJCQkJZGlyZWN0aW9uYWxMaWdodC5NYXNrVHJhbnNmb3JtID0gbmV3IFRyYW5zZm9ybShWZWN0b3IzLlplcm8sIFF1YXRlcm5pb24uSWRlbnRpdHksIG5ldyBWZWN0b3IzKDAuMDA1LCAwLjAwNSwgMC4wMDUpKTsKCQkJCQkJCQl9CgkJCQkJCQkJZWxzZQoJCQkJCQkJCXsKCQkJCQkJCQkJZGlyZWN0aW9uYWxMaWdodC5NYXNrID0gbnVsbDsKCQkJCQkJCQl9CgkJCQkqLwoJCQl9CgkJCWNhdGNoIChFeGNlcHRpb24gZSkKCQkJewoJCQkJTG9nLldhcm5pbmcoZS5NZXNzYWdlKTsKCQkJfQoKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDYpCgkJewoJCQl2YXIgc2NlbmUgPSAoU2NlbmUpc2VuZGVyLlBhcmVudFJvb3Q7CgkJCXZhciBhbWJpZW50TGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkFtYmllbnQgTGlnaHQiKSBhcyBMaWdodDsKCQkJdmFyIGRpcmVjdGlvbmFsTGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRpcmVjdGlvbmFsIExpZ2h0IikgYXMgTGlnaHQ7CgkJCXZhciBzdHJlZXRMaWdodExpZ2h0cyA9IHNjZW5lLkdldENvbXBvbmVudCgiU3RyZWV0IGxpZ2h0IGxpZ2h0cyIpOwoJCQl2YXIgc2t5ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJTa3kiKSBhcyBTa3k7CgkJCS8vdmFyIGRheVNreSA9IHNjZW5lLkdldENvbXBvbmVudCgiRGF5IHNreSIpOwoJCQkvL3ZhciBuaWdodFNreSA9IHNjZW5lLkdldENvbXBvbmVudCgiTmlnaHQgc2t5Iik7CgkJCXZhciBmb2cgPSBzY2VuZS5HZXRDb21wb25lbnQoIkZvZyIpIGFzIEZvZzsKCQkJdmFyIGNhbWVyYSA9IHNjZW5lLkdldENvbXBvbmVudDxDYW1lcmE+KCJDYW1lcmEgRGVmYXVsdCIpOwoKCQkJY3VycmVudE5pZ2h0ID0gIWN1cnJlbnROaWdodDsKCgkJCXRyeQoJCQl7CgkJCQlzY2VuZS5UaW1lT2ZEYXkgPSBjdXJyZW50TmlnaHQgPyAwIDogMTI7CgkJCQlhbWJpZW50TGlnaHQuQnJpZ2h0bmVzcyA9IGN1cnJlbnROaWdodCA_IDI1MDAwIDogMzAwMDA7Ly8xMDAwMDA7CgkJCQlkaXJlY3Rpb25hbExpZ2h0LkVuYWJsZWQgPSAhY3VycmVudE5pZ2h0OwoJCQkJc3RyZWV0TGlnaHRMaWdodHMuRW5hYmxlZCA9IGN1cnJlbnROaWdodDsKCQkJCXNreS5Qcm9jZWR1cmFsQXRtb3NwaGVyZSA9IGN1cnJlbnROaWdodCA_IDAgOiAxOwoJCQkJLy9za3kuUHJvY2VkdXJhbFN0YXJzID0gY3VycmVudE5pZ2h0ID8gMSA6IDA7CgkJCQkvL3NreS5Nb2RlID0gY3VycmVudE5pZ2h0ID8gU2t5Lk1vZGVFbnVtLlJlc291cmNlIDogU2t5Lk1vZGVFbnVtLlByb2NlZHVyYWw7CQkJCQoJCQkJLy8vL3NreS5Qcm9jZWR1cmFsSW50ZW5zaXR5ID0gY3VycmVudE5pZ2h0ID8gMCA6IDE7CgkJCQkvLy8vZGF5U2t5LkVuYWJsZWQgPSAhY3VycmVudE5pZ2h0OwoJCQkJLy8vL25pZ2h0U2t5LkVuYWJsZWQgPSBjdXJyZW50TmlnaHQ7CgkJCQlVcGRhdGVGb2dBbmRGYXJDbGlwUGxhbmUoZm9nLCBjYW1lcmEpOwoJCQkJVXBkYXRlTWljcm9wYXJ0aWNsZXNJbkFpcihzZW5kZXIpOwoJCQl9CgkJCWNhdGNoIChFeGNlcHRpb24gZSkKCQkJewoJCQkJTG9nLldhcm5pbmcoZS5NZXNzYWdlKTsKCQkJfQoKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDgpCgkJewoJCQlFeGl0RnJvbVZlaGljbGUoc2VuZGVyKTsKCgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQl7CgkJCQlpZiAoc3lzdGVtLlBhcmtlZFZlaGljbGVzT2JqZWN0TW9kZS5WYWx1ZSA9PSBUcmFmZmljU3lzdGVtLk9iamVjdE1vZGVFbnVtLlZlaGljbGVDb21wb25lbnQpCgkJCQkJc3lzdGVtLlBhcmtlZFZlaGljbGVzT2JqZWN0TW9kZSA9IFRyYWZmaWNTeXN0ZW0uT2JqZWN0TW9kZUVudW0uU3RhdGljT2JqZWN0OwoJCQkJZWxzZQoJCQkJCXN5c3RlbS5QYXJrZWRWZWhpY2xlc09iamVjdE1vZGUgPSBUcmFmZmljU3lzdGVtLk9iamVjdE1vZGVFbnVtLlZlaGljbGVDb21wb25lbnQ7CgkJCX0KCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDcpCgkJewoJCQljdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmcgPSAhY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nOwoKCQkJdmFyIHNjZW5lID0gc2VuZGVyLlBhcmVudFJvb3Q7CgkJCXZhciByZW5kZXJpbmdQaXBlbGluZSA9IHNjZW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdQaXBlbGluZV9CYXNpYz4oIlJlbmRlcmluZyBQaXBlbGluZSIpOwoJCQl2YXIgY2FtZXJhID0gc2NlbmUuR2V0Q29tcG9uZW50PENhbWVyYT4oIkNhbWVyYSBEZWZhdWx0Iik7CgkJCXZhciBmb2cgPSBzY2VuZS5HZXRDb21wb25lbnQoIkZvZyIpIGFzIEZvZzsKCgkJCS8vY2FtZXJhLkZhckNsaXBQbGFuZSA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDIwMDAgOiAxMDAwOwoJCQlyZW5kZXJpbmdQaXBlbGluZS5NaW5pbXVtVmlzaWJsZVNpemVPZk9iamVjdHMgPSBjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmcgPyAyIDogNDsKCgkJCXJlbmRlcmluZ1BpcGVsaW5lLlNoYWRvd0RpcmVjdGlvbmFsRGlzdGFuY2UgPSBjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmcgPyA2MDAgOiAyMDA7CgkJCXJlbmRlcmluZ1BpcGVsaW5lLlNoYWRvd0RpcmVjdGlvbmFsTGlnaHRDYXNjYWRlcyA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDMgOiAyOwoKCQkJdHJ5CgkJCXsKCQkJCVVwZGF0ZUZvZ0FuZEZhckNsaXBQbGFuZShmb2csIGNhbWVyYSk7CgkJCX0KCQkJY2F0Y2ggKEV4Y2VwdGlvbiBlKQoJCQl7CgkJCQlMb2cuV2FybmluZyhlLk1lc3NhZ2UpOwoJCQl9CgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgoJCQkvKgoJCQl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50Um9vdCBhcyBTY2VuZTsKCQkJaWYgKHNjZW5lICE9IG51bGwpCgkJCXsKCQkJCWlmIChzY2VuZS5PY3RyZWVUaHJlYWRpbmdNb2RlLlZhbHVlID09IE9jdHJlZUNvbnRhaW5lci5UaHJlYWRpbmdNb2RlRW51bS5CYWNrZ3JvdW5kVGhyZWFkKQoJCQkJCXNjZW5lLk9jdHJlZVRocmVhZGluZ01vZGUgPSBPY3RyZWVDb250YWluZXIuVGhyZWFkaW5nTW9kZUVudW0uU2luZ2xlVGhyZWFkZWQ7CgkJCQllbHNlCgkJCQkJc2NlbmUuT2N0cmVlVGhyZWFkaW5nTW9kZSA9IE9jdHJlZUNvbnRhaW5lci5UaHJlYWRpbmdNb2RlRW51bS5CYWNrZ3JvdW5kVGhyZWFkOwoJCQl9CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQkJKi8KCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkMpCgkJewoJCQljdXJyZW50UmFuZG9taXplU3RyZWV0TGlnaHRDb2xvcnMgPSAhY3VycmVudFJhbmRvbWl6ZVN0cmVldExpZ2h0Q29sb3JzOwoJCQkKCQkJdmFyIGxpZ2h0cyA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudCgiU3RyZWV0IGxpZ2h0IGxpZ2h0cyIpOwoJCQlpZihsaWdodHMgIT0gbnVsbCkKCQkJewoJCQkJdmFyIHJhbmRvbSA9IG5ldyBGYXN0UmFuZG9tKCk7CgkJCQkKCQkJCWZvcmVhY2godmFyIGxpZ2h0IGluIGxpZ2h0cy5HZXRDb21wb25lbnRzPExpZ2h0PigpKQoJCQkJewoJCQkJCWlmKGN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9ycykKCQkJCQl7CgkJCQkJCXZhciBjb2xvciA9IGxpZ2h0LkNvbG9yLlZhbHVlOwoJCQkJCQl2YXIgbWF4ID0gMC42ZjsvLzAuMmY7CgkJCQkJCWNvbG9yLlJlZCArPSByYW5kb20uTmV4dCgtbWF4LCBtYXgpOwoJCQkJCQljb2xvci5HcmVlbiArPSByYW5kb20uTmV4dCgtbWF4LCBtYXgpOwoJCQkJCQljb2xvci5CbHVlICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCQkJCWxpZ2h0LkNvbG9yID0gY29sb3I7CgkJCQkJfQoJCQkJCWVsc2UKCQkJCQl7CgkJCQkJCWxpZ2h0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMSwgMC43MTEwMTk2KTsKCQkJCQl9CgkJCQl9CgkJCX0JCgoKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuTSkKCQl7CgkJCWN1cnJlbnRNaWNyb3BhcnRpY2xlc0luQWlyID0gIWN1cnJlbnRNaWNyb3BhcnRpY2xlc0luQWlyOwoJCQlVcGRhdGVNaWNyb3BhcnRpY2xlc0luQWlyKHNlbmRlcik7CgkJCQoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCS8qaWYgKGtleURvd24uS2V5ID09IEVLZXlzLlApCgkJewoJCQljdXJyZW50UmVmbGVjdGlvblByb2JlID0gIWN1cnJlbnRSZWZsZWN0aW9uUHJvYmU7CgoJCQl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50Um9vdDsKCQkJZm9yZWFjaCAodmFyIHByb2JlIGluIHNjZW5lLkdldENvbXBvbmVudHM8UmVmbGVjdGlvblByb2JlPigpKQoJCQkJcHJvYmUuUmVhbFRpbWUgPSBjdXJyZW50UmVmbGVjdGlvblByb2JlOwoKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0qLwoJfQp9CgpwdWJsaWMgdm9pZCBHYW1lTW9kZV9JbnB1dE1lc3NhZ2VFdmVudChOZW9BeGlzLkdhbWVNb2RlIHNlbmRlciwgTmVvQXhpcy5JbnB1dE1lc3NhZ2UgbWVzc2FnZSkKewoJaWYgKCFzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkNvbnRyb2wpKQoJCVByb2Nlc3NJbnB1dE1lc3NhZ2VFdmVudChzZW5kZXIsIG1lc3NhZ2UpOwp9CgpwdWJsaWMgdm9pZCBHYW1lTW9kZV9FbmFibGVkSW5TaW11bGF0aW9uKE5lb0F4aXMuQ29tcG9uZW50IG9iaikKewoJLy8vL2FjdGl2YXRlIG5pZ2h0IG1vZGUKCS8vUHJvY2Vzc0lucHV0TWVzc2FnZUV2ZW50KChHYW1lTW9kZSlvYmosIG5ldyBJbnB1dE1lc3NhZ2VLZXlEb3duKEVLZXlzLkQ2KSk7CgkKCS8vcmFuZG9taXplIHN0cmVldCBsaWdodHMKCXZhciBsaWdodHMgPSBvYmouUGFyZW50Um9vdC5HZXRDb21wb25lbnQoIlN0cmVldCBsaWdodCBsaWdodHMiKTsKCWlmKGxpZ2h0cyAhPSBudWxsKQoJewoJCXZhciByYW5kb20gPSBuZXcgRmFzdFJhbmRvbSgpOwoJCQoJCWZvcmVhY2godmFyIGxpZ2h0IGluIGxpZ2h0cy5HZXRDb21wb25lbnRzPExpZ2h0PigpKQoJCXsKCQkJLy9yYW5kb21pemUgcm90YXRpb24KCQkJdmFyIHRyID0gbGlnaHQuVHJhbnNmb3JtVjsKCQkJdHIgPSB0ci5VcGRhdGVSb3RhdGlvbihRdWF0ZXJuaW9uLkZyb21Sb3RhdGVCeVoocmFuZG9tLk5leHQoTWF0aC5QSSAqIDIpKSk7CgkJCWxpZ2h0LlRyYW5zZm9ybSA9IHRyOwoKLyoKCQkJLy9yYW5kb21pemUgY29sb3JzCgkJCXZhciBjb2xvciA9IGxpZ2h0LkNvbG9yLlZhbHVlOwoJCQl2YXIgbWF4ID0gMC42ZjsvLzAuMmY7CgkJCWNvbG9yLlJlZCArPSByYW5kb20uTmV4dCgtbWF4LCBtYXgpOwoJCQljb2xvci5HcmVlbiArPSByYW5kb20uTmV4dCgtbWF4LCBtYXgpOwoJCQljb2xvci5CbHVlICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCWxpZ2h0LkNvbG9yID0gY29sb3I7CiovCQkJCgkJfQoJfQkKfQ==")]
public class DynamicClass5A28A5214ABA03FF6C4D583A80C7A5913065538E6EA2F37CB78E655F8E65E5DB
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
                    ambientLight.Brightness = currentNight ? 25000 : 30000; //100000;
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

[CSharpScriptGeneratedAttribute("c3RhdGljIGJvb2wgY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nOwpzdGF0aWMgYm9vbCBjdXJyZW50TmlnaHQ7CnN0YXRpYyBpbnQgY3VycmVudFdlYXRoZXI7CnN0YXRpYyBib29sIGN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9yczsKc3RhdGljIGJvb2wgY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXI7Ci8vc3RhdGljIGJvb2wgY3VycmVudFJlZmxlY3Rpb25Qcm9iZSA9IHRydWU7Cgpjb25zdCBpbnQgU3VubnkgPSAwOwpjb25zdCBpbnQgUmFpbkZhbGxpbmcgPSAxOwpjb25zdCBpbnQgUmFpbkZhbGxlbiA9IDI7Cgp2b2lkIFVwZGF0ZUZvZ0FuZEZhckNsaXBQbGFuZShGb2cgZm9nLCBDYW1lcmEgY2FtZXJhKQp7Cglmb2cuRW5hYmxlZCA9ICFjdXJyZW50TmlnaHQ7Ly8gfHwgY3VycmVudFJhaW47Cglmb2cuRGVuc2l0eSA9IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nID8gMC4wMSA6IDAuMDAxOy8vZm9nLkRlbnNpdHkgPSBjdXJyZW50UmFpbiA_IDAuMDEgOiAwLjAwMTsKCglpZiAoY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcpLy9pZiAoY3VycmVudFJhaW4pCgkJZm9nLkFmZmVjdEJhY2tncm91bmQgPSAxOwoJZWxzZQoJCWZvZy5BZmZlY3RCYWNrZ3JvdW5kID0gY3VycmVudE5pZ2h0ID8gMCA6IDAuNTsKCglpZiAoY3VycmVudE5pZ2h0KQoJCWZvZy5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAsIDAsIDApOwoJZWxzZQoJCWZvZy5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNDUzOTYwOCwgMC41MTYwMzkyLCAwLjY1ODgyMzUpOwoKCWlmIChmb2cuRW5hYmxlZCAmJiBmb2cuQWZmZWN0QmFja2dyb3VuZCA9PSAxKQoJCWNhbWVyYS5GYXJDbGlwUGxhbmUgPSAzMDA7CgllbHNlCgkJY2FtZXJhLkZhckNsaXBQbGFuZSA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDIwMDAgOiAxMDAwOwp9Cgp2b2lkIFVwZGF0ZU1pY3JvcGFydGljbGVzSW5BaXIoIENvbXBvbmVudCBzZW5kZXIgKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50Um9vdDsKCXZhciByZW5kZXJpbmdQaXBlbGluZSA9IHNjZW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdQaXBlbGluZV9CYXNpYz4oIlJlbmRlcmluZyBQaXBlbGluZSIpOwoJdmFyIGVmZmVjdCA9IHJlbmRlcmluZ1BpcGVsaW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdFZmZlY3RfTWljcm9wYXJ0aWNsZXNJbkFpcj4oY2hlY2tDaGlsZHJlbjogdHJ1ZSk7CglpZiAoZWZmZWN0ICE9IG51bGwpCgl7CgkJaWYgKGN1cnJlbnRNaWNyb3BhcnRpY2xlc0luQWlyKQoJCXsKCQkJZWZmZWN0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMC43LCAwLjYpOwoJCQkvL2VmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuOCwgMC41KTsKCQkJZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMTU7CgkJfQoJCWVsc2UKCQl7CgkJCWlmIChjdXJyZW50TmlnaHQpCgkJCXsKCQkJCWVmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNzUsIDAuNzUsIDEpOwoJCQkJZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMDE1OwoJCQl9CgkJCWVsc2UKCQkJewoJCQkJLy9zaW11bGF0ZSBpbmRpcmVjdCBsaWdodGluZyBieSBtZWFucyBtaWNyb3BhcnRpY2xlcyBpbiBhaXIKCQkJCWVmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuOCwgMC42MjgpOwoJCQkJLy9lZmZlY3QuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAwLjgsIDAuNSk7CgkJCQllZmZlY3QuTXVsdGlwbGllciA9IDAuMDAwMzsKCQkJfQoKCQkJLy9lZmZlY3QuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgwLjc1LCAwLjc1LCAxKTsKCQkJLy9lZmZlY3QuTXVsdGlwbGllciA9IDAuMDAwMTU7CgkJfQoJfQp9Cgp2b2lkIEV4aXRGcm9tVmVoaWNsZShOZW9BeGlzLkdhbWVNb2RlIGdhbWVNb2RlKQp7Cgl2YXIgb2JqID0gZ2FtZU1vZGUuT2JqZWN0Q29udHJvbGxlZEJ5UGxheWVyLlZhbHVlIGFzIFZlaGljbGU7CglpZiAob2JqICE9IG51bGwpCgl7CgkJdmFyIGlucHV0UHJvY2Vzc2luZyA9IG9iai5HZXRDb21wb25lbnQ8VmVoaWNsZUlucHV0UHJvY2Vzc2luZz4oKTsKCQlpZiAoaW5wdXRQcm9jZXNzaW5nICE9IG51bGwpCgkJCWlucHV0UHJvY2Vzc2luZy5FeGl0QWxsT2JqZWN0c0Zyb21WZWhpY2xlKGdhbWVNb2RlKTsKCX0KfQoKdm9pZCBQcm9jZXNzSW5wdXRNZXNzYWdlRXZlbnQoTmVvQXhpcy5HYW1lTW9kZSBzZW5kZXIsIE5lb0F4aXMuSW5wdXRNZXNzYWdlIG1lc3NhZ2UpCnsKCXZhciBrZXlEb3duID0gbWVzc2FnZSBhcyBJbnB1dE1lc3NhZ2VLZXlEb3duOwoJaWYgKGtleURvd24gIT0gbnVsbCkvLyYmICFzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkNvbnRyb2wpKQoJewoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EMSkKCQl7CgkJCXZhciBtYW5hZ2VyID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PEJ1aWxkaW5nTWFuYWdlcj4oKTsKCQkJaWYgKG1hbmFnZXIgIT0gbnVsbCkKCQkJewoJCQkJbWFuYWdlci5EaXNwbGF5ID0gIW1hbmFnZXIuRGlzcGxheTsKCQkJCW1hbmFnZXIuQ29sbGlzaW9uID0gbWFuYWdlci5EaXNwbGF5OwoJCQl9CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQyKQoJCXsKCQkJRXhpdEZyb21WZWhpY2xlKHNlbmRlcik7CgoJCQl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CgkJCWlmIChzeXN0ZW0gIT0gbnVsbCkKCQkJCXN5c3RlbS5QYXJrZWRWZWhpY2xlcyA9IHN5c3RlbS5QYXJrZWRWZWhpY2xlcy5WYWx1ZSAhPSAwID8gMCA6IDUwMDA7CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQzKQoJCXsKCQkJRXhpdEZyb21WZWhpY2xlKHNlbmRlcik7CgoJCQl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CgkJCWlmIChzeXN0ZW0gIT0gbnVsbCkKCQkJCXN5c3RlbS5GbHlpbmdWZWhpY2xlcyA9IHN5c3RlbS5GbHlpbmdWZWhpY2xlcy5WYWx1ZSAhPSAwID8gMCA6IDUwMDsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDkpCgkJewoJCQl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CgkJCWlmIChzeXN0ZW0gIT0gbnVsbCkKCQkJCXN5c3RlbS5TaW11bGF0ZUR5bmFtaWNPYmplY3RzID0gIXN5c3RlbS5TaW11bGF0ZUR5bmFtaWNPYmplY3RzOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5ENCkKCQl7CgkJCUV4aXRGcm9tVmVoaWNsZShzZW5kZXIpOwoKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uV2Fsa2luZ1BlZGVzdHJpYW5zID0gc3lzdGVtLldhbGtpbmdQZWRlc3RyaWFucy5WYWx1ZSAhPSAwID8gMCA6IDEwMDsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDApCgkJewoJCQl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CgkJCWlmIChzeXN0ZW0gIT0gbnVsbCkKCQkJCXN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnNNYW5hZ2VUYXNrcyA9ICFzeXN0ZW0uV2Fsa2luZ1BlZGVzdHJpYW5zTWFuYWdlVGFza3M7CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ1KQoJCXsKCQkJdmFyIHNjZW5lID0gKFNjZW5lKXNlbmRlci5QYXJlbnRSb290OwoJCQl2YXIgcmVuZGVyaW5nUGlwZWxpbmUgPSBzY2VuZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nUGlwZWxpbmU+KCJSZW5kZXJpbmcgUGlwZWxpbmUiKTsKCQkJdmFyIHJlZmxlY3Rpb24gPSByZW5kZXJpbmdQaXBlbGluZT8uR2V0Q29tcG9uZW50PFJlbmRlcmluZ0VmZmVjdF9SZWZsZWN0aW9uPihjaGVja0NoaWxkcmVuOiB0cnVlKTsKCQkJdmFyIGZvZyA9IHNjZW5lLkdldENvbXBvbmVudCgiRm9nIikgYXMgRm9nOwoJCQl2YXIgcHJlY2lwaXRhdGlvbiA9IHJlbmRlcmluZ1BpcGVsaW5lPy5HZXRDb21wb25lbnQ8UmVuZGVyaW5nRWZmZWN0X1ByZWNpcGl0YXRpb24+KGNoZWNrQ2hpbGRyZW46IHRydWUpOwoJCQl2YXIgc291bmRTb3VyY2VSYWluID0gc2NlbmUuR2V0Q29tcG9uZW50KCJTb3VuZCBTb3VyY2UgUmFpbiIpIGFzIFNvdW5kU291cmNlOwoJCQl2YXIgY2FtZXJhID0gc2NlbmUuR2V0Q29tcG9uZW50PENhbWVyYT4oIkNhbWVyYSBEZWZhdWx0Iik7CgkJCXZhciBkaXJlY3Rpb25hbExpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJEaXJlY3Rpb25hbCBMaWdodCIpIGFzIExpZ2h0OwoKCQkJY3VycmVudFdlYXRoZXIrKzsKCQkJaWYgKGN1cnJlbnRXZWF0aGVyID4gMikKCQkJCWN1cnJlbnRXZWF0aGVyID0gMDsKCQkJLy9jdXJyZW50UmFpbiA9ICFjdXJyZW50UmFpbjsKCgkJCXRyeQoJCQl7CgkJCQlVcGRhdGVGb2dBbmRGYXJDbGlwUGxhbmUoZm9nLCBjYW1lcmEpOwoKCQkJCXNvdW5kU291cmNlUmFpbi5FbmFibGVkID0gY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmc7CgoJCQkJc2NlbmUuUHJlY2lwaXRhdGlvbkZhbGxpbmcgPSBjdXJyZW50V2VhdGhlciA9PSBSYWluRmFsbGluZyA_IDEgOiAwOwoJCQkJc2NlbmUuUHJlY2lwaXRhdGlvbkZhbGxlbiA9IChjdXJyZW50V2VhdGhlciA9PSBSYWluRmFsbGluZyB8fCBjdXJyZW50V2VhdGhlciA9PSBSYWluRmFsbGVuKSA_IDEgOiAwOwoKCQkJCS8vcHJlY2lwaXRhdGlvbi5FbmFibGVkID0gY3VycmVudFJhaW47CgkJCQkvL3NvdW5kU291cmNlUmFpbi5FbmFibGVkID0gY3VycmVudFJhaW47CgkJCQkvL3NjZW5lLlByZWNpcGl0YXRpb25GYWxsaW5nID0gY3VycmVudFJhaW4gPyAxIDogMDsKCQkJCS8vc2NlbmUuUHJlY2lwaXRhdGlvbkZhbGxlbiA9IGN1cnJlbnRSYWluID8gMSA6IDA7CgoJCQkJLyoKCQkJCQkJCQlpZihjdXJyZW50V2VhdGhlciA9PSBSYWluRmFsbGluZykvL2lmIChjdXJyZW50UmFpbikKCQkJCQkJCQl7CgkJCQkJCQkJCWRpcmVjdGlvbmFsTGlnaHQuTWFzayA9IG5ldyBSZWZlcmVuY2VOb1ZhbHVlKEAiU2FtcGxlc1xDaXR5IERlbW9cU2tpZXNcUmFpbiBjbG91ZHMgbWFza1xSYWluIGNsb3VkcyBtYXNrLmpwZyIpOwoJCQkJCQkJCQlkaXJlY3Rpb25hbExpZ2h0Lk1hc2tUcmFuc2Zvcm0gPSBuZXcgVHJhbnNmb3JtKFZlY3RvcjMuWmVybywgUXVhdGVybmlvbi5JZGVudGl0eSwgbmV3IFZlY3RvcjMoMC4wMDUsIDAuMDA1LCAwLjAwNSkpOwoJCQkJCQkJCX0KCQkJCQkJCQllbHNlCgkJCQkJCQkJewoJCQkJCQkJCQlkaXJlY3Rpb25hbExpZ2h0Lk1hc2sgPSBudWxsOwoJCQkJCQkJCX0KCQkJCSovCgkJCX0KCQkJY2F0Y2ggKEV4Y2VwdGlvbiBlKQoJCQl7CgkJCQlMb2cuV2FybmluZyhlLk1lc3NhZ2UpOwoJCQl9CgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5ENikKCQl7CgkJCXZhciBzY2VuZSA9IChTY2VuZSlzZW5kZXIuUGFyZW50Um9vdDsKCQkJdmFyIGFtYmllbnRMaWdodCA9IHNjZW5lLkdldENvbXBvbmVudCgiQW1iaWVudCBMaWdodCIpIGFzIExpZ2h0OwoJCQl2YXIgZGlyZWN0aW9uYWxMaWdodCA9IHNjZW5lLkdldENvbXBvbmVudCgiRGlyZWN0aW9uYWwgTGlnaHQiKSBhcyBMaWdodDsKCQkJdmFyIHN0cmVldExpZ2h0TGlnaHRzID0gc2NlbmUuR2V0Q29tcG9uZW50KCJTdHJlZXQgbGlnaHQgbGlnaHRzIik7CgkJCXZhciBza3kgPSBzY2VuZS5HZXRDb21wb25lbnQoIlNreSIpIGFzIFNreTsKCQkJLy92YXIgZGF5U2t5ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJEYXkgc2t5Iik7CgkJCS8vdmFyIG5pZ2h0U2t5ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJOaWdodCBza3kiKTsKCQkJdmFyIGZvZyA9IHNjZW5lLkdldENvbXBvbmVudCgiRm9nIikgYXMgRm9nOwoJCQl2YXIgY2FtZXJhID0gc2NlbmUuR2V0Q29tcG9uZW50PENhbWVyYT4oIkNhbWVyYSBEZWZhdWx0Iik7CgoJCQljdXJyZW50TmlnaHQgPSAhY3VycmVudE5pZ2h0OwoKCQkJdHJ5CgkJCXsKCQkJCXNjZW5lLlRpbWVPZkRheSA9IGN1cnJlbnROaWdodCA_IDAgOiAxMjsKCQkJCWFtYmllbnRMaWdodC5CcmlnaHRuZXNzID0gY3VycmVudE5pZ2h0ID8gMjUwMDAgOiAzMDAwMDsvLzEwMDAwMDsKCQkJCWRpcmVjdGlvbmFsTGlnaHQuRW5hYmxlZCA9ICFjdXJyZW50TmlnaHQ7CgkJCQlzdHJlZXRMaWdodExpZ2h0cy5FbmFibGVkID0gY3VycmVudE5pZ2h0OwoJCQkJc2t5LlByb2NlZHVyYWxBdG1vc3BoZXJlID0gY3VycmVudE5pZ2h0ID8gMCA6IDE7CgkJCQkvL3NreS5Qcm9jZWR1cmFsU3RhcnMgPSBjdXJyZW50TmlnaHQgPyAxIDogMDsKCQkJCS8vc2t5Lk1vZGUgPSBjdXJyZW50TmlnaHQgPyBTa3kuTW9kZUVudW0uUmVzb3VyY2UgOiBTa3kuTW9kZUVudW0uUHJvY2VkdXJhbDsJCQkJCgkJCQkvLy8vc2t5LlByb2NlZHVyYWxJbnRlbnNpdHkgPSBjdXJyZW50TmlnaHQgPyAwIDogMTsKCQkJCS8vLy9kYXlTa3kuRW5hYmxlZCA9ICFjdXJyZW50TmlnaHQ7CgkJCQkvLy8vbmlnaHRTa3kuRW5hYmxlZCA9IGN1cnJlbnROaWdodDsKCQkJCVVwZGF0ZUZvZ0FuZEZhckNsaXBQbGFuZShmb2csIGNhbWVyYSk7CgkJCQlVcGRhdGVNaWNyb3BhcnRpY2xlc0luQWlyKHNlbmRlcik7CgkJCX0KCQkJY2F0Y2ggKEV4Y2VwdGlvbiBlKQoJCQl7CgkJCQlMb2cuV2FybmluZyhlLk1lc3NhZ2UpOwoJCQl9CgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EOCkKCQl7CgkJCUV4aXRGcm9tVmVoaWNsZShzZW5kZXIpOwoKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCXsKCQkJCWlmIChzeXN0ZW0uUGFya2VkVmVoaWNsZXNPYmplY3RNb2RlLlZhbHVlID09IFRyYWZmaWNTeXN0ZW0uT2JqZWN0TW9kZUVudW0uVmVoaWNsZUNvbXBvbmVudCkKCQkJCQlzeXN0ZW0uUGFya2VkVmVoaWNsZXNPYmplY3RNb2RlID0gVHJhZmZpY1N5c3RlbS5PYmplY3RNb2RlRW51bS5TdGF0aWNPYmplY3Q7CgkJCQllbHNlCgkJCQkJc3lzdGVtLlBhcmtlZFZlaGljbGVzT2JqZWN0TW9kZSA9IFRyYWZmaWNTeXN0ZW0uT2JqZWN0TW9kZUVudW0uVmVoaWNsZUNvbXBvbmVudDsKCQkJfQoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5ENykKCQl7CgkJCWN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA9ICFjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmc7CgoJCQl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50Um9vdDsKCQkJdmFyIHJlbmRlcmluZ1BpcGVsaW5lID0gc2NlbmUuR2V0Q29tcG9uZW50PFJlbmRlcmluZ1BpcGVsaW5lX0Jhc2ljPigiUmVuZGVyaW5nIFBpcGVsaW5lIik7CgkJCXZhciBjYW1lcmEgPSBzY2VuZS5HZXRDb21wb25lbnQ8Q2FtZXJhPigiQ2FtZXJhIERlZmF1bHQiKTsKCQkJdmFyIGZvZyA9IHNjZW5lLkdldENvbXBvbmVudCgiRm9nIikgYXMgRm9nOwoKCQkJLy9jYW1lcmEuRmFyQ2xpcFBsYW5lID0gY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID8gMjAwMCA6IDEwMDA7CgkJCXJlbmRlcmluZ1BpcGVsaW5lLk1pbmltdW1WaXNpYmxlU2l6ZU9mT2JqZWN0cyA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDIgOiA0OwoKCQkJcmVuZGVyaW5nUGlwZWxpbmUuU2hhZG93RGlyZWN0aW9uYWxEaXN0YW5jZSA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDYwMCA6IDIwMDsKCQkJcmVuZGVyaW5nUGlwZWxpbmUuU2hhZG93RGlyZWN0aW9uYWxMaWdodENhc2NhZGVzID0gY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID8gMyA6IDI7CgoJCQl0cnkKCQkJewoJCQkJVXBkYXRlRm9nQW5kRmFyQ2xpcFBsYW5lKGZvZywgY2FtZXJhKTsKCQkJfQoJCQljYXRjaCAoRXhjZXB0aW9uIGUpCgkJCXsKCQkJCUxvZy5XYXJuaW5nKGUuTWVzc2FnZSk7CgkJCX0KCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCgkJCS8qCgkJCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRSb290IGFzIFNjZW5lOwoJCQlpZiAoc2NlbmUgIT0gbnVsbCkKCQkJewoJCQkJaWYgKHNjZW5lLk9jdHJlZVRocmVhZGluZ01vZGUuVmFsdWUgPT0gT2N0cmVlQ29udGFpbmVyLlRocmVhZGluZ01vZGVFbnVtLkJhY2tncm91bmRUaHJlYWQpCgkJCQkJc2NlbmUuT2N0cmVlVGhyZWFkaW5nTW9kZSA9IE9jdHJlZUNvbnRhaW5lci5UaHJlYWRpbmdNb2RlRW51bS5TaW5nbGVUaHJlYWRlZDsKCQkJCWVsc2UKCQkJCQlzY2VuZS5PY3RyZWVUaHJlYWRpbmdNb2RlID0gT2N0cmVlQ29udGFpbmVyLlRocmVhZGluZ01vZGVFbnVtLkJhY2tncm91bmRUaHJlYWQ7CgkJCX0KCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCQkqLwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuQykKCQl7CgkJCWN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9ycyA9ICFjdXJyZW50UmFuZG9taXplU3RyZWV0TGlnaHRDb2xvcnM7CgkJCQoJCQl2YXIgbGlnaHRzID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50KCJTdHJlZXQgbGlnaHQgbGlnaHRzIik7CgkJCWlmKGxpZ2h0cyAhPSBudWxsKQoJCQl7CgkJCQl2YXIgcmFuZG9tID0gbmV3IEZhc3RSYW5kb20oKTsKCQkJCQoJCQkJZm9yZWFjaCh2YXIgbGlnaHQgaW4gbGlnaHRzLkdldENvbXBvbmVudHM8TGlnaHQ+KCkpCgkJCQl7CgkJCQkJaWYoY3VycmVudFJhbmRvbWl6ZVN0cmVldExpZ2h0Q29sb3JzKQoJCQkJCXsKCQkJCQkJdmFyIGNvbG9yID0gbGlnaHQuQ29sb3IuVmFsdWU7CgkJCQkJCXZhciBtYXggPSAwLjZmOy8vMC4yZjsKCQkJCQkJY29sb3IuUmVkICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCQkJCWNvbG9yLkdyZWVuICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCQkJCWNvbG9yLkJsdWUgKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJCQkJbGlnaHQuQ29sb3IgPSBjb2xvcjsKCQkJCQl9CgkJCQkJZWxzZQoJCQkJCXsKCQkJCQkJbGlnaHQuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAxLCAwLjcxMTAxOTYpOwoJCQkJCX0KCQkJCX0KCQkJfQkKCgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5NKQoJCXsKCQkJY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXIgPSAhY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXI7CgkJCVVwZGF0ZU1pY3JvcGFydGljbGVzSW5BaXIoc2VuZGVyKTsKCQkJCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJLyppZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuUCkKCQl7CgkJCWN1cnJlbnRSZWZsZWN0aW9uUHJvYmUgPSAhY3VycmVudFJlZmxlY3Rpb25Qcm9iZTsKCgkJCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRSb290OwoJCQlmb3JlYWNoICh2YXIgcHJvYmUgaW4gc2NlbmUuR2V0Q29tcG9uZW50czxSZWZsZWN0aW9uUHJvYmU+KCkpCgkJCQlwcm9iZS5SZWFsVGltZSA9IGN1cnJlbnRSZWZsZWN0aW9uUHJvYmU7CgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfSovCgl9Cn0KCnB1YmxpYyB2b2lkIEdhbWVNb2RlX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuR2FtZU1vZGUgc2VuZGVyLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQp7CglpZiAoIXNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuQ29udHJvbCkpCgkJUHJvY2Vzc0lucHV0TWVzc2FnZUV2ZW50KHNlbmRlciwgbWVzc2FnZSk7Cn0KCnB1YmxpYyB2b2lkIEdhbWVNb2RlX0VuYWJsZWRJblNpbXVsYXRpb24oTmVvQXhpcy5Db21wb25lbnQgb2JqKQp7CgkvLy8vYWN0aXZhdGUgbmlnaHQgbW9kZQoJLy9Qcm9jZXNzSW5wdXRNZXNzYWdlRXZlbnQoKEdhbWVNb2RlKW9iaiwgbmV3IElucHV0TWVzc2FnZUtleURvd24oRUtleXMuRDYpKTsKCQoJLy9yYW5kb21pemUgc3RyZWV0IGxpZ2h0cwoJdmFyIGxpZ2h0cyA9IG9iai5QYXJlbnRSb290LkdldENvbXBvbmVudCgiU3RyZWV0IGxpZ2h0IGxpZ2h0cyIpOwoJaWYobGlnaHRzICE9IG51bGwpCgl7CgkJdmFyIHJhbmRvbSA9IG5ldyBGYXN0UmFuZG9tKCk7CgkJCgkJZm9yZWFjaCh2YXIgbGlnaHQgaW4gbGlnaHRzLkdldENvbXBvbmVudHM8TGlnaHQ+KCkpCgkJewoJCQkvL3JhbmRvbWl6ZSByb3RhdGlvbgoJCQl2YXIgdHIgPSBsaWdodC5UcmFuc2Zvcm1WOwoJCQl0ciA9IHRyLlVwZGF0ZVJvdGF0aW9uKFF1YXRlcm5pb24uRnJvbVJvdGF0ZUJ5WihyYW5kb20uTmV4dChNYXRoLlBJICogMikpKTsKCQkJbGlnaHQuVHJhbnNmb3JtID0gdHI7CgovKgoJCQkvL3JhbmRvbWl6ZSBjb2xvcnMKCQkJdmFyIGNvbG9yID0gbGlnaHQuQ29sb3IuVmFsdWU7CgkJCXZhciBtYXggPSAwLjZmOy8vMC4yZjsKCQkJY29sb3IuUmVkICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCWNvbG9yLkdyZWVuICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCWNvbG9yLkJsdWUgKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJbGlnaHQuQ29sb3IgPSBjb2xvcjsKKi8JCQkKCQl9Cgl9CQp9")]
public class DynamicClass631747AF6D5EB7D5EE2FF82810F45C84C28A4DECFA110B98F8C6D42FC5DF10A8
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
                    effect.Color = new ColorValue(1, 0.8, 0.628);
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
                    ambientLight.Brightness = currentNight ? 25000 : 30000; //100000;
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
}
#endif