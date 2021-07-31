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

[CSharpScriptGeneratedAttribute("aW50IE1ldGhvZCggaW50IGEsIGludCBiICkKewoJcmV0dXJuIGEgKyBiOwp9Cg==")]
public class DynamicClass_fdaca87b_753f_43f7_9491_56923e364bfc
{
    public NeoAxis.Component_CSharpScript Owner;
    int Method(int a, int b)
    {
        return a + b;
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpCnsKCXZhciBhbmdsZSA9IEVuZ2luZUFwcC5FbmdpbmVUaW1lICogMC4zOwoJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIDIuNTsKCXZhciBsb29rVG8gPSBuZXcgVmVjdG9yMygxMS43Mzc0ODM5MTI0ODI3LCAtMC4wNTE3NzY3NTAzMjQzOSwgLTE1LjUwOTI3NTU4MjUwOTIpOwoJdmFyIGxvb2tBdCA9IFF1YXRlcm5pb24uTG9va0F0KC1vZmZzZXQsIG5ldyBWZWN0b3IzKDAsMCwxKSk7CgkKCXJldHVybiBuZXcgVHJhbnNmb3JtKCBsb29rVG8gKyBvZmZzZXQsIGxvb2tBdCwgVmVjdG9yMy5PbmUgKTsKfQo=")]
public class DynamicClass_665f9463_ba4c_4a19_8695_fdd1e1d153aa
{
    public NeoAxis.Component_CSharpScript Owner;
    Transform Method()
    {
        var angle = EngineApp.EngineTime * 0.3;
        var offset = new Vector3(Math.Cos(angle), Math.Sin(angle), 0) * 2.5;
        var lookTo = new Vector3(11.7374839124827, -0.05177675032439, -15.5092755825092);
        var lookAt = Quaternion.LookAt(-offset, new Vector3(0, 0, 1));
        return new Transform(lookTo + offset, lookAt, Vector3.One);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIF90aGlzID0gc2VuZGVyIGFzIENvbXBvbmVudF9CdXR0b25JblNwYWNlOwoJaWYgKF90aGlzICE9IG51bGwpCgl7CgkJdmFyIGluZGljYXRvciA9IF90aGlzLkdldENvbXBvbmVudCgiSW5kaWNhdG9yIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOwoJCWlmIChpbmRpY2F0b3IgIT0gbnVsbCkKCQkJaW5kaWNhdG9yLkNvbG9yID0gX3RoaXMuQWN0aXZhdGVkID8gbmV3IENvbG9yVmFsdWUoMCwgMSwgMCkgOiBuZXcgQ29sb3JWYWx1ZSgwLjUsIDAuNSwgMC41KTsKCgkJdmFyIGJ1dHRvbk9mZnNldCA9IF90aGlzLkNvbXBvbmVudHMuR2V0QnlQYXRoKCJCdXR0b25cXEF0dGFjaCBUcmFuc2Zvcm0gT2Zmc2V0IikgYXMgQ29tcG9uZW50X1RyYW5zZm9ybU9mZnNldDsKCQlpZiAoYnV0dG9uT2Zmc2V0ICE9IG51bGwpCgkJewoJCQl2YXIgb2Zmc2V0UHVzaGVkID0gMC4wMTsKCQkJdmFyIG9mZnNldERlZmF1bHQgPSAwLjA1OwoKCQkJdmFyIGNvZWYgPSAwLjA7CgkJCWlmIChfdGhpcy5DbGlja2luZyAmJiBfdGhpcy5DbGlja2luZ1RvdGFsVGltZSAhPSAwKQoJCQl7CgkJCQl2YXIgdGltZUZhY3RvciA9IE1hdGhFeC5TYXR1cmF0ZShfdGhpcy5DbGlja2luZ0N1cnJlbnRUaW1lIC8gX3RoaXMuQ2xpY2tpbmdUb3RhbFRpbWUpOwoKCQkJCWlmKHRpbWVGYWN0b3IgPCAwLjUpCgkJCQkJY29lZiA9IHRpbWVGYWN0b3IgKiAyOwoJCQkJZWxzZQoJCQkJCWNvZWYgPSAoMS4wZiAtIHRpbWVGYWN0b3IpICogMjsKCQkJfQoKCQkJdmFyIG9mZnNldCA9IE1hdGhFeC5MZXJwKG9mZnNldERlZmF1bHQsIG9mZnNldFB1c2hlZCwgY29lZik7CgkJCWJ1dHRvbk9mZnNldC5Qb3NpdGlvbk9mZnNldCA9IG5ldyBWZWN0b3IzKG9mZnNldCwgMCwgMCk7CgkJfQoJfQp9")]
public class DynamicClass_109b82f8_fb5e_4c38_9e82_a0ced5bdd4ce
{
    public NeoAxis.Component_CSharpScript Owner;
    public void InteractiveObjectButton_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var _this = sender as Component_ButtonInSpace;
        if (_this != null)
        {
            var indicator = _this.GetComponent("Indicator") as Component_MeshInSpace;
            if (indicator != null)
                indicator.Color = _this.Activated ? new ColorValue(0, 1, 0) : new ColorValue(0.5, 0.5, 0.5);
            var buttonOffset = _this.Components.GetByPath("Button\\Attach Transform Offset") as Component_TransformOffset;
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJdmFyIGdyb3VuZCA9IHNjZW5lLkdldENvbXBvbmVudCgiR3JvdW5kIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOwoJaWYgKGdyb3VuZCAhPSBudWxsKQoJewoJCWlmICghZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbC5SZWZlcmVuY2VTcGVjaWZpZWQpCgkJewoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKCBAIkJhc2VcTWF0ZXJpYWxzXERhcmsgWWVsbG93Lm1hdGVyaWFsIik7CgkJfQoJCWVsc2UKCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IG51bGw7Cgl9Cn0K")]
public class DynamicClass_91e79a62_d695_425f_95e1_ddd720f0115b
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Button_Click(NeoAxis.Component_ButtonInSpace sender)
    {
        var scene = sender.ParentScene;
        var ground = scene.GetComponent("Ground") as Component_MeshInSpace;
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIF90aGlzID0gc2VuZGVyIGFzIENvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlOwoJaWYgKF90aGlzICE9IG51bGwpCgl7CgkJdmFyIGluZGljYXRvck1pbiA9IF90aGlzLkdldENvbXBvbmVudCgiSW5kaWNhdG9yIE1pbiIpIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsKCQlpZiAoaW5kaWNhdG9yTWluICE9IG51bGwpCgkJCWluZGljYXRvck1pbi5Db2xvciA9IF90aGlzLlZhbHVlLlZhbHVlIDw9IF90aGlzLlZhbHVlUmFuZ2UuVmFsdWUuTWluaW11bSA/IG5ldyBDb2xvclZhbHVlKDEsIDAsIDApIDogbmV3IENvbG9yVmFsdWUoMC41LCAwLjUsIDAuNSk7CgoJCXZhciBpbmRpY2F0b3JNYXggPSBfdGhpcy5HZXRDb21wb25lbnQoIkluZGljYXRvciBNYXgiKSBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7CgkJaWYgKGluZGljYXRvck1heCAhPSBudWxsKQoJCQlpbmRpY2F0b3JNYXguQ29sb3IgPSBfdGhpcy5WYWx1ZS5WYWx1ZSA+PSBfdGhpcy5WYWx1ZVJhbmdlLlZhbHVlLk1heGltdW0gPyBuZXcgQ29sb3JWYWx1ZSgwLCAxLCAwKSA6IG5ldyBDb2xvclZhbHVlKDAuNSwgMC41LCAwLjUpOwoKCQl2YXIgYnV0dG9uID0gX3RoaXMuR2V0Q29tcG9uZW50KCJCdXR0b24iKTsKCQlpZiAoYnV0dG9uICE9IG51bGwpCgkJewoJCQl2YXIgb2Zmc2V0ID0gYnV0dG9uLkdldENvbXBvbmVudDxDb21wb25lbnRfVHJhbnNmb3JtT2Zmc2V0PigpOwoJCQlpZiAob2Zmc2V0ICE9IG51bGwpCgkJCXsKCQkJCXZhciBhbmdsZSA9IF90aGlzLkdldFZhbHVlQW5nbGUoKSAtIDkwOwoJCQkJb2Zmc2V0LlJvdGF0aW9uT2Zmc2V0ID0gbmV3IEFuZ2xlcyhhbmdsZSwgMCwgMCkuVG9RdWF0ZXJuaW9uKCk7CgkJCX0KCQl9CgoJCXZhciBtYXJrZXJNaW4gPSBfdGhpcy5HZXRDb21wb25lbnQoIk1hcmtlciBNaW4iKTsKCQlpZiAobWFya2VyTWluICE9IG51bGwpCgkJewoJCQl2YXIgb2Zmc2V0ID0gbWFya2VyTWluLkdldENvbXBvbmVudDxDb21wb25lbnRfVHJhbnNmb3JtT2Zmc2V0PigpOwoJCQlpZiAob2Zmc2V0ICE9IG51bGwpCgkJCXsKCQkJCXZhciBhbmdsZSA9IF90aGlzLkFuZ2xlUmFuZ2UuVmFsdWUuTWluaW11bSAtIDkwOwoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7CgkJCQlvZmZzZXQuUG9zaXRpb25PZmZzZXQgPSBuZXcgVmVjdG9yMygwLjAxLCBNYXRoLkNvcyhhbmdsZVIpICogMC4wNCwgTWF0aC5TaW4oLWFuZ2xlUikgKiAwLjA0KTsKCQkJCW9mZnNldC5Sb3RhdGlvbk9mZnNldCA9IG5ldyBBbmdsZXMoYW5nbGUsIDAsIDApLlRvUXVhdGVybmlvbigpOwoJCQl9CgkJfQoKCQl2YXIgbWFya2VyTWF4ID0gX3RoaXMuR2V0Q29tcG9uZW50KCJNYXJrZXIgTWF4Iik7CgkJaWYgKG1hcmtlck1heCAhPSBudWxsKQoJCXsKCQkJdmFyIG9mZnNldCA9IG1hcmtlck1heC5HZXRDb21wb25lbnQ8Q29tcG9uZW50X1RyYW5zZm9ybU9mZnNldD4oKTsKCQkJaWYgKG9mZnNldCAhPSBudWxsKQoJCQl7CgkJCQl2YXIgYW5nbGUgPSBfdGhpcy5BbmdsZVJhbmdlLlZhbHVlLk1heGltdW0gLSA5MDsKCQkJCXZhciBhbmdsZVIgPSBNYXRoRXguRGVncmVlVG9SYWRpYW4oYW5nbGUpOwoJCQkJb2Zmc2V0LlBvc2l0aW9uT2Zmc2V0ID0gbmV3IFZlY3RvcjMoMC4wMSwgTWF0aC5Db3MoYW5nbGVSKSAqIDAuMDQsIE1hdGguU2luKC1hbmdsZVIpICogMC4wNCk7CgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsKCQkJfQoJCX0KCgkJdmFyIG1hcmtlckN1cnJlbnQgPSBfdGhpcy5HZXRDb21wb25lbnQoIk1hcmtlciBDdXJyZW50Iik7CgkJaWYgKG1hcmtlckN1cnJlbnQgIT0gbnVsbCkKCQl7CgkJCXZhciBvZmZzZXQgPSBtYXJrZXJDdXJyZW50LkdldENvbXBvbmVudDxDb21wb25lbnRfVHJhbnNmb3JtT2Zmc2V0PigpOwoJCQlpZiAob2Zmc2V0ICE9IG51bGwpCgkJCXsKCQkJCXZhciBhbmdsZSA9IF90aGlzLkdldFZhbHVlQW5nbGUoKSAtIDkwOwoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7CgkJCQlvZmZzZXQuUG9zaXRpb25PZmZzZXQgPSBuZXcgVmVjdG9yMygwLjA2LCBNYXRoLkNvcyhhbmdsZVIpICogMC4wNCwgTWF0aC5TaW4oLWFuZ2xlUikgKiAwLjA0KTsKCQkJCW9mZnNldC5Sb3RhdGlvbk9mZnNldCA9IG5ldyBBbmdsZXMoYW5nbGUsIDAsIDApLlRvUXVhdGVybmlvbigpOwoJCQl9CgkJfQoJfQp9")]
public class DynamicClass_368c8f78_95ec_4175_822f_9ba6d1aee479
{
    public NeoAxis.Component_CSharpScript Owner;
    public void InteractiveObjectButton_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var _this = sender as Component_RegulatorSwitchInSpace;
        if (_this != null)
        {
            var indicatorMin = _this.GetComponent("Indicator Min") as Component_MeshInSpace;
            if (indicatorMin != null)
                indicatorMin.Color = _this.Value.Value <= _this.ValueRange.Value.Minimum ? new ColorValue(1, 0, 0) : new ColorValue(0.5, 0.5, 0.5);
            var indicatorMax = _this.GetComponent("Indicator Max") as Component_MeshInSpace;
            if (indicatorMax != null)
                indicatorMax.Color = _this.Value.Value >= _this.ValueRange.Value.Maximum ? new ColorValue(0, 1, 0) : new ColorValue(0.5, 0.5, 0.5);
            var button = _this.GetComponent("Button");
            if (button != null)
            {
                var offset = button.GetComponent<Component_TransformOffset>();
                if (offset != null)
                {
                    var angle = _this.GetValueAngle() - 90;
                    offset.RotationOffset = new Angles(angle, 0, 0).ToQuaternion();
                }
            }

            var markerMin = _this.GetComponent("Marker Min");
            if (markerMin != null)
            {
                var offset = markerMin.GetComponent<Component_TransformOffset>();
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
                var offset = markerMax.GetComponent<Component_TransformOffset>();
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
                var offset = markerCurrent.GetComponent<Component_TransformOffset>();
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikKewoJdmFyIHNjZW5lID0gb2JqLlBhcmVudFNjZW5lOwoKCXZhciBncm91bmQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkdyb3VuZCIpIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsKCWlmIChncm91bmQgIT0gbnVsbCkKCQlncm91bmQuQ29sb3IgPSBDb2xvclZhbHVlLkxlcnAobmV3IENvbG9yVmFsdWUoMSwgMSwgMSksIG5ldyBDb2xvclZhbHVlKDAuNCwgMC45LCAwLjQpLCAoZmxvYXQpb2JqLlZhbHVlKTsKfQo=")]
public class DynamicClass_11505bcb_eadd_45ff_95a0_a0a17c656240
{
    public NeoAxis.Component_CSharpScript Owner;
    public void RegulatorSwitch_ValueChanged(NeoAxis.Component_RegulatorSwitchInSpace obj)
    {
        var scene = obj.ParentScene;
        var ground = scene.GetComponent("Ground") as Component_MeshInSpace;
        if (ground != null)
            ground.Color = ColorValue.Lerp(new ColorValue(1, 1, 1), new ColorValue(0.4, 0.9, 0.4), (float)obj.Value);
    }
}

[CSharpScriptGeneratedAttribute("UXVhdGVybmlvbiBHZXRSb3RhdGlvbk9mZnNldCgpCnsKICAgIHZhciBzcGVlZCA9IC0wLjE7CiAgICB2YXIgbWF0ID0gTWF0cml4My5Gcm9tUm90YXRlQnlYKEVuZ2luZUFwcC5FbmdpbmVUaW1lICogc3BlZWQpOwogICAgcmV0dXJuIG1hdC5Ub1F1YXRlcm5pb24oKTsKfQ==")]
public class DynamicClass_d8aa2c29_ec99_4423_a68b_c246513592d4
{
    public NeoAxis.Component_CSharpScript Owner;
    Quaternion GetRotationOffset()
    {
        var speed = -0.1;
        var mat = Matrix3.FromRotateByX(EngineApp.EngineTime * speed);
        return mat.ToQuaternion();
    }
}

[CSharpScriptGeneratedAttribute("ZG91YmxlIE1ldGhvZCgpCnsKCXJldHVybiAtRW5naW5lQXBwLkVuZ2luZVRpbWUgLyA1Owp9Cg==")]
public class DynamicClass_4e645a2e_b0e8_4b38_9dd5_bdfadea775a2
{
    public NeoAxis.Component_CSharpScript Owner;
    double Method()
    {
        return -EngineApp.EngineTime / 5;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIF90aGlzID0gc2VuZGVyIGFzIENvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlOwoJaWYgKF90aGlzICE9IG51bGwpCgl7CgkJdmFyIG1hcmtlckN1cnJlbnQgPSBfdGhpcy5HZXRDb21wb25lbnQoIk1hcmtlciBDdXJyZW50Iik7CgkJaWYgKG1hcmtlckN1cnJlbnQgIT0gbnVsbCkKCQl7CgkJCXZhciBvZmZzZXQgPSBtYXJrZXJDdXJyZW50LkdldENvbXBvbmVudDxDb21wb25lbnRfVHJhbnNmb3JtT2Zmc2V0PigpOwoJCQlpZiAob2Zmc2V0ICE9IG51bGwpCgkJCXsKCQkJCXZhciBhbmdsZSA9IF90aGlzLkdldFZhbHVlQW5nbGUoKSAtIDkwOwoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7CgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsKCQkJfQoJCX0KCX0KfQ==")]
public class DynamicClass_245d3cb4_d41a_4384_b26d_fdb712fa46cb
{
    public NeoAxis.Component_CSharpScript Owner;
    public void InteractiveObjectButton_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var _this = sender as Component_RegulatorSwitchInSpace;
        if (_this != null)
        {
            var markerCurrent = _this.GetComponent("Marker Current");
            if (markerCurrent != null)
            {
                var offset = markerCurrent.GetComponent<Component_TransformOffset>();
                if (offset != null)
                {
                    var angle = _this.GetValueAngle() - 90;
                    var angleR = MathEx.DegreeToRadian(angle);
                    offset.RotationOffset = new Angles(angle, 0, 0).ToQuaternion();
                }
            }
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJdmFyIGdyb3VuZCA9IHNjZW5lLkdldENvbXBvbmVudCgiR3JvdW5kIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOwoJaWYgKGdyb3VuZCAhPSBudWxsKQoJewoJCWlmICghZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbC5SZWZlcmVuY2VTcGVjaWZpZWQpCgkJewoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKAoJCQkJQCJTYW1wbGVzXFN0YXJ0ZXIgQ29udGVudFxNYXRlcmlhbHNcQ29uY3JldGUgM3gzIG1ldGVyc1xDb25jcmV0ZSAzeDMgbWV0ZXJzLm1hdGVyaWFsIik7CgkJfQoJCWVsc2UKCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IG51bGw7Cgl9Cn0K")]
public class DynamicClass_2db51532_ec34_42c1_a47f_11890190f8bd
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Button_Click(NeoAxis.Component_ButtonInSpace sender)
    {
        var scene = sender.ParentScene;
        var ground = scene.GetComponent("Ground") as Component_MeshInSpace;
        if (ground != null)
        {
            if (!ground.ReplaceMaterial.ReferenceSpecified)
            {
                ground.ReplaceMaterial = ReferenceUtility.MakeReference(@"Samples\Starter Content\Materials\Concrete 3x3 meters\Concrete 3x3 meters.material");
            }
            else
                ground.ReplaceMaterial = null;
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJdmFyIGdyb3VuZCA9IHNjZW5lLkdldENvbXBvbmVudCgiR3JvdW5kIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOwoJaWYgKGdyb3VuZCAhPSBudWxsKQoJewoJCWlmICghZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbC5SZWZlcmVuY2VTcGVjaWZpZWQpCgkJewoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKAoJCQkJQCJDb250ZW50XE1hdGVyaWFsc1xCYXNpYyBMaWJyYXJ5XENvbmNyZXRlXENvbmNyZXRlIEZsb29yIDAxLm1hdGVyaWFsIik7CgkJfQoJCWVsc2UKCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IG51bGw7Cgl9Cn0K")]
public class DynamicClass_d4c16043_af72_453f_8829_60f88e324cc0
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Button_Click(NeoAxis.Component_ButtonInSpace sender)
    {
        var scene = sender.ParentScene;
        var ground = scene.GetComponent("Ground") as Component_MeshInSpace;
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJdmFyIGxpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJEaXJlY3Rpb25hbCBMaWdodCIpIGFzIENvbXBvbmVudF9MaWdodDsKCWlmIChsaWdodCAhPSBudWxsKQoJCWxpZ2h0LkVuYWJsZWQgPSBzZW5kZXIuQWN0aXZhdGVkOwp9Cg==")]
public class DynamicClass_cefb0a7b_173d_469f_a4df_4eefd03f12b9
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Button_Click(NeoAxis.Component_ButtonInSpace sender)
    {
        var scene = sender.ParentScene;
        var light = scene.GetComponent("Directional Light") as Component_Light;
        if (light != null)
            light.Enabled = sender.Activated;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikKewoJdmFyIHNjZW5lID0gb2JqLlBhcmVudFNjZW5lOwoKCXZhciBsaWdodCA9IHNjZW5lLkdldENvbXBvbmVudCgiRGlyZWN0aW9uYWwgTGlnaHQiKSBhcyBDb21wb25lbnRfTGlnaHQ7CglpZiAobGlnaHQgIT0gbnVsbCkKCQlsaWdodC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEuMCwgMS4wLCAxLjAgLSBvYmouVmFsdWUpOwp9Cg==")]
public class DynamicClass_aef5ba62_9fb4_44fa_9a6a_49ace560ec57
{
    public NeoAxis.Component_CSharpScript Owner;
    public void RegulatorSwitch_ValueChanged(NeoAxis.Component_RegulatorSwitchInSpace obj)
    {
        var scene = obj.ParentScene;
        var light = scene.GetComponent("Directional Light") as Component_Light;
        if (light != null)
            light.Color = new ColorValue(1.0, 1.0, 1.0 - obj.Value);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW5wdXRQcm9jZXNzaW5nX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50X0lucHV0UHJvY2Vzc2luZyBzZW5kZXIsIFVJQ29udHJvbCBwbGF5U2NyZWVuLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQp7CgkvL2dldCBhY2Nlc3MgdG8gdGhlIHNoaXAKCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsKCWlmIChzaGlwID09IG51bGwpCgkJcmV0dXJuOwoKCS8vdmFyIGtleURvd24gPSBtZXNzYWdlIGFzIElucHV0TWVzc2FnZUtleURvd247CgkvL2lmKGtleURvd24gIT0gbnVsbCkKCS8vewoJLy8JaWYoa2V5RG93bi5LZXkgPT0gRUtleXMuU3BhY2UpCgkvLwl7CgkvLwkJLy92YXIgYm9keSA9IHNoaXAuR2V0Q29tcG9uZW50PENvbXBvbmVudF9SaWdpZEJvZHkyRD4oKTsKCS8vCQkvL2lmIChib2R5ICE9IG51bGwpCgkvLwkJLy97CgkvLwkJLy8JYm9keS5BcHBseUZvcmNlKG5ldyBWZWN0b3IyKDEsIDApKTsKCS8vCQkvL30KCS8vCX0KCS8vfQp9CgpwdWJsaWMgdm9pZCBJbnB1dFByb2Nlc3NpbmdfU2ltdWxhdGlvblN0ZXAoTmVvQXhpcy5Db21wb25lbnQgb2JqKQp7Cgl2YXIgc2VuZGVyID0gKE5lb0F4aXMuQ29tcG9uZW50X0lucHV0UHJvY2Vzc2luZylvYmo7CgoJLy9nZXQgYWNjZXNzIHRvIHRoZSBzaGlwCgl2YXIgc2hpcCA9IHNlbmRlci5QYXJlbnQ7CglpZiAoc2hpcCA9PSBudWxsKQoJCXJldHVybjsKCgkvL2NvbnRyb2wgdGhlIHNoaXAKCXZhciBib2R5ID0gc2hpcC5HZXRDb21wb25lbnQ8Q29tcG9uZW50X1JpZ2lkQm9keTJEPigpOwoJaWYgKGJvZHkgIT0gbnVsbCkKCXsKCQkvL2tleWJvYXJkCgoJCS8vZmx5IGZyb250CgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuVykgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5VcCkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5OdW1QYWQ4KSkKCQl7CgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOwoJCQlib2R5LkFwcGx5Rm9yY2UoZGlyICogMS4wKTsKCQl9CgoJCS8vZmx5IGJhY2sKCQlpZiAoc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5TKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkRvd24pIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkMikpCgkJewoJCQl2YXIgZGlyID0gYm9keS5UcmFuc2Zvcm1WLlJvdGF0aW9uLkdldEZvcndhcmQoKS5Ub1ZlY3RvcjIoKTsKCQkJYm9keS5BcHBseUZvcmNlKGRpciAqIC0xLjApOwoJCX0KCgkJLy90dXJuIGxlZnQKCQlpZiAoc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5BKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkxlZnQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkNCkpCgkJCWJvZHkuQXBwbHlUb3JxdWUoMS4wKTsKCgkJLy90dXJuIHJpZ2h0CgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuRCkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5SaWdodCkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5OdW1QYWQ2KSkKCQkJYm9keS5BcHBseVRvcnF1ZSgtMS4wKTsKCgkJLy9tb3ZlbWVudCBieSBqb3lzdGljayBheGVzCgkJaWYgKE1hdGguQWJzKHNlbmRlci5Kb3lzdGlja0F4ZXNbMF0pID49IDAuMDEpCgkJCWJvZHkuQXBwbHlUb3JxdWUoLXNlbmRlci5Kb3lzdGlja0F4ZXNbMF0pOwoJCWlmIChNYXRoLkFicyhzZW5kZXIuSm95c3RpY2tBeGVzWzFdKSA+PSAwLjAxKQoJCXsKCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7CgkJCWJvZHkuQXBwbHlGb3JjZShkaXIgKiBzZW5kZXIuSm95c3RpY2tBeGVzWzFdKTsKCQl9CgkJLy9Kb3lzdGlja0F4ZXMKCQkvL0pveXN0aWNrQnV0dG9ucwoJCS8vSm95c3RpY2tQT1ZzCgkJLy9Kb3lzdGlja1NsaWRlcnMKCQkvL0lzSm95c3RpY2tCdXR0b25QcmVzc2VkCgkJLy9HZXRKb3lzdGlja0F4aXMKCQkvL0dldEpveXN0aWNrUE9WCgkJLy9HZXRKb3lzdGlja1NsaWRlcgkJCQoKCX0KCn0K")]
public class DynamicClass_18a9ed60_f4b8_4c26_8c11_a6e988fd60bf
{
    public NeoAxis.Component_CSharpScript Owner;
    public void InputProcessing_InputMessageEvent(NeoAxis.Component_InputProcessing sender, UIControl playScreen, NeoAxis.InputMessage message)
    {
        //get access to the ship
        var ship = sender.Parent;
        if (ship == null)
            return;
    //var keyDown = message as InputMessageKeyDown;
    //if(keyDown != null)
    //{
    //	if(keyDown.Key == EKeys.Space)
    //	{
    //		//var body = ship.GetComponent<Component_RigidBody2D>();
    //		//if (body != null)
    //		//{
    //		//	body.ApplyForce(new Vector2(1, 0));
    //		//}
    //	}
    //}
    }

    public void InputProcessing_SimulationStep(NeoAxis.Component obj)
    {
        var sender = (NeoAxis.Component_InputProcessing)obj;
        //get access to the ship
        var ship = sender.Parent;
        if (ship == null)
            return;
        //control the ship
        var body = ship.GetComponent<Component_RigidBody2D>();
        if (body != null)
        {
            //keyboard
            //fly front
            if (sender.IsKeyPressed(EKeys.W) || sender.IsKeyPressed(EKeys.Up) || sender.IsKeyPressed(EKeys.NumPad8))
            {
                var dir = body.TransformV.Rotation.GetForward().ToVector2();
                body.ApplyForce(dir * 1.0);
            }

            //fly back
            if (sender.IsKeyPressed(EKeys.S) || sender.IsKeyPressed(EKeys.Down) || sender.IsKeyPressed(EKeys.NumPad2))
            {
                var dir = body.TransformV.Rotation.GetForward().ToVector2();
                body.ApplyForce(dir * -1.0);
            }

            //turn left
            if (sender.IsKeyPressed(EKeys.A) || sender.IsKeyPressed(EKeys.Left) || sender.IsKeyPressed(EKeys.NumPad4))
                body.ApplyTorque(1.0);
            //turn right
            if (sender.IsKeyPressed(EKeys.D) || sender.IsKeyPressed(EKeys.Right) || sender.IsKeyPressed(EKeys.NumPad6))
                body.ApplyTorque(-1.0);
            //movement by joystick axes
            if (Math.Abs(sender.JoystickAxes[0]) >= 0.01)
                body.ApplyTorque(-sender.JoystickAxes[0]);
            if (Math.Abs(sender.JoystickAxes[1]) >= 0.01)
            {
                var dir = body.TransformV.Rotation.GetForward().ToVector2();
                body.ApplyForce(dir * sender.JoystickAxes[1]);
            }
        //JoystickAxes
        //JoystickButtons
        //JoystickPOVs
        //JoystickSliders
        //IsJoystickButtonPressed
        //GetJoystickAxis
        //GetJoystickPOV
        //GetJoystickSlider			
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikKewoJdmFyIHNjZW5lID0gb2JqLlBhcmVudFNjZW5lOwoKCXZhciBtZXNoSW5TcGFjZSA9IHNjZW5lLkdldENvbXBvbmVudCgiR3JvdW5kIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOwoJaWYgKG1lc2hJblNwYWNlICE9IG51bGwpCgkJbWVzaEluU3BhY2UuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLjAgLSBvYmouVmFsdWUsIDEuMCwgMS4wIC0gb2JqLlZhbHVlKTsKfQo=")]
public class DynamicClass_5d4c7862_15ca_420e_bf5b_964e6ead5076
{
    public NeoAxis.Component_CSharpScript Owner;
    public void RegulatorSwitch_ValueChanged(NeoAxis.Component_RegulatorSwitchInSpace obj)
    {
        var scene = obj.ParentScene;
        var meshInSpace = scene.GetComponent("Ground") as Component_MeshInSpace;
        if (meshInSpace != null)
            meshInSpace.Color = new ColorValue(1.0 - obj.Value, 1.0, 1.0 - obj.Value);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uRG9fQ2xpY2soTmVvQXhpcy5VSUJ1dHRvbiBzZW5kZXIpCnsKCXZhciBwYXJlbnQgPSBzZW5kZXIuUGFyZW50OwoJdmFyIGxpbmsgPSBwYXJlbnQuUHJvcGVydHlHZXQ8c3RyaW5nPigiTGVhcm4gTGluayIpOwoJU3lzdGVtLkRpYWdub3N0aWNzLlByb2Nlc3MuU3RhcnQoIG5ldyBTeXN0ZW0uRGlhZ25vc3RpY3MuUHJvY2Vzc1N0YXJ0SW5mbyggbGluayApIHsgVXNlU2hlbGxFeGVjdXRlID0gdHJ1ZSB9ICk7Cn0K")]
public class DynamicClass_6f96eab7_011a_4e00_a551_18d7f180f81f
{
    public NeoAxis.Component_CSharpScript Owner;
    public void ButtonDo_Click(NeoAxis.UIButton sender)
    {
        var parent = sender.Parent;
        var link = parent.PropertyGet<string>("Learn Link");
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(link)
        {UseShellExecute = true});
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpCnsKCXZhciBjb250cm9sID0gKFVJQ29udHJvbClzZW5kZXI7Cgljb250cm9sLkNvbG9yTXVsdGlwbGllciA9IGNvbnRyb2wuUmVhZE9ubHkgPyBuZXcgQ29sb3JWYWx1ZSgwLjUsIDAuNSwgMC41KSA6IG5ldyBDb2xvclZhbHVlKDEsIDEsIDEpOwp9Cg==")]
public class DynamicClass_bdd2f9f1_5993_46f1_9f27_05782780cc5f
{
    public NeoAxis.Component_CSharpScript Owner;
    public void _UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var control = (UIControl)sender;
        control.ColorMultiplier = control.ReadOnly ? new ColorValue(0.5, 0.5, 0.5) : new ColorValue(1, 1, 1);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29udHJvbF9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQp7Cgl2YXIgdGFiQ29udHJvbCA9IHNlbmRlci5Db21wb25lbnRzWyJUYWIgQ29udHJvbCJdIGFzIFVJVGFiQ29udHJvbDsKCWlmKHRhYkNvbnRyb2wgPT0gbnVsbCkKCQlyZXR1cm47CgoJYm9vbCBJc0RvbmUoVUlDb250cm9sIGJsb2NrKQoJewoJCXZhciBjaGVjayA9IGJsb2NrLkdldENvbXBvbmVudDxVSUNoZWNrPigiQ2hlY2sgRG9uZSIpOwoJCXJldHVybiBjaGVjayAhPSBudWxsICYmIGNoZWNrLkNoZWNrZWQuVmFsdWUgPT0gVUlDaGVjay5DaGVja1ZhbHVlLkNoZWNrZWQ7IAoJfQoKCXZhciBwYWdlQmFzaWMgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBCYXNpYyIpIGFzIFVJQ29udHJvbDsKCWlmKHBhZ2VCYXNpYyAhPSBudWxsKQoJewoJCXZhciBibG9jazEgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7CgkJdmFyIGJsb2NrMiA9IHBhZ2VCYXNpYy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgMiIpIGFzIFVJQ29udHJvbDsKCQl2YXIgYmxvY2szID0gcGFnZUJhc2ljLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayAzIikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazQgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7CgkJdmFyIGJsb2NrNSA9IHBhZ2VCYXNpYy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgNSIpIGFzIFVJQ29udHJvbDsKCQl2YXIgYmxvY2s2ID0gcGFnZUJhc2ljLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA2IikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazcgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7CgoJCWJsb2NrMi5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2sxKTsKCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7CgkJYmxvY2szLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazIpOwoJCWJsb2NrNC5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2syKTsKCQlibG9jazYuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMykgJiYgIUlzRG9uZShibG9jazUpOwoJCWJsb2NrNy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2sxKSB8fCAhSXNEb25lKGJsb2NrMikgfHwgIUlzRG9uZShibG9jazMpIHx8ICFJc0RvbmUoYmxvY2s0KSB8fCAhSXNEb25lKGJsb2NrNSkgfHwgIUlzRG9uZShibG9jazYpOwoJCQoJCXZhciB0YWJCdXR0b25zID0gdGFiQ29udHJvbC5HZXRBbGxCdXR0b25zKCk7CgkJdGFiQnV0dG9uc1sxXS5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2s3KTsKCQl0YWJCdXR0b25zWzJdLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazcpOwoJfQoKCXZhciBwYWdlU2NyaXB0aW5nID0gdGFiQ29udHJvbC5HZXRDb21wb25lbnQoIlBhZ2UgU2NyaXB0aW5nIikgYXMgVUlDb250cm9sOwoJaWYocGFnZVNjcmlwdGluZyAhPSBudWxsKQoJewoJCXZhciBibG9jazEgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayAxIikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazIgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayAyIikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazMgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayAzIikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazQgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA0IikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazUgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA1IikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazYgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA2IikgYXMgVUlDb250cm9sOwoJCXZhciBibG9jazcgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA3IikgYXMgVUlDb250cm9sOwoKCQlibG9jazIuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7CgkJYmxvY2szLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazIpOwoJCWJsb2NrNC5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2sxKTsKCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7CgkJYmxvY2s2LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOwoJCWJsb2NrNy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2sxKTsJCQoJfQoKfQo=")]
public class DynamicClass_4f5b47ff_552c_4c39_9312_4bff3336512a
{
    public NeoAxis.Component_CSharpScript Owner;
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
            block2.ReadOnly = !IsDone(block1);
            block5.ReadOnly = !IsDone(block1);
            block3.ReadOnly = !IsDone(block2);
            block4.ReadOnly = !IsDone(block2);
            block6.ReadOnly = !IsDone(block3) && !IsDone(block5);
            block7.ReadOnly = !IsDone(block1) || !IsDone(block2) || !IsDone(block3) || !IsDone(block4) || !IsDone(block5) || !IsDone(block6);
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