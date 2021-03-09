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
using NeoAxis.Addon.Pathfinding;

[CSharpScriptGeneratedAttribute("aW50IE1ldGhvZCggaW50IGEsIGludCBiICkNCnsNCglyZXR1cm4gYSArIGI7DQp9DQo=")]
public class DynamicClass_57943b4b_b716_4f1c_aa84_0f57de867a12
{
    public NeoAxis.Component_CSharpScript Owner;
    int Method(int a, int b)
    {
        return a + b;
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpDQp7DQoJdmFyIGFuZ2xlID0gRW5naW5lQXBwLkVuZ2luZVRpbWUgKiAwLjM7DQoJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIDIuNTsNCgl2YXIgbG9va1RvID0gbmV3IFZlY3RvcjMoMTEuNzM3NDgzOTEyNDgyNywgLTAuMDUxNzc2NzUwMzI0MzksIC0xNS41MDkyNzU1ODI1MDkyKTsNCgl2YXIgbG9va0F0ID0gUXVhdGVybmlvbi5Mb29rQXQoLW9mZnNldCwgbmV3IFZlY3RvcjMoMCwwLDEpKTsNCgkNCglyZXR1cm4gbmV3IFRyYW5zZm9ybSggbG9va1RvICsgb2Zmc2V0LCBsb29rQXQsIFZlY3RvcjMuT25lICk7DQp9DQo=")]
public class DynamicClass_60543a3b_d060_4003_a627_e71e332664eb
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkNCnsNCgl2YXIgX3RoaXMgPSBzZW5kZXIgYXMgQ29tcG9uZW50X0J1dHRvbkluU3BhY2U7DQoJaWYgKF90aGlzICE9IG51bGwpDQoJew0KCQl2YXIgaW5kaWNhdG9yID0gX3RoaXMuR2V0Q29tcG9uZW50KCJJbmRpY2F0b3IiKSBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7DQoJCWlmIChpbmRpY2F0b3IgIT0gbnVsbCkNCgkJCWluZGljYXRvci5Db2xvciA9IF90aGlzLkFjdGl2YXRlZCA/IG5ldyBDb2xvclZhbHVlKDAsIDEsIDApIDogbmV3IENvbG9yVmFsdWUoMC41LCAwLjUsIDAuNSk7DQoNCgkJdmFyIGJ1dHRvbk9mZnNldCA9IF90aGlzLkNvbXBvbmVudHMuR2V0QnlQYXRoKCJCdXR0b25cXEF0dGFjaCBUcmFuc2Zvcm0gT2Zmc2V0IikgYXMgQ29tcG9uZW50X1RyYW5zZm9ybU9mZnNldDsNCgkJaWYgKGJ1dHRvbk9mZnNldCAhPSBudWxsKQ0KCQl7DQoJCQl2YXIgb2Zmc2V0UHVzaGVkID0gMC4wMTsNCgkJCXZhciBvZmZzZXREZWZhdWx0ID0gMC4wNTsNCg0KCQkJdmFyIGNvZWYgPSAwLjA7DQoJCQlpZiAoX3RoaXMuQ2xpY2tpbmcgJiYgX3RoaXMuQ2xpY2tpbmdUb3RhbFRpbWUgIT0gMCkNCgkJCXsNCgkJCQl2YXIgdGltZUZhY3RvciA9IE1hdGhFeC5TYXR1cmF0ZShfdGhpcy5DbGlja2luZ0N1cnJlbnRUaW1lIC8gX3RoaXMuQ2xpY2tpbmdUb3RhbFRpbWUpOw0KDQoJCQkJaWYodGltZUZhY3RvciA8IDAuNSkNCgkJCQkJY29lZiA9IHRpbWVGYWN0b3IgKiAyOw0KCQkJCWVsc2UNCgkJCQkJY29lZiA9ICgxLjBmIC0gdGltZUZhY3RvcikgKiAyOw0KCQkJfQ0KDQoJCQl2YXIgb2Zmc2V0ID0gTWF0aEV4LkxlcnAob2Zmc2V0RGVmYXVsdCwgb2Zmc2V0UHVzaGVkLCBjb2VmKTsNCgkJCWJ1dHRvbk9mZnNldC5Qb3NpdGlvbk9mZnNldCA9IG5ldyBWZWN0b3IzKG9mZnNldCwgMCwgMCk7DQoJCX0NCgl9DQp9")]
public class DynamicClass_cc235550_5ba4_4593_9f4e_c11eb2a922a1
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQ0Kew0KCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsNCg0KCXZhciBncm91bmQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkdyb3VuZCIpIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsNCglpZiAoZ3JvdW5kICE9IG51bGwpDQoJew0KCQlpZiAoIWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwuUmVmZXJlbmNlU3BlY2lmaWVkKQ0KCQl7DQoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKA0KCQkJCUAiU2FtcGxlc1xTdGFydGVyIENvbnRlbnRcTWF0ZXJpYWxzXENvbmNyZXRlIDN4MyBtZXRlcnNcQ29uY3JldGUgM3gzIG1ldGVycy5tYXRlcmlhbCIpOw0KCQl9DQoJCWVsc2UNCgkJCWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwgPSBudWxsOw0KCX0NCn0NCg==")]
public class DynamicClass_a56c59a9_11d4_43d3_8be1_dc00d2d9a154
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkNCnsNCgl2YXIgX3RoaXMgPSBzZW5kZXIgYXMgQ29tcG9uZW50X1JlZ3VsYXRvclN3aXRjaEluU3BhY2U7DQoJaWYgKF90aGlzICE9IG51bGwpDQoJew0KCQl2YXIgaW5kaWNhdG9yTWluID0gX3RoaXMuR2V0Q29tcG9uZW50KCJJbmRpY2F0b3IgTWluIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOw0KCQlpZiAoaW5kaWNhdG9yTWluICE9IG51bGwpDQoJCQlpbmRpY2F0b3JNaW4uQ29sb3IgPSBfdGhpcy5WYWx1ZS5WYWx1ZSA8PSBfdGhpcy5WYWx1ZVJhbmdlLlZhbHVlLk1pbmltdW0gPyBuZXcgQ29sb3JWYWx1ZSgxLCAwLCAwKSA6IG5ldyBDb2xvclZhbHVlKDAuNSwgMC41LCAwLjUpOw0KDQoJCXZhciBpbmRpY2F0b3JNYXggPSBfdGhpcy5HZXRDb21wb25lbnQoIkluZGljYXRvciBNYXgiKSBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7DQoJCWlmIChpbmRpY2F0b3JNYXggIT0gbnVsbCkNCgkJCWluZGljYXRvck1heC5Db2xvciA9IF90aGlzLlZhbHVlLlZhbHVlID49IF90aGlzLlZhbHVlUmFuZ2UuVmFsdWUuTWF4aW11bSA/IG5ldyBDb2xvclZhbHVlKDAsIDEsIDApIDogbmV3IENvbG9yVmFsdWUoMC41LCAwLjUsIDAuNSk7DQoNCgkJdmFyIGJ1dHRvbiA9IF90aGlzLkdldENvbXBvbmVudCgiQnV0dG9uIik7DQoJCWlmIChidXR0b24gIT0gbnVsbCkNCgkJew0KCQkJdmFyIG9mZnNldCA9IGJ1dHRvbi5HZXRDb21wb25lbnQ8Q29tcG9uZW50X1RyYW5zZm9ybU9mZnNldD4oKTsNCgkJCWlmIChvZmZzZXQgIT0gbnVsbCkNCgkJCXsNCgkJCQl2YXIgYW5nbGUgPSBfdGhpcy5HZXRWYWx1ZUFuZ2xlKCkgLSA5MDsNCgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsNCgkJCX0NCgkJfQ0KDQoJCXZhciBtYXJrZXJNaW4gPSBfdGhpcy5HZXRDb21wb25lbnQoIk1hcmtlciBNaW4iKTsNCgkJaWYgKG1hcmtlck1pbiAhPSBudWxsKQ0KCQl7DQoJCQl2YXIgb2Zmc2V0ID0gbWFya2VyTWluLkdldENvbXBvbmVudDxDb21wb25lbnRfVHJhbnNmb3JtT2Zmc2V0PigpOw0KCQkJaWYgKG9mZnNldCAhPSBudWxsKQ0KCQkJew0KCQkJCXZhciBhbmdsZSA9IF90aGlzLkFuZ2xlUmFuZ2UuVmFsdWUuTWluaW11bSAtIDkwOw0KCQkJCXZhciBhbmdsZVIgPSBNYXRoRXguRGVncmVlVG9SYWRpYW4oYW5nbGUpOw0KCQkJCW9mZnNldC5Qb3NpdGlvbk9mZnNldCA9IG5ldyBWZWN0b3IzKDAuMDEsIE1hdGguQ29zKGFuZ2xlUikgKiAwLjA0LCBNYXRoLlNpbigtYW5nbGVSKSAqIDAuMDQpOw0KCQkJCW9mZnNldC5Sb3RhdGlvbk9mZnNldCA9IG5ldyBBbmdsZXMoYW5nbGUsIDAsIDApLlRvUXVhdGVybmlvbigpOw0KCQkJfQ0KCQl9DQoNCgkJdmFyIG1hcmtlck1heCA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIE1heCIpOw0KCQlpZiAobWFya2VyTWF4ICE9IG51bGwpDQoJCXsNCgkJCXZhciBvZmZzZXQgPSBtYXJrZXJNYXguR2V0Q29tcG9uZW50PENvbXBvbmVudF9UcmFuc2Zvcm1PZmZzZXQ+KCk7DQoJCQlpZiAob2Zmc2V0ICE9IG51bGwpDQoJCQl7DQoJCQkJdmFyIGFuZ2xlID0gX3RoaXMuQW5nbGVSYW5nZS5WYWx1ZS5NYXhpbXVtIC0gOTA7DQoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7DQoJCQkJb2Zmc2V0LlBvc2l0aW9uT2Zmc2V0ID0gbmV3IFZlY3RvcjMoMC4wMSwgTWF0aC5Db3MoYW5nbGVSKSAqIDAuMDQsIE1hdGguU2luKC1hbmdsZVIpICogMC4wNCk7DQoJCQkJb2Zmc2V0LlJvdGF0aW9uT2Zmc2V0ID0gbmV3IEFuZ2xlcyhhbmdsZSwgMCwgMCkuVG9RdWF0ZXJuaW9uKCk7DQoJCQl9DQoJCX0NCg0KCQl2YXIgbWFya2VyQ3VycmVudCA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIEN1cnJlbnQiKTsNCgkJaWYgKG1hcmtlckN1cnJlbnQgIT0gbnVsbCkNCgkJew0KCQkJdmFyIG9mZnNldCA9IG1hcmtlckN1cnJlbnQuR2V0Q29tcG9uZW50PENvbXBvbmVudF9UcmFuc2Zvcm1PZmZzZXQ+KCk7DQoJCQlpZiAob2Zmc2V0ICE9IG51bGwpDQoJCQl7DQoJCQkJdmFyIGFuZ2xlID0gX3RoaXMuR2V0VmFsdWVBbmdsZSgpIC0gOTA7DQoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7DQoJCQkJb2Zmc2V0LlBvc2l0aW9uT2Zmc2V0ID0gbmV3IFZlY3RvcjMoMC4wNiwgTWF0aC5Db3MoYW5nbGVSKSAqIDAuMDQsIE1hdGguU2luKC1hbmdsZVIpICogMC4wNCk7DQoJCQkJb2Zmc2V0LlJvdGF0aW9uT2Zmc2V0ID0gbmV3IEFuZ2xlcyhhbmdsZSwgMCwgMCkuVG9RdWF0ZXJuaW9uKCk7DQoJCQl9DQoJCX0NCgl9DQp9")]
public class DynamicClass_18175b77_3a00_4291_bc14_f3b08e3a44a4
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikNCnsNCgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7DQoNCgl2YXIgZ3JvdW5kID0gc2NlbmUuR2V0Q29tcG9uZW50KCJHcm91bmQiKSBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7DQoJaWYgKGdyb3VuZCAhPSBudWxsKQ0KCQlncm91bmQuQ29sb3IgPSBDb2xvclZhbHVlLkxlcnAobmV3IENvbG9yVmFsdWUoMSwgMSwgMSksIG5ldyBDb2xvclZhbHVlKDAuNCwgMC45LCAwLjQpLCAoZmxvYXQpb2JqLlZhbHVlKTsNCn0NCg==")]
public class DynamicClass_f9156f60_24a4_4229_a69f_1a6cc705a38d
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

[CSharpScriptGeneratedAttribute("UXVhdGVybmlvbiBHZXRSb3RhdGlvbk9mZnNldCgpDQp7DQogICAgdmFyIHNwZWVkID0gLTAuMTsNCiAgICB2YXIgbWF0ID0gTWF0cml4My5Gcm9tUm90YXRlQnlYKEVuZ2luZUFwcC5FbmdpbmVUaW1lICogc3BlZWQpOw0KICAgIHJldHVybiBtYXQuVG9RdWF0ZXJuaW9uKCk7DQp9")]
public class DynamicClass_4b744017_737c_49f1_8df2_fb910e89210d
{
    public NeoAxis.Component_CSharpScript Owner;
    Quaternion GetRotationOffset()
    {
        var speed = -0.1;
        var mat = Matrix3.FromRotateByX(EngineApp.EngineTime * speed);
        return mat.ToQuaternion();
    }
}

[CSharpScriptGeneratedAttribute("ZG91YmxlIE1ldGhvZCgpDQp7DQoJcmV0dXJuIC1FbmdpbmVBcHAuRW5naW5lVGltZSAvIDU7DQp9DQo=")]
public class DynamicClass_88881fba_ebe8_40db_a590_979edc2cc7a1
{
    public NeoAxis.Component_CSharpScript Owner;
    double Method()
    {
        return -EngineApp.EngineTime / 5;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpDQp7DQoJdmFyIG9iamVjdDEgPSBzZW5kZXIuQ29tcG9uZW50c1siU3BoZXJlIl0gYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOw0KCWlmKG9iamVjdDEgIT0gbnVsbCkNCgkJb2JqZWN0MS5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNSwgMC43NSArIE1hdGguU2luKFRpbWUuQ3VycmVudCkgKiAwLjI1LCAwLjUpOw0KDQoJdmFyIG1hdGVyaWFsMiA9IHNlbmRlci5Db21wb25lbnRzWyJCb3hcXE1hdGVyaWFsIl0gYXMgQ29tcG9uZW50X01hdGVyaWFsOw0KCWlmKG1hdGVyaWFsMiAhPSBudWxsKQ0KCQltYXRlcmlhbDIuRW1pc3NpdmUgPSBuZXcgQ29sb3JWYWx1ZVBvd2VyZWQoMCwgKDEuMCArIE1hdGguU2luKFRpbWUuQ3VycmVudCkpICogNSwgMCk7DQoJCQ0KCXZhciBtYXRlcmlhbDMgPSBzZW5kZXIuQ29tcG9uZW50c1siQ3lsaW5kZXJcXE1hdGVyaWFsIl0gYXMgQ29tcG9uZW50X01hdGVyaWFsOw0KCWlmKG1hdGVyaWFsMyAhPSBudWxsKQ0KCQltYXRlcmlhbDMuUHJvcGVydHlTZXQoIk11bHRpcGxpZXIiLCBuZXcgQ29sb3JWYWx1ZSgxLCAxLCAxLjAgKyAoMS4wICsgTWF0aC5TaW4oVGltZS5DdXJyZW50KSkgKiA1KSk7DQp9DQo=")]
public class DynamicClass_e320258d_e33c_4b4d_ab74_b4973a43dfc4
{
    public NeoAxis.Component_CSharpScript Owner;
    public void _UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var object1 = sender.Components["Sphere"] as Component_MeshInSpace;
        if (object1 != null)
            object1.Color = new ColorValue(0.5, 0.75 + Math.Sin(Time.Current) * 0.25, 0.5);
        var material2 = sender.Components["Box\\Material"] as Component_Material;
        if (material2 != null)
            material2.Emissive = new ColorValuePowered(0, (1.0 + Math.Sin(Time.Current)) * 5, 0);
        var material3 = sender.Components["Cylinder\\Material"] as Component_Material;
        if (material3 != null)
            material3.PropertySet("Multiplier", new ColorValue(1, 1, 1.0 + (1.0 + Math.Sin(Time.Current)) * 5));
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikNCnsNCgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7DQoNCgl2YXIgbWVzaEluU3BhY2UgPSBzY2VuZS5HZXRDb21wb25lbnQoIkdyb3VuZCIpIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsNCglpZiAobWVzaEluU3BhY2UgIT0gbnVsbCkNCgkJbWVzaEluU3BhY2UuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLjAgLSBvYmouVmFsdWUsIDEuMCwgMS4wIC0gb2JqLlZhbHVlKTsNCn0NCg==")]
public class DynamicClass_b2c5f882_f573_408d_8e29_f9ec19febfb1
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQ0Kew0KCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsNCg0KCXZhciBsaWdodCA9IHNjZW5lLkdldENvbXBvbmVudCgiTGlnaHQgMSIpIGFzIENvbXBvbmVudF9MaWdodDsNCglpZiAobGlnaHQgIT0gbnVsbCkNCgkJbGlnaHQuRW5hYmxlZCA9IHNlbmRlci5BY3RpdmF0ZWQ7DQp9DQo=")]
public class DynamicClass_bdac3172_5fcc_462b_b36c_28158f321bf5
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Button_Click(NeoAxis.Component_ButtonInSpace sender)
    {
        var scene = sender.ParentScene;
        var light = scene.GetComponent("Light 1") as Component_Light;
        if (light != null)
            light.Enabled = sender.Activated;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikNCnsNCgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7DQoNCgl2YXIgbGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkxpZ2h0IDEiKSBhcyBDb21wb25lbnRfTGlnaHQ7DQoJaWYgKGxpZ2h0ICE9IG51bGwpDQoJCWxpZ2h0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMS4wLCAxLjAsIDEuMCAtIG9iai5WYWx1ZSk7DQp9DQo=")]
public class DynamicClass_02d50835_5986_4237_8417_014f9d214761
{
    public NeoAxis.Component_CSharpScript Owner;
    public void RegulatorSwitch_ValueChanged(NeoAxis.Component_RegulatorSwitchInSpace obj)
    {
        var scene = obj.ParentScene;
        var light = scene.GetComponent("Light 1") as Component_Light;
        if (light != null)
            light.Color = new ColorValue(1.0, 1.0, 1.0 - obj.Value);
    }
}

[CSharpScriptGeneratedAttribute("DQovLyBUaGUgZXhhbXBsZSBvZiBjb21wdXRpbmcgYSBsaXN0IG9mIG9iamVjdHMgZm9yIHRoZSBHcm91cE9mT2JqZWN0cyB1c2luZyB0aHJlYWRzLg0KDQpjbGFzcyBEYXRhDQp7DQoJcHVibGljIENvbXBvbmVudF9Hcm91cE9mT2JqZWN0cy5PYmplY3RbXSBPYmplY3RzOw0KfQ0KDQpwdWJsaWMgdm9pZCBDb21wdXRlVXNpbmdUaHJlYWRzX0NvbXB1dGVCZWdpbihOZW9BeGlzLkNvbXBvbmVudF9Db21wdXRlVXNpbmdUaHJlYWRzIHNlbmRlcikNCnsNCgkvL0xvZy5JbmZvKCJiZWdpbiIpOw0KDQoJdmFyIGRhdGEgPSBuZXcgRGF0YSgpOw0KCWRhdGEuT2JqZWN0cyA9IG5ldyBDb21wb25lbnRfR3JvdXBPZk9iamVjdHMuT2JqZWN0WzEwMCAqIDEwMF07DQoJc2VuZGVyLkNvbnRleHQuQW55RGF0YSA9IGRhdGE7DQoNCgkvL2hvdyB0byBza2lwIG9uZSBjb21wdXRpbmc6DQoJLy9zZW5kZXIuQ29udGV4dC5BbGxvd0NvbXB1dGUgPSBmYWxzZTsNCn0NCg0KcHVibGljIHZvaWQgQ29tcHV0ZVVzaW5nVGhyZWFkc19Db21wdXRlVGhyZWFkKE5lb0F4aXMuQ29tcG9uZW50X0NvbXB1dGVVc2luZ1RocmVhZHMgc2VuZGVyLCBpbnQgdGhyZWFkSW5kZXgpDQp7DQoJdmFyIGRhdGEgPSBzZW5kZXIuQ29udGV4dC5BbnlEYXRhIGFzIERhdGE7DQoJaWYgKGRhdGEgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQoJdmFyIG9iamVjdHMgPSBkYXRhLk9iamVjdHM7DQoNCgkvL2dldCByYW5nZSBvZiBvYmplY3RzIGZvciB0aGUgdGhyZWFkDQoJdmFyIGZyb20gPSAoZGF0YS5PYmplY3RzLkxlbmd0aCAqIHRocmVhZEluZGV4KSAvIHNlbmRlci5UaHJlYWRDb3VudDsNCgl2YXIgdG8gPSAoZGF0YS5PYmplY3RzLkxlbmd0aCAqICh0aHJlYWRJbmRleCArIDEpKSAvIHNlbmRlci5UaHJlYWRDb3VudDsNCg0KCXZhciByYW5kb20gPSBuZXcgTmVvQXhpcy5SYW5kb20oMCk7Ly8oaW50KShUaW1lLkN1cnJlbnQgKiAwLjI1KSk7DQoNCgkvL2NvbXB1dGUgb2JqZWN0cw0KCWZvciAoaW50IG4gPSBmcm9tOyBuIDwgdG87IG4rKykNCgl7DQoJCXJlZiB2YXIgb2JqID0gcmVmIG9iamVjdHNbbl07DQoNCgkJdmFyIHggPSBuICUgMTAwOw0KCQl2YXIgeSA9IG4gLyAxMDA7DQoNCgkJdmFyIHdhdmluZ1N0YXJ0ID0gcmFuZG9tLk5leHQoTWF0aC5QSSAqIDIpOw0KCQl2YXIgd2F2aW5nQ3ljbGUgPSByYW5kb20uTmV4dCgxLjAsIDIuMCk7DQoJCQ0KCQkvL3VzZSBBbnlEYXRhIHRvIHN0b3JlIGFkZGl0aW9uYWwgZGF0YSBpbiB0aGUgb2JqZWN0DQoJCS8vb2JqLkFueURhdGEgPSA7DQoNCgkJb2JqLkVsZW1lbnQgPSAodXNob3J0KXJhbmRvbS5OZXh0KDIpOw0KCQlvYmouRmxhZ3MgPSBDb21wb25lbnRfR3JvdXBPZk9iamVjdHMuT2JqZWN0LkZsYWdzRW51bS5FbmFibGVkIHwgQ29tcG9uZW50X0dyb3VwT2ZPYmplY3RzLk9iamVjdC5GbGFnc0VudW0uVmlzaWJsZTsNCgkJb2JqLlBvc2l0aW9uID0gbmV3IFZlY3RvcjMoeCAqIDEuMywgeSAqIDEuMywgMS4wICsgTWF0aC5TaW4oIHdhdmluZ1N0YXJ0ICsgVGltZS5DdXJyZW50ICogd2F2aW5nQ3ljbGUgKSAqIDAuMjUgKTsNCgkJb2JqLlJvdGF0aW9uID0gUXVhdGVybmlvbkYuSWRlbnRpdHk7DQoJCW9iai5TY2FsZSA9IG5ldyBWZWN0b3IzRigxLCAxLCAxKTsNCgkJb2JqLkNvbG9yID0gbmV3IENvbG9yVmFsdWUocmFuZG9tLk5leHRGbG9hdCgpLCByYW5kb20uTmV4dEZsb2F0KCksIHJhbmRvbS5OZXh0RmxvYXQoKSk7DQoNCgkJLy92YXIgcG9zID0gbmV3IFZlY3RvcjMobiAqIDEuMywgMCwgMSk7DQoJCS8vdmFyIHJvdCA9IFF1YXRlcm5pb25GLklkZW50aXR5Ow0KCQkvL3ZhciBzY2wgPSBuZXcgVmVjdG9yM0YoMSwgMSwgMSk7DQoJCS8vZGF0YS5PYmplY3RzW25dID0gbmV3IENvbXBvbmVudF9Hcm91cE9mT2JqZWN0cy5PYmplY3QoMCwgMCwgMCwgQ29tcG9uZW50X0dyb3VwT2ZPYmplY3RzLk9iamVjdC5GbGFnc0VudW0uRW5hYmxlZCB8IENvbXBvbmVudF9Hcm91cE9mT2JqZWN0cy5PYmplY3QuRmxhZ3NFbnVtLlZpc2libGUsIHBvcywgcm90LCBzY2wsIFZlY3RvcjRGLlplcm8sIENvbG9yVmFsdWUuT25lLCBWZWN0b3I0Ri5aZXJvLCBWZWN0b3I0Ri5aZXJvKTsgDQoJfQ0KfQ0KDQpwdWJsaWMgdm9pZCBDb21wdXRlVXNpbmdUaHJlYWRzX0NvbXB1dGVFbmQoTmVvQXhpcy5Db21wb25lbnRfQ29tcHV0ZVVzaW5nVGhyZWFkcyBzZW5kZXIpDQp7DQoJLy9Mb2cuSW5mbygiZW5kIik7DQoNCgl2YXIgZGF0YSA9IHNlbmRlci5Db250ZXh0LkFueURhdGEgYXMgRGF0YTsNCglpZiAoZGF0YSA9PSBudWxsKQ0KCQlyZXR1cm47DQoNCgl2YXIgZ3JvdXBPZk9iamVjdHMgPSBzZW5kZXIuUGFyZW50IGFzIENvbXBvbmVudF9Hcm91cE9mT2JqZWN0czsNCglpZiAoZ3JvdXBPZk9iamVjdHMgIT0gbnVsbCkNCgkJZ3JvdXBPZk9iamVjdHMuT2JqZWN0c1NldChkYXRhLk9iamVjdHMsIHRydWUpOw0KfQ0K")]
public class DynamicClass_387c7331_f825_49f2_adec_99ab8ff4e776
{
    public NeoAxis.Component_CSharpScript Owner;
    // The example of computing a list of objects for the GroupOfObjects using threads.
    class Data
    {
        public Component_GroupOfObjects.Object[] Objects;
    }

    public Component_GroupOfObjects.Object[] Objects;
    public void ComputeUsingThreads_ComputeBegin(NeoAxis.Component_ComputeUsingThreads sender)
    {
        //Log.Info("begin");
        var data = new Data();
        data.Objects = new Component_GroupOfObjects.Object[100 * 100];
        sender.Context.AnyData = data;
    //how to skip one computing:
    //sender.Context.AllowCompute = false;
    }

    public void ComputeUsingThreads_ComputeThread(NeoAxis.Component_ComputeUsingThreads sender, int threadIndex)
    {
        var data = sender.Context.AnyData as Data;
        if (data == null)
            return;
        var objects = data.Objects;
        //get range of objects for the thread
        var from = (data.Objects.Length * threadIndex) / sender.ThreadCount;
        var to = (data.Objects.Length * (threadIndex + 1)) / sender.ThreadCount;
        var random = new NeoAxis.Random(0); //(int)(Time.Current * 0.25));
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
            obj.Flags = Component_GroupOfObjects.Object.FlagsEnum.Enabled | Component_GroupOfObjects.Object.FlagsEnum.Visible;
            obj.Position = new Vector3(x * 1.3, y * 1.3, 1.0 + Math.Sin(wavingStart + Time.Current * wavingCycle) * 0.25);
            obj.Rotation = QuaternionF.Identity;
            obj.Scale = new Vector3F(1, 1, 1);
            obj.Color = new ColorValue(random.NextFloat(), random.NextFloat(), random.NextFloat());
        //var pos = new Vector3(n * 1.3, 0, 1);
        //var rot = QuaternionF.Identity;
        //var scl = new Vector3F(1, 1, 1);
        //data.Objects[n] = new Component_GroupOfObjects.Object(0, 0, 0, Component_GroupOfObjects.Object.FlagsEnum.Enabled | Component_GroupOfObjects.Object.FlagsEnum.Visible, pos, rot, scl, Vector4F.Zero, ColorValue.One, Vector4F.Zero, Vector4F.Zero); 
        }
    }

    public void ComputeUsingThreads_ComputeEnd(NeoAxis.Component_ComputeUsingThreads sender)
    {
        //Log.Info("end");
        var data = sender.Context.AnyData as Data;
        if (data == null)
            return;
        var groupOfObjects = sender.Parent as Component_GroupOfObjects;
        if (groupOfObjects != null)
            groupOfObjects.ObjectsSet(data.Objects, true);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgTWVzaEluU3BhY2VBbmltYXRpb25Db250cm9sbGVyX0NhbGN1bGF0ZUJvbmVUcmFuc2Zvcm1zKE5lb0F4aXMuQ29tcG9uZW50X01lc2hJblNwYWNlQW5pbWF0aW9uQ29udHJvbGxlciBzZW5kZXIsIE5lb0F4aXMuQ29tcG9uZW50X1NrZWxldG9uQW5pbWF0aW9uVHJhY2suQ2FsY3VsYXRlQm9uZVRyYW5zZm9ybXNJdGVtW10gcmVzdWx0KQ0Kew0KCS8vdG8gZW5hYmxlIHRoaXMgZXZlbnQgaGFuZGxlciBpbiB0aGUgZWRpdG9yIGNoYW5nZSAiV2hlbiBFbmFibGUiIHByb3BlcnR5IHRvICJTaW11bGF0aW9uIHwgSW5zdGFuY2UgfCBFZGl0b3IiLg0KCS8vY29tcG9uZW50OiBDaGFyYWN0ZXIvTWVzaCBJbiBTcGFjZS9DIyBTY3JpcHQvRXZlbnQgSGFuZGxlciBDYWxjdWxhdGVCb25lVHJhbnNmb3Jtcy4NCgkNCgl2YXIgYm9uZUluZGV4ID0gc2VuZGVyLkdldEJvbmVJbmRleCgiY2hlc3QiKTsNCglpZihib25lSW5kZXggIT0gLTEpDQoJew0KCQlyZWYgdmFyIGl0ZW0gPSByZWYgcmVzdWx0W2JvbmVJbmRleF07DQoNCgkJLy9jYWxjdWxhdGUgYm9uZSBvZmZzZXQNCgkJdmFyIGFuZ2xlID0gbmV3IERlZ3JlZSg2MCkgKiBNYXRoLlNpbihUaW1lLkN1cnJlbnQpOyANCgkJdmFyIG9mZnNldCA9IE1hdHJpeDNGLkZyb21Sb3RhdGVCeVkoKGZsb2F0KWFuZ2xlLkluUmFkaWFucygpKS5Ub1F1YXRlcm5pb24oKTsNCgkJDQoJCS8vdXBkYXRlIHRoZSBib25lDQoJCWl0ZW0uUm90YXRpb24gKj0gb2Zmc2V0Ow0KCX0JDQp9DQo=")]
public class DynamicClass_d9d15e82_6e01_4088_97bc_f88f3f37e773
{
    public NeoAxis.Component_CSharpScript Owner;
    public void MeshInSpaceAnimationController_CalculateBoneTransforms(NeoAxis.Component_MeshInSpaceAnimationController sender, NeoAxis.Component_SkeletonAnimationTrack.CalculateBoneTransformsItem[] result)
    {
        //to enable this event handler in the editor change "When Enable" property to "Simulation | Instance | Editor".
        //component: Character/Mesh In Space/C# Script/Event Handler CalculateBoneTransforms.
        var boneIndex = sender.GetBoneIndex("chest");
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW5wdXRQcm9jZXNzaW5nX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50X0lucHV0UHJvY2Vzc2luZyBzZW5kZXIsIFVJQ29udHJvbCBwbGF5U2NyZWVuLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQ0Kew0KCS8vZ2V0IGFjY2VzcyB0byB0aGUgc2hpcA0KCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsNCglpZiAoc2hpcCA9PSBudWxsKQ0KCQlyZXR1cm47DQoNCgkvKg0KCQl2YXIga2V5RG93biA9IG1lc3NhZ2UgYXMgSW5wdXRNZXNzYWdlS2V5RG93bjsNCgkJaWYoa2V5RG93biAhPSBudWxsKQ0KCQl7DQoJCQlpZihrZXlEb3duLktleSA9PSBFS2V5cy5TcGFjZSkNCgkJCXsNCgkJCQkvL3ZhciBib2R5ID0gc2hpcC5HZXRDb21wb25lbnQ8Q29tcG9uZW50X1JpZ2lkQm9keTJEPigpOw0KCQkJCS8vaWYgKGJvZHkgIT0gbnVsbCkNCgkJCQkvL3sNCgkJCQkvLwlib2R5LkFwcGx5Rm9yY2UobmV3IFZlY3RvcjIoMSwgMCkpOw0KCQkJCS8vfQ0KCQkJfQ0KCQl9DQoJKi8NCg0KfQ0KDQpwdWJsaWMgdm9pZCBJbnB1dFByb2Nlc3NpbmdfU2ltdWxhdGlvblN0ZXAoTmVvQXhpcy5Db21wb25lbnQgb2JqKQ0Kew0KCXZhciBzZW5kZXIgPSAoTmVvQXhpcy5Db21wb25lbnRfSW5wdXRQcm9jZXNzaW5nKW9iajsNCg0KCS8vZ2V0IGFjY2VzcyB0byB0aGUgc2hpcA0KCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsNCglpZiAoc2hpcCA9PSBudWxsKQ0KCQlyZXR1cm47DQoNCgkvL2NvbnRyb2wgdGhlIHNoaXANCgl2YXIgYm9keSA9IHNoaXAuR2V0Q29tcG9uZW50PENvbXBvbmVudF9SaWdpZEJvZHkyRD4oKTsNCglpZiAoYm9keSAhPSBudWxsKQ0KCXsNCgkJLy9mbHkgZnJvbnQNCgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuVykgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5VcCkpDQoJCXsNCgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOw0KCQkJYm9keS5BcHBseUZvcmNlKGRpciAqIDEuMCk7DQoJCX0NCg0KCQkvL2ZseSBiYWNrDQoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlMpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuRG93bikpDQoJCXsNCgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOw0KCQkJYm9keS5BcHBseUZvcmNlKGRpciAqIC0xLjApOw0KCQl9DQoNCgkJLy90dXJuIGxlZnQNCgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuQSkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5MZWZ0KSkNCgkJCWJvZHkuQXBwbHlUb3JxdWUoMS4wKTsNCg0KCQkvL3R1cm4gcmlnaHQNCgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuRCkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5SaWdodCkpDQoJCQlib2R5LkFwcGx5VG9ycXVlKC0xLjApOw0KCX0NCg0KfQ0K")]
public class DynamicClass_9a05b581_aec5_46ea_9e52_cd32384871b1
{
    public NeoAxis.Component_CSharpScript Owner;
    public void InputProcessing_InputMessageEvent(NeoAxis.Component_InputProcessing sender, UIControl playScreen, NeoAxis.InputMessage message)
    {
        //get access to the ship
        var ship = sender.Parent;
        if (ship == null)
            return;
    /*
		var keyDown = message as InputMessageKeyDown;
		if(keyDown != null)
		{
			if(keyDown.Key == EKeys.Space)
			{
				//var body = ship.GetComponent<Component_RigidBody2D>();
				//if (body != null)
				//{
				//	body.ApplyForce(new Vector2(1, 0));
				//}
			}
		}
	*/
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
            //fly front
            if (sender.IsKeyPressed(EKeys.W) || sender.IsKeyPressed(EKeys.Up))
            {
                var dir = body.TransformV.Rotation.GetForward().ToVector2();
                body.ApplyForce(dir * 1.0);
            }

            //fly back
            if (sender.IsKeyPressed(EKeys.S) || sender.IsKeyPressed(EKeys.Down))
            {
                var dir = body.TransformV.Rotation.GetForward().ToVector2();
                body.ApplyForce(dir * -1.0);
            }

            //turn left
            if (sender.IsKeyPressed(EKeys.A) || sender.IsKeyPressed(EKeys.Left))
                body.ApplyTorque(1.0);
            //turn right
            if (sender.IsKeyPressed(EKeys.D) || sender.IsKeyPressed(EKeys.Right))
                body.ApplyTorque(-1.0);
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW5wdXRQcm9jZXNzaW5nX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50X0lucHV0UHJvY2Vzc2luZyBzZW5kZXIsIFVJQ29udHJvbCBwbGF5U2NyZWVuLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQ0Kew0KCS8vZ2V0IGFjY2VzcyB0byB0aGUgc2hpcA0KCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsNCglpZiAoc2hpcCA9PSBudWxsKQ0KCQlyZXR1cm47DQoNCgkvL3ZhciBrZXlEb3duID0gbWVzc2FnZSBhcyBJbnB1dE1lc3NhZ2VLZXlEb3duOw0KCS8vaWYoa2V5RG93biAhPSBudWxsKQ0KCS8vew0KCS8vCWlmKGtleURvd24uS2V5ID09IEVLZXlzLlNwYWNlKQ0KCS8vCXsNCgkvLwkJLy92YXIgYm9keSA9IHNoaXAuR2V0Q29tcG9uZW50PENvbXBvbmVudF9SaWdpZEJvZHkyRD4oKTsNCgkvLwkJLy9pZiAoYm9keSAhPSBudWxsKQ0KCS8vCQkvL3sNCgkvLwkJLy8JYm9keS5BcHBseUZvcmNlKG5ldyBWZWN0b3IyKDEsIDApKTsNCgkvLwkJLy99DQoJLy8JfQ0KCS8vfQ0KfQ0KDQpwdWJsaWMgdm9pZCBJbnB1dFByb2Nlc3NpbmdfU2ltdWxhdGlvblN0ZXAoTmVvQXhpcy5Db21wb25lbnQgb2JqKQ0Kew0KCXZhciBzZW5kZXIgPSAoTmVvQXhpcy5Db21wb25lbnRfSW5wdXRQcm9jZXNzaW5nKW9iajsNCg0KCS8vZ2V0IGFjY2VzcyB0byB0aGUgc2hpcA0KCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsNCglpZiAoc2hpcCA9PSBudWxsKQ0KCQlyZXR1cm47DQoNCgkvL2NvbnRyb2wgdGhlIHNoaXANCgl2YXIgYm9keSA9IHNoaXAuR2V0Q29tcG9uZW50PENvbXBvbmVudF9SaWdpZEJvZHkyRD4oKTsNCglpZiAoYm9keSAhPSBudWxsKQ0KCXsNCgkJLy9rZXlib2FyZA0KDQoJCS8vZmx5IGZyb250DQoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlcpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuVXApIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkOCkpDQoJCXsNCgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOw0KCQkJYm9keS5BcHBseUZvcmNlKGRpciAqIDEuMCk7DQoJCX0NCg0KCQkvL2ZseSBiYWNrDQoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlMpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuRG93bikgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5OdW1QYWQyKSkNCgkJew0KCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7DQoJCQlib2R5LkFwcGx5Rm9yY2UoZGlyICogLTEuMCk7DQoJCX0NCg0KCQkvL3R1cm4gbGVmdA0KCQlpZiAoc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5BKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkxlZnQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkNCkpDQoJCQlib2R5LkFwcGx5VG9ycXVlKDEuMCk7DQoNCgkJLy90dXJuIHJpZ2h0DQoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuUmlnaHQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkNikpDQoJCQlib2R5LkFwcGx5VG9ycXVlKC0xLjApOw0KDQoJCS8vbW92ZW1lbnQgYnkgam95c3RpY2sgYXhlcw0KCQlpZiAoTWF0aC5BYnMoc2VuZGVyLkpveXN0aWNrQXhlc1swXSkgPj0gMC4wMSkNCgkJCWJvZHkuQXBwbHlUb3JxdWUoLXNlbmRlci5Kb3lzdGlja0F4ZXNbMF0pOw0KCQlpZiAoTWF0aC5BYnMoc2VuZGVyLkpveXN0aWNrQXhlc1sxXSkgPj0gMC4wMSkNCgkJew0KCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7DQoJCQlib2R5LkFwcGx5Rm9yY2UoZGlyICogc2VuZGVyLkpveXN0aWNrQXhlc1sxXSk7DQoJCX0NCgkJLy9Kb3lzdGlja0F4ZXMNCgkJLy9Kb3lzdGlja0J1dHRvbnMNCgkJLy9Kb3lzdGlja1BPVnMNCgkJLy9Kb3lzdGlja1NsaWRlcnMNCgkJLy9Jc0pveXN0aWNrQnV0dG9uUHJlc3NlZA0KCQkvL0dldEpveXN0aWNrQXhpcw0KCQkvL0dldEpveXN0aWNrUE9WDQoJCS8vR2V0Sm95c3RpY2tTbGlkZXIJCQkNCg0KCX0NCg0KfQ0K")]
public class DynamicClass_b47fa3ac_d7ce_459e_be7a_e4dbab068056
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQ0Kew0KCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsNCg0KCXZhciBncm91bmQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkdyb3VuZCIpIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsNCglpZiAoZ3JvdW5kICE9IG51bGwpDQoJew0KCQlpZiAoIWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwuUmVmZXJlbmNlU3BlY2lmaWVkKQ0KCQl7DQoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKA0KCQkJCUAiQ29udGVudFxNYXRlcmlhbHNcQmFzaWMgTGlicmFyeVxDb25jcmV0ZVxDb25jcmV0ZSBGbG9vciAwMS5tYXRlcmlhbCIpOw0KCQl9DQoJCWVsc2UNCgkJCWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwgPSBudWxsOw0KCX0NCn0NCg==")]
public class DynamicClass_fb1e77fd_24bc_4c9a_8692_7da717afacce
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

[CSharpScriptGeneratedAttribute("ZG91YmxlIE1ldGhvZCgpDQp7DQoJcmV0dXJuIE1hdGhFeC5TYXR1cmF0ZSggKCBNYXRoLlNpbiggRW5naW5lQXBwLkVuZ2luZVRpbWUgKiAxLjMgKSArIDEuMCApIC8gMiApOw0KfQ0K")]
public class DynamicClass_23a25ea2_a433_4e57_b2e3_10d01646ad25
{
    public NeoAxis.Component_CSharpScript Owner;
    double Method()
    {
        return MathEx.Saturate((Math.Sin(EngineApp.EngineTime * 1.3) + 1.0) / 2);
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpDQp7DQoJdmFyIGFuZ2xlID0gRW5naW5lQXBwLkVuZ2luZVRpbWUgKiAtMS4wOw0KCXZhciBvZmZzZXQgPSBuZXcgVmVjdG9yMyhNYXRoLkNvcyhhbmdsZSksIE1hdGguU2luKGFuZ2xlKSwgMCkgKiAyLjA7DQoJdmFyIGxvb2tUbyA9IG5ldyBWZWN0b3IzKDExLjczNzQ4MzkxMjQ4MjcsIC0wLjA1MTc3Njc1MDMyNDM5LCAtMTQuODA5Mjc1NTgyNTA5Mik7DQoJdmFyIGxvb2tBdCA9IFF1YXRlcm5pb24uTG9va0F0KC1vZmZzZXQsIG5ldyBWZWN0b3IzKDAsMCwxKSk7DQoJDQoJcmV0dXJuIG5ldyBUcmFuc2Zvcm0oIGxvb2tUbyArIG9mZnNldCwgbG9va0F0LCBWZWN0b3IzLk9uZSApOw0KfQ0K")]
public class DynamicClass_912de908_0e0e_43cb_beb2_a7cfde3ef6f4
{
    public NeoAxis.Component_CSharpScript Owner;
    Transform Method()
    {
        var angle = EngineApp.EngineTime * -1.0;
        var offset = new Vector3(Math.Cos(angle), Math.Sin(angle), 0) * 2.0;
        var lookTo = new Vector3(11.7374839124827, -0.05177675032439, -14.8092755825092);
        var lookAt = Quaternion.LookAt(-offset, new Vector3(0, 0, 1));
        return new Transform(lookTo + offset, lookAt, Vector3.One);
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpDQp7DQoJdmFyIGFuZ2xlID0gRW5naW5lQXBwLkVuZ2luZVRpbWUgKiAxLjM7DQoJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIDIuMDsNCgl2YXIgbG9va1RvID0gbmV3IFZlY3RvcjMoMTEuNzM3NDgzOTEyNDgyNywgLTAuMDUxNzc2NzUwMzI0MzksIC0xNS41MDkyNzU1ODI1MDkyKTsNCgl2YXIgbG9va0F0ID0gUXVhdGVybmlvbi5Mb29rQXQoLW9mZnNldCwgbmV3IFZlY3RvcjMoMCwwLDEpKTsNCgkNCglyZXR1cm4gbmV3IFRyYW5zZm9ybSggbG9va1RvICsgb2Zmc2V0LCBsb29rQXQsIG5ldyBWZWN0b3IzKDAuNSwwLjUsMC41KSApOw0KfQ0K")]
public class DynamicClass_5a776a78_834b_4cba_9fa2_6f0367398fe1
{
    public NeoAxis.Component_CSharpScript Owner;
    Transform Method()
    {
        var angle = EngineApp.EngineTime * 1.3;
        var offset = new Vector3(Math.Cos(angle), Math.Sin(angle), 0) * 2.0;
        var lookTo = new Vector3(11.7374839124827, -0.05177675032439, -15.5092755825092);
        var lookAt = Quaternion.LookAt(-offset, new Vector3(0, 0, 1));
        return new Transform(lookTo + offset, lookAt, new Vector3(0.5, 0.5, 0.5));
    }
}

[CSharpScriptGeneratedAttribute("Q29tcG9uZW50X1JlbmRlcmluZ1BpcGVsaW5lIEdldFBpcGVsaW5lKCkNCnsNCglzdHJpbmcgbmFtZTsNCglpZihFbmdpbmVBcHAuRW5naW5lVGltZSAlIDQgPiAyKQ0KCQluYW1lID0gIlJlbmRlcmluZyBQaXBlbGluZSI7DQoJZWxzZQ0KCQluYW1lID0gIlJlbmRlcmluZyBQaXBlbGluZSAyIjsNCgkJDQoJcmV0dXJuIE93bmVyLlBhcmVudC5HZXRDb21wb25lbnQobmFtZSkgYXMgQ29tcG9uZW50X1JlbmRlcmluZ1BpcGVsaW5lOw0KfQ0K")]
public class DynamicClass_53906342_0dfd_4773_9f74_00708aaa8405
{
    public NeoAxis.Component_CSharpScript Owner;
    Component_RenderingPipeline GetPipeline()
    {
        string name;
        if (EngineApp.EngineTime % 4 > 2)
            name = "Rendering Pipeline";
        else
            name = "Rendering Pipeline 2";
        return Owner.Parent.GetComponent(name) as Component_RenderingPipeline;
    }
}
}
#endif