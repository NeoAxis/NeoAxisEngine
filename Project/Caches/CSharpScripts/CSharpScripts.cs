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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUGF0aGZpbmRpbmdHZW9tZXRyeV9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQp7Cgl2YXIgb2JqID0gKE9iamVjdEluU3BhY2Upc2VuZGVyOwoJdmFyIHNvdXJjZVRyYW5zZm9ybSA9IG9iai5UcmFuc2Zvcm0uVmFsdWU7Cgl2YXIgcm90YXRpb24gPSBRdWF0ZXJuaW9uLkZyb21Sb3RhdGVCeVooIEVuZ2luZUFwcC5FbmdpbmVUaW1lICogMC4yICk7CglvYmouVHJhbnNmb3JtID0gbmV3IFRyYW5zZm9ybSggc291cmNlVHJhbnNmb3JtLlBvc2l0aW9uLCByb3RhdGlvbiwgc291cmNlVHJhbnNmb3JtLlNjYWxlICk7IAp9Cg==")]
public class DynamicClassDC0B0074B27FF845D3921D53478ACDDD0FF9207A89AA90C0C0F968DC100764B3
{
    public NeoAxis.CSharpScript Owner;
    public void PathfindingGeometry_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var obj = (ObjectInSpace)sender;
        var sourceTransform = obj.Transform.Value;
        var rotation = Quaternion.FromRotateByZ(EngineApp.EngineTime * 0.2);
        obj.Transform = new Transform(sourceTransform.Position, rotation, sourceTransform.Scale);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgRGVmYXVsdEdhdGVfVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIGdhdGUgPSBzZW5kZXIgYXMgR2F0ZTsKCgkvL3VwZGF0ZSBvcGVuZWQgZ2F0ZSBzdGF0ZQoJaWYoIEVuZ2luZUFwcC5FbmdpbmVUaW1lICUgMTAgPCA1ICkKCQlnYXRlLkRlc2lyZWRTdGF0ZSA9IDA7CgllbHNlCgkJZ2F0ZS5EZXNpcmVkU3RhdGUgPSAxOwoJCgkvL3VwZGF0ZSBwYXRoZmluZGluZyBnZW9tZXRyeQoJdmFyIHBhdGhmaW5kaW5nR2VvbWV0cnkgPSBzZW5kZXIuR2V0Q29tcG9uZW50PFBhdGhmaW5kaW5nR2VvbWV0cnk+KCk7CQoJaWYoIHBhdGhmaW5kaW5nR2VvbWV0cnkgIT0gbnVsbCApCgkJcGF0aGZpbmRpbmdHZW9tZXRyeS5FbmFibGVkID0gIWdhdGUuSXNPcGVuOwp9Cg==")]
public class DynamicClass3D5E78164E9CBB46513C3027C5DA48D26700D350AF5ED2A5DF0600FBE3983F88
{
    public NeoAxis.CSharpScript Owner;
    public void DefaultGate_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var gate = sender as Gate;
        //update opened gate state
        if (EngineApp.EngineTime % 10 < 5)
            gate.DesiredState = 0;
        else
            gate.DesiredState = 1;
        //update pathfinding geometry
        var pathfindingGeometry = sender.GetComponent<PathfindingGeometry>();
        if (pathfindingGeometry != null)
            pathfindingGeometry.Enabled = !gate.IsOpen;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQm94MTJfVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewkKCXZhciBvYmogPSAoT2JqZWN0SW5TcGFjZSlzZW5kZXI7Cgl2YXIgc291cmNlVHJhbnNmb3JtID0gb2JqLlRyYW5zZm9ybS5WYWx1ZTsKCXZhciByb3RhdGlvbiA9IFF1YXRlcm5pb24uRnJvbVJvdGF0ZUJ5WiggRW5naW5lQXBwLkVuZ2luZVRpbWUgKiAwLjIgKTsKCW9iai5UcmFuc2Zvcm0gPSBuZXcgVHJhbnNmb3JtKCBzb3VyY2VUcmFuc2Zvcm0uUG9zaXRpb24sIHJvdGF0aW9uLCBzb3VyY2VUcmFuc2Zvcm0uU2NhbGUgKTsgCn0K")]
public class DynamicClassECF1C4F0EC7492DB674F40AC3E673E8F5FB8D5406C8B2B5FA9C3A702C360AF10
{
    public NeoAxis.CSharpScript Owner;
    public void Box12_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var obj = (ObjectInSpace)sender;
        var sourceTransform = obj.Transform.Value;
        var rotation = Quaternion.FromRotateByZ(EngineApp.EngineTime * 0.2);
        obj.Transform = new Transform(sourceTransform.Position, rotation, sourceTransform.Scale);
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uRG9vcl9DbGljayhOZW9BeGlzLkJ1dHRvbjNEIHNlbmRlciwgTmVvQXhpcy5Db21wb25lbnQgaW5pdGlhdG9yKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJLy9jaGVjayBrZXkgaW4gaW52ZW50b3J5Cgl7CgkJLy9nZXQgYSBjaGFyYWN0ZXIgb2YgdGhlIHBsYXllcgoJCXZhciBwbGF5ZXJDaGFyYWN0ZXIgPSBpbml0aWF0b3IgYXMgQ2hhcmFjdGVyMkQ7CgkJaWYgKHBsYXllckNoYXJhY3RlciA9PSBudWxsKQoJCXsKCQkJTG9nLldhcm5pbmcoInBsYXllckNoYXJhY3RlciBpcyBudWxsLiIpOwoJCQlyZXR1cm47CgkJfQoKCQkvL2NoZWNrcyBwbGF5ZXIncyBjaGFyYWN0ZXIgaGFzIGEga2V5CgkJdmFyIGl0ZW0gPSBwbGF5ZXJDaGFyYWN0ZXIuR2V0SXRlbUJ5UmVzb3VyY2VOYW1lKEAiQ29udGVudFxJdGVtcyAyRFxOZW9BeGlzXEtleVxLZXkuaXRlbTJkdHlwZSIpOwoJCWlmIChpdGVtID09IG51bGwpCgkJewoJCQlTY3JlZW5NZXNzYWdlcy5BZGQoIllvdSBkb24ndCBoYXZlIHRoZSBrZXkuIik7CgkJCXJldHVybjsKCQl9Cgl9CgoJLy9kaXNhYmxlIGRvb3IKCXZhciBib3ggPSBzY2VuZS5HZXRDb21wb25lbnQoIkJveCBEb29yIik7CglpZiAoYm94ICE9IG51bGwpCgkJYm94LkVuYWJsZWQgPSBmYWxzZTsKfQo=")]
public class DynamicClass9CCED16F2D2CB3ACDB8F7EBD2EB66F0E7AF67888D12F5F3B9614EA7E23DBDFEC
{
    public NeoAxis.CSharpScript Owner;
    public void ButtonDoor_Click(NeoAxis.Button3D sender, NeoAxis.Component initiator)
    {
        var scene = sender.ParentScene;
        //check key in inventory
        {
            //get a character of the player
            var playerCharacter = initiator as Character2D;
            if (playerCharacter == null)
            {
                Log.Warning("playerCharacter is null.");
                return;
            }

            //checks player's character has a key
            var item = playerCharacter.GetItemByResourceName(@"Content\Items 2D\NeoAxis\Key\Key.item2dtype");
            if (item == null)
            {
                ScreenMessages.Add("You don't have the key.");
                return;
            }
        }

        //disable door
        var box = scene.GetComponent("Box Door");
        if (box != null)
            box.Enabled = false;
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgU2Vuc29yMkRfT2JqZWN0RW50ZXIoTmVvQXhpcy5TZW5zb3IyRCBzZW5kZXIsIE5lb0F4aXMuT2JqZWN0SW5TcGFjZSBvYmopCnsKCVNjcmVlbk1lc3NhZ2VzLkFkZCgiVGhlIGNoYXJhY3RlciBpcyBpZGVudGlmaWVkIGJ5IHRoZSBzZW5zb3IuIik7Cn0K")]
public class DynamicClass560A3D30CFCCD2EA3059E898E1206E1190334C6CB5F7A88EA065BF00CFA53050
{
    public NeoAxis.CSharpScript Owner;
    public void Sensor2D_ObjectEnter(NeoAxis.Sensor2D sender, NeoAxis.ObjectInSpace obj)
    {
        ScreenMessages.Add("The character is identified by the sensor.");
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
}
#endif