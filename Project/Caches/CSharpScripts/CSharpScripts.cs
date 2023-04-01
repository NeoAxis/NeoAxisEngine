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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX0dlbmVyYXRlU3RhZ2UoTmVvQXhpcy5QbGFudFR5cGUgc2VuZGVyLCBOZW9BeGlzLlBsYW50R2VuZXJhdG9yIGdlbmVyYXRvciwgTmVvQXhpcy5QbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0gc3RhZ2UpCnsKI2lmICFERVBMT1kKCQoJLy9oZXJlIGlzIGEgc2NyaXB0IGZvciB0aGUgcGxhbnQgZ2VuZXJhdG9yIHRvIHNwZWNpYWxpemUgb3VyIHBsYW50IHR5cGUKCQoJc3dpdGNoKCBzdGFnZSApCgl7CgljYXNlIFBsYW50R2VuZXJhdG9yLkVsZW1lbnRUeXBlRW51bS5UcnVuazoKCQl7CgkJCXZhciBtYXRlcmlhbCA9IGdlbmVyYXRvci5GaW5kU3VpdGFibGVNYXRlcmlhbCggUGxhbnRNYXRlcmlhbC5QYXJ0VHlwZUVudW0uQmFyayApOwoJCQl2YXIgc3RhcnRUcmFuc2Zvcm0gPSBuZXcgVHJhbnNmb3JtKCBWZWN0b3IzLlplcm8sIFF1YXRlcm5pb24uTG9va0F0KCBWZWN0b3IzLlpBeGlzLCBWZWN0b3IzLlhBeGlzICkgKTsKCQkJdmFyIGxlbmd0aCA9IGdlbmVyYXRvci5IZWlnaHQgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjgsIDEuMiApOwoJCQl2YXIgdGhpY2tuZXNzID0gbGVuZ3RoIC8gNjAuMDsKCQoJCQl2YXIgdGhpY2tuZXNzRmFjdG9yID0gbmV3IEN1cnZlQ3ViaWNTcGxpbmUxRigpOwoJCQl0aGlja25lc3NGYWN0b3IuQWRkUG9pbnQoIG5ldyBDdXJ2ZTFGLlBvaW50KCAwLCAxICkgKTsKCQkJdGhpY2tuZXNzRmFjdG9yLkFkZFBvaW50KCBuZXcgQ3VydmUxRi5Qb2ludCggMSwgMC4zM2YgKSApOwoJCQkvL3RoaWNrbmVzc0ZhY3Rvci5BZGRQb2ludCggbmV3IEN1cnZlMUYuUG9pbnQoIDAuOTVmLCAwLjMzZiApICk7CgkJCS8vdGhpY2tuZXNzRmFjdG9yLkFkZFBvaW50KCBuZXcgQ3VydmUxRi5Qb2ludCggMSwgMCApICk7CgkKCQkJZ2VuZXJhdG9yLlRydW5rcy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50Q3lsaW5kZXIoIG51bGwsIG1hdGVyaWFsLCBzdGFydFRyYW5zZm9ybSwgbGVuZ3RoLCB0aGlja25lc3MsIHRoaWNrbmVzc0ZhY3RvciwgMTAsIDEzLCB0aGlja25lc3MgKiAwLjUsIDAgKSApOwoJCX0KCQlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uQnJhbmNoOgoJCXsKCQkJdmFyIGNvdW50ID0gNzsKCQkJaWYoIGdlbmVyYXRvci5BZ2UgPCBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSApCgkJCQljb3VudCA9IChpbnQpKCAoZG91YmxlKWNvdW50ICogTWF0aC5Qb3coIGdlbmVyYXRvci5BZ2UgLyBnZW5lcmF0b3IuUGxhbnRUeXBlLk1hdHVyZUFnZSwgMiApICk7CgkKCQkJdmFyIHBhcmVudCA9IGdlbmVyYXRvci5UcnVua3NbIDAgXTsKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CYXJrICk7CgkKCQkJdmFyIGFkZGVkID0gMDsKCQkJZm9yKCBpbnQgbiA9IDA7IG4gPCBjb3VudCAqIDEwOyBuKysgKQoJCQl7CgkJCQl2YXIgdGltZUZhY3Rvck9uUGFyZW50Q3VydmUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjIsIDAuNjUgKTsKCQkJCXZhciB2ZXJ0aWNhbEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMjAuMCwgNTAuMCApOwoJCQkJdmFyIHR3aXN0QW5nbGUgPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAtNDUuMCwgNDUuMCApOwoJCQkJdmFyIHN0YXJ0VHJhbnNmb3JtID0gcGFyZW50LkN1cnZlLkdldFRyYW5zZm9ybU9uU3VyZmFjZSggdGltZUZhY3Rvck9uUGFyZW50Q3VydmUsIGdlbmVyYXRvci5SYW5kb21pemVyLk5leHQoIDEuMCApLCB2ZXJ0aWNhbEFuZ2xlLCB0d2lzdEFuZ2xlICk7CgkKCQkJCXZhciB0aGlja25lc3MgPSBzdGFydFRyYW5zZm9ybS5wYXJlbnRUaGlja25lc3MgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjgsIDEuMCApOwoJCgkJCQl2YXIgbWluQnJhbmNoVHdpZ0xlbmd0aCA9IGdlbmVyYXRvci5IZWlnaHQgLyAxNTAuMDsKCQoJCQkJdmFyIGxlbmd0aCA9IHRoaWNrbmVzcyAqIDM1LjA7CgkJCQlpZiggbGVuZ3RoID49IG1pbkJyYW5jaFR3aWdMZW5ndGggKQoJCQkJewoJCQkJCXZhciB0aGlja25lc3NGYWN0b3IgPSBuZXcgQ3VydmVDdWJpY1NwbGluZTFGKCk7CgkJCQkJdGhpY2tuZXNzRmFjdG9yLkFkZFBvaW50KCBuZXcgQ3VydmUxRi5Qb2ludCggMCwgMSApICk7CgkJCQkJdGhpY2tuZXNzRmFjdG9yLkFkZFBvaW50KCBuZXcgQ3VydmUxRi5Qb2ludCggMSwgMC4zM2YgKSApOwoJCgkJCQkJZ2VuZXJhdG9yLkJyYW5jaGVzLkFkZCggZ2VuZXJhdG9yLkNyZWF0ZUVsZW1lbnRDeWxpbmRlciggcGFyZW50LCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0udHJhbnNmb3JtLCBsZW5ndGgsIHRoaWNrbmVzcywgdGhpY2tuZXNzRmFjdG9yLCAxMC4wLCAxMy4wLCB0aGlja25lc3MgKiAwLjUsIDMuMCApICk7CgkKCQkJCQlhZGRlZCsrOwoJCQkJCWlmKCBhZGRlZCA+PSBjb3VudCApCgkJCQkJCWJyZWFrOwoJCQkJfQoJCQl9CgkJfQoJCWJyZWFrOwoJCgkvL2Nhc2UgRWxlbWVudFR5cGVFbnVtLlR3aWc6CgkvLwlicmVhazsKCQoJY2FzZSBQbGFudEdlbmVyYXRvci5FbGVtZW50VHlwZUVudW0uRmxvd2VyOgoJCXsKCQkJZm9yKCBpbnQgbiA9IDA7IG4gPCBnZW5lcmF0b3IuVHJ1bmtzLkNvdW50ICsgZ2VuZXJhdG9yLkJyYW5jaGVzLkNvdW50OyBuKysgKQoJCQl7CgkJCQl2YXIgbWF0dXJpdHkgPSBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UuVmFsdWUgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjgsIDEuMiApOwoJCQkJaWYoIG1hdHVyaXR5ID4gMC4zMyApCgkJCQl7CgkJCQkJUGxhbnRHZW5lcmF0b3IuRWxlbWVudCBwYXJlbnQ7CgkJCQkJaWYoIG4gPCBnZW5lcmF0b3IuVHJ1bmtzLkNvdW50ICkKCQkJCQkJcGFyZW50ID0gZ2VuZXJhdG9yLlRydW5rc1sgbiBdOwoJCQkJCWVsc2UKCQkJCQkJcGFyZW50ID0gZ2VuZXJhdG9yLkJyYW5jaGVzWyBuIC0gZ2VuZXJhdG9yLlRydW5rcy5Db3VudCBdOwoJCgkJCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5GbG93ZXIgKTsKCQoJCQkJCS8vISEhIXR3aXN0IHJhbmRvbQoJCgkJCQkJdmFyIHRyYW5zZm9ybTEgPSBwYXJlbnQuQ3VydmUuR2V0VHJhbnNmb3JtQnlUaW1lRmFjdG9yKCAxICk7CgkKCQkJCQl2YXIgZGlyZWN0aW9uID0gKCB0cmFuc2Zvcm0xLlBvc2l0aW9uIC0gcGFyZW50LkN1cnZlLkdldFRyYW5zZm9ybUJ5VGltZUZhY3RvciggMC45OSApLlBvc2l0aW9uICkuR2V0Tm9ybWFsaXplKCk7CgkJCQkJdmFyIHJvdGF0aW9uID0gUXVhdGVybmlvbi5Gcm9tRGlyZWN0aW9uWkF4aXNVcCggZGlyZWN0aW9uICk7CgkKCQkJCQl2YXIgdHJhbnNmb3JtID0gbmV3IFRyYW5zZm9ybSggdHJhbnNmb3JtMS5Qb3NpdGlvbiwgcm90YXRpb24gKTsKCQoJCQkJCXZhciBsZW5ndGggPSBtYXRlcmlhbCAhPSBudWxsID8gbWF0ZXJpYWwuUmVhbExlbmd0aC5WYWx1ZSA6IGdlbmVyYXRvci5IZWlnaHQgLyAxNS4wOwoJCQkJCWxlbmd0aCAqPSBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjgsIDEuMiApOwoJCQkJCWlmKCBtYXR1cml0eSA8IDEgKQoJCQkJCQlsZW5ndGggKj0gbWF0dXJpdHk7CgkKCQkJCQl2YXIgd2lkdGggPSBsZW5ndGg7CgkKCQkJCQlnZW5lcmF0b3IuRmxvd2Vycy5BZGQoIGdlbmVyYXRvci5DcmVhdGVFbGVtZW50Qm93bCggcGFyZW50LCBtYXRlcmlhbCwgdHJhbnNmb3JtLCBsZW5ndGgsIHdpZHRoLCBtYXR1cml0eSApICk7CgkJCQl9CgkJCX0KCQl9CgkJYnJlYWs7CgkKCWNhc2UgUGxhbnRHZW5lcmF0b3IuRWxlbWVudFR5cGVFbnVtLkxlYWY6CgkJaWYoIGdlbmVyYXRvci5CcmFuY2hlcy5Db3VudCAhPSAwIHx8IGdlbmVyYXRvci5Ud2lncy5Db3VudCAhPSAwICkKCQl7CgkJCXZhciBzZWxlY3RvciA9IG5ldyBQbGFudEdlbmVyYXRvci5TZWxlY3RvckJ5UHJvYmFiaWxpdHkoIGdlbmVyYXRvciApOwoJCQlzZWxlY3Rvci5BZGRFbGVtZW50cyggZ2VuZXJhdG9yLkJyYW5jaGVzICk7CgkJCS8vc2VsZWN0b3IuQWRkRWxlbWVudHMoIFR3aWdzICk7CgkJCXNlbGVjdG9yLkFkZEVsZW1lbnRzKCBnZW5lcmF0b3IuVHJ1bmtzICk7CgkKCQkJLy8hISEh0YDQsNGB0L_RgNC10LTQtdC70Y_RgtGMINCyINC30LDQstC40YHQuNC80L7RgdGC0Lgg0L7RgiDQtNC70LjQvdGLCgkKCQkJLy8hISEh0YDQsNCy0L3QvtC80LXRgNC90L4g0YDQsNGB0L_RgNC10LTQtdC70Y_RgtGMLiDQsdGA0LDQvdGH0LgsINCy0LXRgtC60Lgg0YLQvtC20LUKCQoJCQkvLyEhISHQv9GA0LjQvNC10L3Rj9GC0YwgTGVhZkNvdW50CgkKCQkJdmFyIG1hdGVyaWFsID0gZ2VuZXJhdG9yLkZpbmRTdWl0YWJsZU1hdGVyaWFsKCBQbGFudE1hdGVyaWFsLlBhcnRUeXBlRW51bS5CcmFuY2hXaXRoTGVhdmVzICk7CgkKCQkJdmFyIGNvdW50ID0gNTA7CgkJCWlmKCBnZW5lcmF0b3IuQWdlIDwgZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UgKQoJCQkJY291bnQgPSAoaW50KSggKGRvdWJsZSljb3VudCAqIE1hdGguUG93KCBnZW5lcmF0b3IuQWdlIC8gZ2VuZXJhdG9yLlBsYW50VHlwZS5NYXR1cmVBZ2UsIDIgKSApOwoJCgkJCS8vaWYoIExPRCA+PSAyICkKCQkJLy8JY291bnQgLz0gMjsKCQkJLy9pZiggTE9EID49IDMgKQoJCQkvLwljb3VudCAvPSA2OwoJCgkJCWZvciggaW50IG4gPSAwOyBuIDwgY291bnQ7IG4rKyApCgkJCXsKCQkJCXZhciBwYXJlbnQgPSBzZWxlY3Rvci5HZXQoKTsKCQoJCQkJLy8hISEh0L_QvtCy0L7RgNCw0YfQuNCy0LDRgtGMINC_0L4g0LPQvtGA0LjQt9C+0L3RgtCw0LvQuD8KCQoJCQkJLy8hISEh0YDQsNGB0L_RgNC10LTQtdC70LXQvdC40LUKCQoJCQkJLy8hISEh0L7RgNC40LXQvdGC0LDRhtC40Y8g0L7RgtC90L7RgdC40YLQtdC70YzQvdC+INGB0L7Qu9C90YbQsC_QstC10YDRhdCwCgkKCQkJCXZhciB2ZXJ0aWNhbEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggLTMwLjAsIDMwLjAgKTsKCQkJCXZhciB0d2lzdEFuZ2xlID0gZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggLTkwLjAsIDkwLjAgKTsKCQoJCQkJdmFyIHN0YXJ0VHJhbnNmb3JtID0gcGFyZW50LkN1cnZlLkdldFRyYW5zZm9ybU9uU3VyZmFjZSggZ2VuZXJhdG9yLlJhbmRvbWl6ZXIuTmV4dCggMC4wMSwgMC42NSApLCBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAxLjAgKSwgdmVydGljYWxBbmdsZSwgdHdpc3RBbmdsZSApOwoJCgkJCQkvLyEhISF0aWx0QW5nbGUKCQoJCQkJdmFyIGxlbmd0aCA9IDAuMTsKCQkJCWlmKCBtYXRlcmlhbCAhPSBudWxsICkKCQkJCXsKCQkJCQl2YXIgbWF0dXJpdHkgPSBNYXRoLk1pbiggZ2VuZXJhdG9yLkFnZSAvIGdlbmVyYXRvci5QbGFudFR5cGUuTWF0dXJlQWdlLlZhbHVlLCAxLjAgKTsKCQkJCQlsZW5ndGggPSBtYXRlcmlhbC5SZWFsTGVuZ3RoICogbWF0dXJpdHkgKiBnZW5lcmF0b3IuUmFuZG9taXplci5OZXh0KCAwLjgsIDEuMiApOwoJCQkJfQoJCgkJCQkvLyEhISFjcm9zcz8KCQkJCWdlbmVyYXRvci5MZWF2ZXMuQWRkKCBnZW5lcmF0b3IuQ3JlYXRlRWxlbWVudFJpYmJvbiggcGFyZW50LCBtYXRlcmlhbCwgc3RhcnRUcmFuc2Zvcm0udHJhbnNmb3JtLCBsZW5ndGgsIDAsIGZhbHNlLCAwICkgKTsKCQkJfQoJCgkJCS8vISEhIQoJCQkvL9C_0YDQvtCy0LXRgNGP0YLRjCDQvNCw0YLQtdGA0LjQsNC7INC10YHRgtGMINC70Lgg0LLQtdGC0LrQsC4KCQkJLy_QtdGB0LvQuCDQvdC10YIg0YLQvtCz0LTQsCDQtNC10LvQsNGC0Ywg0LvQuNGB0YLRjNGPLiDQtdGB0YLRjCDQtdGB0YLRjCDRgtC+0LPQtNCwINCy0YHRjiDQstC10YLQutGDINGA0LjQsdCx0L7QvdC+0LwuCgkKCQl9CgkJYnJlYWs7Cgl9CgkKI2VuZGlmCn0K")]
public class DynamicClass468136F9295E4F7A680B2F65187F215F286B3BC42934CB88DB3C6A0F41712FAD
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgR2FtZU1vZGVfSW5wdXRNZXNzYWdlRXZlbnQoTmVvQXhpcy5HYW1lTW9kZSBzZW5kZXIsIE5lb0F4aXMuSW5wdXRNZXNzYWdlIG1lc3NhZ2UpCnsKCXZhciBrZXlEb3duID0gbWVzc2FnZSBhcyBJbnB1dE1lc3NhZ2VLZXlEb3duOwoJaWYgKGtleURvd24gIT0gbnVsbCkKCXsKCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDEpCgkJewoJCQl2YXIgbWFuYWdlciA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxCdWlsZGluZ01hbmFnZXI+KCk7CgkJCWlmIChtYW5hZ2VyICE9IG51bGwpCgkJCQltYW5hZ2VyLkRpc3BsYXkgPSAhbWFuYWdlci5EaXNwbGF5OwoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJCWlmIChrZXlEb3duLktleSA9PSBFS2V5cy5EMikKCQl7CgkJCXZhciBzeXN0ZW0gPSBzZW5kZXIuUGFyZW50Um9vdC5HZXRDb21wb25lbnQ8VHJhZmZpY1N5c3RlbT4oKTsKCQkJaWYgKHN5c3RlbSAhPSBudWxsKQoJCQkJc3lzdGVtLlBhcmtlZFZlaGljbGVzID0gc3lzdGVtLlBhcmtlZFZlaGljbGVzLlZhbHVlICE9IDAgPyAwIDogNTAwMDsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDMpCgkJewoJCQl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CgkJCWlmIChzeXN0ZW0gIT0gbnVsbCkKCQkJCXN5c3RlbS5GbHlpbmdWZWhpY2xlcyA9IHN5c3RlbS5GbHlpbmdWZWhpY2xlcy5WYWx1ZSAhPSAwID8gMCA6IDEwMDA7CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ0KQoJCXsKCQkJdmFyIHN5c3RlbSA9IHNlbmRlci5QYXJlbnRSb290LkdldENvbXBvbmVudDxUcmFmZmljU3lzdGVtPigpOwoJCQlpZiAoc3lzdGVtICE9IG51bGwpCgkJCQlzeXN0ZW0uU2ltdWxhdGVEeW5hbWljT2JqZWN0cyA9ICFzeXN0ZW0uU2ltdWxhdGVEeW5hbWljT2JqZWN0czsKCQkJbWVzc2FnZS5IYW5kbGVkID0gdHJ1ZTsKCQkJcmV0dXJuOwoJCX0KCQlpZiAoa2V5RG93bi5LZXkgPT0gRUtleXMuRDUpCgkJewoJCQl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CgkJCWlmIChzeXN0ZW0gIT0gbnVsbCkKCQkJewoJCQkJaWYgKHN5c3RlbS5QYXJrZWRWZWhpY2xlc09iamVjdE1vZGUuVmFsdWUgPT0gVHJhZmZpY1N5c3RlbS5PYmplY3RNb2RlRW51bS5WZWhpY2xlQ29tcG9uZW50KQoJCQkJCXN5c3RlbS5QYXJrZWRWZWhpY2xlc09iamVjdE1vZGUgPSBUcmFmZmljU3lzdGVtLk9iamVjdE1vZGVFbnVtLlN0YXRpY09iamVjdDsKCQkJCWVsc2UKCQkJCQlzeXN0ZW0uUGFya2VkVmVoaWNsZXNPYmplY3RNb2RlID0gVHJhZmZpY1N5c3RlbS5PYmplY3RNb2RlRW51bS5WZWhpY2xlQ29tcG9uZW50OwoJCQl9CgkJCW1lc3NhZ2UuSGFuZGxlZCA9IHRydWU7CgkJCXJldHVybjsKCQl9CgkJaWYgKGtleURvd24uS2V5ID09IEVLZXlzLkQ2KQoJCXsKCQkJdmFyIHNjZW5lID0gc2VuZGVyLlBhcmVudFJvb3QgYXMgU2NlbmU7CgkJCWlmIChzY2VuZSAhPSBudWxsKQoJCQl7CgkJCQlpZiAoc2NlbmUuT2N0cmVlVGhyZWFkaW5nTW9kZS5WYWx1ZSA9PSBPY3RyZWVDb250YWluZXIuVGhyZWFkaW5nTW9kZUVudW0uQmFja2dyb3VuZFRocmVhZCkKCQkJCQlzY2VuZS5PY3RyZWVUaHJlYWRpbmdNb2RlID0gT2N0cmVlQ29udGFpbmVyLlRocmVhZGluZ01vZGVFbnVtLlNpbmdsZVRocmVhZGVkOwoJCQkJZWxzZQoJCQkJCXNjZW5lLk9jdHJlZVRocmVhZGluZ01vZGUgPSBPY3RyZWVDb250YWluZXIuVGhyZWFkaW5nTW9kZUVudW0uQmFja2dyb3VuZFRocmVhZDsKCQkJfQoJCQltZXNzYWdlLkhhbmRsZWQgPSB0cnVlOwoJCQlyZXR1cm47CgkJfQoJfQp9Cg==")]
public class DynamicClassF891909AA2EB38E5AB51868F3AD70D02869FCA0BED16BB465F4182D52BD1B94C
{
    public NeoAxis.CSharpScript Owner;
    public void GameMode_InputMessageEvent(NeoAxis.GameMode sender, NeoAxis.InputMessage message)
    {
        var keyDown = message as InputMessageKeyDown;
        if (keyDown != null)
        {
            if (keyDown.Key == EKeys.D1)
            {
                var manager = sender.ParentRoot.GetComponent<BuildingManager>();
                if (manager != null)
                    manager.Display = !manager.Display;
                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D2)
            {
                var system = sender.ParentRoot.GetComponent<TrafficSystem>();
                if (system != null)
                    system.ParkedVehicles = system.ParkedVehicles.Value != 0 ? 0 : 5000;
                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D3)
            {
                var system = sender.ParentRoot.GetComponent<TrafficSystem>();
                if (system != null)
                    system.FlyingVehicles = system.FlyingVehicles.Value != 0 ? 0 : 1000;
                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D4)
            {
                var system = sender.ParentRoot.GetComponent<TrafficSystem>();
                if (system != null)
                    system.SimulateDynamicObjects = !system.SimulateDynamicObjects;
                message.Handled = true;
                return;
            }

            if (keyDown.Key == EKeys.D5)
            {
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

            if (keyDown.Key == EKeys.D6)
            {
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
            }
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgRGVtb01vZGVfU2hvd0tleXNFdmVudChOZW9BeGlzLkRlbW9Nb2RlIHNlbmRlciwgU3lzdGVtLkNvbGxlY3Rpb25zLkdlbmVyaWMuTGlzdDxzdHJpbmc+IGxpbmVzKQp7Cgl2YXIgc3lzdGVtID0gc2VuZGVyLlBhcmVudFJvb3QuR2V0Q29tcG9uZW50PFRyYWZmaWNTeXN0ZW0+KCk7CglpZiAoc3lzdGVtID09IG51bGwpCgkJcmV0dXJuOwoJdmFyIHNjZW5lID0gc3lzdGVtLlBhcmVudFJvb3QgYXMgU2NlbmU7CglpZiAoc2NlbmUgPT0gbnVsbCkKCQlyZXR1cm47CgoJdmFyIHBhcmtlZFZlaGljbGVzQXNTdGF0aWMgPSBzeXN0ZW0uUGFya2VkVmVoaWNsZXNPYmplY3RNb2RlLlZhbHVlID09IFRyYWZmaWNTeXN0ZW0uT2JqZWN0TW9kZUVudW0uU3RhdGljT2JqZWN0OwoJdmFyIHBhcmtlZFZlaGljbGVzQXNTdGF0aWNTdHJpbmcgPSBwYXJrZWRWZWhpY2xlc0FzU3RhdGljID8gIm9uIiA6ICJvZmYiOwoKCXZhciBtdWx0aXRocmVhZGVkU2NlbmVPY3RyZWUgPSBzY2VuZS5PY3RyZWVUaHJlYWRpbmdNb2RlLlZhbHVlID09IE9jdHJlZUNvbnRhaW5lci5UaHJlYWRpbmdNb2RlRW51bS5CYWNrZ3JvdW5kVGhyZWFkOwoJdmFyIG11bHRpdGhyZWFkZWRTY2VuZU9jdHJlZVN0cmluZyA9IG11bHRpdGhyZWFkZWRTY2VuZU9jdHJlZSA_ICJvbiIgOiAib2ZmIjsKCglsaW5lcy5BZGQoIiIpOwoJbGluZXMuQWRkKCIxIC0gc2hvdyBidWlsZGluZ3MiKTsKCWxpbmVzLkFkZCgiMiAtIHNob3cgcGFya2VkIHZlaGljbGVzIik7CglsaW5lcy5BZGQoIjMgLSBzaG93IGZseWluZyB2ZWhpY2xlcyIpOwoJbGluZXMuQWRkKCI0IC0gc2ltdWxhdGUgZmx5aW5nIHZlaGljbGVzIik7CglsaW5lcy5BZGQoIiIpOwoJbGluZXMuQWRkKCQiNSAtIHBhcmtlZCB2ZWhpY2xlcyBhcyBzdGF0aWMgb2JqZWN0cyAoe3BhcmtlZFZlaGljbGVzQXNTdGF0aWNTdHJpbmd9KSIpOwoJbGluZXMuQWRkKCQiNiAtIG11bHRpdGhyZWFkZWQgc2NlbmUgb2N0cmVlICh7bXVsdGl0aHJlYWRlZFNjZW5lT2N0cmVlU3RyaW5nfSkiKTsKfQo=")]
public class DynamicClass2F4FB5DD583562DC740579D2ACB516EE6B5D2C75E4F012D8F9AFF518A2386D05
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
        var parkedVehiclesAsStatic = system.ParkedVehiclesObjectMode.Value == TrafficSystem.ObjectModeEnum.StaticObject;
        var parkedVehiclesAsStaticString = parkedVehiclesAsStatic ? "on" : "off";
        var multithreadedSceneOctree = scene.OctreeThreadingMode.Value == OctreeContainer.ThreadingModeEnum.BackgroundThread;
        var multithreadedSceneOctreeString = multithreadedSceneOctree ? "on" : "off";
        lines.Add("");
        lines.Add("1 - show buildings");
        lines.Add("2 - show parked vehicles");
        lines.Add("3 - show flying vehicles");
        lines.Add("4 - simulate flying vehicles");
        lines.Add("");
        lines.Add($"5 - parked vehicles as static objects ({parkedVehiclesAsStaticString})");
        lines.Add($"6 - multithreaded scene octree ({multithreadedSceneOctreeString})");
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIF90aGlzID0gc2VuZGVyIGFzIEJ1dHRvbkluU3BhY2U7CglpZiAoX3RoaXMgIT0gbnVsbCkKCXsKCQl2YXIgaW5kaWNhdG9yID0gX3RoaXMuR2V0Q29tcG9uZW50KCJJbmRpY2F0b3IiKSBhcyBNZXNoSW5TcGFjZTsKCQlpZiAoaW5kaWNhdG9yICE9IG51bGwpCgkJCWluZGljYXRvci5Db2xvciA9IF90aGlzLkFjdGl2YXRlZCA_IG5ldyBDb2xvclZhbHVlKDAsIDEsIDApIDogbmV3IENvbG9yVmFsdWUoMC41LCAwLjUsIDAuNSk7CgoJCXZhciBidXR0b25PZmZzZXQgPSBfdGhpcy5Db21wb25lbnRzLkdldEJ5UGF0aCgiQnV0dG9uXFxBdHRhY2ggVHJhbnNmb3JtIE9mZnNldCIpIGFzIFRyYW5zZm9ybU9mZnNldDsKCQlpZiAoYnV0dG9uT2Zmc2V0ICE9IG51bGwpCgkJewoJCQl2YXIgb2Zmc2V0UHVzaGVkID0gMC4wMTsKCQkJdmFyIG9mZnNldERlZmF1bHQgPSAwLjA1OwoKCQkJdmFyIGNvZWYgPSAwLjA7CgkJCWlmIChfdGhpcy5DbGlja2luZyAmJiBfdGhpcy5DbGlja2luZ1RvdGFsVGltZSAhPSAwKQoJCQl7CgkJCQl2YXIgdGltZUZhY3RvciA9IE1hdGhFeC5TYXR1cmF0ZShfdGhpcy5DbGlja2luZ0N1cnJlbnRUaW1lIC8gX3RoaXMuQ2xpY2tpbmdUb3RhbFRpbWUpOwoKCQkJCWlmKHRpbWVGYWN0b3IgPCAwLjUpCgkJCQkJY29lZiA9IHRpbWVGYWN0b3IgKiAyOwoJCQkJZWxzZQoJCQkJCWNvZWYgPSAoMS4wZiAtIHRpbWVGYWN0b3IpICogMjsKCQkJfQoKCQkJdmFyIG9mZnNldCA9IE1hdGhFeC5MZXJwKG9mZnNldERlZmF1bHQsIG9mZnNldFB1c2hlZCwgY29lZik7CgkJCWJ1dHRvbk9mZnNldC5Qb3NpdGlvbk9mZnNldCA9IG5ldyBWZWN0b3IzKG9mZnNldCwgMCwgMCk7CgkJfQoJfQp9")]
public class DynamicClassF6852856494DD40287E6AE691C523DC3C2FD0678AAA101EA5B672D67933B19C1
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uSW5TcGFjZSBzZW5kZXIpCnsKCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsKCgkvLyBHZXQgb2JqZWN0IHR5cGUuCgl2YXIgcmVzb3VyY2VOYW1lID0gQCJTYW1wbGVzXFN0YXJ0ZXIgQ29udGVudFxNb2RlbHNcU2NpLWZpIEJveFxTY2ktZmkgQm94Lm9iamVjdGluc3BhY2UiOwoJdmFyIGJveFR5cGUgPSBNZXRhZGF0YU1hbmFnZXIuR2V0VHlwZShyZXNvdXJjZU5hbWUpOwoJaWYoYm94VHlwZSA9PSBudWxsKQoJewoJCUxvZy5XYXJuaW5nKCJPYmplY3QgdHlwZSBpcyBudWxsLiIpOwoJCXJldHVybjsKCX0KCS8vdmFyIGJveFR5cGUgPSBNZXRhZGF0YU1hbmFnZXIuR2V0VHlwZShyZXNvdXJjZU5hbWUpOwoJLy92YXIgcmVzb3VyY2VOYW1lID0gQCJTYW1wbGVzXFN0YXJ0ZXIgQ29udGVudFxTY2VuZSBvYmplY3RzXFNjaS1maSBCb3hcQm94IHR5cGUuc2NlbmUiOwoJLy92YXIgb2JqZWN0TmFtZUluc2lkZVJlc291cmNlID0gIiRCb3giOwoJLy92YXIgYm94VHlwZSA9IE1ldGFkYXRhTWFuYWdlci5HZXRUeXBlKHJlc291cmNlTmFtZSArICJ8IiArIG9iamVjdE5hbWVJbnNpZGVSZXNvdXJjZSk7CgkKCS8vIENyZWF0ZSB0aGUgb2JqZWN0IHdpdGhvdXQgZW5hYmxpbmcuCgl2YXIgYm94ID0gKE1lc2hJblNwYWNlKXNjZW5lLkNyZWF0ZUNvbXBvbmVudChib3hUeXBlLCBlbmFibGVkOiBmYWxzZSk7CgkvL3ZhciBvYmogPSBzY2VuZS5DcmVhdGVDb21wb25lbnQ8TWVzaEluU3BhY2U+KGVuYWJsZWQ6IGZhbHNlKTsKCgkvLyBTZXQgaW5pdGlhbCBwb3NpdGlvbi4KCXZhciByYW5kb20gPSBuZXcgRmFzdFJhbmRvbSgpOwoJYm94LlRyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0oCgkJbmV3IFZlY3RvcjMoMiArIHJhbmRvbS5OZXh0KDAuMCwgNC4wKSwgOCArIHJhbmRvbS5OZXh0KDAuMCwgNC4wKSwgMTAgKyByYW5kb20uTmV4dCgwLjAsIDQuMCkpLCAKCQluZXcgQW5nbGVzKHJhbmRvbS5OZXh0KDM2MC4wKSwgcmFuZG9tLk5leHQoMzYwLjApLCByYW5kb20uTmV4dCgzNjAuMCkpKTsKCS8vYm94LlRyYW5zZm9ybSA9IG5ldyBUcmFuc2Zvcm0obmV3IFZlY3RvcjMoMSwgMSwgMTApLCBRdWF0ZXJuaW9uLklkZW50aXR5KTsKCQoJLy8gRW5hYmxlIHRoZSBvYmplY3QgaW4gdGhlIHNjZW5lLgoJYm94LkVuYWJsZWQgPSB0cnVlOwoKCS8vdmFyIGxpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJEaXJlY3Rpb25hbCBMaWdodCIpIGFzIExpZ2h0OwoJLy9pZiAobGlnaHQgIT0gbnVsbCkKCS8vCWxpZ2h0LkVuYWJsZWQgPSBzZW5kZXIuQWN0aXZhdGVkOwp9Cg==")]
public class DynamicClass44C1B60A61C8C21BEB0FBDD014EC9EAC95B9251AFB420712F0EAAA7923759E8D
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.ButtonInSpace sender)
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

        //var boxType = MetadataManager.GetType(resourceName);
        //var resourceName = @"Samples\Starter Content\Scene objects\Sci-fi Box\Box type.scene";
        //var objectNameInsideResource = "$Box";
        //var boxType = MetadataManager.GetType(resourceName + "|" + objectNameInsideResource);
        // Create the object without enabling.
        var box = (MeshInSpace)scene.CreateComponent(boxType, enabled: false);
        //var obj = scene.CreateComponent<MeshInSpace>(enabled: false);
        // Set initial position.
        var random = new FastRandom();
        box.Transform = new Transform(new Vector3(2 + random.Next(0.0, 4.0), 8 + random.Next(0.0, 4.0), 10 + random.Next(0.0, 4.0)), new Angles(random.Next(360.0), random.Next(360.0), random.Next(360.0)));
        //box.Transform = new Transform(new Vector3(1, 1, 10), Quaternion.Identity);
        // Enable the object in the scene.
        box.Enabled = true;
    //var light = scene.GetComponent("Directional Light") as Light;
    //if (light != null)
    //	light.Enabled = sender.Activated;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIF90aGlzID0gc2VuZGVyIGFzIFJlZ3VsYXRvclN3aXRjaEluU3BhY2U7CglpZiAoX3RoaXMgIT0gbnVsbCkKCXsKCQl2YXIgaW5kaWNhdG9yTWluID0gX3RoaXMuR2V0Q29tcG9uZW50KCJJbmRpY2F0b3IgTWluIikgYXMgTWVzaEluU3BhY2U7CgkJaWYgKGluZGljYXRvck1pbiAhPSBudWxsKQoJCQlpbmRpY2F0b3JNaW4uQ29sb3IgPSBfdGhpcy5WYWx1ZS5WYWx1ZSA8PSBfdGhpcy5WYWx1ZVJhbmdlLlZhbHVlLk1pbmltdW0gPyBuZXcgQ29sb3JWYWx1ZSgxLCAwLCAwKSA6IG5ldyBDb2xvclZhbHVlKDAuNSwgMC41LCAwLjUpOwoKCQl2YXIgaW5kaWNhdG9yTWF4ID0gX3RoaXMuR2V0Q29tcG9uZW50KCJJbmRpY2F0b3IgTWF4IikgYXMgTWVzaEluU3BhY2U7CgkJaWYgKGluZGljYXRvck1heCAhPSBudWxsKQoJCQlpbmRpY2F0b3JNYXguQ29sb3IgPSBfdGhpcy5WYWx1ZS5WYWx1ZSA+PSBfdGhpcy5WYWx1ZVJhbmdlLlZhbHVlLk1heGltdW0gPyBuZXcgQ29sb3JWYWx1ZSgwLCAxLCAwKSA6IG5ldyBDb2xvclZhbHVlKDAuNSwgMC41LCAwLjUpOwoKCQl2YXIgYnV0dG9uID0gX3RoaXMuR2V0Q29tcG9uZW50KCJCdXR0b24iKTsKCQlpZiAoYnV0dG9uICE9IG51bGwpCgkJewoJCQl2YXIgb2Zmc2V0ID0gYnV0dG9uLkdldENvbXBvbmVudDxUcmFuc2Zvcm1PZmZzZXQ+KCk7CgkJCWlmIChvZmZzZXQgIT0gbnVsbCkKCQkJewoJCQkJdmFyIGFuZ2xlID0gX3RoaXMuR2V0VmFsdWVBbmdsZSgpIC0gOTA7CgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsKCQkJfQoJCX0KCgkJdmFyIG1hcmtlck1pbiA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIE1pbiIpOwoJCWlmIChtYXJrZXJNaW4gIT0gbnVsbCkKCQl7CgkJCXZhciBvZmZzZXQgPSBtYXJrZXJNaW4uR2V0Q29tcG9uZW50PFRyYW5zZm9ybU9mZnNldD4oKTsKCQkJaWYgKG9mZnNldCAhPSBudWxsKQoJCQl7CgkJCQl2YXIgYW5nbGUgPSBfdGhpcy5BbmdsZVJhbmdlLlZhbHVlLk1pbmltdW0gLSA5MDsKCQkJCXZhciBhbmdsZVIgPSBNYXRoRXguRGVncmVlVG9SYWRpYW4oYW5nbGUpOwoJCQkJb2Zmc2V0LlBvc2l0aW9uT2Zmc2V0ID0gbmV3IFZlY3RvcjMoMC4wMSwgTWF0aC5Db3MoYW5nbGVSKSAqIDAuMDQsIE1hdGguU2luKC1hbmdsZVIpICogMC4wNCk7CgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsKCQkJfQoJCX0KCgkJdmFyIG1hcmtlck1heCA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIE1heCIpOwoJCWlmIChtYXJrZXJNYXggIT0gbnVsbCkKCQl7CgkJCXZhciBvZmZzZXQgPSBtYXJrZXJNYXguR2V0Q29tcG9uZW50PFRyYW5zZm9ybU9mZnNldD4oKTsKCQkJaWYgKG9mZnNldCAhPSBudWxsKQoJCQl7CgkJCQl2YXIgYW5nbGUgPSBfdGhpcy5BbmdsZVJhbmdlLlZhbHVlLk1heGltdW0gLSA5MDsKCQkJCXZhciBhbmdsZVIgPSBNYXRoRXguRGVncmVlVG9SYWRpYW4oYW5nbGUpOwoJCQkJb2Zmc2V0LlBvc2l0aW9uT2Zmc2V0ID0gbmV3IFZlY3RvcjMoMC4wMSwgTWF0aC5Db3MoYW5nbGVSKSAqIDAuMDQsIE1hdGguU2luKC1hbmdsZVIpICogMC4wNCk7CgkJCQlvZmZzZXQuUm90YXRpb25PZmZzZXQgPSBuZXcgQW5nbGVzKGFuZ2xlLCAwLCAwKS5Ub1F1YXRlcm5pb24oKTsKCQkJfQoJCX0KCgkJdmFyIG1hcmtlckN1cnJlbnQgPSBfdGhpcy5HZXRDb21wb25lbnQoIk1hcmtlciBDdXJyZW50Iik7CgkJaWYgKG1hcmtlckN1cnJlbnQgIT0gbnVsbCkKCQl7CgkJCXZhciBvZmZzZXQgPSBtYXJrZXJDdXJyZW50LkdldENvbXBvbmVudDxUcmFuc2Zvcm1PZmZzZXQ+KCk7CgkJCWlmIChvZmZzZXQgIT0gbnVsbCkKCQkJewoJCQkJdmFyIGFuZ2xlID0gX3RoaXMuR2V0VmFsdWVBbmdsZSgpIC0gOTA7CgkJCQl2YXIgYW5nbGVSID0gTWF0aEV4LkRlZ3JlZVRvUmFkaWFuKGFuZ2xlKTsKCQkJCW9mZnNldC5Qb3NpdGlvbk9mZnNldCA9IG5ldyBWZWN0b3IzKDAuMDYsIE1hdGguQ29zKGFuZ2xlUikgKiAwLjA0LCBNYXRoLlNpbigtYW5nbGVSKSAqIDAuMDQpOwoJCQkJb2Zmc2V0LlJvdGF0aW9uT2Zmc2V0ID0gbmV3IEFuZ2xlcyhhbmdsZSwgMCwgMCkuVG9RdWF0ZXJuaW9uKCk7CgkJCX0KCQl9Cgl9Cn0=")]
public class DynamicClass8FA65C71FE3773D6CC4FBD8C0B1F016DDDA6C168297DFD792460E1F9059D000F
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLlJlZ3VsYXRvclN3aXRjaEluU3BhY2Ugb2JqKQp7Cgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7CgoJLy9jaGFuZ2UgdGhlIGNvbG9yIG9mIHRoZSBsaWdodAoJdmFyIGxpZ2h0ID0gc2NlbmUuR2V0Q29tcG9uZW50KCJEaXJlY3Rpb25hbCBMaWdodCIpIGFzIExpZ2h0OwoJaWYgKGxpZ2h0ICE9IG51bGwpCgkJbGlnaHQuQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLjAsIDEuMCwgMS4wIC0gb2JqLlZhbHVlKTsKfQo=")]
public class DynamicClassCEBF52EC362E864E571175F18ED87E1E6AD5024D09FA661D105B997991E0F5C6
{
    public NeoAxis.CSharpScript Owner;
    public void RegulatorSwitch_ValueChanged(NeoAxis.RegulatorSwitchInSpace obj)
    {
        var scene = obj.ParentScene;
        //change the color of the light
        var light = scene.GetComponent("Directional Light") as Light;
        if (light != null)
            light.Color = new ColorValue(1.0, 1.0, 1.0 - obj.Value);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uSW5TcGFjZSBzZW5kZXIpCnsKCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsKCgl2YXIgZ3JvdW5kID0gc2NlbmUuR2V0Q29tcG9uZW50KCJHcm91bmQiKSBhcyBNZXNoSW5TcGFjZTsKCWlmIChncm91bmQgIT0gbnVsbCkKCXsKCQlpZiAoIWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwuUmVmZXJlbmNlU3BlY2lmaWVkKQoJCXsKCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IFJlZmVyZW5jZVV0aWxpdHkuTWFrZVJlZmVyZW5jZSgKCQkJCUAiQ29udGVudFxNYXRlcmlhbHNcQmFzaWMgTGlicmFyeVxDb25jcmV0ZVxDb25jcmV0ZSBGbG9vciAwMS5tYXRlcmlhbCIpOwoJCX0KCQllbHNlCgkJCWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwgPSBudWxsOwoJfQp9Cg==")]
public class DynamicClass6FE073B43FFD3D061F3EFAF849A9B642EC479C63206579FD4C47D62D3694F034
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
public class DynamicClassECBC0D990E63A9EB29E57236D310C6B20F46D08EFFCC7E7095A8AD752390FB11
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIF90aGlzID0gc2VuZGVyIGFzIFJlZ3VsYXRvclN3aXRjaEluU3BhY2U7CglpZiAoX3RoaXMgIT0gbnVsbCkKCXsKCQl2YXIgbWFya2VyQ3VycmVudCA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIEN1cnJlbnQiKTsKCQlpZiAobWFya2VyQ3VycmVudCAhPSBudWxsKQoJCXsKCQkJdmFyIG9mZnNldCA9IG1hcmtlckN1cnJlbnQuR2V0Q29tcG9uZW50PFRyYW5zZm9ybU9mZnNldD4oKTsKCQkJaWYgKG9mZnNldCAhPSBudWxsKQoJCQl7CgkJCQl2YXIgYW5nbGUgPSBfdGhpcy5HZXRWYWx1ZUFuZ2xlKCkgLSA5MDsKCQkJCXZhciBhbmdsZVIgPSBNYXRoRXguRGVncmVlVG9SYWRpYW4oYW5nbGUpOwoJCQkJb2Zmc2V0LlJvdGF0aW9uT2Zmc2V0ID0gbmV3IEFuZ2xlcyhhbmdsZSwgMCwgMCkuVG9RdWF0ZXJuaW9uKCk7CgkJCX0KCQl9Cgl9Cn0=")]
public class DynamicClassA0DAD80D84A34964365BDE10C3DBBFB7474D8F1CEB91DF49C56700C8B38046A1
{
    public NeoAxis.CSharpScript Owner;
    public void InteractiveObjectButton_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var _this = sender as RegulatorSwitchInSpace;
        if (_this != null)
        {
            var markerCurrent = _this.GetComponent("Marker Current");
            if (markerCurrent != null)
            {
                var offset = markerCurrent.GetComponent<TransformOffset>();
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgSW50ZXJhY3RpdmVPYmplY3RCdXR0b25fVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIF90aGlzID0gc2VuZGVyIGFzIFJlZ3VsYXRvclN3aXRjaEluU3BhY2U7CglpZiAoX3RoaXMgIT0gbnVsbCkKCXsKCQl2YXIgbWFya2VyQ3VycmVudCA9IF90aGlzLkdldENvbXBvbmVudCgiTWFya2VyIEN1cnJlbnQiKTsKCQlpZiAobWFya2VyQ3VycmVudCAhPSBudWxsKQoJCXsKCQkJdmFyIG9mZnNldCA9IG1hcmtlckN1cnJlbnQuR2V0Q29tcG9uZW50PFRyYW5zZm9ybU9mZnNldD4oKTsKCQkJaWYgKG9mZnNldCAhPSBudWxsKQoJCQl7CgkJCQl2YXIgYW5nbGUgPSBfdGhpcy5HZXRWYWx1ZUFuZ2xlKCkgLSAxMzU7CgkJCQl2YXIgYW5nbGVSID0gTWF0aEV4LkRlZ3JlZVRvUmFkaWFuKGFuZ2xlKTsKCQkJCW9mZnNldC5Sb3RhdGlvbk9mZnNldCA9IG5ldyBBbmdsZXMoYW5nbGUsIDAsIDApLlRvUXVhdGVybmlvbigpOwoJCQl9CgkJfQoJfQp9")]
public class DynamicClass1AA07D5A02C83DA4E661788BCFCFF34606BB4761A7450E376AE84DFF32863084
{
    public NeoAxis.CSharpScript Owner;
    public void InteractiveObjectButton_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var _this = sender as RegulatorSwitchInSpace;
        if (_this != null)
        {
            var markerCurrent = _this.GetComponent("Marker Current");
            if (markerCurrent != null)
            {
                var offset = markerCurrent.GetComponent<TransformOffset>();
                if (offset != null)
                {
                    var angle = _this.GetValueAngle() - 135;
                    var angleR = MathEx.DegreeToRadian(angle);
                    offset.RotationOffset = new Angles(angle, 0, 0).ToQuaternion();
                }
            }
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uSW5TcGFjZSBzZW5kZXIpCnsKCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsKCgl2YXIgbGlnaHQgPSBzY2VuZS5HZXRDb21wb25lbnQoIkFtYmllbnQgTGlnaHQiKSBhcyBMaWdodDsKCWlmIChsaWdodCAhPSBudWxsKQoJCWxpZ2h0LkVuYWJsZWQgPSBzZW5kZXIuQWN0aXZhdGVkOwkKfQo=")]
public class DynamicClass2EE3F7E0E9FE11777EEBA83FC45D1AC6138EF2A14E2702A44C993A478D968988
{
    public NeoAxis.CSharpScript Owner;
    public void Button_Click(NeoAxis.ButtonInSpace sender)
    {
        var scene = sender.ParentScene;
        var light = scene.GetComponent("Ambient Light") as Light;
        if (light != null)
            light.Enabled = sender.Activated;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUmVndWxhdG9yU3dpdGNoX1ZhbHVlQ2hhbmdlZChOZW9BeGlzLlJlZ3VsYXRvclN3aXRjaEluU3BhY2Ugb2JqKQp7Cgl2YXIgc2NlbmUgPSBvYmouUGFyZW50U2NlbmU7CgoJdmFyIG1lc2hJblNwYWNlID0gc2NlbmUuR2V0Q29tcG9uZW50KCJHcm91bmQiKSBhcyBNZXNoSW5TcGFjZTsKCWlmIChtZXNoSW5TcGFjZSAhPSBudWxsKQoJCW1lc2hJblNwYWNlLkNvbG9yID0gbmV3IENvbG9yVmFsdWUoMS4wIC0gb2JqLlZhbHVlLCAxLjAsIDEuMCAtIG9iai5WYWx1ZSk7Cn0K")]
public class DynamicClassACBB828C9C939D53CB836D512DB0E8CE1ECC7A45B87B5AA18782E0D80B58164C
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

[CSharpScriptGeneratedAttribute("cHVibGljIGRvdWJsZSBMYXN0U3BlZWRpbmdVcDsKcHVibGljIGRvdWJsZSBMYXN0VHVybmluZzsKCnB1YmxpYyB2b2lkIElucHV0UHJvY2Vzc2luZ19TaW11bGF0aW9uU3RlcChOZW9BeGlzLkNvbXBvbmVudCBvYmopCnsKCXZhciBzZW5kZXIgPSAoTmVvQXhpcy5JbnB1dFByb2Nlc3Npbmcpb2JqOwoKCUxhc3RTcGVlZGluZ1VwID0gMDsKCUxhc3RUdXJuaW5nID0gMDsKCgkvL2dldCBhY2Nlc3MgdG8gdGhlIHNoaXAKCXZhciBzaGlwID0gc2VuZGVyLlBhcmVudDsKCWlmIChzaGlwID09IG51bGwpCgkJcmV0dXJuOwoKCS8vY29udHJvbCB0aGUgc2hpcAoJdmFyIGJvZHkgPSBzaGlwLkdldENvbXBvbmVudDxSaWdpZEJvZHkyRD4oKTsKCWlmIChib2R5ICE9IG51bGwpCgl7CgkJLy9rZXlib2FyZAoKCQkvL2ZseSBmb3J3YXJkCgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuVykgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5VcCkgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5OdW1QYWQ4KSkKCQl7CgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOwoJCQlib2R5LkFwcGx5Rm9yY2UoZGlyICogMS4wKTsJCQoJCQlMYXN0U3BlZWRpbmdVcCArPSAxLjA7CgkJfQoKCQkvL2ZseSBiYWNrCgkJaWYgKHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuUykgfHwgc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5Eb3duKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLk51bVBhZDIpKQoJCXsKCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7CgkJCWJvZHkuQXBwbHlGb3JjZShkaXIgKiAtMS4wKTsJCQkKCQkJTGFzdFNwZWVkaW5nVXAgLT0gMS4wOwoJCX0KCgkJLy90dXJuIGxlZnQKCQlpZiAoc2VuZGVyLklzS2V5UHJlc3NlZChFS2V5cy5BKSB8fCBzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkxlZnQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkNCkpCgkJewoJCQlib2R5LkFwcGx5VG9ycXVlKDEuMCk7CgkJCUxhc3RUdXJuaW5nICs9IDEuMDsKCQl9CgoJCS8vdHVybiByaWdodAoJCWlmIChzZW5kZXIuSXNLZXlQcmVzc2VkKEVLZXlzLkQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuUmlnaHQpIHx8IHNlbmRlci5Jc0tleVByZXNzZWQoRUtleXMuTnVtUGFkNikpCgkJewoJCQlib2R5LkFwcGx5VG9ycXVlKC0xLjApOwoJCQlMYXN0VHVybmluZyAtPSAxLjA7CgkJfQoKCQkvL21vdmVtZW50IGJ5IGpveXN0aWNrIGF4ZXMKCQlpZiAoTWF0aC5BYnMoc2VuZGVyLkpveXN0aWNrQXhlc1swXSkgPj0gMC4wMSkKCQl7CgkJCWJvZHkuQXBwbHlUb3JxdWUoLXNlbmRlci5Kb3lzdGlja0F4ZXNbMF0pOwoJCQlMYXN0VHVybmluZyAtPSBzZW5kZXIuSm95c3RpY2tBeGVzWzBdOwoJCX0KCQlpZiAoTWF0aC5BYnMoc2VuZGVyLkpveXN0aWNrQXhlc1sxXSkgPj0gMC4wMSkKCQl7CgkJCXZhciBkaXIgPSBib2R5LlRyYW5zZm9ybVYuUm90YXRpb24uR2V0Rm9yd2FyZCgpLlRvVmVjdG9yMigpOwoJCQlib2R5LkFwcGx5Rm9yY2UoZGlyICogc2VuZGVyLkpveXN0aWNrQXhlc1sxXSk7CgkJCUxhc3RTcGVlZGluZ1VwICs9IHNlbmRlci5Kb3lzdGlja0F4ZXNbMV07CgkJfQoJCS8vSm95c3RpY2tBeGVzCgkJLy9Kb3lzdGlja0J1dHRvbnMKCQkvL0pveXN0aWNrUE9WcwoJCS8vSm95c3RpY2tTbGlkZXJzCgkJLy9Jc0pveXN0aWNrQnV0dG9uUHJlc3NlZAoJCS8vR2V0Sm95c3RpY2tBeGlzCgkJLy9HZXRKb3lzdGlja1BPVgoJCS8vR2V0Sm95c3RpY2tTbGlkZXIKCgoJCS8vbXVsdGktdG91Y2gKCgkJLy9kZWJ1ZyB0byBjb250cm9sIGJ5IG1vdXNlCgkJLy9WZWN0b3IyW10gdG91Y2hQb3NpdGlvbnMgPSBuZXcgVmVjdG9yMlswXTsJCQoJCS8vaWYoc2VuZGVyLklzTW91c2VCdXR0b25QcmVzc2VkKEVNb3VzZUJ1dHRvbnMuTGVmdCkpCgkJLy8JdG91Y2hQb3NpdGlvbnMgPSBuZXcgVmVjdG9yMltdIHsgc2VuZGVyLk1vdXNlUG9zaXRpb24gfTsKCQkKCQlmb3JlYWNoKHZhciBkYXRhIGluIHNlbmRlci5Ub3VjaFBvaW50ZXJzKQoJCXsKCQkJdmFyIHRvdWNoUG9zaXRpb24gPSBkYXRhLlBvc2l0aW9uOyAKCgkJCWlmKHRvdWNoUG9zaXRpb24uWCA8IDAuNSAmJiB0b3VjaFBvc2l0aW9uLlkgPiAwLjQpCgkJCXsKCQkJCS8vZmx5IGZvcndhcmQsIGJhY2sKCQkJCXsKCQkJCQl2YXIgZmFjdG9yID0gMS4wIC0gKHRvdWNoUG9zaXRpb24uWSAtIDAuNikgLyAwLjQ7CgkJCQkJdmFyIGZvcmNlID0gZmFjdG9yICogMi4wIC0gMS4wOwoJCQkJCWZvcmNlICo9IDEuMjsKCQkJCQlmb3JjZSA9IE1hdGhFeC5DbGFtcChmb3JjZSwgLTEuMCwgMS4wKTsKCgkJCQkJdmFyIGRpciA9IGJvZHkuVHJhbnNmb3JtVi5Sb3RhdGlvbi5HZXRGb3J3YXJkKCkuVG9WZWN0b3IyKCk7CgkJCQkJYm9keS5BcHBseUZvcmNlKGRpciAqIGZvcmNlKTsKCgkJCQkJTGFzdFNwZWVkaW5nVXAgKz0gZm9yY2U7CgkJCQl9CgkJCQkKCQkJCS8vdHVybiBsZWZ0LCByaWdodAoJCQkJewoJCQkJCXZhciBmYWN0b3IgPSAxLjAgLSBNYXRoRXguQ2xhbXAodG91Y2hQb3NpdGlvbi5YIC8gMC4yLCAwLCAxKTsKCQkJCQl2YXIgZm9yY2UgPSBmYWN0b3IgKiAyLjAgLSAxLjA7CQkJCQkKCQkJCQlmb3JjZSAqPSAxLjI7CgkJCQkJZm9yY2UgPSBNYXRoRXguQ2xhbXAoZm9yY2UsIC0xLjAsIDEuMCk7CgkJCQkJCgkJCQkJYm9keS5BcHBseVRvcnF1ZShmb3JjZSk7CgkKCQkJCQlMYXN0VHVybmluZyArPSBmb3JjZTsKCQkJCX0KCQkJfQoJCX0KCgl9CgkKfQoKcHVibGljIHZvaWQgSW5wdXRQcm9jZXNzaW5nX0lucHV0TWVzc2FnZUV2ZW50KE5lb0F4aXMuSW5wdXRQcm9jZXNzaW5nIHNlbmRlciwgTmVvQXhpcy5HYW1lTW9kZSBnYW1lTW9kZSwgTmVvQXhpcy5JbnB1dE1lc3NhZ2UgbWVzc2FnZSkKewoJLy8vL2dldCBhY2Nlc3MgdG8gdGhlIHNoaXAKCS8vdmFyIHNoaXAgPSBzZW5kZXIuUGFyZW50OwoJLy9pZiAoc2hpcCA9PSBudWxsKQoJLy8JcmV0dXJuOwoKCS8vdmFyIGtleURvd24gPSBtZXNzYWdlIGFzIElucHV0TWVzc2FnZUtleURvd247CgkvL2lmKGtleURvd24gIT0gbnVsbCkKCS8vewoJLy8JaWYoa2V5RG93bi5LZXkgPT0gRUtleXMuU3BhY2UpCgkvLwl7CgkvLwkJLy92YXIgYm9keSA9IHNoaXAuR2V0Q29tcG9uZW50PFJpZ2lkQm9keTJEPigpOwoJLy8JCS8vaWYgKGJvZHkgIT0gbnVsbCkKCS8vCQkvL3sKCS8vCQkvLwlib2R5LkFwcGx5Rm9yY2UobmV3IFZlY3RvcjIoMSwgMCkpOwoJLy8JCS8vfQoJLy8JfQoJLy99Cn0K")]
public class DynamicClassEE62FE9974413A4FAA4914B2746D38836D328EAD52495D094ADF6F29C4C232E2
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

[CSharpScriptGeneratedAttribute("UmVuZGVyaW5nUGlwZWxpbmUgR2V0UGlwZWxpbmUoKQp7CglzdHJpbmcgbmFtZTsKCWlmKEVuZ2luZUFwcC5FbmdpbmVUaW1lICUgNiA+IDMpCgkJbmFtZSA9ICJSZW5kZXJpbmcgUGlwZWxpbmUiOwoJZWxzZQoJCW5hbWUgPSAiUmVuZGVyaW5nIFBpcGVsaW5lIDIiOwoJCQoJcmV0dXJuIE93bmVyLlBhcmVudC5HZXRDb21wb25lbnQobmFtZSkgYXMgUmVuZGVyaW5nUGlwZWxpbmU7Cn0K")]
public class DynamicClassE9187D41516A838882B97D8B60F698D11F337B7737278CBFF9DB427D2BB97E84
{
    public NeoAxis.CSharpScript Owner;
    RenderingPipeline GetPipeline()
    {
        string name;
        if (EngineApp.EngineTime % 6 > 3)
            name = "Rendering Pipeline";
        else
            name = "Rendering Pipeline 2";
        return Owner.Parent.GetComponent(name) as RenderingPipeline;
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQnV0dG9uX0NsaWNrKE5lb0F4aXMuQnV0dG9uSW5TcGFjZSBzZW5kZXIpCnsKCXZhciBzY2VuZSA9IHNlbmRlci5QYXJlbnRTY2VuZTsKCgl2YXIgZ3JvdW5kID0gc2NlbmUuR2V0Q29tcG9uZW50KCJHcm91bmQiKSBhcyBNZXNoSW5TcGFjZTsKCWlmIChncm91bmQgIT0gbnVsbCkKCXsKCQlpZiAoIWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwuUmVmZXJlbmNlU3BlY2lmaWVkKQoJCXsKCQkJZ3JvdW5kLlJlcGxhY2VNYXRlcmlhbCA9IFJlZmVyZW5jZVV0aWxpdHkuTWFrZVJlZmVyZW5jZSggQCJCYXNlXE1hdGVyaWFsc1xEYXJrIFllbGxvdy5tYXRlcmlhbCIpOwoJCX0KCQllbHNlCgkJCWdyb3VuZC5SZXBsYWNlTWF0ZXJpYWwgPSBudWxsOwoJfQp9Cg==")]
public class DynamicClass4EEC20F75E94844B3480484FB8A7CC10F386637DC0B6869508D30793F401D012
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

[CSharpScriptGeneratedAttribute("aW50IE1ldGhvZCggaW50IGEsIGludCBiICkKewoJcmV0dXJuIGEgKyBiOwp9Cg==")]
public class DynamicClassEFE66A74484991C50F6D2BF75AD19B08A7F2F3AB36497CEFF9B0405E15C4EB2C
{
    public NeoAxis.CSharpScript Owner;
    int Method(int a, int b)
    {
        return a + b;
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgTWVzaEluU3BhY2VBbmltYXRpb25Db250cm9sbGVyX0NhbGN1bGF0ZUJvbmVUcmFuc2Zvcm1zKE5lb0F4aXMuTWVzaEluU3BhY2VBbmltYXRpb25Db250cm9sbGVyIHNlbmRlciwgTmVvQXhpcy5Ta2VsZXRvbkFuaW1hdGlvblRyYWNrLkNhbGN1bGF0ZUJvbmVUcmFuc2Zvcm1zSXRlbVtdIHJlc3VsdCkKewoJLy90byBlbmFibGUgdGhpcyBldmVudCBoYW5kbGVyIGluIHRoZSBlZGl0b3IgY2hhbmdlICJXaGVuIEVuYWJsZSIgcHJvcGVydHkgdG8gIlNpbXVsYXRpb24gfCBJbnN0YW5jZSB8IEVkaXRvciIuCgkvL2NvbXBvbmVudDogQ2hhcmFjdGVyL01lc2ggSW4gU3BhY2UvQyMgU2NyaXB0L0V2ZW50IEhhbmRsZXIgQ2FsY3VsYXRlQm9uZVRyYW5zZm9ybXMuCgkKCXZhciBib25lSW5kZXggPSBzZW5kZXIuR2V0Qm9uZUluZGV4KCJtaXhhbW9yaWc6U3BpbmUxIik7CglpZihib25lSW5kZXggIT0gLTEpCgl7CgkJcmVmIHZhciBpdGVtID0gcmVmIHJlc3VsdFtib25lSW5kZXhdOwoKCQkvL2NhbGN1bGF0ZSBib25lIG9mZnNldAoJCXZhciBhbmdsZSA9IG5ldyBEZWdyZWUoNjApICogTWF0aC5TaW4oVGltZS5DdXJyZW50KTsgCgkJdmFyIG9mZnNldCA9IE1hdHJpeDNGLkZyb21Sb3RhdGVCeVkoKGZsb2F0KWFuZ2xlLkluUmFkaWFucygpKS5Ub1F1YXRlcm5pb24oKTsKCQkKCQkvL3VwZGF0ZSB0aGUgYm9uZQoJCWl0ZW0uUm90YXRpb24gKj0gb2Zmc2V0OwoJfQkKfQo=")]
public class DynamicClassD11D0BDF2CE301BB2E8F6F60961E3230562912D7DE50E1854F960982DD980C0E
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgU0ZDcmF0ZTFfVXBkYXRlRXZlbnQoTmVvQXhpcy5Db21wb25lbnQgc2VuZGVyLCBmbG9hdCBkZWx0YSkKewoJdmFyIG1lc2hJblNwYWNlID0gKE1lc2hJblNwYWNlKXNlbmRlcjsKCQoJLy91c2UgbWVzaCBvZiB0aGUgY29tcG9uZW50Cgl2YXIgbWVzaCA9IG1lc2hJblNwYWNlLk1lc2guVmFsdWU7CgkKCXZhciBkaXN0YW5jZVN0ZXBzID0gNTsKCXZhciBhbmdsZVN0ZXBzID0gODsKCgl2YXIgYWRkaXRpb25hbEl0ZW1zID0gbmV3IE1lc2hJblNwYWNlLkFkZGl0aW9uYWxJdGVtW2Rpc3RhbmNlU3RlcHMgKiBhbmdsZVN0ZXBzXTsKCglpbnQgY3VycmVudEluZGV4ID0gMDsKCglmb3IgKGludCBkaXN0YW5jZVN0ZXAgPSAwOyBkaXN0YW5jZVN0ZXAgPCBkaXN0YW5jZVN0ZXBzOyBkaXN0YW5jZVN0ZXArKykKCXsKCQl2YXIgZGlzdGFuY2UgPSAoZG91YmxlKWRpc3RhbmNlU3RlcCAqIDEuMGY7CgoJCWZvciAoaW50IGFuZ2xlU3RlcCA9IDA7IGFuZ2xlU3RlcCA8IGFuZ2xlU3RlcHM7IGFuZ2xlU3RlcCsrKQoJCXsKCQkJdmFyIGFuZ2xlID0gTWF0aC5QSSAqIDIgKiBhbmdsZVN0ZXAgLyBhbmdsZVN0ZXBzICsgVGltZS5DdXJyZW50ICogMC41OwoKCQkJdmFyIHBvcyA9IG5ldyBWZWN0b3IzKE1hdGguQ29zKGFuZ2xlKSwgTWF0aC5TaW4oYW5nbGUpLCAwKSAqIGRpc3RhbmNlOwoJCQl2YXIgcm90ID0gUXVhdGVybmlvbi5JZGVudGl0eTsKCQkJdmFyIHNjbCA9IG5ldyBWZWN0b3IzKDAuMyAvIGRpc3RhbmNlLCAwLjMgLyBkaXN0YW5jZSwgMC4zIC8gZGlzdGFuY2UpOwoKCQkJYWRkaXRpb25hbEl0ZW1zW2N1cnJlbnRJbmRleF0gPSBuZXcgTWVzaEluU3BhY2UuQWRkaXRpb25hbEl0ZW0obWVzaCwgcG9zLCByb3QsIHNjbCwgbmV3IENvbG9yVmFsdWUoMSwgMSwgMCkpOwoJCQljdXJyZW50SW5kZXgrKzsKCQl9Cgl9CgoJLyoKCXZhciBhZGRpdGlvbmFsSXRlbXMgPSBuZXcgTWVzaEluU3BhY2UuQWRkaXRpb25hbEl0ZW1bMV07CglyZWYgdmFyIGl0ZW0gPSByZWYgYWRkaXRpb25hbEl0ZW1zWzBdOwoJaXRlbS5NZXNoID0gbWVzaDsKCWl0ZW0uUG9zaXRpb24gPSBuZXcgVmVjdG9yMygxLCAwLCAwKTsKCWl0ZW0uUm90YXRpb24gPSBRdWF0ZXJuaW9uLklkZW50aXR5OzsKCWl0ZW0uU2NhbGUgPSBuZXcgVmVjdG9yMyguMiwgLjIsIC4yKTsKCWl0ZW0uQ29sb3IgPSBuZXcgQ29sb3JWYWx1ZSgxLCAxLCAwKTsKCSovCgoJbWVzaEluU3BhY2UuQWRkaXRpb25hbEl0ZW1zID0gYWRkaXRpb25hbEl0ZW1zOwoKCgkvL3VwZGF0ZSBib3VuZHMgd2hlbiBuZWVkIHVwZGF0ZQoJewoJCXZhciB0ciA9IG1lc2hJblNwYWNlLlRyYW5zZm9ybVY7CgkJdmFyIGJvdW5kcyA9IG5ldyBCb3VuZHMoLTQsIC00LCAtMC41LCA0LCA0LCAwLjUpOwoJCXZhciBib3ggPSBuZXcgQm94KGJvdW5kcywgdHIuUG9zaXRpb24sIHRyLlJvdGF0aW9uLlRvTWF0cml4MygpKTsKCQltZXNoSW5TcGFjZS5TcGFjZUJvdW5kc092ZXJyaWRlID0gbmV3IFNwYWNlQm91bmRzKGJveC5Ub0JvdW5kcygpKTsKCX0KfQo=")]
public class DynamicClassB8560AA3AB17CD7ADD5FDB0E801D1578B96DA0EBC70DADD621E5A293D4809263
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
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUGFpbnRMYXllcl9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQp7Cgl2YXIgbGF5ZXIgPSAoUGFpbnRMYXllcilzZW5kZXI7CglsYXllci5NYXRlcmlhbENvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMSwgMSwgTWF0aEV4LlNpbihUaW1lLkN1cnJlbnQpICogMC41ICsgMC41KTsKfQo=")]
public class DynamicClass87CB81277738E3204CECEEF6374F3AD27C4171EA1B50DAD28C3F381575BB5F23
{
    public NeoAxis.CSharpScript Owner;
    public void PaintLayer_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var layer = (PaintLayer)sender;
        layer.MaterialColor = new ColorValue(1, 1, 1, MathEx.Sin(Time.Current) * 0.5 + 0.5);
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgUGFpbnRMYXllcl9VcGRhdGVFdmVudChOZW9BeGlzLkNvbXBvbmVudCBzZW5kZXIsIGZsb2F0IGRlbHRhKQp7Cgl2YXIgbGF5ZXIgPSAoUGFpbnRMYXllcilzZW5kZXI7CglsYXllci5NYXRlcmlhbENvbG9yID0gbmV3IENvbG9yVmFsdWUoMSwgMSwgMSwgTWF0aEV4LkNvcyhUaW1lLkN1cnJlbnQpICogMC41ICsgMC41KTsKfQo=")]
public class DynamicClass0CEA82422398667C47366B923DF74C9208964575BB36AE3031D0BF5B96FCC8FE
{
    public NeoAxis.CSharpScript Owner;
    public void PaintLayer_UpdateEvent(NeoAxis.Component sender, float delta)
    {
        var layer = (PaintLayer)sender;
        layer.MaterialColor = new ColorValue(1, 1, 1, MathEx.Cos(Time.Current) * 0.5 + 0.5);
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

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgX1VwZGF0ZUV2ZW50KE5lb0F4aXMuQ29tcG9uZW50IHNlbmRlciwgZmxvYXQgZGVsdGEpCnsKCXRyeSAvLyEhISFyZW1vdmUKCXsKCQl2YXIgYWxsID0gVGltZS5DdXJyZW50ICUgNCA+IDIuMDsKCQkKCQlmb3JlYWNoKHZhciBzZW5zb3IgaW4gc2VuZGVyLkdldENvbXBvbmVudHM8U2Vuc29yPigpKQoJCXsKCQkJc2Vuc29yLk1vZGUgPSBhbGwgPyBTZW5zb3IuTW9kZUVudW0uQWxsT2JqZWN0cyA6IFNlbnNvci5Nb2RlRW51bS5PbmVDbG9zZXN0T2JqZWN0OwoJCX0KCX0KCWNhdGNoKEV4Y2VwdGlvbiBlKQoJewoJCUxvZy5XYXJuaW5nKGUuTWVzc2FnZSk7Cgl9CQp9Cg==")]
public class DynamicClass64FEB65296D56DF1A9166589F0262513D23B9252491F8F623346745A66306414
{
    public NeoAxis.CSharpScript Owner;
    public void _UpdateEvent(NeoAxis.Component sender, float delta)
    {
        try //!!!!remove
        {
            var all = Time.Current % 4 > 2.0;
            foreach (var sensor in sender.GetComponents<Sensor>())
            {
                sensor.Mode = all ? Sensor.ModeEnum.AllObjects : Sensor.ModeEnum.OneClosestObject;
            }
        }
        catch (Exception e)
        {
            Log.Warning(e.Message);
        }
    }
}

[CSharpScriptGeneratedAttribute("cHVibGljIHZvaWQgQ29uc3RyYWludF9TaW11bGF0aW9uU3RlcChOZW9BeGlzLkNvbXBvbmVudCBvYmopCnsKCS8vZ2V0IG9iamVjdHMKCXZhciBjb25zdHJhaW50ID0gKENvbnN0cmFpbnRfU2l4RE9GKW9iajsKCXZhciBzY2VuZSA9IGNvbnN0cmFpbnQuUGFyZW50U2NlbmU7Cgl2YXIgc3BoZXJlID0gc2NlbmUuR2V0Q29tcG9uZW50KCJTcGhlcmUiKSBhcyBNZXNoSW5TcGFjZTsKCgkvL3VwZGF0ZSBtb3RvcgoJdmFyIGxvb2tUbyA9IHNwaGVyZS5UcmFuc2Zvcm1WLlBvc2l0aW9uOwoJdmFyIGxvb2tGcm9tID0gY29uc3RyYWludC5UcmFuc2Zvcm1WLlBvc2l0aW9uOwoJdmFyIGRpZmYgPSBsb29rVG8gLSBsb29rRnJvbTsKCWNvbnN0cmFpbnQuQW5ndWxhclpBeGlzTW90b3JUYXJnZXQgPSBNYXRoRXguUmFkaWFuVG9EZWdyZWUoTWF0aEV4LkF0YW4yKGRpZmYuWSwgZGlmZi5YKSk7Cn0K")]
public class DynamicClassD77247E301AC6FDD9339B8A807DBD90D022CBC44A3DD75A24F9F3D50A3B31F21
{
    public NeoAxis.CSharpScript Owner;
    public void Constraint_SimulationStep(NeoAxis.Component obj)
    {
        //get objects
        var constraint = (Constraint_SixDOF)obj;
        var scene = constraint.ParentScene;
        var sphere = scene.GetComponent("Sphere") as MeshInSpace;
        //update motor
        var lookTo = sphere.TransformV.Position;
        var lookFrom = constraint.TransformV.Position;
        var diff = lookTo - lookFrom;
        constraint.AngularZAxisMotorTarget = MathEx.RadianToDegree(MathEx.Atan2(diff.Y, diff.X));
    }
}
}
#endif