using System.Reflection;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Artistic", "Blend Color", "Blend Color")]
    class BlendColorNode : CodeFunctionNode
    {
        public BlendColorNode()
        {
            name = "Blend Color";
        }

        string GetCurrentBlendName()
        {
            return System.Enum.GetName(typeof(BlendMode), m_BlendMode);
        }

        [SerializeField]
        BlendMode m_BlendMode = BlendMode.Overlay;

        [EnumControl("Mode")]
        public BlendMode blendMode
        {
            get { return m_BlendMode; }
            set
            {
                if (m_BlendMode == value)
                    return;

                m_BlendMode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        protected override MethodInfo GetFunctionToConvert()
        {
            return GetType().GetMethod(string.Format("Unity_Blend_{0}", GetCurrentBlendName()),
                BindingFlags.Static | BindingFlags.NonPublic);
        }

        static string Unity_Blend_Burn(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb =  1.0 - (1.0 - Blend.rgb)/(Base.rgb + 0.000000000001);
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
	Out.a = Base.a;
}";
        }

        static string Unity_Blend_Darken(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = min(Blend.rgb, Base.rgb);
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
	Out.a = Base.a;
}";
        }

        static string Unity_Blend_Difference(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = abs(Blend.rgb - Base.rgb);
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_Dodge(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = Base.rgb / (1.0 - clamp(Blend.rgb, 0.000001, 0.999999));
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_Divide(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = Base.rgb / (Blend.rgb + 0.000000000001);
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_Exclusion(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default; 
            return
                @"
{
    Out.rgb = Blend.rgb + Base.rgb - (2.0 * Blend.rgb * Base.rgb);
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_HardLight(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    $precision{slot2dimension} result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    $precision{slot2dimension} result2 = 2.0 * Base * Blend;
    $precision{slot2dimension} zeroOrOne = step(Blend, 0.5);
    Out.rgb = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_HardMix(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = step(1 - Base.rgb, Blend.rgb);
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_Lighten(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = max(Blend.rgb, Base.rgb);
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_LinearBurn(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = Base.rgb + Blend.rgb - 1.0;
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_LinearDodge(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = Base.rgb + Blend.rgb;
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_LinearLight(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = Blend.rgb < 0.5 ? max(Base.rgb + (2 * Blend.rgb) - 1, 0) : min(Base.rgb + 2 * (Blend.rgb - 0.5), 1);
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_LinearLightAddSub(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = Blend.rgb + 2.0 * Base.rgb - 1.0;
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_Multiply(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = Base.rgb * Blend.rgb;
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_Negation(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = 1.0 - abs(1.0 - Blend.rgb - Base.rgb);
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_Screen(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = 1.0 - (1.0 - Blend.rgb) * (1.0 - Base.rgb);
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}";
        }

        static string Unity_Blend_Overlay(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    $precision{slot2dimension} result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    $precision{slot2dimension} result2 = 2.0 * Base * Blend;
    $precision{slot2dimension} zeroOrOne = step(Base, 0.5);
    Out.rgb = result2.rgb * zeroOrOne.rgb + (1 - zeroOrOne.rgb) * result1.rgb;
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}
";
        }

        static string Unity_Blend_PinLight(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    $precision{slot2dimension} check = step (0.5, Blend);
    $precision{slot2dimension} result1 = check * max(2.0 * (Base - 0.5), Blend);
    Out.rgb = result1.rgb + (1.0 - check.rgb) * min(2.0 * Base.rgb, Blend.rgb);
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}
";
        }

        static string Unity_Blend_SoftLight(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    $precision{slot2dimension} result1 = 2.0 * Base * Blend + Base * Base * (1.0 - 2.0 * Blend);
    $precision{slot2dimension} result2 = sqrt(Base) * (2.0 * Blend - 1.0) + 2.0 * Base * (1.0 - Blend);
    $precision{slot2dimension} zeroOrOne = step(0.5, Blend);
    Out.rgb = result2.rgb * zeroOrOne.rgb + (1 - zeroOrOne.rgb) * result1.rgb;
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}
";
        }

        static string Unity_Blend_VividLight(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Base.rgb = clamp(Base.rgb, 0.000001, 0.999999);
    $precision{slot2dimension} result1 = 1.0 - (1.0 - Blend) / (2.0 * Base);
    $precision{slot2dimension} result2 = Blend / (2.0 * (1.0 - Base));
    $precision{slot2dimension} zeroOrOne = step(0.5, Base);
    Out.rgb = result2.rgb * zeroOrOne.rgb + (1 - zeroOrOne.rgb) * result1.rgb;
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}
";
        }

        static string Unity_Blend_Subtract(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = Base.rgb - Blend.rgb;
    Out.rgb = lerp(Base.rgb, Out.rgb, Opacity);
    Out.a = Base.a;
}
";
        }

        static string Unity_Blend_Overwrite(
            [Slot(0, Binding.None)] Vector4 Base,
            [Slot(1, Binding.None)] Vector4 Blend,
            [Slot(3, Binding.None, 1, 1, 1, 1)] Vector1 Opacity,
            [Slot(2, Binding.None)] out Vector4 Out)
        {
            Out = default;
            return
                @"
{
    Out.rgb = lerp(Base.rgb, Blend.rgb, Opacity);
    Out.a = Base.a;
}";
        }
    }
}
