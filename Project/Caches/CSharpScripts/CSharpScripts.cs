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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkNCnsNCgl2YXIgX3RoaXMgPSBzZW5kZXIgYXMgQ29tcG9uZW50X0J1dHRvbkluU3BhY2U7DQoJaWYgKF90aGlzICE9IG51bGwpDQoJew0KCQl2YXIgaW5kaWNhdG9yID0gX3RoaXMuR2V0Q29tcG9uZW50KCJJbmRpY2F0b3IiKSBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7DQoJCWlmIChpbmRpY2F0b3IgIT0gbnVsbCkNCgkJCWluZGljYXRvci5Db2xvciA9IF90aGlzLkFjdGl2YXRlZCA/IG5ldyBDb2xvclZhbHVlKDAsIDEsIDApIDogbmV3IENvbG9yVmFsdWUoMC41LCAwLjUsIDAuNSk7DQoNCgkJdmFyIGJ1dHRvbk9mZnNldCA9IF90aGlzLkNvbXBvbmVudHMuR2V0QnlQYXRoKCJCdXR0b25cXEF0dGFjaCBUcmFuc2Zvcm0gT2Zmc2V0IikgYXMgQ29tcG9uZW50X1RyYW5zZm9ybU9mZnNldDsNCgkJaWYgKGJ1dHRvbk9mZnNldCAhPSBudWxsKQ0KCQl7DQoJCQl2YXIgb2Zmc2V0UHVzaGVkID0gMC4wMTsNCgkJCXZhciBvZmZzZXREZWZhdWx0ID0gMC4wNTsNCg0KCQkJdmFyIGNvZWYgPSAwLjA7DQoJCQlpZiAoX3RoaXMuQ2xpY2tpbmcgJiYgX3RoaXMuQ2xpY2tpbmdUb3RhbFRpbWUgIT0gMCkNCgkJCXsNCgkJCQl2YXIgdGltZUZhY3RvciA9IE1hdGhFeC5TYXR1cmF0ZShfdGhpcy5DbGlja2luZ0N1cnJlbnRUaW1lIC8gX3RoaXMuQ2xpY2tpbmdUb3RhbFRpbWUpOw0KDQoJCQkJaWYodGltZUZhY3RvciA8IDAuNSkNCgkJCQkJY29lZiA9IHRpbWVGYWN0b3IgKiAyOw0KCQkJCWVsc2UNCgkJCQkJY29lZiA9ICgxLjBmIC0gdGltZUZhY3RvcikgKiAyOw0KCQkJfQ0KDQoJCQl2YXIgb2Zmc2V0ID0gTWF0aEV4LkxlcnAob2Zmc2V0RGVmYXVsdCwgb2Zmc2V0UHVzaGVkLCBjb2VmKTsNCgkJCWJ1dHRvbk9mZnNldC5Qb3NpdGlvbk9mZnNldCA9IG5ldyBWZWN0b3IzKG9mZnNldCwgMCwgMCk7DQoJCX0NCgl9DQp9")]
public class DynamicClass_f214ff2a_d827_4ccd_81bc_91647e109f84
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQ0Kew0KCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsNCg0KCXZhciBsaWdodCA9IHNjZW5lLkdldENvbXBvbmVudCgiRGlyZWN0aW9uYWwgTGlnaHQiKSBhcyBDb21wb25lbnRfTGlnaHQ7DQoJaWYgKGxpZ2h0ICE9IG51bGwpDQoJCWxpZ2h0LkVuYWJsZWQgPSBzZW5kZXIuQWN0aXZhdGVkOw0KfQ0K")]
public class DynamicClass_d87bc21c_a114_4410_bb58_33a0a154f424
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkNCnsNCgl2YXIgX3RoaXMgPSBzZW5kZXIgYXMgQ29tcG9uZW50X1JlZ3VsYXRvclN3aXRjaEluU3BhY2U7DQoJaWYgKF90aGlzICE9IG51bGwpDQoJew0KCQl2YXIgaW5kaWNhdG9yTWluID0gX3RoaXMuR2V0Q29tcG9uZW50KCJJbmRpY2F0b3IgTWluIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOw0KCQlpZiAoaW5kaWNhdG9yTWluICE9IG51bGwpDQoJCQlpbmRpY2F0b3JNaW4uQ29sb3IgPSBfdGhpcy5WYWx1ZS5WYWx1ZSA8PSBfdGhpcy5WYWx1ZVJhbmdlLlZhbHVlLk1pbmltdW0gPyBuZXcgQ29sb3JWYWx1ZSgxLCAwLCAwKSA6IG5ldyBDb2xvclZhbHVlKDAuNSwgMC41LCAwLjUpOw0KDQoJCXZhciBpbmRpY2F0b3JNYXggPSBfdGhpcy5HZXRDb21wb25lbnQoIkluZGljYXRvciBNYXgiKSBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7DQoJCWlmIChpbmRpY2F0b3JNYXggIT0gbnVsbCkNCgkJCWluZGljYXRvck1heC5Db2xvciA9IF90aGlzLlZhbHVlLlZhbHVlID49IF90aGlzLlZhbHVlUmFuZ2UuVmFsdWUuTWF4aW11bSA/IG5ldyBDb2xvclZhbHVlKDAsIDEsIDApIDogbmV3IENvbG9yVmFsdWUoMC41LCAwLjUsIDAuNSk7DQoNCgkJdmFyIGJ1dHRvbiA9IF90aGlzLkdldENvbXBvbmVudCgiQnV0dG9uIik7DQoJCWlmIChidXR0b24gIT0gbnVsbCkNCgkJew0KCQkJdmFyIG9mZnNldCA9IGJ1dHRvbi5HZXRDb21wb25lbnQ8Q29tcG9uZW50X1RyYW5zZm9ybU9mZnNldD4oKTsNCgkJCWlmIChvZmZzZXQgIT0gbnVsbCkNCgkJCXsNCgkJCQl2YXIgYW5nbGUgPSBfdGhpcy5HZXRWYWx1ZUFuZ2xlKCkgLSA5MDsNCgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsNCgkJCX0NCgkJfQ0KDQoJCXZhciBtYXJrZXJNaW4gPSBfdGhpcy5HZXRDb21wb25lbnQoIk1hcmtlciBNaW4iKTsNCgkJaWYgKG1hcmtlck1pbiAhPSBudWxsKQ0KCQl7DQoJCQl2YXIgb2Zmc2V0ID0gbWFya2VyTWluLkdldENvbXBvbmVudDxDb21wb25lbnRfVHJhbnNmb3JtT2Zmc2V0PigpOw0KCQkJaWYgKG9mZnNldCAhPSBudWxsKQ0KCQkJew0KCQkJCXZhciBhbmdsZSA9IF90aGlzLkFuZ2xlUmFuZ2UuVmFsdWUuTWluaW11bSAtIDkwOw0KCQkJCXZhciBhbmdsZVIgPSBNYXRoRXguRGVncmVlVG9SYWRpYW4oYW5nbGUpOw0KCQkJCW9mZnNldC5Qb3NpdGlvbk9mZnNldCA9IG5ldyBWZWN0b3IzKDAuMDEsIE1hdGguQ29zKGFuZ2xlUikgKiAwLjA0LCBNYXRoLlNpbigtYW5nbGVSKSAqIDAuMDQpOw0KCQkJCW9mZnNldC5Sb3RhdGlvbk9mZnNldCA9IG5ldyBBbmdsZXMoYW5nbGUsIDAsIDApLlRvUXVhdGVybmlvbigpOw0KCQkJfQ0KCQl9DQoNCgkJdmFyIG1hcmtlck1heCA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIE1heCIpOw0KCQlpZiAobWFya2VyTWF4ICE9IG51bGwpDQoJCXsNCgkJCXZhciBvZmZzZXQgPSBtYXJrZXJNYXguR2V0Q29tcG9uZW50PENvbXBvbmVudF9UcmFuc2Zvcm1PZmZzZXQ+KCk7DQoJCQlpZiAob2Zmc2V0ICE9IG51bGwpDQoJCQl7DQoJCQkJdmFyIGFuZ2xlID0gX3RoaXMuQW5nbGVSYW5nZS5WYWx1ZS5NYXhpbXVtIC0gOTA7DQoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7DQoJCQkJb2Zmc2V0LlBvc2l0aW9uT2Zmc2V0ID0gbmV3IFZlY3RvcjMoMC4wMSwgTWF0aC5Db3MoYW5nbGVSKSAqIDAuMDQsIE1hdGguU2luKC1hbmdsZVIpICogMC4wNCk7DQoJCQkJb2Zmc2V0LlJvdGF0aW9uT2Zmc2V0ID0gbmV3IEFuZ2xlcyhhbmdsZSwgMCwgMCkuVG9RdWF0ZXJuaW9uKCk7DQoJCQl9DQoJCX0NCg0KCQl2YXIgbWFya2VyQ3VycmVudCA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIEN1cnJlbnQiKTsNCgkJaWYgKG1hcmtlckN1cnJlbnQgIT0gbnVsbCkNCgkJew0KCQkJdmFyIG9mZnNldCA9IG1hcmtlckN1cnJlbnQuR2V0Q29tcG9uZW50PENvbXBvbmVudF9UcmFuc2Zvcm1PZmZzZXQ+KCk7DQoJCQlpZiAob2Zmc2V0ICE9IG51bGwpDQoJCQl7DQoJCQkJdmFyIGFuZ2xlID0gX3RoaXMuR2V0VmFsdWVBbmdsZSgpIC0gOTA7DQoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7DQoJCQkJb2Zmc2V0LlBvc2l0aW9uT2Zmc2V0ID0gbmV3IFZlY3RvcjMoMC4wNiwgTWF0aC5Db3MoYW5nbGVSKSAqIDAuMDQsIE1hdGguU2luKC1hbmdsZVIpICogMC4wNCk7DQoJCQkJb2Zmc2V0LlJvdGF0aW9uT2Zmc2V0ID0gbmV3IEFuZ2xlcyhhbmdsZSwgMCwgMCkuVG9RdWF0ZXJuaW9uKCk7DQoJCQl9DQoJCX0NCgl9DQp9")]
public class DynamicClass_033370d1_1ecc_4dbf_be45_2fbbd5d879d7
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikNCnsNCgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7DQoNCgl2YXIgbGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkRpcmVjdGlvbmFsIExpZ2h0IikgYXMgQ29tcG9uZW50X0xpZ2h0Ow0KCWlmIChsaWdodCAhPSBudWxsKQ0KCQlsaWdodC5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDEuMCwgMS4wLCAxLjAgLSBvYmouVmFsdWUpOw0KfQ0K")]
public class DynamicClass_664fe87f_a00c_493d_a3db_bcdfe8074707
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

[CSharpScriptGeneratedAttribute("aW50IE1ldGhvZCggaW50IGEsIGludCBiICkNCnsNCglyZXR1cm4gYSArIGI7DQp9DQo=")]
public class DynamicClass_ebb8f31f_1f6c_45ef_9eab_e47034ccead6
{
    public NeoAxis.Component_CSharpScript Owner;
    int Method(int a, int b)
    {
        return a + b;
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpDQp7DQoJdmFyIGFuZ2xlID0gRW5naW5lQXBwLkVuZ2luZVRpbWUgKiAwLjM7DQoJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIDIuNTsNCgl2YXIgbG9va1RvID0gbmV3IFZlY3RvcjMoMTEuNzM3NDgzOTEyNDgyNywgLTAuMDUxNzc2NzUwMzI0MzksIC0xNS41MDkyNzU1ODI1MDkyKTsNCgl2YXIgbG9va0F0ID0gUXVhdGVybmlvbi5Mb29rQXQoLW9mZnNldCwgbmV3IFZlY3RvcjMoMCwwLDEpKTsNCgkNCglyZXR1cm4gbmV3IFRyYW5zZm9ybSggbG9va1RvICsgb2Zmc2V0LCBsb29rQXQsIFZlY3RvcjMuT25lICk7DQp9DQo=")]
public class DynamicClass_13a71812_aa9c_49b7_9635_7e9f10fe25e9
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

[CSharpScriptGeneratedAttribute("Q29tcG9uZW50X1JlbmRlcmluZ1BpcGVsaW5lIEdldFBpcGVsaW5lKCkNCnsNCglzdHJpbmcgbmFtZTsNCglpZihFbmdpbmVBcHAuRW5naW5lVGltZSAlIDQgPiAyKQ0KCQluYW1lID0gIlJlbmRlcmluZyBQaXBlbGluZSI7DQoJZWxzZQ0KCQluYW1lID0gIlJlbmRlcmluZyBQaXBlbGluZSAyIjsNCgkJDQoJcmV0dXJuIE93bmVyLlBhcmVudC5HZXRDb21wb25lbnQobmFtZSkgYXMgQ29tcG9uZW50X1JlbmRlcmluZ1BpcGVsaW5lOw0KfQ0K")]
public class DynamicClass_41e5361b_8754_4ae1_8437_50aa347556d5
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

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpDQp7DQoJdmFyIGFuZ2xlID0gRW5naW5lQXBwLkVuZ2luZVRpbWUgKiAtMS4wOw0KCXZhciBvZmZzZXQgPSBuZXcgVmVjdG9yMyhNYXRoLkNvcyhhbmdsZSksIE1hdGguU2luKGFuZ2xlKSwgMCkgKiAyLjA7DQoJdmFyIGxvb2tUbyA9IG5ldyBWZWN0b3IzKDExLjczNzQ4MzkxMjQ4MjcsIC0wLjA1MTc3Njc1MDMyNDM5LCAtMTQuODA5Mjc1NTgyNTA5Mik7DQoJdmFyIGxvb2tBdCA9IFF1YXRlcm5pb24uTG9va0F0KC1vZmZzZXQsIG5ldyBWZWN0b3IzKDAsMCwxKSk7DQoJDQoJcmV0dXJuIG5ldyBUcmFuc2Zvcm0oIGxvb2tUbyArIG9mZnNldCwgbG9va0F0LCBWZWN0b3IzLk9uZSApOw0KfQ0K")]
public class DynamicClass_9909df0b_ff49_4720_b3b6_7aaba24864bd
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
public class DynamicClass_f123d5c3_d6c4_4c40_8002_3262b7749544
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

[CSharpScriptGeneratedAttribute("ZG91YmxlIE1ldGhvZCgpDQp7DQoJcmV0dXJuIE1hdGhFeC5TYXR1cmF0ZSggKCBNYXRoLlNpbiggRW5naW5lQXBwLkVuZ2luZVRpbWUgKiAxLjMgKSArIDEuMCApIC8gMiApOw0KfQ0K")]
public class DynamicClass_9fc80893_c57a_4d1a_a3ad_dfd233cf1d38
{
    public NeoAxis.Component_CSharpScript Owner;
    double Method()
    {
        return MathEx.Saturate((Math.Sin(EngineApp.EngineTime * 1.3) + 1.0) / 2);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQ0Kew0KCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsNCg0KCXZhciBncm91bmQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkdyb3VuZCIpIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsNCglpZiAoZ3JvdW5kICE9IG51bGwpDQoJew0KCQlpZiAoIWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwuUmVmZXJlbmNlU3BlY2lmaWVkKQ0KCQl7DQoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKA0KCQkJCUAiQ29udGVudFxNYXRlcmlhbHNcQmFzaWMgTGlicmFyeVxDb25jcmV0ZVxDb25jcmV0ZSBGbG9vciAwMS5tYXRlcmlhbCIpOw0KCQl9DQoJCWVsc2UNCgkJCWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwgPSBudWxsOw0KCX0NCn0NCg==")]
public class DynamicClass_2d8371f9_5c74_4010_a204_7c1db27aab67
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikNCnsNCgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7DQoNCgl2YXIgZ3JvdW5kID0gc2NlbmUuR2V0Q29tcG9uZW50KCJHcm91bmQiKSBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7DQoJaWYgKGdyb3VuZCAhPSBudWxsKQ0KCQlncm91bmQuQ29sb3IgPSBDb2xvclZhbHVlLkxlcnAobmV3IENvbG9yVmFsdWUoMSwgMSwgMSksIG5ldyBDb2xvclZhbHVlKDAuNCwgMC45LCAwLjQpLCAoZmxvYXQpb2JqLlZhbHVlKTsNCn0NCg==")]
public class DynamicClass_4ba32fab_3011_40b2_b925_9cd03af0f909
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW5wdXRQcm9jZXNzaW5nX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50X0lucHV0UHJvY2Vzc2luZyBzZW5kZXIsIFVJQ29udHJvbCBwbGF5U2NyZWVuLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQ0Kew0KCS8vZ2V0IGFjY2VzcyB0byB0aGUgc2hpcA0KCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsNCglpZiAoc2hpcCA9PSBudWxsKQ0KCQlyZXR1cm47DQoNCgkvL3ZhciBrZXlEb3duID0gbWVzc2FnZSBhcyBJbnB1dE1lc3NhZ2VLZXlEb3duOw0KCS8vaWYoa2V5RG93biAhPSBudWxsKQ0KCS8vew0KCS8vCWlmKGtleURvd24uS2V5ID09IEVLZXlzLlNwYWNlKQ0KCS8vCXsNCgkvLwkJLy92YXIgYm9keSA9IHNoaXAuR2V0Q29tcG9uZW50PENvbXBvbmVudF9SaWdpZEJvZHkyRD4oKTsNCgkvLwkJLy9pZiAoYm9keSAhPSBudWxsKQ0KCS8vCQkvL3sNCgkvLwkJLy8JYm9keS5BcHBseUZvcmNlKG5ldyBWZWN0b3IyKDEsIDApKTsNCgkvLwkJLy99DQoJLy8JfQ0KCS8vfQ0KfQ0KDQpwdWJsaWMgdm9pZCBJbnB1dFByb2Nlc3NpbmdfU2ltdWxhdGlvblN0ZXAoTmVvQXhpcy5Db21wb25lbnQgb2JqKQ0Kew0KCXZhciBzZW5kZXIgPSAoTmVvQXhpcy5Db21wb25lbnRfSW5wdXRQcm9jZXNzaW5nKW9iajsNCg0KCS8vZ2V0IGFjY2VzcyB0byB0aGUgc2hpcA0KCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsNCglpZiAoc2hpcCA9PSBudWxsKQ0KCQlyZXR1cm47DQoNCgkvL2NvbnRyb2wgdGhlIHNoaXANCgl2YXIgYm9keSA9IHNoaXAuR2V0Q29tcG9uZW50PENvbXBvbmVudF9SaWdpZEJvZHkyRD4oKTsNCglpZiAoYm9keSAhPSBudWxsKQ0KCXsNCgkJLy9rZXlib2FyZA0KDQoJCS8vZmx5IGZyb250DQoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlcpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuVXApIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkOCkpDQoJCXsNCgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOw0KCQkJYm9keS5BcHBseUZvcmNlKGRpciAqIDEuMCk7DQoJCX0NCg0KCQkvL2ZseSBiYWNrDQoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlMpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuRG93bikgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5OdW1QYWQyKSkNCgkJew0KCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7DQoJCQlib2R5LkFwcGx5Rm9yY2UoZGlyICogLTEuMCk7DQoJCX0NCg0KCQkvL3R1cm4gbGVmdA0KCQlpZiAoc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5BKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkxlZnQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkNCkpDQoJCQlib2R5LkFwcGx5VG9ycXVlKDEuMCk7DQoNCgkJLy90dXJuIHJpZ2h0DQoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuUmlnaHQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkNikpDQoJCQlib2R5LkFwcGx5VG9ycXVlKC0xLjApOw0KDQoJCS8vbW92ZW1lbnQgYnkgam95c3RpY2sgYXhlcw0KCQlpZiAoTWF0aC5BYnMoc2VuZGVyLkpveXN0aWNrQXhlc1swXSkgPj0gMC4wMSkNCgkJCWJvZHkuQXBwbHlUb3JxdWUoLXNlbmRlci5Kb3lzdGlja0F4ZXNbMF0pOw0KCQlpZiAoTWF0aC5BYnMoc2VuZGVyLkpveXN0aWNrQXhlc1sxXSkgPj0gMC4wMSkNCgkJew0KCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7DQoJCQlib2R5LkFwcGx5Rm9yY2UoZGlyICogc2VuZGVyLkpveXN0aWNrQXhlc1sxXSk7DQoJCX0NCgkJLy9Kb3lzdGlja0F4ZXMNCgkJLy9Kb3lzdGlja0J1dHRvbnMNCgkJLy9Kb3lzdGlja1BPVnMNCgkJLy9Kb3lzdGlja1NsaWRlcnMNCgkJLy9Jc0pveXN0aWNrQnV0dG9uUHJlc3NlZA0KCQkvL0dldEpveXN0aWNrQXhpcw0KCQkvL0dldEpveXN0aWNrUE9WDQoJCS8vR2V0Sm95c3RpY2tTbGlkZXIJCQkNCg0KCX0NCg0KfQ0K")]
public class DynamicClass_ed3c5d72_87c0_43f8_a5d6_cf77fef210cd
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW5wdXRQcm9jZXNzaW5nX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50X0lucHV0UHJvY2Vzc2luZyBzZW5kZXIsIFVJQ29udHJvbCBwbGF5U2NyZWVuLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQ0Kew0KCS8vZ2V0IGFjY2VzcyB0byB0aGUgc2hpcA0KCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsNCglpZiAoc2hpcCA9PSBudWxsKQ0KCQlyZXR1cm47DQoNCgkvKg0KCQl2YXIga2V5RG93biA9IG1lc3NhZ2UgYXMgSW5wdXRNZXNzYWdlS2V5RG93bjsNCgkJaWYoa2V5RG93biAhPSBudWxsKQ0KCQl7DQoJCQlpZihrZXlEb3duLktleSA9PSBFS2V5cy5TcGFjZSkNCgkJCXsNCgkJCQkvL3ZhciBib2R5ID0gc2hpcC5HZXRDb21wb25lbnQ8Q29tcG9uZW50X1JpZ2lkQm9keTJEPigpOw0KCQkJCS8vaWYgKGJvZHkgIT0gbnVsbCkNCgkJCQkvL3sNCgkJCQkvLwlib2R5LkFwcGx5Rm9yY2UobmV3IFZlY3RvcjIoMSwgMCkpOw0KCQkJCS8vfQ0KCQkJfQ0KCQl9DQoJKi8NCg0KfQ0KDQpwdWJsaWMgdm9pZCBJbnB1dFByb2Nlc3NpbmdfU2ltdWxhdGlvblN0ZXAoTmVvQXhpcy5Db21wb25lbnQgb2JqKQ0Kew0KCXZhciBzZW5kZXIgPSAoTmVvQXhpcy5Db21wb25lbnRfSW5wdXRQcm9jZXNzaW5nKW9iajsNCg0KCS8vZ2V0IGFjY2VzcyB0byB0aGUgc2hpcA0KCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsNCglpZiAoc2hpcCA9PSBudWxsKQ0KCQlyZXR1cm47DQoNCgkvL2NvbnRyb2wgdGhlIHNoaXANCgl2YXIgYm9keSA9IHNoaXAuR2V0Q29tcG9uZW50PENvbXBvbmVudF9SaWdpZEJvZHkyRD4oKTsNCglpZiAoYm9keSAhPSBudWxsKQ0KCXsNCgkJLy9mbHkgZnJvbnQNCgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuVykgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5VcCkpDQoJCXsNCgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOw0KCQkJYm9keS5BcHBseUZvcmNlKGRpciAqIDEuMCk7DQoJCX0NCg0KCQkvL2ZseSBiYWNrDQoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlMpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuRG93bikpDQoJCXsNCgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOw0KCQkJYm9keS5BcHBseUZvcmNlKGRpciAqIC0xLjApOw0KCQl9DQoNCgkJLy90dXJuIGxlZnQNCgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuQSkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5MZWZ0KSkNCgkJCWJvZHkuQXBwbHlUb3JxdWUoMS4wKTsNCg0KCQkvL3R1cm4gcmlnaHQNCgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuRCkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5SaWdodCkpDQoJCQlib2R5LkFwcGx5VG9ycXVlKC0xLjApOw0KCX0NCg0KfQ0K")]
public class DynamicClass_f6faa223_172b_428b_82e0_eb66c54464ec
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgTWVzaEluU3BhY2VBbmltYXRpb25Db250cm9sbGVyX0NhbGN1bGF0ZUJvbmVUcmFuc2Zvcm1zKE5lb0F4aXMuQ29tcG9uZW50X01lc2hJblNwYWNlQW5pbWF0aW9uQ29udHJvbGxlciBzZW5kZXIsIE5lb0F4aXMuQ29tcG9uZW50X1NrZWxldG9uQW5pbWF0aW9uVHJhY2suQ2FsY3VsYXRlQm9uZVRyYW5zZm9ybXNJdGVtW10gcmVzdWx0KQ0Kew0KCS8vdG8gZW5hYmxlIHRoaXMgZXZlbnQgaGFuZGxlciBpbiB0aGUgZWRpdG9yIGNoYW5nZSAiV2hlbiBFbmFibGUiIHByb3BlcnR5IHRvICJTaW11bGF0aW9uIHwgSW5zdGFuY2UgfCBFZGl0b3IiLg0KCS8vY29tcG9uZW50OiBDaGFyYWN0ZXIvTWVzaCBJbiBTcGFjZS9DIyBTY3JpcHQvRXZlbnQgSGFuZGxlciBDYWxjdWxhdGVCb25lVHJhbnNmb3Jtcy4NCgkNCgl2YXIgYm9uZUluZGV4ID0gc2VuZGVyLkdldEJvbmVJbmRleCgibWl4YW1vcmlnOlNwaW5lMSIpOw0KCWlmKGJvbmVJbmRleCAhPSAtMSkNCgl7DQoJCXJlZiB2YXIgaXRlbSA9IHJlZiByZXN1bHRbYm9uZUluZGV4XTsNCg0KCQkvL2NhbGN1bGF0ZSBib25lIG9mZnNldA0KCQl2YXIgYW5nbGUgPSBuZXcgRGVncmVlKDYwKSAqIE1hdGguU2luKFRpbWUuQ3VycmVudCk7IA0KCQl2YXIgb2Zmc2V0ID0gTWF0cml4M0YuRnJvbVJvdGF0ZUJ5WSgoZmxvYXQpYW5nbGUuSW5SYWRpYW5zKCkpLlRvUXVhdGVybmlvbigpOw0KCQkNCgkJLy91cGRhdGUgdGhlIGJvbmUNCgkJaXRlbS5Sb3RhdGlvbiAqPSBvZmZzZXQ7DQoJfQkNCn0NCg==")]
public class DynamicClass_df92a392_2e4c_406a_acee_1a053b6696b1
{
    public NeoAxis.Component_CSharpScript Owner;
    public void MeshInSpaceAnimationController_CalculateBoneTransforms(NeoAxis.Component_MeshInSpaceAnimationController sender, NeoAxis.Component_SkeletonAnimationTrack.CalculateBoneTransformsItem[] result)
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

[CSharpScriptGeneratedAttribute("DQovLyBUaGUgZXhhbXBsZSBvZiBjb21wdXRpbmcgYSBsaXN0IG9mIG9iamVjdHMgZm9yIHRoZSBHcm91cE9mT2JqZWN0cyB1c2luZyB0aHJlYWRzLg0KDQpjbGFzcyBEYXRhDQp7DQoJcHVibGljIENvbXBvbmVudF9Hcm91cE9mT2JqZWN0cy5PYmplY3RbXSBPYmplY3RzOw0KfQ0KDQpwdWJsaWMgdm9pZCBDb21wdXRlVXNpbmdUaHJlYWRzX0NvbXB1dGVCZWdpbihOZW9BeGlzLkNvbXBvbmVudF9Db21wdXRlVXNpbmdUaHJlYWRzIHNlbmRlcikNCnsNCgkvL0xvZy5JbmZvKCJiZWdpbiIpOw0KDQoJdmFyIGRhdGEgPSBuZXcgRGF0YSgpOw0KCWRhdGEuT2JqZWN0cyA9IG5ldyBDb21wb25lbnRfR3JvdXBPZk9iamVjdHMuT2JqZWN0WzEwMCAqIDEwMF07DQoJc2VuZGVyLkNvbnRleHQuQW55RGF0YSA9IGRhdGE7DQoNCgkvL2hvdyB0byBza2lwIG9uZSBjb21wdXRpbmc6DQoJLy9zZW5kZXIuQ29udGV4dC5BbGxvd0NvbXB1dGUgPSBmYWxzZTsNCn0NCg0KcHVibGljIHZvaWQgQ29tcHV0ZVVzaW5nVGhyZWFkc19Db21wdXRlVGhyZWFkKE5lb0F4aXMuQ29tcG9uZW50X0NvbXB1dGVVc2luZ1RocmVhZHMgc2VuZGVyLCBpbnQgdGhyZWFkSW5kZXgpDQp7DQoJdmFyIGRhdGEgPSBzZW5kZXIuQ29udGV4dC5BbnlEYXRhIGFzIERhdGE7DQoJaWYgKGRhdGEgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQoJdmFyIG9iamVjdHMgPSBkYXRhLk9iamVjdHM7DQoNCgkvL2dldCByYW5nZSBvZiBvYmplY3RzIGZvciB0aGUgdGhyZWFkDQoJdmFyIGZyb20gPSAoZGF0YS5PYmplY3RzLkxlbmd0aCAqIHRocmVhZEluZGV4KSAvIHNlbmRlci5UaHJlYWRDb3VudDsNCgl2YXIgdG8gPSAoZGF0YS5PYmplY3RzLkxlbmd0aCAqICh0aHJlYWRJbmRleCArIDEpKSAvIHNlbmRlci5UaHJlYWRDb3VudDsNCg0KCXZhciByYW5kb20gPSBuZXcgTmVvQXhpcy5SYW5kb20oMCk7Ly8oaW50KShUaW1lLkN1cnJlbnQgKiAwLjI1KSk7DQoNCgkvL2NvbXB1dGUgb2JqZWN0cw0KCWZvciAoaW50IG4gPSBmcm9tOyBuIDwgdG87IG4rKykNCgl7DQoJCXJlZiB2YXIgb2JqID0gcmVmIG9iamVjdHNbbl07DQoNCgkJdmFyIHggPSBuICUgMTAwOw0KCQl2YXIgeSA9IG4gLyAxMDA7DQoNCgkJdmFyIHdhdmluZ1N0YXJ0ID0gcmFuZG9tLk5leHQoTWF0aC5QSSAqIDIpOw0KCQl2YXIgd2F2aW5nQ3ljbGUgPSByYW5kb20uTmV4dCgxLjAsIDIuMCk7DQoJCQ0KCQkvL3VzZSBBbnlEYXRhIHRvIHN0b3JlIGFkZGl0aW9uYWwgZGF0YSBpbiB0aGUgb2JqZWN0DQoJCS8vb2JqLkFueURhdGEgPSA7DQoNCgkJb2JqLkVsZW1lbnQgPSAodXNob3J0KXJhbmRvbS5OZXh0KDIpOw0KCQlvYmouRmxhZ3MgPSBDb21wb25lbnRfR3JvdXBPZk9iamVjdHMuT2JqZWN0LkZsYWdzRW51bS5FbmFibGVkIHwgQ29tcG9uZW50X0dyb3VwT2ZPYmplY3RzLk9iamVjdC5GbGFnc0VudW0uVmlzaWJsZTsNCgkJb2JqLlBvc2l0aW9uID0gbmV3IFZlY3RvcjMoeCAqIDEuMywgeSAqIDEuMywgMS4wICsgTWF0aC5TaW4oIHdhdmluZ1N0YXJ0ICsgVGltZS5DdXJyZW50ICogd2F2aW5nQ3ljbGUgKSAqIDAuMjUgKTsNCgkJb2JqLlJvdGF0aW9uID0gUXVhdGVybmlvbkYuSWRlbnRpdHk7DQoJCW9iai5TY2FsZSA9IG5ldyBWZWN0b3IzRigxLCAxLCAxKTsNCgkJb2JqLkNvbG9yID0gbmV3IENvbG9yVmFsdWUocmFuZG9tLk5leHRGbG9hdCgpLCByYW5kb20uTmV4dEZsb2F0KCksIHJhbmRvbS5OZXh0RmxvYXQoKSk7DQoNCgkJLy92YXIgcG9zID0gbmV3IFZlY3RvcjMobiAqIDEuMywgMCwgMSk7DQoJCS8vdmFyIHJvdCA9IFF1YXRlcm5pb25GLklkZW50aXR5Ow0KCQkvL3ZhciBzY2wgPSBuZXcgVmVjdG9yM0YoMSwgMSwgMSk7DQoJCS8vZGF0YS5PYmplY3RzW25dID0gbmV3IENvbXBvbmVudF9Hcm91cE9mT2JqZWN0cy5PYmplY3QoMCwgMCwgMCwgQ29tcG9uZW50X0dyb3VwT2ZPYmplY3RzLk9iamVjdC5GbGFnc0VudW0uRW5hYmxlZCB8IENvbXBvbmVudF9Hcm91cE9mT2JqZWN0cy5PYmplY3QuRmxhZ3NFbnVtLlZpc2libGUsIHBvcywgcm90LCBzY2wsIFZlY3RvcjRGLlplcm8sIENvbG9yVmFsdWUuT25lLCBWZWN0b3I0Ri5aZXJvLCBWZWN0b3I0Ri5aZXJvKTsgDQoJfQ0KfQ0KDQpwdWJsaWMgdm9pZCBDb21wdXRlVXNpbmdUaHJlYWRzX0NvbXB1dGVFbmQoTmVvQXhpcy5Db21wb25lbnRfQ29tcHV0ZVVzaW5nVGhyZWFkcyBzZW5kZXIpDQp7DQoJLy9Mb2cuSW5mbygiZW5kIik7DQoNCgl2YXIgZGF0YSA9IHNlbmRlci5Db250ZXh0LkFueURhdGEgYXMgRGF0YTsNCglpZiAoZGF0YSA9PSBudWxsKQ0KCQlyZXR1cm47DQoNCgl2YXIgZ3JvdXBPZk9iamVjdHMgPSBzZW5kZXIuUGFyZW50IGFzIENvbXBvbmVudF9Hcm91cE9mT2JqZWN0czsNCglpZiAoZ3JvdXBPZk9iamVjdHMgIT0gbnVsbCkNCgkJZ3JvdXBPZk9iamVjdHMuT2JqZWN0c1NldChkYXRhLk9iamVjdHMsIHRydWUpOw0KfQ0K")]
public class DynamicClass_8878e651_d400_4bbd_9e21_c281f935f436
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQ0Kew0KCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsNCg0KCXZhciBncm91bmQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkdyb3VuZCIpIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsNCglpZiAoZ3JvdW5kICE9IG51bGwpDQoJew0KCQlpZiAoIWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwuUmVmZXJlbmNlU3BlY2lmaWVkKQ0KCQl7DQoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKCBAIkJhc2VcTWF0ZXJpYWxzXERhcmsgWWVsbG93Lm1hdGVyaWFsIik7DQoJCX0NCgkJZWxzZQ0KCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IG51bGw7DQoJfQ0KfQ0K")]
public class DynamicClass_aedc83e3_8333_4d5a_bae3_c9ae398bb0d3
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikNCnsNCgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7DQoNCgl2YXIgbWVzaEluU3BhY2UgPSBzY2VuZS5HZXRDb21wb25lbnQoIkdyb3VuZCIpIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsNCglpZiAobWVzaEluU3BhY2UgIT0gbnVsbCkNCgkJbWVzaEluU3BhY2UuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLjAgLSBvYmouVmFsdWUsIDEuMCwgMS4wIC0gb2JqLlZhbHVlKTsNCn0NCg==")]
public class DynamicClass_9bd72c65_14f8_47e4_b1b0_a0b719abc691
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpDQp7DQoJdmFyIG9iamVjdDEgPSBzZW5kZXIuQ29tcG9uZW50c1siU3BoZXJlIl0gYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOw0KCWlmKG9iamVjdDEgIT0gbnVsbCkNCgkJb2JqZWN0MS5Db2xvciA9IG5ldyBDb2xvclZhbHVlKDAuNSwgMC43NSArIE1hdGguU2luKFRpbWUuQ3VycmVudCkgKiAwLjI1LCAwLjUpOw0KDQoJdmFyIG1hdGVyaWFsMiA9IHNlbmRlci5Db21wb25lbnRzWyJCb3hcXE1hdGVyaWFsIl0gYXMgQ29tcG9uZW50X01hdGVyaWFsOw0KCWlmKG1hdGVyaWFsMiAhPSBudWxsKQ0KCQltYXRlcmlhbDIuRW1pc3NpdmUgPSBuZXcgQ29sb3JWYWx1ZVBvd2VyZWQoMCwgKDEuMCArIE1hdGguU2luKFRpbWUuQ3VycmVudCkpICogNSwgMCk7DQoJCQ0KCXZhciBtYXRlcmlhbDMgPSBzZW5kZXIuQ29tcG9uZW50c1siQ3lsaW5kZXJcXE1hdGVyaWFsIl0gYXMgQ29tcG9uZW50X01hdGVyaWFsOw0KCWlmKG1hdGVyaWFsMyAhPSBudWxsKQ0KCQltYXRlcmlhbDMuUHJvcGVydHlTZXQoIk11bHRpcGxpZXIiLCBuZXcgQ29sb3JWYWx1ZSgxLCAxLCAxLjAgKyAoMS4wICsgTWF0aC5TaW4oVGltZS5DdXJyZW50KSkgKiA1KSk7DQp9DQo=")]
public class DynamicClass_ebcd9ca1_2def_4af5_9754_cd63a00e1ecc
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkNCnsNCgl2YXIgX3RoaXMgPSBzZW5kZXIgYXMgQ29tcG9uZW50X1JlZ3VsYXRvclN3aXRjaEluU3BhY2U7DQoJaWYgKF90aGlzICE9IG51bGwpDQoJew0KCQl2YXIgbWFya2VyQ3VycmVudCA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIEN1cnJlbnQiKTsNCgkJaWYgKG1hcmtlckN1cnJlbnQgIT0gbnVsbCkNCgkJew0KCQkJdmFyIG9mZnNldCA9IG1hcmtlckN1cnJlbnQuR2V0Q29tcG9uZW50PENvbXBvbmVudF9UcmFuc2Zvcm1PZmZzZXQ+KCk7DQoJCQlpZiAob2Zmc2V0ICE9IG51bGwpDQoJCQl7DQoJCQkJdmFyIGFuZ2xlID0gX3RoaXMuR2V0VmFsdWVBbmdsZSgpIC0gOTA7DQoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7DQoJCQkJb2Zmc2V0LlJvdGF0aW9uT2Zmc2V0ID0gbmV3IEFuZ2xlcyhhbmdsZSwgMCwgMCkuVG9RdWF0ZXJuaW9uKCk7DQoJCQl9DQoJCX0NCgl9DQp9")]
public class DynamicClass_e52e2ec1_2398_4d60_9dd9_0f56ec3848f5
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQ0Kew0KCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsNCg0KCXZhciBncm91bmQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkdyb3VuZCIpIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsNCglpZiAoZ3JvdW5kICE9IG51bGwpDQoJew0KCQlpZiAoIWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwuUmVmZXJlbmNlU3BlY2lmaWVkKQ0KCQl7DQoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKA0KCQkJCUAiU2FtcGxlc1xTdGFydGVyIENvbnRlbnRcTWF0ZXJpYWxzXENvbmNyZXRlIDN4MyBtZXRlcnNcQ29uY3JldGUgM3gzIG1ldGVycy5tYXRlcmlhbCIpOw0KCQl9DQoJCWVsc2UNCgkJCWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwgPSBudWxsOw0KCX0NCn0NCg==")]
public class DynamicClass_41925032_7f69_4ae1_87b7_f53e0424a5f9
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIF90aGlzID0gc2VuZGVyIGFzIENvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlOwoJaWYgKF90aGlzICE9IG51bGwpCgl7CgkJdmFyIG1hcmtlckN1cnJlbnQgPSBfdGhpcy5HZXRDb21wb25lbnQoIk1hcmtlciBDdXJyZW50Iik7CgkJaWYgKG1hcmtlckN1cnJlbnQgIT0gbnVsbCkKCQl7CgkJCXZhciBvZmZzZXQgPSBtYXJrZXJDdXJyZW50LkdldENvbXBvbmVudDxDb21wb25lbnRfVHJhbnNmb3JtT2Zmc2V0PigpOwoJCQlpZiAob2Zmc2V0ICE9IG51bGwpCgkJCXsKCQkJCXZhciBhbmdsZSA9IF90aGlzLkdldFZhbHVlQW5nbGUoKSAtIDkwOwoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7CgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsKCQkJfQoJCX0KCX0KfQ==")]
public class DynamicClass_ceb97286_24d0_4efc_92ef_26110e4fbea3
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIF90aGlzID0gc2VuZGVyIGFzIENvbXBvbmVudF9CdXR0b25JblNwYWNlOwoJaWYgKF90aGlzICE9IG51bGwpCgl7CgkJdmFyIGluZGljYXRvciA9IF90aGlzLkdldENvbXBvbmVudCgiSW5kaWNhdG9yIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOwoJCWlmIChpbmRpY2F0b3IgIT0gbnVsbCkKCQkJaW5kaWNhdG9yLkNvbG9yID0gX3RoaXMuQWN0aXZhdGVkID8gbmV3IENvbG9yVmFsdWUoMCwgMSwgMCkgOiBuZXcgQ29sb3JWYWx1ZSgwLjUsIDAuNSwgMC41KTsKCgkJdmFyIGJ1dHRvbk9mZnNldCA9IF90aGlzLkNvbXBvbmVudHMuR2V0QnlQYXRoKCJCdXR0b25cXEF0dGFjaCBUcmFuc2Zvcm0gT2Zmc2V0IikgYXMgQ29tcG9uZW50X1RyYW5zZm9ybU9mZnNldDsKCQlpZiAoYnV0dG9uT2Zmc2V0ICE9IG51bGwpCgkJewoJCQl2YXIgb2Zmc2V0UHVzaGVkID0gMC4wMTsKCQkJdmFyIG9mZnNldERlZmF1bHQgPSAwLjA1OwoKCQkJdmFyIGNvZWYgPSAwLjA7CgkJCWlmIChfdGhpcy5DbGlja2luZyAmJiBfdGhpcy5DbGlja2luZ1RvdGFsVGltZSAhPSAwKQoJCQl7CgkJCQl2YXIgdGltZUZhY3RvciA9IE1hdGhFeC5TYXR1cmF0ZShfdGhpcy5DbGlja2luZ0N1cnJlbnRUaW1lIC8gX3RoaXMuQ2xpY2tpbmdUb3RhbFRpbWUpOwoKCQkJCWlmKHRpbWVGYWN0b3IgPCAwLjUpCgkJCQkJY29lZiA9IHRpbWVGYWN0b3IgKiAyOwoJCQkJZWxzZQoJCQkJCWNvZWYgPSAoMS4wZiAtIHRpbWVGYWN0b3IpICogMjsKCQkJfQoKCQkJdmFyIG9mZnNldCA9IE1hdGhFeC5MZXJwKG9mZnNldERlZmF1bHQsIG9mZnNldFB1c2hlZCwgY29lZik7CgkJCWJ1dHRvbk9mZnNldC5Qb3NpdGlvbk9mZnNldCA9IG5ldyBWZWN0b3IzKG9mZnNldCwgMCwgMCk7CgkJfQoJfQp9")]
public class DynamicClass_e2b675d8_171e_4746_9561_baccb271e606
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJdmFyIGdyb3VuZCA9IHNjZW5lLkdldENvbXBvbmVudCgiR3JvdW5kIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOwoJaWYgKGdyb3VuZCAhPSBudWxsKQoJewoJCWlmICghZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbC5SZWZlcmVuY2VTcGVjaWZpZWQpCgkJewoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKAoJCQkJQCJTYW1wbGVzXFN0YXJ0ZXIgQ29udGVudFxNYXRlcmlhbHNcQ29uY3JldGUgM3gzIG1ldGVyc1xDb25jcmV0ZSAzeDMgbWV0ZXJzLm1hdGVyaWFsIik7CgkJfQoJCWVsc2UKCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IG51bGw7Cgl9Cn0K")]
public class DynamicClass_dcb3438b_5064_499e_959f_b75278317289
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIF90aGlzID0gc2VuZGVyIGFzIENvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlOwoJaWYgKF90aGlzICE9IG51bGwpCgl7CgkJdmFyIGluZGljYXRvck1pbiA9IF90aGlzLkdldENvbXBvbmVudCgiSW5kaWNhdG9yIE1pbiIpIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsKCQlpZiAoaW5kaWNhdG9yTWluICE9IG51bGwpCgkJCWluZGljYXRvck1pbi5Db2xvciA9IF90aGlzLlZhbHVlLlZhbHVlIDw9IF90aGlzLlZhbHVlUmFuZ2UuVmFsdWUuTWluaW11bSA/IG5ldyBDb2xvclZhbHVlKDEsIDAsIDApIDogbmV3IENvbG9yVmFsdWUoMC41LCAwLjUsIDAuNSk7CgoJCXZhciBpbmRpY2F0b3JNYXggPSBfdGhpcy5HZXRDb21wb25lbnQoIkluZGljYXRvciBNYXgiKSBhcyBDb21wb25lbnRfTWVzaEluU3BhY2U7CgkJaWYgKGluZGljYXRvck1heCAhPSBudWxsKQoJCQlpbmRpY2F0b3JNYXguQ29sb3IgPSBfdGhpcy5WYWx1ZS5WYWx1ZSA+PSBfdGhpcy5WYWx1ZVJhbmdlLlZhbHVlLk1heGltdW0gPyBuZXcgQ29sb3JWYWx1ZSgwLCAxLCAwKSA6IG5ldyBDb2xvclZhbHVlKDAuNSwgMC41LCAwLjUpOwoKCQl2YXIgYnV0dG9uID0gX3RoaXMuR2V0Q29tcG9uZW50KCJCdXR0b24iKTsKCQlpZiAoYnV0dG9uICE9IG51bGwpCgkJewoJCQl2YXIgb2Zmc2V0ID0gYnV0dG9uLkdldENvbXBvbmVudDxDb21wb25lbnRfVHJhbnNmb3JtT2Zmc2V0PigpOwoJCQlpZiAob2Zmc2V0ICE9IG51bGwpCgkJCXsKCQkJCXZhciBhbmdsZSA9IF90aGlzLkdldFZhbHVlQW5nbGUoKSAtIDkwOwoJCQkJb2Zmc2V0LlJvdGF0aW9uT2Zmc2V0ID0gbmV3IEFuZ2xlcyhhbmdsZSwgMCwgMCkuVG9RdWF0ZXJuaW9uKCk7CgkJCX0KCQl9CgoJCXZhciBtYXJrZXJNaW4gPSBfdGhpcy5HZXRDb21wb25lbnQoIk1hcmtlciBNaW4iKTsKCQlpZiAobWFya2VyTWluICE9IG51bGwpCgkJewoJCQl2YXIgb2Zmc2V0ID0gbWFya2VyTWluLkdldENvbXBvbmVudDxDb21wb25lbnRfVHJhbnNmb3JtT2Zmc2V0PigpOwoJCQlpZiAob2Zmc2V0ICE9IG51bGwpCgkJCXsKCQkJCXZhciBhbmdsZSA9IF90aGlzLkFuZ2xlUmFuZ2UuVmFsdWUuTWluaW11bSAtIDkwOwoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7CgkJCQlvZmZzZXQuUG9zaXRpb25PZmZzZXQgPSBuZXcgVmVjdG9yMygwLjAxLCBNYXRoLkNvcyhhbmdsZVIpICogMC4wNCwgTWF0aC5TaW4oLWFuZ2xlUikgKiAwLjA0KTsKCQkJCW9mZnNldC5Sb3RhdGlvbk9mZnNldCA9IG5ldyBBbmdsZXMoYW5nbGUsIDAsIDApLlRvUXVhdGVybmlvbigpOwoJCQl9CgkJfQoKCQl2YXIgbWFya2VyTWF4ID0gX3RoaXMuR2V0Q29tcG9uZW50KCJNYXJrZXIgTWF4Iik7CgkJaWYgKG1hcmtlck1heCAhPSBudWxsKQoJCXsKCQkJdmFyIG9mZnNldCA9IG1hcmtlck1heC5HZXRDb21wb25lbnQ8Q29tcG9uZW50X1RyYW5zZm9ybU9mZnNldD4oKTsKCQkJaWYgKG9mZnNldCAhPSBudWxsKQoJCQl7CgkJCQl2YXIgYW5nbGUgPSBfdGhpcy5BbmdsZVJhbmdlLlZhbHVlLk1heGltdW0gLSA5MDsKCQkJCXZhciBhbmdsZVIgPSBNYXRoRXguRGVncmVlVG9SYWRpYW4oYW5nbGUpOwoJCQkJb2Zmc2V0LlBvc2l0aW9uT2Zmc2V0ID0gbmV3IFZlY3RvcjMoMC4wMSwgTWF0aC5Db3MoYW5nbGVSKSAqIDAuMDQsIE1hdGguU2luKC1hbmdsZVIpICogMC4wNCk7CgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsKCQkJfQoJCX0KCgkJdmFyIG1hcmtlckN1cnJlbnQgPSBfdGhpcy5HZXRDb21wb25lbnQoIk1hcmtlciBDdXJyZW50Iik7CgkJaWYgKG1hcmtlckN1cnJlbnQgIT0gbnVsbCkKCQl7CgkJCXZhciBvZmZzZXQgPSBtYXJrZXJDdXJyZW50LkdldENvbXBvbmVudDxDb21wb25lbnRfVHJhbnNmb3JtT2Zmc2V0PigpOwoJCQlpZiAob2Zmc2V0ICE9IG51bGwpCgkJCXsKCQkJCXZhciBhbmdsZSA9IF90aGlzLkdldFZhbHVlQW5nbGUoKSAtIDkwOwoJCQkJdmFyIGFuZ2xlUiA9IE1hdGhFeC5EZWdyZWVUb1JhZGlhbihhbmdsZSk7CgkJCQlvZmZzZXQuUG9zaXRpb25PZmZzZXQgPSBuZXcgVmVjdG9yMygwLjA2LCBNYXRoLkNvcyhhbmdsZVIpICogMC4wNCwgTWF0aC5TaW4oLWFuZ2xlUikgKiAwLjA0KTsKCQkJCW9mZnNldC5Sb3RhdGlvbk9mZnNldCA9IG5ldyBBbmdsZXMoYW5nbGUsIDAsIDApLlRvUXVhdGVybmlvbigpOwoJCQl9CgkJfQoJfQp9")]
public class DynamicClass_5fb9c49e_2720_403a_b55b_ee302de05628
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
public class DynamicClass_4617f1d3_377c_474f_ac1e_01542b649476
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW5wdXRQcm9jZXNzaW5nX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50X0lucHV0UHJvY2Vzc2luZyBzZW5kZXIsIFVJQ29udHJvbCBwbGF5U2NyZWVuLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQp7CgkvL2dldCBhY2Nlc3MgdG8gdGhlIHNoaXAKCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsKCWlmIChzaGlwID09IG51bGwpCgkJcmV0dXJuOwoKCS8vdmFyIGtleURvd24gPSBtZXNzYWdlIGFzIElucHV0TWVzc2FnZUtleURvd247CgkvL2lmKGtleURvd24gIT0gbnVsbCkKCS8vewoJLy8JaWYoa2V5RG93bi5LZXkgPT0gRUtleXMuU3BhY2UpCgkvLwl7CgkvLwkJLy92YXIgYm9keSA9IHNoaXAuR2V0Q29tcG9uZW50PENvbXBvbmVudF9SaWdpZEJvZHkyRD4oKTsKCS8vCQkvL2lmIChib2R5ICE9IG51bGwpCgkvLwkJLy97CgkvLwkJLy8JYm9keS5BcHBseUZvcmNlKG5ldyBWZWN0b3IyKDEsIDApKTsKCS8vCQkvL30KCS8vCX0KCS8vfQp9CgpwdWJsaWMgdm9pZCBJbnB1dFByb2Nlc3NpbmdfU2ltdWxhdGlvblN0ZXAoTmVvQXhpcy5Db21wb25lbnQgb2JqKQp7Cgl2YXIgc2VuZGVyID0gKE5lb0F4aXMuQ29tcG9uZW50X0lucHV0UHJvY2Vzc2luZylvYmo7CgoJLy9nZXQgYWNjZXNzIHRvIHRoZSBzaGlwCgl2YXIgc2hpcCA9IHNlbmRlci5QYXJlbnQ7CglpZiAoc2hpcCA9PSBudWxsKQoJCXJldHVybjsKCgkvL2NvbnRyb2wgdGhlIHNoaXAKCXZhciBib2R5ID0gc2hpcC5HZXRDb21wb25lbnQ8Q29tcG9uZW50X1JpZ2lkQm9keTJEPigpOwoJaWYgKGJvZHkgIT0gbnVsbCkKCXsKCQkvL2tleWJvYXJkCgoJCS8vZmx5IGZyb250CgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuVykgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5VcCkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5OdW1QYWQ4KSkKCQl7CgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOwoJCQlib2R5LkFwcGx5Rm9yY2UoZGlyICogMS4wKTsKCQl9CgoJCS8vZmx5IGJhY2sKCQlpZiAoc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5TKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkRvd24pIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkMikpCgkJewoJCQl2YXIgZGlyID0gYm9keS5UcmFuc2Zvcm1WLlJvdGF0aW9uLkdldEZvcndhcmQoKS5Ub1ZlY3RvcjIoKTsKCQkJYm9keS5BcHBseUZvcmNlKGRpciAqIC0xLjApOwoJCX0KCgkJLy90dXJuIGxlZnQKCQlpZiAoc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5BKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkxlZnQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkNCkpCgkJCWJvZHkuQXBwbHlUb3JxdWUoMS4wKTsKCgkJLy90dXJuIHJpZ2h0CgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuRCkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5SaWdodCkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5OdW1QYWQ2KSkKCQkJYm9keS5BcHBseVRvcnF1ZSgtMS4wKTsKCgkJLy9tb3ZlbWVudCBieSBqb3lzdGljayBheGVzCgkJaWYgKE1hdGguQWJzKHNlbmRlci5Kb3lzdGlja0F4ZXNbMF0pID49IDAuMDEpCgkJCWJvZHkuQXBwbHlUb3JxdWUoLXNlbmRlci5Kb3lzdGlja0F4ZXNbMF0pOwoJCWlmIChNYXRoLkFicyhzZW5kZXIuSm95c3RpY2tBeGVzWzFdKSA+PSAwLjAxKQoJCXsKCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7CgkJCWJvZHkuQXBwbHlGb3JjZShkaXIgKiBzZW5kZXIuSm95c3RpY2tBeGVzWzFdKTsKCQl9CgkJLy9Kb3lzdGlja0F4ZXMKCQkvL0pveXN0aWNrQnV0dG9ucwoJCS8vSm95c3RpY2tQT1ZzCgkJLy9Kb3lzdGlja1NsaWRlcnMKCQkvL0lzSm95c3RpY2tCdXR0b25QcmVzc2VkCgkJLy9HZXRKb3lzdGlja0F4aXMKCQkvL0dldEpveXN0aWNrUE9WCgkJLy9HZXRKb3lzdGlja1NsaWRlcgkJCQoKCX0KCn0K")]
public class DynamicClass_6acec172_98c5_4d91_88bf_20ee46e20554
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

[CSharpScriptGeneratedAttribute("aW50IE1ldGhvZCggaW50IGEsIGludCBiICkKewoJcmV0dXJuIGEgKyBiOwp9Cg==")]
public class DynamicClass_e6053728_f121_42ba_87c3_96bc06d04800
{
    public NeoAxis.Component_CSharpScript Owner;
    int Method(int a, int b)
    {
        return a + b;
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpCnsKCXZhciBhbmdsZSA9IEVuZ2luZUFwcC5FbmdpbmVUaW1lICogMC4zOwoJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIDIuNTsKCXZhciBsb29rVG8gPSBuZXcgVmVjdG9yMygxMS43Mzc0ODM5MTI0ODI3LCAtMC4wNTE3NzY3NTAzMjQzOSwgLTE1LjUwOTI3NTU4MjUwOTIpOwoJdmFyIGxvb2tBdCA9IFF1YXRlcm5pb24uTG9va0F0KC1vZmZzZXQsIG5ldyBWZWN0b3IzKDAsMCwxKSk7CgkKCXJldHVybiBuZXcgVHJhbnNmb3JtKCBsb29rVG8gKyBvZmZzZXQsIGxvb2tBdCwgVmVjdG9yMy5PbmUgKTsKfQo=")]
public class DynamicClass_0ee1086b_87c4_4a63_80c3_c1b99049afd8
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJdmFyIGxpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJEaXJlY3Rpb25hbCBMaWdodCIpIGFzIENvbXBvbmVudF9MaWdodDsKCWlmIChsaWdodCAhPSBudWxsKQoJCWxpZ2h0LkVuYWJsZWQgPSBzZW5kZXIuQWN0aXZhdGVkOwp9Cg==")]
public class DynamicClass_2b5b9700_5d21_4209_8c6f_617a5f7fe9ed
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
public class DynamicClass_233edf0b_6454_4a7a_80a9_0a68869c9bf1
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLkNvbXBvbmVudF9SZWd1bGF0b3JTd2l0Y2hJblNwYWNlIG9iaikKewoJdmFyIHNjZW5lID0gb2JqLlBhcmVudFNjZW5lOwoKCXZhciBtZXNoSW5TcGFjZSA9IHNjZW5lLkdldENvbXBvbmVudCgiR3JvdW5kIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOwoJaWYgKG1lc2hJblNwYWNlICE9IG51bGwpCgkJbWVzaEluU3BhY2UuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLjAgLSBvYmouVmFsdWUsIDEuMCwgMS4wIC0gb2JqLlZhbHVlKTsKfQo=")]
public class DynamicClass_3e4ab726_fa43_4566_be66_3afe03d66952
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpCnsKCXZhciBvYmplY3QxID0gc2VuZGVyLkNvbXBvbmVudHNbIlNwaGVyZSJdIGFzIENvbXBvbmVudF9NZXNoSW5TcGFjZTsKCWlmKG9iamVjdDEgIT0gbnVsbCkKCQlvYmplY3QxLkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMC41LCAwLjc1ICsgTWF0aC5TaW4oVGltZS5DdXJyZW50KSAqIDAuMjUsIDAuNSk7CgoJdmFyIG1hdGVyaWFsMiA9IHNlbmRlci5Db21wb25lbnRzWyJCb3hcXE1hdGVyaWFsIl0gYXMgQ29tcG9uZW50X01hdGVyaWFsOwoJaWYobWF0ZXJpYWwyICE9IG51bGwpCgkJbWF0ZXJpYWwyLkVtaXNzaXZlID0gbmV3IENvbG9yVmFsdWVQb3dlcmVkKDAsICgxLjAgKyBNYXRoLlNpbihUaW1lLkN1cnJlbnQpKSAqIDUsIDApOwoJCQoJdmFyIG1hdGVyaWFsMyA9IHNlbmRlci5Db21wb25lbnRzWyJDeWxpbmRlclxcTWF0ZXJpYWwiXSBhcyBDb21wb25lbnRfTWF0ZXJpYWw7CglpZihtYXRlcmlhbDMgIT0gbnVsbCkKCQltYXRlcmlhbDMuUHJvcGVydHlTZXQoIk11bHRpcGxpZXIiLCBuZXcgQ29sb3JWYWx1ZSgxLCAxLCAxLjAgKyAoMS4wICsgTWF0aC5TaW4oVGltZS5DdXJyZW50KSkgKiA1KSk7Cn0K")]
public class DynamicClass_f603e558_0296_4153_9a53_4abc1274b024
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJdmFyIGdyb3VuZCA9IHNjZW5lLkdldENvbXBvbmVudCgiR3JvdW5kIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOwoJaWYgKGdyb3VuZCAhPSBudWxsKQoJewoJCWlmICghZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbC5SZWZlcmVuY2VTcGVjaWZpZWQpCgkJewoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKCBAIkJhc2VcTWF0ZXJpYWxzXERhcmsgWWVsbG93Lm1hdGVyaWFsIik7CgkJfQoJCWVsc2UKCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IG51bGw7Cgl9Cn0K")]
public class DynamicClass_52dcb80c_3b9c_427d_9688_49d0cfbd4ef3
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

[CSharpScriptGeneratedAttribute("UXVhdGVybmlvbiBHZXRSb3RhdGlvbk9mZnNldCgpCnsKICAgIHZhciBzcGVlZCA9IC0wLjE7CiAgICB2YXIgbWF0ID0gTWF0cml4My5Gcm9tUm90YXRlQnlYKEVuZ2luZUFwcC5FbmdpbmVUaW1lICogc3BlZWQpOwogICAgcmV0dXJuIG1hdC5Ub1F1YXRlcm5pb24oKTsKfQ==")]
public class DynamicClass_7f14f731_bdbe_48fb_830b_36f91c29f893
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
public class DynamicClass_96027296_3732_4154_b2ff_2b245a003247
{
    public NeoAxis.Component_CSharpScript Owner;
    double Method()
    {
        return -EngineApp.EngineTime / 5;
    }
}

[CSharpScriptGeneratedAttribute("Ci8vIFRoZSBleGFtcGxlIG9mIGNvbXB1dGluZyBhIGxpc3Qgb2Ygb2JqZWN0cyBmb3IgdGhlIEdyb3VwT2ZPYmplY3RzIHVzaW5nIHRocmVhZHMuCgpjbGFzcyBEYXRhCnsKCXB1YmxpYyBDb21wb25lbnRfR3JvdXBPZk9iamVjdHMuT2JqZWN0W10gT2JqZWN0czsKfQoKcHVibGljIHZvaWQgQ29tcHV0ZVVzaW5nVGhyZWFkc19Db21wdXRlQmVnaW4oTmVvQXhpcy5Db21wb25lbnRfQ29tcHV0ZVVzaW5nVGhyZWFkcyBzZW5kZXIpCnsKCS8vTG9nLkluZm8oImJlZ2luIik7CgoJdmFyIGRhdGEgPSBuZXcgRGF0YSgpOwoJZGF0YS5PYmplY3RzID0gbmV3IENvbXBvbmVudF9Hcm91cE9mT2JqZWN0cy5PYmplY3RbMTAwICogMTAwXTsKCXNlbmRlci5Db250ZXh0LkFueURhdGEgPSBkYXRhOwoKCS8vaG93IHRvIHNraXAgb25lIGNvbXB1dGluZzoKCS8vc2VuZGVyLkNvbnRleHQuQWxsb3dDb21wdXRlID0gZmFsc2U7Cn0KCnB1YmxpYyB2b2lkIENvbXB1dGVVc2luZ1RocmVhZHNfQ29tcHV0ZVRocmVhZChOZW9BeGlzLkNvbXBvbmVudF9Db21wdXRlVXNpbmdUaHJlYWRzIHNlbmRlciwgaW50IHRocmVhZEluZGV4KQp7Cgl2YXIgZGF0YSA9IHNlbmRlci5Db250ZXh0LkFueURhdGEgYXMgRGF0YTsKCWlmIChkYXRhID09IG51bGwpCgkJcmV0dXJuOwoKCXZhciBvYmplY3RzID0gZGF0YS5PYmplY3RzOwoKCS8vZ2V0IHJhbmdlIG9mIG9iamVjdHMgZm9yIHRoZSB0aHJlYWQKCXZhciBmcm9tID0gKGRhdGEuT2JqZWN0cy5MZW5ndGggKiB0aHJlYWRJbmRleCkgLyBzZW5kZXIuVGhyZWFkQ291bnQ7Cgl2YXIgdG8gPSAoZGF0YS5PYmplY3RzLkxlbmd0aCAqICh0aHJlYWRJbmRleCArIDEpKSAvIHNlbmRlci5UaHJlYWRDb3VudDsKCgl2YXIgcmFuZG9tID0gbmV3IE5lb0F4aXMuUmFuZG9tKDApOy8vKGludCkoVGltZS5DdXJyZW50ICogMC4yNSkpOwoKCS8vY29tcHV0ZSBvYmplY3RzCglmb3IgKGludCBuID0gZnJvbTsgbiA8IHRvOyBuKyspCgl7CgkJcmVmIHZhciBvYmogPSByZWYgb2JqZWN0c1tuXTsKCgkJdmFyIHggPSBuICUgMTAwOwoJCXZhciB5ID0gbiAvIDEwMDsKCgkJdmFyIHdhdmluZ1N0YXJ0ID0gcmFuZG9tLk5leHQoTWF0aC5QSSAqIDIpOwoJCXZhciB3YXZpbmdDeWNsZSA9IHJhbmRvbS5OZXh0KDEuMCwgMi4wKTsKCQkKCQkvL3VzZSBBbnlEYXRhIHRvIHN0b3JlIGFkZGl0aW9uYWwgZGF0YSBpbiB0aGUgb2JqZWN0CgkJLy9vYmouQW55RGF0YSA9IDsKCgkJb2JqLkVsZW1lbnQgPSAodXNob3J0KXJhbmRvbS5OZXh0KDIpOwoJCW9iai5GbGFncyA9IENvbXBvbmVudF9Hcm91cE9mT2JqZWN0cy5PYmplY3QuRmxhZ3NFbnVtLkVuYWJsZWQgfCBDb21wb25lbnRfR3JvdXBPZk9iamVjdHMuT2JqZWN0LkZsYWdzRW51bS5WaXNpYmxlOwoJCW9iai5Qb3NpdGlvbiA9IG5ldyBWZWN0b3IzKHggKiAxLjMsIHkgKiAxLjMsIDEuMCArIE1hdGguU2luKCB3YXZpbmdTdGFydCArIFRpbWUuQ3VycmVudCAqIHdhdmluZ0N5Y2xlICkgKiAwLjI1ICk7CgkJb2JqLlJvdGF0aW9uID0gUXVhdGVybmlvbkYuSWRlbnRpdHk7CgkJb2JqLlNjYWxlID0gbmV3IFZlY3RvcjNGKDEsIDEsIDEpOwoJCW9iai5Db2xvciA9IG5ldyBDb2xvclZhbHVlKHJhbmRvbS5OZXh0RmxvYXQoKSwgcmFuZG9tLk5leHRGbG9hdCgpLCByYW5kb20uTmV4dEZsb2F0KCkpOwoKCQkvL3ZhciBwb3MgPSBuZXcgVmVjdG9yMyhuICogMS4zLCAwLCAxKTsKCQkvL3ZhciByb3QgPSBRdWF0ZXJuaW9uRi5JZGVudGl0eTsKCQkvL3ZhciBzY2wgPSBuZXcgVmVjdG9yM0YoMSwgMSwgMSk7CgkJLy9kYXRhLk9iamVjdHNbbl0gPSBuZXcgQ29tcG9uZW50X0dyb3VwT2ZPYmplY3RzLk9iamVjdCgwLCAwLCAwLCBDb21wb25lbnRfR3JvdXBPZk9iamVjdHMuT2JqZWN0LkZsYWdzRW51bS5FbmFibGVkIHwgQ29tcG9uZW50X0dyb3VwT2ZPYmplY3RzLk9iamVjdC5GbGFnc0VudW0uVmlzaWJsZSwgcG9zLCByb3QsIHNjbCwgVmVjdG9yNEYuWmVybywgQ29sb3JWYWx1ZS5PbmUsIFZlY3RvcjRGLlplcm8sIFZlY3RvcjRGLlplcm8pOyAKCX0KfQoKcHVibGljIHZvaWQgQ29tcHV0ZVVzaW5nVGhyZWFkc19Db21wdXRlRW5kKE5lb0F4aXMuQ29tcG9uZW50X0NvbXB1dGVVc2luZ1RocmVhZHMgc2VuZGVyKQp7CgkvL0xvZy5JbmZvKCJlbmQiKTsKCgl2YXIgZGF0YSA9IHNlbmRlci5Db250ZXh0LkFueURhdGEgYXMgRGF0YTsKCWlmIChkYXRhID09IG51bGwpCgkJcmV0dXJuOwoKCXZhciBncm91cE9mT2JqZWN0cyA9IHNlbmRlci5QYXJlbnQgYXMgQ29tcG9uZW50X0dyb3VwT2ZPYmplY3RzOwoJaWYgKGdyb3VwT2ZPYmplY3RzICE9IG51bGwpCgkJZ3JvdXBPZk9iamVjdHMuT2JqZWN0c1NldChkYXRhLk9iamVjdHMsIHRydWUpOwp9Cg==")]
public class DynamicClass_ecc0400e_e97a_43be_b774_6483635d0fa7
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgTWVzaEluU3BhY2VBbmltYXRpb25Db250cm9sbGVyX0NhbGN1bGF0ZUJvbmVUcmFuc2Zvcm1zKE5lb0F4aXMuQ29tcG9uZW50X01lc2hJblNwYWNlQW5pbWF0aW9uQ29udHJvbGxlciBzZW5kZXIsIE5lb0F4aXMuQ29tcG9uZW50X1NrZWxldG9uQW5pbWF0aW9uVHJhY2suQ2FsY3VsYXRlQm9uZVRyYW5zZm9ybXNJdGVtW10gcmVzdWx0KQp7CgkvL3RvIGVuYWJsZSB0aGlzIGV2ZW50IGhhbmRsZXIgaW4gdGhlIGVkaXRvciBjaGFuZ2UgIldoZW4gRW5hYmxlIiBwcm9wZXJ0eSB0byAiU2ltdWxhdGlvbiB8IEluc3RhbmNlIHwgRWRpdG9yIi4KCS8vY29tcG9uZW50OiBDaGFyYWN0ZXIvTWVzaCBJbiBTcGFjZS9DIyBTY3JpcHQvRXZlbnQgSGFuZGxlciBDYWxjdWxhdGVCb25lVHJhbnNmb3Jtcy4KCQoJdmFyIGJvbmVJbmRleCA9IHNlbmRlci5HZXRCb25lSW5kZXgoIm1peGFtb3JpZzpTcGluZTEiKTsKCWlmKGJvbmVJbmRleCAhPSAtMSkKCXsKCQlyZWYgdmFyIGl0ZW0gPSByZWYgcmVzdWx0W2JvbmVJbmRleF07CgoJCS8vY2FsY3VsYXRlIGJvbmUgb2Zmc2V0CgkJdmFyIGFuZ2xlID0gbmV3IERlZ3JlZSg2MCkgKiBNYXRoLlNpbihUaW1lLkN1cnJlbnQpOyAKCQl2YXIgb2Zmc2V0ID0gTWF0cml4M0YuRnJvbVJvdGF0ZUJ5WSgoZmxvYXQpYW5nbGUuSW5SYWRpYW5zKCkpLlRvUXVhdGVybmlvbigpOwoJCQoJCS8vdXBkYXRlIHRoZSBib25lCgkJaXRlbS5Sb3RhdGlvbiAqPSBvZmZzZXQ7Cgl9CQp9Cg==")]
public class DynamicClass_1eb7d7da_0db6_4120_a6e9_6d49a226cb5b
{
    public NeoAxis.Component_CSharpScript Owner;
    public void MeshInSpaceAnimationController_CalculateBoneTransforms(NeoAxis.Component_MeshInSpaceAnimationController sender, NeoAxis.Component_SkeletonAnimationTrack.CalculateBoneTransformsItem[] result)
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW5wdXRQcm9jZXNzaW5nX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50X0lucHV0UHJvY2Vzc2luZyBzZW5kZXIsIFVJQ29udHJvbCBwbGF5U2NyZWVuLCBOZW9BeGlzLklucHV0TWVzc2FnZSBtZXNzYWdlKQp7CgkvL2dldCBhY2Nlc3MgdG8gdGhlIHNoaXAKCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsKCWlmIChzaGlwID09IG51bGwpCgkJcmV0dXJuOwoKCS8qCgkJdmFyIGtleURvd24gPSBtZXNzYWdlIGFzIElucHV0TWVzc2FnZUtleURvd247CgkJaWYoa2V5RG93biAhPSBudWxsKQoJCXsKCQkJaWYoa2V5RG93bi5LZXkgPT0gRUtleXMuU3BhY2UpCgkJCXsKCQkJCS8vdmFyIGJvZHkgPSBzaGlwLkdldENvbXBvbmVudDxDb21wb25lbnRfUmlnaWRCb2R5MkQ+KCk7CgkJCQkvL2lmIChib2R5ICE9IG51bGwpCgkJCQkvL3sKCQkJCS8vCWJvZHkuQXBwbHlGb3JjZShuZXcgVmVjdG9yMigxLCAwKSk7CgkJCQkvL30KCQkJfQoJCX0KCSovCgp9CgpwdWJsaWMgdm9pZCBJbnB1dFByb2Nlc3NpbmdfU2ltdWxhdGlvblN0ZXAoTmVvQXhpcy5Db21wb25lbnQgb2JqKQp7Cgl2YXIgc2VuZGVyID0gKE5lb0F4aXMuQ29tcG9uZW50X0lucHV0UHJvY2Vzc2luZylvYmo7CgoJLy9nZXQgYWNjZXNzIHRvIHRoZSBzaGlwCgl2YXIgc2hpcCA9IHNlbmRlci5QYXJlbnQ7CglpZiAoc2hpcCA9PSBudWxsKQoJCXJldHVybjsKCgkvL2NvbnRyb2wgdGhlIHNoaXAKCXZhciBib2R5ID0gc2hpcC5HZXRDb21wb25lbnQ8Q29tcG9uZW50X1JpZ2lkQm9keTJEPigpOwoJaWYgKGJvZHkgIT0gbnVsbCkKCXsKCQkvL2ZseSBmcm9udAoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlcpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuVXApKQoJCXsKCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7CgkJCWJvZHkuQXBwbHlGb3JjZShkaXIgKiAxLjApOwoJCX0KCgkJLy9mbHkgYmFjawoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLlMpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuRG93bikpCgkJewoJCQl2YXIgZGlyID0gYm9keS5UcmFuc2Zvcm1WLlJvdGF0aW9uLkdldEZvcndhcmQoKS5Ub1ZlY3RvcjIoKTsKCQkJYm9keS5BcHBseUZvcmNlKGRpciAqIC0xLjApOwoJCX0KCgkJLy90dXJuIGxlZnQKCQlpZiAoc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5BKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkxlZnQpKQoJCQlib2R5LkFwcGx5VG9ycXVlKDEuMCk7CgoJCS8vdHVybiByaWdodAoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuUmlnaHQpKQoJCQlib2R5LkFwcGx5VG9ycXVlKC0xLjApOwoJfQoKfQo=")]
public class DynamicClass_a81eefab_23e8_4980_8ef4_292604214434
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQ29tcG9uZW50X0J1dHRvbkluU3BhY2Ugc2VuZGVyKQp7Cgl2YXIgc2NlbmUgPSBzZW5kZXIuUGFyZW50U2NlbmU7CgoJdmFyIGdyb3VuZCA9IHNjZW5lLkdldENvbXBvbmVudCgiR3JvdW5kIikgYXMgQ29tcG9uZW50X01lc2hJblNwYWNlOwoJaWYgKGdyb3VuZCAhPSBudWxsKQoJewoJCWlmICghZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbC5SZWZlcmVuY2VTcGVjaWZpZWQpCgkJewoJCQlncm91bmQuUmVwbGFjZU1hdGVyaWFsID0gUmVmZXJlbmNlVXRpbGl0eS5NYWtlUmVmZXJlbmNlKAoJCQkJQCJDb250ZW50XE1hdGVyaWFsc1xCYXNpYyBMaWJyYXJ5XENvbmNyZXRlXENvbmNyZXRlIEZsb29yIDAxLm1hdGVyaWFsIik7CgkJfQoJCWVsc2UKCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IG51bGw7Cgl9Cn0K")]
public class DynamicClass_e1899882_2b66_4eb8_a4d4_c145c4ac55fa
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

[CSharpScriptGeneratedAttribute("ZG91YmxlIE1ldGhvZCgpCnsKCXJldHVybiBNYXRoRXguU2F0dXJhdGUoICggTWF0aC5TaW4oIEVuZ2luZUFwcC5FbmdpbmVUaW1lICogMS4zICkgKyAxLjAgKSAvIDIgKTsKfQo=")]
public class DynamicClass_e8f9b7f5_500c_4e19_ae0a_18bd3027dc02
{
    public NeoAxis.Component_CSharpScript Owner;
    double Method()
    {
        return MathEx.Saturate((Math.Sin(EngineApp.EngineTime * 1.3) + 1.0) / 2);
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpCnsKCXZhciBhbmdsZSA9IEVuZ2luZUFwcC5FbmdpbmVUaW1lICogLTEuMDsKCXZhciBvZmZzZXQgPSBuZXcgVmVjdG9yMyhNYXRoLkNvcyhhbmdsZSksIE1hdGguU2luKGFuZ2xlKSwgMCkgKiAyLjA7Cgl2YXIgbG9va1RvID0gbmV3IFZlY3RvcjMoMTEuNzM3NDgzOTEyNDgyNywgLTAuMDUxNzc2NzUwMzI0MzksIC0xNC44MDkyNzU1ODI1MDkyKTsKCXZhciBsb29rQXQgPSBRdWF0ZXJuaW9uLkxvb2tBdCgtb2Zmc2V0LCBuZXcgVmVjdG9yMygwLDAsMSkpOwoJCglyZXR1cm4gbmV3IFRyYW5zZm9ybSggbG9va1RvICsgb2Zmc2V0LCBsb29rQXQsIFZlY3RvcjMuT25lICk7Cn0K")]
public class DynamicClass_51e7e35e_7446_487c_aea1_668a744aa2bb
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

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpCnsKCXZhciBhbmdsZSA9IEVuZ2luZUFwcC5FbmdpbmVUaW1lICogMS4zOwoJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIDIuMDsKCXZhciBsb29rVG8gPSBuZXcgVmVjdG9yMygxMS43Mzc0ODM5MTI0ODI3LCAtMC4wNTE3NzY3NTAzMjQzOSwgLTE1LjUwOTI3NTU4MjUwOTIpOwoJdmFyIGxvb2tBdCA9IFF1YXRlcm5pb24uTG9va0F0KC1vZmZzZXQsIG5ldyBWZWN0b3IzKDAsMCwxKSk7CgkKCXJldHVybiBuZXcgVHJhbnNmb3JtKCBsb29rVG8gKyBvZmZzZXQsIGxvb2tBdCwgbmV3IFZlY3RvcjMoMC41LDAuNSwwLjUpICk7Cn0K")]
public class DynamicClass_89ad96a7_db56_41a0_abda_9b79606731c6
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

[CSharpScriptGeneratedAttribute("Q29tcG9uZW50X1JlbmRlcmluZ1BpcGVsaW5lIEdldFBpcGVsaW5lKCkKewoJc3RyaW5nIG5hbWU7CglpZihFbmdpbmVBcHAuRW5naW5lVGltZSAlIDQgPiAyKQoJCW5hbWUgPSAiUmVuZGVyaW5nIFBpcGVsaW5lIjsKCWVsc2UKCQluYW1lID0gIlJlbmRlcmluZyBQaXBlbGluZSAyIjsKCQkKCXJldHVybiBPd25lci5QYXJlbnQuR2V0Q29tcG9uZW50KG5hbWUpIGFzIENvbXBvbmVudF9SZW5kZXJpbmdQaXBlbGluZTsKfQo=")]
public class DynamicClass_c75a99c3_fb2a_451d_85aa_45a46d32e70e
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