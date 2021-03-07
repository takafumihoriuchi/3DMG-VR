Shader "Custom/OrderedDithering Outline" {
	Properties{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MatrixWidth("Dither Matrix Width/Height", int) = 4
		_MatrixTex("Dither Matrix", 2D) = "black" {}
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.001, 0.03)) = .005
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		UsePass "Toon/Basic Outline/OUTLINE"
		UsePass "Custom/OrderedDithering/FORWARD"
	}
	
	FallBack "Custom/OrderedDithering"
}