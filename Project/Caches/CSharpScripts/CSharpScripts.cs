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

[CSharpScriptGeneratedAttribute("Y2xhc3MgX1RlbXB7Cn0=")]
public class DynamicClassDE8FFF4AC5357F29AC8BBDD6822A0D4AB8BF8DDC6BD03130D02DDFE04424217D
{
    public NeoAxis.CSharpScript Owner;
    class _Temp
    {
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX1NpbXVsYXRpb25TdGVwKE5lb0F4aXMuQ29tcG9uZW50IG9iaikKewp9Cg==")]
public class DynamicClass2A294C417D2E1C884DF3F23781187BEC7AB3677AE099F8681717AE9158DC1043
{
    public NeoAxis.CSharpScript Owner;
    public void _SimulationStep(NeoAxis.Component obj)
    {
    }
}

[CSharpScriptGeneratedAttribute("ZG91YmxlIGxhc3RSZXNldFRpbWU7CgpwdWJsaWMgdm9pZCBfU2ltdWxhdGlvblN0ZXAoTmVvQXhpcy5Db21wb25lbnQgb2JqKQp7CgkKCQoJCn0K")]
public class DynamicClassC00FE76BB21DBDDA70BFF50E61D94A5B817D22EFF19660A2807297AA27C52BB1
{
    public NeoAxis.CSharpScript Owner;
    double lastResetTime;
    public void _SimulationStep(NeoAxis.Component obj)
    {
    }
}

[CSharpScriptGeneratedAttribute("Y29uc3QgZG91YmxlIHJlc2V0VGltZSA9IDEwOwpkb3VibGUgbGFzdFJlc2V0VGltZTsKCnB1YmxpYyB2b2lkIF9TaW11bGF0aW9uU3RlcChOZW9BeGlzLkNvbXBvbmVudCBvYmopCnsKCQoJCgkKfQo=")]
public class DynamicClass85377B362B5A954C11C2005D8C202D5EB1B03E210B8EB3B4FC8F94C458070773
{
    public NeoAxis.CSharpScript Owner;
    const double resetTime = 10;
    double lastResetTime;
    public void _SimulationStep(NeoAxis.Component obj)
    {
    }
}

[CSharpScriptGeneratedAttribute("Y29uc3QgZG91YmxlIHJlc2V0VGltZSA9IDEwOwpkb3VibGUgbGFzdFJlc2V0VGltZTsKCnB1YmxpYyB2b2lkIF9TaW11bGF0aW9uU3RlcChOZW9BeGlzLkNvbXBvbmVudCBvYmopCnsKCS8vc2F2ZSBpbml0aWFsIHN0YXRlIG9mIG9iamVjdHMKCWlmKGxhc3RSZXNldFRpbWUgPT0wKQoJewoJCS8venp6ejsKCQlsYXN0UmVzZXRUaW1lID0gVGltZS5DdXJyZW50OyAgIAoJfQoJCgkvL3Jlc2V0CglpZihsYXN0UmVzZXRUaW1lICE9IDAgJiYgVGltZS5DdXJyZW50IC0gbGFzdFJlc2V0VGltZSA+IHJlc2V0VGltZSkKCXsKCQkvL3p6enp6OwoJfQoJCgkKfQo=")]
public class DynamicClassDCB8542908529B38B7FA2A9CCAC4D6F6F25919E74D4E08E69361A2D5F62A77A4
{
    public NeoAxis.CSharpScript Owner;
    const double resetTime = 10;
    double lastResetTime;
    public void _SimulationStep(NeoAxis.Component obj)
    {
        //save initial state of objects
        if (lastResetTime == 0)
        {
            //zzzz;
            lastResetTime = Time.Current;
        }

        //reset
        if (lastResetTime != 0 && Time.Current - lastResetTime > resetTime)
        {
        //zzzzz;
        }
    }
}

[CSharpScriptGeneratedAttribute("Y29uc3QgZG91YmxlIHJlc2V0VGltZSA9IDEwOwpkb3VibGUgbGFzdFJlc2V0VGltZTsKRGljdGlvbmFyeTxNZXNoSW5TcGFjZSwgVHJhbnNmb3JtPiBvcmlnaW5hbFRyYW5zZm9ybXMgPSBuZXcgRGljdGlvbmFyeTxNZXNoSW5TcGFjZSwgVHJhbnNmb3JtPigpOwoKcHVibGljIHZvaWQgX1NpbXVsYXRpb25TdGVwKE5lb0F4aXMuQ29tcG9uZW50IG9iaikKewoJLy9zYXZlIGluaXRpYWwgc3RhdGUgb2Ygb2JqZWN0cwoJaWYgKGxhc3RSZXNldFRpbWUgPT0gMCkKCXsKCQlsYXN0UmVzZXRUaW1lID0gVGltZS5DdXJyZW50OwoJCQoJCWZvcmVhY2ggKHZhciBjb21wb25lbnQgaW4gU2NlbmUuRmlyc3QuR2V0Q29tcG9uZW50czxNZXNoSW5TcGFjZT4oKSkKCQl7CgkJCWlmIChjb21wb25lbnQuTmFtZS5Db250YWlucygiU0ZDcmF0ZSIpKQoJCQkJb3JpZ2luYWxUcmFuc2Zvcm1zW2NvbXBvbmVudF0gPSBjb21wb25lbnQuVHJhbnNmb3JtLlZhbHVlOwoJCX0KCX0KCgkvL3Jlc2V0CglpZiAobGFzdFJlc2V0VGltZSAhPSAwICYmIFRpbWUuQ3VycmVudCAtIGxhc3RSZXNldFRpbWUgPiByZXNldFRpbWUpCgl7CgkJbGFzdFJlc2V0VGltZSA9IFRpbWUuQ3VycmVudDsKCQkKCQlmb3JlYWNoICh2YXIgcGFpciBpbiBvcmlnaW5hbFRyYW5zZm9ybXMpCgkJCXBhaXIuS2V5LlRyYW5zZm9ybSA9IHBhaXIuVmFsdWU7Cgl9Cn0K")]
public class DynamicClass4CE96A9B761A7FD9F722CDC5B77FC67EC2757341A894CC488C601F7DBA9722F9
{
    public NeoAxis.CSharpScript Owner;
    const double resetTime = 10;
    double lastResetTime;
    Dictionary<MeshInSpace, Transform> originalTransforms = new Dictionary<MeshInSpace, Transform>();
    public void _SimulationStep(NeoAxis.Component obj)
    {
        //save initial state of objects
        if (lastResetTime == 0)
        {
            lastResetTime = Time.Current;
            foreach (var component in Scene.First.GetComponents<MeshInSpace>())
            {
                if (component.Name.Contains("SFCrate"))
                    originalTransforms[component] = component.Transform.Value;
            }
        }

        //reset
        if (lastResetTime != 0 && Time.Current - lastResetTime > resetTime)
        {
            lastResetTime = Time.Current;
            foreach (var pair in originalTransforms)
                pair.Key.Transform = pair.Value;
        }
    }
}

[CSharpScriptGeneratedAttribute("Y29uc3QgZG91YmxlIHJlc2V0VGltZSA9IDE1Owpkb3VibGUgbGFzdFJlc2V0VGltZTsKRGljdGlvbmFyeTxNZXNoSW5TcGFjZSwgVHJhbnNmb3JtPiBvcmlnaW5hbFRyYW5zZm9ybXMgPSBuZXcgRGljdGlvbmFyeTxNZXNoSW5TcGFjZSwgVHJhbnNmb3JtPigpOwoKcHVibGljIHZvaWQgX1NpbXVsYXRpb25TdGVwKE5lb0F4aXMuQ29tcG9uZW50IG9iaikKewoJLy9zYXZlIGluaXRpYWwgc3RhdGUgb2Ygb2JqZWN0cwoJaWYgKGxhc3RSZXNldFRpbWUgPT0gMCkKCXsKCQlsYXN0UmVzZXRUaW1lID0gVGltZS5DdXJyZW50OwoJCQoJCWZvcmVhY2ggKHZhciBjb21wb25lbnQgaW4gU2NlbmUuRmlyc3QuR2V0Q29tcG9uZW50czxNZXNoSW5TcGFjZT4oKSkKCQl7CgkJCWlmIChjb21wb25lbnQuTmFtZS5Db250YWlucygiU0ZDcmF0ZSIpKQoJCQkJb3JpZ2luYWxUcmFuc2Zvcm1zW2NvbXBvbmVudF0gPSBjb21wb25lbnQuVHJhbnNmb3JtLlZhbHVlOwoJCX0KCX0KCgkvL3Jlc2V0CglpZiAobGFzdFJlc2V0VGltZSAhPSAwICYmIFRpbWUuQ3VycmVudCAtIGxhc3RSZXNldFRpbWUgPiByZXNldFRpbWUpCgl7CgkJbGFzdFJlc2V0VGltZSA9IFRpbWUuQ3VycmVudDsKCQkKCQlmb3JlYWNoICh2YXIgcGFpciBpbiBvcmlnaW5hbFRyYW5zZm9ybXMpCgkJCXBhaXIuS2V5LlRyYW5zZm9ybSA9IHBhaXIuVmFsdWU7Cgl9Cn0K")]
public class DynamicClassD2B8DC8389C2B364FB462EB77A2EF2FA944CE864B2F859A1283289E10D88E119
{
    public NeoAxis.CSharpScript Owner;
    const double resetTime = 15;
    double lastResetTime;
    Dictionary<MeshInSpace, Transform> originalTransforms = new Dictionary<MeshInSpace, Transform>();
    public void _SimulationStep(NeoAxis.Component obj)
    {
        //save initial state of objects
        if (lastResetTime == 0)
        {
            lastResetTime = Time.Current;
            foreach (var component in Scene.First.GetComponents<MeshInSpace>())
            {
                if (component.Name.Contains("SFCrate"))
                    originalTransforms[component] = component.Transform.Value;
            }
        }

        //reset
        if (lastResetTime != 0 && Time.Current - lastResetTime > resetTime)
        {
            lastResetTime = Time.Current;
            foreach (var pair in originalTransforms)
                pair.Key.Transform = pair.Value;
        }
    }
}

[CSharpScriptGeneratedAttribute("Y29uc3QgZG91YmxlIHJlc2V0VGltZSA9IDE1Owpkb3VibGUgbGFzdFJlc2V0VGltZTsKRGljdGlvbmFyeTxNZXNoSW5TcGFjZSwgVHJhbnNmb3JtPiBvcmlnaW5hbFRyYW5zZm9ybXMgPSBuZXcgRGljdGlvbmFyeTxNZXNoSW5TcGFjZSwgVHJhbnNmb3JtPigpOwoKcHVibGljIHZvaWQgX1NpbXVsYXRpb25TdGVwKE5lb0F4aXMuQ29tcG9uZW50IG9iaikKewoJLy9zYXZlIGluaXRpYWwgc3RhdGUgb2Ygb2JqZWN0cwoJaWYgKGxhc3RSZXNldFRpbWUgPT0gMCkKCXsKCQlsYXN0UmVzZXRUaW1lID0gVGltZS5DdXJyZW50OwoJCQoJCWZvcmVhY2ggKHZhciBjb21wb25lbnQgaW4gU2NlbmUuRmlyc3QuR2V0Q29tcG9uZW50czxNZXNoSW5TcGFjZT4oKSkKCQl7CgkJCWlmIChjb21wb25lbnQuTmFtZS5Db250YWlucygiU0ZDcmF0ZSIpKQoJCQkJb3JpZ2luYWxUcmFuc2Zvcm1zW2NvbXBvbmVudF0gPSBjb21wb25lbnQuVHJhbnNmb3JtLlZhbHVlOwoJCX0KCX0KCgkvL3Jlc2V0CglpZiAobGFzdFJlc2V0VGltZSAhPSAwICYmIFRpbWUuQ3VycmVudCAtIGxhc3RSZXNldFRpbWUgPiByZXNldFRpbWUgJiYgIUVuZ2luZUFwcC5fRGVidWdDYXBzTG9jaykKCXsKCQlsYXN0UmVzZXRUaW1lID0gVGltZS5DdXJyZW50OwoJCQoJCWZvcmVhY2ggKHZhciBwYWlyIGluIG9yaWdpbmFsVHJhbnNmb3JtcykKCQkJcGFpci5LZXkuVHJhbnNmb3JtID0gcGFpci5WYWx1ZTsKCX0KfQo=")]
public class DynamicClass0A44B8782F5A985308CDB2958F71459F6C9323578F339CAF06841E1371D99682
{
    public NeoAxis.CSharpScript Owner;
    const double resetTime = 15;
    double lastResetTime;
    Dictionary<MeshInSpace, Transform> originalTransforms = new Dictionary<MeshInSpace, Transform>();
    public void _SimulationStep(NeoAxis.Component obj)
    {
        //save initial state of objects
        if (lastResetTime == 0)
        {
            lastResetTime = Time.Current;
            foreach (var component in Scene.First.GetComponents<MeshInSpace>())
            {
                if (component.Name.Contains("SFCrate"))
                    originalTransforms[component] = component.Transform.Value;
            }
        }

        //reset
        if (lastResetTime != 0 && Time.Current - lastResetTime > resetTime && !EngineApp._DebugCapsLock)
        {
            lastResetTime = Time.Current;
            foreach (var pair in originalTransforms)
                pair.Key.Transform = pair.Value;
        }
    }
}

[CSharpScriptGeneratedAttribute("Y29uc3QgZG91YmxlIHJlc2V0VGltZSA9IDIwOwpkb3VibGUgbGFzdFJlc2V0VGltZTsKRGljdGlvbmFyeTxNZXNoSW5TcGFjZSwgVHJhbnNmb3JtPiBvcmlnaW5hbFRyYW5zZm9ybXMgPSBuZXcgRGljdGlvbmFyeTxNZXNoSW5TcGFjZSwgVHJhbnNmb3JtPigpOwoKcHVibGljIHZvaWQgX1NpbXVsYXRpb25TdGVwKE5lb0F4aXMuQ29tcG9uZW50IG9iaikKewoJLy9zYXZlIGluaXRpYWwgc3RhdGUgb2Ygb2JqZWN0cwoJaWYgKGxhc3RSZXNldFRpbWUgPT0gMCkKCXsKCQlsYXN0UmVzZXRUaW1lID0gVGltZS5DdXJyZW50OwoJCQoJCWZvcmVhY2ggKHZhciBjb21wb25lbnQgaW4gU2NlbmUuRmlyc3QuR2V0Q29tcG9uZW50czxNZXNoSW5TcGFjZT4oKSkKCQl7CgkJCWlmIChjb21wb25lbnQuTmFtZS5Db250YWlucygiU0ZDcmF0ZSIpKQoJCQkJb3JpZ2luYWxUcmFuc2Zvcm1zW2NvbXBvbmVudF0gPSBjb21wb25lbnQuVHJhbnNmb3JtLlZhbHVlOwoJCX0KCX0KCgkvL3Jlc2V0CglpZiAobGFzdFJlc2V0VGltZSAhPSAwICYmIFRpbWUuQ3VycmVudCAtIGxhc3RSZXNldFRpbWUgPiByZXNldFRpbWUgKQoJewoJCWxhc3RSZXNldFRpbWUgPSBUaW1lLkN1cnJlbnQ7CgkJCgkJZm9yZWFjaCAodmFyIHBhaXIgaW4gb3JpZ2luYWxUcmFuc2Zvcm1zKQoJCQlwYWlyLktleS5UcmFuc2Zvcm0gPSBwYWlyLlZhbHVlOwoJfQp9Cg==")]
public class DynamicClass41F4FA92BF32BAAA0D93BB318FAC7C014D94107DCB078B2C9F2D4B43E90E769E
{
    public NeoAxis.CSharpScript Owner;
    const double resetTime = 20;
    double lastResetTime;
    Dictionary<MeshInSpace, Transform> originalTransforms = new Dictionary<MeshInSpace, Transform>();
    public void _SimulationStep(NeoAxis.Component obj)
    {
        //save initial state of objects
        if (lastResetTime == 0)
        {
            lastResetTime = Time.Current;
            foreach (var component in Scene.First.GetComponents<MeshInSpace>())
            {
                if (component.Name.Contains("SFCrate"))
                    originalTransforms[component] = component.Transform.Value;
            }
        }

        //reset
        if (lastResetTime != 0 && Time.Current - lastResetTime > resetTime)
        {
            lastResetTime = Time.Current;
            foreach (var pair in originalTransforms)
                pair.Key.Transform = pair.Value;
        }
    }
}
}
#endif