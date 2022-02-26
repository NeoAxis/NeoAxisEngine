#if DEPLOY
namespace Scripts {
// Auto-generated file
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NeoAxis;
using NeoAxis.Editor;
using Project;

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIF90aGlzID0gc2VuZGVyIGFzIEJ1dHRvbkluU3BhY2U7CglpZiAoX3RoaXMgIT0gbnVsbCkKCXsKCQl2YXIgaW5kaWNhdG9yID0gX3RoaXMuR2V0Q29tcG9uZW50KCJJbmRpY2F0b3IiKSBhcyBNZXNoSW5TcGFjZTsKCQlpZiAoaW5kaWNhdG9yICE9IG51bGwpCgkJCWluZGljYXRvci5Db2xvciA9IF90aGlzLkFjdGl2YXRlZCA/IG5ldyBDb2xvclZhbHVlKDAsIDEsIDApIDogbmV3IENvbG9yVmFsdWUoMC41LCAwLjUsIDAuNSk7CgoJCXZhciBidXR0b25PZmZzZXQgPSBfdGhpcy5Db21wb25lbnRzLkdldEJ5UGF0aCgiQnV0dG9uXFxBdHRhY2ggVHJhbnNmb3JtIE9mZnNldCIpIGFzIFRyYW5zZm9ybU9mZnNldDsKCQlpZiAoYnV0dG9uT2Zmc2V0ICE9IG51bGwpCgkJewoJCQl2YXIgb2Zmc2V0UHVzaGVkID0gMC4wMTsKCQkJdmFyIG9mZnNldERlZmF1bHQgPSAwLjA1OwoKCQkJdmFyIGNvZWYgPSAwLjA7CgkJCWlmIChfdGhpcy5DbGlja2luZyAmJiBfdGhpcy5DbGlja2luZ1RvdGFsVGltZSAhPSAwKQoJCQl7CgkJCQl2YXIgdGltZUZhY3RvciA9IE1hdGhFeC5TYXR1cmF0ZShfdGhpcy5DbGlja2luZ0N1cnJlbnRUaW1lIC8gX3RoaXMuQ2xpY2tpbmdUb3RhbFRpbWUpOwoKCQkJCWlmKHRpbWVGYWN0b3IgPCAwLjUpCgkJCQkJY29lZiA9IHRpbWVGYWN0b3IgKiAyOwoJCQkJZWxzZQoJCQkJCWNvZWYgPSAoMS4wZiAtIHRpbWVGYWN0b3IpICogMjsKCQkJfQoKCQkJdmFyIG9mZnNldCA9IE1hdGhFeC5MZXJwKG9mZnNldERlZmF1bHQsIG9mZnNldFB1c2hlZCwgY29lZik7CgkJCWJ1dHRvbk9mZnNldC5Qb3NpdGlvbk9mZnNldCA9IG5ldyBWZWN0b3IzKG9mZnNldCwgMCwgMCk7CgkJfQoJfQp9")]
public class DynamicClass_6473aea9_18b9_48a3_9315_9caf587717e1
{
    public NeoAxis.CSharpScript Owner;
    public void InteractiveObjectButton_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var _this = sender as ButtonInSpace;
        if (_this != null)
        {
            var indicator = _this.GetComponent("Indicator") as MeshInSpace;
            if (indicator != null)
                indicator.Color = _this.Activated ? new ColorValue(0, 1, 0) : new ColorValue(0.5, 0.5, 0.5);
            var buttonOffset = _this.Components.GetByPath("Button\\Attach Transform Offset") as TransformOffset;
            if (buttonOffset != null)
            {
                var offsetPushed = 0.01;
                var offsetDefault = 0.05;
                var coef = 0.0;
                if (_this.Clicking && _this.ClickingTotalTime != 0)
                {
                    var timeFactor = MathEx.Saturate(_this.ClickingCurrentTime / _this.ClickingTotalTime);
                    if (timeFactor < 0.5)
                        coef = timeFactor * 2;
                    else
                        coef = (1.0f - timeFactor) * 2;
                }

                var offset = MathEx.Lerp(offsetDefault, offsetPushed, coef);
                buttonOffset.PositionOffset = new Vector3(offset, 0, 0);
            }
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uSW5TcGFjZSBzZW5kZXIpCnsKCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsKCgl2YXIgbGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRpcmVjdGlvbmFsIExpZ2h0IikgYXMgTGlnaHQ7CglpZiAobGlnaHQgIT0gbnVsbCkKCQlsaWdodC5FbmFibGVkID0gc2VuZGVyLkFjdGl2YXRlZDsKfQo=")]
public class DynamicClass_916d92c5_dddc_4938_9bc3_610dbf46d0f5
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.ButtonInSpace sender)
    {
        var scene = sender.ParentScene;
        var light = scene.GetComponent("Directional Light") as Light;
        if (light != null)
            light.Enabled = sender.Activated;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIF90aGlzID0gc2VuZGVyIGFzIFJlZ3VsYXRvclN3aXRjaEluU3BhY2U7CglpZiAoX3RoaXMgIT0gbnVsbCkKCXsKCQl2YXIgaW5kaWNhdG9yTWluID0gX3RoaXMuR2V0Q29tcG9uZW50KCJJbmRpY2F0b3IgTWluIikgYXMgTWVzaEluU3BhY2U7CgkJaWYgKGluZGljYXRvck1pbiAhPSBudWxsKQoJCQlpbmRpY2F0b3JNaW4uQ29sb3IgPSBfdGhpcy5WYWx1ZS5WYWx1ZSA8PSBfdGhpcy5WYWx1ZVJhbmdlLlZhbHVlLk1pbmltdW0gPyBuZXcgQ29sb3JWYWx1ZSgxLCAwLCAwKSA6IG5ldyBDb2xvclZhbHVlKDAuNSwgMC41LCAwLjUpOwoKCQl2YXIgaW5kaWNhdG9yTWF4ID0gX3RoaXMuR2V0Q29tcG9uZW50KCJJbmRpY2F0b3IgTWF4IikgYXMgTWVzaEluU3BhY2U7CgkJaWYgKGluZGljYXRvck1heCAhPSBudWxsKQoJCQlpbmRpY2F0b3JNYXguQ29sb3IgPSBfdGhpcy5WYWx1ZS5WYWx1ZSA+PSBfdGhpcy5WYWx1ZVJhbmdlLlZhbHVlLk1heGltdW0gPyBuZXcgQ29sb3JWYWx1ZSgwLCAxLCAwKSA6IG5ldyBDb2xvclZhbHVlKDAuNSwgMC41LCAwLjUpOwoKCQl2YXIgYnV0dG9uID0gX3RoaXMuR2V0Q29tcG9uZW50KCJCdXR0b24iKTsKCQlpZiAoYnV0dG9uICE9IG51bGwpCgkJewoJCQl2YXIgb2Zmc2V0ID0gYnV0dG9uLkdldENvbXBvbmVudDxUcmFuc2Zvcm1PZmZzZXQ+KCk7CgkJCWlmIChvZmZzZXQgIT0gbnVsbCkKCQkJewoJCQkJdmFyIGFuZ2xlID0gX3RoaXMuR2V0VmFsdWVBbmdsZSgpIC0gOTA7CgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsKCQkJfQoJCX0KCgkJdmFyIG1hcmtlck1pbiA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIE1pbiIpOwoJCWlmIChtYXJrZXJNaW4gIT0gbnVsbCkKCQl7CgkJCXZhciBvZmZzZXQgPSBtYXJrZXJNaW4uR2V0Q29tcG9uZW50PFRyYW5zZm9ybU9mZnNldD4oKTsKCQkJaWYgKG9mZnNldCAhPSBudWxsKQoJCQl7CgkJCQl2YXIgYW5nbGUgPSBfdGhpcy5BbmdsZVJhbmdlLlZhbHVlLk1pbmltdW0gLSA5MDsKCQkJCXZhciBhbmdsZVIgPSBNYXRoRXguRGVncmVlVG9SYWRpYW4oYW5nbGUpOwoJCQkJb2Zmc2V0LlBvc2l0aW9uT2Zmc2V0ID0gbmV3IFZlY3RvcjMoMC4wMSwgTWF0aC5Db3MoYW5nbGVSKSAqIDAuMDQsIE1hdGguU2luKC1hbmdsZVIpICogMC4wNCk7CgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsKCQkJfQoJCX0KCgkJdmFyIG1hcmtlck1heCA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIE1heCIpOwoJCWlmIChtYXJrZXJNYXggIT0gbnVsbCkKCQl7CgkJCXZhciBvZmZzZXQgPSBtYXJrZXJNYXguR2V0Q29tcG9uZW50PFRyYW5zZm9ybU9mZnNldD4oKTsKCQkJaWYgKG9mZnNldCAhPSBudWxsKQoJCQl7CgkJCQl2YXIgYW5nbGUgPSBfdGhpcy5BbmdsZVJhbmdlLlZhbHVlLk1heGltdW0gLSA5MDsKCQkJCXZhciBhbmdsZVIgPSBNYXRoRXguRGVncmVlVG9SYWRpYW4oYW5nbGUpOwoJCQkJb2Zmc2V0LlBvc2l0aW9uT2Zmc2V0ID0gbmV3IFZlY3RvcjMoMC4wMSwgTWF0aC5Db3MoYW5nbGVSKSAqIDAuMDQsIE1hdGguU2luKC1hbmdsZVIpICogMC4wNCk7CgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsKCQkJfQoJCX0KCgkJdmFyIG1hcmtlckN1cnJlbnQgPSBfdGhpcy5HZXRDb21wb25lbnQoIk1hcmtlciBDdXJyZW50Iik7CgkJaWYgKG1hcmtlckN1cnJlbnQgIT0gbnVsbCkKCQl7CgkJCXZhciBvZmZzZXQgPSBtYXJrZXJDdXJyZW50LkdldENvbXBvbmVudDxUcmFuc2Zvcm1PZmZzZXQ+KCk7CgkJCWlmIChvZmZzZXQgIT0gbnVsbCkKCQkJewoJCQkJdmFyIGFuZ2xlID0gX3RoaXMuR2V0VmFsdWVBbmdsZSgpIC0gOTA7CgkJCQl2YXIgYW5nbGVSID0gTWF0aEV4LkRlZ3JlZVRvUmFkaWFuKGFuZ2xlKTsKCQkJCW9mZnNldC5Qb3NpdGlvbk9mZnNldCA9IG5ldyBWZWN0b3IzKDAuMDYsIE1hdGguQ29zKGFuZ2xlUikgKiAwLjA0LCBNYXRoLlNpbigtYW5nbGVSKSAqIDAuMDQpOwoJCQkJb2Zmc2V0LlJvdGF0aW9uT2Zmc2V0ID0gbmV3IEFuZ2xlcyhhbmdsZSwgMCwgMCkuVG9RdWF0ZXJuaW9uKCk7CgkJCX0KCQl9Cgl9Cn0=")]
public class DynamicClass_1b4f8f4c_2058_4c68_ab1d_1e6dd3d38a41
{
    public NeoAxis.CSharpScript Owner;
    public void InteractiveObjectButton_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var _this = sender as RegulatorSwitchInSpace;
        if (_this != null)
        {
            var indicatorMin = _this.GetComponent("Indicator Min") as MeshInSpace;
            if (indicatorMin != null)
                indicatorMin.Color = _this.Value.Value <= _this.ValueRange.Value.Minimum ? new ColorValue(1, 0, 0) : new ColorValue(0.5, 0.5, 0.5);
            var indicatorMax = _this.GetComponent("Indicator Max") as MeshInSpace;
            if (indicatorMax != null)
                indicatorMax.Color = _this.Value.Value >= _this.ValueRange.Value.Maximum ? new ColorValue(0, 1, 0) : new ColorValue(0.5, 0.5, 0.5);
            var button = _this.GetComponent("Button");
            if (button != null)
            {
                var offset = button.GetComponent<TransformOffset>();
                if (offset != null)
                {
                    var angle = _this.GetValueAngle() - 90;
                    offset.RotationOffset = new Angles(angle, 0, 0).ToQuaternion();
                }
            }

            var markerMin = _this.GetComponent("Marker Min");
            if (markerMin != null)
            {
                var offset = markerMin.GetComponent<TransformOffset>();
                if (offset != null)
                {
                    var angle = _this.AngleRange.Value.Minimum - 90;
                    var angleR = MathEx.DegreeToRadian(angle);
                    offset.PositionOffset = new Vector3(0.01, Math.Cos(angleR) * 0.04, Math.Sin(-angleR) * 0.04);
                    offset.RotationOffset = new Angles(angle, 0, 0).ToQuaternion();
                }
            }

            var markerMax = _this.GetComponent("Marker Max");
            if (markerMax != null)
            {
                var offset = markerMax.GetComponent<TransformOffset>();
                if (offset != null)
                {
                    var angle = _this.AngleRange.Value.Maximum - 90;
                    var angleR = MathEx.DegreeToRadian(angle);
                    offset.PositionOffset = new Vector3(0.01, Math.Cos(angleR) * 0.04, Math.Sin(-angleR) * 0.04);
                    offset.RotationOffset = new Angles(angle, 0, 0).ToQuaternion();
                }
            }

            var markerCurrent = _this.GetComponent("Marker Current");
            if (markerCurrent != null)
            {
                var offset = markerCurrent.GetComponent<TransformOffset>();
                if (offset != null)
                {
                    var angle = _this.GetValueAngle() - 90;
                    var angleR = MathEx.DegreeToRadian(angle);
                    offset.PositionOffset = new Vector3(0.06, Math.Cos(angleR) * 0.04, Math.Sin(-angleR) * 0.04);
                    offset.RotationOffset = new Angles(angle, 0, 0).ToQuaternion();
                }
            }
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLlJlZ3VsYXRvclN3aXRjaEluU3BhY2Ugb2JqKQp7Cgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7CgoJdmFyIGxpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJEaXJlY3Rpb25hbCBMaWdodCIpIGFzIExpZ2h0OwoJaWYgKGxpZ2h0ICE9IG51bGwpCgkJbGlnaHQuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLjAsIDEuMCwgMS4wIC0gb2JqLlZhbHVlKTsKfQo=")]
public class DynamicClass_889ac28e_50c7_4c2b_ba41_37e88bc7c74e
{
    public NeoAxis.CSharpScript Owner;
    public void RegulatorSwitch_ValueChanged(NeoAxis.RegulatorSwitchInSpace obj)
    {
        var scene = obj.ParentScene;
        var light = scene.GetComponent("Directional Light") as Light;
        if (light != null)
            light.Color = new ColorValue(1.0, 1.0, 1.0 - obj.Value);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uSW5TcGFjZSBzZW5kZXIpCnsKCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsKCgl2YXIgZ3JvdW5kID0gc2NlbmUuR2V0Q29tcG9uZW50KCJHcm91bmQiKSBhcyBNZXNoSW5TcGFjZTsKCWlmIChncm91bmQgIT0gbnVsbCkKCXsKCQlpZiAoIWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwuUmVmZXJlbmNlU3BlY2lmaWVkKQoJCXsKCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IFJlZmVyZW5jZVV0aWxpdHkuTWFrZVJlZmVyZW5jZSgKCQkJCUAiQ29udGVudFxNYXRlcmlhbHNcQmFzaWMgTGlicmFyeVxDb25jcmV0ZVxDb25jcmV0ZSBGbG9vciAwMS5tYXRlcmlhbCIpOwoJCX0KCQllbHNlCgkJCWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwgPSBudWxsOwoJfQp9Cg==")]
public class DynamicClass_672e37e4_5e10_496d_91bd_cca6e4bb7344
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.ButtonInSpace sender)
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLlJlZ3VsYXRvclN3aXRjaEluU3BhY2Ugb2JqKQp7Cgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7CgoJdmFyIGdyb3VuZCA9IHNjZW5lLkdldENvbXBvbmVudCgiR3JvdW5kIikgYXMgTWVzaEluU3BhY2U7CglpZiAoZ3JvdW5kICE9IG51bGwpCgkJZ3JvdW5kLkNvbG9yID0gQ29sb3JWYWx1ZS5MZXJwKG5ldyBDb2xvclZhbHVlKDEsIDEsIDEpLCBuZXcgQ29sb3JWYWx1ZSgwLjQsIDAuOSwgMC40KSwgKGZsb2F0KW9iai5WYWx1ZSk7Cn0K")]
public class DynamicClass_07521a82_d3a3_4aaf_abe6_f62225f104ba
{
    public NeoAxis.CSharpScript Owner;
    public void RegulatorSwitch_ValueChanged(NeoAxis.RegulatorSwitchInSpace obj)
    {
        var scene = obj.ParentScene;
        var ground = scene.GetComponent("Ground") as MeshInSpace;
        if (ground != null)
            ground.Color = ColorValue.Lerp(new ColorValue(1, 1, 1), new ColorValue(0.4, 0.9, 0.4), (float)obj.Value);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLlJlZ3VsYXRvclN3aXRjaEluU3BhY2Ugb2JqKQp7Cgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7CgoJdmFyIG1lc2hJblNwYWNlID0gc2NlbmUuR2V0Q29tcG9uZW50KCJHcm91bmQiKSBhcyBNZXNoSW5TcGFjZTsKCWlmIChtZXNoSW5TcGFjZSAhPSBudWxsKQoJCW1lc2hJblNwYWNlLkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMS4wIC0gb2JqLlZhbHVlLCAxLjAsIDEuMCAtIG9iai5WYWx1ZSk7Cn0K")]
public class DynamicClass_5396ad23_76a2_4adc_8c56_d7892757a8c0
{
    public NeoAxis.CSharpScript Owner;
    public void RegulatorSwitch_ValueChanged(NeoAxis.RegulatorSwitchInSpace obj)
    {
        var scene = obj.ParentScene;
        var meshInSpace = scene.GetComponent("Ground") as MeshInSpace;
        if (meshInSpace != null)
            meshInSpace.Color = new ColorValue(1.0 - obj.Value, 1.0, 1.0 - obj.Value);
    }
}

[CSharpScriptGeneratedAttribute("UXVhdGVybmlvbiBHZXRSb3RhdGlvbk9mZnNldCgpCnsKICAgIHZhciBzcGVlZCA9IC0wLjE7CiAgICB2YXIgbWF0ID0gTWF0cml4My5Gcm9tUm90YXRlQnlYKEVuZ2luZUFwcC5FbmdpbmVUaW1lICogc3BlZWQpOwogICAgcmV0dXJuIG1hdC5Ub1F1YXRlcm5pb24oKTsKfQ==")]
public class DynamicClass_e5dc0b42_791f_4f69_86cc_f8046d58d182
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
public class DynamicClass_8fdd2374_0743_4041_bfca_ac18ca82115b
{
    public NeoAxis.CSharpScript Owner;
    double Method()
    {
        return -EngineApp.EngineTime / 5;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIGRvdWJsZSBMYXN0U3BlZWRpbmdVcDsKcHVibGljIGRvdWJsZSBMYXN0VHVybmluZzsKCnB1YmxpYyB2b2lkIElucHV0UHJvY2Vzc2luZ19TaW11bGF0aW9uU3RlcChOZW9BeGlzLkNvbXBvbmVudCBvYmopCnsKCXZhciBzZW5kZXIgPSAoTmVvQXhpcy5JbnB1dFByb2Nlc3Npbmcpb2JqOwoKCUxhc3RTcGVlZGluZ1VwID0gMDsKCUxhc3RUdXJuaW5nID0gMDsKCgkvL2dldCBhY2Nlc3MgdG8gdGhlIHNoaXAKCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsKCWlmIChzaGlwID09IG51bGwpCgkJcmV0dXJuOwoKCS8vY29udHJvbCB0aGUgc2hpcAoJdmFyIGJvZHkgPSBzaGlwLkdldENvbXBvbmVudDxSaWdpZEJvZHkyRD4oKTsKCWlmIChib2R5ICE9IG51bGwpCgl7CgkJLy9rZXlib2FyZAoKCQkvL2ZseSBmb3J3YXJkCgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuVykgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5VcCkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5OdW1QYWQ4KSkKCQl7CgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOwoJCQlib2R5LkFwcGx5Rm9yY2UoZGlyICogMS4wKTsJCQoJCQlMYXN0U3BlZWRpbmdVcCArPSAxLjA7CgkJfQoKCQkvL2ZseSBiYWNrCgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuUykgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5Eb3duKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLk51bVBhZDIpKQoJCXsKCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7CgkJCWJvZHkuQXBwbHlGb3JjZShkaXIgKiAtMS4wKTsJCQkKCQkJTGFzdFNwZWVkaW5nVXAgLT0gMS4wOwoJCX0KCgkJLy90dXJuIGxlZnQKCQlpZiAoc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5BKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkxlZnQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkNCkpCgkJewoJCQlib2R5LkFwcGx5VG9ycXVlKDEuMCk7CgkJCUxhc3RUdXJuaW5nICs9IDEuMDsKCQl9CgoJCS8vdHVybiByaWdodAoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuUmlnaHQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkNikpCgkJewoJCQlib2R5LkFwcGx5VG9ycXVlKC0xLjApOwoJCQlMYXN0VHVybmluZyAtPSAxLjA7CgkJfQoKCQkvL21vdmVtZW50IGJ5IGpveXN0aWNrIGF4ZXMKCQlpZiAoTWF0aC5BYnMoc2VuZGVyLkpveXN0aWNrQXhlc1swXSkgPj0gMC4wMSkKCQl7CgkJCWJvZHkuQXBwbHlUb3JxdWUoLXNlbmRlci5Kb3lzdGlja0F4ZXNbMF0pOwoJCQlMYXN0VHVybmluZyAtPSBzZW5kZXIuSm95c3RpY2tBeGVzWzBdOwoJCX0KCQlpZiAoTWF0aC5BYnMoc2VuZGVyLkpveXN0aWNrQXhlc1sxXSkgPj0gMC4wMSkKCQl7CgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOwoJCQlib2R5LkFwcGx5Rm9yY2UoZGlyICogc2VuZGVyLkpveXN0aWNrQXhlc1sxXSk7CgkJCUxhc3RTcGVlZGluZ1VwICs9IHNlbmRlci5Kb3lzdGlja0F4ZXNbMV07CgkJfQoJCS8vSm95c3RpY2tBeGVzCgkJLy9Kb3lzdGlja0J1dHRvbnMKCQkvL0pveXN0aWNrUE9WcwoJCS8vSm95c3RpY2tTbGlkZXJzCgkJLy9Jc0pveXN0aWNrQnV0dG9uUHJlc3NlZAoJCS8vR2V0Sm95c3RpY2tBeGlzCgkJLy9HZXRKb3lzdGlja1BPVgoJCS8vR2V0Sm95c3RpY2tTbGlkZXIKCgoJCS8vbXVsdGktdG91Y2gKCgkJLy9kZWJ1ZyB0byBjb250cm9sIGJ5IG1vdXNlCgkJLy9WZWN0b3IyW10gdG91Y2hQb3NpdGlvbnMgPSBuZXcgVmVjdG9yMlswXTsJCQoJCS8vaWYoc2VuZGVyLklzTW91c2VCdXR0b25QcmVzc2VkKEVNb3VzZUJ1dHRvbnMuTGVmdCkpCgkJLy8JdG91Y2hQb3NpdGlvbnMgPSBuZXcgVmVjdG9yMltdIHsgc2VuZGVyLk1vdXNlUG9zaXRpb24gfTsKCQkKCQlmb3JlYWNoKHZhciBkYXRhIGluIHNlbmRlci5Ub3VjaFBvaW50ZXJzKQoJCXsKCQkJdmFyIHRvdWNoUG9zaXRpb24gPSBkYXRhLlBvc2l0aW9uOyAKCgkJCWlmKHRvdWNoUG9zaXRpb24uWCA8IDAuNSAmJiB0b3VjaFBvc2l0aW9uLlkgPiAwLjQpCgkJCXsKCQkJCS8vZmx5IGZvcndhcmQsIGJhY2sKCQkJCXsKCQkJCQl2YXIgZmFjdG9yID0gMS4wIC0gKHRvdWNoUG9zaXRpb24uWSAtIDAuNikgLyAwLjQ7CgkJCQkJdmFyIGZvcmNlID0gZmFjdG9yICogMi4wIC0gMS4wOwoJCQkJCWZvcmNlICo9IDEuMjsKCQkJCQlmb3JjZSA9IE1hdGhFeC5DbGFtcChmb3JjZSwgLTEuMCwgMS4wKTsKCgkJCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7CgkJCQkJYm9keS5BcHBseUZvcmNlKGRpciAqIGZvcmNlKTsKCgkJCQkJTGFzdFNwZWVkaW5nVXAgKz0gZm9yY2U7CgkJCQl9CgkJCQkKCQkJCS8vdHVybiBsZWZ0LCByaWdodAoJCQkJewoJCQkJCXZhciBmYWN0b3IgPSAxLjAgLSBNYXRoRXguQ2xhbXAodG91Y2hQb3NpdGlvbi5YIC8gMC4yLCAwLCAxKTsKCQkJCQl2YXIgZm9yY2UgPSBmYWN0b3IgKiAyLjAgLSAxLjA7CQkJCQkKCQkJCQlmb3JjZSAqPSAxLjI7CgkJCQkJZm9yY2UgPSBNYXRoRXguQ2xhbXAoZm9yY2UsIC0xLjAsIDEuMCk7CgkJCQkJCgkJCQkJYm9keS5BcHBseVRvcnF1ZShmb3JjZSk7CgkKCQkJCQlMYXN0VHVybmluZyArPSBmb3JjZTsKCQkJCX0KCQkJfQoJCX0KCgl9CgkKfQoKcHVibGljIHZvaWQgSW5wdXRQcm9jZXNzaW5nX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuSW5wdXRQcm9jZXNzaW5nIHNlbmRlciwgTmVvQXhpcy5HYW1lTW9kZSBnYW1lTW9kZSwgTmVvQXhpcy5JbnB1dE1lc3NhZ2UgbWVzc2FnZSkKewoJLy8vL2dldCBhY2Nlc3MgdG8gdGhlIHNoaXAKCS8vdmFyIHNoaXAgPSBzZW5kZXIuUGFyZW50OwoJLy9pZiAoc2hpcCA9PSBudWxsKQoJLy8JcmV0dXJuOwoKCS8vdmFyIGtleURvd24gPSBtZXNzYWdlIGFzIElucHV0TWVzc2FnZUtleURvd247CgkvL2lmKGtleURvd24gIT0gbnVsbCkKCS8vewoJLy8JaWYoa2V5RG93bi5LZXkgPT0gRUtleXMuU3BhY2UpCgkvLwl7CgkvLwkJLy92YXIgYm9keSA9IHNoaXAuR2V0Q29tcG9uZW50PFJpZ2lkQm9keTJEPigpOwoJLy8JCS8vaWYgKGJvZHkgIT0gbnVsbCkKCS8vCQkvL3sKCS8vCQkvLwlib2R5LkFwcGx5Rm9yY2UobmV3IFZlY3RvcjIoMSwgMCkpOwoJLy8JCS8vfQoJLy8JfQoJLy99Cn0K")]
public class DynamicClass_697973e3_64d2_4f96_a949_9a35117f70c1
{
    public NeoAxis.CSharpScript Owner;
    public double LastSpeedingUp;
    public double LastTurning;
    public void InputProcessing_SimulationStep(NeoAxis.Component obj)
    {
        var sender = (NeoAxis.InputProcessing)obj;
        LastSpeedingUp = 0;
        LastTurning = 0;
        //get access to the ship
        var ship = sender.Parent;
        if (ship == null)
            return;
        //control the ship
        var body = ship.GetComponent<RigidBody2D>();
        if (body != null)
        {
            //keyboard
            //fly forward
            if (sender.IsKeyPressed(EKeys.W) || sender.IsKeyPressed(EKeys.Up) || sender.IsKeyPressed(EKeys.NumPad8))
            {
                var dir = body.TransformV.Rotation.GetForward().ToVector2();
                body.ApplyForce(dir * 1.0);
                LastSpeedingUp += 1.0;
            }

            //fly back
            if (sender.IsKeyPressed(EKeys.S) || sender.IsKeyPressed(EKeys.Down) || sender.IsKeyPressed(EKeys.NumPad2))
            {
                var dir = body.TransformV.Rotation.GetForward().ToVector2();
                body.ApplyForce(dir * -1.0);
                LastSpeedingUp -= 1.0;
            }

            //turn left
            if (sender.IsKeyPressed(EKeys.A) || sender.IsKeyPressed(EKeys.Left) || sender.IsKeyPressed(EKeys.NumPad4))
            {
                body.ApplyTorque(1.0);
                LastTurning += 1.0;
            }

            //turn right
            if (sender.IsKeyPressed(EKeys.D) || sender.IsKeyPressed(EKeys.Right) || sender.IsKeyPressed(EKeys.NumPad6))
            {
                body.ApplyTorque(-1.0);
                LastTurning -= 1.0;
            }

            //movement by joystick axes
            if (Math.Abs(sender.JoystickAxes[0]) >= 0.01)
            {
                body.ApplyTorque(-sender.JoystickAxes[0]);
                LastTurning -= sender.JoystickAxes[0];
            }

            if (Math.Abs(sender.JoystickAxes[1]) >= 0.01)
            {
                var dir = body.TransformV.Rotation.GetForward().ToVector2();
                body.ApplyForce(dir * sender.JoystickAxes[1]);
                LastSpeedingUp += sender.JoystickAxes[1];
            }

            //JoystickAxes
            //JoystickButtons
            //JoystickPOVs
            //JoystickSliders
            //IsJoystickButtonPressed
            //GetJoystickAxis
            //GetJoystickPOV
            //GetJoystickSlider
            //multi-touch
            //debug to control by mouse
            //Vector2[] touchPositions = new Vector2[0];		
            //if(sender.IsMouseButtonPressed(EMouseButtons.Left))
            //	touchPositions = new Vector2[] { sender.MousePosition };
            foreach (var data in sender.TouchPointers)
            {
                var touchPosition = data.Position;
                if (touchPosition.X < 0.5 && touchPosition.Y > 0.4)
                {
                    //fly forward, back
                    {
                        var factor = 1.0 - (touchPosition.Y - 0.6) / 0.4;
                        var force = factor * 2.0 - 1.0;
                        force *= 1.2;
                        force = MathEx.Clamp(force, -1.0, 1.0);
                        var dir = body.TransformV.Rotation.GetForward().ToVector2();
                        body.ApplyForce(dir * force);
                        LastSpeedingUp += force;
                    }

                    //turn left, right
                    {
                        var factor = 1.0 - MathEx.Clamp(touchPosition.X / 0.2, 0, 1);
                        var force = factor * 2.0 - 1.0;
                        force *= 1.2;
                        force = MathEx.Clamp(force, -1.0, 1.0);
                        body.ApplyTorque(force);
                        LastTurning += force;
                    }
                }
            }
        }
    }

    public void InputProcessing_InputMessageEvent(NeoAxis.InputProcessing sender, NeoAxis.GameMode gameMode, NeoAxis.InputMessage message)
    {
    ////get access to the ship
    //var ship = sender.Parent;
    //if (ship == null)
    //	return;
    //var keyDown = message as InputMessageKeyDown;
    //if(keyDown != null)
    //{
    //	if(keyDown.Key == EKeys.Space)
    //	{
    //		//var body = ship.GetComponent<RigidBody2D>();
    //		//if (body != null)
    //		//{
    //		//	body.ApplyForce(new Vector2(1, 0));
    //		//}
    //	}
    //}
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpCnsKCXZhciBvYmplY3QxID0gc2VuZGVyLkNvbXBvbmVudHNbIlNwaGVyZSJdIGFzIE1lc2hJblNwYWNlOwoJaWYob2JqZWN0MSAhPSBudWxsKQoJCW9iamVjdDEuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgwLjUsIDAuNzUgKyBNYXRoLlNpbihUaW1lLkN1cnJlbnQpICogMC4yNSwgMC41KTsKCgl2YXIgbWF0ZXJpYWwyID0gc2VuZGVyLkNvbXBvbmVudHNbIkJveFxcTWF0ZXJpYWwiXSBhcyBNYXRlcmlhbDsKCWlmKG1hdGVyaWFsMiAhPSBudWxsKQoJCW1hdGVyaWFsMi5FbWlzc2l2ZSA9IG5ldyBDb2xvclZhbHVlUG93ZXJlZCgwLCAoMS4wICsgTWF0aC5TaW4oVGltZS5DdXJyZW50KSkgKiA1LCAwKTsKCQkKCXZhciBtYXRlcmlhbDMgPSBzZW5kZXIuQ29tcG9uZW50c1siQ3lsaW5kZXJcXE1hdGVyaWFsIl0gYXMgTWF0ZXJpYWw7CglpZihtYXRlcmlhbDMgIT0gbnVsbCkKCQltYXRlcmlhbDMuUHJvcGVydHlTZXQoIk11bHRpcGxpZXIiLCBuZXcgQ29sb3JWYWx1ZSgxLCAxLCAxLjAgKyAoMS4wICsgTWF0aC5TaW4oVGltZS5DdXJyZW50KSkgKiA1KSk7Cn0K")]
public class DynamicClass_4185ac8e_5afb_4138_b520_a95be3498de5
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uSW5TcGFjZSBzZW5kZXIpCnsKCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsKCgl2YXIgZ3JvdW5kID0gc2NlbmUuR2V0Q29tcG9uZW50KCJHcm91bmQiKSBhcyBNZXNoSW5TcGFjZTsKCWlmIChncm91bmQgIT0gbnVsbCkKCXsKCQlpZiAoIWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwuUmVmZXJlbmNlU3BlY2lmaWVkKQoJCXsKCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IFJlZmVyZW5jZVV0aWxpdHkuTWFrZVJlZmVyZW5jZSggQCJCYXNlXE1hdGVyaWFsc1xEYXJrIFllbGxvdy5tYXRlcmlhbCIpOwoJCX0KCQllbHNlCgkJCWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwgPSBudWxsOwoJfQp9Cg==")]
public class DynamicClass_95b3c19a_5c7d_45f0_bcc9_07bcf0aff489
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.ButtonInSpace sender)
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

[CSharpScriptGeneratedAttribute("Ci8vIFRoZSBleGFtcGxlIG9mIGNvbXB1dGluZyBhIGxpc3Qgb2Ygb2JqZWN0cyBmb3IgdGhlIEdyb3VwT2ZPYmplY3RzIHVzaW5nIHRocmVhZHMuCgpjbGFzcyBEYXRhCnsKCXB1YmxpYyBHcm91cE9mT2JqZWN0cy5PYmplY3RbXSBPYmplY3RzOwp9CgpwdWJsaWMgdm9pZCBDb21wdXRlVXNpbmdUaHJlYWRzX0NvbXB1dGVCZWdpbihOZW9BeGlzLkNvbXB1dGVVc2luZ1RocmVhZHMgc2VuZGVyKQp7CgkvL0xvZy5JbmZvKCJiZWdpbiIpOwoKCXZhciBkYXRhID0gbmV3IERhdGEoKTsKCWRhdGEuT2JqZWN0cyA9IG5ldyBHcm91cE9mT2JqZWN0cy5PYmplY3RbMTAwICogMTAwXTsKCXNlbmRlci5Db250ZXh0LkFueURhdGEgPSBkYXRhOwoKCS8vaG93IHRvIHNraXAgb25lIGNvbXB1dGluZzoKCS8vc2VuZGVyLkNvbnRleHQuQWxsb3dDb21wdXRlID0gZmFsc2U7Cn0KCnB1YmxpYyB2b2lkIENvbXB1dGVVc2luZ1RocmVhZHNfQ29tcHV0ZVRocmVhZChOZW9BeGlzLkNvbXB1dGVVc2luZ1RocmVhZHMgc2VuZGVyLCBpbnQgdGhyZWFkSW5kZXgpCnsKCXZhciBkYXRhID0gc2VuZGVyLkNvbnRleHQuQW55RGF0YSBhcyBEYXRhOwoJaWYgKGRhdGEgPT0gbnVsbCkKCQlyZXR1cm47CgoJdmFyIG9iamVjdHMgPSBkYXRhLk9iamVjdHM7CgoJLy9nZXQgcmFuZ2Ugb2Ygb2JqZWN0cyBmb3IgdGhlIHRocmVhZAoJdmFyIGZyb20gPSAoZGF0YS5PYmplY3RzLkxlbmd0aCAqIHRocmVhZEluZGV4KSAvIHNlbmRlci5UaHJlYWRDb3VudDsKCXZhciB0byA9IChkYXRhLk9iamVjdHMuTGVuZ3RoICogKHRocmVhZEluZGV4ICsgMSkpIC8gc2VuZGVyLlRocmVhZENvdW50OwoKCXZhciByYW5kb20gPSBuZXcgTmVvQXhpcy5GYXN0UmFuZG9tKDApOy8vKGludCkoVGltZS5DdXJyZW50ICogMC4yNSkpOwoKCS8vY29tcHV0ZSBvYmplY3RzCglmb3IgKGludCBuID0gZnJvbTsgbiA8IHRvOyBuKyspCgl7CgkJcmVmIHZhciBvYmogPSByZWYgb2JqZWN0c1tuXTsKCgkJdmFyIHggPSBuICUgMTAwOwoJCXZhciB5ID0gbiAvIDEwMDsKCgkJdmFyIHdhdmluZ1N0YXJ0ID0gcmFuZG9tLk5leHQoTWF0aC5QSSAqIDIpOwoJCXZhciB3YXZpbmdDeWNsZSA9IHJhbmRvbS5OZXh0KDEuMCwgMi4wKTsKCQkKCQkvL3VzZSBBbnlEYXRhIHRvIHN0b3JlIGFkZGl0aW9uYWwgZGF0YSBpbiB0aGUgb2JqZWN0CgkJLy9vYmouQW55RGF0YSA9IDsKCgkJb2JqLkVsZW1lbnQgPSAodXNob3J0KXJhbmRvbS5OZXh0KDIpOwoJCW9iai5GbGFncyA9IEdyb3VwT2ZPYmplY3RzLk9iamVjdC5GbGFnc0VudW0uRW5hYmxlZCB8IEdyb3VwT2ZPYmplY3RzLk9iamVjdC5GbGFnc0VudW0uVmlzaWJsZTsKCQlvYmouUG9zaXRpb24gPSBuZXcgVmVjdG9yMyh4ICogMS4zLCB5ICogMS4zLCAxLjAgKyBNYXRoLlNpbiggd2F2aW5nU3RhcnQgKyBUaW1lLkN1cnJlbnQgKiB3YXZpbmdDeWNsZSApICogMC4yNSApOwoJCW9iai5Sb3RhdGlvbiA9IFF1YXRlcm5pb25GLklkZW50aXR5OwoJCW9iai5TY2FsZSA9IG5ldyBWZWN0b3IzRigxLCAxLCAxKTsKCQlvYmouQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZShyYW5kb20uTmV4dEZsb2F0KCksIHJhbmRvbS5OZXh0RmxvYXQoKSwgcmFuZG9tLk5leHRGbG9hdCgpKTsKCgkJLy92YXIgcG9zID0gbmV3IFZlY3RvcjMobiAqIDEuMywgMCwgMSk7CgkJLy92YXIgcm90ID0gUXVhdGVybmlvbkYuSWRlbnRpdHk7CgkJLy92YXIgc2NsID0gbmV3IFZlY3RvcjNGKDEsIDEsIDEpOwoJCS8vZGF0YS5PYmplY3RzW25dID0gbmV3IEdyb3VwT2ZPYmplY3RzLk9iamVjdCgwLCAwLCAwLCBHcm91cE9mT2JqZWN0cy5PYmplY3QuRmxhZ3NFbnVtLkVuYWJsZWQgfCBHcm91cE9mT2JqZWN0cy5PYmplY3QuRmxhZ3NFbnVtLlZpc2libGUsIHBvcywgcm90LCBzY2wsIFZlY3RvcjRGLlplcm8sIENvbG9yVmFsdWUuT25lLCBWZWN0b3I0Ri5aZXJvLCBWZWN0b3I0Ri5aZXJvKTsgCgl9Cn0KCnB1YmxpYyB2b2lkIENvbXB1dGVVc2luZ1RocmVhZHNfQ29tcHV0ZUVuZChOZW9BeGlzLkNvbXB1dGVVc2luZ1RocmVhZHMgc2VuZGVyKQp7CgkvL0xvZy5JbmZvKCJlbmQiKTsKCgl2YXIgZGF0YSA9IHNlbmRlci5Db250ZXh0LkFueURhdGEgYXMgRGF0YTsKCWlmIChkYXRhID09IG51bGwpCgkJcmV0dXJuOwoKCXZhciBncm91cE9mT2JqZWN0cyA9IHNlbmRlci5QYXJlbnQgYXMgR3JvdXBPZk9iamVjdHM7CglpZiAoZ3JvdXBPZk9iamVjdHMgIT0gbnVsbCkKCQlncm91cE9mT2JqZWN0cy5PYmplY3RzU2V0KGRhdGEuT2JqZWN0cywgdHJ1ZSk7Cn0K")]
public class DynamicClass_7dd01e6a_d4b2_4c13_904f_2d8a2aa4d916
{
    public NeoAxis.CSharpScript Owner;
    // The example of computing a list of objects for the GroupOfObjects using threads.
    class Data
    {
        public GroupOfObjects.Object[] Objects;
    }

    public GroupOfObjects.Object[] Objects;
    public void ComputeUsingThreads_ComputeBegin(NeoAxis.ComputeUsingThreads sender)
    {
        //Log.Info("begin");
        var data = new Data();
        data.Objects = new GroupOfObjects.Object[100 * 100];
        sender.Context.AnyData = data;
    //how to skip one computing:
    //sender.Context.AllowCompute = false;
    }

    public void ComputeUsingThreads_ComputeThread(NeoAxis.ComputeUsingThreads sender, int threadIndex)
    {
        var data = sender.Context.AnyData as Data;
        if (data == null)
            return;
        var objects = data.Objects;
        //get range of objects for the thread
        var from = (data.Objects.Length * threadIndex) / sender.ThreadCount;
        var to = (data.Objects.Length * (threadIndex + 1)) / sender.ThreadCount;
        var random = new NeoAxis.FastRandom(0); //(int)(Time.Current * 0.25));
        //compute objects
        for (int n = from; n < to; n++)
        {
            ref var obj = ref objects[n];
            var x = n % 100;
            var y = n / 100;
            var wavingStart = random.Next(Math.PI * 2);
            var wavingCycle = random.Next(1.0, 2.0);
            //use AnyData to store additional data in the object
            //obj.AnyData = ;
            obj.Element = (ushort)random.Next(2);
            obj.Flags = GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible;
            obj.Position = new Vector3(x * 1.3, y * 1.3, 1.0 + Math.Sin(wavingStart + Time.Current * wavingCycle) * 0.25);
            obj.Rotation = QuaternionF.Identity;
            obj.Scale = new Vector3F(1, 1, 1);
            obj.Color = new ColorValue(random.NextFloat(), random.NextFloat(), random.NextFloat());
        //var pos = new Vector3(n * 1.3, 0, 1);
        //var rot = QuaternionF.Identity;
        //var scl = new Vector3F(1, 1, 1);
        //data.Objects[n] = new GroupOfObjects.Object(0, 0, 0, GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible, pos, rot, scl, Vector4F.Zero, ColorValue.One, Vector4F.Zero, Vector4F.Zero); 
        }
    }

    public void ComputeUsingThreads_ComputeEnd(NeoAxis.ComputeUsingThreads sender)
    {
        //Log.Info("end");
        var data = sender.Context.AnyData as Data;
        if (data == null)
            return;
        var groupOfObjects = sender.Parent as GroupOfObjects;
        if (groupOfObjects != null)
            groupOfObjects.ObjectsSet(data.Objects, true);
    }
}

[CSharpScriptGeneratedAttribute("aW50IE1ldGhvZCggaW50IGEsIGludCBiICkKewoJcmV0dXJuIGEgKyBiOwp9Cg==")]
public class DynamicClass_8f2be157_70bb_4ca1_a5ec_f9faf12d52ae
{
    public NeoAxis.CSharpScript Owner;
    int Method(int a, int b)
    {
        return a + b;
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpCnsKCXZhciBhbmdsZSA9IEVuZ2luZUFwcC5FbmdpbmVUaW1lICogMC4zOwoJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIDIuNTsKCXZhciBsb29rVG8gPSBuZXcgVmVjdG9yMygxMS43Mzc0ODM5MTI0ODI3LCAtMC4wNTE3NzY3NTAzMjQzOSwgLTE1LjUwOTI3NTU4MjUwOTIpOwoJdmFyIGxvb2tBdCA9IFF1YXRlcm5pb24uTG9va0F0KC1vZmZzZXQsIG5ldyBWZWN0b3IzKDAsMCwxKSk7CgkKCXJldHVybiBuZXcgVHJhbnNmb3JtKCBsb29rVG8gKyBvZmZzZXQsIGxvb2tBdCwgVmVjdG9yMy5PbmUgKTsKfQo=")]
public class DynamicClass_e793a6e5_f2e3_4814_badf_f3561e195081
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgTWVzaEluU3BhY2VBbmltYXRpb25Db250cm9sbGVyX0NhbGN1bGF0ZUJvbmVUcmFuc2Zvcm1zKE5lb0F4aXMuTWVzaEluU3BhY2VBbmltYXRpb25Db250cm9sbGVyIHNlbmRlciwgTmVvQXhpcy5Ta2VsZXRvbkFuaW1hdGlvblRyYWNrLkNhbGN1bGF0ZUJvbmVUcmFuc2Zvcm1zSXRlbVtdIHJlc3VsdCkKewoJLy90byBlbmFibGUgdGhpcyBldmVudCBoYW5kbGVyIGluIHRoZSBlZGl0b3IgY2hhbmdlICJXaGVuIEVuYWJsZSIgcHJvcGVydHkgdG8gIlNpbXVsYXRpb24gfCBJbnN0YW5jZSB8IEVkaXRvciIuCgkvL2NvbXBvbmVudDogQ2hhcmFjdGVyL01lc2ggSW4gU3BhY2UvQyMgU2NyaXB0L0V2ZW50IEhhbmRsZXIgQ2FsY3VsYXRlQm9uZVRyYW5zZm9ybXMuCgkKCXZhciBib25lSW5kZXggPSBzZW5kZXIuR2V0Qm9uZUluZGV4KCJtaXhhbW9yaWc6U3BpbmUxIik7CglpZihib25lSW5kZXggIT0gLTEpCgl7CgkJcmVmIHZhciBpdGVtID0gcmVmIHJlc3VsdFtib25lSW5kZXhdOwoKCQkvL2NhbGN1bGF0ZSBib25lIG9mZnNldAoJCXZhciBhbmdsZSA9IG5ldyBEZWdyZWUoNjApICogTWF0aC5TaW4oVGltZS5DdXJyZW50KTsgCgkJdmFyIG9mZnNldCA9IE1hdHJpeDNGLkZyb21Sb3RhdGVCeVkoKGZsb2F0KWFuZ2xlLkluUmFkaWFucygpKS5Ub1F1YXRlcm5pb24oKTsKCQkKCQkvL3VwZGF0ZSB0aGUgYm9uZQoJCWl0ZW0uUm90YXRpb24gKj0gb2Zmc2V0OwoJfQkKfQo=")]
public class DynamicClass_cf7042c4_a15c_4b26_a291_a74239571bf8
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUGFpbnRMYXllcl9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQp7Cgl2YXIgbGF5ZXIgPSAoUGFpbnRMYXllcilzZW5kZXI7CglsYXllci5NYXRlcmlhbENvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMSwgMSwgTWF0aEV4LlNpbihUaW1lLkN1cnJlbnQpICogMC41ICsgMC41KTsKfQo=")]
public class DynamicClass_0866046b_357f_4271_972b_9051647fb7ff
{
    public NeoAxis.CSharpScript Owner;
    public void PaintLayer_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var layer = (PaintLayer)sender;
        layer.MaterialColor = new ColorValue(1, 1, 1, MathEx.Sin(Time.Current) * 0.5 + 0.5);
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpCnsKCXZhciBhbmdsZSA9IEVuZ2luZUFwcC5FbmdpbmVUaW1lICogLTEuMDsKCXZhciBvZmZzZXQgPSBuZXcgVmVjdG9yMyhNYXRoLkNvcyhhbmdsZSksIE1hdGguU2luKGFuZ2xlKSwgMCkgKiAyLjA7Cgl2YXIgbG9va1RvID0gbmV3IFZlY3RvcjMoMTEuNzM3NDgzOTEyNDgyNywgLTAuMDUxNzc2NzUwMzI0MzksIC0xNC44MDkyNzU1ODI1MDkyKTsKCXZhciBsb29rQXQgPSBRdWF0ZXJuaW9uLkxvb2tBdCgtb2Zmc2V0LCBuZXcgVmVjdG9yMygwLDAsMSkpOwoJCglyZXR1cm4gbmV3IFRyYW5zZm9ybSggbG9va1RvICsgb2Zmc2V0LCBsb29rQXQsIFZlY3RvcjMuT25lICk7Cn0K")]
public class DynamicClass_9a04e885_0d4a_487c_9d7f_1de10a829ac5
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
public class DynamicClass_2529a3ba_51d2_4713_aa51_d4a1cff1b29a
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

[CSharpScriptGeneratedAttribute("ZG91YmxlIE1ldGhvZCgpCnsKCXJldHVybiBNYXRoRXguU2F0dXJhdGUoICggTWF0aC5TaW4oIEVuZ2luZUFwcC5FbmdpbmVUaW1lICogMS4zICkgKyAxLjAgKSAvIDIgKTsKfQo=")]
public class DynamicClass_43d16e01_6156_441d_9f32_dc4460f6886c
{
    public NeoAxis.CSharpScript Owner;
    double Method()
    {
        return MathEx.Saturate((Math.Sin(EngineApp.EngineTime * 1.3) + 1.0) / 2);
    }
}

[CSharpScriptGeneratedAttribute("UmVuZGVyaW5nUGlwZWxpbmUgR2V0UGlwZWxpbmUoKQp7CglzdHJpbmcgbmFtZTsKCWlmKEVuZ2luZUFwcC5FbmdpbmVUaW1lICUgNCA+IDIpCgkJbmFtZSA9ICJSZW5kZXJpbmcgUGlwZWxpbmUiOwoJZWxzZQoJCW5hbWUgPSAiUmVuZGVyaW5nIFBpcGVsaW5lIDIiOwoJCQoJcmV0dXJuIE93bmVyLlBhcmVudC5HZXRDb21wb25lbnQobmFtZSkgYXMgUmVuZGVyaW5nUGlwZWxpbmU7Cn0K")]
public class DynamicClass_f1a72f70_88c0_4522_88f4_f720b2856c25
{
    public NeoAxis.CSharpScript Owner;
    RenderingPipeline GetPipeline()
    {
        string name;
        if (EngineApp.EngineTime % 4 > 2)
            name = "Rendering Pipeline";
        else
            name = "Rendering Pipeline 2";
        return Owner.Parent.GetComponent(name) as RenderingPipeline;
    }
}
}
#endif