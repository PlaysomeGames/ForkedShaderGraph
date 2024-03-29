using System.Reflection;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Title("Procedural", "Noise", "Tiling Gradient Noise")]
    class TilingGradientNoiseNode : CodeFunctionNode
    {
        public TilingGradientNoiseNode()
        {
            name = "Tiling Gradient Noise";
        }


        protected override MethodInfo GetFunctionToConvert()
        {
            return GetType().GetMethod("Unity_TilingGradientNoise", BindingFlags.Static | BindingFlags.NonPublic);
        }

        static string Unity_TilingGradientNoise(
            [Slot(0, Binding.MeshUV0)] Vector2 UV,
            [Slot(1, Binding.None, 10, 10, 10, 10)] Vector1 Scale,
			[Slot(2, Binding.None, 8f, 8f, 8f, 8f)] Vector2 Tiling,
            [Slot(3, Binding.None)] out Vector1 Out)
        {
            return
                @"
{ 
    $precision2 p = UV * Scale;
    $precision2 ip = floor(p);
    $precision2 fp = frac(p);
	$precision2 tiling = Tiling;
    $precision d00 = dot(Unity_TilingGradientNoise_Dir_$precision(ip, tiling), fp);
    $precision d01 = dot(Unity_TilingGradientNoise_Dir_$precision(ip + $precision2(0, 1), tiling), fp - $precision2(0, 1));
    $precision d10 = dot(Unity_TilingGradientNoise_Dir_$precision(ip + $precision2(1, 0), tiling), fp - $precision2(1, 0));
    $precision d11 = dot(Unity_TilingGradientNoise_Dir_$precision(ip + $precision2(1, 1), tiling), fp - $precision2(1, 1));
    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
}
";
        }

        public override void GenerateNodeFunction(FunctionRegistry registry, GenerationMode generationMode)
        {
            registry.ProvideFunction($"Unity_TilingGradientNoise_Dir_{concretePrecision.ToShaderString()}", s => s.Append(@"
$precision2 Unity_TilingGradientNoise_Dir_$precision($precision2 p, $precision2 tiling)
{
    // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
	p.xy = p.xy % tiling.xy;
    p = p % 289;
    $precision x = (34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize($precision2(x - floor(x + 0.5), abs(x) - 0.5));
}
"));

            base.GenerateNodeFunction(registry, generationMode);
        }
    }
}
