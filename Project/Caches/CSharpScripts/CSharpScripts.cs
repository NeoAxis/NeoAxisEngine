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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlcikKewoJdmFyIHNjZW5lID0gc2VuZGVyLlBhcmVudFNjZW5lOwoKCS8vIEdldCBvYmplY3QgdHlwZS4KCXZhciByZXNvdXJjZU5hbWUgPSBAIlNhbXBsZXNcU3RhcnRlciBDb250ZW50XE1vZGVsc1xTY2ktZmkgQm94XFNjaS1maSBCb3gub2JqZWN0aW5zcGFjZSI7Cgl2YXIgYm94VHlwZSA9IE1ldGFkYXRhTWFuYWdlci5HZXRUeXBlKHJlc291cmNlTmFtZSk7CglpZihib3hUeXBlID09IG51bGwpCgl7CgkJTG9nLldhcm5pbmcoIk9iamVjdCB0eXBlIGlzIG51bGwuIik7CgkJcmV0dXJuOwoJfQoJCgkvLyBDcmVhdGUgdGhlIG9iamVjdCB3aXRob3V0IGVuYWJsaW5nLgoJdmFyIGJveCA9IChNZXNoSW5TcGFjZSlzY2VuZS5DcmVhdGVDb21wb25lbnQoYm94VHlwZSwgZW5hYmxlZDogZmFsc2UpOwoJLy92YXIgb2JqID0gc2NlbmUuQ3JlYXRlQ29tcG9uZW50PE1lc2hJblNwYWNlPihlbmFibGVkOiBmYWxzZSk7CgoJLy8gU2V0IGluaXRpYWwgcG9zaXRpb24uCgl2YXIgcmFuZG9tID0gbmV3IEZhc3RSYW5kb20oKTsKCWJveC5UcmFuc2Zvcm0gPSBuZXcgVHJhbnNmb3JtKAoJCW5ldyBWZWN0b3IzKDIgKyByYW5kb20uTmV4dCgwLjAsIDQuMCksIDggKyByYW5kb20uTmV4dCgwLjAsIDQuMCksIDEwICsgcmFuZG9tLk5leHQoMC4wLCA0LjApKSwgCgkJbmV3IEFuZ2xlcyhyYW5kb20uTmV4dCgzNjAuMCksIHJhbmRvbS5OZXh0KDM2MC4wKSwgcmFuZG9tLk5leHQoMzYwLjApKSk7CgkKCS8vIEVuYWJsZSB0aGUgb2JqZWN0IGluIHRoZSBzY2VuZS4KCWJveC5FbmFibGVkID0gdHJ1ZTsKCgkvL3ZhciBsaWdodCA9IHNjZW5lLkdldENvbXBvbmVudCgiRGlyZWN0aW9uYWwgTGlnaHQiKSBhcyBMaWdodDsKCS8vaWYgKGxpZ2h0ICE9IG51bGwpCgkvLwlsaWdodC5FbmFibGVkID0gc2VuZGVyLkFjdGl2YXRlZDsKfQo=")]
public class DynamicClass58602B33C450C9B96F091CED5C6B488128BEDCA0575C34F4361D62FFDADD5DDE
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender)
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

[CSharpScriptGeneratedAttribute("c3RhdGljIGJvb2wgY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nOwpzdGF0aWMgYm9vbCBjdXJyZW50TmlnaHQ7CnN0YXRpYyBpbnQgY3VycmVudFdlYXRoZXI7CnN0YXRpYyBib29sIGN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9yczsKc3RhdGljIGJvb2wgY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXI7CnN0YXRpYyBib29sIGN1cnJlbnRSZWZsZWN0aW9uUHJvYmUgPSB0cnVlOwoKY29uc3QgaW50IFN1bm55ID0gMDsKY29uc3QgaW50IFJhaW5GYWxsaW5nID0gMTsKY29uc3QgaW50IFJhaW5GYWxsZW4gPSAyOwoKdm9pZCBVcGRhdGVGb2dBbmRGYXJDbGlwUGxhbmUoRm9nIGZvZywgQ2FtZXJhIGNhbWVyYSkKewoJZm9nLkVuYWJsZWQgPSAhY3VycmVudE5pZ2h0Oy8vIHx8IGN1cnJlbnRSYWluOwoJZm9nLkRlbnNpdHkgPSBjdXJyZW50V2VhdGhlciA9PSBSYWluRmFsbGluZyA_IDAuMDEgOiAwLjAwMTsvL2ZvZy5EZW5zaXR5ID0gY3VycmVudFJhaW4gPyAwLjAxIDogMC4wMDE7CgoJaWYgKGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nKS8vaWYgKGN1cnJlbnRSYWluKQoJCWZvZy5BZmZlY3RCYWNrZ3JvdW5kID0gMTsKCWVsc2UKCQlmb2cuQWZmZWN0QmFja2dyb3VuZCA9IGN1cnJlbnROaWdodCA_IDAgOiAwLjU7CgoJaWYgKGN1cnJlbnROaWdodCkKCQlmb2cuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgwLCAwLCAwKTsKCWVsc2UKCQlmb2cuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgwLjQ1Mzk2MDgsIDAuNTE2MDM5MiwgMC42NTg4MjM1KTsKCglpZiAoZm9nLkVuYWJsZWQgJiYgZm9nLkFmZmVjdEJhY2tncm91bmQgPT0gMSkKCQljYW1lcmEuRmFyQ2xpcFBsYW5lID0gMzAwOwoJZWxzZQoJCWNhbWVyYS5GYXJDbGlwUGxhbmUgPSBjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmcgPyAyMDAwIDogMTAwMDsKfQoKdm9pZCBVcGRhdGVNaWNyb3BhcnRpY2xlc0luQWlyKCBDb21wb25lbnQgc2VuZGVyICkKewoJdmFyIHNjZW5lID0gc2VuZGVyLlBhcmVudFJvb3Q7Cgl2YXIgcmVuZGVyaW5nUGlwZWxpbmUgPSBzY2VuZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nUGlwZWxpbmVfQmFzaWM+KCJSZW5kZXJpbmcgUGlwZWxpbmUiKTsKCXZhciBlZmZlY3QgPSByZW5kZXJpbmdQaXBlbGluZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nRWZmZWN0X01pY3JvcGFydGljbGVzSW5BaXI+KGNoZWNrQ2hpbGRyZW46IHRydWUpOwoJaWYgKGVmZmVjdCAhPSBudWxsKQoJewoJCWlmIChjdXJyZW50TWljcm9wYXJ0aWNsZXNJbkFpcikKCQl7CgkJCWVmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuNywgMC42KTsKCQkJLy9lZmZlY3QuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAwLjgsIDAuNSk7CgkJCWVmZmVjdC5NdWx0aXBsaWVyID0gMC4wMDE1OwoJCX0KCQllbHNlCgkJewoJCQlpZiAoY3VycmVudE5pZ2h0KQoJCQl7CgkJCQllZmZlY3QuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgwLjc1LCAwLjc1LCAxKTsKCQkJCWVmZmVjdC5NdWx0aXBsaWVyID0gMC4wMDAxNTsKCQkJfQoJCQllbHNlCgkJCXsKCQkJCS8vc2ltdWxhdGUgaW5kaXJlY3QgbGlnaHRpbmcgYnkgbWVhbnMgbWljcm9wYXJ0aWNsZXMgaW4gYWlyCgkJCQllZmZlY3QuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAwLjgsIDAuNyk7CgkJCQkvL2VmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuOCwgMC41KTsKCQkJCWVmZmVjdC5NdWx0aXBsaWVyID0gMC4wMDAzOwoJCQl9CgoJCQkvL2VmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNzUsIDAuNzUsIDEpOwoJCQkvL2VmZmVjdC5NdWx0aXBsaWVyID0gMC4wMDAxNTsKCQl9Cgl9Cn0KCnZvaWQgRXhpdEZyb21WZWhpY2xlKE5lb0F4aXMuR2FtZU1vZGUgZ2FtZU1vZGUpCnsKCXZhciBvYmogPSBnYW1lTW9kZS5PYmplY3RDb250cm9sbGVkQnlQbGF5ZXIuVmFsdWUgYXMgVmVoaWNsZTsKCWlmIChvYmogIT0gbnVsbCkKCXsKCQl2YXIgaW5wdXRQcm9jZXNzaW5nID0gb2JqLkdldENvbXBvbmVudDxWZWhpY2xlSW5wdXRQcm9jZXNzaW5nPigpOwoJCWlmIChpbnB1dFByb2Nlc3NpbmcgIT0gbnVsbCkKCQkJaW5wdXRQcm9jZXNzaW5nLkV4aXRBbGxPYmplY3RzRnJvbVZlaGljbGUoZ2FtZU1vZGUpOwoJfQp9Cgp2b2lkIFByb2Nlc3NJbnB1dE1lc3NhZ2VFdmVudChOZW9BeGlzLkdhbWVNb2RlIHNlbmRlciwgTmVvQXhpcy5JbnB1dE1lc3NhZ2UgbWVzc2FnZSkKewoJdmFyIGtleURvd24gPSBtZXNzYWdlIGFzIElucHV0TWVzc2FnZUtleURvd247CglpZiAoa2V5RG93biAhPSBudWxsKS8vJiYgIXNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuQ29udHJvbCkpCgl7CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQxKQoJCXsKCQkJdmFyIG1hbmFnZXIgPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8QnVpbGRpbmdNYW5hZ2VyPigpOwoJCQlpZiAobWFuYWdlciAhPSBudWxsKQoJCQl7CgkJCQltYW5hZ2VyLkRpc3BsYXkgPSAhbWFuYWdlci5EaXNwbGF5OwoJCQkJbWFuYWdlci5Db2xsaXNpb24gPSBtYW5hZ2VyLkRpc3BsYXk7CgkJCX0KCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDIpCgkJewoJCQlFeGl0RnJvbVZlaGljbGUoc2VuZGVyKTsKCgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLlBhcmtlZFZlaGljbGVzID0gc3lzdGVtLlBhcmtlZFZlaGljbGVzLlZhbHVlICE9IDAgPyAwIDogNTAwMDsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDMpCgkJewoJCQlFeGl0RnJvbVZlaGljbGUoc2VuZGVyKTsKCgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLkZseWluZ1ZlaGljbGVzID0gc3lzdGVtLkZseWluZ1ZlaGljbGVzLlZhbHVlICE9IDAgPyAwIDogNTAwOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EOSkKCQl7CgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLlNpbXVsYXRlRHluYW1pY09iamVjdHMgPSAhc3lzdGVtLlNpbXVsYXRlRHluYW1pY09iamVjdHM7CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ0KQoJCXsKCQkJRXhpdEZyb21WZWhpY2xlKHNlbmRlcik7CgoJCQl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CgkJCWlmIChzeXN0ZW0gIT0gbnVsbCkKCQkJCXN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnMgPSBzeXN0ZW0uV2Fsa2luZ1BlZGVzdHJpYW5zLlZhbHVlICE9IDAgPyAwIDogMTAwOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EMCkKCQl7CgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLldhbGtpbmdQZWRlc3RyaWFuc01hbmFnZVRhc2tzID0gIXN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnNNYW5hZ2VUYXNrczsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDUpCgkJewoJCQl2YXIgc2NlbmUgPSAoU2NlbmUpc2VuZGVyLlBhcmVudFJvb3Q7CgkJCXZhciByZW5kZXJpbmdQaXBlbGluZSA9IHNjZW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdQaXBlbGluZT4oIlJlbmRlcmluZyBQaXBlbGluZSIpOwoJCQl2YXIgcmVmbGVjdGlvbiA9IHJlbmRlcmluZ1BpcGVsaW5lPy5HZXRDb21wb25lbnQ8UmVuZGVyaW5nRWZmZWN0X1JlZmxlY3Rpb24+KGNoZWNrQ2hpbGRyZW46IHRydWUpOwoJCQl2YXIgZm9nID0gc2NlbmUuR2V0Q29tcG9uZW50KCJGb2ciKSBhcyBGb2c7CgkJCXZhciBwcmVjaXBpdGF0aW9uID0gcmVuZGVyaW5nUGlwZWxpbmU_LkdldENvbXBvbmVudDxSZW5kZXJpbmdFZmZlY3RfUHJlY2lwaXRhdGlvbj4oY2hlY2tDaGlsZHJlbjogdHJ1ZSk7CgkJCXZhciBzb3VuZFNvdXJjZVJhaW4gPSBzY2VuZS5HZXRDb21wb25lbnQoIlNvdW5kIFNvdXJjZSBSYWluIikgYXMgU291bmRTb3VyY2U7CgkJCXZhciBjYW1lcmEgPSBzY2VuZS5HZXRDb21wb25lbnQ8Q2FtZXJhPigiQ2FtZXJhIERlZmF1bHQiKTsKCQkJdmFyIGRpcmVjdGlvbmFsTGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRpcmVjdGlvbmFsIExpZ2h0IikgYXMgTGlnaHQ7CgoJCQljdXJyZW50V2VhdGhlcisrOwoJCQlpZiAoY3VycmVudFdlYXRoZXIgPiAyKQoJCQkJY3VycmVudFdlYXRoZXIgPSAwOwoJCQkvL2N1cnJlbnRSYWluID0gIWN1cnJlbnRSYWluOwoKCQkJdHJ5CgkJCXsKCQkJCVVwZGF0ZUZvZ0FuZEZhckNsaXBQbGFuZShmb2csIGNhbWVyYSk7CgoJCQkJc291bmRTb3VyY2VSYWluLkVuYWJsZWQgPSBjdXJyZW50V2VhdGhlciA9PSBSYWluRmFsbGluZzsKCgkJCQlzY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGluZyA9IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nID8gMSA6IDA7CgkJCQlzY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGVuID0gKGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nIHx8IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsZW4pID8gMSA6IDA7CgoJCQkJLy9wcmVjaXBpdGF0aW9uLkVuYWJsZWQgPSBjdXJyZW50UmFpbjsKCQkJCS8vc291bmRTb3VyY2VSYWluLkVuYWJsZWQgPSBjdXJyZW50UmFpbjsKCQkJCS8vc2NlbmUuUHJlY2lwaXRhdGlvbkZhbGxpbmcgPSBjdXJyZW50UmFpbiA_IDEgOiAwOwoJCQkJLy9zY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGVuID0gY3VycmVudFJhaW4gPyAxIDogMDsKCgkJCQkvKgoJCQkJCQkJCWlmKGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nKS8vaWYgKGN1cnJlbnRSYWluKQoJCQkJCQkJCXsKCQkJCQkJCQkJZGlyZWN0aW9uYWxMaWdodC5NYXNrID0gbmV3IFJlZmVyZW5jZU5vVmFsdWUoQCJTYW1wbGVzXENpdHkgRGVtb1xTa2llc1xSYWluIGNsb3VkcyBtYXNrXFJhaW4gY2xvdWRzIG1hc2suanBnIik7CgkJCQkJCQkJCWRpcmVjdGlvbmFsTGlnaHQuTWFza1RyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oVmVjdG9yMy5aZXJvLCBRdWF0ZXJuaW9uLklkZW50aXR5LCBuZXcgVmVjdG9yMygwLjAwNSwgMC4wMDUsIDAuMDA1KSk7CgkJCQkJCQkJfQoJCQkJCQkJCWVsc2UKCQkJCQkJCQl7CgkJCQkJCQkJCWRpcmVjdGlvbmFsTGlnaHQuTWFzayA9IG51bGw7CgkJCQkJCQkJfQoJCQkJKi8KCQkJfQoJCQljYXRjaCAoRXhjZXB0aW9uIGUpCgkJCXsKCQkJCUxvZy5XYXJuaW5nKGUuTWVzc2FnZSk7CgkJCX0KCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ2KQoJCXsKCQkJdmFyIHNjZW5lID0gKFNjZW5lKXNlbmRlci5QYXJlbnRSb290OwoJCQl2YXIgYW1iaWVudExpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJBbWJpZW50IExpZ2h0IikgYXMgTGlnaHQ7CgkJCXZhciBkaXJlY3Rpb25hbExpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJEaXJlY3Rpb25hbCBMaWdodCIpIGFzIExpZ2h0OwoJCQl2YXIgc3RyZWV0TGlnaHRMaWdodHMgPSBzY2VuZS5HZXRDb21wb25lbnQoIlN0cmVldCBsaWdodCBsaWdodHMiKTsKCQkJdmFyIHNreSA9IHNjZW5lLkdldENvbXBvbmVudCgiU2t5IikgYXMgU2t5OwoJCQkvL3ZhciBkYXlTa3kgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRheSBza3kiKTsKCQkJLy92YXIgbmlnaHRTa3kgPSBzY2VuZS5HZXRDb21wb25lbnQoIk5pZ2h0IHNreSIpOwoJCQl2YXIgZm9nID0gc2NlbmUuR2V0Q29tcG9uZW50KCJGb2ciKSBhcyBGb2c7CgkJCXZhciBjYW1lcmEgPSBzY2VuZS5HZXRDb21wb25lbnQ8Q2FtZXJhPigiQ2FtZXJhIERlZmF1bHQiKTsKCgkJCWN1cnJlbnROaWdodCA9ICFjdXJyZW50TmlnaHQ7CgoJCQl0cnkKCQkJewoJCQkJc2NlbmUuVGltZU9mRGF5ID0gY3VycmVudE5pZ2h0ID8gMCA6IDEyOyAKCQkJCWFtYmllbnRMaWdodC5CcmlnaHRuZXNzID0gY3VycmVudE5pZ2h0ID8gMjUwMDAgOiAxMDAwMDA7CgkJCQlkaXJlY3Rpb25hbExpZ2h0LkVuYWJsZWQgPSAhY3VycmVudE5pZ2h0OwoJCQkJc3RyZWV0TGlnaHRMaWdodHMuRW5hYmxlZCA9IGN1cnJlbnROaWdodDsKCQkJCXNreS5Nb2RlID0gY3VycmVudE5pZ2h0ID8gU2t5Lk1vZGVFbnVtLlJlc291cmNlIDogU2t5Lk1vZGVFbnVtLlByb2NlZHVyYWw7CgkJCQkvL3NreS5Qcm9jZWR1cmFsSW50ZW5zaXR5ID0gY3VycmVudE5pZ2h0ID8gMCA6IDE7CgkJCQkvL2RheVNreS5FbmFibGVkID0gIWN1cnJlbnROaWdodDsKCQkJCS8vbmlnaHRTa3kuRW5hYmxlZCA9IGN1cnJlbnROaWdodDsKCQkJCVVwZGF0ZUZvZ0FuZEZhckNsaXBQbGFuZShmb2csIGNhbWVyYSk7CgkJCQlVcGRhdGVNaWNyb3BhcnRpY2xlc0luQWlyKHNlbmRlcik7CgkJCX0KCQkJY2F0Y2ggKEV4Y2VwdGlvbiBlKQoJCQl7CgkJCQlMb2cuV2FybmluZyhlLk1lc3NhZ2UpOwoJCQl9CgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EOCkKCQl7CgkJCUV4aXRGcm9tVmVoaWNsZShzZW5kZXIpOwoKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCXsKCQkJCWlmIChzeXN0ZW0uUGFya2VkVmVoaWNsZXNPYmplY3RNb2RlLlZhbHVlID09IFRyYWZmaWNTeXN0ZW0uT2JqZWN0TW9kZUVudW0uVmVoaWNsZUNvbXBvbmVudCkKCQkJCQlzeXN0ZW0uUGFya2VkVmVoaWNsZXNPYmplY3RNb2RlID0gVHJhZmZpY1N5c3RlbS5PYmplY3RNb2RlRW51bS5TdGF0aWNPYmplY3Q7CgkJCQllbHNlCgkJCQkJc3lzdGVtLlBhcmtlZFZlaGljbGVzT2JqZWN0TW9kZSA9IFRyYWZmaWNTeXN0ZW0uT2JqZWN0TW9kZUVudW0uVmVoaWNsZUNvbXBvbmVudDsKCQkJfQoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5ENykKCQl7CgkJCWN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA9ICFjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmc7CgoJCQl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50Um9vdDsKCQkJdmFyIHJlbmRlcmluZ1BpcGVsaW5lID0gc2NlbmUuR2V0Q29tcG9uZW50PFJlbmRlcmluZ1BpcGVsaW5lX0Jhc2ljPigiUmVuZGVyaW5nIFBpcGVsaW5lIik7CgkJCXZhciBjYW1lcmEgPSBzY2VuZS5HZXRDb21wb25lbnQ8Q2FtZXJhPigiQ2FtZXJhIERlZmF1bHQiKTsKCQkJdmFyIGZvZyA9IHNjZW5lLkdldENvbXBvbmVudCgiRm9nIikgYXMgRm9nOwoKCQkJLy9jYW1lcmEuRmFyQ2xpcFBsYW5lID0gY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID8gMjAwMCA6IDEwMDA7CgkJCXJlbmRlcmluZ1BpcGVsaW5lLk1pbmltdW1WaXNpYmxlU2l6ZU9mT2JqZWN0cyA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDIgOiA0OwoKCQkJcmVuZGVyaW5nUGlwZWxpbmUuU2hhZG93RGlyZWN0aW9uYWxEaXN0YW5jZSA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDYwMCA6IDIwMDsKCQkJcmVuZGVyaW5nUGlwZWxpbmUuU2hhZG93RGlyZWN0aW9uYWxMaWdodENhc2NhZGVzID0gY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID8gMyA6IDI7CgoJCQl0cnkKCQkJewoJCQkJVXBkYXRlRm9nQW5kRmFyQ2xpcFBsYW5lKGZvZywgY2FtZXJhKTsKCQkJfQoJCQljYXRjaCAoRXhjZXB0aW9uIGUpCgkJCXsKCQkJCUxvZy5XYXJuaW5nKGUuTWVzc2FnZSk7CgkJCX0KCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCgkJCS8qCgkJCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRSb290IGFzIFNjZW5lOwoJCQlpZiAoc2NlbmUgIT0gbnVsbCkKCQkJewoJCQkJaWYgKHNjZW5lLk9jdHJlZVRocmVhZGluZ01vZGUuVmFsdWUgPT0gT2N0cmVlQ29udGFpbmVyLlRocmVhZGluZ01vZGVFbnVtLkJhY2tncm91bmRUaHJlYWQpCgkJCQkJc2NlbmUuT2N0cmVlVGhyZWFkaW5nTW9kZSA9IE9jdHJlZUNvbnRhaW5lci5UaHJlYWRpbmdNb2RlRW51bS5TaW5nbGVUaHJlYWRlZDsKCQkJCWVsc2UKCQkJCQlzY2VuZS5PY3RyZWVUaHJlYWRpbmdNb2RlID0gT2N0cmVlQ29udGFpbmVyLlRocmVhZGluZ01vZGVFbnVtLkJhY2tncm91bmRUaHJlYWQ7CgkJCX0KCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCQkqLwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuQykKCQl7CgkJCWN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9ycyA9ICFjdXJyZW50UmFuZG9taXplU3RyZWV0TGlnaHRDb2xvcnM7CgkJCQoJCQl2YXIgbGlnaHRzID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50KCJTdHJlZXQgbGlnaHQgbGlnaHRzIik7CgkJCWlmKGxpZ2h0cyAhPSBudWxsKQoJCQl7CgkJCQl2YXIgcmFuZG9tID0gbmV3IEZhc3RSYW5kb20oKTsKCQkJCQoJCQkJZm9yZWFjaCh2YXIgbGlnaHQgaW4gbGlnaHRzLkdldENvbXBvbmVudHM8TGlnaHQ+KCkpCgkJCQl7CgkJCQkJaWYoY3VycmVudFJhbmRvbWl6ZVN0cmVldExpZ2h0Q29sb3JzKQoJCQkJCXsKCQkJCQkJdmFyIGNvbG9yID0gbGlnaHQuQ29sb3IuVmFsdWU7CgkJCQkJCXZhciBtYXggPSAwLjZmOy8vMC4yZjsKCQkJCQkJY29sb3IuUmVkICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCQkJCWNvbG9yLkdyZWVuICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCQkJCWNvbG9yLkJsdWUgKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJCQkJbGlnaHQuQ29sb3IgPSBjb2xvcjsKCQkJCQl9CgkJCQkJZWxzZQoJCQkJCXsKCQkJCQkJbGlnaHQuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAxLCAwLjcxMTAxOTYpOwoJCQkJCX0KCQkJCX0KCQkJfQkKCgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5NKQoJCXsKCQkJY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXIgPSAhY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXI7CgkJCVVwZGF0ZU1pY3JvcGFydGljbGVzSW5BaXIoc2VuZGVyKTsKCQkJCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLlApCgkJewoJCQljdXJyZW50UmVmbGVjdGlvblByb2JlID0gIWN1cnJlbnRSZWZsZWN0aW9uUHJvYmU7CgoJCQl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50Um9vdDsKCQkJZm9yZWFjaCAodmFyIHByb2JlIGluIHNjZW5lLkdldENvbXBvbmVudHM8UmVmbGVjdGlvblByb2JlPigpKQoJCQkJcHJvYmUuUmVhbFRpbWUgPSBjdXJyZW50UmVmbGVjdGlvblByb2JlOwoKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCX0KfQoKcHVibGljIHZvaWQgR2FtZU1vZGVfSW5wdXRNZXNzYWdlRXZlbnQoTmVvQXhpcy5HYW1lTW9kZSBzZW5kZXIsIE5lb0F4aXMuSW5wdXRNZXNzYWdlIG1lc3NhZ2UpCnsKCWlmICghc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5Db250cm9sKSkKCQlQcm9jZXNzSW5wdXRNZXNzYWdlRXZlbnQoc2VuZGVyLCBtZXNzYWdlKTsKfQoKcHVibGljIHZvaWQgR2FtZU1vZGVfRW5hYmxlZEluU2ltdWxhdGlvbihOZW9BeGlzLkNvbXBvbmVudCBvYmopCnsKCS8vYWN0aXZhdGUgbmlnaHQgbW9kZQoJUHJvY2Vzc0lucHV0TWVzc2FnZUV2ZW50KChHYW1lTW9kZSlvYmosIG5ldyBJbnB1dE1lc3NhZ2VLZXlEb3duKEVLZXlzLkQ2KSk7CgkKCS8vcmFuZG9taXplIHN0cmVldCBsaWdodHMKCXZhciBsaWdodHMgPSBvYmouUGFyZW50Um9vdC5HZXRDb21wb25lbnQoIlN0cmVldCBsaWdodCBsaWdodHMiKTsKCWlmKGxpZ2h0cyAhPSBudWxsKQoJewoJCXZhciByYW5kb20gPSBuZXcgRmFzdFJhbmRvbSgpOwoJCQoJCWZvcmVhY2godmFyIGxpZ2h0IGluIGxpZ2h0cy5HZXRDb21wb25lbnRzPExpZ2h0PigpKQoJCXsKCQkJLy9yYW5kb21pemUgcm90YXRpb24KCQkJdmFyIHRyID0gbGlnaHQuVHJhbnNmb3JtVjsKCQkJdHIgPSB0ci5VcGRhdGVSb3RhdGlvbihRdWF0ZXJuaW9uLkZyb21Sb3RhdGVCeVoocmFuZG9tLk5leHQoTWF0aC5QSSAqIDIpKSk7CgkJCWxpZ2h0LlRyYW5zZm9ybSA9IHRyOwoKLyoKCQkJLy9yYW5kb21pemUgY29sb3JzCgkJCXZhciBjb2xvciA9IGxpZ2h0LkNvbG9yLlZhbHVlOwoJCQl2YXIgbWF4ID0gMC42ZjsvLzAuMmY7CgkJCWNvbG9yLlJlZCArPSByYW5kb20uTmV4dCgtbWF4LCBtYXgpOwoJCQljb2xvci5HcmVlbiArPSByYW5kb20uTmV4dCgtbWF4LCBtYXgpOwoJCQljb2xvci5CbHVlICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCWxpZ2h0LkNvbG9yID0gY29sb3I7CiovCQkJCgkJfQoJfQkKfQ==")]
public class DynamicClass586AA445F09274BFA99667B39A53F123641A09E71D778E9F6A4B56C6F071EEB9
{
    public NeoAxis.CSharpScript Owner;
    static bool currentFarDistanceRendering;
    static bool currentNight;
    static int currentWeather;
    static bool currentRandomizeStreetLightColors;
    static bool currentMicroparticlesInAir;
    static bool currentReflectionProbe = true;
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
                    sky.Mode = currentNight ? Sky.ModeEnum.Resource : Sky.ModeEnum.Procedural;
                    //sky.ProceduralIntensity = currentNight ? 0 : 1;
                    //daySky.Enabled = !currentNight;
                    //nightSky.Enabled = currentNight;
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

            if (keyDown.Key == EKeys.P)
            {
                currentReflectionProbe = !currentReflectionProbe;
                var scene = sender.ParentRoot;
                foreach (var probe in scene.GetComponents<ReflectionProbe>())
                    probe.RealTime = currentReflectionProbe;
                message.Handled = true;
                return;
            }
        }
    }

    public void GameMode_InputMessageEvent(NeoAxis.GameMode sender, NeoAxis.InputMessage message)
    {
        if (!sender.IsKeyPressed(EKeys.Control))
            ProcessInputMessageEvent(sender, message);
    }

    public void GameMode_EnabledInSimulation(NeoAxis.Component obj)
    {
        //activate night mode
        ProcessInputMessageEvent((GameMode)obj, new InputMessageKeyDown(EKeys.D6));
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgRGVtb01vZGVfU2hvd0tleXNFdmVudChOZW9BeGlzLkRlbW9Nb2RlIHNlbmRlciwgU3lzdGVtLkNvbGxlY3Rpb25zLkdlbmVyaWMuTGlzdDxzdHJpbmc+IGxpbmVzKQp7Cgl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CglpZiAoc3lzdGVtID09IG51bGwpCgkJcmV0dXJuOwoJdmFyIHNjZW5lID0gc3lzdGVtLlBhcmVudFJvb3QgYXMgU2NlbmU7CglpZiAoc2NlbmUgPT0gbnVsbCkKCQlyZXR1cm47Cgl2YXIgcmVuZGVyaW5nUGlwZWxpbmUgPSBzY2VuZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nUGlwZWxpbmU+KCJSZW5kZXJpbmcgUGlwZWxpbmUiKTsKCWlmIChyZW5kZXJpbmdQaXBlbGluZSA9PSBudWxsKQoJCXJldHVybjsKCgl2YXIgZmFyRGlzdGFuY2VSZW5kZXJpbmcgPSByZW5kZXJpbmdQaXBlbGluZS5NaW5pbXVtVmlzaWJsZVNpemVPZk9iamVjdHMgPT0gMjsKCXZhciBmYXJEaXN0YW5jZVJlbmRlcmluZ1N0cmluZyA9IGZhckRpc3RhbmNlUmVuZGVyaW5nID8gIm9uIiA6ICJvZmYiOwoKCXZhciBwYXJrZWRWZWhpY2xlc0FzU3RhdGljID0gc3lzdGVtLlBhcmtlZFZlaGljbGVzT2JqZWN0TW9kZS5WYWx1ZSA9PSBUcmFmZmljU3lzdGVtLk9iamVjdE1vZGVFbnVtLlN0YXRpY09iamVjdDsKCXZhciBwYXJrZWRWZWhpY2xlc0FzU3RhdGljU3RyaW5nID0gcGFya2VkVmVoaWNsZXNBc1N0YXRpYyA_ICJvbiIgOiAib2ZmIjsKCgl2YXIgbXVsdGl0aHJlYWRlZFNjZW5lT2N0cmVlID0gc2NlbmUuT2N0cmVlVGhyZWFkaW5nTW9kZS5WYWx1ZSA9PSBPY3RyZWVDb250YWluZXIuVGhyZWFkaW5nTW9kZUVudW0uQmFja2dyb3VuZFRocmVhZDsKCXZhciBtdWx0aXRocmVhZGVkU2NlbmVPY3RyZWVTdHJpbmcgPSBtdWx0aXRocmVhZGVkU2NlbmVPY3RyZWUgPyAib24iIDogIm9mZiI7CgoJc3RyaW5nIHJhaW5TdGF0ZTsKCWlmIChzY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGluZyA+IDApCgkJcmFpblN0YXRlID0gImZhbGxpbmciOwoJZWxzZSBpZiAoc2NlbmUuUHJlY2lwaXRhdGlvbkZhbGxlbiA+IDApCgkJcmFpblN0YXRlID0gImZhbGxlbiI7CgllbHNlCgkJcmFpblN0YXRlID0gInN1bm55IjsKCgkvL3ZhciB3YWxraW5nUGVkZXN0cmlhbnNNYW5hZ2VUYXNrc1N0cmluZyA9IHN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnNNYW5hZ2VUYXNrcy5WYWx1ZSA_ICJvbiIgOiAib2ZmIjsKCglsaW5lcy5BZGQoIiIpOwoJbGluZXMuQWRkKCIxIC0gYnVpbGRpbmdzIik7CglsaW5lcy5BZGQoJCIyIC0gcGFya2VkIHZlaGljbGVzIC0ge3N5c3RlbS5HZXRQYXJrZWRWZWhpY2xlcygpLkNvdW50fSIpOwoJbGluZXMuQWRkKCQiMyAtIGZseWluZyB2ZWhpY2xlcyAtIHtzeXN0ZW0uR2V0Rmx5aW5nT2JqZWN0cygpLkNvdW50fSIpOwoJbGluZXMuQWRkKCQiNCAtIHdhbGtpbmcgcGVkZXN0cmlhbnMgLSB7c3lzdGVtLkdldFdhbGtpbmdQZWRlc3RyaWFucygpLkNvdW50fSIpOwoJbGluZXMuQWRkKCQiNSAtIHJhaW4gLSB7cmFpblN0YXRlfSIpOy8vIC0ge3dhbGtpbmdQZWRlc3RyaWFuc01hbmFnZVRhc2tzU3RyaW5nfSIpOwoJbGluZXMuQWRkKCQiNiAtIHRpbWUgb2YgZGF5Iik7Ly8gLSB7d2Fsa2luZ1BlZGVzdHJpYW5zTWFuYWdlVGFza3NTdHJpbmd9Iik7CglsaW5lcy5BZGQoJCI3IC0gZmFyIGRpc3RhbmNlIHJlbmRlcmluZyAtIHtmYXJEaXN0YW5jZVJlbmRlcmluZ1N0cmluZ30iKTsKCWxpbmVzLkFkZCgkIkMgLSByYW5kb21pemUgc3RyZWV0IGxpZ2h0IGNvbG9ycyIpOwoJbGluZXMuQWRkKCQiTSAtIG1pY3JvcGFydGljbGVzIGluIGFpciAoZHVzdCkiKTsKCWxpbmVzLkFkZCgkIlAgLSByZWFsLXRpbWUgcmVmbGVjdGlvbiBwcm9iZSIpOwoJbGluZXMuQWRkKCIiKTsKCWxpbmVzLkFkZCgkIjggLSBwYXJrZWQgdmVoaWNsZXMgYXMgc3RhdGljIG9iamVjdHMgLSB7cGFya2VkVmVoaWNsZXNBc1N0YXRpY1N0cmluZ30iKTsKCWxpbmVzLkFkZCgiOSAtIHNpbXVsYXRlIGZseWluZyB2ZWhpY2xlcyIpOwoJbGluZXMuQWRkKCQiMCAtIGFjdGl2ZSB3YWxraW5nIHBlZGVzdHJpYW5zIik7Ly8gLSB7d2Fsa2luZ1BlZGVzdHJpYW5zTWFuYWdlVGFza3NTdHJpbmd9Iik7CgkvL2xpbmVzLkFkZCgkIjAgLSBtdWx0aXRocmVhZGVkIHNjZW5lIG9jdHJlZSAtIHttdWx0aXRocmVhZGVkU2NlbmVPY3RyZWVTdHJpbmd9Iik7Cn0K")]
public class DynamicClassBFF825A2710DB22C702DAA8724300E161E0F99519629413A4015A9665363410F
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
        lines.Add($"P - real-time reflection probe");
        lines.Add("");
        lines.Add($"8 - parked vehicles as static objects - {parkedVehiclesAsStaticString}");
        lines.Add("9 - simulate flying vehicles");
        lines.Add($"0 - active walking pedestrians"); // - {walkingPedestriansManageTasksString}");
    //lines.Add($"0 - multithreaded scene octree - {multithreadedSceneOctreeString}");
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlcikKewoJdmFyIHNjZW5lID0gc2VuZGVyLlBhcmVudFNjZW5lOwoKCXZhciBncm91bmQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkdyb3VuZCIpIGFzIE1lc2hJblNwYWNlOwoJaWYgKGdyb3VuZCAhPSBudWxsKQoJewoJCWlmICghZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbC5SZWZlcmVuY2VTcGVjaWZpZWQpCgkJewoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKAoJCQkJQCJDb250ZW50XE1hdGVyaWFsc1xCYXNpYyBMaWJyYXJ5XENvbmNyZXRlXENvbmNyZXRlIEZsb29yIDAxLm1hdGVyaWFsIik7CgkJfQoJCWVsc2UKCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IG51bGw7Cgl9Cn0K")]
public class DynamicClass3F5931BF1948DE33E56B3759BE3B1039B68AE910643E396284C12A54E876298C
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender)
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX0dlbmVyYXRlU3RhZ2UoTmVvQXhpcy5QbGFudFR5cGUgc2VuZGVyLCBOZW9BeGlzLlBsYW50R2VuZXJhdG9yIGdlbmVyYXRvciwgTmVvQXhpcy5QbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0gc3RhZ2UpCnsKI2lmICFERVBMT1kKCQoJLy9oZXJlIGlzIGEgc2NyaXB0IGZvciB0aGUgcGxhbnQgZ2VuZXJhdG9yIHRvIHNwZWNpYWxpemUgb3VyIHBsYW50IHR5cGUKCQoJc3dpdGNoKCBzdGFnZSApCgl7CgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5UcnVuazoKCQl7CgkJCXZhciBtYXRlcmlhbCA9IGdlbmVyYXRvci5GaW5kU3VpdGFibGVNYXRlcmlhbCggUGxhbnRNYXRlcmlhbC5QYXJ0VHlwZUVudW0uQmFyayApOwoJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBuZXcgVHJhbnNmb3JtKCBWZWN0b3IzLlplcm8sIFF1YXRlcm5pb24uTG9va0F0KCBWZWN0b3IzLlpBeGlzLCBWZWN0b3IzLlhBeGlzICkgKTsKCQkJdmFyIGxlbmd0aCA9IGdlbmVyYXRvci5IZWlnaHQgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjgsIDEuMiApOwoJCQl2YXIgdGhpY2tuZXNzID0gbGVuZ3RoIC8gNjAuMDsKCQoJCQl2YXIgdGhpY2tuZXNzRmFjdG9yID0gbmV3IEN1cnZlQ3ViaWNTcGxpbmUxRigpOwoJCQl0aGlja25lc3NGYWN0b3IuQWRkUG9pbnQoIG5ldyBDdXJ2ZTFGLlBvaW50KCAwLCAxICkgKTsKCQkJdGhpY2tuZXNzRmFjdG9yLkFkZFBvaW50KCBuZXcgQ3VydmUxRi5Qb2ludCggMSwgMC4zM2YgKSApOwoJCQkvL3RoaWNrbmVzc0ZhY3Rvci5BZGRQb2ludCggbmV3IEN1cnZlMUYuUG9pbnQoIDAuOTVmLCAwLjMzZiApICk7CgkJCS8vdGhpY2tuZXNzRmFjdG9yLkFkZFBvaW50KCBuZXcgQ3VydmUxRi5Qb2ludCggMSwgMCApICk7CgkKCQkJZ2VuZXJhdG9yLlRydW5rcy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50Q3lsaW5kZXIoIG51bGwsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybSwgbGVuZ3RoLCB0aGlja25lc3MsIHRoaWNrbmVzc0ZhY3RvciwgMTAsIDEzLCB0aGlja25lc3MgKiAwLjUsIDAgKSApOwoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uQnJhbmNoOgoJCXsKCQkJdmFyIGNvdW50ID0gNzsKCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQljb3VudCA9IChpbnQpKCAoZG91YmxlKWNvdW50ICogTWF0aC5Qb3coIGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSwgMiApICk7CgkKCQkJdmFyIHBhcmVudCA9IGdlbmVyYXRvci5UcnVua3NbIDAgXTsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkKCQkJdmFyIGFkZGVkID0gMDsKCQkJZm9yKCBpbnQgbiA9IDA7IG4gPCBjb3VudCAqIDEwOyBuKysgKQoJCQl7CgkJCQl2YXIgdGltZUZhY3Rvck9uUGFyZW50Q3VydmUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjA1LCAwLjY1ICk7CgkJCQkvL3ZhciB0aW1lRmFjdG9yT25QYXJlbnRDdXJ2ZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMiwgMC42NSApOwoJCQkJdmFyIHZlcnRpY2FsQW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAyMC4wLCA1MC4wICk7CgkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NS4wLCA0NS4wICk7CgkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yT25QYXJlbnRDdXJ2ZSwgZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICksIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJdmFyIHRoaWNrbmVzcyA9IHN0YXJ0VHJhbnNmb3JtLnBhcmVudFRoaWNrbmVzcyAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4wICk7CgkKCQkJCXZhciBtaW5CcmFuY2hUd2lnTGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAvIDE1MC4wOwoJCgkJCQl2YXIgbGVuZ3RoID0gdGhpY2tuZXNzICogMzUuMDsKCQkJCWlmKCBsZW5ndGggPj0gbWluQnJhbmNoVHdpZ0xlbmd0aCApCgkJCQl7CgkJCQkJdmFyIHRoaWNrbmVzc0ZhY3RvciA9IG5ldyBDdXJ2ZUN1YmljU3BsaW5lMUYoKTsKCQkJCQl0aGlja25lc3NGYWN0b3IuQWRkUG9pbnQoIG5ldyBDdXJ2ZTFGLlBvaW50KCAwLCAxICkgKTsKCQkJCQl0aGlja25lc3NGYWN0b3IuQWRkUG9pbnQoIG5ldyBDdXJ2ZTFGLlBvaW50KCAxLCAwLjMzZiApICk7CgkKCQkJCQlnZW5lcmF0b3IuQnJhbmNoZXMuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBwYXJlbnQsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybS50cmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCB0aGlja25lc3NGYWN0b3IsIDEwLjAsIDEzLjAsIHRoaWNrbmVzcyAqIDAuNSwgMy4wICkgKTsKCQoJCQkJCWFkZGVkKys7CgkJCQkJaWYoIGFkZGVkID49IGNvdW50ICkKCQkJCQkJYnJlYWs7CgkJCQl9CgkJCX0KCQl9CgkJYnJlYWs7CgkKCS8vY2FzZSBFbGVtZW50VHlwZUVudW0uVHdpZzoKCS8vCWJyZWFrOwoJCgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5GbG93ZXI6CgkJewoJCQlmb3IoIGludCBuID0gMDsgbiA8IGdlbmVyYXRvci5UcnVua3MuQ291bnQgKyBnZW5lcmF0b3IuQnJhbmNoZXMuQ291bnQ7IG4rKyApCgkJCXsKCQkJCXZhciBtYXR1cml0eSA9IGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZS5WYWx1ZSAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCQlpZiggbWF0dXJpdHkgPiAwLjMzICkKCQkJCXsKCQkJCQlQbGFudEdlbmVyYXRvci5FbGVtZW50IHBhcmVudDsKCQkJCQlpZiggbiA8IGdlbmVyYXRvci5UcnVua3MuQ291bnQgKQoJCQkJCQlwYXJlbnQgPSBnZW5lcmF0b3IuVHJ1bmtzWyBuIF07CgkJCQkJZWxzZQoJCQkJCQlwYXJlbnQgPSBnZW5lcmF0b3IuQnJhbmNoZXNbIG4gLSBnZW5lcmF0b3IuVHJ1bmtzLkNvdW50IF07CgkKCQkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkZsb3dlciApOwoJCgkJCQkJLy8hISEhdHdpc3QgcmFuZG9tCgkKCQkJCQl2YXIgdHJhbnNmb3JtMSA9IHBhcmVudC5DdXJ2ZS5HZXRUcmFuc2Zvcm1CeVRpbWVGYWN0b3IoIDEgKTsKCQoJCQkJCXZhciBkaXJlY3Rpb24gPSAoIHRyYW5zZm9ybTEuUG9zaXRpb24gLSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtQnlUaW1lRmFjdG9yKCAwLjk5ICkuUG9zaXRpb24gKS5HZXROb3JtYWxpemUoKTsKCQkJCQl2YXIgcm90YXRpb24gPSBRdWF0ZXJuaW9uLkZyb21EaXJlY3Rpb25aQXhpc1VwKCBkaXJlY3Rpb24gKTsKCQoJCQkJCXZhciB0cmFuc2Zvcm0gPSBuZXcgVHJhbnNmb3JtKCB0cmFuc2Zvcm0xLlBvc2l0aW9uLCByb3RhdGlvbiApOwoJCgkJCQkJdmFyIGxlbmd0aCA9IG1hdGVyaWFsICE9IG51bGwgPyBtYXRlcmlhbC5SZWFsTGVuZ3RoLlZhbHVlIDogZ2VuZXJhdG9yLkhlaWdodCAvIDE1LjA7CgkJCQkJbGVuZ3RoICo9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCQkJaWYoIG1hdHVyaXR5IDwgMSApCgkJCQkJCWxlbmd0aCAqPSBtYXR1cml0eTsKCQoJCQkJCXZhciB3aWR0aCA9IGxlbmd0aDsKCQoJCQkJCWdlbmVyYXRvci5GbG93ZXJzLkFkZCggZ2VuZXJhdG9yLkNyZWF0ZUVsZW1lbnRCb3dsKCBwYXJlbnQsIG1hdGVyaWFsLCB0cmFuc2Zvcm0sIGxlbmd0aCwgd2lkdGgsIG1hdHVyaXR5ICkgKTsKCQkJCX0KCQkJfQoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uTGVhZjoKCQlpZiggZ2VuZXJhdG9yLkJyYW5jaGVzLkNvdW50ICE9IDAgfHwgZ2VuZXJhdG9yLlR3aWdzLkNvdW50ICE9IDAgKQoJCXsKCQkJdmFyIHNlbGVjdG9yID0gbmV3IFBsYW50R2VuZXJhdG9yLlNlbGVjdG9yQnlQcm9iYWJpbGl0eSggZ2VuZXJhdG9yICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuQnJhbmNoZXMgKTsKCQkJLy9zZWxlY3Rvci5BZGRFbGVtZW50cyggVHdpZ3MgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5UcnVua3MgKTsKCQoJCQkvLyEhISHRgNCw0YHQv9GA0LXQtNC10LvRj9GC0Ywg0LIg0LfQsNCy0LjRgdC40LzQvtGB0YLQuCDQvtGCINC00LvQuNC90YsKCQoJCQkvLyEhISHRgNCw0LLQvdC+0LzQtdGA0L3QviDRgNCw0YHQv9GA0LXQtNC10LvRj9GC0YwuINCx0YDQsNC90YfQuCwg0LLQtdGC0LrQuCDRgtC+0LbQtQoJCgkJCS8vISEhIdC_0YDQuNC80LXQvdGP0YLRjCBMZWFmQ291bnQKCQoJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJyYW5jaFdpdGhMZWF2ZXMgKTsKCQoJCQl2YXIgY291bnQgPSA1MDsKCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQljb3VudCA9IChpbnQpKCAoZG91YmxlKWNvdW50ICogTWF0aC5Qb3coIGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSwgMiApICk7CgkKCQkJLy9pZiggTE9EID49IDIgKQoJCQkvLwljb3VudCAvPSAyOwoJCQkvL2lmKCBMT0QgPj0gMyApCgkJCS8vCWNvdW50IC89IDY7CgkKCQkJZm9yKCBpbnQgbiA9IDA7IG4gPCBjb3VudDsgbisrICkKCQkJewoJCQkJdmFyIHBhcmVudCA9IHNlbGVjdG9yLkdldCgpOwoJCgkJCQkvLyEhISHQv9C+0LLQvtGA0LDRh9C40LLQsNGC0Ywg0L_QviDQs9C+0YDQuNC30L7QvdGC0LDQu9C4PwoJCgkJCQkvLyEhISHRgNCw0YHQv9GA0LXQtNC10LvQtdC90LjQtQoJCgkJCQkvLyEhISHQvtGA0LjQtdC90YLQsNGG0LjRjyDQvtGC0L3QvtGB0LjRgtC10LvRjNC90L4g0YHQvtC70L3RhtCwL9Cy0LXRgNGF0LAKCQoJCQkJdmFyIHZlcnRpY2FsQW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAtMzAuMCwgMzAuMCApOwoJCQkJdmFyIHR3aXN0QW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAtOTAuMCwgOTAuMCApOwoJCgkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjAxLCAwLjY1ICksIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDEuMCApLCB2ZXJ0aWNhbEFuZ2xlLCB0d2lzdEFuZ2xlICk7CgkKCQkJCS8vISEhIXRpbHRBbmdsZQoJCgkJCQl2YXIgbGVuZ3RoID0gMC4xOwoJCQkJaWYoIG1hdGVyaWFsICE9IG51bGwgKQoJCQkJewoJCQkJCXZhciBtYXR1cml0eSA9IE1hdGguTWluKCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UuVmFsdWUsIDEuMCApOwoJCQkJCWxlbmd0aCA9IG1hdGVyaWFsLlJlYWxMZW5ndGggKiBtYXR1cml0eSAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCQl9CgkKCQkJCS8vISEhIWNyb3NzPwoJCQkJZ2VuZXJhdG9yLkxlYXZlcy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50UmliYm9uKCBwYXJlbnQsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybS50cmFuc2Zvcm0sIGxlbmd0aCwgMCwgZmFsc2UsIDAgKSApOwoJCQl9CgkKCQkJLy8hISEhCgkJCS8v0L_RgNC+0LLQtdGA0Y_RgtGMINC80LDRgtC10YDQuNCw0Lsg0LXRgdGC0Ywg0LvQuCDQstC10YLQutCwLgoJCQkvL9C10YHQu9C4INC90LXRgiDRgtC+0LPQtNCwINC00LXQu9Cw0YLRjCDQu9C40YHRgtGM0Y8uINC10YHRgtGMINC10YHRgtGMINGC0L7Qs9C00LAg0LLRgdGOINCy0LXRgtC60YMg0YDQuNCx0LHQvtC90L7QvC4KCQoJCX0KCQlicmVhazsKCX0KCQojZW5kaWYKfQo=")]
public class DynamicClass06193D10EC2AA9DDBF6A091FBF2F28AEE16CFB1711F516F04A994EEACE9CE028
{
    public NeoAxis.CSharpScript Owner;
    public void _GenerateStage(NeoAxis.PlantType sender, NeoAxis.PlantGenerator generator, NeoAxis.PlantGenerator.ElementTypeEnum stage)
    {
#if !DEPLOY
        //here is a script for the plant generator to specialize our plant type
        switch (stage)
        {
            case PlantGenerator.ElementTypeEnum.Trunk:
            {
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var startTransform = new Transform(Vector3.Zero, Quaternion.LookAt(Vector3.ZAxis, Vector3.XAxis));
                var length = generator.Height * generator.Randomizer.Next(0.8, 1.2);
                var thickness = length / 60.0;
                var thicknessFactor = new CurveCubicSpline1F();
                thicknessFactor.AddPoint(new Curve1F.Point(0, 1));
                thicknessFactor.AddPoint(new Curve1F.Point(1, 0.33f));
                //thicknessFactor.AddPoint( new Curve1F.Point( 0.95f, 0.33f ) );
                //thicknessFactor.AddPoint( new Curve1F.Point( 1, 0 ) );
                generator.Trunks.Add(generator.CreateElementCylinder(null, material, startTransform, length, thickness, thicknessFactor, 10, 13, thickness * 0.5, 0));
            }

                break;
            case PlantGenerator.ElementTypeEnum.Branch:
            {
                var count = 7;
                if (generator.Age < generator.PlantType.MatureAge)
                    count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                var parent = generator.Trunks[0];
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var added = 0;
                for (int n = 0; n < count * 10; n++)
                {
                    var timeFactorOnParentCurve = generator.Randomizer.Next(0.05, 0.65);
                    //var timeFactorOnParentCurve = generator.Randomizer.Next( 0.2, 0.65 );
                    var verticalAngle = generator.Randomizer.Next(20.0, 50.0);
                    var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                    var startTransform = parent.Curve.GetTransformOnSurface(timeFactorOnParentCurve, generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                    var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.8, 1.0);
                    var minBranchTwigLength = generator.Height / 150.0;
                    var length = thickness * 35.0;
                    if (length >= minBranchTwigLength)
                    {
                        var thicknessFactor = new CurveCubicSpline1F();
                        thicknessFactor.AddPoint(new Curve1F.Point(0, 1));
                        thicknessFactor.AddPoint(new Curve1F.Point(1, 0.33f));
                        generator.Branches.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, thicknessFactor, 10.0, 13.0, thickness * 0.5, 3.0));
                        added++;
                        if (added >= count)
                            break;
                    }
                }
            }

                break;
            //case ElementTypeEnum.Twig:
            //	break;
            case PlantGenerator.ElementTypeEnum.Flower:
            {
                for (int n = 0; n < generator.Trunks.Count + generator.Branches.Count; n++)
                {
                    var maturity = generator.Age / generator.PlantType.MatureAge.Value * generator.Randomizer.Next(0.8, 1.2);
                    if (maturity > 0.33)
                    {
                        PlantGenerator.Element parent;
                        if (n < generator.Trunks.Count)
                            parent = generator.Trunks[n];
                        else
                            parent = generator.Branches[n - generator.Trunks.Count];
                        var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Flower);
                        //!!!!twist random
                        var transform1 = parent.Curve.GetTransformByTimeFactor(1);
                        var direction = (transform1.Position - parent.Curve.GetTransformByTimeFactor(0.99).Position).GetNormalize();
                        var rotation = Quaternion.FromDirectionZAxisUp(direction);
                        var transform = new Transform(transform1.Position, rotation);
                        var length = material != null ? material.RealLength.Value : generator.Height / 15.0;
                        length *= generator.Randomizer.Next(0.8, 1.2);
                        if (maturity < 1)
                            length *= maturity;
                        var width = length;
                        generator.Flowers.Add(generator.CreateElementBowl(parent, material, transform, length, width, maturity));
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Leaf:
                if (generator.Branches.Count != 0 || generator.Twigs.Count != 0)
                {
                    var selector = new PlantGenerator.SelectorByProbability(generator);
                    selector.AddElements(generator.Branches);
                    //selector.AddElements( Twigs );
                    selector.AddElements(generator.Trunks);
                    //!!!!распределять в зависимости от длины
                    //!!!!равномерно распределять. бранчи, ветки тоже
                    //!!!!применять LeafCount
                    var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.BranchWithLeaves);
                    var count = 50;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                    //if( LOD >= 2 )
                    //	count /= 2;
                    //if( LOD >= 3 )
                    //	count /= 6;
                    for (int n = 0; n < count; n++)
                    {
                        var parent = selector.Get();
                        //!!!!поворачивать по горизонтали?
                        //!!!!распределение
                        //!!!!ориентация относительно солнца/верха
                        var verticalAngle = generator.Randomizer.Next(-30.0, 30.0);
                        var twistAngle = generator.Randomizer.Next(-90.0, 90.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(generator.Randomizer.Next(0.01, 0.65), generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        //!!!!tiltAngle
                        var length = 0.1;
                        if (material != null)
                        {
                            var maturity = Math.Min(generator.Age / generator.PlantType.MatureAge.Value, 1.0);
                            length = material.RealLength * maturity * generator.Randomizer.Next(0.8, 1.2);
                        }

                        //!!!!cross?
                        generator.Leaves.Add(generator.CreateElementRibbon(parent, material, startTransform.transform, length, 0, false, 0));
                    }
                //!!!!
                //проверять материал есть ли ветка.
                //если нет тогда делать листья. есть есть тогда всю ветку риббоном.
                }

                break;
        }
#endif
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX0dlbmVyYXRlU3RhZ2UoTmVvQXhpcy5QbGFudFR5cGUgc2VuZGVyLCBOZW9BeGlzLlBsYW50R2VuZXJhdG9yIGdlbmVyYXRvciwgTmVvQXhpcy5QbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0gc3RhZ2UpCnsKI2lmICFERVBMT1kKCQoJLy9oZXJlIGlzIGEgc2NyaXB0IGZvciB0aGUgcGxhbnQgZ2VuZXJhdG9yIHRvIHNwZWNpYWxpemUgb3VyIHBsYW50IHR5cGUKCQoJc3dpdGNoKCBzdGFnZSApCgl7CgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5UcnVuazoKCQl7CgkJCXZhciBtYXRlcmlhbCA9IGdlbmVyYXRvci5GaW5kU3VpdGFibGVNYXRlcmlhbCggUGxhbnRNYXRlcmlhbC5QYXJ0VHlwZUVudW0uQnJhbmNoV2l0aExlYXZlcyApOwoJCQl2YXIgbGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodDsKCgkJCXsKCQkJCXZhciB0cmFuc2Zvcm0gPSBuZXcgVHJhbnNmb3JtKCBWZWN0b3IzLlplcm8sIE1hdHJpeDMuTG9va0F0KCBWZWN0b3IzLlpBeGlzLCBWZWN0b3IzLlhBeGlzICkuVG9RdWF0ZXJuaW9uKCkgKTsKCQkJCWdlbmVyYXRvci5UcnVua3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudFJpYmJvbiggbnVsbCwgbWF0ZXJpYWwsIHRyYW5zZm9ybSwgbGVuZ3RoLCAwLCBmYWxzZSwgMCApICk7CgkJCX0KCgkJCXsKCQkJCXZhciB0cmFuc2Zvcm0gPSBuZXcgVHJhbnNmb3JtKCBWZWN0b3IzLlplcm8sIE1hdHJpeDMuTG9va0F0KCBWZWN0b3IzLlpBeGlzLCBWZWN0b3IzLllBeGlzICkuVG9RdWF0ZXJuaW9uKCkgKTsKCQkJCWdlbmVyYXRvci5UcnVua3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudFJpYmJvbiggbnVsbCwgbWF0ZXJpYWwsIHRyYW5zZm9ybSwgbGVuZ3RoLCAwLCBmYWxzZSwgMCApICk7CgkJCX0KCQl9CgkJYnJlYWs7Cgl9CgkKI2VuZGlmCn0K")]
public class DynamicClass48DD214389DFFAD56FD907A468AAD7524113E5BB4C3BDD4E2DC568FB7162CF63
{
    public NeoAxis.CSharpScript Owner;
    public void _GenerateStage(NeoAxis.PlantType sender, NeoAxis.PlantGenerator generator, NeoAxis.PlantGenerator.ElementTypeEnum stage)
    {
#if !DEPLOY
        //here is a script for the plant generator to specialize our plant type
        switch (stage)
        {
            case PlantGenerator.ElementTypeEnum.Trunk:
            {
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.BranchWithLeaves);
                var length = generator.Height;
                {
                    var transform = new Transform(Vector3.Zero, Matrix3.LookAt(Vector3.ZAxis, Vector3.XAxis).ToQuaternion());
                    generator.Trunks.Add(generator.CreateElementRibbon(null, material, transform, length, 0, false, 0));
                }

                {
                    var transform = new Transform(Vector3.Zero, Matrix3.LookAt(Vector3.ZAxis, Vector3.YAxis).ToQuaternion());
                    generator.Trunks.Add(generator.CreateElementRibbon(null, material, transform, length, 0, false, 0));
                }
            }

                break;
        }
#endif
    }
}

[CSharpScriptGeneratedAttribute("c3RhdGljIGJvb2wgY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nOwpzdGF0aWMgYm9vbCBjdXJyZW50TmlnaHQ7CnN0YXRpYyBpbnQgY3VycmVudFdlYXRoZXI7CnN0YXRpYyBib29sIGN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9yczsKc3RhdGljIGJvb2wgY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXI7CnN0YXRpYyBib29sIGN1cnJlbnRSZWZsZWN0aW9uUHJvYmUgPSB0cnVlOwoKY29uc3QgaW50IFN1bm55ID0gMDsKY29uc3QgaW50IFJhaW5GYWxsaW5nID0gMTsKY29uc3QgaW50IFJhaW5GYWxsZW4gPSAyOwoKdm9pZCBVcGRhdGVGb2dBbmRGYXJDbGlwUGxhbmUoRm9nIGZvZywgQ2FtZXJhIGNhbWVyYSkKewoJZm9nLkVuYWJsZWQgPSAhY3VycmVudE5pZ2h0Oy8vIHx8IGN1cnJlbnRSYWluOwoJZm9nLkRlbnNpdHkgPSBjdXJyZW50V2VhdGhlciA9PSBSYWluRmFsbGluZyA_IDAuMDEgOiAwLjAwMTsvL2ZvZy5EZW5zaXR5ID0gY3VycmVudFJhaW4gPyAwLjAxIDogMC4wMDE7CgoJaWYgKGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nKS8vaWYgKGN1cnJlbnRSYWluKQoJCWZvZy5BZmZlY3RCYWNrZ3JvdW5kID0gMTsKCWVsc2UKCQlmb2cuQWZmZWN0QmFja2dyb3VuZCA9IGN1cnJlbnROaWdodCA_IDAgOiAwLjU7CgoJaWYgKGN1cnJlbnROaWdodCkKCQlmb2cuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgwLCAwLCAwKTsKCWVsc2UKCQlmb2cuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgwLjQ1Mzk2MDgsIDAuNTE2MDM5MiwgMC42NTg4MjM1KTsKCglpZiAoZm9nLkVuYWJsZWQgJiYgZm9nLkFmZmVjdEJhY2tncm91bmQgPT0gMSkKCQljYW1lcmEuRmFyQ2xpcFBsYW5lID0gMzAwOwoJZWxzZQoJCWNhbWVyYS5GYXJDbGlwUGxhbmUgPSBjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmcgPyAyMDAwIDogMTAwMDsKfQoKdm9pZCBVcGRhdGVNaWNyb3BhcnRpY2xlc0luQWlyKCBDb21wb25lbnQgc2VuZGVyICkKewoJdmFyIHNjZW5lID0gc2VuZGVyLlBhcmVudFJvb3Q7Cgl2YXIgcmVuZGVyaW5nUGlwZWxpbmUgPSBzY2VuZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nUGlwZWxpbmVfQmFzaWM+KCJSZW5kZXJpbmcgUGlwZWxpbmUiKTsKCXZhciBlZmZlY3QgPSByZW5kZXJpbmdQaXBlbGluZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nRWZmZWN0X01pY3JvcGFydGljbGVzSW5BaXI+KGNoZWNrQ2hpbGRyZW46IHRydWUpOwoJaWYgKGVmZmVjdCAhPSBudWxsKQoJewoJCWlmIChjdXJyZW50TWljcm9wYXJ0aWNsZXNJbkFpcikKCQl7CgkJCWVmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuNywgMC42KTsKCQkJLy9lZmZlY3QuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAwLjgsIDAuNSk7CgkJCWVmZmVjdC5NdWx0aXBsaWVyID0gMC4wMDE1OwoJCX0KCQllbHNlCgkJewoJCQlpZiAoY3VycmVudE5pZ2h0KQoJCQl7CgkJCQllZmZlY3QuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgwLjc1LCAwLjc1LCAxKTsKCQkJCWVmZmVjdC5NdWx0aXBsaWVyID0gMC4wMDAxNTsKCQkJfQoJCQllbHNlCgkJCXsKCQkJCS8vc2ltdWxhdGUgaW5kaXJlY3QgbGlnaHRpbmcgYnkgbWVhbnMgbWljcm9wYXJ0aWNsZXMgaW4gYWlyCgkJCQllZmZlY3QuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAwLjgsIDAuNyk7CgkJCQkvL2VmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuOCwgMC41KTsKCQkJCWVmZmVjdC5NdWx0aXBsaWVyID0gMC4wMDAzOwoJCQl9CgoJCQkvL2VmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNzUsIDAuNzUsIDEpOwoJCQkvL2VmZmVjdC5NdWx0aXBsaWVyID0gMC4wMDAxNTsKCQl9Cgl9Cn0KCnZvaWQgRXhpdEZyb21WZWhpY2xlKE5lb0F4aXMuR2FtZU1vZGUgZ2FtZU1vZGUpCnsKCXZhciBvYmogPSBnYW1lTW9kZS5PYmplY3RDb250cm9sbGVkQnlQbGF5ZXIuVmFsdWUgYXMgVmVoaWNsZTsKCWlmIChvYmogIT0gbnVsbCkKCXsKCQl2YXIgaW5wdXRQcm9jZXNzaW5nID0gb2JqLkdldENvbXBvbmVudDxWZWhpY2xlSW5wdXRQcm9jZXNzaW5nPigpOwoJCWlmIChpbnB1dFByb2Nlc3NpbmcgIT0gbnVsbCkKCQkJaW5wdXRQcm9jZXNzaW5nLkV4aXRBbGxPYmplY3RzRnJvbVZlaGljbGUoZ2FtZU1vZGUpOwoJfQp9Cgp2b2lkIFByb2Nlc3NJbnB1dE1lc3NhZ2VFdmVudChOZW9BeGlzLkdhbWVNb2RlIHNlbmRlciwgTmVvQXhpcy5JbnB1dE1lc3NhZ2UgbWVzc2FnZSkKewoJdmFyIGtleURvd24gPSBtZXNzYWdlIGFzIElucHV0TWVzc2FnZUtleURvd247CglpZiAoa2V5RG93biAhPSBudWxsKS8vJiYgIXNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuQ29udHJvbCkpCgl7CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQxKQoJCXsKCQkJdmFyIG1hbmFnZXIgPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8QnVpbGRpbmdNYW5hZ2VyPigpOwoJCQlpZiAobWFuYWdlciAhPSBudWxsKQoJCQl7CgkJCQltYW5hZ2VyLkRpc3BsYXkgPSAhbWFuYWdlci5EaXNwbGF5OwoJCQkJbWFuYWdlci5Db2xsaXNpb24gPSBtYW5hZ2VyLkRpc3BsYXk7CgkJCX0KCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDIpCgkJewoJCQlFeGl0RnJvbVZlaGljbGUoc2VuZGVyKTsKCgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLlBhcmtlZFZlaGljbGVzID0gc3lzdGVtLlBhcmtlZFZlaGljbGVzLlZhbHVlICE9IDAgPyAwIDogNTAwMDsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDMpCgkJewoJCQlFeGl0RnJvbVZlaGljbGUoc2VuZGVyKTsKCgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLkZseWluZ1ZlaGljbGVzID0gc3lzdGVtLkZseWluZ1ZlaGljbGVzLlZhbHVlICE9IDAgPyAwIDogNTAwOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EOSkKCQl7CgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLlNpbXVsYXRlRHluYW1pY09iamVjdHMgPSAhc3lzdGVtLlNpbXVsYXRlRHluYW1pY09iamVjdHM7CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ0KQoJCXsKCQkJRXhpdEZyb21WZWhpY2xlKHNlbmRlcik7CgoJCQl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CgkJCWlmIChzeXN0ZW0gIT0gbnVsbCkKCQkJCXN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnMgPSBzeXN0ZW0uV2Fsa2luZ1BlZGVzdHJpYW5zLlZhbHVlICE9IDAgPyAwIDogMTAwOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EMCkKCQl7CgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLldhbGtpbmdQZWRlc3RyaWFuc01hbmFnZVRhc2tzID0gIXN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnNNYW5hZ2VUYXNrczsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDUpCgkJewoJCQl2YXIgc2NlbmUgPSAoU2NlbmUpc2VuZGVyLlBhcmVudFJvb3Q7CgkJCXZhciByZW5kZXJpbmdQaXBlbGluZSA9IHNjZW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdQaXBlbGluZT4oIlJlbmRlcmluZyBQaXBlbGluZSIpOwoJCQl2YXIgcmVmbGVjdGlvbiA9IHJlbmRlcmluZ1BpcGVsaW5lPy5HZXRDb21wb25lbnQ8UmVuZGVyaW5nRWZmZWN0X1JlZmxlY3Rpb24+KGNoZWNrQ2hpbGRyZW46IHRydWUpOwoJCQl2YXIgZm9nID0gc2NlbmUuR2V0Q29tcG9uZW50KCJGb2ciKSBhcyBGb2c7CgkJCXZhciBwcmVjaXBpdGF0aW9uID0gcmVuZGVyaW5nUGlwZWxpbmU_LkdldENvbXBvbmVudDxSZW5kZXJpbmdFZmZlY3RfUHJlY2lwaXRhdGlvbj4oY2hlY2tDaGlsZHJlbjogdHJ1ZSk7CgkJCXZhciBzb3VuZFNvdXJjZVJhaW4gPSBzY2VuZS5HZXRDb21wb25lbnQoIlNvdW5kIFNvdXJjZSBSYWluIikgYXMgU291bmRTb3VyY2U7CgkJCXZhciBjYW1lcmEgPSBzY2VuZS5HZXRDb21wb25lbnQ8Q2FtZXJhPigiQ2FtZXJhIERlZmF1bHQiKTsKCQkJdmFyIGRpcmVjdGlvbmFsTGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRpcmVjdGlvbmFsIExpZ2h0IikgYXMgTGlnaHQ7CgoJCQljdXJyZW50V2VhdGhlcisrOwoJCQlpZiAoY3VycmVudFdlYXRoZXIgPiAyKQoJCQkJY3VycmVudFdlYXRoZXIgPSAwOwoJCQkvL2N1cnJlbnRSYWluID0gIWN1cnJlbnRSYWluOwoKCQkJdHJ5CgkJCXsKCQkJCVVwZGF0ZUZvZ0FuZEZhckNsaXBQbGFuZShmb2csIGNhbWVyYSk7CgoJCQkJc291bmRTb3VyY2VSYWluLkVuYWJsZWQgPSBjdXJyZW50V2VhdGhlciA9PSBSYWluRmFsbGluZzsKCgkJCQlzY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGluZyA9IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nID8gMSA6IDA7CgkJCQlzY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGVuID0gKGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nIHx8IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsZW4pID8gMSA6IDA7CgoJCQkJLy9wcmVjaXBpdGF0aW9uLkVuYWJsZWQgPSBjdXJyZW50UmFpbjsKCQkJCS8vc291bmRTb3VyY2VSYWluLkVuYWJsZWQgPSBjdXJyZW50UmFpbjsKCQkJCS8vc2NlbmUuUHJlY2lwaXRhdGlvbkZhbGxpbmcgPSBjdXJyZW50UmFpbiA_IDEgOiAwOwoJCQkJLy9zY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGVuID0gY3VycmVudFJhaW4gPyAxIDogMDsKCgkJCQkvKgoJCQkJCQkJCWlmKGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nKS8vaWYgKGN1cnJlbnRSYWluKQoJCQkJCQkJCXsKCQkJCQkJCQkJZGlyZWN0aW9uYWxMaWdodC5NYXNrID0gbmV3IFJlZmVyZW5jZU5vVmFsdWUoQCJTYW1wbGVzXENpdHkgRGVtb1xTa2llc1xSYWluIGNsb3VkcyBtYXNrXFJhaW4gY2xvdWRzIG1hc2suanBnIik7CgkJCQkJCQkJCWRpcmVjdGlvbmFsTGlnaHQuTWFza1RyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oVmVjdG9yMy5aZXJvLCBRdWF0ZXJuaW9uLklkZW50aXR5LCBuZXcgVmVjdG9yMygwLjAwNSwgMC4wMDUsIDAuMDA1KSk7CgkJCQkJCQkJfQoJCQkJCQkJCWVsc2UKCQkJCQkJCQl7CgkJCQkJCQkJCWRpcmVjdGlvbmFsTGlnaHQuTWFzayA9IG51bGw7CgkJCQkJCQkJfQoJCQkJKi8KCQkJfQoJCQljYXRjaCAoRXhjZXB0aW9uIGUpCgkJCXsKCQkJCUxvZy5XYXJuaW5nKGUuTWVzc2FnZSk7CgkJCX0KCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ2KQoJCXsKCQkJdmFyIHNjZW5lID0gKFNjZW5lKXNlbmRlci5QYXJlbnRSb290OwoJCQl2YXIgYW1iaWVudExpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJBbWJpZW50IExpZ2h0IikgYXMgTGlnaHQ7CgkJCXZhciBkaXJlY3Rpb25hbExpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJEaXJlY3Rpb25hbCBMaWdodCIpIGFzIExpZ2h0OwoJCQl2YXIgc3RyZWV0TGlnaHRMaWdodHMgPSBzY2VuZS5HZXRDb21wb25lbnQoIlN0cmVldCBsaWdodCBsaWdodHMiKTsKCQkJdmFyIHNreSA9IHNjZW5lLkdldENvbXBvbmVudCgiU2t5IikgYXMgU2t5OwoJCQkvL3ZhciBkYXlTa3kgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRheSBza3kiKTsKCQkJLy92YXIgbmlnaHRTa3kgPSBzY2VuZS5HZXRDb21wb25lbnQoIk5pZ2h0IHNreSIpOwoJCQl2YXIgZm9nID0gc2NlbmUuR2V0Q29tcG9uZW50KCJGb2ciKSBhcyBGb2c7CgkJCXZhciBjYW1lcmEgPSBzY2VuZS5HZXRDb21wb25lbnQ8Q2FtZXJhPigiQ2FtZXJhIERlZmF1bHQiKTsKCgkJCWN1cnJlbnROaWdodCA9ICFjdXJyZW50TmlnaHQ7CgoJCQl0cnkKCQkJewoJCQkJc2NlbmUuVGltZU9mRGF5ID0gY3VycmVudE5pZ2h0ID8gMCA6IDEyOyAKCQkJCWFtYmllbnRMaWdodC5CcmlnaHRuZXNzID0gY3VycmVudE5pZ2h0ID8gMjUwMDAgOiAxMDAwMDA7CgkJCQlkaXJlY3Rpb25hbExpZ2h0LkVuYWJsZWQgPSAhY3VycmVudE5pZ2h0OwoJCQkJc3RyZWV0TGlnaHRMaWdodHMuRW5hYmxlZCA9IGN1cnJlbnROaWdodDsKCQkJCS8vc2t5LlByb2NlZHVyYWxJbnRlbnNpdHkgPSBjdXJyZW50TmlnaHQgPyAwIDogMTsKCQkJCS8vZGF5U2t5LkVuYWJsZWQgPSAhY3VycmVudE5pZ2h0OwoJCQkJLy9uaWdodFNreS5FbmFibGVkID0gY3VycmVudE5pZ2h0OwoJCQkJVXBkYXRlRm9nQW5kRmFyQ2xpcFBsYW5lKGZvZywgY2FtZXJhKTsKCQkJCVVwZGF0ZU1pY3JvcGFydGljbGVzSW5BaXIoc2VuZGVyKTsKCQkJfQoJCQljYXRjaCAoRXhjZXB0aW9uIGUpCgkJCXsKCQkJCUxvZy5XYXJuaW5nKGUuTWVzc2FnZSk7CgkJCX0KCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ4KQoJCXsKCQkJRXhpdEZyb21WZWhpY2xlKHNlbmRlcik7CgoJCQl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CgkJCWlmIChzeXN0ZW0gIT0gbnVsbCkKCQkJewoJCQkJaWYgKHN5c3RlbS5QYXJrZWRWZWhpY2xlc09iamVjdE1vZGUuVmFsdWUgPT0gVHJhZmZpY1N5c3RlbS5PYmplY3RNb2RlRW51bS5WZWhpY2xlQ29tcG9uZW50KQoJCQkJCXN5c3RlbS5QYXJrZWRWZWhpY2xlc09iamVjdE1vZGUgPSBUcmFmZmljU3lzdGVtLk9iamVjdE1vZGVFbnVtLlN0YXRpY09iamVjdDsKCQkJCWVsc2UKCQkJCQlzeXN0ZW0uUGFya2VkVmVoaWNsZXNPYmplY3RNb2RlID0gVHJhZmZpY1N5c3RlbS5PYmplY3RNb2RlRW51bS5WZWhpY2xlQ29tcG9uZW50OwoJCQl9CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ3KQoJCXsKCQkJY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID0gIWN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZzsKCgkJCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRSb290OwoJCQl2YXIgcmVuZGVyaW5nUGlwZWxpbmUgPSBzY2VuZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nUGlwZWxpbmVfQmFzaWM+KCJSZW5kZXJpbmcgUGlwZWxpbmUiKTsKCQkJdmFyIGNhbWVyYSA9IHNjZW5lLkdldENvbXBvbmVudDxDYW1lcmE+KCJDYW1lcmEgRGVmYXVsdCIpOwoJCQl2YXIgZm9nID0gc2NlbmUuR2V0Q29tcG9uZW50KCJGb2ciKSBhcyBGb2c7CgoJCQkvL2NhbWVyYS5GYXJDbGlwUGxhbmUgPSBjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmcgPyAyMDAwIDogMTAwMDsKCQkJcmVuZGVyaW5nUGlwZWxpbmUuTWluaW11bVZpc2libGVTaXplT2ZPYmplY3RzID0gY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID8gMiA6IDQ7CgoJCQlyZW5kZXJpbmdQaXBlbGluZS5TaGFkb3dEaXJlY3Rpb25hbERpc3RhbmNlID0gY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID8gNjAwIDogMjAwOwoJCQlyZW5kZXJpbmdQaXBlbGluZS5TaGFkb3dEaXJlY3Rpb25hbExpZ2h0Q2FzY2FkZXMgPSBjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmcgPyAzIDogMjsKCgkJCXRyeQoJCQl7CgkJCQlVcGRhdGVGb2dBbmRGYXJDbGlwUGxhbmUoZm9nLCBjYW1lcmEpOwoJCQl9CgkJCWNhdGNoIChFeGNlcHRpb24gZSkKCQkJewoJCQkJTG9nLldhcm5pbmcoZS5NZXNzYWdlKTsKCQkJfQoKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoKCQkJLyoKCQkJdmFyIHNjZW5lID0gc2VuZGVyLlBhcmVudFJvb3QgYXMgU2NlbmU7CgkJCWlmIChzY2VuZSAhPSBudWxsKQoJCQl7CgkJCQlpZiAoc2NlbmUuT2N0cmVlVGhyZWFkaW5nTW9kZS5WYWx1ZSA9PSBPY3RyZWVDb250YWluZXIuVGhyZWFkaW5nTW9kZUVudW0uQmFja2dyb3VuZFRocmVhZCkKCQkJCQlzY2VuZS5PY3RyZWVUaHJlYWRpbmdNb2RlID0gT2N0cmVlQ29udGFpbmVyLlRocmVhZGluZ01vZGVFbnVtLlNpbmdsZVRocmVhZGVkOwoJCQkJZWxzZQoJCQkJCXNjZW5lLk9jdHJlZVRocmVhZGluZ01vZGUgPSBPY3RyZWVDb250YWluZXIuVGhyZWFkaW5nTW9kZUVudW0uQmFja2dyb3VuZFRocmVhZDsKCQkJfQoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJCSovCgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5DKQoJCXsKCQkJY3VycmVudFJhbmRvbWl6ZVN0cmVldExpZ2h0Q29sb3JzID0gIWN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9yczsKCQkJCgkJCXZhciBsaWdodHMgPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQoIlN0cmVldCBsaWdodCBsaWdodHMiKTsKCQkJaWYobGlnaHRzICE9IG51bGwpCgkJCXsKCQkJCXZhciByYW5kb20gPSBuZXcgRmFzdFJhbmRvbSgpOwoJCQkJCgkJCQlmb3JlYWNoKHZhciBsaWdodCBpbiBsaWdodHMuR2V0Q29tcG9uZW50czxMaWdodD4oKSkKCQkJCXsKCQkJCQlpZihjdXJyZW50UmFuZG9taXplU3RyZWV0TGlnaHRDb2xvcnMpCgkJCQkJewoJCQkJCQl2YXIgY29sb3IgPSBsaWdodC5Db2xvci5WYWx1ZTsKCQkJCQkJdmFyIG1heCA9IDAuNmY7Ly8wLjJmOwoJCQkJCQljb2xvci5SZWQgKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJCQkJY29sb3IuR3JlZW4gKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJCQkJY29sb3IuQmx1ZSArPSByYW5kb20uTmV4dCgtbWF4LCBtYXgpOwoJCQkJCQlsaWdodC5Db2xvciA9IGNvbG9yOwoJCQkJCX0KCQkJCQllbHNlCgkJCQkJewoJCQkJCQlsaWdodC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDEsIDAuNzExMDE5Nik7CgkJCQkJfQoJCQkJfQoJCQl9CQoKCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLk0pCgkJewoJCQljdXJyZW50TWljcm9wYXJ0aWNsZXNJbkFpciA9ICFjdXJyZW50TWljcm9wYXJ0aWNsZXNJbkFpcjsKCQkJVXBkYXRlTWljcm9wYXJ0aWNsZXNJbkFpcihzZW5kZXIpOwoJCQkKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuUCkKCQl7CgkJCWN1cnJlbnRSZWZsZWN0aW9uUHJvYmUgPSAhY3VycmVudFJlZmxlY3Rpb25Qcm9iZTsKCgkJCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRSb290OwoJCQlmb3JlYWNoICh2YXIgcHJvYmUgaW4gc2NlbmUuR2V0Q29tcG9uZW50czxSZWZsZWN0aW9uUHJvYmU+KCkpCgkJCQlwcm9iZS5SZWFsVGltZSA9IGN1cnJlbnRSZWZsZWN0aW9uUHJvYmU7CgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJfQp9CgpwdWJsaWMgdm9pZCBHYW1lTW9kZV9JbnB1dE1lc3NhZ2VFdmVudChOZW9BeGlzLkdhbWVNb2RlIHNlbmRlciwgTmVvQXhpcy5JbnB1dE1lc3NhZ2UgbWVzc2FnZSkKewoJaWYgKCFzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkNvbnRyb2wpKQoJCVByb2Nlc3NJbnB1dE1lc3NhZ2VFdmVudChzZW5kZXIsIG1lc3NhZ2UpOwp9CgpwdWJsaWMgdm9pZCBHYW1lTW9kZV9FbmFibGVkSW5TaW11bGF0aW9uKE5lb0F4aXMuQ29tcG9uZW50IG9iaikKewoJLy9hY3RpdmF0ZSBuaWdodCBtb2RlCglQcm9jZXNzSW5wdXRNZXNzYWdlRXZlbnQoKEdhbWVNb2RlKW9iaiwgbmV3IElucHV0TWVzc2FnZUtleURvd24oRUtleXMuRDYpKTsKCQoJLy9yYW5kb21pemUgc3RyZWV0IGxpZ2h0cwoJdmFyIGxpZ2h0cyA9IG9iai5QYXJlbnRSb290LkdldENvbXBvbmVudCgiU3RyZWV0IGxpZ2h0IGxpZ2h0cyIpOwoJaWYobGlnaHRzICE9IG51bGwpCgl7CgkJdmFyIHJhbmRvbSA9IG5ldyBGYXN0UmFuZG9tKCk7CgkJCgkJZm9yZWFjaCh2YXIgbGlnaHQgaW4gbGlnaHRzLkdldENvbXBvbmVudHM8TGlnaHQ+KCkpCgkJewoJCQkvL3JhbmRvbWl6ZSByb3RhdGlvbgoJCQl2YXIgdHIgPSBsaWdodC5UcmFuc2Zvcm1WOwoJCQl0ciA9IHRyLlVwZGF0ZVJvdGF0aW9uKFF1YXRlcm5pb24uRnJvbVJvdGF0ZUJ5WihyYW5kb20uTmV4dChNYXRoLlBJICogMikpKTsKCQkJbGlnaHQuVHJhbnNmb3JtID0gdHI7CgovKgoJCQkvL3JhbmRvbWl6ZSBjb2xvcnMKCQkJdmFyIGNvbG9yID0gbGlnaHQuQ29sb3IuVmFsdWU7CgkJCXZhciBtYXggPSAwLjZmOy8vMC4yZjsKCQkJY29sb3IuUmVkICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCWNvbG9yLkdyZWVuICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCWNvbG9yLkJsdWUgKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJbGlnaHQuQ29sb3IgPSBjb2xvcjsKKi8JCQkKCQl9Cgl9CQp9Cg==")]
public class DynamicClass0181888AC25A53A4664437ED03D126A80564EB0C1D97EA5AF4BCE830779CBCA3
{
    public NeoAxis.CSharpScript Owner;
    static bool currentFarDistanceRendering;
    static bool currentNight;
    static int currentWeather;
    static bool currentRandomizeStreetLightColors;
    static bool currentMicroparticlesInAir;
    static bool currentReflectionProbe = true;
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
                    //sky.ProceduralIntensity = currentNight ? 0 : 1;
                    //daySky.Enabled = !currentNight;
                    //nightSky.Enabled = currentNight;
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

            if (keyDown.Key == EKeys.P)
            {
                currentReflectionProbe = !currentReflectionProbe;
                var scene = sender.ParentRoot;
                foreach (var probe in scene.GetComponents<ReflectionProbe>())
                    probe.RealTime = currentReflectionProbe;
                message.Handled = true;
                return;
            }
        }
    }

    public void GameMode_InputMessageEvent(NeoAxis.GameMode sender, NeoAxis.InputMessage message)
    {
        if (!sender.IsKeyPressed(EKeys.Control))
            ProcessInputMessageEvent(sender, message);
    }

    public void GameMode_EnabledInSimulation(NeoAxis.Component obj)
    {
        //activate night mode
        ProcessInputMessageEvent((GameMode)obj, new InputMessageKeyDown(EKeys.D6));
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

[CSharpScriptGeneratedAttribute("c3RhdGljIGJvb2wgY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nOwpzdGF0aWMgYm9vbCBjdXJyZW50TmlnaHQ7CnN0YXRpYyBpbnQgY3VycmVudFdlYXRoZXI7CnN0YXRpYyBib29sIGN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9yczsKc3RhdGljIGJvb2wgY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXI7CnN0YXRpYyBib29sIGN1cnJlbnRSZWZsZWN0aW9uUHJvYmUgPSB0cnVlOwoKY29uc3QgaW50IFN1bm55ID0gMDsKY29uc3QgaW50IFJhaW5GYWxsaW5nID0gMTsKY29uc3QgaW50IFJhaW5GYWxsZW4gPSAyOwoKdm9pZCBVcGRhdGVGb2dBbmRGYXJDbGlwUGxhbmUoRm9nIGZvZywgQ2FtZXJhIGNhbWVyYSkKewoJZm9nLkVuYWJsZWQgPSAhY3VycmVudE5pZ2h0Oy8vIHx8IGN1cnJlbnRSYWluOwoJZm9nLkRlbnNpdHkgPSBjdXJyZW50V2VhdGhlciA9PSBSYWluRmFsbGluZyA_IDAuMDEgOiAwLjAwMTsvL2ZvZy5EZW5zaXR5ID0gY3VycmVudFJhaW4gPyAwLjAxIDogMC4wMDE7CgoJaWYgKGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nKS8vaWYgKGN1cnJlbnRSYWluKQoJCWZvZy5BZmZlY3RCYWNrZ3JvdW5kID0gMTsKCWVsc2UKCQlmb2cuQWZmZWN0QmFja2dyb3VuZCA9IGN1cnJlbnROaWdodCA_IDAgOiAwLjU7CgoJaWYgKGN1cnJlbnROaWdodCkKCQlmb2cuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgwLCAwLCAwKTsKCWVsc2UKCQlmb2cuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgwLjQ1Mzk2MDgsIDAuNTE2MDM5MiwgMC42NTg4MjM1KTsKCglpZiAoZm9nLkVuYWJsZWQgJiYgZm9nLkFmZmVjdEJhY2tncm91bmQgPT0gMSkKCQljYW1lcmEuRmFyQ2xpcFBsYW5lID0gMzAwOwoJZWxzZQoJCWNhbWVyYS5GYXJDbGlwUGxhbmUgPSBjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmcgPyAyMDAwIDogMTAwMDsKfQoKdm9pZCBVcGRhdGVNaWNyb3BhcnRpY2xlc0luQWlyKCBDb21wb25lbnQgc2VuZGVyICkKewoJdmFyIHNjZW5lID0gc2VuZGVyLlBhcmVudFJvb3Q7Cgl2YXIgcmVuZGVyaW5nUGlwZWxpbmUgPSBzY2VuZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nUGlwZWxpbmVfQmFzaWM+KCJSZW5kZXJpbmcgUGlwZWxpbmUiKTsKCXZhciBlZmZlY3QgPSByZW5kZXJpbmdQaXBlbGluZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nRWZmZWN0X01pY3JvcGFydGljbGVzSW5BaXI+KGNoZWNrQ2hpbGRyZW46IHRydWUpOwoJaWYgKGVmZmVjdCAhPSBudWxsKQoJewoJCWlmIChjdXJyZW50TWljcm9wYXJ0aWNsZXNJbkFpcikKCQl7CgkJCWVmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuNywgMC42KTsKCQkJLy9lZmZlY3QuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAwLjgsIDAuNSk7CgkJCWVmZmVjdC5NdWx0aXBsaWVyID0gMC4wMDE1OwoJCX0KCQllbHNlCgkJewoJCQlpZiAoY3VycmVudE5pZ2h0KQoJCQl7CgkJCQllZmZlY3QuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgwLjc1LCAwLjc1LCAxKTsKCQkJCWVmZmVjdC5NdWx0aXBsaWVyID0gMC4wMDAxNTsKCQkJfQoJCQllbHNlCgkJCXsKCQkJCS8vc2ltdWxhdGUgaW5kaXJlY3QgbGlnaHRpbmcgYnkgbWVhbnMgbWljcm9wYXJ0aWNsZXMgaW4gYWlyCgkJCQllZmZlY3QuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAwLjgsIDAuNyk7CgkJCQkvL2VmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuOCwgMC41KTsKCQkJCWVmZmVjdC5NdWx0aXBsaWVyID0gMC4wMDAzOwoJCQl9CgoJCQkvL2VmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNzUsIDAuNzUsIDEpOwoJCQkvL2VmZmVjdC5NdWx0aXBsaWVyID0gMC4wMDAxNTsKCQl9Cgl9Cn0KCnZvaWQgRXhpdEZyb21WZWhpY2xlKE5lb0F4aXMuR2FtZU1vZGUgZ2FtZU1vZGUpCnsKCXZhciBvYmogPSBnYW1lTW9kZS5PYmplY3RDb250cm9sbGVkQnlQbGF5ZXIuVmFsdWUgYXMgVmVoaWNsZTsKCWlmIChvYmogIT0gbnVsbCkKCXsKCQl2YXIgaW5wdXRQcm9jZXNzaW5nID0gb2JqLkdldENvbXBvbmVudDxWZWhpY2xlSW5wdXRQcm9jZXNzaW5nPigpOwoJCWlmIChpbnB1dFByb2Nlc3NpbmcgIT0gbnVsbCkKCQkJaW5wdXRQcm9jZXNzaW5nLkV4aXRBbGxPYmplY3RzRnJvbVZlaGljbGUoZ2FtZU1vZGUpOwoJfQp9Cgp2b2lkIFByb2Nlc3NJbnB1dE1lc3NhZ2VFdmVudChOZW9BeGlzLkdhbWVNb2RlIHNlbmRlciwgTmVvQXhpcy5JbnB1dE1lc3NhZ2UgbWVzc2FnZSkKewoJdmFyIGtleURvd24gPSBtZXNzYWdlIGFzIElucHV0TWVzc2FnZUtleURvd247CglpZiAoa2V5RG93biAhPSBudWxsKS8vJiYgIXNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuQ29udHJvbCkpCgl7CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQxKQoJCXsKCQkJdmFyIG1hbmFnZXIgPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8QnVpbGRpbmdNYW5hZ2VyPigpOwoJCQlpZiAobWFuYWdlciAhPSBudWxsKQoJCQl7CgkJCQltYW5hZ2VyLkRpc3BsYXkgPSAhbWFuYWdlci5EaXNwbGF5OwoJCQkJbWFuYWdlci5Db2xsaXNpb24gPSBtYW5hZ2VyLkRpc3BsYXk7CgkJCX0KCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDIpCgkJewoJCQlFeGl0RnJvbVZlaGljbGUoc2VuZGVyKTsKCgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLlBhcmtlZFZlaGljbGVzID0gc3lzdGVtLlBhcmtlZFZlaGljbGVzLlZhbHVlICE9IDAgPyAwIDogNTAwMDsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDMpCgkJewoJCQlFeGl0RnJvbVZlaGljbGUoc2VuZGVyKTsKCgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLkZseWluZ1ZlaGljbGVzID0gc3lzdGVtLkZseWluZ1ZlaGljbGVzLlZhbHVlICE9IDAgPyAwIDogNTAwOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EOSkKCQl7CgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLlNpbXVsYXRlRHluYW1pY09iamVjdHMgPSAhc3lzdGVtLlNpbXVsYXRlRHluYW1pY09iamVjdHM7CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ0KQoJCXsKCQkJRXhpdEZyb21WZWhpY2xlKHNlbmRlcik7CgoJCQl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CgkJCWlmIChzeXN0ZW0gIT0gbnVsbCkKCQkJCXN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnMgPSBzeXN0ZW0uV2Fsa2luZ1BlZGVzdHJpYW5zLlZhbHVlICE9IDAgPyAwIDogMTAwOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EMCkKCQl7CgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLldhbGtpbmdQZWRlc3RyaWFuc01hbmFnZVRhc2tzID0gIXN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnNNYW5hZ2VUYXNrczsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDUpCgkJewoJCQl2YXIgc2NlbmUgPSAoU2NlbmUpc2VuZGVyLlBhcmVudFJvb3Q7CgkJCXZhciByZW5kZXJpbmdQaXBlbGluZSA9IHNjZW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdQaXBlbGluZT4oIlJlbmRlcmluZyBQaXBlbGluZSIpOwoJCQl2YXIgcmVmbGVjdGlvbiA9IHJlbmRlcmluZ1BpcGVsaW5lPy5HZXRDb21wb25lbnQ8UmVuZGVyaW5nRWZmZWN0X1JlZmxlY3Rpb24+KGNoZWNrQ2hpbGRyZW46IHRydWUpOwoJCQl2YXIgZm9nID0gc2NlbmUuR2V0Q29tcG9uZW50KCJGb2ciKSBhcyBGb2c7CgkJCXZhciBwcmVjaXBpdGF0aW9uID0gcmVuZGVyaW5nUGlwZWxpbmU_LkdldENvbXBvbmVudDxSZW5kZXJpbmdFZmZlY3RfUHJlY2lwaXRhdGlvbj4oY2hlY2tDaGlsZHJlbjogdHJ1ZSk7CgkJCXZhciBzb3VuZFNvdXJjZVJhaW4gPSBzY2VuZS5HZXRDb21wb25lbnQoIlNvdW5kIFNvdXJjZSBSYWluIikgYXMgU291bmRTb3VyY2U7CgkJCXZhciBjYW1lcmEgPSBzY2VuZS5HZXRDb21wb25lbnQ8Q2FtZXJhPigiQ2FtZXJhIERlZmF1bHQiKTsKCQkJdmFyIGRpcmVjdGlvbmFsTGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRpcmVjdGlvbmFsIExpZ2h0IikgYXMgTGlnaHQ7CgoJCQljdXJyZW50V2VhdGhlcisrOwoJCQlpZiAoY3VycmVudFdlYXRoZXIgPiAyKQoJCQkJY3VycmVudFdlYXRoZXIgPSAwOwoJCQkvL2N1cnJlbnRSYWluID0gIWN1cnJlbnRSYWluOwoKCQkJdHJ5CgkJCXsKCQkJCVVwZGF0ZUZvZ0FuZEZhckNsaXBQbGFuZShmb2csIGNhbWVyYSk7CgoJCQkJc291bmRTb3VyY2VSYWluLkVuYWJsZWQgPSBjdXJyZW50V2VhdGhlciA9PSBSYWluRmFsbGluZzsKCgkJCQlzY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGluZyA9IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nID8gMSA6IDA7CgkJCQlzY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGVuID0gKGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nIHx8IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsZW4pID8gMSA6IDA7CgoJCQkJLy9wcmVjaXBpdGF0aW9uLkVuYWJsZWQgPSBjdXJyZW50UmFpbjsKCQkJCS8vc291bmRTb3VyY2VSYWluLkVuYWJsZWQgPSBjdXJyZW50UmFpbjsKCQkJCS8vc2NlbmUuUHJlY2lwaXRhdGlvbkZhbGxpbmcgPSBjdXJyZW50UmFpbiA_IDEgOiAwOwoJCQkJLy9zY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGVuID0gY3VycmVudFJhaW4gPyAxIDogMDsKCgkJCQkvKgoJCQkJCQkJCWlmKGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nKS8vaWYgKGN1cnJlbnRSYWluKQoJCQkJCQkJCXsKCQkJCQkJCQkJZGlyZWN0aW9uYWxMaWdodC5NYXNrID0gbmV3IFJlZmVyZW5jZU5vVmFsdWUoQCJTYW1wbGVzXENpdHkgRGVtb1xTa2llc1xSYWluIGNsb3VkcyBtYXNrXFJhaW4gY2xvdWRzIG1hc2suanBnIik7CgkJCQkJCQkJCWRpcmVjdGlvbmFsTGlnaHQuTWFza1RyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oVmVjdG9yMy5aZXJvLCBRdWF0ZXJuaW9uLklkZW50aXR5LCBuZXcgVmVjdG9yMygwLjAwNSwgMC4wMDUsIDAuMDA1KSk7CgkJCQkJCQkJfQoJCQkJCQkJCWVsc2UKCQkJCQkJCQl7CgkJCQkJCQkJCWRpcmVjdGlvbmFsTGlnaHQuTWFzayA9IG51bGw7CgkJCQkJCQkJfQoJCQkJKi8KCQkJfQoJCQljYXRjaCAoRXhjZXB0aW9uIGUpCgkJCXsKCQkJCUxvZy5XYXJuaW5nKGUuTWVzc2FnZSk7CgkJCX0KCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ2KQoJCXsKCQkJdmFyIHNjZW5lID0gKFNjZW5lKXNlbmRlci5QYXJlbnRSb290OwoJCQl2YXIgYW1iaWVudExpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJBbWJpZW50IExpZ2h0IikgYXMgTGlnaHQ7CgkJCXZhciBkaXJlY3Rpb25hbExpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJEaXJlY3Rpb25hbCBMaWdodCIpIGFzIExpZ2h0OwoJCQl2YXIgc3RyZWV0TGlnaHRMaWdodHMgPSBzY2VuZS5HZXRDb21wb25lbnQoIlN0cmVldCBsaWdodCBsaWdodHMiKTsKCQkJdmFyIHNreSA9IHNjZW5lLkdldENvbXBvbmVudCgiU2t5IikgYXMgU2t5OwoJCQkvL3ZhciBkYXlTa3kgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRheSBza3kiKTsKCQkJLy92YXIgbmlnaHRTa3kgPSBzY2VuZS5HZXRDb21wb25lbnQoIk5pZ2h0IHNreSIpOwoJCQl2YXIgZm9nID0gc2NlbmUuR2V0Q29tcG9uZW50KCJGb2ciKSBhcyBGb2c7CgkJCXZhciBjYW1lcmEgPSBzY2VuZS5HZXRDb21wb25lbnQ8Q2FtZXJhPigiQ2FtZXJhIERlZmF1bHQiKTsKCgkJCWN1cnJlbnROaWdodCA9ICFjdXJyZW50TmlnaHQ7CgoJCQl0cnkKCQkJewoJCQkJc2NlbmUuVGltZU9mRGF5ID0gY3VycmVudE5pZ2h0ID8gMCA6IDEyOyAKCQkJCWFtYmllbnRMaWdodC5CcmlnaHRuZXNzID0gY3VycmVudE5pZ2h0ID8gMjUwMDAgOiAxMDAwMDA7CgkJCQlkaXJlY3Rpb25hbExpZ2h0LkVuYWJsZWQgPSAhY3VycmVudE5pZ2h0OwoJCQkJc3RyZWV0TGlnaHRMaWdodHMuRW5hYmxlZCA9IGN1cnJlbnROaWdodDsKCQkJCXNreS5Nb2RlID0gY3VycmVudE5pZ2h0ID8gU2t5Lk1vZGVFbnVtLlJlc291cmNlIDogU2t5Lk1vZGVFbnVtLlByb2NlZHVyYWw7CgkJCQkvL3NreS5Qcm9jZWR1cmFsSW50ZW5zaXR5ID0gY3VycmVudE5pZ2h0ID8gMCA6IDE7CgkJCQkvL2RheVNreS5FbmFibGVkID0gIWN1cnJlbnROaWdodDsKCQkJCS8vbmlnaHRTa3kuRW5hYmxlZCA9IGN1cnJlbnROaWdodDsKCQkJCVVwZGF0ZUZvZ0FuZEZhckNsaXBQbGFuZShmb2csIGNhbWVyYSk7CgkJCQlVcGRhdGVNaWNyb3BhcnRpY2xlc0luQWlyKHNlbmRlcik7CgkJCX0KCQkJY2F0Y2ggKEV4Y2VwdGlvbiBlKQoJCQl7CgkJCQlMb2cuV2FybmluZyhlLk1lc3NhZ2UpOwoJCQl9CgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EOCkKCQl7CgkJCUV4aXRGcm9tVmVoaWNsZShzZW5kZXIpOwoKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCXsKCQkJCWlmIChzeXN0ZW0uUGFya2VkVmVoaWNsZXNPYmplY3RNb2RlLlZhbHVlID09IFRyYWZmaWNTeXN0ZW0uT2JqZWN0TW9kZUVudW0uVmVoaWNsZUNvbXBvbmVudCkKCQkJCQlzeXN0ZW0uUGFya2VkVmVoaWNsZXNPYmplY3RNb2RlID0gVHJhZmZpY1N5c3RlbS5PYmplY3RNb2RlRW51bS5TdGF0aWNPYmplY3Q7CgkJCQllbHNlCgkJCQkJc3lzdGVtLlBhcmtlZFZlaGljbGVzT2JqZWN0TW9kZSA9IFRyYWZmaWNTeXN0ZW0uT2JqZWN0TW9kZUVudW0uVmVoaWNsZUNvbXBvbmVudDsKCQkJfQoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5ENykKCQl7CgkJCWN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA9ICFjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmc7CgoJCQl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50Um9vdDsKCQkJdmFyIHJlbmRlcmluZ1BpcGVsaW5lID0gc2NlbmUuR2V0Q29tcG9uZW50PFJlbmRlcmluZ1BpcGVsaW5lX0Jhc2ljPigiUmVuZGVyaW5nIFBpcGVsaW5lIik7CgkJCXZhciBjYW1lcmEgPSBzY2VuZS5HZXRDb21wb25lbnQ8Q2FtZXJhPigiQ2FtZXJhIERlZmF1bHQiKTsKCQkJdmFyIGZvZyA9IHNjZW5lLkdldENvbXBvbmVudCgiRm9nIikgYXMgRm9nOwoKCQkJLy9jYW1lcmEuRmFyQ2xpcFBsYW5lID0gY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID8gMjAwMCA6IDEwMDA7CgkJCXJlbmRlcmluZ1BpcGVsaW5lLk1pbmltdW1WaXNpYmxlU2l6ZU9mT2JqZWN0cyA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDIgOiA0OwoKCQkJcmVuZGVyaW5nUGlwZWxpbmUuU2hhZG93RGlyZWN0aW9uYWxEaXN0YW5jZSA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDYwMCA6IDIwMDsKCQkJcmVuZGVyaW5nUGlwZWxpbmUuU2hhZG93RGlyZWN0aW9uYWxMaWdodENhc2NhZGVzID0gY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID8gMyA6IDI7CgoJCQl0cnkKCQkJewoJCQkJVXBkYXRlRm9nQW5kRmFyQ2xpcFBsYW5lKGZvZywgY2FtZXJhKTsKCQkJfQoJCQljYXRjaCAoRXhjZXB0aW9uIGUpCgkJCXsKCQkJCUxvZy5XYXJuaW5nKGUuTWVzc2FnZSk7CgkJCX0KCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCgkJCS8qCgkJCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRSb290IGFzIFNjZW5lOwoJCQlpZiAoc2NlbmUgIT0gbnVsbCkKCQkJewoJCQkJaWYgKHNjZW5lLk9jdHJlZVRocmVhZGluZ01vZGUuVmFsdWUgPT0gT2N0cmVlQ29udGFpbmVyLlRocmVhZGluZ01vZGVFbnVtLkJhY2tncm91bmRUaHJlYWQpCgkJCQkJc2NlbmUuT2N0cmVlVGhyZWFkaW5nTW9kZSA9IE9jdHJlZUNvbnRhaW5lci5UaHJlYWRpbmdNb2RlRW51bS5TaW5nbGVUaHJlYWRlZDsKCQkJCWVsc2UKCQkJCQlzY2VuZS5PY3RyZWVUaHJlYWRpbmdNb2RlID0gT2N0cmVlQ29udGFpbmVyLlRocmVhZGluZ01vZGVFbnVtLkJhY2tncm91bmRUaHJlYWQ7CgkJCX0KCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCQkqLwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuQykKCQl7CgkJCWN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9ycyA9ICFjdXJyZW50UmFuZG9taXplU3RyZWV0TGlnaHRDb2xvcnM7CgkJCQoJCQl2YXIgbGlnaHRzID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50KCJTdHJlZXQgbGlnaHQgbGlnaHRzIik7CgkJCWlmKGxpZ2h0cyAhPSBudWxsKQoJCQl7CgkJCQl2YXIgcmFuZG9tID0gbmV3IEZhc3RSYW5kb20oKTsKCQkJCQoJCQkJZm9yZWFjaCh2YXIgbGlnaHQgaW4gbGlnaHRzLkdldENvbXBvbmVudHM8TGlnaHQ+KCkpCgkJCQl7CgkJCQkJaWYoY3VycmVudFJhbmRvbWl6ZVN0cmVldExpZ2h0Q29sb3JzKQoJCQkJCXsKCQkJCQkJdmFyIGNvbG9yID0gbGlnaHQuQ29sb3IuVmFsdWU7CgkJCQkJCXZhciBtYXggPSAwLjZmOy8vMC4yZjsKCQkJCQkJY29sb3IuUmVkICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCQkJCWNvbG9yLkdyZWVuICs9IHJhbmRvbS5OZXh0KC1tYXgsIG1heCk7CgkJCQkJCWNvbG9yLkJsdWUgKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJCQkJbGlnaHQuQ29sb3IgPSBjb2xvcjsKCQkJCQl9CgkJCQkJZWxzZQoJCQkJCXsKCQkJCQkJbGlnaHQuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAxLCAwLjcxMTAxOTYpOwoJCQkJCX0KCQkJCX0KCQkJfQkKCgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5NKQoJCXsKCQkJY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXIgPSAhY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXI7CgkJCVVwZGF0ZU1pY3JvcGFydGljbGVzSW5BaXIoc2VuZGVyKTsKCQkJCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLlApCgkJewoJCQljdXJyZW50UmVmbGVjdGlvblByb2JlID0gIWN1cnJlbnRSZWZsZWN0aW9uUHJvYmU7CgoJCQl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50Um9vdDsKCQkJZm9yZWFjaCAodmFyIHByb2JlIGluIHNjZW5lLkdldENvbXBvbmVudHM8UmVmbGVjdGlvblByb2JlPigpKQoJCQkJcHJvYmUuUmVhbFRpbWUgPSBjdXJyZW50UmVmbGVjdGlvblByb2JlOwoKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCX0KfQoKcHVibGljIHZvaWQgR2FtZU1vZGVfSW5wdXRNZXNzYWdlRXZlbnQoTmVvQXhpcy5HYW1lTW9kZSBzZW5kZXIsIE5lb0F4aXMuSW5wdXRNZXNzYWdlIG1lc3NhZ2UpCnsKCWlmICghc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5Db250cm9sKSkKCQlQcm9jZXNzSW5wdXRNZXNzYWdlRXZlbnQoc2VuZGVyLCBtZXNzYWdlKTsKfQoKcHVibGljIHZvaWQgR2FtZU1vZGVfRW5hYmxlZEluU2ltdWxhdGlvbihOZW9BeGlzLkNvbXBvbmVudCBvYmopCnsKCS8vLy9hY3RpdmF0ZSBuaWdodCBtb2RlCgkvL1Byb2Nlc3NJbnB1dE1lc3NhZ2VFdmVudCgoR2FtZU1vZGUpb2JqLCBuZXcgSW5wdXRNZXNzYWdlS2V5RG93bihFS2V5cy5ENikpOwoJCgkvL3JhbmRvbWl6ZSBzdHJlZXQgbGlnaHRzCgl2YXIgbGlnaHRzID0gb2JqLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50KCJTdHJlZXQgbGlnaHQgbGlnaHRzIik7CglpZihsaWdodHMgIT0gbnVsbCkKCXsKCQl2YXIgcmFuZG9tID0gbmV3IEZhc3RSYW5kb20oKTsKCQkKCQlmb3JlYWNoKHZhciBsaWdodCBpbiBsaWdodHMuR2V0Q29tcG9uZW50czxMaWdodD4oKSkKCQl7CgkJCS8vcmFuZG9taXplIHJvdGF0aW9uCgkJCXZhciB0ciA9IGxpZ2h0LlRyYW5zZm9ybVY7CgkJCXRyID0gdHIuVXBkYXRlUm90YXRpb24oUXVhdGVybmlvbi5Gcm9tUm90YXRlQnlaKHJhbmRvbS5OZXh0KE1hdGguUEkgKiAyKSkpOwoJCQlsaWdodC5UcmFuc2Zvcm0gPSB0cjsKCi8qCgkJCS8vcmFuZG9taXplIGNvbG9ycwoJCQl2YXIgY29sb3IgPSBsaWdodC5Db2xvci5WYWx1ZTsKCQkJdmFyIG1heCA9IDAuNmY7Ly8wLjJmOwoJCQljb2xvci5SZWQgKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJY29sb3IuR3JlZW4gKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJY29sb3IuQmx1ZSArPSByYW5kb20uTmV4dCgtbWF4LCBtYXgpOwoJCQlsaWdodC5Db2xvciA9IGNvbG9yOwoqLwkJCQoJCX0KCX0JCn0=")]
public class DynamicClass8BB9DAE9CAC3AD74E1E5B2AC6E7A1CCC8128DC3C9EB1148E68179123222DB957
{
    public NeoAxis.CSharpScript Owner;
    static bool currentFarDistanceRendering;
    static bool currentNight;
    static int currentWeather;
    static bool currentRandomizeStreetLightColors;
    static bool currentMicroparticlesInAir;
    static bool currentReflectionProbe = true;
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
                    sky.Mode = currentNight ? Sky.ModeEnum.Resource : Sky.ModeEnum.Procedural;
                    //sky.ProceduralIntensity = currentNight ? 0 : 1;
                    //daySky.Enabled = !currentNight;
                    //nightSky.Enabled = currentNight;
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

            if (keyDown.Key == EKeys.P)
            {
                currentReflectionProbe = !currentReflectionProbe;
                var scene = sender.ParentRoot;
                foreach (var probe in scene.GetComponents<ReflectionProbe>())
                    probe.RealTime = currentReflectionProbe;
                message.Handled = true;
                return;
            }
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgRGVtb01vZGVfU2hvd0tleXNFdmVudChOZW9BeGlzLkRlbW9Nb2RlIHNlbmRlciwgU3lzdGVtLkNvbGxlY3Rpb25zLkdlbmVyaWMuTGlzdDxzdHJpbmc+IGxpbmVzKQp7Cgl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CglpZiAoc3lzdGVtID09IG51bGwpCgkJcmV0dXJuOwoJdmFyIHNjZW5lID0gc3lzdGVtLlBhcmVudFJvb3QgYXMgU2NlbmU7CglpZiAoc2NlbmUgPT0gbnVsbCkKCQlyZXR1cm47Cgl2YXIgcmVuZGVyaW5nUGlwZWxpbmUgPSBzY2VuZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nUGlwZWxpbmU+KCJSZW5kZXJpbmcgUGlwZWxpbmUiKTsKCWlmIChyZW5kZXJpbmdQaXBlbGluZSA9PSBudWxsKQoJCXJldHVybjsKCgl2YXIgZmFyRGlzdGFuY2VSZW5kZXJpbmcgPSByZW5kZXJpbmdQaXBlbGluZS5NaW5pbXVtVmlzaWJsZVNpemVPZk9iamVjdHMgPT0gMjsKCXZhciBmYXJEaXN0YW5jZVJlbmRlcmluZ1N0cmluZyA9IGZhckRpc3RhbmNlUmVuZGVyaW5nID8gIm9uIiA6ICJvZmYiOwoKCXZhciBwYXJrZWRWZWhpY2xlc0FzU3RhdGljID0gc3lzdGVtLlBhcmtlZFZlaGljbGVzT2JqZWN0TW9kZS5WYWx1ZSA9PSBUcmFmZmljU3lzdGVtLk9iamVjdE1vZGVFbnVtLlN0YXRpY09iamVjdDsKCXZhciBwYXJrZWRWZWhpY2xlc0FzU3RhdGljU3RyaW5nID0gcGFya2VkVmVoaWNsZXNBc1N0YXRpYyA_ICJvbiIgOiAib2ZmIjsKCgl2YXIgbXVsdGl0aHJlYWRlZFNjZW5lT2N0cmVlID0gc2NlbmUuT2N0cmVlVGhyZWFkaW5nTW9kZS5WYWx1ZSA9PSBPY3RyZWVDb250YWluZXIuVGhyZWFkaW5nTW9kZUVudW0uQmFja2dyb3VuZFRocmVhZDsKCXZhciBtdWx0aXRocmVhZGVkU2NlbmVPY3RyZWVTdHJpbmcgPSBtdWx0aXRocmVhZGVkU2NlbmVPY3RyZWUgPyAib24iIDogIm9mZiI7CgoJc3RyaW5nIHJhaW5TdGF0ZTsKCWlmIChzY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGluZyA+IDApCgkJcmFpblN0YXRlID0gImZhbGxpbmciOwoJZWxzZSBpZiAoc2NlbmUuUHJlY2lwaXRhdGlvbkZhbGxlbiA+IDApCgkJcmFpblN0YXRlID0gImZhbGxlbiI7CgllbHNlCgkJcmFpblN0YXRlID0gInN1bm55IjsKCgkvL3ZhciB3YWxraW5nUGVkZXN0cmlhbnNNYW5hZ2VUYXNrc1N0cmluZyA9IHN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnNNYW5hZ2VUYXNrcy5WYWx1ZSA_ICJvbiIgOiAib2ZmIjsKCglsaW5lcy5BZGQoIiIpOwoJbGluZXMuQWRkKCIxIC0gYnVpbGRpbmdzIik7CglsaW5lcy5BZGQoJCIyIC0gcGFya2VkIHZlaGljbGVzIC0ge3N5c3RlbS5HZXRQYXJrZWRWZWhpY2xlcygpLkNvdW50fSIpOwoJbGluZXMuQWRkKCQiMyAtIGZseWluZyB2ZWhpY2xlcyAtIHtzeXN0ZW0uR2V0Rmx5aW5nT2JqZWN0cygpLkNvdW50fSIpOwoJbGluZXMuQWRkKCQiNCAtIHdhbGtpbmcgcGVkZXN0cmlhbnMgLSB7c3lzdGVtLkdldFdhbGtpbmdQZWRlc3RyaWFucygpLkNvdW50fSIpOwoJbGluZXMuQWRkKCQiNSAtIHJhaW4gLSB7cmFpblN0YXRlfSIpOy8vIC0ge3dhbGtpbmdQZWRlc3RyaWFuc01hbmFnZVRhc2tzU3RyaW5nfSIpOwoJbGluZXMuQWRkKCQiNiAtIHRpbWUgb2YgZGF5Iik7Ly8gLSB7d2Fsa2luZ1BlZGVzdHJpYW5zTWFuYWdlVGFza3NTdHJpbmd9Iik7CglsaW5lcy5BZGQoJCI3IC0gZmFyIGRpc3RhbmNlIHJlbmRlcmluZyAtIHtmYXJEaXN0YW5jZVJlbmRlcmluZ1N0cmluZ30iKTsKCWxpbmVzLkFkZCgkIkMgLSByYW5kb21pemUgc3RyZWV0IGxpZ2h0IGNvbG9ycyIpOwoJbGluZXMuQWRkKCQiTSAtIG1pY3JvcGFydGljbGVzIGluIGFpciAoZHVzdCkiKTsKCWxpbmVzLkFkZCgiIik7CglsaW5lcy5BZGQoJCI4IC0gcGFya2VkIHZlaGljbGVzIGFzIHN0YXRpYyBvYmplY3RzIC0ge3BhcmtlZFZlaGljbGVzQXNTdGF0aWNTdHJpbmd9Iik7CglsaW5lcy5BZGQoIjkgLSBzaW11bGF0ZSBmbHlpbmcgdmVoaWNsZXMiKTsKCWxpbmVzLkFkZCgkIjAgLSBhY3RpdmUgd2Fsa2luZyBwZWRlc3RyaWFucyIpOy8vIC0ge3dhbGtpbmdQZWRlc3RyaWFuc01hbmFnZVRhc2tzU3RyaW5nfSIpOwoJLy9saW5lcy5BZGQoJCIwIC0gbXVsdGl0aHJlYWRlZCBzY2VuZSBvY3RyZWUgLSB7bXVsdGl0aHJlYWRlZFNjZW5lT2N0cmVlU3RyaW5nfSIpOwp9Cg==")]
public class DynamicClass4119F33D963573468452D9837087776EBFA4CC646825F99D6D7407AA2F8347CD
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
        lines.Add("");
        lines.Add($"8 - parked vehicles as static objects - {parkedVehiclesAsStaticString}");
        lines.Add("9 - simulate flying vehicles");
        lines.Add($"0 - active walking pedestrians"); // - {walkingPedestriansManageTasksString}");
    //lines.Add($"0 - multithreaded scene octree - {multithreadedSceneOctreeString}");
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

[CSharpScriptGeneratedAttribute("c3RhdGljIGJvb2wgY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nOwpzdGF0aWMgYm9vbCBjdXJyZW50TmlnaHQ7CnN0YXRpYyBpbnQgY3VycmVudFdlYXRoZXI7CnN0YXRpYyBib29sIGN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9yczsKc3RhdGljIGJvb2wgY3VycmVudE1pY3JvcGFydGljbGVzSW5BaXI7Ci8vc3RhdGljIGJvb2wgY3VycmVudFJlZmxlY3Rpb25Qcm9iZSA9IHRydWU7Cgpjb25zdCBpbnQgU3VubnkgPSAwOwpjb25zdCBpbnQgUmFpbkZhbGxpbmcgPSAxOwpjb25zdCBpbnQgUmFpbkZhbGxlbiA9IDI7Cgp2b2lkIFVwZGF0ZUZvZ0FuZEZhckNsaXBQbGFuZShGb2cgZm9nLCBDYW1lcmEgY2FtZXJhKQp7Cglmb2cuRW5hYmxlZCA9ICFjdXJyZW50TmlnaHQ7Ly8gfHwgY3VycmVudFJhaW47Cglmb2cuRGVuc2l0eSA9IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nID8gMC4wMSA6IDAuMDAxOy8vZm9nLkRlbnNpdHkgPSBjdXJyZW50UmFpbiA_IDAuMDEgOiAwLjAwMTsKCglpZiAoY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcpLy9pZiAoY3VycmVudFJhaW4pCgkJZm9nLkFmZmVjdEJhY2tncm91bmQgPSAxOwoJZWxzZQoJCWZvZy5BZmZlY3RCYWNrZ3JvdW5kID0gY3VycmVudE5pZ2h0ID8gMCA6IDAuNTsKCglpZiAoY3VycmVudE5pZ2h0KQoJCWZvZy5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAsIDAsIDApOwoJZWxzZQoJCWZvZy5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNDUzOTYwOCwgMC41MTYwMzkyLCAwLjY1ODgyMzUpOwoKCWlmIChmb2cuRW5hYmxlZCAmJiBmb2cuQWZmZWN0QmFja2dyb3VuZCA9PSAxKQoJCWNhbWVyYS5GYXJDbGlwUGxhbmUgPSAzMDA7CgllbHNlCgkJY2FtZXJhLkZhckNsaXBQbGFuZSA9IGN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZyA_IDIwMDAgOiAxMDAwOwp9Cgp2b2lkIFVwZGF0ZU1pY3JvcGFydGljbGVzSW5BaXIoIENvbXBvbmVudCBzZW5kZXIgKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50Um9vdDsKCXZhciByZW5kZXJpbmdQaXBlbGluZSA9IHNjZW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdQaXBlbGluZV9CYXNpYz4oIlJlbmRlcmluZyBQaXBlbGluZSIpOwoJdmFyIGVmZmVjdCA9IHJlbmRlcmluZ1BpcGVsaW5lLkdldENvbXBvbmVudDxSZW5kZXJpbmdFZmZlY3RfTWljcm9wYXJ0aWNsZXNJbkFpcj4oY2hlY2tDaGlsZHJlbjogdHJ1ZSk7CglpZiAoZWZmZWN0ICE9IG51bGwpCgl7CgkJaWYgKGN1cnJlbnRNaWNyb3BhcnRpY2xlc0luQWlyKQoJCXsKCQkJZWZmZWN0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMC43LCAwLjYpOwoJCQkvL2VmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuOCwgMC41KTsKCQkJZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMTU7CgkJfQoJCWVsc2UKCQl7CgkJCWlmIChjdXJyZW50TmlnaHQpCgkJCXsKCQkJCWVmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNzUsIDAuNzUsIDEpOwoJCQkJZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMDE1OwoJCQl9CgkJCWVsc2UKCQkJewoJCQkJLy9zaW11bGF0ZSBpbmRpcmVjdCBsaWdodGluZyBieSBtZWFucyBtaWNyb3BhcnRpY2xlcyBpbiBhaXIKCQkJCWVmZmVjdC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDAuOCwgMC43KTsKCQkJCS8vZWZmZWN0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMC44LCAwLjUpOwoJCQkJZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMDM7CgkJCX0KCgkJCS8vZWZmZWN0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMC43NSwgMC43NSwgMSk7CgkJCS8vZWZmZWN0Lk11bHRpcGxpZXIgPSAwLjAwMDE1OwoJCX0KCX0KfQoKdm9pZCBFeGl0RnJvbVZlaGljbGUoTmVvQXhpcy5HYW1lTW9kZSBnYW1lTW9kZSkKewoJdmFyIG9iaiA9IGdhbWVNb2RlLk9iamVjdENvbnRyb2xsZWRCeVBsYXllci5WYWx1ZSBhcyBWZWhpY2xlOwoJaWYgKG9iaiAhPSBudWxsKQoJewoJCXZhciBpbnB1dFByb2Nlc3NpbmcgPSBvYmouR2V0Q29tcG9uZW50PFZlaGljbGVJbnB1dFByb2Nlc3Npbmc+KCk7CgkJaWYgKGlucHV0UHJvY2Vzc2luZyAhPSBudWxsKQoJCQlpbnB1dFByb2Nlc3NpbmcuRXhpdEFsbE9iamVjdHNGcm9tVmVoaWNsZShnYW1lTW9kZSk7Cgl9Cn0KCnZvaWQgUHJvY2Vzc0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuR2FtZU1vZGUgc2VuZGVyLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQp7Cgl2YXIga2V5RG93biA9IG1lc3NhZ2UgYXMgSW5wdXRNZXNzYWdlS2V5RG93bjsKCWlmIChrZXlEb3duICE9IG51bGwpLy8mJiAhc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5Db250cm9sKSkKCXsKCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDEpCgkJewoJCQl2YXIgbWFuYWdlciA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxCdWlsZGluZ01hbmFnZXI+KCk7CgkJCWlmIChtYW5hZ2VyICE9IG51bGwpCgkJCXsKCQkJCW1hbmFnZXIuRGlzcGxheSA9ICFtYW5hZ2VyLkRpc3BsYXk7CgkJCQltYW5hZ2VyLkNvbGxpc2lvbiA9IG1hbmFnZXIuRGlzcGxheTsKCQkJfQoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EMikKCQl7CgkJCUV4aXRGcm9tVmVoaWNsZShzZW5kZXIpOwoKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uUGFya2VkVmVoaWNsZXMgPSBzeXN0ZW0uUGFya2VkVmVoaWNsZXMuVmFsdWUgIT0gMCA_IDAgOiA1MDAwOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EMykKCQl7CgkJCUV4aXRGcm9tVmVoaWNsZShzZW5kZXIpOwoKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uRmx5aW5nVmVoaWNsZXMgPSBzeXN0ZW0uRmx5aW5nVmVoaWNsZXMuVmFsdWUgIT0gMCA_IDAgOiA1MDA7CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ5KQoJCXsKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uU2ltdWxhdGVEeW5hbWljT2JqZWN0cyA9ICFzeXN0ZW0uU2ltdWxhdGVEeW5hbWljT2JqZWN0czsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDQpCgkJewoJCQlFeGl0RnJvbVZlaGljbGUoc2VuZGVyKTsKCgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLldhbGtpbmdQZWRlc3RyaWFucyA9IHN5c3RlbS5XYWxraW5nUGVkZXN0cmlhbnMuVmFsdWUgIT0gMCA_IDAgOiAxMDA7CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQwKQoJCXsKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uV2Fsa2luZ1BlZGVzdHJpYW5zTWFuYWdlVGFza3MgPSAhc3lzdGVtLldhbGtpbmdQZWRlc3RyaWFuc01hbmFnZVRhc2tzOwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5ENSkKCQl7CgkJCXZhciBzY2VuZSA9IChTY2VuZSlzZW5kZXIuUGFyZW50Um9vdDsKCQkJdmFyIHJlbmRlcmluZ1BpcGVsaW5lID0gc2NlbmUuR2V0Q29tcG9uZW50PFJlbmRlcmluZ1BpcGVsaW5lPigiUmVuZGVyaW5nIFBpcGVsaW5lIik7CgkJCXZhciByZWZsZWN0aW9uID0gcmVuZGVyaW5nUGlwZWxpbmU_LkdldENvbXBvbmVudDxSZW5kZXJpbmdFZmZlY3RfUmVmbGVjdGlvbj4oY2hlY2tDaGlsZHJlbjogdHJ1ZSk7CgkJCXZhciBmb2cgPSBzY2VuZS5HZXRDb21wb25lbnQoIkZvZyIpIGFzIEZvZzsKCQkJdmFyIHByZWNpcGl0YXRpb24gPSByZW5kZXJpbmdQaXBlbGluZT8uR2V0Q29tcG9uZW50PFJlbmRlcmluZ0VmZmVjdF9QcmVjaXBpdGF0aW9uPihjaGVja0NoaWxkcmVuOiB0cnVlKTsKCQkJdmFyIHNvdW5kU291cmNlUmFpbiA9IHNjZW5lLkdldENvbXBvbmVudCgiU291bmQgU291cmNlIFJhaW4iKSBhcyBTb3VuZFNvdXJjZTsKCQkJdmFyIGNhbWVyYSA9IHNjZW5lLkdldENvbXBvbmVudDxDYW1lcmE+KCJDYW1lcmEgRGVmYXVsdCIpOwoJCQl2YXIgZGlyZWN0aW9uYWxMaWdodCA9IHNjZW5lLkdldENvbXBvbmVudCgiRGlyZWN0aW9uYWwgTGlnaHQiKSBhcyBMaWdodDsKCgkJCWN1cnJlbnRXZWF0aGVyKys7CgkJCWlmIChjdXJyZW50V2VhdGhlciA+IDIpCgkJCQljdXJyZW50V2VhdGhlciA9IDA7CgkJCS8vY3VycmVudFJhaW4gPSAhY3VycmVudFJhaW47CgoJCQl0cnkKCQkJewoJCQkJVXBkYXRlRm9nQW5kRmFyQ2xpcFBsYW5lKGZvZywgY2FtZXJhKTsKCgkJCQlzb3VuZFNvdXJjZVJhaW4uRW5hYmxlZCA9IGN1cnJlbnRXZWF0aGVyID09IFJhaW5GYWxsaW5nOwoKCQkJCXNjZW5lLlByZWNpcGl0YXRpb25GYWxsaW5nID0gY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcgPyAxIDogMDsKCQkJCXNjZW5lLlByZWNpcGl0YXRpb25GYWxsZW4gPSAoY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcgfHwgY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxlbikgPyAxIDogMDsKCgkJCQkvL3ByZWNpcGl0YXRpb24uRW5hYmxlZCA9IGN1cnJlbnRSYWluOwoJCQkJLy9zb3VuZFNvdXJjZVJhaW4uRW5hYmxlZCA9IGN1cnJlbnRSYWluOwoJCQkJLy9zY2VuZS5QcmVjaXBpdGF0aW9uRmFsbGluZyA9IGN1cnJlbnRSYWluID8gMSA6IDA7CgkJCQkvL3NjZW5lLlByZWNpcGl0YXRpb25GYWxsZW4gPSBjdXJyZW50UmFpbiA_IDEgOiAwOwoKCQkJCS8qCgkJCQkJCQkJaWYoY3VycmVudFdlYXRoZXIgPT0gUmFpbkZhbGxpbmcpLy9pZiAoY3VycmVudFJhaW4pCgkJCQkJCQkJewoJCQkJCQkJCQlkaXJlY3Rpb25hbExpZ2h0Lk1hc2sgPSBuZXcgUmVmZXJlbmNlTm9WYWx1ZShAIlNhbXBsZXNcQ2l0eSBEZW1vXFNraWVzXFJhaW4gY2xvdWRzIG1hc2tcUmFpbiBjbG91ZHMgbWFzay5qcGciKTsKCQkJCQkJCQkJZGlyZWN0aW9uYWxMaWdodC5NYXNrVHJhbnNmb3JtID0gbmV3IFRyYW5zZm9ybShWZWN0b3IzLlplcm8sIFF1YXRlcm5pb24uSWRlbnRpdHksIG5ldyBWZWN0b3IzKDAuMDA1LCAwLjAwNSwgMC4wMDUpKTsKCQkJCQkJCQl9CgkJCQkJCQkJZWxzZQoJCQkJCQkJCXsKCQkJCQkJCQkJZGlyZWN0aW9uYWxMaWdodC5NYXNrID0gbnVsbDsKCQkJCQkJCQl9CgkJCQkqLwoJCQl9CgkJCWNhdGNoIChFeGNlcHRpb24gZSkKCQkJewoJCQkJTG9nLldhcm5pbmcoZS5NZXNzYWdlKTsKCQkJfQoKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDYpCgkJewoJCQl2YXIgc2NlbmUgPSAoU2NlbmUpc2VuZGVyLlBhcmVudFJvb3Q7CgkJCXZhciBhbWJpZW50TGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkFtYmllbnQgTGlnaHQiKSBhcyBMaWdodDsKCQkJdmFyIGRpcmVjdGlvbmFsTGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRpcmVjdGlvbmFsIExpZ2h0IikgYXMgTGlnaHQ7CgkJCXZhciBzdHJlZXRMaWdodExpZ2h0cyA9IHNjZW5lLkdldENvbXBvbmVudCgiU3RyZWV0IGxpZ2h0IGxpZ2h0cyIpOwoJCQl2YXIgc2t5ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJTa3kiKSBhcyBTa3k7CgkJCS8vdmFyIGRheVNreSA9IHNjZW5lLkdldENvbXBvbmVudCgiRGF5IHNreSIpOwoJCQkvL3ZhciBuaWdodFNreSA9IHNjZW5lLkdldENvbXBvbmVudCgiTmlnaHQgc2t5Iik7CgkJCXZhciBmb2cgPSBzY2VuZS5HZXRDb21wb25lbnQoIkZvZyIpIGFzIEZvZzsKCQkJdmFyIGNhbWVyYSA9IHNjZW5lLkdldENvbXBvbmVudDxDYW1lcmE+KCJDYW1lcmEgRGVmYXVsdCIpOwoKCQkJY3VycmVudE5pZ2h0ID0gIWN1cnJlbnROaWdodDsKCgkJCXRyeQoJCQl7CgkJCQlzY2VuZS5UaW1lT2ZEYXkgPSBjdXJyZW50TmlnaHQgPyAwIDogMTI7IAoJCQkJYW1iaWVudExpZ2h0LkJyaWdodG5lc3MgPSBjdXJyZW50TmlnaHQgPyAyNTAwMCA6IDEwMDAwMDsKCQkJCWRpcmVjdGlvbmFsTGlnaHQuRW5hYmxlZCA9ICFjdXJyZW50TmlnaHQ7CgkJCQlzdHJlZXRMaWdodExpZ2h0cy5FbmFibGVkID0gY3VycmVudE5pZ2h0OwoJCQkJc2t5Lk1vZGUgPSBjdXJyZW50TmlnaHQgPyBTa3kuTW9kZUVudW0uUmVzb3VyY2UgOiBTa3kuTW9kZUVudW0uUHJvY2VkdXJhbDsKCQkJCS8vc2t5LlByb2NlZHVyYWxJbnRlbnNpdHkgPSBjdXJyZW50TmlnaHQgPyAwIDogMTsKCQkJCS8vZGF5U2t5LkVuYWJsZWQgPSAhY3VycmVudE5pZ2h0OwoJCQkJLy9uaWdodFNreS5FbmFibGVkID0gY3VycmVudE5pZ2h0OwoJCQkJVXBkYXRlRm9nQW5kRmFyQ2xpcFBsYW5lKGZvZywgY2FtZXJhKTsKCQkJCVVwZGF0ZU1pY3JvcGFydGljbGVzSW5BaXIoc2VuZGVyKTsKCQkJfQoJCQljYXRjaCAoRXhjZXB0aW9uIGUpCgkJCXsKCQkJCUxvZy5XYXJuaW5nKGUuTWVzc2FnZSk7CgkJCX0KCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ4KQoJCXsKCQkJRXhpdEZyb21WZWhpY2xlKHNlbmRlcik7CgoJCQl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CgkJCWlmIChzeXN0ZW0gIT0gbnVsbCkKCQkJewoJCQkJaWYgKHN5c3RlbS5QYXJrZWRWZWhpY2xlc09iamVjdE1vZGUuVmFsdWUgPT0gVHJhZmZpY1N5c3RlbS5PYmplY3RNb2RlRW51bS5WZWhpY2xlQ29tcG9uZW50KQoJCQkJCXN5c3RlbS5QYXJrZWRWZWhpY2xlc09iamVjdE1vZGUgPSBUcmFmZmljU3lzdGVtLk9iamVjdE1vZGVFbnVtLlN0YXRpY09iamVjdDsKCQkJCWVsc2UKCQkJCQlzeXN0ZW0uUGFya2VkVmVoaWNsZXNPYmplY3RNb2RlID0gVHJhZmZpY1N5c3RlbS5PYmplY3RNb2RlRW51bS5WZWhpY2xlQ29tcG9uZW50OwoJCQl9CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ3KQoJCXsKCQkJY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID0gIWN1cnJlbnRGYXJEaXN0YW5jZVJlbmRlcmluZzsKCgkJCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRSb290OwoJCQl2YXIgcmVuZGVyaW5nUGlwZWxpbmUgPSBzY2VuZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nUGlwZWxpbmVfQmFzaWM+KCJSZW5kZXJpbmcgUGlwZWxpbmUiKTsKCQkJdmFyIGNhbWVyYSA9IHNjZW5lLkdldENvbXBvbmVudDxDYW1lcmE+KCJDYW1lcmEgRGVmYXVsdCIpOwoJCQl2YXIgZm9nID0gc2NlbmUuR2V0Q29tcG9uZW50KCJGb2ciKSBhcyBGb2c7CgoJCQkvL2NhbWVyYS5GYXJDbGlwUGxhbmUgPSBjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmcgPyAyMDAwIDogMTAwMDsKCQkJcmVuZGVyaW5nUGlwZWxpbmUuTWluaW11bVZpc2libGVTaXplT2ZPYmplY3RzID0gY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID8gMiA6IDQ7CgoJCQlyZW5kZXJpbmdQaXBlbGluZS5TaGFkb3dEaXJlY3Rpb25hbERpc3RhbmNlID0gY3VycmVudEZhckRpc3RhbmNlUmVuZGVyaW5nID8gNjAwIDogMjAwOwoJCQlyZW5kZXJpbmdQaXBlbGluZS5TaGFkb3dEaXJlY3Rpb25hbExpZ2h0Q2FzY2FkZXMgPSBjdXJyZW50RmFyRGlzdGFuY2VSZW5kZXJpbmcgPyAzIDogMjsKCgkJCXRyeQoJCQl7CgkJCQlVcGRhdGVGb2dBbmRGYXJDbGlwUGxhbmUoZm9nLCBjYW1lcmEpOwoJCQl9CgkJCWNhdGNoIChFeGNlcHRpb24gZSkKCQkJewoJCQkJTG9nLldhcm5pbmcoZS5NZXNzYWdlKTsKCQkJfQoKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoKCQkJLyoKCQkJdmFyIHNjZW5lID0gc2VuZGVyLlBhcmVudFJvb3QgYXMgU2NlbmU7CgkJCWlmIChzY2VuZSAhPSBudWxsKQoJCQl7CgkJCQlpZiAoc2NlbmUuT2N0cmVlVGhyZWFkaW5nTW9kZS5WYWx1ZSA9PSBPY3RyZWVDb250YWluZXIuVGhyZWFkaW5nTW9kZUVudW0uQmFja2dyb3VuZFRocmVhZCkKCQkJCQlzY2VuZS5PY3RyZWVUaHJlYWRpbmdNb2RlID0gT2N0cmVlQ29udGFpbmVyLlRocmVhZGluZ01vZGVFbnVtLlNpbmdsZVRocmVhZGVkOwoJCQkJZWxzZQoJCQkJCXNjZW5lLk9jdHJlZVRocmVhZGluZ01vZGUgPSBPY3RyZWVDb250YWluZXIuVGhyZWFkaW5nTW9kZUVudW0uQmFja2dyb3VuZFRocmVhZDsKCQkJfQoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJCSovCgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5DKQoJCXsKCQkJY3VycmVudFJhbmRvbWl6ZVN0cmVldExpZ2h0Q29sb3JzID0gIWN1cnJlbnRSYW5kb21pemVTdHJlZXRMaWdodENvbG9yczsKCQkJCgkJCXZhciBsaWdodHMgPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQoIlN0cmVldCBsaWdodCBsaWdodHMiKTsKCQkJaWYobGlnaHRzICE9IG51bGwpCgkJCXsKCQkJCXZhciByYW5kb20gPSBuZXcgRmFzdFJhbmRvbSgpOwoJCQkJCgkJCQlmb3JlYWNoKHZhciBsaWdodCBpbiBsaWdodHMuR2V0Q29tcG9uZW50czxMaWdodD4oKSkKCQkJCXsKCQkJCQlpZihjdXJyZW50UmFuZG9taXplU3RyZWV0TGlnaHRDb2xvcnMpCgkJCQkJewoJCQkJCQl2YXIgY29sb3IgPSBsaWdodC5Db2xvci5WYWx1ZTsKCQkJCQkJdmFyIG1heCA9IDAuNmY7Ly8wLjJmOwoJCQkJCQljb2xvci5SZWQgKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJCQkJY29sb3IuR3JlZW4gKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJCQkJY29sb3IuQmx1ZSArPSByYW5kb20uTmV4dCgtbWF4LCBtYXgpOwoJCQkJCQlsaWdodC5Db2xvciA9IGNvbG9yOwoJCQkJCX0KCQkJCQllbHNlCgkJCQkJewoJCQkJCQlsaWdodC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEsIDEsIDAuNzExMDE5Nik7CgkJCQkJfQoJCQkJfQoJCQl9CQoKCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLk0pCgkJewoJCQljdXJyZW50TWljcm9wYXJ0aWNsZXNJbkFpciA9ICFjdXJyZW50TWljcm9wYXJ0aWNsZXNJbkFpcjsKCQkJVXBkYXRlTWljcm9wYXJ0aWNsZXNJbkFpcihzZW5kZXIpOwoJCQkKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQkvKmlmIChrZXlEb3duLktleSA9PSBFS2V5cy5QKQoJCXsKCQkJY3VycmVudFJlZmxlY3Rpb25Qcm9iZSA9ICFjdXJyZW50UmVmbGVjdGlvblByb2JlOwoKCQkJdmFyIHNjZW5lID0gc2VuZGVyLlBhcmVudFJvb3Q7CgkJCWZvcmVhY2ggKHZhciBwcm9iZSBpbiBzY2VuZS5HZXRDb21wb25lbnRzPFJlZmxlY3Rpb25Qcm9iZT4oKSkKCQkJCXByb2JlLlJlYWxUaW1lID0gY3VycmVudFJlZmxlY3Rpb25Qcm9iZTsKCgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9Ki8KCX0KfQoKcHVibGljIHZvaWQgR2FtZU1vZGVfSW5wdXRNZXNzYWdlRXZlbnQoTmVvQXhpcy5HYW1lTW9kZSBzZW5kZXIsIE5lb0F4aXMuSW5wdXRNZXNzYWdlIG1lc3NhZ2UpCnsKCWlmICghc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5Db250cm9sKSkKCQlQcm9jZXNzSW5wdXRNZXNzYWdlRXZlbnQoc2VuZGVyLCBtZXNzYWdlKTsKfQoKcHVibGljIHZvaWQgR2FtZU1vZGVfRW5hYmxlZEluU2ltdWxhdGlvbihOZW9BeGlzLkNvbXBvbmVudCBvYmopCnsKCS8vLy9hY3RpdmF0ZSBuaWdodCBtb2RlCgkvL1Byb2Nlc3NJbnB1dE1lc3NhZ2VFdmVudCgoR2FtZU1vZGUpb2JqLCBuZXcgSW5wdXRNZXNzYWdlS2V5RG93bihFS2V5cy5ENikpOwoJCgkvL3JhbmRvbWl6ZSBzdHJlZXQgbGlnaHRzCgl2YXIgbGlnaHRzID0gb2JqLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50KCJTdHJlZXQgbGlnaHQgbGlnaHRzIik7CglpZihsaWdodHMgIT0gbnVsbCkKCXsKCQl2YXIgcmFuZG9tID0gbmV3IEZhc3RSYW5kb20oKTsKCQkKCQlmb3JlYWNoKHZhciBsaWdodCBpbiBsaWdodHMuR2V0Q29tcG9uZW50czxMaWdodD4oKSkKCQl7CgkJCS8vcmFuZG9taXplIHJvdGF0aW9uCgkJCXZhciB0ciA9IGxpZ2h0LlRyYW5zZm9ybVY7CgkJCXRyID0gdHIuVXBkYXRlUm90YXRpb24oUXVhdGVybmlvbi5Gcm9tUm90YXRlQnlaKHJhbmRvbS5OZXh0KE1hdGguUEkgKiAyKSkpOwoJCQlsaWdodC5UcmFuc2Zvcm0gPSB0cjsKCi8qCgkJCS8vcmFuZG9taXplIGNvbG9ycwoJCQl2YXIgY29sb3IgPSBsaWdodC5Db2xvci5WYWx1ZTsKCQkJdmFyIG1heCA9IDAuNmY7Ly8wLjJmOwoJCQljb2xvci5SZWQgKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJY29sb3IuR3JlZW4gKz0gcmFuZG9tLk5leHQoLW1heCwgbWF4KTsKCQkJY29sb3IuQmx1ZSArPSByYW5kb20uTmV4dCgtbWF4LCBtYXgpOwoJCQlsaWdodC5Db2xvciA9IGNvbG9yOwoqLwkJCQoJCX0KCX0JCn0=")]
public class DynamicClass7060A57114A4F5F9E0C2FD39FB93E6B1E8DA883B9A2188B5E04035DA429A0B24
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
                    sky.Mode = currentNight ? Sky.ModeEnum.Resource : Sky.ModeEnum.Procedural;
                    //sky.ProceduralIntensity = currentNight ? 0 : 1;
                    //daySky.Enabled = !currentNight;
                    //nightSky.Enabled = currentNight;
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlcikKewoJdmFyIHNjZW5lID0gc2VuZGVyLlBhcmVudFNjZW5lOwoKCXZhciBsaWdodCA9IHNjZW5lLkdldENvbXBvbmVudCgiQW1iaWVudCBMaWdodCIpIGFzIExpZ2h0OwoJaWYgKGxpZ2h0ICE9IG51bGwpCgkJbGlnaHQuRW5hYmxlZCA9IHNlbmRlci5BY3RpdmF0ZWQ7CQp9Cg==")]
public class DynamicClassB72C2804A6E79F68D86E5708825D4EAFA3C78C53C8E351341A229618C92B99C0
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender)
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX0dlbmVyYXRlU3RhZ2UoTmVvQXhpcy5QbGFudFR5cGUgc2VuZGVyLCBOZW9BeGlzLlBsYW50R2VuZXJhdG9yIGdlbmVyYXRvciwgTmVvQXhpcy5QbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0gc3RhZ2UpCnsKCS8vVGhpcyBzY3JpcHQgaXMgaW50ZW5kZWQgdG8gc3BlY2lmeSB0aGUgZGF0YSBmb3IgYSBnZW5lcmF0b3IuCgkKCQoJLy9pZGVhczoKCQoJLy_QsdC10YDQtdC30LA6CgkvL9Cx0YDQsNC90Ycg0LIgNyDRgNCw0Lcg0YLQvtC90YzRiNC1INGH0LXQvCDRgNC+0LTQuNGC0LXQu9GMCgkvL9GB0YLQstC+0LssINCx0YDQsNC90YfQuCwg0LLQtdGC0LrQuCDRgNC+0LLQvdGL0LUKCS8v0LzQsNC70L4g0LHRgNCw0L3Rh9C10LksINCy0LXRgtC+0Log0YLQvtC20LUKCS8v0YHQstC10YDRhdGDINCx0L7Qu9GM0YjQtSDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LTRg9CxOgoJLy_QsdC+0LvRjNGI0LUg0LjQt9C+0LPQvdGD0YLQvtGB0YLRjCDRh9C10Lwg0LHQtdGA0LXQt9CwCgkvL9Cy0L3QuNC30YMg0YLQvtC20LUg0LzQvdC+0LPQviDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LXQu9GMOgoJLy_QstC10YLQvtC6INC90LXRgiwg0L7QtNC90Lgg0LHRgNCw0L3Rh9C4PwoJCgkvL9Cy0YvRgdC+0YLQsAoJLy_RgtC+0LvRidC40L3QsCDRgdC_0LvQsNC50L3QvtC5CgoKI2lmICFERVBMT1kKCXZhciBtaW5CcmFuY2hUd2lnTGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAvIDUwLjA7CgkJCglzd2l0Y2goIHN0YWdlICkKCXsKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlRydW5rOgoJCXsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkJCXZhciBzdGFydFRyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oIFZlY3RvcjMuWmVybywgUXVhdGVybmlvbi5Mb29rQXQoIFZlY3RvcjMuWkF4aXMsIFZlY3RvcjMuWEF4aXMgKSApOwoJCQl2YXIgbGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCXZhciB0aGlja25lc3MgPSBsZW5ndGggLyAyMC4wOwoJCgkJCWdlbmVyYXRvci5UcnVua3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBudWxsLCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCBudWxsLCAxNS4wLCAyNC4wLCB0aGlja25lc3MgKiAwLjIsIDAgKSApOwoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uQnJhbmNoOgoJCXsKCQkJdmFyIGNvdW50ID0gNjA7Ly80MDsvLyA0MDsKCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQljb3VudCA9IChpbnQpKCAoZG91YmxlKWNvdW50ICogTWF0aC5Qb3coIGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSwgMiApICk7CgkKCQkJdmFyIHBhcmVudCA9IGdlbmVyYXRvci5UcnVua3NbIDAgXTsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkKCQkJdmFyIGNvbGxpc2lvbkNoZWNrZXIgPSBuZXcgUGxhbnRHZW5lcmF0b3IuQ29sbGlzaW9uQ2hlY2tlcigpOwoJCgkJCXZhciBhZGRlZCA9IDA7CgkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJewoJCQkJdmFyIHRpbWVGYWN0b3IgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjIsIDAuOTUgKTsKCQkJCXZhciB0d2lzdEZhY3RvciA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDEuMCApOwoJCgkJCQlpZiggIWNvbGxpc2lvbkNoZWNrZXIuSW50ZXJzZWN0cyggdGltZUZhY3RvciwgdHdpc3RGYWN0b3IgKSApCgkJCQl7CgkJCQkJdmFyIHZlcnRpY2FsQW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCA0NSwgMTAwICk7CgkJCQkJdmFyIHR3aXN0QW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAtNDUsIDQ1ICk7CgkJCQkJdmFyIHN0YXJ0VHJhbnNmb3JtID0gcGFyZW50LkN1cnZlLkdldFRyYW5zZm9ybU9uU3VyZmFjZSggdGltZUZhY3RvciwgdHdpc3RGYWN0b3IsIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJCXZhciB0aGlja25lc3MgPSBzdGFydFRyYW5zZm9ybS5wYXJlbnRUaGlja25lc3MgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjMsIDAuNSApOwoJCgkJCQkJdmFyIGxlbmd0aCA9IHRoaWNrbmVzcyAqIDIwLjA7CgkJCQkJaWYoIGxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkKCQkJCQl7CgkJCQkJCWdlbmVyYXRvci5CcmFuY2hlcy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50Q3lsaW5kZXIoIHBhcmVudCwgbWF0ZXJpYWwsIHN0YXJ0VHJhbnNmb3JtLnRyYW5zZm9ybSwgbGVuZ3RoLCB0aGlja25lc3MsIG51bGwsIDEwLjAsIDE0LjAsIHRoaWNrbmVzcyAqIDAuNSwgMC4wNSApICk7Ly8gMC4xICkgKTsKCQoJCQkJCQljb2xsaXNpb25DaGVja2VyLkFkZCggdGltZUZhY3RvciwgdHdpc3RGYWN0b3IgKTsKCQoJCQkJCQlhZGRlZCsrOwoJCQkJCQlpZiggYWRkZWQgPj0gY291bnQgKQoJCQkJCQkJYnJlYWs7CgkJCQkJfQoJCQkJfQoJCQl9CgkJfQoJCWJyZWFrOwoJCgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5Ud2lnOgoJCXsKCQkJdmFyIHNlbGVjdG9yID0gbmV3IFBsYW50R2VuZXJhdG9yLlNlbGVjdG9yQnlQcm9iYWJpbGl0eSggZ2VuZXJhdG9yICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuQnJhbmNoZXMuV2hlcmUoIGIgPT4gYi5MZW5ndGggPj0gbWluQnJhbmNoVHdpZ0xlbmd0aCApICk7CgkKCQkJaWYoIHNlbGVjdG9yLkNvdW50ICE9IDAgKQoJCQl7CgkJCQl2YXIgY291bnQgPSA0MDA7Ly8zMDA7Ly8gNDAwOy8vIDIwMDsKCQkJCWlmKCBnZW5lcmF0b3IuQWdlIDwgZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UgKQoJCQkJCWNvdW50ID0gKGludCkoIChkb3VibGUpY291bnQgKiBNYXRoLlBvdyggZ2VuZXJhdG9yLkFnZSAvIGdlbmVyYXRvci5QbGFudFR5cGUuTWF0dXJlQWdlLCAyICkgKTsKCQoJCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkKCQkJCXZhciBhZGRlZCA9IDA7CgkJCQlmb3IoIGludCBuID0gMDsgbiA8IGNvdW50ICogMTA7IG4rKyApCgkJCQl7CgkJCQkJdmFyIHBhcmVudCA9IHNlbGVjdG9yLkdldCgpOwoJCgkJCQkJdmFyIHRpbWVGYWN0b3JPblBhcmVudEN1cnZlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMC4yNSwgMC45NSApOwoJCQkJCXZhciB2ZXJ0aWNhbEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggNDUuMCwgMTAwLjAgKTsKCQkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NS4wLCA0NS4wICk7CgkJCQkJdmFyIHN0YXJ0VHJhbnNmb3JtID0gcGFyZW50LkN1cnZlLkdldFRyYW5zZm9ybU9uU3VyZmFjZSggdGltZUZhY3Rvck9uUGFyZW50Q3VydmUsIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDEuMCApLCB2ZXJ0aWNhbEFuZ2xlLCB0d2lzdEFuZ2xlICk7CgkKCQkJCQl2YXIgdGhpY2tuZXNzID0gc3RhcnRUcmFuc2Zvcm0ucGFyZW50VGhpY2tuZXNzICogZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMC4zLCAwLjUgKTsKCQoJCQkJCXZhciBsZW5ndGggPSB0aGlja25lc3MgKiAyMC4wOwoJCQkJCWlmKCBsZW5ndGggPj0gbWluQnJhbmNoVHdpZ0xlbmd0aCApCgkJCQkJewoJCQkJCQlnZW5lcmF0b3IuVHdpZ3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBwYXJlbnQsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybS50cmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCBudWxsLCAxMC4wLCAxNC4wLCB0aGlja25lc3MgKiAwLjUsIDAuMiApICk7CgkKCQkJCQkJYWRkZWQrKzsKCQkJCQkJaWYoIGFkZGVkID49IGNvdW50ICkKCQkJCQkJCWJyZWFrOwoJCQkJCX0KCQkJCX0KCQkJfQoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uTGVhZjoKCQlpZiggZ2VuZXJhdG9yLkJyYW5jaGVzLkNvdW50ICE9IDAgfHwgZ2VuZXJhdG9yLlR3aWdzLkNvdW50ICE9IDAgKQoJCXsKCQkJdmFyIHNlbGVjdG9yID0gbmV3IFBsYW50R2VuZXJhdG9yLlNlbGVjdG9yQnlQcm9iYWJpbGl0eSggZ2VuZXJhdG9yICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuQnJhbmNoZXMgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5Ud2lncyApOwoJCQlzZWxlY3Rvci5BZGRFbGVtZW50cyggZ2VuZXJhdG9yLlRydW5rcyApOwoJCgkJCS8vISEhIdGA0LDRgdC_0YDQtdC00LXQu9GP0YLRjCDQsiDQt9Cw0LLQuNGB0LjQvNC+0YHRgtC4INC+0YIg0LTQu9C40L3RiwoJCgkJCS8vISEhIdGA0LDQstC90L7QvNC10YDQvdC+INGA0LDRgdC_0YDQtdC00LXQu9GP0YLRjC4g0LHRgNCw0L3Rh9C4LCDQstC10YLQutC4INGC0L7QttC1CgkKCQkJLy8hISEh0L_RgNC40LzQtdC90Y_RgtGMIExlYWZDb3VudAoJCgkJCXZhciBjb3VudCA9IDMwMDA7Ly8yMDAwOy8vIDE1MDA7Ly8gMjAwMDsvLyAyNTAwOwoJCQlpZiggZ2VuZXJhdG9yLkFnZSA8IGdlbmVyYXRvci5QbGFudFR5cGUuTWF0dXJlQWdlICkKCQkJCWNvdW50ID0gKGludCkoIChkb3VibGUpY291bnQgKiBNYXRoLlBvdyggZ2VuZXJhdG9yLkFnZSAvIGdlbmVyYXRvci5QbGFudFR5cGUuTWF0dXJlQWdlLCAyLjUgKSApOwoJCgkJCS8vaWYoIExPRCA+PSAyICkKCQkJLy8JY291bnQgLz0gMjsKCQkJLy9pZiggTE9EID49IDMgKQoJCQkvLwljb3VudCAvPSA2OwoJCgkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQ7IG4rKyApCgkJCXsKCQkJCXZhciBwYXJlbnQgPSBzZWxlY3Rvci5HZXQoKTsKCQoJCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CcmFuY2hXaXRoTGVhdmVzICk7CgkKCQkJCS8vISEhIdC_0L7QstC+0YDQsNGH0LjQstCw0YLRjCDQv9C+INCz0L7RgNC40LfQvtC90YLQsNC70Lg_CgkKCQkJCS8vISEhIdGA0LDRgdC_0YDQtdC00LXQu9C10L3QuNC1CgkKCQkJCS8vISEhIdC+0YDQuNC10L3RgtCw0YbQuNGPINC+0YLQvdC+0YHQuNGC0LXQu9GM0L3QviDRgdC+0LvQvdGG0LAv0LLQtdGA0YXQsAoJCgkJCQl2YXIgdmVydGljYWxBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDkwLjAgLSA0NS4wLCA5MC4wICsgNDUuMCApOwoJCQkJdmFyIHR3aXN0QW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAtNDUuMCwgNDUuMCApOwoJCgkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjMsIDAuOTcgKSwgZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICksIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJLy8hISEhdGlsdEFuZ2xlCgkKCQkJCXZhciBsZW5ndGggPSAxLjA7CgkJCQlpZiggbWF0ZXJpYWwgIT0gbnVsbCApCgkJCQkJbGVuZ3RoID0gbWF0ZXJpYWwuUmVhbExlbmd0aCAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgoJCQkJLy9pZiggTE9EID49IDIgKQoJCQkJLy8JbGVuZ3RoICo9IDEuNTsKCQkJCS8vaWYoIExPRCA+PSAzICkKCQkJCS8vCWxlbmd0aCAqPSAxLjU7CgkKCQkJCWdlbmVyYXRvci5MZWF2ZXMuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudFJpYmJvbiggcGFyZW50LCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0udHJhbnNmb3JtLCBsZW5ndGgsIDAsIHRydWUsIDQ1ICkgKTsKCQkJfQoJCgkJCS8vISEhIQoJCQkvL9C_0YDQvtCy0LXRgNGP0YLRjCDQvNCw0YLQtdGA0LjQsNC7INC10YHRgtGMINC70Lgg0LLQtdGC0LrQsC4KCQkJLy_QtdGB0LvQuCDQvdC10YIg0YLQvtCz0LTQsCDQtNC10LvQsNGC0Ywg0LvQuNGB0YLRjNGPLiDQtdGB0YLRjCDQtdGB0YLRjCDRgtC+0LPQtNCwINCy0YHRjiDQstC10YLQutGDINGA0LjQsdCx0L7QvdC+0LwuCgkKCQl9CgkJYnJlYWs7Cgl9CgkKI2VuZGlmCn0K")]
public class DynamicClass6B3DB836F3D9C592A89A015F5F100BB91E261ADC0B48A24422E0EB15BF21634A
{
    public NeoAxis.CSharpScript Owner;
    public void _GenerateStage(NeoAxis.PlantType sender, NeoAxis.PlantGenerator generator, NeoAxis.PlantGenerator.ElementTypeEnum stage)
    {
        //This script is intended to specify the data for a generator.
        //ideas:
        //береза:
        //бранч в 7 раз тоньше чем родитель
        //ствол, бранчи, ветки ровные
        //мало бранчей, веток тоже
        //сверху больше растительности
        //дуб:
        //больше изогнутость чем береза
        //внизу тоже много растительности
        //ель:
        //веток нет, одни бранчи?
        //высота
        //толщина сплайной
#if !DEPLOY
        var minBranchTwigLength = generator.Height / 50.0;
        switch (stage)
        {
            case PlantGenerator.ElementTypeEnum.Trunk:
            {
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var startTransform = new Transform(Vector3.Zero, Quaternion.LookAt(Vector3.ZAxis, Vector3.XAxis));
                var length = generator.Height * generator.Randomizer.Next(0.8, 1.2);
                var thickness = length / 20.0;
                generator.Trunks.Add(generator.CreateElementCylinder(null, material, startTransform, length, thickness, null, 15.0, 24.0, thickness * 0.2, 0));
            }

                break;
            case PlantGenerator.ElementTypeEnum.Branch:
            {
                var count = 60; //40;// 40;
                if (generator.Age < generator.PlantType.MatureAge)
                    count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                var parent = generator.Trunks[0];
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var collisionChecker = new PlantGenerator.CollisionChecker();
                var added = 0;
                for (int n = 0; n < count * 10; n++)
                {
                    var timeFactor = generator.Randomizer.Next(0.2, 0.95);
                    var twistFactor = generator.Randomizer.Next(1.0);
                    if (!collisionChecker.Intersects(timeFactor, twistFactor))
                    {
                        var verticalAngle = generator.Randomizer.Next(45, 100);
                        var twistAngle = generator.Randomizer.Next(-45, 45);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactor, twistFactor, verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Branches.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.05)); // 0.1 ) );
                            collisionChecker.Add(timeFactor, twistFactor);
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Twig:
            {
                var selector = new PlantGenerator.SelectorByProbability(generator);
                selector.AddElements(generator.Branches.Where(b => b.Length >= minBranchTwigLength));
                if (selector.Count != 0)
                {
                    var count = 400; //300;// 400;// 200;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                    var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                    var added = 0;
                    for (int n = 0; n < count * 10; n++)
                    {
                        var parent = selector.Get();
                        var timeFactorOnParentCurve = generator.Randomizer.Next(0.25, 0.95);
                        var verticalAngle = generator.Randomizer.Next(45.0, 100.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactorOnParentCurve, generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Twigs.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.2));
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Leaf:
                if (generator.Branches.Count != 0 || generator.Twigs.Count != 0)
                {
                    var selector = new PlantGenerator.SelectorByProbability(generator);
                    selector.AddElements(generator.Branches);
                    selector.AddElements(generator.Twigs);
                    selector.AddElements(generator.Trunks);
                    //!!!!распределять в зависимости от длины
                    //!!!!равномерно распределять. бранчи, ветки тоже
                    //!!!!применять LeafCount
                    var count = 3000; //2000;// 1500;// 2000;// 2500;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2.5));
                    //if( LOD >= 2 )
                    //	count /= 2;
                    //if( LOD >= 3 )
                    //	count /= 6;
                    for (int n = 0; n < count; n++)
                    {
                        var parent = selector.Get();
                        var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.BranchWithLeaves);
                        //!!!!поворачивать по горизонтали?
                        //!!!!распределение
                        //!!!!ориентация относительно солнца/верха
                        var verticalAngle = generator.Randomizer.Next(90.0 - 45.0, 90.0 + 45.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(generator.Randomizer.Next(0.3, 0.97), generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        //!!!!tiltAngle
                        var length = 1.0;
                        if (material != null)
                            length = material.RealLength * generator.Randomizer.Next(0.8, 1.2);
                        //if( LOD >= 2 )
                        //	length *= 1.5;
                        //if( LOD >= 3 )
                        //	length *= 1.5;
                        generator.Leaves.Add(generator.CreateElementRibbon(parent, material, startTransform.transform, length, 0, true, 45));
                    }
                //!!!!
                //проверять материал есть ли ветка.
                //если нет тогда делать листья. есть есть тогда всю ветку риббоном.
                }

                break;
        }
#endif
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX0dlbmVyYXRlU3RhZ2UoTmVvQXhpcy5QbGFudFR5cGUgc2VuZGVyLCBOZW9BeGlzLlBsYW50R2VuZXJhdG9yIGdlbmVyYXRvciwgTmVvQXhpcy5QbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0gc3RhZ2UpCnsKCS8vVGhpcyBzY3JpcHQgaXMgaW50ZW5kZWQgdG8gc3BlY2lmeSB0aGUgZGF0YSBmb3IgYSBnZW5lcmF0b3IuCgkKCQoJLy9pZGVhczoKCQoJLy_QsdC10YDQtdC30LA6CgkvL9Cx0YDQsNC90Ycg0LIgNyDRgNCw0Lcg0YLQvtC90YzRiNC1INGH0LXQvCDRgNC+0LTQuNGC0LXQu9GMCgkvL9GB0YLQstC+0LssINCx0YDQsNC90YfQuCwg0LLQtdGC0LrQuCDRgNC+0LLQvdGL0LUKCS8v0LzQsNC70L4g0LHRgNCw0L3Rh9C10LksINCy0LXRgtC+0Log0YLQvtC20LUKCS8v0YHQstC10YDRhdGDINCx0L7Qu9GM0YjQtSDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LTRg9CxOgoJLy_QsdC+0LvRjNGI0LUg0LjQt9C+0LPQvdGD0YLQvtGB0YLRjCDRh9C10Lwg0LHQtdGA0LXQt9CwCgkvL9Cy0L3QuNC30YMg0YLQvtC20LUg0LzQvdC+0LPQviDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LXQu9GMOgoJLy_QstC10YLQvtC6INC90LXRgiwg0L7QtNC90Lgg0LHRgNCw0L3Rh9C4PwoJCgkvL9Cy0YvRgdC+0YLQsAoJLy_RgtC+0LvRidC40L3QsCDRgdC_0LvQsNC50L3QvtC5CgoKI2lmICFERVBMT1kKCXZhciBtaW5CcmFuY2hUd2lnTGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAvIDUwLjA7CgkJCglzd2l0Y2goIHN0YWdlICkKCXsKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlRydW5rOgoJCXsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkJCXZhciBzdGFydFRyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oIFZlY3RvcjMuWmVybywgUXVhdGVybmlvbi5Mb29rQXQoIFZlY3RvcjMuWkF4aXMsIFZlY3RvcjMuWEF4aXMgKSApOwoJCQl2YXIgbGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCXZhciB0aGlja25lc3MgPSBsZW5ndGggLyAyMC4wOwoJCgkJCWdlbmVyYXRvci5UcnVua3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBudWxsLCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCBudWxsLCAxNS4wLCAyNC4wLCB0aGlja25lc3MgKiAwLjIsIDAgKSApOwoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uQnJhbmNoOgoJCXsKCQkJdmFyIGNvdW50ID0gNjA7Ly80MDsvLyA0MDsKCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQljb3VudCA9IChpbnQpKCAoZG91YmxlKWNvdW50ICogTWF0aC5Qb3coIGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSwgMiApICk7CgkKCQkJdmFyIHBhcmVudCA9IGdlbmVyYXRvci5UcnVua3NbIDAgXTsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkKCQkJdmFyIGNvbGxpc2lvbkNoZWNrZXIgPSBuZXcgUGxhbnRHZW5lcmF0b3IuQ29sbGlzaW9uQ2hlY2tlcigpOwoJCgkJCXZhciBhZGRlZCA9IDA7CgkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJewoJCQkJdmFyIHRpbWVGYWN0b3IgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjEvKiAwLjIqLywgMC45NSApOwoJCQkJdmFyIHR3aXN0RmFjdG9yID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICk7CgkKCQkJCWlmKCAhY29sbGlzaW9uQ2hlY2tlci5JbnRlcnNlY3RzKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciApICkKCQkJCXsKCQkJCQl2YXIgdmVydGljYWxBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDQ1LCAxMDAgKTsKCQkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NSwgNDUgKTsKCQkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciwgdmVydGljYWxBbmdsZSwgdHdpc3RBbmdsZSApOwoJCgkJCQkJdmFyIHRoaWNrbmVzcyA9IHN0YXJ0VHJhbnNmb3JtLnBhcmVudFRoaWNrbmVzcyAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMywgMC41ICk7CgkKCQkJCQl2YXIgbGVuZ3RoID0gdGhpY2tuZXNzICogMjAuMDsKCQkJCQlpZiggbGVuZ3RoID49IG1pbkJyYW5jaFR3aWdMZW5ndGggKQoJCQkJCXsKCQkJCQkJZ2VuZXJhdG9yLkJyYW5jaGVzLkFkZCggZ2VuZXJhdG9yLkNyZWF0ZUVsZW1lbnRDeWxpbmRlciggcGFyZW50LCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0udHJhbnNmb3JtLCBsZW5ndGgsIHRoaWNrbmVzcywgbnVsbCwgMTAuMCwgMTQuMCwgdGhpY2tuZXNzICogMC41LCAwLjA1ICkgKTsvLyAwLjEgKSApOwoJCgkJCQkJCWNvbGxpc2lvbkNoZWNrZXIuQWRkKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciApOwoJCgkJCQkJCWFkZGVkKys7CgkJCQkJCWlmKCBhZGRlZCA+PSBjb3VudCApCgkJCQkJCQlicmVhazsKCQkJCQl9CgkJCQl9CgkJCX0KCQl9CgkJYnJlYWs7CgkKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlR3aWc6CgkJewoJCQl2YXIgc2VsZWN0b3IgPSBuZXcgUGxhbnRHZW5lcmF0b3IuU2VsZWN0b3JCeVByb2JhYmlsaXR5KCBnZW5lcmF0b3IgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5CcmFuY2hlcy5XaGVyZSggYiA9PiBiLkxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkgKTsKCQoJCQlpZiggc2VsZWN0b3IuQ291bnQgIT0gMCApCgkJCXsKCQkJCXZhciBjb3VudCA9IDQwMDsvLzMwMDsvLyA0MDA7Ly8gMjAwOwoJCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQkJY291bnQgPSAoaW50KSggKGRvdWJsZSljb3VudCAqIE1hdGguUG93KCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UsIDIgKSApOwoJCgkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJhcmsgKTsKCQoJCQkJdmFyIGFkZGVkID0gMDsKCQkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJCXsKCQkJCQl2YXIgcGFyZW50ID0gc2VsZWN0b3IuR2V0KCk7CgkKCQkJCQl2YXIgdGltZUZhY3Rvck9uUGFyZW50Q3VydmUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjI1LCAwLjk1ICk7CgkJCQkJdmFyIHZlcnRpY2FsQW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCA0NS4wLCAxMDAuMCApOwoJCQkJCXZhciB0d2lzdEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggLTQ1LjAsIDQ1LjAgKTsKCQkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yT25QYXJlbnRDdXJ2ZSwgZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICksIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJCXZhciB0aGlja25lc3MgPSBzdGFydFRyYW5zZm9ybS5wYXJlbnRUaGlja25lc3MgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjMsIDAuNSApOwoJCgkJCQkJdmFyIGxlbmd0aCA9IHRoaWNrbmVzcyAqIDIwLjA7CgkJCQkJaWYoIGxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkKCQkJCQl7CgkJCQkJCWdlbmVyYXRvci5Ud2lncy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50Q3lsaW5kZXIoIHBhcmVudCwgbWF0ZXJpYWwsIHN0YXJ0VHJhbnNmb3JtLnRyYW5zZm9ybSwgbGVuZ3RoLCB0aGlja25lc3MsIG51bGwsIDEwLjAsIDE0LjAsIHRoaWNrbmVzcyAqIDAuNSwgMC4yICkgKTsKCQoJCQkJCQlhZGRlZCsrOwoJCQkJCQlpZiggYWRkZWQgPj0gY291bnQgKQoJCQkJCQkJYnJlYWs7CgkJCQkJfQoJCQkJfQoJCQl9CgkJfQoJCWJyZWFrOwoJCgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5MZWFmOgoJCWlmKCBnZW5lcmF0b3IuQnJhbmNoZXMuQ291bnQgIT0gMCB8fCBnZW5lcmF0b3IuVHdpZ3MuQ291bnQgIT0gMCApCgkJewoJCQl2YXIgc2VsZWN0b3IgPSBuZXcgUGxhbnRHZW5lcmF0b3IuU2VsZWN0b3JCeVByb2JhYmlsaXR5KCBnZW5lcmF0b3IgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5CcmFuY2hlcyApOwoJCQlzZWxlY3Rvci5BZGRFbGVtZW50cyggZ2VuZXJhdG9yLlR3aWdzICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuVHJ1bmtzICk7CgkKCQkJLy8hISEh0YDQsNGB0L_RgNC10LTQtdC70Y_RgtGMINCyINC30LDQstC40YHQuNC80L7RgdGC0Lgg0L7RgiDQtNC70LjQvdGLCgkKCQkJLy8hISEh0YDQsNCy0L3QvtC80LXRgNC90L4g0YDQsNGB0L_RgNC10LTQtdC70Y_RgtGMLiDQsdGA0LDQvdGH0LgsINCy0LXRgtC60Lgg0YLQvtC20LUKCQoJCQkvLyEhISHQv9GA0LjQvNC10L3Rj9GC0YwgTGVhZkNvdW50CgkKCQkJdmFyIGNvdW50ID0gMzAwMDsvLzIwMDA7Ly8gMTUwMDsvLyAyMDAwOy8vIDI1MDA7CgkJCWlmKCBnZW5lcmF0b3IuQWdlIDwgZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UgKQoJCQkJY291bnQgPSAoaW50KSggKGRvdWJsZSljb3VudCAqIE1hdGguUG93KCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UsIDIuNSApICk7CgkKCQkJLy9pZiggTE9EID49IDIgKQoJCQkvLwljb3VudCAvPSAyOwoJCQkvL2lmKCBMT0QgPj0gMyApCgkJCS8vCWNvdW50IC89IDY7CgkKCQkJZm9yKCBpbnQgbiA9IDA7IG4gPCBjb3VudDsgbisrICkKCQkJewoJCQkJdmFyIHBhcmVudCA9IHNlbGVjdG9yLkdldCgpOwoJCgkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJyYW5jaFdpdGhMZWF2ZXMgKTsKCQoJCQkJLy8hISEh0L_QvtCy0L7RgNCw0YfQuNCy0LDRgtGMINC_0L4g0LPQvtGA0LjQt9C+0L3RgtCw0LvQuD8KCQoJCQkJLy8hISEh0YDQsNGB0L_RgNC10LTQtdC70LXQvdC40LUKCQoJCQkJLy8hISEh0L7RgNC40LXQvdGC0LDRhtC40Y8g0L7RgtC90L7RgdC40YLQtdC70YzQvdC+INGB0L7Qu9C90YbQsC_QstC10YDRhdCwCgkKCQkJCXZhciB2ZXJ0aWNhbEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggOTAuMCAtIDQ1LjAsIDkwLjAgKyA0NS4wICk7CgkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NS4wLCA0NS4wICk7CgkKCQkJCXZhciBzdGFydFRyYW5zZm9ybSA9IHBhcmVudC5DdXJ2ZS5HZXRUcmFuc2Zvcm1PblN1cmZhY2UoIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMywgMC45NyApLCBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAxLjAgKSwgdmVydGljYWxBbmdsZSwgdHdpc3RBbmdsZSApOwoJCgkJCQkvLyEhISF0aWx0QW5nbGUKCQoJCQkJdmFyIGxlbmd0aCA9IDEuMDsKCQkJCWlmKCBtYXRlcmlhbCAhPSBudWxsICkKCQkJCQlsZW5ndGggPSBtYXRlcmlhbC5SZWFsTGVuZ3RoICogZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMC44LCAxLjIgKTsKCgkJCQkvL2lmKCBMT0QgPj0gMiApCgkJCQkvLwlsZW5ndGggKj0gMS41OwoJCQkJLy9pZiggTE9EID49IDMgKQoJCQkJLy8JbGVuZ3RoICo9IDEuNTsKCQoJCQkJZ2VuZXJhdG9yLkxlYXZlcy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50UmliYm9uKCBwYXJlbnQsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybS50cmFuc2Zvcm0sIGxlbmd0aCwgMCwgdHJ1ZSwgNDUgKSApOwoJCQl9CgkKCQkJLy8hISEhCgkJCS8v0L_RgNC+0LLQtdGA0Y_RgtGMINC80LDRgtC10YDQuNCw0Lsg0LXRgdGC0Ywg0LvQuCDQstC10YLQutCwLgoJCQkvL9C10YHQu9C4INC90LXRgiDRgtC+0LPQtNCwINC00LXQu9Cw0YLRjCDQu9C40YHRgtGM0Y8uINC10YHRgtGMINC10YHRgtGMINGC0L7Qs9C00LAg0LLRgdGOINCy0LXRgtC60YMg0YDQuNCx0LHQvtC90L7QvC4KCQoJCX0KCQlicmVhazsKCX0KCQojZW5kaWYKfQo=")]
public class DynamicClass8B5D2846666F42912530FD40A96E464AB8663FE6CD01C98E31E95059A38C49CB
{
    public NeoAxis.CSharpScript Owner;
    public void _GenerateStage(NeoAxis.PlantType sender, NeoAxis.PlantGenerator generator, NeoAxis.PlantGenerator.ElementTypeEnum stage)
    {
        //This script is intended to specify the data for a generator.
        //ideas:
        //береза:
        //бранч в 7 раз тоньше чем родитель
        //ствол, бранчи, ветки ровные
        //мало бранчей, веток тоже
        //сверху больше растительности
        //дуб:
        //больше изогнутость чем береза
        //внизу тоже много растительности
        //ель:
        //веток нет, одни бранчи?
        //высота
        //толщина сплайной
#if !DEPLOY
        var minBranchTwigLength = generator.Height / 50.0;
        switch (stage)
        {
            case PlantGenerator.ElementTypeEnum.Trunk:
            {
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var startTransform = new Transform(Vector3.Zero, Quaternion.LookAt(Vector3.ZAxis, Vector3.XAxis));
                var length = generator.Height * generator.Randomizer.Next(0.8, 1.2);
                var thickness = length / 20.0;
                generator.Trunks.Add(generator.CreateElementCylinder(null, material, startTransform, length, thickness, null, 15.0, 24.0, thickness * 0.2, 0));
            }

                break;
            case PlantGenerator.ElementTypeEnum.Branch:
            {
                var count = 60; //40;// 40;
                if (generator.Age < generator.PlantType.MatureAge)
                    count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                var parent = generator.Trunks[0];
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var collisionChecker = new PlantGenerator.CollisionChecker();
                var added = 0;
                for (int n = 0; n < count * 10; n++)
                {
                    var timeFactor = generator.Randomizer.Next(0.1 /* 0.2*/, 0.95);
                    var twistFactor = generator.Randomizer.Next(1.0);
                    if (!collisionChecker.Intersects(timeFactor, twistFactor))
                    {
                        var verticalAngle = generator.Randomizer.Next(45, 100);
                        var twistAngle = generator.Randomizer.Next(-45, 45);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactor, twistFactor, verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Branches.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.05)); // 0.1 ) );
                            collisionChecker.Add(timeFactor, twistFactor);
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Twig:
            {
                var selector = new PlantGenerator.SelectorByProbability(generator);
                selector.AddElements(generator.Branches.Where(b => b.Length >= minBranchTwigLength));
                if (selector.Count != 0)
                {
                    var count = 400; //300;// 400;// 200;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                    var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                    var added = 0;
                    for (int n = 0; n < count * 10; n++)
                    {
                        var parent = selector.Get();
                        var timeFactorOnParentCurve = generator.Randomizer.Next(0.25, 0.95);
                        var verticalAngle = generator.Randomizer.Next(45.0, 100.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactorOnParentCurve, generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Twigs.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.2));
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Leaf:
                if (generator.Branches.Count != 0 || generator.Twigs.Count != 0)
                {
                    var selector = new PlantGenerator.SelectorByProbability(generator);
                    selector.AddElements(generator.Branches);
                    selector.AddElements(generator.Twigs);
                    selector.AddElements(generator.Trunks);
                    //!!!!распределять в зависимости от длины
                    //!!!!равномерно распределять. бранчи, ветки тоже
                    //!!!!применять LeafCount
                    var count = 3000; //2000;// 1500;// 2000;// 2500;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2.5));
                    //if( LOD >= 2 )
                    //	count /= 2;
                    //if( LOD >= 3 )
                    //	count /= 6;
                    for (int n = 0; n < count; n++)
                    {
                        var parent = selector.Get();
                        var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.BranchWithLeaves);
                        //!!!!поворачивать по горизонтали?
                        //!!!!распределение
                        //!!!!ориентация относительно солнца/верха
                        var verticalAngle = generator.Randomizer.Next(90.0 - 45.0, 90.0 + 45.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(generator.Randomizer.Next(0.3, 0.97), generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        //!!!!tiltAngle
                        var length = 1.0;
                        if (material != null)
                            length = material.RealLength * generator.Randomizer.Next(0.8, 1.2);
                        //if( LOD >= 2 )
                        //	length *= 1.5;
                        //if( LOD >= 3 )
                        //	length *= 1.5;
                        generator.Leaves.Add(generator.CreateElementRibbon(parent, material, startTransform.transform, length, 0, true, 45));
                    }
                //!!!!
                //проверять материал есть ли ветка.
                //если нет тогда делать листья. есть есть тогда всю ветку риббоном.
                }

                break;
        }
#endif
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX0dlbmVyYXRlU3RhZ2UoTmVvQXhpcy5QbGFudFR5cGUgc2VuZGVyLCBOZW9BeGlzLlBsYW50R2VuZXJhdG9yIGdlbmVyYXRvciwgTmVvQXhpcy5QbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0gc3RhZ2UpCnsKCS8vVGhpcyBzY3JpcHQgaXMgaW50ZW5kZWQgdG8gc3BlY2lmeSB0aGUgZGF0YSBmb3IgYSBnZW5lcmF0b3IuCgkKCQoJLy9pZGVhczoKCQoJLy_QsdC10YDQtdC30LA6CgkvL9Cx0YDQsNC90Ycg0LIgNyDRgNCw0Lcg0YLQvtC90YzRiNC1INGH0LXQvCDRgNC+0LTQuNGC0LXQu9GMCgkvL9GB0YLQstC+0LssINCx0YDQsNC90YfQuCwg0LLQtdGC0LrQuCDRgNC+0LLQvdGL0LUKCS8v0LzQsNC70L4g0LHRgNCw0L3Rh9C10LksINCy0LXRgtC+0Log0YLQvtC20LUKCS8v0YHQstC10YDRhdGDINCx0L7Qu9GM0YjQtSDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LTRg9CxOgoJLy_QsdC+0LvRjNGI0LUg0LjQt9C+0LPQvdGD0YLQvtGB0YLRjCDRh9C10Lwg0LHQtdGA0LXQt9CwCgkvL9Cy0L3QuNC30YMg0YLQvtC20LUg0LzQvdC+0LPQviDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LXQu9GMOgoJLy_QstC10YLQvtC6INC90LXRgiwg0L7QtNC90Lgg0LHRgNCw0L3Rh9C4PwoJCgkvL9Cy0YvRgdC+0YLQsAoJLy_RgtC+0LvRidC40L3QsCDRgdC_0LvQsNC50L3QvtC5CgoKI2lmICFERVBMT1kKCXZhciBtaW5CcmFuY2hUd2lnTGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAvIDUwLjA7CgkJCglzd2l0Y2goIHN0YWdlICkKCXsKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlRydW5rOgoJCXsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkJCXZhciBzdGFydFRyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oIFZlY3RvcjMuWmVybywgUXVhdGVybmlvbi5Mb29rQXQoIFZlY3RvcjMuWkF4aXMsIFZlY3RvcjMuWEF4aXMgKSApOwoJCQl2YXIgbGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCXZhciB0aGlja25lc3MgPSBsZW5ndGggLyAyMC4wOwoJCgkJCWdlbmVyYXRvci5UcnVua3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBudWxsLCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCBudWxsLCAxNS4wLCAyNC4wLCB0aGlja25lc3MgKiAwLjIsIDAgKSApOwoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uQnJhbmNoOgoJCXsKCQkJdmFyIGNvdW50ID0gNjA7Ly80MDsvLyA0MDsKCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQljb3VudCA9IChpbnQpKCAoZG91YmxlKWNvdW50ICogTWF0aC5Qb3coIGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSwgMiApICk7CgkKCQkJdmFyIHBhcmVudCA9IGdlbmVyYXRvci5UcnVua3NbIDAgXTsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkKCQkJdmFyIGNvbGxpc2lvbkNoZWNrZXIgPSBuZXcgUGxhbnRHZW5lcmF0b3IuQ29sbGlzaW9uQ2hlY2tlcigpOwoJCgkJCXZhciBhZGRlZCA9IDA7CgkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJewoJCQkJdmFyIHRpbWVGYWN0b3IgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjEvKiAwLjIqLywgMC45NSApOwoJCQkJdmFyIHR3aXN0RmFjdG9yID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICk7CgkKCQkJCWlmKCAhY29sbGlzaW9uQ2hlY2tlci5JbnRlcnNlY3RzKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciApICkKCQkJCXsKCQkJCQl2YXIgdmVydGljYWxBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDQ1LCAxMDAgKTsKCQkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NSwgNDUgKTsKCQkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciwgdmVydGljYWxBbmdsZSwgdHdpc3RBbmdsZSApOwoJCgkJCQkJdmFyIHRoaWNrbmVzcyA9IHN0YXJ0VHJhbnNmb3JtLnBhcmVudFRoaWNrbmVzcyAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMywgMC41ICk7CgkKCQkJCQl2YXIgbGVuZ3RoID0gdGhpY2tuZXNzICogMjAuMDsKCQkJCQlpZiggbGVuZ3RoID49IG1pbkJyYW5jaFR3aWdMZW5ndGggKQoJCQkJCXsKCQkJCQkJZ2VuZXJhdG9yLkJyYW5jaGVzLkFkZCggZ2VuZXJhdG9yLkNyZWF0ZUVsZW1lbnRDeWxpbmRlciggcGFyZW50LCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0udHJhbnNmb3JtLCBsZW5ndGgsIHRoaWNrbmVzcywgbnVsbCwgMTAuMCwgMTQuMCwgdGhpY2tuZXNzICogMC41LCAwLjA1ICkgKTsvLyAwLjEgKSApOwoJCgkJCQkJCWNvbGxpc2lvbkNoZWNrZXIuQWRkKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciApOwoJCgkJCQkJCWFkZGVkKys7CgkJCQkJCWlmKCBhZGRlZCA+PSBjb3VudCApCgkJCQkJCQlicmVhazsKCQkJCQl9CgkJCQl9CgkJCX0KCQl9CgkJYnJlYWs7CgkKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlR3aWc6CgkJewoJCQl2YXIgc2VsZWN0b3IgPSBuZXcgUGxhbnRHZW5lcmF0b3IuU2VsZWN0b3JCeVByb2JhYmlsaXR5KCBnZW5lcmF0b3IgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5CcmFuY2hlcy5XaGVyZSggYiA9PiBiLkxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkgKTsKCQoJCQlpZiggc2VsZWN0b3IuQ291bnQgIT0gMCApCgkJCXsKCQkJCXZhciBjb3VudCA9IDQwMDsvLzMwMDsvLyA0MDA7Ly8gMjAwOwoJCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQkJY291bnQgPSAoaW50KSggKGRvdWJsZSljb3VudCAqIE1hdGguUG93KCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UsIDIgKSApOwoJCgkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJhcmsgKTsKCQoJCQkJdmFyIGFkZGVkID0gMDsKCQkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJCXsKCQkJCQl2YXIgcGFyZW50ID0gc2VsZWN0b3IuR2V0KCk7CgkKCQkJCQl2YXIgdGltZUZhY3Rvck9uUGFyZW50Q3VydmUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjI1LCAwLjk1ICk7CgkJCQkJdmFyIHZlcnRpY2FsQW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCA0NS4wLCAxMDAuMCApOwoJCQkJCXZhciB0d2lzdEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggLTQ1LjAsIDQ1LjAgKTsKCQkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yT25QYXJlbnRDdXJ2ZSwgZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICksIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJCXZhciB0aGlja25lc3MgPSBzdGFydFRyYW5zZm9ybS5wYXJlbnRUaGlja25lc3MgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjMsIDAuNSApOwoJCgkJCQkJdmFyIGxlbmd0aCA9IHRoaWNrbmVzcyAqIDIwLjA7CgkJCQkJaWYoIGxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkKCQkJCQl7CgkJCQkJCWdlbmVyYXRvci5Ud2lncy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50Q3lsaW5kZXIoIHBhcmVudCwgbWF0ZXJpYWwsIHN0YXJ0VHJhbnNmb3JtLnRyYW5zZm9ybSwgbGVuZ3RoLCB0aGlja25lc3MsIG51bGwsIDEwLjAsIDE0LjAsIHRoaWNrbmVzcyAqIDAuNSwgMC4yICkgKTsKCQoJCQkJCQlhZGRlZCsrOwoJCQkJCQlpZiggYWRkZWQgPj0gY291bnQgKQoJCQkJCQkJYnJlYWs7CgkJCQkJfQoJCQkJfQoJCQl9CgkJfQoJCWJyZWFrOwoJCgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5MZWFmOgoJCWlmKCBnZW5lcmF0b3IuQnJhbmNoZXMuQ291bnQgIT0gMCB8fCBnZW5lcmF0b3IuVHdpZ3MuQ291bnQgIT0gMCApCgkJewoJCQl2YXIgc2VsZWN0b3IgPSBuZXcgUGxhbnRHZW5lcmF0b3IuU2VsZWN0b3JCeVByb2JhYmlsaXR5KCBnZW5lcmF0b3IgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5CcmFuY2hlcyApOwoJCQlzZWxlY3Rvci5BZGRFbGVtZW50cyggZ2VuZXJhdG9yLlR3aWdzICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuVHJ1bmtzICk7CgkKCQkJLy8hISEh0YDQsNGB0L_RgNC10LTQtdC70Y_RgtGMINCyINC30LDQstC40YHQuNC80L7RgdGC0Lgg0L7RgiDQtNC70LjQvdGLCgkKCQkJLy8hISEh0YDQsNCy0L3QvtC80LXRgNC90L4g0YDQsNGB0L_RgNC10LTQtdC70Y_RgtGMLiDQsdGA0LDQvdGH0LgsINCy0LXRgtC60Lgg0YLQvtC20LUKCQoJCQkvLyEhISHQv9GA0LjQvNC10L3Rj9GC0YwgTGVhZkNvdW50CgkKCQkJdmFyIGNvdW50ID0gNDAwMDsvLzIwMDA7Ly8gMTUwMDsvLyAyMDAwOy8vIDI1MDA7CgkJCWlmKCBnZW5lcmF0b3IuQWdlIDwgZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UgKQoJCQkJY291bnQgPSAoaW50KSggKGRvdWJsZSljb3VudCAqIE1hdGguUG93KCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UsIDIuNSApICk7CgkKCQkJLy9pZiggTE9EID49IDIgKQoJCQkvLwljb3VudCAvPSAyOwoJCQkvL2lmKCBMT0QgPj0gMyApCgkJCS8vCWNvdW50IC89IDY7CgkKCQkJZm9yKCBpbnQgbiA9IDA7IG4gPCBjb3VudDsgbisrICkKCQkJewoJCQkJdmFyIHBhcmVudCA9IHNlbGVjdG9yLkdldCgpOwoJCgkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJyYW5jaFdpdGhMZWF2ZXMgKTsKCQoJCQkJLy8hISEh0L_QvtCy0L7RgNCw0YfQuNCy0LDRgtGMINC_0L4g0LPQvtGA0LjQt9C+0L3RgtCw0LvQuD8KCQoJCQkJLy8hISEh0YDQsNGB0L_RgNC10LTQtdC70LXQvdC40LUKCQoJCQkJLy8hISEh0L7RgNC40LXQvdGC0LDRhtC40Y8g0L7RgtC90L7RgdC40YLQtdC70YzQvdC+INGB0L7Qu9C90YbQsC_QstC10YDRhdCwCgkKCQkJCXZhciB2ZXJ0aWNhbEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggOTAuMCAtIDQ1LjAsIDkwLjAgKyA0NS4wICk7CgkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NS4wLCA0NS4wICk7CgkKCQkJCXZhciBzdGFydFRyYW5zZm9ybSA9IHBhcmVudC5DdXJ2ZS5HZXRUcmFuc2Zvcm1PblN1cmZhY2UoIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMywgMC45NyApLCBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAxLjAgKSwgdmVydGljYWxBbmdsZSwgdHdpc3RBbmdsZSApOwoJCgkJCQkvLyEhISF0aWx0QW5nbGUKCQoJCQkJdmFyIGxlbmd0aCA9IDEuMDsKCQkJCWlmKCBtYXRlcmlhbCAhPSBudWxsICkKCQkJCQlsZW5ndGggPSBtYXRlcmlhbC5SZWFsTGVuZ3RoICogZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMC44LCAxLjIgKTsKCgkJCQkvL2lmKCBMT0QgPj0gMiApCgkJCQkvLwlsZW5ndGggKj0gMS41OwoJCQkJLy9pZiggTE9EID49IDMgKQoJCQkJLy8JbGVuZ3RoICo9IDEuNTsKCQoJCQkJZ2VuZXJhdG9yLkxlYXZlcy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50UmliYm9uKCBwYXJlbnQsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybS50cmFuc2Zvcm0sIGxlbmd0aCwgMCwgdHJ1ZSwgNDUgKSApOwoJCQl9CgkKCQkJLy8hISEhCgkJCS8v0L_RgNC+0LLQtdGA0Y_RgtGMINC80LDRgtC10YDQuNCw0Lsg0LXRgdGC0Ywg0LvQuCDQstC10YLQutCwLgoJCQkvL9C10YHQu9C4INC90LXRgiDRgtC+0LPQtNCwINC00LXQu9Cw0YLRjCDQu9C40YHRgtGM0Y8uINC10YHRgtGMINC10YHRgtGMINGC0L7Qs9C00LAg0LLRgdGOINCy0LXRgtC60YMg0YDQuNCx0LHQvtC90L7QvC4KCQoJCX0KCQlicmVhazsKCX0KCQojZW5kaWYKfQo=")]
public class DynamicClass3101698B293DE01E9FEB141C90B7B457B823907997D710C92AE64A1813741D17
{
    public NeoAxis.CSharpScript Owner;
    public void _GenerateStage(NeoAxis.PlantType sender, NeoAxis.PlantGenerator generator, NeoAxis.PlantGenerator.ElementTypeEnum stage)
    {
        //This script is intended to specify the data for a generator.
        //ideas:
        //береза:
        //бранч в 7 раз тоньше чем родитель
        //ствол, бранчи, ветки ровные
        //мало бранчей, веток тоже
        //сверху больше растительности
        //дуб:
        //больше изогнутость чем береза
        //внизу тоже много растительности
        //ель:
        //веток нет, одни бранчи?
        //высота
        //толщина сплайной
#if !DEPLOY
        var minBranchTwigLength = generator.Height / 50.0;
        switch (stage)
        {
            case PlantGenerator.ElementTypeEnum.Trunk:
            {
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var startTransform = new Transform(Vector3.Zero, Quaternion.LookAt(Vector3.ZAxis, Vector3.XAxis));
                var length = generator.Height * generator.Randomizer.Next(0.8, 1.2);
                var thickness = length / 20.0;
                generator.Trunks.Add(generator.CreateElementCylinder(null, material, startTransform, length, thickness, null, 15.0, 24.0, thickness * 0.2, 0));
            }

                break;
            case PlantGenerator.ElementTypeEnum.Branch:
            {
                var count = 60; //40;// 40;
                if (generator.Age < generator.PlantType.MatureAge)
                    count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                var parent = generator.Trunks[0];
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var collisionChecker = new PlantGenerator.CollisionChecker();
                var added = 0;
                for (int n = 0; n < count * 10; n++)
                {
                    var timeFactor = generator.Randomizer.Next(0.1 /* 0.2*/, 0.95);
                    var twistFactor = generator.Randomizer.Next(1.0);
                    if (!collisionChecker.Intersects(timeFactor, twistFactor))
                    {
                        var verticalAngle = generator.Randomizer.Next(45, 100);
                        var twistAngle = generator.Randomizer.Next(-45, 45);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactor, twistFactor, verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Branches.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.05)); // 0.1 ) );
                            collisionChecker.Add(timeFactor, twistFactor);
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Twig:
            {
                var selector = new PlantGenerator.SelectorByProbability(generator);
                selector.AddElements(generator.Branches.Where(b => b.Length >= minBranchTwigLength));
                if (selector.Count != 0)
                {
                    var count = 400; //300;// 400;// 200;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                    var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                    var added = 0;
                    for (int n = 0; n < count * 10; n++)
                    {
                        var parent = selector.Get();
                        var timeFactorOnParentCurve = generator.Randomizer.Next(0.25, 0.95);
                        var verticalAngle = generator.Randomizer.Next(45.0, 100.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactorOnParentCurve, generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Twigs.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.2));
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Leaf:
                if (generator.Branches.Count != 0 || generator.Twigs.Count != 0)
                {
                    var selector = new PlantGenerator.SelectorByProbability(generator);
                    selector.AddElements(generator.Branches);
                    selector.AddElements(generator.Twigs);
                    selector.AddElements(generator.Trunks);
                    //!!!!распределять в зависимости от длины
                    //!!!!равномерно распределять. бранчи, ветки тоже
                    //!!!!применять LeafCount
                    var count = 4000; //2000;// 1500;// 2000;// 2500;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2.5));
                    //if( LOD >= 2 )
                    //	count /= 2;
                    //if( LOD >= 3 )
                    //	count /= 6;
                    for (int n = 0; n < count; n++)
                    {
                        var parent = selector.Get();
                        var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.BranchWithLeaves);
                        //!!!!поворачивать по горизонтали?
                        //!!!!распределение
                        //!!!!ориентация относительно солнца/верха
                        var verticalAngle = generator.Randomizer.Next(90.0 - 45.0, 90.0 + 45.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(generator.Randomizer.Next(0.3, 0.97), generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        //!!!!tiltAngle
                        var length = 1.0;
                        if (material != null)
                            length = material.RealLength * generator.Randomizer.Next(0.8, 1.2);
                        //if( LOD >= 2 )
                        //	length *= 1.5;
                        //if( LOD >= 3 )
                        //	length *= 1.5;
                        generator.Leaves.Add(generator.CreateElementRibbon(parent, material, startTransform.transform, length, 0, true, 45));
                    }
                //!!!!
                //проверять материал есть ли ветка.
                //если нет тогда делать листья. есть есть тогда всю ветку риббоном.
                }

                break;
        }
#endif
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX0dlbmVyYXRlU3RhZ2UoTmVvQXhpcy5QbGFudFR5cGUgc2VuZGVyLCBOZW9BeGlzLlBsYW50R2VuZXJhdG9yIGdlbmVyYXRvciwgTmVvQXhpcy5QbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0gc3RhZ2UpCnsKCS8vVGhpcyBzY3JpcHQgaXMgaW50ZW5kZWQgdG8gc3BlY2lmeSB0aGUgZGF0YSBmb3IgYSBnZW5lcmF0b3IuCgkKCQoJLy9pZGVhczoKCQoJLy_QsdC10YDQtdC30LA6CgkvL9Cx0YDQsNC90Ycg0LIgNyDRgNCw0Lcg0YLQvtC90YzRiNC1INGH0LXQvCDRgNC+0LTQuNGC0LXQu9GMCgkvL9GB0YLQstC+0LssINCx0YDQsNC90YfQuCwg0LLQtdGC0LrQuCDRgNC+0LLQvdGL0LUKCS8v0LzQsNC70L4g0LHRgNCw0L3Rh9C10LksINCy0LXRgtC+0Log0YLQvtC20LUKCS8v0YHQstC10YDRhdGDINCx0L7Qu9GM0YjQtSDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LTRg9CxOgoJLy_QsdC+0LvRjNGI0LUg0LjQt9C+0LPQvdGD0YLQvtGB0YLRjCDRh9C10Lwg0LHQtdGA0LXQt9CwCgkvL9Cy0L3QuNC30YMg0YLQvtC20LUg0LzQvdC+0LPQviDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LXQu9GMOgoJLy_QstC10YLQvtC6INC90LXRgiwg0L7QtNC90Lgg0LHRgNCw0L3Rh9C4PwoJCgkvL9Cy0YvRgdC+0YLQsAoJLy_RgtC+0LvRidC40L3QsCDRgdC_0LvQsNC50L3QvtC5CgoKI2lmICFERVBMT1kKCXZhciBtaW5CcmFuY2hUd2lnTGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAvIDUwLjA7CgkJCglzd2l0Y2goIHN0YWdlICkKCXsKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlRydW5rOgoJCXsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkJCXZhciBzdGFydFRyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oIFZlY3RvcjMuWmVybywgUXVhdGVybmlvbi5Mb29rQXQoIFZlY3RvcjMuWkF4aXMsIFZlY3RvcjMuWEF4aXMgKSApOwoJCQl2YXIgbGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCXZhciB0aGlja25lc3MgPSBsZW5ndGggLyAyMC4wOwoJCgkJCWdlbmVyYXRvci5UcnVua3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBudWxsLCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCBudWxsLCAxNS4wLCAyNC4wLCB0aGlja25lc3MgKiAwLjIsIDAgKSApOwoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uQnJhbmNoOgoJCXsKCQkJdmFyIGNvdW50ID0gNjA7Ly80MDsvLyA0MDsKCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQljb3VudCA9IChpbnQpKCAoZG91YmxlKWNvdW50ICogTWF0aC5Qb3coIGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSwgMiApICk7CgkKCQkJdmFyIHBhcmVudCA9IGdlbmVyYXRvci5UcnVua3NbIDAgXTsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkKCQkJdmFyIGNvbGxpc2lvbkNoZWNrZXIgPSBuZXcgUGxhbnRHZW5lcmF0b3IuQ29sbGlzaW9uQ2hlY2tlcigpOwoJCgkJCXZhciBhZGRlZCA9IDA7CgkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJewoJCQkJdmFyIHRpbWVGYWN0b3IgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjA1LyogMC4yKi8sIDAuOTUgKTsKCQkJCXZhciB0d2lzdEZhY3RvciA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDEuMCApOwoJCgkJCQlpZiggIWNvbGxpc2lvbkNoZWNrZXIuSW50ZXJzZWN0cyggdGltZUZhY3RvciwgdHdpc3RGYWN0b3IgKSApCgkJCQl7CgkJCQkJdmFyIHZlcnRpY2FsQW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCA0NSwgMTAwICk7CgkJCQkJdmFyIHR3aXN0QW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAtNDUsIDQ1ICk7CgkJCQkJdmFyIHN0YXJ0VHJhbnNmb3JtID0gcGFyZW50LkN1cnZlLkdldFRyYW5zZm9ybU9uU3VyZmFjZSggdGltZUZhY3RvciwgdHdpc3RGYWN0b3IsIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJCXZhciB0aGlja25lc3MgPSBzdGFydFRyYW5zZm9ybS5wYXJlbnRUaGlja25lc3MgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjMsIDAuNSApOwoJCgkJCQkJdmFyIGxlbmd0aCA9IHRoaWNrbmVzcyAqIDIwLjA7CgkJCQkJaWYoIGxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkKCQkJCQl7CgkJCQkJCWdlbmVyYXRvci5CcmFuY2hlcy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50Q3lsaW5kZXIoIHBhcmVudCwgbWF0ZXJpYWwsIHN0YXJ0VHJhbnNmb3JtLnRyYW5zZm9ybSwgbGVuZ3RoLCB0aGlja25lc3MsIG51bGwsIDEwLjAsIDE0LjAsIHRoaWNrbmVzcyAqIDAuNSwgMC4wNSApICk7Ly8gMC4xICkgKTsKCQoJCQkJCQljb2xsaXNpb25DaGVja2VyLkFkZCggdGltZUZhY3RvciwgdHdpc3RGYWN0b3IgKTsKCQoJCQkJCQlhZGRlZCsrOwoJCQkJCQlpZiggYWRkZWQgPj0gY291bnQgKQoJCQkJCQkJYnJlYWs7CgkJCQkJfQoJCQkJfQoJCQl9CgkJfQoJCWJyZWFrOwoJCgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5Ud2lnOgoJCXsKCQkJdmFyIHNlbGVjdG9yID0gbmV3IFBsYW50R2VuZXJhdG9yLlNlbGVjdG9yQnlQcm9iYWJpbGl0eSggZ2VuZXJhdG9yICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuQnJhbmNoZXMuV2hlcmUoIGIgPT4gYi5MZW5ndGggPj0gbWluQnJhbmNoVHdpZ0xlbmd0aCApICk7CgkKCQkJaWYoIHNlbGVjdG9yLkNvdW50ICE9IDAgKQoJCQl7CgkJCQl2YXIgY291bnQgPSA0MDA7Ly8zMDA7Ly8gNDAwOy8vIDIwMDsKCQkJCWlmKCBnZW5lcmF0b3IuQWdlIDwgZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UgKQoJCQkJCWNvdW50ID0gKGludCkoIChkb3VibGUpY291bnQgKiBNYXRoLlBvdyggZ2VuZXJhdG9yLkFnZSAvIGdlbmVyYXRvci5QbGFudFR5cGUuTWF0dXJlQWdlLCAyICkgKTsKCQoJCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkKCQkJCXZhciBhZGRlZCA9IDA7CgkJCQlmb3IoIGludCBuID0gMDsgbiA8IGNvdW50ICogMTA7IG4rKyApCgkJCQl7CgkJCQkJdmFyIHBhcmVudCA9IHNlbGVjdG9yLkdldCgpOwoJCgkJCQkJdmFyIHRpbWVGYWN0b3JPblBhcmVudEN1cnZlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMC4yNSwgMC45NSApOwoJCQkJCXZhciB2ZXJ0aWNhbEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggNDUuMCwgMTAwLjAgKTsKCQkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NS4wLCA0NS4wICk7CgkJCQkJdmFyIHN0YXJ0VHJhbnNmb3JtID0gcGFyZW50LkN1cnZlLkdldFRyYW5zZm9ybU9uU3VyZmFjZSggdGltZUZhY3Rvck9uUGFyZW50Q3VydmUsIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDEuMCApLCB2ZXJ0aWNhbEFuZ2xlLCB0d2lzdEFuZ2xlICk7CgkKCQkJCQl2YXIgdGhpY2tuZXNzID0gc3RhcnRUcmFuc2Zvcm0ucGFyZW50VGhpY2tuZXNzICogZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMC4zLCAwLjUgKTsKCQoJCQkJCXZhciBsZW5ndGggPSB0aGlja25lc3MgKiAyMC4wOwoJCQkJCWlmKCBsZW5ndGggPj0gbWluQnJhbmNoVHdpZ0xlbmd0aCApCgkJCQkJewoJCQkJCQlnZW5lcmF0b3IuVHdpZ3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBwYXJlbnQsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybS50cmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCBudWxsLCAxMC4wLCAxNC4wLCB0aGlja25lc3MgKiAwLjUsIDAuMiApICk7CgkKCQkJCQkJYWRkZWQrKzsKCQkJCQkJaWYoIGFkZGVkID49IGNvdW50ICkKCQkJCQkJCWJyZWFrOwoJCQkJCX0KCQkJCX0KCQkJfQoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uTGVhZjoKCQlpZiggZ2VuZXJhdG9yLkJyYW5jaGVzLkNvdW50ICE9IDAgfHwgZ2VuZXJhdG9yLlR3aWdzLkNvdW50ICE9IDAgKQoJCXsKCQkJdmFyIHNlbGVjdG9yID0gbmV3IFBsYW50R2VuZXJhdG9yLlNlbGVjdG9yQnlQcm9iYWJpbGl0eSggZ2VuZXJhdG9yICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuQnJhbmNoZXMgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5Ud2lncyApOwoJCQlzZWxlY3Rvci5BZGRFbGVtZW50cyggZ2VuZXJhdG9yLlRydW5rcyApOwoJCgkJCS8vISEhIdGA0LDRgdC_0YDQtdC00LXQu9GP0YLRjCDQsiDQt9Cw0LLQuNGB0LjQvNC+0YHRgtC4INC+0YIg0LTQu9C40L3RiwoJCgkJCS8vISEhIdGA0LDQstC90L7QvNC10YDQvdC+INGA0LDRgdC_0YDQtdC00LXQu9GP0YLRjC4g0LHRgNCw0L3Rh9C4LCDQstC10YLQutC4INGC0L7QttC1CgkKCQkJLy8hISEh0L_RgNC40LzQtdC90Y_RgtGMIExlYWZDb3VudAoJCgkJCXZhciBjb3VudCA9IDQwMDA7Ly8yMDAwOy8vIDE1MDA7Ly8gMjAwMDsvLyAyNTAwOwoJCQlpZiggZ2VuZXJhdG9yLkFnZSA8IGdlbmVyYXRvci5QbGFudFR5cGUuTWF0dXJlQWdlICkKCQkJCWNvdW50ID0gKGludCkoIChkb3VibGUpY291bnQgKiBNYXRoLlBvdyggZ2VuZXJhdG9yLkFnZSAvIGdlbmVyYXRvci5QbGFudFR5cGUuTWF0dXJlQWdlLCAyLjUgKSApOwoJCgkJCS8vaWYoIExPRCA+PSAyICkKCQkJLy8JY291bnQgLz0gMjsKCQkJLy9pZiggTE9EID49IDMgKQoJCQkvLwljb3VudCAvPSA2OwoJCgkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQ7IG4rKyApCgkJCXsKCQkJCXZhciBwYXJlbnQgPSBzZWxlY3Rvci5HZXQoKTsKCQoJCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CcmFuY2hXaXRoTGVhdmVzICk7CgkKCQkJCS8vISEhIdC_0L7QstC+0YDQsNGH0LjQstCw0YLRjCDQv9C+INCz0L7RgNC40LfQvtC90YLQsNC70Lg_CgkKCQkJCS8vISEhIdGA0LDRgdC_0YDQtdC00LXQu9C10L3QuNC1CgkKCQkJCS8vISEhIdC+0YDQuNC10L3RgtCw0YbQuNGPINC+0YLQvdC+0YHQuNGC0LXQu9GM0L3QviDRgdC+0LvQvdGG0LAv0LLQtdGA0YXQsAoJCgkJCQl2YXIgdmVydGljYWxBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDkwLjAgLSA0NS4wLCA5MC4wICsgNDUuMCApOwoJCQkJdmFyIHR3aXN0QW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAtNDUuMCwgNDUuMCApOwoJCgkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjMsIDAuOTcgKSwgZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICksIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJLy8hISEhdGlsdEFuZ2xlCgkKCQkJCXZhciBsZW5ndGggPSAxLjA7CgkJCQlpZiggbWF0ZXJpYWwgIT0gbnVsbCApCgkJCQkJbGVuZ3RoID0gbWF0ZXJpYWwuUmVhbExlbmd0aCAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgoJCQkJLy9pZiggTE9EID49IDIgKQoJCQkJLy8JbGVuZ3RoICo9IDEuNTsKCQkJCS8vaWYoIExPRCA+PSAzICkKCQkJCS8vCWxlbmd0aCAqPSAxLjU7CgkKCQkJCWdlbmVyYXRvci5MZWF2ZXMuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudFJpYmJvbiggcGFyZW50LCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0udHJhbnNmb3JtLCBsZW5ndGgsIDAsIHRydWUsIDQ1ICkgKTsKCQkJfQoJCgkJCS8vISEhIQoJCQkvL9C_0YDQvtCy0LXRgNGP0YLRjCDQvNCw0YLQtdGA0LjQsNC7INC10YHRgtGMINC70Lgg0LLQtdGC0LrQsC4KCQkJLy_QtdGB0LvQuCDQvdC10YIg0YLQvtCz0LTQsCDQtNC10LvQsNGC0Ywg0LvQuNGB0YLRjNGPLiDQtdGB0YLRjCDQtdGB0YLRjCDRgtC+0LPQtNCwINCy0YHRjiDQstC10YLQutGDINGA0LjQsdCx0L7QvdC+0LwuCgkKCQl9CgkJYnJlYWs7Cgl9CgkKI2VuZGlmCn0K")]
public class DynamicClass7A7F512CFE18A344DB820666E21D973F862986C852ABD5D2EF7A56F5AFEB00B8
{
    public NeoAxis.CSharpScript Owner;
    public void _GenerateStage(NeoAxis.PlantType sender, NeoAxis.PlantGenerator generator, NeoAxis.PlantGenerator.ElementTypeEnum stage)
    {
        //This script is intended to specify the data for a generator.
        //ideas:
        //береза:
        //бранч в 7 раз тоньше чем родитель
        //ствол, бранчи, ветки ровные
        //мало бранчей, веток тоже
        //сверху больше растительности
        //дуб:
        //больше изогнутость чем береза
        //внизу тоже много растительности
        //ель:
        //веток нет, одни бранчи?
        //высота
        //толщина сплайной
#if !DEPLOY
        var minBranchTwigLength = generator.Height / 50.0;
        switch (stage)
        {
            case PlantGenerator.ElementTypeEnum.Trunk:
            {
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var startTransform = new Transform(Vector3.Zero, Quaternion.LookAt(Vector3.ZAxis, Vector3.XAxis));
                var length = generator.Height * generator.Randomizer.Next(0.8, 1.2);
                var thickness = length / 20.0;
                generator.Trunks.Add(generator.CreateElementCylinder(null, material, startTransform, length, thickness, null, 15.0, 24.0, thickness * 0.2, 0));
            }

                break;
            case PlantGenerator.ElementTypeEnum.Branch:
            {
                var count = 60; //40;// 40;
                if (generator.Age < generator.PlantType.MatureAge)
                    count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                var parent = generator.Trunks[0];
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var collisionChecker = new PlantGenerator.CollisionChecker();
                var added = 0;
                for (int n = 0; n < count * 10; n++)
                {
                    var timeFactor = generator.Randomizer.Next(0.05 /* 0.2*/, 0.95);
                    var twistFactor = generator.Randomizer.Next(1.0);
                    if (!collisionChecker.Intersects(timeFactor, twistFactor))
                    {
                        var verticalAngle = generator.Randomizer.Next(45, 100);
                        var twistAngle = generator.Randomizer.Next(-45, 45);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactor, twistFactor, verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Branches.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.05)); // 0.1 ) );
                            collisionChecker.Add(timeFactor, twistFactor);
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Twig:
            {
                var selector = new PlantGenerator.SelectorByProbability(generator);
                selector.AddElements(generator.Branches.Where(b => b.Length >= minBranchTwigLength));
                if (selector.Count != 0)
                {
                    var count = 400; //300;// 400;// 200;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                    var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                    var added = 0;
                    for (int n = 0; n < count * 10; n++)
                    {
                        var parent = selector.Get();
                        var timeFactorOnParentCurve = generator.Randomizer.Next(0.25, 0.95);
                        var verticalAngle = generator.Randomizer.Next(45.0, 100.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactorOnParentCurve, generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Twigs.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.2));
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Leaf:
                if (generator.Branches.Count != 0 || generator.Twigs.Count != 0)
                {
                    var selector = new PlantGenerator.SelectorByProbability(generator);
                    selector.AddElements(generator.Branches);
                    selector.AddElements(generator.Twigs);
                    selector.AddElements(generator.Trunks);
                    //!!!!распределять в зависимости от длины
                    //!!!!равномерно распределять. бранчи, ветки тоже
                    //!!!!применять LeafCount
                    var count = 4000; //2000;// 1500;// 2000;// 2500;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2.5));
                    //if( LOD >= 2 )
                    //	count /= 2;
                    //if( LOD >= 3 )
                    //	count /= 6;
                    for (int n = 0; n < count; n++)
                    {
                        var parent = selector.Get();
                        var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.BranchWithLeaves);
                        //!!!!поворачивать по горизонтали?
                        //!!!!распределение
                        //!!!!ориентация относительно солнца/верха
                        var verticalAngle = generator.Randomizer.Next(90.0 - 45.0, 90.0 + 45.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(generator.Randomizer.Next(0.3, 0.97), generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        //!!!!tiltAngle
                        var length = 1.0;
                        if (material != null)
                            length = material.RealLength * generator.Randomizer.Next(0.8, 1.2);
                        //if( LOD >= 2 )
                        //	length *= 1.5;
                        //if( LOD >= 3 )
                        //	length *= 1.5;
                        generator.Leaves.Add(generator.CreateElementRibbon(parent, material, startTransform.transform, length, 0, true, 45));
                    }
                //!!!!
                //проверять материал есть ли ветка.
                //если нет тогда делать листья. есть есть тогда всю ветку риббоном.
                }

                break;
        }
#endif
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX0dlbmVyYXRlU3RhZ2UoTmVvQXhpcy5QbGFudFR5cGUgc2VuZGVyLCBOZW9BeGlzLlBsYW50R2VuZXJhdG9yIGdlbmVyYXRvciwgTmVvQXhpcy5QbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0gc3RhZ2UpCnsKCS8vVGhpcyBzY3JpcHQgaXMgaW50ZW5kZWQgdG8gc3BlY2lmeSB0aGUgZGF0YSBmb3IgYSBnZW5lcmF0b3IuCgkKCQoJLy9pZGVhczoKCQoJLy_QsdC10YDQtdC30LA6CgkvL9Cx0YDQsNC90Ycg0LIgNyDRgNCw0Lcg0YLQvtC90YzRiNC1INGH0LXQvCDRgNC+0LTQuNGC0LXQu9GMCgkvL9GB0YLQstC+0LssINCx0YDQsNC90YfQuCwg0LLQtdGC0LrQuCDRgNC+0LLQvdGL0LUKCS8v0LzQsNC70L4g0LHRgNCw0L3Rh9C10LksINCy0LXRgtC+0Log0YLQvtC20LUKCS8v0YHQstC10YDRhdGDINCx0L7Qu9GM0YjQtSDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LTRg9CxOgoJLy_QsdC+0LvRjNGI0LUg0LjQt9C+0LPQvdGD0YLQvtGB0YLRjCDRh9C10Lwg0LHQtdGA0LXQt9CwCgkvL9Cy0L3QuNC30YMg0YLQvtC20LUg0LzQvdC+0LPQviDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LXQu9GMOgoJLy_QstC10YLQvtC6INC90LXRgiwg0L7QtNC90Lgg0LHRgNCw0L3Rh9C4PwoJCgkvL9Cy0YvRgdC+0YLQsAoJLy_RgtC+0LvRidC40L3QsCDRgdC_0LvQsNC50L3QvtC5CgoKI2lmICFERVBMT1kKCXZhciBtaW5CcmFuY2hUd2lnTGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAvIDUwLjA7CgkJCglzd2l0Y2goIHN0YWdlICkKCXsKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlRydW5rOgoJCXsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkJCXZhciBzdGFydFRyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oIFZlY3RvcjMuWmVybywgUXVhdGVybmlvbi5Mb29rQXQoIFZlY3RvcjMuWkF4aXMsIFZlY3RvcjMuWEF4aXMgKSApOwoJCQl2YXIgbGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCXZhciB0aGlja25lc3MgPSBsZW5ndGggLyAyMC4wOwoJCgkJCWdlbmVyYXRvci5UcnVua3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBudWxsLCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCBudWxsLCAxNS4wLCAyNC4wLCB0aGlja25lc3MgKiAwLjIsIDAgKSApOwoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uQnJhbmNoOgoJCXsKCQkJdmFyIGNvdW50ID0gNjA7Ly80MDsvLyA0MDsKCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQljb3VudCA9IChpbnQpKCAoZG91YmxlKWNvdW50ICogTWF0aC5Qb3coIGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSwgMiApICk7CgkKCQkJdmFyIHBhcmVudCA9IGdlbmVyYXRvci5UcnVua3NbIDAgXTsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkKCQkJdmFyIGNvbGxpc2lvbkNoZWNrZXIgPSBuZXcgUGxhbnRHZW5lcmF0b3IuQ29sbGlzaW9uQ2hlY2tlcigpOwoJCgkJCXZhciBhZGRlZCA9IDA7CgkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJewoJCQkJdmFyIHRpbWVGYWN0b3IgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjEvKiAwLjIqLywgMC45NSApOwoJCQkJdmFyIHR3aXN0RmFjdG9yID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICk7CgkKCQkJCWlmKCAhY29sbGlzaW9uQ2hlY2tlci5JbnRlcnNlY3RzKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciApICkKCQkJCXsKCQkJCQl2YXIgdmVydGljYWxBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDQ1LCAxMDAgKTsKCQkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NSwgNDUgKTsKCQkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciwgdmVydGljYWxBbmdsZSwgdHdpc3RBbmdsZSApOwoJCgkJCQkJdmFyIHRoaWNrbmVzcyA9IHN0YXJ0VHJhbnNmb3JtLnBhcmVudFRoaWNrbmVzcyAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMywgMC41ICk7CgkKCQkJCQl2YXIgbGVuZ3RoID0gdGhpY2tuZXNzICogMjAuMDsKCQkJCQlpZiggbGVuZ3RoID49IG1pbkJyYW5jaFR3aWdMZW5ndGggKQoJCQkJCXsKCQkJCQkJZ2VuZXJhdG9yLkJyYW5jaGVzLkFkZCggZ2VuZXJhdG9yLkNyZWF0ZUVsZW1lbnRDeWxpbmRlciggcGFyZW50LCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0udHJhbnNmb3JtLCBsZW5ndGgsIHRoaWNrbmVzcywgbnVsbCwgMTAuMCwgMTQuMCwgdGhpY2tuZXNzICogMC41LCAwLjA1ICkgKTsvLyAwLjEgKSApOwoJCgkJCQkJCWNvbGxpc2lvbkNoZWNrZXIuQWRkKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciApOwoJCgkJCQkJCWFkZGVkKys7CgkJCQkJCWlmKCBhZGRlZCA+PSBjb3VudCApCgkJCQkJCQlicmVhazsKCQkJCQl9CgkJCQl9CgkJCX0KCQl9CgkJYnJlYWs7CgkKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlR3aWc6CgkJewoJCQl2YXIgc2VsZWN0b3IgPSBuZXcgUGxhbnRHZW5lcmF0b3IuU2VsZWN0b3JCeVByb2JhYmlsaXR5KCBnZW5lcmF0b3IgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5CcmFuY2hlcy5XaGVyZSggYiA9PiBiLkxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkgKTsKCQoJCQlpZiggc2VsZWN0b3IuQ291bnQgIT0gMCApCgkJCXsKCQkJCXZhciBjb3VudCA9IDQwMDsvLzMwMDsvLyA0MDA7Ly8gMjAwOwoJCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQkJY291bnQgPSAoaW50KSggKGRvdWJsZSljb3VudCAqIE1hdGguUG93KCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UsIDIgKSApOwoJCgkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJhcmsgKTsKCQoJCQkJdmFyIGFkZGVkID0gMDsKCQkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJCXsKCQkJCQl2YXIgcGFyZW50ID0gc2VsZWN0b3IuR2V0KCk7CgkKCQkJCQl2YXIgdGltZUZhY3Rvck9uUGFyZW50Q3VydmUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjI1LCAwLjk1ICk7CgkJCQkJdmFyIHZlcnRpY2FsQW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCA0NS4wLCAxMDAuMCApOwoJCQkJCXZhciB0d2lzdEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggLTQ1LjAsIDQ1LjAgKTsKCQkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yT25QYXJlbnRDdXJ2ZSwgZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICksIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJCXZhciB0aGlja25lc3MgPSBzdGFydFRyYW5zZm9ybS5wYXJlbnRUaGlja25lc3MgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjMsIDAuNSApOwoJCgkJCQkJdmFyIGxlbmd0aCA9IHRoaWNrbmVzcyAqIDIwLjA7CgkJCQkJaWYoIGxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkKCQkJCQl7CgkJCQkJCWdlbmVyYXRvci5Ud2lncy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50Q3lsaW5kZXIoIHBhcmVudCwgbWF0ZXJpYWwsIHN0YXJ0VHJhbnNmb3JtLnRyYW5zZm9ybSwgbGVuZ3RoLCB0aGlja25lc3MsIG51bGwsIDEwLjAsIDE0LjAsIHRoaWNrbmVzcyAqIDAuNSwgMC4yICkgKTsKCQoJCQkJCQlhZGRlZCsrOwoJCQkJCQlpZiggYWRkZWQgPj0gY291bnQgKQoJCQkJCQkJYnJlYWs7CgkJCQkJfQoJCQkJfQoJCQl9CgkJfQoJCWJyZWFrOwoJCgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5MZWFmOgoJCWlmKCBnZW5lcmF0b3IuQnJhbmNoZXMuQ291bnQgIT0gMCB8fCBnZW5lcmF0b3IuVHdpZ3MuQ291bnQgIT0gMCApCgkJewoJCQl2YXIgc2VsZWN0b3IgPSBuZXcgUGxhbnRHZW5lcmF0b3IuU2VsZWN0b3JCeVByb2JhYmlsaXR5KCBnZW5lcmF0b3IgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5CcmFuY2hlcyApOwoJCQlzZWxlY3Rvci5BZGRFbGVtZW50cyggZ2VuZXJhdG9yLlR3aWdzICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuVHJ1bmtzICk7CgkKCQkJLy8hISEh0YDQsNGB0L_RgNC10LTQtdC70Y_RgtGMINCyINC30LDQstC40YHQuNC80L7RgdGC0Lgg0L7RgiDQtNC70LjQvdGLCgkKCQkJLy8hISEh0YDQsNCy0L3QvtC80LXRgNC90L4g0YDQsNGB0L_RgNC10LTQtdC70Y_RgtGMLiDQsdGA0LDQvdGH0LgsINCy0LXRgtC60Lgg0YLQvtC20LUKCQoJCQkvLyEhISHQv9GA0LjQvNC10L3Rj9GC0YwgTGVhZkNvdW50CgkKCQkJdmFyIGNvdW50ID0gNTAwMDsvLzIwMDA7Ly8gMTUwMDsvLyAyMDAwOy8vIDI1MDA7CgkJCWlmKCBnZW5lcmF0b3IuQWdlIDwgZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UgKQoJCQkJY291bnQgPSAoaW50KSggKGRvdWJsZSljb3VudCAqIE1hdGguUG93KCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UsIDIuNSApICk7CgkKCQkJLy9pZiggTE9EID49IDIgKQoJCQkvLwljb3VudCAvPSAyOwoJCQkvL2lmKCBMT0QgPj0gMyApCgkJCS8vCWNvdW50IC89IDY7CgkKCQkJZm9yKCBpbnQgbiA9IDA7IG4gPCBjb3VudDsgbisrICkKCQkJewoJCQkJdmFyIHBhcmVudCA9IHNlbGVjdG9yLkdldCgpOwoJCgkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJyYW5jaFdpdGhMZWF2ZXMgKTsKCQoJCQkJLy8hISEh0L_QvtCy0L7RgNCw0YfQuNCy0LDRgtGMINC_0L4g0LPQvtGA0LjQt9C+0L3RgtCw0LvQuD8KCQoJCQkJLy8hISEh0YDQsNGB0L_RgNC10LTQtdC70LXQvdC40LUKCQoJCQkJLy8hISEh0L7RgNC40LXQvdGC0LDRhtC40Y8g0L7RgtC90L7RgdC40YLQtdC70YzQvdC+INGB0L7Qu9C90YbQsC_QstC10YDRhdCwCgkKCQkJCXZhciB2ZXJ0aWNhbEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggOTAuMCAtIDQ1LjAsIDkwLjAgKyA0NS4wICk7CgkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NS4wLCA0NS4wICk7CgkKCQkJCXZhciBzdGFydFRyYW5zZm9ybSA9IHBhcmVudC5DdXJ2ZS5HZXRUcmFuc2Zvcm1PblN1cmZhY2UoIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMywgMC45NyApLCBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAxLjAgKSwgdmVydGljYWxBbmdsZSwgdHdpc3RBbmdsZSApOwoJCgkJCQkvLyEhISF0aWx0QW5nbGUKCQoJCQkJdmFyIGxlbmd0aCA9IDEuMDsKCQkJCWlmKCBtYXRlcmlhbCAhPSBudWxsICkKCQkJCQlsZW5ndGggPSBtYXRlcmlhbC5SZWFsTGVuZ3RoICogZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMC44LCAxLjIgKTsKCgkJCQkvL2lmKCBMT0QgPj0gMiApCgkJCQkvLwlsZW5ndGggKj0gMS41OwoJCQkJLy9pZiggTE9EID49IDMgKQoJCQkJLy8JbGVuZ3RoICo9IDEuNTsKCQoJCQkJZ2VuZXJhdG9yLkxlYXZlcy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50UmliYm9uKCBwYXJlbnQsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybS50cmFuc2Zvcm0sIGxlbmd0aCwgMCwgdHJ1ZSwgNDUgKSApOwoJCQl9CgkKCQkJLy8hISEhCgkJCS8v0L_RgNC+0LLQtdGA0Y_RgtGMINC80LDRgtC10YDQuNCw0Lsg0LXRgdGC0Ywg0LvQuCDQstC10YLQutCwLgoJCQkvL9C10YHQu9C4INC90LXRgiDRgtC+0LPQtNCwINC00LXQu9Cw0YLRjCDQu9C40YHRgtGM0Y8uINC10YHRgtGMINC10YHRgtGMINGC0L7Qs9C00LAg0LLRgdGOINCy0LXRgtC60YMg0YDQuNCx0LHQvtC90L7QvC4KCQoJCX0KCQlicmVhazsKCX0KCQojZW5kaWYKfQo=")]
public class DynamicClassF74AE31E88C437A76BA9820D5755C3EC398F2003726C1A832232DF7DCD2432E7
{
    public NeoAxis.CSharpScript Owner;
    public void _GenerateStage(NeoAxis.PlantType sender, NeoAxis.PlantGenerator generator, NeoAxis.PlantGenerator.ElementTypeEnum stage)
    {
        //This script is intended to specify the data for a generator.
        //ideas:
        //береза:
        //бранч в 7 раз тоньше чем родитель
        //ствол, бранчи, ветки ровные
        //мало бранчей, веток тоже
        //сверху больше растительности
        //дуб:
        //больше изогнутость чем береза
        //внизу тоже много растительности
        //ель:
        //веток нет, одни бранчи?
        //высота
        //толщина сплайной
#if !DEPLOY
        var minBranchTwigLength = generator.Height / 50.0;
        switch (stage)
        {
            case PlantGenerator.ElementTypeEnum.Trunk:
            {
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var startTransform = new Transform(Vector3.Zero, Quaternion.LookAt(Vector3.ZAxis, Vector3.XAxis));
                var length = generator.Height * generator.Randomizer.Next(0.8, 1.2);
                var thickness = length / 20.0;
                generator.Trunks.Add(generator.CreateElementCylinder(null, material, startTransform, length, thickness, null, 15.0, 24.0, thickness * 0.2, 0));
            }

                break;
            case PlantGenerator.ElementTypeEnum.Branch:
            {
                var count = 60; //40;// 40;
                if (generator.Age < generator.PlantType.MatureAge)
                    count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                var parent = generator.Trunks[0];
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var collisionChecker = new PlantGenerator.CollisionChecker();
                var added = 0;
                for (int n = 0; n < count * 10; n++)
                {
                    var timeFactor = generator.Randomizer.Next(0.1 /* 0.2*/, 0.95);
                    var twistFactor = generator.Randomizer.Next(1.0);
                    if (!collisionChecker.Intersects(timeFactor, twistFactor))
                    {
                        var verticalAngle = generator.Randomizer.Next(45, 100);
                        var twistAngle = generator.Randomizer.Next(-45, 45);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactor, twistFactor, verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Branches.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.05)); // 0.1 ) );
                            collisionChecker.Add(timeFactor, twistFactor);
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Twig:
            {
                var selector = new PlantGenerator.SelectorByProbability(generator);
                selector.AddElements(generator.Branches.Where(b => b.Length >= minBranchTwigLength));
                if (selector.Count != 0)
                {
                    var count = 400; //300;// 400;// 200;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                    var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                    var added = 0;
                    for (int n = 0; n < count * 10; n++)
                    {
                        var parent = selector.Get();
                        var timeFactorOnParentCurve = generator.Randomizer.Next(0.25, 0.95);
                        var verticalAngle = generator.Randomizer.Next(45.0, 100.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactorOnParentCurve, generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Twigs.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.2));
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Leaf:
                if (generator.Branches.Count != 0 || generator.Twigs.Count != 0)
                {
                    var selector = new PlantGenerator.SelectorByProbability(generator);
                    selector.AddElements(generator.Branches);
                    selector.AddElements(generator.Twigs);
                    selector.AddElements(generator.Trunks);
                    //!!!!распределять в зависимости от длины
                    //!!!!равномерно распределять. бранчи, ветки тоже
                    //!!!!применять LeafCount
                    var count = 5000; //2000;// 1500;// 2000;// 2500;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2.5));
                    //if( LOD >= 2 )
                    //	count /= 2;
                    //if( LOD >= 3 )
                    //	count /= 6;
                    for (int n = 0; n < count; n++)
                    {
                        var parent = selector.Get();
                        var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.BranchWithLeaves);
                        //!!!!поворачивать по горизонтали?
                        //!!!!распределение
                        //!!!!ориентация относительно солнца/верха
                        var verticalAngle = generator.Randomizer.Next(90.0 - 45.0, 90.0 + 45.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(generator.Randomizer.Next(0.3, 0.97), generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        //!!!!tiltAngle
                        var length = 1.0;
                        if (material != null)
                            length = material.RealLength * generator.Randomizer.Next(0.8, 1.2);
                        //if( LOD >= 2 )
                        //	length *= 1.5;
                        //if( LOD >= 3 )
                        //	length *= 1.5;
                        generator.Leaves.Add(generator.CreateElementRibbon(parent, material, startTransform.transform, length, 0, true, 45));
                    }
                //!!!!
                //проверять материал есть ли ветка.
                //если нет тогда делать листья. есть есть тогда всю ветку риббоном.
                }

                break;
        }
#endif
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX0dlbmVyYXRlU3RhZ2UoTmVvQXhpcy5QbGFudFR5cGUgc2VuZGVyLCBOZW9BeGlzLlBsYW50R2VuZXJhdG9yIGdlbmVyYXRvciwgTmVvQXhpcy5QbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0gc3RhZ2UpCnsKCS8vVGhpcyBzY3JpcHQgaXMgaW50ZW5kZWQgdG8gc3BlY2lmeSB0aGUgZGF0YSBmb3IgYSBnZW5lcmF0b3IuCgkKCQoJLy9pZGVhczoKCQoJLy_QsdC10YDQtdC30LA6CgkvL9Cx0YDQsNC90Ycg0LIgNyDRgNCw0Lcg0YLQvtC90YzRiNC1INGH0LXQvCDRgNC+0LTQuNGC0LXQu9GMCgkvL9GB0YLQstC+0LssINCx0YDQsNC90YfQuCwg0LLQtdGC0LrQuCDRgNC+0LLQvdGL0LUKCS8v0LzQsNC70L4g0LHRgNCw0L3Rh9C10LksINCy0LXRgtC+0Log0YLQvtC20LUKCS8v0YHQstC10YDRhdGDINCx0L7Qu9GM0YjQtSDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LTRg9CxOgoJLy_QsdC+0LvRjNGI0LUg0LjQt9C+0LPQvdGD0YLQvtGB0YLRjCDRh9C10Lwg0LHQtdGA0LXQt9CwCgkvL9Cy0L3QuNC30YMg0YLQvtC20LUg0LzQvdC+0LPQviDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LXQu9GMOgoJLy_QstC10YLQvtC6INC90LXRgiwg0L7QtNC90Lgg0LHRgNCw0L3Rh9C4PwoJCgkvL9Cy0YvRgdC+0YLQsAoJLy_RgtC+0LvRidC40L3QsCDRgdC_0LvQsNC50L3QvtC5CgoKI2lmICFERVBMT1kKCXZhciBtaW5CcmFuY2hUd2lnTGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAvIDUwLjA7CgkJCglzd2l0Y2goIHN0YWdlICkKCXsKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlRydW5rOgoJCXsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkJCXZhciBzdGFydFRyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oIFZlY3RvcjMuWmVybywgUXVhdGVybmlvbi5Mb29rQXQoIFZlY3RvcjMuWkF4aXMsIFZlY3RvcjMuWEF4aXMgKSApOwoJCQl2YXIgbGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCXZhciB0aGlja25lc3MgPSBsZW5ndGggLyAyMC4wOwoJCgkJCWdlbmVyYXRvci5UcnVua3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBudWxsLCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCBudWxsLCAxNS4wLCAyNC4wLCB0aGlja25lc3MgKiAwLjIsIDAgKSApOwoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uQnJhbmNoOgoJCXsKCQkJdmFyIGNvdW50ID0gNjA7Ly80MDsvLyA0MDsKCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQljb3VudCA9IChpbnQpKCAoZG91YmxlKWNvdW50ICogTWF0aC5Qb3coIGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSwgMiApICk7CgkKCQkJdmFyIHBhcmVudCA9IGdlbmVyYXRvci5UcnVua3NbIDAgXTsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkKCQkJdmFyIGNvbGxpc2lvbkNoZWNrZXIgPSBuZXcgUGxhbnRHZW5lcmF0b3IuQ29sbGlzaW9uQ2hlY2tlcigpOwoJCgkJCXZhciBhZGRlZCA9IDA7CgkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJewoJCQkJdmFyIHRpbWVGYWN0b3IgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjEvKiAwLjIqLywgMC45NSApOwoJCQkJdmFyIHR3aXN0RmFjdG9yID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICk7CgkKCQkJCWlmKCAhY29sbGlzaW9uQ2hlY2tlci5JbnRlcnNlY3RzKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciApICkKCQkJCXsKCQkJCQl2YXIgdmVydGljYWxBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDQ1LCAxMDAgKTsKCQkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NSwgNDUgKTsKCQkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciwgdmVydGljYWxBbmdsZSwgdHdpc3RBbmdsZSApOwoJCgkJCQkJdmFyIHRoaWNrbmVzcyA9IHN0YXJ0VHJhbnNmb3JtLnBhcmVudFRoaWNrbmVzcyAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMywgMC41ICk7CgkKCQkJCQl2YXIgbGVuZ3RoID0gdGhpY2tuZXNzICogMjAuMDsKCQkJCQlpZiggbGVuZ3RoID49IG1pbkJyYW5jaFR3aWdMZW5ndGggKQoJCQkJCXsKCQkJCQkJZ2VuZXJhdG9yLkJyYW5jaGVzLkFkZCggZ2VuZXJhdG9yLkNyZWF0ZUVsZW1lbnRDeWxpbmRlciggcGFyZW50LCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0udHJhbnNmb3JtLCBsZW5ndGgsIHRoaWNrbmVzcywgbnVsbCwgMTAuMCwgMTQuMCwgdGhpY2tuZXNzICogMC41LCAwLjA1ICkgKTsvLyAwLjEgKSApOwoJCgkJCQkJCWNvbGxpc2lvbkNoZWNrZXIuQWRkKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciApOwoJCgkJCQkJCWFkZGVkKys7CgkJCQkJCWlmKCBhZGRlZCA+PSBjb3VudCApCgkJCQkJCQlicmVhazsKCQkJCQl9CgkJCQl9CgkJCX0KCQl9CgkJYnJlYWs7CgkKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlR3aWc6CgkJewoJCQl2YXIgc2VsZWN0b3IgPSBuZXcgUGxhbnRHZW5lcmF0b3IuU2VsZWN0b3JCeVByb2JhYmlsaXR5KCBnZW5lcmF0b3IgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5CcmFuY2hlcy5XaGVyZSggYiA9PiBiLkxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkgKTsKCQoJCQlpZiggc2VsZWN0b3IuQ291bnQgIT0gMCApCgkJCXsKCQkJCXZhciBjb3VudCA9IDQwMDsvLzMwMDsvLyA0MDA7Ly8gMjAwOwoJCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQkJY291bnQgPSAoaW50KSggKGRvdWJsZSljb3VudCAqIE1hdGguUG93KCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UsIDIgKSApOwoJCgkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJhcmsgKTsKCQoJCQkJdmFyIGFkZGVkID0gMDsKCQkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJCXsKCQkJCQl2YXIgcGFyZW50ID0gc2VsZWN0b3IuR2V0KCk7CgkKCQkJCQl2YXIgdGltZUZhY3Rvck9uUGFyZW50Q3VydmUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjI1LCAwLjk1ICk7CgkJCQkJdmFyIHZlcnRpY2FsQW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCA0NS4wLCAxMDAuMCApOwoJCQkJCXZhciB0d2lzdEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggLTQ1LjAsIDQ1LjAgKTsKCQkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yT25QYXJlbnRDdXJ2ZSwgZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICksIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJCXZhciB0aGlja25lc3MgPSBzdGFydFRyYW5zZm9ybS5wYXJlbnRUaGlja25lc3MgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjMsIDAuNSApOwoKCQkJCQl2YXIgbGVuZ3RoID0gdGhpY2tuZXNzICogMzAuMDsvLzIwLjA7CgkJCQkJaWYoIGxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkKCQkJCQl7CgkJCQkJCWdlbmVyYXRvci5Ud2lncy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50Q3lsaW5kZXIoIHBhcmVudCwgbWF0ZXJpYWwsIHN0YXJ0VHJhbnNmb3JtLnRyYW5zZm9ybSwgbGVuZ3RoLCB0aGlja25lc3MsIG51bGwsIDEwLjAsIDE0LjAsIHRoaWNrbmVzcyAqIDAuNSwgMC4yICkgKTsKCQoJCQkJCQlhZGRlZCsrOwoJCQkJCQlpZiggYWRkZWQgPj0gY291bnQgKQoJCQkJCQkJYnJlYWs7CgkJCQkJfQoJCQkJfQoJCQl9CgkJfQoJCWJyZWFrOwoJCgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5MZWFmOgoJCWlmKCBnZW5lcmF0b3IuQnJhbmNoZXMuQ291bnQgIT0gMCB8fCBnZW5lcmF0b3IuVHdpZ3MuQ291bnQgIT0gMCApCgkJewoJCQl2YXIgc2VsZWN0b3IgPSBuZXcgUGxhbnRHZW5lcmF0b3IuU2VsZWN0b3JCeVByb2JhYmlsaXR5KCBnZW5lcmF0b3IgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5CcmFuY2hlcyApOwoJCQlzZWxlY3Rvci5BZGRFbGVtZW50cyggZ2VuZXJhdG9yLlR3aWdzICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuVHJ1bmtzICk7CgkKCQkJLy8hISEh0YDQsNGB0L_RgNC10LTQtdC70Y_RgtGMINCyINC30LDQstC40YHQuNC80L7RgdGC0Lgg0L7RgiDQtNC70LjQvdGLCgkKCQkJLy8hISEh0YDQsNCy0L3QvtC80LXRgNC90L4g0YDQsNGB0L_RgNC10LTQtdC70Y_RgtGMLiDQsdGA0LDQvdGH0LgsINCy0LXRgtC60Lgg0YLQvtC20LUKCQoJCQkvLyEhISHQv9GA0LjQvNC10L3Rj9GC0YwgTGVhZkNvdW50CgkKCQkJdmFyIGNvdW50ID0gNTAwMDsvLzIwMDA7Ly8gMTUwMDsvLyAyMDAwOy8vIDI1MDA7CgkJCWlmKCBnZW5lcmF0b3IuQWdlIDwgZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UgKQoJCQkJY291bnQgPSAoaW50KSggKGRvdWJsZSljb3VudCAqIE1hdGguUG93KCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UsIDIuNSApICk7CgkKCQkJLy9pZiggTE9EID49IDIgKQoJCQkvLwljb3VudCAvPSAyOwoJCQkvL2lmKCBMT0QgPj0gMyApCgkJCS8vCWNvdW50IC89IDY7CgkKCQkJZm9yKCBpbnQgbiA9IDA7IG4gPCBjb3VudDsgbisrICkKCQkJewoJCQkJdmFyIHBhcmVudCA9IHNlbGVjdG9yLkdldCgpOwoJCgkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJyYW5jaFdpdGhMZWF2ZXMgKTsKCQoJCQkJLy8hISEh0L_QvtCy0L7RgNCw0YfQuNCy0LDRgtGMINC_0L4g0LPQvtGA0LjQt9C+0L3RgtCw0LvQuD8KCQoJCQkJLy8hISEh0YDQsNGB0L_RgNC10LTQtdC70LXQvdC40LUKCQoJCQkJLy8hISEh0L7RgNC40LXQvdGC0LDRhtC40Y8g0L7RgtC90L7RgdC40YLQtdC70YzQvdC+INGB0L7Qu9C90YbQsC_QstC10YDRhdCwCgkKCQkJCXZhciB2ZXJ0aWNhbEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggOTAuMCAtIDQ1LjAsIDkwLjAgKyA0NS4wICk7CgkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NS4wLCA0NS4wICk7CgkKCQkJCXZhciBzdGFydFRyYW5zZm9ybSA9IHBhcmVudC5DdXJ2ZS5HZXRUcmFuc2Zvcm1PblN1cmZhY2UoIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMywgMC45NyApLCBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAxLjAgKSwgdmVydGljYWxBbmdsZSwgdHdpc3RBbmdsZSApOwoJCgkJCQkvLyEhISF0aWx0QW5nbGUKCQoJCQkJdmFyIGxlbmd0aCA9IDEuMDsKCQkJCWlmKCBtYXRlcmlhbCAhPSBudWxsICkKCQkJCQlsZW5ndGggPSBtYXRlcmlhbC5SZWFsTGVuZ3RoICogZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMC44LCAxLjIgKTsKCgkJCQkvL2lmKCBMT0QgPj0gMiApCgkJCQkvLwlsZW5ndGggKj0gMS41OwoJCQkJLy9pZiggTE9EID49IDMgKQoJCQkJLy8JbGVuZ3RoICo9IDEuNTsKCQoJCQkJZ2VuZXJhdG9yLkxlYXZlcy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50UmliYm9uKCBwYXJlbnQsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybS50cmFuc2Zvcm0sIGxlbmd0aCwgMCwgdHJ1ZSwgNDUgKSApOwoJCQl9CgkKCQkJLy8hISEhCgkJCS8v0L_RgNC+0LLQtdGA0Y_RgtGMINC80LDRgtC10YDQuNCw0Lsg0LXRgdGC0Ywg0LvQuCDQstC10YLQutCwLgoJCQkvL9C10YHQu9C4INC90LXRgiDRgtC+0LPQtNCwINC00LXQu9Cw0YLRjCDQu9C40YHRgtGM0Y8uINC10YHRgtGMINC10YHRgtGMINGC0L7Qs9C00LAg0LLRgdGOINCy0LXRgtC60YMg0YDQuNCx0LHQvtC90L7QvC4KCQoJCX0KCQlicmVhazsKCX0KCQojZW5kaWYKfQo=")]
public class DynamicClassE4FEDE24B877969E6FCFFE8C84B8ECA82146DCBDA56F68CA6C0320DA66CB9F38
{
    public NeoAxis.CSharpScript Owner;
    public void _GenerateStage(NeoAxis.PlantType sender, NeoAxis.PlantGenerator generator, NeoAxis.PlantGenerator.ElementTypeEnum stage)
    {
        //This script is intended to specify the data for a generator.
        //ideas:
        //береза:
        //бранч в 7 раз тоньше чем родитель
        //ствол, бранчи, ветки ровные
        //мало бранчей, веток тоже
        //сверху больше растительности
        //дуб:
        //больше изогнутость чем береза
        //внизу тоже много растительности
        //ель:
        //веток нет, одни бранчи?
        //высота
        //толщина сплайной
#if !DEPLOY
        var minBranchTwigLength = generator.Height / 50.0;
        switch (stage)
        {
            case PlantGenerator.ElementTypeEnum.Trunk:
            {
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var startTransform = new Transform(Vector3.Zero, Quaternion.LookAt(Vector3.ZAxis, Vector3.XAxis));
                var length = generator.Height * generator.Randomizer.Next(0.8, 1.2);
                var thickness = length / 20.0;
                generator.Trunks.Add(generator.CreateElementCylinder(null, material, startTransform, length, thickness, null, 15.0, 24.0, thickness * 0.2, 0));
            }

                break;
            case PlantGenerator.ElementTypeEnum.Branch:
            {
                var count = 60; //40;// 40;
                if (generator.Age < generator.PlantType.MatureAge)
                    count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                var parent = generator.Trunks[0];
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var collisionChecker = new PlantGenerator.CollisionChecker();
                var added = 0;
                for (int n = 0; n < count * 10; n++)
                {
                    var timeFactor = generator.Randomizer.Next(0.1 /* 0.2*/, 0.95);
                    var twistFactor = generator.Randomizer.Next(1.0);
                    if (!collisionChecker.Intersects(timeFactor, twistFactor))
                    {
                        var verticalAngle = generator.Randomizer.Next(45, 100);
                        var twistAngle = generator.Randomizer.Next(-45, 45);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactor, twistFactor, verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Branches.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.05)); // 0.1 ) );
                            collisionChecker.Add(timeFactor, twistFactor);
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Twig:
            {
                var selector = new PlantGenerator.SelectorByProbability(generator);
                selector.AddElements(generator.Branches.Where(b => b.Length >= minBranchTwigLength));
                if (selector.Count != 0)
                {
                    var count = 400; //300;// 400;// 200;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                    var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                    var added = 0;
                    for (int n = 0; n < count * 10; n++)
                    {
                        var parent = selector.Get();
                        var timeFactorOnParentCurve = generator.Randomizer.Next(0.25, 0.95);
                        var verticalAngle = generator.Randomizer.Next(45.0, 100.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactorOnParentCurve, generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 30.0; //20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Twigs.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.2));
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Leaf:
                if (generator.Branches.Count != 0 || generator.Twigs.Count != 0)
                {
                    var selector = new PlantGenerator.SelectorByProbability(generator);
                    selector.AddElements(generator.Branches);
                    selector.AddElements(generator.Twigs);
                    selector.AddElements(generator.Trunks);
                    //!!!!распределять в зависимости от длины
                    //!!!!равномерно распределять. бранчи, ветки тоже
                    //!!!!применять LeafCount
                    var count = 5000; //2000;// 1500;// 2000;// 2500;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2.5));
                    //if( LOD >= 2 )
                    //	count /= 2;
                    //if( LOD >= 3 )
                    //	count /= 6;
                    for (int n = 0; n < count; n++)
                    {
                        var parent = selector.Get();
                        var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.BranchWithLeaves);
                        //!!!!поворачивать по горизонтали?
                        //!!!!распределение
                        //!!!!ориентация относительно солнца/верха
                        var verticalAngle = generator.Randomizer.Next(90.0 - 45.0, 90.0 + 45.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(generator.Randomizer.Next(0.3, 0.97), generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        //!!!!tiltAngle
                        var length = 1.0;
                        if (material != null)
                            length = material.RealLength * generator.Randomizer.Next(0.8, 1.2);
                        //if( LOD >= 2 )
                        //	length *= 1.5;
                        //if( LOD >= 3 )
                        //	length *= 1.5;
                        generator.Leaves.Add(generator.CreateElementRibbon(parent, material, startTransform.transform, length, 0, true, 45));
                    }
                //!!!!
                //проверять материал есть ли ветка.
                //если нет тогда делать листья. есть есть тогда всю ветку риббоном.
                }

                break;
        }
#endif
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX0dlbmVyYXRlU3RhZ2UoTmVvQXhpcy5QbGFudFR5cGUgc2VuZGVyLCBOZW9BeGlzLlBsYW50R2VuZXJhdG9yIGdlbmVyYXRvciwgTmVvQXhpcy5QbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0gc3RhZ2UpCnsKCS8vVGhpcyBzY3JpcHQgaXMgaW50ZW5kZWQgdG8gc3BlY2lmeSB0aGUgZGF0YSBmb3IgYSBnZW5lcmF0b3IuCgkKCQoJLy9pZGVhczoKCQoJLy_QsdC10YDQtdC30LA6CgkvL9Cx0YDQsNC90Ycg0LIgNyDRgNCw0Lcg0YLQvtC90YzRiNC1INGH0LXQvCDRgNC+0LTQuNGC0LXQu9GMCgkvL9GB0YLQstC+0LssINCx0YDQsNC90YfQuCwg0LLQtdGC0LrQuCDRgNC+0LLQvdGL0LUKCS8v0LzQsNC70L4g0LHRgNCw0L3Rh9C10LksINCy0LXRgtC+0Log0YLQvtC20LUKCS8v0YHQstC10YDRhdGDINCx0L7Qu9GM0YjQtSDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LTRg9CxOgoJLy_QsdC+0LvRjNGI0LUg0LjQt9C+0LPQvdGD0YLQvtGB0YLRjCDRh9C10Lwg0LHQtdGA0LXQt9CwCgkvL9Cy0L3QuNC30YMg0YLQvtC20LUg0LzQvdC+0LPQviDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LXQu9GMOgoJLy_QstC10YLQvtC6INC90LXRgiwg0L7QtNC90Lgg0LHRgNCw0L3Rh9C4PwoJCgkvL9Cy0YvRgdC+0YLQsAoJLy_RgtC+0LvRidC40L3QsCDRgdC_0LvQsNC50L3QvtC5CgoKI2lmICFERVBMT1kKCXZhciBtaW5CcmFuY2hUd2lnTGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAvIDUwLjA7CgkJCglzd2l0Y2goIHN0YWdlICkKCXsKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlRydW5rOgoJCXsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkJCXZhciBzdGFydFRyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oIFZlY3RvcjMuWmVybywgUXVhdGVybmlvbi5Mb29rQXQoIFZlY3RvcjMuWkF4aXMsIFZlY3RvcjMuWEF4aXMgKSApOwoJCQl2YXIgbGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCXZhciB0aGlja25lc3MgPSBsZW5ndGggLyAyMC4wOwoJCgkJCWdlbmVyYXRvci5UcnVua3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBudWxsLCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCBudWxsLCAxNS4wLCAyNC4wLCB0aGlja25lc3MgKiAwLjIsIDAgKSApOwoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uQnJhbmNoOgoJCXsKCQkJdmFyIGNvdW50ID0gNjA7Ly80MDsvLyA0MDsKCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQljb3VudCA9IChpbnQpKCAoZG91YmxlKWNvdW50ICogTWF0aC5Qb3coIGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSwgMiApICk7CgkKCQkJdmFyIHBhcmVudCA9IGdlbmVyYXRvci5UcnVua3NbIDAgXTsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkKCQkJdmFyIGNvbGxpc2lvbkNoZWNrZXIgPSBuZXcgUGxhbnRHZW5lcmF0b3IuQ29sbGlzaW9uQ2hlY2tlcigpOwoJCgkJCXZhciBhZGRlZCA9IDA7CgkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJewoJCQkJdmFyIHRpbWVGYWN0b3IgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjEvKiAwLjIqLywgMC45NSApOwoJCQkJdmFyIHR3aXN0RmFjdG9yID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICk7CgkKCQkJCWlmKCAhY29sbGlzaW9uQ2hlY2tlci5JbnRlcnNlY3RzKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciApICkKCQkJCXsKCQkJCQl2YXIgdmVydGljYWxBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDQ1LCAxMDAgKTsKCQkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NSwgNDUgKTsKCQkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciwgdmVydGljYWxBbmdsZSwgdHdpc3RBbmdsZSApOwoJCgkJCQkJdmFyIHRoaWNrbmVzcyA9IHN0YXJ0VHJhbnNmb3JtLnBhcmVudFRoaWNrbmVzcyAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMywgMC41ICk7CgkKCQkJCQl2YXIgbGVuZ3RoID0gdGhpY2tuZXNzICogMjAuMDsKCQkJCQlpZiggbGVuZ3RoID49IG1pbkJyYW5jaFR3aWdMZW5ndGggKQoJCQkJCXsKCQkJCQkJZ2VuZXJhdG9yLkJyYW5jaGVzLkFkZCggZ2VuZXJhdG9yLkNyZWF0ZUVsZW1lbnRDeWxpbmRlciggcGFyZW50LCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0udHJhbnNmb3JtLCBsZW5ndGgsIHRoaWNrbmVzcywgbnVsbCwgMTAuMCwgMTQuMCwgdGhpY2tuZXNzICogMC41LCAwLjA1ICkgKTsvLyAwLjEgKSApOwoJCgkJCQkJCWNvbGxpc2lvbkNoZWNrZXIuQWRkKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciApOwoJCgkJCQkJCWFkZGVkKys7CgkJCQkJCWlmKCBhZGRlZCA+PSBjb3VudCApCgkJCQkJCQlicmVhazsKCQkJCQl9CgkJCQl9CgkJCX0KCQl9CgkJYnJlYWs7CgkKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlR3aWc6CgkJewoJCQl2YXIgc2VsZWN0b3IgPSBuZXcgUGxhbnRHZW5lcmF0b3IuU2VsZWN0b3JCeVByb2JhYmlsaXR5KCBnZW5lcmF0b3IgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5CcmFuY2hlcy5XaGVyZSggYiA9PiBiLkxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkgKTsKCQoJCQlpZiggc2VsZWN0b3IuQ291bnQgIT0gMCApCgkJCXsKCQkJCXZhciBjb3VudCA9IDQwMDsvLzMwMDsvLyA0MDA7Ly8gMjAwOwoJCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQkJY291bnQgPSAoaW50KSggKGRvdWJsZSljb3VudCAqIE1hdGguUG93KCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UsIDIgKSApOwoJCgkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJhcmsgKTsKCQoJCQkJdmFyIGFkZGVkID0gMDsKCQkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJCXsKCQkJCQl2YXIgcGFyZW50ID0gc2VsZWN0b3IuR2V0KCk7CgkKCQkJCQl2YXIgdGltZUZhY3Rvck9uUGFyZW50Q3VydmUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjI1LCAwLjk1ICk7CgkJCQkJdmFyIHZlcnRpY2FsQW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCA0NS4wLCAxMDAuMCApOwoJCQkJCXZhciB0d2lzdEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggLTQ1LjAsIDQ1LjAgKTsKCQkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yT25QYXJlbnRDdXJ2ZSwgZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICksIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJCXZhciB0aGlja25lc3MgPSBzdGFydFRyYW5zZm9ybS5wYXJlbnRUaGlja25lc3MgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjMsIDAuNSApOwoKCQkJCQl2YXIgbGVuZ3RoID0gdGhpY2tuZXNzICogMzAuMDsvLzIwLjA7CgkJCQkJaWYoIGxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkKCQkJCQl7CgkJCQkJCWdlbmVyYXRvci5Ud2lncy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50Q3lsaW5kZXIoIHBhcmVudCwgbWF0ZXJpYWwsIHN0YXJ0VHJhbnNmb3JtLnRyYW5zZm9ybSwgbGVuZ3RoLCB0aGlja25lc3MsIG51bGwsIDEwLjAsIDE0LjAsIHRoaWNrbmVzcyAqIDAuNSwgMC4yICkgKTsKCQoJCQkJCQlhZGRlZCsrOwoJCQkJCQlpZiggYWRkZWQgPj0gY291bnQgKQoJCQkJCQkJYnJlYWs7CgkJCQkJfQoJCQkJfQoJCQl9CgkJfQoJCWJyZWFrOwoJCgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5MZWFmOgoJCWlmKCBnZW5lcmF0b3IuQnJhbmNoZXMuQ291bnQgIT0gMCB8fCBnZW5lcmF0b3IuVHdpZ3MuQ291bnQgIT0gMCApCgkJewoJCQl2YXIgc2VsZWN0b3IgPSBuZXcgUGxhbnRHZW5lcmF0b3IuU2VsZWN0b3JCeVByb2JhYmlsaXR5KCBnZW5lcmF0b3IgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5CcmFuY2hlcyApOwoJCQlzZWxlY3Rvci5BZGRFbGVtZW50cyggZ2VuZXJhdG9yLlR3aWdzICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuVHJ1bmtzICk7CgkKCQkJLy8hISEh0YDQsNGB0L_RgNC10LTQtdC70Y_RgtGMINCyINC30LDQstC40YHQuNC80L7RgdGC0Lgg0L7RgiDQtNC70LjQvdGLCgkKCQkJLy8hISEh0YDQsNCy0L3QvtC80LXRgNC90L4g0YDQsNGB0L_RgNC10LTQtdC70Y_RgtGMLiDQsdGA0LDQvdGH0LgsINCy0LXRgtC60Lgg0YLQvtC20LUKCQoJCQkvLyEhISHQv9GA0LjQvNC10L3Rj9GC0YwgTGVhZkNvdW50CgkKCQkJdmFyIGNvdW50ID0gNDAwMDsvLzIwMDA7Ly8gMTUwMDsvLyAyMDAwOy8vIDI1MDA7CgkJCWlmKCBnZW5lcmF0b3IuQWdlIDwgZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UgKQoJCQkJY291bnQgPSAoaW50KSggKGRvdWJsZSljb3VudCAqIE1hdGguUG93KCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UsIDIuNSApICk7CgkKCQkJLy9pZiggTE9EID49IDIgKQoJCQkvLwljb3VudCAvPSAyOwoJCQkvL2lmKCBMT0QgPj0gMyApCgkJCS8vCWNvdW50IC89IDY7CgkKCQkJZm9yKCBpbnQgbiA9IDA7IG4gPCBjb3VudDsgbisrICkKCQkJewoJCQkJdmFyIHBhcmVudCA9IHNlbGVjdG9yLkdldCgpOwoJCgkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJyYW5jaFdpdGhMZWF2ZXMgKTsKCQoJCQkJLy8hISEh0L_QvtCy0L7RgNCw0YfQuNCy0LDRgtGMINC_0L4g0LPQvtGA0LjQt9C+0L3RgtCw0LvQuD8KCQoJCQkJLy8hISEh0YDQsNGB0L_RgNC10LTQtdC70LXQvdC40LUKCQoJCQkJLy8hISEh0L7RgNC40LXQvdGC0LDRhtC40Y8g0L7RgtC90L7RgdC40YLQtdC70YzQvdC+INGB0L7Qu9C90YbQsC_QstC10YDRhdCwCgkKCQkJCXZhciB2ZXJ0aWNhbEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggOTAuMCAtIDQ1LjAsIDkwLjAgKyA0NS4wICk7CgkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NS4wLCA0NS4wICk7CgkKCQkJCXZhciBzdGFydFRyYW5zZm9ybSA9IHBhcmVudC5DdXJ2ZS5HZXRUcmFuc2Zvcm1PblN1cmZhY2UoIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMywgMC45NyApLCBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAxLjAgKSwgdmVydGljYWxBbmdsZSwgdHdpc3RBbmdsZSApOwoJCgkJCQkvLyEhISF0aWx0QW5nbGUKCQoJCQkJdmFyIGxlbmd0aCA9IDEuMDsKCQkJCWlmKCBtYXRlcmlhbCAhPSBudWxsICkKCQkJCQlsZW5ndGggPSBtYXRlcmlhbC5SZWFsTGVuZ3RoICogZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMC44LCAxLjIgKTsKCgkJCQkvL2lmKCBMT0QgPj0gMiApCgkJCQkvLwlsZW5ndGggKj0gMS41OwoJCQkJLy9pZiggTE9EID49IDMgKQoJCQkJLy8JbGVuZ3RoICo9IDEuNTsKCQoJCQkJZ2VuZXJhdG9yLkxlYXZlcy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50UmliYm9uKCBwYXJlbnQsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybS50cmFuc2Zvcm0sIGxlbmd0aCwgMCwgdHJ1ZSwgNDUgKSApOwoJCQl9CgkKCQkJLy8hISEhCgkJCS8v0L_RgNC+0LLQtdGA0Y_RgtGMINC80LDRgtC10YDQuNCw0Lsg0LXRgdGC0Ywg0LvQuCDQstC10YLQutCwLgoJCQkvL9C10YHQu9C4INC90LXRgiDRgtC+0LPQtNCwINC00LXQu9Cw0YLRjCDQu9C40YHRgtGM0Y8uINC10YHRgtGMINC10YHRgtGMINGC0L7Qs9C00LAg0LLRgdGOINCy0LXRgtC60YMg0YDQuNCx0LHQvtC90L7QvC4KCQoJCX0KCQlicmVhazsKCX0KCQojZW5kaWYKfQo=")]
public class DynamicClass49D41EC3BEBE5C4B6EAFB8BB61E737B4D7814F2EF685E763D2DD7E6DF001A46A
{
    public NeoAxis.CSharpScript Owner;
    public void _GenerateStage(NeoAxis.PlantType sender, NeoAxis.PlantGenerator generator, NeoAxis.PlantGenerator.ElementTypeEnum stage)
    {
        //This script is intended to specify the data for a generator.
        //ideas:
        //береза:
        //бранч в 7 раз тоньше чем родитель
        //ствол, бранчи, ветки ровные
        //мало бранчей, веток тоже
        //сверху больше растительности
        //дуб:
        //больше изогнутость чем береза
        //внизу тоже много растительности
        //ель:
        //веток нет, одни бранчи?
        //высота
        //толщина сплайной
#if !DEPLOY
        var minBranchTwigLength = generator.Height / 50.0;
        switch (stage)
        {
            case PlantGenerator.ElementTypeEnum.Trunk:
            {
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var startTransform = new Transform(Vector3.Zero, Quaternion.LookAt(Vector3.ZAxis, Vector3.XAxis));
                var length = generator.Height * generator.Randomizer.Next(0.8, 1.2);
                var thickness = length / 20.0;
                generator.Trunks.Add(generator.CreateElementCylinder(null, material, startTransform, length, thickness, null, 15.0, 24.0, thickness * 0.2, 0));
            }

                break;
            case PlantGenerator.ElementTypeEnum.Branch:
            {
                var count = 60; //40;// 40;
                if (generator.Age < generator.PlantType.MatureAge)
                    count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                var parent = generator.Trunks[0];
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var collisionChecker = new PlantGenerator.CollisionChecker();
                var added = 0;
                for (int n = 0; n < count * 10; n++)
                {
                    var timeFactor = generator.Randomizer.Next(0.1 /* 0.2*/, 0.95);
                    var twistFactor = generator.Randomizer.Next(1.0);
                    if (!collisionChecker.Intersects(timeFactor, twistFactor))
                    {
                        var verticalAngle = generator.Randomizer.Next(45, 100);
                        var twistAngle = generator.Randomizer.Next(-45, 45);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactor, twistFactor, verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Branches.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.05)); // 0.1 ) );
                            collisionChecker.Add(timeFactor, twistFactor);
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Twig:
            {
                var selector = new PlantGenerator.SelectorByProbability(generator);
                selector.AddElements(generator.Branches.Where(b => b.Length >= minBranchTwigLength));
                if (selector.Count != 0)
                {
                    var count = 400; //300;// 400;// 200;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                    var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                    var added = 0;
                    for (int n = 0; n < count * 10; n++)
                    {
                        var parent = selector.Get();
                        var timeFactorOnParentCurve = generator.Randomizer.Next(0.25, 0.95);
                        var verticalAngle = generator.Randomizer.Next(45.0, 100.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactorOnParentCurve, generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 30.0; //20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Twigs.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.2));
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Leaf:
                if (generator.Branches.Count != 0 || generator.Twigs.Count != 0)
                {
                    var selector = new PlantGenerator.SelectorByProbability(generator);
                    selector.AddElements(generator.Branches);
                    selector.AddElements(generator.Twigs);
                    selector.AddElements(generator.Trunks);
                    //!!!!распределять в зависимости от длины
                    //!!!!равномерно распределять. бранчи, ветки тоже
                    //!!!!применять LeafCount
                    var count = 4000; //2000;// 1500;// 2000;// 2500;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2.5));
                    //if( LOD >= 2 )
                    //	count /= 2;
                    //if( LOD >= 3 )
                    //	count /= 6;
                    for (int n = 0; n < count; n++)
                    {
                        var parent = selector.Get();
                        var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.BranchWithLeaves);
                        //!!!!поворачивать по горизонтали?
                        //!!!!распределение
                        //!!!!ориентация относительно солнца/верха
                        var verticalAngle = generator.Randomizer.Next(90.0 - 45.0, 90.0 + 45.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(generator.Randomizer.Next(0.3, 0.97), generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        //!!!!tiltAngle
                        var length = 1.0;
                        if (material != null)
                            length = material.RealLength * generator.Randomizer.Next(0.8, 1.2);
                        //if( LOD >= 2 )
                        //	length *= 1.5;
                        //if( LOD >= 3 )
                        //	length *= 1.5;
                        generator.Leaves.Add(generator.CreateElementRibbon(parent, material, startTransform.transform, length, 0, true, 45));
                    }
                //!!!!
                //проверять материал есть ли ветка.
                //если нет тогда делать листья. есть есть тогда всю ветку риббоном.
                }

                break;
        }
#endif
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX0dlbmVyYXRlU3RhZ2UoTmVvQXhpcy5QbGFudFR5cGUgc2VuZGVyLCBOZW9BeGlzLlBsYW50R2VuZXJhdG9yIGdlbmVyYXRvciwgTmVvQXhpcy5QbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0gc3RhZ2UpCnsKCS8vVGhpcyBzY3JpcHQgaXMgaW50ZW5kZWQgdG8gc3BlY2lmeSB0aGUgZGF0YSBmb3IgYSBnZW5lcmF0b3IuCgkKCQoJLy9pZGVhczoKCQoJLy_QsdC10YDQtdC30LA6CgkvL9Cx0YDQsNC90Ycg0LIgNyDRgNCw0Lcg0YLQvtC90YzRiNC1INGH0LXQvCDRgNC+0LTQuNGC0LXQu9GMCgkvL9GB0YLQstC+0LssINCx0YDQsNC90YfQuCwg0LLQtdGC0LrQuCDRgNC+0LLQvdGL0LUKCS8v0LzQsNC70L4g0LHRgNCw0L3Rh9C10LksINCy0LXRgtC+0Log0YLQvtC20LUKCS8v0YHQstC10YDRhdGDINCx0L7Qu9GM0YjQtSDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LTRg9CxOgoJLy_QsdC+0LvRjNGI0LUg0LjQt9C+0LPQvdGD0YLQvtGB0YLRjCDRh9C10Lwg0LHQtdGA0LXQt9CwCgkvL9Cy0L3QuNC30YMg0YLQvtC20LUg0LzQvdC+0LPQviDRgNCw0YHRgtC40YLQtdC70YzQvdC+0YHRgtC4CgkKCS8v0LXQu9GMOgoJLy_QstC10YLQvtC6INC90LXRgiwg0L7QtNC90Lgg0LHRgNCw0L3Rh9C4PwoJCgkvL9Cy0YvRgdC+0YLQsAoJLy_RgtC+0LvRidC40L3QsCDRgdC_0LvQsNC50L3QvtC5CgoKI2lmICFERVBMT1kKCXZhciBtaW5CcmFuY2hUd2lnTGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAvIDUwLjA7CgkJCglzd2l0Y2goIHN0YWdlICkKCXsKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlRydW5rOgoJCXsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkJCXZhciBzdGFydFRyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oIFZlY3RvcjMuWmVybywgUXVhdGVybmlvbi5Mb29rQXQoIFZlY3RvcjMuWkF4aXMsIFZlY3RvcjMuWEF4aXMgKSApOwoJCQl2YXIgbGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCXZhciB0aGlja25lc3MgPSBsZW5ndGggLyAyMC4wOwoJCgkJCWdlbmVyYXRvci5UcnVua3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBudWxsLCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCBudWxsLCAxNS4wLCAyNC4wLCB0aGlja25lc3MgKiAwLjIsIDAgKSApOwoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uQnJhbmNoOgoJCXsKCQkJdmFyIGNvdW50ID0gNjA7Ly80MDsvLyA0MDsKCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQljb3VudCA9IChpbnQpKCAoZG91YmxlKWNvdW50ICogTWF0aC5Qb3coIGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSwgMiApICk7CgkKCQkJdmFyIHBhcmVudCA9IGdlbmVyYXRvci5UcnVua3NbIDAgXTsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkKCQkJdmFyIGNvbGxpc2lvbkNoZWNrZXIgPSBuZXcgUGxhbnRHZW5lcmF0b3IuQ29sbGlzaW9uQ2hlY2tlcigpOwoJCgkJCXZhciBhZGRlZCA9IDA7CgkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJewoJCQkJdmFyIHRpbWVGYWN0b3IgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjEvKiAwLjIqLywgMC45NSApOwoJCQkJdmFyIHR3aXN0RmFjdG9yID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICk7CgkKCQkJCWlmKCAhY29sbGlzaW9uQ2hlY2tlci5JbnRlcnNlY3RzKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciApICkKCQkJCXsKCQkJCQl2YXIgdmVydGljYWxBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDQ1LCAxMDAgKTsKCQkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NSwgNDUgKTsKCQkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciwgdmVydGljYWxBbmdsZSwgdHdpc3RBbmdsZSApOwoJCgkJCQkJdmFyIHRoaWNrbmVzcyA9IHN0YXJ0VHJhbnNmb3JtLnBhcmVudFRoaWNrbmVzcyAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMywgMC41ICk7CgkKCQkJCQl2YXIgbGVuZ3RoID0gdGhpY2tuZXNzICogMjAuMDsKCQkJCQlpZiggbGVuZ3RoID49IG1pbkJyYW5jaFR3aWdMZW5ndGggKQoJCQkJCXsKCQkJCQkJZ2VuZXJhdG9yLkJyYW5jaGVzLkFkZCggZ2VuZXJhdG9yLkNyZWF0ZUVsZW1lbnRDeWxpbmRlciggcGFyZW50LCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0udHJhbnNmb3JtLCBsZW5ndGgsIHRoaWNrbmVzcywgbnVsbCwgMTAuMCwgMTQuMCwgdGhpY2tuZXNzICogMC41LCAwLjA1ICkgKTsvLyAwLjEgKSApOwoJCgkJCQkJCWNvbGxpc2lvbkNoZWNrZXIuQWRkKCB0aW1lRmFjdG9yLCB0d2lzdEZhY3RvciApOwoJCgkJCQkJCWFkZGVkKys7CgkJCQkJCWlmKCBhZGRlZCA+PSBjb3VudCApCgkJCQkJCQlicmVhazsKCQkJCQl9CgkJCQl9CgkJCX0KCQl9CgkJYnJlYWs7CgkKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlR3aWc6CgkJewoJCQl2YXIgc2VsZWN0b3IgPSBuZXcgUGxhbnRHZW5lcmF0b3IuU2VsZWN0b3JCeVByb2JhYmlsaXR5KCBnZW5lcmF0b3IgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5CcmFuY2hlcy5XaGVyZSggYiA9PiBiLkxlbmd0aCA+PSBtaW5CcmFuY2hUd2lnTGVuZ3RoICkgKTsKCQoJCQlpZiggc2VsZWN0b3IuQ291bnQgIT0gMCApCgkJCXsKCQkJCXZhciBjb3VudCA9IDQwMDsvLzMwMDsvLyA0MDA7Ly8gMjAwOwoJCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQkJY291bnQgPSAoaW50KSggKGRvdWJsZSljb3VudCAqIE1hdGguUG93KCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UsIDIgKSApOwoJCgkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJhcmsgKTsKCQoJCQkJdmFyIGFkZGVkID0gMDsKCQkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQgKiAxMDsgbisrICkKCQkJCXsKCQkJCQl2YXIgcGFyZW50ID0gc2VsZWN0b3IuR2V0KCk7CgkKCQkJCQl2YXIgdGltZUZhY3Rvck9uUGFyZW50Q3VydmUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjI1LCAwLjk1ICk7CgkJCQkJdmFyIHZlcnRpY2FsQW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCA0NS4wLCAxMDAuMCApOwoJCQkJCXZhciB0d2lzdEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggLTQ1LjAsIDQ1LjAgKTsKCQkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yT25QYXJlbnRDdXJ2ZSwgZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICksIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJCXZhciB0aGlja25lc3MgPSBzdGFydFRyYW5zZm9ybS5wYXJlbnRUaGlja25lc3MgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjMsIDAuNSApOwoKCQkJCQl2YXIgbGVuZ3RoID0gdGhpY2tuZXNzICogMjUuMDsgLy8yMC4wOwoJCQkJCWlmKCBsZW5ndGggPj0gbWluQnJhbmNoVHdpZ0xlbmd0aCApCgkJCQkJewoJCQkJCQlnZW5lcmF0b3IuVHdpZ3MuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBwYXJlbnQsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybS50cmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCBudWxsLCAxMC4wLCAxNC4wLCB0aGlja25lc3MgKiAwLjUsIDAuMiApICk7CgkKCQkJCQkJYWRkZWQrKzsKCQkJCQkJaWYoIGFkZGVkID49IGNvdW50ICkKCQkJCQkJCWJyZWFrOwoJCQkJCX0KCQkJCX0KCQkJfQoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uTGVhZjoKCQlpZiggZ2VuZXJhdG9yLkJyYW5jaGVzLkNvdW50ICE9IDAgfHwgZ2VuZXJhdG9yLlR3aWdzLkNvdW50ICE9IDAgKQoJCXsKCQkJdmFyIHNlbGVjdG9yID0gbmV3IFBsYW50R2VuZXJhdG9yLlNlbGVjdG9yQnlQcm9iYWJpbGl0eSggZ2VuZXJhdG9yICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuQnJhbmNoZXMgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5Ud2lncyApOwoJCQlzZWxlY3Rvci5BZGRFbGVtZW50cyggZ2VuZXJhdG9yLlRydW5rcyApOwoJCgkJCS8vISEhIdGA0LDRgdC_0YDQtdC00LXQu9GP0YLRjCDQsiDQt9Cw0LLQuNGB0LjQvNC+0YHRgtC4INC+0YIg0LTQu9C40L3RiwoJCgkJCS8vISEhIdGA0LDQstC90L7QvNC10YDQvdC+INGA0LDRgdC_0YDQtdC00LXQu9GP0YLRjC4g0LHRgNCw0L3Rh9C4LCDQstC10YLQutC4INGC0L7QttC1CgkKCQkJLy8hISEh0L_RgNC40LzQtdC90Y_RgtGMIExlYWZDb3VudAoJCgkJCXZhciBjb3VudCA9IDQwMDA7Ly8yMDAwOy8vIDE1MDA7Ly8gMjAwMDsvLyAyNTAwOwoJCQlpZiggZ2VuZXJhdG9yLkFnZSA8IGdlbmVyYXRvci5QbGFudFR5cGUuTWF0dXJlQWdlICkKCQkJCWNvdW50ID0gKGludCkoIChkb3VibGUpY291bnQgKiBNYXRoLlBvdyggZ2VuZXJhdG9yLkFnZSAvIGdlbmVyYXRvci5QbGFudFR5cGUuTWF0dXJlQWdlLCAyLjUgKSApOwoJCgkJCS8vaWYoIExPRCA+PSAyICkKCQkJLy8JY291bnQgLz0gMjsKCQkJLy9pZiggTE9EID49IDMgKQoJCQkvLwljb3VudCAvPSA2OwoJCgkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQ7IG4rKyApCgkJCXsKCQkJCXZhciBwYXJlbnQgPSBzZWxlY3Rvci5HZXQoKTsKCQoJCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CcmFuY2hXaXRoTGVhdmVzICk7CgkKCQkJCS8vISEhIdC_0L7QstC+0YDQsNGH0LjQstCw0YLRjCDQv9C+INCz0L7RgNC40LfQvtC90YLQsNC70Lg_CgkKCQkJCS8vISEhIdGA0LDRgdC_0YDQtdC00LXQu9C10L3QuNC1CgkKCQkJCS8vISEhIdC+0YDQuNC10L3RgtCw0YbQuNGPINC+0YLQvdC+0YHQuNGC0LXQu9GM0L3QviDRgdC+0LvQvdGG0LAv0LLQtdGA0YXQsAoJCgkJCQl2YXIgdmVydGljYWxBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDkwLjAgLSA0NS4wLCA5MC4wICsgNDUuMCApOwoJCQkJdmFyIHR3aXN0QW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAtNDUuMCwgNDUuMCApOwoJCgkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjMsIDAuOTcgKSwgZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICksIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJLy8hISEhdGlsdEFuZ2xlCgkKCQkJCXZhciBsZW5ndGggPSAxLjA7CgkJCQlpZiggbWF0ZXJpYWwgIT0gbnVsbCApCgkJCQkJbGVuZ3RoID0gbWF0ZXJpYWwuUmVhbExlbmd0aCAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgoJCQkJLy9pZiggTE9EID49IDIgKQoJCQkJLy8JbGVuZ3RoICo9IDEuNTsKCQkJCS8vaWYoIExPRCA+PSAzICkKCQkJCS8vCWxlbmd0aCAqPSAxLjU7CgkKCQkJCWdlbmVyYXRvci5MZWF2ZXMuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudFJpYmJvbiggcGFyZW50LCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0udHJhbnNmb3JtLCBsZW5ndGgsIDAsIHRydWUsIDQ1ICkgKTsKCQkJfQoJCgkJCS8vISEhIQoJCQkvL9C_0YDQvtCy0LXRgNGP0YLRjCDQvNCw0YLQtdGA0LjQsNC7INC10YHRgtGMINC70Lgg0LLQtdGC0LrQsC4KCQkJLy_QtdGB0LvQuCDQvdC10YIg0YLQvtCz0LTQsCDQtNC10LvQsNGC0Ywg0LvQuNGB0YLRjNGPLiDQtdGB0YLRjCDQtdGB0YLRjCDRgtC+0LPQtNCwINCy0YHRjiDQstC10YLQutGDINGA0LjQsdCx0L7QvdC+0LwuCgkKCQl9CgkJYnJlYWs7Cgl9CgkKI2VuZGlmCn0K")]
public class DynamicClass3BCB9A97327DA5B2FB2205E2F8D023A9F7CC31C9CAFCA1A626A73E433201566C
{
    public NeoAxis.CSharpScript Owner;
    public void _GenerateStage(NeoAxis.PlantType sender, NeoAxis.PlantGenerator generator, NeoAxis.PlantGenerator.ElementTypeEnum stage)
    {
        //This script is intended to specify the data for a generator.
        //ideas:
        //береза:
        //бранч в 7 раз тоньше чем родитель
        //ствол, бранчи, ветки ровные
        //мало бранчей, веток тоже
        //сверху больше растительности
        //дуб:
        //больше изогнутость чем береза
        //внизу тоже много растительности
        //ель:
        //веток нет, одни бранчи?
        //высота
        //толщина сплайной
#if !DEPLOY
        var minBranchTwigLength = generator.Height / 50.0;
        switch (stage)
        {
            case PlantGenerator.ElementTypeEnum.Trunk:
            {
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var startTransform = new Transform(Vector3.Zero, Quaternion.LookAt(Vector3.ZAxis, Vector3.XAxis));
                var length = generator.Height * generator.Randomizer.Next(0.8, 1.2);
                var thickness = length / 20.0;
                generator.Trunks.Add(generator.CreateElementCylinder(null, material, startTransform, length, thickness, null, 15.0, 24.0, thickness * 0.2, 0));
            }

                break;
            case PlantGenerator.ElementTypeEnum.Branch:
            {
                var count = 60; //40;// 40;
                if (generator.Age < generator.PlantType.MatureAge)
                    count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                var parent = generator.Trunks[0];
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var collisionChecker = new PlantGenerator.CollisionChecker();
                var added = 0;
                for (int n = 0; n < count * 10; n++)
                {
                    var timeFactor = generator.Randomizer.Next(0.1 /* 0.2*/, 0.95);
                    var twistFactor = generator.Randomizer.Next(1.0);
                    if (!collisionChecker.Intersects(timeFactor, twistFactor))
                    {
                        var verticalAngle = generator.Randomizer.Next(45, 100);
                        var twistAngle = generator.Randomizer.Next(-45, 45);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactor, twistFactor, verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Branches.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.05)); // 0.1 ) );
                            collisionChecker.Add(timeFactor, twistFactor);
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Twig:
            {
                var selector = new PlantGenerator.SelectorByProbability(generator);
                selector.AddElements(generator.Branches.Where(b => b.Length >= minBranchTwigLength));
                if (selector.Count != 0)
                {
                    var count = 400; //300;// 400;// 200;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                    var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                    var added = 0;
                    for (int n = 0; n < count * 10; n++)
                    {
                        var parent = selector.Get();
                        var timeFactorOnParentCurve = generator.Randomizer.Next(0.25, 0.95);
                        var verticalAngle = generator.Randomizer.Next(45.0, 100.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(timeFactorOnParentCurve, generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.3, 0.5);
                        var length = thickness * 25.0; //20.0;
                        if (length >= minBranchTwigLength)
                        {
                            generator.Twigs.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.2));
                            added++;
                            if (added >= count)
                                break;
                        }
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Leaf:
                if (generator.Branches.Count != 0 || generator.Twigs.Count != 0)
                {
                    var selector = new PlantGenerator.SelectorByProbability(generator);
                    selector.AddElements(generator.Branches);
                    selector.AddElements(generator.Twigs);
                    selector.AddElements(generator.Trunks);
                    //!!!!распределять в зависимости от длины
                    //!!!!равномерно распределять. бранчи, ветки тоже
                    //!!!!применять LeafCount
                    var count = 4000; //2000;// 1500;// 2000;// 2500;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2.5));
                    //if( LOD >= 2 )
                    //	count /= 2;
                    //if( LOD >= 3 )
                    //	count /= 6;
                    for (int n = 0; n < count; n++)
                    {
                        var parent = selector.Get();
                        var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.BranchWithLeaves);
                        //!!!!поворачивать по горизонтали?
                        //!!!!распределение
                        //!!!!ориентация относительно солнца/верха
                        var verticalAngle = generator.Randomizer.Next(90.0 - 45.0, 90.0 + 45.0);
                        var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(generator.Randomizer.Next(0.3, 0.97), generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        //!!!!tiltAngle
                        var length = 1.0;
                        if (material != null)
                            length = material.RealLength * generator.Randomizer.Next(0.8, 1.2);
                        //if( LOD >= 2 )
                        //	length *= 1.5;
                        //if( LOD >= 3 )
                        //	length *= 1.5;
                        generator.Leaves.Add(generator.CreateElementRibbon(parent, material, startTransform.transform, length, 0, true, 45));
                    }
                //!!!!
                //проверять материал есть ли ветка.
                //если нет тогда делать листья. есть есть тогда всю ветку риббоном.
                }

                break;
        }
#endif
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX0dlbmVyYXRlU3RhZ2UoTmVvQXhpcy5QbGFudFR5cGUgc2VuZGVyLCBOZW9BeGlzLlBsYW50R2VuZXJhdG9yIGdlbmVyYXRvciwgTmVvQXhpcy5QbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0gc3RhZ2UpCnsJCgkvL1RoaXMgc2NyaXB0IGlzIGludGVuZGVkIHRvIHNwZWNpZnkgdGhlIGRhdGEgZm9yIGEgZ2VuZXJhdG9yLgoJCiNpZiAhREVQTE9ZCglzd2l0Y2goIHN0YWdlICkKCXsKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLlRydW5rOgoJCXsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkJCXZhciBzdGFydFRyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oIFZlY3RvcjMuWmVybywgUXVhdGVybmlvbi5Mb29rQXQoIFZlY3RvcjMuWkF4aXMsIFZlY3RvcjMuWEF4aXMgKSApOwoJCQl2YXIgbGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCXZhciB0aGlja25lc3MgPSBsZW5ndGggLyA2MC4wOwoJCgkJCXZhciB0aGlja25lc3NGYWN0b3IgPSBuZXcgQ3VydmVDdWJpY1NwbGluZTFGKCk7CgkJCXRoaWNrbmVzc0ZhY3Rvci5BZGRQb2ludCggbmV3IEN1cnZlMUYuUG9pbnQoIDAsIDEgKSApOwoJCQl0aGlja25lc3NGYWN0b3IuQWRkUG9pbnQoIG5ldyBDdXJ2ZTFGLlBvaW50KCAxLCAwLjMzZiApICk7CgkJCS8vdGhpY2tuZXNzRmFjdG9yLkFkZFBvaW50KCBuZXcgQ3VydmUxRi5Qb2ludCggMC45NWYsIDAuMzNmICkgKTsKCQkJLy90aGlja25lc3NGYWN0b3IuQWRkUG9pbnQoIG5ldyBDdXJ2ZTFGLlBvaW50KCAxLCAwICkgKTsKCQoJCQlnZW5lcmF0b3IuVHJ1bmtzLkFkZCggZ2VuZXJhdG9yLkNyZWF0ZUVsZW1lbnRDeWxpbmRlciggbnVsbCwgbWF0ZXJpYWwsIHN0YXJ0VHJhbnNmb3JtLCBsZW5ndGgsIHRoaWNrbmVzcywgdGhpY2tuZXNzRmFjdG9yLCAxMCwgMTMsIHRoaWNrbmVzcyAqIDAuNSwgMCApICk7CgkJfQoJCWJyZWFrOwoJCgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5CcmFuY2g6CgkJewoJCQl2YXIgY291bnQgPSA3OwoJCQlpZiggZ2VuZXJhdG9yLkFnZSA8IGdlbmVyYXRvci5QbGFudFR5cGUuTWF0dXJlQWdlICkKCQkJCWNvdW50ID0gKGludCkoIChkb3VibGUpY291bnQgKiBNYXRoLlBvdyggZ2VuZXJhdG9yLkFnZSAvIGdlbmVyYXRvci5QbGFudFR5cGUuTWF0dXJlQWdlLCAyICkgKTsKCQoJCQl2YXIgcGFyZW50ID0gZ2VuZXJhdG9yLlRydW5rc1sgMCBdOwoJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJhcmsgKTsKCQoJCQl2YXIgYWRkZWQgPSAwOwoJCQlmb3IoIGludCBuID0gMDsgbiA8IGNvdW50ICogMTA7IG4rKyApCgkJCXsKCQkJCXZhciB0aW1lRmFjdG9yT25QYXJlbnRDdXJ2ZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuMiwgMC42NSApOwoJCQkJdmFyIHZlcnRpY2FsQW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAyMC4wLCA1MC4wICk7CgkJCQl2YXIgdHdpc3RBbmdsZSA9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIC00NS4wLCA0NS4wICk7CgkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCB0aW1lRmFjdG9yT25QYXJlbnRDdXJ2ZSwgZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMS4wICksIHZlcnRpY2FsQW5nbGUsIHR3aXN0QW5nbGUgKTsKCQoJCQkJdmFyIHRoaWNrbmVzcyA9IHN0YXJ0VHJhbnNmb3JtLnBhcmVudFRoaWNrbmVzcyAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4wICk7CgkKCQkJCXZhciBtaW5CcmFuY2hUd2lnTGVuZ3RoID0gZ2VuZXJhdG9yLkhlaWdodCAvIDE1MC4wOwoJCgkJCQl2YXIgbGVuZ3RoID0gdGhpY2tuZXNzICogMzUuMDsKCQkJCWlmKCBsZW5ndGggPj0gbWluQnJhbmNoVHdpZ0xlbmd0aCApCgkJCQl7CgkJCQkJdmFyIHRoaWNrbmVzc0ZhY3RvciA9IG5ldyBDdXJ2ZUN1YmljU3BsaW5lMUYoKTsKCQkJCQl0aGlja25lc3NGYWN0b3IuQWRkUG9pbnQoIG5ldyBDdXJ2ZTFGLlBvaW50KCAwLCAxICkgKTsKCQkJCQl0aGlja25lc3NGYWN0b3IuQWRkUG9pbnQoIG5ldyBDdXJ2ZTFGLlBvaW50KCAxLCAwLjMzZiApICk7CgkKCQkJCQlnZW5lcmF0b3IuQnJhbmNoZXMuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudEN5bGluZGVyKCBwYXJlbnQsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybS50cmFuc2Zvcm0sIGxlbmd0aCwgdGhpY2tuZXNzLCB0aGlja25lc3NGYWN0b3IsIDEwLjAsIDEzLjAsIHRoaWNrbmVzcyAqIDAuNSwgMy4wICkgKTsKCQoJCQkJCWFkZGVkKys7CgkJCQkJaWYoIGFkZGVkID49IGNvdW50ICkKCQkJCQkJYnJlYWs7CgkJCQl9CgkJCX0KCQl9CgkJYnJlYWs7CgkKCS8vY2FzZSBFbGVtZW50VHlwZUVudW0uVHdpZzoKCS8vCWJyZWFrOwoJCgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5GbG93ZXI6CgkJewoJCQlmb3IoIGludCBuID0gMDsgbiA8IGdlbmVyYXRvci5UcnVua3MuQ291bnQgKyBnZW5lcmF0b3IuQnJhbmNoZXMuQ291bnQ7IG4rKyApCgkJCXsKCQkJCXZhciBtYXR1cml0eSA9IGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZS5WYWx1ZSAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCQlpZiggbWF0dXJpdHkgPiAwLjMzICkKCQkJCXsKCQkJCQlQbGFudEdlbmVyYXRvci5FbGVtZW50IHBhcmVudDsKCQkJCQlpZiggbiA8IGdlbmVyYXRvci5UcnVua3MuQ291bnQgKQoJCQkJCQlwYXJlbnQgPSBnZW5lcmF0b3IuVHJ1bmtzWyBuIF07CgkJCQkJZWxzZQoJCQkJCQlwYXJlbnQgPSBnZW5lcmF0b3IuQnJhbmNoZXNbIG4gLSBnZW5lcmF0b3IuVHJ1bmtzLkNvdW50IF07CgkKCQkJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkZsb3dlciApOwoJCgkJCQkJLy8hISEhdHdpc3QgcmFuZG9tCgkKCQkJCQl2YXIgdHJhbnNmb3JtMSA9IHBhcmVudC5DdXJ2ZS5HZXRUcmFuc2Zvcm1CeVRpbWVGYWN0b3IoIDEgKTsKCQoJCQkJCXZhciBkaXJlY3Rpb24gPSAoIHRyYW5zZm9ybTEuUG9zaXRpb24gLSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtQnlUaW1lRmFjdG9yKCAwLjk5ICkuUG9zaXRpb24gKS5HZXROb3JtYWxpemUoKTsKCQkJCQl2YXIgcm90YXRpb24gPSBRdWF0ZXJuaW9uLkZyb21EaXJlY3Rpb25aQXhpc1VwKCBkaXJlY3Rpb24gKTsKCQoJCQkJCXZhciB0cmFuc2Zvcm0gPSBuZXcgVHJhbnNmb3JtKCB0cmFuc2Zvcm0xLlBvc2l0aW9uLCByb3RhdGlvbiApOwoJCgkJCQkJdmFyIGxlbmd0aCA9IG1hdGVyaWFsICE9IG51bGwgPyBtYXRlcmlhbC5SZWFsTGVuZ3RoLlZhbHVlIDogZ2VuZXJhdG9yLkhlaWdodCAvIDE1LjA7CgkJCQkJbGVuZ3RoICo9IGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCQkJaWYoIG1hdHVyaXR5IDwgMSApCgkJCQkJCWxlbmd0aCAqPSBtYXR1cml0eTsKCQoJCQkJCXZhciB3aWR0aCA9IGxlbmd0aDsKCQoJCQkJCWdlbmVyYXRvci5GbG93ZXJzLkFkZCggZ2VuZXJhdG9yLkNyZWF0ZUVsZW1lbnRCb3dsKCBwYXJlbnQsIG1hdGVyaWFsLCB0cmFuc2Zvcm0sIGxlbmd0aCwgd2lkdGgsIG1hdHVyaXR5ICkgKTsKCQkJCX0KCQkJfQoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uTGVhZjoKCQlpZiggZ2VuZXJhdG9yLkJyYW5jaGVzLkNvdW50ICE9IDAgfHwgZ2VuZXJhdG9yLlR3aWdzLkNvdW50ICE9IDAgKQoJCXsKCQkJdmFyIHNlbGVjdG9yID0gbmV3IFBsYW50R2VuZXJhdG9yLlNlbGVjdG9yQnlQcm9iYWJpbGl0eSggZ2VuZXJhdG9yICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuQnJhbmNoZXMgKTsKCQkJLy9zZWxlY3Rvci5BZGRFbGVtZW50cyggVHdpZ3MgKTsKCQkJc2VsZWN0b3IuQWRkRWxlbWVudHMoIGdlbmVyYXRvci5UcnVua3MgKTsKCQoJCQkvLyEhISHRgNCw0YHQv9GA0LXQtNC10LvRj9GC0Ywg0LIg0LfQsNCy0LjRgdC40LzQvtGB0YLQuCDQvtGCINC00LvQuNC90YsKCQoJCQkvLyEhISHRgNCw0LLQvdC+0LzQtdGA0L3QviDRgNCw0YHQv9GA0LXQtNC10LvRj9GC0YwuINCx0YDQsNC90YfQuCwg0LLQtdGC0LrQuCDRgtC+0LbQtQoJCgkJCS8vISEhIdC_0YDQuNC80LXQvdGP0YLRjCBMZWFmQ291bnQKCQoJCQl2YXIgbWF0ZXJpYWwgPSBnZW5lcmF0b3IuRmluZFN1aXRhYmxlTWF0ZXJpYWwoIFBsYW50TWF0ZXJpYWwuUGFydFR5cGVFbnVtLkJyYW5jaFdpdGhMZWF2ZXMgKTsKCQoJCQl2YXIgY291bnQgPSA1MDsKCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQljb3VudCA9IChpbnQpKCAoZG91YmxlKWNvdW50ICogTWF0aC5Qb3coIGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSwgMiApICk7CgkKCQkJLy9pZiggTE9EID49IDIgKQoJCQkvLwljb3VudCAvPSAyOwoJCQkvL2lmKCBMT0QgPj0gMyApCgkJCS8vCWNvdW50IC89IDY7CgkKCQkJZm9yKCBpbnQgbiA9IDA7IG4gPCBjb3VudDsgbisrICkKCQkJewoJCQkJdmFyIHBhcmVudCA9IHNlbGVjdG9yLkdldCgpOwoJCgkJCQkvLyEhISHQv9C+0LLQvtGA0LDRh9C40LLQsNGC0Ywg0L_QviDQs9C+0YDQuNC30L7QvdGC0LDQu9C4PwoJCgkJCQkvLyEhISHRgNCw0YHQv9GA0LXQtNC10LvQtdC90LjQtQoJCgkJCQkvLyEhISHQvtGA0LjQtdC90YLQsNGG0LjRjyDQvtGC0L3QvtGB0LjRgtC10LvRjNC90L4g0YHQvtC70L3RhtCwL9Cy0LXRgNGF0LAKCQoJCQkJdmFyIHZlcnRpY2FsQW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAtMzAuMCwgMzAuMCApOwoJCQkJdmFyIHR3aXN0QW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAtOTAuMCwgOTAuMCApOwoJCgkJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtT25TdXJmYWNlKCBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjAxLCAwLjY1ICksIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDEuMCApLCB2ZXJ0aWNhbEFuZ2xlLCB0d2lzdEFuZ2xlICk7CgkKCQkJCS8vISEhIXRpbHRBbmdsZQoJCgkJCQl2YXIgbGVuZ3RoID0gMC4xOwoJCQkJaWYoIG1hdGVyaWFsICE9IG51bGwgKQoJCQkJewoJCQkJCXZhciBtYXR1cml0eSA9IE1hdGguTWluKCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UuVmFsdWUsIDEuMCApOwoJCQkJCWxlbmd0aCA9IG1hdGVyaWFsLlJlYWxMZW5ndGggKiBtYXR1cml0eSAqIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDAuOCwgMS4yICk7CgkJCQl9CgkKCQkJCS8vISEhIWNyb3NzPwoJCQkJZ2VuZXJhdG9yLkxlYXZlcy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50UmliYm9uKCBwYXJlbnQsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybS50cmFuc2Zvcm0sIGxlbmd0aCwgMCwgZmFsc2UsIDAgKSApOwoJCQl9CgkKCQkJLy8hISEhCgkJCS8v0L_RgNC+0LLQtdGA0Y_RgtGMINC80LDRgtC10YDQuNCw0Lsg0LXRgdGC0Ywg0LvQuCDQstC10YLQutCwLgoJCQkvL9C10YHQu9C4INC90LXRgiDRgtC+0LPQtNCwINC00LXQu9Cw0YLRjCDQu9C40YHRgtGM0Y8uINC10YHRgtGMINC10YHRgtGMINGC0L7Qs9C00LAg0LLRgdGOINCy0LXRgtC60YMg0YDQuNCx0LHQvtC90L7QvC4KCQoJCX0KCQlicmVhazsKCX0JCiNlbmRpZgp9Cg==")]
public class DynamicClassFA7C95FED86BC2E8308957BFAB1A8BA4DC43C959803B67C22B608DA54F8033A4
{
    public NeoAxis.CSharpScript Owner;
    public void _GenerateStage(NeoAxis.PlantType sender, NeoAxis.PlantGenerator generator, NeoAxis.PlantGenerator.ElementTypeEnum stage)
    {
        //This script is intended to specify the data for a generator.
#if !DEPLOY
        switch (stage)
        {
            case PlantGenerator.ElementTypeEnum.Trunk:
            {
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var startTransform = new Transform(Vector3.Zero, Quaternion.LookAt(Vector3.ZAxis, Vector3.XAxis));
                var length = generator.Height * generator.Randomizer.Next(0.8, 1.2);
                var thickness = length / 60.0;
                var thicknessFactor = new CurveCubicSpline1F();
                thicknessFactor.AddPoint(new Curve1F.Point(0, 1));
                thicknessFactor.AddPoint(new Curve1F.Point(1, 0.33f));
                //thicknessFactor.AddPoint( new Curve1F.Point( 0.95f, 0.33f ) );
                //thicknessFactor.AddPoint( new Curve1F.Point( 1, 0 ) );
                generator.Trunks.Add(generator.CreateElementCylinder(null, material, startTransform, length, thickness, thicknessFactor, 10, 13, thickness * 0.5, 0));
            }

                break;
            case PlantGenerator.ElementTypeEnum.Branch:
            {
                var count = 7;
                if (generator.Age < generator.PlantType.MatureAge)
                    count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                var parent = generator.Trunks[0];
                var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Bark);
                var added = 0;
                for (int n = 0; n < count * 10; n++)
                {
                    var timeFactorOnParentCurve = generator.Randomizer.Next(0.2, 0.65);
                    var verticalAngle = generator.Randomizer.Next(20.0, 50.0);
                    var twistAngle = generator.Randomizer.Next(-45.0, 45.0);
                    var startTransform = parent.Curve.GetTransformOnSurface(timeFactorOnParentCurve, generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                    var thickness = startTransform.parentThickness * generator.Randomizer.Next(0.8, 1.0);
                    var minBranchTwigLength = generator.Height / 150.0;
                    var length = thickness * 35.0;
                    if (length >= minBranchTwigLength)
                    {
                        var thicknessFactor = new CurveCubicSpline1F();
                        thicknessFactor.AddPoint(new Curve1F.Point(0, 1));
                        thicknessFactor.AddPoint(new Curve1F.Point(1, 0.33f));
                        generator.Branches.Add(generator.CreateElementCylinder(parent, material, startTransform.transform, length, thickness, thicknessFactor, 10.0, 13.0, thickness * 0.5, 3.0));
                        added++;
                        if (added >= count)
                            break;
                    }
                }
            }

                break;
            //case ElementTypeEnum.Twig:
            //	break;
            case PlantGenerator.ElementTypeEnum.Flower:
            {
                for (int n = 0; n < generator.Trunks.Count + generator.Branches.Count; n++)
                {
                    var maturity = generator.Age / generator.PlantType.MatureAge.Value * generator.Randomizer.Next(0.8, 1.2);
                    if (maturity > 0.33)
                    {
                        PlantGenerator.Element parent;
                        if (n < generator.Trunks.Count)
                            parent = generator.Trunks[n];
                        else
                            parent = generator.Branches[n - generator.Trunks.Count];
                        var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.Flower);
                        //!!!!twist random
                        var transform1 = parent.Curve.GetTransformByTimeFactor(1);
                        var direction = (transform1.Position - parent.Curve.GetTransformByTimeFactor(0.99).Position).GetNormalize();
                        var rotation = Quaternion.FromDirectionZAxisUp(direction);
                        var transform = new Transform(transform1.Position, rotation);
                        var length = material != null ? material.RealLength.Value : generator.Height / 15.0;
                        length *= generator.Randomizer.Next(0.8, 1.2);
                        if (maturity < 1)
                            length *= maturity;
                        var width = length;
                        generator.Flowers.Add(generator.CreateElementBowl(parent, material, transform, length, width, maturity));
                    }
                }
            }

                break;
            case PlantGenerator.ElementTypeEnum.Leaf:
                if (generator.Branches.Count != 0 || generator.Twigs.Count != 0)
                {
                    var selector = new PlantGenerator.SelectorByProbability(generator);
                    selector.AddElements(generator.Branches);
                    //selector.AddElements( Twigs );
                    selector.AddElements(generator.Trunks);
                    //!!!!распределять в зависимости от длины
                    //!!!!равномерно распределять. бранчи, ветки тоже
                    //!!!!применять LeafCount
                    var material = generator.FindSuitableMaterial(PlantMaterial.PartTypeEnum.BranchWithLeaves);
                    var count = 50;
                    if (generator.Age < generator.PlantType.MatureAge)
                        count = (int)((double)count * Math.Pow(generator.Age / generator.PlantType.MatureAge, 2));
                    //if( LOD >= 2 )
                    //	count /= 2;
                    //if( LOD >= 3 )
                    //	count /= 6;
                    for (int n = 0; n < count; n++)
                    {
                        var parent = selector.Get();
                        //!!!!поворачивать по горизонтали?
                        //!!!!распределение
                        //!!!!ориентация относительно солнца/верха
                        var verticalAngle = generator.Randomizer.Next(-30.0, 30.0);
                        var twistAngle = generator.Randomizer.Next(-90.0, 90.0);
                        var startTransform = parent.Curve.GetTransformOnSurface(generator.Randomizer.Next(0.01, 0.65), generator.Randomizer.Next(1.0), verticalAngle, twistAngle);
                        //!!!!tiltAngle
                        var length = 0.1;
                        if (material != null)
                        {
                            var maturity = Math.Min(generator.Age / generator.PlantType.MatureAge.Value, 1.0);
                            length = material.RealLength * maturity * generator.Randomizer.Next(0.8, 1.2);
                        }

                        //!!!!cross?
                        generator.Leaves.Add(generator.CreateElementRibbon(parent, material, startTransform.transform, length, 0, false, 0));
                    }
                //!!!!
                //проверять материал есть ли ветка.
                //если нет тогда делать листья. есть есть тогда всю ветку риббоном.
                }

                break;
        }
#endif
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RfT2JqZWN0SW50ZXJhY3Rpb25JbnB1dE1lc3NhZ2VFdmVudChOZW9BeGlzLkludGVyYWN0aXZlT2JqZWN0IHNlbmRlciwgTmVvQXhpcy5HYW1lTW9kZSBnYW1lTW9kZSwgTmVvQXhpcy5JbnB1dE1lc3NhZ2UgbWVzc2FnZSwgcmVmIGJvb2wgaGFuZGxlZCkKewoJLy9wcm9jZXNzIG1vdXNlIGNsaWNrCgl2YXIgbW91c2VEb3duID0gbWVzc2FnZSBhcyBJbnB1dE1lc3NhZ2VNb3VzZUJ1dHRvbkRvd247CglpZiAobW91c2VEb3duICE9IG51bGwgJiYgKG1vdXNlRG93bi5CdXR0b24gPT0gRU1vdXNlQnV0dG9ucy5MZWZ0IHx8IG1vdXNlRG93bi5CdXR0b24gPT0gRU1vdXNlQnV0dG9ucy5SaWdodCkpCgl7CgkJLy9nZXQgdGhlIGRvb3IKCQl2YXIgZG9vciA9IHNlbmRlci5QYXJlbnQgYXMgRG9vcjsKCQlpZiAoZG9vciAhPSBudWxsKQoJCXsKCQkJLy9tYXJrIHRoZSBpbnB1dCBtZXNzYWdlIGlzIHByb2Nlc3NlZAoJCQloYW5kbGVkID0gdHJ1ZTsKCgkJCS8vZ2V0IGEgY2hhcmFjdGVyIG9mIHRoZSBwbGF5ZXIKCQkJdmFyIHBsYXllckNoYXJhY3RlciA9IGdhbWVNb2RlLk9iamVjdENvbnRyb2xsZWRCeVBsYXllci5WYWx1ZSBhcyBDaGFyYWN0ZXI7CgkJCWlmIChwbGF5ZXJDaGFyYWN0ZXIgIT0gbnVsbCkKCQkJewoJCQkJaWYgKHNlbmRlci5OZXR3b3JrSXNDbGllbnQpCgkJCQl7CgkJCQkJLy9uZXR3b3JrIG1vZGUKCQkJCQkvL2hpbnQ6IHRvIGVuYWJsZSBuZXR3b3JraW5nIG5lZWQgdG8gZW5hYmxlIE5ldHdvcmtNb2RlIGZvciBDIyBzY3JpcHQKCQkJCQlzZW5kZXIuQmVnaW5OZXR3b3JrTWVzc2FnZVRvU2VydmVyKCJDbGljayIpOwoJCQkJCXNlbmRlci5FbmROZXR3b3JrTWVzc2FnZSgpOwoJCQkJfQoJCQkJZWxzZQoJCQkJewoJCQkJCS8vc2luZ2xlIG1vZGUKCgkJCQkJLy9jaGVja3MgcGxheWVyJ3MgY2hhcmFjdGVyIGhhcyBhIGtleQoJCQkJCXZhciBpdGVtID0gcGxheWVyQ2hhcmFjdGVyLkdldEl0ZW1CeVJlc291cmNlTmFtZShAIkNvbnRlbnRcSXRlbXNcQXV0aG9yc1xOZW9BeGlzXEtleVxLZXkuaXRlbXR5cGUiKTsKCQkJCQlpZiAoaXRlbSA9PSBudWxsKQoJCQkJCXsKCQkJCQkJU2NyZWVuTWVzc2FnZXMuQWRkKCJZb3UgbmVlZCB0byBoYXZlIGEga2V5IHRvIG9wZW4gdGhlIGRvb3IuIik7CgkJCQkJCXJldHVybjsKCQkJCQl9CgoJCQkJCS8vb3BlbiBvciBjbG9zZSB0aGUgZG9vcgoJCQkJCWlmIChkb29yLklzQ2xvc2VkKQoJCQkJCQlkb29yLk9wZW4oKTsKCQkJCQllbHNlCgkJCQkJCWRvb3IuQ2xvc2UoKTsKCQkJCX0KCQkJfQoJCX0KCX0KfQo=")]
public class DynamicClass5711147F4EF21D9FA0B3F2A911C199B3C253C826E1E624F6756828BFEEA15789
{
    public NeoAxis.CSharpScript Owner;
    public void InteractiveObject_ObjectInteractionInputMessageEvent(NeoAxis.InteractiveObject sender, NeoAxis.GameMode gameMode, NeoAxis.InputMessage message, ref bool handled)
    {
        //process mouse click
        var mouseDown = message as InputMessageMouseButtonDown;
        if (mouseDown != null && (mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right))
        {
            //get the door
            var door = sender.Parent as Door;
            if (door != null)
            {
                //mark the input message is processed
                handled = true;
                //get a character of the player
                var playerCharacter = gameMode.ObjectControlledByPlayer.Value as Character;
                if (playerCharacter != null)
                {
                    if (sender.NetworkIsClient)
                    {
                        //network mode
                        //hint: to enable networking need to enable NetworkMode for C# script
                        sender.BeginNetworkMessageToServer("Click");
                        sender.EndNetworkMessage();
                    }
                    else
                    {
                        //single mode
                        //checks player's character has a key
                        var item = playerCharacter.GetItemByResourceName(@"Content\Items\Authors\NeoAxis\Key\Key.itemtype");
                        if (item == null)
                        {
                            ScreenMessages.Add("You need to have a key to open the door.");
                            return;
                        }

                        //open or close the door
                        if (door.IsClosed)
                            door.Open();
                        else
                            door.Close();
                    }
                }
            }
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RfUmVjZWl2ZU5ldHdvcmtNZXNzYWdlRnJvbUNsaWVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIE5lb0F4aXMuU2VydmVyTmV0d29ya1NlcnZpY2VfQ29tcG9uZW50cy5DbGllbnRJdGVtIGNsaWVudCwgc3RyaW5nIG1lc3NhZ2UsIE5lb0F4aXMuQXJyYXlEYXRhUmVhZGVyIHJlYWRlciwgcmVmIGJvb2wgZXJyb3IpCnsKI2lmICFDTElFTlQKCS8vc2VydmVyIHNpZGUJCglpZiAobWVzc2FnZSA9PSAiQ2xpY2siKQoJewoJCS8vZ2V0IHBsYXllciBjaGFyYWN0ZXIKCQl2YXIgbmV0d29ya0xvZ2ljID0gTmV0d29ya0xvZ2ljVXRpbGl0eS5HZXROZXR3b3JrTG9naWMoc2VuZGVyKSBhcyBOZXR3b3JrTG9naWM7CgkJdmFyIHBsYXllckNoYXJhY3RlciA9IG5ldHdvcmtMb2dpYz8uU2VydmVyR2V0T2JqZWN0Q29udHJvbGxlZEJ5VXNlcihjbGllbnQuVXNlcikgYXMgQ2hhcmFjdGVyOwoJCWlmIChwbGF5ZXJDaGFyYWN0ZXIgIT0gbnVsbCkKCQl7CgkJCS8vY2hlY2tzIHBsYXllcidzIGNoYXJhY3RlciBoYXMgYSBrZXkKCQkJdmFyIGl0ZW0gPSBwbGF5ZXJDaGFyYWN0ZXIuR2V0SXRlbUJ5UmVzb3VyY2VOYW1lKEAiQ29udGVudFxJdGVtc1xBdXRob3JzXE5lb0F4aXNcS2V5XEtleS5pdGVtdHlwZSIpOwoJCQlpZiAoaXRlbSA9PSBudWxsKQoJCQl7CgkJCQluZXR3b3JrTG9naWMuU2VuZFNjcmVlbk1lc3NhZ2VUb0NsaWVudChjbGllbnQsICJZb3UgbmVlZCB0byBoYXZlIGEga2V5IHRvIG9wZW4gdGhlIGRvb3IuIiwgZmFsc2UpOwoJCQkJcmV0dXJuOwoJCQl9CgoJCQkvL2dldCB0aGUgZG9vcgoJCQl2YXIgZG9vciA9IHNlbmRlci5QYXJlbnQgYXMgRG9vcjsKCQkJaWYgKGRvb3IgIT0gbnVsbCkKCQkJewoJCQkJLy9vcGVuIG9yIGNsb3NlIHRoZSBkb29yCgkJCQlpZiAoZG9vci5Jc0Nsb3NlZCkKCQkJCQlkb29yLk9wZW4oKTsKCQkJCWVsc2UKCQkJCQlkb29yLkNsb3NlKCk7CgkJCX0KCQl9Cgl9CiNlbmRpZgp9Cg==")]
public class DynamicClass5153FD979B7AAF1C286AD7DD0EEF16BC8CDD53A140732B80935F75EFDA3F8699
{
    public NeoAxis.CSharpScript Owner;
    public void InteractiveObject_ReceiveNetworkMessageFromClient(NeoAxis.Component sender, NeoAxis.ServerNetworkService_Components.ClientItem client, string message, NeoAxis.ArrayDataReader reader, ref bool error)
    {
#if !CLIENT
        //server side	
        if (message == "Click")
        {
            //get player character
            var networkLogic = NetworkLogicUtility.GetNetworkLogic(sender) as NetworkLogic;
            var playerCharacter = networkLogic?.ServerGetObjectControlledByUser(client.User) as Character;
            if (playerCharacter != null)
            {
                //checks player's character has a key
                var item = playerCharacter.GetItemByResourceName(@"Content\Items\Authors\NeoAxis\Key\Key.itemtype");
                if (item == null)
                {
                    networkLogic.SendScreenMessageToClient(client, "You need to have a key to open the door.", false);
                    return;
                }

                //get the door
                var door = sender.Parent as Door;
                if (door != null)
                {
                    //open or close the door
                    if (door.IsClosed)
                        door.Open();
                    else
                        door.Close();
                }
            }
        }
#endif
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uIHNlbmRlcikKewoJdmFyIHNjZW5lID0gc2VuZGVyLlBhcmVudFNjZW5lOwoKCS8vb3BlbiBvciBjbG9zZSB0aGUgZG9vcgoJdmFyIGRvb3IgPSBzY2VuZS5HZXRDb21wb25lbnQ8RG9vcj4oIkRvb3IiKTsKCWlmKGRvb3IgIT0gbnVsbCkKCXsKCQlpZihzZW5kZXIuQWN0aXZhdGVkKQoJCQlkb29yLk9wZW4oKTsKCQllbHNlCgkJCWRvb3IuQ2xvc2UoKTsKCQkJCgkJLy9kb29yLkRlc2lyZWRTdGF0ZSA9IHNlbmRlci5BY3RpdmF0ZWQuVmFsdWUgPyAxIDogMDsKCX0JCn0K")]
public class DynamicClassA3F9F3F05204E8C18ADBDB48055280F635894F4F2E82161F2EA6F8CCA239D1A3
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.Button sender)
    {
        var scene = sender.ParentScene;
        //open or close the door
        var door = scene.GetComponent<Door>("Door");
        if (door != null)
        {
            if (sender.Activated)
                door.Open();
            else
                door.Close();
        //door.DesiredState = sender.Activated.Value ? 1 : 0;
        }
    }
}

[CSharpScriptGeneratedAttribute("CnN0YXRpYyBib29sIG5lYXJDYW1lcmE7CnN0YXRpYyBib29sIGFkZGl0aW9uYWxMaWdodHMgPSB0cnVlOwpzdGF0aWMgYm9vbCBzaGFkb3dzID0gdHJ1ZTsKCnB1YmxpYyB2b2lkIEdhbWVNb2RlX1JlbmRlclVJKE5lb0F4aXMuR2FtZU1vZGUgc2VuZGVyLCBOZW9BeGlzLkNhbnZhc1JlbmRlcmVyIHJlbmRlcmVyKQp7Cgl2YXIgbGluZXMgPSBuZXcgTGlzdDxzdHJpbmc+KCk7CgoJbGluZXMuQWRkKCJDIC0gc3dpdGNoIGNhbWVyYSIpOwoJbGluZXMuQWRkKCJMIC0gYWRkaXRpb25hbCBsaWdodHMiKTsKCWxpbmVzLkFkZCgiSCAtIHNoYWRvd3MiKTsKCWxpbmVzLkFkZCgiIik7CglsaW5lcy5BZGQoIkY3IC0gZnJlZSBjYW1lcmEiKTsKCWxpbmVzLkFkZCgiVyBBIFMgRCBRIEUgLSBmcmVlIGNhbWVyYSBjb250cm9sIik7CglsaW5lcy5BZGQoIiIpOwoJbGluZXMuQWRkKCJZb3UgYWxzbyBjYW4gcGxheSB3aXRoIGFudGlhbGlhc2luZyBhbmQgb3RoZXIgc2V0dGluZ3MgZnJvbSBPcHRpb25zIChFc2MpIik7CgoJdmFyIGZvbnRTaXplID0gcmVuZGVyZXIuRGVmYXVsdEZvbnRTaXplOwoJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IyKGZvbnRTaXplICogcmVuZGVyZXIuQXNwZWN0UmF0aW9JbnYgKiAwLjgsIDAuOCk7CgoJLy9kcmF3IGJhY2tncm91bmQKCXsKCQl2YXIgbWF4TGVuZ3RoID0gMC4wOwoJCWZvcmVhY2ggKHZhciBsaW5lIGluIGxpbmVzKQoJCXsKCQkJdmFyIGxlbmd0aCA9IHJlbmRlcmVyLkRlZmF1bHRGb250LkdldFRleHRMZW5ndGgoZm9udFNpemUsIHJlbmRlcmVyLCBsaW5lKTsKCQkJaWYgKGxlbmd0aCA+IG1heExlbmd0aCkKCQkJCW1heExlbmd0aCA9IGxlbmd0aDsKCQl9CgkJdmFyIHJlY3QgPSBvZmZzZXQgKyBuZXcgUmVjdGFuZ2xlKDAsIDAsIG1heExlbmd0aCwgZm9udFNpemUgKiBsaW5lcy5Db3VudCk7CgkJcmVjdC5FeHBhbmQobmV3IFZlY3RvcjIoZm9udFNpemUgKiAwLjIsIGZvbnRTaXplICogMC4yICogcmVuZGVyZXIuQXNwZWN0UmF0aW8pKTsKCQlyZW5kZXJlci5BZGRRdWFkKHJlY3QsIG5ldyBDb2xvclZhbHVlKDAsIDAsIDAsIDAuNzUpKTsKCX0KCgkvL2RyYXcgdGV4dCAKCUNhbnZhc1JlbmRlcmVyVXRpbGl0eS5BZGRUZXh0TGluZXNXaXRoU2hhZG93KHJlbmRlcmVyLlZpZXdwb3J0Rm9yU2NyZWVuQ2FudmFzUmVuZGVyZXIsIHJlbmRlcmVyLkRlZmF1bHRGb250LCByZW5kZXJlci5EZWZhdWx0Rm9udFNpemUsIGxpbmVzLCBuZXcgUmVjdGFuZ2xlKG9mZnNldC5YLCBvZmZzZXQuWSwgMSwgMSksIEVIb3Jpem9udGFsQWxpZ25tZW50LkxlZnQsIEVWZXJ0aWNhbEFsaWdubWVudC5Ub3AsIG5ldyBDb2xvclZhbHVlKDEsIDEsIDEpKTsKfQoKcHVibGljIHZvaWQgR2FtZU1vZGVfSW5wdXRNZXNzYWdlRXZlbnQoTmVvQXhpcy5HYW1lTW9kZSBzZW5kZXIsIE5lb0F4aXMuSW5wdXRNZXNzYWdlIG1lc3NhZ2UpCnsKCXZhciBrZXlEb3duID0gbWVzc2FnZSBhcyBJbnB1dE1lc3NhZ2VLZXlEb3duOwoJaWYgKGtleURvd24gIT0gbnVsbCkKCXsKCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuQykKCQl7CgkJCS8vdXBkYXRlIGNhbWVyYSBzZXR0aW5ncwoJCQluZWFyQ2FtZXJhID0gIW5lYXJDYW1lcmE7CgoJCQkvL3VwZGF0ZSBjYW1lcmEKCQkJdmFyIHNjZW5lID0gc2VuZGVyLkZpbmRQYXJlbnQ8U2NlbmU+KCk7CgkJCXNjZW5lLkNhbWVyYURlZmF1bHQgPSBzY2VuZS5HZXRDb21wb25lbnQ8Q2FtZXJhPihuZWFyQ2FtZXJhID8gIkNhbWVyYSBOZWFyIiA6ICJDYW1lcmEgRmFyIik7CgkJCVNpbXVsYXRpb25BcHAuTWFpblZpZXdwb3J0Lk5vdGlmeUluc3RhbnRDYW1lcmFNb3ZlbWVudCgpOwoKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkwpCgkJewoJCQlhZGRpdGlvbmFsTGlnaHRzID0gIWFkZGl0aW9uYWxMaWdodHM7CgoJCQl2YXIgc2NlbmUgPSBzZW5kZXIuRmluZFBhcmVudDxTY2VuZT4oKTsKCgkJCWZvcmVhY2ggKHZhciBsaWdodCBpbiBzY2VuZS5HZXRDb21wb25lbnRzPExpZ2h0PigpKQoJCQl7CgkJCQlpZiAobGlnaHQuTmFtZSA9PSAiTGlnaHQiIHx8IGxpZ2h0Lk5hbWUuQ29udGFpbnMoIkxpZ2h0ICIpKQoJCQkJCWxpZ2h0LkVuYWJsZWQgPSBhZGRpdGlvbmFsTGlnaHRzOwoJCQl9CgoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoKCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuSCkKCQl7CgkJCXNoYWRvd3MgPSAhc2hhZG93czsKCgkJCXZhciBzY2VuZSA9IHNlbmRlci5GaW5kUGFyZW50PFNjZW5lPigpOwoJCQl2YXIgcGlwZWxpbmUgPSBzY2VuZS5HZXRDb21wb25lbnQ8UmVuZGVyaW5nUGlwZWxpbmVfQmFzaWM+KCk7CgkJCXBpcGVsaW5lLlNoYWRvd3MgPSBzaGFkb3dzOwoKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCX0KfQo=")]
public class DynamicClass203766919532FBDF7507EA0AB541C9E0FA1FA638235567EA048A6D2163EDC889
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
}
#endif