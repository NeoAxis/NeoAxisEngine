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
public class DynamicClass_2c98fcc0_55ce_4671_b625_bfd1c266b9cb
{
    public NeoAxis.Component_CSharpScript Owner;
    int Method(int a, int b)
    {
        return a + b;
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpDQp7DQoJdmFyIGFuZ2xlID0gRW5naW5lQXBwLkVuZ2luZVRpbWUgKiAwLjM7DQoJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIDIuNTsNCgl2YXIgbG9va1RvID0gbmV3IFZlY3RvcjMoMTEuNzM3NDgzOTEyNDgyNywgLTAuMDUxNzc2NzUwMzI0MzksIC0xNS41MDkyNzU1ODI1MDkyKTsNCgl2YXIgbG9va0F0ID0gUXVhdGVybmlvbi5Mb29rQXQoLW9mZnNldCwgbmV3IFZlY3RvcjMoMCwwLDEpKTsNCgkNCglyZXR1cm4gbmV3IFRyYW5zZm9ybSggbG9va1RvICsgb2Zmc2V0LCBsb29rQXQsIFZlY3RvcjMuT25lICk7DQp9DQo=")]
public class DynamicClass_3b13b8e0_815c_4d4c_9217_8fd35546f368
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
public class DynamicClass_4b386ee8_41d7_466a_89a6_1f4632ad426c
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQ0Kew0KCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsNCg0KCXZhciBsaWdodCA9IHNjZW5lLkdldENvbXBvbmVudCgiTGlnaHQgMSIpIGFzIENvbXBvbmVudF9MaWdodDsNCglpZiAobGlnaHQgIT0gbnVsbCkNCgkJbGlnaHQuRW5hYmxlZCA9IHNlbmRlci5BY3RpdmF0ZWQ7DQp9DQo=")]
public class DynamicClass_561a1ec3_9daa_4bd6_b73e_b8a718026720
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkNCnsNCgl2YXIgX3RoaXMgPSBzZW5kZXIgYXMgQ29tcG9uZW50X1JlZ3VsYXRvclN3aXRjaEluU3BhY2U7DQoJaWYgKF90aGlzICE9IG51bGwpDQoJew0KCQl2YXIgaW5kaWNhdG9yTWluID0gX3RoaXMuR2V0Q29tcG9uZW50KCJJbmRpY2F0b3IgTWluIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOw0KCQlpZiAoaW5kaWNhdG9yTWluICE9IG51bGwpDQoJCQlpbmRpY2F0b3JNaW4uQ29sb3IgPSBfdGhpcy5WYWx1ZS5WYWx1ZSA8PSBfdGhpcy5WYWx1ZVJhbmdlLlZhbHVlLk1pbmltdW0gPyBuZXcgQ29sb3JWYWx1ZSgxLCAwLCAwKSA6IG5ldyBDb2xvclZhbHVlKDAuNSwgMC41LCAwLjUpOw0KDQoJCXZhciBpbmRpY2F0b3JNYXggPSBfdGhpcy5HZXRDb21wb25lbnQoIkluZGljYXRvciBNYXgiKSBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7DQoJCWlmIChpbmRpY2F0b3JNYXggIT0gbnVsbCkNCgkJCWluZGljYXRvck1heC5Db2xvciA9IF90aGlzLlZhbHVlLlZhbHVlID49IF90aGlzLlZhbHVlUmFuZ2UuVmFsdWUuTWF4aW11bSA/IG5ldyBDb2xvclZhbHVlKDAsIDEsIDApIDogbmV3IENvbG9yVmFsdWUoMC41LCAwLjUsIDAuNSk7DQoNCgkJdmFyIGJ1dHRvbiA9IF90aGlzLkdldENvbXBvbmVudCgiQnV0dG9uIik7DQoJCWlmIChidXR0b24gIT0gbnVsbCkNCgkJew0KCQkJdmFyIG9mZnNldCA9IGJ1dHRvbi5HZXRDb21wb25lbnQ8Q29tcG9uZW50X1RyYW5zZm9ybU9mZnNldD4oKTsNCgkJCWlmIChvZmZzZXQgIT0gbnVsbCkNCgkJCXsNCgkJCQl2YXIgYW5nbGUgPSBfdGhpcy5HZXRWYWx1ZUFuZ2xlKCkgLSA5MDsNCgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsNCgkJCX0NCgkJfQ0KDQoJCXZhciBtYXJrZXJNaW4gPSBfdGhpcy5HZXRDb21wb25lbnQoIk1hcmtlciBNaW4iKTsNCgkJaWYgKG1hcmtlck1pbiAhPSBudWxsKQ0KCQl7DQoJCQl2YXIgb2Zmc2V0ID0gbWFya2VyTWluLkdldENvbXBvbmVudDxDb21wb25lbnRfVHJhbnNmb3JtT2Zmc2V0PigpOw0KCQkJaWYgKG9mZnNldCAhPSBudWxsKQ0KCQkJew0KCQkJCXZhciBhbmdsZSA9IF90aGlzLkFuZ2xlUmFuZ2UuVmFsdWUuTWluaW11bSAtIDkwOw0KCQkJCXZhciBhbmdsZVIgPSBNYXRoRXguRGVncmVlVG9SYWRpYW4oYW5nbGUpOw0KCQkJCW9mZnNldC5Qb3NpdGlvbk9mZnNldCA9IG5ldyBWZWN0b3IzKDAuMDEsIE1hdGguQ29zKGFuZ2xlUikgKiAwLjA0LCBNYXRoLlNpbigtYW5nbGVSKSAqIDAuMDQpOw0KCQkJCW9mZnNldC5Sb3RhdGlvbk9mZnNldCA9IG5ldyBBbmdsZXMoYW5nbGUsIDAsIDApLlRvUXVhdGVybmlvbigpOw0KCQkJfQ0KCQl9DQoNCgkJdmFyIG1hcmtlck1heCA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIE1heCIpOw0KCQlpZiAobWFya2VyTWF4ICE9IG51bGwpDQoJCXsNCgkJCXZhciBvZmZzZXQgPSBtYXJrZXJNYXguR2V0Q29tcG9uZW50PENvbXBvbmVudF9UcmFuc2Zvcm1PZmZzZXQ+KCk7DQoJCQlpZiAob2Zmc2V0ICE9IG51bGwpDQoJCQl7DQoJCQkJdmFyIGFuZ2xlID0gX3RoaXMuQW5nbGVSYW5nZS5WYWx1ZS5NYXhpbXVtIC0gOTA7DQoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7DQoJCQkJb2Zmc2V0LlBvc2l0aW9uT2Zmc2V0ID0gbmV3IFZlY3RvcjMoMC4wMSwgTWF0aC5Db3MoYW5nbGVSKSAqIDAuMDQsIE1hdGguU2luKC1hbmdsZVIpICogMC4wNCk7DQoJCQkJb2Zmc2V0LlJvdGF0aW9uT2Zmc2V0ID0gbmV3IEFuZ2xlcyhhbmdsZSwgMCwgMCkuVG9RdWF0ZXJuaW9uKCk7DQoJCQl9DQoJCX0NCg0KCQl2YXIgbWFya2VyQ3VycmVudCA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIEN1cnJlbnQiKTsNCgkJaWYgKG1hcmtlckN1cnJlbnQgIT0gbnVsbCkNCgkJew0KCQkJdmFyIG9mZnNldCA9IG1hcmtlckN1cnJlbnQuR2V0Q29tcG9uZW50PENvbXBvbmVudF9UcmFuc2Zvcm1PZmZzZXQ+KCk7DQoJCQlpZiAob2Zmc2V0ICE9IG51bGwpDQoJCQl7DQoJCQkJdmFyIGFuZ2xlID0gX3RoaXMuR2V0VmFsdWVBbmdsZSgpIC0gOTA7DQoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7DQoJCQkJb2Zmc2V0LlBvc2l0aW9uT2Zmc2V0ID0gbmV3IFZlY3RvcjMoMC4wNiwgTWF0aC5Db3MoYW5nbGVSKSAqIDAuMDQsIE1hdGguU2luKC1hbmdsZVIpICogMC4wNCk7DQoJCQkJb2Zmc2V0LlJvdGF0aW9uT2Zmc2V0ID0gbmV3IEFuZ2xlcyhhbmdsZSwgMCwgMCkuVG9RdWF0ZXJuaW9uKCk7DQoJCQl9DQoJCX0NCgl9DQp9")]
public class DynamicClass_3ea101ad_23f8_47c5_81a2_4259f92623db
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikNCnsNCgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7DQoNCgl2YXIgbGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkxpZ2h0IDEiKSBhcyBDb21wb25lbnRfTGlnaHQ7DQoJaWYgKGxpZ2h0ICE9IG51bGwpDQoJCWxpZ2h0LkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMS4wLCAxLjAsIDEuMCAtIG9iai5WYWx1ZSk7DQp9DQo=")]
public class DynamicClass_0b92b0b9_31b7_47a4_9aca_47e536c76e41
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW5wdXRQcm9jZXNzaW5nX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50X0lucHV0UHJvY2Vzc2luZyBzZW5kZXIsIFVJQ29udHJvbCBwbGF5U2NyZWVuLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQ0Kew0KCS8vZ2V0IGFjY2VzcyB0byB0aGUgc2hpcA0KCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsNCglpZiAoc2hpcCA9PSBudWxsKQ0KCQlyZXR1cm47DQoNCgkvL3ZhciBrZXlEb3duID0gbWVzc2FnZSBhcyBJbnB1dE1lc3NhZ2VLZXlEb3duOw0KCS8vaWYoa2V5RG93biAhPSBudWxsKQ0KCS8vew0KCS8vCWlmKGtleURvd24uS2V5ID09IEVLZXlzLlNwYWNlKQ0KCS8vCXsNCgkvLwkJLy92YXIgYm9keSA9IHNoaXAuR2V0Q29tcG9uZW50PENvbXBvbmVudF9SaWdpZEJvZHkyRD4oKTsNCgkvLwkJLy9pZiAoYm9keSAhPSBudWxsKQ0KCS8vCQkvL3sNCgkvLwkJLy8JYm9keS5BcHBseUZvcmNlKG5ldyBWZWN0b3IyKDEsIDApKTsNCgkvLwkJLy99DQoJLy8JfQ0KCS8vfQ0KfQ0KDQpwdWJsaWMgdm9pZCBJbnB1dFByb2Nlc3NpbmdfU2ltdWxhdGlvblN0ZXAoTmVvQXhpcy5Db21wb25lbnQgb2JqKQ0Kew0KCXZhciBzZW5kZXIgPSAoTmVvQXhpcy5Db21wb25lbnRfSW5wdXRQcm9jZXNzaW5nKW9iajsNCg0KCS8vZ2V0IGFjY2VzcyB0byB0aGUgc2hpcA0KCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsNCglpZiAoc2hpcCA9PSBudWxsKQ0KCQlyZXR1cm47DQoNCgkvL2NvbnRyb2wgdGhlIHNoaXANCgl2YXIgYm9keSA9IHNoaXAuR2V0Q29tcG9uZW50PENvbXBvbmVudF9SaWdpZEJvZHkyRD4oKTsNCglpZiAoYm9keSAhPSBudWxsKQ0KCXsNCgkJLy9rZXlib2FyZA0KDQoJCS8vZmx5IGZyb250DQoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlcpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuVXApIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkOCkpDQoJCXsNCgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOw0KCQkJYm9keS5BcHBseUZvcmNlKGRpciAqIDEuMCk7DQoJCX0NCg0KCQkvL2ZseSBiYWNrDQoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlMpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuRG93bikgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5OdW1QYWQyKSkNCgkJew0KCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7DQoJCQlib2R5LkFwcGx5Rm9yY2UoZGlyICogLTEuMCk7DQoJCX0NCg0KCQkvL3R1cm4gbGVmdA0KCQlpZiAoc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5BKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkxlZnQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkNCkpDQoJCQlib2R5LkFwcGx5VG9ycXVlKDEuMCk7DQoNCgkJLy90dXJuIHJpZ2h0DQoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuUmlnaHQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkNikpDQoJCQlib2R5LkFwcGx5VG9ycXVlKC0xLjApOw0KDQoJCS8vbW92ZW1lbnQgYnkgam95c3RpY2sgYXhlcw0KCQlpZiAoTWF0aC5BYnMoc2VuZGVyLkpveXN0aWNrQXhlc1swXSkgPj0gMC4wMSkNCgkJCWJvZHkuQXBwbHlUb3JxdWUoLXNlbmRlci5Kb3lzdGlja0F4ZXNbMF0pOw0KCQlpZiAoTWF0aC5BYnMoc2VuZGVyLkpveXN0aWNrQXhlc1sxXSkgPj0gMC4wMSkNCgkJew0KCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7DQoJCQlib2R5LkFwcGx5Rm9yY2UoZGlyICogc2VuZGVyLkpveXN0aWNrQXhlc1sxXSk7DQoJCX0NCgkJLy9Kb3lzdGlja0F4ZXMNCgkJLy9Kb3lzdGlja0J1dHRvbnMNCgkJLy9Kb3lzdGlja1BPVnMNCgkJLy9Kb3lzdGlja1NsaWRlcnMNCgkJLy9Jc0pveXN0aWNrQnV0dG9uUHJlc3NlZA0KCQkvL0dldEpveXN0aWNrQXhpcw0KCQkvL0dldEpveXN0aWNrUE9WDQoJCS8vR2V0Sm95c3RpY2tTbGlkZXIJCQkNCg0KCX0NCg0KfQ0K")]
public class DynamicClass_b8750f61_7b05_4451_a97d_d98995b60cc3
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

[CSharpScriptGeneratedAttribute("UXVhdGVybmlvbiBHZXRSb3RhdGlvbk9mZnNldCgpDQp7DQogICAgdmFyIHNwZWVkID0gLTAuMTsNCiAgICB2YXIgbWF0ID0gTWF0cml4My5Gcm9tUm90YXRlQnlYKEVuZ2luZUFwcC5FbmdpbmVUaW1lICogc3BlZWQpOw0KICAgIHJldHVybiBtYXQuVG9RdWF0ZXJuaW9uKCk7DQp9")]
public class DynamicClass_a9a446dd_55c4_4bb7_b671_0f5f5a5625d4
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
public class DynamicClass_ea22d034_e8df_49f7_8d9d_246cdac5c839
{
    public NeoAxis.Component_CSharpScript Owner;
    double Method()
    {
        return -EngineApp.EngineTime / 5;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikNCnsNCgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7DQoNCgl2YXIgbWVzaEluU3BhY2UgPSBzY2VuZS5HZXRDb21wb25lbnQoIkdyb3VuZCIpIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsNCglpZiAobWVzaEluU3BhY2UgIT0gbnVsbCkNCgkJbWVzaEluU3BhY2UuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLjAgLSBvYmouVmFsdWUsIDEuMCwgMS4wIC0gb2JqLlZhbHVlKTsNCn0NCg==")]
public class DynamicClass_bab298c3_637c_4abb_8158_cebb7a2961e0
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW5wdXRQcm9jZXNzaW5nX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50X0lucHV0UHJvY2Vzc2luZyBzZW5kZXIsIE5lb0F4aXMuVUlDb250cm9sIHBsYXlTY3JlZW4sIE5lb0F4aXMuQ29tcG9uZW50X0dhbWVNb2RlIGdhbWVNb2RlLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQ0Kew0KCXZhciBjYXIgPSBPd25lci5QYXJlbnQuUGFyZW50Ow0KCWlmIChjYXIgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQp9DQoNCnB1YmxpYyB2b2lkIElucHV0UHJvY2Vzc2luZ19TaW11bGF0aW9uU3RlcChOZW9BeGlzLkNvbXBvbmVudCBvYmopDQp7DQoJdmFyIHNlbmRlciA9IChOZW9BeGlzLkNvbXBvbmVudF9JbnB1dFByb2Nlc3Npbmcpb2JqOw0KDQoJLy9nZXQgY2FyIG9iamVjdA0KCXZhciBjYXIgPSBPd25lci5QYXJlbnQuUGFyZW50Ow0KCWlmIChjYXIgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQoJLy9nZXQgY29udHJvbCBkYXRhDQoJZG91YmxlIHRocm90dGxlID0gMDsNCglpZiAoc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5XKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlVwKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLk51bVBhZDgpKQ0KCQl0aHJvdHRsZSArPSAxOw0KCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlMpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuRG93bikgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5OdW1QYWQyKSkNCgkJdGhyb3R0bGUgLT0gMTsNCglkb3VibGUgc3RlZXJpbmcgPSAwOw0KCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkEpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTGVmdCkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5OdW1QYWQ0KSkNCgkJc3RlZXJpbmcgLT0gMTsNCglpZiAoc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5EKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlJpZ2h0KSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLk51bVBhZDYpKQ0KCQlzdGVlcmluZyArPSAxOw0KCXZhciBicmFrZSA9IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuU3BhY2UpOw0KDQoJLy9nZXQgZnJvbnQgY29uc3RyYWludHMNCgl2YXIgZnJvbnRDb25zdHJhaW50cyA9IG5ldyBMaXN0PENvbXBvbmVudF9Db25zdHJhaW50PigpOw0KCXsNCgkJdmFyIHJpZ2h0ID0gY2FyLkdldENvbXBvbmVudCgiRnJvbnQgUmlnaHQgQ29uc3RyYWludCIpIGFzIENvbXBvbmVudF9Db25zdHJhaW50Ow0KCQlpZiAocmlnaHQgIT0gbnVsbCkNCgkJCWZyb250Q29uc3RyYWludHMuQWRkKHJpZ2h0KTsNCgkJdmFyIGxlZnQgPSBjYXIuR2V0Q29tcG9uZW50KCJGcm9udCBMZWZ0IENvbnN0cmFpbnQiKSBhcyBDb21wb25lbnRfQ29uc3RyYWludDsNCgkJaWYgKGxlZnQgIT0gbnVsbCkNCgkJCWZyb250Q29uc3RyYWludHMuQWRkKGxlZnQpOw0KCX0NCg0KCS8vZ2V0IGJhY2sgY29uc3RyYWludHMNCgl2YXIgYmFja0NvbnN0cmFpbnRzID0gbmV3IExpc3Q8Q29tcG9uZW50X0NvbnN0cmFpbnQ+KCk7DQoJew0KCQl2YXIgcmlnaHQgPSBjYXIuR2V0Q29tcG9uZW50KCJCYWNrIFJpZ2h0IENvbnN0cmFpbnQiKSBhcyBDb21wb25lbnRfQ29uc3RyYWludDsNCgkJaWYgKHJpZ2h0ICE9IG51bGwpDQoJCQliYWNrQ29uc3RyYWludHMuQWRkKHJpZ2h0KTsNCgkJdmFyIGxlZnQgPSBjYXIuR2V0Q29tcG9uZW50KCJCYWNrIExlZnQgQ29uc3RyYWludCIpIGFzIENvbXBvbmVudF9Db25zdHJhaW50Ow0KCQlpZiAobGVmdCAhPSBudWxsKQ0KCQkJYmFja0NvbnN0cmFpbnRzLkFkZChsZWZ0KTsNCgl9DQoNCgkvLyEhISFlbmdpbmUgYnVnIGZpeC4gbW90b3IgZG9lc24ndCB3YWtlIHVwIHJpZ2lkIGJvZHkNCglpZiAodGhyb3R0bGUgIT0gMCB8fCBicmFrZSkNCgl7DQoJCXZhciBib2R5ID0gY2FyLkdldENvbXBvbmVudCgiQ29sbGlzaW9uIEJvZHkiKSBhcyBDb21wb25lbnRfUmlnaWRCb2R5Ow0KCQlpZiAoYm9keSAhPSBudWxsKQ0KCQkJYm9keS5SaWdpZEJvZHkuQWN0aXZhdGUoKTsNCgl9DQoNCgkvL3VwZGF0ZSBmcm9udCBjb25zdHJhaW50cw0KCWZvcmVhY2ggKHZhciBjb25zdHJhaW50IGluIGZyb250Q29uc3RyYWludHMpDQoJew0KCQlpZiAoYnJha2UpDQoJCXsNCgkJCWNvbnN0cmFpbnQuQW5ndWxhckF4aXNYTW90b3JUYXJnZXRWZWxvY2l0eSA9IDA7DQoJCQljb25zdHJhaW50LkFuZ3VsYXJBeGlzWE1vdG9yTWF4Rm9yY2UgPSAxMDsNCgkJfQ0KCQllbHNlDQoJCXsNCgkJCWNvbnN0cmFpbnQuQW5ndWxhckF4aXNYTW90b3JUYXJnZXRWZWxvY2l0eSA9IDE1LjAgKiAtdGhyb3R0bGU7DQoJCQljb25zdHJhaW50LkFuZ3VsYXJBeGlzWE1vdG9yTWF4Rm9yY2UgPSB0aHJvdHRsZSAhPSAwID8gMC41IDogMDsNCgkJfQ0KDQoJCWNvbnN0cmFpbnQuQW5ndWxhckF4aXNaU2Vydm9UYXJnZXQgPSBjb25zdHJhaW50LkFuZ3VsYXJBeGlzWkxpbWl0SGlnaC5WYWx1ZSAqIHN0ZWVyaW5nOw0KCQljb25zdHJhaW50LkFuZ3VsYXJBeGlzWk1vdG9yVGFyZ2V0VmVsb2NpdHkgPSAxOw0KCQljb25zdHJhaW50LkFuZ3VsYXJBeGlzWk1vdG9yTWF4Rm9yY2UgPSAxMDsNCgl9DQoNCgkvL3VwZGF0ZSBiYWNrIGNvbnN0cmFpbnRzDQoJZm9yZWFjaCAodmFyIGNvbnN0cmFpbnQgaW4gYmFja0NvbnN0cmFpbnRzKQ0KCXsNCgkJY29uc3RyYWludC5Bbmd1bGFyQXhpc1pNb3RvclRhcmdldFZlbG9jaXR5ID0gMDsNCgkJY29uc3RyYWludC5Bbmd1bGFyQXhpc1pNb3Rvck1heEZvcmNlID0gYnJha2UgPyAxMCA6IDA7DQoJfQ0KDQp9DQo=")]
public class DynamicClass_7d594c37_09da_4cab_b085_697b5f106460
{
    public NeoAxis.Component_CSharpScript Owner;
    public void InputProcessing_InputMessageEvent(NeoAxis.Component_InputProcessing sender, NeoAxis.UIControl playScreen, NeoAxis.Component_GameMode gameMode, NeoAxis.InputMessage message)
    {
        var car = Owner.Parent.Parent;
        if (car == null)
            return;
    }

    public void InputProcessing_SimulationStep(NeoAxis.Component obj)
    {
        var sender = (NeoAxis.Component_InputProcessing)obj;
        //get car object
        var car = Owner.Parent.Parent;
        if (car == null)
            return;
        //get control data
        double throttle = 0;
        if (sender.IsKeyPressed(EKeys.W) || sender.IsKeyPressed(EKeys.Up) || sender.IsKeyPressed(EKeys.NumPad8))
            throttle += 1;
        if (sender.IsKeyPressed(EKeys.S) || sender.IsKeyPressed(EKeys.Down) || sender.IsKeyPressed(EKeys.NumPad2))
            throttle -= 1;
        double steering = 0;
        if (sender.IsKeyPressed(EKeys.A) || sender.IsKeyPressed(EKeys.Left) || sender.IsKeyPressed(EKeys.NumPad4))
            steering -= 1;
        if (sender.IsKeyPressed(EKeys.D) || sender.IsKeyPressed(EKeys.Right) || sender.IsKeyPressed(EKeys.NumPad6))
            steering += 1;
        var brake = sender.IsKeyPressed(EKeys.Space);
        //get front constraints
        var frontConstraints = new List<Component_Constraint>();
        {
            var right = car.GetComponent("Front Right Constraint") as Component_Constraint;
            if (right != null)
                frontConstraints.Add(right);
            var left = car.GetComponent("Front Left Constraint") as Component_Constraint;
            if (left != null)
                frontConstraints.Add(left);
        }

        //get back constraints
        var backConstraints = new List<Component_Constraint>();
        {
            var right = car.GetComponent("Back Right Constraint") as Component_Constraint;
            if (right != null)
                backConstraints.Add(right);
            var left = car.GetComponent("Back Left Constraint") as Component_Constraint;
            if (left != null)
                backConstraints.Add(left);
        }

        //!!!!engine bug fix. motor doesn't wake up rigid body
        if (throttle != 0 || brake)
        {
            var body = car.GetComponent("Collision Body") as Component_RigidBody;
            if (body != null)
                body.RigidBody.Activate();
        }

        //update front constraints
        foreach (var constraint in frontConstraints)
        {
            if (brake)
            {
                constraint.AngularAxisXMotorTargetVelocity = 0;
                constraint.AngularAxisXMotorMaxForce = 10;
            }
            else
            {
                constraint.AngularAxisXMotorTargetVelocity = 15.0 * -throttle;
                constraint.AngularAxisXMotorMaxForce = throttle != 0 ? 0.5 : 0;
            }

            constraint.AngularAxisZServoTarget = constraint.AngularAxisZLimitHigh.Value * steering;
            constraint.AngularAxisZMotorTargetVelocity = 1;
            constraint.AngularAxisZMotorMaxForce = 10;
        }

        //update back constraints
        foreach (var constraint in backConstraints)
        {
            constraint.AngularAxisZMotorTargetVelocity = 0;
            constraint.AngularAxisZMotorMaxForce = brake ? 10 : 0;
        }
    }
}

[CSharpScriptGeneratedAttribute("Y2xhc3MgX1RlbXB7DQp9")]
public class DynamicClass_02ecc08a_7d58_49c7_9897_bcd39fb3aa8d
{
    public NeoAxis.Component_CSharpScript Owner;
    class _Temp
    {
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQm94X1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpDQp7DQp9DQo=")]
public class DynamicClass_255b56b4_b341_4ad4_aff7_4102fe01ca81
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Box_UpdateEvent(NeoAxis.Component sender, float delta)
    {
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQm94X1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpDQp7DQoJdmFyIG9iaiA9IE93bmVyLlBhcmVudCBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7DQoJaWYob2JqICE9IG51bGwpDQoJew0KCQl2YXIgb2Zmc2V0ID0gbmV3IFZlY3RvcjIoTWF0aC5Db3MoIFRpbWUuQ3VycmVudCksIE1hdGguU2luKCBUaW1lLkN1cnJlbnQpKTsNCg0KCQlvYmouU2V0UG9zaXRpb24obmV3IFZlY3RvcjMoMi45MTk5MTIzOTU0Nzc0NyArIG9mZnNldC5YLCA2LjIxNjg0NjM0Mjk4NDI1ICsgb2Zmc2V0LlksIC0wLjUpKTsNCgl9DQp9DQo=")]
public class DynamicClass_f4f67947_98a7_41ea_8785_9bdbd749b0b4
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Box_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var obj = Owner.Parent as Component_MeshInSpace;
        if (obj != null)
        {
            var offset = new Vector2(Math.Cos(Time.Current), Math.Sin(Time.Current));
            obj.SetPosition(new Vector3(2.91991239547747 + offset.X, 6.21684634298425 + offset.Y, -0.5));
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQm94X1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpDQp7DQoJdmFyIG9iaiA9IE93bmVyLlBhcmVudCBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7DQoJaWYob2JqICE9IG51bGwpDQoJew0KCQl2YXIgb2Zmc2V0ID0gbmV3IFZlY3RvcjIoTWF0aC5Db3MoIFRpbWUuQ3VycmVudCksIE1hdGguU2luKCBUaW1lLkN1cnJlbnQpKTsNCgkJb2JqLlNldFBvc2l0aW9uKG5ldyBWZWN0b3IzKDIuOTE5OTEyMzk1NDc3NDcgKyBvZmZzZXQuWCwgNi4yMTY4NDYzNDI5ODQyNSArIG9mZnNldC5ZLCAtMC41KSk7DQoJfQ0KfQ0K")]
public class DynamicClass_6ca60a71_e34f_4a29_8bd0_04b234fb99e9
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Box_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var obj = Owner.Parent as Component_MeshInSpace;
        if (obj != null)
        {
            var offset = new Vector2(Math.Cos(Time.Current), Math.Sin(Time.Current));
            obj.SetPosition(new Vector3(2.91991239547747 + offset.X, 6.21684634298425 + offset.Y, -0.5));
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQm94X1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpDQp7DQoJdmFyIG9iaiA9IE93bmVyLlBhcmVudCBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7DQoJaWYgKG9iaiAhPSBudWxsKQ0KCXsNCgkJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IyKE1hdGguQ29zKFRpbWUuQ3VycmVudCksIE1hdGguU2luKFRpbWUuQ3VycmVudCkpOw0KCQlvYmouU2V0UG9zaXRpb24obmV3IFZlY3RvcjMoMi45MTk5MTIzOTU0Nzc0NyArIG9mZnNldC5YLCA2LjIxNjg0NjM0Mjk4NDI1ICsgb2Zmc2V0LlksIC0wLjUpKTsNCgl9DQp9DQo=")]
public class DynamicClass_68e248de_2839_401a_992d_001f31e7d1c6
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Box_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var obj = Owner.Parent as Component_MeshInSpace;
        if (obj != null)
        {
            var offset = new Vector2(Math.Cos(Time.Current), Math.Sin(Time.Current));
            obj.SetPosition(new Vector3(2.91991239547747 + offset.X, 6.21684634298425 + offset.Y, -0.5));
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQ0Kew0KCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsNCg0KCXZhciBncm91bmQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkdyb3VuZCIpIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsNCglpZiAoZ3JvdW5kICE9IG51bGwpDQoJew0KCQlpZiAoIWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwuUmVmZXJlbmNlU3BlY2lmaWVkKQ0KCQl7DQoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKA0KCQkJCUAiU2FtcGxlc1xTdGFydGVyIENvbnRlbnRcTWF0ZXJpYWxzXENvbmNyZXRlIDN4MyBtZXRlcnNcQ29uY3JldGUgM3gzIG1ldGVycy5tYXRlcmlhbCIpOw0KCQl9DQoJCWVsc2UNCgkJCWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwgPSBudWxsOw0KCX0NCn0NCg==")]
public class DynamicClass_38468059_775c_4bb5_aceb_a35333b3bacd
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikNCnsNCgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7DQoNCgl2YXIgZ3JvdW5kID0gc2NlbmUuR2V0Q29tcG9uZW50KCJHcm91bmQiKSBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7DQoJaWYgKGdyb3VuZCAhPSBudWxsKQ0KCQlncm91bmQuQ29sb3IgPSBDb2xvclZhbHVlLkxlcnAobmV3IENvbG9yVmFsdWUoMSwgMSwgMSksIG5ldyBDb2xvclZhbHVlKDAuNCwgMC45LCAwLjQpLCAoZmxvYXQpb2JqLlZhbHVlKTsNCn0NCg==")]
public class DynamicClass_8dd8d3cf_5de2_4f9b_b3a7_0d7b2f6b324b
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQm94X1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpDQp7DQoJLy8hISEhDQoJTG9nLkluZm8oU2ltdWxhdGlvbkFwcC5WaWRlb01vZGUuVG9TdHJpbmcoKSk7DQoJDQoJdmFyIG9iaiA9IE93bmVyLlBhcmVudCBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7DQoJaWYgKG9iaiAhPSBudWxsKQ0KCXsNCgkJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IyKE1hdGguQ29zKFRpbWUuQ3VycmVudCksIE1hdGguU2luKFRpbWUuQ3VycmVudCkpOw0KCQlvYmouU2V0UG9zaXRpb24obmV3IFZlY3RvcjMoMi45MTk5MTIzOTU0Nzc0NyArIG9mZnNldC5YLCA2LjIxNjg0NjM0Mjk4NDI1ICsgb2Zmc2V0LlksIC0wLjUpKTsNCgl9DQp9DQo=")]
public class DynamicClass_87488758_d54d_44e8_a372_79daaeefd9df
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Box_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        //!!!!
        Log.Info(SimulationApp.VideoMode.ToString());
        var obj = Owner.Parent as Component_MeshInSpace;
        if (obj != null)
        {
            var offset = new Vector2(Math.Cos(Time.Current), Math.Sin(Time.Current));
            obj.SetPosition(new Vector3(2.91991239547747 + offset.X, 6.21684634298425 + offset.Y, -0.5));
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uMV9DbGljayhOZW9BeGlzLlVJQnV0dG9uIHNlbmRlcikNCnsNCn0NCg==")]
public class DynamicClass_e5217de5_e881_4714_adff_898175c0f087
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Button1_Click(NeoAxis.UIButton sender)
    {
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uMV9DbGljayhOZW9BeGlzLlVJQnV0dG9uIHNlbmRlcikNCnsNCgkvL2FjY2VzcyB0byBVSUNvbnRyb2wNCgl2YXIgY29udHJvbCA9IHNlbmRlci5QYXJlbnRSb290IGFzIFVJQ29udHJvbDsNCgkNCglNZXNzYWdlQm94V2luZG93LlNob3dJbmZvKGNvbnRyb2wsICJUaGUgdGV4dCBvZiB0aGUgbWVzc2FnZS4iLCAiTWVzc2FnZSIpOw0KfQ0K")]
public class DynamicClass_564d9742_3fa2_4807_ba3e_58b472647f43
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Button1_Click(NeoAxis.UIButton sender)
    {
        //access to UIControl
        var control = sender.ParentRoot as UIControl;
        MessageBoxWindow.ShowInfo(control, "The text of the message.", "Message");
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uMV9DbGljayhOZW9BeGlzLlVJQnV0dG9uIHNlbmRlcikNCnsNCgkvL2FjY2VzcyB0byBVSUNvbnRyb2wNCgl2YXIgY29udHJvbCA9IHNlbmRlci5QYXJlbnRSb290IGFzIFVJQ29udHJvbDsNCg0KCS8vc2hvdyBtZXNzYWdlIGJveA0KCU1lc3NhZ2VCb3hXaW5kb3cuU2hvd0luZm8oY29udHJvbCwgIlRoZSB0ZXh0IG9mIHRoZSBtZXNzYWdlLiIsICJNZXNzYWdlIik7DQp9DQo=")]
public class DynamicClass_34a8814e_340c_46d6_b1a0_0d0899027f3f
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Button1_Click(NeoAxis.UIButton sender)
    {
        //access to UIControl
        var control = sender.ParentRoot as UIControl;
        //show message box
        MessageBoxWindow.ShowInfo(control, "The text of the message.", "Message");
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uMV9DbGljayhOZW9BeGlzLlVJQnV0dG9uIHNlbmRlcikNCnsNCgkvL2dldCBVSUNvbnRyb2wNCgl2YXIgY29udHJvbCA9IHNlbmRlci5QYXJlbnRSb290IGFzIFVJQ29udHJvbDsNCg0KCS8vc2hvdyBtZXNzYWdlIGJveA0KCU1lc3NhZ2VCb3hXaW5kb3cuU2hvd0luZm8oY29udHJvbCwgIlRoZSB0ZXh0IG9mIHRoZSBtZXNzYWdlLiIsICJNZXNzYWdlIik7DQp9DQo=")]
public class DynamicClass_a3c01986_a438_48b2_9bae_6459e216d95f
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Button1_Click(NeoAxis.UIButton sender)
    {
        //get UIControl
        var control = sender.ParentRoot as UIControl;
        //show message box
        MessageBoxWindow.ShowInfo(control, "The text of the message.", "Message");
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uMl9DbGljayhOZW9BeGlzLlVJQnV0dG9uIHNlbmRlcikNCnsNCn0NCg==")]
public class DynamicClass_36e86dd3_50fa_4fd8_8467_67fb6bf72dd2
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Button2_Click(NeoAxis.UIButton sender)
    {
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uMl9DbGljayhOZW9BeGlzLlVJQnV0dG9uIHNlbmRlcikNCnsNCgkvL2dldCBVSUNvbnRyb2wNCgl2YXIgY29udHJvbCA9IHNlbmRlci5QYXJlbnRSb290IGFzIFVJQ29udHJvbDsNCg0KCS8vc2hvdyBtZXNzYWdlIGJveA0KCU1lc3NhZ2VCb3hXaW5kb3cuU2hvd0luZm8oY29udHJvbCwgIlRoZSB0ZXh0IG9mIHRoZSBtZXNzYWdlLiIsICJNZXNzYWdlIik7CQ0KfQ0K")]
public class DynamicClass_95b3fa2c_d777_4be5_a5f7_83f60c5864fa
{
    public NeoAxis.Component_CSharpScript Owner;
    public void Button2_Click(NeoAxis.UIButton sender)
    {
        //get UIControl
        var control = sender.ParentRoot as UIControl;
        //show message box
        MessageBoxWindow.ShowInfo(control, "The text of the message.", "Message");
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgTWVzaEluU3BhY2VBbmltYXRpb25Db250cm9sbGVyX0NhbGN1bGF0ZUJvbmVUcmFuc2Zvcm1zKE5lb0F4aXMuQ29tcG9uZW50X01lc2hJblNwYWNlQW5pbWF0aW9uQ29udHJvbGxlciBzZW5kZXIsIE5lb0F4aXMuQ29tcG9uZW50X1NrZWxldG9uQW5pbWF0aW9uVHJhY2suQ2FsY3VsYXRlQm9uZVRyYW5zZm9ybXNJdGVtW10gcmVzdWx0KQ0Kew0KCS8vdG8gZW5hYmxlIHRoaXMgZXZlbnQgaGFuZGxlciBpbiB0aGUgZWRpdG9yIGNoYW5nZSAiV2hlbiBFbmFibGUiIHByb3BlcnR5IHRvICJTaW11bGF0aW9uIHwgSW5zdGFuY2UgfCBFZGl0b3IiLg0KCS8vY29tcG9uZW50OiBDaGFyYWN0ZXIvTWVzaCBJbiBTcGFjZS9DIyBTY3JpcHQvRXZlbnQgSGFuZGxlciBDYWxjdWxhdGVCb25lVHJhbnNmb3Jtcy4NCgkNCgl2YXIgYm9uZUluZGV4ID0gc2VuZGVyLkdldEJvbmVJbmRleCgiY2hlc3QiKTsNCglpZihib25lSW5kZXggIT0gLTEpDQoJew0KCQlyZWYgdmFyIGl0ZW0gPSByZWYgcmVzdWx0W2JvbmVJbmRleF07DQoNCgkJLy9jYWxjdWxhdGUgYm9uZSBvZmZzZXQNCgkJdmFyIGFuZ2xlID0gbmV3IERlZ3JlZSg2MCkgKiBNYXRoLlNpbihUaW1lLkN1cnJlbnQpOyANCgkJdmFyIG9mZnNldCA9IE1hdHJpeDNGLkZyb21Sb3RhdGVCeVkoKGZsb2F0KWFuZ2xlLkluUmFkaWFucygpKS5Ub1F1YXRlcm5pb24oKTsNCgkJDQoJCS8vdXBkYXRlIHRoZSBib25lDQoJCWl0ZW0uUm90YXRpb24gKj0gb2Zmc2V0Ow0KCX0JDQp9DQo=")]
public class DynamicClass_0b9dc950_27c7_433f_ae30_bd2de1f73be1
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
public class DynamicClass_798b0233_080b_4759_8f65_6a34facfd6b9
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

[CSharpScriptGeneratedAttribute("ZG91YmxlIE1ldGhvZCgpDQp7DQoJcmV0dXJuIE1hdGhFeC5TYXR1cmF0ZSggKCBNYXRoLlNpbiggRW5naW5lQXBwLkVuZ2luZVRpbWUgKiAxLjMgKSArIDEuMCApIC8gMiApOw0KfQ0K")]
public class DynamicClass_f554d425_1d39_4f90_8111_327d3f9af3cc
{
    public NeoAxis.Component_CSharpScript Owner;
    double Method()
    {
        return MathEx.Saturate((Math.Sin(EngineApp.EngineTime * 1.3) + 1.0) / 2);
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpDQp7DQoJdmFyIGFuZ2xlID0gRW5naW5lQXBwLkVuZ2luZVRpbWUgKiAtMS4wOw0KCXZhciBvZmZzZXQgPSBuZXcgVmVjdG9yMyhNYXRoLkNvcyhhbmdsZSksIE1hdGguU2luKGFuZ2xlKSwgMCkgKiAyLjA7DQoJdmFyIGxvb2tUbyA9IG5ldyBWZWN0b3IzKDExLjczNzQ4MzkxMjQ4MjcsIC0wLjA1MTc3Njc1MDMyNDM5LCAtMTQuODA5Mjc1NTgyNTA5Mik7DQoJdmFyIGxvb2tBdCA9IFF1YXRlcm5pb24uTG9va0F0KC1vZmZzZXQsIG5ldyBWZWN0b3IzKDAsMCwxKSk7DQoJDQoJcmV0dXJuIG5ldyBUcmFuc2Zvcm0oIGxvb2tUbyArIG9mZnNldCwgbG9va0F0LCBWZWN0b3IzLk9uZSApOw0KfQ0K")]
public class DynamicClass_fa2500c1_d4f4_46c2_970d_7468decf1c2c
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
public class DynamicClass_776f8bdb_ca2a_4cf0_9b07_44d8bf762e35
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
public class DynamicClass_75eabc64_950a_4dbf_ac0e_b7800322cb55
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