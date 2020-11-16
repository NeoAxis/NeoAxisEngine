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
public class DynamicClass_7ae00e83_5936_41b2_a5b2_a9ecdf38d31a
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
public class DynamicClass_16c1cf69_68c4_41d2_9de5_f7eab6678173
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
public class DynamicClass_c89c1fbd_1744_4a8a_9ab1_05ee68d9a10e
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
public class DynamicClass_f6d13fe4_a43e_4860_b90a_26f5f6f9a13a
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uRG9fQ2xpY2soTmVvQXhpcy5VSUJ1dHRvbiBzZW5kZXIpDQp7DQoJdmFyIHBhcmVudCA9IHNlbmRlci5QYXJlbnQ7DQoJdmFyIGxpbmsgPSBwYXJlbnQuUHJvcGVydHlHZXQ8c3RyaW5nPigiTGVhcm4gTGluayIpOw0KCVN5c3RlbS5EaWFnbm9zdGljcy5Qcm9jZXNzLlN0YXJ0KCBuZXcgU3lzdGVtLkRpYWdub3N0aWNzLlByb2Nlc3NTdGFydEluZm8oIGxpbmsgKSB7IFVzZVNoZWxsRXhlY3V0ZSA9IHRydWUgfSApOw0KfQ0K")]
public class DynamicClass_7ae8dd67_6814_4264_a1d4_74be1b613c2b
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpDQp7DQoJdmFyIGNvbnRyb2wgPSAoVUlDb250cm9sKXNlbmRlcjsNCgljb250cm9sLkNvbG9yTXVsdGlwbGllciA9IGNvbnRyb2wuUmVhZE9ubHkgPyBuZXcgQ29sb3JWYWx1ZSgwLjUsIDAuNSwgMC41KSA6IG5ldyBDb2xvclZhbHVlKDEsIDEsIDEpOw0KfQ0K")]
public class DynamicClass_891bf813_c8a2_4d49_b69e_55030323fa73
{
    public NeoAxis.Component_CSharpScript Owner;
    public void _UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var control = (UIControl)sender;
        control.ColorMultiplier = control.ReadOnly ? new ColorValue(0.5, 0.5, 0.5) : new ColorValue(1, 1, 1);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29udHJvbF9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQ0Kew0KCXZhciB0YWJDb250cm9sID0gc2VuZGVyLkNvbXBvbmVudHNbIlRhYiBDb250cm9sIl0gYXMgVUlUYWJDb250cm9sOw0KCWlmKHRhYkNvbnRyb2wgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQoJYm9vbCBJc0RvbmUoVUlDb250cm9sIGJsb2NrKQ0KCXsNCgkJdmFyIGNoZWNrID0gYmxvY2suR2V0Q29tcG9uZW50PFVJQ2hlY2s+KCJDaGVjayBEb25lIik7DQoJCXJldHVybiBjaGVjayAhPSBudWxsICYmIGNoZWNrLkNoZWNrZWQuVmFsdWUgPT0gVUlDaGVjay5DaGVja1ZhbHVlLkNoZWNrZWQ7IA0KCX0NCg0KCXZhciBwYWdlQmFzaWMgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBCYXNpYyIpIGFzIFVJQ29udHJvbDsNCglpZihwYWdlQmFzaWMgIT0gbnVsbCkNCgl7DQoJCXZhciBibG9jazEgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDIiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazMgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDMiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazQgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDUiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazYgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDYiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazcgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoNCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCWJsb2NrMy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2syKTsNCgkJYmxvY2s0LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazIpOw0KCQlibG9jazYuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMykgJiYgIUlzRG9uZShibG9jazUpOw0KCQlibG9jazcuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSkgfHwgIUlzRG9uZShibG9jazIpIHx8ICFJc0RvbmUoYmxvY2szKSB8fCAhSXNEb25lKGJsb2NrNCkgfHwgIUlzRG9uZShibG9jazUpIHx8ICFJc0RvbmUoYmxvY2s2KTsNCgkJDQoJCXZhciB0YWJCdXR0b25zID0gdGFiQ29udHJvbC5HZXRBbGxCdXR0b25zKCk7DQoJCXRhYkJ1dHRvbnNbMV0uUmVhZE9ubHkgPSBibG9jazcuUmVhZE9ubHk7DQoJCXRhYkJ1dHRvbnNbMl0uUmVhZE9ubHkgPSBibG9jazcuUmVhZE9ubHk7DQoNCgl9DQoJCQkNCn0NCg==")]
public class DynamicClass_0a41b21c_6b79_4af9_8417_e1f3769de6cf
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
            tabButtons[1].ReadOnly = block7.ReadOnly;
            tabButtons[2].ReadOnly = block7.ReadOnly;
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29udHJvbF9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQ0Kew0KCXZhciB0YWJDb250cm9sID0gc2VuZGVyLkNvbXBvbmVudHNbIlRhYiBDb250cm9sIl0gYXMgVUlUYWJDb250cm9sOw0KCWlmKHRhYkNvbnRyb2wgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQoJYm9vbCBJc0RvbmUoVUlDb250cm9sIGJsb2NrKQ0KCXsNCgkJdmFyIGNoZWNrID0gYmxvY2suR2V0Q29tcG9uZW50PFVJQ2hlY2s+KCJDaGVjayBEb25lIik7DQoJCXJldHVybiBjaGVjayAhPSBudWxsICYmIGNoZWNrLkNoZWNrZWQuVmFsdWUgPT0gVUlDaGVjay5DaGVja1ZhbHVlLkNoZWNrZWQ7IA0KCX0NCg0KCXZhciBwYWdlQmFzaWMgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBCYXNpYyIpIGFzIFVJQ29udHJvbDsNCglpZihwYWdlQmFzaWMgIT0gbnVsbCkNCgl7DQoJCXZhciBibG9jazEgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDIiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazMgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDMiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazQgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDUiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazYgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDYiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazcgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoNCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCWJsb2NrMy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2syKTsNCgkJYmxvY2s0LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazIpOw0KCQlibG9jazYuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMykgJiYgIUlzRG9uZShibG9jazUpOw0KCQlibG9jazcuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSkgfHwgIUlzRG9uZShibG9jazIpIHx8ICFJc0RvbmUoYmxvY2szKSB8fCAhSXNEb25lKGJsb2NrNCkgfHwgIUlzRG9uZShibG9jazUpIHx8ICFJc0RvbmUoYmxvY2s2KTsNCgkJDQoJCXZhciB0YWJCdXR0b25zID0gdGFiQ29udHJvbC5HZXRBbGxCdXR0b25zKCk7DQoJCXRhYkJ1dHRvbnNbMV0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJCXRhYkJ1dHRvbnNbMl0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoNCgl9DQoJCQkNCn0NCg==")]
public class DynamicClass_5441768f_5b8f_42c9_be3b_aece9b2beea1
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
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29udHJvbF9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQ0Kew0KCXZhciB0YWJDb250cm9sID0gc2VuZGVyLkNvbXBvbmVudHNbIlRhYiBDb250cm9sIl0gYXMgVUlUYWJDb250cm9sOw0KCWlmKHRhYkNvbnRyb2wgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQoJYm9vbCBJc0RvbmUoVUlDb250cm9sIGJsb2NrKQ0KCXsNCgkJdmFyIGNoZWNrID0gYmxvY2suR2V0Q29tcG9uZW50PFVJQ2hlY2s+KCJDaGVjayBEb25lIik7DQoJCXJldHVybiBjaGVjayAhPSBudWxsICYmIGNoZWNrLkNoZWNrZWQuVmFsdWUgPT0gVUlDaGVjay5DaGVja1ZhbHVlLkNoZWNrZWQ7IA0KCX0NCg0KCXZhciBwYWdlQmFzaWMgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBCYXNpYyIpIGFzIFVJQ29udHJvbDsNCglpZihwYWdlQmFzaWMgIT0gbnVsbCkNCgl7DQoJCXZhciBibG9jazEgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDIiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazMgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDMiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazQgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDUiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazYgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDYiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazcgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoNCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCWJsb2NrMy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2syKTsNCgkJYmxvY2s0LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazIpOw0KCQlibG9jazYuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMykgJiYgIUlzRG9uZShibG9jazUpOw0KCQlibG9jazcuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSkgfHwgIUlzRG9uZShibG9jazIpIHx8ICFJc0RvbmUoYmxvY2szKSB8fCAhSXNEb25lKGJsb2NrNCkgfHwgIUlzRG9uZShibG9jazUpIHx8ICFJc0RvbmUoYmxvY2s2KTsNCgkJDQoJCXZhciB0YWJCdXR0b25zID0gdGFiQ29udHJvbC5HZXRBbGxCdXR0b25zKCk7DQoJCXRhYkJ1dHRvbnNbMV0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJCXRhYkJ1dHRvbnNbMl0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJfQ0KCQkJDQp9DQo=")]
public class DynamicClass_3a73740e_53a9_44b4_9bf7_9ef0b76d7d90
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
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29udHJvbF9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQ0Kew0KCXZhciB0YWJDb250cm9sID0gc2VuZGVyLkNvbXBvbmVudHNbIlRhYiBDb250cm9sIl0gYXMgVUlUYWJDb250cm9sOw0KCWlmKHRhYkNvbnRyb2wgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQoJYm9vbCBJc0RvbmUoVUlDb250cm9sIGJsb2NrKQ0KCXsNCgkJdmFyIGNoZWNrID0gYmxvY2suR2V0Q29tcG9uZW50PFVJQ2hlY2s+KCJDaGVjayBEb25lIik7DQoJCXJldHVybiBjaGVjayAhPSBudWxsICYmIGNoZWNrLkNoZWNrZWQuVmFsdWUgPT0gVUlDaGVjay5DaGVja1ZhbHVlLkNoZWNrZWQ7IA0KCX0NCg0KCXZhciBwYWdlQmFzaWMgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBCYXNpYyIpIGFzIFVJQ29udHJvbDsNCglpZihwYWdlQmFzaWMgIT0gbnVsbCkNCgl7DQoJCXZhciBibG9jazEgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDIiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazMgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDMiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazQgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDUiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazYgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDYiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazcgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoNCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCWJsb2NrMy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2syKTsNCgkJYmxvY2s0LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazIpOw0KCQlibG9jazYuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMykgJiYgIUlzRG9uZShibG9jazUpOw0KCQlibG9jazcuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSkgfHwgIUlzRG9uZShibG9jazIpIHx8ICFJc0RvbmUoYmxvY2szKSB8fCAhSXNEb25lKGJsb2NrNCkgfHwgIUlzRG9uZShibG9jazUpIHx8ICFJc0RvbmUoYmxvY2s2KTsNCgkJDQoJCXZhciB0YWJCdXR0b25zID0gdGFiQ29udHJvbC5HZXRBbGxCdXR0b25zKCk7DQoJCXRhYkJ1dHRvbnNbMV0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJCXRhYkJ1dHRvbnNbMl0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJfQ0KDQoJdmFyIHBhZ2VTY3JpcHRpbmcgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBTY3JpcHRpbmciKSBhcyBVSUNvbnRyb2w7DQoJaWYocGFnZVNjcmlwdGluZyAhPSBudWxsKQ0KCXsNCgkJDQoJfQ0KDQp9DQo=")]
public class DynamicClass_5d2c4f30_71ff_4fdb_ae8c_5764c3d48ed5
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
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29udHJvbF9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQ0Kew0KCXZhciB0YWJDb250cm9sID0gc2VuZGVyLkNvbXBvbmVudHNbIlRhYiBDb250cm9sIl0gYXMgVUlUYWJDb250cm9sOw0KCWlmKHRhYkNvbnRyb2wgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQoJYm9vbCBJc0RvbmUoVUlDb250cm9sIGJsb2NrKQ0KCXsNCgkJdmFyIGNoZWNrID0gYmxvY2suR2V0Q29tcG9uZW50PFVJQ2hlY2s+KCJDaGVjayBEb25lIik7DQoJCXJldHVybiBjaGVjayAhPSBudWxsICYmIGNoZWNrLkNoZWNrZWQuVmFsdWUgPT0gVUlDaGVjay5DaGVja1ZhbHVlLkNoZWNrZWQ7IA0KCX0NCg0KCXZhciBwYWdlQmFzaWMgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBCYXNpYyIpIGFzIFVJQ29udHJvbDsNCglpZihwYWdlQmFzaWMgIT0gbnVsbCkNCgl7DQoJCXZhciBibG9jazEgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDIiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazMgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDMiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazQgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDUiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazYgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDYiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazcgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoNCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCWJsb2NrMy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2syKTsNCgkJYmxvY2s0LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazIpOw0KCQlibG9jazYuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMykgJiYgIUlzRG9uZShibG9jazUpOw0KCQlibG9jazcuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSkgfHwgIUlzRG9uZShibG9jazIpIHx8ICFJc0RvbmUoYmxvY2szKSB8fCAhSXNEb25lKGJsb2NrNCkgfHwgIUlzRG9uZShibG9jazUpIHx8ICFJc0RvbmUoYmxvY2s2KTsNCgkJDQoJCXZhciB0YWJCdXR0b25zID0gdGFiQ29udHJvbC5HZXRBbGxCdXR0b25zKCk7DQoJCXRhYkJ1dHRvbnNbMV0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJCXRhYkJ1dHRvbnNbMl0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJfQ0KDQoJdmFyIHBhZ2VTY3JpcHRpbmcgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBTY3JpcHRpbmciKSBhcyBVSUNvbnRyb2w7DQoJaWYocGFnZVNjcmlwdGluZyAhPSBudWxsKQ0KCXsNCgkJdmFyIGJsb2NrMSA9IHBhZ2VCYXNpYy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgMSIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrMiA9IHBhZ2VCYXNpYy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgMiIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrMyA9IHBhZ2VCYXNpYy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgMyIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrNCA9IHBhZ2VCYXNpYy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgNCIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrNSA9IHBhZ2VCYXNpYy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgNSIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrNiA9IHBhZ2VCYXNpYy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgNiIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrNyA9IHBhZ2VCYXNpYy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgNyIpIGFzIFVJQ29udHJvbDsNCgkJDQoJfQ0KDQp9DQo=")]
public class DynamicClass_c6d16a87_ef18_4ad2_a574_ce11d68d7d71
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
            var block1 = pageBasic.GetComponent("LearningBlock 1") as UIControl;
            var block2 = pageBasic.GetComponent("LearningBlock 2") as UIControl;
            var block3 = pageBasic.GetComponent("LearningBlock 3") as UIControl;
            var block4 = pageBasic.GetComponent("LearningBlock 4") as UIControl;
            var block5 = pageBasic.GetComponent("LearningBlock 5") as UIControl;
            var block6 = pageBasic.GetComponent("LearningBlock 6") as UIControl;
            var block7 = pageBasic.GetComponent("LearningBlock 7") as UIControl;
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29udHJvbF9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQ0Kew0KCXZhciB0YWJDb250cm9sID0gc2VuZGVyLkNvbXBvbmVudHNbIlRhYiBDb250cm9sIl0gYXMgVUlUYWJDb250cm9sOw0KCWlmKHRhYkNvbnRyb2wgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQoJYm9vbCBJc0RvbmUoVUlDb250cm9sIGJsb2NrKQ0KCXsNCgkJdmFyIGNoZWNrID0gYmxvY2suR2V0Q29tcG9uZW50PFVJQ2hlY2s+KCJDaGVjayBEb25lIik7DQoJCXJldHVybiBjaGVjayAhPSBudWxsICYmIGNoZWNrLkNoZWNrZWQuVmFsdWUgPT0gVUlDaGVjay5DaGVja1ZhbHVlLkNoZWNrZWQ7IA0KCX0NCg0KCXZhciBwYWdlQmFzaWMgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBCYXNpYyIpIGFzIFVJQ29udHJvbDsNCglpZihwYWdlQmFzaWMgIT0gbnVsbCkNCgl7DQoJCXZhciBibG9jazEgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDIiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazMgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDMiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazQgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDUiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazYgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDYiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazcgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoNCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCWJsb2NrMy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2syKTsNCgkJYmxvY2s0LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazIpOw0KCQlibG9jazYuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMykgJiYgIUlzRG9uZShibG9jazUpOw0KCQlibG9jazcuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSkgfHwgIUlzRG9uZShibG9jazIpIHx8ICFJc0RvbmUoYmxvY2szKSB8fCAhSXNEb25lKGJsb2NrNCkgfHwgIUlzRG9uZShibG9jazUpIHx8ICFJc0RvbmUoYmxvY2s2KTsNCgkJDQoJCXZhciB0YWJCdXR0b25zID0gdGFiQ29udHJvbC5HZXRBbGxCdXR0b25zKCk7DQoJCXRhYkJ1dHRvbnNbMV0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJCXRhYkJ1dHRvbnNbMl0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJfQ0KDQoJdmFyIHBhZ2VTY3JpcHRpbmcgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBTY3JpcHRpbmciKSBhcyBVSUNvbnRyb2w7DQoJaWYocGFnZVNjcmlwdGluZyAhPSBudWxsKQ0KCXsNCgkJdmFyIGJsb2NrMSA9IHBhZ2VTY3JpcHRpbmcuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayAyIikgYXMgVUlDb250cm9sOw0KCQl2YXIgYmxvY2szID0gcGFnZVNjcmlwdGluZy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgMyIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrNCA9IHBhZ2VTY3JpcHRpbmcuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA1IikgYXMgVUlDb250cm9sOw0KCQl2YXIgYmxvY2s2ID0gcGFnZVNjcmlwdGluZy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgNiIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrNyA9IHBhZ2VTY3JpcHRpbmcuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoJCQ0KCX0NCg0KfQ0K")]
public class DynamicClass_31051165_7519_46be_b2dd_16940d564674
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
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29udHJvbF9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQ0Kew0KCXZhciB0YWJDb250cm9sID0gc2VuZGVyLkNvbXBvbmVudHNbIlRhYiBDb250cm9sIl0gYXMgVUlUYWJDb250cm9sOw0KCWlmKHRhYkNvbnRyb2wgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQoJYm9vbCBJc0RvbmUoVUlDb250cm9sIGJsb2NrKQ0KCXsNCgkJdmFyIGNoZWNrID0gYmxvY2suR2V0Q29tcG9uZW50PFVJQ2hlY2s+KCJDaGVjayBEb25lIik7DQoJCXJldHVybiBjaGVjayAhPSBudWxsICYmIGNoZWNrLkNoZWNrZWQuVmFsdWUgPT0gVUlDaGVjay5DaGVja1ZhbHVlLkNoZWNrZWQ7IA0KCX0NCg0KCXZhciBwYWdlQmFzaWMgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBCYXNpYyIpIGFzIFVJQ29udHJvbDsNCglpZihwYWdlQmFzaWMgIT0gbnVsbCkNCgl7DQoJCXZhciBibG9jazEgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDIiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazMgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDMiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazQgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDUiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazYgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDYiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazcgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoNCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCWJsb2NrMy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2syKTsNCgkJYmxvY2s0LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazIpOw0KCQlibG9jazYuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMykgJiYgIUlzRG9uZShibG9jazUpOw0KCQlibG9jazcuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSkgfHwgIUlzRG9uZShibG9jazIpIHx8ICFJc0RvbmUoYmxvY2szKSB8fCAhSXNEb25lKGJsb2NrNCkgfHwgIUlzRG9uZShibG9jazUpIHx8ICFJc0RvbmUoYmxvY2s2KTsNCgkJDQoJCXZhciB0YWJCdXR0b25zID0gdGFiQ29udHJvbC5HZXRBbGxCdXR0b25zKCk7DQoJCXRhYkJ1dHRvbnNbMV0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJCXRhYkJ1dHRvbnNbMl0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJfQ0KDQoJdmFyIHBhZ2VTY3JpcHRpbmcgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBTY3JpcHRpbmciKSBhcyBVSUNvbnRyb2w7DQoJaWYocGFnZVNjcmlwdGluZyAhPSBudWxsKQ0KCXsNCgkJdmFyIGJsb2NrMSA9IHBhZ2VTY3JpcHRpbmcuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayAyIikgYXMgVUlDb250cm9sOw0KCQl2YXIgYmxvY2szID0gcGFnZVNjcmlwdGluZy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgMyIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrNCA9IHBhZ2VTY3JpcHRpbmcuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA1IikgYXMgVUlDb250cm9sOw0KCQl2YXIgYmxvY2s2ID0gcGFnZVNjcmlwdGluZy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgNiIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrNyA9IHBhZ2VTY3JpcHRpbmcuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoNCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCQ0KCX0NCg0KfQ0K")]
public class DynamicClass_8cf560de_767c_4803_ac87_c565eb8a2210
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
            block5.ReadOnly = !IsDone(block1);
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29udHJvbF9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQ0Kew0KCXZhciB0YWJDb250cm9sID0gc2VuZGVyLkNvbXBvbmVudHNbIlRhYiBDb250cm9sIl0gYXMgVUlUYWJDb250cm9sOw0KCWlmKHRhYkNvbnRyb2wgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQoJYm9vbCBJc0RvbmUoVUlDb250cm9sIGJsb2NrKQ0KCXsNCgkJdmFyIGNoZWNrID0gYmxvY2suR2V0Q29tcG9uZW50PFVJQ2hlY2s+KCJDaGVjayBEb25lIik7DQoJCXJldHVybiBjaGVjayAhPSBudWxsICYmIGNoZWNrLkNoZWNrZWQuVmFsdWUgPT0gVUlDaGVjay5DaGVja1ZhbHVlLkNoZWNrZWQ7IA0KCX0NCg0KCXZhciBwYWdlQmFzaWMgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBCYXNpYyIpIGFzIFVJQ29udHJvbDsNCglpZihwYWdlQmFzaWMgIT0gbnVsbCkNCgl7DQoJCXZhciBibG9jazEgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDIiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazMgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDMiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazQgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDUiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazYgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDYiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazcgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoNCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCWJsb2NrMy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2syKTsNCgkJYmxvY2s0LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazIpOw0KCQlibG9jazYuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMykgJiYgIUlzRG9uZShibG9jazUpOw0KCQlibG9jazcuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSkgfHwgIUlzRG9uZShibG9jazIpIHx8ICFJc0RvbmUoYmxvY2szKSB8fCAhSXNEb25lKGJsb2NrNCkgfHwgIUlzRG9uZShibG9jazUpIHx8ICFJc0RvbmUoYmxvY2s2KTsNCgkJDQoJCXZhciB0YWJCdXR0b25zID0gdGFiQ29udHJvbC5HZXRBbGxCdXR0b25zKCk7DQoJCXRhYkJ1dHRvbnNbMV0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJCXRhYkJ1dHRvbnNbMl0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJfQ0KDQoJdmFyIHBhZ2VTY3JpcHRpbmcgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBTY3JpcHRpbmciKSBhcyBVSUNvbnRyb2w7DQoJaWYocGFnZVNjcmlwdGluZyAhPSBudWxsKQ0KCXsNCgkJdmFyIGJsb2NrMSA9IHBhZ2VTY3JpcHRpbmcuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayAyIikgYXMgVUlDb250cm9sOw0KCQl2YXIgYmxvY2szID0gcGFnZVNjcmlwdGluZy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgMyIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrNCA9IHBhZ2VTY3JpcHRpbmcuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA1IikgYXMgVUlDb250cm9sOw0KCQl2YXIgYmxvY2s2ID0gcGFnZVNjcmlwdGluZy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgNiIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrNyA9IHBhZ2VTY3JpcHRpbmcuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoNCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazMuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCWJsb2NrNC5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2sxKTsNCgkJYmxvY2s1LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazYuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCWJsb2NrNy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2sxKTsJCQ0KCX0NCg0KfQ0K")]
public class DynamicClass_087cda3d_e988_40fd_b9ea_23f78328a1ac
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
            block3.ReadOnly = !IsDone(block1);
            block4.ReadOnly = !IsDone(block1);
            block5.ReadOnly = !IsDone(block1);
            block6.ReadOnly = !IsDone(block1);
            block7.ReadOnly = !IsDone(block1);
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29udHJvbF9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQ0Kew0KCXZhciB0YWJDb250cm9sID0gc2VuZGVyLkNvbXBvbmVudHNbIlRhYiBDb250cm9sIl0gYXMgVUlUYWJDb250cm9sOw0KCWlmKHRhYkNvbnRyb2wgPT0gbnVsbCkNCgkJcmV0dXJuOw0KDQoJYm9vbCBJc0RvbmUoVUlDb250cm9sIGJsb2NrKQ0KCXsNCgkJdmFyIGNoZWNrID0gYmxvY2suR2V0Q29tcG9uZW50PFVJQ2hlY2s+KCJDaGVjayBEb25lIik7DQoJCXJldHVybiBjaGVjayAhPSBudWxsICYmIGNoZWNrLkNoZWNrZWQuVmFsdWUgPT0gVUlDaGVjay5DaGVja1ZhbHVlLkNoZWNrZWQ7IA0KCX0NCg0KCXZhciBwYWdlQmFzaWMgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBCYXNpYyIpIGFzIFVJQ29udHJvbDsNCglpZihwYWdlQmFzaWMgIT0gbnVsbCkNCgl7DQoJCXZhciBibG9jazEgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDIiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazMgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDMiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazQgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDUiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazYgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDYiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazcgPSBwYWdlQmFzaWMuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoNCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazUuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCWJsb2NrMy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2syKTsNCgkJYmxvY2s0LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazIpOw0KCQlibG9jazYuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMykgJiYgIUlzRG9uZShibG9jazUpOw0KCQlibG9jazcuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSkgfHwgIUlzRG9uZShibG9jazIpIHx8ICFJc0RvbmUoYmxvY2szKSB8fCAhSXNEb25lKGJsb2NrNCkgfHwgIUlzRG9uZShibG9jazUpIHx8ICFJc0RvbmUoYmxvY2s2KTsNCgkJDQoJCXZhciB0YWJCdXR0b25zID0gdGFiQ29udHJvbC5HZXRBbGxCdXR0b25zKCk7DQoJCXRhYkJ1dHRvbnNbMV0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJCXRhYkJ1dHRvbnNbMl0uUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrNyk7DQoJfQ0KDQoJdmFyIHBhZ2VTY3JpcHRpbmcgPSB0YWJDb250cm9sLkdldENvbXBvbmVudCgiUGFnZSBTY3JpcHRpbmciKSBhcyBVSUNvbnRyb2w7DQoJaWYocGFnZVNjcmlwdGluZyAhPSBudWxsKQ0KCXsNCgkJdmFyIGJsb2NrMSA9IHBhZ2VTY3JpcHRpbmcuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDEiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazIgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayAyIikgYXMgVUlDb250cm9sOw0KCQl2YXIgYmxvY2szID0gcGFnZVNjcmlwdGluZy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgMyIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrNCA9IHBhZ2VTY3JpcHRpbmcuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDQiKSBhcyBVSUNvbnRyb2w7DQoJCXZhciBibG9jazUgPSBwYWdlU2NyaXB0aW5nLkdldENvbXBvbmVudCgiTGVhcm5pbmdCbG9jayA1IikgYXMgVUlDb250cm9sOw0KCQl2YXIgYmxvY2s2ID0gcGFnZVNjcmlwdGluZy5HZXRDb21wb25lbnQoIkxlYXJuaW5nQmxvY2sgNiIpIGFzIFVJQ29udHJvbDsNCgkJdmFyIGJsb2NrNyA9IHBhZ2VTY3JpcHRpbmcuR2V0Q29tcG9uZW50KCJMZWFybmluZ0Jsb2NrIDciKSBhcyBVSUNvbnRyb2w7DQoNCgkJYmxvY2syLlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazMuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMik7DQoJCWJsb2NrNC5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2sxKTsNCgkJYmxvY2s1LlJlYWRPbmx5ID0gIUlzRG9uZShibG9jazEpOw0KCQlibG9jazYuUmVhZE9ubHkgPSAhSXNEb25lKGJsb2NrMSk7DQoJCWJsb2NrNy5SZWFkT25seSA9ICFJc0RvbmUoYmxvY2sxKTsJCQ0KCX0NCg0KfQ0K")]
public class DynamicClass_395d2032_36c8_4e61_8464_d6b4b7182c3e
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

[CSharpScriptGeneratedAttribute("aW50IE1ldGhvZCggaW50IGEsIGludCBiICkNCnsNCglyZXR1cm4gYSArIGI7DQp9DQo=")]
public class DynamicClass_756b8960_21a1_47e9_94e7_deab66da2c8e
{
    public NeoAxis.Component_CSharpScript Owner;
    int Method(int a, int b)
    {
        return a + b;
    }
}

[CSharpScriptGeneratedAttribute("VHJhbnNmb3JtIE1ldGhvZCgpDQp7DQoJdmFyIGFuZ2xlID0gRW5naW5lQXBwLkVuZ2luZVRpbWUgKiAwLjM7DQoJdmFyIG9mZnNldCA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIDIuNTsNCgl2YXIgbG9va1RvID0gbmV3IFZlY3RvcjMoMTEuNzM3NDgzOTEyNDgyNywgLTAuMDUxNzc2NzUwMzI0MzksIC0xNS41MDkyNzU1ODI1MDkyKTsNCgl2YXIgbG9va0F0ID0gUXVhdGVybmlvbi5Mb29rQXQoLW9mZnNldCwgbmV3IFZlY3RvcjMoMCwwLDEpKTsNCgkNCglyZXR1cm4gbmV3IFRyYW5zZm9ybSggbG9va1RvICsgb2Zmc2V0LCBsb29rQXQsIFZlY3RvcjMuT25lICk7DQp9DQo=")]
public class DynamicClass_839b3c5f_a38f_4fb8_9e8e_4a703d29f841
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
}
#endif